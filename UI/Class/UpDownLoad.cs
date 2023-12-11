using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.Implementation.Internal;
using DevReport;
using Microsoft.SqlServer.Server;
using MotionCtrl;
using Newtonsoft.Json.Linq;
using Sunny.UI.Win32;
using UI.Class;
using Win32Lib;
using static UI.Cam;
using static UI.Product.Tray;

namespace UI
{
    public class UpDownLoad
    {
        #region 状态
        public enum EM_STA
        {
            [Description("未知")]
            UNKNOW,
            [Description("忙")]
            BUSY,
            [Description("等待")]
            WAIT,
            [Description("回零中")]
            HOME,
            [Description("就绪")]
            READY,
            [Description("GRR测试")]
            GRRTEST,
            [Description("取放检测")]
            CHECK,
            [Description("工站取料")]
            PICKWS,
            [Description("料盘放料")]
            PLACETRAY,
            [Description("飞拍失败放料")]
            FLYERRPLACE,
            [Description("料盘取料")]
            PICKTRAY,
            [Description("工站放料")]
            PLACEWS,
            [Description("上下料结束")]
            UPDOWNLOADEND,
            [Description("仓储进出料")]
            CALLTRAY,
            [Description("GRR拍照")]
            GRR_CAM,
            [Description("错误")]
            ERR,
            [Description("退出")]
            QUIT,
            [Description("马达拍照失败放料")]
            MOTORCAMERRPLACE
        }

        //寻找工站模组方式
        public enum EM_FIN_MOD
        {
            ALL,    //全工站搜索
            LOW,    //低8位工站搜索
            HIGH,    //高8位工站搜索
            NONE    //不搜索

        }

        //飞拍流程配置
        public enum EM_FLY_CFG
        {
            MOD_DW,   //下相机拍模组底部
            MOD_UP,   //上相机拍模组表面
            WS_UP,    //上相机拍工站表面
            WS_MOD_UP //上相机拍工站上物料
        }
        #endregion

        #region 委托CheckBox
        public delegate void ReadChkHandler();
        public static void SetCalResult()
        {
            if (FrMain.frrun.ckb_wait_upload.InvokeRequired)
            {
                ReadChkHandler set = new ReadChkHandler(SetCalResult);//委托的方法参数应和SetCalResult一致
                FrMain.frrun.ckb_wait_upload.Invoke(set, new object[] { }); //此方法第二参数用于传入方法,代替形参result
            }
            else
            {
                bWaitforUpDownload = FrMain.frrun.ckb_wait_upload.Checked;
            }
        }


        #endregion

        #region  参数
        //上下料id
        public int id;
        //上下料描述符
        public string disc;
        //上下料英文描述
        public string englishdisc;
        //第一次开机标志
        public bool IsFirst = true;
        //第一次上料需回检
        //public bool isfirst = true;
        //错误信息
        public string ErrMsg = string.Empty;
        //当前放料最后工站拍照坐标
        public ST_XY FeedBackPos = new ST_XY();
        //当前放料工站
        public List<WS.MdDat> FeedBackWs = new List<WS.MdDat>();
        //当前放料工站位置记录
        public bool bUpdateFBPos_OKNG = false;
        //当前放料工站位置记录
        public bool bUpdateFBPos_UT = false;
        //下一次工站拍照位置
        public List<WS.MdDat> CurPlaceMod = new List<WS.MdDat>();
        //上料等待
        public static bool bWaitforUpDownload = false;
        //X1与X2的行程
        public static double DisAxisX = 0;
        public int BackXtid;//飞拍失败放回的吸头ID

        public List<WS.MdDat> PickMod = new List<WS.MdDat>();
        //
        public double time_cnt = 0;

        public EM_STA status_ud = EM_STA.UNKNOW;

        public List<Product.Tray.PosInf> placepos = new List<Product.Tray.PosInf>();

        public EM_FIN_MOD FindMod = EM_FIN_MOD.ALL;

        public bool bfinal = false;

        public bool bflycam = false;

        public EM_STA Proc = EM_STA.CHECK;
        public EM_STA GrrProc = EM_STA.PICKWS;

        public int ErrTryPickCnt = 0;

        public int ErrMotorTryPickCnt = 0;

        public bool Demo = false;

        public double TrayGoPos = 0;

        public string barcodestr = "";//料盘二维码

        public static ST_XYA Vs_TrayOK = new ST_XYA(), Vs_TrayNg = new ST_XYA();
        //lock
        private static readonly object pickws = new object();
        private static readonly object placews = new object();
        private static readonly object picktray = new object();
        private static readonly object placetray = new object();
        List<Cam.VisionTask> modupcam_task = new List<Cam.VisionTask>();

        public bool bOnlyOneXt = false;
        #endregion

        #region 硬件
        /// <summary>
        ///上下料对应下相机
        /// </summary>
        public Cam dwcam = null;
        /// <summary>
        ///上下料对应上相机
        /// </summary>
        public Cam upcam = null;
        /// <summary>
        /// 上料X轴
        /// </summary>
        public AXIS ax_x = null;
        /// <summary>
        /// 上料Y轴
        /// </summary>
        public AXIS ax_y = null;
        /// <summary>
        /// 上料Z轴
        /// </summary>
        public AXIS ax_z = null;
        /// <summary>
        /// 上料U轴
        /// </summary>
        public AXIS ax_u1 = null;
        /// <summary>
        /// 上料U轴
        /// </summary>
        public AXIS ax_u2 = null;
        /// <summary>
        ///上下料对应的吸头
        /// </summary>
        public List<XT> list_xt = new List<XT>();
        /// <summary>
        ///上下料对应的待测仓储
        /// </summary>
        public TrayBox traybox_fd = COM.traybox_fd;
        /// <summary>
        ///上下料对应的OK仓储
        /// </summary>
        public TrayBox traybox_ok = COM.traybox_ok;
        /// <summary>
        ///上下料对应的NG仓储
        /// </summary>
        public TrayBox traybox_ng = COM.traybox_ng;
        #endregion

        #region 初始化
        public UpDownLoad()
        { }

