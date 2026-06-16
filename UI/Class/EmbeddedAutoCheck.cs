using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MotionCtrl;
using static UI.LightBox;
using System.Windows.Forms;
using static UI.WS;
using System.Security.Cryptography.X509Certificates;
using ControlzEx.Standard;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using static SerialCommander;
using Win32Lib;

namespace UI.Class
{
    public class AutoInspectionParameter
    {
        // 距离参数（单位可以根据实际需要定义）
        private SerialCommander _serialCommander;

        public double FarDistanceBeforeZoom { get; set; }   // 增距镜未下降前远焦
        public double FarDistanceAfterZoom { get; set; }    // 增距镜下降后远焦
        public double MiddleDistance { get; set; }          // 中距
        public double NearDistance { get; set; }            // 近焦
        public double Distance {  get; set; } //距离

        // 光学参数
        public double CCT { get; set; }        // 色温
        public double LUX { get; set; }            // 照度
        public GyroscopeData gyroscopeData { get; set; } //陀螺仪数值
        public JitterThreshold jitterThreshold { get; set; } //陀螺仪上下限
        bool ifZoomDown {  get; set; }//增距镜是否下降
        //每天测量次数
        public int count { get; set; } //次数
        /// <summary>
        /// 9个位置的Lux
        /// </summary>
        public List<double> LuxWith9point { get; private set; } = new List<double>();
        /// <summary>
        /// 
        /// </summary>
        public List<double> CctWith9point { get; private set; } = new List<double>();
        /// <summary>
        /// 是否到点检时间
        /// </summary>
        public static bool ifcheckontime;
        /// <summary>
        /// 到点检时间且第一次从sta==1开始
        /// </summary>
        public static bool ifcheckontime_enabled;
        
        public static int _elapsedMilliseconds = 0;
        /// <summary>
        /// 是否完成单次点检
        /// </summary>
        public static bool ifcheck;

        
        public static void SaveGyroscopeData(
    string comName,
    GyroscopeData data)
        {
            string baseDir = @"D:\Gyroscope";
            string dayDir = Path.Combine(baseDir, DateTime.Now.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(dayDir))
                Directory.CreateDirectory(dayDir);

            string filePath = Path.Combine(dayDir, "GyroscopeLog.csv");
            bool fileExists = File.Exists(filePath);

            using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                if (!fileExists)
                {
                    sw.WriteLine(
                        "Time,COM,GyroX,GyroY,GyroZ,AccelX,AccelY,AccelZ,AccelVector,IsJittering");
                }

                sw.WriteLine(string.Format(
                    "{0},{1},{2:F6},{3:F6},{4:F6},{5:F6},{6:F6},{7:F6},{8:F6},{9}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    comName,
                    data.GyroX,
                    data.GyroY,
                    data.GyroZ,
                    data.AccelX,
                    data.AccelY,
                    data.AccelZ,
                    data.AccelVector,
                    data.IsJittering
                ));
            }
        }

