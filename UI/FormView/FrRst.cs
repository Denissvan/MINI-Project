using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DevReport;
using MotionCtrl;
using Win32Lib;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;

namespace UI
{
    public partial class FrRst : Form
    {
        bool bflag;
        bool breset = false;

        #region 委托弹框
        delegate DialogResult MessageBoxShow(string text,string caption, MessageBoxButtons buttons,MessageBoxIcon icon);
        DialogResult MessageBoxShowF(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var dr = MessageBox.Show(text,caption,buttons,icon);
            return dr;
        }
        public DialogResult ShowMessage(string text, string caption="提示", MessageBoxButtons buttons= MessageBoxButtons.OKCancel, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            var dr = new DialogResult();
            try
            {
                var tmp = this.Invoke(new MessageBoxShow(MessageBoxShowF), new object[] { text, caption, buttons, icon });
                if (tmp != null) dr = (DialogResult)tmp;
            }
            catch
            {

            }
            return dr;
        }
        #endregion
        #region 委托操作按键
        delegate void EnableObject(object sender,bool ben = true);
        void EnableObj(object sender, bool ben = true)
        {
            ((dynamic)sender).Enabled = ben;
        }
        public void Enablebtn(object sender, bool ben = true)
        {
            try
            {
                this.Invoke(new EnableObject(EnableObj), new object[] { sender, ben });
            }
            catch
            {

            }
        }
        #endregion

        
        public FrRst()
        {
            InitializeComponent();
        }

        public bool bupdate
        {
            get { return timer_update.Enabled; }
            set { timer_update.Enabled = value; }
        }

