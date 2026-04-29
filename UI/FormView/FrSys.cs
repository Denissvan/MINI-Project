using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MotionCtrl;
using System.Reflection;
using DevReport;
using Win32Lib;
using System.IO;
using UI.Class;
using Sunny.UI;
using Newtonsoft.Json;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using IniFile = MotionCtrl.IniFile;
using Newtonsoft.Json.Linq;
using static MotionCtrl.AXIS;
namespace UI
{
    public partial class FrSys : Form
    {
        public bool bupdate
        {
            get { return tmr_update.Enabled; }
            set
            {
                tmr_update.Enabled = value;
                if (value == false) ioTable.ShowCfg(-1);
            }
        }
        private bool isAPointSelected = false;
        private bool isBPointSelected = false;
        private bool isCPointSelected = false;
        public FrSys()
        {
            InitializeComponent();

   

            if (FrMain.frsuser != null)
                PmsfrSys(FrMain.frsuser.user1.cur_user.pms);
            else
                PmsfrSys(User.PERMISSION.Operator);
        }

        public void bEn(bool en = true)
        {
            axisConfig.Enabled = en;
            btn_update_card.Enabled = en;
            btn_load_axis_cfg.Enabled = en;
            btn_save_axis_cfg.Enabled = en;
            rbtn_upcode.Enabled = en;
            rbtn_dwcode.Enabled = en;
            rbtn_NoCap.Enabled = en;
            //运行模式
            rbtn_twomd.Enabled = en;
            rbtn_md1.Enabled = en;
            rbtn_md2.Enabled = en;
            //运行方式
            rbtn_run_normal.Enabled = en;
            rbtn_run_updw.Enabled = en;
            rbtn_run_empty.Enabled = en;
            btn_saveparm2.Enabled = en;
            //门禁
            //chk_leftlightbox.Enabled = en;
            //chk_rightlightbox.Enabled = en;
            //chk_tray.Enabled = en;
            //chk_updown.Enabled = en;
            chk_leftlightbox.Enabled = false;
            chk_rightlightbox.Enabled = false;
            chk_tray.Enabled = false;
            chk_updown.Enabled = false;
            //视觉回检
            Chk_OpenVs.Enabled = en;
            nud_vschk_xyofs.Enabled = en;
            nud_vschk_rofs.Enabled = en;
            btn_ptset_save.Enabled = en;
            //料盘放料检测
            rbtn_OpenVsTray.Enabled = en;
            rbtn_CloseVsTray.Enabled = en;
            //上下料移动保护(针对转盘)
            rbtn_safeoff.Enabled = en;
            rbtn_safeon.Enabled = en;
            //GRR流程
            chk_grr.Enabled = en;
            nud_grrtestcnt.Enabled = en;
            nud_grrtudlcnt.Enabled = en;
            //相机存储图片
            chk_upcam1.Enabled = en;
            chk_dwcam1.Enabled = en;
            chk_upcam2.Enabled = en;
            chk_dwcam2.Enabled = en;
            //模组带起
            Chk_ModPasteUp.Enabled = en;
            nud_PlaceDly.Enabled = en;
            nud_MovUpDly.Enabled = en;
            //Y1设置
            rbtn_Y1en.Enabled = en;
            rbtn_Y1dis.Enabled = en;
            //右光箱Y1设置
            rbtn_RY1en.Enabled = en;
            rbtn_RY1dis.Enabled = en;
            //光箱设置
            rbtn_lben.Enabled = en;
            rbtn_lbdis.Enabled = en;
            //开图顺序设置
            rbtn_pre_turnon.Enabled = en;
            rbtn_next_turnon.Enabled = en;
            nud_OpenDly.Enabled = en;
            //料盘二维码设置
            rbtn_traybaren.Enabled = en;
            rbtn_traybardis.Enabled = en;
            //模组大小设置
            rbtn_small.Enabled = en;
            rbtn_big.Enabled = en;
            //模组单双开设置
            rbtn_single.Enabled = en;
            rbtn_double.Enabled = en;
            rbtn_Sml.Enabled = en;
            rbtn_Lag.Enabled = en;
            //吸头1先运动设置
            rbtn_xt1firstdis.Enabled = en;
            rbtn_xt1firsten.Enabled = en;
            //霍尔测试设置
            rbtn_hallen.Enabled = en;
            rbtn_halldis.Enabled = en;
            chk_downfliptest.Enabled = en;
            //取放料角度开放设置
            rbtn_open_degree.Enabled = en;
            rbtn_close_degree.Enabled = en;
            nud_offset_degree.Enabled = en;
            //二维码回检设备
            rbtn_OpenBarCamBack.Enabled = en;
            rbtn_CloseBarCamBack.Enabled = en;
            //OK品下料检测
            rbtn_OkCheckEn.Enabled = en;
            rbtn_OkCheckDis.Enabled = en;
            //工站视觉模板增加有无检测
            rbtn_OpenWsVsAddCheck.Enabled = en;
            rbtn_CloseWsVsAddCheck.Enabled = en;
            //吸头偏差
            nud_dwCam_xt1_ofs.Enabled = en;
            nud_dwCam_xt2_ofs.Enabled = en;
            nud_dwCam_xt3_ofs.Enabled = en;
            nud_dwCam_xt4_ofs.Enabled = en;
            //高透模偏移检测
            chk_GTMCheck.Enabled = en;
            nud_GTMOfs.Enabled = en;
            //同工位连续同NG提示
            chk_SameNGTip.Enabled = en;
            nud_SameNGTipCnt.Enabled = en;
            //同工位连续同NG提示
            chk_SameRowNGTip.Enabled = en;
            nud_SameRowNGTipCnt.Enabled = en;
            //保养
            nud_FixtrueMaintain.Enabled = en;
            nud_EquipmentMaintain.Enabled = en;
            //工站二次取料
            rbtn_OpenWsPickAgain.Enabled = en;
            rbtn_CloseWsPickAgain.Enabled = en;
            //数据上传设置
            chk_UpLoadData.Enabled = en;
            chk_UpdateSoft.Enabled = en;
            //回检继续运行设置
            rbtn_backerrcontinueen.Enabled = en;
            rbtn_backerrcontinuedis.Enabled = en;
            //夹具工位非水平设置
            rbtn_nonparallelen.Enabled = en;
            rbtn_nonparalleldis.Enabled = en;
            //清洗时间间隔设置
            rbtn_cleandis.Enabled = en;
            rbtn_cleanen.Enabled = en;
            nud_cleaninterval.Enabled = en;
            //点检时间
            dateTimePicker1.Enabled = en;
            dateTimePicker2.Enabled = en;
            //上下料周期保存设置
            rbtn_CycleEn.Enabled = en;
            rbtn_CycleDis.Enabled = en;
            //降温设置
            rbtn_coolen.Enabled = en;
            rbtn_cooldis.Enabled = en;
            //DCC写入移至上下料设置
            rbtn_delaytesten.Enabled = en;
            rbtn_delaytestdis.Enabled = en;
            //NG管控设置
            rbtn_ngcontroldis.Enabled = en;
            rbtn_ngcontrolen.Enabled = en;
            nud_ngcode.Enabled = en;
            nud_ngscale.Enabled = en;
            //持续报警设置
            rbtn_ContinuousAlarmEn.Enabled = en;
            rbtn_ContinuousAlarmDis.Enabled = en;
            ////数据上传设置
            rbtn_NgWarningEn.Enabled = en;
            rbtn_NgWarningDis.Enabled = en;
            nud_okRate.Enabled = en;
            //Mes模式
            rbtn_MesLocal.Enabled = en;
            rbtn_MesRemote.Enabled = en;
            //夹具扫码
            btnStartJigSan1.Enabled = en;
            btnStartJigSan2.Enabled = en;
            btnStartJigSan3.Enabled = en;
            btnStartJigSan4.Enabled = en;
            rabt_jigscan_ON.Enabled = en;
            rabt_jigscan_OFF.Enabled = en;
            nud_JigSendTime.Enabled = en;
            //测试时间
            nud_TestTime.Enabled = en;
            //新增拍二维码位置
            rbtn_AddCapQrcodeEn.Enabled = en;
            rbtn_DwAddCapQrcodeEn.Enabled = en;
            rbtn_DwAddCapQrcodeOff.Enabled = en;
            rbtn_AddCapQrcodeDis.Enabled = en;
            //光源选择
            rad_G4C.Enabled = en;
            rad_light_xsj.Enabled = en;
        }

        public void PmsfrSys(User.PERMISSION pms)
        {
            npointCail1.PmsEn(pms);
            affineCail_Dw1ToUp1.PmsEn(pms);
            affineCail_Dw2ToUp2.PmsEn(pms);
            rotAndFly1.PmsEn(pms);
            CogRecordDisplay_sys.PmsEn((pms > User.PERMISSION.Engineer) ? true : false);
            bEn((pms < User.PERMISSION.Engineer) ? false : true);


        }

        private void FrSys_Load(object sender, EventArgs e)
        {

            //如果不在设计模式才执行这块代码
            //加载卡列表CardTable
            FieldInfo[] pArray = typeof(MT).GetFields();
            CARD card;
            foreach (FieldInfo p in pArray)
            {
                if (p.FieldType.Name == "CARD")
                {
                    card = ((CARD)p.GetValue(typeof(MT)));
                    if (!PT_SET.LbEn && (card.id == 1 || card.id == 2))
                    {
                        ;
                    }
                    else
                    {
                        CardTable.AddCard(card);
                    }
                }

                if (p.FieldType.Name == "GPIO")
                    ioTable.AddIO(((GPIO)p.GetValue(typeof(MT))));

                if (p.FieldType.Name == "Cylinder")
                    cylinderTable.AddCylinder(((Cylinder)p.GetValue(typeof(MT))));
            }
            ioTable.ShowCfg(0);

            //轴列表
            ctb_ax_sel_SelectedIndexChanged(ctb_ax_sel, null);
            CogRecordDisplay_sys.list_cam.Clear();
            CogRecordDisplay_sys.AddCam(COM.ListCam);
            npointCail1.AddUDLoad(COM.List_UDLoad);
            affineCail_Dw1ToUp1.Init(ref COM.UDLoad1);
            affineCail_Dw2ToUp2.Init(ref COM.UDLoad2);
            rotAndFly1.AddUDLoad(COM.List_UDLoad);
            taskNpoint1.AddCam(COM.ListCam);
            NewSysInf.LoadSysInfCfg(out var msg);
            ShowData();


            // rotAndFly1.AddXt(COM.ListXT);
        }

        private void tmr_update_Tick(object sender, EventArgs e)
        {
            int t;
            tmr_update.Enabled = false;
            switch (ctb_sys.SelectedIndex)
            {
                case 0:
                    t = Environment.TickCount;
                    CardTable.UpdateShow();
                    t = Environment.TickCount - t;
                    lb_card_update_ms.Text = string.Format("{0}ms", t);
                    break;
                case 1:
                    t = Environment.TickCount;
                    axisTable.UpdateShow();
                    t = Environment.TickCount - t;
                    lb_ax_update_ms.Text = string.Format("{0}ms", t);
                    break;
                case 2:
                    if (ioTable.Visible) ioTable.UpdateShow();
                    if (cylinderTable.Visible) cylinderTable.UpdateShow();
                    lb_io_update_ms.Text = string.Format("{0}ms", ioTable.UpdateCt);
                    break;
            }
            //CardTable.UpdateShow();
            CardTable.ChangeColumn();
            axisTable.ChangeColumn();
            axisConfig.ChangeColumn();
            ioTable.ChangeColumn();
            cylinderTable.ChangeColumn();
            tmr_update.Interval = 500;
            tmr_update.Enabled = true;
        }

        private void btn_load_axis_cfg_Click(object sender, EventArgs e)
        {
            axisConfig.LoadFrFile();
            MessageBox.Show(VAR.IsChinese ? "轴列表参数加载完成!" : "The axis list parameters are loaded!\r\n轴列表参数加载完成!");
        }

        private void btn_save_axis_cfg_Click(object sender, EventArgs e)
        {
            if (true == axisConfig.SaveToFile())
            {
                MessageBox.Show(VAR.IsChinese ? "轴列表参数保存完成!" : "The axis list parameters are saved!\r\n轴列表参数保存完成!");
            }
        }

        private void ctb_ax_sel_SelectedIndexChanged(object sender, EventArgs e)
        {
            axisTable.ClearAxis();
            axisConfig.ClearAxis();
            axisTable.UpdateShow();
            axisConfig.UpdateShow();
            switch (((CTabControl)sender).SelectedIndex)
            {
                case 0:
                    axisTable.AddAxis(MT.Axlist_UDL1);
                    axisConfig.AddAxis(MT.Axlist_UDL1_ExpLC);
                    break;
                case 1:
                    axisTable.AddAxis(MT.Axlist_UDL2);
                    axisConfig.AddAxis(MT.Axlist_UDL2_ExpLC);
                    break;
                case 2:
                    axisTable.AddAxis(MT.GetAxlistUdlLcForCurrentMode());
                    axisConfig.AddAxis(MT.GetAxlistUdlLcForCurrentMode());
                    break;
                case 3:
                    axisTable.AddAxis(MT.AxList_BOX_LEFT);
                    axisConfig.AddAxis(MT.AxList_BOX_LEFT);
                    break;
                case 4:
                    axisTable.AddAxis(MT.AxList_BOX_RIGHT);
                    axisConfig.AddAxis(MT.AxList_BOX_RIGHT);

                    break;
                case 5:
                    axisTable.AddAxis(MT.AxList_BOX_OPT);
                    axisConfig.AddAxis(MT.AxList_BOX_OPT);
                    break;
                case 6:
                    axisTable.AddAxis(MT.AxList_WS1);
                    axisConfig.AddAxis(MT.AxList_WS1);
                    axisTable.AddAxis(MT.AxList_WS2);
                    axisConfig.AddAxis(MT.AxList_WS2);
                    axisTable.AddAxis(MT.AxList_WS3);
                    axisConfig.AddAxis(MT.AxList_WS3);
                    axisTable.AddAxis(MT.AxList_WS4);
                    axisConfig.AddAxis(MT.AxList_WS4);
                    break;
                case 7:
                    axisTable.AddAxis(MT.Axlist_UDL1_ExpLC);
                    axisTable.AddAxis(MT.Axlist_UDL2_ExpLC);
                    axisTable.AddAxis(MT.Axlist_UDL_LC);
                    axisTable.AddAxis(MT.AxList_BOX_LEFT);
                    axisTable.AddAxis(MT.AxList_BOX_RIGHT);
                    axisTable.AddAxis(MT.AxList_BOX_OPT);
                    axisTable.AddAxis(MT.AxList_WS);

                    axisConfig.AddAxis(MT.Axlist_UDL1_ExpLC);
                    axisConfig.AddAxis(MT.Axlist_UDL2_ExpLC);
                    axisConfig.AddAxis(MT.Axlist_UDL_LC);
                    axisConfig.AddAxis(MT.AxList_BOX_LEFT);
                    axisConfig.AddAxis(MT.AxList_BOX_RIGHT);
                    axisConfig.AddAxis(MT.AxList_BOX_OPT);
                    axisConfig.AddAxis(MT.AxList_WS);
                    break;
            }
            bupdate = true;
        }

