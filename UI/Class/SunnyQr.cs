using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MotionCtrl;
using SunyLib.TCPHelper;

namespace UI.Class
{
    public class SunnyQr
    {
        #region 参数

        public int Id;

        /// <summary>
        /// 通信服务
        /// </summary>
        private AxTcpClient EpsonRobot = new AxTcpClient();

        /// <summary>
        /// EpsonRobot接收命令
        /// </summary>
        public string EpsonRobotResult = string.Empty;

        /// <summary>
        /// EpsonRobot接收内容
        /// </summary>
        public string EpsonRobotData = string.Empty;

        /// <summary>
        /// EpsonStatus接收命令
        /// </summary>
        public string EpsonStatusResult = string.Empty;

        /// <summary>
        /// EpsonStatus接收内容
        /// </summary>
        public string EpsonStatusData = string.Empty;

        /// <summary>
        /// 自锁
        /// </summary>
        private static readonly Object SendRobotLockObj = new object();

        private static readonly Object SendStatusLockObj = new object();

        public bool Isconnect = false;

        public bool Issuccess = false;//收到二维码标志位

        public string QrCode = "ERRORCODE";//收到二维码标志位
        #endregion

        #region 初始化

        public EM_RES Init(string ip = "")
        {
            EpsonRobot.OnReceviceByte -= new AxTcpClient.ReceviceByteEventHandler(TcpClienEpsonRobotOnReceviceByte);
            EpsonRobot.OnStateInfo -= new AxTcpClient.StateInfoEventHandler(TcpClientEpsonRobotOnStateInfo);
            EpsonRobot.OnReceviceByte += new AxTcpClient.ReceviceByteEventHandler(TcpClienEpsonRobotOnReceviceByte);
            EpsonRobot.OnStateInfo += new AxTcpClient.StateInfoEventHandler(TcpClientEpsonRobotOnStateInfo);
            EM_RES res = EpsonOpen(ip);
            return res;
        }

        #endregion

        public SunnyQr(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// 接收处理
        /// </summary>
        /// <param name="date"></param>
        private void TcpClienEpsonRobotOnReceviceByte(byte[] date)
        {
            string str = Encoding.Default.GetString(date);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "收到舜宇扫码枪"+Id+"的信息: "+ str);
            string str2 = "收到舜宇扫码枪" + Id + "的信息:" + str;
            Utility.WriteStrToCSV(str2);
            Issuccess = true;
            QrCode = str;
        }

