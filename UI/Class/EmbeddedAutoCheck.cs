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
        public EM_RES AutoCheckDistance(LightBox lb, int idx, bool ifZoomDown)
        {
            var guard = ComPortGuard.Get(PT_SET.COM_1);
            guard.Wait();
            try
            {
                using (SerialCommander comm = new SerialCommander(PT_SET.COM_1))
                {
                    int temp = 0;
                   
                    comm.SetLaserStatus(true);
                    Thread.Sleep(1000);
                    PosDef pos = lb.ListPos.Find(x => x.ID == idx);
                    double distance = -1;
                ReTest:
                    distance = comm.ReadDistanceWithWait(30000);
              
                    Thread.Sleep(1000);
        
                    distance = comm.ReadDistanceWithWait(60000);
                    if (Math.Abs(distance - pos.ActualDistanceParam + pos.Comp + pos.TeleComp) > pos.DistanceThreshold && temp < 2)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的距离超出范围,重新测试");
                        temp++;
                        goto ReTest;
                    }
                    else if (Math.Abs(distance - pos.ActualDistanceParam + pos.Comp + pos.TeleComp) > pos.DistanceThreshold && temp > 2||temp==2)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的距离超出范围,重新测试超过2次");
                        return EM_RES.ERR;
                    }
                    else
                    {
                        double distanceToSave = distance > 0 ? distance : -1;
                        SaveToTxt("距离参数", comm, pos.Name, distanceToSave);
                        int? mesId = GetDistanceMesIdFromPosName(pos.Name);
                        if (mesId.HasValue)
                        {
                            Msg.secsManager.Send(new BaseInfo() { Id = mesId.Value, Value = $"{distanceToSave}" }, 1);
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
        public EM_RES ReadAndAddAllLightData3COMs(LightBox lb,int idx)
        {
            EM_RES res=EM_RES.OK;
            try
            {
                PosDef pos = lb.ListPos.Find(x => x.ID == idx);
                
                foreach (string com in new[] { PT_SET.COM_1, PT_SET.COM_2, PT_SET.COM_3 })
                {
                    if (!string.IsNullOrEmpty(com))
                    {
                        int temp = 0;
                        switch (com)
                        {
                            case "COM8":
                                temp = 8;
                                break;
                            case "COM9":
                                temp = 9;
                                break;
                            case "COM10":
                                temp = 10;
                                break;
                        }
                        if (lb.name != COM.OTPLightBox.name)
                        {

              
                           
                                using (SerialCommander comm = new SerialCommander("COM" + (temp + 3).ToString()))
                                {
                                    Thread.Sleep(5000);
                                    var (lux, cct) = comm.ReadLuxCctWithWait(20000);
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{temp + 3}读出参数lux{lux},cct{cct}");
                                    SaveToTxt("光源色温点检参数", temp + 3, pos.Name, lux, cct);
                                if (pos.Name.Contains("AFC"))
                                {
                                    if ((temp + 3).ToString() == "COM11")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 251, Value = $"{lux},{cct}" }, 1);
                                    }
                                    else if ((temp + 3).ToString() == "COM12")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 252, Value = $"{lux},{cct}" }, 1);
                                    }
                                    else if ((temp + 3).ToString() == "COM13")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 253, Value = $"{lux},{cct}" }, 1);
                                    }
                                }
                                else if (pos.Name.Contains("DCC"))
                                {
                                    if ((temp + 3).ToString() == "COM11")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 257, Value = $"{lux},{cct}" }, 1);
                                    }
                                    else if ((temp + 3).ToString() == "COM12")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 258, Value = $"{lux},{cct}" }, 1);
                                    }
                                    else if ((temp + 3).ToString() == "COM13")
                                    {
                                        Msg.secsManager.Send(new BaseInfo() { Id = 253, Value = $"{lux},{cct}" }, 1);
                                    }
                                }
                                    comm.Close();
                                    LuxWith9point.Add(lux);
                                    if (Math.Abs(lux - pos.ActualLuxParam) > pos.DistanceThreshold)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的光源超出{pos.ActualLuxParam}");
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
                                        return EM_RES.OK;
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
                                            int luxpercent = CalculateLuxResult();
                                            if (luxpercent < PT_SET.afc_cct_uniformity_precent)
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.afc_cct_uniformity_precent}");
                                                res = EM_RES.ERR;
                                            }
                                        }
                                        if (lb.name == COM.RightLightBox.name)
                                        {
                                            int luxpercent = CalculateLuxResult();
                                            if (luxpercent < PT_SET.dcc_cct_uniformity_precent)
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_cct_uniformity_precent}");
                                                res = EM_RES.ERR;
                                            }
                                        }
                                        if (lb.name == COM.OTPLightBox.name)
                                        {
                                            int luxpercent = CalculateLuxResult();
                                            if (luxpercent < PT_SET.otp_cct_uniformity_precent)
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_cct_uniformity_precent}");
                                                res = EM_RES.ERR;
                                            }
                                        }
                                        return res;
                                    }


                                }
                            
             
                        }
                        else
                        {
                            using (SerialCommander comm = new SerialCommander(com))
                            {
                                Thread.Sleep(5000);
                                var (lux, cct) = comm.ReadLuxCctWithWait(20000);
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{com}读出参数lux{lux},cct{cct}");
                                if(com == "COM8")
                                {
                                    Msg.secsManager.Send(new BaseInfo() { Id = 254, Value = $"{lux},{cct}" }, 1);
                                }
                                else if (com == "COM9")
                                {
                                    Msg.secsManager.Send(new BaseInfo() { Id = 255, Value = $"{lux},{cct}" }, 1);
                                }
                                else if(com == "COM10")
                                {
                                    Msg.secsManager.Send(new BaseInfo() { Id = 256, Value = $"{lux},{cct}" }, 1);
                                }

                                SaveToTxt("光源色温点检参数", com, pos.Name, lux, cct);
                                comm.Close();
                                LuxWith9point.Add(lux);
                                if (Math.Abs(lux - pos.ActualLuxParam) > pos.DistanceThreshold)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的光源超出{pos.ActualLuxParam}");
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
                                    return EM_RES.OK;
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
                                        int luxpercent = CalculateLuxResult();
                                        if (luxpercent < PT_SET.afc_cct_uniformity_precent)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.afc_cct_uniformity_precent}");
                                            res = EM_RES.ERR;
                                        }
                                    }
                                    if (lb.name == COM.RightLightBox.name)
                                    {
                                        int luxpercent = CalculateLuxResult();
                                        if (luxpercent < PT_SET.dcc_cct_uniformity_precent)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_cct_uniformity_precent}");
                                            res = EM_RES.ERR;
                                        }
                                    }
                                    if (lb.name == COM.OTPLightBox.name)
                                    {
                                        int luxpercent = CalculateLuxResult();
                                        if (luxpercent < PT_SET.otp_cct_uniformity_precent)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{pos.Name}位置的均匀度小于{PT_SET.dcc_cct_uniformity_precent}");
                                            res = EM_RES.ERR;
                                        }
                                    }
                                    return res;
                                }


                            }
                        }

                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"当前COM口值: '{com}'");
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{ex.Message}");
                return EM_RES.ERR;
            }
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


            return (int)Math.Round(result);
        }
        public double CalculateCctResult()
        {
            // 检查列表是否有足够的数据
            if (LuxWith9point.Count < 9)
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
        public void RunFullLightScan3x3(ref bool bquit,AXIS axis1, AXIS axis2,LightBox lb, int idx,ST_XYZ distance1, ST_XYZ distance2, ST_XYZ distance3)
        {
            EM_RES res=EM_RES.OK;
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
                    PosDef.X1 = distance1.x;
                    PosDef.Z2 = distance1.z;
       
                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第一次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
                //res = axis1.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //res = axis2.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
                if (res == EM_RES.OK)
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    PosDef.X1 = distance2.x;
                    PosDef.Z2 = distance2.z;
  
                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第二次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
                //res = axis1.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis1.disc}定位到0");
                //res = axis2.MoveTo(ref bquit, 0);
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{axis2.disc}定位到0");
                if (res == EM_RES.OK)
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    PosDef.X1 = distance3.x;
                    PosDef.Z2 = distance3.z;

                    res = lb.MoveTo(ref bquit, PosDef, 0);
                }
                Thread.Sleep(2000);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第三次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
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
                axis2.MoveTo(ref bquit, distance1.z);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第一次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
                axis2.MoveTo(ref bquit, distance2.z);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第二次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
                axis2.MoveTo(ref bquit, distance3.z);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "第三次读光源色温参数");
                ReadAndAddAllLightData3COMs(lb, idx);
            }

        }


    }
}