        private void ctb_sys_SelectedIndexChanged(object sender, EventArgs e)
        {
            bupdate = true;
            CogRecordDisplay_sys.list_cam.Clear();
            CogRecordDisplay_sys.AddCam(COM.ListCam);
            if (((CTabControl)sender).SelectedIndex != 2)
                ioTable.ShowCfg(-1);
            if ((((CTabControl)sender).SelectedTab.Name) == "tp_sysparm") ShowData();
            if (ctb_sys.SelectedIndex > 2)
            {
                var allline = File.ReadAllLines(System.Environment.CurrentDirectory + "\\vInfo.ini");
                textBoVinfo.Clear();
                foreach (var info in allline)
                    textBoVinfo.AppendText(info + "\r\n");
            }

        }

        private void btn_update_card_Click(object sender, EventArgs e)
        {
            string str = string.Empty;
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = MT.CardList;
            }
            else
            {
                TempCardList = MT.CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                if (card.isReady)
                {
                    if (EM_RES.OK == card.DownLoadFile()) str = VAR.IsChinese ? (str + String.Format("{0}/id{1},更新配置成功！\r\n", card.disc, card.id)) : (str + String.Format("{0}/id{1},Update configuration succeeded! \r\n         ({0}/id{1},更新配置成功！\r\n)", card.disc, card.id));
                    else str = VAR.IsChinese ? (str + String.Format("{0}/id{1},更新配置失败！\r\n", card.disc, card.id)) : (str + String.Format("{0}/id{1},Update configuration failed! \r\n         ({0}/id{1},更新配置失败！\r\n)", card.disc, card.id));
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? String.Format("{0}/id{1},未初始化！", card.disc, card.id) : String.Format("{0}/id{1},Uninitialized!        ({0}/id{1},未初始化！)", card.disc, card.id), DReport.EmErrCode.ToolError, (int)DReport.EmHareware.Card);
                    str = VAR.IsChinese ? (str + String.Format("{0}/id{1},未初始化！\r\n", card.disc, card.id)) : (str + String.Format("{0}/id{1},Uninitialized!            ({0}/id{1},未初始化！\r\n", card.disc, card.id));
                }
            }
            MessageBox.Show(str);
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, String.Format("stop"));
            VAR.gsys_set.bquit = true;
            MT.AllAxStop();
        }

        private void ctb_io_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((CTabControl)sender).SelectedTab.Name)
            {
                case "tp_in":
                    cylinderTable.Visible = false;
                    ioTable.Visible = true;
                    ioTable.Top = 60;
                    ioTable.Left = 30;
                    ioTable.Width = ctb_io_type.Width;
                    ioTable.Height = 780;
                    ioTable.ShowCfg(1);
                    break;
                case "tp_out":
                    cylinderTable.Visible = false;
                    ioTable.Visible = true;
                    ioTable.Top = 60;
                    ioTable.Left = 30;
                    ioTable.Width = ctb_io_type.Width;
                    ioTable.Height = 780;
                    ioTable.ShowCfg(0);
                    break;
                case "tp_cy":
                    ioTable.Visible = false;
                    cylinderTable.Visible = true;
                    cylinderTable.Top = 60;
                    cylinderTable.Left = 30;
                    cylinderTable.Width = ctb_io_type.Width;
                    cylinderTable.Height = 780;
                    break;
            }

        }

        public void ShowData()
        {
            dataGrideSysInfo1.ShowDataToGride();
            //光源选择
            rad_G4C.Checked = false;
            rad_light_xsj.Checked = false;
            rad_G4C.Checked = PT_SET.bG4C;
            rad_light_xsj.Checked = !PT_SET.bG4C;
            ckws1wait.Checked = COM.ws1.waitopen;
            ckws2wait.Checked = COM.ws2.waitopen;
            ckws3wait.Checked = COM.ws3.waitopen;
            ckws4wait.Checked = COM.ws4.waitopen;
            nbwaittime.Value = COM.ws1.waittime;

            checkmagic.Checked = PT_SET.Ismagic;
            nummagic.Value = PT_SET.magictimes;

            ckclosewait.Checked = PT_SET.CloseWait;
            nbclosewaittime.Value = PT_SET.CloseWaitTime;
            cbttpcloseair.Checked = PT_SET.otpclose;
            numcloseair.Value = (decimal)PT_SET.otpclosetime;

            cbafccloseair.Checked = PT_SET.afcclose;
            numafc.Value = (decimal)PT_SET.afcclosetime;
            rbtn_opencheckoven.Checked = PT_SET.OvenCheck;
            rbtn_closecheckoven.Checked = !PT_SET.OvenCheck;
            //二维码扫码方式
            rbtn_dwcode.Checked = false;
            rbtn_upcode.Checked = false;
            rbtn_NoCap.Checked = false;
            if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN) rbtn_upcode.Checked = true;
            else if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN) rbtn_dwcode.Checked = true;
            else rbtn_NoCap.Checked = true;

            if (PT_SET.Isaloneset)
            {
                cbisaloneset.Checked = true;
            }
            else
            {
                cbisaloneset.Checked = false;
            }
            //运行模式
            rbtn_md1.Checked = false;
            rbtn_md2.Checked = false;
            rbtn_twomd.Checked = false;
            if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.BOTH_WORK) rbtn_twomd.Checked = true;
            else if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.MD1_WORK) rbtn_md1.Checked = true;
            else if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.MD2_WORK) rbtn_md2.Checked = true;

            //运行方式
            rbtn_run_normal.Checked = false;
            rbtn_run_updw.Checked = false;
            rbtn_run_empty.Checked = false;
            if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL) rbtn_run_normal.Checked = true;
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_UPDW) rbtn_run_updw.Checked = true;
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_EMPTY) rbtn_run_empty.Checked = true;

            if (PT_SET.CloseWait)
            {
                ckclosewait.Checked = true;
            }
            else ckclosewait.Checked = false;
            nbclosewaittime.Value = PT_SET.CloseWaitTime;
            //下料前二维码检测
            if (PT_SET.OpenDownQrde)
            {
                if (PT_SET.UpDownQrde)
                {
                    rbtn_openupdownqrcode.Checked = true;
                    rbtn_undownqrcode.Checked = false;
                    rbtn_opendownqrcode.Checked = false;
                }
                else if (PT_SET.DownDownQrde)
                {
                    rbtn_openupdownqrcode.Checked = false;
                    rbtn_undownqrcode.Checked = false;
                    rbtn_opendownqrcode.Checked = true;
                }
            }
            else
            {
                rbtn_openupdownqrcode.Checked = false;
                rbtn_undownqrcode.Checked = true;
                rbtn_opendownqrcode.Checked = false;
            }

            //视觉回检
            Chk_OpenVs.Checked = PT_SET.bEnVsFB;
            nud_vschk_xyofs.Value = (decimal)PT_SET.Vs_XYofs;
            nud_vschk_rofs.Value = (decimal)PT_SET.Vs_Rofs;

            //料盘位置检测
            rbtn_OpenVsTray.Checked = false;
            rbtn_CloseVsTray.Checked = false;
            if (PT_SET.bEnVsTray) rbtn_OpenVsTray.Checked = true;
            else rbtn_CloseVsTray.Checked = true;

            //Y1轴开启设置
            rbtn_Y1dis.Checked = false;
            rbtn_Y1en.Checked = false;
            if (PT_SET.Y1En) rbtn_Y1en.Checked = true;
            else rbtn_Y1dis.Checked = true;

            //右光箱Y1轴开启设置
            rbtn_RY1dis.Checked = false;
            rbtn_RY1en.Checked = false;
            if (PT_SET.RY1En) rbtn_RY1en.Checked = true;
            else rbtn_RY1dis.Checked = true;
            safepos.Value = (decimal)PT_SET.safepos;
            //左右光箱开启设置
            rbtn_lbdis.Checked = false;
            rbtn_lben.Checked = false;
            if (PT_SET.LbEn) rbtn_lben.Checked = true;
            else rbtn_lbdis.Checked = true;

            //开图顺序设置
            rbtn_next_turnon.Checked = false;
            rbtn_pre_turnon.Checked = false;
            if (PT_SET.turnon) rbtn_pre_turnon.Checked = true;
            else rbtn_next_turnon.Checked = true;
            nud_OpenDly.Value = PT_SET.OpenDly;

            //料盘二维码拍照设置
            rbtn_traybaren.Checked = false;
            rbtn_traybardis.Checked = false;
            if (PT_SET.TrayBarcodeEn) rbtn_traybaren.Checked = true;
            else rbtn_traybardis.Checked = true;

            //吸头1先运动设置
            rbtn_xt1firstdis.Checked = false;
            rbtn_xt1firsten.Checked = false;
            if (PT_SET.xt1firsten) rbtn_xt1firsten.Checked = true;
            else rbtn_xt1firstdis.Checked = true;

            rbtUpDnAddTestOn.Checked = PT_SET.bUpDnAddTest;
            rbtUpDnAddTestOff.Checked = !PT_SET.bUpDnAddTest;

            //模组大小设置
            rbtn_big.Checked = false;
            rbtn_small.Checked = false;
            if (PT_SET.issmall) rbtn_small.Checked = true;
            else rbtn_big.Checked = true;

            //模式开启设置
            rbtn_double.Checked = false;
            rbtn_single.Checked = false;
            rbtn_Sml.Checked = false;
            rbtn_Lag.Checked = false;
            if (PT_SET.bitOpenMode == 1) rbtn_single.Checked = true;
            else if (PT_SET.bitOpenMode == 2) rbtn_double.Checked = true;
            else if (PT_SET.bitOpenMode == 3) rbtn_Lag.Checked = true;
            else if (PT_SET.bitOpenMode == 4) rbtn_Sml.Checked = true;
            //霍尔测试设置
            rbtn_hallen.Checked = false;
            rbtn_halldis.Checked = false;
            if (PT_SET.HallEn) rbtn_hallen.Checked = true;
            else rbtn_halldis.Checked = true;
            chk_downfliptest.Checked = PT_SET.bDownFlipTest;

            //开关取放料角度设置
            rbtn_open_degree.Checked = false;
            rbtn_close_degree.Checked = false;
            if (PT_SET.isopen_degree
                ) rbtn_open_degree.Checked = true;
            else rbtn_close_degree.Checked = true;
            nud_offset_degree.Value = PT_SET.degree;

            //开关工站下料取放料角度设置
            rbtn_opendown_degree.Checked = false;
            rbtn_closedown_degree.Checked = false;
            if (PT_SET.isopendown_degree) rbtn_opendown_degree.Checked = true;
            else rbtn_closedown_degree.Checked = true;
            nud_offsetdown_degree.Value = PT_SET.downdegree;

            //二维码回检设备
            rbtn_OpenBarCamBack.Checked = false;
            rbtn_CloseBarCamBack.Checked = false;
            if (PT_SET.bBarcodeCamBackEn) rbtn_OpenBarCamBack.Checked = true;
            else rbtn_CloseBarCamBack.Checked = true;

            rbtn_AddCapQrcodeEnOnly.Checked = false;
            rbtn_AddCapQrcodeEnOnlyClose.Checked = false;
            //单独进行二维码回检
            if (PT_SET.bBarcodeCamBackEnOnly) rbtn_AddCapQrcodeEnOnly.Checked = true;
            else rbtn_AddCapQrcodeEnOnlyClose.Checked = true;

            //OK品下料检测
            rbtn_OkCheckEn.Checked = false;
            rbtn_OkCheckDis.Checked = false;
            /*if (PT_SET.bOkCheck)*/
            rbtn_OkCheckEn.Checked = true;
            //else rbtn_OkCheckDis.Checked = true;

            if (PT_SET.bboxCheck) rbtn_boxCheckEn.Checked = true;
            else rbtn_boxCheckOff.Checked = true;
            boxCheckpos1X.Value = (decimal)PT_SET.boxpos1.x;
            boxCheckpos1Y.Value = (decimal)PT_SET.boxpos1.y;
            boxCheckpos2X.Value = (decimal)PT_SET.boxpos2.x;
            boxCheckpos2Y.Value = (decimal)PT_SET.boxpos2.y;
            boxCheckpos3X.Value = (decimal)PT_SET.boxpos3.x;
            boxCheckpos3Y.Value = (decimal)PT_SET.boxpos3.y;
            boxCheckpos4X.Value = (decimal)PT_SET.boxpos4.x;
            boxCheckpos4Y.Value = (decimal)PT_SET.boxpos4.y;
            boxChecksetvalue.Value = (decimal)PT_SET.boxsetpos;
            boxChecksetpos1.Value = (decimal)PT_SET.boxsetpos1;
            boxChecksetpos2.Value = (decimal)PT_SET.boxsetpos2;
            boxChecksetpos3.Value = (decimal)PT_SET.boxsetpos3;
            boxChecksetpos4.Value = (decimal)PT_SET.boxsetpos4;


            ////吸头屏蔽
            //rbtXt11EnableOff.Checked = false;
            //rbtXt11EnableOn.Checked = false;
            //rbtXt12EnableOff.Checked = false;
            //rbtXt12EnableOn.Checked = false;
            //rbtXt21EnableOff.Checked = false;
            //rbtXt21EnableOn.Checked = false;
            //rbtXt22EnableOff.Checked = false;
            //rbtXt22EnableOn.Checked = false;

            //_=COM.xt1.Enable? rbtXt11EnableOn.Checked = true:rbtXt11EnableOff.Checked = true;
            //_ = COM.xt2.Enable ? rbtXt12EnableOn.Checked = true : rbtXt12EnableOff.Checked = true;
            //_ = COM.xt3.Enable ? rbtXt21EnableOn.Checked = true : rbtXt21EnableOff.Checked = true;
            //_ = COM.xt4.Enable ? rbtXt22EnableOn.Checked = true : rbtXt22EnableOff.Checked = true;
            rbtUpDn1OneXtOff.Checked = false;
            rbtUpDn1OneXtOn.Checked = false;
            rbtUpDn2OneXtOff.Checked = false;
            rbtUpDn2OneXtOn.Checked = false;
            if (PT_SET.bUpDn1XtOnlyOne) rbtUpDn1OneXtOn.Checked = true; else rbtUpDn1OneXtOff.Checked = true;
            if (PT_SET.bUpDn2XtOnlyOne) rbtUpDn2OneXtOn.Checked = true; else rbtUpDn2OneXtOff.Checked = true;

            //工站视觉模板增加有无检测
            rbtn_OpenWsVsAddCheck.Checked = false;
            rbtn_CloseWsVsAddCheck.Checked = false;
            if (PT_SET.bWsVsAddCheckEn) rbtn_OpenWsVsAddCheck.Checked = true;
            else rbtn_CloseWsVsAddCheck.Checked = true;

            //上下料移动保护(针对转盘)
            rbtn_safeoff.Checked = false;
            rbtn_safeon.Checked = false;
            if (PT_SET.bUdMovSafe) rbtn_safeon.Checked = true;
            else rbtn_safeoff.Checked = true;

            //门禁
            chk_leftlightbox.Checked = PT_SET.bEnLBoxDr;
            chk_rightlightbox.Checked = PT_SET.bEnRBoxDr;
            chk_tray.Checked = PT_SET.bEnTrayDr;
            chk_updown.Checked = PT_SET.bEnUpDownDr;

            //GRR流程
            chk_grr.Checked = PT_SET.bGrrFlow;
            nud_grrtestcnt.Value = PT_SET.GRRTestCnt;
            nud_grrtudlcnt.Value = PT_SET.GRRUdlCnt;

            //相机存储图片
            chk_upcam1.Checked = COM.CamUp1.bSaveImage;
            chk_dwcam1.Checked = COM.CamDw1.bSaveImage;
            chk_upcam2.Checked = COM.CamUp2.bSaveImage;
            chk_dwcam2.Checked = COM.CamDw2.bSaveImage;

            //吸头偏差
            nud_dwCam_xt1_ofs.Value = (decimal)COM.xt1.cap_offset;
            nud_dwCam_xt2_ofs.Value = (decimal)COM.xt2.cap_offset;
            nud_dwCam_xt3_ofs.Value = (decimal)COM.xt3.cap_offset;
            nud_dwCam_xt4_ofs.Value = (decimal)COM.xt4.cap_offset;

            nud_dwCam_xt1_Qrofs.Value = (decimal)COM.xt1.DwCapQrCodeoffset;
            nud_dwCam_xt2_Qrofs.Value = (decimal)COM.xt2.DwCapQrCodeoffset;
            nud_dwCam_xt3_Qrofs.Value = (decimal)COM.xt3.DwCapQrCodeoffset;
            nud_dwCam_xt4_Qrofs.Value = (decimal)COM.xt4.DwCapQrCodeoffset;

            //模组带起
            Chk_ModPasteUp.Checked = PT_SET.bModPasteUp;
            nud_PlaceDly.Value = PT_SET.PlaceDly;
            nud_MovUpDly.Value = PT_SET.MovUpDly;

            //高透模偏移检测
            chk_GTMCheck.Checked = PT_SET.bGTMCheck;
            nud_GTMOfs.Value = (decimal)PT_SET.GTMOfs;
            //同工位连续同NG提示
            chk_SameNGTip.Checked = PT_SET.bSameNGTip;
            PT_SET.SameNGTipCnt = Math.Max((int)nud_SameNGTipCnt.Minimum, Math.Min((int)nud_SameNGTipCnt.Maximum, PT_SET.SameNGTipCnt));
            nud_SameNGTipCnt.Value = (decimal)PT_SET.SameNGTipCnt;
            //同工位连续同NG提示
            chk_SameRowNGTip.Checked = PT_SET.bSameRowNGTip;
            nud_SameRowNGTipCnt.Value = (decimal)PT_SET.SameRowNGTipCnt;
            //保养
            nud_EquipmentMaintain.Value = PT_SET.EquipmentMT;
            nud_FixtrueMaintain.Value = PT_SET.FixtrueMT;
            //数据上传设置
            chk_UpLoadData.Checked = PT_SET.bUploadData;
            chk_UpdateSoft.Checked = PT_SET.bUpdateSoft;
            //工站二次取料
            rbtn_OpenWsPickAgain.Checked = false;
            rbtn_CloseWsPickAgain.Checked = false;
            if (PT_SET.bWsPickAgain) rbtn_OpenWsPickAgain.Checked = true;
            else rbtn_CloseWsPickAgain.Checked = true;


            //回检失败继续运行设置
            rbtn_backerrcontinueen.Checked = false;
            rbtn_backerrcontinuedis.Checked = false;
            if (PT_SET.bBackerrcontinue) rbtn_backerrcontinueen.Checked = true;
            else rbtn_backerrcontinuedis.Checked = true;
            //夹具双工位非水平设置
            rbtn_nonparallelen.Checked = false;
            rbtn_nonparalleldis.Checked = false;
            if (PT_SET.bNonparallel) rbtn_nonparallelen.Checked = true;
            else rbtn_nonparalleldis.Checked = true;
            //夹具清洗设置
            rbtn_cleanen.Checked = false;
            rbtn_cleandis.Checked = false;
            if (PT_SET.bCleanen) rbtn_cleanen.Checked = true;
            else rbtn_cleandis.Checked = true;
            nud_cleaninterval.Value = (decimal)PT_SET.Cleaninterval;

            //相机参数设置
            rbtn_camcfgsetdis.Checked = false;
            rbtn_camcfgseten.Checked = false;
            if (PT_SET.BCamcfgset) rbtn_camcfgseten.Checked = true;
            else rbtn_camcfgsetdis.Checked = true;
            nud_tray_exposure1.Value = (decimal)PT_SET.TrayExposure[0];
            nud_tray_exposure2.Value = (decimal)PT_SET.TrayExposure[1];
            nud_tray_brightness1.Value = (decimal)PT_SET.TrayBrightness[0];
            nud_tray_brightness2.Value = (decimal)PT_SET.TrayBrightness[1];
            nud_tray_contrast1.Value = (decimal)PT_SET.TrayContrast[0];
            nud_tray_contrast2.Value = (decimal)PT_SET.TrayContrast[1];

            nud_ws_exposure1.Value = (decimal)PT_SET.WsExposure[0];
            nud_ws_exposure2.Value = (decimal)PT_SET.WsExposure[1];

            nud_ws_brightness1.Value = (decimal)PT_SET.WsBrightness[0];
            nud_ws_brightness2.Value = (decimal)PT_SET.WsBrightness[1];

            nud_ws_contrast1.Value = (decimal)PT_SET.WsContrast[0];
            nud_ws_contrast2.Value = (decimal)PT_SET.WsContrast[1];
            nud_jig_exposure1.Value = (decimal)PT_SET.JigExposure[0];
            nud_jig_exposure2.Value = (decimal)PT_SET.JigExposure[1];
            nud_jig_brightness1.Value = (decimal)PT_SET.JigBrightness[0];
            nud_jig_brightness2.Value = (decimal)PT_SET.JigBrightness[1];
            nud_jig_contrast1.Value = (decimal)PT_SET.JigContrast[0];
            nud_jig_contrast2.Value = (decimal)PT_SET.JigContrast[1];


            //点检时间设置
            dateTimePicker1.Value = Convert.ToDateTime(PT_SET.CheckTimeMorning);
            dateTimePicker2.Value = Convert.ToDateTime(PT_SET.CheckTimeEvening);
            //执行点检时间设置
            dateTimePicker3.Value = Convert.ToDateTime(PT_SET.CheckTimeMorning1);
            dateTimePicker4.Value = Convert.ToDateTime(PT_SET.CheckTimeEvening2);
            //上下料周期保存设置
            rbtn_CycleEn.Checked = false;
            rbtn_CycleDis.Checked = false;
            if (PT_SET.bCycle) rbtn_CycleEn.Checked = true;
            else rbtn_CycleDis.Checked = true;

            //降温设置
            rbtn_coolen.Checked = false;
            rbtn_cooldis.Checked = false;
            if (PT_SET.bCool) rbtn_coolen.Checked = true;
            else rbtn_cooldis.Checked = true;

            //DCC测试移至上下料
            rbtn_delaytesten.Checked = false;
            rbtn_delaytestdis.Checked = false;
            if (PT_SET.bDelayTest) rbtn_delaytesten.Checked = true;
            else rbtn_delaytestdis.Checked = true;

            //NG管控设置
            rbtn_ngcontrolen.Checked = false;
            rbtn_ngcontroldis.Checked = false;
            if (PT_SET.bNgControl) rbtn_ngcontrolen.Checked = true;
            else rbtn_ngcontroldis.Checked = true;
            nud_ngscale.Value = (decimal)PT_SET.ngScale;
            nud_ngcode.Value = (decimal)PT_SET.ngCode;

            //持续报警设置
            rbtn_ContinuousAlarmEn.Checked = false;
            rbtn_ContinuousAlarmDis.Checked = false;
            if (PT_SET.bContinuousAlarm) rbtn_ContinuousAlarmEn.Checked = true;
            else rbtn_ContinuousAlarmDis.Checked = true;


            //良率管控
            rbtn_NgWarningEn.Checked = true;
            rbtn_NgWarningDis.Checked = false;
            //if (PT_SET.bNgWarn) rbtn_NgWarningEn.Checked = true;
            //else rbtn_NgWarningDis.Checked = true;
            nud_okRate.Value = (decimal)PT_SET.OkRate;

            rbtn_MesLocal.Checked = false;
            rbtn_MesRemote.Checked = false;
            if (PT_SET.IsMesLocal) rbtn_MesLocal.Checked = true;
            else rbtn_MesRemote.Checked = true;
            //gy0123夹具上传数据间隔时间
            nud_JigSendTime.Value = PT_SET.JigCntSend;

            //最大测试时间
            nud_TestTime.Value = PT_SET.TestTime;

            //夹具扫码gy0123
            btnStartJigSan1.Checked = COM.ws1.bjigSan;
            btnStartJigSan2.Checked = COM.ws2.bjigSan;
            btnStartJigSan3.Checked = COM.ws3.bjigSan;
            btnStartJigSan4.Checked = COM.ws4.bjigSan;
            rabt_jigscan_ON.Checked = PT_SET.bJigSan;
            rabt_jigscan_OFF.Checked = !PT_SET.bJigSan;

            //新增上拍二维码
            rbtn_AddCapQrcodeDis.Checked = false;
            rbtn_AddCapQrcodeEn.Checked = false;
            if (PT_SET.bAddCapQrcode) rbtn_AddCapQrcodeEn.Checked = true;
            else rbtn_AddCapQrcodeDis.Checked = true;

            checkpicktray.Checked = PT_SET.bpicktray;
            numpicktrayx1.Value = (decimal)PT_SET.picktrayxt1.x;
            numpicktrayy1.Value = (decimal)PT_SET.picktrayxt1.y;
            numpicktrayx2.Value = (decimal)PT_SET.picktrayxt2.x;
            numpicktrayy2.Value = (decimal)PT_SET.picktrayxt2.y;
            numpicktrayx3.Value = (decimal)PT_SET.picktrayxt3.x;
            numpicktrayy3.Value = (decimal)PT_SET.picktrayxt3.y;
            numpicktrayx4.Value = (decimal)PT_SET.picktrayxt4.x;
            numpicktrayy4.Value = (decimal)PT_SET.picktrayxt4.y;
            cbcity.SelectedIndex = PT_SET.citynum;
            ckupload.Checked = PT_SET.upload;
            ckcloseazd.Checked = PT_SET.closeazd;
            ckJigDownPhoto.Checked = PT_SET.bJigDownPhoto;
            rbtn_DwAddCapQrcodeEn.Checked = PT_SET.bDwAddCapQrcode;
            rbtn_DwAddCapQrcodeOff.Checked = !PT_SET.bDwAddCapQrcode;
            rbtn_check2open.Checked = PT_SET.Check2open;
            rbtn_check2close.Checked = !PT_SET.Check2open;
            //回检管控
            nud_LeftArea.Value = (decimal)PT_SET.LeftArea;
            nud_RightArea.Value = (decimal)PT_SET.RightArea;
            nud_UpArea.Value = (decimal)PT_SET.UpArea;
            nud_DownArea.Value = (decimal)PT_SET.DownArea;
            nud_Areaofset.Value = (decimal)PT_SET.Area;

            nud_LeftArea2.Value = (decimal)PT_SET.LeftArea2;
            nud_RightArea2.Value = (decimal)PT_SET.RightArea2;
            nud_UpArea2.Value = (decimal)PT_SET.UpArea2;
            nud_DownArea2.Value = (decimal)PT_SET.DownArea2;
            nud_Areaofset2.Value = (decimal)PT_SET.Area2;

            nud_LeftArea3.Value = (decimal)PT_SET.LeftArea3;
            nud_RightArea3.Value = (decimal)PT_SET.RightArea3;
            nud_UpArea3.Value = (decimal)PT_SET.UpArea3;
            nud_DownArea3.Value = (decimal)PT_SET.DownArea3;
            nud_Areaofset3.Value = (decimal)PT_SET.Area3;

            nud_LeftArea4.Value = (decimal)PT_SET.LeftArea4;
            nud_RightArea4.Value = (decimal)PT_SET.RightArea4;
            nud_UpArea4.Value = (decimal)PT_SET.UpArea4;
            nud_DownArea4.Value = (decimal)PT_SET.DownArea4;
            nud_Areaofset4.Value = (decimal)PT_SET.Area4;

            cb_ConnectorCheck.Checked = PT_SET.bConnectorCheck;

            if (PT_SET.EqpPos.Length > 0)
                tb_eqp_pos.Text = PT_SET.EqpPos;
            if (PT_SET.EqpSN.Length > 0)
                tb_eqp_sn.Text = PT_SET.EqpSN;


            //马达扫码设置
            rbtn_motoren.Checked = false;
            rbtn_motordis.Checked = false;
            if (PT_SET.bmotorphoto) rbtn_motoren.Checked = true;
            else rbtn_motordis.Checked = true;
            nud_MotorZ1.Value = (decimal)PT_SET.MotorZ1;
            nud_MotorZ2.Value = (decimal)PT_SET.MotorZ2;
            nud_MotorZ3.Value = (decimal)PT_SET.MotorZ3;
            nud_MotorZ4.Value = (decimal)PT_SET.MotorZ4;
            numsunnyokrate.Value = (decimal)PT_SET.Motorrate;
            nud_MotorAngle1.Value = (decimal)PT_SET.MotorAngle1;
            nud_Motornum.Value = (decimal)PT_SET.Motornum;

            numng1.Value = (decimal)PT_SET.Numng1;
            numng2.Value = (decimal)PT_SET.Numng2;
            numng3.Value = (decimal)PT_SET.Numng3;
            numng4.Value = (decimal)PT_SET.Numng4;
            numng5.Value = (decimal)PT_SET.Numng5;
            numngrate1.Value = (decimal)PT_SET.Numngrate1;
            numngrate2.Value = (decimal)PT_SET.Numngrate2;
            numngrate3.Value = (decimal)PT_SET.Numngrate3;
            numngrate4.Value = (decimal)PT_SET.Numngrate4;
            numngrate5.Value = (decimal)PT_SET.Numngrate5;
            checknumng1.Checked = PT_SET.Ckngrate1;
            checknumng2.Checked = PT_SET.Ckngrate2;
            checknumng3.Checked = PT_SET.Ckngrate3;
            checknumng4.Checked = PT_SET.Ckngrate4;
            checknumng5.Checked = PT_SET.Ckngrate5;

            nud_MotorAngle2.Value = (decimal)PT_SET.MotorAngle2;
            nud_MotorAngle3.Value = (decimal)PT_SET.MotorAngle3;
            nud_MotorAngle4.Value = (decimal)PT_SET.MotorAngle4;
            nud_MotorbarcodeDigits.Value = (decimal)PT_SET.motorBarcodeDigits;
            if (PT_SET.bsunnyqr) checkBoxSunnyQr.Checked = true;
            else checkBoxSunnyQr.Checked = false;

            if (PT_SET.bdownqr) checkBoxdownQr.Checked = true;
            else checkBoxdownQr.Checked = false;

            if (PT_SET.bsunnyqrleft) checksunnyleft.Checked = true;
            else checksunnyleft.Checked = false;
            if (PT_SET.bsunnyqrright) checksunnyright.Checked = true;
            else checksunnyright.Checked = false;
            if (PT_SET.bsunnyqralm) checkBoxSunnyAlm.Checked = true;
            else checkBoxSunnyAlm.Checked = false;
            if (PT_SET.bsunnyqralmtray) checkBoxSunnyAlmTray.Checked = true;
            else checkBoxSunnyAlmTray.Checked = false;
            textBoxSunnyIp.Text = PT_SET.sunnyqrip0;
            textBoxSunnyIp2.Text = PT_SET.sunnyqrip1;
            radWsNgRateShowEn.Checked = PT_SET.bWsNgRateShow;
            radWsNgRateShowOff.Checked = !PT_SET.bWsNgRateShow;
            NumWsNgCntPer20.Value = (decimal)PT_SET.CntWsNgRateShow;


            //自动点检
            rbt_autoChkOff.Checked = !PT_SET.AutoChkEn;
            rbt_autoChkOn.Checked = PT_SET.AutoChkEn;


            rbt_ws1.Checked = PT_SET.AutoChkSelectWs == (int)PT_SET.AutoChkWs.WS1;
            rbt_ws2.Checked = PT_SET.AutoChkSelectWs == (int)PT_SET.AutoChkWs.WS2;
            rbt_ws3.Checked = PT_SET.AutoChkSelectWs == (int)PT_SET.AutoChkWs.WS3;
            rbt_ws4.Checked = PT_SET.AutoChkSelectWs == (int)PT_SET.AutoChkWs.WS4;

            rbt_modeV.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Vivo;
            rbt_modeM.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Mi;
            rbt_modeO.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Oppo;
            rbt_modeS.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Sum;
            rbt_modeH.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Huawei;
            rbt_modeL.Checked = PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Lenov;

            ///距离光源自动点检
            AFC_cctdraggableProgressBar.Percentage = PT_SET.afc_cct_uniformity_precent;
            AFC_luxdraggableProgressBar.Percentage = PT_SET.afc_lux_uniformity_precent;
            DCC_cctdraggableProgressBar.Percentage = PT_SET.dcc_cct_uniformity_precent;
            DCC_luxdraggableProgressBar.Percentage = PT_SET.dcc_lux_uniformity_precent;
            OTP_cctdraggableProgressBar.Percentage = PT_SET.otp_cct_uniformity_precent;
            OTP_luxdraggableProgressBar.Percentage = PT_SET.otp_lux_uniformity_precent;

            ////屏蔽
            AFC_APOS.Value = PT_SET.ApointposAFC;
            AFC_BPOS.Value = PT_SET.BpointposAFC;
            AFC_CPOS.Value = PT_SET.CpointposAFC;
            DCC_APOS.Value = PT_SET.ApointposDCC;
            DCC_BPOS.Value = PT_SET.BpointposDCC;
            DCC_CPOS.Value = PT_SET.CpointposDCC;
            OTP_APOS.Value = PT_SET.ApointposOTP;
            OTP_BPOS.Value = PT_SET.BpointposOTP;
            OTP_CPOS.Value = PT_SET.CpointposOTP;



            AFC_distance_check_open.Checked = PT_SET.AFC_distance_check_open ? true : false;
            DCC_distance_check_open.Checked = PT_SET.DCC_distance_check_open ? true : false;
            OTP_distance_check_open.Checked = PT_SET.OTP_distance_check_open ? true : false;
            AFC_luxcct_check_open.Checked = PT_SET.AFC_luxcct_check_open ? true : false;
            DCC_luxcct_check_open.Checked = PT_SET.DCC_luxcct_check_open ? true : false;
            OTP_luxcct_check_open.Checked = PT_SET.OTP_luxcct_check_open ? true : false;
            if (PT_SET.AFC_distance_check_open || PT_SET.DCC_distance_check_open)
            {
                COM_1.Enabled = true;
                COM_1.Text = PT_SET.COM_1;
            }
            else
            {
                COM_1.Enabled = false;
            }

            if (PT_SET.AFC_luxcct_check_open || PT_SET.OTP_luxcct_check_open|| PT_SET.DCC_luxcct_check_open)
            {
                COM_2.Enabled = true;
                COM_1.Text = PT_SET.COM_1;
                COM_2.Text = PT_SET.COM_2;
                COM_3.Text = PT_SET.COM_3;
            }
            else

            {
                COM_2.Enabled = false;
                COM_3.Enabled = false;
            }
            switch (PT_SET.CheckWs)
            {
                case 1: CheckWsCombox.Text = "工站1"; break;
                case 2: CheckWsCombox.Text = "工站2"; break;
                case 3: CheckWsCombox.Text = "工站3"; break;
                case 4: CheckWsCombox.Text = "工站4"; break;

            }
            distance_coefficient.Value = (decimal)PT_SET.distance_coefficient;
            stanum.Text=(PT_SET.stanum).ToString();
            rbt_modeM_CheckedChanged(rbt_modeL, null);

        }
        private void rbt_modeM_CheckedChanged(object sender, EventArgs e)
        {
            int poscnt = 1;

            if (rbt_modeM.Checked) poscnt = 16;
            else if (rbt_modeO.Checked)
            {
                poscnt = 8;
            }
            else if (rbt_modeV.Checked)
            {
                poscnt = 8;
            }
            else poscnt = 1;
            if (poscnt == 1)
                lbAutoChkShowMsg.Text = $"请选择1个点检模组的上料位置";
            else
                lbAutoChkShowMsg.Text = $"当前模式固定了放置点检模组的位置";
            if (poscnt == 16)
            {
                for (int Num = 1; Num <= 16; Num++)
                {
                    object btn = this.panel_autoChkSet.Controls.Find("btn" + Num, true)[0];
                    if (btn != null)
                    {
                        ((Button)btn).BackColor = Color.Green;
                        ((Button)btn).Enabled = false;
                    }
                }
            }
            else
            if (poscnt == 8)
            {
                for (int Num = 1; Num <= 16; Num++)
                {
                    object btn = this.panel_autoChkSet.Controls.Find("btn" + Num, true)[0];
                    if (btn != null)
                    {
                        if (Num <= 8)
                            ((Button)btn).BackColor = Color.Green;
                        else
                            ((Button)btn).BackColor = Color.Gray;
                        ((Button)btn).Enabled = false;
                    }
                }

            }
            else
            {
                bool ben = false;
                bool bfrist = true;
                for (int Num = 1; Num <= 16; Num++)
                {
                    object btn = this.panel_autoChkSet.Controls.Find("btn" + Num, true)[0];

                    if (btn != null)
                    {

                        if (Num <= 8)
                        {
                            var mm = (1 << (Num - 1));
                            var obj = (int)(PT_SET.AutoChkSmallMdEn & mm);
                            ben = obj != 0;
                        }
                        else if (Num > 8)
                        {
                            var mm = (1 << (Num - 9));
                            var obj = (int)(PT_SET.AutoChkMaxMdEn & mm);
                            ben = obj != 0;
                        }

                        if (ben && bfrist)
                        {
                            ((Button)btn).BackColor = Color.Green;
                            bfrist = false;
                        }
                        else ((Button)btn).BackColor = Color.Gray;

                        ((Button)btn).Enabled = true;

                    }
                }
            }
        }

        public void GetData()
        {
            PT_SET.bpicktray = checkpicktray.Checked;
            PT_SET.picktrayxt1.x = (double)numpicktrayx1.Value;
            PT_SET.picktrayxt1.y = (double)numpicktrayy1.Value;
            PT_SET.picktrayxt2.x = (double)numpicktrayx2.Value;
            PT_SET.picktrayxt2.y = (double)numpicktrayy2.Value;
            PT_SET.picktrayxt3.x = (double)numpicktrayx3.Value;
            PT_SET.picktrayxt3.y = (double)numpicktrayy3.Value;
            PT_SET.picktrayxt4.x = (double)numpicktrayx4.Value;
            PT_SET.picktrayxt4.y = (double)numpicktrayy4.Value;
            PT_SET.citynum = cbcity.SelectedIndex;
            PT_SET.upload =ckupload.Checked;
            PT_SET.closeazd = ckcloseazd.Checked;
            //烘烤检测
            PT_SET.OvenCheck = rbtn_opencheckoven.Checked;
            //光源选择
            PT_SET.bG4C = rad_G4C.Checked;
            //夹具扫码gy0123
            COM.ws1.bjigSan = btnStartJigSan1.Checked;
            COM.ws2.bjigSan = btnStartJigSan2.Checked;
            COM.ws3.bjigSan = btnStartJigSan3.Checked;
            COM.ws4.bjigSan = btnStartJigSan4.Checked;
            PT_SET.bJigSan = rabt_jigscan_ON.Checked;
            PT_SET.Check2open = rbtn_check2open.Checked;
            PT_SET.bJigDownPhoto = ckJigDownPhoto.Checked;
            PT_SET.Isaloneset = cbisaloneset.Checked;
            PT_SET.Ismagic = checkmagic.Checked;
            PT_SET.magictimes = (int)nummagic.Value;
            PT_SET.CloseWait = ckclosewait.Checked;
            PT_SET.CloseWaitTime = (int)nbclosewaittime.Value;

            //等待夹具完全打开才进行测试
            COM.ws1.waitopen = ckws1wait.Checked;
            COM.ws2.waitopen = ckws2wait.Checked;
            COM.ws3.waitopen = ckws3wait.Checked;
            COM.ws4.waitopen = ckws4wait.Checked;
            foreach (WS ws in COM.list_ws)
            {
                ws.waittime = (int)nbwaittime.Value;
            }

            //二维码扫码方式
            if (rbtn_upcode.Checked) PT_SET.BarcodeMode = (int)PT_SET.BAR_SCAN.UP_SCAN;
            else if (rbtn_dwcode.Checked) PT_SET.BarcodeMode = (int)PT_SET.BAR_SCAN.DW_SCAN;
            else PT_SET.BarcodeMode = (int)PT_SET.BAR_SCAN.NO_SCAN;
            PT_SET.safepos = (double)safepos.Value;
            //运行模式
            if (rbtn_twomd.Checked) PT_SET.UpDownRunMode = (int)PT_SET.RUN_MD.BOTH_WORK;
            else if (rbtn_md1.Checked) PT_SET.UpDownRunMode = (int)PT_SET.RUN_MD.MD1_WORK;
            else if (rbtn_md2.Checked) PT_SET.UpDownRunMode = (int)PT_SET.RUN_MD.MD2_WORK;
            //下料前检测二维码
            if (rbtn_openupdownqrcode.Checked)
            {
                PT_SET.OpenDownQrde = true;
                PT_SET.UpDownQrde = true;
                PT_SET.DownDownQrde = false;
            }
            else if (rbtn_undownqrcode.Checked)
            {
                PT_SET.OpenDownQrde = false;
                PT_SET.UpDownQrde = false;
                PT_SET.DownDownQrde = false;
            }
            else if (rbtn_opendownqrcode.Checked)
            {
                PT_SET.OpenDownQrde = true;
                PT_SET.UpDownQrde = false;
                PT_SET.DownDownQrde = true;
            }
            if (cbttpcloseair.Checked)
            {
                PT_SET.otpclose = true;
            }
            else
            {
                PT_SET.otpclose = false;
            }
            PT_SET.otpclosetime = (double)numcloseair.Value;
            if (cbafccloseair.Checked)
            {
                PT_SET.afcclose = true;
            }
            else
            {
                PT_SET.afcclose = false;
            }
            PT_SET.afcclosetime = (double)numafc.Value;

            //运行方式
            if (rbtn_run_normal.Checked) PT_SET.RunPattern = (int)PT_SET.RUN_PATTERN.RUN_NORMAL;
            else if (rbtn_run_updw.Checked) PT_SET.RunPattern = (int)PT_SET.RUN_PATTERN.RUN_UPDW;
            else if (rbtn_run_empty.Checked) PT_SET.RunPattern = (int)PT_SET.RUN_PATTERN.RUN_EMPTY;
            //视觉回检
            //PT_SET.bEnVsFB = Chk_OpenVs.Checked;
            PT_SET.Vs_XYofs = (double)nud_vschk_xyofs.Value;
            PT_SET.Vs_Rofs = (double)nud_vschk_rofs.Value;
            //吸头偏差
            COM.xt1.cap_offset = (double)nud_dwCam_xt1_ofs.Value;
            COM.xt2.cap_offset = (double)nud_dwCam_xt2_ofs.Value;
            COM.xt3.cap_offset = (double)nud_dwCam_xt3_ofs.Value;
            COM.xt4.cap_offset = (double)nud_dwCam_xt4_ofs.Value;

            COM.xt1.DwCapQrCodeoffset = (double)nud_dwCam_xt1_Qrofs.Value;
            COM.xt2.DwCapQrCodeoffset = (double)nud_dwCam_xt2_Qrofs.Value;
            COM.xt3.DwCapQrCodeoffset = (double)nud_dwCam_xt3_Qrofs.Value;
            COM.xt4.DwCapQrCodeoffset = (double)nud_dwCam_xt4_Qrofs.Value;
            //料盘位置检测
            if (rbtn_OpenVsTray.Checked) PT_SET.bEnVsTray = true;
            else if (rbtn_CloseVsTray.Checked) PT_SET.bEnVsTray = false;
            //Y1轴开启设置
            if (rbtn_Y1en.Checked) PT_SET.Y1En = true;
            else if (rbtn_Y1dis.Checked) PT_SET.Y1En = false;
            //右光箱Y1轴开启设置
            if (rbtn_RY1en.Checked) PT_SET.RY1En = true;
            else if (rbtn_RY1dis.Checked) PT_SET.RY1En = false;
            //左右光箱开启设置
            if (rbtn_lben.Checked) PT_SET.LbEn = true;
            else if (rbtn_lbdis.Checked) PT_SET.LbEn = false;
            //开图顺序设置
            if (rbtn_pre_turnon.Checked) PT_SET.turnon = true;
            else if (rbtn_next_turnon.Checked) PT_SET.turnon = false;
            PT_SET.OpenDly = (int)nud_OpenDly.Value;
            //料盘二维码拍照设置
            if (rbtn_traybaren.Checked) PT_SET.TrayBarcodeEn = true;
            else if (rbtn_traybardis.Checked) PT_SET.TrayBarcodeEn = false;
            //吸头1先运动设置
            if (rbtn_xt1firsten.Checked) PT_SET.xt1firsten = true;
            else if (rbtn_xt1firstdis.Checked) PT_SET.xt1firsten = false;
            //模组大小设置
            if (rbtn_small.Checked) PT_SET.issmall = true;
            else if (rbtn_big.Checked) PT_SET.issmall = false;
            //单双开设置
            if (rbtn_single.Checked) PT_SET.bitOpenMode = 1;
            else if (rbtn_double.Checked) PT_SET.bitOpenMode = 2;
            else if (rbtn_Lag.Checked) PT_SET.bitOpenMode = 3;
            else if (rbtn_Sml.Checked) PT_SET.bitOpenMode = 4;
            //霍尔测试
            if (rbtn_hallen.Checked) PT_SET.HallEn = true;
            else if (rbtn_halldis.Checked) PT_SET.HallEn = false;
            PT_SET.bDownFlipTest = chk_downfliptest.Checked;
            //取放料开放角度设置
            if (rbtn_open_degree.Checked) PT_SET.isopen_degree = true;
            else if (rbtn_close_degree.Checked) PT_SET.isopen_degree = false;
            PT_SET.degree = (int)nud_offset_degree.Value;

            //工站下料开放角度设置
            if (rbtn_opendown_degree.Checked) PT_SET.isopendown_degree = true;
            else if (rbtn_closedown_degree.Checked) PT_SET.isopendown_degree = false;
            PT_SET.downdegree = (int)nud_offsetdown_degree.Value;

            //二维码回检设置
            if (rbtn_OpenBarCamBack.Checked) PT_SET.bBarcodeCamBackEn = true;
            else if (rbtn_CloseBarCamBack.Checked) PT_SET.bBarcodeCamBackEn = false;
            if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
            {
                PT_SET.bBarcodeCamBackEn = true;
            }

            if (rbtn_AddCapQrcodeEnOnly.Checked) PT_SET.bBarcodeCamBackEnOnly = true;
            else if (rbtn_AddCapQrcodeEnOnlyClose.Checked) PT_SET.bBarcodeCamBackEnOnly = false;

            //rbtn_CloseBarCamBack.Checked = false;
            //if (!rbtn_CloseBarCamBack.Checked) rbtn_OpenBarCamBack.Checked = true; 
            //PT_SET.bBarcodeCamBackEn = rbtn_OpenBarCamBack.Checked;
            //OK品下料检测设置
            /*if (rbtn_OkCheckEn.Checked)*/
            PT_SET.bOkCheck = true;
            //**  else if (rbtn_OkCheckDis.Checked)*/ PT_SET.bOkCheck = false;*/
            //夹具安装防呆检测设置
            if (rbtn_boxCheckEn.Checked) PT_SET.bboxCheck = true;
            else if (rbtn_boxCheckOff.Checked) PT_SET.bboxCheck = false;
            PT_SET.boxpos1.x = (double)boxCheckpos1X.Value;
            PT_SET.boxpos1.y = (double)boxCheckpos1Y.Value;
            PT_SET.boxpos2.x = (double)boxCheckpos2X.Value;
            PT_SET.boxpos2.y = (double)boxCheckpos2Y.Value;
            PT_SET.boxpos3.x = (double)boxCheckpos3X.Value;
            PT_SET.boxpos3.y = (double)boxCheckpos3Y.Value;
            PT_SET.boxpos4.x = (double)boxCheckpos4X.Value;
            PT_SET.boxpos4.y = (double)boxCheckpos4Y.Value;
            PT_SET.boxsetpos = (double)boxChecksetvalue.Value;
            PT_SET.boxsetpos1 = (double)boxChecksetpos1.Value;
            PT_SET.boxsetpos2 = (double)boxChecksetpos2.Value;
            PT_SET.boxsetpos3 = (double)boxChecksetpos3.Value;
            PT_SET.boxsetpos4 = (double)boxChecksetpos4.Value;


            //工站视觉模板增加有无检测
            if (rbtn_OpenWsVsAddCheck.Checked) PT_SET.bWsVsAddCheckEn = true;
            else if (rbtn_CloseWsVsAddCheck.Checked) PT_SET.bWsVsAddCheckEn = false;
            //上下料移动保护(针对转盘)
            if (rbtn_safeon.Checked) PT_SET.bUdMovSafe = true;
            else if (rbtn_safeoff.Checked) PT_SET.bUdMovSafe = false;
            //门禁
            if (!chk_leftlightbox.Checked) chk_leftlightbox.Checked = true;
            if (!chk_rightlightbox.Checked) chk_rightlightbox.Checked = true;
            if (!chk_tray.Checked) chk_tray.Checked = true;
            if (!chk_updown.Checked) chk_updown.Checked = true;
            PT_SET.bEnLBoxDr = chk_leftlightbox.Checked;
            PT_SET.bEnRBoxDr = chk_rightlightbox.Checked;
            PT_SET.bEnTrayDr = chk_tray.Checked;
            PT_SET.bEnUpDownDr = chk_updown.Checked;

            ////吸头屏蔽
            PT_SET.bUpDn1XtOnlyOne = rbtUpDn1OneXtOn.Checked;
            PT_SET.bUpDn2XtOnlyOne = rbtUpDn2OneXtOn.Checked;



            //_ = rbtXt11EnableOn.Checked? COM.xt1.Enable = true : COM.xt1.Enable = false;
            //_ = rbtXt12EnableOn.Checked ? COM.xt2.Enable = true : COM.xt2.Enable = false;
            //_ = rbtXt21EnableOn.Checked ? COM.xt3.Enable = true : COM.xt3.Enable = false;
            //_ = rbtXt22EnableOn.Checked ? COM.xt4.Enable = true : COM.xt4.Enable = false;


            //GRR流程
            PT_SET.bGrrFlow = chk_grr.Checked;
            PT_SET.GRRTestCnt = (int)nud_grrtestcnt.Value;
            PT_SET.GRRUdlCnt = (int)nud_grrtudlcnt.Value;
            // 相机存储图片
            COM.CamUp1.bSaveImage = chk_upcam1.Checked;
            COM.CamDw1.bSaveImage = chk_dwcam1.Checked;
            COM.CamUp2.bSaveImage = chk_upcam2.Checked;
            COM.CamDw2.bSaveImage = chk_dwcam2.Checked;
            //模组带起
            PT_SET.bModPasteUp = Chk_ModPasteUp.Checked;
            PT_SET.PlaceDly = (int)nud_PlaceDly.Value;
            PT_SET.MovUpDly = (int)nud_MovUpDly.Value;
            //高透模偏移检测
            PT_SET.bGTMCheck = chk_GTMCheck.Checked;
            PT_SET.GTMOfs = (double)nud_GTMOfs.Value;
            //同工位连续同NG提示
            PT_SET.bSameNGTip = chk_SameNGTip.Checked;
            PT_SET.SameNGTipCnt = Math.Max((int)nud_SameNGTipCnt.Minimum, Math.Min((int)nud_SameNGTipCnt.Maximum, (int)nud_SameNGTipCnt.Value));
            nud_SameNGTipCnt.Value = PT_SET.SameNGTipCnt;

            //上下料位置增加测试
            PT_SET.bUpDnAddTest = rbtUpDnAddTestOn.Checked;

            //同工位连续同NG提示
            PT_SET.bSameRowNGTip = chk_SameRowNGTip.Checked;
            PT_SET.SameRowNGTipCnt = (int)nud_SameRowNGTipCnt.Value;
            //保养
            PT_SET.EquipmentMT = (int)nud_EquipmentMaintain.Value;
            PT_SET.FixtrueMT = (int)nud_FixtrueMaintain.Value;
            //数据上传设置
            PT_SET.bUploadData = chk_UpLoadData.Checked;
            PT_SET.bUpdateSoft = chk_UpdateSoft.Checked;
            //工站二次取料
            if (rbtn_OpenWsPickAgain.Checked) PT_SET.bWsPickAgain = true;
            else if (rbtn_CloseWsPickAgain.Checked) PT_SET.bWsPickAgain = false;

            //回检失败继续运行设置
            if (rbtn_backerrcontinueen.Checked) PT_SET.bBackerrcontinue = true;
            else if (rbtn_backerrcontinuedis.Checked) PT_SET.bBackerrcontinue = false;
            //夹具双工位非水平设置
            if (rbtn_nonparallelen.Checked) PT_SET.bNonparallel = true;
            else if (rbtn_nonparalleldis.Checked) PT_SET.bNonparallel = false;
            //夹具清洗设置
            if (rbtn_cleanen.Checked) PT_SET.bCleanen = true;
            else if (rbtn_cleandis.Checked) PT_SET.bCleanen = false;
            PT_SET.Cleaninterval = (double)nud_cleaninterval.Value;
            //相机参数设置

            if (rbtn_camcfgseten.Checked) PT_SET.BCamcfgset = true;
            else if (rbtn_camcfgsetdis.Checked) PT_SET.BCamcfgset = false;
            PT_SET.TrayExposure[0] = (double)nud_tray_exposure1.Value;
            PT_SET.TrayExposure[1] = (double)nud_tray_exposure2.Value;
            PT_SET.TrayBrightness[0] = (double)nud_tray_brightness1.Value;
            PT_SET.TrayBrightness[1] = (double)nud_tray_brightness2.Value;
            PT_SET.TrayContrast[0] = (double)nud_tray_contrast1.Value;
            PT_SET.TrayContrast[1] = (double)nud_tray_contrast2.Value;

            PT_SET.JigExposure[0] = (double)nud_jig_exposure1.Value;
            PT_SET.JigExposure[1] = (double)nud_jig_exposure2.Value;
            PT_SET.JigBrightness[0] = (double)nud_jig_brightness1.Value;
            PT_SET.JigBrightness[1] = (double)nud_jig_brightness2.Value;
            PT_SET.JigContrast[0] = (double)nud_jig_contrast1.Value;
            PT_SET.JigContrast[1] = (double)nud_jig_contrast2.Value;
            PT_SET.WsExposure[0] = (double)nud_ws_exposure1.Value;
            PT_SET.WsExposure[1] = (double)nud_ws_exposure2.Value;

            PT_SET.WsBrightness[0] = (double)nud_ws_brightness1.Value;
            PT_SET.WsBrightness[1] = (double)nud_ws_brightness2.Value;

            PT_SET.WsContrast[0] = (double)nud_ws_contrast1.Value;
            PT_SET.WsContrast[1] = (double)nud_ws_contrast2.Value;
            //点检时间设置
            PT_SET.CheckTimeMorning = dateTimePicker1.Value.ToString("HH:mm:ss");
            PT_SET.CheckTimeEvening = dateTimePicker2.Value.ToString("HH:mm:ss");
            //执行点检时间设置
            PT_SET.CheckTimeMorning1 = dateTimePicker3.Value.ToString("HH:mm:ss");
            PT_SET.CheckTimeEvening2 = dateTimePicker4.Value.ToString("HH:mm:ss");
            //上下料周期保存设置
            if (rbtn_CycleEn.Checked) PT_SET.bCycle = true;
            else if (rbtn_CycleDis.Checked) PT_SET.bCycle = false;
            //降温设置
            if (rbtn_coolen.Checked) PT_SET.bCool = true;
            else if (rbtn_cooldis.Checked) PT_SET.bCool = false;
            //DCC测试移至上下料设置
            if (rbtn_delaytesten.Checked) PT_SET.bDelayTest = true;
            else if (rbtn_delaytestdis.Checked) PT_SET.bDelayTest = false;
            //ng管控设置
            if (rbtn_ngcontrolen.Checked) PT_SET.bNgControl = true;
            else if (rbtn_ngcontroldis.Checked) PT_SET.bNgControl = false;
            PT_SET.ngCode = (int)nud_ngcode.Value;
            PT_SET.ngScale = (double)nud_ngscale.Value;
            //持续报警设置
            if (rbtn_ContinuousAlarmEn.Checked) PT_SET.bContinuousAlarm = true;
            else if (rbtn_ContinuousAlarmDis.Checked) PT_SET.bContinuousAlarm = false;
            //良率管控
            PT_SET.bNgWarn = true;
            //if (rbtn_NgWarningEn.Checked) PT_SET.bNgWarn = true;
            //else if (rbtn_NgWarningDis.Checked) PT_SET.bNgWarn = false;
            PT_SET.OkRate = (double)nud_okRate.Value;
            //Mes模式
            if (rbtn_MesLocal.Checked) PT_SET.IsMesLocal = true;
            else if (rbtn_MesRemote.Checked) PT_SET.IsMesLocal = false;
            //夹具上报时间设置gy0123
            PT_SET.JigCntSend = (int)nud_JigSendTime.Value;
            //最大测试时间
            PT_SET.TestTime = (int)nud_TestTime.Value;

            //新增上拍二维码
            if (rbtn_AddCapQrcodeEn.Checked) PT_SET.bAddCapQrcode = true;
            else if (rbtn_AddCapQrcodeDis.Checked) PT_SET.bAddCapQrcode = false;
            PT_SET.bDwAddCapQrcode = rbtn_DwAddCapQrcodeEn.Checked;

            //回检管控
            PT_SET.Area = (double)nud_Areaofset.Value;
            PT_SET.LeftArea = (double)nud_LeftArea.Value;
            PT_SET.RightArea = (double)nud_RightArea.Value;
            PT_SET.UpArea = (double)nud_UpArea.Value;
            PT_SET.DownArea = (double)nud_DownArea.Value;

            PT_SET.Area2 = (double)nud_Areaofset2.Value;
            PT_SET.LeftArea2 = (double)nud_LeftArea2.Value;
            PT_SET.RightArea2 = (double)nud_RightArea2.Value;
            PT_SET.UpArea2 = (double)nud_UpArea2.Value;
            PT_SET.DownArea2 = (double)nud_DownArea2.Value;

            PT_SET.Area3 = (double)nud_Areaofset3.Value;
            PT_SET.LeftArea3 = (double)nud_LeftArea3.Value;
            PT_SET.RightArea3 = (double)nud_RightArea3.Value;
            PT_SET.UpArea3 = (double)nud_UpArea3.Value;
            PT_SET.DownArea3 = (double)nud_DownArea3.Value;


            PT_SET.Area4 = (double)nud_Areaofset4.Value;
            PT_SET.LeftArea4 = (double)nud_LeftArea4.Value;
            PT_SET.RightArea4 = (double)nud_RightArea4.Value;
            PT_SET.UpArea4 = (double)nud_UpArea4.Value;
            PT_SET.DownArea4 = (double)nud_DownArea4.Value;

            PT_SET.bConnectorCheck = cb_ConnectorCheck.Checked;
            //设备信息
            if (tb_eqp_pos.Text.Length > 0)
                PT_SET.EqpPos = tb_eqp_pos.Text;
            if (tb_eqp_sn.Text.Length > 0)
                PT_SET.EqpSN = tb_eqp_sn.Text;
            //马达扫码设置
            if (rbtn_motoren.Checked) PT_SET.bmotorphoto = true;
            else if (rbtn_motordis.Checked) PT_SET.bmotorphoto = false;
            PT_SET.MotorZ1 = (double)nud_MotorZ1.Value;
            PT_SET.MotorZ2 = (double)nud_MotorZ2.Value;
            PT_SET.MotorZ3 = (double)nud_MotorZ3.Value;
            PT_SET.MotorZ4 = (double)nud_MotorZ4.Value;
            PT_SET.Motornum = (int)nud_Motornum.Value;
            PT_SET.Motorrate = (double)numsunnyokrate.Value;
            PT_SET.MotorAngle1 = (int)nud_MotorAngle1.Value;
            PT_SET.MotorAngle2 = (int)nud_MotorAngle2.Value;
            PT_SET.MotorAngle3 = (int)nud_MotorAngle3.Value;
            PT_SET.MotorAngle4 = (int)nud_MotorAngle4.Value;
            PT_SET.Numng1 = (int)numng1.Value;
            PT_SET.Numng2 = (int)numng2.Value;
            PT_SET.Numng3 = (int)numng3.Value;
            PT_SET.Numng4 = (int)numng4.Value;
            PT_SET.Numng5 = (int)numng5.Value;
            PT_SET.Numngrate1 = (double)numngrate1.Value;
            PT_SET.Numngrate2 = (double)numngrate2.Value;
            PT_SET.Numngrate3 = (double)numngrate3.Value;
            PT_SET.Numngrate4 = (double)numngrate4.Value;
            PT_SET.Numngrate5 = (double)numngrate5.Value;
            PT_SET.Ckngrate1 = checknumng1.Checked;
            PT_SET.Ckngrate2 = checknumng2.Checked;
            PT_SET.Ckngrate3 = checknumng3.Checked;
            PT_SET.Ckngrate4 = checknumng4.Checked;
            PT_SET.Ckngrate5 = checknumng5.Checked;


            PT_SET.motorBarcodeDigits = (int)nud_MotorbarcodeDigits.Value;
            PT_SET.bsunnyqr = checkBoxSunnyQr.Checked;
            PT_SET.bdownqr = checkBoxdownQr.Checked;
            PT_SET.bsunnyqrright = checksunnyright.Checked;
            PT_SET.bsunnyqrleft = checksunnyleft.Checked;

            PT_SET.bsunnyqralm = checkBoxSunnyAlm.Checked;
            PT_SET.bsunnyqralmtray = checkBoxSunnyAlmTray.Checked;
            PT_SET.sunnyqrip0 = textBoxSunnyIp.Text;
            PT_SET.sunnyqrip1 = textBoxSunnyIp2.Text;
            PT_SET.bWsNgRateShow = radWsNgRateShowEn.Checked;
            PT_SET.CntWsNgRateShow = (int)NumWsNgCntPer20.Value;


            PT_SET.AutoChkEn = rbt_autoChkOn.Checked;
            if (rbt_ws1.Checked) PT_SET.AutoChkSelectWs = (int)PT_SET.AutoChkWs.WS1;
            else if (rbt_ws2.Checked) PT_SET.AutoChkSelectWs = (int)PT_SET.AutoChkWs.WS2;
            else if (rbt_ws3.Checked) PT_SET.AutoChkSelectWs = (int)PT_SET.AutoChkWs.WS3;
            else if (rbt_ws4.Checked) PT_SET.AutoChkSelectWs = (int)PT_SET.AutoChkWs.WS4;
            int PosCnt = 1;
            PT_SET.AutoChkMod mod = PT_SET.AutoChkMod.Oppo;
            if (rbt_modeM.Checked)
            { mod = PT_SET.AutoChkMod.Mi; PosCnt = 16; }
            else if (rbt_modeO.Checked)
            { mod = PT_SET.AutoChkMod.Oppo; PosCnt = 8; }
            else if (rbt_modeS.Checked) mod = PT_SET.AutoChkMod.Sum;
            else if (rbt_modeH.Checked) mod = PT_SET.AutoChkMod.Huawei;
            else if (rbt_modeL.Checked) mod = PT_SET.AutoChkMod.Lenov;
            else if (rbt_modeV.Checked)
            {
                mod = PT_SET.AutoChkMod.Vivo;
                PosCnt = 8;
            }
            PT_SET.AutoChkMode = (int)mod;

            int cnt = 0;
            if (PT_SET.AutoChkEn)
            {
                for (int Num = 1; Num <= 16; Num++)
                {
                    object btn = this.panel_autoChkSet.Controls.Find("btn" + Num, true)[0];
                    if (btn != null)
                    {
                        if (((Button)btn).BackColor == Color.Green)
                            cnt++;
                    }
                }
                if (cnt != PosCnt)
                {
                    MessageBox.Show("当前设置点检位置不符合要求,\r\n,/r/n,请重新按要求选择位置");
                }
                else
                {

                    if (PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Mi)
                    {
                        PT_SET.AutoChkMaxMdEn = 255;
                        PT_SET.AutoChkSmallMdEn = 255;
                    }
                    else if (PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Oppo)
                    {
                        PT_SET.AutoChkMaxMdEn = 0;
                        PT_SET.AutoChkSmallMdEn = 255;

                    }
                    else if (PT_SET.AutoChkMode == (int)PT_SET.AutoChkMod.Vivo)
                    {
                        PT_SET.AutoChkMaxMdEn = 0;
                        PT_SET.AutoChkSmallMdEn = 255;
                    }
                    else
                    {


                        int maxposEn = 0;
                        int minposEn = 0;
                        for (int Num = 1; Num <= 16; Num++)
                        {
                            object btn = this.panel_autoChkSet.Controls.Find("btn" + Num, true)[0];
                            if (btn != null)
                            {
                                bool en = ((Button)btn).BackColor == Color.Green;
                                int obj = 0;
                                if (en) obj = 1;
                                if (Num > 8 && Num <= 16)
                                {
                                    maxposEn = (maxposEn | (obj << (Num - 9)));
                                }
                                else if (Num > 0 && Num <= 8)
                                    minposEn = (minposEn | (obj << (Num - 1)));
                            }
                        }
                        PT_SET.AutoChkMaxMdEn = maxposEn;
                        PT_SET.AutoChkSmallMdEn = minposEn;
                    }
                }
            }
            PT_SET.afc_cct_uniformity_precent= AFC_luxdraggableProgressBar.Percentage;
            PT_SET.afc_lux_uniformity_precent= AFC_cctdraggableProgressBar.Percentage;
            PT_SET.dcc_cct_uniformity_precent = DCC_cctdraggableProgressBar.Percentage;
            PT_SET.dcc_lux_uniformity_precent = DCC_luxdraggableProgressBar.Percentage;
            PT_SET.otp_cct_uniformity_precent = OTP_cctdraggableProgressBar.Percentage;
            PT_SET.otp_lux_uniformity_precent = OTP_luxdraggableProgressBar.Percentage;

            ////屏蔽
            PT_SET.ApointposAFC = AFC_APOS.Value;
            PT_SET.BpointposAFC = AFC_BPOS.Value;
            PT_SET.CpointposAFC = AFC_CPOS.Value;
            PT_SET.ApointposDCC = DCC_APOS.Value;
            PT_SET.BpointposDCC = DCC_BPOS.Value;
            PT_SET.CpointposDCC = DCC_CPOS.Value;
            PT_SET.ApointposOTP = OTP_APOS.Value;
            PT_SET.BpointposOTP = OTP_BPOS.Value;
            PT_SET.CpointposOTP = OTP_CPOS.Value;



            var uniquePorts = new HashSet<string>();
            bool a=uniquePorts.Add(COM_1.Text);
            bool b = uniquePorts.Add(COM_2.Text);
            bool c = uniquePorts.Add(COM_3.Text);
            if (!(a && b && c))
            {
                MessageBox.Show("输入的COM口有重复,COM口保存失败");
            }
            else
            {
                PT_SET.COM_1 = COM_1.Text;
                PT_SET.COM_2 = COM_2.Text;
                PT_SET.COM_3 = COM_3.Text;
            }
            PT_SET.CheckTimer= (double)AutoCheckTimer.Value;

            PT_SET.AFC_distance_check_open = AFC_distance_check_open.Checked ? true : false;
            PT_SET.DCC_distance_check_open = DCC_distance_check_open.Checked ? true : false;
            PT_SET.OTP_distance_check_open = OTP_distance_check_open.Checked ? true : false;
            PT_SET.AFC_luxcct_check_open = AFC_luxcct_check_open.Checked ? true : false;
            PT_SET.DCC_luxcct_check_open = DCC_luxcct_check_open.Checked ? true : false;
            PT_SET.OTP_luxcct_check_open = OTP_luxcct_check_open.Checked ? true : false;

            switch (CheckWsCombox.Text)
            {
                case "工站1":PT_SET.CheckWs = 1; break;
                case "工站2": PT_SET.CheckWs = 2; break;
                case "工站3": PT_SET.CheckWs = 3; break;
                case "工站4": PT_SET.CheckWs = 4; break;

            }
            PT_SET.distance_coefficient = (double)distance_coefficient.Value;
            PT_SET.stanum = int.Parse(stanum.Text);
           
        }
        public bool ReloadUpcamTask(string tskname)
        {
            bool res = true;
            foreach (Cam upcam in COM.ListUpCam)
            {
                Cam.VisionTask tsk = upcam.List_vs_task.Find(s => s.TaskName.Contains(tskname));
                if (tsk != null)
                    res = tsk.Load();
                Cam.VisionTask copytsk = upcam.List_vs_copytask.Find(s => s.TaskName.Contains(tskname));
                if (copytsk != null) upcam.List_vs_copytask.Remove(copytsk);
            }
            return res;
        }


        private void btn_ptset_save_Click(object sender, EventArgs e)
        {
            bool VsUpdate = false;
            bool WsUpdate = false;
            bool res = true;
            int bitOpenMode_temp = 1;

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "当前参数保存者:" + FrMain.frsuser.user1.cur_user);
            Type staticClassType = typeof(PT_SET);
            // 获取所有字段
            FieldInfo[] fields = staticClassType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{field.Name}: {field.GetValue(null)}");
            }

            // 获取所有属性
            PropertyInfo[] properties = staticClassType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            Console.WriteLine("Properties:");
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(null)}");
            }

            try
            {
                //判断是否要重新加载任务
                btn_ptset_save.Enabled = false;
                if ((rbtn_upcode.Checked && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN) ||
                    (rbtn_dwcode.Checked && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN) ||
                    (rbtn_NoCap.Checked && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.NO_SCAN)) VsUpdate = true;
                if (PT_SET.bGTMCheck != chk_GTMCheck.Checked) VsUpdate = true;
                if (Math.Abs(PT_SET.GTMOfs - (double)nud_GTMOfs.Value) > 0.02) VsUpdate = true;
                if (VsUpdate)
                {
                    res = ReloadUpcamTask(CONST.ModUpFw);
                    if (!res)
                    {
                        MessageBox.Show(VAR.IsChinese ? string.Format("保存失败,{0}加载失败!", CONST.ModUpFw) : string.Format("Save failed, {0} failed to load!\r\n保存失败,{0}加载失败!", CONST.ModUpFw), VAR.IsChinese ? "警告" : "Warning");
                        return;
                    }
                }
                VsUpdate = false;
                if (PT_SET.bBarcodeCamBackEn != rbtn_OpenBarCamBack.Checked) VsUpdate = true;
                if (VsUpdate)
                {
                    res = ReloadUpcamTask(CONST.WsModUpFw);
                    if (!res)
                    {
                        MessageBox.Show(VAR.IsChinese ? string.Format("保存失败,{0}加载失败!", CONST.WsModUpFw) : string.Format("Save failed, {0} failed to load!\r\n保存失败,{0}加载失败!", CONST.WsModUpFw), VAR.IsChinese ? "警告" : "Warning");
                        return;
                    }
                }
                VsUpdate = false;
                if (rbtn_OpenWsVsAddCheck.Checked != PT_SET.bWsVsAddCheckEn) VsUpdate = true;
                if (VsUpdate)
                {
                    res = ReloadUpcamTask(CONST.WsUpFw);
                    if (!res)
                    {
                        MessageBox.Show(VAR.IsChinese ? string.Format("保存失败,{0}加载失败!", CONST.WsUpFw) : string.Format("Save failed, {0} failed to load!\r\n保存失败,{0}加载失败!", CONST.WsUpFw), VAR.IsChinese ? "警告" : "Warning");
                        return;
                    }
                }

                if (rbtn_single.Checked) bitOpenMode_temp = 1;
                else if (rbtn_double.Checked) bitOpenMode_temp = 2;
                else if (rbtn_Lag.Checked) bitOpenMode_temp = 3;
                else if (rbtn_Sml.Checked) bitOpenMode_temp = 4;
                if (PT_SET.LbEn != rbtn_lben.Checked || PT_SET.issmall != rbtn_small.Checked ||
                    PT_SET.bitOpenMode != bitOpenMode_temp) WsUpdate = true;



                GetData();
                var bgetOk = dataGrideSysInfo1.GetDataFromGride();

                PT_SET.SavePtCfg(VAR.gsys_set.cur_product_name);
                PT_SET.LoadPtCfg(VAR.gsys_set.cur_product_name);
                foreach (Cam cam in COM.ListCam)
                {
                    cam.LoadOrSaveImgCfg();
                    cam.LoadOrSaveImgCfg(false);
                }

                foreach (XT xt in COM.ListXT)
                {
                    xt.LoadOrSaveCapOfsCfg();
                    xt.LoadOrSaveCapOfsCfg(false);
                }
                if (WsUpdate)
                {

                    foreach (WS ws in COM.list_ws)
                    {
                        foreach (WS.MdDat md in ws.list_md)
                        {
                            md.benable = true;
                        }
                        ws.SaveCfg();
                        ws.LoadCfg();
                    }
                }

                if (ckws1wait.Checked || !ckws1wait.Checked || nbwaittime.Value != 0)
                {
                    foreach (WS ws in COM.list_ws)
                    {
                        ws.SaveCfg();
                        ws.LoadCfg();
                    }
                }

                ShowData();
                string vv = "0";
                if (PT_SET.bJigSan) vv = "1";
                Msg.secsManager.Send(new BaseInfo() { Id = 9, Value = vv });

                if (PT_SET.EqpSN == "123456")
                {
                    MessageBox.Show(VAR.IsChinese ? "请在设置参数3界面填写设备编号和厂区!" : "please write the information of eqpument on SefForm Set3\r\n请在设置参数3界面填写设备编号和厂区!",
                        VAR.IsChinese ? "提示" : "Prompt");
                    return;
                }
                MessageBox.Show(VAR.IsChinese ? "保存成功!" : "Saved successfully!\r\n保存成功!", VAR.IsChinese ? "提示" : "Prompt");
            }
            finally
            {
                btn_ptset_save.Enabled = true;
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //gy0123夹具扫码设置

        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            double ng_image_rate, ng_iic_rate, ng_exposure_rate, ng_miss_pix_rate, ng_OS_rate, ng_other_rate;
            ng_image_rate = 0.88;
            ng_iic_rate = 0.99;
            ng_exposure_rate = 0.77;
            ng_miss_pix_rate = 0.66;
            ng_OS_rate = 0.55;
            ng_other_rate = 0.33;
            string ng_msg = string.Format("\r\n 开图NG率:{0} \r\n IIC读写NG率:{1} \r\n 曝光NG率:{2} \r\n 图像丢帧率:{3}\r\n OS异常率:{4}\r\n 其他NG率: {5}",
            ng_image_rate.ToString("P"), ng_iic_rate.ToString("P"), ng_exposure_rate.ToString("P"),
            ng_miss_pix_rate.ToString("P"), ng_OS_rate.ToString("P"), ng_other_rate.ToString("P"));

            DialogResult dr = FrRun.Dialog(Color.Yellow, "警告", "当前良率低，请注意!可清零！" + ng_msg, "确定", "清除");
            if (dr == DialogResult.Cancel)
            {
                DialogResult drq = FrRun.Dialog(Color.Yellow, "警告", "清除后临时生产数据归零。", "确定", "取消");
                if (dr == DialogResult.OK)
                {
                    DRpt.Report_Opration(1000, 0, "归位按键按下!");

                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            WS.MdDat md = new WS.MdDat();
            md.cnt_ng_exposure = 0;
            md.cnt_ng_iic = 0;
            md.cnt_ng_image = 0;
            md.cnt_ng_miss_pix = 0;
            md.cnt_ng_OS = 0;
            md.cnt_ng_other = 0;
            md.cnt_ok = 0;
            UpDownLoad mupd = new UpDownLoad();
            mupd.GetModJigData(1, md);//发送更新之后的数据清零
        }

        private async void btnmotoScanClick(object sender, EventArgs e)
        {
            Button mbtn = (Button)sender;

            try
            {
                mbtn.Enabled = false;
                Task mm = new Task(() => {
                    int xtid = 0;
                    UpDownLoad mupd = COM.UDLoad2;
                    //if(PT_SET.bmotorphoto==false)
                    //{
                    //    MessageBox.Show( "请开启扫码功能：");
                    //    return;
                    //}
                    if (mbtn.Name.Contains("1") || mbtn.Name.Contains("2"))
                    {
                        mupd = COM.UDLoad1;
                        if (mbtn.Name.Contains("1"))
                        {
                            xtid = 0;

                        }
                        else
                        {
                            xtid = 1;
                        }
                    }
                    if (mbtn.Name.Contains("3") || mbtn.Name.Contains("4"))
                    {
                        if (mbtn.Name.Contains("3"))
                        {
                            xtid = 0;

                        }
                        else
                        {
                            xtid = 1;
                        }
                    }
                    var xt = mupd.list_xt[xtid];
                    if (xt.XtMd == null)
                    {
                        xt.XtMd = new Product.MdDat();
                    }
                    xt.XtMd.motor_barcode = "";
                    bool bquit = false;
                    var res = mupd.MotoScan(ref bquit, xtid);
                    if (xt.XtMd.motor_barcode != null && xt.XtMd.motor_barcode.Length > 3)
                        MessageBox.Show(xt.disc + "扫码成功，结果是：" + xt.XtMd.motor_barcode);
                    else MessageBox.Show(xt.disc + "扫码失败：");
                });
                mm.Start();
                await mm;
            }
            finally
            {
                mbtn.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            MT.COM3.ReInit();
            if (MT.COM3.bComInitOK) MessageBox.Show("打开成功");
            else MessageBox.Show("打开失败");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MT.COM4.ReInit();
            if (MT.COM4.bComInitOK) MessageBox.Show("打开成功");
            else MessageBox.Show("打开失败");
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            int i = 0;
            string cntNgRateFor20 = "";
            while (i < 10)
            {
                var ngcodes = NewSysInf.UserParams.NgRateCodes;
                string[] ngCodeList = new string[20];
                if (ngcodes.Length > 0)
                    ngCodeList = ngcodes.Split(',');
                foreach (var code in ngCodeList)
                {
                    var mdCode = "233";
                    if (mdCode == code)
                    {
                        if (cntNgRateFor20.Length > 0)
                            cntNgRateFor20 += ',' + mdCode;
                        else
                            cntNgRateFor20 += mdCode;
                    }
                }


                SQLData.NGRateShow("测试位置", ref cntNgRateFor20);
                i++;

            }
        }

        private async void btnChkInfoGet_Click(object sender, EventArgs e)
        {
            var oldstr = btnChkInfoGet.Text;
            btnChkInfoGet.Text = "测试中";
            string errmsg = "";
            string okmsg = "";
            Task mm = new Task(() =>
            {
                try
                {

                    foreach (var ws in COM.list_ws)
                    {
                        int[] temp1List = new int[16], temp2List = new int[16];
                        var res1 = ws.GetPcChkInfo(out var pUCFFactoryMode1, out var pUCFFactoryMode2, temp1List, temp2List, false);
                        if (!res1)
                        {
                            errmsg += ws.disc + ",";
                        }

                    }
                }
                finally
                {

                    btnChkInfoGet.Text = oldstr;
                }

            });
            mm.Start();
            Thread.Sleep(100);
            await mm;

            if (errmsg.Length > 1)
            {
                MessageBox.Show($"获取工位点检信息失败{errmsg}");
            }
            else
            {
                MessageBox.Show($"获取工位点检信息成功:{okmsg}");
            }
        }

        private async void btnChkGet_Click(object sender, EventArgs e)
        {
            var oldstr = btnChkGet.Text;
            btnChkGet.Text = "测试中";
            string errmsg = "";
            string okmsg = "";
            Task mm = new Task(() =>
            {
                try
                {

                    foreach (var ws in COM.list_ws)
                    {
                        var res1 = ws.GetPcChkMod(out var pUCFFactoryMode1, out var pUCFFactoryMode2, out var temp1, out var temp2, false);
                        if (!res1)
                        {
                            errmsg += ws.disc + ",";
                        }

                    }
                }
                finally
                {

                    btnChkGet.Text = oldstr;
                }

            });
            mm.Start();
            Thread.Sleep(100);
            await mm;

            if (errmsg.Length > 1)
            {
                MessageBox.Show($"获取工位点检状态失败{errmsg}");
            }
            else
            {
                MessageBox.Show($"获取工位点检状态成功:{okmsg}");
            }
        }

        private async void btnChkSetChk_Click(object sender, EventArgs e)
        {
            var oldstr = btnChkSetChk.Text;
            btnChkSetChk.Text = "测试中";
            btnChkSetChk.Enabled = false;
            string errmsg = "";
            string okmsg = "";
            Task mm = new Task(() =>
            {
                try
                {
                    foreach (var ws in COM.list_ws)
                    {

                        var res1 = ws.SetPcAutoChk(0, false);
                        if (res1)
                        {
                            okmsg += ws.disc + ",";
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"切换工位{ws.disc}点检模式成功");
                        }
                        else
                        {
                            errmsg += ws.disc + ",";
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"切换工位{ws.disc}点检模式失败");
                        }

                    }
                }
                finally
                {
                    btnChkSetChk.Enabled = true;
                    btnChkSetChk.Text = oldstr;
                }
            });
            mm.Start();
            Thread.Sleep(100);
            await mm;
            if (errmsg.Length > 1)
            {
                MessageBox.Show($"切换工位{errmsg}点检模式失败");
            }
            else
            {
                MessageBox.Show($"切换工位{okmsg}点检模式成功");
            }
        }

        private async void btnChkSetProduct_Click(object sender, EventArgs e)
        {
            var oldstr = btnChkSetProduct.Text;
            btnChkSetProduct.Text = "测试中";
            btnChkSetProduct.Enabled = false;
            string errmsg = "";
            string okmsg = "";
            Task mm = new Task(() =>
            {
                try
                {
                    foreach (var ws in COM.list_ws)
                    {
                        var res1 = ws.SetPcPrductMod(false);
                        if (res1)
                        {
                            okmsg += ws.disc + ",";
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"切换工位{ws.disc}生产模式成功");
                        }
                        else
                        {
                            errmsg += ws.disc + ",";
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"切换工位{ws.disc}生产模式失败");
                        }

                    }
                }
                finally
                {
                    btnChkSetProduct.Enabled = true;
                    btnChkSetProduct.Text = oldstr;
                }
            });
            mm.Start();
            Thread.Sleep(100);
            await mm;
            if (errmsg.Length > 1)
            {
                MessageBox.Show($"切换工位{errmsg}生产模式失败");
            }
            else
            {
                MessageBox.Show($"切换工位{okmsg}生产模式成功");
            }
        }

        private async void btnGetChkResult_Click(object sender, EventArgs e)
        {
            var curbtn = (Button)sender;
            var oldstr = curbtn.Text;
            curbtn.Text = "测试中";
            string errmsg = "";
            string okmsg = "";
            Task mm = new Task(() =>
            {
                try
                {

                    foreach (var ws in COM.list_ws)
                    {
                        var res1 = ws.GetPcChkResult(out var temp1, out var temp2, false);
                        if (res1)
                        {

                            string msg = "点检结果1：" + temp1 + "点检结果2：" + temp2;

                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"获取工位{ws.disc}点检结果成功");
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"获取工位{ws.disc}+{msg}");

                        }
                        else
                        {
                            errmsg += ws.disc + ",";
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"获取工位{ws.disc}点检结果失败");
                        }

                    }
                }
                finally
                {

                    curbtn.Text = oldstr;
                }

            });
            mm.Start();
            Thread.Sleep(100);
            await mm;

            if (errmsg.Length > 1)
            {
                MessageBox.Show($"{oldstr}失败{errmsg}");
            }
            else
            {
                MessageBox.Show($"{oldstr}成功:{okmsg}");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            double posx = COM.List_UDLoad[0].ax_x.fenc_pos;
            double posy = COM.List_UDLoad[0].ax_y.fenc_pos;
            posy = posy + (double)nud_dwCam_xt1_Qrofs.Value;
            MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[0].ax_x, posx, ref COM.List_UDLoad[0].ax_y, posy, true);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            double posx = COM.List_UDLoad[0].ax_x.fenc_pos;
            double posy = COM.List_UDLoad[0].ax_y.fenc_pos;
            posy = posy + (double)nud_dwCam_xt2_Qrofs.Value;
            MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[0].ax_x, posx, ref COM.List_UDLoad[0].ax_y, posy, true);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            double posx = COM.List_UDLoad[1].ax_x.fenc_pos;
            double posy = COM.List_UDLoad[1].ax_y.fenc_pos;
            posy = posy + (double)nud_dwCam_xt3_Qrofs.Value;
            MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[1].ax_x, posx, ref COM.List_UDLoad[1].ax_y, posy, true);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            double posx = COM.List_UDLoad[1].ax_x.fenc_pos;
            double posy = COM.List_UDLoad[1].ax_y.fenc_pos;
            posy = posy + (double)nud_dwCam_xt4_Qrofs.Value;
            MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[1].ax_x, posx, ref COM.List_UDLoad[1].ax_y, posy, true);
        }

        private void buttonboxcheckmove_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("确定要进行定位，(上下料模块2)点击是进行位置1定位，点击否进行位置2定位，点击取消则退出 ？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            PT_SET.boxpos1.x = (double)boxCheckpos1X.Value;
            PT_SET.boxpos1.y = (double)boxCheckpos1Y.Value;
            PT_SET.boxpos2.x = (double)boxCheckpos2X.Value;
            PT_SET.boxpos2.y = (double)boxCheckpos2Y.Value;
            VAR.gsys_set.bquit = false;
            if (res == DialogResult.Yes)
            {

                EM_RES rest = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_z, 0);
                if (rest == EM_RES.OK)
                {
                    MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_x, PT_SET.boxpos1.x, ref COM.UDLoad2.ax_y, PT_SET.boxpos1.y);
                }
            }
            else if (res == DialogResult.Yes)
            {
                EM_RES rest = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_z, 0);
                if (rest == EM_RES.OK)
                {
                    MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_x, PT_SET.boxpos2.x, ref COM.UDLoad2.ax_y, PT_SET.boxpos2.y);
                }
            }
        }

        private void buttonboxchecklearn_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("确定要进行学习，(上下料模块2)点击是进行位置1学习，点击否进行位置2学习，点击取消则退出 ？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (res == DialogResult.Yes)
            {
                boxCheckpos1X.Value = COM.UDLoad2.ax_x.enc_pos;
                boxCheckpos1Y.Value = COM.UDLoad2.ax_y.enc_pos;
            }
            else if (res == DialogResult.Yes)
            {
                boxCheckpos2X.Value = COM.UDLoad2.ax_x.enc_pos;
                boxCheckpos2Y.Value = COM.UDLoad2.ax_y.enc_pos;
            }
        }

        private void buttonboxcheckmove2_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("确定要进行定位，(上下料模块1)点击是进行位置3定位，点击否进行位置4定位，点击取消则退出 ？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            PT_SET.boxpos3.x = (double)boxCheckpos3X.Value;
            PT_SET.boxpos3.y = (double)boxCheckpos3Y.Value;
            PT_SET.boxpos4.x = (double)boxCheckpos4X.Value;
            PT_SET.boxpos4.y = (double)boxCheckpos4Y.Value;
            VAR.gsys_set.bquit = false;
            if (res == DialogResult.Yes)
            {

                EM_RES rest = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_z, 0);
                if (rest == EM_RES.OK)
                {
                    MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, PT_SET.boxpos3.x, ref COM.UDLoad1.ax_y, PT_SET.boxpos3.y);
                }
            }
            else if (res == DialogResult.Yes)
            {
                EM_RES rest = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_z, 0);
                if (rest == EM_RES.OK)
                {
                    MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, PT_SET.boxpos4.x, ref COM.UDLoad1.ax_y, PT_SET.boxpos4.y);
                }
            }
        }

        private void buttonboxchecklearn2_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("确定要进行学习，(上下料模块1)点击是进行位置3学习，点击否进行位置24学习，点击取消则退出 ？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (res == DialogResult.Yes)
            {
                boxCheckpos3X.Value = COM.UDLoad1.ax_x.enc_pos;
                boxCheckpos3Y.Value = COM.UDLoad1.ax_y.enc_pos;
            }
            else if (res == DialogResult.Yes)
            {
                boxCheckpos4X.Value = COM.UDLoad1.ax_x.enc_pos;
                boxCheckpos4Y.Value = COM.UDLoad1.ax_y.enc_pos;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            bool bq = false;
            string Code;
            var res = COM.Sunnnyqr0.QrAction(ref bq, out Code);
            if (res != EM_RES.OK)
            {
                MessageBox.Show("扫码失败");
            }
            else
            {
                if ((Code.Length - 4) != PT_SET.motorBarcodeDigits && NewSysInf.UserParams.bCheckMotoCodeLength)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, $"马达二维码位数校验失败，扫码长度:{Code.Length},设置长度:{PT_SET.motorBarcodeDigits}");
                }
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            bool bq = false;
            string Code;
            var res = COM.Sunnnyqr1.QrAction(ref bq, out Code);
            if (res != EM_RES.OK)
            {
                MessageBox.Show("扫码失败");
            }
            else
            {
                if ((Code.Length - 4) != PT_SET.motorBarcodeDigits && NewSysInf.UserParams.bCheckMotoCodeLength)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, $"马达二维码位数校验失败，扫码长度:{Code.Length},设置长度:{PT_SET.motorBarcodeDigits}");
                }
            }
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
            if (textBoxpassword.Text == "hyq20231102000!")
            {
                if (groupBox67.Enabled)
                {
                    groupBox67.Enabled = false;
                    textBoxpassword.Text = "";
                }
                else
                {
                    groupBox67.Enabled = true;

                }
            }
        }

        private void rbtn_motoren_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_lightgetpos_Click(object sender, EventArgs e)
        {
            
            OTP_APOS.Z = MT.AXIS_BOX_OTP_Z.fenc_pos;
        }

        private void btn_lightgotopos_Click(object sender, EventArgs e)
        {
           
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            double pos = Convert.ToDouble(OTP_APOS.Z);
            res = MT.AXIS_BOX_OTP_Z.MoveTo(ref bquit, pos, 25000, true);

        }
       
        private void UpdateLabelSelectionState(Label label)
        {
            bool isSelected = false;

            // 根据Label名称获取选中状态
            switch (label.Name)
            {
                case "lblAPoint":
                    isSelected = isAPointSelected;
                    break;
                case "lblBPoint":
                    isSelected = isBPointSelected;
                    break;
                case "lblCPoint":
                    isSelected = isCPointSelected;
                    break;
            }

            // 更新外观
            label.ForeColor = isSelected ? System.Drawing.Color.Red : System.Drawing.Color.Blue;
            label.Font = new System.Drawing.Font(label.Font, isSelected ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
        }
        private void SetLabelClickableStyle(Label label)
        {
            label.Cursor = Cursors.Hand;
            label.ForeColor = System.Drawing.Color.Blue;
            label.Text += " (点击选择)";
        }
      
        private void button13_Click(object sender, EventArgs e)
        {
            //屏蔽
            DCC_APOS.X = MT.AXIS_BOX_R_X1.fenc_pos;
            DCC_APOS.Z = MT.AXIS_BOX_R_Z2.fenc_pos;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //屏蔽
            AFC_APOS.X = MT.AXIS_BOX_L_X1.fenc_pos;
            AFC_APOS.Z = MT.AXIS_BOX_L_Z2.fenc_pos;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            LightBox lb = COM.RightLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = DCC_APOS.X;
            PosDef.Z2 = DCC_APOS.Z;
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef, 0);


        }

        private void button16_Click(object sender, EventArgs e)
        {

            LightBox lb = COM.LeftLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = AFC_APOS.X;
            PosDef.Z2= AFC_APOS.Z;
            //屏蔽
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef,0);

        }

        private void button18_Click(object sender, EventArgs e)
        {
            //屏蔽
            AFC_BPOS.X = MT.AXIS_BOX_L_X1.fenc_pos;
            AFC_BPOS.Z = MT.AXIS_BOX_L_Z2.fenc_pos;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //屏蔽
            AFC_CPOS.X = MT.AXIS_BOX_L_X1.fenc_pos;
            AFC_CPOS.Z = MT.AXIS_BOX_L_Z2.fenc_pos;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            LightBox lb = COM.LeftLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = AFC_BPOS.X;
            PosDef.Z2 = AFC_BPOS.Z;
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef, 0);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            LightBox lb = COM.LeftLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = AFC_CPOS.X;
            PosDef.Z2 = AFC_CPOS.Z;
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef, 0);

        }

        private void button22_Click(object sender, EventArgs e)
        {
            DCC_BPOS.X = MT.AXIS_BOX_R_X1.fenc_pos;
            DCC_BPOS.Z = MT.AXIS_BOX_R_Z2.fenc_pos;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            LightBox lb = COM.RightLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = DCC_BPOS.X;
            PosDef.Z2 = DCC_BPOS.Z;
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef, 0);

        }

        private void button24_Click(object sender, EventArgs e)
        {
            DCC_CPOS.X = MT.AXIS_BOX_R_X1.fenc_pos;
            DCC_CPOS.Z = MT.AXIS_BOX_R_Z2.fenc_pos;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            LightBox lb = COM.RightLightBox;
            LightBox.PosDef PosDef = new LightBox.PosDef();
            PosDef.X1 = DCC_CPOS.X;
            PosDef.Z2 = DCC_CPOS.Z;
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            res = lb.MoveTo(ref bquit, PosDef, 0);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            OTP_BPOS.Z = MT.AXIS_BOX_OTP_Z.fenc_pos;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            OTP_CPOS.Z = MT.AXIS_BOX_OTP_Z.fenc_pos;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            double pos = Convert.ToDouble(OTP_BPOS.Z);
            res = MT.AXIS_BOX_OTP_Z.MoveTo(ref bquit, pos, 25000, true);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            EM_RES res;
            bool bquit = false;
            MT.SetAllAxToManualSpd();
            double pos = Convert.ToDouble(OTP_CPOS.Z);
            res = MT.AXIS_BOX_OTP_Z.MoveTo(ref bquit, pos, 25000, true);
        }
    }
}
