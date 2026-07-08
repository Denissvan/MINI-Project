using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;
using System.IO.Ports;
using System.Windows.Documents;
using System.Windows.Markup.Primitives;
using DevReport;
using Win32Lib;
using System.Diagnostics;
using UI.Class;
using static UI.Cam;
using MesUIWpf;
using System.Xml.Linq;
using Sunny.UI.Win32;
using User = MotionCtrl.User;
using System.IO.Packaging;
using System.Globalization;
using MesUIWpf.Models;

namespace UI
{
    public partial class FrRun : Form
    {
        private double lastngcnt;

        System.Timers.Timer timer;

        private static readonly object _Locker = new object();
        private TrayBox GetRunTrayBoxForOkSlot()
        {
            return COM.traybox_ok;
        }

        private TrayBox GetRunTrayBoxForNgSlot()
        {
            return COM.traybox_ng;
        }

        private Product.Tray GetRunTrayForOkSlot()
        {
            TrayBox traybox = GetRunTrayBoxForOkSlot();
            return traybox != null ? traybox.tray_cur : null;
        }

        private Product.Tray GetRunTrayForNgSlot()
        {
            TrayBox traybox = GetRunTrayBoxForNgSlot();
            return traybox != null ? traybox.tray_cur : null;
        }

        private bool IsChangingTrayBox()
        {
            return UploadModle.bWaitforUpload ||
                   (COM.traybox_fd != null && COM.traybox_fd.ChgML) ||
                   (COM.traybox_ok != null && COM.traybox_ok.ChgML) ||
                   (COM.traybox_ng != null && COM.traybox_ng.ChgML);
        }

        private void ApplyRunTrayUiMapping()
        {
            traybox_ok.box = GetRunTrayBoxForOkSlot();
            traybox_ng.box = GetRunTrayBoxForNgSlot();

            tray_ok.TrayName = VAR.IsChinese ? "OK料盘" : "OK Tray";
            tray_ng.TrayName = VAR.IsChinese ? "NG料盘" : "NG Tray";

            traybox_ok.TrayBoxName = "OK";
            traybox_ng.TrayBoxName = "NG";

            btn_new_traybox_ok.Text = "更换OK料盒";
            btn_new_traybox_ng.Text = "更换NG料盒";

            if (RuntimeMachineMode.IsTrayBoxSwapped)
            {
                tableLayoutPanel4.SetCellPosition(traybox_ok, new TableLayoutPanelCellPosition(0, 2));
                tableLayoutPanel4.SetCellPosition(traybox_ng, new TableLayoutPanelCellPosition(0, 1));
                tableLayoutPanel4.SetCellPosition(tray_ok, new TableLayoutPanelCellPosition(1, 2));
                tableLayoutPanel4.SetCellPosition(tray_ng, new TableLayoutPanelCellPosition(1, 1));
                btn_new_traybox_ok.Location = new Point(0, 597);
                btn_new_traybox_ng.Location = new Point(0, 519);
            }
            else
            {
                tableLayoutPanel4.SetCellPosition(traybox_ok, new TableLayoutPanelCellPosition(0, 1));
                tableLayoutPanel4.SetCellPosition(traybox_ng, new TableLayoutPanelCellPosition(0, 2));
                tableLayoutPanel4.SetCellPosition(tray_ok, new TableLayoutPanelCellPosition(1, 1));
                tableLayoutPanel4.SetCellPosition(tray_ng, new TableLayoutPanelCellPosition(1, 2));
                btn_new_traybox_ok.Location = new Point(0, 519);
                btn_new_traybox_ng.Location = new Point(0, 597);
            }
        }

        public bool bupdate
        {
            get
            {
                return timer_500ms.Enabled;
            }
            //初次延迟5s再显示
            set
            {
                if (timer_500ms.Interval != 500) timer_500ms.Interval = 5000;
                timer_500ms.Enabled = value;

                //if(value)timer.Start(); 
                //else timer.Stop();
            }
        }

        public void PmsfrRun(User.PERMISSION pms)
        {
            cogDisplayer_run.PmsEn((pms > User.PERMISSION.Engineer) ? true : false);
            cb_product_list.Enabled = false;// (pms >= User.PERMISSION.Engineer) ? true : false;
            ckb_Check.Enabled = (pms >= User.PERMISSION.Engineer) ? true : false;
            updatecheck.Enabled = (pms >= User.PERMISSION.SuperAdmin) ? true : false;
            // ckb_not_open.Enabled= (pms >= User.PERMISSION.Engineer) ? true : false;
        }
        //#region
        //class ParameterHelper
        //{
        //    public int[] X_Cor
        //    {
        //        get;
        //        set;
        //    }
        //    public int[] Y_Cor
        //    {
        //        get;
        //        set;
        //    }
        //    public int[] U_Cor
        //    {
        //        get;
        //        set;
        //    }
        //    public int[] V_Cor
        //    {
        //        get;
        //        set;
        //    }
        //    //仿射变换模型六参数
        //    public double m1
        //    {
        //        get
        //        {
        //            return ((U_Cor[1] - U_Cor[0]) - m2 * (Y_Cor[1] - Y_Cor[0])) / (X_Cor[1] - X_Cor[0]);
        //        }
        //    }
        //    public double m2
        //    {
        //        get
        //        {
        //            return ((U_Cor[1] - U_Cor[0]) * (X_Cor[2] - X_Cor[0]) - (U_Cor[2] - U_Cor[0]) * (X_Cor[1] - X_Cor[0])) /
        //                ((Y_Cor[1] - Y_Cor[0]) * (X_Cor[2] - X_Cor[0]) - (Y_Cor[2] - Y_Cor[0]) * (X_Cor[1] - X_Cor[0]));
        //        }
        //    }
        //    public double m3
        //    {
        //        get
        //        {
        //            return ((V_Cor[1] - V_Cor[0]) - m4 * (Y_Cor[1] - Y_Cor[0])) / (X_Cor[1] - X_Cor[0]);
        //        }
        //    }
        //    public double m4
        //    {
        //        get
        //        {
        //            return ((V_Cor[1] - V_Cor[0]) * (X_Cor[2] - X_Cor[0]) - (V_Cor[2] - V_Cor[0]) * (X_Cor[1] - X_Cor[0])) /
        //                ((Y_Cor[1] - Y_Cor[0]) * (X_Cor[2] - X_Cor[0]) - (Y_Cor[2] - Y_Cor[0]) * (X_Cor[1] - X_Cor[0]));
        //        }
        //    }
        //    public double tx
        //    {
        //        get
        //        {
        //            return U_Cor[0] - m1 * X_Cor[0] - m2 * Y_Cor[0];
        //        }
        //    }
        //    public double ty
        //    {
        //        get
        //        {
        //            return V_Cor[0] - m3 * X_Cor[0] - m4 * Y_Cor[0];
        //        }
        //    }
        //}
        //#endregion

        public FrRun()
        {
            InitializeComponent();
            if (FrMain.frsuser != null)
                PmsfrRun(FrMain.frsuser.user1.cur_user.pms);
            else
                PmsfrRun(User.PERMISSION.Operator);
            //PmsfrRun(User.PERMISSION.Operator);
            //timer = new System.Timers.Timer(1000);
            //timer.AutoReset = true;
            //timer.Elapsed += (obj, args) =>
            //{
            //    timer.Stop();
            //    try
            //    {
            //        UpdateDisplay();
            //        //Thread.Sleep(10000);
            //    }
            //    catch(Exception ex)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message);
            //    }

            //    timer.AutoReset = false;
            //    timer.Interval = 500;
            //    timer.Start();
            //};
        }

        private void cb_product_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            //加载产品
            //try
            //{
            //    VAR.gsys_set.cur_product_name = ((ComboBox)sender).Text;
            //    EM_RES ret = COM.product.LoadDat(VAR.gsys_set.cur_product_name);
            //    if (ret != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "产品数据加载失败!");
            //    else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "产品数据加载成功!");

            //    foreach (WS ws in COM.list_ws)
            //    {
            //        ws.LoadCfg();
            //    }
            //    COM.XtInit(VAR.gsys_set.cur_product_name);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //COM.CamInit();
            //cogDisplayer_run.list_cam.Clear();
            //cogDisplayer_run.AddCam(COM.ListCam);
        }

