using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;
using System.Reflection;
using System.Threading;
using DevReport;
using MySql.Data.MySqlClient;
using Win32Lib;


namespace UI
{
    public partial class FrMain : Form
    {
        public static FrCount frcount = new FrCount();
        public static FrSys frsys = new FrSys();
        public static FrRun frrun = new FrRun();
        public static FrProduct frproduct = new FrProduct();
        public static FrUser frsuser = new FrUser();
        public static FrRst frrst = new FrRst();
        public static FrMain frmain = null;

        //KeyboardHook k_hook = new KeyboardHook();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.Style |= 0x100000;
                //cp.Style |= 0x200000;
                //cp.Style |= 0x00000040;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public FrMain()
        {
            InitializeComponent();
            frmain = this;
            Msg.secsManager.Send(new BaseInfo() { Value = Path.GetFullPath("..") + @"\product" }, TypeId: 3);
            Msg.secsManager.ReceiveMsg += msg =>
            {
                if (msg.TypeId == 13)
                {
                    UpDownLoad.GetAllJigData();//发送一次夹具生产数据  最新需求不清空gy0123

                    //foreach (var ws in COM.list_ws)
                    //{
                    //    foreach (var md in ws.list_md)
                    //    {

                    //        md.cnt_ok = 0; md.cnt_ng_image = 0; md.cnt_ng_OS = 0; md.
                    //              cnt_ng_miss_pix = 0; md.cnt_ng_exposure = 0; md.cnt_ng_iic = 0; md.cnt_ng_other = 0;
                    //    }
                    //    ws.SaveCfg();
                    //}
                }
                else
                if (msg.TypeId == 3)
                {
                    Msg.secsManager.Send(new BaseInfo() { Value = Path.GetFullPath("..") + @"\product" }, TypeId: 3);
                }
                else if (msg.TypeId == 4)
                {

                    //if (msg.Infos.Count() != 2) return;
                    var result = 0;
                    //ret结果说明，0为成功
                    //0 - ok, completed 已完成
                    //1 - invalid command 无效的命令
                    //2 - cannot do now 现在不能做
                    //3 - parameter error 参数错误
                    //4 - initiated for asynchronous completion 以异步启动完成
                    //5 - rejected, already in desired condition 拒绝，已处于所需的状态
                    //6 - invalid object 无效对象

                    if (msg.Infos[0].Value.ToLower() == "start")
                    {
                        frrun.btn_run_Click(null, null);
                        //开始操作
                        result = 4;
                    }
                    else if (msg.Infos[0].Value.ToLower() == "stop")
                    {
                        //停止操作
                        frrun.btn_stop_Click(null, null);
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW, VAR.IsChinese ? "MES命令停机!" : "MES STOP");
                        result = 4;
                    }
                    else if (msg.Infos[0].Value.ToLower() == "startcheck")
                    {
                        MT.IsAllowStartUpdate = true;
                        if (Convert.ToInt32(msg.Infos[2].Value) == 0)
                            MT.IsAllowStart = false;
                        else if (Convert.ToInt32(msg.Infos[2].Value) == 1)
                            MT.IsAllowStart = true;
                        result = 0;
                    }
                    else if (msg.Infos[0].Value.ToLower() == "stripstartcheck")
                    {
                        //停止操作
                        result = 0;
                        if (msg.Infos[2].Value == "1")
                        {
                            MT.IsAllowStartByTray = true;
                            //允许
                        }
                        else if (msg.Infos[2].Value == "0")
                        {
                            MT.IsAllowStartByTray = false;
                            //不允许，停止加工，重新开始
                        }
                        MT.IsAllowStartUpdateByTray = true;
                    }

                    List<BaseInfo> msgs = new List<BaseInfo>();
                    msgs.Add(new BaseInfo() { Id = 1, Value = $"{result}" });
                    msgs.Add(new BaseInfo() { Id = 2, Value = msg.Infos[1].Value });
                    Msg.secsManager.SendRange(msgs, TypeId: 4);
                }
            };
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Control.IsKeyLocked(Keys.NumLock) == true) return;
            ////check mouse posInf
            //if (!this.GetTopLevel()) return;
            //if (Control.MousePosition.X < 700 || Control.MousePosition.Y > 70) return;

            //if ((VAR.gsys_set.status != CONST.SYS_STATUS_STANDBY) && (VAR.gsys_set.status != CONST.SYS_STATUS_PAUSE)) return;

            //VAR.gsys_set.bquit = false;
            ////左右按键
            //if (e.KeyData == Keys.Left)
            //{
            //    MT.AXIS_X.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_N);
            //    e.Handled = true;
            //}
            //else if (e.KeyData == Keys.Right)
            //{
            //    MT.AXIS_X.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_P);
            //    e.Handled = true;
            //}

            ////上下按键
            //if (e.KeyData == Keys.Up)
            //{
            //    MT.AXIS_Y.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_N);
            //    e.Handled = true;
            //}
            //else if (e.KeyData == Keys.Down)
            //{
            //    MT.AXIS_Y.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_P);
            //    e.Handled = true;
            //}