        private void FrUser_Load(object sender, EventArgs e)
        {

        }
        void SetBtnStatusColor(Button btn, AXIS.HOME_STA sta, bool bflag)
        {
            if (sta == AXIS.HOME_STA.OK) btn.BackColor = Color.DodgerBlue;
            else if (sta == AXIS.HOME_STA.HOMING) btn.BackColor = bflag ? Color.DodgerBlue : SystemColors.ButtonFace;
            else if (sta == AXIS.HOME_STA.ERROR || sta == AXIS.HOME_STA.UNKOWN) btn.BackColor = Color.Orange;
            else btn.BackColor = SystemColors.ButtonFace;
        }        
        private void btn_ul_dl_home_Click(object sender, EventArgs e)
        {
            EM_RES res_chk=EM_RES.OK;
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (COM.UDLoad1.status_ud == UpDownLoad.EM_STA.HOME || COM.UDLoad2.status_ud ==UpDownLoad.EM_STA.HOME) return;
            if (DialogResult.OK != MessageBox.Show(VAR.IsChinese?"确定要复位 【上下料】？": "Are you sure you want to reset 【updownload】? \r\n确定要复位 【上下料】？", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)) return;
            MT.DoorAlarmMsg = string.Empty;
            MT.DoorAlarmMsgTemp = string.Empty;
            res_chk = MT.ChkAllSafeSen(0x01);
            if (res_chk != EM_RES.OK) return;
            Task task_home = new Task(() =>
            {
                try
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_RESET);
                    Enablebtn(btn_all_home, false);
                    Enablebtn(sender, false);
                    VAR.gsys_set.bquit = false;
                    EM_RES  res=new EM_RES();
                    EM_RES[] array_res=new EM_RES[2];
                    Task[] task_udlhome = new Task[2];
                    foreach (UpDownLoad Udl in COM.List_UDLoad)
                    {
                         task_udlhome[COM.List_UDLoad.IndexOf(Udl)] = new Task(() =>{ array_res[COM.List_UDLoad.IndexOf(Udl)] = Udl.Home(ref VAR.gsys_set.bquit); });
                         task_udlhome[COM.List_UDLoad.IndexOf(Udl)].Start();
                         Udl.status_ud = UpDownLoad.EM_STA.HOME;
                    }

                    task_udlhome[0].Wait();
                    task_udlhome[1].Wait();
                    if (array_res[0] == EM_RES.OK && array_res[1] == EM_RES.OK) res = EM_RES.OK;
                    else res = EM_RES.ERR;
                    for (int i = 0; i < COM.List_UDLoad.Count; i++)
                    {
                        if(array_res[i]==EM_RES.OK) COM.List_UDLoad[i].status_ud= UpDownLoad.EM_STA.READY;
                        else COM.List_UDLoad[i].status_ud = UpDownLoad.EM_STA.UNKNOW;
                    }
                     
                    ShowMessage(VAR.IsChinese?string.Format("【{0}】 复位 {1}！", "上下料", res == EM_RES.OK ? "成功" : "失败"): string.Format("【{0}】 reset {1}!\r\n【{2}】 复位 {3}！", "Updownload", res == EM_RES.OK ? "Successful" : "Failed", "上下料", res == EM_RES.OK ? "成功" : "失败"), VAR.IsChinese?"提示": "prompt", MessageBoxButtons.OK);
                }
                finally
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                    Enablebtn(sender, true);
                    Enablebtn(btn_all_home, true);
                    COM.UDLoad1.ax_y.DisableHcmp(COM.CamDw1.TriIO.num);
                    COM.UDLoad1.ax_y.DisableHcmp(COM.CamUp1.TriIO.num);
                    COM.UDLoad2.ax_y.DisableHcmp(COM.CamDw2.TriIO.num);
                    COM.UDLoad2.ax_y.DisableHcmp(COM.CamUp2.TriIO.num);
                }
            }
            );
            task_home.Start();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            DRpt.Report_Opration(1000, 0, "上下料停止按键按下!");
            VAR.gsys_set.bquit = true;
            UpDownLoad.bquit = true;
            UpDownLoad.UD1Stop();
            UpDownLoad.UD2Stop();
            UpDownLoad.LCStop();
            //MT.AllAxStop();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            bflag = !bflag;

            //左光箱
            SetBtnStatusColor(btn_lightbox_left_x1, MT.AXIS_BOX_L_X1.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_left_x2, MT.AXIS_BOX_L_X2.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_left_z2, MT.AXIS_BOX_L_Z1.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_left_z1, MT.AXIS_BOX_L_Z2.home_status, bflag);
            if (MT.AXIS_BOX_L_Y1 != null)
            {
                btn_lightbox_left_y1.Visible = true;
                SetBtnStatusColor(btn_lightbox_left_y1, MT.AXIS_BOX_L_Y1.home_status, bflag);               
            }
            else btn_lightbox_left_y1.Visible = false;

            //右光箱
            SetBtnStatusColor(btn_lightbox_right_x1, MT.AXIS_BOX_R_X1.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_right_x2, MT.AXIS_BOX_R_X2.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_right_z1, MT.AXIS_BOX_R_Z1.home_status, bflag);
            SetBtnStatusColor(btn_lightbox_right_z2, MT.AXIS_BOX_R_Z2.home_status, bflag);

            //上下料
            SetBtnStatusColor(btn_udl2_x, MT.AXIS_UDL2_X.home_status, bflag);
            SetBtnStatusColor(btn_udl2_z, MT.AXIS_UDL2_Z.home_status, bflag);
            SetBtnStatusColor(btn_udl2_y, MT.AXIS_UDL2_Y.home_status, bflag);
            SetBtnStatusColor(btn_udl2_u1, MT.AXIS_UDL2_U1.home_status, bflag);
            SetBtnStatusColor(btn_udl2_u2, MT.AXIS_UDL2_U2.home_status, bflag);

            SetBtnStatusColor(btn_udl1_x, MT.AXIS_UDL1_X.home_status, bflag);
            SetBtnStatusColor(btn_udl1_y, MT.AXIS_UDL1_Y.home_status, bflag);
            SetBtnStatusColor(btn_udl1_z, MT.AXIS_UDL1_Z.home_status, bflag);
            SetBtnStatusColor(btn_udl1_u1, MT.AXIS_UDL1_U1.home_status, bflag);
            SetBtnStatusColor(btn_udl1_u2, MT.AXIS_UDL1_U2.home_status, bflag);

            SetBtnStatusColor(btn_traybox_fd_z, MT.AXIS_UDL_FD_Z.home_status, bflag);
            SetBtnStatusColor(btn_traybox_fd_x, MT.AXIS_UDL_FD_X.home_status, bflag);
            SetBtnStatusColor(btn_traybox_ok_z, MT.AXIS_UDL_OK_Z.home_status, bflag);
            SetBtnStatusColor(btn_traybox_ok_x, MT.AXIS_UDL_OK_X.home_status, bflag);
            SetBtnStatusColor(btn_traybox_ng_z, MT.AXIS_UDL_NG_Z.home_status, bflag);
            SetBtnStatusColor(btn_traybox_ng_x, MT.AXIS_UDL_NG_X.home_status, bflag);

            //WS1
            //SetBtnStatusColor(btn_zt_ws1_f, COM.ws1.ax_fr.home_status, bflag);
           // SetBtnStatusColor(btn_zt_ws1_b, COM.ws1.ax_bk.home_status, bflag);
            SetBtnStatusColor(btn_zt_ws1_u, COM.ws1.ax_u.home_status, bflag);
            //WS2
            //SetBtnStatusColor(btn_zt_ws2_f, COM.ws2.ax_fr.home_status, bflag);
            //SetBtnStatusColor(btn_zt_ws2_b, COM.ws2.ax_bk.home_status, bflag);
            SetBtnStatusColor(btn_zt_ws2_u, COM.ws2.ax_u.home_status, bflag);
            //WS3
            //SetBtnStatusColor(btn_zt_ws3_f, COM.ws3.ax_fr.home_status, bflag);
            //SetBtnStatusColor(btn_zt_ws3_b, COM.ws3.ax_bk.home_status, bflag);
            SetBtnStatusColor(btn_zt_ws3_u, COM.ws3.ax_u.home_status, bflag);
            //WS4
            //SetBtnStatusColor(btn_zt_ws4_f, COM.ws4.ax_fr.home_status, bflag);
            //SetBtnStatusColor(btn_zt_ws4_b, COM.ws4.ax_bk.home_status, bflag);
            SetBtnStatusColor(btn_zt_ws4_u, COM.ws4.ax_u.home_status, bflag);

            SetBtnStatusColor(btn_zt_z_otp, MT.AXIS_BOX_OTP_Z.home_status, bflag);

            lb_leftbox.Text = COM.LeftLightBox.StrOfPos;
            lb_rightbox.Text = COM.RightLightBox.StrOfPos;


            lb_udl2.Text = COM.UDLoad2.StrOfPos;
            lb_box_ok.Text = COM.traybox_ok.StrOfPos;
            lb_box_ng.Text = COM.traybox_ng.StrOfPos;

            lb_udl1.Text = COM.UDLoad1.StrOfPos;
            lb_box_fd.Text = COM.traybox_fd.StrOfPos;

            lb_ws_1.Text = COM.ws1.StrOfPos;
            lb_ws_2.Text = COM.ws2.StrOfPos;
            lb_ws_3.Text = COM.ws3.StrOfPos;
            lb_ws_4.Text = COM.ws4.StrOfPos;
            lb_tp.Text = Utility.GetDescription(Turntable.mCurPos, VAR.IsChinese);
            lb_opt.Text = COM.OTPLightBox.StrOfPos;
        }

        private async void btn_lightbox_left_home_Click(object sender, EventArgs e)
        {
            EM_RES res_chk = EM_RES.OK;
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (COM.LeftLightBox.status == LightBox.EM_STA.HOME) return;
            if(DialogResult.OK != MessageBox.Show(VAR.IsChinese?"确定要复位 【左光箱】？":"Are you sure you want to reset 【left light box】?\r\n确定要复位 【左光箱】？", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))return;
            res_chk = MT.ChkAllSafeSen(0x08);
            if (res_chk != EM_RES.OK) return;
            DRpt.Report_Opration(1000, 0, "左光箱回零按键按下!");
            Task task_home = new Task(()=>
            {
                try
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_RESET);
                    Enablebtn(sender, false);
                    Enablebtn(btn_all_home, false);
                    VAR.gsys_set.bquit = false;
                    EM_RES res = COM.LeftLightBox.Home(ref VAR.gsys_set.bquit);
                    if(res==EM_RES.OK) DRpt.Report_Opration(1000, 0, "左光箱回零成功!");
                    else DRpt.Report_Opration(1000, 0, "左光箱回零失败!");
                    ShowMessage(VAR.IsChinese?string.Format("【{0}】 复位 {1}！", COM.LeftLightBox.disc, res == EM_RES.OK ? "成功" : "失败"): string.Format("【{0}】 reset {1}！\r\n【{2}】 复位 {3}！", COM.LeftLightBox.english_disc, res == EM_RES.OK ? "Successful" : "Failed",COM.LeftLightBox.disc, res == EM_RES.OK ? "成功" : "失败"), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK);
                }
                finally
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                    Enablebtn(sender, true);
                    Enablebtn(btn_all_home, true);
                }
            }
            );
            task_home.Start();
            await task_home;
        }

        private void btn_lightbox_left_stop_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            DRpt.Report_Opration(1000, 0, "左光箱停止按键按下!");
            MT.DoorAlarmMsg = string.Empty;
            MT.DoorAlarmMsgTemp = string.Empty;
            MT.AxisHomeQuit(MT.AxList_BOX_LEFT);
        }
        private async void btn_lightbox_right_home_Click(object sender, EventArgs e)
        {
            EM_RES res_chk = EM_RES.OK;
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (COM.RightLightBox.status == LightBox.EM_STA.HOME) return;
            MT.DoorAlarmMsg = string.Empty;
            MT.DoorAlarmMsgTemp = string.Empty;
            if (DialogResult.OK != MessageBox.Show(VAR.IsChinese?"确定要复位 【右光箱】？": "Are you sure you want to reset 【right light box】?\r\n确定要复位 【右光箱】？", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)) return;
            res_chk = MT.ChkAllSafeSen(0x04);
            if (res_chk != EM_RES.OK) return;
            DRpt.Report_Opration(1000, 0, "右光箱回零按键按下!");
            Task task_home = new Task(() =>
            {
                try
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_RESET);
                    Enablebtn(btn_all_home, false);
                    Enablebtn(sender, false);
                    VAR.gsys_set.bquit = false;
                    EM_RES res = COM.RightLightBox.Home(ref VAR.gsys_set.bquit);
                    if(res==EM_RES.OK) DRpt.Report_Opration(1000, 0, "右光箱回零成功!");
                    else DRpt.Report_Opration(1000, 0, "右光箱回零失败!");
                    ShowMessage(VAR.IsChinese?string.Format("【{0}】 复位 {1}！", COM.RightLightBox.disc, res == EM_RES.OK ? "成功" : "失败"): string.Format("【{0}】 reset {1}！\r\n【{2}】 复位 {3}！", COM.RightLightBox.english_disc, res == EM_RES.OK ? "successful" : "failed", COM.RightLightBox.disc, res == EM_RES.OK ? "成功" : "失败"), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK);
                }
                finally
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                    Enablebtn(sender, true);
                    Enablebtn(btn_all_home, true);
                }
            }
            );
            task_home.Start();
            await task_home;
        }
        private void btn_lightbox_right_stop_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            DRpt.Report_Opration(1000, 0, "右光箱停止按键按下!");
            MT.AxisHomeQuit(MT.AxList_BOX_RIGHT);
        }

        private void btn__all_stop_Click(object sender, EventArgs e)
        {
            DRpt.Report_Opration(1000, 0, "停止按键按下!");
            COM.Stop();
        }

        private async void btn_all_home_Click(object sender, EventArgs e)
        {
            EM_RES res_chk = EM_RES.OK;
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (COM.bhomeing) return;
          
             COM.bhomeing = true;
            //if (DialogResult.OK != MessageBox.Show("确定要系统复位 ？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)) return;
            MT.DoorAlarmMsg = string.Empty;
            MT.DoorAlarmMsgTemp = string.Empty;
            res_chk = MT.ChkAllSafeSen();
            if (res_chk != EM_RES.OK) return;
            DRpt.Report_Opration(1000, 0, "整体回零按键按下!");
            Task task_home = new Task(() =>
            {
                try
                {
                    COM.bhomeing = true;
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_RESET);
                    if (PT_SET.LbEn)
                    {
                        Enablebtn(btn_lightbox_left_home, false);
                        Enablebtn(btn_lightbox_right_home, false);
                    }
                    Enablebtn(btn_zt_home, false);
                    Enablebtn(btn_ul_dl_home, false);
                    if (PT_SET.LbEn)
                        Enablebtn(btn_lightbox_left_home, false);
                    Enablebtn(sender, false);
                    EM_RES res = COM.Home();
                    if (res == EM_RES.OK)
                    {
                        bool bqtemp = false;
                        foreach (WS wstemp in COM.list_ws)
                        {
                            wstemp.PowerOff(ref bqtemp);
                        }
                        DRpt.Report_Opration(1000, 0, "整体回零成功!");
                        for (int t = 1; t < 37; t++)
                        {
                            Msg.secsManager.Send(new BaseInfo() { Id = t, Value = "false" }, 1);
                        }
                        Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                        Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);

                    }
                    else DRpt.Report_Opration(1000, 0, "整体回零失败!");
                    ShowMessage(VAR.IsChinese?string.Format("【{0}】 复位 {1}", "系统", res == EM_RES.OK ? "成功" : "失败"): string.Format("【{0}】 reset {1} \r\n【{2}】 复位 {3}", "system", res == EM_RES.OK ? "Successful" : "Failed", "系统", res == EM_RES.OK ? "成功" : "失败"), VAR.IsChinese ? "提示" : "prompt", MessageBoxButtons.OK);
                }
                finally
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                    if (PT_SET.LbEn)
                    {
                        Enablebtn(btn_lightbox_left_home, true);
                        Enablebtn(btn_lightbox_right_home, true);
                    }
                    Enablebtn(btn_zt_home, true);
                    Enablebtn(btn_ul_dl_home, true);
                    Enablebtn(sender, true);
                    COM.bhomeing = false;
                }
            }
            );
            task_home.Start();
            await task_home;
        }

        private async void btn_zt_home_Click(object sender, EventArgs e)
        {
            EM_RES res;
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (DialogResult.OK != MessageBox.Show(VAR.IsChinese?"确定要复位 【工位/转台】？": "Are you sure you want to reset 【WS/Turn table】?\r\n确定要复位 【工位/转台】？", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)) return;
            res = MT.ChkAllSafeSen(0x02);
            if (res != EM_RES.OK) return;
            DRpt.Report_Opration(1000, 0, "转台/工位/OTP回零按键按下!");
            //确保上料Y/下料Y位置
            if (COM.UDLoad1.ax_x.home_status != AXIS.HOME_STA.OK||COM.UDLoad1.ax_y.home_status!=AXIS.HOME_STA.OK|| !MT.AXIS_UDL1_X.isORG || MT.AXIS_UDL1_Y.fenc_pos > 700)
            {
                MessageBox.Show(VAR.IsChinese?"上料模块[1]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！": "Upload 1 X-axis is not at the origin or Y-axis position> 700, there may be interference, it is forbidden to turn the table back to zero!\r\n上料模块[1]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (COM.UDLoad2.ax_x.home_status != AXIS.HOME_STA.OK || COM.UDLoad2.ax_y.home_status != AXIS.HOME_STA.OK || !MT.AXIS_UDL2_X.isORG || MT.AXIS_UDL2_Y.fenc_pos > 700)
            {
                MessageBox.Show(VAR.IsChinese ? "上料模块[2]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！": "Upload 2 X-axis is not at the origin or Y-axis position> 700, there may be interference, it is forbidden to turn the table back to zero!\r\n上料模块[2]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task task_home = new Task(() =>
            {
                try
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_RESET);
                    //转台不在位置 / 工位不在位置 / OPT不在原点 禁止复位
                    Turntable.EM_STA sta = Turntable.GetCurSta;
                    if ((sta < Turntable.EM_STA.POS0 || sta > Turntable.EM_STA.POS3) && !MT.AXIS_BOX_OTP_Z.isORG)
                    {
                        MessageBox.Show(VAR.IsChinese?"转台不在位置，且OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位转台或OTP轴！": "Turn table is not in position and the OTP axis is not at the origin, there is a risk of collision! \r\n Please check to make sure there is no risk, and then manually reset the turn table or OTP axis!\r\n转台不在位置，且OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位转台或OTP轴！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //OTP光源复位(转台不在位置/工位不在位置禁止复位)
                    if ((sta >= Turntable.EM_STA.POS0 && sta <= Turntable.EM_STA.POS3))
                    {
                        WS ws = Turntable.GetWSOnPos(Turntable.EM_STA.POS2);
                        if (ws == null && !ws.isInTestPos)
                        {
                            MessageBox.Show(VAR.IsChinese ? "工站不在测试位置，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴/或对应工站！": "Workstation is not in the test position, there is a risk of collision! \r\n Please check to make sure there is no risk, and then manually reset the OTP axis / or the corresponding station!\r\n工站不在测试位置，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴/或对应工站！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (EM_RES.OK != COM.OTPLightBox.Home(ref VAR.gsys_set.bquit))
                        {
                            MessageBox.Show(VAR.IsChinese ? "转台/工位 回零失败!": "Turn table / Workstation return to zero failed!");
                            return;
                        }
                    }

                    //转台复位
                    if ((sta < Turntable.EM_STA.POS0 || sta > Turntable.EM_STA.POS3) && !MT.AXIS_BOX_OTP_Z.isORG)
                    {
                        MessageBox.Show(VAR.IsChinese ? "OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴！": "OTP axis is not at the origin, there is a risk of collision! \r\n Please check to make sure there is no risk, and then manually reset the OTP axis!\r\nOTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    MT.DoorAlarmMsg = string.Empty;
                    MT.DoorAlarmMsgTemp = string.Empty;
                    if (EM_RES.OK == COM.TurntableHome2(ref VAR.gsys_set.bquit))
                    {
                        //foreach (WS ws in COM.list_ws)
                        //{
                        //    ws.Status = WS.EM_STA.REDAY;

                        //    ws.TestStatus = WS.EM_TEST_STA.UNTEST;
                        //    foreach (WS.MdDat md in ws.list_md) md.res = -1;
                        //}

                        VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "就绪":"Ready", 0, true);
                        DRpt.Report_Opration(1000, 0, "转台/工位/OTP回零成功!");
                        MessageBox.Show(VAR.IsChinese?"转台/工位/OTP 回零成功!": "Turn table / Workstation / OTP return to zero successfully!");
                    }
                    else
                    {
                        DRpt.Report_Opration(1000, 0, "转台/工位/OTP回零失败!");
                        MessageBox.Show(VAR.IsChinese ? "转台/工位/OTP 回零失败!" : "Turn table / Workstation / OTP return to zero failed!");
                    }
                }
                finally
                {
                    MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                }
            });
            task_home.Start();
            await task_home;

            ////复位工站

            //foreach (WS ws in COM.list_ws)
            //{
            //    res = ws.NextTest(-1);
            //    if (res != EM_RES.OK)
            //    {
            //        ws.TestStatus = WS.EM_TEST_STA.ERROR;
            //        break;
            //    }
            //    //Thread.Sleep(1000);
            //    ////复位当前测试  
            //    //res = NextTest(0);
            //    //if (res != EM_RES.OK)
            //    //{
            //    //    TestStatus = EM_TEST_STA.ERROR;
            //    //    break;
            //    //}
            //    int _sta = 0;
            //    ws.WaitTestResult(ref _sta, 300);
            //    foreach (WS.MdDat md in ws.list_md)
            //    {
            //        if (md.res >= 0)
            //        {
            //            md.res = -1;
            //            ws.Status = WS.EM_STA.REDAY;
            //            ws.TestStatus = WS.EM_TEST_STA.UNTEST;
            //        }
                        
            //    }
               
            //    //foreach (WS.MdDat md in ws.list_md) md.res = -1;
            //}
        }

        private void btn_zt_stop_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            VAR.gsys_set.bquit = true;
            DRpt.Report_Opration(1000, 0, "转台/工位/OTP停止按键按下!");
            foreach (WS ws in COM.list_ws)
            {
                ws.ax_u.Stop();
                //ws.ax_fr.Stop();
                //ws.ax_bk.Stop();
            }
        }
    }
}