        private void FrRun_Load(object sender, EventArgs e)
        {
            COM.product.LoadProductList(cb_product_list, VAR.gsys_set.cur_product_name);
            Msg.secsManager.Send(new BaseInfo() { Id = 2, Value = cb_product_list.Text });
            Msg.secsManager.Send(new BaseInfo() { Id = 2 }, TypeId: 2);
            for (int i = 1; i < 37; i++)
            {
                Msg.secsManager.Send(new BaseInfo() { Id = i, Value = "false" }, 1);
            }
            this.cb_product_list.SelectedIndexChanged += new System.EventHandler(this.cb_product_list_SelectedIndexChanged);
            COM.traybox_fd.SetSta(TrayBox.EM_STA.UNTEST);
            COM.traybox_ok.SetSta(TrayBox.EM_STA.FULL);
            COM.traybox_ng.SetSta(TrayBox.EM_STA.FULL);
            COM.traybox_fd.NewBox(Product.EM_CM_RES.UNTEST);
            COM.traybox_ok.NewBox(Product.EM_CM_RES.EMPTY);
            COM.traybox_ng.NewBox(Product.EM_CM_RES.EMPTY);

            ws1.ws = COM.ws1;
            ws2.ws = COM.ws2;
            ws3.ws = COM.ws3;
            ws4.ws = COM.ws4;

            traybox_fd.box = COM.traybox_fd;
            tray_fd.TrayName = VAR.IsChinese ? "待测料盘" : "Feed Tray";
            tray_fd.TrayColor = Color.DarkOrange;
            ApplyRunTrayUiMapping();
            tray_ok.TrayColor = Color.Lime;
            tray_ng.TrayColor = Color.Red;
            VAR.gsys_set.beep_tmr = 3000;
            VAR.sys_inf.Init(lb_war_inf, MT.GPIO_OUT_ALM_RED, MT.GPIO_OUT_ALM_GREEN, MT.GPIO_OUT_ALM_YELLOW, MT.GPIO_OUT_ALM_BEEPER, VAR.gsys_set.beep_tmr);//lb_war_inf
            VAR.msg.StartUpdate(dgv_msg);
            COM.vs_msg.StartUpdate(dgv_vs);
            cogDisplayer_run.AddCam(COM.ListCam);
            VAR.dttt.Columns.Add("DateTime", typeof(string));
            VAR.dttt.Columns.Add("TickCount", typeof(string));
            cbCheckModEn.Checked = VAR.isAutoChkMode == true;


            BaseAction.LightBoxSendMesAll();//上传光箱测试位置信息
        }

