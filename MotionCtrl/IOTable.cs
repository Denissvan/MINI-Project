using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace MotionCtrl
{
    public partial class IOTable : UserControl
    {
        public int UpdateCt;
        List<GPIO> list_IO=new List<GPIO> ();
        Color cl_out_on = Color.Lime;
        Color cl_in_on = Color.Orange;
        private static readonly Object LockObj = new object();
        int showcfg = 0;
        public int cnt = 0;
        public IOTable()
        {
            InitializeComponent();
            list_IO.Clear();            
        }

        private void FillTableWithAxisInf(GPIO io, int row = -2)
        {
            if (io == null) return;
            //if empty or add mode then add
            //if (dgv.Rows.Count == 0|| row == -2) row = dgv.Rows.Add();
            if (dgv.Rows.Count == 0 || row < 0 || row >= dgv.Rows.Count) row = dgv.Rows.Add();
            //the last row
            else if (row < 0) row = dgv.Rows.Count - 1;
            if (VAR.IsChinese)
            {
                dgv.Rows[row].Cells[0].Value = io.str_disc;
                dgv.Rows[row].Cells[1].Value = io._isON ? "ON" : "OFF";
                dgv.Rows[row].Cells[2].Value = io.dir == GPIO.IO_DIR.IN ? "IN" : "OUT";
                dgv.Rows[row].Cells[3].Value = io.axis != null ? io.axis.disc : io.card.disc;
                dgv.Rows[row].Cells[4].Value = io.num;
            }
            else
            {
                dgv.Rows[row].Cells[0].Value = io.english_disc;
                dgv.Rows[row].Cells[1].Value = io._isON ? "ON" : "OFF";
                dgv.Rows[row].Cells[2].Value = io.dir == GPIO.IO_DIR.IN ? "IN" : "OUT";
                dgv.Rows[row].Cells[3].Value = io.axis != null ? io.axis.disc : io.card.disc;
                dgv.Rows[row].Cells[4].Value = io.num;
            }
        }

        public void AddIO(GPIO io)
        {
            if (true)//if (ax.isInit)
            {
                if (io != null && io.card!=null && list_IO.Contains(io) == false)
                {
                    list_IO.Add(io);
                    FillTableWithAxisInf(io);
                }
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, String.Format("板卡列表,{0} 未初始化！", io.disc));
                return;
            }            
        }

        public void AddIO(List<GPIO> list_io)
        {
            foreach (GPIO io in list_io)
            {
                AddIO(io);
            }
        }

        public void ClearIO()
        {
            list_IO.Clear();
        }

        public void AutoUpdate(int intv_ms=300)
        {
            if (intv_ms > 0)
            {
                tmr_update.Interval = intv_ms;
                tmr_update.Enabled = true;
                tmr_update.Start();                
            }
            else
            {
                tmr_update.Enabled = false;
                tmr_update.Stop ();
            }
        }

        bool btsk = false;

        Task TskUpdateIO = null;
        void UpdateIO()
        {
            while (showcfg >= 0)
            {
                btsk = true;
                try
                {
                    btsk = true;
                    int t = Environment.TickCount;
                    //缓存，避免修改list_io时冲突
                    List<GPIO> list_temp = new List<GPIO>();
                    foreach (GPIO io in list_IO) list_temp.Add(io);
                    foreach (GPIO io in list_temp)
                    {
                        if (showcfg == 0 && io.dir!= GPIO.IO_DIR.OUT)continue;
                        if (showcfg == 1 && io.dir!= GPIO.IO_DIR.IN)continue;                        
                        io._isON = io.isON;
                    }
                    UpdateCt = Environment.TickCount - t;
                    Thread.Sleep(10);
                }
                catch(Exception ex)
                {
                    
                }
            }
            btsk = false;
        }
        public void UpdateShow()
        {
            if (TskUpdateIO == null|| TskUpdateIO.IsCompleted)
            {
                TskUpdateIO = new Task(UpdateIO);
                TskUpdateIO.Start();
            }

            if (dgv.Rows.Count != list_IO.Count) dgv.Rows.Clear();
            for (int r = 0; r < list_IO.Count; r++)
            {
                FillTableWithAxisInf(list_IO.ElementAt(r), r);
            }
            dgv.Update();
        }

        private void tmr_update_Tick(object sender, EventArgs e)
        {
            UpdateShow();            
        }
        /// <summary>
        /// 分类显示
        /// </summary>
        /// <param name="cfg">0：只显示OUT,1:只显示IN, 2：显示所有</param>
        public void ShowCfg(int cfg = 0)
        {
            showcfg = cfg;
            if(cfg == 0)
            {
                dgv.Columns[2].Visible = false;
                dgv.Columns[5].Visible = true;
                dgv.Columns[6].Visible = true;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[2].Value.ToString() == "OUT") row.Visible = true;
                    else row.Visible = false;
                }
            }
            else if (cfg == 1)
            {
                dgv.Columns[2].Visible = false;
                dgv.Columns[5].Visible = false;
                dgv.Columns[6].Visible = false;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[2].Value.ToString() == "IN") row.Visible = true;
                    else row.Visible = false;
                }
            }
            else if (cfg == 2)
            {
                dgv.Columns[2].Visible = true;
                dgv.Columns[5].Visible = true;
                dgv.Columns[6].Visible = true;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Visible = true;
                }
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            EM_RES ret;
            if (e.RowIndex < 0 || e.RowIndex > list_IO.Count) return;
            if (list_IO.ElementAt(e.RowIndex).str_disc != dgv.Rows[e.RowIndex].Cells[0].Value.ToString()&& list_IO.ElementAt(e.RowIndex).english_disc != dgv.Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                MessageBox.Show(VAR.IsChinese?"IO列表异常，请重新启动软件!": "IO list is abnormal, please restart the software! \r\n（IO列表异常，请重新启动软件!)");
                return;
            }

            //打开
            if (e.ColumnIndex == 5)
            {
                string str;
                bool table0 = false, table90 = false, table270 = false, table_inp = false, table_move = false, istable = false, isInp = true;
                str = list_IO.ElementAt(e.RowIndex).str_disc;
                if (str == "转台顺时针(FWD)信号")
                {
                    foreach (GPIO gpio in list_IO)
                    {
                        if (gpio.str_disc == "转台到位(X2)信号")
                        {
                           if(gpio.isON) table_inp = true;
                           else if (gpio.isOFF) isInp = false;
                        }
                        if (gpio.str_disc == "转台转动(Y1)信号" && gpio.isOFF)
                        {
                            table_move = true;
                        }
                        if (gpio.str_disc == "转台0°信号" && gpio.isON)
                        {
                            table0 = true;
                        }
                        if (gpio.str_disc == "转台90°信号" && gpio.isOFF)
                        {
                            table90 = true;
                        }
                        if (gpio.str_disc == "转台270°信号" && gpio.isOFF)
                        {
                            table270 = true;
                        }
                        if (!table0 && table90 && !table270)
                        {
                            isInp = false;
                        }
                        if (table0 && table90 && table270)
                        {
                            MessageBox.Show(VAR.IsChinese?"当前位置在POS3，不能顺时针转动": "The current position is at POS3 and cannot be turned clockwise \r\n(当前位置在POS3，不能顺时针转动)");
                            return;
                        }
                    }
                    istable = true;
                }
                if (str == "转台逆时针(REV)信号")
                {
                    foreach (GPIO gpio in list_IO)
                    {
                        if (gpio.str_disc == "转台到位(X2)信号")
                        {
                            if (gpio.isON) table_inp = true;
                            else if (gpio.isOFF) isInp = false;
                        }
                        else if (gpio.str_disc == "转台到位(X2)信号" && gpio.isOFF)
                        {
                            isInp = false;
                        }
                        if (gpio.str_disc == "转台转动(Y1)信号" && gpio.isOFF)
                        {
                            table_move = true;
                        }
                        if (gpio.str_disc == "转台0°信号" && gpio.isON)
                        {
                            table0 = true;
                        }
                        if (gpio.str_disc == "转台90°信号" && gpio.isON)
                        {
                            table90 = true;
                        }
                        if (gpio.str_disc == "转台270°信号" && gpio.isON)
                        {
                            table270 = true;
                        }
                        if (!table0 && !table90 && table270)
                        {
                            isInp = false;
                        }
                        if (table0 && table90 && table270)
                        {
                            MessageBox.Show(VAR.IsChinese?"当前位置在POS0，不能逆时针转动": "Current position is at POS0, cannot be turned counterclockwise\r\n(当前位置在POS0，不能逆时针转动)");
                            return;
                        }
                    }
                    istable = true;
                }
                if (istable)
                {
                    DialogResult Dlgres;
                    Dlgres = MessageBox.Show("1.按'是'检测后进行顺时针/逆时针旋转!\n2.按'否'强制进行顺时针/逆时针旋转!\n3.按'取消'退出!", "提示",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (DialogResult.No == Dlgres) isInp = false;
                    else if (DialogResult.Cancel == Dlgres) return;

                    if (table_inp && table_move)
                    {
                        ret = list_IO.ElementAt(e.RowIndex).SetOn();
                         Thread.Sleep(600);
                        ret = list_IO.ElementAt(e.RowIndex).SetOff();
                        if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese ? list_IO.ElementAt(e.RowIndex).disc + "打开异常!Err:" + ret.ToString(): list_IO.ElementAt(e.RowIndex).disc + "Set on!Err:" + ret.ToString());
                        else
                            MessageBox.Show(VAR.IsChinese?"打开正常": "Set on normally\r\n(打开正常)");
                    }
                    else if (!isInp)
                    {
                        if (cnt < 1)
                        {
                            ret = list_IO.ElementAt(e.RowIndex).SetOn();
                            Thread.Sleep(600);
                            ret = list_IO.ElementAt(e.RowIndex).SetOff();
                            if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese ? list_IO.ElementAt(e.RowIndex).disc + "打开异常!Err:" + ret.ToString(): list_IO.ElementAt(e.RowIndex).disc + "Set on!Err:" + ret.ToString());
                            else
                                MessageBox.Show(VAR.IsChinese?"打开正常": "Set on normally\r\n(打开正常)");
                            cnt++;
                        }
                        else
                        {
                            MessageBox.Show(VAR.IsChinese?"当前转盘未到位，且转动次数超过1，请重启软件确认!": "The current turntable is not in place, and the number of rotations exceeds 1, please restart the software to confirm!\r\n(当前转盘未到位，且转动次数超过1，请重启软件确认!)");
                        }
                    }
                    else
                    {
                        MessageBox.Show(VAR.IsChinese ? "转盘到位/转动信号异常": "Turntable in-position / rotation signal abnormal\r\n(转盘到位/转动信号异常)");
                    }
                }
                else
                {
                    ret = list_IO.ElementAt(e.RowIndex).SetOn();
                    if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese?list_IO.ElementAt(e.RowIndex).disc + "打开异常!Err:" + ret.ToString(): list_IO.ElementAt(e.RowIndex).disc + "Open err!Err:" + ret.ToString());
                }
            }
            //关闭
            else if (e.ColumnIndex == 6)
            {
                ret = list_IO.ElementAt(e.RowIndex).SetOff();
                if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese?list_IO.ElementAt(e.RowIndex).disc + "关闭异常!Err:" + ret.ToString(): list_IO.ElementAt(e.RowIndex).disc + "Close err!Err:" + ret.ToString());
            }
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == 1)
            {
                if (e.Value.ToString() == "ON")
                {
                    if (list_IO.ElementAt(e.RowIndex).dir == GPIO.IO_DIR.OUT) e.CellStyle.BackColor = cl_out_on;
                    else e.CellStyle.BackColor = cl_in_on;
                }
                else if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = Color.WhiteSmoke;
            }
            else if ((e.RowIndex & 1) == 1)
            {
                e.CellStyle.BackColor = Color.WhiteSmoke;
            }
        }


        public void ChangeColumn()
        { //disc status dir card num btn_open btn_close
            if (VAR.IsChinese)
            {
                disc.HeaderText = "IO";
                status.HeaderText = "状态";
                dir.HeaderText = "类型";
                card.HeaderText = "板卡";
                num.HeaderText = "编号";
                btn_open.Text = "打开";
                btn_close.Text = "关闭";
            }
            else
            {
                disc.HeaderText = "Disc";
                status.HeaderText = "Status";
                dir.HeaderText = "Dir";
                card.HeaderText = "Card";
                num.HeaderText = "Num";
                btn_open.Text = "Open";
                btn_close.Text = "Close";
            }
        }
    }
}