        public UpDownLoad(int id, string disc, string englishdisc, ref Cam dwcam, ref Cam upcam, ref AXIS ax_x, ref AXIS ax_y,
                           ref AXIS ax_z, ref AXIS ax_u1, ref AXIS ax_u2, ref List<XT> list_xt)
        {
            this.id = id;
            this.disc = disc;
            this.upcam = upcam;
            this.dwcam = dwcam;
            this.ax_x = ax_x;
            this.ax_y = ax_y;
            this.ax_z = ax_z;
            this.ax_u1 = ax_u1;
            this.ax_u2 = ax_u2;
            this.list_xt = list_xt;
            this.englishdisc = englishdisc;
            foreach (XT xt in list_xt)
            {
                xt.Parent = this;
                if (this.ax_y.m_id != xt.ax_y.m_id)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}的Y轴与{1}Y轴不匹配!", this.disc, xt.disc) : string.Format("{0}'Y axis and {2} do not match           ({1}的Y轴与{2}Y轴不匹配!)", this.englishdisc, this.disc, xt.disc), DReport.EmErrCode.ToolError, (int)DReport.EmHareware.UpDownLoad + id);
                }
            }
        }
        
        public int XtCnt//吸头启用计数
        {
            get
            {
                int i = 0;
                foreach(var xt in list_xt)
                {
                    if (xt.Enable) i++;
                }
                return i;
            }
        }
        #endregion

        #region 坐标获取函数
        double x = 0, y = 0, z = 0, u1 = 0, u2 = 0;
        bool btsk = false;
        public ST_XYZA CurPos
        {
            get
            {
                ST_XYZA pos = new ST_XYZA();
                pos.x = ax_x != null ? ax_x.fenc_pos : 0;
                pos.y = ax_y != null ? ax_y.fenc_pos : 0;
                pos.z = ax_z != null ? ax_z.fenc_pos : 0;
                pos.a = ax_u1 != null ? ax_u1.fcmd_pos : 0;
                return pos;
            }
        }
        public string StrOfPos
        {
            get
            {
                Task tsk = new Task(() =>
                {
                    if (btsk) return;
                    btsk = true;
                    x = ax_x != null ? ax_x.fcmd_pos : 0;
                    y = ax_y != null ? ax_y.fcmd_pos : 0;
                    z = ax_z != null ? ax_z.fcmd_pos : 0;
                    u1 = ax_u1 != null ? ax_u1.fcmd_pos : 0;
                    u2 = ax_u2 != null ? ax_u2.fcmd_pos : 0;
                    btsk = false;
                });
                tsk.Start();
                if (id == 0) return string.Format("X: {0:000.000}\nY: {1:000.000}\nZ: {2:000.000}\nU1: {3:000.000}\nU2: {4:000.000}", x, y, z, u1, u2);
                else return string.Format("X: {0:000.000}  U1: {3:000.000}\nY: {1:000.000}  U2: {4:000.000}\nZ: {2:000.000}", x, y, z, u1, u2);
            }
        }
        #endregion

        #region 定位函数
        //Z抬起
        public EM_RES Zup(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;
            res = MT.Move(ref bquit, ref ax_z, 0);
            return res;
        }
        //快速回初始位置
        public EM_RES GoZero(ref bool bquit, bool IsZero = true)
        {
            EM_RES res = EM_RES.OK;

            res = MT.ZupMove(ref bquit, ref ax_x, /*id==0 ? PT_SET.safepos : -PT_SET.safepos*/0, ref ax_y, IsZero == true ? 0 : list_xt[1].st_cap_pos.y , ref ax_u1, 0, ref ax_u2, 0);
            if (res != EM_RES.OK) return res;
            return EM_RES.OK;
        }
        #endregion

        #region 回零
        public EM_RES Home(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;

            if (bquit) return EM_RES.QUIT;
            //吸头上没有物料清空
            foreach (XT xt in list_xt)
            {
                if (!xt.cy_zk.isONByChkSen)
                    xt.cy_zk.SetOff();
                xt.gpio_pzk.SetOff();
            }
            //Z轴复位
            res = MT.AxisHome(ref bquit, ax_z);
            if (res != EM_RES.OK) return res;

            //复位XYA  模块1同时把料仓复位
            if (id == 0)
            {
                res = MT.AxisHome(ref bquit, ax_x, ax_y, ax_u1, ax_u2, traybox_fd.ax_x, traybox_ok.ax_x, traybox_ng.ax_x);
                if (res != EM_RES.OK) return res;
                res = MT.AxisHome(ref bquit, traybox_fd.ax_z, traybox_ok.ax_z, traybox_ng.ax_z);
            }
            else
            {
                res = MT.AxisHome(ref bquit, ax_x, ax_y, ax_u1, ax_u2);
            }
            if (res != EM_RES.OK) return res;

            foreach (XT xt in list_xt)
            {
                if (xt.cy_zk.isONByChkSen && IsFirst)
                    xt.cy_zk.SetOff();
            }
            IsFirst = false;
            return EM_RES.OK;
        }


        #endregion

        #region  停止
        public void Stop()
        {
            ax_x.bhomequit = true;
            ax_x.Stop();

            ax_y.bhomequit = true;
            ax_y.Stop();

            ax_z.bhomequit = true;
            ax_z.Stop();

            ax_u1.bhomequit = true;
            ax_u1.Stop();

            ax_u2.bhomequit = true;
            ax_u2.Stop();

            traybox_fd.ax_x.bhomequit = true;
            traybox_fd.ax_x.Stop();

            traybox_fd.ax_z.bhomequit = true;
            traybox_fd.ax_z.Stop();

            traybox_ok.ax_x.bhomequit = true;
            traybox_ok.ax_x.Stop();

            traybox_ok.ax_z.bhomequit = true;
            traybox_ok.ax_z.Stop();

            traybox_ok.ax_x.bhomequit = true;
            traybox_ok.ax_x.Stop();

            traybox_ng.ax_x.bhomequit = true;
            traybox_ng.ax_x.Stop();
        }


        #endregion

        #region 相关函数
        /// <summary>
        /// 确认下一个流程
        /// </summary>
        /// <param name="MdNum"></param>
        /// <param name="ws"></param>
        /// <param name="ud_id"></param>
        /// <returns></returns>
        public EM_STA ChkNextProc(ref int MdNum, ref int MdNum_1, WS ws, int ud_id)
        {
            EM_STA proc_temp = EM_STA.CHECK;
            int numempty = 10000, numtestok = 10000;
            List<WS.MdDat> pos_empty = new List<WS.MdDat>();
            List<WS.MdDat> pos_testok = new List<WS.MdDat>();
            GetWsPosTeam(ref pos_empty, ws, Product.EM_CM_RES.EMPTY, COM.List_UDLoad[ud_id].FindMod);
            GetWsPosTeam(ref pos_testok, ws, Product.EM_CM_RES.OK, COM.List_UDLoad[ud_id].FindMod);

            if (pos_empty.Count > 0)
                numempty = (pos_empty[0].Num - 1) % 8;
            if (pos_testok.Count > 0)
                numtestok = (pos_testok[0].Num - 1) % 8;

            if (!VAR.ClearMt)
            {
                if (pos_testok.Count > 0 && pos_empty.Count > 0)
                {
                    MdNum = numtestok > numempty ? numempty : numtestok;
                    MdNum_1 = numtestok > numempty ? pos_empty[0].Num : pos_testok[0].Num;
                    proc_temp = numtestok > numempty ? EM_STA.PICKTRAY : EM_STA.PICKWS;
                }
                else if (pos_testok.Count > 0)
                {
                    MdNum = numtestok;
                    MdNum_1 = pos_testok[0].Num;
                    proc_temp = EM_STA.PICKWS;
                }
                else if (pos_empty.Count > 0)
                {
                    MdNum = numempty;
                    MdNum_1 = pos_empty[0].Num;
                    proc_temp = EM_STA.PICKTRAY;
                }
                else
                {
                    MdNum = 10000;
                    MdNum_1 = 10000;
                    proc_temp = EM_STA.UPDOWNLOADEND;
                }
            }
            else
            {
                if (pos_testok.Count > 0)
                {
                    MdNum = numtestok;
                    MdNum_1 = pos_testok[0].Num;
                    proc_temp = EM_STA.PICKWS;
                }
                else
                {
                    MdNum = 10000;
                    MdNum_1 = 10000;
                    proc_temp = EM_STA.UPDOWNLOADEND;
                    //isfirst = true;
                }
            }
            return proc_temp;
        }

        public EM_STA ChkGrrNextProc(WS ws, int ud_id)
        {
            EM_STA proc_temp = EM_STA.CHECK;
            List<WS.MdDat> pos_empty = new List<WS.MdDat>();
            List<WS.MdDat> pos_testok = new List<WS.MdDat>();
            GetWsOrderPosTeam(ref pos_empty, ws, Product.EM_CM_RES.EMPTY, COM.List_UDLoad[ud_id].FindMod);
            GetWsOrderPosTeam(ref pos_testok, ws, Product.EM_CM_RES.OK, COM.List_UDLoad[ud_id].FindMod);
            if (pos_empty.Count > 0 || pos_testok.Count > 0)
                proc_temp = EM_STA.GRRTEST;
            else
                proc_temp = EM_STA.UPDOWNLOADEND;
            return proc_temp;
        }
        /// <summary>
        /// 吸头上的物料放回待测料盘
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="xt"></param>
        /// <returns></returns>
        public EM_RES XtMovPickPos(ref bool bquit, XT xt)
        {
            EM_RES res = EM_RES.OK;
            double pos_tray_x = 0;
            double ax_x_pos = ax_x.fenc_pos;
            if (Math.Abs(ax_x.fenc_pos) < 50)
            {
                if (id == 0) ax_x_pos = 50;
                else if (id == 1) ax_x_pos = -50;
            }
            pos_tray_x = -xt.xt_pos_pick_mod.x + ax_x_pos + xt.xt_pos_pick_tray_x;
            if (pos_tray_x < 0)
            {
                xt.xt_pos_pick_mod.x = ax_x_pos - pos_tray_x;
                pos_tray_x = 0;
            }
            else
            {
                xt.xt_pos_pick_mod.x = ax_x_pos;
            }
            //move tray_x
            res = MT.ZupMove(ref bquit, ref ax_x, xt.xt_pos_pick_mod.x, ref ax_y, xt.xt_pos_pick_mod.y, ref traybox_fd.ax_x, pos_tray_x);
            return EM_RES.OK;
        }
        /// <summary>
        /// 根据顺序查找工站的模组（按列搜索）
        /// </summary>
        /// <param name="PlaceMod">返回结果</param>
        /// <param name="ws">工站</param>
        /// <param name="res">查找类型</param>
        /// <param name="mod">搜索方式</param>
        public void GetWsPosTeam(ref List<WS.MdDat> PlaceMod, WS ws, Product.EM_CM_RES res = Product.EM_CM_RES.EMPTY, EM_FIN_MOD mod = EM_FIN_MOD.ALL)
        {
            PlaceMod.Clear();
            if (mod == EM_FIN_MOD.NONE) return;
           
            int n = 0;
            for (int i = 0; i < ws.list_md.Count; i++)
            {
                if (i % 2 == 0) n = i - i / 2;
                else n = i + (ws.list_md.Count - 1 - i) / 2;

                if ((mod == EM_FIN_MOD.LOW && (n % 8 >= 4)) || (mod == EM_FIN_MOD.HIGH && (n % 8 < 4))) continue;
                if (n < ws.list_md.Count)
                {
                    
                    WS.MdDat WsMod = ws.list_md[n];
                    if (!WsMod.benable && res == Product.EM_CM_RES.EMPTY) continue;

                    //if (VAR.gsys_set.isChkMode && VAR.ChkPC != WsMod.PC_ID) continue;
                    if ((WsMod.res != (int)res && res < Product.EM_CM_RES.OK) || (WsMod.res < (int)res && res == Product.EM_CM_RES.OK)) continue;
                    if (PlaceMod.Count > 0 && PlaceMod[0].Num % 8 != WsMod.Num % 8) break;
                    PlaceMod.Add(WsMod);
                    bool bOnlySelectOne = id == 0 ? PT_SET.bUpDn1XtOnlyOne : PT_SET.bUpDn2XtOnlyOne;
                    if (bOnlySelectOne) break;
                     if (WsMod.Num > 8) break;
                }
            }
        }

        /// <summary>
        /// 根据顺序查找工站的模组(按个搜索)搜索一个
        /// </summary>
        /// <param name="PlaceMod">返回结果</param>
        /// <param name="ws">工站</param>
        /// <param name="res">查找类型</param>
        /// <param name="mod">搜索方式</param>
        public void GetWsOrderPosTeam(ref List<WS.MdDat> PlaceMod, WS ws, Product.EM_CM_RES res = Product.EM_CM_RES.EMPTY, EM_FIN_MOD mod = EM_FIN_MOD.ALL)
        {
            PlaceMod.Clear();
            if (mod == EM_FIN_MOD.NONE) return;
            int n = 0;
            for (int i = 0; i < ws.list_md.Count; i++)
            {
                n = i;
                if ((mod == EM_FIN_MOD.LOW && (n % 8 >= 4)) || (mod == EM_FIN_MOD.HIGH && (n % 8 < 4))) continue;
                if (n < ws.list_md.Count)
                {
                    WS.MdDat WsMod = ws.list_md[n];
                    if (!WsMod.benable) continue;
                    if (VAR.isAutoChkMode)
                    {
                        if (WsMod.bAutoChkOk && res == Product.EM_CM_RES.EMPTY)
                            continue;//点检已经ok的不上料
                        if (!VAR.ClearMt)//自动点检模式
                        {
                            if (WsMod.res == -1 && (int)res != -1)
                                continue;//对待测的忽略
                        }
                    }
                    // if (VAR.gsys_set.isChkMode && VAR.ChkPC != WsMod.PC_ID) continue;
                    if ((WsMod.res != (int)res && res < Product.EM_CM_RES.OK) || (WsMod.res < (int)res && res == Product.EM_CM_RES.OK)) continue;
                    PlaceMod.Add(WsMod);
                    break;
                }
            }
        }
        /// <summary>
        /// 飞拍配置
        /// </summary>
        /// <param name="flyCfg">模式</param>
        /// <param name="frPos">配置的点位</param>
        /// <param name="cnt">配置个数</param>
        /// <param name="Ofs_On">补偿</param>
        /// <param name="Dir">运动方向  true:Low->High    false:High->low</param>
        public void FlyCfg(EM_FLY_CFG flyCfg, List<ST_XYN> frPos, int cnt, bool Dir = true, bool Ofs_On = true, bool IfClear = true, List<ST_XYN> capQrcodePos = null)
        {
            if (cnt <= 0) return;
            List<double> TriPos = new List<double>();
            List<string> ListTaskName = new List<string>();
            List<double> xt_ofs = new List<double>();
            List<double> ws_ofs = new List<double>();
            List<double> mod_ofs = new List<double>();
            List<double> ofs = new List<double>();
            List<ST_XYN> ST_TriPos = new List<ST_XYN>();
            List<ST_XYN> TriPosList = new List<ST_XYN>();
            string taskname = string.Empty;
            int chkcnt = 0;
            int xt_num = 0;
            double xtofs = 0;
            bool bQcodeChkByPhoto = PT_SET.bAddCapQrcode && (flyCfg == EM_FLY_CFG.MOD_UP || (flyCfg == EM_FLY_CFG.WS_MOD_UP && PT_SET.bBarcodeCamBackEn));
            if (Ofs_On)
            {
                xt_ofs.Add(list_xt[0].xt_cap_ofs);
                xt_ofs.Add(list_xt[1].xt_cap_ofs);
                if (cnt > 1)
                {
                    if (Dir)
                    {
                        ws_ofs.Add(list_xt[0].ws_cap_near_ofs);
                        ws_ofs.Add(list_xt[0].ws_cap_far_ofs);
                        mod_ofs.Add(list_xt[0].ws_cap_near_ofs);
                        mod_ofs.Add(0.00);

                    }
                    else
                    {
                        ws_ofs.Add(list_xt[0].ws_cap_far_ofs);
                        ws_ofs.Add(list_xt[0].ws_cap_near_ofs);
                        mod_ofs.Add(list_xt[0].ws_cap_near_ofs);
                        mod_ofs.Add(0.00);

                    }
                }
                else
                {
                    ws_ofs.Add(list_xt[0].ws_cap_far_ofs);
                    mod_ofs.Add(0.00);
                }

            }

            if (cnt > 1)
            {
                frPos.Sort((a, b) =>
                {
                    if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                    else return -1;
                });
                if (bQcodeChkByPhoto)
                {
                    capQrcodePos?.Sort((a, b) =>
                    {
                        if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                        else return -1;
                    });
                }
            }

            switch (flyCfg)
            {
                case EM_FLY_CFG.MOD_DW:
                    if (Demo && !PT_SET.xt1firsten) chkcnt = cnt;
                    else chkcnt = list_xt.Count;
                    //if (!VAR.ClearMt || cnt > 1) chkcnt = cnt;
                    //else chkcnt = list_xt.Count;
                    for (int i = 0; i < chkcnt; i++)
                    {
                        if (Dir)
                        {
                            xt_num = (i + 1) % 2;
                            if (Ofs_On)
                                xtofs = Math.Abs(xt_ofs[xt_num]);
                        }
                        else
                        {
                            xt_num = i;
                            if (Ofs_On)
                                xtofs = -Math.Abs(xt_ofs[xt_num]);
                        }

                        if ((list_xt[xt_num].cy_zk.isONByChkSen && list_xt[xt_num].XtMd != null && !Demo) || (Demo && list_xt[xt_num].XtMd != null))
                        {

                            var dwCapPos = Ofs_On ? list_xt[xt_num].st_cap_pos.y - xtofs : list_xt[xt_num].st_cap_pos.y;
                            
                            if(PT_SET.bDwAddCapQrcode)
                            {
                                var dwCapQrPos = list_xt[xt_num].st_cap_pos.y + list_xt[xt_num].DwCapQrCodeoffset ;
                                if ( (dwCapQrPos >= dwCapPos&& Dir)|| dwCapQrPos <= dwCapPos && !Dir)
                                {
                                    
                                    //TriPos.Add(dwCapPos);
                                    //ListTaskName.Add(CONST.ModDwFw[xt_num]);
                                    //ST_TriPos.Add(new ST_XYN(0.00, list_xt[xt_num].st_cap_pos.y, xt_num));
                                    TriPos.Add(dwCapQrPos);
                                    ListTaskName.Add(CONST.DwFindQrCodeFw[xt_num]);
                                    ST_TriPos.Add(new ST_XYN(0.00, dwCapQrPos, xt_num));
                                }
                                else
                                {
                                    TriPos.Add(dwCapQrPos);
                                    ListTaskName.Add(CONST.DwFindQrCodeFw[xt_num]);
                                    ST_TriPos.Add(new ST_XYN(0.00, dwCapQrPos, xt_num));
                                    //TriPos.Add(dwCapPos);
                                    //ListTaskName.Add(CONST.ModDwFw[xt_num]);
                                    //ST_TriPos.Add(new ST_XYN(0.00, list_xt[xt_num].st_cap_pos.y, xt_num));
                                }
                            }
                            else
                            {
                                TriPos.Add(dwCapPos);
                                ListTaskName.Add(CONST.ModDwFw[xt_num]);
                                ST_TriPos.Add(new ST_XYN(0.00, list_xt[xt_num].st_cap_pos.y, xt_num));
                            }                         
                        }

                    }
                    dwcam.ListTaskCfg(ListTaskName, ST_TriPos);
                    ax_y.SetHcmp(TriPos.ToArray(), dwcam.TriIO.num);
                    break;
                case EM_FLY_CFG.WS_UP:
                    taskname = CONST.WsUpFw;
                    ofs.AddRange(ws_ofs);
                    break;
                case EM_FLY_CFG.WS_MOD_UP:
                    taskname = CONST.WsModUpFw;
                    ofs.AddRange(ws_ofs);
                    break;
                case EM_FLY_CFG.MOD_UP:
                    taskname = CONST.ModUpFw;
                    ofs.AddRange(mod_ofs);
                    break;
            }

            if (taskname != string.Empty)//上拍照
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (Ofs_On)
                    {
                        if (Dir)
                        {
                            TriPos.Add(frPos[i].y - Math.Abs(ofs[i]));
                            if (bQcodeChkByPhoto)
                                TriPos.Add(capQrcodePos[i].y - Math.Abs(ofs[i]));
                        }
                        else
                        {
                            TriPos.Add(frPos[i].y + Math.Abs(ofs[i]));
                            if (bQcodeChkByPhoto)
                                TriPos.Add(capQrcodePos[i].y + Math.Abs(ofs[i]));
                        }
                    }
                    else
                    {
                        TriPos.Add(frPos[i].y);
                        if (bQcodeChkByPhoto) 
                            TriPos.Add(capQrcodePos[i].y);
                    }
                    ListTaskName.Add(taskname);
                    if (PT_SET.bAddCapQrcode)
                    {
                        if (flyCfg == EM_FLY_CFG.MOD_UP && capQrcodePos.Count > 0)
                            ListTaskName.Add(CONST.FindQrCodeFw);
                        else if (flyCfg == EM_FLY_CFG.WS_MOD_UP && PT_SET.bBarcodeCamBackEn && capQrcodePos.Count > 0)
                            ListTaskName.Add(CONST.WsModQrcode_Shp);
                    }
                }
                if (bQcodeChkByPhoto)
                {
                    TriPosList = frPos.Concat(capQrcodePos).ToList();
                    TriPosList.Sort((a, b) =>
                    {
                        if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                        else return -1;
                    });
                }
                upcam.ListTaskCfg(ListTaskName, bQcodeChkByPhoto ? TriPosList : frPos, IfClear);
                ax_y.SetHcmp(TriPos.ToArray(), upcam.TriIO.num, 4, IfClear);
            }
        }
        /// <summary>
        /// 等待飞拍视觉结果
        /// </summary>
        public EM_RES WaitCamRes(Cam cam, int cnt)
        {
            int n;
            int count;
            if (cnt < 1) return EM_RES.OK;
            for (n = 0; n < 200; n++)
            {
                count = 0;
                foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                {
                    if (task.ResData.bUpdate) count++;
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("结果校验过程count数量{0}相机任务{1}", count,cam.List_vs_task_cur.Count));
                if (count == cam.List_vs_task_cur.Count)
                {
                    if (Demo)
                    {
                        foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                        {
                            task.ResData.bOK = true;
                            task.ResData.PosMM = new ST_XYA(0, 0, 0);
                            task.ResData.BarCode = "abc";
                        }
                    }
                    break;
                }
                //time out
                Thread.Sleep(30);
            }

            if (n >= 200)
            {
                foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                {
                    task.ResData.bOK = false;
                    if (!task.ResData.bUpdate)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}{1}数据更新超时!", cam.mName, task.TaskName) : string.Format("{0}{1} data update timeout!          ({0}{1}数据更新超时!)", cam.mName, task.TaskName), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Cam);

                    }
                }

                return EM_RES.TIMEOUT;
            }
            if(cam.inputImageCnt!=cam.List_vs_task_cur.Count)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR,  string.Format("当前工站{0}相机{1}输入图片计数和相机任务数不等，所有任务设置拍照失败!", disc, cam.mName)
                    , DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Cam);
                foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                {
                    task.ResData.bOK = false;
                    task.ResData.bUpdate = true;
                }
                cam.inputImageErrCnt++;
                if (cam.inputImageErrCnt >2)
                {
                    cam.inputImageErrCnt = 0;

                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "相机异常!" : "Cam Err", 20, true, ErrCode: ShowErrMsg.WsPhotoErr);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = VAR.IsChinese ? "确定" : "Keep running";
                    warn.ws = null;
                    warn.title = VAR.IsChinese ? "提示:相机异常" : "Tip: Material is biased";
                    warn.msg = VAR.IsChinese ? $"当前相机{cam.disc}发生多次误触发，请确认相机" : "Cam has abnormal tri Err,pls check the cam !\r\n当前相机发生多次误触发，请确认相机!";
                    warn.lb_msg = warn.msg + "\r\n  1.请检查相机，按确定将继续生产!\r\n  ";
                    DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.BarcodeAbnormal);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    return EM_RES.OK;
                }
            }

            if (NewSysInf.UserParams.bPicCntForSafe&&  cam.ListResultTemp.Count != cam.List_vs_task_cur.Count)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("当前工站{0}相机{1}输入图片计数和相机任务数不等，所有任务设置拍照失败!", disc, cam.mName)
                    , DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Cam);
                foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                {
                    task.ResData.bOK = false;
                    task.ResData.bUpdate = true;
                }
                cam.inputImageErrCnt++;
                if (cam.inputImageErrCnt > 2)
                {
                    cam.inputImageErrCnt = 0;

                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "相机异常!" : "Cam Err", 20, true, ErrCode: ShowErrMsg.WsPhotoErr);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = VAR.IsChinese ? "确定" : "Keep running";
                    warn.ws = null;
                    warn.title = VAR.IsChinese ? "提示:相机异常" : "Tip: Material is biased";
                    warn.msg = VAR.IsChinese ? $"当前相机{cam.disc}发生多次误触发，请确认相机" : "Cam has abnormal tri Err,pls check the cam !\r\n当前相机发生多次误触发，请确认相机!";
                    warn.lb_msg = warn.msg + "\r\n  1.请检查相机，按确定将继续生产!\r\n  ";
                    DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.BarcodeAbnormal);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    return EM_RES.OK;
                }
            }

            return EM_RES.OK;
        }

        /// <summary>
        /// 飞拍结果确认
        /// </summary>
        /// <param name="cam">检查相机</param>
        /// <param name="cnt">结果个数</param>
        /// <returns></returns>
        public EM_RES ChkCamRes(Cam cam, int cnt, bool chksame = true)
        {
            if (cnt <= 0 || Demo) return EM_RES.OK;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR,  string.Format("飞拍结果数量:{0}校验数量：{1}", cam.List_vs_task_cur.Count, cnt), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.CaptureAbnomal);
            if (cam.List_vs_task_cur.Count == cnt)
            {
                if (cam.List_vs_task_cur.Count == 2 && chksame)
                {
                    ST_XYA camres = cam.List_vs_task_cur[0].ResData.PosMM - cam.List_vs_task_cur[1].ResData.PosMM;
                    if ((Math.Abs(camres.x) < 0.002) && (Math.Abs(camres.y) < 0.002 && (Math.Abs(camres.a) < 0.002)) && cam.List_vs_task_cur[0].ResData.bOK && cam.List_vs_task_cur[1].ResData.bOK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}_{1}飞拍两结果数据一样", disc, cam.disc) : string.Format("{0}_{1} The data of the two shots are the same     ({2}_{3}飞拍两结果数据一样)", englishdisc, cam.englishdisc, disc, cam.disc), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.CaptureAbnomal);
                        return EM_RES.ERR;
                    }
                }
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}_{1}飞拍缺少结果", disc, cam.disc) : string.Format("{0}_{1} flyshot result is null      ({2}_{3}飞拍缺少结果)", englishdisc, cam.englishdisc, disc, cam.disc), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.CaptureAbnomal);
                return EM_RES.CAM_LACKRES;
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 寻找OK与NG料盘放料的第一个位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EM_RES FindOkOrNgTrayPos(ref ST_XYZA pos, TrayBox traybox, WS ws)
        {
            List<WS.MdDat> EmptyPos = new List<WS.MdDat>();
            int i = 0;
            //寻找第一个OK料
            foreach (XT xt in list_xt)
            {
                if ((!xt.cy_zk.isONByChkSen && !Demo) || xt.XtMd == null)
                {
                    i++;
                    xt.cy_zk.SetOff();
                    continue;
                }
                if (xt.XtMd.res != (int)Product.EM_CM_RES.OK && traybox.disc == traybox_ok.disc) continue;
                if (xt.XtMd.res <= (int)Product.EM_CM_RES.OK && traybox.disc == traybox_ng.disc) continue;
                int ngcode = traybox.disc == traybox_ok.disc ? -10000 : xt.XtMd.res;
                List<Product.Tray.PosInf> Listpos_empty = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
                List<Product.Tray.PosInf> Listpos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY, ngcode);

                if (ax_x.fenc_pos < (ws.list_md[0].st_pos[id].x + 5) && ax_x.fenc_pos > (ws.list_md[7].st_pos[id].x - 5))
                    pos.x = ax_x.fenc_pos;
                else
                    pos.x = FindMod == EM_FIN_MOD.HIGH ? ws.list_md[7].st_pos[id].x : ws.list_md[0].st_pos[id].x;
                pos.z = 0;
                if (!traybox.IsReady || traybox.tray_cur == null || Listpos.Count == 0)
                {
                    pos.y = COM.tray_ok.tl[id].y + xt.st_rol_cap.y - list_xt[0].st_rol_cap.y;
                    pos.a = -10000;
                }
                else
                {
                    if (Listpos_empty.Count == traybox.tray_cur.list_pos.Count && PT_SET.bEnVsTray)
                    {
                        pos.y = traybox.tray_cur.TrayVsPos[id].y;
                        pos.a = -traybox.tray_cur.TrayVsPos[id].x + pos.x + traybox.tray_cur.TrayVsPos[id].z;
                    }
                    else
                    {
                        pos.y = Listpos[0].Pos[id].y + xt.st_rol_cap.y - list_xt[0].st_rol_cap.y;
                        pos.a = -Listpos[0].Pos[id].x + pos.x + Listpos[0].Pos[id].a;
                    }
                    if (pos.a < 0)
                    {
                        pos.x = pos.x - pos.a;
                        pos.a = 0;
                    }
                }

                return EM_RES.OK;
            }
            if (i == 2) return EM_RES.ABORT;
            return EM_RES.NEXT;
        }

        /// <summary>
        /// 寻找料盘放料另一模块的下一个的X1位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EM_RES FindNextTrayPosX1(ref double pos_x1, TrayBox traybox, WS ws, UpDownLoad ud)
        {
            List<WS.MdDat> EmptyPos = new List<WS.MdDat>();
            ST_XYZA pos = new ST_XYZA();
            Product.EM_CM_RES traymod = Product.EM_CM_RES.EMPTY;
            //寻找第一个OK料
            traymod = traybox.disc == traybox_fd.disc ? Product.EM_CM_RES.UNTEST : Product.EM_CM_RES.EMPTY;
            List<Product.Tray.PosInf> Listpos = traybox.tray_cur.GetPosList(traymod);
            //if (Listpos.Count > 0 &&((ud.bUpdateFBPos_OKNG && traymod==Product.EM_CM_RES.EMPTY)||(ud.bUpdateFBPos_UT && traymod==Product.EM_CM_RES.UNTEST)))
            //if (FindMod != EM_FIN_MOD.ALL && ((traybox_fd.disc==traybox.disc && !bUpdateFBPos_UT)||(traybox_fd.disc!=traybox.disc && !bUpdateFBPos_OKNG)))
            //{
            //     pos_x1 = -10000;
            //     return EM_RES.OK;
            //}

            if (Listpos.Count > 0 && (ud.TrayGoPos < (ws.list_md[0].st_pos[ud.id].x + 5) && ud.TrayGoPos > (ws.list_md[7].st_pos[ud.id].x - 5)))
            {
                //pos.x = ud.FeedBackPos.x;
                pos.x = ud.TrayGoPos;
                if (traybox.disc == COM.traybox_ng.disc)
                {
                    int cn = traybox.tray_cur.list_pos.Count / 2;
                    pos.a = -traybox.tray_cur.list_pos[cn].Pos[ud.id].x + pos.x + traybox.tray_cur.list_pos[cn].Pos[ud.id].a;
                }
                else pos.a = -Listpos[0].Pos[ud.id].x + pos.x + Listpos[0].Pos[ud.id].a;
                if (pos.a < 0)
                {
                    pos.x = pos.x - pos.a;
                    pos.a = 0;
                }
                pos_x1 = pos.a;
            }
            else
            {
                pos_x1 = -10000;
            }
            return EM_RES.OK;

        }

        /// <summary>
        /// 寻找待测料盘上料的最后一个位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EM_RES FindFdTrayPos(ref ST_XYZA pos, ref List<ST_XYN> TrayTriPos, ref List<ST_XYN> TrayCapQrcodeTriPos, WS ws, bool Dir = true)
        {
            List<WS.MdDat> UntestPos = new List<WS.MdDat>();
            List<Product.Tray.PosInf> Listpos = traybox_fd.tray_cur.GetPosList(Product.EM_CM_RES.UNTEST);
            if (Listpos.Count == 0 || traybox_fd.tray_cur == null || !traybox_fd.IsReady) return EM_RES.END;
            GetWsPosTeam(ref UntestPos, ws, Product.EM_CM_RES.EMPTY, FindMod);
            if (UntestPos.Count == 0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}放料完成或找不到可以放料的位置!", ws.disc) : string.Format("{0}place completed or can't find place to place     ({0}放料完成或找不到可以放料的位置!)", ws.disc));
                return EM_RES.ABORT;
            }
            TrayTriPos.Clear();
            TrayCapQrcodeTriPos.Clear();
            double pos_compen = (Dir == true) ? 10 : -10;
            if (ax_x.fenc_pos < (ws.list_md[0].st_pos[id].x + 10) && ax_x.fenc_pos > (ws.list_md[7].st_pos[id].x - 10))
            {
                pos.x = ax_x.fenc_pos;
                // pos.x = FeedBackPos.x;
            }
            else
            {
                if (UntestPos.Count != 0) pos.x = UntestPos[0].st_pos[id].x;
                else pos.x = FindMod == EM_FIN_MOD.HIGH ? ws.list_md[7].st_pos[id].x : ws.list_md[0].st_pos[id].x;
            }

            if (UntestPos.Count > 1 && Listpos.Count > 1 && Listpos[0].Row == Listpos[1].Row)
            {

                TrayTriPos.Add(new ST_XYN(pos.x, Listpos[0].Pos[id].y, Listpos[0].idx));
                TrayTriPos.Add(new ST_XYN(pos.x, Listpos[1].Pos[id].y, Listpos[1].idx));
                TrayTriPos.Sort((a, b) =>
                {
                    if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                    else return -1;
                });
                pos.y = TrayTriPos[1].y + pos_compen;
                if (PT_SET.bAddCapQrcode)
                {
                    var capPosList = new List<PosInf> { traybox_fd.tray_cur.list_AddCapQrcodepos[Listpos[0].idx], traybox_fd.tray_cur.list_AddCapQrcodepos[Listpos[1].idx] };
                    TrayCapQrcodeTriPos?.Add(new ST_XYN(pos.x, capPosList[0].Pos[id].y, capPosList[0].idx));
                    TrayCapQrcodeTriPos?.Add(new ST_XYN(pos.x, capPosList[1].Pos[id].y, capPosList[1].idx));

                    var listTemp = TrayTriPos.Concat(TrayCapQrcodeTriPos).ToList();
                    listTemp.Sort((a, b) =>
                    {
                        if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                        else return -1;
                    });
                    pos.y = listTemp[listTemp.Count - 1].y + pos_compen;
                }
            }
            else
            {

                TrayTriPos.Add(new ST_XYN(pos.x, Listpos[0].Pos[id].y, Listpos[0].idx));
                pos.y = Listpos[0].Pos[id].y + pos_compen;
                if (PT_SET.bAddCapQrcode)
                {
                    TrayCapQrcodeTriPos?.Add(new ST_XYN(pos.x, traybox_fd.tray_cur.list_AddCapQrcodepos[Listpos[0].idx].Pos[id].y, traybox_fd.tray_cur.list_AddCapQrcodepos[Listpos[0].idx].idx));
                    var listTemp = TrayTriPos.Concat(TrayCapQrcodeTriPos).ToList();
                    listTemp.Sort((a, b) =>
                    {
                        if ((a.y > b.y && Dir) || (a.y < b.y && !Dir)) return 1;
                        else return -1;
                    });
                    pos.y = listTemp[listTemp.Count - 1].y + pos_compen;
                }
            }

            pos.a = -Listpos[0].Pos[id].x + pos.x + Listpos[0].Pos[id].a;

            if (pos.a < 0)
            {
                pos.x = pos.x - pos.a;
                pos.a = 0;
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 飞拍移动到料盘
        /// </summary>
        /// <returns></returns>
        public EM_RES FLyToTray(ref bool bquit, ST_XYZA endpos, List<ST_XYN> WsTriPos, List<ST_XYN> TrayTriPos, List<ST_XYN> TrayCapQrcodeTriPos, List<ST_XYN> WsModQrcodeTriPos, List<ST_XYN> DownQrcodeTriPos, TrayBox traybox = null, bool Dir = false)
        {

            EM_RES res;
            double pos_mov_x = list_xt[0].st_rol_cap.y;
            AXIS ax_x_tray = null;
            int xt_cnt = 0;
            if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn && ax_y.fenc_pos > list_xt[0].st_rol_cap.y)
            {
                foreach (XT xt in list_xt)
                {
                    if ((xt.cy_zk.isONByChkSen && xt.XtMd != null && xt.XtMd.res >= 0 && !Demo) || (Demo && xt.XtMd != null)) 
                        xt_cnt++;
                }

            }
            if (WsTriPos.Count == 0 && TrayTriPos.Count == 0 && (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN || (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn && xt_cnt == 0)))
            {
                return EM_RES.NEXT;
            }
                
            //加载流程
            try
            {
                //if (traybox != null && endpos.a != -10000) ax_x_tray = traybox.ax_x;
                if (traybox != null) ax_x_tray = traybox.ax_x;
                if (PT_SET.bDwAddCapQrcode)
                {

                }
                else
                {
                    //加载工站拍照点
                    FlyCfg(EM_FLY_CFG.WS_MOD_UP, WsTriPos, WsTriPos.Count, Dir, capQrcodePos: WsModQrcodeTriPos);
                }
                
               

                //加载吸头拍照点
                if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn)
                {

                    FlyCfg(EM_FLY_CFG.MOD_DW, new List<ST_XYN>(), xt_cnt, Dir);
                }

                ////二维码绑定回检
                //if (PT_SET.OpenDownQrde && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN)
                //{
                //    if(PT_SET.DownDownQrde)
                //    {
                //        FlyCfg(EM_FLY_CFG.MOD_DW, DownQrcodeTriPos, xt_cnt, Dir);
                //    }
                //}
                    //加载料盘拍照点
                FlyCfg(EM_FLY_CFG.MOD_UP, TrayTriPos, TrayTriPos.Count, Dir, true, WsTriPos.Count > 0 ? false : true, TrayCapQrcodeTriPos);
                
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍结束位置X:{1:f3},Y:{2:f3},X1:{3:f3},方向{4}", disc, endpos.x, endpos.y, endpos.a, Dir) : string.Format("{0}move to flyshot end posX :{2:f3},Y:{3:f3},X1:{4:f3},DIR:{5}       ({1}定位到飞拍结束位置X:{2:f3},Y:{3:f3},X1:{4:f3},方向{5})", englishdisc, disc, endpos.x, endpos.y, endpos.a, Dir));

                if (ax_x_tray != null)
                {
                    int n = 0;
                    while (traybox.runin || !traybox.ax_x.isStop)
                    {
                        if (n++ > 500) break;
                        if (bquit) return EM_RES.QUIT;
                        Thread.Sleep(10);
                    }
                    traybox.MoveToLastPos[id] = true;
                    if (n > 500)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位超时5S", ax_x_tray.disc) : string.Format("{0}move timeout(5s)       ({0}定位超时5S)", ax_x_tray.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                        return EM_RES.ERR;
                    }
                    if (traybox.disc == COM.traybox_fd.disc)
                    {
                        int posmaxofs = WsTriPos.Count > 0 ? 10 : 2;
                        if ((Math.Abs(ax_x_tray.fenc_pos - endpos.a) > posmaxofs) && !traybox.runin && traybox.ax_x.isStop)
                        {
                            //检查Z轴高度
                            if (Math.Abs(ax_z.fenc_pos) > 3)
                            {
                                res = MT.Move(ref bquit, ref ax_z, 0);
                                if (res != EM_RES.OK) return res;
                            }

                            res = MT.Move(ref bquit, ref ax_x_tray, endpos.a);
                            if (res != EM_RES.OK) return res;
                        }
                    }
                }

                //检查Z轴高度
                if (Math.Abs(ax_z.fenc_pos) > 3)
                {
                    res = MT.Move(ref bquit, ref ax_z, 0);
                    if (res != EM_RES.OK) return res;
                }

                //定位X X1与Y轴
                if (WsTriPos.Count > 0)
                {
                    res = ax_y.MoveTo(ref bquit, endpos.y);
                    if (res != EM_RES.OK) return res;
                    int t = 0;
                    while (true)
                    {
                        if (ax_y.fenc_pos < pos_mov_x)
                        {
                            res = MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_x_tray, endpos.a, ref ax_u1, PT_SET.isopen_degree ? -PT_SET.degree : 0, ref ax_u2, PT_SET.isopen_degree ? -PT_SET.degree : 0, 3000);
                            if (res != EM_RES.OK) return res;
                            break;
                        }
                        if (bquit) return EM_RES.QUIT;
                        Thread.Sleep(5);
                        if (t++ > 1000)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}飞拍定位超时!", ax_y.disc) : string.Format("{0}  flyshot move timeout!    ({0}飞拍定位超时!)", ax_y.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.TIMEOUT;
                        }

                    }
                    res = ax_y.WaitForMoveDone(ref bquit, endpos.y, 10000);
                    if (res != EM_RES.OK) return res;
                }
                else
                {

                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}放料{1}--> X:{2} Y:{3} X1:{4}", disc, traybox.disc, endpos.x, endpos.y, endpos.a) : string.Format("{0}Place mod{1}-->X:{4} Y:{5} X1:{6}         ({2}放料{3}--> X:{4} Y:{5} X1:{6})", englishdisc, traybox.name, disc, traybox.disc, endpos.x, endpos.y, endpos.a));
                    AXIS ax_x_tray_temp = traybox.runin == true ? null : ax_x_tray;
                    res = MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_y, endpos.y, ref ax_x_tray_temp, endpos.a);
                    if (res != EM_RES.OK) return res;
                }

            }
            finally
            {
                traybox.runin = false;
                ax_y.Stop();
                ax_y.DisableHcmp(upcam.TriIO.num);
                if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn)
                    ax_y.DisableHcmp(dwcam.TriIO.num);
            }
            return EM_RES.OK;
        }

        //最后一个位置回检
        public EM_RES FLyToLastPos(ref bool bquit, ST_XYZA endpos, List<ST_XYN> WsTriPos, bool Dir = false, List<ST_XYN> capQrcodePos = null)
        {

            EM_RES res;
            double pos_mov_x = list_xt[0].st_rol_cap.y;
            int xt_cnt = 0;
            if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn && ax_y.fenc_pos > list_xt[0].st_rol_cap.y)
            {
                foreach (XT xt in list_xt)
                {
                    if ((xt.cy_zk.isONByChkSen && xt.XtMd != null && xt.XtMd.res >= 0 && !Demo) || (Demo && xt.XtMd != null)) xt_cnt++;
                }

            }
            if (WsTriPos.Count == 0 && (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN || (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn && xt_cnt == 0))) return EM_RES.NEXT;

            //加载流程
            try
            {
                //加载工站拍照点
                if (PT_SET.bDwAddCapQrcode)
                {

                }
                else
                {
                    FlyCfg(EM_FLY_CFG.WS_MOD_UP, WsTriPos, WsTriPos.Count, Dir, capQrcodePos: capQrcodePos);
                }
              
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍结束位置X:{1:f3},Y:{2:f3},X1:{3:f3},方向{4}", disc, endpos.x, endpos.y, endpos.a, Dir) : string.Format("{0}move to flyshot end posX :{2:f3},Y:{3:f3},X1:{4:f3},DIR:{5}            ({1}定位到飞拍结束位置X:{2:f3},Y:{3:f3},X1:{4:f3},方向{5})", englishdisc, disc, endpos.x, endpos.y, endpos.a, Dir));
                //定位X X1与Y轴
                if (WsTriPos.Count > 0)
                {
                    res = ax_y.MoveTo(ref bquit, endpos.y);
                    if (res != EM_RES.OK) return res;
                    int t = 0;
                    while (true)
                    {
                        if (ax_y.fenc_pos < pos_mov_x)
                        {
                            res = MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_u1, 0, ref ax_u2, 0, 3000);
                            if (res != EM_RES.OK) return res;
                            break;
                        }
                        if (bquit) return EM_RES.QUIT;
                        Thread.Sleep(5);
                        if (t++ > 1000)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}飞拍定位超时!", ax_y.disc) : string.Format("{0} flyshot move timeout!     ({0}飞拍定位超时!)", ax_y.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.TIMEOUT;
                        }

                    }
                    res = ax_y.WaitForMoveDone(ref bquit, endpos.y, 10000);
                    if (res != EM_RES.OK) return res;
                }
            }
            finally
            {
                ax_y.Stop();
                ax_y.DisableHcmp(upcam.TriIO.num);
                //if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn)
                //    ax_y.DisableHcmp(dwcam.TriIO.num);
            }
            return EM_RES.OK;
        }
        int QrCodeChkErrCnt = 0;
        /// <summary>
        /// 工站上拍照二维码检查异常弹窗
        /// </summary>
        /// <returns></returns>
        public EM_RES QrCodeChkShow(Cam.VisionTask task, WS.MdDat md)
        {
            //取消判断，强制开启 bUpWsChkQrCodeEn
            if (PT_SET.BarcodeMode != (int)PT_SET.BAR_SCAN.UP_SCAN || NewSysInf.UserParams.bUpWsChkQrCodeOff)
                return EM_RES.OK;
            var QrCode = task.ResData.BarCode;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"开始二维码对比,检出二维码{QrCode},模组二维码{md.bardcode}");

            if (QrCode == null || QrCode.Length < 3)
            {
                bool bGetOrcodOnWs = NewSysInf.UserParams.bGetOrcodOnWs;
                if (bGetOrcodOnWs)
                {
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"当前设置仅在工站扫码，模组{md.test_idx}拍二维码失败将取回重新上料");
                    return EM_RES.RETRY;
                }
                if (QrCodeChkErrCnt < PT_SET.UpWsChkQrCodeCnt)
                {
                    QrCodeChkErrCnt++;
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"当前模组{md.test_idx}无二维码，自动更新3342");
                    return EM_RES.RETRY;
                }
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "二维码异常!" : "ArCode Err", 20, true, ErrCode: ShowErrMsg.WsPhotoErr);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = VAR.IsChinese ? "确定" : "Keep running";
                warn.ws = null;
                warn.title = VAR.IsChinese ? "提示:二维码异常" : "Tip: Material is biased";
                warn.msg = VAR.IsChinese ? $"当前工站识别二维码失败!流程名字{task.TaskName},请停机从新做视觉" : "This Ws Cannot FInd QrCode !\r\n当前工站识别二维码失败!";
                warn.lb_msg = warn.msg + "\r\n  1.请检查视觉，按确定将产品取出继续生产!\r\n  ";
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.BarcodeAbnormal);
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                md.res = 3342;
                QrCodeChkErrCnt = 0;
                return EM_RES.OK;

            }
            else if (QrCode != md.bardcode)
            {
                bool bGetOrcodOnWs = NewSysInf.UserParams.bGetOrcodOnWs;
                if (bGetOrcodOnWs)
                {
                    QrCodeChkErrCnt++;
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"当前模组{md.test_idx}无二维码，自动更新3342");
                    return EM_RES.RETRY;
                    //md.bardcode = QrCode;
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"当前设置在工站扫码，工站{md.WS_ID}-{md.test_idx}自动更新二维码{QrCode}");
                    //return EM_RES.OK;
                }
                if (md.bardcode == null || md.bardcode.Length < 3)
                {
                    QrCodeChkErrCnt++;
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"当前模组{md.test_idx}无二维码，自动更新3342");
                    return EM_RES.RETRY;

                    //md.bardcode = QrCode;
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"当前模组无二维码，自动更新二维码{QrCode}");
                    //VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    //return EM_RES.OK;
                }
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "二维码异常!" : "ArCode Err", 20, true, ErrCode: ShowErrMsg.WsPhotoErr);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = VAR.IsChinese ? "更新二维码" : "Update QrCode";
                warn.abort_txt = VAR.IsChinese ? "NG放回" : "Abort Err";
                //  warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                warn.ws = null;
                warn.title = VAR.IsChinese ? "提示:二维码异常" : "Tip: Material is biased";
                warn.msg = VAR.IsChinese ? $"当前工站识别二维码{QrCode}与模组二维码不一致{md.bardcode}!流程名字{task.TaskName}" : "This Ws  Find QrCode is different with recoder  !\r\n当前工站识别二维码失败!";
                warn.lb_msg = warn.msg + "\r\n  1.如果当作Ng放回,请按NG放回!\r\n  " +
                    $"2.按更新二维码，则按照当前二维码给模组,模组编号{md.Num}!" +
                                              "3.按停止运行则停机，请从新检查吸头真空是否异常，后再运行!";
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.BarcodeAbnormal);
                if (DialogResult.OK == logres)
                {
                    //md.bardcode = QrCode;
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "二维码不一致员工操作，更新二维码");
                    //VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    //return EM_RES.OK;
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "二维码不一致员工操作，NG放回");
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    return EM_RES.RETRY;
                }
                else
                {
                    md.res = 3342;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "二维码不一致员工操作，NG放回");
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    return EM_RES.RETRY;
                    //md.bardcode = QrCode;
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "二维码不一致员工操作，更新二维码");
                    //VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    //return EM_RES.OK;
                }
            }
            return EM_RES.OK;
        }

        public EM_RES FlyCamToLastPos(ref bool bquit, WS ws, ref ST_XYZA endpos, bool dir = true)
        {
            List<ST_XYN> WsTriPos = new List<ST_XYN>();
            List<ST_XYN> capQrcodePos = new List<ST_XYN>();
            // List<ST_XYN> TrayTriPos = new List<ST_XYN>();
            List<Cam.VisionTask> List_vs_task_temp = new List<Cam.VisionTask>();
            //List<Cam.VisionTask> List_vs_task_DwTemp = new List<Cam.VisionTask>();
           
            bool bQcodeChkByPhoto = PT_SET.bAddCapQrcode && PT_SET.bBarcodeCamBackEn;
            VisionOutPutData BackCamdata1= new VisionOutPutData();
            VisionOutPutData BackCamdata2 = new VisionOutPutData();
            //ST_XYZA endpos=new ST_XYZA();
            EM_RES res = EM_RES.OK;
            //配置工站回拍
            if (PT_SET.bDwAddCapQrcode)
            {
                if (FeedBackWs.Count > 0)
                {
                    foreach (WS.MdDat md in FeedBackWs)
                    {
                        XT xt;
                        if (md.Num > 8)
                        {
                            xt = list_xt[0];
                        }
                        else
                        {
                            xt = list_xt[1];
                        }
                        res = UpCamBackCheck(ref bquit, ref ws, xt, id, new ST_XY(FeedBackPos.x, md.st_pos[id].y), ref BackCamdata1);
                        if (res != EM_RES.OK)
                        {
                            return res;
                        }
                    }
                    if (PT_SET.Check2open)
                    {
                        foreach (WS.MdDat md in FeedBackWs)
                        {
                            XT xt;
                            if (md.Num > 8)
                            {
                                xt = list_xt[0];
                            }
                            else
                            {
                                xt = list_xt[1];
                            }
                            res = UpCamBackCheck2(ref bquit, ref ws, xt, id, new ST_XY(FeedBackPos.x, md.st_pos[id].y), ref BackCamdata2);
                            if (res != EM_RES.OK)
                            {
                                return res;
                            }
                        }
                    }                   
                }
            }
            else
            {
                //配置工站回拍
                if (((bUpdateFBPos_OKNG) || (bUpdateFBPos_UT)) && FeedBackWs.Count > 0)// && ax_y.fenc_pos > list_xt[0].st_cap_pos.y)
                {
                    foreach (WS.MdDat md in FeedBackWs)
                    {
                        WsTriPos.Add(new ST_XYN(FeedBackPos.x, md.st_pos[id].y, md.Num - 1));
                        if (bQcodeChkByPhoto) capQrcodePos.Add(new ST_XYN(FeedBackPos.x, md.st_CapQrcodePos[id].y, md.Num - 1));
                    }

                    if (Math.Abs(FeedBackPos.x - ax_x.fenc_pos) > 2 || Math.Abs(FeedBackPos.y - ax_y.fenc_pos) > 5)
                    {

                        res = MT.Move(ref bquit, ref ax_x, FeedBackPos.x, ref ax_y, FeedBackPos.y);
                        if (res != EM_RES.OK) return res;
                    }

                    dir = false;
                }
            }
          
            //配置飞拍结束位置
            endpos.x = FeedBackPos.x;
            endpos.y = list_xt[1].st_rol_cap.y;
            endpos.z = 0;
            endpos.a = 0;

            // if (ax_y.fenc_pos > endpos.y && dir && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn) dir = false;
            //加载触发点并定位
            res = FLyToLastPos(ref bquit, endpos, WsTriPos, dir, capQrcodePos);
            if (res != EM_RES.OK) return res;

            //等待结果
            // bool barcode_err = false;
            var triPosCnt = WsTriPos.Count;
            if (bQcodeChkByPhoto)
                triPosCnt += WsTriPos.Count;
            if (PT_SET.bDwAddCapQrcode)
            {

            }
            else
            {
                WaitCamRes(upcam, triPosCnt);
            }
                
            //检查结果 
            res = ChkCamRes(upcam, triPosCnt);
            if (res != EM_RES.OK) return res;



            //回拍结果判断
            //modupcam_task.Clear();
            bool mtofs = false;
            if (!Demo)
            {
                var wsTriCnt = WsTriPos.Count;
                if (bQcodeChkByPhoto) wsTriCnt = wsTriCnt * 2;
                //复制任务
                for (int i = 0; wsTriCnt > 0 && i < wsTriCnt; i++)
                {
                    List_vs_task_temp.Add(upcam.List_vs_task_cur[i]);
                }

                for (int i = 0; WsTriPos.Count > 0 && i < WsTriPos.Count; i++)
                {
                    Cam.VisionTask task = List_vs_task_temp[i];
                    Cam.VisionTask qrcodeTask = null;
                    if (bQcodeChkByPhoto)//单独扫码位置
                    {
                        if (task.TaskName != CONST.WsModUpFw) continue;

                        if (i % 2 == 0)
                        {
                            qrcodeTask = List_vs_task_temp[i + 1];
                        }
                        else
                        {
                            qrcodeTask = List_vs_task_temp[i - 1];
                        }
                        if (qrcodeTask.TaskName != CONST.WsModQrcode_Shp) return EM_RES.ERR;
                    }
                    else
                    {
                        qrcodeTask = task;
                    }
                    //二维码回检
                    res = QrCodeChkShow(qrcodeTask, ws.list_md[WsTriPos[i].n]);
                    if (res != EM_RES.OK) return res;

                    //放偏回检确认
                    res = CheckWsPutOkShow(task, WsTriPos[i]);
                    if (res != EM_RES.OK) return res;
                }

            }

            //清空相机数据
            if (!upcam.FlushOk && !upcam.FlushUpdate)
            {
                Task tak = new Task(() =>
                {
                    upcam.FlushUpdate = true;
                    upcam.mAcqFifo.Flush();
                    upcam.FlushOk = true;
                    upcam.FlushUpdate = false;
                });
                tak.Start();
            }

            FeedBackWs.Clear();
            //if (barcode_err) return EM_RES.ABORT;
            return EM_RES.OK;
        }
        /// <summary>
        /// 判断是否有放偏
        /// </summary>
        /// <param name="task"></param>
        /// <param name="posid"></param>
        /// <returns></returns>
        private bool  CheckArea(Cam.VisionTask task,int posid,out int NgType)
        {
            bool ret = false;
            NgType = 0;
            try
            {
                NgType = 0;
                var date = task.ResData.Message.Split(',');
                var res = AreaCal(date, out var left, out var right, out var up, out var down);
                if (res != EM_RES.OK) return  ret=false;
                bool bXYNg = Math.Abs(task.ResData.PosMM.x) > PT_SET.Vs_XYofs
                            || Math.Abs(task.ResData.PosMM.y) > PT_SET.Vs_XYofs;
                bool bANg = Math.Abs(task.ResData.PosMM.a) > PT_SET.Vs_Rofs;
                bool bAreaNg= (Math.Abs(left - PT_SET.LeftArea) > PT_SET.Area && id == 0)
                            || (Math.Abs(right - PT_SET.RightArea) > PT_SET.Area && id == 0)
                            || (up != 0) && (Math.Abs(up - PT_SET.UpArea) > PT_SET.Area && PT_SET.bConnectorCheck && id == 0)
                            || (down != 0)&&(Math.Abs(down - PT_SET.DownArea) > PT_SET.Area && PT_SET.bConnectorCheck && id == 0)
                            || (Math.Abs(left - PT_SET.LeftArea2) > PT_SET.Area2 && id == 1)
                            || (Math.Abs(right - PT_SET.RightArea2) > PT_SET.Area2 && id == 1)
                            || (up!=0)&&(Math.Abs(up - PT_SET.UpArea2) > PT_SET.Area2 && PT_SET.bConnectorCheck && id == 1)
                            || (down!=0)&&(Math.Abs(down - PT_SET.DownArea2) > PT_SET.Area2 && PT_SET.bConnectorCheck && id == 1);
                NgType = bXYNg ? 1 : bANg ? 2 : bAreaNg ? 3 : 0;
                var bNG = !task.ResData.bOK    || bXYNg  || bANg  || bAreaNg;
                return ret = !bNG;

            }
            catch(Exception ee)
            {
                return  ret =false;
            }
            finally
            {
                if(!ret)
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("物料放偏坐标{0}，工位{1}重新确认", task.ResData.PosMM.ToString(), posid)
                    : string.Format("Crooked material pos{0},WS{1} reconfirms           (物料放偏坐标{0}，工位{1}重新确认)", task.ResData.PosMM.ToString(), posid));
            }
           

                        
        }
        private EM_RES AreaCal(string[] data, out double left, out double right, out double up, out double down)
        {
            left = double.MaxValue;
            right = double.MaxValue;
            up = double.MaxValue; ;
            down = double.MaxValue;
            if (data.Length < 2)
            {
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                warn.ws = null;//增加语言
                warn.title = MultiLanguage.TxtSelct("提示:数据异常", "Tip: Data File  Err", "Mẹo: Lỗi tệp dữ liệu");
                warn.msg = MultiLanguage.TxtSelct(
                    "当前放偏检测没有防压伤功能!",
                    "The current bias detection does not have the function of preventing pressure injury!",
                    "Phát hiện sai lệch hiện tại không có chức năng ngăn ngừa chấn thương do áp suất!");
                warn.lb_msg = MultiLanguage.TxtSelct(
                    "提示:当前缺少防压伤面积数据，请确认视觉文件和模板，请确认!\r\n" +
                    "1.如果忽视,请按继续运行键!\r\n  " +
                    "2.请停止运行，更新最新视觉文件和模板后，再按运行键!",

                    "Tip: Please confirm the visual file and template, please confirm!\r\n" +
                    "1. If ignored, please press the continue key!\r\n" +
                    "2. Please stop running, update the latest visual files and templates, and then press the run button!",

                    "Mẹo: Vui lòng xác nhận tệp trực quan và mẫu, vui lòng xác nhận!\r\n" +
                    "1. Nếu bị bỏ qua, vui lòng nhấn phím tiếp tục!\r\n" +
                    "2. Vui lòng ngừng chạy, cập nhật tệp trực quan và mẫu mới nhất, sau đó nhấn nút chạy!");
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "拍照异常!" : "PhotoErr", 20, true);
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                if (DialogResult.Cancel == logres)
                {
                    return EM_RES.ERR;
                }
                else
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行!" : "RUN", 0, true);
                left = 0;
                right = 0;
                up = 0;
                down = 0;
            }
            else 
            {
                left = 0;
                right = 0;
                up =0; ;
                down = 0;

                left = Convert.ToDouble(data[0]);
                right = Convert.ToDouble(data[1]);
                if (data.Length > 2)
                    up = Convert.ToDouble(data[2]);
                if (data.Length > 3)
                    down = Convert.ToDouble(data[3]);
            }
            return EM_RES.OK;
        }

       private EM_RES CheckWsPutOkShow(Cam.VisionTask task,ST_XYN WsTriPos)
        {
            EM_RES res = EM_RES.OK;
            int posid = WsTriPos.n + 1;
            var bPosOk = CheckArea(task, posid,out var ngtype);
            if (bPosOk == false)
            {
            RECAM:
                res = MT.ZupMove(ref bquit, ref ax_x, WsTriPos.x, ref ax_y, WsTriPos.y);
                if (res != EM_RES.OK) return res;
                res = upcam.FindTaskTriAndWait(CONST.WsModUpFw, Demo);
                if (res != EM_RES.OK)
                {
                    if (res != EM_RES.CAM_ERR) return res;
                }
                else
                {
                    //放偏确认
                    bPosOk = CheckArea(upcam.curTask, posid, out  ngtype);
                }
                if (!bPosOk && PT_SET.bEnVsFB)
                {
                    upcam.SaveOriginImage(upcam.curTask.Image, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, upcam.mName), string.Format("{0}{1}.jpg",
                                      DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "物料放偏!" : "Place deflected", 20, true, ErrCode: ShowErrMsg.WsPutDivErr);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    string ngString = ngtype == 1 ? "XY偏移超限制" : ngtype == 2 ? "角度超限制" : ngtype == 3 ? "面积检测超标" : "放偏检测拍照失败";
                    if (PT_SET.bBackerrcontinue)
                    {
                        warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "tiếp tục chạy");
                        warn.abort_txt = MultiLanguage.TxtSelct("拍照确认", "Take a photo", "Xác nhận ảnh");
                        warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "ngừng chạy");
                        warn.ws = null;//增加语言
                        warn.title = MultiLanguage.TxtSelct("提示:物料放偏", "Tip: Material is biased", "Mẹo: bù đắp vật liệu");
                        warn.msg = MultiLanguage.TxtSelct("当前工站有物料放偏!", "There is material deviation in the current station!", "Có sự sai lệch vật liệu ở trạm hiện tại");
                        warn.lb_msg = MultiLanguage.TxtSelct(
                           $"提示:{disc}工站位置{posid}放偏检测异常-{ngString}，请确认!\r\n" +
                            "1.如果没有物料放偏,请按继续运行键!\r\n" +
                            "2.如有物料放偏请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!",

                            "Tip:There is material deviation in the current station, please confirm! \r\n " +
                            "1. If there is no material deviation, please press the continue running key! \r\n" +
                            "2. If the material is biased, please press the stop running button to exit the operation, and then press the run button after the upper left corner of the interface is ready!",

                            "Mẹo: Có sự sai lệch vật liệu tại trạm hiện tại, vui lòng xác nhận!\r\n" +
                            "1. Nếu không có sai lệch vật liệu, vui lòng nhấn nút tiếp tục!\r\n" +
                            "2. Nếu vật liệu không được căn chỉnh, vui lòng nhấn nút dừng hoạt động để thoát khỏi hoạt động, sau đó nhấn nút vận hành sau khi góc trên bên trái của giao diện hiển thị đã sẵn sàng!");
                        DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                        if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                        else if (DialogResult.Abort == logres)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                            MT.GPIO_OUT_KL_START.SetOn();
                            MT.GPIO_OUT_KL_RESET.SetOff();
                            MT.GPIO_OUT_KL_STOP.SetOff();
                            goto RECAM;
                        }
                        else
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                            MT.GPIO_OUT_KL_START.SetOn();
                            MT.GPIO_OUT_KL_RESET.SetOff();
                            MT.GPIO_OUT_KL_STOP.SetOff();
                        }
                    }
                    else
                    {
                        //warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                        warn.ok_txt = MultiLanguage.TxtSelct("拍照确认", "Take a photo", "Xác nhận ảnh");
                        warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "ngừng chạy");
                        warn.ws = null;//增加语言
                        warn.title = MultiLanguage.TxtSelct("提示:物料放偏", "Tip: Material is biased", "Mẹo: bù đắp vật liệu");
                        warn.msg = MultiLanguage.TxtSelct("当前工站有物料放偏!", "There is material deviation in the current station!", "Có sự sai lệch vật liệu ở trạm hiện tại");
                        warn.lb_msg = MultiLanguage.TxtSelct(
                                $"提示:{disc}工站位置{posid}放偏检测异常-{ngString}，请确认!\r\n" +
                            "1.如果没有物料放偏,请按继续运行键!\r\n" +
                            "2.如有物料放偏请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!",

                            "Tip:There is material deviation in the current station, please confirm! \r\n " +
                            "1. If there is no material deviation, please press the continue running key! \r\n" +
                            "2. If the material is biased, please press the stop running button to exit the operation, and then press the run button after the upper left corner of the interface is ready!",

                            "Mẹo: Có sự sai lệch vật liệu tại trạm hiện tại, vui lòng xác nhận!\r\n" +
                            "1. Nếu không có sai lệch vật liệu, vui lòng nhấn nút tiếp tục!\r\n" +
                            "2. Nếu vật liệu không được căn chỉnh, vui lòng nhấn nút dừng hoạt động để thoát khỏi hoạt động, sau đó nhấn nút vận hành sau khi góc trên bên trái của giao diện hiển thị đã sẵn sàng!");
                        DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                        if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                        else if (DialogResult.OK == logres)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                            goto RECAM;
                        }

                    }

                }
                return EM_RES.OK;
            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 飞拍移动到料盘
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="traybox"></param>
        /// <returns></returns>
        public EM_RES FlyCamToTray(ref bool bquit, WS ws, TrayBox traybox, ref ST_XYZA endpos, bool dir = true)
        {
            List<ST_XYN> WsTriPos = new List<ST_XYN>();
            List<ST_XYN> TrayTriPos = new List<ST_XYN>();
            List<ST_XYN> TrayCapQrcodeTriPos = new List<ST_XYN>();
            List<ST_XYN> WsModQrcodeTriPos = new List<ST_XYN>();
            List<ST_XYN> DownQrcodeTriPos = new List<ST_XYN>();
            List<Cam.VisionTask> List_vs_task_temp = new List<Cam.VisionTask>();
            List<Cam.VisionTask> List_vs_task_DwTemp = new List<Cam.VisionTask>();
            bool bQcodeChkByPhoto = PT_SET.bAddCapQrcode && PT_SET.bBarcodeCamBackEn;
            VisionOutPutData BackCamdata1 = new VisionOutPutData();
            VisionOutPutData BackCamdata2 = new VisionOutPutData();
            int xt_cnt = 0;
            //ST_XYZA endpos=new ST_XYZA();
            EM_RES res = EM_RES.OK;
            //配置工站回拍
            if (PT_SET.bDwAddCapQrcode)
            {
                if (FeedBackWs.Count > 0)
                {
                    foreach (WS.MdDat md in FeedBackWs)
                    {
                        XT xt;
                        if (md.Num > 8)
                        {
                            xt = list_xt[0];
                        }
                        else
                        {
                            xt = list_xt[1];
                        }
                        res = UpCamBackCheck(ref bquit, ref ws, xt, id, new ST_XY(FeedBackPos.x, md.st_pos[id].y), ref BackCamdata1);
                        if (res != EM_RES.OK)
                        {
                            return res;
                        }
                    }
                    if (PT_SET.Check2open)
                    {
                        foreach (WS.MdDat md in FeedBackWs)
                        {
                            XT xt;
                            if (md.Num > 8)
                            {
                                xt = list_xt[0];
                            }
                            else
                            {
                                xt = list_xt[1];
                            }
                            res = UpCamBackCheck2(ref bquit, ref ws, xt, id, new ST_XY(FeedBackPos.x, md.st_pos[id].y), ref BackCamdata2);
                            if (res != EM_RES.OK)
                            {
                                return res;
                            }
                        }
                    }

                }
            }
            else
            {
                if (((bUpdateFBPos_OKNG && traybox.disc != traybox_fd.disc) || (bUpdateFBPos_UT && traybox.disc == traybox_fd.disc)) && FeedBackWs.Count > 0)// && ax_y.fenc_pos > list_xt[0].st_cap_pos.y)
                {
                    foreach (WS.MdDat md in FeedBackWs)
                    {
                        WsTriPos.Add(new ST_XYN(FeedBackPos.x, md.st_pos[id].y, md.Num - 1));
                    }

                    if (Math.Abs(FeedBackPos.x - ax_x.fenc_pos) > 2 || Math.Abs(FeedBackPos.y - ax_y.fenc_pos) > 5)
                    {

                        res = MT.ZupMove(ref bquit, ref ax_x, FeedBackPos.x, ref ax_y, FeedBackPos.y);

                        //ax_y.SetToWorkSpd();

                        if (res != EM_RES.OK) return res;
                    }

                    dir = false;
                }
            }


            //配置飞拍结束位置
            if (traybox.disc == traybox_fd.disc)
            {
                res = FindFdTrayPos(ref endpos, ref TrayTriPos, ref TrayCapQrcodeTriPos, ws, dir);
            }
            else
            {
                res = FindOkOrNgTrayPos(ref endpos, traybox, ws);

            }
            if (bQcodeChkByPhoto)
            {
                WsModQrcodeTriPos.Clear();
                foreach (WS.MdDat md in FeedBackWs)
                {
                    WsModQrcodeTriPos.Add(new ST_XYN(FeedBackPos.x, md.st_CapQrcodePos[id].y, md.Num - 1));
                }
            }

            //if (PT_SET.OpenDownQrde && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN)//二维码回检绑定检查
            //{
            //    List<double> xt_ofs = new List<double>();
            //    xt_ofs.Add(list_xt[0].xt_cap_ofs);
            //    xt_ofs.Add(list_xt[1].xt_cap_ofs);
            //    bool Ofs_On = false;
            //    if (PT_SET.DownDownQrde)
            //    {
            //        var dwCapPos0 = Ofs_On ? list_xt[0].st_cap_pos.y - xt_ofs[0] : list_xt[0].st_cap_pos.y;
            //        var dwCapPos1 = Ofs_On ? list_xt[1].st_cap_pos.y - xt_ofs[1] : list_xt[1].st_cap_pos.y;
            //        DownQrcodeTriPos.Clear();
            //        DownQrcodeTriPos.Add(new ST_XYN(FeedBackPos.x, dwCapPos0, 1));
            //        DownQrcodeTriPos.Add(new ST_XYN(FeedBackPos.x, dwCapPos1, 2));
            //    }
            //}

            if (res != EM_RES.ABORT && res != EM_RES.OK) return res;
            else if (res == EM_RES.ABORT) return EM_RES.OK;

            if (ax_y.fenc_pos > endpos.y && dir && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn && traybox.disc != traybox_fd.disc)
            {
                dir = false;
            }

            //加载触发点并定位
            res = FLyToTray(ref bquit, endpos, WsTriPos, TrayTriPos, TrayCapQrcodeTriPos, WsModQrcodeTriPos, DownQrcodeTriPos, traybox, dir);
            if (res != EM_RES.OK) return res;

            //等待结果
            bool barcode_err = false;
            var triPosCnt = WsTriPos.Count + TrayTriPos.Count;
            if (PT_SET.bAddCapQrcode && TrayTriPos.Count != 0) triPosCnt += TrayTriPos.Count;
            if (bQcodeChkByPhoto && WsTriPos.Count != 0) triPosCnt += WsTriPos.Count;

            if (PT_SET.bDwAddCapQrcode)
            {

                triPosCnt -= WsTriPos.Count;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("结果校验数量triPosCnt{0}", triPosCnt));
            }
            else
            {
                WaitCamRes(upcam, triPosCnt);
                if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bBarcodeCamBackEn)
                {
                    foreach (XT xt in list_xt)
                    {
                        if ((xt.cy_zk.isONByChkSen && xt.XtMd != null && xt.XtMd.res >= 0 && !Demo) || (Demo && xt.XtMd != null))
                        {
                            xt_cnt++;
                            //List_vs_task_DwTemp.Add(dwcam.List_vs_task_cur[list_xt.IndexOf(xt)]);
                        }
                    }
                    if (xt_cnt > 0)
                    {
                        WaitCamRes(dwcam, xt_cnt);
                        //复制任务
                        for (int i = 0; xt_cnt > 0 && i < xt_cnt; i++)
                        {
                            List_vs_task_DwTemp.Add(dwcam.List_vs_task_cur[i]);
                        }

                        if (!Demo)
                        {
                            //检查模组二维码             
                            foreach (Cam.VisionTask tsk in List_vs_task_DwTemp)
                            {
                                if (list_xt[tsk.TriPos.n].XtMd.res == 0 &&
                                    (tsk.ResData.BarCode == null || tsk.ResData.BarCode.Length < 1 ||
                                     tsk.ResData.BarCode != list_xt[tsk.TriPos.n].XtMd.bardcode))
                                {
                                    barcode_err = true;
RECAP:
                                    res = MT.ZupMove(ref bquit, ref ax_y, tsk.TriPos.y);
                                    if (res != EM_RES.OK) return res;
                                    res = dwcam.FindTaskTriAndWait(CONST.ModDwFw[tsk.TriPos.n], Demo);
                                    if (res != EM_RES.OK)
                                    {
                                        if (res != EM_RES.CAM_ERR) return res;
                                    }

                                    if (res == EM_RES.OK &&
                                        (dwcam.curTask.ResData.BarCode == null || dwcam.curTask.ResData.BarCode.Length < 1))
                                    {
                                        if (PT_SET.bBackerrcontinue)
                                        {
                                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "没检到二维码!" : "No Barcode", 20, true);
                                            MT.ST_WARN st_warn1 = new MT.ST_WARN();
                                            warning fr_warn1 = new warning();//增加语言
                                            st_warn1.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                                            st_warn1.abort_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                                            st_warn1.ws = null;
                                            st_warn1.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                                            st_warn1.title = MultiLanguage.TxtSelct("提示:没检到二维码", "Tip: No barcode detected", "Tip: No barcode detected");
                                            st_warn1.msg = MultiLanguage.TxtSelct(
                                                "下相机" + list_xt[tsk.TriPos.n].disc + "上模组二维码无法识别.",
                                                $"Dwcam" + list_xt[tsk.TriPos.n].disc + "module barcode cannot be identified",
                                                $"Không thể xác định mã vạch mô-đun Dwcam" + list_xt[tsk.TriPos.n].disc);
                                            st_warn1.lb_msg = MultiLanguage.TxtSelct(
                                                "提示:" + st_warn1.msg + "请确认!\r\n" +
                                                "1.按'继续运行'键则放弃当前检查继续运行!\r\n" +
                                                "2.如需确认问题请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!\r\n " +
                                                "3.如按重新拍照键则重新获取当前位置图像!",

                                                "Tip:" + st_warn1.msg + "\r\nPlease confirm!\r\n " +
                                                "1.Press the 'Continue Running' key to abandon the current check and continue running!\r\n" +
                                                "2.If you need to confirm the problem, please press the stop running button to exit the operation, and then press the run button after the top left corner of the interface is ready!\r\n" +
                                                "3.If you press the 'Take a photo' button, the current position image will be obtained again!",

                                                "Mẹo:" + st_warn1.msg + "\r\nVui lòng xác nhận!\r\n " +
                                                "1.Nhấn phím 'Tiếp tục Chạy' để bỏ kiểm tra hiện tại và tiếp tục chạy!\r\n" +
                                                "2.Nếu bạn cần xác nhận sự cố, vui lòng nhấn nút dừng chạy để thoát khỏi hoạt động, sau đó nhấn nút chạy sau khi góc trên cùng bên trái của giao diện đã sẵn sàng!\r\n" +
                                                "3.Nếu bạn nhấn nút 'Chụp ảnh', hình ảnh vị trí hiện tại sẽ được lấy lại!");
                                            DialogResult logres1 = MT.Display_frwarn(fr_warn1, st_warn1, ERR_ALM.EmErrItem.BarcodeAbnormal);
                                            if (DialogResult.Cancel == logres1) return EM_RES.ERR;

                                            else if (DialogResult.Abort == logres1)
                                            {
                                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                                goto RECAP;
                                            }
                                            else
                                            {
                                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                                goto RECAP;
                                            }
                                        }
                                        else
                                        {
                                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "没检到二维码!" : "No Barcode", 20, true);
                                            MT.ST_WARN st_warn1 = new MT.ST_WARN();
                                            warning fr_warn1 = new warning();
                                            //st_warn1.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                                            st_warn1.ok_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                                            st_warn1.ws = null;
                                            st_warn1.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy"); // VAR.IsChinese ? "停止运行" : "Stop running";
                                            st_warn1.title = MultiLanguage.TxtSelct("提示:没检到二维码", "Tip: No barcode detected", "Tip: No barcode detected");
                                            st_warn1.msg = MultiLanguage.TxtSelct(
                                                "下相机" + list_xt[tsk.TriPos.n].disc + "上模组二维码无法识别.",
                                                $"Dwcam" + list_xt[tsk.TriPos.n].disc + "module barcode cannot be identified",
                                                $"Không thể xác định mã vạch mô-đun Dwcam" + list_xt[tsk.TriPos.n].disc);
                                            st_warn1.lb_msg = MultiLanguage.TxtSelct(
                                                "提示:" + st_warn1.msg + "请确认!\r\n" +
                                                "1.按'继续运行'键则放弃当前检查继续运行!\r\n" +
                                                "2.如需确认问题请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!\r\n " +
                                                "3.如按重新拍照键则重新获取当前位置图像!",

                                                "Tip:" + st_warn1.msg + "\r\nPlease confirm!\r\n " +
                                                "1.Press the 'Continue Running' key to abandon the current check and continue running!\r\n" +
                                                "2.If you need to confirm the problem, please press the stop running button to exit the operation, and then press the run button after the top left corner of the interface is ready!\r\n" +
                                                "3.If you press the 'Take a photo' button, the current position image will be obtained again!",

                                                "Mẹo:" + st_warn1.msg + "\r\nVui lòng xác nhận!\r\n " +
                                                "1.Nhấn phím 'Tiếp tục Chạy' để bỏ kiểm tra hiện tại và tiếp tục chạy!\r\n" +
                                                "2.Nếu bạn cần xác nhận sự cố, vui lòng nhấn nút dừng chạy để thoát khỏi hoạt động, sau đó nhấn nút chạy sau khi góc trên cùng bên trái của giao diện đã sẵn sàng!\r\n" +
                                                "3.Nếu bạn nhấn nút 'Chụp ảnh', hình ảnh vị trí hiện tại sẽ được lấy lại!");
                                            DialogResult logres1 = MT.Display_frwarn(fr_warn1, st_warn1, ERR_ALM.EmErrItem.BarcodeAbnormal);
                                            if (DialogResult.Cancel == logres1) return EM_RES.ERR;

                                            if (DialogResult.OK == logres1)
                                            {
                                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                                goto RECAP;
                                            }
                                        }
                                    }

                                    //下料前进行二维码防呆
                                    if (PT_SET.OpenDownQrde && PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN)
                                    {
                                        String str1, str2, str3, str4, str5;
                                        str2 = "当前工站位置：" + ws.num + "回检二维码：" + tsk.ResData.BarCode + "记录二维码：" + list_xt[tsk.TriPos.n].XtMd.bardcode;
                                        Utility.WriteStrToCSV("", str2);
                                        bool bAlmin = false;
                                        if (PT_SET.DownDownQrde)
                                        {
                                            if (dwcam.curTask.ResData.BarCode != null && dwcam.curTask.ResData.BarCode.Length > 1 &&
                                          dwcam.curTask.ResData.BarCode != list_xt[tsk.TriPos.n].XtMd.bardcode && !Demo)
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
                                             VAR.IsChinese ? string.Format("下相机->{0}回检二维码不一致,实拍Barcode:{1},记录Barcode:{2},记录二维码结果0改为111",
                                                 list_xt[tsk.TriPos.n].disc, tsk.ResData.BarCode,
                                                 list_xt[tsk.TriPos.n].XtMd.bardcode) : string.Format("DwCam{0} check back barcode is inconsistent,new barcode:{1},old barcode:{2},recording the barcode results as 111.           (下相机->{0}回检二维码不一致,实拍Barcode:{1},记录Barcode:{2},记录二维码结果0改为111)",
                                                 list_xt[tsk.TriPos.n].disc, tsk.ResData.BarCode,
                                                 list_xt[tsk.TriPos.n].XtMd.bardcode))
                                         ;
                                                list_xt[tsk.TriPos.n].XtMd.res = 111;
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料前检测二维码未通过:" + list_xt[tsk.TriPos.n].XtMd.bardcode);

                                                MT.ST_WARN st_warn = new MT.ST_WARN();
                                                warning fr_warn = new warning();
                                                st_warn.ok_txt = VAR.IsChinese ? "确定" : "Give up";
                                                st_warn.abort_txt = VAR.IsChinese ? "确定" : "Try again";
                                                st_warn.ws = ws;
                                                st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                                                st_warn.title = VAR.IsChinese ? "提示:下料前检测二维码未通过!" : "Tip: Abnormal suction!";
                                                st_warn.msg = string.Format("下相机->{0}回检二维码不一致,实拍Barcode:{1},记录Barcode:{2},记录二维码结果0改为111",
                                                 list_xt[tsk.TriPos.n].disc, tsk.ResData.BarCode,
                                                 list_xt[tsk.TriPos.n].XtMd.bardcode);
                                                st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.点击确定将继续运行!\r\n  " +
                                                    "2.点击停止将停止运行!\r\n  ";

                                                DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                                                if (DialogResult.Cancel == logres)
                                                {
                                                    return EM_RES.ERR;
                                                }
                                            }
                                            else
                                            {
                                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料前检测二维码通过:" + list_xt[tsk.TriPos.n].XtMd.bardcode);
                                            }
                                        }
                                    }


                                    //{
                                    //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
                                    //        VAR.IsChinese ? string.Format("下相机->{0}回检二维码不一致,实拍Barcode:{1},记录Barcode:{2},记录二维码结果0改为111",
                                    //            list_xt[tsk.TriPos.n].disc, tsk.ResData.BarCode,
                                    //            list_xt[tsk.TriPos.n].XtMd.bardcode) : string.Format("DwCam{0} check back barcode is inconsistent,new barcode:{1},old barcode:{2},recording the barcode results as 111.           (下相机->{0}回检二维码不一致,实拍Barcode:{1},记录Barcode:{2},记录二维码结果0改为111)",
                                    //            list_xt[tsk.TriPos.n].disc, tsk.ResData.BarCode,
                                    //            list_xt[tsk.TriPos.n].XtMd.bardcode))
                                    //    ;
                                    //    list_xt[tsk.TriPos.n].XtMd.res = 111;
                                    //}
                                }
                            }
                        }
                    }
                }


                //检查结果 
                res = ChkCamRes(upcam, triPosCnt);
                if (res != EM_RES.OK) return res;
                //回拍结果判断
                modupcam_task.Clear();
                bool mtofs = false;
                if (!Demo)
                {
                    int wsTriCnt = WsTriPos.Count;
                    if (bQcodeChkByPhoto) wsTriCnt = wsTriCnt * 2;
                    //复制任务
                    for (int i = 0; wsTriCnt > 0 && i < wsTriCnt; i++)
                    {
                        List_vs_task_temp.Add(upcam.List_vs_task_cur[i]);
                    }

                    //寻找上相机拍模组数据
                    if (TrayTriPos.Count > 0)
                    {
                        foreach (Cam.VisionTask task in upcam.List_vs_task_cur)
                        {
                            if (task.TaskName.Contains(CONST.ModUpFw) || task.TaskName.Contains(CONST.FindQrCodeFw))
                                modupcam_task.Add(task);
                        }

                        if (modupcam_task.Count > 1)
                        {
                            Cam.VisionTask task1 = modupcam_task.Find(s => s.TaskName.Equals("ModUp_Shp"));
                            Cam.VisionTask task2 = modupcam_task.Find(s => s.TaskName.Equals("ModUp_Shp1"));
                            if (task1 != null && task2 != null && task1.ResData.TimeStamp > task2.ResData.TimeStamp)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} photograph (ModUp_Shp) TimeStamp err!ModUp_Shp:{2},ModUp_Shp1:{3}           ({1}拍模组ModUp_Shp时间戳有误!ModUp_Shp:{2},ModUp_Shp1:{3})", englishdisc, disc, task1.ResData.TimeStamp, task2.ResData.TimeStamp), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id);
                                task1.ResData.bOK = false;
                                task2.ResData.bOK = false;
                            }
                        }
                    }

                    for (int i = 0; WsTriPos.Count > 0 && i < WsTriPos.Count; i++)
                    {
                        Cam.VisionTask task = List_vs_task_temp[i];
                        Cam.VisionTask qrcodeTask = null;
                        if (bQcodeChkByPhoto)//单独扫码位置
                        {
                            if (task.TaskName != CONST.WsModUpFw) continue;

                            if (i % 2 == 0)
                            {
                                qrcodeTask = List_vs_task_temp[i + 1];
                            }
                            else
                            {
                                qrcodeTask = List_vs_task_temp[i - 1];
                            }
                            if (qrcodeTask.TaskName != CONST.WsModQrcode_Shp) return EM_RES.ERR;
                        }
                        else
                        {
                            qrcodeTask = task;
                        }
                        //二维码回检
                        res = QrCodeChkShow(qrcodeTask, ws.list_md[WsTriPos[i].n]);
                        if (res != EM_RES.OK && res != EM_RES.RETRY) return res;

                        //放偏回检确认
                        res = CheckWsPutOkShow(task, WsTriPos[i]);
                        if (res != EM_RES.OK) return res;
                    }

                }
            }

            //清空相机数据
            if (!upcam.FlushOk && !upcam.FlushUpdate)
            {
                 Task tak = new Task(() =>
                {
                    upcam.FlushUpdate = true;
                    upcam.mAcqFifo.Flush();
                    upcam.FlushOk = true;
                    upcam.FlushUpdate = false;
                });
                tak.Start();
            }

            if (!dwcam.FlushOk && !dwcam.FlushUpdate)
            {
                Task takdw = new Task(() =>
                {
                    dwcam.FlushUpdate = true;
                    dwcam.mAcqFifo.Flush();
                    dwcam.FlushOk = true;
                    dwcam.FlushUpdate = false;
                });
                takdw.Start();
            }
            FeedBackWs.Clear();
            if (barcode_err && traybox.disc == traybox_ok.disc) return EM_RES.ABORT;
            return EM_RES.OK;
        }
        public uint CurLightSet=0;
        //相机参数设置
        public void Camcfgset(bool istray, bool bJig = false)
        {
            if (!PT_SET.BCamcfgset) return;
                Cam curCam=upcam;
                if (curCam == null) return;
            int i = id;
    
                try
                {
                    Cam.ST_CAP_CFG cap_cfg = curCam.curCapCfg;
                    double exposure = istray ? PT_SET.TrayExposure[i] : PT_SET.WsExposure[i];
                    if (bJig) exposure = PT_SET.JigExposure[i];
                    if (curCam != null && exposure > 0 && exposure < 1000)
                    {
                        cap_cfg.Exposure = exposure;
                    }
                    else MessageBox.Show(VAR.IsChinese ? "曝光参数数据异常!\r\n请检查数据格式及大小范围!" : "The exposure parameter data is abnormal! \r\nPlease check the data format and size range!");
                    double Brightness = istray ? PT_SET.TrayBrightness[i] : PT_SET.WsBrightness[i];
                    if (bJig) Brightness = PT_SET.JigBrightness[i];
                    if (curCam != null && Brightness >= 0 && Brightness < 1)
                    {
                        cap_cfg.Brightness = Brightness;
                    }
                    else MessageBox.Show(VAR.IsChinese ? "亮度参数数据异常!\r\n请检查数据格式及大小范围!" : "The brightness parameter data is abnormal! \r\nPlease check the data format and size range!");

                    double Contrast = istray ? PT_SET.TrayContrast[i] : PT_SET.WsContrast[i];
                    if (bJig) Contrast = PT_SET.JigContrast[i];
                    if (curCam != null && Contrast >= 0 && Contrast < 1)
                    {
                        cap_cfg.Contrast = Contrast;
                    }
                    else MessageBox.Show(VAR.IsChinese ? "对比参数数据异常!\r\n请检查数据格式及大小范围!" : "The comparison parameter data is abnormal! \r\nPlease check the data format and size range!");
                    //save
                    if (curCam.isLive) curCam.liveCapCfg = cap_cfg;
                    else curCam.curCapCfg = cap_cfg;
                    curCam.SaveCfg();
                    //MessageBox.Show(VAR.IsChinese ? "参数保存完成！" : "Parameters are saved!\r\n参数保存完成！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "参数数据异常!\r\n请检查数据格式及大小范围!" : "Parameter data is abnormal!\r\nPlease check the data format and size range!\r\n参数数据异常!\r\n请检查数据格式及大小范围!", ex.Message);
                }
            

        }

        public void Camcfgset(uint idx=1)
        {
            if (!PT_SET.BCamcfgset) return;
            Cam curCam = upcam;
            if (curCam == null) return;
            int i = id;
            if(CurLightSet== idx) return;
            try
            {
                CurLightSet = idx;
                Cam.ST_CAP_CFG cap_cfg = curCam.curCapCfg;
                double exposure= idx == 0 ? PT_SET.TrayExposure[i] : idx == 1 ? PT_SET.WsExposure[i]: PT_SET.JigExposure[i];
                if (curCam != null && exposure > 0 && exposure < 1000)
                {
                    cap_cfg.Exposure = exposure;
                }
                else MessageBox.Show(VAR.IsChinese ? "曝光参数数据异常!\r\n请检查数据格式及大小范围!" : "The exposure parameter data is abnormal! \r\nPlease check the data format and size range!");
                double Brightness = idx == 0 ? PT_SET.TrayBrightness[i] : idx == 1 ? PT_SET.WsBrightness[i] : PT_SET.JigBrightness[i];
                if (curCam != null && Brightness >= 0 && Brightness < 1)
                {
                    cap_cfg.Brightness = Brightness;
                }
                else MessageBox.Show(VAR.IsChinese ? "亮度参数数据异常!\r\n请检查数据格式及大小范围!" : "The brightness parameter data is abnormal! \r\nPlease check the data format and size range!");
                double Contrast = idx == 0 ? PT_SET.TrayContrast[i] : idx == 1 ? PT_SET.WsContrast[i] : PT_SET.JigContrast[i];
                if (curCam != null && Contrast >= 0 && Contrast < 1)
                {
                    cap_cfg.Contrast = Contrast;
                }
                else MessageBox.Show(VAR.IsChinese ? "对比参数数据异常!\r\n请检查数据格式及大小范围!" : "The comparison parameter data is abnormal! \r\nPlease check the data format and size range!");
                //save
                if (curCam.isLive) curCam.liveCapCfg = cap_cfg;
                else curCam.curCapCfg = cap_cfg;
                curCam.SaveCfg();
                //MessageBox.Show(VAR.IsChinese ? "参数保存完成！" : "Parameters are saved!\r\n参数保存完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(VAR.IsChinese ? "参数数据异常!\r\n请检查数据格式及大小范围!" : "Parameter data is abnormal!\r\nPlease check the data format and size range!\r\n参数数据异常!\r\n请检查数据格式及大小范围!", ex.Message);
            }


        }
        #endregion

        #region GRR测试流程
        public EM_RES GRRFlow(ref bool bquit, ref WS ws, UpDownLoad ud, bool IsDemo = false)
        {
            EM_RES res = EM_RES.OK;
            List<WS.MdDat> WsPickmd = new List<WS.MdDat>();
            Product.MdDat xt_md = new Product.MdDat();
            ST_XY xtdwvs = new ST_XY();
            ST_XYZA pickpos = new ST_XYZA();
            List<Cam.VisionOutPutData> dwData = new List<Cam.VisionOutPutData>();
            bool Isfinal = false;
            while (!bquit && !VAR.gsys_set.bquit)
            {
                //取料
                if (GrrProc == EM_STA.PICKWS)
                {
                    foreach (XT xt in ud.list_xt)
                    {
                        if (((!xt.cy_zk.isONByChkSen && !IsDemo) || IsDemo) && xt.XtMd == null)
                        {
                        NEXT:
                            WsPickmd.Clear();
                            GetWsOrderPosTeam(ref WsPickmd, ws, Product.EM_CM_RES.OK);
                            if (WsPickmd.Count > 0)
                            {
                                xtdwvs = xt.st_vs_pos_xtshp.ToXY();
                                pickpos = WsPickmd[0].st_pickpos[id].ToXYZA() + xt.st_rol_cap.ToXYZA() -
                                          list_xt[0].st_rol_cap.ToXYZA() + xtdwvs.ToXYZA();
                                pickpos.z = xt.id == 0 ? pickpos.z : -pickpos.z;
                                pickpos.a = 0;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
                                    VAR.IsChinese ? string.Format("{0}工站取料: X:{1} Y:{2}", xt.disc, pickpos.x, pickpos.y) : string.Format("{0},WS Pickmod:X:{1} Y:{2}      ({0}工站取料: X:{1} Y:{2})", xt.disc, pickpos.x, pickpos.y));
                            REPICK:
                                res = xt.Pick(ref bquit, pickpos, true, IsDemo, true);
                                if (res == EM_RES.OK)
                                {
                                    xt_md.res = WsPickmd[0].res;
                                    xt_md.bardcode = WsPickmd[0].bardcode;
                                    xt_md.motor_barcode = WsPickmd[0].motor_barcode;
                                    xt_md.Num = WsPickmd[0].Num;
                                    xt_md.WS_ID = WsPickmd[0].WS_ID;
                                    xt_md.PC_ID = WsPickmd[0].PC_ID;
                                    xt.XtMd = xt_md.Clone();
                                    WsPickmd[0].res = -2;
                                    WsPickmd[0].bardcode = null;
                                    WsPickmd[0].motor_barcode = null;
                                }
                                else if (res == EM_RES.PICK_ERR)
                                {
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "吸料异常!" : "Draw ERR", 20, true);
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "工站吸料异常", DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.UpDownLoad + id);
                                    MT.ST_WARN st_warn = new MT.ST_WARN();//增加语言
                                    warning fr_warn = new warning();
                                    st_warn.ok_txt = MultiLanguage.TxtSelct("放弃吸取", "Give up", "Từ bỏ");
                                    st_warn.abort_txt = MultiLanguage.TxtSelct("重新吸取", "Try again", "Thử lại");
                                    st_warn.ws = ws;
                                    st_warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                                    st_warn.title = MultiLanguage.TxtSelct("提示:吸料异常", "Tip: Abnormal suction", "Mẹo: Hút bất thường");
                                    st_warn.msg = MultiLanguage.TxtSelct(
                                        $"{disc}下料失败,{xt.disc}无法吸起当前工站第{WsPickmd[0].Num}模组!",
                                        $"{englishdisc} download filed,{xt.disc} can't suck up module {WsPickmd[0].Num} of the current station!",
                                        $"{englishdisc} tải xuống đã nộp,{xt.disc} không thể hút hết mô-đun {WsPickmd[0].Num} của nhà ga hiện tại!");
                                    st_warn.lb_msg = MultiLanguage.TxtSelct(
                                        "提示: " + st_warn.msg + "请确认!\r\n  1.如果工站上没有物料, 请按放弃吸取键继续吸下一个物料!\r\n" +
                                        "2.如有物料请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!\r\n" +
                                        "3.如按重新吸取键则吸嘴再次吸取!",

                                        "Tip: " + st_warn.msg + "\r\n Please confirm!\r\n" +
                                        "1.If there is no material on the station, please press the Give up button to continue sucking the next material!\r\n" +
                                        "2.If there is any material, please press the stop running button to exit the operation. " +
                                        "After the ready is displayed in the upper left corner of the interface, press the run button again!\r\n" +
                                        "3.If the 'Try again' button is pressed, the suction nozzle sucks again!",

                                        "Mẹo: " + st_warn.msg + "\r\n Vui lòng xác nhận!\r\n" +
                                        "1.Nếu không có nguyên liệu trên ga, vui lòng nhấn nút Bỏ để tiếp tục hút nguyên liệu tiếp theo! \r\n" +
                                        "2.Nếu có bất kỳ tài liệu nào, vui lòng nhấn nút dừng chạy để thoát khỏi hoạt động." +
                                        "Sau khi sẵn sàng hiển thị ở góc trên bên trái của giao diện, hãy nhấn lại nút chạy!\r\n" +
                                        "3.Nếu nhấn nút 'Thử lại', vòi hút sẽ hút lại!");
                                    DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                                    if (DialogResult.Cancel == logres) return EM_RES.ERR;
                                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                    if (DialogResult.OK == logres)
                                    {
                                        WsPickmd[0].res = -2;
                                        WsPickmd[0].bardcode = null;
                                        WsPickmd[0].motor_barcode = null;
                                    }
                                    else if (DialogResult.Abort == logres)
                                    {
                                        goto REPICK;
                                    }
                                    else if (DialogResult.Retry == logres)
                                    {
                                        goto NEXT;
                                    }


                                }
                                else if (res != EM_RES.ABORT) return res;
                            }
                        }
                    }

                    GrrProc = EM_STA.GRR_CAM;
                }

                if (GrrProc == EM_STA.GRR_CAM)
                {
                    //确认是否有可取模组
                    WsPickmd.Clear();
                    GetWsOrderPosTeam(ref WsPickmd, ws, Product.EM_CM_RES.OK);
                    if (WsPickmd.Count == 0) Isfinal = true;

                    //拍照
                    dwData.Clear();
                    for (int i = 0; i < ud.list_xt.Count; i++)
                    {
                        XT xt = ud.list_xt[(i + 1) % 2];
                        if (((xt.cy_zk.isONByChkSen && !IsDemo) || IsDemo) && xt.XtMd != null &&
                            (Isfinal || (!Isfinal && xt.id == 1)))
                        {
                            res = MT.ZupMove(ref bquit, ref xt.ax_y, xt.st_cap_pos.y, ref xt.ax_u, 0, true);
                            if (res != EM_RES.OK) return res;
                            Thread.Sleep(50);
                            res = dwcam.FindTaskTriAndWait(CONST.ModDwFw[xt.id], IsDemo);
                            if (res != EM_RES.OK) return res;
                            dwData.Add(dwcam.curTask.ResData);
                        }
                    }

                    GrrProc = EM_STA.PLACEWS;
                }

                //放料
                if (GrrProc == EM_STA.PLACEWS)
                {
                    for (int i = 0; i < ud.list_xt.Count; i++)
                    {
                        XT xt = ud.list_xt[(i + 1) % 2];
                        if (((xt.cy_zk.isONByChkSen && !IsDemo) || IsDemo) && xt.XtMd != null &&
                            (Isfinal || (!Isfinal && xt.id == 1)))
                        {
                            WsPickmd.Clear();
                            GetWsOrderPosTeam(ref WsPickmd, ws);
                            if (WsPickmd.Count > 0)
                            {
                                res = MT.ZupMove(ref bquit, ref xt.ax_x, WsPickmd[0].st_pos[ud.id].x, ref xt.ax_y,
                                    WsPickmd[0].st_pos[ud.id].y, true);
                                if (res != EM_RES.OK) return res;
                                Thread.Sleep(50);
                                res = upcam.FindTaskTriAndWait(CONST.WsUpFw, IsDemo);
                                if (res != EM_RES.OK)
                                {
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "拍照错误!" : "Cam ERR", 20, true);
                                    MT.ST_WARN warn = new MT.ST_WARN();
                                    warning fr_warn = new warning();//增加语言
                                    warn.ok_txt = MultiLanguage.TxtSelct("确定", "OK", "VÂNG");
                                    warn.ws = null;
                                    warn.title = MultiLanguage.TxtSelct("提示:拍照错误", "Tip:Failed to take a picture", "Mẹo: Không chụp được ảnh");
                                    warn.msg = MultiLanguage.TxtSelct(
                                        $"工站位置{WsPickmd[0].Num}重拍失败,请检查对应的工站位置是否有异物!",
                                        $"Workstation location {WsPickmd[0].Num} failed to retake, please check if there is a foreign object in the corresponding station",
                                        $"Vị trí trạm làm việc {WsPickmd[0].Num} không thể thi lại, vui lòng kiểm tra xem có vật thể lạ trong trạm tương ứng không");
                                    warn.lb_msg = MultiLanguage.TxtSelct(
                                        $"提示:工站位置{WsPickmd[0].Num}重拍失败,请检查对应的工站位置是否有异物或物料，按确定键停止,确认无异物后再按运行键继续!",

                                        $"Tip: Workstation location {WsPickmd[0].Num} failed to retake.Please check whether there is any foreign object or material " +
                                        $"in the corresponding station location. Press the OK key to stop. After confirming that there are no foreign objects, press the Run key to continue!",

                                        $"Mẹo: Không thể thực hiện lại vị trí trạm làm việc {WsPickmd[0].Num}. Vui lòng kiểm tra xem có bất kỳ vật thể hoặc vật thể lạ nào ở vị trí trạm " +
                                        $"tương ứng hay không. Bấm phím OK để dừng. Sau khi xác nhận không có vật lạ, hãy nhấn phím Run để tiếp tục!");
                                    MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                                    return EM_RES.ERR;
                                }

                                res = xt.XtPickOrPlaceMod(ref bquit, WsPickmd[0].st_pos[ud.id].ToXY(),
                                    upcam.curTask.ResData.PosMM, dwData[i].PosMM, WsPickmd[0].st_pos[ud.id].z, Demo,
                                    XT.EM_XTFLOW.PLACEMOD, false, PT_SET.bModPasteUp);
                                if (res == EM_RES.OK)
                                {
                                    WsPickmd[0].bardcode = xt.XtMd.bardcode;                               
                                    WsPickmd[0].res = -1;
                                    WsPickmd[0].motor_barcode = xt.XtMd.motor_barcode;
                                    xt.XtMd = null;
                                }
                                else return res;

                            }
                            else break;
                        }
                    }
                    GrrProc = EM_STA.PICKWS;
                }
                //判断是否完成
                WsPickmd.Clear();
                GetWsOrderPosTeam(ref WsPickmd, ws);
                if (WsPickmd.Count == 0)
                {
                    GrrProc = EM_STA.PICKWS;
                    break;
                }
            }
            return EM_RES.OK;
        }
        #endregion

        #region 放模组于料盘
        //预判下一个位置
        public EM_RES MoveNextTrayX1Pos(ref bool bquit, TrayBox traybox, WS ws, UpDownLoad ud)
        {
            EM_RES res = EM_RES.OK;
            try
            {
                traybox.runin = true;
                double pos_x1 = -10000;
                if (traybox.tray_cur != null)
                    FindNextTrayPosX1(ref pos_x1, traybox, ws, ud);
                traybox.runUpdate = true;
                if (traybox_ng.runUpdate && traybox_ok.runUpdate)
                {
                    bUpdateFBPos_OKNG = false;
                    traybox_ng.runUpdate = false;
                    traybox_ok.runUpdate = false;
                }
                if (traybox_fd.runUpdate)
                {
                    bUpdateFBPos_UT = false;
                    traybox.runUpdate = false;
                }
                if (pos_x1 > -10 && (traybox.disc != traybox_ng.disc) && (!traybox.MoveToLastPos[(id + 1) % 2]))
                {
                    if (traybox.ax_x.isStop && traybox.tray_cur != null && traybox.IsReady)
                    {
                        res = MT.Move(ref bquit, ref traybox.ax_x, pos_x1);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}提前定位", traybox.ax_x.disc) : string.Format("{0} move in advance       ({0}提前定位)", traybox.ax_x.disc));
                    }

                }
                return res;
            }
            finally
            {
                //traybox.MoveToLastPos[(id + 1) % 2] = false;
                traybox.MoveToLastPos[id] = false;
                traybox.runin = false;
            }
        }
        public EM_RES TrayPlaceMd(ref bool bquit, TrayBox traybox, WS ws)
        {
            if (!traybox.IsReady) return EM_RES.ABORT;
            if (traybox.tray_cur == null) return EM_RES.PARA_OUTOFRANG;
            placepos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            if (placepos.Count == 0) return EM_RES.END;
            if (bquit) return EM_RES.QUIT;
            try
            {
                // EM_RES res=ax_y.SetSpeed(3000);
                //  if (res != EM_RES.OK) return res;PT_SET.isopen_degree ? -PT_SET.degree : 0
                //if (PT_SET.isopen_degree)
                //{
                //    foreach (XT xt in list_xt)
                //    {
                //        if ((!xt.cy_zk.isONByChkSen && !Demo) || xt.XtMd == null)
                //        {
                //            xt.cy_zk.SetOff();
                //            xt.XtMd = null;
                //            continue;
                //        }
                //        EM_RES res = MT.ZupMove(ref bquit, ref xt.ax_u, -PT_SET.degree);
                //        if (res != EM_RES.OK)
                //        {
                //            return res;
                //        }
                //    }
                //}
                List<XT> tempXt = new List<XT>();
                foreach (XT xt in list_xt)
                    tempXt.Add(xt);
                if (NewSysInf.UserParams.bXtPrePlaceTrayByBigId)//先放大编号
                {                   
                   if(tempXt.Count>1&& tempXt[0].id < tempXt[1].id)
                    {
                        tempXt.Reverse();
                    }                  
                }
                else
                {
                    if (tempXt.Count > 1 && tempXt[0].id > tempXt[1].id)
                    {
                        tempXt.Reverse();
                    }
                }
                foreach (XT xt in tempXt)
                {
                    if ((!xt.cy_zk.isONByChkSen && !Demo) || xt.XtMd == null)
                    {
                        xt.cy_zk.SetOff();
                        xt.XtMd = null;
                        continue;
                    }
                    if (traybox.disc == traybox_ok.disc && xt.XtMd.res != 0) continue;
                    if (traybox.disc == traybox_ng.disc && xt.XtMd.res < 1) continue;

                    EM_RES res = xt.PlaceMd(ref bquit, traybox, ws, Demo);
                    if (res != EM_RES.OK) return res;

                }
            }
            finally
            {
                // ax_y.SetSpeed(ax_y.spd_work);
            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 放模组
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES PlaceMdToTray(ref bool bquit, WS ws)
        {
            EM_RES res;
            int cnt = 0;
            //放料前旋转 - 90°，不管是否有料；
            //if (PT_SET.isopen_degree)
            //{
            //    res = MT.ZupMove(ref bquit, ref ax_u1, -PT_SET.degree, ref ax_u2, -PT_SET.degree);
            //    if (res != EM_RES.OK) return res;
            //}
            while (!bquit)
            {
                //仓储进出料
                res = TrayPlaceMd(ref bquit, traybox_ng, ws);
                if (res == EM_RES.PARA_OUTOFRANG || res == EM_RES.END)
                {
                    if (traybox_ng.IsReady)
                    {
                        traybox_ng.IsReady = false;
                        Task tskng = new Task(() =>
                        {
                            traybox_ng.Tray_Action(Demo);
                        });
                        tskng.Start();
                    }

                }
                else if (res != EM_RES.OK && res != EM_RES.ABORT)
                {
                    return res;
                }


                res = TrayPlaceMd(ref bquit, traybox_ok, ws);
                if (res == EM_RES.PARA_OUTOFRANG || res == EM_RES.END)
                {
                    if (traybox_ok.IsReady)
                    {
                        traybox_ok.IsReady = false;
                        Task tskok = new Task(() =>
                        {
                            traybox_ok.Tray_Action(Demo);
                        });
                        tskok.Start();
                    }
                }
                else if (res != EM_RES.OK && res != EM_RES.ABORT)
                {
                    return res;
                }

                //确认物料完成
                cnt = 0;
                foreach (XT xt in list_xt)
                {
                    if (!xt.cy_zk.isONByChkSen && xt.XtMd == null) cnt++;
                }

                if (cnt == list_xt.Count) break;
                Thread.Sleep(50);
            }
            return EM_RES.OK;
        }
        #endregion

        #region 上相机定位回检

        public EM_RES UpCamBackCheck(ref bool bquit, ref WS ws, XT xt,int udid, ST_XY pos_mod_upcam, ref VisionOutPutData ResData)
        {
            //进行上相机拍照
            EM_RES res = EM_RES.OK;
            if (!PT_SET.bDwAddCapQrcode)
            {
                return EM_RES.OK;
            }
    RECHECK:
            for (int i = 0; i < 2; i++)
            {
                res = xt.UpCam(ref bquit, pos_mod_upcam, CONST.WsModUpFw, ref ResData, Demo);
                if (res == EM_RES.OK)
                {
                    break;
                }
            }
            if (res != EM_RES.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机回检拍照失败，流程为:{1}", xt.disc, CONST.WsModUpFw.ToString()));
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "拍照失败!" : "Cam ERR", 20, true);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = "重新拍照";
                warn.abort_txt = "停止";
                warn.cancle_txt =    PT_SET.bBackerrcontinue ? "取消":"停止运行";
                warn.title = "提示:上相机回检拍照失败!";
                warn.msg = string.Format("{0}上相机拍照失败，流程为:{1}", xt.disc, CONST.WsModUpFw.ToString());
                warn.lb_msg = string.Format("{0}上相机拍照失败\r\n点击重新拍照，则重新进行回检拍照！！！\r\n点击取消这默认放料OK\r\n点击停止则运行会停止\r\n", xt.disc);
                DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                if (resulte == DialogResult.Yes)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了是",disc));
                    goto RECHECK;
                }
                else if (resulte == DialogResult.Cancel)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                  
                    if (PT_SET.bBackerrcontinue )
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了取消", disc));
                        return EM_RES.OK;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了停止", disc));
                        return EM_RES.ERR;
                    }   
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了停止", disc));
                    return EM_RES.ERR;
                }
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机拍照完成，数据X:{1},Y:{2},Z:{3}", xt.disc, ResData.PosMM.x, ResData.PosMM.y, ResData.PosMM.a));
            //清空相机数据
            if (!upcam.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!upcam.FlushOk && !upcam.FlushUpdate)
                    {
                        upcam.FlushUpdate = true;
                        upcam.mAcqFifo.Flush();
                        upcam.FlushOk = true;
                        upcam.FlushUpdate = false;
                    }
                });
                tak.Start();
            }

            string[] date = new string[3];
            double left = double.MaxValue;
            double right = double.MaxValue;
            double up = double.MaxValue; ;
            double down = double.MaxValue;
            if (xt.upcam.curTask.ResData.bOK)
            {
                try
                {
                    date = upcam.curTask.ResData.Message.Split(',');
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("回检data数量{0}，data值:{1}，Message值:{2}", date.Length, date.ToString(),upcam.curTask.ResData.Message.ToString()));
                    var ret = AreaCal(date, out left, out right, out up, out down);
                    if (ret != EM_RES.OK) return ret;              

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (((((Math.Abs(left - PT_SET.LeftArea) > PT_SET.Area) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea) > PT_SET.Area) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea) > PT_SET.Area) && PT_SET.bConnectorCheck)
                || ((Math.Abs(down - PT_SET.DownArea) > PT_SET.Area) && PT_SET.bConnectorCheck))&&id==0)|| ((((Math.Abs(left - PT_SET.LeftArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck)
                || ((Math.Abs(down - PT_SET.DownArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck)) && id == 1))
            {
                xt.upcam.SaveOriginImage(xt.upcam.curTask.Image, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, xt.upcam.mName), string.Format("{0}{1}.jpg",
                            DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("物料放偏坐标{0}，请确认", xt.upcam.curTask.ResData.PosMM.ToString()));
                date = upcam.curTask.ResData.Message.Split(',');
                try
                {
                    if (date.Length < 2)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("报警2"));
                        MT.ST_WARN warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                        warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        warn.ws = null;
                        warn.title = VAR.IsChinese ? "提示:数据异常" : "Tip: Data File  Err";
                        warn.msg = "当前放偏检测没有防压伤功能!";
                        warn.lb_msg = "提示:请确认视觉文件和模板，请确认!\r\n  1.如果忽视,请按继续运行键!\r\n  " +
                                                      "2.请停止运行，更新最新视觉文件和模板后，再按运行键!";
                        DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                        if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                        else

                            left = 0;
                        right = 0;
                        up = 0;
                        down = 0;
                    }
                    else
                    {
                        left = Convert.ToDouble(date[0]);
                        right = Convert.ToDouble(date[1]);
                        up = Convert.ToDouble(date[2]);
                        down = Convert.ToDouble(date[3]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (((((Math.Abs(left - PT_SET.LeftArea) > PT_SET.Area) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea) > PT_SET.Area) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea) > PT_SET.Area) && PT_SET.bConnectorCheck)
                    || ((Math.Abs(down - PT_SET.DownArea) > PT_SET.Area) && PT_SET.bConnectorCheck)) && id == 0) || ((((Math.Abs(left - PT_SET.LeftArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck)
                    || ((Math.Abs(down - PT_SET.DownArea2) > PT_SET.Area2) && PT_SET.bConnectorCheck)) && id == 1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机回检失败,存在放偏，流程为:{1}", xt.disc, CONST.WsModUpFw.ToString()));
                    upcam.SaveOriginImage(upcam.curTask.Image, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, upcam.mName), string.Format("{0}{1}.jpg",
                                       DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "回检失败!" : "Cam ERR", 20, true);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = "重新拍照";
                    warn.abort_txt = "停止";
                    warn.cancle_txt = "取消";
                    warn.title = "提示:上相机回检拍照失败!";
                    warn.msg = string.Format("{0}上相机回检失败,存在放偏，流程为:{1}", xt.disc, CONST.WsModUpFw.ToString());
                    warn.lb_msg = string.Format("{0}上相机回检失败,存在放偏\r\n点击重新拍照，则重新进行回检拍照！！！\r\n点击取消则默认放置OK，设备继续运行\r\n点击停止则运行会停止\r\n", xt.disc);
                    DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                    if (resulte == DialogResult.Yes)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了是", disc));
                        goto RECHECK;
                    }
                    else if (resulte == DialogResult.Cancel)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了取消", disc));
                        return EM_RES.OK;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了停止", disc));
                        bquit = true;
                        return EM_RES.ERR;
                    }
                }
            }

            return EM_RES.OK;
        }
        #endregion

        #region 上相机定位回检2

        public EM_RES UpCamBackCheck2(ref bool bquit, ref WS ws, XT xt, int udid, ST_XY pos_mod_upcam, ref VisionOutPutData ResData)
        {
            //进行上相机拍照
            EM_RES res = EM_RES.OK;
            if (!PT_SET.bDwAddCapQrcode)
            {
                return EM_RES.OK;
            }
RECHECK:
            pos_mod_upcam.x = pos_mod_upcam.x + xt.DwCapQrCodeoffset;
            for (int i = 0; i < 2; i++)
            {
                res = xt.UpCam(ref bquit, pos_mod_upcam, CONST.WsModUpFw2, ref ResData, Demo);
                if (res == EM_RES.OK)
                {
                    break;
                }
            }
            if (res != EM_RES.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机回检拍照失败，流程为:{1}", xt.disc, CONST.WsModUpFw2.ToString()));
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "拍照失败!" : "Cam ERR", 20, true);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = "重新拍照";
                warn.abort_txt = "停止";
                warn.cancle_txt = "取消";
                warn.title = "提示:上相机回检拍照失败!";
                warn.msg = string.Format("{0}上相机拍照失败，流程为:{1}", xt.disc, CONST.WsModUpFw2.ToString());
                warn.lb_msg = string.Format("{0}上相机拍照失败\r\n点击重新拍照，则重新进行回检拍照！！！\r\n点击取消这默认放料OK\r\n点击停止则运行会停止\r\n", xt.disc);
                DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                if (resulte == DialogResult.Yes)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了是", disc));
                    goto RECHECK;
                }
                else if (resulte == DialogResult.Cancel)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了取消", disc));
                    return EM_RES.OK;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机拍照失败，选择了取消或者停止", disc));
                    return EM_RES.ERR;
                }
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机拍照完成，数据X:{1},Y:{2},Z:{3}", xt.disc, ResData.PosMM.x, ResData.PosMM.y, ResData.PosMM.a));
            //清空相机数据
            if (!upcam.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!upcam.FlushOk && !upcam.FlushUpdate)
                    {
                        upcam.FlushUpdate = true;
                        upcam.mAcqFifo.Flush();
                        upcam.FlushOk = true;
                        upcam.FlushUpdate = false;
                    }
                });
                tak.Start();
            }

            string[] date = new string[3];
            double left = double.MaxValue;
            double right = double.MaxValue;
            double up = double.MaxValue; ;
            double down = double.MaxValue;
            if (xt.upcam.curTask.ResData.bOK)
            {
                try
                {
                    date = xt.upcam.curTask.ResData.Message.Split(',');
                    var ret = AreaCal(date, out left, out right, out up, out down);
                    if (ret != EM_RES.OK) return ret;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (((((Math.Abs(left - PT_SET.LeftArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck)
                || ((Math.Abs(down - PT_SET.DownArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck)) && id == 0) || ((((Math.Abs(left - PT_SET.LeftArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck)
                || ((Math.Abs(down - PT_SET.DownArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck)) && id == 1))
            {
                xt.upcam.SaveOriginImage(xt.upcam.curTask.Image, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, xt.upcam.mName), string.Format("{0}{1}.jpg",
                            DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("物料放偏坐标{0}，请确认", xt.upcam.curTask.ResData.PosMM.ToString()));
                date = xt.upcam.curTask.ResData.Message.Split(',');
                try
                {
                    if (date.Length < 2)
                    {
                        MT.ST_WARN warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                        warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        warn.ws = null;
                        warn.title = VAR.IsChinese ? "提示:数据异常" : "Tip: Data File  Err";
                        warn.msg = "当前放偏检测没有防压伤功能!";
                        warn.lb_msg = "提示:请确认视觉文件和模板，请确认!\r\n  1.如果忽视,请按继续运行键!\r\n  " +
                                                      "2.请停止运行，更新最新视觉文件和模板后，再按运行键!";
                        DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                        if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                        else

                            left = 0;
                        right = 0;
                        up = 0;
                        down = 0;
                    }
                    else
                    {
                        left = Convert.ToDouble(date[0]);
                        right = Convert.ToDouble(date[1]);
                        up = Convert.ToDouble(date[2]);
                        down = Convert.ToDouble(date[3]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (((((Math.Abs(left - PT_SET.LeftArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck)
                    || ((Math.Abs(down - PT_SET.DownArea3) > PT_SET.Area3) && PT_SET.bConnectorCheck)) && id == 0) || ((((Math.Abs(left - PT_SET.LeftArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck) || ((Math.Abs(right - PT_SET.RightArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck) || ((Math.Abs(up - PT_SET.UpArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck)
                    || ((Math.Abs(down - PT_SET.DownArea4) > PT_SET.Area4) && PT_SET.bConnectorCheck)) && id == 1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机回检失败,存在放偏，流程为:{1}", xt.disc, CONST.WsModUpFw2.ToString()));
                    upcam.SaveOriginImage(upcam.curTask.Image, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, upcam.mName), string.Format("{0}{1}.jpg",
                                       DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "回检失败!" : "Cam ERR", 20, true);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = "重新拍照";
                    warn.abort_txt = "停止";
                    warn.cancle_txt = "取消";
                    warn.title = "提示:上相机回检拍照失败!";
                    warn.msg = string.Format("{0}上相机回检失败,存在放偏，流程为:{1}", xt.disc, CONST.WsModUpFw2.ToString());
                    warn.lb_msg = string.Format("{0}上相机回检失败,存在放偏\r\n点击重新拍照，则重新进行回检拍照！！！\r\n点击取消则默认放置OK，设备继续运行\r\n点击停止则运行会停止\r\n", xt.disc);
                    DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                    if (resulte == DialogResult.Yes)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了是", disc));
                        goto RECHECK;
                    }
                    else if (resulte == DialogResult.Cancel)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了取消", disc));
                        return EM_RES.OK;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上相机回检失败，选择了停止", disc));
                        bquit = true;
                        return EM_RES.ERR;
                    }
                }
            }

            return EM_RES.OK;
        }
        #endregion

        #region 下相机定位拍照

        public EM_RES DwCamAction(ref bool bquit, XT xt, string CapFlow, ref VisionOutPutData ResData)
        {
//进行下相机拍照
RECAP:
            var res = xt.DwCam(ref bquit, CapFlow, ref ResData, Demo);
            if (res != EM_RES.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照失败，流程为:{1}", xt.disc, CapFlow));

                BackXtid = xt.id;
                return EM_RES.CAM_ERR_PLACE_BACK;

                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "拍照失败!" : "Cam ERR", 20, true);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = "确定";
                warn.abort_txt = "停止";
                warn.cancle_txt = "重新取料";
                warn.title = "提示:下相机拍照失败!";
                warn.msg = string.Format("{0}下相机拍照失败，流程为:{1}", xt.disc, CapFlow);
                warn.lb_msg = string.Format("{0}下相机拍照失败\r\n点击确定，则重新进行拍照！！！\r\n点击重新取料则放回物料，重新取料\r\n点击停止则运行会停止\r\n", xt.disc);
                DialogResult resulte = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);

                if (resulte == DialogResult.OK)
                {
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照失败，选择了是", disc));
                    goto RECAP;
                }
                else if (resulte == DialogResult.Cancel)
                {
                    BackXtid = xt.id;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照失败，选择了重新取料", disc));
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                    return EM_RES.CAM_ERR_PLACE_BACK;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照失败，选择了停止", disc));
                    return EM_RES.ERR;
                }
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}下相机拍照完成，数据X:{1},Y:{2},Z:{3}", xt.disc, ResData.PosMM.x, ResData.PosMM.y, ResData.PosMM.a));
            return EM_RES.OK;
        }
        #endregion

        #region 取工站模组函数
        public bool HaveWsPickMd(List<WS.MdDat> WsPickPos)
        {
            if (WsPickPos == null || WsPickPos.Count == 0) return false;
            foreach (WS.MdDat md in WsPickPos)
            {
                if ( md.res > -1)
                    return true;
            }
            return false;
        }
        public EM_RES PickWsMod(ref bool bquit, ref WS ws)
        {
            EM_RES res = EM_RES.OK;
            int xtid = 0;
            Product.MdDat pXtMd = new Product.MdDat();
            ST_XYZA pickpos = new ST_XYZA();
            List<WS.MdDat> WsPickPos = new List<WS.MdDat>();
            ST_XY xtvs = new ST_XY();
            bool tryagain = false;
            bool updatebarcode = false;
            int PickNum = 0;
            string[] Errmsg = new string[3];
            bool bAlm = false;
            // if (WsPickPos.Count > 0) WsPickPos.Clear();
            GetWsPosTeam(ref WsPickPos, ws, Product.EM_CM_RES.OK, FindMod);
            while (!bquit)
            {
                bool HaveMd = HaveWsPickMd(WsPickPos);
                if (!HaveMd)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? "HaveWsPickMd没有找到测试完成的模组!" : "HaveWsPickMd,no mod was found to complete the test!         (HaveWsPickMd没有找到测试完成的模组!)");
                    return EM_RES.END;
                }
                if (tryagain)
                {
                    tryagain = false;
                    if (PickNum < (WsPickPos[0].Num % 8) % 4)
                        return EM_RES.OK;
                }
                //  if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN && updatebarcode == false && PT_SET.bOkCheck)
                //优化取料前全部检测防止压伤
                if ( updatebarcode == false && PT_SET.bOkCheck)
                {
                    updatebarcode = true;
                    foreach (WS.MdDat md in WsPickPos)
                    {
                        //if (md.res != 0) continue;
                        RECAP:
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "Pick_move->md:"+md.Num.ToString());
                        //if (md.st_pos[id].y - ax_y.fenc_pos < 150)
                        //{
                        //    res = ax_y.SetToWorkSpd(0.4);
                        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}速度降为原来的40%:", ax_y.disc));
                        //}

                        res = MT.ZupMove(ref bquit, ref ax_x, md.st_pos[id].x, ref ax_y, md.st_pos[id].y);
                        //ax_y.SetToWorkSpd();
                        if (res != EM_RES.OK) return res;
                        //int i = 0;
                        //while (!upcam.FlushOk)
                        //{
                        //    Thread.Sleep(5);
                        //    if (i++ > 30)
                        //    {
                        //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "等待upcam.FlushOk为True超时(0.15S)");
                        //        break;
                        //    }
                        //}
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "Pick_Move_End_Cap->md:" + md.Num.ToString());
                        res = upcam.FindTaskTriAndWait(CONST.WsModUpFw, Demo);
                        //下料前进行二维码防呆
                        if (PT_SET.OpenDownQrde && PT_SET.BarcodeMode != (int)PT_SET.BAR_SCAN.NO_SCAN)
                        {
                            String str1, str2, str3, str4, str5;
                            str2 = "当前工站位置：" + ws.num + "工位号："+md.Num+ "回检二维码：" + upcam.curTask.ResData.BarCode + "记录二维码：" + md.bardcode;
                            Utility.WriteStrToCSV("", str2);
                            bool bAlmin = false;
                            if (PT_SET.UpDownQrde)
                            {
                                if (upcam.curTask.ResData.BarCode != null && upcam.curTask.ResData.BarCode.Length > 1 && upcam.curTask.ResData.BarCode != md.bardcode && !Demo)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}_工位:{1}->回检二维码不一致,实拍Barcode:{2},记录Barcode:{3},把记录二维码结果改为111", ws.disc, md.Num, upcam.curTask.ResData.BarCode, md.bardcode) : string.Format(" {0}-WS:{1},barcodes are inconsistent,new barcode:{2},old barcode:{3},change the result to 111.         ({0}_工位:{1}->回检二维码不一致,实拍Barcode:{2},记录Barcode:{3},把记录二维码结果改为111)", ws.disc, md.Num, upcam.curTask.ResData.BarCode, md.bardcode));
                                    if (md.res == 0) md.res = 111;
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料前检测二维码未通过:" + md.bardcode);
                            
                                    MT.ST_WARN st_warn = new MT.ST_WARN();
                                    warning fr_warn = new warning();
                                    st_warn.ok_txt = VAR.IsChinese ? "确定" : "Give up";
                                    st_warn.abort_txt = VAR.IsChinese ? "确定" : "Try again";
                                    st_warn.ws = ws;
                                    st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                                    st_warn.title = VAR.IsChinese ? "提示:下料前检测二维码未通过!" : "Tip: Abnormal suction!";
                                    st_warn.msg = VAR.IsChinese ? string.Format("{0}_工位:{1}->回检二维码不一致,实拍Barcode:{2},记录Barcode:{3},把记录二维码结果改为111", ws.disc, md.Num, upcam.curTask.ResData.BarCode, md.bardcode) : string.Format(" {0}-WS:{1},barcodes are inconsistent,new barcode:{2},old barcode:{3},change the result to 111.         ({0}_工位:{1}->回检二维码不一致,实拍Barcode:{2},记录Barcode:{3},把记录二维码结果改为111)", ws.disc, md.Num, upcam.curTask.ResData.BarCode, md.bardcode);
                                    st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.点击确定将继续运行!\r\n  " +
                                        "2.点击停止将停止运行!\r\n  ";

                                    DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                                    if (DialogResult.Cancel == logres)
                                    {
                                        return EM_RES.ERR;
                                    }
                                }
                                else
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料前检测二维码通过:" + md.bardcode);
                                }
                            }
                        }
                        bAlm = false;
                        if (!upcam.curTask.ResData.bUpdate || !upcam.curTask.ResData.bOK || Math.Abs(upcam.curTask.ResData.PosMM.x) > PT_SET.Vs_XYofs
                             || Math.Abs(upcam.curTask.ResData.PosMM.y) > PT_SET.Vs_XYofs || Math.Abs(upcam.curTask.ResData.PosMM.a) > PT_SET.Vs_Rofs)
                        {
                            Errmsg[0] = VAR.IsChinese ? "模组被带偏" : "Open deflected";
                            Errmsg[1] = VAR.IsChinese ? string.Format("{0}_工位{1}上模组被带偏.", ws.disc, md.Num) : string.Format("{0}_ws{1}product was deflected by fixture.", ws.disc, md.Num);
                            bAlm = true;
                        }
                        else if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN && (res == EM_RES.OK &&PT_SET.bBarcodeCamBackEn&& (upcam.curTask.ResData.BarCode == null || upcam.curTask.ResData.BarCode.Length < 1)) && !Demo)
                        {
                            Errmsg[0] = VAR.IsChinese ? "没检到二维码!" : "No Barcode";
                            Errmsg[1] = VAR.IsChinese
                                ? string.Format("{0}_工位{1}上模组二维码无法识别.", ws.disc, md.Num)
                                : string.Format("{0}_ws{1}can't recognize barcode.", ws.disc, md.Num);
                            bAlm = true;
                        }

                        //if (res == EM_RES.OK && (upcam.curTask.ResData.BarCode == null || upcam.curTask.ResData.BarCode.Length < 1))
                        if (bAlm)
                        {
                            List<int> NgSameResNum = new List<int>();
                            bAlm = false;
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, Errmsg[0], 20, true);
                            MT.ST_WARN st_warn1 = new MT.ST_WARN();//增加语言
                            warning fr_warn1 = new warning();
                            st_warn1.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                            st_warn1.abort_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                            st_warn1.ws = null;
                            NgSameResNum.Add(md.Num);
                            st_warn1.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                            st_warn1.msg = Errmsg[1];
                            st_warn1.title = MultiLanguage.TxtSelct("提示:" + Errmsg[0], "Tip:" + Errmsg[0], "Mẹo:" + Errmsg[0]);
                            st_warn1.lb_msg = MultiLanguage.TxtSelct(
                                "提示:" + st_warn1.msg + "请确认!\r\n  " +
                                "1.按'继续运行'键则放弃当前检查继续运行!\r\n " +
                                "2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按运行键!\r\n " +
                                "3.如按'重新拍照'键则重新获取当前位置图像!",

                                "Tip: " + st_warn1.msg + "\r\nPlease confirm!\r\n  " +
                                "1.Press the 'Keep running' button to abandon the current check and continue running!\r\n  " +
                                "2.If you need to confirm the problem, press the 'Stop running' button to exit the operation. After the ready is displayed in the upper left corner of the interface, press the Run button!\r\n" +
                                "3.If you press the 'Take a photo' button, you will get the current position image again!",

                                "Mẹo:" + st_warn1.msg + "\r\nVui lòng xác nhận!\r\n  " +
                                "1.Nhấn nút 'Tiếp tục chạy' để bỏ kiểm tra hiện tại và tiếp tục chạy!\r\n " +
                                "2.Nếu bạn cần xác nhận sự cố, hãy nhấn nút 'Dừng chạy' để thoát khỏi hoạt động. Sau khi sự sẵn sàng hiển thị ở góc trên bên trái giao diện, hãy nhấn nút Run! \r\n" +
                                "3.Nếu bạn nhấn nút 'Chụp ảnh', bạn sẽ nhận lại được hình ảnh vị trí hiện tại!");
                            DialogResult logres1 = MT.Display_frwarn(fr_warn1, st_warn1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            if (DialogResult.Cancel == logres1) return EM_RES.ERR;
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                            if (DialogResult.Abort == logres1)
                            {
                                goto RECAP;
                            }
                        } 
                    }

                    if (!upcam.FlushOk && !upcam.FlushUpdate)
                    {
                        Task tak = new Task(() =>
                        {
                            upcam.FlushUpdate = true;
                            upcam.mAcqFifo.Flush();
                            upcam.FlushOk = true;
                            upcam.FlushUpdate = false;
                        });
                        tak.Start();
                    }
                }
              

                //取料
                foreach (WS.MdDat md in WsPickPos)
                {
                    bool OnlyOneXt = id == 0 ? PT_SET.bUpDn1XtOnlyOne : PT_SET.bUpDn2XtOnlyOne;

                    //xtid = md.Num / 9;
                    if (!OnlyOneXt) xtid = md.Num / 9;
                    else if (PT_SET.xt1firsten) xtid = 0;
                    else xtid = 1;
                    xtvs = list_xt[xtid].st_vs_pos_xtshp.ToXY();
                    pickpos = md.st_pickpos[id].ToXYZA() + list_xt[xtid].st_rol_cap.ToXYZA() - list_xt[0].st_rol_cap.ToXYZA() + xtvs.ToXYZA();
                    pickpos.z = xtid == 0 ? pickpos.z : -pickpos.z;
                    pickpos.a = PT_SET.isopendown_degree ? PT_SET.downdegree : 0;
                PICK:
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}工站取料: X:{1} Y:{2}", disc, pickpos.x, pickpos.y) : string.Format("{0}WS pickmod: X:{2} Y:{3}            ({1}工站取料: X:{2} Y:{3})", englishdisc, disc, pickpos.x, pickpos.y));
                    for (int i = 0; i < 2; i++)
                    {
                        bool bmove = NewSysInf.UserParams.bPickWsDis;//取料后偏移运动
                       // bmove = true;//临时测试强制开启
                        res = list_xt[xtid].Pick(ref bquit, pickpos, true, Demo, true, bmove);
                        if (res != EM_RES.PICK_ERR || !PT_SET.bWsPickAgain) break;
                        else
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}工站重取物料: X:{1} Y:{2}", disc, pickpos.x, pickpos.y) : string.Format("{0} WS retry to pick mod: X:{2} Y:{3}           ({1}工站重取物料: X:{2} Y:{3})", englishdisc, disc, pickpos.x, pickpos.y));
                    }
                    if (res == EM_RES.OK)
                    {
                        pXtMd.res = md.res;
                        pXtMd.bardcode = md.bardcode;
                        pXtMd.Num = md.Num;
                        pXtMd.WS_ID = md.WS_ID;
                        pXtMd.PC_ID = md.PC_ID;
                        list_xt[xtid].XtMd = pXtMd.Clone();
                        md.res = -2;
                        md.bardcode = null;
                    }
                    else if (res == EM_RES.PICK_ERR)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "吸料异常!" : "Draw ERR", 20, true);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "工站吸料异常", DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.UpDownLoad + id);
                        MT.ST_WARN st_warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        st_warn.ok_txt = VAR.IsChinese ? "放弃吸取" : "Give up";
                        st_warn.abort_txt = VAR.IsChinese ? "重新吸取" : "Try again";
                        st_warn.ws = ws;
                        st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        st_warn.title = VAR.IsChinese ? "提示:吸料异常!" : "Tip: Abnormal suction!";
                        st_warn.msg = VAR.IsChinese ? string.Format("{0}下料失败,{1}无法吸起当前工站第{2}模组!", disc, list_xt[xtid].disc, md.Num) : string.Format("{0} download failed,{2} can't suck up module {3} of the current station \r\n{1}下料失败,{2}无法吸起当前工站第{3}模组!", englishdisc, disc, list_xt[xtid].disc, md.Num);
                        st_warn.lb_msg = VAR.IsChinese ? "提示:" + st_warn.msg + "请确认!\r\n  1.如果工站上没有物料,请按放弃吸取键继续吸下一个物料!\r\n  " +
                                         "2.如有物料请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!\r\n 3.如按重新吸取键则吸嘴再次吸取!" : "Tip:" + st_warn.msg + "\r\nPlease confirm!\r\n  1.If there is no material on the station, please press the 'Give up' button to continue sucking the next material!\r\n  " +
                                                                                                      "2.If there is any material, press the 'Stop running' button to exit the operation, and then press the run button after the ready display on the upper left corner of the interface!\r\n 3.If the 'Try again' button is pressed, the suction nozzle sucks again!" + "\r\n提示:" + st_warn.msg + "请确认!\r\n  1.如果工站上没有物料,请按放弃吸取键继续吸下一个物料!\r\n  " +
                                                                                                      "2.如有物料请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!\r\n 3.如按重新吸取键则吸嘴再次吸取!";
                        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        ////增加门禁检测
                        //while (PT_SET.bEnUpDownDr)
                        //{
                        //   res = MT.ChkAllSafeSen(0x01);
                        //    if (res != EM_RES.OK)
                        //    {
                        //        MT.DoorAlmTip();
                        //    }
                        //    else break;
                        //    Thread.Sleep(100);
                        //}

                        if (DialogResult.Cancel == logres) return EM_RES.ERR;
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        if (DialogResult.OK == logres)
                        {
                            md.res = -2;
                            md.bardcode = null;
                        }
                        else if (DialogResult.Abort == logres)
                        {
                            goto PICK;
                        }
                        else if (DialogResult.Retry == logres)
                        {
                            tryagain = true;
                            PickNum = (WsPickPos[0].Num % 8) % 4;
                            break;
                        }

                    }
                    else
                    {
                        if (res != EM_RES.ABORT) return res;
                    }
                }
                if (tryagain)
                {
                    if (WsPickPos.Count > 0) WsPickPos.Clear();
                    GetWsPosTeam(ref WsPickPos, ws, Product.EM_CM_RES.OK, FindMod);
                    break;
                }
                else
                {
                    if (FindMod == EM_FIN_MOD.ALL || (FindMod != EM_FIN_MOD.ALL && TrayGoPos == 0))
                        TrayGoPos = ax_x.fenc_pos;

                    break;
                }
            }
            return EM_RES.OK;
        }
        #endregion

        #region 飞拍失败放回料盘
        public EM_RES DwCamErrPalceBack(ref bool bquit)
        {
            int xtnum=0;
            EM_RES res = EM_RES.OK;
            foreach (Cam.VisionTask task in dwcam.List_vs_task_cur)
            {
                if (PT_SET.bDwAddCapQrcode)
                {
                    xtnum = BackXtid;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("飞拍失败放回料盘xtnum为{0}，当前相机任务数量为{1}", xtnum,dwcam.List_vs_task_cur.Count), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                }
                else
                {

                    xtnum = task.TriPos.n;
                }

                if (xtnum == 2) xtnum = 0;
                if (xtnum == 3) xtnum = 1;
                if (!task.ResData.bOK && list_xt[xtnum].cy_zk.isONByChkSen && task.ResData.bUpdate)
                {
                    try
                    {

                        int n = 0;
                        while (traybox_fd.runin || !traybox_fd.ax_x.isStop)
                        {
                            if (n++ > 500) break;
                            if (bquit) return EM_RES.QUIT;
                            Thread.Sleep(10);
                        }
                        traybox_fd.MoveToLastPos[id] = true;
                        if (n > 500)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("等待{0}定位超时5S", traybox_fd.ax_x.disc) : string.Format("Wait for {0} to move timeout(5s)         (等待{0}定位超时5S)", traybox_fd.ax_x.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.ERR;
                        }

                        res = XtMovPickPos(ref bquit, list_xt[xtnum]);
                        if (res != EM_RES.OK) break;
                        PosInf curpos = null;
                        if (traybox_fd.tray_cur != null)
                        {
                            curpos = traybox_fd.tray_cur.list_pos.ElementAt(list_xt[xtnum].XtMd.idx);
                        }
                        if (traybox_fd.tray_cur == null || curpos==null||(curpos.md!=null&& curpos.md.res != -2))
                        {
                        ReChk:
                            MT.ST_WARN warn = new MT.ST_WARN();
                            warning fr_warn = new warning();//增加语言
                            warn.ok_txt = MultiLanguage.TxtSelct("继续", "GoOn", "Đi tiếp");
                            warn.cancle_txt = MultiLanguage.TxtSelct("停止", "Stop", "Ngừng lại");
                            warn.title = MultiLanguage.TxtSelct("提示:飞拍失败放回异常", "Tip: Fly Err Back Place Tray Error", "Mẹo: Fly Err Back Place Tray Error");
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "飞拍失败放回异常" : "Place Tray Error", 10, true);
                            warn.msg = MultiLanguage.TxtSelct("飞拍失败放回异常!", "Place Tray Error", "Lỗi đặt khay");
                            warn.lb_msg = MultiLanguage.TxtSelct(
                                $"{disc}当前飞拍失败放回料盘找不到位置，请手动取走吸头 {list_xt[xtnum].disc} 上物料后按继续运行。否者按停止运行则停机",

                                $"{disc} The current flyer fails to put it back on the tray and can't find the position. Please manually remove the material on the suction head" +
                                $"{list_xt[xtnum].disc} and press Continue to run. Otherwise, press stop to stop",

                                $"{disc} Tờ rơi hiện tại không thể đặt nó trở lại khay và không thể tìm thấy vị trí. Vui lòng loại bỏ thủ công vật liệu trên đầu hút " +
                                $"{list_xt[xtnum].disc} và nhấn Tiếp tục để chạy. Nếu không, hãy nhấn dừng để dừng lại");
                            DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                            if (logres == DialogResult.OK)
                            {

                                if (list_xt[xtnum].cy_zk.isONByChkSen)
                                    goto ReChk;
                                else
                                {
                                    list_xt[xtnum].XtMd = null;
                                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "Run", 0, true);
                                    continue;
                                }
                            }
                            else return EM_RES.ERR;
                        }

                        //res = MT.ZupMove(ref bquit, ref ax_x, list_xt[xtnum].xt_pos_pick_mod.x, ref ax_y, list_xt[xtnum].xt_pos_pick_mod.y);
                        //if (res != EM_RES.OK) return res;
                        res = list_xt[xtnum].PickOrPlace(ref bquit, list_xt[xtnum].xt_pos_pick_mod, false, Demo);
                        if (res == EM_RES.OK) list_xt[xtnum].XtMd = null;
                        else if (res != EM_RES.OK) break;
                    }
                    finally
                    {
                        traybox_fd.runin = false;
                    }
                }
            }

            return res;
        }
        #endregion

        #region 马达拍照失败放回料盘
        public EM_RES MotorErrPalceBack(ref bool bquit)
        {
            //List<Product.Tray.PosInf> ListPickMod = new List<Product.Tray.PosInf>();
            //ListPickMod = traybox_fd.tray_cur.GetPosList();

             VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, $"进入马达二维码失败放回");

            EM_RES res = EM_RES.OK;
            for (int i = 0; i < 2; i++)
            {
                //var barcod = list_xt[i].XtMd.motor_barcode;
                bool bMdMotoHaveScan = list_xt[i].XtMd != null && list_xt[i].XtMd.bhaveMotoScan;
                
                string barcode = list_xt[i].XtMd.motor_barcode;
                bool bErrBar= (barcode.Length != PT_SET.motorBarcodeDigits&&NewSysInf.UserParams.bCheckMotoCodeLength) || barcode.Length < 5;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, $"{list_xt[i].disc}+马达二维码失败放回bErrBar{bErrBar}bMdMotoHaveScan{bMdMotoHaveScan}");
                if ( bMdMotoHaveScan && bErrBar/*|| (ListPickMod.Count<2)&& list_xt[i].cy_zk.isONByChkSen*/)
                {
                    try
                    {
                        int n = 0;
                        while (traybox_fd.runin || !traybox_fd.ax_x.isStop)
                        {
                            if (n++ > 500) break;
                            if (bquit) return EM_RES.QUIT;
                            Thread.Sleep(10);
                        }
                        traybox_fd.MoveToLastPos[id] = true;
                        if (n > 500)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("等待{0}定位超时5S", traybox_fd.ax_x.disc) : string.Format("Wait for {0} to move timeout(5s)         (等待{0}定位超时5S)", traybox_fd.ax_x.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.UpDownLoad + id, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.ERR;
                        }

                        res = XtMovPickPos(ref bquit, list_xt[i]);
                        if (res != EM_RES.OK) break;

                        //res = MT.ZupMove(ref bquit, ref ax_x, list_xt[xtnum].xt_pos_pick_mod.x, ref ax_y, list_xt[xtnum].xt_pos_pick_mod.y);
                        //if (res != EM_RES.OK) return res;
                        res = list_xt[i].PickOrPlace(ref bquit, list_xt[i].xt_pos_pick_mod, false, Demo);
                        if (res == EM_RES.OK)
                        {
                            list_xt[i].XtMd = null;
                        }
                        else if (res != EM_RES.OK) break;
                    }
                    finally
                    {
                        traybox_fd.runin = false;
                    }
                }
            }

            return res;
        }
        #endregion

        #region 放模组于工站函数

        /// <summary>
        /// 移动到工站飞拍
        /// </summary>
        public EM_RES FlyToWs(ref bool bquit, ref ST_XY pos_flystop, List<WS.MdDat> WsTriPos, bool Ofs_On = true)
        {
            try
            {
                EM_RES res;
                double fly_safepos = list_xt[1].st_rol_cap.y - 180;
                List<ST_XYN> TriPos = new List<ST_XYN>();

                //配置下拍照
                FlyCfg(EM_FLY_CFG.MOD_DW, new List<ST_XYN>(), WsTriPos.Count, true, Ofs_On);

                //配置工站拍照
                FeedBackWs.Clear();
                foreach (WS.MdDat md in WsTriPos)
                {
                    TriPos.Add(new ST_XYN(WsTriPos[WsTriPos.Count - 1].st_pos[id].x, md.st_pos[id].y, md.Num - 1));
                    FeedBackWs.Add(md.Clone());
                }
                if (WsTriPos.Count > 0)
                {
                    pos_flystop.x = WsTriPos[WsTriPos.Count - 1].st_pos[id].x;
                    pos_flystop.y = WsTriPos[WsTriPos.Count - 1].st_pos[id].y + 50;
                    // pos_flystop.y = list_xt[0].st_cap_pos.y + 200;
                }
                FlyCfg(EM_FLY_CFG.WS_UP, TriPos, WsTriPos.Count, true, Ofs_On);

                //定位飞拍
              //  VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}Y轴飞拍前其它轴定位!", disc) : string.Format("other axes move before {0}Y axis flyshot!          ({1}Y轴飞拍前其它轴定位!)", englishdisc, disc));
                if (Math.Abs(pos_flystop.x - ax_x.fenc_pos) > 50 || ax_y.fenc_pos > fly_safepos)
                {
                    AXIS axisy = null;
                    if (ax_y.fenc_pos > fly_safepos) axisy = ax_y;
                    res = MT.ZupMove(ref bquit, ref ax_x, pos_flystop.x, ref axisy, fly_safepos, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }
                if (Math.Abs(ax_u2.fcmd_pos) > 8 || Math.Abs(ax_u1.fcmd_pos) > 8)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍放料前转正角度U1:{1} U2:{2}", disc, ax_u1.fcmd_pos, ax_u2.fcmd_pos) : string.Format("{0} XT U axis home before flyshot U1:{2} U2:{3}           ({1}定位到飞拍放料前转正角度U1:{2} U2:{3})", englishdisc, disc, ax_u1.fcmd_pos, ax_u2.fcmd_pos));
                    res = MT.ZupMove(ref bquit, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍放料起飞", disc) : string.Format("{0}  move to flyshot end pos         ({1}定位到飞拍放料起飞)", englishdisc, disc));
                res = MT.ZupMove(ref bquit, ref ax_x, pos_flystop.x, ref ax_y, pos_flystop.y, ref ax_u1, 0, ref ax_u2, 0);
                if (res != EM_RES.OK) return res;

                //等待INP完成
                // res = ax_x.WaitINP(ref bquit, 1000);
                // if (res != EM_RES.OK) return res;

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍放料结束位置X:{1:f3},Y:{2:f3}", disc, pos_flystop.x, pos_flystop.y) : string.Format("{0}  move to flyshot end pos X:{1:f3},Y:{2:f3}           ({1}定位到飞拍放料结束位置X:{1:f3},Y:{2:f3})", disc, pos_flystop.x, pos_flystop.y));
                return EM_RES.OK;
            }
            finally
            {
                ax_y.DisableHcmp(upcam.TriIO.num);
                ax_y.DisableHcmp(dwcam.TriIO.num);
            }

        }

        /// <summary>
        /// 移动到工站定拍方式(下相机定拍上相机飞拍)
        /// </summary>
        public EM_RES NotFlyToWs(ref bool bquit, ref ST_XY pos_flystop, List<WS.MdDat> WsTriPos, out VisionOutPutData[] DownCamdata, bool Ofs_On = true)
        {
            try
            {
                EM_RES res;
                double fly_safepos = list_xt[1].st_rol_cap.y - 180;
                List<ST_XYN> TriPos = new List<ST_XYN>();
                DownCamdata = new VisionOutPutData[4];
                ////配置下拍照
                //FlyCfg(EM_FLY_CFG.MOD_DW, new List<ST_XYN>(), WsTriPos.Count, true, Ofs_On);

                //配置工站拍照
                FeedBackWs.Clear();
                foreach (WS.MdDat md in WsTriPos)
                {
                    TriPos.Add(new ST_XYN(WsTriPos[WsTriPos.Count - 1].st_pos[id].x, md.st_pos[id].y, md.Num - 1));
                    FeedBackWs.Add(md.Clone());
                }
                if (WsTriPos.Count > 0)
                {
                    pos_flystop.x = WsTriPos[WsTriPos.Count - 1].st_pos[id].x;
                    pos_flystop.y = WsTriPos[WsTriPos.Count - 1].st_pos[id].y + 50;
                    // pos_flystop.y = list_xt[0].st_cap_pos.y + 200;
                }
                FlyCfg(EM_FLY_CFG.WS_UP, TriPos, WsTriPos.Count, true, Ofs_On);

                //定位飞拍
                //  VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}Y轴飞拍前其它轴定位!", disc) : string.Format("other axes move before {0}Y axis flyshot!          ({1}Y轴飞拍前其它轴定位!)", englishdisc, disc));
                if (Math.Abs(pos_flystop.x - ax_x.fenc_pos) > 50 || ax_y.fenc_pos > fly_safepos)
                {
                    AXIS axisy = null;
                    if (ax_y.fenc_pos > fly_safepos) axisy = ax_y;
                    res = MT.ZupMove(ref bquit, ref ax_x, pos_flystop.x, ref axisy, fly_safepos, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }

                if (Math.Abs(ax_u2.fcmd_pos) > 8 || Math.Abs(ax_u1.fcmd_pos) > 8)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍放料前转正角度U1:{1} U2:{2}", disc, ax_u1.fcmd_pos, ax_u2.fcmd_pos) : string.Format("{0} XT U axis home before flyshot U1:{2} U2:{3}           ({1}定位到飞拍放料前转正角度U1:{2} U2:{3})", englishdisc, disc, ax_u1.fcmd_pos, ax_u2.fcmd_pos));
                    res = MT.ZupMove(ref bquit, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}下相机定拍", disc) : string.Format("{0}  move to flyshot end pos)", englishdisc, disc));
                

                foreach (XT xt in list_xt)
                {
                    
                    int id = 0;
                    int camid = 0;
                    XT xttemp;
                    if (xt.id == 2 || xt.id == 0)
                    {
                        xttemp = list_xt[1];
                    }
                    else
                    {
                        xttemp = list_xt[0];
                    }

                    if (!(xttemp.cy_zk.isONByChkSen && xttemp.XtMd != null))
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照时，吸嘴上无料不进行拍照", disc));
                        continue;
                    }

                    if (xttemp.id == 2 || xttemp.id == 0)
                    {
                        camid = 0;
                        id = 0;
                    }
                    else
                    {
                        camid = 1;
                        id = 2;
                    }
                                   
                    if (xttemp.cy_zk.isON && xttemp.cy_zk.isONByChkSen)
                    {
                        
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}下相机进行特征拍照", xttemp.disc));
                        res = DwCamAction(ref bquit, xttemp, CONST.ModDwFw[camid], ref DownCamdata[id]);
                        if (res != EM_RES.OK)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍照失败", xttemp.disc));
                            return res;
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}下相机进行二维码拍照", xttemp.disc));
                        res = DwCamAction(ref bquit, xttemp, CONST.DwFindQrCodeFw[camid], ref DownCamdata[id+1]);
                        if (res != EM_RES.OK)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下相机拍二维码失败", xttemp.disc));
                            return res;
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}下相机进行二维码结果为：{1}", xttemp.disc, DownCamdata[id + 1].BarCode));

                    }

                }
                


                res = MT.ZupMove(ref bquit, ref ax_x, pos_flystop.x, ref ax_y, pos_flystop.y, ref ax_u1, 0, ref ax_u2, 0);
                if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}定位到飞拍放料结束位置X:{1:f3},Y:{2:f3}", disc, pos_flystop.x, pos_flystop.y) : string.Format("{0}  move to flyshot end pos X:{1:f3},Y:{2:f3}           ({1}定位到飞拍放料结束位置X:{1:f3},Y:{2:f3})", disc, pos_flystop.x, pos_flystop.y));
                return EM_RES.OK;
            }
            finally
            {
                ax_y.DisableHcmp(upcam.TriIO.num);
                ax_y.DisableHcmp(dwcam.TriIO.num);
            }

        }

        /// <summary>
        /// 马达扫二维码
        /// </summary>
        /// <param name="xtid"></param>
        /// <returns></returns>
        public   EM_RES MotoScan(ref bool bquit,int xtid)
        {
            var posu1 = id == 1 ? PT_SET.MotorAngle3 : PT_SET.MotorAngle1;
            var posu2 = id == 1 ? PT_SET.MotorAngle4 : PT_SET.MotorAngle2;
            double Z = 0;
            if (id==0)
            {
                Z = xtid == 0 ? PT_SET.MotorZ1 : PT_SET.MotorZ2;
            }
            else
            {
                Z = xtid == 0 ? PT_SET.MotorZ3 : PT_SET.MotorZ4;
            }
            
            var xt = list_xt[xtid];
            var ax_y_pos = traybox_fd.motor_barcode_pos[xt.id];
            string barcode = "";
            EM_RES res = EM_RES.OK;
            try
            {
                var com = id == 0 ? MT.COM3 : MT.COM4;
                if (!xt.bCanMotoScan)
                {
                    if (xt.XtMd == null)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, xt.disc + "不需要扫马达二维码吸头空");
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, xt.disc + "不需要扫马达二维码:" + xt.XtMd.motor_barcode);
                    }
                    return EM_RES.OK;
                }

                res = MT.ZupMove(ref bquit, ref ax_y, ax_y_pos, ref ax_z,Z, ref ax_u1, posu1, ref ax_u2, posu2);
                if (res != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, xt.disc + "定位马达二维码失败");
                    return res;
                }
                if (PT_SET.bsunnyqr)
                {
                    REQRACTION:
                    string Code = "ERRORCODE";
                    if(id==0)
                    {
                        res = COM.Sunnnyqr0.QrAction(ref bquit, out Code);
                    }
                    else
                    {
                        res = COM.Sunnnyqr1.QrAction(ref bquit, out Code);
                    }
                    xt.XtMd.bhaveMotoScan = true;//已经马达扫码
                    if (res != EM_RES.OK)
                    {
                        if (PT_SET.bsunnyqralm)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "马达二维码NG" + barcode);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "扫码枪5秒没有返回结果!");
                            MT.ST_WARN warn = new MT.ST_WARN();
                            warning fr_warn = new warning();
                            warn.ok_txt = MultiLanguage.TxtSelct("重新扫码", "GoOn", "Đi tiếp");
                            warn.abort_txt = MultiLanguage.TxtSelct("继续运行", "GoOn", "Đi tiếp");
                            warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop", "Ngừng lại");
                            warn.title = MultiLanguage.TxtSelct("提示:扫码异常", "Tip: Fly Err Back Place Tray Error", "Mẹo: Fly Err Back Place Tray Error");
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "扫码异常" : "Place Tray Error", 10, true);
                            warn.msg = MultiLanguage.TxtSelct("侧边扫码异常!", "Place Tray Error", "Lỗi đặt khay");
                            warn.lb_msg = MultiLanguage.TxtSelct($"当前侧边扫码异常\r\n" +
                                "1、点击重新扫码，则重新进行扫码动作\r\n" +
                                "2、点击继续运行，则不在扫码，二维码默认ErrorCode\r\n" +
                                "3、点击停止运行，则不在扫码，设备停止\r\n");
                            DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                            if (logres == DialogResult.OK)
                            {
                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                goto REQRACTION;

                            }
                            else if (logres == DialogResult.Abort)
                            {
                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                Code = "ERRORCODE";
                            }
                            else if (logres == DialogResult.Cancel)
                            {
                                return EM_RES.QUIT;
                            }
                        }

                        Code = "ERRORCODE";
                        xt.XtMd.motor_barcode = Code;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + $"马达二维码失败{xt.XtMd.motor_barcode}");
                        return EM_RES.OK;
                    }
                    else
                    {
                        Code = Code.Trim();
                        xt.XtMd.motor_barcode = Code;
                        if ((Code.Length) != PT_SET.motorBarcodeDigits && NewSysInf.UserParams.bCheckMotoCodeLength)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + xt.disc + $"马达二维码位数校验失败，扫码长度:{Code.Length},设置长度:{PT_SET.motorBarcodeDigits}");
                            return EM_RES.ERR;
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + xt.disc + $"马达二维码成功{xt.XtMd.motor_barcode}");
                        return EM_RES.OK;
                    }
                }
                else
                {
                    if (com.bComInitOK == false)
                    {
                        com.ReInit();
                        if (com.bComInitOK == false)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, xt.disc + "马达二维码串口初始化失败");
                            return EM_RES.ERR;
                        }
                    }
                    com.bCheckQrLength = NewSysInf.UserParams.bCheckMotoCodeLength;
                    bool bok = com.ReadDataByString(out barcode, 100, PT_SET.Motornum);
                    barcode = barcode.Trim();
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "马达二维码长度为" + barcode.Length);
                    if (barcode.Length > PT_SET.motorBarcodeDigits)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "马达二维码设置的长度为" + PT_SET.motorBarcodeDigits);
                        barcode = "ERRORCODE";
                    }
                    
                    xt.XtMd.bhaveMotoScan = true;//已经马达扫码
                    xt.XtMd.motor_barcode = barcode;
                    if (!bok)
                    {
                        xt.XtMd.motor_barcode = "ERRORCODE";
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "马达二维码NG" + xt.XtMd.motor_barcode);
                        return EM_RES.OK;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + xt.disc + $"马达二维码成功{barcode}");
                        return EM_RES.OK;
                    }
                }
                    
            }
            catch (Exception ee)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, xt.disc + "马达二维码异常" + ee.ToString());
                return EM_RES.ERR;
            }
            finally
            {
                bool mbquit = false;
                var res2 = MT.ZupMove(ref mbquit, ref ax_z, 0, ref ax_u1, 0, ref ax_u2, 0);
            }
            return res;
        }
       
        /// <summary>
        /// 放料于工站
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="WsTriPos">工站</param>
        public EM_RES FlyPlaceWsMod(ref bool bquit, List<WS.MdDat> WsTriPos, WS ws)
        {
            EM_RES res = EM_RES.OK;
            ST_XY Pos_Upcam = new ST_XY();
            int xtzk = 0;
            if (!Demo)
            {
                foreach (XT xt in list_xt)
                {
                    if (xt.cy_zk.isON && xt.cy_zk.isONByChkSen) xtzk++;
                    if (xt.cy_zk.isON && xt.cy_zk.isOFFByChkSen) xt.cy_zk.SetOff();
                }
                if (VAR.ClearMt)
                {

                    if (xtzk == 1 && WsTriPos.Count > 1)
                        WsTriPos.RemoveAt(1);
                }
                else
                {
                    if (WsTriPos.Count > xtzk)
                        return EM_RES.RETRY;
                }
            }

            //进行马达二维码扫描
            if (PT_SET.bmotorphoto)
            {
                try
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "放工站前先马达扫二维码");
                    int i = 0;
                    foreach (var xt in list_xt)
                    {
                        res = MotoScan(ref bquit, i);
                        if (res != EM_RES.OK)
                        {
                            return res;
                        }
                        if (xt.bCanMotoScan)
                        {
                            if (xt.XtMd != null)
                            {
                                if (xt.XtMd.motor_barcode.Contains("ERROR"))
                                {
                                    PT_SET.qrngnum++;
                                }
                                else
                                {
                                    PT_SET.qroknum++;
                                }
                                int allnum = PT_SET.qrngnum + PT_SET.qroknum;
                                double rate = PT_SET.qroknum / (allnum == 0 ? 1 : allnum);
                                if (rate * 100 < PT_SET.Motorrate)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "当前侧边扫码良率低：" + rate);
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "当前侧边扫码良率低!");
                                    MT.ST_WARN warn = new MT.ST_WARN();
                                    warning fr_warn = new warning();
                                    warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "GoOn", "Đi tiếp");
                                    warn.abort_txt = MultiLanguage.TxtSelct("停止运行", "GoOn", "Đi tiếp");
                                    warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop", "Ngừng lại");
                                    warn.title = MultiLanguage.TxtSelct("提示:扫码异常", "Tip: Fly Err Back Place Tray Error", "Mẹo: Fly Err Back Place Tray Error");
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "当前侧边扫码良率低" : "Place Tray Error", 10, true);
                                    warn.msg = MultiLanguage.TxtSelct("当前侧边扫码良率低!", "Place Tray Error", "Lỗi đặt khay");
                                    warn.lb_msg = MultiLanguage.TxtSelct($"当前侧边扫码良率低\r\n" +
                                        "1、点击继续运行，则数据进行清空\r\n" +
                                        "2、点击停止运行，设备停止，请进行确认\r\n");
                                    DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                                    if (logres == DialogResult.OK)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                        PT_SET.qrngnum = 0;
                                        PT_SET.qrngnum = 0;
                                    }
                                    else
                                    {
                                        PT_SET.qrngnum = 0;
                                        PT_SET.qrngnum = 0;
                                        return EM_RES.QUIT;
                                    }
                                }
                            }
                        }
                        i++;
                    }
                }
                catch (Exception ee)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "马达二维码异常" + ee.ToString());
                    return EM_RES.ERR;
                }
                finally
                {
                    //马达二维码绑定成功要将U轴转回来
                    bool mbquit = false;
                    var res1 = MT.ZupMove(ref mbquit, ref ax_z, 0, ref ax_u1, 0, ref ax_u2, 0);

                }              
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "开始进行下飞拍到工站");
            //bool dwcam_recap_err = false;
            for (int n = 0; n < 2; n++)
            {
                //定位到放料位
                res = FlyToWs(ref bquit, ref FeedBackPos, WsTriPos);
                if (res != EM_RES.OK) return res;
                int cntemp = 0;
                cntemp = WsTriPos.Count;
                //等待视觉回传结果 
                if (PT_SET.bDwAddCapQrcode)
                {
                    cntemp = WsTriPos.Count*2;
                }
                WaitCamRes(dwcam, WsTriPos.Count);
                WaitCamRes(upcam, WsTriPos.Count);
                //检查视觉回传结果 

                res = ChkCamRes(dwcam, cntemp);
                if (res != EM_RES.OK)
                {
                    if (n == 0) continue;
                    else return res;
                }
                res = ChkCamRes(upcam, WsTriPos.Count, false);
                if (res != EM_RES.OK)
                {
                    if (n == 0) continue;
                    else return res;
                }
                break;
            }

            //确认有没有模组拍照料失败
            int xtnum;
            bool dwcam_recap_err = false;
            foreach (Cam.VisionTask task in dwcam.List_vs_task_cur)
            {
                xtnum = (dwcam.List_vs_task_cur.IndexOf(task) + 1) % 2;
                
                if (!task.ResData.bOK)
                {
                    if (!PT_SET.bDwAddCapQrcode)
                    {
                        res = MT.ZupMove(ref bquit, ref ax_y, list_xt[xtnum].st_cap_pos.y, ref list_xt[xtnum].ax_u, 0);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}飞拍失败，定位到拍照位置Y:{1}mm->重拍", list_xt[xtnum].disc, list_xt[xtnum].ax_y.fenc_pos) : string.Format("{0} flyshot failed,move to Y:{1}mm->retry photograph               ({0}飞拍失败，定位到拍照位置Y:{1}mm->重拍)", list_xt[xtnum].disc, list_xt[xtnum].ax_y.fenc_pos), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ErrCode: ShowErrMsg.UpDnFlyErr);
                        if (res != EM_RES.OK) return res;
                        Thread.Sleep(50);
                        res = task.TriAndWaitResult(ref bquit, 1000, 1, Demo);
                        if (res != EM_RES.OK)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}重拍失败！", list_xt[xtnum].disc) : string.Format("{0} photograph again failed!        ({0}重拍失败！)", list_xt[xtnum].disc), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id);
                            dwcam_recap_err = true;

                        }
                    }
                    else
                    {
                        xtnum = task.TriPos.n;
                        res = MT.ZupMove(ref bquit, ref ax_y, task.TriPos.y, ref list_xt[xtnum].ax_u, 0);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}飞拍失败，定位到拍照位置Y:{1}mm->重拍", list_xt[xtnum].disc, list_xt[xtnum].ax_y.fenc_pos) : string.Format("{0} flyshot failed,move to Y:{1}mm->retry photograph               ({0}飞拍失败，定位到拍照位置Y:{1}mm->重拍)", list_xt[xtnum].disc, list_xt[xtnum].ax_y.fenc_pos), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ErrCode: ShowErrMsg.UpDnFlyErr);
                        if (res != EM_RES.OK) return res;
                        Thread.Sleep(50);
                        res = task.TriAndWaitResult(ref bquit, 1000, 1, Demo);
                        if (res != EM_RES.OK)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}重拍失败！", list_xt[xtnum].disc) : string.Format("{0} photograph again failed!        ({0}重拍失败！)", list_xt[xtnum].disc), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id);
                            dwcam_recap_err = true;

                        }
                    }
                }
            }
            if (dwcam_recap_err) return EM_RES.CAM_ERR_PLACE_BACK;


            foreach (Cam.VisionTask task in upcam.List_vs_task_cur)
            {
                xtnum = upcam.List_vs_task_cur.IndexOf(task);
                if (!task.ResData.bOK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("工站位置{0}飞拍失败，定位到拍照位置重拍!", WsTriPos[xtnum].Num) : string.Format("WS num{0} flyshot failed,move to photograph pos to take a picture agian!              (工站位置{0}飞拍失败，定位到拍照位置重拍!)", WsTriPos[xtnum].Num), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ErrCode: ShowErrMsg.UpDnFlyErr);
                    res = MT.ZupMove(ref bquit, ref ax_x, FeedBackPos.x, ref ax_y, WsTriPos[xtnum].st_pos[id].y);
                    if (res != EM_RES.OK) return res;
                    Thread.Sleep(50);
                    res = task.TriAndWaitResult(ref bquit, 1000, 1, Demo);
                    if (res != EM_RES.OK)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "拍照错误!" : "Cam ERR", 20, true);
                        MT.ST_WARN warn = new MT.ST_WARN();//增加语言
                        warning fr_warn = new warning();
                        warn.ok_txt = MultiLanguage.TxtSelct("确定", "OK", "VÂNG");
                        warn.ws = null;
                        warn.title = MultiLanguage.TxtSelct("提示:拍照错误!", "Tip: Failed to take a picture!", "Mẹo: Không chụp được ảnh!");
                        warn.msg = MultiLanguage.TxtSelct(
                            $"工站位置{WsTriPos[xtnum].Num}重拍失败,请检查对应的工站位置是否有异物!",
                            $"Workstation location {WsTriPos[xtnum].Num} failed to retake, please check the corresponding station location for foreign objects!",
                            $"Vị trí trạm làm việc {WsTriPos[xtnum].Num} không thể thi lại, vui lòng kiểm tra vị trí trạm tương ứng để tìm vật thể lạ! ");
                        warn.lb_msg = MultiLanguage.TxtSelct(
                            $"提示:工站位置{WsTriPos[xtnum].Num}重拍失败,请检查对应的工站位置是否有异物或物料，按确定键停止,确认无异物后再按运行键继续!",
                            $"Tips:Workstation location {WsTriPos[xtnum].Num} failed to retake. Please check whether there is any foreign object or material in the corresponding station location. " +
                            $"Press the OK key to stop. After confirming that there are no foreign objects, press the Run key to continue!",

                            $"Mẹo: Không thể thực hiện lại vị trí trạm làm việc {0}. Vui lòng kiểm tra xem có bất kỳ vật thể hoặc vật chất lạ nào ở vị trí ga tương ứng hay không. " +
                            $"Press the OK key to stop. After confirming that there are no foreign objects, press the Run key to continue!");
                        MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                        return EM_RES.ERR;
                    }
                }
            }
            

            //清空相机数据
            if (!upcam.FlushOk || !dwcam.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!upcam.FlushOk && !upcam.FlushUpdate)
                    {
                        upcam.FlushUpdate = true;
                        upcam.mAcqFifo.Flush();
                        upcam.FlushOk = true;
                        upcam.FlushUpdate = false;
                    }
                    if (!dwcam.FlushOk && !dwcam.FlushUpdate)
                    {
                        dwcam.FlushUpdate = true;
                        dwcam.mAcqFifo.Flush();
                        dwcam.FlushOk = true;
                        dwcam.FlushUpdate = false;
                    }
                });
                tak.Start();
            }

            #region 先放吸头2后放吸头1    
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "开始进行吸头放工站");
            foreach (Cam.VisionTask dw_task in dwcam.List_vs_task_cur)
            {               
                xtnum = dw_task.TriPos.n;
                if (dw_task.ResData.bOK && dw_task.ResData.bUpdate)
                {
                    if (dw_task.TaskName.Contains("ModDw"))
                    {
                        for (int j = upcam.List_vs_task_cur.Count - 1; j >= 0; j--)
                        {
                            Cam.VisionTask up_task = upcam.List_vs_task_cur[j];
                            if (up_task.ResData.bOK && up_task.ResData.bUpdate)
                            {
                                // Pos_Upcam.x = FeedBackPos.x;
                                //Pos_Upcam.y = WsTriPos[j].st_pos[id].y;  
                                Pos_Upcam.x = up_task.TriPos.x;
                                Pos_Upcam.y = up_task.TriPos.y;

                                bool bdismove = NewSysInf.UserParams.bPlaceWsDis;
                                res = list_xt[xtnum].XtPickOrPlaceMod(ref bquit, Pos_Upcam, up_task.ResData.PosMM, dw_task.ResData.PosMM, WsTriPos[j].st_pos[id].z, Demo, XT.EM_XTFLOW.PLACEMOD, false, PT_SET.bModPasteUp, bdismove);
                                if (res == EM_RES.OK)
                                {
                                    if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
                                    {
                                        WsTriPos[j].bardcode = list_xt[xtnum].XtMd.bardcode;
                                    }
                                    else if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && !PT_SET.bDwAddCapQrcode)
                                        WsTriPos[j].bardcode = dw_task.ResData.BarCode;
                                   
                                   
                                    WsTriPos[j].res = -1;
                                    WsTriPos[j].IsNormal = VAR.Isnormal;
                                    
                                    if (PT_SET.bmotorphoto)
                                    {
                                        var bqrcod = list_xt[xtnum].XtMd.motor_barcode;
                                        if (PT_SET.bsunnyqr)
                                        {
                                            WsTriPos[j].motor_barcode = bqrcod;
                                        }
                                        else
                                        {
                                            if(PT_SET.bsunnyqralm)
                                            {
                                                if ((bqrcod.Length != PT_SET.motorBarcodeDigits && NewSysInf.UserParams.bCheckMotoCodeLength) || (bqrcod == null || bqrcod.Length < 5))
                                                {
                                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}放工站的马达二维码异常，强制下料！", list_xt[xtnum].disc));
                                                    WsTriPos[j].res = 1;
                                                }
                                                else
                                                {
                                                    WsTriPos[j].motor_barcode = bqrcod;
                                                }
                                            }
                                            else
                                            {
                                                WsTriPos[j].motor_barcode = bqrcod;
                                            }

                                        }                                        
                                    }
                                    list_xt[xtnum].XtMd = null;
                                    COUNT_DATA.allcnt[ws.num]++;
                                    if (PT_SET.bNgControl) COUNT_DATA.ngctrlallcnt++;
                                    if (PT_SET.EquipmentMT != 0) COUNT_DATA.CurEquipmentMT++;
                                }
                                else return res;
                                up_task.ResData.bUpdate = false;
                                dw_task.ResData.bUpdate = false;
                                break;
                            }
                        }
                    }
                    else if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN && PT_SET.bDwAddCapQrcode&& dw_task.TaskName.Contains("FindQrCode"))
                        WsTriPos[xtnum].bardcode = dw_task.ResData.BarCode;
                }

                //}
            }

            bUpdateFBPos_UT = true;
            bUpdateFBPos_OKNG = true;
            #endregion
            return EM_RES.OK;
        }

        /// <summary>
        /// 放料于工站（下相机定拍方式）
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="WsTriPos">工站</param>
        public EM_RES NotFlyPlaceWsMod(ref bool bquit, List<WS.MdDat> WsTriPos, WS ws)
        {
            EM_RES res = EM_RES.OK;
            ST_XY Pos_Upcam = new ST_XY();
            int xtzk = 0;
            if (!Demo)
            {
                foreach (XT xt in list_xt)
                {
                    if (xt.cy_zk.isON && xt.cy_zk.isONByChkSen) xtzk++;
                    if (xt.cy_zk.isON && xt.cy_zk.isOFFByChkSen) xt.cy_zk.SetOff();
                }
                if (VAR.ClearMt)
                {

                    if (xtzk == 1 && WsTriPos.Count > 1)
                        WsTriPos.RemoveAt(1);
                }
                else
                {
                    if (WsTriPos.Count > xtzk)
                        return EM_RES.RETRY;
                }
            }

            //进行马达二维码扫描
            if (PT_SET.bmotorphoto)
            {
                try
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "放工站前先马达扫二维码");
                    int i = 0;
                    foreach (var xt in list_xt)
                    {
                        res = MotoScan(ref bquit, i);
                        if (res != EM_RES.OK)
                        {
                            return res;
                        }
                        if (xt.XtMd.motor_barcode.Contains("ERROR"))
                        {
                            PT_SET.qrngnum++;
                        }
                        else
                        {
                            PT_SET.qroknum++;
                        }
                        int allnum = PT_SET.qrngnum + PT_SET.qroknum;
                        double rate = PT_SET.qroknum / (allnum == 0 ? 1 : allnum);
                        if (rate * 100 < PT_SET.Motorrate)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + xt.disc + "当前侧边扫码良率低：" + rate);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "当前侧边扫码良率低!");
                            MT.ST_WARN warn = new MT.ST_WARN();
                            warning fr_warn = new warning();
                            warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "GoOn", "Đi tiếp");
                            warn.abort_txt = MultiLanguage.TxtSelct("停止运行", "GoOn", "Đi tiếp");
                            warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop", "Ngừng lại");
                            warn.title = MultiLanguage.TxtSelct("提示:扫码异常", "Tip: Fly Err Back Place Tray Error", "Mẹo: Fly Err Back Place Tray Error");
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "当前侧边扫码良率低" : "Place Tray Error", 10, true);
                            warn.msg = MultiLanguage.TxtSelct("当前侧边扫码良率低!", "Place Tray Error", "Lỗi đặt khay");
                            warn.lb_msg = MultiLanguage.TxtSelct($"当前侧边扫码良率低\r\n" +
                                "1、点击继续运行，则数据进行清空\r\n" +
                                "2、点击停止运行，设备停止，请进行确认\r\n");
                            DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                            if (logres == DialogResult.OK)
                            {
                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                PT_SET.qrngnum = 0;
                                PT_SET.qrngnum = 0;
                            }
                            else
                            {
                                PT_SET.qrngnum = 0;
                                PT_SET.qrngnum = 0;
                                return EM_RES.QUIT;
                            }
                        }
                        i++;
                    }

                }
                catch (Exception ee)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "马达二维码异常" + ee.ToString());
                    return EM_RES.ERR;
                }
                finally
                {
                    //马达二维码绑定成功要将U轴转回来
                    bool mbquit = false;
                    var res1 = MT.ZupMove(ref mbquit, ref ax_u1, 0, ref ax_u2, 0);

                }
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "开始进行下拍到工站");

            VisionOutPutData[] DownCamdata = new VisionOutPutData[4];
            for (int n = 0; n < 2; n++)
            {
                //定位到放料位
                res = NotFlyToWs(ref bquit, ref FeedBackPos, WsTriPos, out DownCamdata);
                if (res != EM_RES.OK) return res;
                int cntemp = 0;
                cntemp = WsTriPos.Count;
                //等待视觉回传结果 
                if (PT_SET.bDwAddCapQrcode)
                {
                    cntemp = WsTriPos.Count * 2;
                }

                WaitCamRes(upcam, WsTriPos.Count);
                res = ChkCamRes(upcam, WsTriPos.Count, false);
                if (res != EM_RES.OK)
                {
                    if (n == 0) continue;
                    else return res;
                }
                break;
            }

            //确认有没有模组拍照料失败
            int xtnum;

            foreach (Cam.VisionTask task in upcam.List_vs_task_cur)
            {
                xtnum = upcam.List_vs_task_cur.IndexOf(task);
                if (!task.ResData.bOK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("工站位置{0}飞拍失败，定位到拍照位置重拍!", WsTriPos[xtnum].Num) : string.Format("WS num{0} flyshot failed,move to photograph pos to take a picture agian!              (工站位置{0}飞拍失败，定位到拍照位置重拍!)", WsTriPos[xtnum].Num), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.UpDownLoad + id, ErrCode: ShowErrMsg.UpDnFlyErr);
                    res = MT.ZupMove(ref bquit, ref ax_x, FeedBackPos.x, ref ax_y, WsTriPos[xtnum].st_pos[id].y);
                    if (res != EM_RES.OK) return res;
                    Thread.Sleep(50);
                    res = task.TriAndWaitResult(ref bquit, 1000, 1, Demo);
                    if (res != EM_RES.OK)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "拍照错误!" : "Cam ERR", 20, true);
                        MT.ST_WARN warn = new MT.ST_WARN();//增加语言
                        warning fr_warn = new warning();
                        warn.ok_txt = MultiLanguage.TxtSelct("确定", "OK", "VÂNG");
                        warn.ws = null;
                        warn.title = MultiLanguage.TxtSelct("提示:拍照错误!", "Tip: Failed to take a picture!", "Mẹo: Không chụp được ảnh!");
                        warn.msg = MultiLanguage.TxtSelct(
                            $"工站位置{WsTriPos[xtnum].Num}重拍失败,请检查对应的工站位置是否有异物!",
                            $"Workstation location {WsTriPos[xtnum].Num} failed to retake, please check the corresponding station location for foreign objects!",
                            $"Vị trí trạm làm việc {WsTriPos[xtnum].Num} không thể thi lại, vui lòng kiểm tra vị trí trạm tương ứng để tìm vật thể lạ! ");
                        warn.lb_msg = MultiLanguage.TxtSelct(
                            $"提示:工站位置{WsTriPos[xtnum].Num}重拍失败,请检查对应的工站位置是否有异物或物料，按确定键停止,确认无异物后再按运行键继续!",
                            $"Tips:Workstation location {WsTriPos[xtnum].Num} failed to retake. Please check whether there is any foreign object or material in the corresponding station location. " +
                            $"Press the OK key to stop. After confirming that there are no foreign objects, press the Run key to continue!",

                            $"Mẹo: Không thể thực hiện lại vị trí trạm làm việc {0}. Vui lòng kiểm tra xem có bất kỳ vật thể hoặc vật chất lạ nào ở vị trí ga tương ứng hay không. " +
                            $"Press the OK key to stop. After confirming that there are no foreign objects, press the Run key to continue!");
                        MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                        return EM_RES.ERR;
                    }
                }
            }


            //清空相机数据
            if (!upcam.FlushOk || !dwcam.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!upcam.FlushOk && !upcam.FlushUpdate)
                    {
                        upcam.FlushUpdate = true;
                        upcam.mAcqFifo.Flush();
                        upcam.FlushOk = true;
                        upcam.FlushUpdate = false;
                    }
                    if (!dwcam.FlushOk && !dwcam.FlushUpdate)
                    {
                        dwcam.FlushUpdate = true;
                        dwcam.mAcqFifo.Flush();
                        dwcam.FlushOk = true;
                        dwcam.FlushUpdate = false;
                    }
                });
                tak.Start();
            }

            #region 先放吸头2后放吸头1    

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "开始进行吸头放工站");
            for (int j = upcam.List_vs_task_cur.Count - 1; j >= 0; j--)
            {
                if(upcam.List_vs_task_cur.Count==1)
                {
                    xtnum = 1;
                }
                else
                {
                    xtnum = j > 1 ? 1 : j;
                }
                
                int DataNum = 0;
                if (j == 1)
                {
                    DataNum = 2;
                }
                else
                {
                    DataNum = 0;
                }
                if (upcam.List_vs_task_cur.Count == 1)
                {
                    DataNum = 2;                  
                }
                Cam.VisionTask up_task = upcam.List_vs_task_cur[j];
                if (up_task.ResData.bOK && up_task.ResData.bUpdate)
                {
                    Pos_Upcam.x = up_task.TriPos.x;
                    Pos_Upcam.y = up_task.TriPos.y;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "J：" + j + "Datanum:" + DataNum + "xtnum:" + xtnum);
                    bool bdismove = NewSysInf.UserParams.bPlaceWsDis;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "上相机数据："+up_task.ResData.PosMM + "下相机数据" + DownCamdata[DataNum].PosMM);
                    res = list_xt[xtnum].XtPickOrPlaceMod(ref bquit, Pos_Upcam, up_task.ResData.PosMM, DownCamdata[DataNum].PosMM, WsTriPos[j].st_pos[id].z, Demo, XT.EM_XTFLOW.PLACEMOD, false, PT_SET.bModPasteUp, bdismove);
                    if (res == EM_RES.OK)
                    {
                        if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
                        {
                            WsTriPos[j].bardcode = list_xt[xtnum].XtMd.bardcode;
                        }
                        else if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.DW_SCAN)
                        {
                            WsTriPos[j].bardcode = DownCamdata[DataNum + 1].BarCode;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "二维码为:" + DownCamdata[DataNum + 1].BarCode.ToString());
                        }

                        WsTriPos[j].res = -1;
                        WsTriPos[j].IsNormal = VAR.Isnormal;

                        if (PT_SET.bmotorphoto)
                        {
                            string bqrcod = list_xt[xtnum].XtMd.motor_barcode;
                            if (PT_SET.bsunnyqr)
                            {
                                WsTriPos[j].motor_barcode = bqrcod;
                            }
                            else
                            {
                                if ((bqrcod.Length != PT_SET.motorBarcodeDigits && NewSysInf.UserParams.bCheckMotoCodeLength) || (bqrcod == null || bqrcod.Length < 5))
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}放工站的马达二维码异常，强制下料！", list_xt[xtnum].disc));
                                    WsTriPos[j].res = 1;
                                }
                                else
                                {
                                    WsTriPos[j].motor_barcode = bqrcod;

                                }
                            }
                                

                        }
                        list_xt[xtnum].XtMd = null;
                        COUNT_DATA.allcnt[ws.num]++;
                        if (PT_SET.bNgControl) COUNT_DATA.ngctrlallcnt++;
                        if (PT_SET.EquipmentMT != 0) COUNT_DATA.CurEquipmentMT++;
                    }
                    else return res;
                    up_task.ResData.bUpdate = false;
                    //break;
                }
            }

            bUpdateFBPos_UT = true;
            bUpdateFBPos_OKNG = true;
            #endregion
            return EM_RES.OK;
        }

        /// <summary>
        /// 放模组于工站流程
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="flycam"></param>
        /// <param name="placemd"></param>
        /// <returns></returns>
        public EM_RES PlaceMdToWs(ref bool bquit, List<WS.MdDat> placemd, WS ws)
        {
            EM_RES res = EM_RES.OK;
            ////放料前旋转90°，不管是否有料；
            //if (PT_SET.isopen_degree)
            //{
            //    res = MT.ZupMove(ref bquit, ref ax_u1, PT_SET.degree, ref ax_u2, PT_SET.degree);
            //    if (res != EM_RES.OK) return res;
            //}
            if (PT_SET.bDwAddCapQrcode)
            {
                res = NotFlyPlaceWsMod(ref bquit, placemd, ws);
            }
            else
            {
                res = FlyPlaceWsMod(ref bquit, placemd, ws);
            }
                
           // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}结束,{1}", disc, res.ToString()) : string.Format("{0} end,{2} ({1}结束,{2})", englishdisc, disc, res.ToString()));
            if (res == EM_RES.OK)
            {
                ErrTryPickCnt = 0;
     
            }

            return res;
        }

        #endregion

        #region 取料盘模组函数
        /// <summary>
        /// 飞拍取料
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES FlyTrayPickMod(ref bool bquit, WS ws)
        {
            //List<Cam.VisionTask> upcam_task = new List<Cam.VisionTask>();
            ST_XY Pos_Upcam = new ST_XY();
            ST_XYZA endpos = new ST_XYZA();
            ST_XYA xt_vs_dat = new ST_XYA(-10000, -10000, 0);
            EM_RES res = FlyCamToTray(ref bquit, ws, traybox_fd, ref endpos);
            if (res != EM_RES.OK) return res;
            //排列数据
            if (modupcam_task.Count > 1)
            {
                modupcam_task.Sort((a, b) =>
                {
                    if (a.TriPos.y < b.TriPos.y) return 1;
                    else return -1;
                });
            }
            //吸料
            if (PT_SET.xt1firsten)
            {
                //for (int i = list_xt.Count - 1; i >= 0; i--)
                if (PT_SET.bAddCapQrcode)
                {
                    for (int i = 0; i <= list_xt.Count - 1; i++)
                    {
                        if ((list_xt[i].cy_zk.isOFFByChkSen && !Demo) || Demo)
                        {
                            for (int j = modupcam_task.Count - 1; j >= 0; j--)
                            {
                                Cam.VisionTask uptask = modupcam_task[j];
                                Cam.VisionTask qrcodeTask = null;
                                if (uptask.TaskName != CONST.ModUpFw) continue;
                                if (j % 2 == 0)
                                {
                                    qrcodeTask = modupcam_task[j + 1];
                                }
                                else
                                {
                                    qrcodeTask = modupcam_task[j - 1];
                                }
                                if (qrcodeTask.TaskName != CONST.FindQrCodeFw) return EM_RES.ERR;
                                if (uptask.ResData.bOK && uptask.ResData.bUpdate && qrcodeTask.ResData.bOK && qrcodeTask.ResData.bUpdate)
                                {
                                    Product.Tray.PosInf posinf = traybox_fd.tray_cur.list_pos[uptask.TriPos.n];
                                    Pos_Upcam.x = uptask.TriPos.x;
                                    Pos_Upcam.y = uptask.TriPos.y;
                                   var  curBarCode= qrcodeTask.ResData.BarCode;
                                    res = list_xt[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, uptask.ResData.PosMM, xt_vs_dat, posinf.Pos[id].z, Demo);
                                    if (res == EM_RES.OK)
                                    {
                                        MT.Move(ref bquit, ref list_xt[i].ax_u, 0);
                                        list_xt[i].XtMd = posinf.md;
                                        COUNT_DATA.SuctionAllcnt++;
                                        list_xt[i].XtMd.bardcode = curBarCode;
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}飞拍取料bAddCapQrcode模式------barcode---{1}!", list_xt[i].disc, list_xt[i].XtMd.bardcode));
                                        posinf.md = null;

                                    }
                                    else return res;
                                    uptask.ResData.bUpdate = false;
                                    qrcodeTask.ResData.bUpdate = false;
                                    break;
                                }
                            }
                        }

                    }
                }
                else
                {
                    for (int i = 0; i <= list_xt.Count - 1; i++)
                    {
                        if ((list_xt[i].cy_zk.isOFFByChkSen && !Demo) || Demo)
                        {
                            for (int j = modupcam_task.Count - 1; j >= 0; j--)
                            //foreach (Cam.VisionTask uptask in modupcam_task)
                            {
                                Cam.VisionTask uptask = modupcam_task[j];
                                if (uptask.ResData.bOK && uptask.ResData.bUpdate)
                                {
                                    Product.Tray.PosInf posinf = traybox_fd.tray_cur.list_pos[uptask.TriPos.n];
                                    Pos_Upcam.x = uptask.TriPos.x;
                                    Pos_Upcam.y = uptask.TriPos.y;
                                    var curBarCode = uptask.ResData.BarCode;
                                    var curTaskName = uptask.TaskName;
                                  //  VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"{list_xt[i].disc}取料前任务名称{curTaskName}二维码{curBarCode}模组序号{uptask.TriPos.n}!");
                                    res = list_xt[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, uptask.ResData.PosMM, xt_vs_dat, posinf.Pos[id].z, Demo);
                                    if (res == EM_RES.OK)
                                    {
                                        list_xt[i].XtMd = posinf.md;
                                        COUNT_DATA.SuctionAllcnt++;
                                        if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
                                        {
                                            list_xt[i].XtMd.bardcode = curBarCode;
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}飞拍取料------barcode---{1}!", list_xt[i].disc, list_xt[i].XtMd.bardcode));
                                        }
                                        posinf.md = null;
                                       // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"{list_xt[i].disc}取料后任务名称{uptask.TaskName}二维码{ uptask.ResData.BarCode}模组序号{uptask.TriPos.n}!");
                                    }
                                    else return res;
                                    uptask.ResData.bUpdate = false;
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                if (PT_SET.bAddCapQrcode)
                {
                    for (int i = list_xt.Count - 1; i >= 0; i--)
                    {
                        if ((list_xt[i].cy_zk.isOFFByChkSen && !Demo) || Demo)
                        {
                            foreach (Cam.VisionTask uptask in modupcam_task)
                            {
                                if (uptask.TaskName != CONST.ModUpFw) continue;
                                Cam.VisionTask qrcodeTask = null;
                                if (modupcam_task.IndexOf(uptask) % 2 == 0)
                                {
                                    qrcodeTask = modupcam_task[modupcam_task.IndexOf(uptask) + 1];

                                }
                                else
                                {
                                    qrcodeTask = modupcam_task[modupcam_task.IndexOf(uptask) - 1];
                                }
                                if (qrcodeTask.TaskName != CONST.FindQrCodeFw) return EM_RES.ERR;
                                if (uptask.ResData.bOK && uptask.ResData.bUpdate && qrcodeTask.ResData.bOK && qrcodeTask.ResData.bUpdate)
                                {
                                    Product.Tray.PosInf posinf = traybox_fd.tray_cur.list_pos[uptask.TriPos.n];
                                    Pos_Upcam.x = uptask.TriPos.x;
                                    Pos_Upcam.y = uptask.TriPos.y;
                                    var curBarCode = qrcodeTask.ResData.BarCode;
                                    res = list_xt[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, uptask.ResData.PosMM, xt_vs_dat, posinf.Pos[id].z, Demo);
                                    if (res == EM_RES.OK)
                                    {
                                        list_xt[i].XtMd = posinf.md;
                                        COUNT_DATA.SuctionAllcnt++;
                                        list_xt[i].XtMd.bardcode = curBarCode;
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}飞拍取料PT_SET.bAddCapQrcode------barcode---{1}!", list_xt[i].disc, list_xt[i].XtMd.bardcode));
                                        posinf.md = null;

                                    }
                                    else return res;
                                    uptask.ResData.bUpdate = false;
                                    qrcodeTask.ResData.bUpdate = false;
                                    break;
                                }
                            }
                        }

                    }
                }
                else
                {
                    for (int i = list_xt.Count - 1; i >= 0; i--)
                    {
                        if ((list_xt[i].cy_zk.isOFFByChkSen && !Demo) || Demo)
                        {
                            foreach (Cam.VisionTask uptask in modupcam_task)
                            {
                                if (uptask.ResData.bOK && uptask.ResData.bUpdate)
                                {
                                    Product.Tray.PosInf posinf = traybox_fd.tray_cur.list_pos[uptask.TriPos.n];
                                    Pos_Upcam.x = uptask.TriPos.x;
                                    Pos_Upcam.y = uptask.TriPos.y;
                                var curBarCode=  uptask.ResData.BarCode;
                                    var curTaskName = uptask.TaskName;
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"{list_xt[i].disc}取料前任务名称{curTaskName}二维码{curBarCode}模组编号{uptask.TriPos.n}!");
                                    res = list_xt[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, uptask.ResData.PosMM, xt_vs_dat, posinf.Pos[id].z, Demo);
                                    if (res == EM_RES.OK)
                                    {
                                        list_xt[i].XtMd = posinf.md.Clone();
                                        COUNT_DATA.SuctionAllcnt++;
                                        if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
                                        {                                        
                                            list_xt[i].XtMd.bardcode = curBarCode;
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}飞拍取料------barcode---{1}!", list_xt[i].disc, list_xt[i].XtMd.bardcode));
                                        }
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"{list_xt[i].disc}取料后任务名称{uptask.TaskName}二维码{ uptask.ResData.BarCode}模组编号{uptask.TriPos.n}!");
                                        posinf.md = null;
                                       

                                    }
                                    else return res;
                                    uptask.ResData.bUpdate = false;
                                    break;
                                }
                            }
                        }

                    }
                }

            }




            return EM_RES.OK;
        }

        /// <summary>
        /// 料盘取料
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="IsFly"></param>
        /// <param name="Try"></param>
        /// <returns></returns>
        public EM_RES TrayPickMod(ref bool bquit, ref bool IsFly, WS ws)
        {
            EM_RES res = EM_RES.OK;
            EM_RES ret = EM_RES.OK;
            string str = "";
            string barcodetemp = "";
            int trycnt = 0;
            bool search = false;
            int TryPickCnt = 0, TryCnt = 0;
            bool pickone = false;
            List<Product.Tray.PosInf> ListPickMod = new List<Product.Tray.PosInf>();
            if (!traybox_fd.IsReady) return EM_RES.ABORT;
            if (!Demo && list_xt[0].cy_zk.isONByChkSen && list_xt[0].XtMd != null && list_xt[1].cy_zk.isONByChkSen && list_xt[1].XtMd != null) return EM_RES.OK;
            //判断料盘状态
            if (traybox_fd.tray_cur == null) return EM_RES.PARA_OUTOFRANG;
            ListPickMod = traybox_fd.tray_cur.GetPosList();
            if (ListPickMod != null && ListPickMod.Count == 0) return EM_RES.END;
            GetWsPosTeam(ref CurPlaceMod, ws, Product.EM_CM_RES.EMPTY, FindMod);
            int xtzkcnt = 0;
            if (!Demo)
            {
                foreach (XT xt in list_xt)
                {
                    if (xt.cy_zk.isON && xt.cy_zk.isONByChkSen) xtzkcnt++;
                    if (xt.cy_zk.isON && !xt.cy_zk.isONByChkSen) xt.cy_zk.SetOff();
                }
                if (CurPlaceMod.Count == xtzkcnt) return EM_RES.OK;
            }
            if (CurPlaceMod.Count == 1) pickone = true;
            if (traybox_fd.ischangetray && PT_SET.TrayBarcodeEn)
            {
                traybox_fd.ischangetray = false;
                ret = MT.Move(ref VAR.gsys_set.bquit, ref MT.AXIS_UDL_FD_X, id == COM.UDLoad1.id ? traybox_fd.tray_barcode_a1 : traybox_fd.tray_barcode_a2);
                if (ret != EM_RES.OK) return ret;
                ret = MT.Move(ref VAR.gsys_set.bquit, ref ax_x, id == COM.UDLoad1.id ? traybox_fd.tray_barcode_x1 : traybox_fd.tray_barcode_x2, ref ax_y, id == COM.UDLoad1.id ? traybox_fd.tray_barcode_y1 : traybox_fd.tray_barcode_y2);
                if (ret != EM_RES.OK) return ret;
                while (trycnt++ < 3)
                {
                    if (id == COM.UDLoad1.id)
                        ret = COM.CamUp1.FindTaskTriAndWait(CONST.TrayUpFw, Demo);
                    else
                        ret = COM.CamUp2.FindTaskTriAndWait(CONST.TrayUpFw, Demo);
                    if (ret != EM_RES.OK) continue;
                    Cam.VisionTask task = id == COM.UDLoad1.id ? COM.CamUp1.curTask : COM.CamUp2.curTask;
                    if (task.ResData.BarCode == "") continue;
                    barcodetemp = task.ResData.BarCode;
                    if (barcodetemp != barcodestr)
                    {

                        bool delete = database.DeleteIndex(barcodestr, out str);
                        if (!delete)
                        {
                            delete = database.DeleteIndex(barcodestr, out str);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, str);
                        }
                        barcodestr = barcodetemp;
                    }
                    search = database.IsExitIndex(barcodestr, out str);
                    if (!search)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, str);
                        break;
                        ////界面提示                 
                        //VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "未震动", 10, true);
                        //MT.ST_WARN warn = new MT.ST_WARN();
                        //warning fr_warn = new warning();
                        //warn.ws = null;
                        //warn.ok_txt = "更换料盘";
                        //warn.cancle_txt = "停止运行";
                        //warn.abort_txt = "重新拍照";
                        //warn.title = "提示:供料盘未震动!";
                        //warn.msg = disc + "供料盘没有震动请确认后按确定继续!";
                        //warn.lb_msg = "提示:" + "供料盘没有震动请确认后按确定继续!";
                        //DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.Null);
                        //if (logres == DialogResult.OK)
                        //{
                        //    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        //    return EM_RES.END;
                        //}
                        //else if (logres == DialogResult.Cancel)
                        //{
                        //    res = EM_RES.ERR;
                        //    return res;
                        //}
                        //else if (logres == DialogResult.Abort)
                        //{
                        //    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        //    continue;
                        //}
                    }
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "当前供料盘已进行过震动。");
                    break;
                }
                if (trycnt == 4)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("连续3次拍不到料盘二维码"));
                    //界面提示                 
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "未震动", 10, true);
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();//增加语言
                    warn.ws = null;
                    warn.ok_txt = MultiLanguage.TxtSelct("更换料盘", "Replace tray", "Thay khay");
                    warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                    warn.title = MultiLanguage.TxtSelct("提示:供料盘未震动!", "Tip: Feed tray is not shaken!", "Tip: Feed tray is not shaken!");
                    warn.msg = MultiLanguage.TxtSelct(
                        disc + "供料盘没有震动请确认后按确定继续!",
                        englishdisc + "The feed tray is not vibrating, please confirm after pressing 'Replace tray' key to continue!",
                        disc + "Khay nạp không rung, vui lòng xác nhận sau khi nhấn phím 'Thay khay' để tiếp tục!");
                    warn.lb_msg = MultiLanguage.TxtSelct("" +
                        "提示:" + disc + "供料盘没有震动请确认后按确定继续!",
                        "Tip:" + englishdisc + "The feed tray is not vibrating, please confirm after pressing 'Replace tray' key to continue!",
                        "Mẹo:" + disc + "Khay nạp không rung, vui lòng xác nhận sau khi nhấn phím 'Thay khay' để tiếp tục!");
                    DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.Null);
                    if (logres == DialogResult.OK)
                    {
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, "运行", 0, true);
                        return EM_RES.END;
                    }
                    else if (logres == DialogResult.Cancel)
                    {
                        res = EM_RES.ERR;
                        return res;
                    }
                }

            }



            //取料
            while (TryCnt++ < 5)
            {
                if (!IsFly)//|| ax_y.fenc_pos < list_xt[0].st_cap_pos.y)
                {
                    foreach (XT xt in list_xt)
                    {
                        //如果只有一个目标，根据需求吸料
                        if (pickone && !PT_SET.xt1firsten && xt.id == list_xt[0].id)
                            continue;
                        else if (pickone && PT_SET.xt1firsten && xt.id == list_xt[1].id)
                            continue;
                        if (!Demo)
                        {
                            if ((xt.cy_zk.isONByChkSen && xt.XtMd == null) || (!xt.cy_zk.isONByChkSen && xt.XtMd != null))
                            {
                                xt.XtMd = null;
                                xt.cy_zk.SetOff();
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? disc + "检查到（真空达到阀值但没有模组信息)或(有模组信息真空达不到阀值)" : englishdisc + "checked (vacuum reaches threshold but no module information) or (with module information vacuum cannot reach threshold)                    (检查到（真空达到阀值但没有模组信息)或(有模组信息真空达不到阀值))", DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.UpDownLoad + id);
                                Thread.Sleep(100);
                            }
                        }
                        if ((Demo && xt.XtMd == null) || (!xt.cy_zk.isONByChkSen && xt.XtMd == null && !Demo))
                        {
                            ListPickMod = traybox_fd.tray_cur.GetPosList();
                            if (ListPickMod != null && ListPickMod.Count == 0)
                            {
                                if (VAR.ClearMt)
                                {
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "清料提示!" : "Clear warning", 20, true);
                                    MT.ST_WARN st_warn = new MT.ST_WARN();//增加语言
                                    warning fr_warn = new warning();
                                    st_warn.ok_txt = MultiLanguage.TxtSelct("清料", "Clear", "清料");
                                    st_warn.ws = null;
                                    st_warn.cancle_txt = MultiLanguage.TxtSelct("取消", "Cancel", "Cancel");
                                    st_warn.msg = MultiLanguage.TxtSelct("清料提示!", "Clear tip", "Rõ ràng tiền boa");
                                    st_warn.title = MultiLanguage.TxtSelct("提示:清料提示!", "Tip:Clear", "Mẹo: Rõ ràng");
                                    st_warn.lb_msg = MultiLanguage.TxtSelct(
                                        "提示:是否要清料?\r\n  " +
                                        "1.按清料键进行清料流程",
                                        "Tip: Do you want to clear the material?\r\n" +
                                        "1.Press the 'Clear' button to clear the material",
                                        "Mẹo: Bạn có muốn xóa tài liệu không?\r\n" +
                                        "1.Nhấn nút 'Clear' để xóa vật liệu");
                                    DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.Null);
                                    if (DialogResult.OK == logres)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                        return EM_RES.NEXT;
                                    }
                                    else
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                        return EM_RES.END;
                                    }

                                }
                                else
                                {
                                    return EM_RES.END;
                                }

                            }
                            string barcode;
                            //定拍取料
                            res = xt.XtCapMovMod(ref bquit, out barcode, ListPickMod[0].Pos[id], CONST.ModUpFw, CurPlaceMod[0].st_pos[id].x, XT.EM_XTFLOW.PICKMOD, true, Demo, capQrcodePos: traybox_fd.tray_cur.list_AddCapQrcodepos[ListPickMod[0].idx].Pos[id]);
                            if (res == EM_RES.OK)
                            {
                                //更新料盘
                                TryPickCnt = 0;
                                xt.XtMd = ListPickMod[0].md;
                                COUNT_DATA.SuctionAllcnt++;
                                if (PT_SET.BarcodeMode == (int)PT_SET.BAR_SCAN.UP_SCAN)
                                {
                                    xt.XtMd.bardcode = upcam.curTask.ResData.BarCode;
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}定拍取料------barcode---{1}!", xt.disc, xt.XtMd.bardcode));
                                }
                                if (PT_SET.bAddCapQrcode)
                                {
                                    xt.XtMd.bardcode = barcode;
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}定拍取料bAddCapQrcode------barcode---{1}!", xt.disc, xt.XtMd.bardcode));
                                }
                                ListPickMod[0].md = null;

                            }
                            else if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR)
                            {
                                ListPickMod[0].md.res = (int)Product.EM_CM_RES.CAMERR;
                                TryPickCnt++;
                                COUNT_DATA.SuctionAllcnt++;
                                COUNT_DATA.SuctionErrcnt++;
                                if (TryPickCnt > 4)
                                {
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "吸料异常!" : "Draw ERR", 20, true);
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "料盘吸料异常", DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.UpDownLoad + id);
                                    MT.ST_WARN st_warn = new MT.ST_WARN();
                                    warning fr_warn = new warning();//增加语言
                                    st_warn.ok_txt = MultiLanguage.TxtSelct("归位确认", "Home", "cài lại");
                                    st_warn.ws = null;
                                    st_warn.cancle_txt = MultiLanguage.TxtSelct("更换料盘", "Replace tray", "Thay khay");
                                    st_warn.abort_txt = MultiLanguage.TxtSelct("清料", "Clear", "Thông thoáng");
                                    st_warn.title = MultiLanguage.TxtSelct("提示:吸料异常!", "Tip: Abnormal suction!", "Mẹo: Hút bất thường!");
                                    st_warn.msg = MultiLanguage.TxtSelct($"{xt.disc}连续取料5次模组失败!", $"{xt.disc} failed to pick mod 5 times", $"{xt.disc} không chọn được mod 5 lần");
                                    st_warn.lb_msg = MultiLanguage.TxtSelct(
                                        $"提示:{xt.disc}连续取料5次失败,请确认料盘是否放反!\r\n " +
                                        $"1.如果放反请按更换料盘键进行料盘更换!\r\n" +
                                        $"2.如没有放反请按归位确认键分析原因,注意请等轴归位完成后再打开门!\r\n" +
                                        $"3.如清料请按清料键!",

                                        $"Tip:{xt.disc} failed to pick mod 5 times.Please confirm whether the tray is turned upside down!\r\n" +
                                        $"1.If it is reversed, press the Replace Tray key to change the tray!\r\n" +
                                        $"2.If there is no reversal, please press the home key to analyze the cause. Note that please wait for the axis to complete the homing before opening the door!\r\n" +
                                        $"3.If you want to clear the material, please press the key",

                                        $"Mẹo: {xt.disc} không chọn được mod 5 lần. Vui lòng xác nhận xem khay có bị lật ngược hay không!\r\n" +
                                        $"1.Nếu nó bị đảo ngược, hãy nhấn phím Replace Tray để thay đổi khay!\r\n" +
                                        $"2.Nếu không đảo ngược được, vui lòng nhấn phím Home để phân tích nguyên nhân. Lưu ý rằng vui lòng đợi cho trục hoàn thành homing trước khi mở cửa!\r\n" +
                                        $"3.Nếu bạn muốn xóa vật liệu, vui lòng nhấn phím");
                                    DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                                    if (DialogResult.Cancel == logres)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                        return EM_RES.END;

                                    }
                                    else if (DialogResult.Abort == logres)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                        VAR.ClearMt = true;
                                        return EM_RES.NEXT;

                                    }
                                    return res;
                                }
                                else continue;
                            }
                            else return res;
                        }
                    }

                }
                else
                {
                    //飞拍取料
                    IsFly = false;
                    res = FlyTrayPickMod(ref bquit, ws);
                    if (res != EM_RES.OK)
                    {
                        if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR) continue;
                        else return res;
                    }
                }

                if ((((list_xt[0].cy_zk.isONByChkSen && list_xt[0].XtMd != null && list_xt[1].cy_zk.isONByChkSen && list_xt[1].XtMd != null && !pickone) || (list_xt[1].cy_zk.isONByChkSen && list_xt[1].XtMd != null && pickone)) && !Demo) || (Demo && list_xt[0].XtMd != null && list_xt[1].XtMd != null) || VAR.ClearMt)
                {
                    List<Product.Tray.PosInf> ListPickModnew1 = new List<Product.Tray.PosInf>();//增加提前下料
                    ListPickModnew1 = traybox_fd.tray_cur.GetPosList();
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量：" + ListPickModnew1.Count);
                    if (ListPickModnew1 != null && ListPickModnew1.Count == 0)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量为0，提前下料");
                        //更换料盘
                        if (traybox_fd.IsReady)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量为0，开始提前下料");
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("请更换{0}====================", traybox_fd.disc) : string.Format("Please replace {0}====================               (请更换{1}====================)", traybox_fd.name, traybox_fd.disc));
                            traybox_fd.IsReady = false;
                            Task fdtsk = new Task(() => { traybox_fd.Tray_Action(Demo); });
                            fdtsk.Start();
                        }
                    }
                    return EM_RES.OK;
                }
            }
            if (TryCnt >= 5)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? disc + "连续取料尝试次数超过5次!" : englishdisc + "Pick failed five times!       (连续取料尝试次数超过5次!)", DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.UpDownLoad + id);
            }

            List<Product.Tray.PosInf> ListPickModnew = new List<Product.Tray.PosInf>();//增加提前下料
            ListPickModnew = traybox_fd.tray_cur.GetPosList();
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量："+ ListPickModnew.Count);
            if (ListPickModnew != null && ListPickModnew.Count == 0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量为0，提前下料" );
                //更换料盘
                if (traybox_fd.IsReady)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "取料后剩余物料数量为0，开始提前下料");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("请更换{0}====================", traybox_fd.disc) : string.Format("Please replace {0}====================               (请更换{1}====================)", traybox_fd.name, traybox_fd.disc));
                    traybox_fd.IsReady = false;
                    Task fdtsk = new Task(() => { traybox_fd.Tray_Action(Demo); });
                    fdtsk.Start();
                }
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 料盘取料流程
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public EM_RES PickMdToTray(ref bool bquit, ref bool flycam, WS ws)
        {
            //取料
            EM_RES res = EM_RES.OK;
            // bool ChangeFdTray = false;
            //料盘取料前 旋转0°
            if (PT_SET.isopen_degree)
            {
                res = MT.ZupMove(ref bquit, ref ax_u1, 0, ref ax_u2, 0);
                if (res != EM_RES.OK) return res;
            }
            while (!bquit)
            {
                // flycam = false;
                res = TrayPickMod(ref bquit, ref flycam, ws);
                if (res != EM_RES.ABORT)
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上料线程取料结束,{0}", res.ToString()) : string.Format("Upload thread end,{0}       (上料线程取料结束,{0})", res.ToString()));

                if (res == EM_RES.OK)
                {
                    #region old
                    ////定位下一个待料料盘的位置
                    //if (traybox_fd.tray_cur != null)
                    //    ListPickMod = traybox_fd.tray_cur.GetPosList();
                    //else
                    //    ListPickMod.Clear();

                    //if (ListPickMod.Count > 0)
                    //{
                    //    GetWsListPos(ref NextPlaceMod, ws);
                    //    Task tsk = new Task(() => { MovNextTrayXPos(ListPickMod[0], NextPlaceMod); });
                    //    tsk.Start();
                    //}
                    //else
                    //{
                    //    ChangTray = true;
                    //}
                    #endregion
                    break;
                }
                else if (res == EM_RES.PARA_OUTOFRANG || res == EM_RES.END)
                {
                    //更换料盘
                    if (traybox_fd.IsReady)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("请更换{0}====================", traybox_fd.disc) : string.Format("Please replace {0}====================               (请更换{1}====================)", traybox_fd.name, traybox_fd.disc));
                        traybox_fd.IsReady = false;
                        Task fdtsk = new Task(() => { traybox_fd.Tray_Action(Demo); });
                        fdtsk.Start();
                    }
                }
                else if (res == EM_RES.NEXT)
                {
                    // //如有物料放回原位

                    // foreach (XT xt in list_xt)
                    // {
                    //     if (!xt.cy_zk.isONByChkSen && !Demo) continue;
                    //     res = XtMovPickPos(ref bquit, xt);
                    //     if (res != EM_RES.OK) return res;
                    //     res = xt.PickOrPlace(ref bquit, xt.xt_pos_pick_mod, false, Demo);
                    //     if (res != EM_RES.OK) return res;
                    // }

                    // VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "清料中，不上料!");
                    //// res = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);
                    // if (res != EM_RES.OK) return res;
                    // else break;
                    if (VAR.ClearMt) res = EM_RES.OK;
                    return res;

                }
                else
                {
                    if (res != EM_RES.ABORT) return res;
                    Thread.Sleep(100);

                }
            }

            return res;
        }
        #endregion

        #region 上下料动作
        /// <summary>
        /// 上料动作
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public EM_RES UpDownLoadModleAct(ref bool bquit, WS ws)
        {
            bfinal = false;
            bflycam = false;
            double FeedBackPos_temp = 0;
            string msg;
            int num = 10000, num_1 = 10000, num_2 = 10000, num_3 = 10000, num_4 = 10000;
            EM_RES res = EM_RES.OK;
            bool bChkWait = false;
            int msg_outcnt = 0;
            EM_STA Proc1 = EM_STA.CHECK;
            traybox_fd.MoveToLastPos[id] = true;
            traybox_ng.MoveToLastPos[id] = true;
            traybox_ok.MoveToLastPos[id] = true;
            //取料
            double tick = Environment.TickCount;
            while (!bquit && !bfinal)
            {
                ws.breschanged = true;
                if (ws.FeedStatus != WS.EM_STA.REDAYFORUPDOWNLOAD && !Demo)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}上料时，状态异常，{1}!", ws.disc, ws.FeedStatus.ToString()) : string.Format("{0}Abnormal state when upload!         ({0}上料时，状态异常，{1}!)", ws.disc, ws.FeedStatus.ToString()), DReport.EmErrCode.StatusAbnormal, (int)DReport.EmHareware.TurnTable + ws.num + 1, ERR_ALM.EmErrItem.StautsAbnomal);
                    return EM_RES.ERR;
                }

                if (bquit)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}线程退出->系统强制退出!", disc) : string.Format("{0} thread end->system end !       ({1}线程退出->系统强制退出!}", englishdisc, disc));
                    return EM_RES.QUIT;
                }