        // 封装单次读取逻辑
        private bool ReadAndAddLightData(SerialCommander comm, ref List<double> luxList, ref List<double> cctList)
        {
            var waitHandle = new System.Threading.ManualResetEvent(false);
            double receivedLux = -1;
            double receivedCct = -1;

            Action<double> onLuxUpdated = (lux) =>
            {
                if (receivedLux == -1) // 只接收第一次数据
                {
                    receivedLux = lux;
                    waitHandle.Set();
                }
            };

            Action<double> onCctUpdated = (cct) =>
            {
                if (receivedCct == -1) // 只接收第一次数据
                {
                    receivedCct = cct;
                    waitHandle.Set();
                }
            };

            try
            {
                comm.LuxUpdated += onLuxUpdated;
                comm.CctUpdated += onCctUpdated;
                comm.ReadLightCalibration();

                bool isReceived = waitHandle.WaitOne(5000);
                if (!isReceived || receivedLux == -1 || receivedCct == -1)
                {
                    
                    return false;
                }

                // 线程安全地添加数据
                lock (luxList) luxList.Add(receivedLux);
                lock (cctList) cctList.Add(receivedCct);
                return true;
            }
            finally
            {
                // 确保解绑，避免内存泄漏或事件干扰
                comm.LuxUpdated -= onLuxUpdated;
                comm.CctUpdated -= onCctUpdated;
            }
        }
        public static void SaveToTxt(params object[] parameters)
        {
            try
            {
                string filePath = string.Format("{0}\\product\\{1}\\AutoCheckData.ini", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name);
                // 确保目录存在
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 构建要保存的内容
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string content = $"[{timestamp}] 保存的参数: " +
                               string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"));

                // 追加到文件（使用AppendAllText自动处理流的打开和关闭）
                File.AppendAllText(filePath, content + Environment.NewLine);

                // 可选：输出保存成功信息
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"成功保存到 {filePath}");
            }
            catch (Exception ex)
            {
                // 处理可能的异常
                Console.WriteLine($"保存失败: {ex.Message}");
                throw; // 可以根据需要决定是否向上抛出异常
            }
        }

        private static int? GetDistanceMesIdFromPosName(string posName)
        {
            if (string.IsNullOrEmpty(posName))
            {
                return null;
            }

            var map = new Dictionary<string, int>(StringComparer.Ordinal)
            {
                { "AFC近焦1", 260 },
                { "AFC近焦2", 261 },
                { "AFC近焦3", 262 },
                { "AFC中距1左", 263 },
                { "AFC中距1右", 264 },
                { "AFC中距2左", 265 },
                { "AFC中距2右", 266 },
                { "AFC中距3左", 267 },
                { "AFC中距3右", 268 },
                { "AFC远焦1左", 269 },
                { "AFC远焦1右", 270 },
                { "AFC远焦2左", 271 },
                { "AFC远焦2右", 272 },
                { "AFC远焦3左", 273 },
                { "AFC远焦3右", 274 },
                { "DCC近焦1", 275 },
                { "DCC近焦2", 276 },
                { "DCC近焦3", 277 },
                { "DCC中距1左", 278 },
                { "DCC中距1右", 279 },
                { "DCC中距2左", 280 },
                { "DCC中距2右", 281 },
                { "DCC中距3左", 282 },
                { "DCC中距3右", 283 },
                { "DCC远焦1左", 284 },
                { "DCC远焦1右", 285 },
                { "DCC远焦2左", 286 },
                { "DCC远焦2右", 287 },
                { "DCC远焦3左", 288 },
                { "DCC远焦3右", 289 },
                { "AFC色温CH1", 290 },
                { "AFC色温CH2", 309 },
                { "AFC色温CH3", 297 },
                { "OTP色温CH1", 347 },
                { "OTP色温CH2", 366 },
                { "OTP色温CH3", 385 },
                 { "DCC色温CH1", 404 },
                { "DCC色温CH2", 423 },
                { "DCC色温CH3", 442 },
            };

            foreach (var pair in map)
            {
                if (posName.Contains(pair.Key))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        private enum LightMesValueType
        {
            Cct,
            Lux
        }

        private static bool TryGetLightMesBaseId(string posName, LightMesValueType valueType, out int baseId)
        {
            baseId = 0;
            if (string.IsNullOrEmpty(posName))
            {
                return false;
            }

            int panelOffset;
            if (posName.Contains("AFC"))
            {
                panelOffset = 0;
            }
            else if (posName.Contains("OTP"))
            {
                panelOffset = 57;
            }
            else if (posName.Contains("DCC"))
            {
                panelOffset = 114;
            }
            else
            {
                return false;
            }

            int channelIndex;
            if (posName.Contains("CH1"))
            {
                channelIndex = 0;
            }
            else if (posName.Contains("CH2"))
            {
                channelIndex = 1;
            }
            else if (posName.Contains("CH3"))
            {
                channelIndex = 2;
            }
            else
            {
                return false;
            }

            int typeBase = valueType == LightMesValueType.Cct ? 291 : 300;
            baseId = typeBase + panelOffset + (channelIndex * 19);
            return true;
        }

        private static bool TrySendLightMesData(string posName, int pointIndex, double lux, double cct)
        {
            if (pointIndex < 1 || pointIndex > 9)
            {
                return false;
            }

            if (!TryGetLightMesBaseId(posName, LightMesValueType.Lux, out int luxBaseId) ||
                !TryGetLightMesBaseId(posName, LightMesValueType.Cct, out int cctBaseId))
            {
                return false;
            }

            int luxId = luxBaseId + pointIndex - 1;
            int cctId = cctBaseId + pointIndex - 1;
            Msg.secsManager.Send(new BaseInfo() { Id = luxId, Value = $"{lux}" });
            Msg.secsManager.Send(new BaseInfo() { Id = cctId, Value = $"{cct}" });
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"Mes成功发送点位{pointIndex}的照度svid为{luxId}、色温svid为{cctId}，value分别为{lux},{cct}");
            return true;
        }

        public static EM_RES CheckGyroscopeJitter(SerialCommander comm,
    string comName)
        {
            try
            {

                    GyroscopeData gyro = comm.ReadGyroscopeWithWait(3000);
                    JitterThreshold threshold = comm.ReadJitterThresholdWithWait(3000);
                    SaveGyroscopeData(comName, gyro);
                    comm.Close();
               
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "陀螺仪参数保存成功");
                return EM_RES.OK;
            }
            catch (TimeoutException ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message);
                return EM_RES.ERR;
            }
        }

        public EM_RES AutoCheckPass(LightBox lb,int sta)
        {
            EM_RES res=EM_RES.OK;
            EM_TEST_STA TestStatus = EM_TEST_STA.UNTEST;
        ReTest:
            res=AutoCheckDistance(lb, sta, ifZoomDown: false);
            if (res != EM_RES.OK)
            {
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, "自动点检距离失败!", 20, true);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = "重新测量";
                warn.abort_txt = "停止";
                warn.title = "提示:自动点检距离失败!";
                warn.msg = string.Format("提示:{0}位置{1}自动点检距离失败!", lb.name, sta);
                warn.lb_msg = string.Format("{0}位置{1}自动点检距离失败!\r\n点击重新测量，则重新进行该位置距离检测！！！\r\n点击停止，则会停止测试\r\n", lb.name, sta);
                DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                if (resulte == DialogResult.Yes)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}位置{1}自动点检距离失败,选择了继续", lb.name, sta));
                    goto ReTest;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}位置{1}自动点检距离失败,选择了停止", lb.name, sta));
                    TestStatus = EM_TEST_STA.ERROR;
                    return EM_RES.ERR;
                }
                
            }
            return EM_RES.OK;
        }

        public EM_RES ReadStandardBoardInfo(LightBox lb)
        {
            if (!PT_SET.StandardBoardReadEn)
            {
                return EM_RES.OK;
            }

            string boardName;
            string portName = GetStandardBoardCom(lb, out boardName);
            if (string.IsNullOrWhiteSpace(portName))
            {
                return StandardBoardReadFail(string.Format("{0}标板信息读取已开启,但未设置COM口", boardName));
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("标板读取快照: 面别={0}, COM={1}, 结果=开始读取", boardName, portName));

            string error;
            string boardInfo;
            using (StandardBoardReader reader = new StandardBoardReader(portName))
            {
                if (!reader.Open(out error))
                {
                    return StandardBoardReadFail(error);
                }

                if (!reader.ReadText(out boardInfo, out error))
                {
                    return StandardBoardReadFail(error);
                }
            }

            SaveToTxt("标板信息", boardName, portName, boardInfo);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("标板读取快照: 面别={0}, COM={1}, 结果=OK, 信息={2}", boardName, portName, boardInfo));
            return EM_RES.OK;
        }

        private string GetStandardBoardCom(LightBox lb, out string boardName)
        {
            if (lb == COM.LeftLightBox)
            {
                boardName = "AFC";
                return PT_SET.StandardBoardAfcCom;
            }

            if (lb == COM.RightLightBox)
            {
                boardName = "DCC";
                return PT_SET.StandardBoardDccCom;
            }

            boardName = lb == null ? "未知光箱" : lb.name;
            return string.Empty;
        }

        private EM_RES StandardBoardReadFail(string message)
        {
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("标板读取快照: 结果=NG, 原因={0}", message));
            VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, "自动点检读取标板信息失败!", 20, true);
            MT.ST_WARN warn = new MT.ST_WARN();
            warning fr_warn = new warning();
            warn.ok_txt = "停止";
            warn.title = "提示:读取标板信息失败!";
            warn.msg = message;
            warn.lb_msg = message + "\r\n读取标板信息失败，本次点检停止！";
            MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("读取标板信息失败:{0}", message));
            return EM_RES.ERR;
        }

        public EM_RES AutoCheckDistance(LightBox lb, int idx, bool ifZoomDown)
        {
            string comName = GetDistanceCheckCom(lb);
            if (string.IsNullOrWhiteSpace(comName))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{lb.name}自动点检未设置整合COM口");
                return EM_RES.ERR;
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"自动点检COM映射: 面别={GetLightCheckFaceName(lb)}, 项目=距离, sta={idx}, COM={comName}");
            var guard = ComPortGuard.Get(comName);
            guard.Wait();
            try
            {
                using (SerialCommander comm = new SerialCommander(comName))
                {
                    int temp = 0;
                   
                    comm.SetLaserStatus(true);
                    Thread.Sleep(1000);
                    PosDef pos = lb.ListPos.Find(x => x.ID == idx);
                    if (pos == null)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{lb.name}未找到点检位置{idx}");
                        return EM_RES.ERR;
                    }
                    double distance = -1;
                ReTest:
                    distance = comm.ReadDistanceWithWait(30000);
              
                    Thread.Sleep(1000);
        
                    distance = comm.ReadDistanceWithWait(60000);
                    double distanceDiff = Math.Abs(distance - pos.ActualDistanceParam + pos.Comp + pos.TeleComp);
                    if (distanceDiff > pos.DistanceThreshold && temp < 2)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的距离超出范围,当前值:{distance},标准值:{pos.ActualDistanceParam},偏差:{distanceDiff},阈值:{pos.DistanceThreshold},重新测试");
                        temp++;
                        goto ReTest;
                    }
                    else if (distanceDiff > pos.DistanceThreshold)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的距离超出范围,当前值:{distance},标准值:{pos.ActualDistanceParam},偏差:{distanceDiff},阈值:{pos.DistanceThreshold},重新测试超过2次");
                        return EM_RES.ERR;
                    }
                    else
                    {
                        double distanceToSave = distance > 0 ? distance : -1;
                        SaveToTxt("距离参数", comm, pos.Name, distanceToSave);
                        int? mesId = GetDistanceMesIdFromPosName(pos.Name);
                        if (mesId.HasValue)
                        {
                            Msg.secsManager.Send(new BaseInfo() { Id = mesId.Value, Value = $"{distanceToSave + pos.Comp + pos.TeleComp}" }, 0);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}成功发送svid为{mesId.Value}，value值为{distanceToSave}的mes信息");
                            temp++;
                        }


                        comm.SetLaserStatus(false);
                        //SaveToTxt("补偿后距离参数", pos.Name, (distance - pos.ActualDistanceParam + pos.Comp + pos.TeleComp));
                        comm.Close();
                    }
                    //if (pos != null)
                    //{
                    //    if (Math.Abs(distance - pos.ActualDistanceParam+pos.Comp+pos.TeleComp) > pos.DistanceThreshold)
                    //    {
                    //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的距离超出范围");
                    //        return EM_RES.ERR;
                    //    }
                    //    return EM_RES.OK;

                    //}
                    return EM_RES.OK;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is TimeoutException)
            {
              
                MessageBox.Show($"{ex.Message}");
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{ex.Message}");
                return EM_RES.ERR;
            }
            finally
            {
                guard.Release();
            }
            
        }
        /// <summary>
        /// 一次Z轴移动，三串口依次采样lux+cct，添加到结果列表
        /// </summary>
        public EM_RES ReadAndAddAllLightData3COMs(LightBox lb, int idx, int moveIndex)
        {
            EM_RES res=EM_RES.OK;
            try
            {
                PosDef pos = lb.ListPos.Find(x => x.ID == idx);
                if (pos == null)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{lb.name}未找到点检位置{idx}");
                    return EM_RES.ERR;
                }

                string com = GetLightCheckCom(lb);
                if (string.IsNullOrWhiteSpace(com))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{lb.name}自动点检未设置光源色温COM口");
                    return EM_RES.ERR;
                }

                string faceName = GetLightCheckFaceName(lb);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"自动点检COM映射: 面别={faceName}, 项目=光源色温, sta={idx}, moveIndex={moveIndex}, COM={com}");
                var guard = ComPortGuard.Get(com);
                guard.Wait();
                try
                {
                    using (SerialCommander comm = new SerialCommander(com))
                    {
                        Thread.Sleep(5000);
                        List<SerialCommander.LightChannelData> channels = comm.ReadLightChannelsWithWait(20000);
                        if (channels == null || channels.Count < 3)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{com}光源色温返回通道数不足,实际:{(channels == null ? 0 : channels.Count)}");
                            return EM_RES.ERR;
                        }

                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"自动点检三通道读取完成: 面别={faceName}, sta={idx}, moveIndex={moveIndex}, COM={com}, 通道数={channels.Count}");
                        foreach (SerialCommander.LightChannelData channel in channels.OrderBy(x => x.Channel).Take(3))
                        {
                            int pointIndex = moveIndex * 3 + channel.Channel;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{com} CH{channel.Channel}读出参数lux{channel.Lux},cct{channel.Cct}");
                            EM_RES channelRes = AddLightPointData(lb, pos, pointIndex, $"{com}-CH{channel.Channel}", channel.Lux, channel.Cct);
                            if (channelRes != EM_RES.OK)
                            {
                                res = channelRes;
                            }
                        }

                        comm.Close();
                    }
                }
                finally
                {
                    guard.Release();
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"自动点检光源色温采样结束: 面别={faceName}, sta={idx}, moveIndex={moveIndex}, COM={com}, LuxCount={LuxWith9point.Count}, CctCount={CctWith9point.Count}, 结果={res}");
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{ex.Message}");
                return EM_RES.ERR;
            }
        }

        private string GetDistanceCheckCom(LightBox lb)
        {
            return PT_SET.COM_1;
        }

        private string GetLightCheckCom(LightBox lb)
        {
            return lb == COM.OTPLightBox ? PT_SET.COM_1 : PT_SET.COM_2;
        }

        private string GetLightCheckFaceName(LightBox lb)
        {
            if (lb == COM.OTPLightBox) return "OTP";
            if (lb == COM.LeftLightBox) return "AFC";
            if (lb == COM.RightLightBox) return "DCC";
            return lb == null ? "Unknown" : lb.name;
        }

        private EM_RES AddLightPointData(LightBox lb, PosDef pos, int pointIndex, string sourceName, double lux, double cct)
        {
            EM_RES res = EM_RES.OK;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"自动点检光源点位数据: 面别={GetLightCheckFaceName(lb)}, 位置={pos.Name}, 点位={pointIndex}, 来源={sourceName}, lux={lux}, cct={cct}");
            SaveToTxt("光源色温点检参数", sourceName, pos.Name, lux, cct);
            if (!TrySendLightMesData(pos.Name, pointIndex, lux, cct))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, $"{pos.Name}点位{pointIndex}未匹配到MES上传SVID");
            }

            LuxWith9point.Add(lux);
            double luxDiff = Math.Abs(lux - pos.ActualLuxParam);
            if (luxDiff > pos.LuxThreshold)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的照度偏离标准值,当前值:{lux},标准值:{pos.ActualLuxParam},偏差:{luxDiff},阈值:{pos.LuxThreshold}");
                res = EM_RES.ERR;
            }

            if (LuxWith9point.Count == 9)
            {
                if (lb.name == COM.LeftLightBox.name)
                {
                    int luxpercent = CalculateLuxResult();
                    if (luxpercent < PT_SET.afc_lux_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.afc_lux_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
                if (lb.name == COM.RightLightBox.name)
                {
                    int luxpercent = CalculateLuxResult();
                    if (luxpercent < PT_SET.dcc_lux_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_lux_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
                if (lb.name == COM.OTPLightBox.name)
                {
                    int luxpercent = CalculateLuxResult();
                    if (luxpercent < PT_SET.otp_lux_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.otp_lux_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
            }

            CctWith9point.Add(cct);
            if (Math.Abs(cct - pos.ActualCctParam) > pos.CctThreshold)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的光源超出{pos.ActualCctParam}");
                res = EM_RES.ERR;
            }

            if (CctWith9point.Count == 9)
            {
                if (lb.name == COM.LeftLightBox.name)
                {
                    double cctpercent = CalculateCctResult();
                    if (cctpercent < PT_SET.afc_cct_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.afc_cct_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
                if (lb.name == COM.RightLightBox.name)
                {
                    double cctpercent = CalculateCctResult();
                    if (cctpercent < PT_SET.dcc_cct_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_cct_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
                if (lb.name == COM.OTPLightBox.name)
                {
                    double cctpercent = CalculateCctResult();
                    if (cctpercent < PT_SET.otp_cct_uniformity_precent)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.otp_cct_uniformity_precent}");
                        res = EM_RES.ERR;
                    }
                }
            }

            return res;
        }
        public int CalculateLuxResult()
        {
            // 检查列表是否有足够的数据
            if (LuxWith9point.Count < 2)
            {
                throw new InvalidOperationException("列表中至少需要包含两个数值才能进行计算");
            }

            // 找出最大值和最小值
            double max = LuxWith9point.Max();
            double min = LuxWith9point.Min();

            // 避免除以零的情况
            if (max + min == 0)
            {
                return 100; // 
            }

            // 应用公式计算
            double result = 1 - (max - min) / (max + min);
            // 转换为百分比
            result *= 100;

            return (int)Math.Round(result);
        }
        public double CalculateCctResult()
        {
            // 检查列表是否有足够的数据
            if (CctWith9point.Count < 9)
            {
                throw new InvalidOperationException("列表中至少需要包含两个数值才能进行计算");
            }

            // 找出最大值和最小值
            double max = CctWith9point.Max();
            double min = CctWith9point.Min();

            // 避免除以零的情况
            if (max + min == 0)
            {
                return 100.0; 
            }

            // 应用公式计算
            double result = 1 - (max - min) / (max + min);
            // 转换为百分比
            result *= 100;

            return result;
        }
        /// <summary>
        /// 重复3次Z轴动作，完整采集9组lux+cct
        /// </summary>
        public EM_RES AutoCheckLightPass(ref bool bquit, AXIS axis1, AXIS axis2, LightBox lb, int sta, ST_XYZ distance1, ST_XYZ distance2, ST_XYZ distance3)
        {
            EM_RES res = EM_RES.OK;
        ReTest:
            res = RunFullLightScan3x3(ref bquit, axis1, axis2, lb, sta, distance1, distance2, distance3);
            if (res != EM_RES.OK)
            {
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, "自动点检光源色温失败!", 20, true);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = "重新测量";
                warn.abort_txt = "停止";
                warn.title = "提示:自动点检光源色温失败!";
                warn.msg = string.Format("提示:{0}位置{1}自动点检光源色温失败!", lb.name, sta);
                warn.lb_msg = string.Format("{0}位置{1}自动点检光源色温失败!\r\n点击重新测量，则重新进行该位置光源色温检测！！！\r\n点击停止，则会停止测试\r\n", lb.name, sta);
                DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                if (resulte == DialogResult.Yes)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}位置{1}自动点检光源色温失败,选择了继续", lb.name, sta));
                    goto ReTest;
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}位置{1}自动点检光源色温失败,选择了停止", lb.name, sta));
                return EM_RES.ERR;
            }

            return EM_RES.OK;
        }

        public EM_RES RunFullLightScan3x3(ref bool bquit,AXIS axis1, AXIS axis2,LightBox lb, int idx,ST_XYZ distance1, ST_XYZ distance2, ST_XYZ distance3)
        {
            EM_RES res=EM_RES.OK;
            LuxWith9point.Clear();
            CctWith9point.Clear();
            PosDef sourcePos = lb.ListPos.Find(x => x.ID == idx);
            if (sourcePos == null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{lb.name}未找到点检位置{idx}");
                return EM_RES.ERR;
            }
            if(axis1!=null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "AFC,DCC位置下的光源色温参数读取");
                //res = axis1.MoveTo(ref bquit, 0);
                //if (res == EM_RES.OK)
                //{
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //}
                //res = axis2.MoveTo(ref bquit, 0);
                //if (res == EM_RES.OK)
                //{
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //}
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
                if (res == EM_RES.OK)
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    PosDef.Channel = sourcePos.Channel;
                    PosDef.X1 = distance1.x;
                    PosDef.Z2 = distance1.z;

                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                if (res != EM_RES.OK) return res;
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第一次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 0);
                if (res != EM_RES.OK) return res;
                //res = axis1.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //res = axis2.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
                if (res == EM_RES.OK)
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    PosDef.Channel = sourcePos.Channel;
                    PosDef.X1 = distance2.x;
                    PosDef.Z2 = distance2.z;

                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                if (res != EM_RES.OK) return res;
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第二次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 1);
                if (res != EM_RES.OK) return res;
                //res = axis1.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //res = axis2.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
                if (res == EM_RES.OK)
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    PosDef.Channel = sourcePos.Channel;
                    PosDef.X1 = distance3.x;
                    PosDef.Z2 = distance3.z;

                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                if (res != EM_RES.OK) return res;
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第三次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 2);
                if (res != EM_RES.OK) return res;
                //res = axis1.MoveTo(ref bquit, 0);
                //if (res == EM_RES.OK)
                //{
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //}
                //res = axis2.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "OTP位置下的光源色温参数读取");
                res = axis2.MoveTo(ref bquit, distance1.z);
                if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第一次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 0);
                if (res != EM_RES.OK) return res;
                res = axis2.MoveTo(ref bquit, distance2.z);
                if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第二次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 1);
                if (res != EM_RES.OK) return res;
                res = axis2.MoveTo(ref bquit, distance3.z);
                if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第三次读光源色温参数");
                res = ReadAndAddAllLightData3COMs(lb, idx, 2);
                if (res != EM_RES.OK) return res;
            }

            return EM_RES.OK;
        }


    }
}