        /// <summary>
        /// 状态检测
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="state"></param>
        private void TcpClientEpsonRobotOnStateInfo(string msg, SocketState state)
        {
            if (state == SocketState.Connected)
            {
                Isconnect = true;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, EpsonRobot.ServerIp + ":" + EpsonRobot.ServerPort + "..." + msg);
            }
            else
            {
                Isconnect = false;
                if (state == SocketState.Reconnection)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, EpsonRobot.ServerIp + ":" + EpsonRobot.ServerPort + "..." + msg);
                }
                if (state == SocketState.Connecting)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, EpsonRobot.ServerIp + ":" + EpsonRobot.ServerPort + "..." + msg);
                }
            }
        }

        public EM_RES EpsonOpen(string ip = "")
        {
            EM_RES res = EpsonOpen(EpsonRobot, ip, 51236);
            if (res != EM_RES.OK) return res;
            Thread.Sleep(1000);         
            return EM_RES.OK;
        }

        /// <summary>
        /// 连接机械手
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public EM_RES EpsonOpen(AxTcpClient tcp, string ip = "", int port=51236)
        {
            if (!tcp.IsStartTcpthreading)
            {
                try
                {
                    tcp.ReConectedCount = 0;
                    tcp.ServerIp = ip;
                    tcp.ServerPort =port;
                    tcp.StartConnection();
                }
                catch (Exception)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "连接 " + tcp.ServerIp + ": " + tcp.ServerPort + "...失败!");
                    return EM_RES.ERR;
                }
            }
            return EM_RES.OK;
        }

        public EM_RES EpsonClose()
        {
            if (EpsonRobot.IsStartTcpthreading)
            {
                try
                {
                    EpsonRobot.StopConnection();
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "关闭 " + EpsonRobot.ServerIp + ": " + EpsonRobot.ServerPort + "...！");
                }
                catch (Exception)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "断开 " + EpsonRobot.ServerIp + ": " + EpsonRobot.ServerPort + "...失败!");
                    return EM_RES.ERR;
                }
            }
            return EM_RES.OK;
        }


        public EM_RES QrAction(ref bool bquit,out string Code)
        {
            EM_RES res;
            REQR:
            for (int i = 0; i <= PT_SET.Motornum; i++)
            {
                QrCode = "ERRORCODE";
                Issuccess = false;
                res = EpsonRobotSendCmd();
                if (res != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "与舜宇扫码枪数据通信异常!"+Id);
                    Code = "ERRORCODE";
                    return EM_RES.ERR;
                }
                int n = 0;

                while (!Issuccess && !bquit)
                {
                    if (n++ > 500) break;
                    if (bquit)
                    {
                        Code = "ERRORCODE";
                        return EM_RES.QUIT;
                    }

                    Thread.Sleep(10);
                }

                if (n > 500 && PT_SET.bsunnyqralm)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "扫码枪5秒没有返回结果!");
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = MultiLanguage.TxtSelct("重新扫码", "GoOn", "Đi tiếp");
                    warn.abort_txt = MultiLanguage.TxtSelct("继续运行", "GoOn", "Đi tiếp");
                    warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop", "Ngừng lại");
                    warn.title = MultiLanguage.TxtSelct("提示:扫码异常", "Tip: Fly Err Back Place Tray Error", "Mẹo: Fly Err Back Place Tray Error");
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "飞拍失败放回异常" : "Place Tray Error", 10, true);
                    warn.msg = MultiLanguage.TxtSelct("侧边扫码异常!", "Place Tray Error", "Lỗi đặt khay");
                    warn.lb_msg = MultiLanguage.TxtSelct($"当前侧边扫码异常\r\n" +
                        "1、点击重新扫码，则重新进行扫码动作\r\n" +
                        "2、点击继续运行，则不在扫码，二维码默认ErrorCode\r\n" +
                        "3、点击停止运行，则不在扫码，设备停止\r\n");
                    DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                    if (logres == DialogResult.OK)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        goto REQR;

                    }
                    else if (logres == DialogResult.Abort)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        Code = "ERRORCODE";
                        return EM_RES.OK;

                    }
                    else if (logres == DialogResult.Cancel)
                    {
                        Code = "ERRORCODE";
                        return EM_RES.QUIT;
                    }
                }

                Code = QrCode;
                if (Code.Contains("errorcode"))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "扫码枪第"+i+"次扫码失败");
                    continue;
                }
                else
                {
                    break;
                }

            }

            Code = QrCode;
            if (Code.Contains("errorcode"))
            {
                return EM_RES.ERR;
            }
            return EM_RES.OK;

        }

        /// <summary>
        /// 发达命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="overtime"></param>
        /// <returns></returns>
        public EM_RES EpsonRobotSendCmd()
        {
            lock (SendRobotLockObj)
            {
                try
                {
                    if(!EpsonRobot.IsConnect)
                    {
                        EM_RES res = EpsonClose();
                        Thread.Sleep(500);
                        if (Id==0)
                        {
                            res = EpsonOpen(PT_SET.sunnyqrip0);
                            return res;
                        }
                        else
                        {
                            res = EpsonOpen(PT_SET.sunnyqrip1);
                            return res;
                        }
                       
                    }

                    EpsonRobotResult = string.Empty;
                    string  data = "T";
                    EpsonRobot.SendCommand(data);
                    return EM_RES.OK;
                }
                catch
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "与舜宇扫码枪数据通信异常!");
                    return EM_RES.ERR;
                }

            }
        }


    }
}