        public async void btn_run_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    int dllProbeRet = TestPC.StartTestFlow(3, 0, "DLL_PROBE".ToCharArray());
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("开始键DLL探测 StartTestFlow, ID=3, ret={0}", dllProbeRet));
            //}
            //catch (DllNotFoundException ex)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "开始键DLL探测失败,无法加载 dllforcomv8.dll:" + ex.Message);
            //}
            //catch (BadImageFormatException ex)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "开始键DLL探测失败,DLL位数不匹配:" + ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "开始键DLL探测异常:" + ex.Message);
            //}
            try
            {
                if (!PT_SET.IsMesLocal)
                {
                    MT.IsAllowStartUpdate = false;
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                    Msg.secsManager.Send(new BaseInfo() { Id = 3 }, 2);
                    await Task.Run(() =>
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "正在等待MES上位指令!");
                        SpinWait.SpinUntil(() => MT.IsAllowStartUpdate, 10000);
                    });
                    MT.IsAllowStartUpdate = false;
                    //fr?.Close();
                    if (!MT.IsAllowStart)
                    {
                        Dialog(Color.Yellow, "警告", "被MES禁止启动！请联系相关人员。");
                        return;
                    }
                }
                try
                {
                    if (Process.GetProcessesByName("SecsApp").Count() == 0)
                    {
                        var path = @"Release\SecsApp.exe";
                        if (File.Exists(path)) Process.Start(path);
                    }
                }
                catch (Exception ee)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "启动SECE失败请手动开启!");
                    MessageBox.Show("启动SECE失败请重启软件或手动开启secs");
                    return;
                }
                finally
                {
                    if (Process.GetProcessesByName("SecsApp").Count() == 0)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "启动SECE失败请手动开启!");
                        MessageBox.Show("启动SECE失败请停机重启软件或手动开启secs");
                    }
                }
                bool bqtemp = false;
                foreach (WS wstemp in COM.list_ws)
                {
                    wstemp.PowerOn(ref bqtemp);
                }
                if (VAR.gsys_set.status == EM_SYS_STA.REPAIR)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("提示:当前设备正在维修，无法运行!") : "提示: 当前设备正在维修，无法运行!         (The current device is under repair and cannot be operated!)");
                    return;
                }

                List<CARD> TempCardList;
                if (PT_SET.LbEn)
                {
                    TempCardList = MT.CardList;
                }
                else
                {
                    TempCardList = MT.CardList1;
                }

                //检测复位状态
                foreach (CARD card in TempCardList)
                {
                    if (card.isReady == false)
                    {
                        MessageBox.Show(VAR.IsChinese ? string.Format("{0}未初始化!", card.disc) : string.Format("{0} is not initialized!\r\n 未初始化!", card.disc), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    foreach (AXIS ax in card.AxList)
                    {
                        if (ax.home_status != AXIS.HOME_STA.OK)
                        {
                            MessageBox.Show(VAR.IsChinese ? string.Format("{0} 状态异常，{1}!\r\n请先复位", ax.disc, ax.home_status) : string.Format("{0} Abnormal state,{1}!\r\n Please reset!\r\n{0} 状态异常，{1}!\r\n请先复位", ax.disc, ax.home_status), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                DRpt.Report_Status(DReport.EmStatus.Run, DReport.EmHareware.Null, DReport.EmStatus.Run.GetDescription(VAR.IsChinese));
                Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);

                ////转台门
                //if (MT.GPIO_IN_TT_DOOR.isOFF)
                //{
                //    Thread.Sleep(10);
                //    if (MT.GPIO_IN_TT_DOOR.isOFF)
                //    {
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("转台门打开!"));
                //        VAR.gsys_set.bquit = true;
                //        Action.stop();
                //        return;
                //    }
                //}
                //上下料门
                //if (MT.GPIO_IN_UD_DOOR.isOFF)
                //{
                //    Thread.Sleep(10);
                //    if (MT.GPIO_IN_UD_DOOR.isOFF)
                //    {
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("上下料门打开!"));
                //        VAR.gsys_set.bquit = true;
                //        Action.stop();
                //        return;
                //    }
                //}

                //检测运行状态
                if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "---开始键按下---" : "--- Press the start key ---    (---开始键按下---)");
                DRpt.Report_Opration(1000, 0, VAR.IsChinese ? "---开始键按下---" : "--- Press the start key ---    (---开始键按下---)");
               
                VAR.gsys_set.bquit = false;
                //COM.UDLoad1.Demo = true;
                //COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.HIGH;
                //COM.UDLoad2.Demo = true;
                //COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.LOW;
                //WS.Demo = true;
                WS.bquit = false;
                UpDownLoad.bquit = false;
                WS.bpause = false;
                BaseAction.run();
            }
            finally
            {
                //MT.IsAllowStart = false;
                COUNT_DATA.SaveCountCfg(VAR.gsys_set.cur_product_name);
            }

        }

        int i = 0;
        public void btn_stop_Click(object sender, EventArgs e)
        {
            //for (int n = 0; n < 10; n++)
            //{
            //VAR.gsys_set.bquit = true;

            WS.bquit = true;
            UpDownLoad.bquit = true;
            MT.GPIO_OUT_ALM_BEEPER.SetOff();
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "---停止键按下---" : "---Press the stop key---      (---停止键按下---)");
            DRpt.Report_Opration(1000, 0, VAR.IsChinese ? "---停止键按下---" : "---Press the stop key---      (---停止键按下---)");
            VAR.gsys_set.status = EM_SYS_STA.STANDBY;
            if (VAR.gsys_set.status != EM_SYS_STA.RUN)
            {

                //VAR.gsys_set.bquit = true;
                if (VAR.gsys_set.status != EM_SYS_STA.RESET)
                {
                    COM.LeftLightBox.Stop();
                    COM.RightLightBox.Stop();
                    UpDownLoad.UD1Stop();
                    UpDownLoad.UD2Stop();
                    UpDownLoad.LCStop();
                    foreach (WS ws in COM.list_ws)
                    {
                        ws.ax_u.Stop();
                    }
                }
                else
                {
                    MT.AllAxStop();
                }
            }

            //}
            //VAR.gsys_set.status = EM_SYS_STA.STANDBY;
            //VAR.sys_inf.Set(EM_ALM_STA.NOR_BLUE, "就绪", 0, true);
        }

        delegate void UpdateCallback();
        void UpdateDisplay()
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            int n = 0;
            while (!IsHandleCreated)
            {
                //解决窗体关闭时出现“访问已释放句柄“的异常
                if (Disposing || IsDisposed)
                    return;
                Application.DoEvents();
                Thread.Sleep(1);
                if (n++ > 100) return;
            }
            if (InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {

                UpdateCallback d = new UpdateCallback(UpdateDisplay);
                BeginInvoke(d, new object[] { });
            }
            else
            {
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "START");
                traybox_fd.UpdateShow();
                traybox_ok.UpdateShow();
                traybox_ng.UpdateShow();
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "START1");
                tray_fd.tray_dat = traybox_fd.box.tray_cur;
                tray_ok.tray_dat = GetRunTrayForOkSlot();
                tray_ng.tray_dat = GetRunTrayForNgSlot();


                tray_fd.UpdateShow();
                tray_ok.UpdateShow();
                tray_ng.UpdateShow();
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "START2");
                ws1.UpdateShow();
                ws2.UpdateShow();
                ws3.UpdateShow();
                ws4.UpdateShow();
                //DownLoadMd.UpdateShow();
                //UpLoadMd.UpdateShow();
                //Turnplate.UpdateShow();

                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "START3");
                //lb_leftbox.Text = COM.LeftLightBox.StrOfPos;
                //LightBox.PosDef posdef = COM.LeftLightBox.CurPosDef;
                //lb_lightbox_left_posref.Text = posdef != null ? posdef.Name : "未定义位置";

                //lb_rightbox.Text = COM.RightLightBox.StrOfPos;
                //posdef = COM.RightLightBox.CurPosDef;
                //lb_lightbox_right_posref.Text = posdef != null ? posdef.Name : "未定义位置";

                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "END");
            }
        }
        //public Task show;
        private bool colorchange = false;
        private bool ckb_chk = false;
        bool _bShowDialog;
        bool IsCanClear = true;
        private void timer_500ms_Tick(object sender, EventArgs e)
        {
            BaseAction.bnotfeed = ckb_not_open.Checked;
            if (ckb_Check.Checked)
            {
                VAR.gsys_set.mode = EM_SYS_MODE.CHK;
                if (0 < nud_TestCnt.Value && nud_TestCnt.Value < 6) VAR.ChkCnt = (int)nud_TestCnt.Value;
                else VAR.ChkCnt = 1;

            }
            else
            {
                VAR.gsys_set.mode = EM_SYS_MODE.NORMAL;
            }
            //if(show==null||(show!=null&&show.IsCompleted))
            //{
            //   show = new Task(() =>
            //    {
            UpdateDisplay();
            UpLoad1.UpdateShow(COM.UDLoad1);
            upLoad2.UpdateShow(COM.UDLoad2);
            // DownLoadMd.UpdateShow();
            //});
            //show.Start();
            //}
            if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL && PT_SET.bGrrFlow) lb_mod.Text = VAR.IsChinese ? "<正常GRR测试>" : "Normal GRR";
            else if (PT_SET.RunPattern != (int)PT_SET.RUN_PATTERN.RUN_NORMAL && PT_SET.bGrrFlow) lb_mod.Text = VAR.IsChinese ? "<空跑GRR测试>" : "Empty GRR";
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL && VAR.gsys_set.mode == EM_SYS_MODE.CHK) lb_mod.Text = VAR.IsChinese ? "<正常点检模式>" : "Normal Chk";
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL && VAR.isAutoChkMode) lb_mod.Text = VAR.IsChinese ? "<自动点检模式>" : "Auto Chk";
            else if (PT_SET.RunPattern != (int)PT_SET.RUN_PATTERN.RUN_NORMAL && VAR.gsys_set.mode == EM_SYS_MODE.CHK) lb_mod.Text = VAR.IsChinese ? "<空跑点检模式>" : "UpDwLoad Chk";
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL) lb_mod.Text = VAR.IsChinese ? "<正常生产模式>" : "Normal";
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_UPDW) lb_mod.Text = VAR.IsChinese ? "<有料空跑模式>" : "UpDwLoad";
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_EMPTY) lb_mod.Text = VAR.IsChinese ? "<无料空跑模式>" : "Empty UpDwLoad";

            timer_500ms.Interval = 500;

            //if (COM.CamUp2.isInit && VAR.gsys_set.bpause)
            //{
            //    timer_500ms.Interval = 2000;
            //    COM.CamUp2.Triger();
            //}
            if (VAR.ClearMt)
            {
                btn_ClearMt.BackColor = Color.Cyan;
                btn_ClearMt.Text = VAR.IsChinese ? "-清料中-" : "-Clearing-";
            }
            else
            {
                btn_ClearMt.BackColor = SystemColors.ButtonFace;
                btn_ClearMt.Text = VAR.IsChinese ? "清料" : "Clear";
            }

            if (!ckb_wait_upload.Checked) UpDownLoad.bWaitforUpDownload = false;
            if (UpDownLoad.bWaitforUpDownload)
            {
                if (!colorchange) btn_upload_ok.BackColor = SystemColors.ButtonFace;
                else btn_upload_ok.BackColor = Color.Orange;
                colorchange = !colorchange;
                btn_upload_ok.Text = VAR.IsChinese ? "等待上料..." : "Wait for UpDwLoad";
            }
            else
            {
                colorchange = false;
                btn_upload_ok.BackColor = SystemColors.ButtonFace;
                btn_upload_ok.Text = VAR.IsChinese ? "上料完成" : "UpDwLoad finished";
            }

            lb_tt_n.Text = Turntable.n.ToString();
            lb_tt_ct.Text = Turntable.ct.ToString("000.0s");
            if (VAR.gsys_set.status == EM_SYS_STA.RUN)
            {
                rbtn_NormalProduct.Enabled = false;
                rbtn_RetestProduct.Enabled = false;
                lb_tt_tmr.Text = ((System.Environment.TickCount - Turntable.tmr) / 1000).ToString("000.0s");
                //COUNT_DATA.runtime++;
            }

            else
            {
                rbtn_NormalProduct.Enabled = true;
                rbtn_RetestProduct.Enabled = true;
                lb_tt_tmr.Text = "000.0s";

            }
            double min = COUNT_DATA.runtime;
            min = min / 60;
            tb_run_time.Text = string.Format("{0:000}H{1:00}M", Math.Floor(min / 60), min % 60);
            min = COUNT_DATA.waittime;
            min = min / 60;
            tb_stop_time.Text = string.Format("{0:000}H{1:00}M", Math.Floor(min / 60), min % 60);
            min = COUNT_DATA.waitwltime;
            min = min / 60;
            tb_waitwl_time.Text = string.Format("{0:000}H{1:00}M", Math.Floor(min / 60), min % 60);
            min = COUNT_DATA.runtime + COUNT_DATA.waittime;
            if (min == 0) min = 1;
            min = 1 - COUNT_DATA.waittime / min;
            min = min * 100;
            tb_Activation.Text = min.ToString("f2") + "%";
            tb_LastClearTime.Text = COUNT_DATA.dt.ToString();

            lb_leftbox.Text = COM.LeftLightBox.StrOfPos;
            lb_rightbox.Text = COM.RightLightBox.StrOfPos;
            lb_cur_grrtestcnt.Text = COM.ws1.GrrTestCnt.ToString();
            lb_cur_grrudlcnt.Text = COM.ws1.GrrUDLcnt.ToString();
            lb_set_grrtestcnt.Text = PT_SET.GRRTestCnt.ToString();
            lb_set_grrudlcnt.Text = PT_SET.GRRUdlCnt.ToString();
            lb_EquipmentMT.Text = COUNT_DATA.CurEquipmentMT.ToString();
            lb_FixtrueMT.Text = COUNT_DATA.CurFixtrueMT.ToString();



            if (COUNT_DATA.OkTwoTestCnt > 50000 || COUNT_DATA.NgTwoTestCnt > 50000)
            {
                COUNT_DATA.Clear();
            }
            var curHour = DateTime.Now;
            if (curHour.Hour == 8 && IsCanClear)//|| curHour.Hour == 20
            {
                IsCanClear = false;
                COUNT_DATA.Clear();
            }

            if (curHour.Hour == 9)
            {
                IsCanClear = true;
            }

            double all_cnt = COUNT_DATA.allcnt[0] + COUNT_DATA.allcnt[1] + COUNT_DATA.allcnt[2] + COUNT_DATA.allcnt[3];
            double ok_cnt = COUNT_DATA.okcnt[0] + COUNT_DATA.okcnt[1] + COUNT_DATA.okcnt[2] + COUNT_DATA.okcnt[3];
            double ng_cnt = COUNT_DATA.ngcnt[0] + COUNT_DATA.ngcnt[1] + COUNT_DATA.ngcnt[2] + COUNT_DATA.ngcnt[3];
            double hour = COUNT_DATA.runtime + COUNT_DATA.waittime;
            double uph = all_cnt / (hour == 0 ? 0.001 : hour) * 3600;
            double ok_cntNormal = ok_cnt - COUNT_DATA.OkTwoTestCnt;
            double ng_cntNormal = ng_cnt - COUNT_DATA.NgTwoTestCnt;
            double uphNormal = (ok_cntNormal + ng_cntNormal) / (hour == 0 ? 0.001 : hour) * 3600;
            double all_cntNornmal = ok_cntNormal + ng_cntNormal;
            if (tb_all_cnt.Text != all_cnt.ToString("f0") || tb_ok_cnt.Text != ok_cnt.ToString("f0") ||
                tb_uph.Text != uph.ToString("f0") || lastngcnt != ng_cnt)
            {
                Msg.secsManager.Send((new BaseInfo() { Id = 3, Value = all_cntNornmal.ToString("f0") }));
                Msg.secsManager.Send((new BaseInfo() { Id = 4, Value = ok_cntNormal.ToString("f0") }));
                Msg.secsManager.Send((new BaseInfo() { Id = 5, Value = ng_cntNormal.ToString("f0") }));
                Msg.secsManager.Send((new BaseInfo() { Id = 6, Value = uphNormal.ToString("f0") }));
                if (ok_cntNormal >= 0)
                    Msg.secsManager.Send((new BaseInfo() { Id = 10, Value = ok_cntNormal.ToString("f0") }));
                if (ng_cntNormal >= 0)

                {
                    Msg.secsManager.Send((new BaseInfo() { Id = 11, Value = ng_cntNormal.ToString("f0") }));
                    Msg.secsManager.Send((new BaseInfo() { Id = 12, Value = uphNormal.ToString("f0") }));
                }
                Msg.secsManager.Send(new BaseInfo() { Id = 2 }, TypeId: 2);
            }
            lastngcnt = COUNT_DATA.ngcnt[0] + COUNT_DATA.ngcnt[1] + COUNT_DATA.ngcnt[2] + COUNT_DATA.ngcnt[3];

            tb_uph.Text = uph.ToString("f0");
            tb_all_cnt.Text = all_cnt.ToString("f0");
            tb_ok_cnt.Text = ok_cnt.ToString("f0");
            double ok_rate = ok_cnt / (ok_cnt + ng_cnt != 0 ? ok_cnt + ng_cnt : 1) * 100;
            tb_ok_rate.Text = ok_rate.ToString("f2") + "%";
            double openimage_rate = 0;
            if (ok_cnt + ng_cnt != 0)
                openimage_rate = (1 - COUNT_DATA.openimagerate / (ok_cnt + ng_cnt)) * 100;
            else openimage_rate = 0;
            tb_OpenImageRate.Text = openimage_rate.ToString("f2") + "%";
            double suction_rate = 0;
            suction_rate = COUNT_DATA.SuctionErrcnt;
            suction_rate = suction_rate / (COUNT_DATA.SuctionAllcnt != 0 ? COUNT_DATA.SuctionAllcnt : 1) * 100;

            if (PT_SET.bNgWarn && !_bShowDialog && ok_rate <= PT_SET.OkRate)
            {
                _bShowDialog = true;
                DialogResult dr = Dialog(Color.Yellow, "警告", "当前良率低，请注意!可清零！", "确定", "清除");
                if (dr == DialogResult.Cancel)
                {
                    DialogResult drq = Dialog(Color.Yellow, "警告", "清除后临时生产数据归零。", "确定", "取消");
                    if (dr == DialogResult.OK)
                    {
                        DRpt.Report_Opration(1000, 0, "归位按键按下!");
                        COUNT_DATA.Clear();
                    }
                    _bShowDialog = false;
                }
            }
            tb_SuctionErrRate.Text = suction_rate.ToString("f2") + "%";
            ws1.WorkStationName = VAR.IsChinese ? "工站1" : "WS1";
            ws2.WorkStationName = VAR.IsChinese ? "工站2" : "WS2";
            ws3.WorkStationName = VAR.IsChinese ? "工站3" : "WS3";
            ws4.WorkStationName = VAR.IsChinese ? "工站4" : "WS4";
            traybox_fd.TrayBoxName = VAR.IsChinese ? "供料" : "Feed";
            ApplyRunTrayUiMapping();

            bool bSound = NewSysInf.UserParams.RedLightSund;
            if (bSound && MT.GPIO_OUT_ALM_RED.isON)
            {
                MT.GPIO_OUT_ALM_BEEPER.SetOn();
                Thread.Sleep(250);
                MT.GPIO_OUT_ALM_BEEPER.SetOff();
            }

        }

        private void btn_faf_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            if (COM.RightLightBox.status == LightBox.EM_STA.UNKNOW || COM.RightLightBox.status == LightBox.EM_STA.ERR) return;
            COM.RightLightBox.ax_x1.SetToWorkSpd();
            COM.RightLightBox.ax_x2.SetToWorkSpd();
            COM.RightLightBox.ax_z1.SetToWorkSpd();
            COM.RightLightBox.ax_z2.SetToWorkSpd();
            COM.RightLightBox.TraceMapping("运行页定位", "按钮=右光箱远焦,目标=X1=保持/X2=300/Z1=0/Z2=0");
            COM.RightLightBox.MoveTo(ref VAR.gsys_set.bquit, double.MaxValue, 300, 0, 0);
        }

        private void btn_naf_Click(object sender, EventArgs e)
        {
            if (COM.RightLightBox.status == LightBox.EM_STA.UNKNOW || COM.RightLightBox.status == LightBox.EM_STA.ERR) return;
            COM.RightLightBox.TraceMapping("运行页定位", "按钮=右光箱近焦,目标=X1=100/X2=保持/Z1=-500/Z2=0");
            COM.RightLightBox.MoveTo(ref VAR.gsys_set.bquit, 100, double.MaxValue, -500, 0);
        }

        private void btn_dust_Click(object sender, EventArgs e)
        {
            if (COM.RightLightBox.status == LightBox.EM_STA.UNKNOW || COM.RightLightBox.status == LightBox.EM_STA.ERR) return;
            COM.RightLightBox.TraceMapping("运行页定位", string.Format("按钮=右光箱污坏点,目标=X1={0}/X2=保持/Z1=0/Z2=-500", COM.RightLightBox.ax_x1.slp));
            COM.RightLightBox.MoveTo(ref VAR.gsys_set.bquit, COM.RightLightBox.ax_x1.slp, double.MaxValue, 0, -500);
        }

        private void btn_ready_Click(object sender, EventArgs e)
        {
            if (COM.RightLightBox.status == LightBox.EM_STA.UNKNOW || COM.RightLightBox.status == LightBox.EM_STA.ERR) return;
            COM.RightLightBox.TraceMapping("运行页定位", "按钮=右光箱暗态/准备位,目标=X1=0/X2=0/Z1=0/Z2=0");
            COM.RightLightBox.MoveTo(ref VAR.gsys_set.bquit, 0, 0, 0, 0);
        }

        private void btn_l_faf_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            if (COM.LeftLightBox.status == LightBox.EM_STA.UNKNOW || COM.LeftLightBox.status == LightBox.EM_STA.ERR) return;
            COM.LeftLightBox.ax_x1.SetToWorkSpd();
            COM.LeftLightBox.ax_x2.SetToWorkSpd();
            COM.LeftLightBox.ax_z1.SetToWorkSpd();
            COM.LeftLightBox.ax_z2.SetToWorkSpd();
            COM.LeftLightBox.TraceMapping("运行页定位", "按钮=左光箱远焦,目标=X1=保持/X2=300/Z1=0/Z2=0");
            COM.LeftLightBox.MoveTo(ref VAR.gsys_set.bquit, double.MaxValue, 300, 0, 0);
        }

        private void btn_l_naf_Click(object sender, EventArgs e)
        {
            if (COM.LeftLightBox.status == LightBox.EM_STA.UNKNOW || COM.LeftLightBox.status == LightBox.EM_STA.ERR) return;
            COM.LeftLightBox.TraceMapping("运行页定位", "按钮=左光箱近焦,目标=X1=100/X2=保持/Z1=-500/Z2=0");
            COM.LeftLightBox.MoveTo(ref VAR.gsys_set.bquit, 100, double.MaxValue, -500, 0);
        }

        private void btn_l_dust_Click(object sender, EventArgs e)
        {
            if (COM.LeftLightBox.status == LightBox.EM_STA.UNKNOW || COM.LeftLightBox.status == LightBox.EM_STA.ERR) return;
            COM.LeftLightBox.TraceMapping("运行页定位", string.Format("按钮=左光箱污坏点,目标=X1={0}/X2=保持/Z1=0/Z2=-500", COM.LeftLightBox.ax_x1.slp));
            COM.LeftLightBox.MoveTo(ref VAR.gsys_set.bquit, COM.LeftLightBox.ax_x1.slp, double.MaxValue, 0, -500);
        }

        private void btn_l_ready_Click(object sender, EventArgs e)
        {
            if (COM.LeftLightBox.status == LightBox.EM_STA.UNKNOW || COM.LeftLightBox.status == LightBox.EM_STA.ERR) return;
            COM.LeftLightBox.TraceMapping("运行页定位", "按钮=左光箱暗态/准备位,目标=X1=0/X2=0/Z1=0/Z2=0");
            COM.LeftLightBox.MoveTo(ref VAR.gsys_set.bquit, 0, 0, 0, 0);
        }

        private void btn_upload_ok_Click(object sender, EventArgs e)
        {
            UpDownLoad.bWaitforUpDownload = false;
            #region oldtest
            //MT.SetAllAxToWorkSpd();
            //VAR.gsys_set.bquit = false;
            //UpDownLoad.bquit = false;
            //VAR.ClearMt = false;
            //COM.UDLoad1.Demo = true;
            //COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.HIGH;
            //COM.UDLoad2.Demo = true;
            //COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.LOW;
            //WS ws = COM.ws4;
            //ws.Status = WS.EM_STA.REDAYFORUPDOWNLOAD;

            //ws.FeedStatus = WS.EM_STA.REDAYFORUPDOWNLOAD;
            //ws.TestStatus = WS.EM_TEST_STA.COMPLETED;
            //foreach (WS.MdDat md in ws.list_md)
            //{
            //    md.res =0;
            //}

            //Task tsk1 = new Task(() =>
            //{
            //    EM_RES res = EM_RES.OK;
            //    res = COM.UDLoad1.UpDownLoadModleAct(ref UpDownLoad.bquit, ws);
            //    if (res != EM_RES.OK)
            //    {
            //        UpDownLoad.bquit = true;
            //        if (res == EM_RES.MOVE_PROTECT || res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
            //            VAR.gsys_set.bquit = true;
            //    }
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}线程退出->系统强制退出!", COM.UDLoad1.disc));
            //});
            //tsk1.Start();
            //Task tsk = new Task(() =>
            //    {
            //        EM_RES res = EM_RES.OK;
            //        res = COM.UDLoad2.UpDownLoadModleAct(ref UpDownLoad.bquit, ws);
            //        if (res != EM_RES.OK)
            //        {
            //            UpDownLoad.bquit = true;
            //            if (res == EM_RES.MOVE_PROTECT || res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
            //                VAR.gsys_set.bquit = true;
            //        }
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}线程退出->系统强制退出!", COM.UDLoad2.disc));
            //    });
            //tsk.Start();
            #endregion

        }

        private void btn_new_traybox_fd_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "重新加载供料仓?\r\n注:加载后请拿掉供料轴上的料盘!" : "Reload the feed tray box? \r\n Note: Please remove the feed tray after loading!\r\n重新加载供料仓?\r\n注:加载后请拿掉供料轴上的料盘!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            DRpt.Report_Opration(1000, 0, "更换供料仓按键按下!");
            EM_RES res = COM.traybox_fd.NewBox();
            if (res == EM_RES.OK) COM.traybox_fd.IsReady = true;
            COM.traybox_fd.tray_idx = 0;
            COM.traybox_fd.tray_cur = null;
            //if (DialogResult.No == MessageBox.Show("供料导轨上有料盘?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    COM.traybox_fd.tray_cur = null;
            //}
        }

        private void btn_new_traybox_ok_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "重新加载OK料仓?\r\n注:加载后请拿掉OK料轴上的料盘!" : "Reload the OK tray box? \r\n Note: Please remove the OK tray after loading!\r\n重新加载OK料仓?\r\n注:加载后请拿掉OK料轴上的料盘!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            DRpt.Report_Opration(1000, 0, "更换OK料仓按键按下!");
            EM_RES res = COM.traybox_ok.NewBox();
            if (res == EM_RES.OK) COM.traybox_ok.IsReady = true;
            COM.traybox_ok.tray_idx = 0;
            COM.traybox_ok.tray_cur = null;
            //if (DialogResult.No == MessageBox.Show("OK导轨上有料盘?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    COM.traybox_ok.tray_cur = null;
            //}
        }

        private void btn_new_traybox_ng_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "重新加载NG料仓?\r\n注:加载后请拿掉NG料轴上的料盘!" : "Reload the NG tray box? \r\n Note: Please remove the NG tray after loading!\r\n重新加载NG料仓?\r\n注:加载后请拿掉NG料轴上的料盘!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            DRpt.Report_Opration(1000, 0, "更换NG料仓按键按下!");
            EM_RES res = COM.traybox_ng.NewBox();
            if (res == EM_RES.OK) COM.traybox_ng.IsReady = true;
            COM.traybox_ng.tray_idx = 0;
            COM.traybox_ng.tray_cur = null;
            //if (DialogResult.No == MessageBox.Show("NG导轨上有料盘?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    COM.traybox_ng.tray_cur = null;
            //}
        }

        private void timer_key_Tick(object sender, EventArgs e)
        {
            //EMG
            //if (MT.GPIO_IN_EMG5.isOFF)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_EMG5.isOFF)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("EMG按键"));
            //        VAR.gsys_set.bquit = true;
            //        BaseAction.stop();
            //    }
            //}
            //转台门
            //if (MT.GPIO_IN_TT_DOOR.isOFF)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_TT_DOOR.isOFF)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("转台门打开!"));
            //        VAR.gsys_set.bquit = true;
            // Action.stop();
            //    }
            //}
            //上下料门
            //if (MT.GPIO_IN_UD_DOOR.isOFF)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_UD_DOOR.isOFF)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("上下料门打开!"));
            //        VAR.gsys_set.bquit = true;
            //        Action.stop();
            //    }
            //}
            //start
            //if (MT.GPIO_IN_KEY_START.isON)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_KEY_START.isON)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("开始按键"));
            //        //检测复位状态
            //        foreach (CARD card in MT.CardList)
            //        {
            //            if (card.isReady == false)
            //            {
            //                MessageBox.Show(string.Format("{0}未初始化!", card.disc), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //            foreach (AXIS ax in card.AxList)
            //            {
            //                if (ax.home_status != AXIS.HOME_STA.OK)
            //                {
            //                    MessageBox.Show(string.Format("{0} 状态异常，{1}!\r\n请先复位", ax.disc, ax.home_status), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                    return;
            //                }
            //            }
            //        }

            //        //检测运行状态
            //        if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;

            //        VAR.gsys_set.bquit = false;
            //        Action.run();
            //    }
            //}

            ////stop
            //if (MT.GPIO_IN_KEY_STOP.isON)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_KEY_STOP.isON)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("停止按键"));
            //        Action.stop();                    
            //    }
            //}

            //reset
            //if (MT.GPIO_IN_KEY_RESET.isON)
            //{
            //    Thread.Sleep(10);
            //    if (MT.GPIO_IN_KEY_RESET.isON)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("复位按键"));

            //        //检测复位状态
            //        bool bResetOk = true;
            //        foreach (CARD card in MT.CardList)
            //        {
            //            if (card.isReady == false)
            //            {
            //                bResetOk = false;
            //                return;
            //            }
            //            foreach (AXIS ax in card.AxList)
            //            {
            //                if (ax.home_status != AXIS.HOME_STA.OK)
            //                {
            //                    bResetOk = false;
            //                    return;
            //                }
            //            }
            //        }
            //        if (bResetOk == false)
            //        {
            //            MessageBox.Show("请先手动复位！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }

            //        if (VAR.gsys_set.status != EM_SYS_STA.STANDBY) return;
            //        try
            //        {
            //            VAR.gsys_set.bquit = false;
            //            //Z up
            //            EM_RES res = MT.AXIS_UL_Z.MoveTo(ref VAR.gsys_set.bquit, 0, 0);
            //            res = MT.AXIS_DL_Z.MoveTo(ref VAR.gsys_set.bquit, 0, 3000, false);
            //            if (res != EM_RES.OK) return;
            //            MT.AXIS_UL_Z.WaitForMoveDone(ref VAR.gsys_set.bquit, 0, 3000, false);

            //            //XYU
            //            res = MT.ZupMove(ref VAR.gsys_set.bquit, ref UploadModle.ax_x, 0, ref UploadModle.ax_y, 0, ref UploadModle.ax_u1, 0, ref UploadModle.ax_u2, 0);
            //            if (res != EM_RES.OK) return;

            //            //Y
            //            res = MT.AXIS_DL_Y.MoveTo(ref VAR.gsys_set.bquit, 0, 10000, true);
            //            if (res != EM_RES.OK) return;
            //        }
            //        finally
            //        {
            //            MT.AXIS_UL_X.Stop();
            //            MT.AXIS_UL_Y.Stop();
            //            MT.AXIS_UL_Z.Stop();
            //            MT.AXIS_UL_U1.Stop();
            //            MT.AXIS_UL_U2.Stop();

            //            MT.AXIS_DL_Z.Stop(); 
            //            MT.AXIS_DL_Y.Stop();                        
            //        }
            //    }
            //}
        }

        private void btn_home_Click(object sender, EventArgs e)
        {

            if (VAR.gsys_set.status != EM_SYS_STA.RUN)
            {
                DRpt.Report_Opration(1000, 0, "归位按键按下!");
                try
                {
                    MT.SetAllAxToManualSpd();
                    VAR.gsys_set.bquit = false;
                    EM_RES res = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_z, 0, ref COM.UDLoad2.ax_z, 0);
                    if (res == EM_RES.OK)
                    {
                        MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, 0, ref COM.UDLoad1.ax_y, 0, ref COM.UDLoad2.ax_x, 0, ref COM.UDLoad2.ax_y, 0);
                    }
                }
                finally
                {
                    MT.SetAllAxToWorkSpd();
                }

            }
        }



        private void btn_ClearMt_Click(object sender, EventArgs e)
        {

            //COM.CamUp1.FindTaskTriAndWait(CONST.ModUpFw);
            if (VAR.gsys_set.status == EM_SYS_STA.RUN && VAR.ClearMt == false)
            {
                if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "是否清料？" : "Are you sure to clear?\r\n是否清料？", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                DRpt.Report_Opration(1000, 0, "清料按键按下!");
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("清料") : "Clear   (清料)");
                VAR.ClearMt = true;
            }

        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            if (IsChangingTrayBox())
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? "更换料盒中, 禁止暂停!" : "Changing tray box, pause is disabled!");
                return;
            }

            WS.bpause = true;
            UpDownLoad.bquit = true;
            MT.GPIO_OUT_ALM_BEEPER.SetOff();
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "---暂停键按下---" : "--- Press the pause key ---");
            DRpt.Report_Opration(1000, 0, "暂停键按下!");
            COM.UDLoad1.Ispause=true;
            COM.UDLoad2.Ispause = true;
        }

        private void btn_SetZero_Click(object sender, EventArgs e)
        {
            DRpt.Report_Opration(1000, 0, "清零按键按下!");
            COUNT_DATA.Clear();
        }

        private void rbtn_NormalProduct_CheckedChanged(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            if (rbtn_NormalProduct.Checked)
            {
                VAR.Isnormal = true;
                VAR.bSameNGTip_Temp = true;
                DRpt.Report_Opration(1000, 0, "正常品模式按键按下!");
            }
            else
            {
                VAR.Isnormal = false;
                DRpt.Report_Opration(1000, 0, "复测品模式按键按下!");
                //if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "是否暂时取消同工位连续同NG报警?" : "Are you sure to temporarily cancel the continuous same-NG alarm at the same station?\r\n是否暂时取消同工位连续同NG报警?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                VAR.bSameNGTip_Temp = false;
            }
        }

        private void btn_FixtrueMT_Click(object sender, EventArgs e)
        {
            COUNT_DATA.CurFixtrueMT = 0;
        }

        private void btn_EquimentMT_Click(object sender, EventArgs e)
        {
            COUNT_DATA.CurEquipmentMT = 0;
        }

        private void dtexport_Click(object sender, EventArgs e)
        {
            //if (!PT_SET.bCycle) return;
            //var path = VAR.gsys_set.GetCurProductPath + string.Format("turninterval\\");
            //if (path != null && !Directory.Exists(path)) Directory.CreateDirectory(path);
            //Utility.WriteToXml(VAR.dttt, path + string.Format("{0}转盘.xml", DateTime.Now.ToString("HHmmss")));
            //VAR.dttt.Rows.Clear();
        }

        private void updatecheck_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("按是选择点检模式①:1、5、8工位，按否选择点检模式②:1-8工位，按取消则取消屏蔽并上传点检完毕数据", "点检上传", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {

                foreach (WS ws in COM.list_ws)
                {
                    foreach (WS.MdDat md in ws.list_md)
                    {
                        if (md.Num == 1 || md.Num == 5 || md.Num == 8)
                            md.benable = true;
                        else md.benable = false;
                    }
                    ws.SaveCfg();
                    ws.LoadCfg();
                }
                DRpt.Report_Opration(2000, 0, string.Format("开始点检,点检人:{0}", FrMain.frsuser.user1.cur_user.name));
            }
            if (result == DialogResult.No)
            {

                foreach (WS ws in COM.list_ws)
                {
                    foreach (WS.MdDat md in ws.list_md)
                    {
                        if (md.Num <= 8 && md.Num >= 1)
                            md.benable = true;
                        else md.benable = false;
                    }
                    ws.SaveCfg();
                    ws.LoadCfg();
                }
                DRpt.Report_Opration(2000, 0, string.Format("开始点检,点检人:{0}", FrMain.frsuser.user1.cur_user.name));
            }
            if (result == DialogResult.Cancel)
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
                DRpt.Report_Opration(2000, 0, string.Format("点检完毕,点检人:{0}", FrMain.frsuser.user1.cur_user.name));
            }

        }

        /// <summary>
        /// 警告窗口
        /// </summary>
        /// <param name="bkColor">背景颜色</param>
        /// <param name="title">警告标题</param>
        /// <param name="info">警告内容</param>
        /// <param name="okText">OK按键名称</param>
        /// <param name="cancelText">Cancel按键名称</param>
        /// <returns>返回按键结果</returns>
        public static DialogResult Dialog(Color bkColor, string title, string info = "", string okText = "确定", string cancelText = "", string AbortText = "")
        {
            lock (_Locker)
            {
                //警告标题
                Label lb_Title = new Label
                {
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = title
                };

                FlowLayoutPanel flp_Panel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    Margin = new Padding(0)
                };
                Button btn_Ok = new Button
                {
                    Name = "btnOk",
                    Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                    Dock = DockStyle.Bottom,
                    Width = 140,
                    Height = 80,
                    Margin = new Padding(10),
                    Text = okText.Length == 0 ? "确定" : okText,
                    BackColor = SystemColors.ButtonFace,
                    Font = new Font(flp_Panel.Font.FontFamily, 16)
                };
                btn_Ok.Click += (sender, e) =>
                {
                    var f = ((Button)sender).FindForm();
                    if (f == null) return;
                    f.DialogResult = DialogResult.OK;
                    f.Close();
                };
                flp_Panel.Controls.Add(btn_Ok);

                if (cancelText.Length > 0)
                {
                    Button btn_Cancel = new Button
                    {
                        Name = "btnCancel",
                        Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                        Dock = DockStyle.Bottom,
                        Width = 140,
                        Height = 80,
                        Text = cancelText,
                        BackColor = SystemColors.ButtonFace,
                        Margin = new Padding(10),
                        Font = new Font(flp_Panel.Font.FontFamily, 16)
                    };
                    btn_Cancel.Click += (sender, e) =>
                    {
                        var f = ((Button)sender).FindForm();
                        if (f == null) return;
                        f.DialogResult = DialogResult.Cancel;
                        f.Close();
                    };
                    flp_Panel.Controls.Add(btn_Cancel);
                }

                if (AbortText.Length > 0)
                {
                    Button btn_Abort = new Button
                    {
                        Name = "btnAbort",
                        Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                        Dock = DockStyle.Bottom,
                        Width = 140,
                        Height = 80,
                        Text = AbortText,
                        Margin = new Padding(10),
                        BackColor = SystemColors.ButtonFace,
                        Font = new Font(flp_Panel.Font.FontFamily, 16)
                    };
                    btn_Abort.Click += (sender, e) =>
                    {
                        var f = ((Button)sender).FindForm();
                        if (f == null) return;
                        f.DialogResult = DialogResult.Abort;
                        f.Close();
                    };
                    flp_Panel.Controls.Add(btn_Abort);
                }

                TableLayoutPanel tlp_Panel = new TableLayoutPanel();
                tlp_Panel.Font = new Font(flp_Panel.Font.FontFamily, 36);
                tlp_Panel.Dock = DockStyle.Fill;
                tlp_Panel.ColumnCount = 1;
                tlp_Panel.RowCount = 1 + 1 + (info.Length > 0 ? 1 : 0);
                tlp_Panel.BorderStyle = BorderStyle.None;
                int rowIdx = 0;

                //警告标题
                tlp_Panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                tlp_Panel.Controls.Add(lb_Title);
                tlp_Panel.SetRow(lb_Title, rowIdx++);
                tlp_Panel.SetColumn(lb_Title, 0);

                //警告内容
                if (info.Length > 0)
                {
                    Label lb_Info = new Label
                    {
                        AutoSize = true,
                        Anchor = AnchorStyles.Left,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.TopLeft,
                        Text = info,
                        Font = new Font(flp_Panel.Font.FontFamily, 24)
                    };
                    tlp_Panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                    tlp_Panel.Controls.Add(lb_Info);
                    tlp_Panel.SetRow(lb_Info, rowIdx++);
                    tlp_Panel.SetColumn(lb_Info, 0);
                }

                //按键
                tlp_Panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                tlp_Panel.Controls.Add(flp_Panel);
                tlp_Panel.SetRow(flp_Panel, rowIdx);
                tlp_Panel.SetColumn(flp_Panel, 0);

                Form fr = new Form
                {
                    Name = "LogSearch",
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    Width = 600,
                    Height = 400,
                    Text = @"",
                    ShowIcon = false,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    TopMost = true,
                    StartPosition = FormStartPosition.CenterScreen,
                    BackColor = bkColor


                };
                fr.Activate();
                fr.Controls.Add(tlp_Panel);
                fr.ShowDialog();
                return fr.DialogResult;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var vi = NewSysInf.UserParams.bPickWsDis;
        }

        private void cbCheckModEn_CheckedChanged(object sender, EventArgs e)
        {
            if (!PT_SET.AutoChkEn)
            {
                MessageBox.Show("设置中未开启自动点检功能不能设置");
                return;
            }

            if (cbCheckModEn.Checked)
            {
                if (MessageBox.Show("是否直接开启自动点检并清空点检次数记录，必须确保已经清料才可以开始", "点检提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    WS.AutoChkCnt = 0;
                    foreach (var ws in COM.list_ws)
                    {
                        WS.TempAutoChkCnt = -1;
                        foreach (var md in ws.list_md)
                        {
                            md.bAutoChkOk = false;
                        }
                    }
                    VAR.isAutoChkMode = true;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("手动直接重新开启自动点检"));

                }
                else
                {
                    cbCheckModEn.Checked = false;
                }
            }
            else
            {
                VAR.isAutoChkMode = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Product.Tray.PosInf> placepos = new List<Product.Tray.PosInf>();
            Product.Tray tray = new Product.Tray(COM.traybox_ok.strCfgPath);
            COM.traybox_ok.tray_cur = tray;
            placepos = COM.traybox_ok.tray_cur.GetPosList(Product.EM_CM_RES.UNTEST);
            if (placepos.Count > 0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料，计算前位置{1}", COM.traybox_ok.disc, placepos[0].Pos[0].ToXY().ToString()));
            }

        }

        public void waring(int Numng1, double rate)
        {
            // 查询XML文档中的元素
            string xmlFilePath = VAR.gsys_set.GetCurProductPath + "ngcode.xml";
            XDocument xDoc = XDocument.Load(xmlFilePath);
            int targetId = Numng1;
            string textInfo = null;
            var item = xDoc.Root.Elements("代码").FirstOrDefault(e => (string)e.Attribute("名称") == targetId.ToString());

            var element = xDoc.Root.Elements().Where(e => e.Name.LocalName.StartsWith("Row") && (string)e.Attribute("代码") == targetId.ToString()).FirstOrDefault(); // 获取第一个匹配的元素  
            // 检查是否找到了对应的元素  
            if (element != null)
            {
                textInfo = (string)element.Attribute("名称");
            }

            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "良率报警!" : "Same NG", 20, true, ErrCode: ShowErrMsg.SeriesSameNGCode);
            MT.ST_WARN st_warn = new MT.ST_WARN();
            warning fr_warn = new warning();//增加语言
            st_warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Ti?p t?c ch?y");
            st_warn.ws = COM.ws1;
            st_warn.cancle_txt = MultiLanguage.TxtSelct("继续运行", "Stop running", "Ng?ng ch?y");
            st_warn.title = "良率报警";
            st_warn.msg = "请注意，不良项：" + textInfo==null? Numng1.ToString() : textInfo + "已经超过设置值:" + rate.ToString();
            st_warn.lb_msg = st_warn.msg + "点击继续运行后，将不再提示（4小时后重新提示）";
            string newmsg = PT_SET.EqpSN + "," + VAR.gsys_set.cur_product_name + "," + st_warn.msg;
            Msg.secsManager.Send(new BaseInfo() { Id = 246, Value = Encoding.UTF8.GetBytes(newmsg).Aggregate("", (current, next) => current + " " + next).TrimStart() });
            Msg.secsManager.Send(new BaseInfo() { Id = 10 }, TypeId: 2);
            DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
            if (DialogResult.Cancel == logres)
            {
            }

            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            SQLData.TestDataAddTime(0, 1, 1000);
            SQLData.TestDataAddTime(2, 1, 1000);
            SQLData.TestDataAddTime(2, 2, 2000);
            SQLData.TestDataAddTime(2, 3, 3000);
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            SQLData.TestDataAddTime(0, 1, 1000);
            SQLData.TestDataAddTime(2, 1, 1000);
            SQLData.TestDataAddTime(2, 2, 2000);
            SQLData.TestDataAddTime(2, 3, 3000);
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            SQLData.TestDataAddTime(0, 1, 1000);
            SQLData.TestDataAddTime(1, 1, 1000);
            SQLData.TestDataAddTime(2, 1, 1000);
            SQLData.TestDataAddTime(2, 2, 2000);
            SQLData.TestDataAddTime(2, 3, 3000);
        }

        private void button2_Click_4(object sender, EventArgs e)
        {
            //// 获取当前日期  
            //DateTime today = DateTime.Today;

            //// 设置时间为0点1分  
            //DateTime start = new DateTime(today.Year, today.Month, today.Day, 0, 1, 0);
            //DateTime stop = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //MessageBox.Show(start.ToString());
            //MessageBox.Show(stop.ToString());
            //foreach (WS.MdDat md in COM.ws1.list_md)
            //{

            //    md.res = 267;
            //    md.bardcode = "TEast";
            //}

            //SQLData.TestDataAdd(COM.ws1);
            //double rate = 0;
            //SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, 266);
            //waring(PT_SET.Numng2, PT_SET.Numngrate2);

            SQLData.TestDataAddClose(2, 62);

        }

        bool show1 = true;
        bool show2 = true;
        bool show3 = true;
        bool show4 = true;
        bool show5 = true;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (PT_SET.Ckngrate1)
            {
                double rate = 0;
                SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, PT_SET.Numng1);
                labnum1.Text = PT_SET.Numng1.ToString();
                labnumrate1.Text = (rate * 100).ToString() + "%";
                if (rate * 100 > PT_SET.Numngrate1)
                {
                    PT_SET.Ckngrate1war = true;
                    if (show1)
                    {
                        show1 = false;
                        waring(PT_SET.Numng1, PT_SET.Numngrate1);
                    }

                }
                else
                {
                    PT_SET.Ckngrate1war = false;
                }
            }
            else
            {
                PT_SET.Ckngrate1war = false;
                labnum1.Text = "";
                labnumrate1.Text = "";
            }

            if (PT_SET.Ckngrate2)
            {
                double rate = 0;
                SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, PT_SET.Numng2);
                labnum2.Text = PT_SET.Numng2.ToString();
                labnumrate2.Text = (rate * 100).ToString() + "%";
                if (rate * 100 > PT_SET.Numngrate2)
                {
                    PT_SET.Ckngrate2war = true;
                    if (show2)
                    {
                        show2 = false;
                        waring(PT_SET.Numng2, PT_SET.Numngrate2);
                    }
                }
                else
                {
                    PT_SET.Ckngrate2war = false;
                }
            }
            else
            {
                labnum2.Text = "";
                labnumrate2.Text = "";
                PT_SET.Ckngrate2war = false;
            }

            if (PT_SET.Ckngrate3)
            {
                double rate = 0;
                SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, PT_SET.Numng3);
                labnum3.Text = PT_SET.Numng3.ToString();
                labnumrate3.Text = (rate * 100).ToString() + "%";
                if (rate * 100 > PT_SET.Numngrate3)
                {
                    PT_SET.Ckngrate3war = true;
                    if (show3)
                    {
                        show3 = false;
                        waring(PT_SET.Numng3, PT_SET.Numngrate3);
                    }
                }
                else
                {
                    PT_SET.Ckngrate3war = false;
                }
            }
            else
            {
                labnum3.Text = "";
                labnumrate3.Text = "";
                PT_SET.Ckngrate3war = false;
            }

            if (PT_SET.Ckngrate4)
            {
                double rate = 0;
                SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, PT_SET.Numng4);
                labnum4.Text = PT_SET.Numng4.ToString();
                labnumrate4.Text = (rate * 100).ToString() + "%";
                if (rate * 100 > PT_SET.Numngrate4)
                {
                    PT_SET.Ckngrate4war = true;
                    if (show4)
                    {
                        show4 = false;
                        waring(PT_SET.Numng4, PT_SET.Numngrate4);
                    }
                }
                else
                {
                    PT_SET.Ckngrate4war = false;
                }
            }
            else
            {
                labnum4.Text = "";
                labnumrate4.Text = "";
                PT_SET.Ckngrate4war = false;
            }

            if (PT_SET.Ckngrate5)
            {
                double rate = 0;
                SQLData.TestDataSelectPro(FrMain.frcount.sqlSelector_count_data, out rate, PT_SET.Numng5);
                labnum5.Text = PT_SET.Numng5.ToString();
                labnumrate5.Text = (rate * 100).ToString() + "%";
                if (rate * 100 > PT_SET.Numngrate5)
                {
                    PT_SET.Ckngrate5war = true;
                    if (show5)
                    {
                        show5 = false;
                        waring(PT_SET.Numng5, PT_SET.Numngrate5);
                    }
                }
                else
                {
                    PT_SET.Ckngrate5war = false;
                }

            }
            else
            {
                labnum5.Text = "";
                labnumrate5.Text = "";
                PT_SET.Ckngrate5war = false;
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

         


            if (PT_SET.Ckngrate1 && PT_SET.Ckngrate1war)
            {
                waring(PT_SET.Numng1, PT_SET.Numngrate1);
            }

            if (PT_SET.Ckngrate2 && PT_SET.Ckngrate2war)
            {
                waring(PT_SET.Numng2, PT_SET.Numngrate2);
            }

            if (PT_SET.Ckngrate3 && PT_SET.Ckngrate3war)
            {
                waring(PT_SET.Numng3, PT_SET.Numngrate3);
            }

            if (PT_SET.Ckngrate4 && PT_SET.Ckngrate4war)
            {
                waring(PT_SET.Numng4, PT_SET.Numngrate4);
            }

            if (PT_SET.Ckngrate5 && PT_SET.Ckngrate5war)
            {
                waring(PT_SET.Numng5, PT_SET.Numngrate5);
            }

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            int ubenable = 0; //工位关闭数量
            int benable = 0; //工位开始数量
            foreach (WS temp in COM.list_ws)
            {
                foreach (WS.MdDat md in temp.list_md)
                {
                    if (!md.benable)
                    {
                        ubenable++;
                    }
                    else
                    {
                        benable++;
                    }
                }
            }
            labelopennum.Text = benable.ToString("f0");
            labelclosenum.Text = ubenable.ToString("f0");
            SQLData.TestDataAddClose(ubenable, benable);

            Msg.secsManager.Send((new BaseInfo() { Id = 244, Value = benable.ToString("f0") }));  //工位开始数量
            Msg.secsManager.Send((new BaseInfo() { Id = 245, Value = ubenable.ToString("f0") }));//工位关闭数量

        }

        Stopwatch stopwatch = new Stopwatch();
        bool show = false;
        private void timer4_Tick(object sender, EventArgs e)
        {
            btn_pause.Enabled = !IsChangingTrayBox();
            double secondss = stopwatchnew.ElapsedMilliseconds / 1000;

            if (secondss >= 41400)
            {
                stopwatchnew.Stop();
                DialogResult dr = Dialog(Color.Yellow, "警告!", "软件点检时间10分钟后超时，\r\n请清料准备点检！\r\n", "继续运行", "取消", "停止运行");
                if (dr == DialogResult.Abort)
                {
                    WS.bquit = true;
                    UpDownLoad.bquit = true;
                    MT.GPIO_OUT_ALM_BEEPER.SetOff();
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "---停止键按下---" : "---Press the stop key---      (---停止键按下---)");
                    DRpt.Report_Opration(1000, 0, VAR.IsChinese ? "---停止键按下---" : "---Press the stop key---      (---停止键按下---)");
                    VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                    if (VAR.gsys_set.status != EM_SYS_STA.RUN)
                    {

                        //VAR.gsys_set.bquit = true;
                        if (VAR.gsys_set.status != EM_SYS_STA.RESET)
                        {
                            COM.LeftLightBox.Stop();
                            COM.RightLightBox.Stop();
                            UpDownLoad.UD1Stop();
                            UpDownLoad.UD2Stop();
                            UpDownLoad.LCStop();
                            foreach (WS ws in COM.list_ws)
                            {
                                ws.ax_u.Stop();
                            }
                        }
                        else
                        {
                            MT.AllAxStop();
                        }
                    }
                }
            }

            if (VAR.gsys_set.status == EM_SYS_STA.STANDBY || IsChangingTrayBox())
            {
                show = false;
                stopwatch.Start();
            }
            else
            {
                stopwatch.Stop();
                show = true;
                var secondst = stopwatch.ElapsedMilliseconds / 1000;

                //if (secondst<=180 && secondst>5 && !PT_SET.Isshowmsgpre && !PT_SET.Isshowmsg)
                //{
                //    // 获取当前时间  
                //    DateTime now = DateTime.Now;

                //    if (secondst<=60)
                //    {
                //        double minute = (secondst / 60);
                //      // 创建一个表示1分钟的时间间隔  
                //      TimeSpan threeMinutes = TimeSpan.FromMinutes(minute);
                //        // 从当前时间中减去1分钟  
                //        DateTime threeMinutesAgo = now.Subtract(threeMinutes);
                //        PT_SET.WaitMode = 25;
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("（微停机）待机上传开始事件，原因为{0}", PT_SET.WaitMode));
                //        Msg.secsManager.Send(new BaseInfo() { Id = 250, Value = (DateTime.Now - threeMinutesAgo).TotalSeconds.ToString("F2") });
                //    }
                //    else
                //    {
                //        double minute = (secondst / 60);
                //        // 创建一个表示3分钟的时间间隔  
                //        TimeSpan threeMinutes = TimeSpan.FromMinutes(minute);
                //        // 从当前时间中减去3分钟  
                //        DateTime threeMinutesAgo = now.Subtract(threeMinutes);
                //        PT_SET.WaitMode = 26;
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("（微停机）待机上传开始事件，原因为{0}", PT_SET.WaitMode));
                //        Msg.secsManager.Send(new BaseInfo() { Id = 250, Value = (DateTime.Now - threeMinutesAgo).TotalSeconds.ToString("F2") });
                //    }
                //    Msg.secsManager.Send(new BaseInfo() { Id = 241, Value = PT_SET.WaitMode.ToString() });
                //    Msg.secsManager.Send(new BaseInfo() { Id = 242, Value = Encoding.UTF8.GetBytes(MesData.CobMesIdleStatusDict[PT_SET.WaitMode]).Aggregate("", (current, next) => current + " " + next).TrimStart() });
                //    Msg.secsManager.Send(new BaseInfo() { Id = 8 }, TypeId: 2);
                //    PT_SET.Isshowmsgpre = true;

                //    #region  待机细分

                //    if (PT_SET.Isshowmsgpre)
                //    {
                //        PT_SET.Isshowmsgpre = false;
                //        PT_SET.WaitMode = PT_SET.WaitMode == 0 ? 26 : PT_SET.WaitMode;
                //        Msg.secsManager.Send(new BaseInfo() { Id = 241, Value = PT_SET.WaitMode.ToString() });
                //        Msg.secsManager.Send(new BaseInfo() { Id = 242, Value = Encoding.UTF8.GetBytes(MesData.CobMesIdleStatusDict[PT_SET.WaitMode]).Aggregate("", (current, next) => current + " " + next).TrimStart() });
                //        Msg.secsManager.Send(new BaseInfo() { Id = 9 }, TypeId: 2);
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("（微停机）待机上传结束事件，原因为{0}", PT_SET.WaitMode));
                //        Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                //        Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                //    }

                //    #endregion
                //}

                stopwatch.Reset();

            }

            double seconds = stopwatch.ElapsedMilliseconds / 1000;

            if (seconds > 180)
            {
                stopwatch.Stop();

                if (!show && !PT_SET.Isshowmsg)
                {
                    DRpt.Report_Status(DReport.EmStatus.Wait, DReport.EmHareware.Null, DReport.EmStatus.Wait.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Wait).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                    // 获取当前时间  
                    DateTime now = DateTime.Now;
                    // 创建一个表示3分钟的时间间隔  
                    TimeSpan threeMinutes = TimeSpan.FromMinutes(3);
                    // 从当前时间中减去3分钟  
                    DateTime threeMinutesAgo = now.Subtract(threeMinutes);

                    Msg.secsManager.Send(new BaseInfo() { Id = 240, Value = threeMinutesAgo.ToString("yyyy-MM-dd HH:mm:ss") });
                    show = true;
                    COM.mesWaitWarning.WindowState = System.Windows.WindowState.Maximized;
                    PT_SET.WaitMode = COM.mesWaitWarning.OnShowDialog();

                    show = false;
                    stopwatch.Reset();
                    if (PT_SET.WaitMode == -1) return;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("待机上传开始事件，原因为{0}", PT_SET.WaitMode));
                    Msg.secsManager.Send(new BaseInfo() { Id = 250, Value = (DateTime.Now - threeMinutesAgo).TotalSeconds.ToString("F2") });
                    Msg.secsManager.Send(new BaseInfo() { Id = 241, Value = PT_SET.WaitMode.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 242, Value = Encoding.UTF8.GetBytes(MesData.CobMesIdleStatusDict[PT_SET.WaitMode]).Aggregate("", (current, next) => current + " " + next).TrimStart() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 8 }, TypeId: 2);
                    PT_SET.Isshowmsg = true;
                }
            }
        }

        Stopwatch stopwatchnew = new Stopwatch();
        private void ckb_Check_CheckedChanged(object sender, EventArgs e)
        {
            stopwatchnew.Reset();
            stopwatchnew.Start();
        }
    }
}
