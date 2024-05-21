using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public class SerialCom
    {
        public SerialPort ComDevice = new SerialPort();
        /// <summary>
        /// 串口名字
        /// </summary>
        public string PortName = "COM2";
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate = 38400;
        /// <summary>
        /// 文件路径
        /// </summary>
        /// <param name="IniPath"></param>
        /// <param name="ErrorMessage"></param>
        public string IniPath = "";
        /// <summary>
        /// 需要发送的数据
        /// </summary>
        public byte[] bytSendData;
        /// <summary>
        /// 需要发送的数据2
        /// </summary>
        public byte[] bytSendData2;
        /// <summary>
        /// 串口接收到的数据
        /// </summary>
        public byte[] bytReceData;
        /// <summary>
        /// 串口接收到数据事件
        /// </summary>
        public event EventHandler ComReceivedEvent;
        public object Locker = new object();
        /// <summary>
        /// 串口描述: 左点胶激光
        /// </summary>
        public string Description;

        public bool bIsReadBuffer = false;

        public SerialCom()
        {

        }


        //串口初始化
        public bool InitSerialCom(string Description, string PortName, int BaudRate)
        {
            this.Description = Description;
            this.PortName = PortName;
            this.BaudRate = BaudRate;

            ComDevice.DataReceived += new SerialDataReceivedEventHandler(SerialCom_DataReceived);
            if (SerialPort.GetPortNames().Length == 0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}未发现可用串口", Description));
                //Logger.Error($"{Description}未发现可用串口!");
                return false;
            }

            if (ComDevice.IsOpen == false)
            {
                //设置串口属性
                ComDevice.PortName = PortName;
                ComDevice.BaudRate = BaudRate;
                ComDevice.Parity = Parity.None;    //奇偶校验 无
                ComDevice.DataBits = 8;            //数据长度 8
                ComDevice.StopBits = StopBits.One; //停止位   1
                ComDevice.Close();
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.ToString());
                    //Logger.Error(ex.ToString());
                    return false;
                }
            }
            else
            {
                ComDevice.Close();
            }
            return true;
        }

        readonly object obj = new object();

        /// <summary>
        /// 串口接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SerialCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (obj)
                {
                    if (bIsReadBuffer)
                        Thread.Sleep(200);

                    bytReceData = null;
                    //开辟接收缓冲区
                    Thread.Sleep(200);
                    byte[] ReDatas = new byte[ComDevice.BytesToRead];
                    //从串口读取数据
                    ComDevice.Read(ReDatas, 0, ReDatas.Length);
                    //实现数据的解码与显示
                    DecodeASCIIData(ReDatas);
                    DecodeHexData(ReDatas);
                    ComReceivedEvent(this, null);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            
        }

        /// <summary>
        /// 解码数据, 对应字符串
        /// </summary>
        /// <param name="data">串口通信的数据编码方式因串口而异，需要查询串口相关信息以获取</param>
        public void DecodeASCIIData(byte[] data)
        {
            //string str = new ASCIIEncoding().GetString(data);
            string str = Encoding.Default.GetString(data);
        }
        public void DecodeHexData(byte[] data)
        {
            bytReceData = data;
        }

        public bool SendData(byte[] data, bool bIsReadBuffer = false)
        {
            lock (Locker)
            {
                bool bfrist = true;
            Retry:
                if (ComDevice.IsOpen)
                {
                    try
                    {
                        this.bIsReadBuffer = bIsReadBuffer;
                        //发送信息
                        ComDevice.Write(data, 0, data.Length);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}发送失败: {1}", Description, ex.ToString()));
                        //Logger.Error($"{Description}发送失败: {ex.ToString()}");
                    }
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}串口没打开", Description));
                    if (bfrist)
                    {
                        ComDevice.Open();
                        goto Retry;
                        bfrist = false;
                    }
                }
                return false;
            }
        }

        public bool SendData(string data, bool bIsReadBuffer = false)
        {
            lock (Locker)
            {
                bool bfrist = true;
                Retry:
                if (ComDevice.IsOpen)
                {
                    try
                    {
                        this.bIsReadBuffer = bIsReadBuffer;
                        //发送信息
                        ComDevice.Write(data);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}发送失败: {1}", Description, ex.ToString()));
                        //Logger.Error($"{Description}发送失败: {ex.ToString()}");
                    }
                }
                else 
                {

                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}串口没打开", Description));
                    if (bfrist)
                    {
                        try
                        {
                            ComDevice.Open();
                        }
                        catch(Exception ee)
                        {
                            bfrist = false;
                            return false;
                        }
                        goto Retry;
                        bfrist = false;
                    }
                    //Logger.Error($"{Description}串口没打开");
                }
                return false;
            }
        }
        /// <summary>
        /// 十进制的字符串转为十六进制的字符串
        /// </summary>
        /// <param name="Ten"></param>
        /// <returns></returns>
        public string Ten2Hex(string Ten)
        {
            ulong tenValue = Convert.ToUInt64(Ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = TenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = TenValue2Char(tenValue) + hex;
            return hex;
        }
        /// <summary>
        /// 十进制的数值转为十六进制的字符
        /// </summary>
        /// <param name="Ten"></param>
        /// <returns></returns>
        public string TenValue2Char(ulong Ten)
        {
            switch (Ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return Ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }

        /// <summary>
        /// ASCII转字符串
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// 字符串转ASCII
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public byte[] str2ASCII(String xmlStr)
        {
            return Encoding.Default.GetBytes(xmlStr);
        }
    }
}
