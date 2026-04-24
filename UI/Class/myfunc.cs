using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;
using System.Threading.Tasks;
using Cognex.VisionPro;
using DevReport;
using Win32Lib;
using UI.Class;
using MesUIWpf;
using System.Text;
using System.Linq;
using MesUIWpf.Models;
using ControlzEx.Standard;
using static SerialCommander;
using static UI.WS;

namespace UI
{
    public delegate void SET_STAUS(EM_ALM_STA sta, string str, int beep = 100, bool bset = false);

    #region COM

    public static class COM
    {
        //NG代码
        public static NGCodeDef NGDef = new NGCodeDef();
        //工站
        public static WS ws1 = new WS(0, MT.AXIS_WS1_U, MT.List_CLD_WS1_FR, MT.List_CLD_WS1_BK, MT.List_CLD_WS1, MT.GPIO_OUT_WS1_GZ_POWER, MT.List_GPIO_OUT_WS1_ZK, MT.GPIO_OUT_WS1_Wind);

        public static WS ws2 = new WS(1, MT.AXIS_WS2_U, MT.List_CLD_WS2_FR, MT.List_CLD_WS2_BK, MT.List_CLD_WS2, MT.GPIO_OUT_WS2_GZ_POWER, MT.List_GPIO_OUT_WS2_ZK, MT.GPIO_OUT_WS2_Wind);

        public static WS ws3 = new WS(2, MT.AXIS_WS3_U, MT.List_CLD_WS3_FR, MT.List_CLD_WS3_BK, MT.List_CLD_WS3, MT.GPIO_OUT_WS3_GZ_POWER, MT.List_GPIO_OUT_WS3_ZK, MT.GPIO_OUT_WS3_Wind);

        public static WS ws4 = new WS(3, MT.AXIS_WS4_U, MT.List_CLD_WS4_FR, MT.List_CLD_WS4_BK, MT.List_CLD_WS4, MT.GPIO_OUT_WS4_GZ_POWER, MT.List_GPIO_OUT_WS4_ZK, MT.GPIO_OUT_WS4_Wind);

        public static List<WS> list_ws = new List<WS>() { ws1, ws2, ws3, ws4 };
        public static bool Cannottest = false;
        public static SunnyQr Sunnnyqr0 = new SunnyQr(0);
        public static SunnyQr Sunnnyqr1 = new SunnyQr(1);
        public static Product.Tray tray_fd, tray_ok, tray_ng;
        public static MesWaitWarning mesWaitWarning = new MesWaitWarning(MesProductType.MINI);


        public static TrayBox traybox_fd = new TrayBox("TrayBox_FD", "供料仓", TrayBox.EM_ROLE.FEED, TrayBox.EM_DIR.IN_OUT, 10, MT.AXIS_UDL_FD_X,
            MT.AXIS_UDL_FD_Z, MT.GPIO_IN_UL_INP_FD_TRAYBOX, MT.GPIO_IN_UL_RDY_FD_TRAY, MT.GPIO_OUT_UL_FD_TRAY,
            MT.GPIO_IN_UDL_FD_TRAY_ON, MT.CLD_UDL_FDTRAY_HD);

        public static TrayBox traybox_ok = CreateTrayBoxOk();

        public static TrayBox traybox_ng = CreateTrayBoxNg();

        public static List<TrayBox> List_traybox = new List<TrayBox>() { traybox_fd, traybox_ok, traybox_ng };

        public static string GetStartupModeDescription()
        {
            return RuntimeMachineMode.IsTrayBoxSwapped ? "料仓互换版" : "普通版本";
        }

        public static string DescribeTrayBoxMapping(TrayBox traybox)
        {
            string cfgFile = Path.GetFileName(traybox.strCfgPath);
            return string.Format("{0}映射: 逻辑={1}, 物理轴={2}/{3}, 配置={4}",
                traybox.disc,
                traybox.role,
                traybox.ax_x.disc,
                traybox.ax_z.disc,
                cfgFile);
        }

        private static TrayBox CreateTrayBoxOk()
        {
            if (RuntimeMachineMode.IsTrayBoxSwapped)
            {
                return new TrayBox("TrayBox_OK", "OK料仓", TrayBox.EM_ROLE.OK, TrayBox.EM_DIR.IN_OUT, 10, MT.AXIS_UDL_NG_X,
                    MT.AXIS_UDL_NG_Z, MT.GPIO_IN_DL_INP_NG_TRAYBOX, MT.GPIO_IN_DL_RDY_NG_TRAY, MT.GPIO_OUT_DL_NG_TRAY,
                    MT.GPIO_IN_UDL_NG_TRAY_ON, MT.CLD_UDL_NGTRAY_HD);
            }

            return new TrayBox("TrayBox_OK", "OK料仓", TrayBox.EM_ROLE.OK, TrayBox.EM_DIR.IN_OUT, 10, MT.AXIS_UDL_OK_X,
                MT.AXIS_UDL_OK_Z, MT.GPIO_IN_DL_INP_OK_TRAYBOX, MT.GPIO_IN_DL_RDY_OK_TRAY, MT.GPIO_OUT_DL_OK_TRAY,
                MT.GPIO_IN_UDL_OK_TRAY_ON, MT.CLD_UDL_OKTRAY_HD);
        }

        private static TrayBox CreateTrayBoxNg()
        {
            if (RuntimeMachineMode.IsTrayBoxSwapped)
            {
                return new TrayBox("TrayBox_NG", "NG料仓", TrayBox.EM_ROLE.NG, TrayBox.EM_DIR.IN_OUT, 10, MT.AXIS_UDL_OK_X,
                    MT.AXIS_UDL_OK_Z, MT.GPIO_IN_DL_INP_OK_TRAYBOX, MT.GPIO_IN_DL_RDY_OK_TRAY, MT.GPIO_OUT_DL_OK_TRAY,
                    MT.GPIO_IN_UDL_OK_TRAY_ON, MT.CLD_UDL_OKTRAY_HD);
            }

            return new TrayBox("TrayBox_NG", "NG料仓", TrayBox.EM_ROLE.NG, TrayBox.EM_DIR.IN_OUT, 10, MT.AXIS_UDL_NG_X,
                MT.AXIS_UDL_NG_Z, MT.GPIO_IN_DL_INP_NG_TRAYBOX, MT.GPIO_IN_DL_RDY_NG_TRAY, MT.GPIO_OUT_DL_NG_TRAY,
                MT.GPIO_IN_UDL_NG_TRAY_ON, MT.CLD_UDL_NGTRAY_HD);
        }

        //组装轴1相机
        public static Cam CamUp1 = new Cam("CamUp1", "上相机1", true, false, false, MT.CamUp1Triger, MT.MoveHandle,
            MT.GPIO_OUT_UL_CAM_UP1);
        public static Cam CamDw1 = new Cam("CamDw1", "下相机1", false, true, false, MT.CamDw1Triger, MT.MoveHandle,
            MT.GPIO_OUT_UL_CAM_DW1);

        //组装轴2相机
        public static Cam CamUp2 = new Cam("CamUp2", "上相机2", true, false, false, MT.CamUp2Triger, MT.MoveHandle,
            MT.GPIO_OUT_UL_CAM_UP2);
        public static Cam CamDw2 = new Cam("CamDw2", "下相机2", false, true, false, MT.CamDw2Triger, MT.MoveHandle,
            MT.GPIO_OUT_UL_CAM_DW2);

        // public static List<Cam> ListCam1 = new List<Cam>() { CamUp1,CamDw1 };
        //public static List<Cam> ListCam2 = new List<Cam>() { CamUp2,CamDw2 };
        public static List<Cam> ListUpCam = new List<Cam>() { CamUp1, CamUp2 };
        public static List<Cam> ListDwCam = new List<Cam>() { CamDw1, CamDw2 };
        public static List<Cam> ListCam = new List<Cam>() { CamUp1, CamDw1, CamUp2, CamDw2 };

        //目标
        public static Product product = new Product();

        //左光箱
        public static LightBox LeftLightBox;// = new LightBox("LeftLightBox", "左光箱", MT.AXIS_BOX_L_X1, MT.AXIS_BOX_L_X2, MT.AXIS_BOX_L_Y1,
                                            // MT.AXIS_BOX_L_Z1, MT.AXIS_BOX_L_Z2);

        //右光箱
        public static LightBox RightLightBox;// = new LightBox("RightLightBox", "右光箱","RightLightBox", MT.AXIS_BOX_R_X1, MT.AXIS_BOX_R_X2,null,
                                             // MT.AXIS_BOX_R_Z1, MT.AXIS_BOX_R_Z2);

        //OTP光箱
        public static LightBox OTPLightBox =
            new LightBox("OTPLightBox", "OTP光箱", "OTPLightBox", null, null, null, null, null, MT.AXIS_BOX_OTP_Z);

        //空光箱
        public static LightBox LightBox =
            new LightBox("LightBox", "光箱", "LightBox", null, null, null, null, null, null);

        //吸头
        //上料模块0(机台内部)
        public static XT xt1 = new XT(0, ref MT.AXIS_UDL1_X, ref MT.AXIS_UDL1_Y, ref MT.AXIS_UDL1_Z, ref MT.AXIS_UDL1_U1,
            ref COM.CamUp1, ref COM.CamDw1, MT.CLD_UDL1_N1, MT.GPIO_OUT_UDL1_PZK_N1);

        public static XT xt2 = new XT(1, ref MT.AXIS_UDL1_X, ref MT.AXIS_UDL1_Y, ref MT.AXIS_UDL1_Z, ref MT.AXIS_UDL1_U2,
            ref COM.CamUp1, ref COM.CamDw1, MT.CLD_UDL1_N2, MT.GPIO_OUT_UDL1_PZK_N2);
        //上料模块1(机台外部)
        public static XT xt3 = new XT(2, ref MT.AXIS_UDL2_X, ref MT.AXIS_UDL2_Y, ref MT.AXIS_UDL2_Z, ref MT.AXIS_UDL2_U1,
            ref COM.CamUp2, ref COM.CamDw2, MT.CLD_UDL2_N1, MT.GPIO_OUT_UDL2_PZK_N1);

        public static XT xt4 = new XT(3, ref MT.AXIS_UDL2_X, ref MT.AXIS_UDL2_Y, ref MT.AXIS_UDL2_Z, ref MT.AXIS_UDL2_U2,
            ref COM.CamUp2, ref COM.CamDw2, MT.CLD_UDL2_N2, MT.GPIO_OUT_UDL2_PZK_N2);

        public static List<XT> ListXT1 = new List<XT> { xt1, xt2 };
        public static List<XT> ListXT2 = new List<XT> { xt3, xt4 };
        public static List<XT> ListXT = new List<XT> { xt1, xt2, xt3, xt4 };
        public static EM_RES XtInit(string productname)
        {

            foreach (XT xt in COM.ListXT)
            {
                EM_RES ret = xt.LoadCfg(productname);
                if (ret != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? xt.disc + "加载参数出错!" : xt.disc + "ERROR:Load parameter!", DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.Nozzle + xt.id + 1);
                    MessageBox.Show(VAR.IsChinese ? xt.disc + "加载参数出错!" : xt.disc + "ERROR:Load parameter!");
                    return ret;
                }
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "加载吸头参数成功！" : "Load XT parameter successfully!     (加载吸头参数成功！)");
            return EM_RES.OK;
        }

        //上下料模块
        public static UpDownLoad UDLoad1, UDLoad2;
        public static List<UpDownLoad> List_UDLoad;
        public static EM_RES TrayBoxInit(string productname = "")
        {
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese
                ? string.Format("启动模式:{0}", GetStartupModeDescription())
                : string.Format("Startup mode: {0}     (启动模式:{0})", GetStartupModeDescription()));

