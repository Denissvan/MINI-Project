using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MotionCtrl;

namespace UI
{
    public class IlluminationController
    {
        //SerialPort
        SerialCom sCom = new SerialCom();
        /// <summary>
        /// 描述
        /// </summary>
        string Description;
        /// <summary>
        /// 串口名字
        /// </summary>
        public string PortName = "COM3";
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate = 115200;
        /// <summary>
        /// 串口打开状态
        /// </summary>
        public bool bComInitOK = false;
        /// <summary>
        /// 读出的数据
        /// </summary>
        public string dData;
        /// <summary>
        /// 读出状态
        /// </summary>
        public bool bReaded = false;

        public byte[] bySendData;
        Stopwatch sw = new Stopwatch();

        public IlluminationController()
        {

        }
        public IlluminationController(string Description, string PortName, int BaudRate)
        {
            this.Description = Description;
            this.PortName = PortName;
            this.BaudRate = BaudRate;


            if (sCom.InitSerialCom(Description, PortName, BaudRate))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"{Description} 打开串口成功");
                //Logger.Info($"{Description} 打开串口成功");
                sCom.ComReceivedEvent += new EventHandler(ComReceivedDo_Com);
                bComInitOK = true;
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"{Description} 打开串口失败");
                //Logger.Info($"{Description} 打开串口失败");
                bComInitOK = false;
            }
        }

        private void ComReceivedDo_Com(object sender, EventArgs e)
        {
            //byte[] data = sCom.bytReceData;
            //byte[] newdata = new byte[data.Length - 3];

            //string str = Encoding.UTF8.GetString(data);
            //string sData = "";


            ////提取数据
            //int j = 0;
            //for (int i = 0; i < data.Length; i++)
            //{
            //    if (data[i] != 0x02 && data[i] != 0x0D && data[i] != 0x0A)
            //    {
            //        newdata[j] = data[i];
            //        j++;
            //    }
            //}
            //sData = System.Text.Encoding.ASCII.GetString(newdata);

            //dData = sData;

            //bReaded = true;
        }

        public bool ReadData(out string data, int timeout = 200)
        {

            bySendData = new byte[] { 0x54 };

            data = "";
            bReaded = false;
            sCom.SendData(bySendData);

            //等待数据
            //sw.Restart();I
            int i = 0;
            while (true)
            {
                if (bReaded == true)
                    break;
                //if (sw.ElapsedMilliseconds > timeout)       //容易超时
                if (i > 20)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{Description} 读取数据,超时{timeout}ms");
                    //Logger.Error($"{Description} 读取数据,超时{timeout}ms");
                    //sw.Stop();
                    return false;
                }
                Thread.Sleep(100);
                i++;
            }
            //sw.Stop();

            //if (dData == double.MaxValue)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{Description} 读取的数据错误");
            //    //Logger.Error($"{Description} 读取的数据错误");
            //    return false;
            //}

            data = dData;
            return true;
        }

        public bool SetChannel(int chset)
        {
            byte a = 0;
            switch (chset)
            {
                case 4:
                    a = 1;
                    break;
                case 5:
                    a = 2;
                    break;
                case 6:
                    a = 4;
                    break;
                default:
                    break;
            }
            bySendData = new byte[] { 90, 8, 1, 1, 0, a, 0, 3 };
            sCom.SendData(bySendData);
            return true;
        }
        public void XSJSatrt()
        {
            if (bComInitOK == false)
            {
                if (sCom.InitSerialCom(Description, PortName, BaudRate))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"{Description} 打开串口成功");
                    //Logger.Info($"{Description} 打开串口成功");
                    sCom.ComReceivedEvent += new EventHandler(ComReceivedDo_Com);
                    bComInitOK = true;
                }
                else
                {
                    bComInitOK = false;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{Description} 打开串口失败");
                    return;
                }
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "启动轩仕佳光源");
            bySendData = new byte[] { 76, 66, 0, 1, 0, 143 };
            sCom.SendData(bySendData);
            Thread.Sleep(300);
        }
        public bool SetChannelXSJ(int chset)
        {
            byte a = 0;
            byte b = 147;
            switch (chset)
            {
                case 4:
                    a = 0;
                    b = 147;
                    break;
                case 5:
                    a = 1;
                    b = 148;
                    break;
                case 6:
                    a = 2;
                    b = 149;
                    break;
                default:
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "切换轩仕佳光源参数错误");
                    return false;
                    break;
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "切换轩仕佳光源通道为" + a);
            bySendData = new byte[] { 76, 71, a, 0, 0, b };
            sCom.SendData(bySendData);
            return true;
        }
    }
}
