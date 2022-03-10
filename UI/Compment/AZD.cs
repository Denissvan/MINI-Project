using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI.Compment
{
    public partial class AZD : UserControl
    {
        public WS ws = null;
        public bool bon_zk = true;
        public bool bon_power = true;
        public AZD()
        {
            InitializeComponent();
            lb2.Click += new EventHandler(lb1_Click);
            lb3.Click += new EventHandler(lb1_Click);
            lb4.Click += new EventHandler(lb1_Click);

            lb5.Click += new EventHandler(lb1_Click);
            lb6.Click += new EventHandler(lb1_Click);
            lb7.Click += new EventHandler(lb1_Click);
            lb8.Click += new EventHandler(lb1_Click);
        }

        private void lb1_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            if (ws.list_cld_fr == null && ws.list_cld_fr.Count == 0) return;
            if (ws.list_cld_bk == null && ws.list_cld_bk.Count == 0) return;

            //get cylinder
            int idx = -1;
            if (!int.TryParse(((Label)sender).Text, out idx)) return;
            idx -= 1;
            if (idx < 0) return;
            Cylinder cy = null;
            if (idx < ws.list_cld_fr.Count)
            {
                cy = ws.list_cld_fr.ElementAt(idx);
            }
            else
            {
                idx -= ws.list_cld_fr.Count;
                if (idx < ws.list_cld_bk.Count)
                {
                    cy = ws.list_cld_bk.ElementAt(idx);
                }
            }
            if (cy == null || cy.io_out1 == null || cy.io_out2 == null) return;                     

            //check
            EM_RES res = EM_RES.OK;            
            if (cy.isON)
            {
                if (!ws.isUInFeedPos)
                {
                    if (DialogResult.OK != MessageBox.Show(VAR.IsChinese?string.Format("{0}不在水平上料位，打开压盖可能掉料，确定要打开？", ws.disc): string.Format("{0} is not in the horizontal loading level, the gland may fall when you open it. Are you sure you want to open it?\r\n{0}不在水平上料位，打开压盖可能掉料，确定要打开？", ws.disc), "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                        return;
                }
                //在光箱位置确保安全
                LightBox lb = null;
                res = COM.GetLightBox(ws.num, ref lb);
                if (lb != null && !lb.isInSafePos)
                {
                    MessageBox.Show(VAR.IsChinese?string.Format("{0} 与光箱可能有干涉，禁止打开！\r\n请先复位光箱!", cy.io_out1.str_disc): string.Format("There may be interference between the {0} and the light box. Do not open it! \r\n Please reset the light box first!\r\n {1} 与光箱可能有干涉，禁止打开！\r\n请先复位光箱!", cy.io_out1.english_disc,cy.io_out1.str_disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (res != EM_RES.OK && (!COM.RightLightBox.isInSafePos || !COM.LeftLightBox.isInSafePos || !COM.OTPLightBox.isInSafePos))
                {
                    MessageBox.Show(VAR.IsChinese ? string.Format("{0},转台未知位置，与光箱可能有干涉，禁止打开！\r\n请先复位光箱!", cy.io_out1.str_disc): string.Format("{0},The unknown position of the turntable may interfere with the light box, and it is forbidden to open it! \r\n Please reset the light box first!\r\n{1},转台未知位置，与光箱可能有干涉，禁止打开！\r\n请先复位光箱!", cy.io_out1.english_disc, cy.io_out1.str_disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (ws.isUInFeedPos&&PT_SET.bCool)
            {
                ws.gpio_out_gz_wind.SetOff();
                if (ws.gpio_out_gz_wind.res != EM_RES.OK) return;
            }
            //actiton
            //res = cy.Invert(); 
            if (cy.isON)
            {
                cy.SetOn();
            }
            else
            {
                cy.SetOff();
            }
            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?string.Format("{0}操作异常！", cy.io_out1.str_disc): string.Format("{0} Abnormal operation!\r\n{1}操作异常！", cy.io_out1.english_disc, cy.io_out1.str_disc),VAR.IsChinese? "提示": "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool ChkAndUpdatePnl(List<Cylinder> list_cld, TableLayoutPanel tbp,int indofs = 0)
        {
            if (list_cld == null || tbp == null) return false;

            bool bAllOpen = true;
            int num = 0;
             for (int n = 0; n < list_cld.Count; n++)
            {
                Control[] control = tbp.Controls.Find(string.Format("pnl{0}", n + 1 + indofs), true);
                if (control != null && control.Count() > 0)
                {
                    //check out
                    if (list_cld[n].isOFF)                         //气缸打开，夹具打开
                    {
                        if (n + 1 + indofs > list_cld.Count) ((Panel)control[0]).Dock = DockStyle.Bottom;
                        else ((Panel)control[0]).Dock = DockStyle.Top;

                        ((Panel)control[0]).BorderStyle = BorderStyle.Fixed3D;
                        ((Panel)control[0]).BackColor = Color.Orange;
                        bAllOpen = false;

                        //check sensor
                        //if (list_cld[n].isOFFByChkSen && !list_cld[n].isONByChkSen)
                        if (list_cld[n].isOFFByChkSen && !list_cld[n].isONByChkSen)      //放宽条件
                        {
                            ((Panel)control[0]).BackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (n + 1 + indofs > list_cld.Count) ((Panel)control[0]).Dock = DockStyle.Top;
                        else ((Panel)control[0]).Dock = DockStyle.Bottom;

                        ((Panel)control[0]).BorderStyle = BorderStyle.FixedSingle;
                        ((Panel)control[0]).BackColor = SystemColors.InactiveCaption;

                        //check sensor
                        if (list_cld[n].isOFFByChkSen && !list_cld[n].isONByChkSen)
                        {
                            if (n + 1 + indofs > list_cld.Count) ((Panel)control[0]).Dock = DockStyle.Bottom;
                            else ((Panel)control[0]).Dock = DockStyle.Top;
                            ((Panel)control[0]).BackColor = Color.Lime;
                        }
                        else if (list_cld[n].isOFFByChkSen || !list_cld[n].isONByChkSen)            //气缸打开，夹具打开
                        {
                            ((Panel)control[0]).BackColor = Color.Orange;
                            //bAllOpen = true;
                            num++;
                        }
                    }

                    //if (((Panel) control[0]).BackColor == Color.Lime)
                    //{
                    //    if (n + 1 + indofs > list_cld.Count ) ((Panel)control[0]).Dock = DockStyle.Bottom;
                    //    else ((Panel)control[0]).Dock = DockStyle.Top;
                    //}
                    //else
                    //{
                    //    if (n + 1 + indofs > list_cld.Count) ((Panel)control[0]).Dock = DockStyle.Top;
                    //    else ((Panel)control[0]).Dock = DockStyle.Bottom;
                    //}
                }
            }
             if (num == list_cld.Count)
             {
                 bAllOpen = true;
             }
            return bAllOpen;
        }
        public void UpdateShow()
        {
            if (ws == null) return;
            if (ws.list_cld_fr == null && ws.list_cld_fr.Count == 0) return;
            if (ws.list_cld_bk == null && ws.list_cld_bk.Count == 0) return;
            string str_sta = VAR.IsChinese?"未知":"Unknown";
            if(ws.ax_u!=null)
            {
                if (ws.isUInTestPos)
                    str_sta = VAR.IsChinese ? "测试位◐": "Test◐";
                else if (ws.isUInFeedPos)
                    str_sta = VAR.IsChinese ? "水平位◓": "Feed◐";
                else
                    str_sta = VAR.IsChinese ? "异常":"ERROR";
            }
            if(VAR.IsChinese) lbstatus.Text = string.Format("{0}  {1}  状态:{2}      {3}    {4}", ws.disc, Utility.GetDescription(Turntable.GetWSSta(ws.num), VAR.IsChinese), Utility.GetDescription(ws.Status, VAR.IsChinese), str_sta, ws.StrOfPosA);
            else lbstatus.Text = string.Format("{0}  {1}  Status:{2}      {3}    {4}",ws.disc, Utility.GetDescription(Turntable.GetWSSta(ws.num), VAR.IsChinese), Utility.GetDescription(ws.Status, VAR.IsChinese), str_sta,ws.StrOfPosA);

            bool bAllOpen = true;
            bAllOpen = ChkAndUpdatePnl(ws.list_cld_fr, tblA);
            if(VAR.IsChinese)btn_open_close_a.Text = bAllOpen ? "合" : "开";
            else btn_open_close_a.Text = bAllOpen ? "Close" : "Open";
            bAllOpen = ChkAndUpdatePnl(ws.list_cld_bk, tblB , ws.list_cld_fr.Count);
            if (VAR.IsChinese) btn_open_close_b.Text = bAllOpen ? "合" : "开";
            else btn_open_close_b.Text = bAllOpen ? "Close" : "Open";
            if (VAR.IsChinese) btn_zk_on_off.Text = bon_zk ? "真空" : "破真空";
            else btn_zk_on_off.Text = bon_zk ? "Vacuum" : "Vacuum Break";
            if (VAR.IsChinese) btn_power_on_off.Text = bon_power ? "上电" : "失电";
            else btn_power_on_off.Text = bon_power ? "Power Up" : "Power Down";
            //if (ws.isFrOpen)
            //{
            //    tblA.BackColor = SystemColors.Control;
            //    tblA.Dock = DockStyle.Bottom;
            //    btn_open_close_a.Text = "合";
            //}
            //else if (ws.isFrClose)
            //{
            //    tblA.BackColor = SystemColors.Control;
            //    tblA.Dock = DockStyle.Top;
            //    btn_open_close_a.Text = "开";
            //}
            //else
            //{
            //    tblA.BackColor = Color.Yellow;
            //}

            //if (ws.isBkOpen)
            //{
            //    tblB.BackColor = SystemColors.Control;
            //    tblB.Dock = DockStyle.Top;
            //    btn_open_close_b.Text = "合";
            //}
            //else if (ws.isBkClose)
            //{
            //    tblB.BackColor = SystemColors.Control;
            //    tblB.Dock = DockStyle.Bottom;
            //    btn_open_close_b.Text = "开";
            //}
            //else
            //{
            //    tblB.BackColor = Color.Yellow;
            //}
        }

        private void AZD_Resize(object sender, EventArgs e)
        {
            tblA.Height = tbp_btnA.Height * 2 / 3;
            tblB.Height = tbp_btnB.Height * 2 / 3;
            for (int i = 0; i < 4; i++)
            {
                Control[] control = tblA.Controls.Find(string.Format("pnl{0}", i + 1), true);
                if (control != null && control.Count() > 0)
                {
                    ((Panel) control[0]).Height = tblA.Height * 2 / 3;
                }
                  control = tblB.Controls.Find(string.Format("pnl{0}", i + 5), true);
                if (control != null && control.Count() > 0)
                {
                    ((Panel)control[0]).Height = tblB.Height * 2 / 3;
                }
            }
            
        }

      


        private void btn_u_0_a_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese?string.Format("{0}确定要旋转到测试位?", ws.disc): string.Format("Are you sure you want to rotate {0} to the test position?\r\n{0}确定要旋转到测试位?", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            VAR.gsys_set.bquit = false;
            EM_RES res = ws.TurnToTest(ref VAR.gsys_set.bquit,true);
            if (res == EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}旋转到测试位完成！", ws.disc): string.Format("{0} is rotated to the test position！\r\n{0}旋转到测试位完成！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (res == EM_RES.MOVE_PROTECT)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}旋转到测试位异常！\r\n气缸未全部压盖到位", ws.disc):string.Format("The rotation of {0} to the test position is abnormal! \r\n Cylinders are not fully capped\r\n{0}旋转到测试位异常！\r\n气缸未全部压盖到位", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}旋转到测试位异常，{1}", ws.disc,Utility.GetDescription(res, VAR.IsChinese)): string.Format("The rotation of {0} to the test position is abnormal!,{1}\r\n{0}旋转到测试位异常，{2}", ws.disc, Utility.GetDescription(res, VAR.IsChinese), Utility.GetDescription(res)), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_u_90_a_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese ? string.Format("{0}确定要旋转到上料位(水平位)?", ws.disc): string.Format("Are you sure you want to rotate {0} to the loading level (horizontal position)?\r\n{0}确定要旋转到上料位(水平位)?", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;

            VAR.gsys_set.bquit = false;
            EM_RES res = ws.TurnToFeed(ref VAR.gsys_set.bquit,true);
            if (res == EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?string.Format("{0}旋转到上料位(水平位)完成！", ws.disc): string.Format("Rotate {0} to the loading level (horizontal position)!\r\n{0}旋转到上料位(水平位)完成！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (res == EM_RES.MOVE_PROTECT)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}旋转到上料位(水平位)异常！\r\n安全保护!", ws.disc): string.Format("The rotation of {0} to loading level (horizontal position) is abnormal\r\n{0}旋转到上料位(水平位)异常！\r\n安全保护!", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}旋转到上料位(水平位)异常，{1}", ws.disc, Utility.GetDescription(res, VAR.IsChinese)): string.Format("The rotation of {0} to loading level (horizontal position) abnormal,{1}\r\n{0}旋转到上料位(水平位)异常，{2}", ws.disc, Utility.GetDescription(res, VAR.IsChinese), Utility.GetDescription(res)), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void btn_x_n_a_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("前排点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        //private void btn_x_p_a_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("前排点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        //private void btn_x_p_b_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("后排点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        //private void btn_x_n_b_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("后排点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        //private void btn_u_ccw_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("U点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        //private void btn_u_cw_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("U点动功能未完善！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //}

        private void btn_test_pos_Click(object sender, EventArgs e)
        {
            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            res = ws.SetupForTest(ref VAR.gsys_set.bquit);
            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?string.Format("{0}转到测试状态异常！", ws.disc): string.Format("{0} went to the test pos abnormally!\r\n{0}转到测试状态异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_feed_pos_Click(object sender, EventArgs e)
        {
            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            res = ws.SetupForFeed(ref VAR.gsys_set.bquit);
            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}转到上下料状态异常！", ws.disc): string.Format("{0} went to the feed pos abnormally!\r\n{0}转到上下料状态异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_open_close_b_Click(object sender, EventArgs e)
        {
            bool bon = ((Button)sender).Text == (VAR.IsChinese?"开":"Open");

            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;

            if (ws.pos_idx == (int)Turntable.EM_STA.POS0 && (!COM.UDLoad1.ax_x.isORG || !COM.UDLoad2.ax_x.isORG || COM.UDLoad1.ax_y.fenc_pos > COM.UDLoad1.list_xt[1].st_cap_pos.y || COM.UDLoad2.ax_y.fenc_pos > COM.UDLoad2.list_xt[1].st_cap_pos.y))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("上下料未回安全位置,{0}禁止气缸开合", ws.disc): string.Format("Updownload did not return to the safe position, {0} forbidden to open and close the cylinder!          (上下料未回安全位置,{0}禁止气缸开合)", ws.disc));
                return;
            }

            if (bon)
                res = ws.BkCyUp(ref VAR.gsys_set.bquit);
            else
                res = ws.BkCyDown(ref VAR.gsys_set.bquit);

            if (res != EM_RES.OK)
             MessageBox.Show(VAR.IsChinese?string.Format("{0}后排动作异常！", ws.disc): string.Format("{0} Back row abnormal\r\n{0}后排动作异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_open_close_a_Click(object sender, EventArgs e)
        {
            bool bon = ((Button)sender).Text == (VAR.IsChinese ? "开" : "Open");

            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;

            if (ws.pos_idx == (int)Turntable.EM_STA.POS0 && (!COM.UDLoad1.ax_x.isORG || !COM.UDLoad2.ax_x.isORG || COM.UDLoad1.ax_y.fenc_pos > COM.UDLoad1.list_xt[1].st_cap_pos.y || COM.UDLoad2.ax_y.fenc_pos > COM.UDLoad2.list_xt[1].st_cap_pos.y))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("上下料未回安全位置,{0}禁止气缸开合", ws.disc): string.Format("Updownload did not return to the safe position, {0} forbidden to open and close the cylinder!         (上下料未回安全位置,{0}禁止气缸开合)", ws.disc));
                return;
            }

            if (bon)
                res = ws.FrCyUp(ref VAR.gsys_set.bquit);
            else
                res = ws.FrCyDown(ref VAR.gsys_set.bquit);

            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?string.Format("{0}前排动作异常！", ws.disc): string.Format("{0}Front row abnormal\r\n{0}前排动作异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_ZK_on_off_Click(object sender, EventArgs e)
        {
            bon_zk = ((Button)sender).Text == (VAR.IsChinese ? "真空" : "Vacuum");
            
            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            if (bon_zk)
            {
                res = ws.ZKOn(ref VAR.gsys_set.bquit);
                bon_zk = false;
            }
            else
            {
                res = ws.ZKOff(ref VAR.gsys_set.bquit);
                bon_zk = true;
            }

            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?string.Format("{0}真空、破真空异常！", ws.disc): string.Format("Vacuum and vacuum breaking is abnormal!\r\n{0}真空、破真空异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_power_on_off_Click(object sender, EventArgs e)
        {
            bon_power = ((Button)sender).Text == (VAR.IsChinese ? "上电" : "Power Up");

            EM_RES res = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            if (bon_power)
            {
                res = ws.PowerOn(ref VAR.gsys_set.bquit);
                bon_power = false;
            }
            else
            {
                res = ws.PowerOff(ref VAR.gsys_set.bquit);
                bon_power = true;
            }

            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?string.Format("{0}上下电异常！", ws.disc): string.Format("{0}Power on and off abnormally!\r\n{0}上下电异常！", ws.disc), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