            foreach (TrayBox traybox in COM.List_traybox)
            {
                EM_RES ret = traybox.LoadCfg(productname);
                if (ret != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? traybox.disc + "加载参数出错!" : traybox.name + "ERROR:Load parameter!", DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.UpDownLoad);
                    MessageBox.Show(VAR.IsChinese ? traybox.disc + "加载参数出错!" : traybox.name + "Error loading parameters" + "\r\n" + traybox.disc + "加载参数出错!");
                    return ret;

                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese
                    ? DescribeTrayBoxMapping(traybox)
                    : string.Format("{0} mapping: logical={1}, axis={2}/{3}, cfg={4}     ({5})",
                        traybox.name,
                        traybox.role,
                        traybox.ax_x.disc,
                        traybox.ax_z.disc,
                        Path.GetFileName(traybox.strCfgPath),
                        DescribeTrayBoxMapping(traybox)));
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "仓储参数成功！" : "Load Traybox parameter successfully!     (仓储参数成功！)");
            return EM_RES.OK;
        }

        //视觉
        public static Msg vs_msg = new Msg(200, Msg.EM_MSGTYPE.NOR | Msg.EM_MSGTYPE.ERR);

        #region 设备复位

        public static bool bhomeing = false;

        public static EM_RES Home()
        {
            try
            {
                VAR.gsys_set.bquit = false;
                WS.bquit = false;
                WS.bpause = false;
                UpDownLoad.bquit = false;
                bhomeing = true;
                //光箱/上料
                EM_RES resLB = EM_RES.OK;
                EM_RES resRB = EM_RES.OK;
                EM_RES resUL = EM_RES.OK;
                EM_RES res = EM_RES.OK;

                Task taskLeftLbHome = new Task(() => { resLB = LeftLightBox.Home(ref VAR.gsys_set.bquit); });
                Task taskRightLbHome = new Task(() => { resRB = RightLightBox.Home(ref VAR.gsys_set.bquit); });
                Task taskUploadHome = new Task(() =>
                {
                    try
                    {
                        EM_RES[] array_res = new EM_RES[2];
                        Task[] task_udlhome = new Task[2];
                        foreach (UpDownLoad Udl in COM.List_UDLoad)
                        {
                            task_udlhome[COM.List_UDLoad.IndexOf(Udl)] = new Task(() => { array_res[COM.List_UDLoad.IndexOf(Udl)] = Udl.Home(ref VAR.gsys_set.bquit); });
                            task_udlhome[COM.List_UDLoad.IndexOf(Udl)].Start();
                            Udl.status_ud = UpDownLoad.EM_STA.HOME;
                        }

                        task_udlhome[0].Wait();
                        task_udlhome[1].Wait();
                        if (array_res[0] == EM_RES.OK && array_res[1] == EM_RES.OK) resUL = EM_RES.OK;
                        else resUL = EM_RES.ERR;
                        for (int i = 0; i < COM.List_UDLoad.Count; i++)
                        {
                            if (array_res[i] == EM_RES.OK) COM.List_UDLoad[i].status_ud = UpDownLoad.EM_STA.READY;
                            else COM.List_UDLoad[i].status_ud = UpDownLoad.EM_STA.UNKNOW;
                        }


                    }
                    finally
                    {

                        COM.UDLoad1.ax_y.DisableHcmp(COM.CamDw1.TriIO.num);
                        COM.UDLoad1.ax_y.DisableHcmp(COM.CamUp1.TriIO.num);
                        COM.UDLoad2.ax_y.DisableHcmp(COM.CamDw2.TriIO.num);
                        COM.UDLoad2.ax_y.DisableHcmp(COM.CamUp2.TriIO.num);
                    }

                });
                if (PT_SET.LbEn)
                {
                    taskLeftLbHome.Start();
                    taskRightLbHome.Start();
                }
                taskUploadHome.Start();

                //wait
                while (!VAR.gsys_set.bquit)
                {
                    if (PT_SET.LbEn)
                    {
                        if (taskLeftLbHome.IsCompleted && taskLeftLbHome.IsCompleted && taskUploadHome.IsCompleted) break;
                    }
                    else
                    {
                        if (taskUploadHome.IsCompleted) break;
                    }
                    Application.DoEvents();
                    Thread.Sleep(10);
                }

                if (resLB != EM_RES.OK || resRB != EM_RES.OK || resUL != EM_RES.OK) res = EM_RES.ERR;

                if (VAR.gsys_set.bquit || res != EM_RES.OK)
                {
                    if (PT_SET.LbEn)
                    {
                        LeftLightBox.Stop();
                        RightLightBox.Stop();
                    }
                    UpDownLoad.UD1Stop();
                    UpDownLoad.UD2Stop();
                    UpDownLoad.LCStop();
                    return VAR.gsys_set.bquit ? EM_RES.QUIT : res;
                }



                //下料

                //EM_RES resDL = EM_RES.OK;

                //Task taskDownloadHome = new Task(() => { resDL = DownloadModle.Home(ref VAR.gsys_set.bquit); });

                //taskDownloadHome.Start();

                ////wait
                //while (!VAR.gsys_set.bquit)
                //{
                //    if ( taskDownloadHome.IsCompleted) break;
                //    Application.DoEvents();
                //    Thread.Sleep(10);
                //}

                //if ( resDL != EM_RES.OK) res = EM_RES.ERR;

                //if (VAR.gsys_set.bquit || res != EM_RES.OK)
                //{
                //    //taskTurnplateHome.Stop();
                //    DownloadModle.Stop();
                //    return VAR.gsys_set.bquit ? EM_RES.QUIT : res;
                //}
                //转盘
                EM_RES resTP = EM_RES.OK;
                Task taskTurnplateHome = new Task(() =>
                {

                    //确保上料Y/下料Y位置
                    if (COM.UDLoad1.ax_x.home_status != AXIS.HOME_STA.OK || COM.UDLoad1.ax_y.home_status != AXIS.HOME_STA.OK || !MT.AXIS_UDL1_X.isORG || MT.AXIS_UDL1_Y.fenc_pos > 700)
                    {
                        MessageBox.Show(VAR.IsChinese ? "上料模块[1]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！" : "Upload[1] x axis is not in the origin or y axis pos>700,There may be interference, it is forbidden to return to zero!\r\n上料模块[1]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        resTP = EM_RES.ERR;
                        return;

                    }
                    if (COM.UDLoad2.ax_x.home_status != AXIS.HOME_STA.OK || COM.UDLoad2.ax_y.home_status != AXIS.HOME_STA.OK || !MT.AXIS_UDL2_X.isORG || MT.AXIS_UDL2_Y.fenc_pos > 700)
                    {
                        MessageBox.Show(VAR.IsChinese ? "上料模块[2]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！" : "Upload[2] x axis is not in the origin or y axis pos>700,There may be interference, it is forbidden to return to zero!\r\n上料模块[2]_X轴不在原点或Y轴位置>700,可能有干涉,禁止转台回零！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        resTP = EM_RES.ERR;
                        return;
                    }
                    //转台不在位置 / 工位不在位置 / OPT不在原点 禁止复位
                    Turntable.EM_STA sta = Turntable.GetCurSta;
                    if ((sta < Turntable.EM_STA.POS0 || sta > Turntable.EM_STA.POS3) && !MT.AXIS_BOX_OTP_Z.isORG)
                    {
                        MessageBox.Show(VAR.IsChinese ? "转台不在位置，且OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位转台或OTP轴！" : "The turntable is not in position, and the OTP is not at the origin, and there is a collision risk!\r\nPlease check to confirm the risk-free and reset the turntable or the OTP shaft manually!\r\n转台不在位置，且OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位转台或OTP轴！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        resTP = EM_RES.ERR;
                        return;
                    }

                    //OTP光源复位(转台不在位置/工位不在位置禁止复位)
                    if ((sta >= Turntable.EM_STA.POS0 && sta <= Turntable.EM_STA.POS3))
                    {
                        WS ws = Turntable.GetWSOnPos(Turntable.EM_STA.POS2);
                        if (ws == null && !ws.isInTestPos)
                        {
                            MessageBox.Show(VAR.IsChinese ? "工站不在测试位置，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴/或对应工站！" : "Workstation is not in the test position, there is a risk of collision!\r\nPlease check to confirm that there is no risk, and then manually reset the OTP axis or corresponding station!\r\n工站不在测试位置，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴/或对应工站！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            resTP = EM_RES.ERR;
                            return;
                        }
                        if (EM_RES.OK != COM.OTPLightBox.Home(ref VAR.gsys_set.bquit))
                        {
                            MessageBox.Show(VAR.IsChinese ? "转台/工位 回零失败!" : "Turntable/workstation reset failed\r\n转台/工位 回零失败!");
                            resTP = EM_RES.ERR;
                            return;
                        }
                    }

                    //转台复位
                    if ((sta < Turntable.EM_STA.POS0 || sta > Turntable.EM_STA.POS3) && !MT.AXIS_BOX_OTP_Z.isORG)
                    {
                        MessageBox.Show(VAR.IsChinese ? "OTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴！" : "OTP axis is not at the origin, there is a risk of collision!\r\nPlease check to make sure there is no risk, and then manually reset the OTP axis!\r\nOTP轴不在原点，有撞机风险!\r\n请检查确认无风险，再手动复位OTP轴！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        resTP = EM_RES.ERR;
                        return;
                    }
                    if (EM_RES.OK == COM.TurntableHome2(ref VAR.gsys_set.bquit))
                    {
                        resTP = EM_RES.OK;
                        VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "就绪" : "Ready", 0, true);

                    }
                    else resTP = EM_RES.ERR;


                });
                taskTurnplateHome.Start();
                //wait
                while (!VAR.gsys_set.bquit)
                {
                    if (taskTurnplateHome.IsCompleted) break;
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
                if (resTP != EM_RES.OK) res = EM_RES.ERR;
                if (res == EM_RES.OK)
                {
                    DRpt.Report_Status(DReport.EmStatus.Ready, DReport.EmHareware.Null, DReport.EmStatus.Ready.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Ready).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                }
                return res;
            }
            finally
            {
                bhomeing = false;
            }
        }

        /// <summary>
        /// 停止轴运动，停止Home动作
        /// </summary>
        public static void Stop()
        {
            VAR.gsys_set.bquit = true;
            UpDownLoad.bquit = true;
            WS.bquit = true;
            LeftLightBox.Stop();
            RightLightBox.Stop();
            UpDownLoad.UD1Stop();
            UpDownLoad.UD2Stop();
            UpDownLoad.LCStop();
            //UploadModle.Stop();
            //taskTurnplateHome.Stop();
            // DownloadModle.Stop();

            foreach (WS ws in COM.list_ws)
            {
                ws.ax_u.Stop();
                //ws.ax_fr.Stop();
                //ws.ax_bk.Stop();
            }
        }

        #endregion

        #region 相机初始化

        public static EM_RES CamInit(int timeout = 1000)
        {
            //Task TaskCamInit = new Task(() =>
            //{
            try
            {

                foreach (Cam cam in ListCam)
                {
                    string path = string.Format("{0}\\product\\{1}\\Camera\\{2}\\", Path.GetFullPath(".."),
                        VAR.gsys_set.cur_product_name, cam.mName);
                    cam.status = Cam.CAM_STA.INIT;
                    cam.Init(path);
                    cam.LoadTask(path);
                    path = string.Format("{0}\\syscfg\\Calibration\\{1}\\", Path.GetFullPath(".."), cam.mName);
                    cam.LoadCaliTool(path);
                    if (cam.isInit) cam.status = Cam.CAM_STA.READY;
                    else cam.status = Cam.CAM_STA.DISCONNECT;
                }

                //affTransTool use pix space               
                //Cam.VisionTask task = CamUp1.List_vs_task.Find(s => s.TaskName.Equals("AffTransTool"));
                //if (task != null) task.ListCaliTool = new List<Cam.CaliTool>();

                //task = CamUp2.List_vs_task.Find(s => s.TaskName.Equals("AffTransTool"));
                //if (task != null) task.ListCaliTool = new List<Cam.CaliTool>();

                //task = CamDown.List_vs_task.Find(s => s.TaskName.Equals("AffTransTool"));
                //if (task != null) task.ListCaliTool = new List<Cam.CaliTool>();


                //CamDown to CamUp1 transform
                foreach (Cam.CaliTool tool in CamUp1.ListCaliTool)
                    CamDw1.ListCaliTool.Add(tool);

                //CamUp2 to CamUp1 transform
                foreach (Cam.CaliTool tool in CamUp2.ListCaliTool)
                    CamDw2.ListCaliTool.Add(tool);

                foreach (Cam cam in COM.ListCam)
                {
                    cam.ListCaliTool.Sort((a, b) => { return a.name.CompareTo(b.name); });
                }

                CogVisionToolMultiThreading.Enable = true;
                CogVisionToolMultiThreading.ThreadCountMode =
                    CogVisionToolMultiThreadingThreadCountModeConstants.HardwareDefined;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}初始化异常", ex.Message) : string.Format("{0} Error:Init    ({0}初始化异常)", ex.Message), DReport.EmErrCode.InitFailed, (int)DReport.EmHareware.Cam);
                return EM_RES.ERR;
            }
            // }
            //);
            // TaskCamInit.Start();
            // await TaskCamInit;

            //if (timeout == 0) return EM_RES.OK;

            ////wait
            //int t = 0;
            //do
            //{
            //    t += 10;
            //    if (t >= timeout) return EM_RES.TIMEOUT;

            //    if (TaskCamInit.IsCompleted || VAR.gsys_set.bclose) break;
            //    Thread.Sleep(10);
            //    Application.DoEvents();

            //} while (true);

            return EM_RES.OK;
        }

        public static EM_RES CamLoadCailTool()
        {
            try
            {
                foreach (Cam cam in ListCam)
                {
                    string path = string.Format("{0}\\syscfg\\Calibration\\{1}\\", Path.GetFullPath(".."), cam.mName);
                    cam.LoadCaliTool(path);
                }

                //CamDw1 to CamUp1 transform
                foreach (Cam.CaliTool tool in CamUp1.ListCaliTool)
                    CamDw1.ListCaliTool.Add(tool);

                //CamDw2 to CamUp2 transform
                foreach (Cam.CaliTool tool in CamUp2.ListCaliTool)
                    CamDw2.ListCaliTool.Add(tool);

                foreach (Cam cam in COM.ListCam)
                {
                    cam.ListCaliTool.Sort((a, b) => { return a.name.CompareTo(b.name); });
                }
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载校正关系异常", ex.Message) : string.Format("{0} Load correction relationship abnormal        ({0}加载校正关系异常)", ex.Message), DReport.EmErrCode.ToolError, (int)DReport.EmHareware.Cam);
                return EM_RES.ERR;
            }

            return EM_RES.OK;
        }

        public static bool CamisOnInitStatus
        {
            get
            {
                foreach (Cam cam in ListCam)
                {
                    if (cam.status == Cam.CAM_STA.INIT) return true;
                }

                return false;
            }
        }

        #endregion

        #region 板卡初始化

        //public EM_RES HwInit()
        //{
        //    //硬件初始化
        //    Task TaskHWInit = new Task(() =>
        //    {
        //        EM_RES res = MT.Init(Path.GetFullPath("..") + "\\syscfg\\");
        //        if (res != EM_RES.OK) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "板卡始化失败!");
        //        else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "板卡始化成功!");
        //    }
        //    );
        //    TaskHWInit.Start();

        //    //for (int n = 0; n < 3000; n++)
        //    //{
        //    //    if (TaskHWInit.IsCompleted || VAR.gsys_set.bclose) break;
        //    //    Thread.Sleep(10);
        //    //    Application.DoEvents();
        //    //}

        //    return EM_RES.OK;
        //}

        #endregion

        #region 测试转台复位

        public static EM_RES TurntableHome(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;

            try
            {
                //start u axis home
                foreach (WS ws in list_ws)
                {
                    res = ws.ax_u.HomeTask(30000);
                    if (res != EM_RES.OK) return res;
                }

                //wait home end
                while (true)
                {
                    Thread.Sleep(200);
                    Application.DoEvents();
                    foreach (WS ws in list_ws)
                    {
                        if (!ws.ax_u.HomeTaskisEnd) continue;
                    }

                    break;
                }

                //check result
                foreach (WS ws in list_ws)
                {
                    if (ws.ax_u.HomeTaskRet != EM_RES.OK) return EM_RES.ERR;
                }

                //u up
                foreach (WS ws in list_ws)
                {
                    res = ws.ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                    if (res != EM_RES.OK) return res;
                }

                //all up
                foreach (WS ws in list_ws)
                {
                    res = ws.FrCyUp(ref bquit);
                    if (res != EM_RES.OK) return res;
                    res = ws.BkCyUp(ref bquit);
                    if (res != EM_RES.OK) return res;
                }

                ////start fr/bk axis home
                //foreach (WS ws in list_ws)
                //{
                //    res = ws.ax_fr.HomeTask(30000);
                //    if (res != EM_RES.OK) return res;
                //    res = ws.ax_bk.HomeTask(30000);
                //    if (res != EM_RES.OK) return res;
                //}

                ////wait home end
                //while (true)
                //{
                //    Thread.Sleep(200);
                //    Application.DoEvents();
                //    foreach (WS ws in list_ws)
                //    {
                //        if (!ws.ax_fr.HomeTaskisEnd || !ws.ax_bk.HomeTaskisEnd) continue;
                //    }

                //    break;
                //}

                ////check result
                //foreach (WS ws in list_ws)
                //{
                //    if (ws.ax_fr.HomeTaskRet != EM_RES.OK || ws.ax_bk.HomeTaskRet != EM_RES.OK) return EM_RES.ERR;
                //}

                ////fr/bk axis
                //foreach (WS ws in list_ws)
                //{
                //    res = ws.ax_fr.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                //    if (res != EM_RES.OK) return res;
                //    res = ws.ax_bk.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                //    if (res != EM_RES.OK) return res;
                //}

                //all down
                foreach (WS ws in list_ws)
                {
                    res = ws.FrCyDown(ref bquit);
                    if (res != EM_RES.OK) return res;
                    res = ws.BkCyDown(ref bquit);
                    if (res != EM_RES.OK) return res;
                }

                //u down
                foreach (WS ws in list_ws)
                {
                    res = ws.ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.N);
                    if (res != EM_RES.OK) return res;
                }

                return EM_RES.OK;
            }
            finally
            {
                //foreach (WS ws in list_ws)
                //{
                //    ws.ax_u.Stop();
                //    ws.ax_fr.Stop();
                //    ws.ax_bk.Stop();
                //}
            }
        }

        public static EM_RES TurntableHome2(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;

            try
            {
                if (PT_SET.LbEn)
                {
                    //check
                    if (LeftLightBox.ax_x2.home_status != AXIS.HOME_STA.OK ||
                        LeftLightBox.ax_x2.fcmd_pos > LeftLightBox.pos_safe_xz)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese ? string.Format("转台可能与{0}有干涉，请先回零{1}!", LeftLightBox.ax_x2.disc, LeftLightBox.ax_x2.disc) : string.Format("The turntable may interfere with {0}, please return to origin first {1}!         (转台可能与{0}有干涉，请先回零{1}!)", LeftLightBox.ax_x2.disc, LeftLightBox.ax_x2.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox);
                        return EM_RES.MOVE_PROTECT;
                    }

                    if (RightLightBox.ax_x2.home_status != AXIS.HOME_STA.OK ||
                        RightLightBox.ax_x2.fcmd_pos > RightLightBox.pos_safe_xz)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese ? string.Format("转台可能与{0}有干涉，请先回零{1}!", RightLightBox.ax_x2.disc, RightLightBox.ax_x2.disc) : string.Format("The turntable may interfere with {0}, please return to origin first {1}!        (转台可能与{0}有干涉，请先回零{1}!)", RightLightBox.ax_x2.disc, RightLightBox.ax_x2.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox);
                        return EM_RES.MOVE_PROTECT;
                    }
                }

                if (!OTPLightBox.ax_z_otp.isORG)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                        VAR.IsChinese ? string.Format("转台可能与{0}有干涉，请先回零{1}!", OTPLightBox.ax_z_otp.disc, OTPLightBox.ax_z_otp.disc) : string.Format("The turntable may interfere with {0}, please return to origin first {1}!        (转台可能与{0}有干涉，请先回零{1}!)", OTPLightBox.ax_z_otp.disc, OTPLightBox.ax_z_otp.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox);
                    return EM_RES.MOVE_PROTECT;
                }

                //link
                res = MT.CARD_ORIENTAL485_5.Init();
                if (res != EM_RES.OK) return res;

                //fr/bk
                foreach (WS ws in list_ws)
                {
                    //ws.ax_fr.SVRON = true;
                    //ws.ax_bk.SVRON = true;
                    ws.ax_u.SVRON = true;
                    //if (ws.isFrUp)
                    //{
                    //res = ws.ax_fr.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                    //if (res != EM_RES.OK) return res;
                    //ws.FrCyDown1(ref bquit);
                    //}

                    //if (ws.isBkUp)
                    //{
                    //res = ws.ax_bk.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                    //if (res != EM_RES.OK) return res;
                    //ws.BkCyDown1(ref bquit);
                    //}

                    //if (ws.ax_fr.SVRON)
                    //    ws.ax_fr.home_status = AXIS.HOME_STA.OK;
                    //if (ws.ax_bk.SVRON)
                    //    ws.ax_bk.home_status = AXIS.HOME_STA.OK;

                    res = ws.AllCyDown(ref bquit, 1000, false, -1, false);
                    if (res != EM_RES.OK) return res;
                    res = ws.ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.N);
                    if (res != EM_RES.OK) return res;
                    res = ws.ZKOff(ref VAR.gsys_set.bquit);
                    if (res != EM_RES.OK) return res;

                    if (ws.ax_u.SVRON && Math.Abs(ws.ax_u.fenc_pos) < 1)
                        ws.ax_u.home_status = AXIS.HOME_STA.OK;
                }

                return EM_RES.OK;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message, DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable);
                return EM_RES.ERR;
            }
            finally
            {
                //foreach (WS ws in list_ws)
                //{
                //    ws.ax_u.Stop();
                //    ws.ax_fr.Stop();
                //    ws.ax_bk.Stop();
                //}
            }
        }

        #endregion

        #region 获取当前工站对应的光箱

        public static EM_RES GetLightBox(int ws_num, ref LightBox lb)
        {
            //获取当前转盘位置
            Turntable.EM_STA pos = Turntable.GetWSSta(ws_num);
            switch (pos)
            {
                //转盘位置未知
                default:
                    lb = null;
                    return EM_RES.ERR;
                //上料位
                case Turntable.EM_STA.POS0:
                    lb = null;
                    break;
                //左光箱
                case Turntable.EM_STA.POS1:
                    lb = LeftLightBox;
                    break;
                //OTP光箱
                case Turntable.EM_STA.POS2:
                    lb = OTPLightBox;
                    break;
                //右光箱
                case Turntable.EM_STA.POS3:
                    lb = RightLightBox;
                    break;
            }

            return EM_RES.OK;
        }

        #endregion

        public static readonly Object UDLockObj = new object();
        public static readonly Object TrunTableLockObj = new object();
    }

    #endregion

    #region 基本动作

    public static class BaseAction
    {
        #region 加载对应产品参数

        public static EM_RES LoadProductCfg(string productname)
        {
            EM_RES ret = EM_RES.OK;
            try
            {
                bool ischange = false;
                VAR.gsys_set.cur_product_name = productname;
                IniFile inf = new IniFile(Path.GetFullPath("..") + "\\syscfg\\syscfg.ini");
                inf.WriteString("PRODUCT_SET", "CUR_PRODCUT_NAME", VAR.gsys_set.cur_product_name, ref ischange, false);
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "正在切换" : "Changing", -1, true);
                if (COM.NGDef == null) COM.NGDef = new NGCodeDef();
                ret = COM.NGDef.LoadCfg();
                if (ret != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("NGDef函数加载数据失败!") : "NGDef function failed to load data!", DReport.EmErrCode.GetParamError);
                    return ret;
                }

                if (COM.product == null) COM.product = new Product();

                foreach (WS ws in COM.list_ws)
                {
                    ret = ws.LoadCfg();
                    if (ret != EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}数据加载失败!", ws.disc) : string.Format("{0} Data loading failed!       ({0}数据加载失败!)", ws.disc), DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.TurnTable + ws.num + 1);
                        return ret;
                    }
                }

                //加载吸头
                ret = COM.XtInit(VAR.gsys_set.cur_product_name);
                if (ret != EM_RES.OK) return ret;
                //加载仓储
                ret = COM.TrayBoxInit();
                if (ret != EM_RES.OK) return ret;

                //加载视觉
                ret = COM.CamInit();
                if (ret != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "相机初始化失败!" : "Camera initialization failed!       (相机初始化失败!)", DReport.EmErrCode.InitFailed, (int)DReport.EmHareware.Cam);
                    return ret;
                }
                else VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "相机初始化成功!" : "Camera initialized successfully!      (相机初始化成功!)");


                UpDownLoad.DisAxisX = Math.Abs(COM.xt1.st_pos_samevs[0].x) + Math.Abs(COM.xt1.st_pos_samevs[1].x);
                if (UpDownLoad.DisAxisX < 400)
                    UpDownLoad.DisAxisX = 400;

                PT_SET.LoadPtCfg(VAR.gsys_set.cur_product_name);
                COM.tray_fd = new Product.Tray(COM.traybox_fd.strCfgPath, 0);
                COM.tray_ok = new Product.Tray(COM.traybox_ok.strCfgPath, 0);
                COM.tray_ng = new Product.Tray(COM.traybox_ng.strCfgPath, 0);
                FrMain.frproduct.tray_fd = COM.tray_fd;
                FrMain.frproduct.tray_ok = COM.tray_ok;
                FrMain.frproduct.tray_ng = COM.tray_ng;
                FrMain.frproduct.ctb_tray_cfg.SelectedIndex = 0;
                FrMain.frproduct.tray = FrMain.frproduct.tray_fd;
                FrMain.frproduct.ShowTrayData(FrMain.frproduct.tray);
                FrMain.frproduct.ShowTrayBoxData(FrMain.frproduct.traybox);
                COM.traybox_fd.SetSta(TrayBox.EM_STA.UNTEST);
                COM.traybox_ok.SetSta(TrayBox.EM_STA.FULL);
                COM.traybox_ng.SetSta(TrayBox.EM_STA.FULL);
                COM.traybox_fd.NewBox(Product.EM_CM_RES.UNTEST);
                COM.traybox_ok.NewBox(Product.EM_CM_RES.EMPTY);
                COM.traybox_ng.NewBox(Product.EM_CM_RES.EMPTY);
                COM.LeftLightBox.LoadCfg();
                COM.RightLightBox.LoadCfg();
                COM.OTPLightBox.LoadCfg();
                FrMain.frproduct.lightBoxDef.UpdateShow();
                FrMain.frproduct.ctb_gz.SelectedIndex = 0;
                FrMain.frproduct.ws_status.ws = COM.ws1;
                FrMain.frproduct.wsModuelDef.ws = FrMain.frproduct.ws_status.ws;
                FrMain.frproduct.wsModuelDef.UpdateShow();
                FrMain.frproduct.wsModuelDef.ShowOtherData();
                FrMain.frproduct.ws_status.UpdateShow();
                FrMain.frproduct.xtOfsAdj1.trayUpdate(COM.tray_fd);
                if (FrMain.frrun != null && FrMain.frrun.cogDisplayer_run != null)
                {
                    FrMain.frrun.cogDisplayer_run.list_cam.Clear();
                    FrMain.frrun.cogDisplayer_run.AddCam(COM.ListCam);
                }

                if (FrMain.frproduct != null && FrMain.frproduct.cogDisplayer_product != null)
                {
                    FrMain.frproduct.cogDisplayer_product.list_cam.Clear();
                    FrMain.frproduct.cogDisplayer_product.AddCam(COM.ListCam);
                }

                if (FrMain.frproduct != null && FrMain.frproduct.pmAlignEdit1 != null)
                {
                    FrMain.frproduct.pmAlignEdit1.InitDisPlay(FrMain.frproduct.cogDisplayer_product.cogRecordDisplay, FrMain.frproduct.maskDisplay1);
                    FrMain.frproduct.pmAlignEdit1.list_cam.Clear();
                    FrMain.frproduct.pmAlignEdit1.AddCam(COM.ListCam);
                }

                if (FrMain.frsys != null && FrMain.frsys.CogRecordDisplay_sys != null)
                {
                    FrMain.frsys.CogRecordDisplay_sys.list_cam.Clear();
                    FrMain.frsys.CogRecordDisplay_sys.AddCam(COM.ListCam);
                }
                return EM_RES.OK;

            }
            finally
            {
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "切换完成" : "Changed", -1, true);
            }
        }

        #endregion

        public static bool bnotfeed = false;

        #region 运行

        static Task run_task = null;

        #region 获取运行模式
        public static void GetRunMode()
        {
            if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.BOTH_WORK)
            {
                COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.HIGH;
                COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.LOW;
            }
            else if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.MD1_WORK)
            {
                COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.ALL;
                COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.NONE;
            }
            else if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.MD2_WORK)
            {
                COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.NONE;
                COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.ALL;
            }
            else
            {
                COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.HIGH;
                COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.LOW;
            }
            //确认运行方式
            if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_NORMAL)
            {
                COM.UDLoad1.Demo = false;
                COM.UDLoad2.Demo = false;
                WS.Demo = false;
            }
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_UPDW)
            {
                COM.UDLoad1.Demo = false;
                COM.UDLoad2.Demo = false;
                WS.Demo = true;
            }
            else if (PT_SET.RunPattern == (int)PT_SET.RUN_PATTERN.RUN_EMPTY)
            {
                COM.UDLoad1.Demo = true;
                COM.UDLoad2.Demo = true;
                WS.Demo = true;
            }
            else
            {
                COM.UDLoad1.Demo = false;
                COM.UDLoad2.Demo = false;
                WS.Demo = false;
            }
        }
        #endregion

        #region 保养提示

        public static EM_RES MaintainTip(int curVal, int LimVal, bool IsFixtrue = true)
        {
            EM_RES ret = EM_RES.OK;
            if (LimVal != 0 && curVal > LimVal)
            {
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "保养提示!" : "Upkeep", 20, true);
                MT.ST_WARN st_warn = new MT.ST_WARN();
                warning fr_warn = new warning();//增加语言
                st_warn.ok_txt = MultiLanguage.TxtSelct("确认返回", "Confirm return", "xác nhận trở lại");
                st_warn.ws = null;
                st_warn.title = MultiLanguage.TxtSelct("提示:保养提示!", "Tip: Maintenance tips!", "Mẹo: Mẹo bảo trì!");
                st_warn.msg = MultiLanguage.TxtSelct(
                    "提示:" + st_warn.msg + "\r\n请确认!\r\n  请按'确认返回'键进行相应的保养，保养后把当前次数清零，再按'运行'键进行生产!",
                    "Tip:" + st_warn.msg + "\r\nPlease confirm! \r\n Please press the 'Confirm Back' button to perform the corresponding maintenance. After maintenance, clear the current number to zero, and then press the 'Run' button to produce!",
                    "dấu hiệu:" + st_warn.msg + "\r\n Vui lòng xác nhận!\r\n Vui lòng nhấn phím xác nhận và quay lại để thực hiện bảo trì tương ứng, sau khi bảo trì, đặt lại số lần hiện tại, sau đó nhấn phím 'chạy' để bắt đầu sản xuất!");
                st_warn.lb_msg = MultiLanguage.TxtSelct(
                    "提示:" + st_warn.msg + "\r\n请确认!\r\n  请按'确认返回'键进行相应的保养，保养后把当前次数清零，再按'运行'键进行生产!",
                    "Tip:" + st_warn.msg + "\r\nPlease confirm! \r\n Please press the 'Confirm Back' button to perform the corresponding maintenance. After maintenance, clear the current number to zero, and then press the 'Run' button to produce!",
                    "dấu hiệu:" + st_warn.msg + "\r\n Vui lòng xác nhận!\r\n Vui lòng nhấn phím xác nhận và quay lại để thực hiện bảo trì tương ứng, sau khi bảo trì, đặt lại số lần hiện tại, sau đó nhấn phím 'chạy' để bắt đầu sản xuất!");
                DialogResult logres1 = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.Null);
                ret = EM_RES.PARA_OUTOFRANG;
            }

            return ret;
        }
        #endregion

        public static void run()
        {
            if (run_task == null || run_task != null && run_task.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "创建运行线程" : "Create a Run thread     (创建运行线程)");
                if (run_task != null) run_task.Dispose();
                run_task = new Task(run_th);
                run_task.Start();
            }
        }

        public static void LightBoxSendMes(LightBox light, int secsId, int PosId)
        {
            var mpos = light.ListPos.Find(s => s.ID == PosId);
            if (mpos != null)
            {
                if (light.disc.Contains("左"))
                {
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId, Value = mpos.X1.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 1, Value = mpos.X2.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 2, Value = mpos.Y1.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 3, Value = mpos.Z1.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 4, Value = mpos.Z2.ToString() });
                }
                else if (light.disc.Contains("右"))
                {
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId, Value = mpos.X1.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 1, Value = mpos.X2.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 2, Value = mpos.Z1.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId + 3, Value = mpos.Z2.ToString() });
                }else
                {
                    Msg.secsManager.Send(new BaseInfo() { Id = secsId , Value = mpos.Z1.ToString() });
                }

                Msg.secsManager.Send(new BaseInfo() { Id = 7 }, TypeId: 2);
            }
        }

        public static void LightBoxSendMesAll()
        {
            int mesid = 14;
            try
            {
                foreach (var pos in COM.LeftLightBox.ListPos)
                {
                    LightBoxSendMes(COM.LeftLightBox, mesid, pos.ID);
                    mesid = mesid + 5;
                }
                mesid = 101;
                foreach (var pos in COM.RightLightBox.ListPos)
                {
                    LightBoxSendMes(COM.RightLightBox, mesid, pos.ID);
                    mesid = mesid + 4;
                }
                mesid = 94;
                foreach (var pos in COM.OTPLightBox.ListPos)
                {
                    LightBoxSendMes(COM.OTPLightBox, mesid, pos.ID);
                    mesid = mesid + 1;
                }
            }catch (Exception ee)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "光箱上传数据异常");
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "光箱上传数据成功");
        }



        //public static void run_th_old()
        //{
        //    EM_RES res = EM_RES.OK;
        //    EM_RES res2 = EM_RES.OK;
        //    int tick = 0;

        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "运行线程启动");
        //    VAR.gsys_set.status = EM_SYS_STA.RUN;
        //    VAR.sys_inf.Set(EM_ALM_STA.NOR_BLUE, "正在运行", -1);
        //    MT.GPIO_OUT_KL_START.SetOn();
        //    MT.SetAllAxToWorkSpd();

        //    //确认测试软件联机
        //    List<TestPC.InfoData> list_info = new List<TestPC.InfoData>();
        //    Turntable.tmr = System.Environment.TickCount;
        //    foreach (WS ws in COM.list_ws)
        //    {
        //        int tryCnt = 0;
        //        do
        //        {
        //            res = ws.GetTestInfo(ref list_info);
        //            if (res == EM_RES.OK) break;
        //            tryCnt++;
        //            if (tryCnt >= 3)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("{0}与测试软件通信异常！", ws.disc));
        //                VAR.gsys_set.status = EM_SYS_STA.STANDBY;
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, string.Format("{0}通信", ws.disc), 20, true);
        //                return;
        //            }
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("{0}与测试软件通信重连接...{1}", ws.disc, tryCnt));
        //            Thread.Sleep(1000);
        //        } while (true);
        //    }

        //    while (VAR.gsys_set.bclose == false && VAR.gsys_set.bquit == false)
        //    {
        //        tick = System.Environment.TickCount;
        //        VAR.sys_inf.Set(EM_ALM_STA.NOR_BLUE, "运行", 0, true);
        //        VAR.gsys_set.status = EM_SYS_STA.RUN;
        //        MT.GPIO_OUT_ALM_GREEN.SetOn();
        //        MT.GPIO_OUT_KL_START.SetOn();

        //        #region 安全保护

        //        //if (!MT.isSafeSen)
        //        //{
        //        //    if (bsafe == false) VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, "安全光栅/门锁触发1");
        //        //    bsafe = true;
        //        //    if (VAR.gsys_set.status == EM_SYS_STA.RUN)
        //        //    {
        //        //        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "安全防护");
        //        //        VAR.gsys_set.bpause = true;
        //        //    }
        //        //}
        //        //else bsafe = false;

        //        #endregion

        //        //运行测试
        //        COM.ws1.RunTestTask();
        //        COM.ws2.RunTestTask();
        //        COM.ws3.RunTestTask();
        //        COM.ws4.RunTestTask();

        //        //上下料工位准备
        //        WS workstation = Turntable.GetWSOnFeedPos;
        //        if (workstation == null)
        //        {
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料前，转台位置异常!");
        //            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //            break;
        //        }
        //        //确认是否要上下料
        //        if (!bnotfeed && (workstation.TestStatus == WS.EM_TEST_STA.COMPLETED || workstation.TestStatus == WS.EM_TEST_STA.EMPTY))
        //        {
        //            res = workstation.SetupForFeed(ref VAR.gsys_set.bquit);
        //            if (res != EM_RES.OK) break;

        //            //运行上下料线程
        //            DownloadModle.RunTask();
        //            Thread.Sleep(200);
        //            UploadModle.RunTask();
        //        }

        //        Thread.Sleep(200);

        //        //等待测试完成
        //        COM.ws1.WaitTestTask(ref VAR.gsys_set.bquit);
        //        COM.ws2.WaitTestTask(ref VAR.gsys_set.bquit);
        //        COM.ws3.WaitTestTask(ref VAR.gsys_set.bquit);
        //        COM.ws4.WaitTestTask(ref VAR.gsys_set.bquit);

        //        //测试结果
        //        foreach (WS ws in COM.list_ws)
        //        {
        //            if (ws.TestStatus == WS.EM_TEST_STA.ERROR)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 测试出错!", ws.disc));
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, string.Format("{0}出错", ws.disc), 20, true);
        //                VAR.gsys_set.bpause = true;
        //                res = EM_RES.ERR;
        //                break;
        //            }
        //        }

        //        if (res != EM_RES.OK)
        //        {
        //            break;
        //        }

        //        //上下料结果
        //        if (!bnotfeed)
        //        {
        //            res = DownloadModle.WaitTask(ref VAR.gsys_set.bquit);
        //            res2 = UploadModle.WaitTask(ref VAR.gsys_set.bquit);
        //            if (res != EM_RES.OK)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "下料异常!", 20, true);
        //                break;
        //            }

        //            if (res2 != EM_RES.OK)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上料异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "上料异常!",20, true);
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            //上下料工位归位
        //            workstation = Turntable.GetWSOnFeedPos;
        //            if (workstation == null)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料后，转台位置异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //                break;
        //            }

        //            //复位测试结果
        //            workstation.ResetResultOfMd();
        //            //更新物料状态
        //            workstation.TestStatus = WS.EM_TEST_STA.UNTEST;

        //            //提前开图
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 启动测试", workstation.disc));
        //            res = workstation.StartTestFlow();
        //            if (res != EM_RES.OK)
        //            {
        //                Thread.Sleep(1500);
        //                res = workstation.StartTestFlow();
        //                if (res != EM_RES.OK)
        //                {
        //                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
        //                        string.Format("{0} StartTestFlow err", workstation.disc));
        //                    workstation.Status = WS.EM_STA.LINKERR;
        //                }
        //            }

        //            //计时开始
        //            workstation.tmr = System.Environment.TickCount;
        //        }

        //        //上下料工位归位
        //        workstation = Turntable.GetWSOnFeedPos;
        //        if (workstation == null)
        //        {
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料后，转台位置异常!");
        //            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //            break;
        //        }

        //        res = workstation.TurnToTest(ref VAR.gsys_set.bquit);
        //        if (res != EM_RES.OK) break;

        //        //旋转转台
        //        res = Turntable.MoveToNext(ref VAR.gsys_set.bquit);
        //        if (res != EM_RES.OK) break;

        //        //计时
        //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "U T=" + (Environment.TickCount - tick).ToString());
        //    }

        //    MT.GPIO_OUT_ALM_GREEN.SetOff();
        //    MT.GPIO_OUT_KL_START.SetOff();
        //    VAR.gsys_set.status = EM_SYS_STA.STANDBY;
        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "运行线程结束");
        //}



        public static bool bRuning = false;
        public static void run_th()
        {
            try
            {
                bRuning = true;
                MT.DoorAlarmMsg = string.Empty;
                MT.DoorAlarmMsgTemp = string.Empty;
                bool GrrSetZero = false;
                EM_RES res = MT.SetAllAxToWorkSpd();
                if (res != EM_RES.OK) return;
                //确认运行模式
                GetRunMode();
                // 确认保养
                res = MaintainTip(COUNT_DATA.CurFixtrueMT, PT_SET.FixtrueMT);
                if (res != EM_RES.OK) return;

                res = MaintainTip(COUNT_DATA.CurEquipmentMT, PT_SET.EquipmentMT, false);
                if (res != EM_RES.OK) return;
                //检测门禁
                res = MT.ChkAllSafeSen(0xff);
                if (res != EM_RES.OK)
                {
                    MT.ErrAlmTip();
                    return;
                }
                //检测GRR流程
                if (PT_SET.bGrrFlow)
                {
                    DialogResult Diares = MessageBox.Show(VAR.IsChinese ? "当前模式为GRR流程测试模式!\r\n1.按'是'重新开始。\r\n2.按'否'继续上次测试。\r\n3.按'取消'退出测试！" : "The current mode is the GRR process test mode!\r\n1. Press 'Yes' to restart.\r\n2. Press 'No' to continue the last test. \r\n3. Press 'Cancel' to exit the test!\r\n当前模式为GRR流程测试模式!\r\n1.按'是'重新开始。\r\n2.按'否'继续上次测试。\r\n3.按'取消'退出测试！", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (Diares == DialogResult.Cancel) return;
                    else if (Diares == DialogResult.Yes)
                    {
                        GrrSetZero = true;
                        COM.UDLoad1.GrrProc = UpDownLoad.EM_STA.PICKWS;
                    }
                    // else if (Diares == DialogResult.No) GrrSetZero = false;
                    COM.UDLoad1.FindMod = UpDownLoad.EM_FIN_MOD.ALL;
                    COM.UDLoad2.FindMod = UpDownLoad.EM_FIN_MOD.NONE;
                }
                //确认测试软件联机
                List<TestPC.InfoData> list_info = new List<TestPC.InfoData>();
                Turntable.tmr = System.Environment.TickCount;
                foreach (WS ws in COM.list_ws)
                {
                    int tryCnt = 0;
                    if (GrrSetZero)
                    {
                        ws.GrrTestCnt = 0;
                        ws.GrrUDLcnt = 0;
                    }
                    do
                    {
                        res = ws.GetTestInfo(ref list_info, WS.Demo);
                        if (res == EM_RES.OK) break;
                        tryCnt++;
                        if (tryCnt >= 3)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0}与测试软件通信异常！", ws.disc) : string.Format("{0} link err!        ({0}与测试软件通信异常！)", ws.disc), DReport.EmErrCode.ComTimout, (int)DReport.EmHareware.TurnTable + ws.num + 1);
                            VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? string.Format("{0}通信", ws.disc) : string.Format("{0}communication", ws.disc), 20, true);
                            return;
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0}与测试软件通信重连接...{1}", ws.disc, tryCnt) : string.Format("{0} reconnect,trycnt:{1}        ({0}与测试软件通信重连接...{1})", ws.disc, tryCnt), DReport.EmErrCode.ComTimout, (int)DReport.EmHareware.TurnTable + ws.num + 1);
                        Thread.Sleep(1000);
                    } while (true);
                }
                //如有测试结果错误,把错误结果清掉
                foreach (WS ws in COM.list_ws)
                {
                    if (ws.Status == WS.EM_STA.ERR)
                        ws.Status = WS.EM_STA.REDAY;
                    if (ws.TestStatus == WS.EM_TEST_STA.ERROR)
                        ws.TestStatus = WS.EM_TEST_STA.UNTEST;
                    if (NewSysInf.UserParams.bClearSetOffWs)//请料后关闭
                    {
                            ws.PowerOn(ref VAR.gsys_set.bquit);
                    }

                }
                if (NewSysInf.UserParams.bClearSetOffWs)
                    Thread.Sleep(400);
                 //初始化
                VAR.SysErrAlm.ErrItem = ERR_ALM.EmErrItem.Null;
                VAR.SysErrAlm.ErrStr = string.Empty;
                MT.DoorAlarmMsg = string.Empty;
                MT.DoorAlarmMsgTemp = string.Empty;
                ////归位
                //  foreach (UpDownLoad ud in COM.List_UDLoad)
                //  {
                //      res = ud.GoZero(ref VAR.gsys_set.bquit);
                //      if (res != EM_RES.OK) return;
                //  }
                if (PT_SET.AutoChkEn && VAR.isAutoChkMode && !VAR.ClearMt)
                {
                    var ws = COM.list_ws.Find(s => s.num == (int)PT_SET.AutoChkSelectWs);

                    var res1 = ws.GetPcChkMod(out var pUCFFactoryMode1, out var pUCFFactoryMode2, out var temp1, out var temp2);
                    if (res1)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"func获取工位{ws.disc}是否点检模式成功");
                        var bAutoChkMod = ws.PcIsChkModShow(pUCFFactoryMode1, pUCFFactoryMode2);
                        if (!bAutoChkMod)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"func提示点检模式后选择停机");
                            return;
                        }
                    }
                    else
                    {
                        var errmsg = ws.disc + ",";
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"func获取工位{ws.disc}点检模式失败");
                        return;
                    }

                    int[] temp1List = new int[16], temp2List = new int[16];
                    res1 = ws.GetPcChkInfo(out var pUCFFac1, out var pUCFFac2, temp1List, temp2List);
                    if (!res1)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"func获取工位{ws.disc}获取点检要求信息失败");
                        return;
                    }
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"func获取工位{ws.disc}获取点检要求信息成功");
                }
                if (PT_SET.AutoChkEn && !VAR.isAutoChkMode && !VAR.ClearMt)
                {
                    var ws = COM.list_ws.Find(s => s.num == (int)PT_SET.AutoChkSelectWs);

                    var res1 = ws.GetPcChkMod(out var pUCFFactoryMode1, out var pUCFFactoryMode2, out var temp1, out var temp2);
                    if (res1)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"func获取工位{ws.disc}是否在生产模式成功");
                        var bAutoChkMod = ws.PcIsProductModShow(pUCFFactoryMode1, pUCFFactoryMode2);
                        if (!bAutoChkMod)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"func提示生产模式后选择停机");
                            return;
                        }
                    }
                    else
                    {
                        var errmsg = ws.disc + ",";
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"func获取工位{ws.disc}点检模式失败");
                        return;
                    }          
                }

                #region  待机细分

                if (PT_SET.Isshowmsg)
                { 
                    PT_SET.Isshowmsg = false;
                    PT_SET.WaitMode = PT_SET.WaitMode == 0 ? 26 : PT_SET.WaitMode;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("待机上传结束事件，原因为{0}", PT_SET.WaitMode));
                    Msg.secsManager.Send(new BaseInfo() { Id = 241, Value = PT_SET.WaitMode.ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 242, Value = Encoding.UTF8.GetBytes(MesData.CobMesIdleStatusDict[PT_SET.WaitMode]).Aggregate("", (current, next) => current + " " + next).TrimStart() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 9 }, TypeId: 2);
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                }

                #endregion

                //启动线程
                Task TskTest = new Task(() => { test_th(); });
                TskTest.Start();

                #region 料仓进入
                //OK料仓
                if (COM.traybox_ok.tray_cur == null && COM.traybox_ok.IsReady)
                {
                    COM.traybox_ok.IsReady = false;
                    Task tskok = new Task(() =>
                    {
                        COM.traybox_ok.Tray_Action(COM.UDLoad1.Demo);
                    });
                    tskok.Start();
                }
                //OK料仓
                if (COM.traybox_ng.tray_cur == null && COM.traybox_ng.IsReady)
                {
                    COM.traybox_ng.IsReady = false;
                    Task tskng = new Task(() =>
                    {
                        COM.traybox_ng.Tray_Action(COM.UDLoad1.Demo);
                    });
                    tskng.Start();
                }
                //OK料仓
                if (COM.traybox_fd.tray_cur == null && COM.traybox_fd.IsReady)
                {
                    COM.traybox_fd.IsReady = false;
                    Task tskfd = new Task(() =>
                    {
                        COM.traybox_fd.Tray_Action(COM.UDLoad1.Demo);
                    });
                    tskfd.Start();
                }
                #endregion

                UpDownLoad.RunTask();
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "运行线程启动" : "Run thread start     (运行线程启动)");
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                MT.OnlyKeyLOn(MT.GPIO_OUT_KL_START);
                MT.GPIO_OUT_ALM_GREEN.SetOn();
                MT.GPIO_OUT_ALM_RED.SetOff();
                MT.GPIO_OUT_ALM_YELLOW.SetOff();
                MT.GPIO_OUT_ALM_BEEPER.SetOff();
                Thread.Sleep(100);
                //确认是否重工
                DRpt.Report_Product(VAR.gsys_set.cur_product_name, !VAR.Isnormal);
                DRpt.Report_Status(DReport.EmStatus.Run, DReport.EmHareware.Null, DReport.EmStatus.Run.GetDescription(VAR.IsChinese));
                int i = 3000;
                bool show = true;
                while (true)
                {
                    Thread.Sleep(10);
                    i++;
                    if(i>=3000)
                    {
                        i = 0;
                        for (int t = 1; t < 37; t++)
                        {
                            Msg.secsManager.Send(new BaseInfo() { Id = t, Value = "false" }, 1);
                        }
                        if (COM.traybox_fd.ChgML || COM.traybox_ng.ChgML || COM.traybox_ok.ChgML)
                        {

                            //DRpt.Report_Status(DReport.EmStatus.Wait, DReport.EmHareware.Null, DReport.EmStatus.Wait.GetDescription(VAR.IsChinese));
                            //Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Wait).ToString() });
                            //Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                            VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                        }
                        else
                        {
                            VAR.gsys_set.status = EM_SYS_STA.RUN;
                            show = true;
                          
                        }
                    }
                    //测试线程退出则通知上料/下料退出
                    if (!brun)
                    {
                        if (UpDownLoad.status == UpDownLoad.EM_STA.WAIT)
                            UpDownLoad.bquit = true;
                    }
                    if( VAR.sys_inf.info.Contains("运行"))
                        VAR.gsys_set.status = EM_SYS_STA.RUN;
                    if (UpDownLoad.brun || brun)
                    {
                        // VAR.sys_inf.Set(EM_ALM_STA.NOR_BLUE, "运行", 0, false);
                        VAR.gsys_set.status = EM_SYS_STA.RUN;


                        MT.GPIO_OUT_KL_START.SetOn();
                        MT.GPIO_OUT_KL_RESET.SetOff();
                        MT.GPIO_OUT_KL_STOP.SetOff();
                    }
                    else
                    {
                        if (COM.ws1.TestStatus == WS.EM_TEST_STA.EMPTY && COM.ws2.TestStatus == WS.EM_TEST_STA.EMPTY &&
                          COM.ws3.TestStatus == WS.EM_TEST_STA.EMPTY && COM.ws4.TestStatus == WS.EM_TEST_STA.EMPTY && VAR.ClearMt)
                        {
                            bool bSound = NewSysInf.UserParams.bClearSetOffWs;
                            if(bSound)
                            {
                                foreach (var ws in COM.list_ws)
                                     ws.PowerOff(ref VAR.gsys_set.bquit);
                            }
                            if (PT_SET.AutoChkEn)
                            {
                                WS.AutoChkCnt = 0;
                                WS.ListChkTemp.Clear();
                                foreach (var ws in COM.list_ws)
                                {
                                    WS.TempAutoChkCnt = -1;
                                    foreach (WS.MdDat md in ws.list_md)
                                        md.bAutoChkOk = false;
                                }
                            }
                            VAR.ClearMt = false;
                            UpDownLoad.bquit = false;
                            //进仓储
                            Task tsk_fd = new Task(() =>
                            {
                                EM_RES res_fd = EM_RES.OK;
                                bool bq_fd = false;
                                res_fd = COM.UDLoad1.traybox_fd.Tray_Action(false, TrayBox.EM_DIR.ONLY_IN);
                                if (res_fd == EM_RES.OK)
                                    MT.Move(ref bq_fd, ref COM.UDLoad1.traybox_fd.ax_z, 0);
                            });
                            tsk_fd.Start();
                            Task tsk_ok = new Task(() =>
                            {
                                EM_RES res_ok = EM_RES.OK;
                                bool bq_ok = false;
                                res_ok = COM.UDLoad1.traybox_ok.Tray_Action(false, TrayBox.EM_DIR.ONLY_IN);
                                if (res_ok == EM_RES.OK)
                                    MT.Move(ref bq_ok, ref COM.UDLoad1.traybox_ok.ax_z, 0);
                            });
                            tsk_ok.Start();
                            Task tsk_ng = new Task(() =>
                            {
                                EM_RES res_ng = EM_RES.OK;
                                bool bq_ng = false;
                                res_ng = COM.UDLoad1.traybox_ng.Tray_Action(false, TrayBox.EM_DIR.ONLY_IN);
                                if (res_ng == EM_RES.OK)
                                    MT.Move(ref bq_ng, ref COM.UDLoad1.traybox_ng.ax_z, 0);
                            });
                            tsk_ng.Start();
                        }
                        break;
                    }

                   
                }
                //归位
                foreach (UpDownLoad ud in COM.List_UDLoad)
                {
                    res = ud.GoZero(ref VAR.gsys_set.bquit, false);

                }

                VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                Thread.Sleep(300);

                //门禁
                if (MT.DoorAlarmMsg == string.Empty && VAR.SysErrAlm.ErrItem == ERR_ALM.EmErrItem.Null)
                {
                    DRpt.Report_Status(DReport.EmStatus.Ready, DReport.EmHareware.Null, DReport.EmStatus.Ready.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Ready).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "就绪" : "Ready", 0);
                }
                else
                {
                    DRpt.Report_Status(DReport.EmStatus.Error, DReport.EmHareware.Null, DReport.EmStatus.Error.GetDescription(VAR.IsChinese));
                    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Error).ToString() });
                    Msg.secsManager.Send(new BaseInfo() { Id = 1 }, 2);
                    MT.ErrAlmTip();
                }
            }
            finally
            {
                bRuning = false;
                MT.GPIO_OUT_ALM_GREEN.SetOff();
                MT.GPIO_OUT_KL_START.SetOff();
                MT.OnlyKeyLOn(MT.GPIO_OUT_KL_STOP);
                VAR.gsys_set.status = EM_SYS_STA.STANDBY;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "总运行线程结束" : "Total running thread ends    (总运行线程结束)");
            }
        }

        #endregion

        #region 测试线程
        public static bool brun = false;


        public static void test_th()
        {
            EM_RES res = EM_RES.OK;
            brun = true;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "测试线程启动" : "Test thread start     (测试线程启动)");
            int tick = System.Environment.TickCount;
            bool TurnTestDelay = false;
            try
            {
                while (VAR.gsys_set.bclose == false && VAR.gsys_set.bquit == false)
                {

                    if (VAR.gsys_set.bquit || WS.bquit || WS.bpause)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "测试线程结束" : "Test thread ends     (测试线程结束)");
                        break;
                    }
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "启动测试" : "Start test    (启动测试)");
                    //运行测试
                    COM.ws1.RunTestTask();
                    COM.ws2.RunTestTask();
                    COM.ws3.RunTestTask();
                    COM.ws4.RunTestTask();

                    //上下料
                    WS workstation = Turntable.GetWSOnFeedPos;
                    if (workstation == null)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "上下料前，转台位置异常!" : "ERROR:Turntable position abnormal before updownload!", DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable, ErrCode: ShowErrMsg.WsErr);
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "转台位置异常!" : "TT ERR", 20, true);
                        break;
                    }

                    //等待测试完成
                    workstation.WaitTestTask(ref VAR.gsys_set.bquit);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}:{1},上下料:{2}", workstation.disc, workstation.Status.ToString(), UpDownLoad.status.ToString()) : string.Format("{0}:{1},updwload:{2}      ({0}:{1},上下料:{2})", workstation.disc, workstation.Status.ToString(), UpDownLoad.status.ToString()));
                    //workstation.list_md.Find(s => { return s.res > -1; });
                    bool bfeed = false;
                    bool bchk = false;
                    int enablecnt = 0;
                    bool wsenable = true;
                    foreach (WS.MdDat md in workstation.list_md)
                    {
                        //if (VAR.gsys_set.isChkMode && VAR.ChkPC != md.PC_ID)
                        //{
                        //    //md.res = -2;
                        //    //bchk = false;
                        //    continue;
                        //}
                        //if (VAR.gsys_set.isChkMode && VAR.ChkPC == md.PC_ID)
                        //{
                        //    bchk = true;
                        //}
                        if (md.benable && md.res > -1)
                        {
                            bfeed = true;
                            //break;
                        }
                        if (!md.benable)
                        {
                            enablecnt++;
                            md.res = -2;
                        }
                    }

                    if (enablecnt == workstation.list_md.Count)
                    {
                        wsenable = false;
                        workstation.TestStatus = WS.EM_TEST_STA.EMPTY;
                    }

                    if (VAR.gsys_set.isChkMode && !PT_SET.bGrrFlow)
                    {
                        if (workstation.ChkCnt > 0 && workstation.ChkCnt < VAR.ChkCnt && !VAR.ClearMt) bfeed = false;
                        else if (workstation.ChkCnt >= VAR.ChkCnt && !VAR.ClearMt && wsenable) VAR.ClearMt = true;
                    }

                    if (PT_SET.bGrrFlow)
                    {
                        // bchk = false;
                        if (workstation.GrrUDLcnt > 0 && workstation.GrrTestCnt < PT_SET.GRRTestCnt && !VAR.ClearMt)
                            bfeed = false;
                        if (workstation.GrrUDLcnt >= PT_SET.GRRUdlCnt && !VAR.ClearMt && wsenable) VAR.ClearMt = true;
                    }
                    //if (VAR.gsys_set.isChkMode && !bfeed)
                    //    workstation.TestStatus = WS.EM_TEST_STA.EMPTY;
                    workstation.Iserrfirstbox = true;
                    bool readyByCompleted = bfeed && workstation.TestStatus == WS.EM_TEST_STA.COMPLETED;
                    bool readyByEmpty = wsenable && workstation.TestStatus == WS.EM_TEST_STA.EMPTY && !VAR.ClearMt;
                    bool readyByFeedStatus = workstation.FeedStatus == WS.EM_STA.REDAYFORUPDOWNLOAD;

                    //检查是否需要下料
                    // if (!bnotfeed && (bfeed && workstation.TestStatus == WS.EM_TEST_STA.COMPLETED || (wsenable && workstation.TestStatus == WS.EM_TEST_STA.EMPTY && !VAR.ClearMt && (!VAR.gsys_set.isChkMode || (VAR.gsys_set.isChkMode && bchk))) || workstation.FeedStatus == WS.EM_STA.REDAYFORUPDOWNLOAD))
                    if (!bnotfeed && (readyByCompleted || readyByEmpty || readyByFeedStatus))
                    {
                        //检测上下料安全
                        //todo
                        if (PT_SET.bCool)
                        {
                            workstation.gpio_out_gz_wind.SetOff();
                            if (workstation.gpio_out_gz_wind.res != EM_RES.OK) break;
                        }
                        //转到上料位
                        res = workstation.SetupForFeed(ref VAR.gsys_set.bquit);
                        if (res != EM_RES.OK) break;

                        //光幕感应
                        if (MT.GPIO_IN_LIGHT.isON)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "上下料前，光幕感应异常!" : "Light curtain induction abnormal before updownload.    (上下料前，光幕感应异常!)", DReport.EmErrCode.HarewareProtect, (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.StautsAbnomal, ErrCode: ShowErrMsg.LightErr);
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "光幕感应异常!" : "LCurtain ERR", 20, true);
                            break;
                        }

                        if (VAR.isAutoChkMode && (workstation.num == PT_SET.AutoChkSelectWs) && !VAR.ClearMt)
                        {

                            var tempOldCnt = WS.TempAutoChkCnt;
                            var tempNewCnt = WS.AutoChkCnt;
                            if (tempOldCnt != tempNewCnt)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "提示点检模组上料");

                               var bok = workstation.PutModShowMsg();
                                if (!bok)
                                {
                                    return;
                                }
                                WS.TempAutoChkCnt = WS.AutoChkCnt;
                            }


                        }
                        workstation.FeedStatus = WS.EM_STA.REDAYFORUPDOWNLOAD;
                        // 重置计时器和状态
                        if(AutoInspectionParameter.ifcheck&&(workstation.num+1==PT_SET.CheckWs) && !VAR.ClearMt)
                        {
                            AutoInspectionParameter.ifcheckontime_enabled = false;
                            AutoInspectionParameter._elapsedMilliseconds = 0;
                            AutoInspectionParameter.ifcheckontime = false;
                            AutoInspectionParameter.ifcheck = false;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{workstation.num + 1}重置机台点检状态");
                        }

                        ////通知上下料
                        //if(workstation.FeedStatus != WS.EM_STA.REDAYFORUPLOAD)
                        //    workstation.FeedStatus = WS.EM_STA.REDAYFORDOWNLOAD;
                        bool bq = false;

                        if (PT_SET.closeazd)
                        {
                            workstation.PowerOff(ref bq);
                        }

                        //等待上下料完成
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}等待上下料...", workstation.disc) : string.Format("{0} Wait for updownload...          ({0}等待上下料...)", workstation.disc));
                        while (!VAR.gsys_set.bquit)
                        {
                            if (Turntable.GetCurSta > Turntable.EM_STA.POS3 && (UpDownLoad.status == UpDownLoad.EM_STA.UPDOWNLOADEND))
                            {
                                VAR.gsys_set.bquit = true;
                                if (!WS.bpause)
                                    WS.bquit = true;
                                UpDownLoad.bquit = true;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}上料/下料时,转盘位置异常!", workstation.disc) : string.Format("{0}Turntable position abnormal when upload or download.           ({0}上料/下料时,转盘位置异常!)", workstation.disc), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable, ERR_ALM.EmErrItem.StautsAbnomal);
                                return;
                            }
                            if (UpDownLoad.brun == false)
                            {
                                List<string> udSnapshot = new List<string>();
                                foreach (WS.MdDat md in workstation.list_md)
                                {
                                    if (md.benable)
                                    {
                                        udSnapshot.Add(string.Format("工位{0}:res={1},test_idx={2},bc={3}", md.Num, md.res, md.test_idx, string.IsNullOrWhiteSpace(md.bardcode) ? "EMPTY" : md.bardcode));
                                    }
                                }
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}上下料线程退出快照,status={1},FeedStatus={2},UpDownLoad.status={3}->{4}", workstation.disc, workstation.Status, workstation.FeedStatus, UpDownLoad.status, string.Join(" | ", udSnapshot)));
                                if (UpDownLoad.status == UpDownLoad.EM_STA.ERR)
                                {
                                    if (WS.bpause)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 暂停打断上下料,恢复后必须先重新上下料", workstation.disc));
                                    }
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0}上下料异常，测试线程结束", workstation.disc) : string.Format("{0}ERROR:Updownload, test thread ends       ({0}上下料异常，测试线程结束)", workstation.disc));
                                    //VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "上下料异常!", 20, true);
                                    if (VAR.SysErrAlm.ErrItem != ERR_ALM.EmErrItem.Null)
                                    {
                                        MT.ST_WARN st_warn = new MT.ST_WARN();
                                        warning fr_warn = new warning();
                                        st_warn.ok_txt = VAR.IsChinese ? "确认" : "OK";
                                        st_warn.ws = null;
                                        st_warn.msg = VAR.SysErrAlm.ErrStr;
                                        st_warn.lb_msg = VAR.SysErrAlm.ErrStr;
                                        VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, Utility.GetDescription(VAR.SysErrAlm.ErrItem, VAR.IsChinese), 20, true);
                                        st_warn.title = VAR.IsChinese ? "提示:" + VAR.SysErrAlm.ErrStr : "Tip:" + VAR.SysErrAlm.ErrStr;
                                        MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.Null);
                                    }
                                }

                                //break;
                                return;
                            }
                            //if (COM.UDLoad2.brun_ud == false)
                            //{
                            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上料异常，测试线程结束", workstation.disc));
                            //    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "上下料2异常!", 20, true);
                            //    return;
                            //}
                            if (workstation.FeedStatus == WS.EM_STA.REDAYFORTEST && UpDownLoad.status != UpDownLoad.EM_STA.UPDOWNLOADEND)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}上下料结束", workstation.disc) : string.Format("{0} updownload ends     ({0}上下料结束)", workstation.disc));
                                if (VAR.ClearMt)
                                    workstation.SetupForTest(ref VAR.gsys_set.bquit);
                                TurnTestDelay = true;
                                if (PT_SET.bGrrFlow)
                                {
                                    workstation.GrrTestCnt = 0;
                                    workstation.GrrUDLcnt++;
                                }
                                break;
                            }
                            Thread.Sleep(50);
                        }
                        workstation.Status = WS.EM_STA.REDAY;
                    }
                    else
                    {
                        //if ((workstation.TestStatus != WS.EM_TEST_STA.EMPTY)&&(!VAR.gsys_set.isChkMode||(VAR.gsys_set.isChkMode  && bchk)))
                        if (workstation.TestStatus != WS.EM_TEST_STA.EMPTY)
                        {
                            //复位测试结果
                            workstation.ResetResultOfMd();
                            //更新物料状态
                            workstation.TestStatus = WS.EM_TEST_STA.UNTEST;
                            res = workstation.WaitBeforeOpenImage(ref VAR.gsys_set.bquit);
                            if (res != EM_RES.OK)
                            {
                                workstation.Status = WS.EM_STA.LINKERR;
                                break;
                            }
                            //提前开图
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 启动测试", workstation.disc) : string.Format("{0}Start test     ({0} 启动测试)", workstation.disc));
                            res = workstation.StartTestFlow(0, WS.Demo);
                            if (res != EM_RES.OK)
                            {
                                Thread.Sleep(1500);
                                res = workstation.StartTestFlow();
                                if (res != EM_RES.OK)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} StartTestFlow err", workstation.disc));
                                    workstation.Status = WS.EM_STA.LINKERR;
                                    break;
                                }
                            }
                            //计时开始
                            workstation.tmr = System.Environment.TickCount;

                            //通知测试
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知REDAYFORTEST", workstation.disc) : string.Format("{0}Notice  REDAYFORTEST     ({0} 通知REDAYFORTEST)", workstation.disc));
                            if (PT_SET.HallEn && !WS.Demo)
                            {

                                res = workstation.TurnToFeedSafe(ref VAR.gsys_set.bquit);
                                if (res != EM_RES.OK)
                                {
                                    workstation.Status = WS.EM_STA.ERR;
                                    break;
                                }
                                int sta = 0;
                                while (!VAR.gsys_set.bquit)
                                {
                                    res = workstation.WaitTestResult(ref sta, PT_SET.TestTime, WS.Demo);
                                    if (res == EM_RES.PARA_ERR || res == EM_RES.QUIT)
                                    {

                                        //不同步等异常
                                        break;
                                    }
                                    else if (res != EM_RES.OK)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} WaitTestResult err", workstation.disc));
                                        workstation.Status = WS.EM_STA.LINKERR;
                                        break;
                                    }

                                    if (sta == 301)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试Hall", workstation.disc) : string.Format("{0} Notice Hall     ({0} 通知测试Hall)", workstation.disc));
                                        res = workstation.NextTest(sta, WS.Demo);
                                        if (res != EM_RES.OK)
                                        {
                                            res = workstation.NextTest(sta, WS.Demo);
                                            if (res != EM_RES.OK)
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", workstation.disc) : string.Format("{0}  ERROR:Notice test!      ({0} 通知测试出错!)", workstation.disc), emerr: DReport.EmErrCode.TestFailed);
                                                workstation.TestStatus = WS.EM_TEST_STA.ERROR;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                if (res == EM_RES.PARA_ERR || res == EM_RES.QUIT || res != EM_RES.OK)
                                    break;
                                res = workstation.SetupForTest(ref VAR.gsys_set.bquit);
                                if (res != EM_RES.OK)
                                {
                                    workstation.Status = WS.EM_STA.ERR;
                                    break;
                                }
                            }
                        }

                        workstation.FeedStatus = WS.EM_STA.REDAYFORTEST;
                    }

                    //等待测试完成
                    COM.ws1.WaitTestTask(ref VAR.gsys_set.bquit);
                    COM.ws2.WaitTestTask(ref VAR.gsys_set.bquit);
                    COM.ws3.WaitTestTask(ref VAR.gsys_set.bquit);
                    COM.ws4.WaitTestTask(ref VAR.gsys_set.bquit);

                    //测试结果
                    foreach (WS ws in COM.list_ws)
                    {
                        if (ws.TestStatus == WS.EM_TEST_STA.ERROR)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 测试出错!", ws.disc) : string.Format("{0} Test err!      ({0} 测试出错!)", ws.disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + ws.num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                            //VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, string.Format("{0}出错", ws.disc), 20, true);
                            VAR.gsys_set.bpause = true;
                            res = EM_RES.ERR;
                            break;
                        }
                    }
                    if (res != EM_RES.OK || VAR.gsys_set.bquit || WS.bquit || WS.bpause)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "测试异常，测试线程结束" : "Test err, test thread ends      (测试异常，测试线程结束)");
                        break;
                    }

                    GyroscopeMonitor.SetState(GyroCheckState.Stop);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("测试完成,T={0:F2}s", (Environment.TickCount - tick) / 1000.0) : string.Format("Test completed,T={0:F2}s          (测试完成,T={0:F2}s)", (Environment.TickCount - tick) / 1000.0));
                    if (COM.ws1.TestStatus == WS.EM_TEST_STA.EMPTY && COM.ws2.TestStatus == WS.EM_TEST_STA.EMPTY &&
                        COM.ws3.TestStatus == WS.EM_TEST_STA.EMPTY && COM.ws4.TestStatus == WS.EM_TEST_STA.EMPTY && VAR.ClearMt)
                    {
                        //VAR.ClearMt = false;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "清料完成,系统退出!" : "Clearing is complete and the system exits!          (清料完成,系统退出!)");
                        //进仓储
                        //Task tsk_fd = new Task(() =>
                        //{
                        //    EM_RES res_fd = EM_RES.OK;
                        //    bool bq_fd = false;
                        //    res_fd = COM.UDLoad1.traybox_fd.Tray_Action(TrayBox.EM_DIR.ONLY_IN);
                        //    if (res_fd == EM_RES.OK)
                        //        MT.Move(ref bq_fd, ref COM.UDLoad1.traybox_fd.ax_z, 0);
                        //});
                        //Task tsk_ok = new Task(() =>
                        // {
                        //     EM_RES res_ok = EM_RES.OK;
                        //     bool bq_ok = false;
                        //     res_ok = COM.UDLoad1.traybox_ok.Tray_Action(TrayBox.EM_DIR.ONLY_IN);
                        //     if (res_ok == EM_RES.OK)
                        //         MT.Move(ref bq_ok, ref COM.UDLoad1.traybox_ok.ax_z, 0);
                        // });
                        //Task tsk_ng = new Task(() =>
                        //{
                        //    EM_RES res_ng = EM_RES.OK;
                        //    bool bq_ng = false;
                        //    res_ng = COM.UDLoad1.traybox_ng.Tray_Action(TrayBox.EM_DIR.ONLY_IN);
                        //    if (res_ng == EM_RES.OK)
                        //        MT.Move(ref bq_ng, ref COM.UDLoad1.traybox_ng.ax_z, 0);
                        //});
                        if (VAR.ClearMt)
                        {
                            foreach (WS ws in COM.list_ws)
                            {
                                ws.GrrTestCnt = 0;
                                ws.GrrUDLcnt = 0;
                                ws.ChkCnt = 0;
                            }
                        }

                        break;
                    }

                    //if (!PT_SET.turnon)
                    //{
                    //    //提前开图
                    //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0} 启动测试", workstation.disc): string.Format("{0} Start test        ({0} 启动测试)", workstation.disc));
                    //    res = workstation.StartTestFlow(0, WS.Demo);
                    //    if (res != EM_RES.OK)
                    //    {
                    //        Thread.Sleep(1500);
                    //        res = workstation.StartTestFlow();
                    //        if (res != EM_RES.OK)
                    //        {
                    //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} StartTestFlow err", workstation.disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable+ workstation.num+1, ERR_ALM.EmErrItem.TestAbnormal);
                    //            workstation.Status = WS.EM_STA.LINKERR;
                    //            break;
                    //        }
                    //    }
                    //}


                    //旋转转台
                    workstation.FeedStatus = WS.EM_STA.REDAYFORTEST;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}转下一位置...", workstation.disc) : string.Format("{0} turn to next pos         ({0}转下一位置...)", workstation.disc));
                    if (UpDownLoad.status != UpDownLoad.EM_STA.UPDOWNLOADEND)
                    {
                        Thread.Sleep(100);
                        if (UpDownLoad.status != UpDownLoad.EM_STA.UPDOWNLOADEND)
                        {
                            if (workstation.FeedStatus != WS.EM_STA.REDAYFORTEST)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}旋转时状态异常，{1}", workstation.disc, workstation.FeedStatus.ToString()) : string.Format("{0}  Abnormal state during rotation ,{1}        ({0}旋转时状态异常，{1})", workstation.disc, workstation.FeedStatus.ToString()), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable + workstation.num + 1, ERR_ALM.EmErrItem.StautsAbnomal);
                                break;
                            }
                            res = workstation.SetupForTest(ref VAR.gsys_set.bquit, TurnTestDelay);
                            if (res != EM_RES.OK) break;
                            TurnTestDelay = false;
                            //确保在测试位置
                            if (workstation.isInTestPos)
                            {
                                res = Turntable.MoveToNext(ref VAR.gsys_set.bquit);
                                if (res != EM_RES.OK)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "转盘异常!" : "Turntable abnormal!(转盘异常!)", DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable, ERR_ALM.EmErrItem.StautsAbnomal);
                                    //  VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转盘异常!", 20, true, ERR_ALM.EmErrItem.StautsAbnomal);
                                    break;
                                }
                            }
                            else
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}上下料后，翻转时不在测试位", workstation.disc) : string.Format("After updownload,{0} is not in test position when flipped        ({0}上下料后，翻转时不在测试位)", workstation.disc), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable + workstation.num + 1, ERR_ALM.EmErrItem.StautsAbnomal);
                                break;
                            }
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("切换工位周期, T={0:F2}", (Environment.TickCount - tick) / 1000.0) : string.Format("Switching station cycle,T={0:F2}        (切换工位周期, T={0:F2})", (Environment.TickCount - tick) / 1000.0));
                            tick = System.Environment.TickCount;
                            continue;
                        }
                    }
                    //旋转状态
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("旋转状态异常, 工站:{0},下料:{1},上料:{2}", workstation.Status.ToString(), DownloadModle.status.ToString(), UploadModle.status.ToString()) : string.Format("Abnormal rotation,WS:{0},Download:{1},Upload:{2}           (旋转状态异常, 工站:{0},下料:{1},上料:{2})", workstation.Status.ToString(), DownloadModle.status.ToString(), UploadModle.status.ToString()), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.TurnTable + workstation.num + 1, ERR_ALM.EmErrItem.StautsAbnomal);
                    break;
                }
            }
            finally
            {
                brun = false;
                TurnTestDelay = false;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "测试线程结束" : "Test thread ends    (测试线程结束)");
            }
        }
        #endregion

        #region 上下料线程

        //public static void UpDownLoad_th()
        //{
        //    EM_RES res = EM_RES.OK;
        //    EM_RES res2 = EM_RES.OK;
        //    int tick = 0;

        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "运行线程启动");
        //    VAR.gsys_set.status = EM_SYS_STA.RUN;
        //    VAR.sys_inf.Set(EM_ALM_STA.NOR_BLUE, "正在运行", -1);
        //    MT.GPIO_OUT_KL_START.SetOn();
        //    MT.SetAllAxToWorkSpd();

        //    while (VAR.gsys_set.bclose == false && VAR.gsys_set.bquit == false)
        //    {

        //        //上下料工位准备
        //        WS workstation = Turntable.GetWSOnFeedPos;
        //        if (workstation == null)
        //        {
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料前，转台位置异常!");
        //            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //            break;
        //        }
        //        //确认是否要上下料
        //        if (!bnotfeed && (workstation.TestStatus == WS.EM_TEST_STA.COMPLETED || workstation.TestStatus == WS.EM_TEST_STA.EMPTY))
        //        {
        //            res = workstation.SetupForFeed(ref VAR.gsys_set.bquit);
        //            if (res != EM_RES.OK) break;
        //            //光幕感应
        //            if (MT.GPIO_IN_LIGHT.isON)
        //            {
        //                break;
        //            }
        //        }

        //        //运行上下料线程
        //        DownloadModle.RunTask();
        //        Thread.Sleep(200);
        //        UploadModle.RunTask();




        //        //上下料结果
        //        if (!bnotfeed)
        //        {
        //            res = DownloadModle.WaitTask(ref VAR.gsys_set.bquit);
        //            res2 = UploadModle.WaitTask(ref VAR.gsys_set.bquit);
        //            if (res != EM_RES.OK)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "下料异常!", 20, true);
        //                break;
        //            }

        //            if (res2 != EM_RES.OK)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上料异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "上料异常!",20, true);
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            //上下料工位归位
        //            workstation = Turntable.GetWSOnFeedPos;
        //            if (workstation == null)
        //            {
        //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料后，转台位置异常!");
        //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //                break;
        //            }

        //            //复位测试结果
        //            workstation.ResetResultOfMd();
        //            //更新物料状态
        //            workstation.TestStatus = WS.EM_TEST_STA.UNTEST;

        //            //提前开图
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 启动测试", workstation.disc));
        //            res = workstation.StartTestFlow();
        //            if (res != EM_RES.OK)
        //            {
        //                Thread.Sleep(1500);
        //                res = workstation.StartTestFlow();
        //                if (res != EM_RES.OK)
        //                {
        //                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
        //                        string.Format("{0} StartTestFlow err", workstation.disc));
        //                    workstation.Status = WS.EM_STA.LINKERR;
        //                }
        //            }

        //            //计时开始
        //            workstation.tmr = System.Environment.TickCount;
        //        }

        //        //上下料工位归位
        //        workstation = Turntable.GetWSOnFeedPos;
        //        if (workstation == null)
        //        {
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上下料后，转台位置异常!");
        //            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "转台位置异常!", 20, true);
        //            break;
        //        }

        //        res = workstation.TurnToTest(ref VAR.gsys_set.bquit);
        //        if (res != EM_RES.OK) break;

        //        //旋转转台
        //        res = Turntable.MoveToNext(ref VAR.gsys_set.bquit);
        //        if (res != EM_RES.OK) break;

        //        //计时
        //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "U T=" + (Environment.TickCount - tick).ToString());
        //    }

        //    MT.GPIO_OUT_ALM_GREEN.SetOff();
        //    MT.GPIO_OUT_KL_START.SetOff();
        //    VAR.gsys_set.status = EM_SYS_STA.STANDBY;
        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "总运行线程结束");
        //}
        #endregion

        #region 停止

        public static void stop()
        {
            int n;

            //if (VAR.gsys_set.status == CONST.SYS_STATUS_EMG || VAR.gsys_set.status == CONST.SYS_STATUS_ERR || VAR.gsys_set.status == CONST.SYS_STATUS_UNKOWN)
            {
                MT.AllAxStop();
                //    return;
            }

            //唤醒再停止
            VAR.gsys_set.bpause = false;
            // VAR.gsys_set.mre_pause.Set();
            //quit
            for (n = 0; n < 50; n++)
            {
                VAR.gsys_set.bquit = true;
                UploadModle.bquit = true;
                DownloadModle.bquit = true;
                Thread.Sleep(10);
                //Application.DoEvents();
            }

            //wait stop
            //EM_RES ret = TD.Task_Stop();
            EM_RES ret = EM_RES.OK;
            if (ret == EM_RES.OK)
            {
                if (VAR.gsys_set.status != EM_SYS_STA.EMG && VAR.gsys_set.status != EM_SYS_STA.ERR &&
                    VAR.gsys_set.status != EM_SYS_STA.UNKOWN)
                    VAR.gsys_set.status = EM_SYS_STA.STANDBY;
            }
        }

        #endregion

        #region 暂停

        //public static bool pause(ref EM_SYS_STA status, ref bool bquit, bool bquit2 = false)
        //{
        //    bool bpause = false;
        //    if (VAR.gsys_set.bpause == true || MT.GPIO_IN_FR_DOOR.isOFF)
        //    {
        //        bpause = true;
        //        VAR.gsys_set.bpause = true;
        //        if (VAR.gsys_set.status != EM_SYS_STA.PAUSE) MT.Beeper(100);
        //    }

        //    while ((VAR.gsys_set.mode & EM_SYS_MODE.STEP) == EM_SYS_MODE.STEP &&
        //           VAR.gsys_set.status == EM_SYS_STA.RUN || (VAR.gsys_set.bpause == true || MT.GPIO_IN_FR_DOOR.isOFF))
        //    {
        //        if (VAR.gsys_set.bpause == true || MT.GPIO_IN_FR_DOOR.isOFF)
        //        {
        //            status = EM_SYS_STA.PAUSE;
        //            VAR.gsys_set.status = EM_SYS_STA.PAUSE;
        //        }

        //        //继续运行
        //        if (MT.GPIO_IN_KEY_START.AssertON())
        //        {
        //            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
        //            break;
        //        }

        //        //复位键退出
        //        if (MT.GPIO_IN_KEY_STOP.AssertON())
        //        {
        //            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
        //            //if (!VAR.isStepMode)
        //            bquit = true;
        //            break;
        //        }

        //        if (bquit || bquit2) break;

        //        //发生错误
        //        if (VAR.gsys_set.status == EM_SYS_STA.EMG || VAR.gsys_set.status == EM_SYS_STA.ERR ||
        //            VAR.gsys_set.status == EM_SYS_STA.UNKOWN)
        //        {
        //            bquit = true;
        //            break;
        //        }

        //        Thread.Sleep(10);
        //        Application.DoEvents();
        //    }

        //    //检查系统状态
        //    if (VAR.gsys_set.status == EM_SYS_STA.RUN || VAR.gsys_set.status == EM_SYS_STA.PAUSE)
        //    {
        //        status = EM_SYS_STA.RUN;
        //        VAR.gsys_set.status = EM_SYS_STA.RUN;
        //        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
        //    }
        //    else
        //    {
        //        //bquit = true;
        //    }

        //    return bpause;
        //}

        #endregion

        #region 退出

        public static EM_RES Close()
        {
            //stop
            stop();
            VAR.gsys_set.bclose = true;
            Thread.Sleep(100);
            //Application.DoEvents();
            //Thread.Sleep(100);
            //Application.DoEvents();
            for (int n = 0; n < 100; n++)
            {
                VAR.gsys_set.bquit = true;
                VAR.gsys_set.bpause = false;
                VAR.gsys_set.bclose = true;
                Thread.Sleep(10);
                //Application.DoEvents();
            }

            VAR.gsys_set.bquit = false;
            //MT.ResetIO();

            //close card
            MT.Close();

            //disconnet camera
            foreach (Cam cam in COM.ListCam)
            {
                cam.Dispose();
            }

            Thread.Sleep(100);
            //Application.DoEvents();
            //Thread.Sleep(100);
            //Application.DoEvents();

            return EM_RES.OK;
        }

        #endregion
       static int i = 0;
    }

    #endregion

    #region 绘图

    //public class TG_DRAW
    //{
    //    public ST_XY st_ul_pos;    // mm
    //    public ST_XY st_sc;        // mm/pix
    //    int w = 30;             // pix
    //    int h = 30;             // pix
    //    int bdir_x = 1;
    //    int bdir_y = 1;
    //    public bool bmousedonw = false;
    //    public bool bsel_rect = false;
    //    Rectangle sel_rect;
    //    Point pt_start = new Point();
    //    List<Rectangle> list_area = new List<Rectangle>();

    //    #region 初始化
    //    public int Init(List<TG_DATA> list_tg, int pic_width, int pic_height)
    //    {
    //        Rectangle rect = new Rectangle();
    //        double min_dis = 0;
    //        int ret = GetTgMaxRegion(list_tg, ref rect, ref min_dis);
    //        if (ret == CONST.RES_OK)
    //        {
    //            st_sc.x = (pic_width < 0 ? -1 : 1) * (double)rect.Width / (double)(Math.Abs(pic_width) - 30.0 * 2) * bdir_x;
    //            st_sc.y = (pic_height < 0 ? -1 : 1) * (double)rect.Height / (double)(Math.Abs(pic_height) - 30.0 * 2) * bdir_y;
    //            st_ul_pos.x = rect.X - st_sc.x * 30 + (pic_width < 0 ? rect.Width : 0);
    //            st_ul_pos.y = rect.Y - st_sc.y * 30 + (pic_height < 0 ? rect.Height : 0);

    //            w = 30;
    //            if ((int)Math.Abs(min_dis / st_sc.x) < 30) w = (int)Math.Abs(min_dis / st_sc.x);
    //            if (w < 10) w = 10;
    //            h = w;
    //        }
    //        return ret;
    //    }
    //    public TG_DRAW()
    //    { }
    //    public TG_DRAW(List<TG_DATA> list_tg, int pic_width, int pic_height)
    //    {
    //        Init(list_tg, pic_width, pic_height);
    //    }
    //    public TG_DRAW(VAR.ST_XY st_ul_pos, VAR.ST_XY st_sc, int w = 30, int h = 30)
    //    {
    //        this.st_sc = st_sc;
    //        this.st_ul_pos = st_ul_pos;
    //        this.w = w;
    //        this.h = h;
    //    }
    //    #endregion
    //    #region 坐标转换
    //    public Point Pix2MM(int x, int y)
    //    {
    //        Point pt = new Point();
    //        pt.X = (int)(st_ul_pos.x + x * st_sc.x);
    //        pt.Y = (int)(st_ul_pos.y + y * st_sc.y);
    //        return pt;
    //    }
    //    public VAR.ST_XY MM2Pix(double x, double y)
    //    {
    //        VAR.ST_XY st_pos = new VAR.ST_XY();
    //        st_pos.x = (x - st_ul_pos.x) / st_sc.x;
    //        st_pos.y = (y - st_ul_pos.y) / st_sc.y;
    //        return st_pos;
    //    }
    //    public Rectangle MM2Pix(Rectangle rect)
    //    {
    //        rect.X = (int)(((double)rect.X - st_ul_pos.x) / st_sc.x);
    //        rect.Y = (int)(((double)rect.Y - st_ul_pos.y) / st_sc.y);
    //        rect.Width = (int)(((double)rect.Width - 0) / st_sc.x);
    //        rect.Height = (int)(((double)rect.Height - 0) / st_sc.y);
    //        return rect;
    //    }
    //    public PointF MM2Pix(VAR.ST_XYZ st_pos_mm)
    //    {
    //        VAR.ST_XY posInf = MM2Pix(st_pos_mm.x, st_pos_mm.y);
    //        return new PointF((float)posInf.x, (float)posInf.y);
    //    }
    //    public Rectangle Pos2Rec(VAR.ST_XYZ st_pos_mm)
    //    {
    //        PointF ptf = MM2Pix(st_pos_mm);
    //        return new Rectangle((int)(ptf.X - w / 2), (int)(ptf.Y - h / 2), w, h);
    //    }
    //    double DisOfP2P(VAR.ST_XYZ pt1, VAR.ST_XYZ pt2)
    //    {
    //        double dx = pt1.x - pt2.x;
    //        double dy = pt1.y - pt2.y;
    //        return Math.Sqrt(dx * dx + dy * dy);
    //    }
    //    #endregion
    //    #region 计算最小包围区域，目标最小距离
    //    public Rectangle GetTgMaxRegion(List<TG_DATA> list_tg)
    //    {
    //        Rectangle rect = new Rectangle(0, 0, -1, -1);
    //        if (list_tg.Count == 0) return rect;

    //        //最小包含区域  
    //        double min_x = double.MaxValue;
    //        double max_x = double.MinValue;
    //        double min_y = double.MaxValue;
    //        double max_y = double.MinValue;
    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            if (min_x > tg.st_pos_cap_set.x) min_x = tg.st_pos_cap_set.x;
    //            if (max_x < tg.st_pos_cap_set.x) max_x = tg.st_pos_cap_set.x;

    //            if (tg.bBM)
    //            {
    //                if (min_x > tg.st_pos_bm_set.x) min_x = tg.st_pos_bm_set.x;
    //                if (max_x < tg.st_pos_bm_set.x) max_x = tg.st_pos_bm_set.x;
    //            }

    //            if (min_y > tg.st_pos_cap_set.y) min_y = tg.st_pos_cap_set.y;
    //            if (max_y < tg.st_pos_cap_set.y) max_y = tg.st_pos_cap_set.y;

    //            if (tg.bBM)
    //            {
    //                if (min_y > tg.st_pos_bm_set.y) min_y = tg.st_pos_bm_set.y;
    //                if (max_y < tg.st_pos_bm_set.y) max_y = tg.st_pos_bm_set.y;
    //            }
    //        }

    //        rect.X = (int)(bdir_x == 1 ? min_x : max_x);
    //        rect.Y = (int)(bdir_y == 1 ? min_y : max_y);
    //        rect.Width = (int)Math.Abs(max_x - min_x);
    //        rect.Height = (int)Math.Abs(max_y - min_y);

    //        return rect;
    //    }
    //    public double GetTgMinDis(List<TG_DATA> list_tg)
    //    {
    //        //目标间最小距离
    //        double min_dis = double.MaxValue;
    //        double dis = 0;
    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            if (tg.name == "BC" || tg.name == "MK") continue;
    //            foreach (TG_DATA tg_temp in list_tg)
    //            {
    //                if (tg.name == "BC" || tg.name == "MK") continue;
    //                if (tg.id == tg_temp.id) continue;
    //                dis = DisOfP2P(tg.st_pos_cap_set, tg_temp.st_pos_cap_set);
    //                if (min_dis > dis) min_dis = dis;
    //            }
    //        }
    //        return min_dis == double.MaxValue ? 0 : min_dis;
    //    }
    //    public int GetTgMaxRegion(List<TG_DATA> list_tg, ref Rectangle rect, ref double min_dis)
    //    {
    //        if (list_tg.Count == 0) return CONST.RES_PARA_ERR;

    //        rect = GetTgMaxRegion(list_tg);
    //        min_dis = GetTgMinDis(list_tg);
    //        if (rect.Width < 0 || rect.Height < 0 || min_dis == 0) return CONST.RES_ERR;
    //        else return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 生成区域
    //    public int CreateAreaRect(params List<TG_DATA>[] list_list_tg)
    //    {
    //        TARGET target = new TARGET();

    //        List<TG_DATA> list_area_tg = new List<TG_DATA>();
    //        list_area.Clear();
    //        for (int n = 0; n < 8; n++)
    //        {
    //            list_area_tg.Clear();
    //            foreach (List<TG_DATA> list_tg in list_list_tg)
    //            {
    //                list_area_tg.AddRange(target.GetTgByArea(n, list_tg));
    //            }
    //            Rectangle rect = GetTgMaxRegion(list_area_tg);
    //            rect = MM2Pix(rect);
    //            if (rect.Width < 0)
    //            {
    //                rect.Width = -rect.Width;
    //                rect.X -= rect.Width;
    //            }
    //            if (rect.Height < 0)
    //            {
    //                rect.Height = -rect.Height;
    //                rect.Y -= rect.Height;
    //            }
    //            rect.X -= w / 2 + 5;
    //            rect.Y -= h / 2 + 5;
    //            rect.Width += w + 10;
    //            rect.Height += h + 10;
    //            list_area.Add(rect);
    //        }
    //        return list_area.Count;
    //    }
    //    #endregion
    //    #region 目标颜色
    //    public Color GetColorByName(string tgname)
    //    {
    //        switch (tgname)
    //        {
    //            case "TA": return Color.DodgerBlue;
    //            case "TB": return Color.Lime;
    //            case "TC": return Color.Violet;
    //            case "TD": return Color.SkyBlue;
    //            case "MK": return Color.Orange;
    //            case "BC": return Color.Yellow;
    //            case "SEL": return Color.Red;
    //            case "ERR": return Color.Red;
    //            case "STA": return Color.Gray;
    //        }
    //        return Color.White;
    //    }
    //    #endregion
    //    #region 绘制单个目标
    //    public void DrawTg(TG_DATA tg, ref Graphics gg, bool bSolid = true, bool bMTF = false, bool bBM = false, bool bECNUM = false)
    //    {
    //        Color cl_sel = GetColorByName("SEL");
    //        Color cl_tg = GetColorByName(tg.name);
    //        Color cl_err = GetColorByName("ERR");
    //        Color cl_sta = GetColorByName("STA");

    //        Pen p = new Pen(Color.DodgerBlue, 3);

    //        if (gg == null) return;
    //        if (st_sc.x == 0 || st_sc.y == 0) return;

    //        if (bSolid)
    //        {
    //            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
    //            p.Width = 3;
    //        }
    //        else
    //        {
    //            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
    //            p.Width = 1;
    //        }

    //        if (tg.bselected) p.Color = cl_sel;
    //        else if (tg.btf_h_err) p.Color = cl_err;
    //        else if (tg.status > 0) p.Color = cl_sta;
    //        else p.Color = cl_tg;

    //        //显示非MARK点
    //        if (tg.name != "MK" && tg.name != "BC")
    //        {
    //            gg.DrawRectangle(p, Pos2Rec(tg.st_pos_cap_set));
    //            //手动补贴时候打X
    //            if (bMTF && tg.bmtf == false || tg.status < 0 || tg.cap_mode == TG_DATA.EM_CAP_MODE.DISABLE)
    //            {
    //                PointF pt1 = MM2Pix(tg.st_pos_cap_set);
    //                pt1.X -= w / 2;
    //                pt1.Y -= h / 2;
    //                PointF pt2 = pt1;
    //                pt2.X += w;
    //                pt2.Y += h;
    //                PointF pt3 = pt1;
    //                PointF pt4 = pt2;
    //                pt3.Y += h;
    //                pt4.Y -= h;

    //                p.Width = 2;
    //                if (tg.status < 0 || tg.cap_mode == TG_DATA.EM_CAP_MODE.DISABLE) p.Color = cl_tg;
    //                else p.Color = Color.Red;
    //                gg.DrawLine(p, pt1, pt2);
    //                gg.DrawLine(p, pt3, pt4);

    //                if (tg.bselected) p.Color = cl_sel;
    //                else p.Color = cl_tg;
    //            }
    //            //显示BM点
    //            if (bBM && tg.bBM)
    //            {
    //                p.Width = 1;
    //                gg.DrawLine(p, MM2Pix(tg.st_pos_cap_set), MM2Pix(tg.st_pos_bm_set));
    //                if (tg.bBMCap || tg.bselected) p.Color = cl_sel;
    //                else p.Color = cl_tg;

    //                if (bSolid) p.Width = 3;
    //                gg.DrawEllipse(p, Pos2Rec(tg.st_pos_bm_set));
    //            }
    //        }
    //        else
    //        {
    //            //mark
    //            if (!tg.bselected && tg.cap_mode == TG_DATA.EM_CAP_MODE.DISABLE) p.Color = Color.DarkGray;
    //            gg.DrawEllipse(p, Pos2Rec(tg.st_pos_cap_set));
    //        }

    //        //显示编号
    //        int ftsize = 14;
    //        if (w < 12) ftsize = 10;
    //        else if (w < 18) ftsize = 12;
    //        else if (w < 24) ftsize = 14;
    //        Font ft = new Font("宋体", ftsize);
    //        PointF pt = MM2Pix(tg.st_pos_cap_set);
    //        pt.X -= 5;
    //        pt.Y -= 5;
    //        gg.DrawString(tg.id.ToString(), ft, Brushes.White, pt);
    //        if (bECNUM && tg.name != "MK" && tg.name != "BC")
    //        {
    //            pt.X += (5 + w / 2);
    //            gg.DrawString(tg.ec_num.ToString(), ft, Brushes.Red, pt);
    //        }
    //    }
    //    #endregion
    //    #region 绘制目标序列
    //    public void DrawTgList(List<TG_DATA> list_tg, ref Graphics gg, bool bSolid = true, bool bMTF = false, bool bBM = false, bool bBCNum = false)
    //    {
    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            DrawTg(tg, ref gg, bSolid, bMTF, bBM, bBCNum);
    //        }
    //        //显示选框
    //        if (bsel_rect)
    //        {
    //            Pen p = new Pen(Color.Gold, 1);
    //            gg.DrawRectangle(p, sel_rect);
    //        }
    //        //显示分区            
    //        foreach (Rectangle area in list_area)
    //        {
    //            Pen p = new Pen(Color.Gold, 2);
    //            p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
    //            gg.DrawRectangle(p, area);
    //            //显示编号
    //            int ftsize = 14;
    //            Font ft = new Font("宋体", ftsize);
    //            gg.DrawString("A" + list_area.IndexOf(area).ToString(), ft, Brushes.Gold, new Point(area.X, area.Y - 20));
    //        }
    //        //显示刻度
    //    }
    //    #endregion
    //    #region 点选
    //    public TG_DATA SelectByPoint(ref List<TG_DATA> list_tg, int x, int y, bool bsel_one = false)
    //    {
    //        if (bsel_rect) return null;
    //        int select_cnt = 0;

    //        TG_DATA tg_temp = null;

    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            if (tg.bselected) select_cnt++;
    //            if (select_cnt >= 2) break;
    //        }
    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            if (Pos2Rec(tg.st_pos_cap_set).Contains(new Point(x, y)))
    //            {
    //                //单个选择不取反
    //                if (select_cnt >= 2) tg.bselected = !tg.bselected;
    //                else tg.bselected = true;
    //                tg_temp = tg;
    //            }
    //        }
    //        //diselect all
    //        if (!bsel_rect || bsel_one)
    //        {
    //            foreach (TG_DATA tg in list_tg) tg.bselected = false;
    //        }
    //        if (tg_temp != null && bsel_one) tg_temp.bselected = true;

    //        return tg_temp;
    //    }
    //    #endregion
    //    #region 框选
    //    public void StartRectSelect(int x, int y)
    //    {
    //        pt_start.X = x;
    //        pt_start.Y = y;
    //        bmousedonw = true;
    //    }
    //    public void EndRectSelect()
    //    {
    //        bmousedonw = false;
    //        bsel_rect = false;
    //    }
    //    public int SelectByRect(ref List<TG_DATA> list_tg, int end_x, int end_y)
    //    {
    //        if (bmousedonw == false) return 0;

    //        sel_rect = new Rectangle(pt_start.X, pt_start.Y, end_x - pt_start.X, end_y - pt_start.Y);
    //        if (sel_rect.Width < 0)
    //        {
    //            sel_rect.X += sel_rect.Width;
    //            sel_rect.Width = -sel_rect.Width;
    //        }
    //        if (sel_rect.Height < 0)
    //        {
    //            sel_rect.Y += sel_rect.Height;
    //            sel_rect.Height = -sel_rect.Height;
    //        }

    //        if (sel_rect.Width > 3 || sel_rect.Height > 3) bsel_rect = true;
    //        else bsel_rect = false;
    //        if (bsel_rect == false) return 0;

    //        int selcnt = 0;
    //        foreach (TG_DATA tg in list_tg)
    //        {
    //            if (sel_rect.Contains(Pos2Rec(tg.st_pos_cap_set)))
    //            {
    //                tg.bselected = true;
    //                selcnt++;
    //            }
    //            else tg.bselected = false;
    //        }

    //        return selcnt;
    //    }
    //    #endregion
    //    #region 只选当前目标
    //    public void OnlySelCurTG(ref List<TG_DATA> list_tg, ref TG_DATA cur_tg)
    //    {
    //        foreach (TG_DATA tg in list_tg) tg.bselected = false;
    //        if (cur_tg != null) cur_tg.bselected = true;
    //    }
    //    #endregion
    //}
    //#endregion
    //#region 平台扫描
    //public class SCAN
    //{
    //    #region 参数
    //    public bool bscaning = false;
    //    public bool bsaveing = false;
    //    public bool bmousedonw = false;
    //    public bool bsel_rect = false;
    //    List<Rectangle> list_area = new List<Rectangle>();
    //    public Image img;

    //    public VAR.ST_XY st_ul_cap = new VAR.ST_XY();
    //    public VAR.ST_XYA st_ul = new VAR.ST_XYA();
    //    public VAR.ST_XYA st_br = new VAR.ST_XYA();
    //    public double xs, ys;
    //    public double ofs_x, ofs_y;
    //    public double x, y;

    //    public bool bReadyToArray = false;

    //    public string[] array_list_name = new string[7] { "TA", "TB", "TC", "TD", "MK", "BC", "COPY" };
    //    public bool[] array_chk = new bool[7];
    //    public List<TG_DATA> list_tg_a = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_b = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_c = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_d = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_mk = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_bc = new List<TG_DATA>();

    //    public List<TG_DATA> list_tg_copy = new List<TG_DATA>();
    //    public List<TG_DATA> list_tg_array = new List<TG_DATA>();
    //    public List<TG_DATA>[] array_list_tg = new List<TG_DATA>[7];
    //    public List<TG_DATA> list_bc_num = new List<TG_DATA>();
    //    #endregion
    //    #region 初始化
    //    public SCAN()
    //    {
    //    }
    //    #endregion
    //    #region 加载图片及参数
    //    public int LoadImg(string produtname)
    //    {
    //        string fileroad = Path.GetFullPath("..") + "\\product\\" + produtname + "\\temp.inf";
    //        IniFiles inf = new IniFiles(fileroad);
    //        st_ul_cap.x = inf.ReadDouble("SCAN", "START_CAP_X", 250);
    //        st_ul_cap.y = inf.ReadDouble("SCAN", "START_CAP_Y", 250);
    //        st_ul.x = inf.ReadDouble("SCAN", "UL_X", st_ul.x);
    //        st_ul.y = inf.ReadDouble("SCAN", "UL_Y", st_ul.y);
    //        st_ul.a = inf.ReadDouble("SCAN", "UL_A", st_ul.a);
    //        st_br.x = inf.ReadDouble("SCAN", "DR_X", st_br.x);
    //        st_br.y = inf.ReadDouble("SCAN", "DR_Y", st_br.y);
    //        st_br.a = inf.ReadDouble("SCAN", "DR_A", st_br.a);
    //        xs = inf.ReadDouble("SCAN", "XS", 0.23405);
    //        ys = inf.ReadDouble("SCAN", "YS", 0.23405);
    //        ofs_x = inf.ReadDouble("SCAN", "OFS_X", 0);
    //        ofs_y = inf.ReadDouble("SCAN", "OFS_Y", 0);

    //        try
    //        {
    //            Stream s = File.Open(Path.GetFullPath("..") + "\\product\\" + produtname + "\\view.bmp", FileMode.Open);
    //            img = Image.FromStream(s);
    //            s.Close();
    //        }
    //        catch
    //        {
    //            return CONST.RES_ERR;
    //        }
    //        return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 加载数据
    //    public void LoadTGData(bool bloadfrfile = true)
    //    {
    //        array_list_tg[0] = list_tg_a;
    //        array_list_tg[1] = list_tg_b;
    //        array_list_tg[2] = list_tg_c;
    //        array_list_tg[3] = list_tg_d;
    //        array_list_tg[4] = list_tg_mk;
    //        array_list_tg[5] = list_tg_bc;
    //        array_list_tg[6] = list_tg_copy;

    //        //clear
    //        for (int n = 0; n < array_list_tg.Count(); n++)
    //        {
    //            if (array_list_tg[n] != null) array_list_tg[n].Clear();
    //        }

    //        //load
    //        if (bloadfrfile) COM.tg.LoadDat(VAR.gsys_set.cur_product_name);

    //        for (int n = 0; n < COM.tg.list_tg.Count; n++)
    //        {
    //            for (int m = 0; m < 6; m++)
    //            {
    //                if (COM.tg.list_tg[n].name == array_list_name[m])
    //                {
    //                    TG_DATA tg = COM.tg.list_tg[n];
    //                    if (tg.status == -1) tg.bmtf = false;
    //                    else tg.bmtf = true;
    //                    tg.bselected = false;
    //                    array_list_tg[m].Add(tg);
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //    #region 更新目标状态
    //    public int UpdateTGStatus()
    //    {

    //        int idx = 0;

    //        //check
    //        idx = 0;
    //        for (int n = 0; n < 6; n++)
    //        {
    //            foreach (TG_DATA tg in array_list_tg[n])
    //            {
    //                if (COM.tg.list_tg[idx].name != tg.name || COM.tg.list_tg[idx].id != tg.id)
    //                {
    //                    MessageBox.Show("更新贴付状态数据异常!");
    //                    return CONST.RES_ERR;
    //                }
    //                idx++;
    //            }
    //        }
    //        //update
    //        idx = 0;
    //        for (int n = 0; n < 6; n++)
    //        {
    //            foreach (TG_DATA tg in array_list_tg[n])
    //            {
    //                COM.tg.list_tg[idx].bmtf = tg.bmtf;
    //                if (tg.bmtf) COM.tg.list_tg[idx].status = -1;
    //                idx++;
    //            }
    //        }

    //        MessageBox.Show("更新贴付状态成功!");
    //        return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 保存数据
    //    public void SaveTGData()
    //    {
    //        //save tg data
    //        COM.tg.list_tg.Clear();
    //        for (int n = 0; n < 6; n++)
    //        {
    //            foreach (TG_DATA tg in array_list_tg[n]) COM.tg.list_tg.Add(tg);
    //        }
    //        int ret = COM.tg.SaveTgDat(VAR.gsys_set.cur_product_name);


    //        //保存设置            
    //        COM.tg.SaveCfg(VAR.gsys_set.cur_product_name);

    //        //重载
    //        ret += COM.tg.LoadDat(VAR.gsys_set.cur_product_name);

    //        if (ret == CONST.RES_OK) MessageBox.Show("数据保存完成!");
    //        else MessageBox.Show("数据保存失败!");
    //    }
    //    #endregion
    //    #region 保存扫描数据及参数
    //    public int SaveImg(string produtname)
    //    {
    //        //if (!File.Exists(Path.GetFullPath("..") + "\\product\\" + produtname + "\\temp.bmp")) return CONST.RES_PARA_ERR;
    //        //File.Copy(Path.GetFullPath("..") + "\\product\\" + produtname + "\\temp.bmp", Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\view.bmp", true);
    //        if (img != null) img.Save(Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\view.bmp");
    //        else return CONST.RES_PARA_ERR;

    //        string fileroad = Path.GetFullPath("..") + "\\product\\" + produtname + "\\temp.inf";
    //        IniFiles inf = new IniFiles(fileroad);
    //        inf.WriteDouble("SCAN", "START_CAP_X", st_ul_cap.x);
    //        inf.WriteDouble("SCAN", "START_CAP_Y", st_ul_cap.y);
    //        inf.WriteDouble("SCAN", "XS", xs);
    //        inf.WriteDouble("SCAN", "YS", ys);
    //        ofs_x = inf.ReadDouble("SCAN", "OFS_X", 0);
    //        ofs_y = inf.ReadDouble("SCAN", "OFS_Y", 0);

    //        try
    //        {
    //            Stream s = File.Open(Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\view.bmp", FileMode.Open);
    //            img = Image.FromStream(s);
    //            s.Close();
    //        }
    //        catch
    //        {
    //            img = new Bitmap(2000, 2000);
    //            Graphics gg = Graphics.FromImage(img);
    //            gg.FillRectangle(Brushes.DarkGray, new Rectangle(new Point(), img.Size));
    //            return CONST.RES_PARA_ERR;
    //        }

    //        return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 坐标转换
    //    public VAR.ST_XY PosPix2MM(int x, int y)
    //    {
    //        VAR.ST_XY st_pos = new VAR.ST_XY();
    //        st_pos.x = st_ul_cap.x + x * xs;
    //        st_pos.y = st_ul_cap.y + y * ys;
    //        return st_pos;
    //    }
    //    public VAR.ST_XY PosMM2Pix(double x, double y)
    //    {
    //        VAR.ST_XY st_pos = new VAR.ST_XY();
    //        st_pos.x = (x - st_ul_cap.x) / xs;
    //        st_pos.y = (y - st_ul_cap.y) / ys;
    //        return st_pos;
    //    }
    //    public PointF MM2Pix(VAR.ST_XYZ st_pos_mm)
    //    {
    //        PointF st_pos = new PointF();
    //        st_pos.X = (float)((st_pos_mm.x - st_ul_cap.x) / xs);
    //        st_pos.Y = (float)((st_pos_mm.y - st_ul_cap.y) / ys);
    //        return st_pos;
    //    }
    //    public VAR.ST_XY PosMM2Pix(VAR.ST_XYZ st_pos_mm)
    //    {
    //        VAR.ST_XY st_pos = new VAR.ST_XY();
    //        st_pos.x = (st_pos_mm.x - st_ul_cap.x) / xs;
    //        st_pos.y = (st_pos_mm.y - st_ul_cap.y) / ys;
    //        return st_pos;
    //    }

    //    public Rectangle Pos2Rec(VAR.ST_XYZ st_pos_mm, int w = 30, int h = 30)
    //    {
    //        VAR.ST_XY st_pos = new VAR.ST_XY();
    //        st_pos.x = (st_pos_mm.x - st_ul_cap.x) / xs;
    //        st_pos.y = (st_pos_mm.y - st_ul_cap.y) / ys;
    //        Rectangle rect = new Rectangle((int)(st_pos.x - w / 2), (int)(st_pos.y - h / 2), w, h);
    //        return rect;
    //    }
    //    #endregion
    //    #region 扫描
    //    public int Scan(ref int percent, VAR.ST_XYA st_ul, VAR.ST_XYA st_br, int pic_w, int pic_h)
    //    {
    //        //计算行列
    //        int xn = 0, yn = 0;
    //        double dx = 0, dy = 0;
    //        int pic_dx = 0, pic_dy = 0;
    //        int img_w = 2482;
    //        int img_h = 2102;
    //        VAR.ST_XYZA st_stransform;
    //        CogTransform2DLinear stransform;
    //        stransform = (CogTransform2DLinear)VisionTasks.Calibs[0].mCheckerboardTool.Calibration.OwnedWarpParams.GetOutputImageRootFromCalibratedTransform();
    //        st_stransform.x = 1 / stransform.ScalingX;
    //        st_stransform.y = 1 / stransform.ScalingY;
    //        stransform = (CogTransform2DLinear)VisionTasks.Calibs[1].mCamNPointTool.Calibration.GetComputedUncalibratedFromCalibratedTransform();
    //        st_stransform.x = st_stransform.x / stransform.ScalingX;
    //        st_stransform.y = st_stransform.y / stransform.ScalingY;

    //        if (Math.Abs(stransform.ScalingX) < 0.001 || Math.Abs(stransform.ScalingX) < 0.001)
    //        {
    //            MessageBox.Show("相机像素比例异常，请先校准后再扫描!");
    //            return CONST.RES_PARA_ERR;
    //        }

    //        Visionimage.CaptrueAndWaitResult(CameraNumber.Camera1, TaskProcess.CameraScan);
    //        img_h = VisionRun.ListVisionData[0].OutPutImage.Height;
    //        img_w = VisionRun.ListVisionData[0].OutPutImage.Width;

    //        dx = img_w * st_stransform.x * -1;
    //        dy = img_h * st_stransform.y * -1;

    //        xn = (int)(Math.Abs(st_ul.x - st_br.x) / Math.Abs(dx) + 0.999) + 1;
    //        yn = (int)(Math.Abs(st_ul.y - st_br.y) / Math.Abs(dy) + 0.999) + 1;


    //        pic_dx = pic_w / xn;
    //        pic_dy = pic_h / yn;

    //        //保存比列
    //        if (((double)pic_dx / (double)pic_dy) > ((double)img_w / (double)img_h)) pic_dx = (int)(pic_dy * ((double)img_w / (double)img_h));
    //        else pic_dy = (int)((double)pic_dx * ((double)img_h / (double)img_w));

    //        img = new Bitmap(pic_w, pic_h);
    //        Graphics gg = Graphics.FromImage(img);
    //        gg.FillRectangle(Brushes.DarkGray, new Rectangle(new Point(), img.Size));

    //        percent = 0;

    //        xs = dx / (double)pic_dx;
    //        ys = dy / (double)pic_dy;
    //        st_ul_cap.x = st_ul.x - dx / 2;
    //        st_ul_cap.y = st_ul.y - dy / 2;
    //        bsaveing = false;
    //        bscaning = true;
    //        for (int n = 0; n < xn; n++)
    //        {
    //            for (int m = 0; m < yn; m++)
    //            {
    //                //防护
    //                if (CONST.RES_OK != MT.ChkSafeSen()) goto ERR_END;

    //                //按键取消
    //                if (MT.GPIO_IN_KEY_STOP.isON) goto ERR_END;

    //                //检查是否超范围
    //                double x = st_ul.x + n * dx;
    //                double y = st_ul.y + m * dy;
    //                float kx = 0;
    //                float ky = 0;
    //                if (x < MT.AXIS_X.sln)
    //                {
    //                    kx = (float)(Math.Abs((MT.AXIS_X.sln - x) / dx));
    //                    x = MT.AXIS_X.sln;
    //                }
    //                if (x > MT.AXIS_X.slp)
    //                {
    //                    kx = (float)(Math.Abs((MT.AXIS_X.slp - x) / dx));
    //                    x = MT.AXIS_X.slp;
    //                }
    //                if (y < MT.AXIS_Y.sln)
    //                {
    //                    ky = (float)(Math.Abs((MT.AXIS_Y.sln - y) / dy));
    //                    y = MT.AXIS_Y.sln;
    //                }
    //                if (y > MT.AXIS_Y.slp)
    //                {
    //                    ky = (float)(Math.Abs((MT.AXIS_Y.slp - y) / dy));
    //                    y = MT.AXIS_Y.slp;
    //                }
    //                //move
    //                int ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref MT.AXIS_X, x, ref MT.AXIS_Y, y);
    //                if (ret != CONST.RES_OK)
    //                    goto ERR_END;

    //                //wait inp
    //                MT.AXIS_X.WaitINP(ref VAR.gsys_set.bquit);
    //                MT.AXIS_Y.WaitINP(ref VAR.gsys_set.bquit);

    //                //triger
    //                //int mode_temp = VAR.gsys_set.mode;
    //                //VAR.gsys_set.mode = CONST.SYS_RUN_MODE_DEMO;
    //                Visionimage.CaptrueAndWaitResult(CameraNumber.Camera1, TaskProcess.CameraScan);
    //                //VAR.gsys_set.mode = mode_temp;

    //                //get img
    //                Image img1 = VisionRun.ListVisionData[0].OutPutImage.ScaleImage(pic_dx, pic_dy).ToBitmap();
    //                //draw tu buf
    //                gg.DrawImage(img1, n * pic_dx - kx * pic_dx, m * pic_dy - ky * pic_dy, pic_dx, pic_dy);

    //                Thread.Sleep(1);
    //                Application.DoEvents();

    //                //进度
    //                percent = (int)((n * yn + m + 1) * 100.0 / (xn * yn));
    //            }
    //        }
    //        if (gg != null)
    //        {
    //            gg.Dispose();
    //            gg = null;
    //        }
    //        percent = 100;
    //        Thread.Sleep(100);
    //        Application.DoEvents();
    //        bscaning = false;

    //        //save posInf
    //        string fileroad = Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\temp.inf";
    //        IniFiles inf = new IniFiles(fileroad);

    //        this.st_ul = st_ul;
    //        this.st_br = st_br;
    //        inf.WriteDouble("SCAN", "UL_X", st_ul.x);
    //        inf.WriteDouble("SCAN", "UL_Y", st_ul.y);
    //        inf.WriteDouble("SCAN", "UL_A", st_ul.a);
    //        inf.WriteDouble("SCAN", "DR_X", st_br.x);
    //        inf.WriteDouble("SCAN", "DR_Y", st_br.y);
    //        inf.WriteDouble("SCAN", "DR_A", st_br.a);
    //        inf.WriteDouble("SCAN", "SC_X", xs);
    //        inf.WriteDouble("SCAN", "SC_Y", ys);
    //        //save img
    //        //bsaveing = true;
    //        //img.Save(Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\temp.bmp");            
    //        //bsaveing = false;
    //        bscaning = false;
    //        return CONST.RES_OK;

    //        ERR_END:
    //        if (gg != null)
    //        {
    //            gg.Dispose();
    //            gg = null;
    //        }
    //        bscaning = false;
    //        return CONST.RES_ERR;
    //    }
    //    #endregion
    //    #region 阵列
    //    public int Aarry(int xn, int yn, int num_area, int cap_mode, string partname = "")
    //    {

    //        VAR.ST_XYZ[] st_pos = new VAR.ST_XYZ[1];
    //        int ret = COM.tg.ArrayPos2(list_tg_array[0].st_pos_cap_set, list_tg_array[1].st_pos_cap_set, list_tg_array[2].st_pos_cap_set, xn, yn, ref st_pos);
    //        if (ret != CONST.RES_OK) return ret;

    //        //copy to list            
    //        for (int n = 0; n < st_pos.Length; n++)
    //        {
    //            TG_DATA tg = new TG_DATA();
    //            tg.partname = partname;
    //            tg.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode > -1 ? cap_mode : 0);
    //            tg.area_num = num_area;
    //            tg.st_pos_cap_set = st_pos[n];
    //            tg.name = list_tg_array[0].name;

    //            if (tg.bBM)
    //            {
    //                tg.st_pos_bm_set.x = tg.st_pos_cap_set.x + (list_tg_array[0].st_pos_bm_set.x - list_tg_array[0].st_pos_cap_set.x);
    //                tg.st_pos_bm_set.y = tg.st_pos_cap_set.y + (list_tg_array[0].st_pos_bm_set.y - list_tg_array[0].st_pos_cap_set.y);
    //            }
    //            else tg.st_pos_bm_set = new VAR.ST_XYZ();

    //            for (int m = 0; m < 4; m++)
    //            {
    //                if (tg.name == array_list_name[m])
    //                {
    //                    tg.id = array_list_tg[m].Count;
    //                    array_list_tg[m].Add(tg);
    //                }
    //            }
    //        }

    //        return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 准备阵列
    //    public int PrepareForArray(double mx, double my, int num_area, int cap_mode, bool bBM = false, string partname = "")
    //    {
    //        int ret = CONST.RES_ERR;
    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                list_tg_array.Clear();
    //                TG_DATA tg = new TG_DATA();
    //                tg.name = array_list_name[n];
    //                tg.partname = partname;
    //                tg.area_num = num_area;
    //                tg.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode > -1 ? cap_mode : 0);

    //                //第1点
    //                tg.id = 0;
    //                tg.st_pos_cap_set.x = mx;
    //                tg.st_pos_cap_set.y = my;
    //                if (bBM)
    //                {
    //                    tg.bBM = true;
    //                    tg.st_pos_bm_set.x = tg.st_pos_cap_set.x + 10 * (xs > 0 ? 1 : -1); ;
    //                    tg.st_pos_bm_set.y = tg.st_pos_cap_set.y + 10 * (ys > 0 ? 1 : -1); ;
    //                }
    //                list_tg_array.Add(tg);

    //                //第2点
    //                tg = new TG_DATA();
    //                tg.name = array_list_name[n];
    //                tg.partname = partname;
    //                tg.area_num = num_area;
    //                tg.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode > -1 ? cap_mode : 0);
    //                tg.id = 1;
    //                tg.bBM = false;
    //                tg.st_pos_cap_set.x = mx + 30 * (xs > 0 ? 1 : -1); ;
    //                tg.st_pos_cap_set.y = my;
    //                list_tg_array.Add(tg);

    //                //第3点
    //                tg = new TG_DATA();
    //                tg.name = array_list_name[n];
    //                tg.partname = partname;
    //                tg.area_num = num_area;
    //                tg.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode > -1 ? cap_mode : 0);
    //                tg.id = 2;
    //                tg.bBM = false;
    //                tg.st_pos_cap_set.x = mx;
    //                tg.st_pos_cap_set.y = my + 30 * (ys > 0 ? 1 : -1);
    //                list_tg_array.Add(tg);

    //                //第2/3点用于阵列
    //                bReadyToArray = true;
    //                ret = CONST.RES_OK;
    //                break;
    //            }
    //        }

    //        return ret;
    //    }
    //    #endregion
    //    #region 双击定位
    //    public int DoubleClik(int pix_x, int pix_y)
    //    {
    //        int ret = CONST.RES_OK;
    //        x = st_ul_cap.x + pix_x * xs;
    //        y = st_ul_cap.y + pix_y * ys;

    //        int cap_mode = -1;
    //        for (int n = 0; n < 5; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (Pos2Rec(array_list_tg[n][m].st_pos_cap_set).Contains(new Point(pix_x, pix_y)))
    //                    {
    //                        x = array_list_tg[n][m].st_pos_cap_set.x;
    //                        y = array_list_tg[n][m].st_pos_cap_set.y;
    //                        if (CONST.RES_OK != COM.tg.GetCapMode(array_list_tg[n][m].name, ref cap_mode)) cap_mode = -1;
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        //offset
    //        string fileroad = Path.GetFullPath("..") + "\\product\\" + VAR.gsys_set.cur_product_name + "\\temp.inf";
    //        IniFiles inf = new IniFiles(fileroad);
    //        ofs_x = inf.ReadDouble("SCAN", "OFS_X", 0);
    //        ofs_y = inf.ReadDouble("SCAN", "OFS_Y", 0);

    //        if (VAR.gsys_set.status == CONST.SYS_STATUS_STANDBY || VAR.gsys_set.status == CONST.SYS_STATUS_PAUSE)
    //        {
    //            if (DialogResult.Yes == MessageBox.Show(string.Format("是否定位到以下坐标？ \r\nX:{0:F3} + {1:F3} \r\nY:{2:F3} + {3:F3}", x, ofs_x, y, ofs_y), "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
    //            {
    //                ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref MT.AXIS_X, x + ofs_x, ref MT.AXIS_Y, y + ofs_y);
    //                Thread.Sleep(300);
    //                return ret;
    //            }
    //        }

    //        return CONST.RES_ABORT;
    //    }
    //    #endregion
    //    #region 修改编号
    //    public bool SetTGNum(ref int num, int pix_x, int pix_y, string num_type = "EC")
    //    {
    //        for (int n = 0; n < 4; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (Pos2Rec(array_list_tg[n][m].st_pos_cap_set).Contains(new Point(pix_x, pix_y)))
    //                    {
    //                        TG_DATA tg = array_list_tg[n][m];
    //                        if (!list_bc_num.Contains(tg))
    //                        {
    //                            if (num >= 0)
    //                            {
    //                                switch (num_type)
    //                                {
    //                                    default:
    //                                    case "EC":
    //                                        tg.ec_num = num;
    //                                        break;
    //                                    case "TF":
    //                                        tg.tf_seq = num;
    //                                        break;
    //                                }

    //                                array_list_tg[n][m] = tg;
    //                                list_bc_num.Add(tg);
    //                                num++;
    //                            }
    //                        }
    //                        return true;
    //                    }
    //                }
    //            }
    //        }

    //        return false;
    //    }
    //    #endregion
    //    #region 选择
    //    public bool TGSel(bool bBM, bool barray, ref TG_DATA cur_tg, int pix_x, int pix_y, bool bset = false, bool bsel = false, bool brev = false)
    //    {
    //        bool bget_sel = false;
    //        bool btemp_sel = false;
    //        TG_DATA tg = new TG_DATA();

    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    btemp_sel = Pos2Rec(array_list_tg[n][m].st_pos_cap_set).Contains(new Point(pix_x, pix_y));
    //                    if (bBM && array_list_tg[n][m].bBM)
    //                    {
    //                        tg = array_list_tg[n][m];
    //                        tg.bBMCap = Pos2Rec(array_list_tg[n][m].st_pos_bm_set).Contains(new Point(pix_x, pix_y));
    //                        array_list_tg[n][m] = tg;
    //                    }
    //                    if (bset || btemp_sel)
    //                    {
    //                        tg = array_list_tg[n][m];
    //                        tg.bselected = btemp_sel || brev ? !tg.bselected : bsel;
    //                        array_list_tg[n][m] = tg;
    //                        bget_sel = bget_sel || btemp_sel;
    //                    }
    //                    if (btemp_sel)
    //                    {
    //                        cur_tg = array_list_tg[n][m];
    //                        break;
    //                    }
    //                }
    //            }
    //            if (btemp_sel) break;
    //        }

    //        if (barray)
    //        {
    //            for (int m = 0; m < list_tg_array.Count; m++)
    //            {
    //                for (int n = 0; n < 6; n++)
    //                {
    //                    if (array_chk[n] && array_list_name[n] == list_tg_array[0].name)
    //                    {
    //                        btemp_sel = Pos2Rec(list_tg_array[m].st_pos_cap_set).Contains(new Point(pix_x, pix_y));
    //                        if (bBM && list_tg_array[m].bBM)
    //                        {
    //                            tg = list_tg_array[m];
    //                            tg.bBMCap = Pos2Rec(list_tg_array[m].st_pos_bm_set).Contains(new Point(pix_x, pix_y));
    //                            list_tg_array[m] = tg;
    //                        }
    //                        if (bset || btemp_sel)
    //                        {
    //                            tg = list_tg_array[m];
    //                            tg.bselected = btemp_sel || brev ? !tg.bselected : bsel;
    //                            list_tg_array[m] = tg;
    //                            bget_sel = bget_sel || btemp_sel;
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        return bget_sel;
    //    }
    //    #endregion
    //    #region 检查选中状态
    //    public bool TGSelChk(ref bool bsel, int pix_x, int pix_y, bool barray, bool bBM)
    //    {
    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (Pos2Rec(array_list_tg[n][m].st_pos_cap_set).Contains(new Point(pix_x, pix_y)))
    //                    {
    //                        bsel = array_list_tg[n][m].bselected;
    //                        return true;
    //                    }
    //                    if (bBM && array_list_tg[n][m].bBM)
    //                    {
    //                        if (Pos2Rec(array_list_tg[n][m].st_pos_bm_set).Contains(new Point(pix_x, pix_y)))
    //                        {
    //                            bsel = array_list_tg[n][m].bBMCap;
    //                            return true;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        if (barray)
    //        {
    //            for (int m = 0; m < list_tg_array.Count; m++)
    //            {
    //                for (int n = 0; n < 6; n++)
    //                {
    //                    if (Pos2Rec(list_tg_array[m].st_pos_cap_set).Contains(new Point(pix_x, pix_y)))
    //                    {
    //                        bsel = list_tg_array[m].bselected;
    //                        return true;
    //                    }
    //                    if (bBM && list_tg_array[m].bBM)
    //                    {
    //                        if (Pos2Rec(list_tg_array[m].st_pos_bm_set).Contains(new Point(pix_x, pix_y)))
    //                        {
    //                            bsel = list_tg_array[m].bBMCap;
    //                            return true;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        return false;
    //    }
    //    #endregion
    //    #region 拖放
    //    public void TGDrag(double dx, double dy, bool barray, bool bpaste, bool bBM = false)
    //    {
    //        TG_DATA tg = new TG_DATA();

    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (array_list_tg[n][m].bselected)
    //                    {
    //                        tg = array_list_tg[n][m];
    //                        tg.st_pos_cap_set.x += dx;
    //                        tg.st_pos_cap_set.y += dy;
    //                        if (bBM && tg.bBM)
    //                        {
    //                            tg.st_pos_bm_set.x += dx;
    //                            tg.st_pos_bm_set.y += dy;
    //                        }
    //                        array_list_tg[n][m] = tg;
    //                    }
    //                    else if (array_list_tg[n][m].bBMCap)
    //                    {
    //                        if (bBM && array_list_tg[n][m].bBM)
    //                        {
    //                            tg = array_list_tg[n][m];
    //                            tg.st_pos_bm_set.x += dx;
    //                            tg.st_pos_bm_set.y += dy;
    //                            array_list_tg[n][m] = tg;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        if (bpaste)
    //        {
    //            for (int n = 0; n < list_tg_copy.Count; n++)
    //            {
    //                if (list_tg_copy[n].bselected)
    //                {
    //                    tg = list_tg_copy[n];
    //                    tg.st_pos_cap_set.x += dx;
    //                    tg.st_pos_cap_set.y += dy;
    //                    if (bBM && tg.bBM)
    //                    {
    //                        tg.st_pos_bm_set.x += dx;
    //                        tg.st_pos_bm_set.y += dy;
    //                    }
    //                    list_tg_copy[n] = tg;
    //                }
    //                else if (list_tg_copy[n].bBMCap)
    //                {
    //                    if (bBM && list_tg_copy[n].bBM)
    //                    {
    //                        tg = list_tg_copy[n];
    //                        tg.st_pos_bm_set.x += dx;
    //                        tg.st_pos_bm_set.y += dy;
    //                        list_tg_copy[n] = tg;
    //                    }
    //                }
    //            }
    //        }

    //        if (barray)
    //        {
    //            for (int n = 0; n < list_tg_array.Count; n++)
    //            {
    //                if (list_tg_array[n].bselected)
    //                {
    //                    tg = list_tg_array[n];
    //                    tg.st_pos_cap_set.x += dx;
    //                    tg.st_pos_cap_set.y += dy;
    //                    if (bBM && tg.bBM)
    //                    {
    //                        tg.st_pos_bm_set.x += dx;
    //                        tg.st_pos_bm_set.y += dy;
    //                    }
    //                    list_tg_array[n] = tg;
    //                }
    //                else if (list_tg_array[n].bBMCap)
    //                {
    //                    if (bBM && list_tg_array[n].bBM)
    //                    {
    //                        tg = list_tg_array[n];
    //                        tg.st_pos_bm_set.x += dx;
    //                        tg.st_pos_bm_set.y += dy;
    //                        list_tg_array[n] = tg;
    //                    }
    //                }
    //            }
    //        }

    //    }
    //    #endregion
    //    #region 复制
    //    public bool Copy()
    //    {
    //        list_tg_copy.Clear();
    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                foreach (TG_DATA tg in array_list_tg[n])
    //                {
    //                    if (tg.bselected) list_tg_copy.Add(tg.clone());
    //                }
    //            }
    //        }

    //        if (list_tg_copy.Count == 0) return false;
    //        return true;
    //    }
    //    #endregion
    //    #region 粘贴
    //    public void Paste()
    //    {
    //        bool bcopy = false;

    //        //对应复制
    //        foreach (TG_DATA tg in list_tg_copy)
    //        {
    //            for (int n = 0; n < 6; n++)
    //            {
    //                if (array_chk[n] && array_list_tg[n].Count > 0 && tg.name == array_list_tg[n][0].name)
    //                {
    //                    TG_DATA tg_temp = array_list_tg[n][0].clone();
    //                    tg_temp.id = array_list_tg[n].Count;
    //                    tg_temp.bBM = tg.bBM;
    //                    tg_temp.area_num = tg.area_num;
    //                    tg_temp.st_pos_cap_set = tg.st_pos_cap_set;
    //                    tg_temp.st_pos_bm_set = tg.st_pos_bm_set;
    //                    array_list_tg[n].Add(tg_temp);
    //                    bcopy = true;
    //                }
    //            }
    //        }

    //        //没有对应复制，则复制到一个选择类
    //        if (!bcopy)
    //        {
    //            for (int n = 0; n < 6; n++)
    //            {
    //                if (array_chk[n])
    //                {
    //                    foreach (TG_DATA tg in list_tg_copy)
    //                    {
    //                        TG_DATA tg_temp = tg.clone();
    //                        if (array_list_tg[n].Count > 0)
    //                            tg_temp = array_list_tg[n][0].clone();
    //                        else
    //                            tg_temp.name = array_list_name[n];
    //                        tg_temp.id = array_list_tg[n].Count;
    //                        tg_temp.bBM = tg.bBM;
    //                        tg_temp.area_num = tg.area_num;
    //                        tg_temp.st_pos_cap_set = tg.st_pos_cap_set;
    //                        tg_temp.st_pos_bm_set = tg.st_pos_bm_set;
    //                        array_list_tg[n].Add(tg_temp);
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //    #region 清除
    //    public void Clear()
    //    {
    //        List<TG_DATA> list_del = new List<TG_DATA>();

    //        //TA
    //        foreach (TG_DATA tg in list_tg_a)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_a.Remove(tg);

    //        //TB
    //        foreach (TG_DATA tg in list_tg_b)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_b.Remove(tg);

    //        //TC
    //        foreach (TG_DATA tg in list_tg_c)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_c.Remove(tg);

    //        //TD
    //        foreach (TG_DATA tg in list_tg_d)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_d.Remove(tg);

    //        //MK
    //        foreach (TG_DATA tg in list_tg_mk)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_mk.Remove(tg);

    //        //BC
    //        foreach (TG_DATA tg in list_tg_bc)
    //        {
    //            if (tg.bselected) list_del.Add(tg);
    //        }
    //        foreach (TG_DATA tg in list_del) list_tg_bc.Remove(tg);
    //    }
    //    #endregion
    //    #region 框选
    //    public void RectSel(double start_mx, double start_my, double end_mx, double end_my)
    //    {
    //        //框选
    //        Rectangle rect = new Rectangle((int)start_mx, (int)start_my, (int)(end_mx - start_mx), (int)(end_my - start_my));
    //        if (rect.Width < 0)
    //        {
    //            rect.Width = -rect.Width;
    //            rect.X -= rect.Width;
    //        }
    //        if (rect.Height < 0)
    //        {
    //            rect.Height = -rect.Height;
    //            rect.Y -= rect.Height;
    //        }
    //        for (int n = 0; n < 6; n++)
    //        {
    //            TG_DATA tg = new TG_DATA();
    //            if (array_chk[n])
    //            {
    //                //draw.SlectByRect(array_list_tg[n], new Point(md_pix_x, md_pix_y), new Point(pix_x, pix_y));
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    tg = array_list_tg[n][m];
    //                    if (rect.Contains((int)tg.st_pos_cap_set.x, (int)tg.st_pos_cap_set.y))
    //                    {
    //                        tg.bselected = true;
    //                        array_list_tg[n][m] = tg;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //    #region 增加
    //    public void AddTG(double mx, double my, int area_num, int cap_mode, bool bBM = false, string partname = "")
    //    {
    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                TG_DATA st_tg = new TG_DATA();
    //                st_tg.st_pos_cap_set.x = mx;
    //                st_tg.st_pos_cap_set.y = my;
    //                st_tg.name = array_list_name[n];
    //                st_tg.partname = partname;
    //                st_tg.id = array_list_tg[n].Count;
    //                st_tg.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode);
    //                st_tg.area_num = area_num;

    //                if (array_list_tg[n].Count > 0)
    //                {
    //                    st_tg.st_offset = array_list_tg[n][0].st_offset;
    //                    st_tg.st_layer_offset = array_list_tg[n][0].st_layer_offset;
    //                }
    //                else
    //                {
    //                    st_tg.st_offset = new VAR.ST_XYZA();
    //                    st_tg.st_layer_offset = new VAR.ST_XYZA();
    //                }

    //                if (st_tg.name == "MK" || st_tg.name == "BC") st_tg.cap_mode = TG_DATA.EM_CAP_MODE.CAP;

    //                if ((st_tg.name != "MK" && st_tg.name != "BC") && bBM)
    //                {
    //                    st_tg.bBM = true;
    //                    if (array_list_tg[n].Count > 0 && array_list_tg[n][0].bBM)
    //                    {
    //                        st_tg.st_pos_bm_set.x = array_list_tg[n][0].st_pos_bm_set.x - array_list_tg[n][0].st_pos_cap_set.x + mx;
    //                        st_tg.st_pos_bm_set.y = array_list_tg[n][0].st_pos_bm_set.y - array_list_tg[n][0].st_pos_cap_set.y + my;
    //                    }
    //                    else
    //                    {
    //                        st_tg.st_pos_bm_set.x = 5 + mx;
    //                        st_tg.st_pos_bm_set.y = 5 + my;
    //                    }
    //                }

    //                array_list_tg[n].Add(st_tg);
    //                break;
    //            }
    //        }
    //    }
    //    #endregion
    //    #region 校验
    //    public int CapCali(ref List<TG_DATA> list_tg)
    //    {
    //        int ret = CONST.RES_OK;
    //        if (list_tg == null) return CONST.RES_PARA_ERR;
    //        if (list_tg.Count == 0) return CONST.RES_OK;

    //        //move R
    //        ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref MT.AXIS_R, st_ul.a);
    //        if (ret != CONST.RES_OK)
    //        {
    //            //Move to Rdy posInf first
    //            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref MT.AXIS_X, FDH.st_pos_ready.x, ref MT.AXIS_Y, FDH.st_pos_ready.y);
    //            if (ret != CONST.RES_OK) return ret;
    //            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref MT.AXIS_R, st_ul.a);
    //            if (ret != CONST.RES_OK) return ret;
    //        }

    //        TG_DATA tg = list_tg[0];
    //        for (int n = 0; n < list_tg.Count; n++)
    //        {
    //            tg = list_tg[n];
    //            tg.st_pos_cap_cur.x = tg.st_pos_cap_set.x + ofs_x;
    //            tg.st_pos_cap_cur.y = tg.st_pos_cap_set.y + ofs_y;
    //            if (tg.name == "MK") tg.st_pos_mk_cur = tg.st_pos_cap_cur;
    //            //cap
    //            ret = COM.tg.TgCap(ref tg, true, true, true);
    //            if (tg.status == -1) tg.bmtf = false;
    //            else tg.bmtf = true;
    //            if (ret == CONST.RES_OK)
    //            {
    //                tg.st_pos_cap_set = tg.st_pos_cap_cur;
    //                tg.st_pos_cap_set.x -= ofs_x;
    //                tg.st_pos_cap_set.y -= ofs_y;
    //                tg.st_pos_cap_set.z = tg.st_vs_cur.z;
    //                list_tg[n] = tg;
    //            }
    //            else break;

    //            //bm
    //            if (tg.bBM)
    //            {
    //                ret = COM.tg.TgCap(ref tg, false, true, true, tg.bBM);
    //                if (ret == CONST.RES_OK)
    //                {
    //                    tg.st_pos_bm_set = tg.st_pos_bm_cur;
    //                    list_tg[n] = tg;
    //                }
    //                else break;
    //            }
    //        }
    //        if (ret != CONST.RES_OK)
    //        {
    //            MessageBox.Show(string.Format("目标{0}，编号{1} 校正失败!", tg.name, tg.id));
    //        }
    //        return ret;
    //    }

    //    public int Cali()
    //    {
    //        int ret;
    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                ret = CapCali(ref array_list_tg[n]);
    //                if (ret == CONST.RES_OK)
    //                {
    //                    if (array_list_name[n] == "MK")
    //                    {
    //                        COM.tg.list_cur_mark.Clear();
    //                        foreach (TG_DATA tg in array_list_tg[n])
    //                            COM.tg.list_cur_mark.Add(tg.st_pos_cap_cur);
    //                        COM.tg.SaveCfg(VAR.gsys_set.cur_product_name);
    //                    }
    //                }
    //                else return ret;
    //            }
    //        }

    //        return CONST.RES_OK;
    //    }
    //    #endregion
    //    #region 修改拍照模式
    //    public void ModifyCapMode(int cap_mode)
    //    {
    //        bool bedit_all = false;

    //        if (cap_mode < 0) return;

    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (array_list_tg[n][m].bselected)
    //                    {
    //                        if ((int)array_list_tg[n][m].cap_mode != cap_mode)
    //                        {
    //                            if (bedit_all || DialogResult.Yes == MessageBox.Show(string.Format("是否修改整个类别？"), "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
    //                            {
    //                                bedit_all = true;
    //                                for (int mm = 0; mm < array_list_tg[n].Count; mm++)
    //                                {
    //                                    TG_DATA tg_temp = array_list_tg[n][mm];
    //                                    tg_temp.cap_mode = (TG_DATA.EM_CAP_MODE)(cap_mode);
    //                                    array_list_tg[n][mm] = tg_temp;
    //                                }
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //    #region 修改区号
    //    public void ModifyAreaNum(int area_num)
    //    {
    //        bool bedit_all = false;

    //        if (area_num < 0) return;

    //        for (int n = 0; n < 6; n++)
    //        {
    //            if (array_chk[n])
    //            {
    //                for (int m = 0; m < array_list_tg[n].Count; m++)
    //                {
    //                    if (array_list_tg[n][m].bselected)
    //                    {
    //                        if ((int)array_list_tg[n][m].area_num != area_num)
    //                        {
    //                            if (bedit_all || DialogResult.Yes == MessageBox.Show(string.Format("是否修改整个类别？"), "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
    //                            {
    //                                bedit_all = true;
    //                                for (int mm = 0; mm < array_list_tg[n].Count; mm++)
    //                                {
    //                                    TG_DATA tg_temp = array_list_tg[n][mm];
    //                                    tg_temp.area_num = area_num;
    //                                    array_list_tg[n][mm] = tg_temp;
    //                                }
    //                                break;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //}

    #endregion
}
