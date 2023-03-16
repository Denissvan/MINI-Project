using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public class BarcodeScanner
    {
        //SerialPort
      public  SerialCom sCom = new SerialCom();
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

        public BarcodeScanner()
        {

        }
        public BarcodeScanner(string Description, string PortName, int BaudRate)
        {
            this.Description = Description;
            this.PortName = PortName;
            this.BaudRate = BaudRate;


            if (sCom.InitSerialCom(Description, PortName, BaudRate))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("{0} 打开串口成功", Description));
                //Logger.Info($"{Description} 打开串口成功");
                sCom.ComReceivedEvent += new EventHandler(ComReceivedDo_Com);
                bComInitOK = true;
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("{0} 打开串口失败", Description));
                //Logger.Info($"{Description} 打开串口失败");
                bComInitOK = false;
            }
        }

        public void ReInit()
        {
            if (sCom.InitSerialCom(Description, PortName, BaudRate))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("{0} 打开串口成功", Description));
                //Logger.Info($"{Description} 打开串口成功");
                sCom.ComReceivedEvent += new EventHandler(ComReceivedDo_Com);
                bComInitOK = true;
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("{0} 打开串口失败", Description));
                //Logger.Info($"{Description} 打开串口失败");
                bComInitOK = false;
            }
        }

        private void ComReceivedDo_Com(object sender, EventArgs e)
        {
            try
            {
                byte[] data = sCom.bytReceData;
                if (data.Length < 5)
                {
                    bReaded = true;
                    dData = "";
                    return;
                }
                byte[] newdata = new byte[data.Length - 2];

                string str = Encoding.UTF8.GetString(data);
                string sData = "";


                //提取数据
                for (int i = 0; i < newdata.Length; i++)
                {
                    newdata[i] = data[i];
                    //if (data[i] != 0x02 && data[i] != 0x0D && data[i] != 0x0A && data[i] != 0x20)
                    //{
                    //    newdata[j] = data[i];
                    //    j++;
                    //}
                }
                sData = System.Text.Encoding.ASCII.GetString(newdata);

                dData = sData;

                bReaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            
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
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 读取数据,超时{1}ms", Description, timeout));
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

        public bool ReadDataByString(out string data, int timeout = 200, int traycnt = 1)
        {

            var bySendDataStr = "T";

            data = "";
            bool bok = false;
            bReaded = false;
            sCom.SendData(bySendDataStr);
            int i = 0;
            int trayCnt = 0;
            while (true)
            {
                if (bReaded == true)
                    break;
                //if (sw.ElapsedMilliseconds > timeout)       //容易超时
                if (i > 20)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 读取数据,超时{1}ms", Description, timeout));
                    trayCnt++;
                    if (trayCnt <= traycnt)
                    {
                        i = 0;
                        sCom.SendData(bySendDataStr);
                        continue;
                    }
                    bok = false;
                    break;
                }
                Thread.Sleep(100);
                i++;
            }

            data = dData;
            if (data.Length != PT_SET.motorBarcodeDigits || data.Length < 5)
            {
                data = "";               
                return false;
            }else
            {
               
            }

            return true;
        }

        public bool SetChannel(int chset)
        {
            byte a = 0x00;
            byte b = 0x00;
            switch (chset)
            {
                case 4:
                    a = 0x00;
                    b = 0x93;
                    break;
                case 5:
                    a = 0x01;
                    b = 0x94;
                    break;
                case 6:
                    a = 0x02;
                    b = 0x95;
                    break;
                case 7:
                    a = 0x03;
                    b = 0x96;
                    break;
                default:
                    break;
            }
            if (b == 0x00) return false;
            bySendData = new byte[] { 0x4c, 0x47, a, 0x00, 0x00, b };
            sCom.SendData(bySendData);
            return true;
        }
    }
}
