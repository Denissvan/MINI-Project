using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public static class UploadModle
    {
        public static bool bWaitforUpload;
        public static TrayBox traybox_fd = COM.traybox_fd;
        public static AXIS ax_x = null;
        public static AXIS ax_y = null;
        public static AXIS ax_z = null;
        public static AXIS ax_u1 = null;
        public static AXIS ax_u2 = null;
        public static AXIS ax_x_tray = null;
        public static Cam CamDw =null;
        //public static Cylinder cy_uln1 = MT.CLD_UL_N1;
        //public static Cylinder cy_uln2 = MT.CLD_UL_N2;
        //public static GPIO pzk1 = MT.GPIO_OUT_UL_PZK_N1;
        //public static GPIO pzk2 = MT.GPIO_OUT_UL_PZK_N2;
        public static GPIO io_out_zrst =null;
        public static GPIO io_in_zrst = null;
        public static XT xt1 = null;
        public static XT xt2 =null;
        //当前工站拍照位置
        public static List<WS.MdDat> PlaceMod = new List<WS.MdDat>();
        //下一次工站拍照位置
        public static List<WS.MdDat> NextPlaceMod = new List<WS.MdDat>();

        public static double time_cnt = 0;
        public static double Ct = 0;
        public static EM_STA UnloadProcess = EM_STA.PICK;

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
            [Description("放料")]
            PLACE,
            [Description("取料")]
            PICK,
            [Description("仓储进出料")]
            CALLTRAY,
            [Description("错误")]
            ERR
        }
        public static EM_STA status = EM_STA.UNKNOW;

        static double x = 0, y = 0, z = 0, u1 = 0, u2 = 0;
        static bool btsk = false;
        static bool backcam = false;
        static bool final = false;

        public static ST_XYZA CurPos
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
        public static string StrOfPos
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
                return string.Format("X: {0:000.000}\nY: {1:000.000}\nZ: {2:000.000}\nU1: {3:000.000}\nU2: {4:000.000}", x, y, z, u1, u2);
            }
        }

        //取料
        public static EM_RES Pick()
        {
            //check
            //if()
            //move up
            //move to posInf
            //down
            return EM_RES.OK;
        }
        //移动到工站飞拍
        public static EM_RES FLyCapToWs(ref bool bquit, ref ST_XY pos_flystop, List<WS.MdDat> WsTriPos, bool Ofs_On = true)
        {
            EM_RES res = EM_RES.OK;
            int i;
            List<double> dwpos = new List<double>();
            List<double> dWsTriPos = new List<double>();
            List<string> DwListTaskName = new List<string>();//{ CONST.ModShpDwFw[1], CONST.ModShpDwFw[0] };
            List<string> UpListTaskName = new List<string>();
            double xt1_cap_ofs = 0, xt2_cap_ofs = 0, ws_cap_near_ofs = 0, ws_cap_far_ofs = 0;
            try
            {
                if (Ofs_On)
                {
                    xt1_cap_ofs = COM.xt1.xt_cap_ofs;
                    xt2_cap_ofs = COM.xt2.xt_cap_ofs;
                    ws_cap_near_ofs = COM.xt1.ws_cap_near_ofs;
                    ws_cap_far_ofs = COM.xt1.ws_cap_far_ofs; 
                }
                // MT.Move(ref bquit, ref ax_y, 0);
              

               // CamTaskListCfg[CONST.DwCam] = false;
                //增加吸头2飞拍参数
                dwpos.Add(xt2.st_cap_pos.y - Math.Abs(xt2_cap_ofs));
                DwListTaskName.Add(CONST.ModDwFw[1]);

                //增加吸头1飞拍参数
                if (WsTriPos.Count != 1)
                {
                    dwpos.Add(xt1.st_cap_pos.y - Math.Abs(xt1_cap_ofs));
                    DwListTaskName.Add(CONST.ModDwFw[0]);
                }
                res = CamDw.ListTaskCfg(DwListTaskName,new List<ST_XYN>());
                if (res != EM_RES.OK) return res;
                ax_y.SetHcmp(dwpos.ToArray(), CamDw.TriIO.num);

                //增加测试位触发点及停止位置
                if (WsTriPos.Count > 0)
                {
                    for (i = 0; i < WsTriPos.Count; i++)
                    {
                        UpListTaskName.Add(CONST.WsUpFw);
                        //近端飞拍补偿
                        if (WsTriPos[i].Num < 9)
                        {
                            dWsTriPos.Add(WsTriPos[i].st_pos[0].y - Math.Abs(ws_cap_near_ofs));
                        }
                        else
                        {
                            dWsTriPos.Add(WsTriPos[i].st_pos[0].y - Math.Abs(ws_cap_far_ofs));
                        }
                        pos_flystop.x = WsTriPos[i].st_pos[0].x;
                        pos_flystop.y = WsTriPos[i].st_pos[0].y + 50;
                    }

                }

                res = COM.CamUp2.ListTaskCfg(UpListTaskName,new List<ST_XYN>());
                if (res != EM_RES.OK) return res;

                ax_y.SetHcmp(dWsTriPos.ToArray(), COM.CamUp2.TriIO.num);
                //EM_RES ret = MT.Move(ref bquit, ref ax_y, endpos);
                //确保Y的飞拍位置
                //double fly_safepos = xt2.st_cap_pos.y - ax_y.spd_cur * ax_y.tacc * 1.2;
                double fly_safepos = 180;
                //if (ax_y.fenc_pos > fly_safepos)
                //{
                //    res = MT.Move(ref bquit, ref ax_y, fly_safepos);
                //    if(res!=EM_RES.OK) 
                //     return res;
                //}
                //转正U轴
             

                //X
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "上料轴飞拍前其它轴定位!");
                if (Math.Abs(pos_flystop.x - ax_x.fenc_pos) > 50 || ax_y.fenc_pos > fly_safepos )
                {
                    AXIS axisy = null;
                    if (ax_y.fenc_pos > fly_safepos) axisy = ax_y;
                    res = MT.Move(ref bquit, ref ax_x, pos_flystop.x, ref axisy, fly_safepos, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }
                if(Math.Abs( ax_u2.fenc_pos)>5||Math.Abs( ax_u1.fenc_pos)>5)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("上料轴定位到飞拍放料转正角度U1:{0} U2:{1}", ax_u1.fenc_pos, ax_u2.fenc_pos));
                    res = MT.Move(ref bquit, ref ax_u1, 0, ref ax_u2, 0);
                    if (res != EM_RES.OK) return res;
                }               
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "上料轴定位到飞拍放料起飞");
                res = MT.Move(ref bquit, ref ax_x, pos_flystop.x, ref ax_y, pos_flystop.y, ref ax_u1, 0, ref ax_u2, 0);
                if (res != EM_RES.OK) return res;
                //等待INP完成
                res = ax_x.WaitINP(ref bquit, 1000);
                if (res != EM_RES.OK) return res;
                //Thread.Sleep(300);
                //res = ax_y.WaitINP(ref bquit, 1000);
                //if (res != EM_RES.OK) return res;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("上料轴定位到飞拍放料结束位置X:{0:f3},Y:{1:f3}", pos_flystop.x, pos_flystop.y));
                ////启动Y轴
   


            }
            finally
            {
                ax_y.SetHcmp(dwpos.ToArray(), CamDw.TriIO.num, 0);
                ax_y.SetHcmp(dWsTriPos.ToArray(), COM.CamUp2.TriIO.num, 0);
            }

            return EM_RES.OK;
        }

        public static EM_RES test_FLyCapToTray(ref bool bquit)
        {
            EM_RES res;
            double pos_mov_x = 0;
            ST_XYZA endpos = new ST_XYZA();
            List<double> TrayTriPosTemp = new List<double>();
            int i;
            List<double> TrayTriPos = new List<double>();
            List<string> Up1ListTaskName = new List<string>(){ CONST.ModUpFw, CONST.ModUpFw };
           // List<string> Up2ListTaskName = new List<string>();// { CONST.ModShpUpFw, CONST.ModShpUpFw };
            //加载流程
            COM.CamUp1.mAcqFifo.Flush();
            try
            {
                // 加载工站拍照点
                TrayTriPos.Add(traybox_fd.tray_cur.list_pos[0].Pos[0].y);
                TrayTriPos.Add(traybox_fd.tray_cur.list_pos[1].Pos[0].y);
                endpos.x = traybox_fd.tray_cur.list_pos[1].Pos[0].x;
                endpos.y = traybox_fd.tray_cur.list_pos[1].Pos[0].y - 10;
                endpos.a = traybox_fd.tray_cur.list_pos[1].Pos[0].a;
                MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_y, 550, ref ax_x_tray, endpos.a);
                // 加载料盘拍照点
                if (TrayTriPos.Count > 0)
                {
                    for (i = 0; i < TrayTriPos.Count; i++)
                    {
                        //Up1ListTaskName.Add(CONST.ModShpUpFw);
                        //if (i < TrayTriPos.Count-1)
                        //{
                        TrayTriPosTemp.Add(TrayTriPos[i] + Math.Abs(COM.xt1.ws_cap_far_ofs));
                        //}                        
                        //else if (i == TrayTriPos.Count - 1)
                        //{
                        //    TrayTriPosTemp.Add(TrayTriPos[i]);
                        //}

                    }
                    res = COM.CamUp1.ListTaskCfg(Up1ListTaskName,new List<ST_XYN>());
                    if (res != EM_RES.OK) return res;
                    ax_y.SetHcmp(TrayTriPosTemp.ToArray(), COM.CamUp1.TriIO.num);
                }
                //料盘进仓储位置确认
                if (endpos.a > traybox_fd.fd_safe_x)
                {
                    res = traybox_fd.PosTrayMovIn(ref bquit);
                    if (res != EM_RES.OK) return res;
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("上料轴定位到飞拍取料结束位置X:{0:f3},Y:{1:f3},X1:{2:f3}", endpos.x, endpos.y, endpos.a));
                //定位X X1与Y轴
                if (Math.Abs(ax_x_tray.fenc_pos - endpos.a) > 50)
                {
                    res = MT.Move(ref bquit, ref ax_x_tray, endpos.a, 3000);
                    if (res != EM_RES.OK) return res;
                }

                res = ax_y.MoveTo(ref bquit, endpos.y);
                if (res != EM_RES.OK) return res;
                //int t = 0;
                //while (true)
                //{
                //    if (ax_y.fenc_pos < pos_mov_x)
                //    {
                //        res = MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_x_tray, endpos.a, ref ax_u1, 0, ref ax_u2, 0, 3000);
                //        if (res != EM_RES.OK) return res;
                //        break;
                //    }
                //    if (bquit) return EM_RES.QUIT;
                //    Thread.Sleep(5);
                //    if (t++ > 1000)
                //    {
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}飞拍定位超时!", ax_y.disc));
                //        return EM_RES.TIMEOUT;
                //    }
                //}
                res = ax_y.WaitForMoveDone(ref bquit, endpos.y, 10000);
                if (res != EM_RES.OK) return res;
                res = WaitCamRes(COM.CamUp1);
                if (res != EM_RES.OK) return res;
                if (COM.CamUp1.List_vs_task_cur[0].ResData.BarCode == COM.CamUp1.List_vs_task_cur[1].ResData.BarCode)
                    return EM_RES.ERR;
                //if ("6S1P8B600ZVT" != COM.CamUp1.List_vs_task_cur[0].ResData.BarCode)
                //    return EM_RES.ERR;
                //if ("6S1P8B500506" != COM.CamUp1.List_vs_task_cur[1].ResData.BarCode)
                //    return EM_RES.ERR;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}:{1} {2}:{3}",COM.CamUp1.List_vs_task_cur[0].TaskName,COM.CamUp1.List_vs_task_cur[0].ResData.BarCode, COM.CamUp1.List_vs_task_cur[1].TaskName,COM.CamUp1.List_vs_task_cur[1].ResData.BarCode));
                //return EM_RES.OK;
            }
            finally
            {
                ax_y.Stop();      
                if (TrayTriPos.Count > 0)
                    ax_y.SetHcmp(TrayTriPosTemp.ToArray(), COM.CamUp1.TriIO.num, 0);
            }
            return EM_RES.OK;
        }

        public static EM_RES FLyCapToTray(ref bool bquit, ST_XYZA endpos, List<double> WsTriPos, List<double> TrayTriPos)
        {
            EM_RES res;
            double pos_mov_x = 0;
            List<double> TrayTriPosTemp = new List<double>();
            int i;
            List<string> Up1ListTaskName = new List<string>();//{ CONST.ModShpUpFw, CONST.ModShpUpFw };
            List<string> Up2ListTaskName = new List<string>();// { CONST.ModShpUpFw, CONST.ModShpUpFw };
            //加载流程

            try
            {
                // 加载工站拍照点
                if (WsTriPos.Count > 0)
                {
                    for (i = 0; i < WsTriPos.Count; i++)
                    {
                        Up2ListTaskName.Add(CONST.ModUpFw);
                        pos_mov_x = WsTriPos[i];
                    }
                    res = COM.CamUp2.ListTaskCfg(Up2ListTaskName,new List<ST_XYN>());
                    if (res != EM_RES.OK) return res;
                    ax_y.SetHcmp(WsTriPos.ToArray(), COM.CamUp2.TriIO.num);
                }

                // 加载料盘拍照点
                if (TrayTriPos.Count > 0)
                {
                    for (i = 0; i < TrayTriPos.Count; i++)
                    {
                        Up1ListTaskName.Add(CONST.ModUpFw);
                        //if (i < TrayTriPos.Count-1)
                        //{
                        TrayTriPosTemp.Add(TrayTriPos[i] + Math.Abs(COM.xt1.ws_cap_far_ofs));
                        //}
                        //else if (i == TrayTriPos.Count - 1)
                        //{
                        //    TrayTriPosTemp.Add(TrayTriPos[i]);
                        //}

                    }
                    res = COM.CamUp1.ListTaskCfg(Up1ListTaskName, new List<ST_XYN>());
                    if (res != EM_RES.OK) return res;
                    ax_y.SetHcmp(TrayTriPosTemp.ToArray(), COM.CamUp1.TriIO.num);
                }
                //料盘进仓储位置确认
                if (endpos.a > traybox_fd.fd_safe_x)
                {
                    res = traybox_fd.PosTrayMovIn(ref bquit);
                    if (res != EM_RES.OK) return res;
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("上料轴定位到飞拍取料结束位置X:{0:f3},Y:{1:f3},X1:{2:f3}", endpos.x, endpos.y, endpos.a));
                //定位X X1与Y轴
                if (Math.Abs(ax_x_tray.fenc_pos - endpos.a) > 50)
                {
                    res = MT.Move(ref bquit, ref ax_x_tray, endpos.a, 3000);
                    if (res != EM_RES.OK) return res;
                }

                res = ax_y.MoveTo(ref bquit, endpos.y);
                if (res != EM_RES.OK) return res;
                int t = 0;
                while (true)
                {
                    if (ax_y.fenc_pos < pos_mov_x)
                    {
                        res = MT.Move(ref bquit, ref ax_x, endpos.x, ref ax_x_tray, endpos.a,ref ax_u1,0,ref ax_u2,0, 3000);
                        if (res != EM_RES.OK) return res;
                        break;
                    }
                    if (bquit) return EM_RES.QUIT;
                    Thread.Sleep(5);
                    if (t++ > 1000)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}飞拍定位超时!", ax_y.disc));
                        return EM_RES.TIMEOUT;
                    }

                }
                res = ax_y.WaitForMoveDone(ref bquit, endpos.y, 10000);
                if (res != EM_RES.OK) return res;
            }
            finally
            {
                ax_y.Stop();
                if (WsTriPos.Count > 0)
                    ax_y.SetHcmp(WsTriPos.ToArray(), COM.CamUp2.TriIO.num, 0);
                if (TrayTriPos.Count > 0)
                    ax_y.SetHcmp(TrayTriPosTemp.ToArray(), COM.CamUp1.TriIO.num, 0);
            }
            return EM_RES.OK;
        }
        //判断视觉数据更新
        public static EM_RES WaitCamRes(Cam cam)
        {
            int n;
            int count;

            for (n = 0; n < 200; n++)
            {
                count = 0;
                foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                {
                    if (task.ResData.bUpdate) count++;
                }

                if (count == cam.List_vs_task_cur.Count)
                {
                    if (VAR.gsys_set.isDemoMode)
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
                Thread.Sleep(10);
                if (n >= 200)
                {
                    foreach (Cam.VisionTask task in cam.List_vs_task_cur)
                    {
                        if (!task.ResData.bUpdate)
                        {
                            task.ResData.bOK = false;
                            
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}{1}数据更新超时!", cam.mName, task.TaskName));
                        }
                    }
                    return EM_RES.TIMEOUT;
                }
            }
            return EM_RES.OK;
        }


        #region 飞拍放料(old)
        //public static EM_RES FlyToWsPlaceMod(ref bool bquit,ref ST_XYZA[] place, ST_XY st_place, List<double> WsTriPos)
        //{

        //    EM_RES ret;
        //    int t = 0;
        //    ST_XY Pos_Upcam;
        //    int dwidx=0,upidx=0;
        //    //升Z轴U1与U2转为0度
        //    ret = MT.ZupMove(ref bquit, ref ax_x, st_place.x, ref ax_u1, 0, ref ax_u2, 0);
        //    if (ret != EM_RES.OK) return ret;
        //    //定位到放料位
        //    ret = FLyCapToWs(ref bquit, st_place.y, WsTriPos);
        //    if (ret != EM_RES.OK) return ret;
        //    //确认视觉回传结果 
        //    t = Environment.TickCount;
        //    WaitCamRes(COM.CamDown);
        //    WaitCamRes(COM.CamUp2);
        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("拍照结果等待时间{0}", Environment.TickCount - t));
        //    //对调下相机结果
        //    if(COM.ListXT.Count==COM.CamDown.List_vs_task_cur.Count)
        //    {
        //        COM.CamDown.List_vs_task_cur.Add(COM.CamDown.List_vs_task_cur[0]);
        //        COM.CamDown.List_vs_task_cur.Remove(COM.CamDown.List_vs_task_cur[0]);
        //    }

        //    //放料          
        //    foreach (Cam.VisionTask dwtask in COM.CamDown.List_vs_task_cur)
        //    {
        //        if (dwtask.ResData.bOK && dwtask.ResData.bUpdate)
        //        {
        //            foreach (Cam.VisionTask uptask in COM.CamUp2.List_vs_task_cur)
        //            {
        //                if (uptask.ResData.bOK && uptask.ResData.bUpdate)
        //                {
        //                    dwidx = COM.CamDown.List_vs_task_cur.IndexOf(dwtask);
        //                    upidx = COM.CamUp2.List_vs_task_cur.IndexOf(uptask);
        //                    Pos_Upcam.x = st_place.x;
        //                    Pos_Upcam.y = WsTriPos[upidx];
        //                    ret = COM.ListXT[dwidx].XtPickOrPlaceMod(ref bquit, ref place[dwidx], COM.CamUp2, Pos_Upcam, uptask.ResData.PosMM, dwtask.ResData.PosMM, false);
        //                    if (ret != EM_RES.OK) return ret;
        //                    uptask.ResData.bUpdate = false;
        //                    dwtask.ResData.bUpdate = false;
        //                    break;
        //                }
        //            }
        //        }

        //    }

        //    //检测到吸头还没有放完物料
        //    foreach (Cam.VisionTask dwtask in COM.CamDown.List_vs_task_cur)
        //    {
        //        if (dwtask.ResData.bOK && dwtask.ResData.bUpdate)
        //        {

        //        }
        //    }
        //        return EM_RES.OK;
        //}
        #endregion

        public static EM_RES FlyToWsPlaceMod(ref bool bquit, List<WS.MdDat> WsTriPos)
        {

            EM_RES res = EM_RES.OK;
            XT xt = new XT();
            ST_XY pos_flystop = new ST_XY();
            ST_XY Pos_Upcam;
            bool dwcam_recap_err = false;
            //int dwidx = 0, upidx = 0;
            int n = 0;
            //升Z轴U1与U2转为0度
            for (n = 0; n < 2; n++)
            {
                if (ax_z.fenc_pos > 0.1)
                {
                    res = MT.ZupMove(ref bquit, ref ax_z, 0);
                    if (res != EM_RES.OK) return res;
                }
                //定位到放料位
                res = FLyCapToWs(ref bquit, ref pos_flystop, WsTriPos);
                if (res != EM_RES.OK) return res;
                //确认视觉回传结果 
                WaitCamRes(CamDw);
                WaitCamRes(COM.CamUp2);
                //对调下相机结果
                if (COM.ListXT.Count == CamDw.List_vs_task_cur.Count)
                {
                    CamDw.List_vs_task_cur.Add(CamDw.List_vs_task_cur[0]);
                    CamDw.List_vs_task_cur.Remove(CamDw.List_vs_task_cur[0]);
                    if ((CamDw.List_vs_task_cur[0].ResData.PosMM.x == CamDw.List_vs_task_cur[1].ResData.PosMM.x) && (CamDw.List_vs_task_cur[0].ResData.PosMM.y == CamDw.List_vs_task_cur[1].ResData.PosMM.y)
                        && CamDw.List_vs_task_cur[0].ResData.bOK && CamDw.List_vs_task_cur[1].ResData.bOK)
                       {
                        if (n == 0)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("飞拍{0}两个视觉数据一样,重拍！", CamDw.disc));
                            continue;
                        }
                        else
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("飞拍{0}两个视觉数据一样,重拍NG！", CamDw.disc));
                            return EM_RES.ERR;
                        }
                    }
                }
                else
                {
                    if (CamDw.List_vs_task_cur.Count == WsTriPos.Count)
                     break;
                    if (n == 0)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}拍照缺少拍照结果,重拍!", CamDw.disc));
                        continue;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}拍照缺少拍照结果,重拍NG!", CamDw.disc));
                        return EM_RES.CAM_ERR;
                    }

                }
                if (COM.CamUp2.List_vs_task_cur.Count == WsTriPos.Count)
                {
                    //检查模组结果是否一样
                    if (COM.CamUp2.List_vs_task_cur.Count == 2 && COM.CamUp2.List_vs_task_cur[0].ResData.bOK && COM.CamUp2.List_vs_task_cur[1].ResData.bOK)
                    {
                        if ((COM.CamUp2.List_vs_task_cur[0].ResData.PosMM.x == COM.CamUp2.List_vs_task_cur[1].ResData.PosMM.x) && (COM.CamUp2.List_vs_task_cur[0].ResData.PosMM.y == COM.CamUp2.List_vs_task_cur[1].ResData.PosMM.y))
                        {
                            if (n == 0)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("飞拍{0}两个视觉数据一样,重拍！", COM.CamUp2.disc));
                                continue;
                            }
                            else
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("飞拍{0}两个视觉数据一样,重拍NG！", COM.CamUp2.disc));
                                return EM_RES.ERR;
                            }
                        }
                    }
                }
                else if (COM.CamUp2.List_vs_task_cur.Count != WsTriPos.Count)
                {
                    if (n == 0)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}拍照缺少拍照结果,重拍!", COM.CamUp2.disc));
                        continue;
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}拍照缺少拍照结果,重拍NG!", COM.CamUp2.disc));
                        return EM_RES.CAM_ERR;
                    }
                }
                break;
            }
            //if (COM.CamDown.List_vs_task_cur[0].ResData.bOK == false && COM.CamDown.List_vs_task_cur[1].ResData.bOK == false)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "两模组拍照失败!");
            //    return EM_RES.CAM_ERR;
            //}
            //if (COM.CamUp2.List_vs_task_cur[0].ResData.bOK == false && COM.CamUp2.List_vs_task_cur[1].ResData.bOK == false)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "两工站位拍照失败!");
            //    return EM_RES.CAM_ERR;
            //}

            //确认有没有模组拍照料失败
            foreach (Cam.VisionTask task in CamDw.List_vs_task_cur)
            {
                if (!task.ResData.bOK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}拍照失败，定位到拍照位置重拍", COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].disc));
                    res = MT.ZupMove(ref bquit, ref ax_y, COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].st_cap_pos.y, ref COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].ax_u, 0);
                    if (res != EM_RES.OK) return res;
                    res = task.TriAndWaitResult(ref bquit);
                    if (res != EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}重拍失败！", COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].disc));
                        //return EM_RES.CAM_ERR;
                        dwcam_recap_err = true;
                    }
                }
            }

            //再次确认下相机结果NG放回料盒
            if (dwcam_recap_err)
            {
               
                double pos_tray_x = 0;
                foreach (Cam.VisionTask task in CamDw.List_vs_task_cur)
                {
                    if (!task.ResData.bOK)
                    {

                        pos_tray_x = -COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].xt_pos_pick_mod.x + ax_x.fenc_pos + COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].xt_pos_pick_tray_x;
                        if (pos_tray_x < 0)
                        {
                            COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].xt_pos_pick_mod.x = ax_x.fenc_pos - pos_tray_x;
                            pos_tray_x = 0;

                        }
                        else
                        {
                            COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].xt_pos_pick_mod.x = ax_x.fenc_pos;
                        }
                        if (pos_tray_x > traybox_fd.fd_safe_x)
                        {
                            res = traybox_fd.PosTrayMovIn(ref bquit);
                            if (res != EM_RES.OK) return res;
                        }
                        //move tray_x
                        res = MT.Move(ref bquit, ref traybox_fd.ax_x, pos_tray_x);
                        if (res != EM_RES.OK) return res;
                        res = COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].PickOrPlace(ref bquit, COM.ListXT[CamDw.List_vs_task_cur.IndexOf(task)].xt_pos_pick_mod, false,VAR.gsys_set.isDemoMode);
                        if (res != EM_RES.OK) break;
                    }
                }
                return EM_RES.PLACE_ERR;
            }


            //确认上相机工站数据
            foreach (Cam.VisionTask task in COM.CamUp2.List_vs_task_cur)
            {
                if (!task.ResData.bOK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("工站位置{0}拍照失败，定位到拍照位置重拍!", WsTriPos[COM.CamUp2.List_vs_task_cur.IndexOf(task)].Num));
                    res = MT.ZupMove(ref bquit, ref ax_x, pos_flystop.x, ref ax_y, WsTriPos[COM.CamUp2.List_vs_task_cur.IndexOf(task)].st_pos[0].y);
                    if (res != EM_RES.OK) return res;
 
                    res = task.TriAndWaitResult(ref bquit);
                    if (res != EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("工站位置{0}重拍失败,请检查对应的工站位置是否有异物!", WsTriPos[COM.CamUp2.List_vs_task_cur.IndexOf(task)].Num));
                        return EM_RES.CAM_ERR;
                    }
                }

            }
            //清空相机数据
            if (!CamDw.FlushOk || !COM.CamUp2.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!CamDw.FlushOk)
                    {
                        CamDw.mAcqFifo.Flush();
                        CamDw.FlushOk = true;
                    }
                    if (!COM.CamUp2.FlushOk)
                    {
                        COM.CamUp2.mAcqFifo.Flush();
                        COM.CamUp2.FlushOk = true;
                    }
                    
                });
                tak.Start();
            }

          #region 先放吸头2再放吸头1
            for (int i = CamDw.List_vs_task_cur.Count - 1; i >= 0; i--)
            {
                if (CamDw.List_vs_task_cur[i].ResData.bOK && CamDw.List_vs_task_cur[i].ResData.bUpdate)
                {
                    for (int j = COM.CamUp2.List_vs_task_cur.Count - 1; j >= 0; j--)
                    {
                        if (COM.CamUp2.List_vs_task_cur[j].ResData.bOK && COM.CamUp2.List_vs_task_cur[j].ResData.bUpdate)
                        {
                            Pos_Upcam.x = pos_flystop.x;
                            Pos_Upcam.y = WsTriPos[j].st_pos[0].y;
                            if (WsTriPos.Count == 1)
                            {
                                res = COM.xt2.XtPickOrPlaceMod(ref bquit, Pos_Upcam, COM.CamUp2.List_vs_task_cur[j].ResData.PosMM, CamDw.List_vs_task_cur[i].ResData.PosMM, WsTriPos[j].st_pos[0].z,VAR.gsys_set.isDemoMode,XT.EM_XTFLOW.PLACEMOD, false);
                            }
                            else
                            {
                                res = COM.ListXT[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, COM.CamUp2.List_vs_task_cur[j].ResData.PosMM, CamDw.List_vs_task_cur[i].ResData.PosMM, WsTriPos[j].st_pos[0].z,VAR.gsys_set.isDemoMode, XT.EM_XTFLOW.PLACEMOD, false);
                            }                          
                            if (res == EM_RES.OK)
                            {

                                if (WsTriPos.Count == 1)
                                {
                                    WsTriPos[j].bardcode = COM.xt2.XtMd.bardcode;
                                    COM.xt2.XtMd = null;
                                }
                                else
                                {
                                    WsTriPos[j].bardcode = COM.ListXT[i].XtMd.bardcode;
                                    COM.ListXT[i].XtMd = null;
                                }                                   
                                WsTriPos[j].res = -1;
                               
                            }
                            else return res;
                            COM.CamUp2.List_vs_task_cur[j].ResData.bUpdate = false;
                            CamDw.List_vs_task_cur[i].ResData.bUpdate = false;
                            break;
                        }
                    }
                }
            }
            #endregion
            //放料          
            //foreach (Cam.VisionTask dwtask in COM.CamDown.List_vs_task_cur)
            //{
            //    if (dwtask.ResData.bOK && dwtask.ResData.bUpdate)
            //    {
            //        foreach (Cam.VisionTask uptask in COM.CamUp2.List_vs_task_cur)
            //        {
            //            if (uptask.ResData.bOK && uptask.ResData.bUpdate)
            //            {
            //                dwidx = COM.CamDown.List_vs_task_cur.IndexOf(dwtask);
            //                upidx = COM.CamUp2.List_vs_task_cur.IndexOf(uptask);
            //                Pos_Upcam.x = pos_flystop.x;
            //                Pos_Upcam.y = WsTriPos[upidx].st_pos.y;
            //                res = COM.ListXT[dwidx].XtPickOrPlaceMod(ref bquit, COM.CamUp2, Pos_Upcam, uptask.ResData.PosMM, dwtask.ResData.PosMM, WsTriPos[upidx].st_pos.z, false);

            //                if (res == EM_RES.OK)
            //                {
            //                    if (!VAR.gsys_set.isDemoMode)
            //                        WsTriPos[upidx].bardcode = COM.ListXT[dwidx].XtMd.bardcode;
            //                    WsTriPos[upidx].res = -1;
            //                    COM.ListXT[dwidx].XtMd = null;

            //                }
            //                else return res;
            //                uptask.ResData.bUpdate = false;
            //                dwtask.ResData.bUpdate = false;
            //                break;
            //            }
            //        }
            //    }

            //}
            time_cnt = Environment.TickCount;
            return EM_RES.OK;
        }
        //飞拍取料
        public static EM_RES FlyToTrayPickMod(ref bool bquit, List<WS.MdDat> WsTriPos, List<Product.Tray.PosInf> TrayTriPos,bool pickone=false)
        {

            EM_RES res;
            List<double> dTrayTriPos = new List<double>();
            List<double> dWsTriPos = new List<double>();
            ST_XYZA pos_pick = new ST_XYZA();
            ST_XY Pos_Upcam;
            int upidx;
            ST_XYA xt_vs_dat = new ST_XYA(-10000, -10000, 0);
            //料盘轴移动放料位
            if (TrayTriPos.Count > 1)
            {
                if (TrayTriPos[0].Row == TrayTriPos[1].Row && !pickone)
                {
                    dTrayTriPos.Add(TrayTriPos[0].Pos[0].y);
                    dTrayTriPos.Add(TrayTriPos[1].Pos[0].y);
                    pos_pick.y = TrayTriPos[1].Pos[0].y - 10;
                }
                //else if (TrayTriPos[0].Row != TrayTriPos[1].Row && TrayTriPos.Count>=3 && !pickone)
                //{
                //    TrayTriPos[0].md = null;
                //    dTrayTriPos.Add(TrayTriPos[1].Pos.y);
                //    dTrayTriPos.Add(TrayTriPos[2].Pos.y);
                //    pos_pick.y = TrayTriPos[2].Pos.y - 10;
                //}
                else
                {

                    dTrayTriPos.Add(TrayTriPos[0].Pos[0].y);
                    pos_pick.y = TrayTriPos[0].Pos[0].y - 10;
                }
            }
            else
            {
                dTrayTriPos.Add(TrayTriPos[0].Pos[0].y);
                pos_pick.y = TrayTriPos[0].Pos[0].y - 10;
            }

            pos_pick.a = -TrayTriPos[0].Pos[0].x + ax_x.fenc_pos + TrayTriPos[0].Pos[0].a;
            if (pos_pick.a < 0)
            {
                pos_pick.x = ax_x.fenc_pos - pos_pick.a;
                pos_pick.a = 0;
            }
            else
            {
                pos_pick.x = ax_x.fenc_pos;
            }
            //加工站触发位
            if (WsTriPos.Count > 0)
            {
                for (int i = 0; i < WsTriPos.Count; i++)
                {
                    dWsTriPos.Add(WsTriPos[i].st_pos[0].y);
                }
                if (WsTriPos.Count == 2)
                {
                    dWsTriPos.Add(WsTriPos[0].st_pos[0].y);
                    dWsTriPos.Remove(WsTriPos[0].st_pos[0].y);
                }
            }
            //定位到取料位
            res = FLyCapToTray(ref bquit, pos_pick, dWsTriPos, dTrayTriPos);
            if (res != EM_RES.OK) return res;
            //等结果，然后清空相机，再判断等待结果
            //等待视觉回传结果 
            res = WaitCamRes(COM.CamUp1);
            EM_RES res2 = WaitCamRes(COM.CamUp2);

            //检查视觉结果
            if (res != EM_RES.OK) return res;
            if (res2 != EM_RES.OK) return res2;

            bool mtofs = false;
            foreach (Cam.VisionTask task in COM.CamUp2.List_vs_task_cur)
            {
                if (!task.ResData.bOK || Math.Abs(task.ResData.PosMM.x) > 0.3 || Math.Abs(task.ResData.PosMM.y) > 0.3 || Math.Abs(task.ResData.PosMM.a) > 2.6)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("物料放偏坐标{0}", task.ResData.PosMM.ToString()));
                    mtofs = true;
                }
            }

            if (mtofs)
            {
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "物料放偏!": "Place deflected", 20, true);
                warning fr_Uppwarn = new warning();
                fr_Uppwarn.TopMost = true;
                fr_Uppwarn.btn_ok.Text = VAR.IsChinese?"继续运行":"Keep running";
                fr_Uppwarn.btn_cancle.Text = VAR.IsChinese ? "停止运行":"Stop running";
                fr_Uppwarn.btn_ok.Visible = true;
                fr_Uppwarn.btn_cancle.Visible = true;
                fr_Uppwarn.BackColor = Color.Yellow;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?"当前工站有物料放偏!": "There is material deviation in the current station!    (当前工站有物料放偏!)");
                fr_Uppwarn.lb_msg.Text = VAR.IsChinese?"提示:当前工站有物料放偏，请确认!\r\n  1.如果没有物料放偏,请按继续运行键!\r\n  " +
                 "2.如有物料放偏请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!": "Tip:There is material deviation in the current station, please confirm!\r\n  1.If there is no material deviation, please press the keep running button!\r\n  " +
                                                          "2.If there is material deviation, please press the stop running button to exit the operation, and then press the run button after the ready is displayed in the upper left corner of the interface!"+ "提示:当前工站有物料放偏，请确认!\r\n  1.如果没有物料放偏,请按继续运行键!\r\n  " +
                                                          "2.如有物料放偏请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!";

                
                DialogResult logres = fr_Uppwarn.ShowDialog();
                if (DialogResult.Cancel == logres)
                {
                    return EM_RES.ERR;
                }
                else
                {
                    MT.OnlyOnelightON(0);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                }
            }
            //清空相机数据
            if (!COM.CamUp1.FlushOk || !COM.CamUp2.FlushOk)
            {
                Task tak = new Task(() =>
                {
                    if (!COM.CamUp1.FlushOk)
                    {
                        COM.CamUp1.mAcqFifo.Flush();
                        COM.CamUp1.FlushOk = true;
                    }
                    if (!COM.CamUp2.FlushOk)
                    {
                        COM.CamUp2.mAcqFifo.Flush();
                        COM.CamUp2.FlushOk = true;
                    }
                });
                tak.Start();
            }         

            //吸料
            //foreach (XT xt in COM.ListXT)
            for (int i = COM.ListXT.Count - 1; i >= 0; i--)
            {
                //如果只取一个，XT2取料
                if (pickone && (COM.ListXT[i].id == COM.xt1.id)) continue;

                if (COM.ListXT[i].cy_zk.isOFFByChkSen)
                {
                    foreach (Cam.VisionTask uptask in COM.CamUp1.List_vs_task_cur)
                    {
                        if (uptask.ResData.bOK && uptask.ResData.bUpdate)
                        {
                            upidx = COM.CamUp1.List_vs_task_cur.IndexOf(uptask);
                            Pos_Upcam.x = pos_pick.x;
                            Pos_Upcam.y = dTrayTriPos[upidx];
                            res = COM.ListXT[i].XtPickOrPlaceMod(ref bquit, Pos_Upcam, uptask.ResData.PosMM, xt_vs_dat, TrayTriPos[upidx].Pos[0].z,VAR.gsys_set.isDemoMode, XT.EM_XTFLOW.PICKMOD, true);
                            if (res == EM_RES.OK)
                            {
                                COM.ListXT[i].XtMd = TrayTriPos[upidx].md;
                                COM.ListXT[i].XtMd.bardcode = uptask.ResData.BarCode;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}------barcode---{1}!", COM.ListXT[i].disc, COM.ListXT[i].XtMd.bardcode));
                                TrayTriPos[upidx].md = null;

                            }
                            else return res;
                            uptask.ResData.bUpdate = false;
                            break;
                        }
                    }
                }

            }

            return EM_RES.OK;
        }
        /// <summary>
        /// 找工站上空的工位
        /// </summary>
        /// <param name="PlaceMod"></param>
        public static void GetWsListPos(ref  List<WS.MdDat> PlaceMod, WS ws)
        {
            PlaceMod.Clear();
            foreach (WS.MdDat WsMod in ws.list_md)
            {
                if (WsMod.res == -2 && WsMod.benable == true)//结果-2为空位
                {
                    if(VAR.gsys_set.isChkMode && VAR.ChkPC != WsMod.PC_ID) continue;
                    PlaceMod.Add(WsMod);
                    if (WsMod.Num < (ws.list_md.Count / 2 + 1) && ws.list_md[WsMod.Num + ws.list_md.Count / 2 - 1].res == -2 && ws.list_md[WsMod.Num + ws.list_md.Count / 2 - 1].benable == true)
                    {
                        if (VAR.gsys_set.isChkMode && VAR.ChkPC != ws.list_md[WsMod.Num + ws.list_md.Count / 2 - 1].PC_ID) break; 
                        PlaceMod.Add(ws.list_md[WsMod.Num + ws.list_md.Count / 2 - 1]);
                    }
                    break;
                }
            }
        }

        public static EM_RES MovNextTrayXPos(Product.Tray.PosInf pospick, List<WS.MdDat> posplace)
        {
            EM_RES res;
            double x = 0;
            if (posplace.Count > 0)
            {
                for (int i = 0; i < posplace.Count; i++)
                {
                    x = -pospick.Pos[0].x + pospick.Pos[0].a + posplace[i].st_pos[0].x;
                }
                res = MT.Move(ref VAR.gsys_set.bquit, ref traybox_fd.ax_x, x);
                if (res != EM_RES.OK) return res;

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
        public static EM_RES ToTrayPickMod(ref bool bquit, ref bool IsFly,WS ws)
        {
            EM_RES res = EM_RES.OK;
            int TryPickCnt = 0, TryCnt = 0;
            bool pickone=false;
            List<Product.Tray.PosInf> ListPickMod = new List<Product.Tray.PosInf>();
            if (!traybox_fd.IsReady) return EM_RES.ABORT;
            if (!VAR.gsys_set.isDemoMode && xt1.cy_zk.isONByChkSen && xt2.cy_zk.isONByChkSen) return EM_RES.OK;
            //判断料盘状态
            if (traybox_fd.tray_cur == null) return EM_RES.PARA_OUTOFRANG;
            ListPickMod = traybox_fd.tray_cur.GetPosList();
            if (ListPickMod != null && ListPickMod.Count == 0) return EM_RES.END;
            GetWsListPos(ref NextPlaceMod, ws);
            if (NextPlaceMod.Count == 1)
                pickone = true;
            else
                pickone = false;
            //取料
            while (TryCnt++ < 5)
            {
                if (!IsFly || ax_y.fenc_pos < xt1.st_rol_cap.y)
                {
                    foreach (XT xt in COM.ListXT)
                    {
                        //如果只有一个目标，则吸头2吸料
                        if (pickone && xt.id == COM.xt1.id)
                            continue;

                        if (VAR.gsys_set.isDemoMode || ((!xt.cy_zk.isONByChkSen) && !VAR.gsys_set.isDemoMode))
                        {
                            ListPickMod = traybox_fd.tray_cur.GetPosList();
                            if (ListPickMod != null && ListPickMod.Count == 0)
                            {
                                if (VAR.ClearMt)
                                {
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "清料提示!":"Clear warning", 20, true);
                                    warning fr_ULwarn1 = new warning();
                                    fr_ULwarn1.btn_ok.Text = VAR.IsChinese?"清料":"Clear";
                                    fr_ULwarn1.btn_cancle.Text = VAR.IsChinese ? "取消":"Cancel";
                                    fr_ULwarn1.btn_ok.Visible = true;
                                    fr_ULwarn1.btn_cancle.Visible = true;
                                    fr_ULwarn1.TopMost = true;
                                    fr_ULwarn1.BackColor = Color.Yellow;
                                    fr_ULwarn1.lb_msg.Text = VAR.IsChinese?"提示:是否要清料?\r\n  1.按清料键进行清料流程": "Tip: Do you want to clear the material ? \r\n1.Press the Clear button to clear the material.\r\n提示:是否要清料?\r\n  1.按清料键进行清料流程";
                                    DialogResult logres1 = fr_ULwarn1.ShowDialog();
                                    if (DialogResult.OK == logres1)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                                        return EM_RES.NEXT;


                                    }
                                    else
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
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
                            res = xt.XtCapMovMod(ref bquit, out  barcode, ListPickMod[0].Pos[0], CONST.ModUpFw,10,XT.EM_XTFLOW.PICKMOD, true,VAR.gsys_set.isDemoMode);
                            if (res == EM_RES.OK)
                            {
                                //更新料盘
                                TryPickCnt = 0;
                                xt.XtMd = ListPickMod[0].md;
                                xt.XtMd.bardcode = COM.CamUp1.curTask.ResData.BarCode;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}------barcode---{1}!", xt.disc, xt.XtMd.bardcode));
                                ListPickMod[0].md = null;

                            }
                            else
                            {
                                if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR)
                                {
                                    ListPickMod[0].md.res = (int)Product.EM_CM_RES.CAMERR;
                                    TryPickCnt++;
                                    if (TryPickCnt > 2)
                                    {
                                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "吸料异常!":"Draw ERR", 20, true);
                                        warning fr_ULwarn = new warning();
                                        fr_ULwarn.btn_ok.Text = VAR.IsChinese?"确认":"OK";
                                        fr_ULwarn.btn_cancle.Text = VAR.IsChinese ? "更换料盘":"Replace tray";
                                        fr_ULwarn.btn_ok.Visible = true;
                                        fr_ULwarn.btn_cancle.Visible = true;
                                        if (VAR.ClearMt)
                                        {
                                            fr_ULwarn.btn_abort.Text = VAR.IsChinese?"清料":"Clear";
                                            fr_ULwarn.btn_abort.Visible = true;
                                        }
                                        fr_ULwarn.TopMost = true;
                                        fr_ULwarn.BackColor = Color.Yellow;
                                        fr_ULwarn.lb_msg.Text = VAR.IsChinese ? string.Format("提示:{0}连续取料3次失败,请确认料盘是否放反!\r\n  1.如果放反请按更换料盘键进行料盘更换!\r\n  2.如没有放反请按确认键分析原因!\r\n  3.如清料请按清料键!", xt.disc): string.Format("Tip: {0} failed to pick mod 3 consecutive times, please confirm whether the tray is reversed! \r\n1. If it is reversed, please press the replace tray key to replace the tray! \r\n 2. Press the OK button to analyze the cause! \r\n 3.If you want to clear the material, please press the clear button!\r\n提示:{0}连续取料3次失败,请确认料盘是否放反!\r\n  1.如果放反请按更换料盘键进行料盘更换!\r\n  2.如没有放反请按确认键分析原因!\r\n  3.如清料请按清料键!", xt.disc);                                      
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}连续取料3次模组失败!", xt.disc): string.Format("({0} failed to pick mod 3 consecutive times        {0}连续取料3次模组失败!)", xt.disc));
                                        DialogResult logres=fr_ULwarn.ShowDialog();
                                        if (DialogResult.Cancel == logres)
                                        {
                                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                                            return EM_RES.END;
                                            
                                        }
                                        else if (DialogResult.Abort == logres)
                                        {
                                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                                            return EM_RES.NEXT;
                                           
                                        }
                                        return res;
                                    }
                                    else continue;
                                }
                            }
                        }
                    }

                }
                else
                {
                    //飞拍取料
                    IsFly = false;
                    res = FlyToTrayPickMod(ref bquit, PlaceMod, ListPickMod,pickone);
                    if (res != EM_RES.OK)
                    {
                        if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR) continue;
                        else return res;
                    }
                }

                if ((((xt1.cy_zk.isONByChkSen && xt2.cy_zk.isONByChkSen && !pickone)||(xt2.cy_zk.isONByChkSen && pickone)) && !VAR.gsys_set.isDemoMode) || VAR.gsys_set.isDemoMode) return EM_RES.OK;
            }
            if (TryCnt >= 5)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料轴连续取料尝试次数超过5次!");
            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 上料动作
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="ws"></param>
        /// <returns></returns>

        public static EM_RES UploadModleAct(ref bool bquit, WS ws)
        {


            List<Product.Tray.PosInf> ListPickMod = new List<Product.Tray.PosInf>();
            //List<Product.Tray.PosInf> PickMod = new List<Product.Tray.PosInf>();
            //下相机拍照失败重拍后重新取料次数
            int dwErrTryPick = 0;
            bool ChangTray = false;
            final = false;
            EM_RES res = EM_RES.OK;
            //取料
            while (!bquit && !final)
            {
                ws.breschanged = true;
                if (ws.FeedStatus != WS.EM_STA.REDAYFORUPLOAD)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}上料时，状态异常，{1}!", ws.disc, ws.FeedStatus.ToString()));
                    return EM_RES.ERR;
                }

                //检测是否有可料的工位
                GetWsListPos(ref NextPlaceMod, ws);
                if (NextPlaceMod.Count == 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}放料完成或找不到可以放料的位置!", ws.disc));
                    res = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);
                    if (res != EM_RES.OK) return res;
                    final = true;
                    return EM_RES.OK;
                }
                //料盘取料
                if (UnloadProcess == EM_STA.PICK)
                {
                    //等待换料盘完成
                    while (!bquit && traybox_fd.tray_cur!=null && !traybox_fd.IsReady)
                    {
                        Thread.Sleep(100);
                    }

                    //取料
                    res = ToTrayPickMod(ref  bquit, ref  backcam, ws);

                    if(res != EM_RES.ABORT)
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("上料线程取料结束,{0}", res.ToString()));

                    if (res == EM_RES.OK)
                    {
                        
                        UnloadProcess = EM_STA.PLACE;
                        PlaceMod.Clear();
                        //定位下一个待料料盘的位置
                        if (traybox_fd.tray_cur != null)
                            ListPickMod = traybox_fd.tray_cur.GetPosList();
                        else
                            ListPickMod.Clear();

                        if (ListPickMod.Count > 0)
                        {
                            GetWsListPos(ref NextPlaceMod, ws);
                            Task tsk = new Task(() =>
                            {
                                MovNextTrayXPos(ListPickMod[0], NextPlaceMod);
                            });
                            tsk.Start();
                        }
                        else
                        {
                            ChangTray = true;
                        }
                    }
                    else if (res == EM_RES.PARA_OUTOFRANG || res == EM_RES.END)
                    {
                        ChangTray = true;
                    }
                    else if (res == EM_RES.NEXT)
                    {
                        double pos_tray_x = 0;
                        //如有物料放回原位
                        foreach (XT xt in COM.ListXT)
                        {
                            if (!xt.cy_zk.isONByChkSen && !VAR.gsys_set.isDemoMode)
                                continue;
                           
                              pos_tray_x = -xt.xt_pos_pick_mod.x + ax_x.fenc_pos + xt.xt_pos_pick_tray_x;
                            if (pos_tray_x < 0)
                            {
                                xt.xt_pos_pick_mod.x = ax_x.fenc_pos - pos_tray_x;
                                pos_tray_x = 0;
                            }
                            else
                            {
                               xt.xt_pos_pick_mod.x = ax_x.fenc_pos;
                            }
                            if (pos_tray_x > traybox_fd.fd_safe_x)
                            {
                                res = traybox_fd.PosTrayMovIn(ref bquit);
                                if (res != EM_RES.OK) return res;
                            }
                            //move tray_x
                            res = MT.Move(ref bquit, ref traybox_fd.ax_x, pos_tray_x);
                            if (res != EM_RES.OK) return res;
                            res = xt.PickOrPlace(ref bquit, xt.xt_pos_pick_mod, false,VAR.gsys_set.isDemoMode);
                            if (res != EM_RES.OK) return res;
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "清料中，不上料!");
                        res = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);
                        if (res != EM_RES.OK) return res;
                        final = true;
                        return EM_RES.OK;
                    }
                    else
                    {
                        if (res != EM_RES.ABORT) return res;

                    }


                    //更换料盘
                    if (ChangTray)
                    {
                       
                        bWaitforUpload = true;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("请更换上料盘======================="));
                        while (bWaitforUpload == true && !bquit)
                        {
                            Thread.Sleep(100);
                        }
                        ChangTray = false;
                        if (traybox_fd.IsReady)
                        {
                            traybox_fd.IsReady = false;
                            Task tskfd = new Task(() =>
                            {
                                traybox_fd.Tray_Action(false);
                            });
                            tskfd.Start();
                        }
                    }
                }

                //放料
                if (UnloadProcess == EM_STA.PLACE)
                {
                    //取空的测试位
                    if (PlaceMod.Count == 0)
                    {
                        GetWsListPos(ref PlaceMod, ws);
                    }

                    //没有找到放料位跳出
                    if (PlaceMod.Count == 0)
                    {
                        //放完回到原点位置
                        UnloadProcess = EM_STA.PICK;
                        res = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);
                        if (res != EM_RES.OK) return res;
                        final = true;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}放料完成!", ws.disc));
                        return EM_RES.OK;
                    }
                    //放料
                    res = FlyToWsPlaceMod(ref bquit, PlaceMod);
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("上下料结束,{1}", res.ToString()));
                    if (res == EM_RES.OK)
                    {
                        backcam = true;
                        dwErrTryPick = 0;
                        //找下一个放料位
                        UnloadProcess = EM_STA.PICK;
                        //清料
                        if (VAR.ClearMt)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "清料中，不上料!");
                            res = MT.ZupMove(ref bquit, ref ax_x, 0, ref ax_y, 0);
                            if (res != EM_RES.OK) return res;
                            final = true;
                            backcam = false;
                            //Task tskfdtrayin = new Task(() =>
                            //{
                            //    traybox_fd.Tray_Action(TrayBox.EM_DIR.ONLY_IN);
                            //});
                            //tskfdtrayin.Start();
                            return EM_RES.OK;
                        }
                    }
                    //下相机拍照NG重取物料
                    else if (res == EM_RES.PLACE_ERR)
                    {
                        dwErrTryPick++;
                        backcam = false;
                        UnloadProcess = EM_STA.PICK;
                        if (dwErrTryPick > 1)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("重取物料,{0}飞拍失败,请确认原因!", CamDw.disc));
                            //todo
                            //放回料盘
                            return EM_RES.ERR;
                        }
                        else
                        {
                             VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}飞拍失败,重取物料!", CamDw.disc));
                            continue;
                        }
                    }
                    else return res;
                }
            }

            return EM_RES.OK;
        }
        /// <summary>
        /// 下料模块复位
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public static EM_RES ZHome(ref bool bquit)
        {
            //确认是否使能
            int alm_time = 2000;
            ax_z.home_status = AXIS.HOME_STA.HOMING;
            foreach (XT xt in COM.ListXT)
            {
                if (!xt.cy_zk.isONByChkSen)
                    xt.cy_zk.SetOff();
            }
            try
            {
                if (!ax_z.isSVRON)
                {
                    ax_z.SVRON = true;
                    //wait for svr on
                    Thread.Sleep(1000);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ax_z.home_status = AXIS.HOME_STA.ERROR;
                return EM_RES.ERR;

            }
            //复位开始
            io_out_zrst.SetOn();
            Thread.Sleep(200);
            io_out_zrst.SetOff();
            //等待复位完成
            while (true)
            {
                //quit
                Thread.Sleep(5);
                if (bquit) return EM_RES.QUIT;
                if (io_in_zrst.isON)
                {
                    Thread.Sleep(30);
                    if (io_in_zrst.isON) break;
                }
                if (alm_time-- < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 复位超时!", ax_z.disc));
                    ax_z.home_status = AXIS.HOME_STA.ERROR;
                    return EM_RES.TIMEOUT;
                }

            }
            Thread.Sleep(500);
           // MT.Move(ref bquit, ref ax_z, ax_z.home_offset);  
            ax_z.fcmd_pos = 0;
            ax_z.fenc_pos = 0;
            

            //if (Math.Abs(ax_z.fenc_pos) > 0.1)
            //    res = MT.Move(ref bquit, ref ax_z, 0);
            //if (res != EM_RES.OK) return res;

            ax_z.home_status = AXIS.HOME_STA.OK;
            return EM_RES.OK;
        }

        /// <summary>
        /// 上料模块复位
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public static EM_RES Home(ref bool bquit)
        {
            EM_RES ret = EM_RES.OK;

            if (bquit) return EM_RES.QUIT;

            //res = MT.AxisHome(ref bquit, ax_z);
            //if (res != EM_RES.OK) return res;
            //确保下料气缸抬升
            //foreach (Cylinder cy in MT.List_CLD_UD_HD) cy.SetOff();
            //下料Z轴先抬升
            ret = MT.AxisHome(ref bquit, DownloadModle.ax_z);
            if (ret != EM_RES.OK) return ret;

            //Z轴先抬升
            ret = ZHome(ref bquit);
            if (ret != EM_RES.OK) return ret;

            //确保Z原点感应
            if (!io_in_zrst.isON)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 未在原点处，有撞机风险!", ax_z.disc));
                return EM_RES.ERR;
            }

            //检查下料模块是否已经抬起
            if (!DownloadModle.isUp) return EM_RES.MOVE_PROTECT;

            //other axis
            ret = MT.AxisHome(ref bquit, ax_x, ax_y, ax_u1, ax_u2, traybox_fd.ax_x);
            if (ret != EM_RES.OK) return ret;

            ret = MT.AxisHome(ref bquit, traybox_fd.ax_z);
            if (ret != EM_RES.OK) return ret;
            
            return ret;
        }
        /// <summary>
        /// 停止轴运动，停止Home动作
        /// </summary>
        public static void Stop()
        {
            ax_x.bhomequit = true;
            ax_x.Stop();

            ax_y.bhomequit = true;
            ax_y.Stop();

            ax_u1.bhomequit = true;
            ax_u1.Stop();

            ax_u2.bhomequit = true;
            ax_u2.Stop();

            traybox_fd.ax_x.bhomequit = true;
            traybox_fd.ax_x.Stop();

            traybox_fd.ax_z.bhomequit = true;
            traybox_fd.ax_z.Stop();
        }

        //静拍检查更新工装上模组状态(是否有模组，模组二维码)
        //检查后把NG料下料
        public static EM_RES CheckAndUpdateMdOnWs(ref bool bquit,bool bChkBardcode = true)
        {
            //获取当前转盘位置       
            WS ws = Turntable.GetWSOnFeedPos;
            if (ws == null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "检查工站模组时，找不工站");
                return EM_RES.ERR;
            }

            //切到上料状态
            EM_RES res = ws.SetupForFeed(ref bquit);
            if (res != EM_RES.OK) return res;

            foreach (WS.MdDat md in ws.list_md)
            {
                //搜寻模组
                //识别二维码
                if (bChkBardcode)
                {

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
        public static EM_RES PickMdFromTray(ref bool bquit, int cnt, ref bool IsFly)
        {
            EM_RES res = EM_RES.OK;
            int TryPickCnt = 0, TryCnt = 0;
            bool pickone = false;

            //check cnt
            if (cnt == 0) return EM_RES.OK;

            //check zk
            if (!VAR.gsys_set.isDemoMode)
            {
                //pick tow
                if(xt1.cy_zk.isONByChkSen && xt2.cy_zk.isONByChkSen)
                    return EM_RES.OK;
                //pick one
                if (cnt == 1 && (xt1.cy_zk.isONByChkSen || xt2.cy_zk.isONByChkSen))
                    return EM_RES.OK;
            }

            //check tray
            if (!traybox_fd.IsReady)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "取料时，料盘未就绪");
                return EM_RES.ABORT;
            }
            if (traybox_fd.tray_cur == null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "取料时，料盘为null");
                return EM_RES.PARA_ERR;
            }
            //get md form tray
            List<Product.Tray.PosInf>  listOfMdForPick = traybox_fd.tray_cur.GetPosList();
            if (listOfMdForPick == null || listOfMdForPick.Count == 0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "取料时，料盘已空");
                //todo
                //换盘
                return EM_RES.END;
            }

            //取料
            while (TryCnt++ < 5)
            {
                //get md form tray
                listOfMdForPick = traybox_fd.tray_cur.GetPosList();
                if (listOfMdForPick == null || listOfMdForPick.Count == 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "取料时，料盘已空");
                    //todo
                    //换盘
                    return EM_RES.END;
                }

                //静拍
                if (ax_y.fenc_pos < xt1.st_rol_cap.y)
                {
                    foreach (XT xt in COM.ListXT)
                    {
                        //如果只有一个目标，则吸头2吸料
                        if (cnt ==1 && xt.id == COM.xt1.id)
                            continue;
                        string barcode;
                        //定拍取料
                        res = xt.XtCapMovMod(ref bquit,out barcode, listOfMdForPick[0].Pos[0], CONST.ModUpFw,10,XT.EM_XTFLOW.PICKMOD, true,VAR.gsys_set.isDemoMode);
                        if (res == EM_RES.OK)
                        {
                            //更新料盘
                            TryPickCnt = 0;
                            xt.XtMd = listOfMdForPick[0].md;
                            xt.XtMd.bardcode = COM.CamUp1.curTask.ResData.BarCode;
                            listOfMdForPick[0].md = null;

                        }
                        else
                        {
                            if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR)
                            {
                                listOfMdForPick[0].md.res = (int)Product.EM_CM_RES.CAMERR;
                                TryPickCnt++;
                                if (TryPickCnt > 2)
                                {                                    
                                    
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}连续取料3次模组失败!", xt.disc));
                                    return res;
                                }
                                else continue;
                            }
                        }

                    }

                }
                else
                {
                    //飞拍取料
                    IsFly = false;
                    res = FlyToTrayPickMod(ref bquit, PlaceMod, listOfMdForPick, pickone);
                    if (res != EM_RES.OK)
                    {
                        if (res == EM_RES.CAM_ERR || res == EM_RES.TIMEOUT || res == EM_RES.PICK_ERR) continue;
                        else return res;
                    }
                }

                if ((((xt1.cy_zk.isONByChkSen && xt2.cy_zk.isONByChkSen && !pickone) || (xt2.cy_zk.isONByChkSen && pickone)) && !VAR.gsys_set.isDemoMode) || VAR.gsys_set.isDemoMode) return EM_RES.OK;
            }
            if (TryCnt >= 5)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料轴连续取料尝试次数超过5次!");
            }
            return EM_RES.OK;
        }

        #region 上料线程
        public static Task UploadTask = null;
        public static bool brun = false;
        public static bool bquit = false;
        public static void RunTask()
        {
            if (UploadTask == null || UploadTask != null && UploadTask.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "创建上料线程");
                if (UploadTask != null) UploadTask.Dispose();
                UploadTask = new Task(Upload);
                brun = true;
                UploadTask.Start();
                status = EM_STA.BUSY;
            }
        }

        public static EM_RES WaitTask(ref bool bquit)
        {
            if (UploadTask == null) return EM_RES.PARA_ERR;

            while (!bquit)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                if (UploadTask.IsCompleted) break;
                if (!brun) break;
            }
            if (status == EM_STA.READY)
                return EM_RES.OK;
            else
                return EM_RES.ERR;
        }
        public static void Upload()
        {
            EM_RES res = EM_RES.OK;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "上料线程开始...");
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
                    Thread.Sleep(200);
                    continue;
                }

                //等待上料                
                if (ws.FeedStatus != WS.EM_STA.REDAYFORUPLOAD)
                {
                    Thread.Sleep(200);
                    continue;
                }

                //检查当前工站状态
                if (ws.TestStatus == WS.EM_TEST_STA.ERROR)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("上料线程检测到{0}状态异常!", ws.disc));
                    status = EM_STA.ERR;
                    break;
                }

                //确保当前工站准备好上料
                if (!ws.isInFeedPos)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 上料时，不在上下料位!", ws.disc));
                    res = EM_RES.ERR;
                    break;
                }

                //上下料动作
                status = EM_STA.PLACE;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上料准备...", ws.disc));

                while (bquit == false&&VAR.gsys_set.bquit ==false)
                {
                    Thread.Sleep(10);
                    if (Monitor.TryEnter(COM.UDLockObj, 1000)) 
                        break;
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}开始上料...", ws.disc));
                int tick = Environment.TickCount;

                foreach (Cylinder cy in ws.list_cld_fr)
                {
                    cy.SetOn();
                }                
                res = UploadModleAct(ref VAR.gsys_set.bquit, ws);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("上料线程完成,{0}", res.ToString()));
                Monitor.Exit(COM.UDLockObj);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "上料线程，exit wait");

                bWaitforUpload = true;
                while (bWaitforUpload == true && !bquit)
                {
                    Thread.Sleep(100);
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "bWaitforUpload");
                foreach (Cylinder cy in ws.list_cld_fr)
                {
                    cy.SetOff();
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, " cy.SetOff");
                Thread.Sleep(500);
              
                if (res != EM_RES.OK)
                {
                    break;
                }

               

                //上完闭合
                res = ws.SetupForTest(ref VAR.gsys_set.bquit);
                if (res != EM_RES.OK) break;

                //复位测试结果
                ws.ResetResultOfMd();
                //更新物料状态
                ws.TestStatus = WS.EM_TEST_STA.UNTEST;

                //提前开图
                if (PT_SET.turnon)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 启动测试", ws.disc));
                    res = ws.StartTestFlow();
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
                }


                //计时开始
                ws.tmr = System.Environment.TickCount;

                //上料机构复位
               // res = MT.ZupMove(ref VAR.gsys_set.bquit, ref ax_y, 0);

                //通知测试
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 通知REDAYFORTEST", ws.disc));
                ws.FeedStatus = WS.EM_STA.REDAYFORTEST;

                Ct = (Environment.TickCount - tick) / 1000.0;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("上料用时{0:F3}s", Ct));
            }

            if (res != EM_RES.OK)
            {
                status = EM_STA.ERR;
            }
            else
            {
                status = EM_STA.READY;
            }
            brun = false;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "上料线程结束!");
        }
        #endregion
    }
}