RECHECKAGAIN:
                //检测工站目前状态(是取还是放)
                if (Proc == EM_STA.CHECK)
                {
                    if (FindMod != EM_FIN_MOD.NONE)
                    {
                        if (!PT_SET.bGrrFlow || (PT_SET.bGrrFlow && (ws.GrrUDLcnt == 0 || VAR.ClearMt)))
                        {
                            Proc = ChkNextProc(ref num, ref num_3, ws, id);
                            if (FindMod == EM_FIN_MOD.LOW && id == COM.UDLoad2.id)
                            {
                                Proc1 = ChkNextProc(ref num_1, ref num_4, ws, 0);
                                if (num_1 < 10000)
                                    num_1 = num_1 % 4;

                                bChkWait = false;
                                num_2 = 20000;
                                if (COM.UDLoad1.FeedBackWs != null && COM.UDLoad1.FeedBackWs.Count > 0 && Proc1 != EM_STA.UPDOWNLOADEND)
                                {
                                    num_2 = (COM.UDLoad1.FeedBackWs[0].Num - 1) % 8 % 4;
                                }
                                else
                                {
                                    if (num_3 < 10000)
                                    {
                                        if (Math.Abs(ws.list_md[num_3 - 1].st_pos[1].x) + Math.Abs(COM.UDLoad1.ax_x.fenc_pos) + MT.xsafedis > DisAxisX)
                                        {
                                            bChkWait = true;
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "比较值1" + Math.Abs(ws.list_md[num_3 - 1].st_pos[1].x).ToString()+ "比较值2" + Math.Abs(COM.UDLoad1.ax_x.fenc_pos));
                                        }
                                      
                                    }
                                }


                                if (Proc != EM_STA.UPDOWNLOADEND &&
                                    //((!VAR.ClearMt && num_1 == num && Proc >= COM.UDLoad1.Proc )||(VAR.ClearMt && num_1 == num) || num_1 < num || ((!VAR.ClearMt || (VAR.ClearMt && !PT_SET.issmall)) && num - num_2 > 1) || bChkWait))
                                    ((!VAR.ClearMt && num_1 == num && Proc >= COM.UDLoad1.Proc) || (VAR.ClearMt && num_1 == num) || num_1 < num || num - num_2 > 1 || bChkWait || (Proc1 == EM_STA.UPDOWNLOADEND && !COM.UDLoad1.bfinal)))
                                {
                                    msg_outcnt++;
                                    if (msg_outcnt > 30)
                                    {
                                        msg_outcnt = 0;
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("0->Proc:{0},Num_C:{1},Num_O:{2},FeedbackNum:{3},1-> Proc{4},Num_C:{5},Num_O{6},模块2等待3S", COM.UDLoad1.Proc, num_1, num_4, num_2, Proc, num, num_3) : string.Format("0->Proc:{0},Num_C:{1},Num_O:{2},FeedbackNum:{3},1-> Proc{4},Num_C:{5},Num_O{6},UpDwload2 waited 3s.               (0->Proc:{0},Num_C:{1},Num_O:{2},FeedbackNum:{3},1-> Proc{4},Num_C:{5},Num_O{6},模块2等待3S)", COM.UDLoad1.Proc, num_1, num_4, num_2, Proc, num, num_3));
                                    }

                                    Proc = EM_STA.CHECK;
                                    Thread.Sleep(100);
                                    continue;
                                }
                                msg_outcnt = 0;
                            }
                        }
                        else
                        {
                            Proc = ChkGrrNextProc(ws, id);
                        }
                    }

                    if (Proc == EM_STA.UPDOWNLOADEND || FindMod == EM_FIN_MOD.NONE)
                    {                       
                            Camcfgset();
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}检测物料完成", disc));
                        Proc = EM_STA.CHECK;
                        if (FindMod != EM_FIN_MOD.NONE)
                        {
                            ST_XYZA enpos = new ST_XYZA();
                            res = FlyCamToLastPos(ref bquit, ws, ref enpos);
                            if (res != EM_RES.OK && res != EM_RES.NEXT && res != EM_RES.RETRY)
                            {
                                return res;
                            }
                            else if(res == EM_RES.RETRY)
                            {
                                goto RECHECKAGAIN;
                            }
                            //isfirst = true;
                        }
                        //isfirst = false;
                        //归位
                        bUpdateFBPos_UT = false;
                        bUpdateFBPos_OKNG = false;
                        FeedBackPos.x = 0;
                        FeedBackPos.y = 0;
                        res = GoZero(ref bquit, false);
                        if (res != EM_RES.OK) return res;
                        bfinal = true;
                        break;
                        // return EM_RES.OK;
                    }


                    if (FindMod != EM_FIN_MOD.ALL || FindMod != EM_FIN_MOD.NONE)
                    {

                        if (FeedBackPos_temp != FeedBackPos.x)
                        {
                            FeedBackPos_temp = FeedBackPos.x;
                            TrayGoPos = FeedBackPos.x;
                        }
                        else TrayGoPos = 0;

                    }
                    else TrayGoPos = 0;
                }
                //GRR测试流程
                if (Proc == EM_STA.GRRTEST && !bquit)
                {
                    res = GRRFlow(ref bquit, ref ws, this, Demo);
                    if (res == EM_RES.OK) Proc = EM_STA.CHECK;
                    else return res;
                }
                //工站取料
                lock (pickws)
                {
                    if (Proc == EM_STA.PICKWS && !bquit)
                    {
                            Camcfgset();
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}工站取料进入!", disc));
                        res = PickWsMod(ref bquit, ref ws);
                        // if (res == EM_RES.ERR||res==EM_RES.SAFE_PROTECT)return res;
                        if (res != EM_RES.OK && res != EM_RES.END) return res;
                        else
                        {
                            if (((list_xt[0].cy_zk.isONByChkSen || list_xt[1].cy_zk.isONByChkSen) && !Demo) || Demo)
                            {
                                Proc = EM_STA.PLACETRAY;
                            }
                            else Proc = EM_STA.CHECK;
                        }
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}工站取料退出!", disc));
                    }
                }


                //料盘放料
                lock (placetray)
                {
                    if (Proc == EM_STA.PLACETRAY && !bquit)
                    {
                            Camcfgset();
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}料盘放料进入!", disc));
                        res = PlaceMdToTray(ref bquit, ws);
                        if (res != EM_RES.OK) return res;
                        else
                        {
                            if (!VAR.ClearMt)
                            {
                                Proc = EM_STA.PICKTRAY;
                                bflycam = true;
                            }
                            else
                            {
                                Proc = EM_STA.CHECK;
                                bflycam = false;

                            }

                            Task tsk_ng = new Task(() =>
                            {
                                EM_RES res_ng = MoveNextTrayX1Pos(ref UpDownLoad.bquit, traybox_ng, ws, FindMod == EM_FIN_MOD.ALL ? this : COM.List_UDLoad[(id + 1) % 2]);
                                if (res_ng != EM_RES.OK)
                                {
                                    UpDownLoad.bquit = true;
                                    if (res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
                                        VAR.gsys_set.bquit = true;
                                }
                            });
                            Task tsk_ok = new Task(() =>
                            {
                                EM_RES res_ok = MoveNextTrayX1Pos(ref UpDownLoad.bquit, traybox_ok, ws, FindMod == EM_FIN_MOD.ALL ? this : COM.List_UDLoad[(id + 1) % 2]);
                                if (res_ok != EM_RES.OK)
                                {
                                    UpDownLoad.bquit = true;
                                    if (res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
                                        VAR.gsys_set.bquit = true;
                                }
                            });
                            tsk_ng.Start();
                            tsk_ok.Start();
                        }
                        //  VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}料盘放料退出!", disc));
                    }
                }

                lock (picktray)
                {
                    //飞拍失败放回
                    if (Proc == EM_STA.FLYERRPLACE && !bquit)
                    {
                        res = DwCamErrPalceBack(ref bquit);
                        if (res == EM_RES.OK)
                        {
                            ErrTryPickCnt++;
                            if (ErrTryPickCnt > 1)
                            {
                                ErrTryPickCnt = 0;
                                msg = VAR.IsChinese ? string.Format("重取物料,{0}飞拍失败,请确认原因!", dwcam.disc) : string.Format("Retry to pick,{0} fly shot failed,please check!         (重取物料,{1}飞拍失败,请确认原因!)", dwcam.englishdisc, dwcam.disc);
                                res = EM_RES.ERR;
                            }
                            else
                            {
                                msg = VAR.IsChinese ? string.Format("{0}飞拍失败,重取物料!", dwcam.disc) : string.Format("{0} flyshot failed,retry to pick mod!          ({1}飞拍失败,重取物料!)", dwcam.englishdisc, dwcam.disc);
                                Proc = EM_STA.PICKTRAY;

                            }
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg, DReport.EmErrCode.CaptureFailed, (int)DReport.EmHareware.Cam, ErrCode: ShowErrMsg.UpDnFlyErr);
                        }

                        if (res != EM_RES.OK)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "飞拍失败!" : "Fly ERR", 20, true, ErrCode: ShowErrMsg.UpDnFlyErr);
                            return res;
                        }
                    }
                    if (Proc == EM_STA.MOTORCAMERRPLACE && !bquit)
                    {
                        res = MotorErrPalceBack(ref bquit);
                        if (res == EM_RES.OK)
                        {
                            ErrMotorTryPickCnt++;
                            if (ErrMotorTryPickCnt > 4)
                            {
                                ErrMotorTryPickCnt = 0;
                                msg = VAR.IsChinese ? string.Format("重取物料,马达扫码失败,请确认原因!") : string.Format("Retry to pick, motor cam failed,please check!         (重取物料,马达拍照失败,请确认原因!)");
                                res = EM_RES.ERR;
                            }
                            else
                            {
                                msg = VAR.IsChinese ? string.Format("马达扫码失败,重取物料!") : string.Format("motor cam failed,retry to pick mod!          (马达拍照失败,重取物料!)");
                                Proc = EM_STA.PICKTRAY;

                            }
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg, DReport.EmErrCode.CaptureFailed, (int)DReport.EmHareware.Cam);
                        }
                        else
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "马达扫码失败!" : "moto ERR", 20, true);
                            return res;
                        }
                    }

                    //料盘取料
                    if (Proc == EM_STA.PICKTRAY && !bquit)
                    {
                        uint TrayLightSet = 0;
                            Camcfgset(TrayLightSet);
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}料盘取料进入!", disc));
                        GetWsPosTeam(ref CurPlaceMod, ws, Product.EM_CM_RES.EMPTY, FindMod);
                        if (CurPlaceMod.Count != 0 && !VAR.ClearMt)
                        {
                            //bflycam = false;
                            res = PickMdToTray(ref bquit, ref bflycam, ws);
                            if (res != EM_RES.OK) return res;
                        }

                        if (((list_xt[0].cy_zk.isONByChkSen || list_xt[1].cy_zk.isONByChkSen) && !Demo) || Demo) Proc = EM_STA.PLACEWS;
                        else Proc = EM_STA.CHECK;
                        Task tsk_fd = new Task(() =>
                        {
                            EM_RES res_fd = MoveNextTrayX1Pos(ref UpDownLoad.bquit, traybox_fd, ws, FindMod == EM_FIN_MOD.ALL ? this : COM.List_UDLoad[(id + 1) % 2]);
                            if (res_fd != EM_RES.OK)
                            {
                                UpDownLoad.bquit = true;
                                if (res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
                                    VAR.gsys_set.bquit = true;
                            }

                        });
                        tsk_fd.Start();
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}料盘取料退出!", disc));
                    }
                }

                //工站放料
                lock (placews)
                {

                    if (Proc == EM_STA.PLACEWS && !bquit)
                    {
                            Camcfgset();
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}工站放料进入!", disc));
                        GetWsPosTeam(ref CurPlaceMod, ws, Product.EM_CM_RES.EMPTY, FindMod);
                        if (CurPlaceMod.Count != 0)
                        {
                            res = PlaceMdToWs(ref bquit, CurPlaceMod, ws);
                            if (res == EM_RES.OK)
                            {
                                Proc = EM_STA.CHECK;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "工站放料成功后跳转到步骤检测流程,开启bflycam");
                                bflycam = true;
                            }
                            else if (res == EM_RES.CAM_ERR_PLACE_BACK)
                            {
                                bflycam = false;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "工站放料失败(拍照)跳转到飞拍失败放回料盘流程,关闭bflycam");
                                Proc = EM_STA.FLYERRPLACE;
                            }
                            else if (res == EM_RES.RETRY)
                            {
                                Proc = EM_STA.CHECK;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "工站放料失败后（retry）跳转到步骤检测流程,bflycam=false");
                                bflycam = false;
                            }
                            else if (res == EM_RES.MOTOR_CAM_ERR_PLACE_BACK)
                            {
                                bflycam = false;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "工站放料失败（马达扫码）跳转到放料流程,bflycam=false");
                                Proc = EM_STA.MOTORCAMERRPLACE;
                            }
                            else if (res != EM_RES.OK)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "工站放料失败严重异常:"+res);
                                return res;
                            }
                        }
                        else
                        {
                            Proc = EM_STA.CHECK;
                        }

                        
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}工站放料退出!", disc));
                    }
                }

            }

            Ct_ud = (Environment.TickCount - tick) / 1000.0;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}完成退出用时{1:F3}s", disc, Ct_ud) : string.Format("{0} quit,Time {2:F3}s          ({1}完成退出用时{2:F3}s)", englishdisc, disc, Ct_ud));
            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}退出", disc));
            return EM_RES.OK;
        }
        #endregion

        #region  上下料线程
        public Task UpDownTask = null;
        public bool brun_ud = false;
        public double Ct_ud = 0;
        public void RunUdTask()
        {
            if (UpDownTask == null || UpDownTask != null && UpDownTask.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("创建{0}线程!", disc) : string.Format("Create {0} thread!         创建{1}线程!", englishdisc, disc));
                if (UpDownTask != null) UpDownTask.Dispose();
                UpDownTask = new Task(UdLoad);
                brun_ud = true;
                UpDownTask.Start();
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}线程未退出无法创建!", disc) : string.Format(" {0} Thread could not be created without exiting !        ({1}线程未退出无法创建!)", englishdisc, disc));
            }
        }

        public EM_RES WaitUdTask(ref bool bequit)
        {
            if (UpDownTask == null) return EM_RES.PARA_ERR;
            Thread.Sleep(100);
            int cnt = 0;
            while (brun_ud)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                if (UpDownTask.IsCompleted) break;
                if (bequit || VAR.gsys_set.bquit)
                {
                    if (cnt++ > 2000)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("等待{0}线程停止超时20S!", disc) : string.Format("Waiting for {0} thread to stop timeout 20S!           (等待{1}线程停止超时20S!)", englishdisc, disc));
                        status_ud = EM_STA.ERR;
                        break;
                    }
                }
                // if (!brun_ud) break;
            }

            if (status_ud == EM_STA.READY)
                return EM_RES.OK;
            else
                return EM_RES.ERR;
        }

        public void UdLoad()
        {
            EM_RES res = EM_RES.OK;
            if (status_ud == EM_STA.UNKNOW || status_ud == EM_STA.HOME)
            {
                brun_ud = false;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}没有复位,禁止上下料!", disc) : string.Format("{0} is not reset, and updwload is prohibited!          ({1}没有复位,禁止上下料!)", englishdisc, disc));
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}线程开始!", disc) : string.Format("{0} thread start!       ({1}线程开始!)", englishdisc, disc));
                WS ws = Turntable.GetWSOnFeedPos;
                res = UpDownLoadModleAct(ref bquit, ws);
                if (res != EM_RES.OK)
                {
                    if (res == EM_RES.QUIT) status_ud = EM_STA.QUIT;
                    else status_ud = EM_STA.ERR;
                    MT.BeQuitEn(true);
                    if (res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
                        MT.BeQuitEn(true, true);

                }
                else
                {
                    status_ud = EM_STA.READY;
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}线程结束!", disc) : string.Format("{0} thread end!          ({1}线程结束!)", englishdisc, disc));
                brun_ud = false;
            }

        }

        #endregion

        #region 总上下料线程

        public static Task UpDownLoadTask = null;
        public static bool brun = false;
        public static bool bquit = false;
        public static double Ct = 0;
        public static EM_STA status = EM_STA.UNKNOW;

        #region 停止
        public static void UD1Stop()
        {
            foreach (AXIS ax in MT.Axlist_UDL1_ExpLC)
            {
                ax.bhomequit = true;
                ax.Stop();
            }
        }
        public static void UD2Stop()
        {
            foreach (AXIS ax in MT.Axlist_UDL2_ExpLC)
            {
                ax.bhomequit = true;
                ax.Stop();
            }
        }

        public static void LCStop()
        {
            foreach (AXIS ax in MT.Axlist_UDL_LC)
            {
                ax.bhomequit = true;
                ax.Stop();
            }
        }
        #endregion

        public static void RunTask()
        {
            if (UpDownLoadTask == null || UpDownLoadTask != null && UpDownLoadTask.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("创建上下料线程!") : string.Format("Create updownload thread!          (创建上下料线程!)"));
                if (UpDownLoadTask != null) UpDownLoadTask.Dispose();
                UpDownLoadTask = new Task(UpDownload);
                brun = true;
                UpDownLoadTask.Start();
                status = EM_STA.BUSY;
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("上下料线程未完成无法创建!") : string.Format("Can't create new updownload thread because old thread is not finished yet.              (上下料线程未完成无法创建!)"));
            }
        }

        public static EM_RES WaitTask(ref bool bequit)
        {
            if (UpDownLoadTask == null) return EM_RES.PARA_ERR;

            while (!bequit)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                if (UpDownLoadTask.IsCompleted) break;
                if (!brun) break;
            }

            if (status == EM_STA.READY)
                return EM_RES.OK;
            else if (status == EM_STA.QUIT)
                return EM_RES.QUIT;
            else
                return EM_RES.ERR;
        }
        public static void UpDownload()
        {
            EM_RES res = EM_RES.OK;
            EM_RES res1 = EM_RES.OK;
            EM_RES res2 = EM_RES.OK;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("上下料线程开始!") : string.Format("Updownload thread start!            (上下料线程开始!)"));
            bquit = false;
            while (!VAR.gsys_set.bquit && bquit == false)
            {
                status = EM_STA.WAIT;
                brun = true;

                //准备好料盘
                //todo

                //获取当前转盘位置       
                WS ws = Turntable.GetWSOnFeedPos;
                if (ws == null)
                {
                    Thread.Sleep(50);
                    continue;
                }

                //等待上下料                
                if (ws.FeedStatus != WS.EM_STA.REDAYFORUPDOWNLOAD)
                {
                    Thread.Sleep(50);
                    continue;
                }

                //检查当前工站状态
                if (ws.TestStatus == WS.EM_TEST_STA.ERROR)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("上料线程检测到{0}状态异常!", ws.disc) : string.Format("ERROR:{0} status            (上料线程检测到{0}状态异常!)", ws.disc), DReport.EmErrCode.StatusAbnormal, (int)DReport.EmHareware.TurnTable + ws.num + 1, ERR_ALM.EmErrItem.CaptureAbnomal);
                    status = EM_STA.ERR;
                    break;
                }

                //确保当前工站准备好上料
                if (!ws.isInFeedPos)
                {
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 上下料时，不在上下料位!", ws.disc));
                    //res = EM_RES.ERR;
                    //break;
                    Thread.Sleep(50);
                    continue;
                }
                // Thread.Sleep(150);
                //上下料动作

                status = EM_STA.UPDOWNLOADEND;
                bool bSame = false;
                //同一工位同样NG是否超范围
                if (PT_SET.bSameNGTip && VAR.bSameNGTip_Temp&&!VAR.isAutoChkMode)
                {
                    try
                    {
                        List<int> NgSameResNum = new List<int>();
                        // List<int> NgSameRes= new List<int>();
                        string StrNgSameResNum = string.Empty;
                        string strNgSameRes = string.Empty;
                        foreach (WS.MdDat md in ws.list_md)
                        {
                            if (md.NgSameRes_cnt >= PT_SET.SameNGTipCnt)
                            {
                                NgSameResNum.Add(md.Num);
                                strNgSameRes += md.last_res.ToString();
                                if (VAR.Isnormal)
                                {
                                    md.benable = false;
                                    md.bardcode = "Err";
                                }
                                bSame = true;
                            }
                        }

                        if (bSame)
                        {
                            ws.SaveCfg();
                            ws.LoadCfg();
                            bSame = false;
                        }
                        if (NgSameResNum.Count > 0)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "工位连续异常!" : "Same NG", 20, true, ErrCode: ShowErrMsg.SeriesSameNGCode);
                            MT.ST_WARN st_warn = new MT.ST_WARN();
                            warning fr_warn = new warning();//增加语言
                            st_warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                            st_warn.ws = null;
                            st_warn.ws_num = NgSameResNum;
                            foreach (int num in st_warn.ws_num)
                            {
                                StrNgSameResNum += num.ToString() + ",";
                            }

                            st_warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                            st_warn.title = MultiLanguage.TxtSelct("提示:工位连续异常!", "Tip: Workstation is continuously abnormal!", "Mẹo: Máy trạm liên tục bất thường!");
                            st_warn.msg = MultiLanguage.TxtSelct(
                                $"{ws.disc}有以下工位:{StrNgSameResNum}连续出现相同NG分别对应:{strNgSameRes}!",
                                $"{ws.disc} There are the following stations: {StrNgSameResNum}, Consecutive occurrences of the same NG respectively correspond to: {strNgSameRes}",
                                $"{ws.disc} Có các trạm sau: {StrNgSameResNum}, Các lần xuất hiện liên tiếp của cùng một NG tương ứng là: {strNgSameRes}");
                            st_warn.lb_msg = MultiLanguage.TxtSelct(
                                $"提示:{st_warn.msg}请确认!\r\n  " +
                                $"1.按'继续运行'键则继续运行!\r\n " +
                                $"2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!",

                                $"Tip: {st_warn.msg} Please confirm!\r\n" +
                                $"1.Press 'Keep running' to keep running!\r\n" +
                                $"2.If you need to confirm the problem, press the 'Stop Running' button to exit the operation. " +
                                $"After the ready in the upper left corner of the interface, press the 'Run' button!",

                                $"Mẹo: {st_warn.msg} Vui lòng xác nhận!\r\n" +
                                $"1.Nhấn 'Tiếp tục chạy' để tiếp tục chạy!\r\n" +
                                $"2.Nếu bạn cần xác nhận sự cố, hãy nhấn nút 'Dừng Chạy' để thoát khỏi hoạt động. " +
                                $"Sau khi đã sẵn sàng ở góc trên bên trái của giao diện, hãy nhấn nút 'Chạy'!");
                            DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            if (DialogResult.Cancel == logres)
                            {
                                res = EM_RES.ERR;
                                break;
                            }

                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        }
                    }
                    finally
                    {
                        foreach (WS.MdDat md in ws.list_md)
                        {
                            if (md.NgSameRes_cnt >= PT_SET.SameNGTipCnt) md.NgSameRes_cnt = 0;
                        }
                    }
                }

                ////同排工站同样NG是否超范围
                //if (PT_SET.bSameRowNGTip)
                //{
                //    List<WS.MdDat> lmds = new List<WS.MdDat>();
                //    WS.MdDat[] _md = new WS.MdDat[2];
                //    int[] num = new int[2] { 0, 0 };
                //    foreach (List<WS.MdDat> list_md in ws.list_list_md)
                //    {
                //        foreach (WS.MdDat md in list_md)
                //        {
                //            lmds.Clear();
                //            if (md.benable && md.res > 1)
                //            {
                //                lmds = list_md.FindAll(delegate (WS.MdDat a) { return md.res > 1 && a.res == md.res || !a.benable; });
                //                if (lmds.Count >= PT_SET.SameRowNGTipCnt && lmds.Count>num[ws.list_list_md.IndexOf(list_md)])
                //                {
                //                    _md[ws.list_list_md.IndexOf(list_md)] = md;
                //                    num[ws.list_list_md.IndexOf(list_md)] = lmds.Count;
                //                }
                //            }
                //        }
                //    }

                //    if (num[0]>0 || num[1]>0)
                //    {
                //        string[] ngstr = new string[2];
                //        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "工站同排异常!":"Same row NG", 20, true);
                //        MT.ST_WARN st_warn = new MT.ST_WARN();
                //        warning fr_warn = new warning();
                //        st_warn.ok_txt = VAR.IsChinese?"继续运行":"Keep running";
                //        st_warn.ws = null;
                //        st_warn.cancle_txt = VAR.IsChinese ? "停止运行":"Stop running";
                //        st_warn.title = VAR.IsChinese ? "提示:工站同排异常!": "Tip: The station is abnormal in the same row!";
                //        if (num[0] > 0) ngstr[0] = VAR.IsChinese ? string.Format("第一排(1-8工位):NG数量(加上屏蔽工位):{0},NG代码:{1}!", num[0], _md[0].res) : string.Format("First row (1-8 stations): NG quantity (plus shielding stations): {0}, NG code: {1}!", num[0], _md[0].res);
                //        else ngstr[0] = String.Empty;
                //        if (num[1] > 0) ngstr[1] = VAR.IsChinese ? string.Format("第二排(9-16工位):NG数量(加上屏蔽工位):{0},NG代码:{1}!", num[1], _md[1].res): string.Format("Second row (9-16 stations): NG quantity (plus shielding station): {0}, NG code: {1}!", num[1], _md[1].res);
                //        else ngstr[1] = String.Empty;
                //        st_warn.msg = VAR.IsChinese ? string.Format("{0}同排工位连续出现相同NG达到设定数量[{1}]!{2}{3}", ws.disc,PT_SET.SameRowNGTipCnt, ngstr[0], ngstr[1]): string.Format("{0} The same NG appears continuously in the same row of stations to reach the set number [{1}]!{2}{3}", ws.disc, PT_SET.SameRowNGTipCnt, ngstr[0], ngstr[1]);
                //        st_warn.lb_msg = VAR.IsChinese ? "提示:" + st_warn.msg + "请确认!\r\n  1.按'继续运行'键则继续运行!\r\n  " +
                //                         "2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!\r\n": "Tip:" + st_warn.msg + "\r\nPlease confirm!\r\n  1.Press 'Keep running' to keep running!\r\n  " +
                //                                                                          "2.If you need to confirm the problem, press the 'Stop Running' button to exit the operation. After the ready in the upper left corner of the interface, press the 'Run' button.!\r\n";
                //        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn,ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                //        if (DialogResult.Cancel == logres)
                //        {
                //            res = EM_RES.ERR;
                //            break;
                //        }

                //        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                //    }
                //}

                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上下料准备...", ws.disc));

                //while (bquit == false && VAR.gsys_set.bquit == false)
                //{
                //    Thread.Sleep(10);
                //    if (Monitor.TryEnter(COM.UDLockObj, 1000))
                //        break;
                //}

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}开始上下料...", ws.disc) : string.Format("{0} Start to updownload...       ({0}开始上下料...)", ws.disc));
                int tick = Environment.TickCount;

                //foreach (Cylinder cy in ws.list_cld_fr)
                //{
                //    cy.SetOn();
                //}
                ws.bUpDnStart = true;
                ws.bOnUpDnPos = true;
                ws.bUpDnPosGoOnTest = false;

                ws.UdSwtime.Restart();
                ws.UdSwtime.Start();
                COM.UDLoad1.RunUdTask();
                Thread.Sleep(5);
                COM.UDLoad2.RunUdTask();
                res1 = COM.UDLoad1.WaitUdTask(ref bquit);
                res2 = COM.UDLoad2.WaitUdTask(ref bquit);
                //res = UploadModleAct(ref VAR.gsys_set.bquit, ws);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("完成{0}上下料动作...,Res1:{1},Res2:{2}", ws.disc, res1.ToString(), res2.ToString()) : string.Format("{0} UpDownLoadModleAct is finished...,Res1:{1},Res2:{2}             (完成{0}上下料动作...,Res1:{1},Res2:{2})", ws.disc, res1.ToString(), res2.ToString()));
                //Monitor.Exit(COM.UDLockObj);
                if (ws.UdSwtime.ElapsedMilliseconds != 0)
                {
                    ws.UdSwtime.Stop();
                    String str;
                    str = "工站:" +","+ ws.disc + "上下料用时:" + "," + ws.UdSwtime.ElapsedMilliseconds;
                    Utility.WriteStrToCSVPre(str);
                    PT_SET.udtime = ws.UdSwtime.ElapsedMilliseconds / 1000;
                }

                bWaitforUpDownload = true;
                SetCalResult();
                while (bWaitforUpDownload && !bquit && !VAR.gsys_set.bquit)
                {
                    Thread.Sleep(100);
                }

                if (res1 != EM_RES.OK || res2 != EM_RES.OK || bquit || VAR.gsys_set.bquit)
                {
                    break;
                }
                //gy0123夹具扫码
                if (PT_SET.bJigSan)
                {
                    if (PT_SET.UpDownRunMode == (int)PT_SET.RUN_MD.MD2_WORK)
                        res = COM.UDLoad2.JigSan(ref bquit, ref ws);
                    else res = COM.UDLoad1.JigSan(ref bquit, ref ws);
                    if (res != EM_RES.OK) break;
                }
                if (PT_SET.bboxCheck)
                {
                    var curHour = DateTime.Now;
                    if ((curHour.Hour == 8|| curHour.Hour == 20) && !ws.BoxCheckFinsh)
                    {
                        
                        res = ws.AllCyClose(ref VAR.gsys_set.bquit);
                        if (res != EM_RES.OK) break;
                        res = COM.UDLoad1.BoxCheck(ref bquit, COM.UDLoad1.id);
                        if (res != EM_RES.OK) break;
                        res = COM.UDLoad1.GoZero(ref bquit, false);
                        if (res != EM_RES.OK) break;
                        res = COM.UDLoad2.BoxCheck(ref bquit, COM.UDLoad2.id);
                        if (res != EM_RES.OK) break;
                        res = COM.UDLoad2.GoZero(ref bquit, false);
                        if (res != EM_RES.OK) break;
                        ws.BoxCheckFinsh = true;
                    }
                    if (curHour.Hour == 9 || curHour.Hour == 21)
                    {
                        if (ws.BoxCheckFinsh)
                        {
                            ws.BoxCheckFinsh = false;
                        }
                    }
                }

                if (PT_SET.HallEn)
                {
                    res = ws.AllCyClose(ref VAR.gsys_set.bquit);
                    if (res != EM_RES.OK) break;

                }
                else
                {
                    //上完闭合            
                    res = ws.SetupForTest(ref VAR.gsys_set.bquit);
                    if (res != EM_RES.OK) break;

                }

                //确认立柱位置            
                if (VAR.gsys_set.isChkMode && ws.ChkCnt == 0 && !PT_SET.bGrrFlow)
                {
                    try
                    {
                        double x = 0, y = 0, dis = 0;
                        bool overofs = false;
                    //定位
                    RECAP:
                        res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, ws.pos_CapLiZhu.x, ref COM.UDLoad1.ax_y, ws.pos_CapLiZhu.y);
                        if (res != EM_RES.OK) break;
                        Thread.Sleep(100);
                        //拍照
                        res = COM.UDLoad1.upcam.FindTaskTriAndWait(CONST.LiZhuFw);
                        if (res == EM_RES.OK)
                        {
                            x = COM.UDLoad1.upcam.curTask.ResData.PosMM.x;
                            y = COM.UDLoad1.upcam.curTask.ResData.PosMM.y;
                            dis = Math.Sqrt((x * x + y * y));
                            if (dis > ws.Cap_LiZhu_Limit) overofs = true;
                        }
                        else if (res != EM_RES.CAM_ERR)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}立柱拍照异常,请确认！", ws.disc) : string.Format("{0} Abnormal photograph of column ,please check!           ({0}立柱拍照异常,请确认！)", ws.disc), DReport.EmErrCode.CaptureFailed, (int)DReport.EmHareware.TurnTable + ws.num + 1);
                            break;
                        }
                        if (overofs || res == EM_RES.CAM_ERR)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "拍立柱异常!" : "Cam ERR", 20, true);
                            MT.ST_WARN st_warn = new MT.ST_WARN();
                            warning fr_warn = new warning();//增加语言
                            st_warn.ok_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                            st_warn.ws = null;
                            st_warn.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                            st_warn.title = MultiLanguage.TxtSelct("提示：异常!", "Tip: Abnormal!", "Mẹo: Bất thường!");
                            if (!overofs)
                                st_warn.msg = MultiLanguage.TxtSelct($"{ws.disc}立柱拍照失败", $"{ws.disc} Post photo failed", $"{ws.disc} Không thể chụp ảnh các cột trụ");
                            else
                                st_warn.msg = MultiLanguage.TxtSelct(
                                    $"{ws.disc}立柱拍照数据超规格,数据X={x.ToString("f3")},y={y.ToString("f3")},dis={dis.ToString("f3")},Lmt={ws.Cap_LiZhu_Limit}",
                                    $"{ws.disc} Column photo data exceeds specifications,data: X={x.ToString("f3")},y={y.ToString("f3")},dis={dis.ToString("f3")},Lmt={ws.Cap_LiZhu_Limit}",
                                    $"{ws.disc} Dữ liệu ảnh cột vượt quá thông số kỹ thuật, dữ liệu: X={x.ToString("f3")},y={y.ToString("f3")},dis={dis.ToString("f3")},Lmt={ws.Cap_LiZhu_Limit}");

                            st_warn.lb_msg = MultiLanguage.TxtSelct(
                                $"提示:{st_warn.msg} \r\n 请确认原因!\r\n" +
                                $"1.如需重新拍照请按'重新拍照'按键\r\n" +
                                $"2.如需确认原因请按停止运行键退出运行，待界面左上角显示就绪后进行检查!",

                                $"Tip:{st_warn.msg} \r\n Please confirm the reason!\r\n" +
                                $"1.If you need to take a photo again, press the 'Take a photo' button\r\n" +
                                $"2.If you need to confirm the reason, please press the stop running button to exit the operation, and then check after the ready display on the top left!",

                                $"Mẹo:{st_warn.msg} \r\n Vui lòng xác nhận lý do!\r\n" +
                                $"1.If you need to take a photo again, press the 'Take a photo' button\r\n " +
                                $"2.Nếu bạn cần xác nhận lý do, vui lòng nhấn nút dừng chạy để thoát khỏi hoạt động, sau đó kiểm tra sau khi màn hình sẵn sàng ở trên cùng bên trái!");
                            DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            if (logres == DialogResult.OK)
                            {
                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                goto RECAP;
                            }
                            else if (logres == DialogResult.Cancel)
                            {
                                res = EM_RES.ERR;
                                break;
                            }
                        }
                    }
                    finally
                    {
                        COM.UDLoad1.GoZero(ref bquit, false);
                    }

                }

                int cnt = 0;
                foreach (WS.MdDat md in ws.list_md)
                {
                    if (!md.benable) continue;
                    if (md.res == -1) cnt++;
                }
                if (cnt > 0)
                {
                    //复位测试结果
                    ws.ResetResultOfMd();
                    //更新物料状态
                    ws.TestStatus = WS.EM_TEST_STA.UNTEST;


                    if (PT_SET.turnon) //提前开图
                    {
                        //提前开图
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 启动测试", ws.disc) : string.Format("{0} Start test!           ({0} 启动测试)", ws.disc));
                        var mtime = NewSysInf.UserParams.BeforeOpenImageWaitTime;
                        if (mtime > 0)
                        {
                             VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "上料后延迟开图时间" + mtime);
                              Thread.Sleep(mtime);
                        }
                        res = ws.StartTestFlow(0, WS.Demo);
                        if (res != EM_RES.OK)
                        {
                            Thread.Sleep(1500);
                            res = ws.StartTestFlow();
                            if (res != EM_RES.OK)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} StartTestFlow err", ws.disc));
                                ws.Status = WS.EM_STA.LINKERR;
                                break;
                            }
                        }
                        if (PT_SET.HallEn && !WS.Demo)
                        {
                            int sta = 0;
                            while (!VAR.gsys_set.bquit && bquit == false)
                            {
                                res = ws.WaitTestResult(ref sta, PT_SET.TestTime, WS.Demo);
                                if (res == EM_RES.PARA_ERR || res == EM_RES.QUIT)
                                {
                                    //不同步等异常
                                    break;
                                }
                                else if (res != EM_RES.OK)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} WaitTestResult err", ws.disc));
                                    ws.Status = WS.EM_STA.LINKERR;
                                    break;
                                }

                                if (sta == 301)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试Hall", ws.disc) : string.Format("{0} Notice Hall.        ({0} 通知测试Hall)", ws.disc));
                                    res = ws.NextTest(sta, WS.Demo);
                                    if (res != EM_RES.OK)
                                    {
                                        res = ws.NextTest(sta, WS.Demo);
                                        if (res != EM_RES.OK)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", ws.disc) : string.Format("ERROR: notice test!          ({0} 通知测试出错!)", ws.disc), emerr: DReport.EmErrCode.TestFailed);
                                            ws.TestStatus = WS.EM_TEST_STA.ERROR;
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

                            //上完闭合            
                            res = ws.SetupForTest(ref VAR.gsys_set.bquit);
                            if (res != EM_RES.OK) break;
                        }
                    }
                }
                else
                {
                    ws.TestStatus = WS.EM_TEST_STA.EMPTY;
                }
                //计时开始
                ws.tmr = System.Environment.TickCount;

                //通知测试
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知REDAYFORTEST", ws.disc) : string.Format("{0} REDAYFORTEST", ws.disc));
                ws.FeedStatus = WS.EM_STA.REDAYFORTEST;

                Ct = (Environment.TickCount - tick) / 1000.0;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料完成用时{0:F3}s", Ct) : string.Format("Updownload is finished ,Time:{0:F3}s.          (上下料完成用时{0:F3}s)", Ct));
            }

            if (res != EM_RES.OK || res1 != EM_RES.OK || res2 != EM_RES.OK)
            {
                if ((res == EM_RES.OK || res == EM_RES.QUIT) && (res1 == EM_RES.QUIT || res1 == EM_RES.OK) && (res2 == EM_RES.OK || res2 == EM_RES.QUIT))
                    status = EM_STA.QUIT;
                else
                    status = EM_STA.ERR;
            }
            else
            {
                status = EM_STA.READY;
            }


            brun = false;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("上下料线程结束!") : "Updownload thread end!         (上下料线程结束!)");
        }
        #endregion

        #region 夹具扫码和安装防呆

        //发送二维码夹具数据，仅仅开机发送一次gy0123
        public bool bSendJig = true;
        /// <summary>
        /// 夹具扫码
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public EM_RES JigSan(ref bool bquit, ref WS ws)
        {
            if (!(PT_SET.bJigSan && ws.bjigSan))//都开启才扫码
                return EM_RES.OK;
            string[] Errmsg = new string[3];
            bool bAlmJig = false;
            foreach (var md in ws.list_md)
            {
            REJIGCAP:
                if (md.Num % 2 == 0) continue;//偶数编号不扫码，从1开始gy0123
                if (!md.benable && !ws.list_md[md.Num].benable) continue;//一个夹具上的两个模组全部屏蔽，不扫码

                var res = MT.ZupMove(ref bquit, ref ax_x, md.st_jigpos[id].x, ref ax_y, md.st_jigpos[id].y);
                //ax_y.SetToWorkSpd();
                if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具扫码:" + md.Num.ToString());
                    Camcfgset(2);
                var BarMode = PT_SET.BarcodeMode;//记录当前模式
                PT_SET.BarcodeMode = (int)PT_SET.BAR_SCAN.UP_SCAN;//临时设置上相机扫码模式，扫码完成之后设置回来
                res = upcam.FindTaskTriAndWait(CONST.JigSanFw, Demo);
                PT_SET.BarcodeMode = (int)BarMode;
                if (res != EM_RES.OK) return EM_RES.ERR;
                else if ((upcam.curTask.ResData.BarCode == null || upcam.curTask.ResData.BarCode.Length < 1) && !Demo)
                {
                    Errmsg[0] = VAR.IsChinese ? "没检到夹具二维码!" : "No Barcode";
                    Errmsg[1] = VAR.IsChinese
                        ? string.Format("{0}_工位{1}上夹具二维码无法识别.", ws.disc, md.Num)
                        : string.Format("{0}_ws{1}can't recognize barcode.", ws.disc, md.Num);
                    if ((res == EM_RES.OK && (upcam.curTask.ResData.BarCode == null || upcam.curTask.ResData.BarCode.Length < 1)) && !Demo)
                    {
                        Errmsg[0] = VAR.IsChinese ? "没检到二维码!" : "No Barcode";
                        Errmsg[1] = VAR.IsChinese
                            ? string.Format("{0}_工位{1}上夹具二维码无法识别.", ws.disc, md.Num)
                            : string.Format("{0}_ws{1}can't recognize barcode.", ws.disc, md.Num);
                        bAlmJig = true;
                    }
                }
                else
                {
                    string StrCode = upcam.curTask.ResData.BarCode;
                    string jigcode = StrCode + "_1";
                    if (md.jig_ID != jigcode)//夹具有更新，夹具生产数量清空
                    {

                        GetModJigData(ws.num, md);//发送更新之前的数据
                        GetModJigData(ws.num, ws.list_md[md.Num]);

                        md.jig_ID = jigcode;
                        ws.list_md[md.Num].jig_ID = StrCode + "_2";//同一个夹具的另外一个模组位
                        md.cnt_ng_exposure = 0;
                        md.cnt_ng_iic = 0;
                        md.cnt_ng_image = 0;
                        md.cnt_ng_miss_pix = 0;
                        md.cnt_ng_OS = 0;
                        md.cnt_ng_other = 0;
                        md.cnt_ok = 0;
                        ws.list_md[md.Num].cnt_ng_exposure = 0;
                        ws.list_md[md.Num].cnt_ng_iic = 0;
                        ws.list_md[md.Num].cnt_ng_image = 0;
                        ws.list_md[md.Num].cnt_ng_miss_pix = 0;
                        ws.list_md[md.Num].cnt_ng_OS = 0;
                        ws.list_md[md.Num].cnt_ng_other = 0;
                        ws.list_md[md.Num].cnt_ok = 0;
                        GetModJigData(ws.num, md);//发送更新之后的数据清零
                        GetModJigData(ws.num, ws.list_md[md.Num]);
                    }
                    else if (bSendJig)
                    {
                        GetModJigData(ws.num, md);//发送更新之前的数据
                        GetModJigData(ws.num, ws.list_md[md.Num]);
                    }
                }
                if (bAlmJig)
                {
                    List<int> NgSameResNum = new List<int>();
                    bAlmJig = false;
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, Errmsg[0], 20, true);
                    MT.ST_WARN st_warn1 = new MT.ST_WARN();
                    warning fr_warn1 = new warning();//增肌啊语言
                    st_warn1.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                    st_warn1.abort_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                    st_warn1.ws = null;
                    NgSameResNum.Add(md.Num);
                    st_warn1.cancle_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                    st_warn1.msg = Errmsg[1];
                    st_warn1.title = MultiLanguage.TxtSelct($"提示:{Errmsg[0]}", $"Tip: {Errmsg[0]}", $"Mẹo: {Errmsg[0]}");
                    st_warn1.lb_msg = MultiLanguage.TxtSelct(
                        $"提示: {st_warn1.msg} 请确认!\r\n" +
                        $"1.按'继续运行'键则放弃当前检查继续运行!\r\n" +
                        $"2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按运行键!\r\n" +
                        $"3.如按'重新拍照'键则重新获取当前位置图像!",

                        $"Tip: {st_warn1.msg} \r\nPlease confirm!\r\n" +
                        $"1.Press the 'Keep running' button to abandon the current check and continue running!\r\n" +
                        $"2.If you need to confirm the problem, press the 'Stop running' button to exit the operation. " +
                        $"After the ready is displayed in the upper left corner of the interface, press the Run button!\r\n " +
                        $"3.If you press the 'Take a photo' button, you will get the current position image again!",

                        $"Mẹo: {st_warn1.msg} \r\n Please confirm!\r\n" +
                        $"1.Nhấn nút 'Tiếp tục chạy' để bỏ kiểm tra hiện tại và tiếp tục chạy!\r\n" +
                        $"2.Nếu bạn cần xác nhận sự cố, hãy nhấn nút 'Dừng chạy' để thoát khỏi hoạt động. " +
                        $"Sau khi sẵn sàng hiển thị ở góc trên bên trái của giao diện, nhấn nút Chạy！\r\n" +
                        $"3.Nếu bạn nhấn nút 'Chụp ảnh', bạn sẽ nhận lại được hình ảnh vị trí hiện tại!");
                    DialogResult logres1 = MT.Display_frwarn(fr_warn1, st_warn1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                    if (DialogResult.Cancel == logres1) return EM_RES.ERR;
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    if (DialogResult.Abort == logres1)
                    {
                        goto REJIGCAP;
                    }
                }
            }
            var res1 = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);//gy0123扫码结束归位。
            //ax_y.SetToWorkSpd();
            if (res1 != EM_RES.OK) return res1;


            ws.bjigSan = false;//关闭夹具扫码
            bSendJig = false;//关闭开机上报数据
            res1 = ws.SaveCfg();
            return res1;

        }

        //夹具安装防呆检测
        public EM_RES BoxCheck(ref bool bquit, int id)
        {
            try
            {
                if (!PT_SET.bboxCheck)//开启才扫码
                {
                    return EM_RES.OK;
                }
                List<ST_XY> listpos = new List<ST_XY>();
                List<double> setpos = new List<double>();
                if (id == 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具安装防呆检测添加位置4和3"); 
                    listpos.Add(PT_SET.boxpos4);
                    listpos.Add(PT_SET.boxpos3);
                    setpos.Add(PT_SET.boxsetpos4);
                    setpos.Add(PT_SET.boxsetpos3);                  
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具安装防呆检测添加位置1和2");
                    listpos.Add(PT_SET.boxpos1);
                    listpos.Add(PT_SET.boxpos2);
                    setpos.Add(PT_SET.boxsetpos1);
                    setpos.Add(PT_SET.boxsetpos2);
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具安装防呆检测开始");
                int num = 0;
                foreach (ST_XY xy in listpos)
                {
                    if (num > 1)
                    {
                        num = 0;
                    }
                   RECAM:
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具安装防呆开始进行定位 X:" + xy.x +"Y:"+ xy.y);
                    var res = MT.ZupMove(ref bquit, ref ax_x, xy.x, ref ax_y, xy.y);
                    //if (res != EM_RES.OK) return res;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具安装防呆定位完成，开始拍照");
                    res = upcam.FindTaskTriAndWait(num==0 ? CONST.BoxCheck1: CONST.BoxCheck2, Demo);
                    if (res != EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "检测夹具安装拍照失败");

                        MT.ST_WARN st_warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        st_warn.ok_txt = VAR.IsChinese ? "重新拍照" : "Give up";
                        st_warn.abort_txt = VAR.IsChinese ? "继续运行" : "Try again";
                        st_warn.ws = null;
                        st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        st_warn.title = VAR.IsChinese ? "提示:检测夹具安装拍照失败!" : "Tip: Abnormal suction!";
                        st_warn.msg = VAR.IsChinese ? "提示:检测夹具安装拍照失败!" : "Tip: Abnormal suction!";
                        st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.点击重新拍照将进行重新拍照!\r\n  " + 
                            "2.点击继续运行则继续运行!\r\n  " +
                            "3.点击停止将停止运行!\r\n  ";

                        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        if (DialogResult.OK == logres)
                        {
                            goto RECAM;
                        }
                        else if (DialogResult.Abort == logres)
                        {
                            return EM_RES.OK;
                        }
                        else if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                    }
                    if (!upcam.curTask.ResData.bOK || Math.Abs((Math.Abs(upcam.curTask.ResData.PosMM.x)) - setpos[num]) >= PT_SET.boxsetpos)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "检测夹具安装未通过");

                        MT.ST_WARN st_warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        st_warn.ok_txt = VAR.IsChinese ? "确定" : "Give up";
                        st_warn.abort_txt = VAR.IsChinese ? "确定" : "Try again";
                        st_warn.ws = null;
                        st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        st_warn.title = VAR.IsChinese ? "提示:检测夹具安装未通过!" : "Tip: Abnormal suction!";
                        st_warn.msg = VAR.IsChinese ? "提示:检测夹具安装未通过!" : "Tip: Abnormal suction!";
                        st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.点击确定将继续运行!\r\n  " +
                            "2.点击停止将停止运行!\r\n  ";

                        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        if (DialogResult.Cancel == logres)
                        {
                            return EM_RES.ERR;
                        }
                    }
                    num++;
                }
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "夹具检测发生异常"+ ex.ToString());
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "夹具检测通过");
            return EM_RES.OK;
        }

        /// <summary>
        /// 更新所有工站数据
        /// </summary>
        /// <param name="wsID"></param>
        /// <param name="md"></param>
        public static void GetAllJigData()
        {
            string SendData = "";
            foreach (var ws in COM.list_ws)
            {
                foreach (var md in ws.list_md)
                {
                    if (SendData != "") SendData = SendData + ";";
                    SendData = SendData + string.Format(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                            ws.num, md.Num, md.jig_ID, md.cnt_ok, md.cnt_ng_image, md.cnt_ng_OS, md.
                            cnt_ng_miss_pix, md.cnt_ng_exposure, md.cnt_ng_iic, md.cnt_ng_other);
                }
            }
            Msg.secsManager.Send(new BaseInfo() { Id = 7, Value = SendData });
            //Msg.secsManager.Send(new BaseInfo() { Id = 4 }, TypeId: 2);
        }
        /// <summary>
        /// 更新单个夹具的生产数据
        /// </summary>
        /// <param name="wsID"></param>
        /// <param name="md"></param>
        public void GetModJigData(int wsID, WS.MdDat md)
        {
            string SendData = "";
            if (SendData != "") SendData = SendData + ";";
            SendData = SendData + string.Format(@"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    wsID, md.Num, md.jig_ID, md.cnt_ok, md.cnt_ng_image, md.cnt_ng_OS, md.
                    cnt_ng_miss_pix, md.cnt_ng_exposure, md.cnt_ng_iic, md.cnt_ng_other);

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"工站{wsID}夹具{md.Num}有更新");
            Msg.secsManager.Send(new BaseInfo() { Id = 8, Value = SendData });
            Msg.secsManager.Send(new BaseInfo() { Id = 4 }, TypeId: 2);
            Task.Delay(10).Wait();
        }

        #endregion
    }
}