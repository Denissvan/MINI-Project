using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keyence.AutoID.SDK;
using MotionCtrl;

namespace SunnyPrjTemplate.Controls
{
    public partial class ScanCode : UserControl
    {
        public ReaderAccessor m_reader = new ReaderAccessor();
        private ReaderSearcher m_searcher = new ReaderSearcher();
        List<NicSearchResult> m_nicList = new List<NicSearchResult>();

        public ScanCode()
        {
            InitializeComponent();
            m_nicList = m_searcher.ListUpNic();
            if (m_nicList != null)
            {
                for (int i = 0; i < m_nicList.Count; i++)
                {
                    NICcomboBox.Items.Add(m_nicList[i].NicIpAddr);
                }
            }

            if (NICcomboBox.Items.Count > 0)
            {
                NICcomboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 扫码控件初始化
        /// </summary>
        /// <param name="ip"></param>
        public void Init(string ip)
        {
            bool res = true;
            if (IsHandleCreated && !IsDisposed && !Disposing)
            {
                liveviewForm1.EndReceive();
                liveviewForm1.IpAddress = ip;
                res = liveviewForm1.BeginReceive();
                if (!res)
                {
                   VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,($"IP：{ip}扫码显示界面连接错误"));
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"IP：{ip}扫码显示界面连接成功");
                }
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"IP：{ip}扫码显示界面未创建句柄，跳过实时显示连接");
            }

            m_reader.IpAddress = ip;
            res = m_reader.Connect((data) =>
            {
                SafeReceivedDataWrite(Encoding.ASCII.GetString(data));
            });
            if (!res)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"IP：{ip}扫码器连接错误");
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"IP：{ip}扫码器连接成功");
            }
        }

        private void SchBtn_Click(object sender, EventArgs e)
        {
            if (!m_searcher.IsSearching)
            {
                m_searcher.SelectedNicSearchResult = m_nicList[NICcomboBox.SelectedIndex];
                NICcomboBox.Enabled = false;
                SchBtn.Enabled = false;
                SctBtn.Enabled = false;
                comboBox1.Items.Clear();
                //Start searching readers.
                m_searcher.Start((res) =>
                {
                    //Define searched actions here.Defined actions work asynchronously.
                    //"SearchListUp" works when a reader was searched.
                    BeginInvoke(new delegateUserControl(SearchListUp), res.IpAddress);
                });
            }
        }

        private void SctBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (SctBtn.Checked)
            {
                if (comboBox1.SelectedItem != null)
                {
                    //Stop liveview.
                    liveviewForm1.EndReceive();
                    //Set ip address of liveview.
                    liveviewForm1.IpAddress = comboBox1.SelectedItem.ToString();
                    //Start liveview.
                    bool res = liveviewForm1.BeginReceive();
                    if (!res)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "扫码显示界面连接错误");
                    }

                    //Set ip address of ReaderAccessor.
                    m_reader.IpAddress = comboBox1.SelectedItem.ToString();
                    //Connect TCP/IP.
                    res = m_reader.Connect((data) =>
                    {
                        //Define received data actions here.Defined actions work asynchronously.
                        //"ReceivedDataWrite" works when reading data was received.
                        SafeReceivedDataWrite(Encoding.ASCII.GetString(data));
                    });
                    if (!res)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "扫码器连接错误");
                    }

                    NICcomboBox.Enabled = false;
                    SchBtn.Enabled = false;
                    comboBox1.Enabled = false;
                    TgrBtn.Enabled = true;
                }
            }
            else
            {
                NICcomboBox.Enabled = true;
                SchBtn.Enabled = true;
                comboBox1.Enabled = true;
                TgrBtn.Enabled = false;
            }
        }

        private void TgrBtn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                //ExecCommand("command")is for sending a command and getting a command response.
                ReceivedDataWrite(m_reader.ExecCommand("LON"));
            }
        }

        private delegate void delegateUserControl(string str);

        private void SafeReceivedDataWrite(string receivedData)
        {
            try
            {
                if (IsHandleCreated && !IsDisposed && !Disposing)
                {
                    BeginInvoke(new delegateUserControl(ReceivedDataWrite), receivedData);
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"基恩士扫码回调返回:{receivedData}");
                }
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "基恩士扫码回调异常:" + ex);
            }
        }

        private void ReceivedDataWrite(string receivedData)
        {
            try
            {
                if (receivedData == "")
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "二维码信息获取失败");
                    return;
                }

                DataText.Text = "[" + m_reader.IpAddress + "][" + DateTime.Now + "]" + receivedData;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "基恩士扫码数据显示异常:" + ex);
            }
        }

        private void SearchListUp(string ip)
        {
            if (ip != "")
            {
                comboBox1.Items.Add(ip);
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                return;
            }
            else
            {
                NICcomboBox.Enabled = true;
                SctBtn.Enabled = true;
                SchBtn.Enabled = true;
            }
        }
    }
}