            ////上页下页按键
            //if (e.KeyData == Keys.PageUp)
            //{
            //    MT.AXIS_R.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_N);
            //    e.Handled = true;
            //}
            //else if (e.KeyData == Keys.PageDown)
            //{
            //    MT.AXIS_R.JOG_VMove(ref VAR.gsys_set.bquit, AXIS.DIR_P);
            //    e.Handled = true;
            //}
            ////手动速度切换
            //if (e.KeyData == Keys.End)
            //{
            //    if (MT.AXIS_X.spd_cur == MT.AXIS_X.spd_manual_high)
            //    {
            //        foreach (AXIS ax in MT.AxisList) ax.SetToManualLowSpd();
            //    }
            //    else
            //    {
            //        foreach (AXIS ax in MT.AxisList) ax.SetToManualHighSpd();
            //    }
            //    e.Handled = true;
            //}
        }

        public void ProductEn(User.PERMISSION pms)
        {

        }


        private void hook_KeyUp(object sender, KeyEventArgs e)
        {
            //if (Control.IsKeyLocked(Keys.NumLock) == true) return;
            if (VAR.gsys_set.status != EM_SYS_STA.STANDBY && VAR.gsys_set.status != EM_SYS_STA.PAUSE) return;

            if (e.KeyData == Keys.Add)
            {
                if (tsc_step_sel.SelectedIndex < (tsc_step_sel.Items.Count - 1)) tsc_step_sel.SelectedIndex++;
                e.Handled = true;
            }

            else if (e.KeyData == Keys.Subtract)
            {
                if (tsc_step_sel.SelectedIndex > 0) tsc_step_sel.SelectedIndex--;
                e.Handled = true;
            }

            if ((e.KeyData == Keys.Left) || (e.KeyData == Keys.Right) || (e.KeyData == Keys.Up) || (e.KeyData == Keys.Down) || (e.KeyData == Keys.PageUp) || (e.KeyData == Keys.PageDown))
            {
                //MT.AXIS_X.JOG_Stop();
                //MT.AXIS_Y.JOG_Stop();
                //MT.AXIS_R.JOG_Stop();
                e.Handled = true;
            }
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private Form FindForm(string formName)
        {
            foreach (Form form in Application.OpenForms)//获得所有打开的窗体
            {
                if (form.Name == formName)
                {
                    return form;
                }
            }
            return null;
        }
        private void rbtn_sys_CheckedChanged(object sender, EventArgs e)
        {
            //Form form = new FrSys();
            //pnl_sub.Controls.Clear();
            //form.TopLevel = false;
            //form.FormBorderStyle = FormBorderStyle.None;
            //pnl_sub.Controls.Add(form);
            //form.Left = 0;
            //form.Top = 0;
            //form.Show();
        }
        public void form_sel(string btn_name, string page_name = "", string page_name2 = "")
        {
            //    if (frrun != null) frrun.timer_update.Enabled = false;
            //    if (frsys != null) frsys.timer_update.Enabled = false;

            Font ft = new Font("Microsoft Sans Serif", 18, FontStyle.Bold);
            rbtn_run.Font = ft;
            rbtn_product.Font = ft;
            rbtn_sys.Font = ft;
            rbtn_count.Font = ft;

            rbtn_run.ForeColor = Color.DarkGray;
            rbtn_product.ForeColor = Color.DarkGray;
            rbtn_sys.ForeColor = Color.DarkGray;
            rbtn_count.ForeColor = Color.DarkGray;

            rbtn_run.BackColor = Color.Transparent;
            rbtn_product.BackColor = Color.Transparent;
            rbtn_sys.BackColor = Color.Transparent;
            rbtn_count.BackColor = Color.Transparent;

            Form form = null;
            ft = new Font("Microsoft Sans Serif", 22, FontStyle.Bold);

            //if (frsys == null) frsys = new FrSys();
            if (frsys != null) frsys.bupdate = false;
            //if (frrst == null) frrst = new FrRst();
            if (frrst != null) frrst.bupdate = false;
            //if (frproduct == null) frproduct = new FrProduct();
            if (frproduct != null) frproduct.bupdate = false;
            //if (frrun == null) frrun = new FrRun();
            if (frrun != null) frrun.bupdate = false;

            if (frsuser != null) frsuser.bupdate = false;
            switch (btn_name)
            {
                default:
                case "rbtn_run":
                    rbtn_run.Checked = true;
                    rbtn_run.ForeColor = Color.WhiteSmoke;
                    rbtn_run.Font = ft;
                    if (frrun == null) frrun = new FrRun();
                    form = frrun;
                    frrun.bupdate = true;
                    foreach (Cam cam in COM.ListCam) cam.mCogRecDisplay = frrun.cogDisplayer_run.cogRecordDisplay;
                    break;
                case "rbtn_product":
                    rbtn_product.Checked = true;
                    rbtn_product.ForeColor = Color.WhiteSmoke;
                    rbtn_product.Font = ft;
                    if (frproduct == null) frproduct = new FrProduct();
                    form = frproduct;
                    frproduct.bupdate = true;
                    foreach (Cam cam in COM.ListCam) cam.mCogRecDisplay = frproduct.cogDisplayer_product.cogRecordDisplay;

                    ////page select
                    //if (frproduct.ctb_prodcut.TabPages[page_name] != null) frproduct.ctb_prodcut.TabPages[page_name].Select();
                    //if (page_name == "tb_tg_cfg")
                    //{
                    //    //VisionRun.Display = new VisionDisplay(frproduct.cogRecordDisplay_live, "");
                    //    if (frproduct.ctb_tg_view.TabPages[page_name2] != null) frproduct.ctb_tg_view.TabPages[page_name2].Select();
                    //}
                    //else if (page_name == "tb_tg_vs")
                    //{
                    //    //VisionRun.Display = new VisionDisplay(frproduct.DisPlayAndImageMask1.CogRecordDisplay, "");
                    //    if (frproduct.ctb_vs_cfg.TabPages[page_name2] != null) frproduct.ctb_vs_cfg.TabPages[page_name2].Select();
                    //}
                    //else if (page_name == "tb_ofs")
                    //{
                    //    //VisionRun.Display = new VisionDisplay(frproduct.cogRecordDisplay_ofs, "");
                    //    if (frproduct.ctb_ofs.TabPages[page_name2] != null) frproduct.ctb_ofs.TabPages[page_name2].Select();
                    //}
                    break;
                case "rbtn_count":
                    rbtn_count.Checked = true;
                    rbtn_count.ForeColor = Color.WhiteSmoke;
                    rbtn_count.Font = ft;
                    if (frcount == null) frcount = new FrCount();
                    form = frcount;
                    break;
                case "rbtn_sys":
                    rbtn_sys.Checked = true;
                    rbtn_sys.ForeColor = Color.WhiteSmoke;
                    rbtn_sys.Font = ft;
                    if (frsys == null) frsys = new FrSys();
                    form = frsys;
                    frsys.bupdate = true;
                    foreach (Cam cam in COM.ListCam) cam.mCogRecDisplay = frsys.CogRecordDisplay_sys.cogRecordDisplay;
                    ////page select
                    //if (frsys.ctb_sys.TabPages[page_name] != null) frsys.ctb_sys.TabPages[page_name].Select();
                    //if (page_name == "tb_cali")
                    //{
                    //    if (frsys.ctb_cali.TabPages[page_name2] != null) frsys.ctb_cali.TabPages[page_name2].Select();
                    //}
                    //frsys.timer_update.Enabled = true;
                    //if (VisionRun.Display.m_strName != "frsysCogRecordDisplay")
                    //    VisionRun.Display = new VisionDisplay(frsys.CogRecordDisplay, "frsysCogRecordDisplay");
                    break;
                case "rbtn_user":
                    rbtn_user.Checked = true;
                    rbtn_user.ForeColor = Color.WhiteSmoke;
                    rbtn_user.Font = ft;
                    if (frsuser == null) frsuser = new FrUser();
                    form = frsuser;
                    frsuser.bupdate = true;
                    break;

                case "rbtn_rst":
                    rbtn_rst.Checked = true;
                    rbtn_rst.ForeColor = Color.WhiteSmoke;
                    rbtn_rst.Font = ft;
                    if (frrst == null) frrst = new FrRst();
                    form = frrst;
                    frrst.bupdate = true;
                    if (!PT_SET.LbEn)
                    {
                        frrst.pnl_lightbox_left.Visible = false;
                        frrst.pnl_lightbox_right.Visible = false;
                    }
                    break;
            }

            if (form == null) return;
            pnl_sub.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            pnl_sub.Controls.Add(form);
            form.Left = 0;
            form.Top = 0;
            form.Width = pnl_sub.Width;
            form.Height = pnl_sub.Height - 8;
            form.Show();
        }

        private void FrMain_Load(object sender, EventArgs e)
        {
            DRpt.Report_CreateTable();
            EM_RES ret;
            LanguageInit();

            Msg.secsManager.Start();
            VAR.msg.ShowMsgCfg(1000, (Msg.EM_MSGTYPE)0xffff);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("系统启动...") : "System start ...        (系统启动...)");
            DRpt.Report_Status(DReport.EmStatus.StartUp, DReport.EmHareware.Null, DReport.EmStatus.StartUp.GetDescription(VAR.IsChinese));
            Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.StartUp).ToString() });
            Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
            lbVer.Text = string.Format("WLTmini2   V{0}", Assembly.GetExecutingAssembly().GetName().Version);
            //load sys config
            VAR.gsys_set.LoadSysCfg();
            VAR.gsys_set.status = EM_SYS_STA.UNKOWN;
            VAR.gsys_set.bclose = false;
        
            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "正在加载" : "Loading", 2, true);
            PT_SET.LoadPtCfg(VAR.gsys_set.cur_product_name);
            NewSysInf.LoadSysInfCfg(out var msg);

            //加载产品
            try
            {
                if (COM.NGDef == null)
                    COM.NGDef = new NGCodeDef();
                COM.NGDef.LoadCfg();

                if (COM.product == null) COM.product = new Product();
                //ret = COM.product.LoadDat(VAR.gsys_set.cur_product_name);
                //if (ret != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "产品数据加载失败!");
                //else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "产品数据加载成功!");


                //加载吸头
                COM.XtInit(VAR.gsys_set.cur_product_name);
                //加载仓储
                COM.TrayBoxInit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //硬件初始化
            Task TaskHWInit = new Task(() =>
            {
                ret = MT.Init(Path.GetFullPath("..") + "\\syscfg\\");
                if (ret != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "板卡初始化失败!" : "Board initialization failed!         (板卡初始化失败!)", DReport.EmErrCode.InitFailed, (int)DReport.EmHareware.Card);
                else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "板卡初始化失败!" : "Board initialization failed!         (板卡初始化失败!)");
            }
            );
            TaskHWInit.Start();


            UpDownLoad.DisAxisX = Math.Abs(COM.xt1.st_pos_samevs[0].x) + Math.Abs(COM.xt1.st_pos_samevs[1].x);
            if (UpDownLoad.DisAxisX < 400)
                UpDownLoad.DisAxisX = 400;
            //相机初始化
            //Task TaskCamInit = new Task(() =>
            //{
            //    ret = MT.Init(Path.GetFullPath("..") + "\\syscfg\\");
            //    if (ret != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "板卡始化失败!");
            //    else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "板卡始化成功!");
            //}
            ret = COM.CamInit();
            if (ret != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "相机初始化失败!" : "Camera initialization failed!         (相机初始化失败!)", DReport.EmErrCode.InitFailed, (int)DReport.EmHareware.Cam);
            else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "相机初始化失败!" : "Camera initialization failed!         (相机初始化失败!)");
            Task TaskTrayInit = new Task(() =>
            {
                COM.tray_fd = new Product.Tray(COM.traybox_fd.strCfgPath, 0);
                COM.tray_ok = new Product.Tray(COM.traybox_ok.strCfgPath, 0);
                COM.tray_ng = new Product.Tray(COM.traybox_ng.strCfgPath, 0);
            }
            );
            TaskTrayInit.Start();
            //上下料模块
            COM.UDLoad1 = new UpDownLoad(0, "上下料模块1", "UpDwLoad1", ref COM.CamDw1, ref COM.CamUp1, ref MT.AXIS_UDL1_X, ref MT.AXIS_UDL1_Y,
            ref MT.AXIS_UDL1_Z, ref MT.AXIS_UDL1_U1, ref MT.AXIS_UDL1_U2, ref COM.ListXT1);

            COM.UDLoad2 = new UpDownLoad(1, "上下料模块2", "UpDwLoad2", ref COM.CamDw2, ref COM.CamUp2, ref MT.AXIS_UDL2_X, ref MT.AXIS_UDL2_Y,
            ref MT.AXIS_UDL2_Z, ref MT.AXIS_UDL2_U1, ref MT.AXIS_UDL2_U2, ref COM.ListXT2);

            COM.List_UDLoad = new List<UpDownLoad>() { COM.UDLoad1, COM.UDLoad2 };

            UpDownLoad.DisAxisX = Math.Abs(COM.xt1.st_pos_samevs[0].x) + Math.Abs(COM.xt1.st_pos_samevs[1].x);
            if (UpDownLoad.DisAxisX < 400)
                UpDownLoad.DisAxisX = 400;

            //PT_SET.LoadPtCfg(VAR.gsys_set.cur_product_name);

            foreach (WS ws in COM.list_ws)
            {
                //if (ws.num == 1 || ws.num == 3)
                {
                    ws.LoadCfg();
                }
            }

            if (PT_SET.Y1En)
            {
                MT.AXIS_BOX_L_Y1 = new AXIS(4, MT.CARD_ECI2600_1, "左光箱Y1", "LB_Y1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
            }
            MT.AxList_BOX_LEFT = new List<AXIS> { MT.AXIS_BOX_L_X1, MT.AXIS_BOX_L_X2, MT.AXIS_BOX_L_Y1, MT.AXIS_BOX_L_Z1, MT.AXIS_BOX_L_Z2 };
            COM.LeftLightBox = new LightBox("LeftLightBox", "左光箱", "LeftLightBox", MT.AXIS_BOX_L_X1, MT.AXIS_BOX_L_X2, MT.AXIS_BOX_L_Y1, MT.AXIS_BOX_L_Z1, MT.AXIS_BOX_L_Z2);

            if (PT_SET.RY1En)
            {
                MT.AXIS_BOX_R_Y1 = new AXIS(4, MT.CARD_ECI2400_2, "右光箱Y1", "RB_Y1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
            }
            MT.AxList_BOX_RIGHT = new List<AXIS> { MT.AXIS_BOX_R_X1, MT.AXIS_BOX_R_X2, MT.AXIS_BOX_R_Y1, MT.AXIS_BOX_R_Z1, MT.AXIS_BOX_R_Z2 };
            COM.RightLightBox = new LightBox("RightLightBox", "右光箱", "RightLightBox", MT.AXIS_BOX_R_X1, MT.AXIS_BOX_R_X2, MT.AXIS_BOX_R_Y1, MT.AXIS_BOX_R_Z1, MT.AXIS_BOX_R_Z2);


            Application.DoEvents();
            Thread.Sleep(10);

            timer_reconnect.Enabled = true;
            if (frrun != null) frrun.bupdate = true;

            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "待回零" : "Wait Home", 10, true);

            if (MT.isReady)
            {
                ////钩子侦测按键           
                //k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下
                //k_hook.KeyUpEvent += new KeyEventHandler(hook_KeyUp);//钩住键按下
                //k_hook.Start();//安装键盘钩子
            }
            COUNT_DATA.LoadCountCfg(VAR.gsys_set.cur_product_name);
            MT.SetSafeFunc();
            MT.GPIO_OUT_TT_REV.ChkSafe = Turntable.ChkSafe;
            MT.GPIO_OUT_TT_FWD.ChkSafe = Turntable.ChkSafe;
            MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
            //clear data
            Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\log\\", -30);
            Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\CsvData\\", -5);
            foreach (Cam cam in COM.ListCam)
            {
                Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\Image\\" + cam.mName + "\\", -5, false);
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? ("设备当前版本:" + lbVer.Text) : ("Device current version:        (设备当前版本:)" + lbVer.Text));
            //tooltip
            tlp_Show.Active = true;
            //tlp_Show.AutoPopDelay = 5000;
            //tlp_Show.InitialDelay = 500;
            //tlp_Show.AutoPopDelay = 500;
            //tlp_Show.ReshowDelay = 100;
            tlp_Show.SetToolTip(pictureBox1, VAR.IsChinese ? "点击显示设备维护框!" : "Click to show the equipment maintenance box!    (点击显示设备维护框!)");
            tlp_Show.SetToolTip(lbVer, VAR.IsChinese ? "点击显示设备管理系统!" : "Click to display the device management system!     (点击显示设备管理系统!)");
            tlp_Show.SetToolTip(lb_ver, VAR.IsChinese ? "点击软件最小化!" : "Click software to minimize!    (点击软件最小化!)");
            tlp_Show.SetToolTip(lb_Date, VAR.IsChinese ? "点击显示上下料轴控制!" : "Click to show the loading and unloading axis control!    (点击显示上下料轴控制!)");
            tlp_Show.SetToolTip(lb_Time, VAR.IsChinese ? "点击显示上下料轴控制!" : "Click to show the loading and unloading axis control!    (点击显示上下料轴控制!)");
            tlp_Show.SetToolTip(lbl_update, VAR.IsChinese ? "点击检测软件是否有新版本!" : "Click to detect if there is a new version of the software!    (点击检测软件是否有新版本!)");

            DRpt.Report_Status(DReport.EmStatus.Unkown, DReport.EmHareware.Null, DReport.EmStatus.Unkown.GetDescription(VAR.IsChinese));
            Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Unkown).ToString() });
            Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
            //Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";

            Msg.secsManager.Send(new BaseInfo() { Value = Path.GetFullPath("..") + @"\product" }, TypeId: 3);
            string vv = "1";
            if (PT_SET.bJigSan) vv = "0";
            Msg.secsManager.Send(new BaseInfo() { Id = 9, Value = vv });//夹具扫码开启上报

        }
        private void rbtn_run_Click(object sender, EventArgs e)
        {

            switch (((RadioButton)sender).Name)
            {
                case "rbtn_run":
                    form_sel(((RadioButton)sender).Name);
                    break;
                case "rbtn_product":
                    if (frproduct == null) frproduct = new FrProduct();
                    switch (frproduct.ctb_product.SelectedTab != null ? frproduct.ctb_product.SelectedTab.Name : "")
                    {
                        case "tb_tg_cfg":
                            form_sel(((RadioButton)sender).Name, frproduct.ctb_product.SelectedTab.Name);
                            break;
                        case "tb_tg_vs":
                            form_sel(((RadioButton)sender).Name, frproduct.ctb_product.SelectedTab.Name);
                            break;
                        case "tb_ofs":
                            form_sel(((RadioButton)sender).Name, frproduct.ctb_product.SelectedTab.Name);
                            break;
                        default:
                            form_sel(((RadioButton)sender).Name, frproduct.ctb_product.SelectedTab.Name);
                            break;
                    }
                    break;
                case "rbtn_sys":
                    if (frsys == null) frsys = new FrSys();
                    if (frsys.ctb_sys.SelectedTab != null)
                        form_sel(((RadioButton)sender).Name, frsys.ctb_sys.SelectedTab.Name);
                    break;
                case "rbtn_count":
                    form_sel(((RadioButton)sender).Name);
                    break;
                default:
                    form_sel(((RadioButton)sender).Name);
                    break;
            }

        }


        private EM_SYS_STA SysStatusTemp;
        SysTimeCnt CurSysTimeCnt = new SysTimeCnt();
        List<SysTimeCnt> ListSysTimeCnt = new List<SysTimeCnt>();
        string CurHourStr = "";
        bool bEditionUpdate = false;//更新版本信息
        private void timer_update_Tick(object sender, EventArgs e)
        {

            try
            {
                DateTime curDateTime = DateTime.Now;
                TimeSpan sp = curDateTime.Subtract(Convert.ToDateTime(PT_SET.Lastcleantime));
                if (PT_SET.bCleanen)
                {
                    if (sp.Minutes >= PT_SET.Cleaninterval)
                    {
                        if (VAR.gsys_set.status == EM_SYS_STA.RUN && VAR.ClearMt == false)
                        {
                            PT_SET.Lastcleantime = curDateTime.ToString();
                            PT_SET.SavePtCfg(VAR.gsys_set.cur_product_name);
                            if (MessageBox.Show("夹具清洗时间已到，是否进行清料", "夹具清洗提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                DRpt.Report_Opration(1000, 0, "清料按键按下!");
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("清料") : "Clear   (清料)");
                                VAR.ClearMt = true;
                            }
                        }
                        else
                        {
                            PT_SET.Lastcleantime = curDateTime.ToString();
                            MessageBox.Show("请进行夹具清洗!");
                        }
                    }
                }
                lb_Date.Text = System.DateTime.Today.ToString("yyyy-MM-dd");
                lb_Time.Text = System.DateTime.Now.ToString("HH:mm:ss");
                if (frsuser != null)
                {
                    if (frsuser.user1.cur_user.bUpdate == true)
                    {
                        frsuser.user1.cur_user.bUpdate = false;
                        if (frproduct != null)
                            frproduct.PmsfrProduct(frsuser.user1.cur_user.pms);
                        if (frsys != null)
                            frsys.PmsfrSys(frsuser.user1.cur_user.pms);
                        if (frrun != null)
                            frrun.PmsfrRun(frsuser.user1.cur_user.pms);
                    }
                }


                if (SysStatusTemp != VAR.gsys_set.status)
                {
                    SysStatusTemp = VAR.gsys_set.status;
                    if (VAR.gsys_set.status == EM_SYS_STA.RUN)
                    {
                        rbtn_rst.Enabled = false;
                        rbtn_product.Enabled = false;
                        // rbtn_count.Enabled = false;
                        frrun.btn_stop.BackColor = SystemColors.ButtonFace;
                        frrun.btn_run.BackColor = Color.Lime;
                        rbtn_sys.Enabled = false;
                        rbtn_user.Enabled = false;
                        if (frsuser != null)
                        {
                            frsuser.user1.cur_user.pms = User.PERMISSION.None;
                            frsuser.user1.cur_user.password = "";
                            frsuser.user1.tb_pw.Text = "";
                            frsuser.user1.cur_user.bUpdate = true;
                        }
                    }
                    else
                    {
                        frrun.btn_stop.BackColor = Color.OrangeRed;
                        frrun.btn_run.BackColor = SystemColors.ButtonFace;
                        rbtn_rst.Enabled = true;
                        rbtn_product.Enabled = true;
                        rbtn_count.Enabled = true;
                        rbtn_sys.Enabled = true;
                        rbtn_user.Enabled = true;
                    }
                }
               
                if (DateTime.Now.ToString("HH:mm:ss") == PT_SET.CheckTimeMorning ||
                    DateTime.Now.ToString("HH:mm:ss") == PT_SET.CheckTimeEvening)
                {
                    DRpt.Report_Opration(1000, 0, "点检时间已到!");
                    if (VAR.gsys_set.status == EM_SYS_STA.RUN && VAR.ClearMt == false)
                    {
                        Thread.Sleep(1000);
                        if (MessageBox.Show("点检时间已到，是否进行清料，点检后请在主界面设备信息界面点击点检上传信息！", "点检提醒", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            DRpt.Report_Opration(1000, 0, "清料按键按下!");
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("清料") : "Clear   (清料)");
                            VAR.ClearMt = true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        MessageBox.Show("请进行点检，并在主界面设备信息界面点击点检上传信息！");
                    }
              
                }
                if (DateTime.Now.Hour%2==1&& bEditionUpdate==false)
                {
                    bEditionUpdate = true;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? ("设备当前版本:" + lbVer.Text) : ("Device current version:        (设备当前版本:)" + lbVer.Text));
                }
               else if (DateTime.Now.Hour % 2 != 1&& bEditionUpdate==true)
                {
                    bEditionUpdate = false;
                }
              
            }
            catch
            {

            }

        }
        int cnt = 0, savecnt = 0;
        public double tck = -1;
        int JisSendTimeCnt = 0;
        public DReport.EmStatus SendStauts = DReport.EmStatus.Run;
        bool bhavesend = false;
        private async void timer_key_Tick(object sender, EventArgs e)
        {
            timer_key.Interval = 100;
            int i;
            var curHour = DateTime.Now.Hour;
            if (curHour == 9 )
            {
                if (!bhavesend)
                {
                    bhavesend = true;
                    string vv = "0";
                    if (PT_SET.bJigSan) vv = "1";                  
                    Msg.secsManager.Send(new BaseInfo() { Id = 9, Value = vv });
                }
            }
            else
                bhavesend = false;
            if (MT.GPIO_IN_KEY_START.isON)
            {
                i = 0;
                Thread.Sleep(30);
                if (MT.GPIO_IN_KEY_START.isON)
                {
                    MT.GPIO_OUT_ALM_BEEPER.SetOff();
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("---开始键按下---") : "--- Press the start key ---       (---开始键按下---)");
                    DRpt.Report_Opration(1000, 0, "开始键按下!");
                    if (VAR.gsys_set.status == EM_SYS_STA.REPAIR)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("提示:当前设备正在维修，无法运行!") : "提示:当前设备正在维修，无法运行!    (Tip: The current device is being repaired and cannot run!)");
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

                    if (!PT_SET.IsMesLocal)
                    {
                        Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                        Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                        await Task.Run(() =>
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "正在等待MES上位指令!");
                            SpinWait.SpinUntil(() => MT.IsAllowStartUpdate, 10000);
                        });
                        MT.IsAllowStartUpdate = false;
                        //fr?.Close();
                        if (!MT.IsAllowStart)
                        {
                            FrRun.Dialog(Color.Yellow, "警告", "被MES禁止启动！请联系相关人员。");
                            return;
                        }
                    }
                    //检测复位状态
                    foreach (CARD card in TempCardList)
                    {
                        if (card.isReady == false)
                        {
                            MessageBox.Show(VAR.IsChinese ? string.Format("{0}未初始化!", card.disc) : string.Format("{0} Uninitialized!!\r\n未初始化!", card.disc), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        foreach (AXIS ax in card.AxList)
                        {
                            if (ax.home_status != AXIS.HOME_STA.OK)
                            {
                                MessageBox.Show(VAR.IsChinese ? string.Format("{0} 状态异常，{1}!\r\n请先复位", ax.disc, ax.home_status) : string.Format("Abnormal {0} status,{1}! \r\n Please reset first{0} \r\n 状态异常，{1}!\r\n请先复位", ax.disc, ax.home_status), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    //检测运行状态
                    if (VAR.gsys_set.status != EM_SYS_STA.RUN)
                    {

                        VAR.gsys_set.bquit = false;
                        UpDownLoad.bquit = false;
                        WS.bquit = false;
                        WS.bpause = false;
                        BaseAction.run();
                    }
                    else
                    {
                        try
                        {
                            warning fr;
                            fr = (warning)FindForm("warning");
                            if (fr != null)
                            {
                                fr.btn_ok.BeginInvoke(new Action(() =>
                                {
                                    fr.btn_ok.PerformClick();
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? ("fr.btn_ok.PerformClick错误:" + ex.ToString()) : ("fr.btn_ok.PerformClick err:" + ex.ToString()), DReport.EmErrCode.ToolError);
                        }



                    }
                    while (MT.GPIO_IN_KEY_START.isON)
                    {
                        if (i++ > 600)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("等待开始按键松开大于3秒！") : "Wait for the start button to release for more than 3 seconds!     (等待开始按键松开大于3秒！)", DReport.EmErrCode.ToolError);
                            MessageBox.Show(VAR.IsChinese ? string.Format("等待开始按键松开大于3秒！") : "Wait for the start button to release for more than 3 seconds!\r\n等待开始按键松开大于3秒!");
                            break;
                        }
                        Thread.Sleep(5);
                        Application.DoEvents();
                    }
                }
            }

            //stop
            if (MT.GPIO_IN_KEY_STOP.isON)
            {
                i = 0;
                Thread.Sleep(30);
                if (MT.GPIO_IN_KEY_STOP.isON)
                {
                    DRpt.Report_Opration(1000, 0, "停止键按下!");
                    // VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("停止按键按下"));
                    //Action.stop();
                    if (frrun != null)
                        frrun.btn_stop.PerformClick();

                    while (MT.GPIO_IN_KEY_STOP.isON)
                    {
                        if (i++ > 600)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("等待停止按键松开大于3秒！"), DReport.EmErrCode.ToolError);
                            MessageBox.Show(VAR.IsChinese ? "等待停止按键松开大于3秒！" : "Wait for the stop button to release for more than 3 seconds!\r\n等待停止按键松开大于3秒！");
                            break;
                        }
                        Thread.Sleep(5);
                        Application.DoEvents();
                    }
                }
            }

            //stop
            if (MT.GPIO_IN_KEY_RESET.isON)
            {
                i = 0;
                Thread.Sleep(30);
                if (MT.GPIO_IN_KEY_RESET.isON)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("---复位键按下---") : "--- Press the reset key ---        (---复位键按下---)");
                    DRpt.Report_Opration(1000, 0, VAR.IsChinese ? string.Format("---复位键按下---") : "--- Press the reset key ---        (---复位键按下---)");
                    //Action.stop();
                    form_sel("rbtn_rst");
                    frrst.btn_all_home.PerformClick();
                }
                while (MT.GPIO_IN_KEY_RESET.isON)
                {
                    if (i++ > 600)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("等待复位按键松开大于3秒！") : "Wait for the reset button to release for more than 3 seconds!      (等待复位按键松开大于3秒！)", DReport.EmErrCode.ToolError);
                        MessageBox.Show(VAR.IsChinese ? string.Format("等待复位按键松开大于3秒！") : "Wait for the reset button to release for more than 3 seconds!\r\n等待复位按键松开大于3秒！");
                        break;
                    }
                    Thread.Sleep(5);
                    Application.DoEvents();
                }
            }

            if (!BaseAction.bRuning)
            {
                
                MT.GPIO_OUT_ALM_GREEN.SetOff();//gy0123非运行状态亮黄灯，绿灯灭
                if (MT.GPIO_OUT_ALM_RED.isOFF)
                MT.GPIO_OUT_ALM_YELLOW.SetOn();
            }
            SYS_INF.bRuning = BaseAction.bRuning;
            //if (VAR.gsys_set.status == EM_SYS_STA.RUN && !MT.IsErrAlm)
            //{
            //    if (cnt >= 10)
            //    {
            //        cnt = 0;
            //        COUNT_DATA.runtime++;
            //    }

            //}
            // else
            //{

             if (VAR.gsys_set.status != EM_SYS_STA.RUN)
            {
                cnt++;
                if (cnt >= 10)
                {
                    cnt = 0;
                    savecnt++;
                    //COUNT_DATA.waittime++;
                    if (savecnt >= 6)
                    {
                        savecnt = 0;
                        COUNT_DATA.SaveCountCfg(VAR.gsys_set.cur_product_name);
                        Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\log\\", -30);
                        Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\CsvData\\", -5);
                        foreach (Cam cam in COM.ListCam)
                        {
                            Utility.CatchDiskAndDeleteFile(null, Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\Image\\" + cam.mName + "\\", -5, false);
                        }
                    }

                }

            }

            if (tck == -1) tck = DateTime.Now.Ticks;
            double addtime = (DateTime.Now.Ticks - tck) / 10000000.0;
            tck = DateTime.Now.Ticks;
          //  if (VAR.gsys_set.status == EM_SYS_STA.RUN && !VAR.IsErrAlm && !COM.traybox_fd.ChgML && !COM.traybox_ng.ChgML && !COM.traybox_ok.ChgML)
             if (BaseAction.bRuning && (VAR.sys_inf.info.Contains("运行")|| VAR.sys_inf.info.Contains("RUN")))
            {

                COUNT_DATA.runtime += addtime;
            }
            else
            {
                COUNT_DATA.waittime += addtime;
            }

            if (VAR.gsys_set.status == EM_SYS_STA.RUN &&
                (COM.traybox_fd.ChgML || COM.traybox_ng.ChgML || COM.traybox_ok.ChgML))
            {
                COUNT_DATA.waitwltime += addtime;

                MT.GPIO_OUT_ALM_YELLOW.SetOn();//待料黄灯
                // DRpt.Report_Status(DReport.EmStatus.Wait, DReport.EmHareware.Null, DReport.EmStatus.Wait.GetDescription());
            }
            else
            {
                if (VAR.gsys_set.status == EM_SYS_STA.RUN)
                    MT.GPIO_OUT_ALM_YELLOW.SetOff();//
            }

            //状态发送
            if (VAR.gsys_set.status == EM_SYS_STA.RUN)
            {
                if (Msg.AlarmInfo.Count > 0)
                {
                    List<Alarm> tempAlarmInfo = new List<Alarm>();
                    foreach (var ErrInf in Msg.AlarmInfo)
                        tempAlarmInfo.Add(ErrInf);
                    if (tempAlarmInfo.Count > 0)
                    {
                        foreach (var info in tempAlarmInfo)
                        {
                            Msg.secsManager.Send(new BaseInfo() { Id = info.Alarmid + 1, Value = "false" }, 1);
                            info.EndTime = DateTime.Now;
                            SQLData.AlarmTestDataAdd(info);
                        }
                        Msg.AlarmInfo.Clear();
                    }
                }

                if (SendStauts != DReport.EmStatus.Wait &&
                    (COM.traybox_fd.ChgML || COM.traybox_ng.ChgML || COM.traybox_ok.ChgML))
                {
                    SendStauts = DReport.EmStatus.Wait;
                    DRpt.Report_Status(DReport.EmStatus.Wait, DReport.EmHareware.Null, DReport.EmStatus.Wait.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Wait).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                }
                else if (SendStauts != DReport.EmStatus.Error && VAR.IsErrAlm && !COM.traybox_fd.ChgML && !COM.traybox_ng.ChgML && !COM.traybox_ok.ChgML)
                {
                    SendStauts = DReport.EmStatus.Error;
                    DRpt.Report_Status(DReport.EmStatus.Error, DReport.EmHareware.Null, DReport.EmStatus.Error.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Error).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                }
                else if (SendStauts != DReport.EmStatus.Run && !VAR.IsErrAlm && !COM.traybox_fd.ChgML && !COM.traybox_ng.ChgML && !COM.traybox_ok.ChgML)
                {
                    SendStauts = DReport.EmStatus.Run;
                    DRpt.Report_Status(DReport.EmStatus.Run, DReport.EmHareware.Null, DReport.EmStatus.Run.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                }
                JisSendTimeCnt++;
                if (JisSendTimeCnt > (PT_SET.JigCntSend * (60000 / timer_key.Interval)))//半个小时发一次工站夹具生产数据gy0123
                {
                    JisSendTimeCnt = 0;
                    if (PT_SET.bJigSan)
                        UpDownLoad.GetAllJigData();//发送一次夹具生产数据               
                }


            }
        }

        private void lb_Date_Click(object sender, EventArgs e)
        {
            //if (VAR.gsys_set.status == CONST.SYS_STATUS_STANDBY)
            if (!ts_manual.Visible && !ts_manual2.Visible)
            {
                ts_manual.Top = pnl_sub.Top;
                ts_manual.Left = tbl_main.Width - ts_manual.Width;
                ts_manual.Visible = !ts_manual.Visible;
                if (tsc_step_sel.SelectedIndex < 0 || tsc_step_sel.SelectedIndex >= tsc_step_sel.Items.Count) tsc_step_sel.SelectedIndex = 1;
                else tsc_step_sel.SelectedIndex = tsc_step_sel2.SelectedIndex;
            }
            else if (ts_manual.Visible && !ts_manual2.Visible)
            {
                ts_manual.Visible = !ts_manual.Visible;
                ts_manual2.Top = pnl_sub.Top;
                ts_manual2.Left = tbl_main.Width - ts_manual2.Width;
                ts_manual2.Visible = !ts_manual2.Visible;
                tsc_step_sel2.SelectedIndex = tsc_step_sel.SelectedIndex;
            }
            else if (!ts_manual.Visible && ts_manual2.Visible)
            {
                ts_manual2.Visible = !ts_manual2.Visible;
                frrun.cTabControl1.SelectedTab.Refresh();
            }
        }
        private void StrToAxis(String pch, ref AXIS axis, ref AXIS.AX_DIR dir)
        {
            if (true == pch.Equals("tsb_1xp"))
            {
                axis = MT.AXIS_UDL1_X;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_1xn"))
            {
                axis = MT.AXIS_UDL1_X;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_1yp"))
            {
                axis = MT.AXIS_UDL1_Y;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_1yn"))
            {
                axis = MT.AXIS_UDL1_Y;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_1zp"))
            {
                axis = MT.AXIS_UDL1_Z;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_1zn"))
            {
                axis = MT.AXIS_UDL1_Z;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_1u1p"))
            {
                axis = MT.AXIS_UDL1_U1;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_1u1n"))
            {
                axis = MT.AXIS_UDL1_U1;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_1u2p"))
            {
                axis = MT.AXIS_UDL1_U2;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_1u2n"))
            {
                axis = MT.AXIS_UDL1_U2;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_2xp"))
            {
                axis = MT.AXIS_UDL2_X;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_2xn"))
            {
                axis = MT.AXIS_UDL2_X;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_2yp"))
            {
                axis = MT.AXIS_UDL2_Y;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_2yn"))
            {
                axis = MT.AXIS_UDL2_Y;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_2zp"))
            {
                axis = MT.AXIS_UDL2_Z;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_2zn"))
            {
                axis = MT.AXIS_UDL2_Z;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_2u1p"))
            {
                axis = MT.AXIS_UDL2_U1;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_2u1n"))
            {
                axis = MT.AXIS_UDL2_U1;
                dir = AXIS.AX_DIR.N;
            }
            else if (true == pch.Equals("tsb_2u2p"))
            {
                axis = MT.AXIS_UDL2_U2;
                dir = AXIS.AX_DIR.P;
            }
            else if (true == pch.Equals("tsb_2u2n"))
            {
                axis = MT.AXIS_UDL2_U2;
                dir = AXIS.AX_DIR.N;
            }
            else
            {
                axis = null;
                dir = AXIS.AX_DIR.N;
            }

        }
        double step = 0.1;
        private void tsb_xn_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            AXIS ax = null;
            AXIS.AX_DIR dir = AXIS.AX_DIR.P;
            StrToAxis(btn.Name, ref ax, ref dir);
            if (ax == null) return;
            //z限制1mm步进
            if ((ax.id == MT.AXIS_UDL1_Z.id || ax.id == MT.AXIS_UDL2_Z.id) && step > 1) step = 1;
            ax.JOG_Step(ref VAR.gsys_set.bquit, dir, step);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, btn.Text + " down," + ax.disc);
            Thread.Sleep(500);
        }

        private void tsc_step_sel_Click(object sender, EventArgs e)
        {

        }

        private void tsb_xn_MouseUp(object sender, MouseEventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            AXIS ax = null;
            AXIS.AX_DIR dir = AXIS.AX_DIR.P;
            StrToAxis(btn.Text, ref ax, ref dir);
            if (ax == null) return;
            ax.JOG_Stop();
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, btn.Text + " up," + ax.disc);
        }

        private void tsb_stop_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = true;
            MT.AllAxStop();
        }

        private void FrMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            frcount.SaveSysTimeCnt();
            if (VAR.gsys_set.status == EM_SYS_STA.RUN)
            {
                if (MessageBox.Show(VAR.IsChinese ? "运行中，是否要停止?" : "Do you want to stop during operation?\r\n运行中，是否要停止?", VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    for (int n = 0; n < 100; n++)
                    {
                        VAR.gsys_set.bquit = true;
                        VAR.gsys_set.bpause = false;
                        //VAR.gsys_set.bclose = true;
                        Thread.Sleep(10);
                        Application.DoEvents();
                    }
                    VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                    return;
                }
            }
            if (MessageBox.Show(VAR.IsChinese ? "是确定要关闭软件?" : "Are you sure you want to close the software?", VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                DRpt.Report_Status(DReport.EmStatus.Quit, DReport.EmHareware.Null, DReport.EmStatus.Quit.GetDescription(VAR.IsChinese));
                Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Quit).ToString() });
                Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                Msg.secsManager.Send(new BaseInfo(), 10);
                Task.Delay(10).Wait();
                Msg.secsManager.Stop();
                MT.GPIO_OUT_ALM_GREEN.SetOff();
                MT.GPIO_OUT_ALM_RED.SetOff();
                MT.GPIO_OUT_ALM_YELLOW.SetOff();
                MT.GPIO_OUT_ALM_BEEPER.SetOff();
                Task.Delay(10).Wait();
                //k_hook.KeyDownEvent -= hook_KeyDown;//钩住键按下
                //k_hook.KeyUpEvent -= hook_KeyUp;//钩住键按下
                //k_hook.Stop();//安装键盘钩子
                //Acquistion.AllCameraDisconnect();

                BaseAction.Close();
                Environment.Exit(Environment.ExitCode);
            }
            else e.Cancel = true;

        }

        private void tsc_step_sel_SelectedIndexChanged(object sender, EventArgs e)
        {
            double[] step_array = { 0.01, 0.1, 1, 10 };

            try
            {
                if (ts_manual2.Visible)
                {
                    step = step_array[tsc_step_sel2.SelectedIndex];
                }
                else
                {
                    step = step_array[tsc_step_sel.SelectedIndex];
                }
            }
            catch
            {
                MessageBox.Show(VAR.IsChinese ? "步距数据出错!" : "Step data error!\r\n步距数据出错!");
                step = 0.1;
            }
            //foreach (AXIS ax in MT.AxisListExceptFd) ax.manual_step = step;
        }

        private async void timer_reconnect_Tick(object sender, EventArgs e)
        {
            try
            {
                ((System.Windows.Forms.Timer)sender).Enabled = false;
                Task Reconnect = new Task(() =>
                {
                    MT.ChkAndReConnect();
                }
                );
                Reconnect.Start();
                await Reconnect;
            }
            finally
            {
                ((System.Windows.Forms.Timer)sender).Enabled = true;
            }
        }

        private void Minimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private bool bin = false;
        private async void lbVer_Click(object sender, EventArgs e)
        {

            if (VAR.gsys_set.status == EM_SYS_STA.RUN || bin) return;
            bin = true;
            string udPath = AppDomain.CurrentDomain.BaseDirectory + "DM.exe";
            Task tsk = new Task(() =>
            {
                if (File.Exists(udPath))
                {
                    //获取当前版本
                    string curVer = string.Format(@"{0}.{1}", Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);
                    //获取当前线程名，编译把程序名称改为项目名称，如WLTmini,OLT等
                    string progressName = Assembly.GetExecutingAssembly().GetName().Name;
                    //参数{sn,tabtext,hostIp,curVer,launch,progressName,prjname}
                    //sn:指定序列号，空为本机硬盘序号
                    //tabtext:指定页面，空为默认页面，版本管理(默认)/反馈记录/设备信息/用户管理/设备报告
                    //hostIp:服务器IP,空为自动获取
                    //curVer：当前版本，发现高于当前版本时高亮显示可用版本
                    //launch: 升级后启动的程序路径
                    //progressName：升级前要关闭的线程
                    //prjName：项目名称，一般与progressName设为一致
                    var p = Process.Start(udPath, string.Format(",,,{0},,{1},{2}", curVer, progressName, progressName));
                    if (p != null) p.WaitForExit();
                }
            });
            tsk.Start();
            await tsk;
            bin = false;
        }

        private bool bein = false;
        private async void lbl_update_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN || bein) return;
            bein = true;
            int tt = Environment.TickCount;
            Task tsk = new Task(() =>
            {
                //每小时在设备空闲时候检查，耗时约1秒，最后用线程查询，返回1说明有可更新版本
                string udPath = AppDomain.CurrentDomain.BaseDirectory + "update.exe";
                if (File.Exists(udPath))
                {
                    //获取当前版本
                    string curVer = string.Format(@"{0}.{1}", Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);
                    //{sn,hostIp,curVer,launch,progressName,mode,prjname}
                    var p = Process.Start(udPath, string.Format(",,{0},,,chk,{1}", curVer, Assembly.GetExecutingAssembly().GetName().Name));
                    if (p != null) p.WaitForExit();
                    //Text = $@"{p.ExitCode},{Environment.TickCount - tt} ms";
                    if (p.ExitCode == 1) MessageBox.Show(VAR.IsChinese ? string.Format(@"有新版本可用!({0}ms)!", Environment.TickCount - tt) : string.Format(@"A new version is available!({0}ms)!", Environment.TickCount - tt), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else if (p.ExitCode == 0) MessageBox.Show(VAR.IsChinese ? string.Format(@"已经是新版本!({0}ms)!", Environment.TickCount - tt) : string.Format(@"Already new version!({0}ms)!", Environment.TickCount - tt), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else if (p.ExitCode == -5) MessageBox.Show(VAR.IsChinese ? string.Format(@"项目不存在!({0}ms)!", Environment.TickCount - tt) : string.Format(@"Item does not exist!({0}ms)!", Environment.TickCount - tt), VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else MessageBox.Show(@"查询出错!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            tsk.Start();
            await tsk;
            bein = false;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN) return;
            EM_SYS_STA sys_status = VAR.gsys_set.status;
            VAR.gsys_set.status = EM_SYS_STA.REPAIR;
            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "设备维修!" : "Equ upkeep", 20, true);
            MT.ST_WARN warn = new MT.ST_WARN();
            warning fr_warn = new warning();//增加语言
            warn.ok_txt = MultiLanguage.TxtSelct("确定", "OK", "VÂNG");
            warn.ws = null;
            warn.title = MultiLanguage.TxtSelct("提示:设备维修!", "Tip: Equipment maintenance!", "Mẹo: Bảo trì thiết bị!");
            warn.msg = MultiLanguage.TxtSelct("当前设备在维修状态", "The current equipment is in maintenance status!", "Thiết bị hiện tại đang trong tình trạng bảo trì!");
            
            warn.lb_msg = MultiLanguage.TxtSelct(
                "提示：当前设备正在维修，请确认设备内没有维修人员或QC巡检确认人员后再关闭此对话框",
                "Tip: The current equipment is being repaired, please confirm that there are no maintenance personnel in the equipment before closing this dialog box!",
                "Mẹo: Thiết bị hiện tại đang được sửa chữa, vui lòng xác nhận rằng không có nhân viên bảo trì trong thiết bị trước khi đóng hộp thoại này! ");
            MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.Null, false);
            VAR.gsys_set.status = sys_status;
        }

        private void LanguageInit()
        {
            string lang = MultiLanguage.GetDefaultLanguage();
            LanguageSwitch(this, lang);
            LanguageSwitch(frrun, lang);
            LanguageSwitch(frcount, lang);
            LanguageSwitch(frproduct, lang);
            LanguageSwitch(frrst, lang);
            LanguageSwitch(frsuser, lang);
            LanguageSwitch(frsys, lang);
        }
        /// <summary>
        /// 语言选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void languageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VAR.gsys_set.status == EM_SYS_STA.RUN || frsuser.user1.cur_user.pms < User.PERMISSION.SuperAdmin) return;

            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            string lang = "";
            if (menu == chineseToolStripMenuItem)
            {
                lang = Enum.GetName(typeof(enumLanguage), 0);
                chineseToolStripMenuItem.Checked = true;
                englishToolStripMenuItem.Checked = false;
                vietnameseToolStripMenuItem.Checked = false;

                frrun.cogDisplayer_run.toolStripLabel4.Text = "显示模式";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "相机列表";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "流程编辑";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "曝光(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "亮度(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "对比度(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "保存参数";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "图像对中";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "保存图片";
                frrun.cogDisplayer_run.tsb_continue.Text = "连续测量";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "显示模式";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "相机列表";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "流程编辑";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "曝光(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "亮度(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "对比度(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "保存参数";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "图像对中";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "保存图片";
                frproduct.cogDisplayer_product.tsb_continue.Text = "连续测量";
            }
            else if (menu == englishToolStripMenuItem)
            {
                lang = Enum.GetName(typeof(enumLanguage), 1);
                chineseToolStripMenuItem.Checked = false;
                englishToolStripMenuItem.Checked = true;
                vietnameseToolStripMenuItem.Checked = false;

                frrun.cogDisplayer_run.toolStripLabel4.Text = "Display mode";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "Cam list";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "Edit";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "Exposure(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "Brightness(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "Contrast(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "Save cfg";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "Center";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "Save img";
                frrun.cogDisplayer_run.tsb_continue.Text = "Live";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "Display mode";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "Cam list";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "Edit";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "Exposure(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "Brightness(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "Contrast(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "Save cfg";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "Center";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "Save img";
                frproduct.cogDisplayer_product.tsb_continue.Text = "Live";
            }
            else if (menu == vietnameseToolStripMenuItem)
            {
                lang = Enum.GetName(typeof(enumLanguage), 2);
                chineseToolStripMenuItem.Checked = false;
                englishToolStripMenuItem.Checked = false;
                vietnameseToolStripMenuItem.Checked = true;
                frrun.cogDisplayer_run.toolStripLabel4.Text = "显示模式";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "相机列表";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "流程编辑";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "曝光(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "亮度(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "对比度(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "保存参数";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "图像对中";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "保存图片";
                frrun.cogDisplayer_run.tsb_continue.Text = "连续测量";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "显示模式";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "相机列表";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "流程编辑";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "曝光(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "亮度(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "对比度(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "保存参数";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "图像对中";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "保存图片";
                frproduct.cogDisplayer_product.tsb_continue.Text = "连续测量";
            }

            LanguageSwitch(this, lang);
            LanguageSwitch(frrun, lang);
            LanguageSwitch(frcount, lang);
            LanguageSwitch(frproduct, lang);
            LanguageSwitch(frrst, lang);
            LanguageSwitch(frsuser, lang);
            LanguageSwitch(frsys, lang);


        }


        private void LanguageSwitch(Form form, string lang)
        {
            if (!MultiLanguage.LoadLanguage(form, lang))
            {
                return;
            }
            MultiLanguage.SetDefaultLanguage(lang);
            if (lang == Enum.GetName(typeof(enumLanguage), 0))
            {
                chineseToolStripMenuItem.Checked = true;
                englishToolStripMenuItem.Checked = false;
                vietnameseToolStripMenuItem.Checked = false;

                VAR.IsChinese = true;
                MultiLanguage.CurrentLanguage = enumLanguage.Chinese;
                frsys.affineCail_Dw1ToUp1.lbl_affine_place.Text = "吸头1放料位";
                frsys.affineCail_Dw1ToUp1.lbl_affine_upcam.Text = "上相机1拍照位";
                frsys.affineCail_Dw1ToUp1.lbl_affine_downcam.Text = "下相机1拍照位";
                frsys.affineCail_Dw2ToUp2.lbl_affine_place.Text = "吸头3放料位";
                frsys.affineCail_Dw2ToUp2.lbl_affine_upcam.Text = "上相机2拍照位";
                frsys.affineCail_Dw2ToUp2.lbl_affine_downcam.Text = "下相机2拍照位";
                frsys.npointCail1.lbl_unit_cam.Text =
                frsys.npointCail1.cb_UDload.SelectedIndex == 0 ? "上相机1拍照位" : "上相机2拍照位";
                frrun.cogDisplayer_run.toolStripLabel4.Text = "显示模式";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "相机列表";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "流程编辑";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "曝光(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "亮度(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "对比度(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "保存参数";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "图像对中";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "保存图片";
                frrun.cogDisplayer_run.tsb_continue.Text = "连续测量";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "显示模式";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "相机列表";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "流程编辑";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "曝光(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "亮度(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "对比度(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "保存参数";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "图像对中";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "保存图片";
                frproduct.cogDisplayer_product.tsb_continue.Text = "连续测量";

            }
            else if (lang == Enum.GetName(typeof(enumLanguage), 1))
            {
                chineseToolStripMenuItem.Checked = false;
                englishToolStripMenuItem.Checked = true;
                vietnameseToolStripMenuItem.Checked = false;

                VAR.IsChinese = false;
                MultiLanguage.CurrentLanguage = enumLanguage.English;
                frsys.affineCail_Dw1ToUp1.lbl_affine_place.Text = "XT1 Place";
                frsys.affineCail_Dw1ToUp1.lbl_affine_upcam.Text = "UpCam1";
                frsys.affineCail_Dw1ToUp1.lbl_affine_downcam.Text = "DwCam1";
                frsys.affineCail_Dw2ToUp2.lbl_affine_place.Text = "XT3 Place";
                frsys.affineCail_Dw2ToUp2.lbl_affine_upcam.Text = "UpCam2";
                frsys.affineCail_Dw2ToUp2.lbl_affine_downcam.Text = "DwCam2";
                frsys.npointCail1.lbl_unit_cam.Text =
                frsys.npointCail1.cb_UDload.SelectedIndex == 0 ? "UpCam1" : "UpCam2";
                frrun.cogDisplayer_run.toolStripLabel4.Text = "Display mode";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "Cam list";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "Edit";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "Exposure(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "Brightness(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "Contrast(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "Save cfg";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "Center";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "Save img";
                frrun.cogDisplayer_run.tsb_continue.Text = "Live";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "Display mode";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "Cam list";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "Edit";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "Exposure(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "Brightness(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "Contrast(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "Save cfg";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "Center";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "Save img";
                frproduct.cogDisplayer_product.tsb_continue.Text = "Live";
            }
            else if (lang == Enum.GetName(typeof(enumLanguage), 2))
            {
                chineseToolStripMenuItem.Checked = false;
                englishToolStripMenuItem.Checked = false;
                vietnameseToolStripMenuItem.Checked = true;

                VAR.IsChinese = false;
                MultiLanguage.CurrentLanguage = enumLanguage.Vietnamese;
                frsys.affineCail_Dw1ToUp1.lbl_affine_place.Text = "XT1 Place";
                frsys.affineCail_Dw1ToUp1.lbl_affine_upcam.Text = "UpCam1";
                frsys.affineCail_Dw1ToUp1.lbl_affine_downcam.Text = "DwCam1";
                frsys.affineCail_Dw2ToUp2.lbl_affine_place.Text = "XT3 Place";
                frsys.affineCail_Dw2ToUp2.lbl_affine_upcam.Text = "UpCam2";
                frsys.affineCail_Dw2ToUp2.lbl_affine_downcam.Text = "DwCam2";
                frsys.npointCail1.lbl_unit_cam.Text =
                frsys.npointCail1.cb_UDload.SelectedIndex == 0 ? "UpCam1" : "UpCam2";
                frrun.cogDisplayer_run.toolStripLabel4.Text = "Display mode";
                frrun.cogDisplayer_run.tsb_cam_list.Text = "Cam list";
                frrun.cogDisplayer_run.tsb_block_edit.Text = "Edit";
                frrun.cogDisplayer_run.toolStripLabel5.Text = "Exposure(ms)";
                frrun.cogDisplayer_run.toolStripLabel2.Text = "Brightness(0-1)";
                frrun.cogDisplayer_run.toolStripLabel3.Text = "Contrast(0-1)";
                frrun.cogDisplayer_run.tsb_save.Text = "Save cfg";
                frrun.cogDisplayer_run.tsb_GoCenter.Text = "Center";
                frrun.cogDisplayer_run.tsb_save_screen.Text = "Save img";
                frrun.cogDisplayer_run.tsb_continue.Text = "Live";
                frproduct.cogDisplayer_product.toolStripLabel4.Text = "Display mode";
                frproduct.cogDisplayer_product.tsb_cam_list.Text = "Cam list";
                frproduct.cogDisplayer_product.tsb_block_edit.Text = "Edit";
                frproduct.cogDisplayer_product.toolStripLabel5.Text = "Exposure(ms)";
                frproduct.cogDisplayer_product.toolStripLabel2.Text = "Brightness(0-1)";
                frproduct.cogDisplayer_product.toolStripLabel3.Text = "Contrast(0-1)";
                frproduct.cogDisplayer_product.tsb_save.Text = "Save cfg";
                frproduct.cogDisplayer_product.tsb_GoCenter.Text = "Center";
                frproduct.cogDisplayer_product.tsb_save_screen.Text = "Save img";
                frproduct.cogDisplayer_product.tsb_continue.Text = "Live";
            }


        }




        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            Msg.secsManager.DealMessage(m);
            base.WndProc(ref m);
        }
    }
}
