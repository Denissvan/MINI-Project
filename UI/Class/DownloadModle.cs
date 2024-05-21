using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Cognex.VisionPro.Exceptions;
using MotionCtrl;
using Color = System.Drawing.Color;

namespace UI
{
    public static class DownloadModle
    {
        
        public static TrayBox traybox_ok = COM.traybox_ok;
        public static TrayBox traybox_ng = COM.traybox_ng;
        public static List<Cylinder> List_CLD_HD_HD = new List<Cylinder>();
        public static List<Cylinder> List_CLD_UD_HD = new List<Cylinder>();
        //public static Cylinder CLD_DL_HD_TRAY_NG = MT.CLD_DL_HD_TRAY_NG;
        //public static Cylinder CLD_DL_HD_TRAY_OK = MT.CLD_DL_HD_TRAY_OK;

        public static AXIS ax_y = null;
        public static AXIS ax_z = null;
        public static List<WS.MdDat> List_md = new List<WS.MdDat>();
        public static List<Product.Tray.PosInf> List_traypos = new List<Product.Tray.PosInf>();
        public static double Dwload_Ysafe = 360;
        public static double Dwload_Zsafe = 24;
        public static bool Dwload_Zsafe_IsOn = false;
        public static EM_STA DownloadProcess = EM_STA.PICK;
        public static double Ct = 0;
        public static double cy_high = 10;//气缸行程
        public enum EM_STA
        {
            [Description("未知")]
            UNKNOW,
            [Description("等待")]
            WAIT,
            [Description("忙")]
            BUSY,
            [Description("回零中")]
            HOME,
            [Description("就绪")]
            READY,
            [Description("放料")]
            PLACE,
            [Description("取料")]
            PICK,
            [Description("错误")]
            ERR
        }

        static double y = 0, z = 0;
        static bool btsk = false;
        public static string StrOfPos
        {
            get
            {
                Task tsk = new Task(() =>
                {
                    if (btsk) return;
                    btsk = true;
                    y = ax_y != null ? ax_y.fcmd_pos : 0;
                    z = ax_z != null ? ax_z.fcmd_pos : 0;
                    btsk = false;
                });
                tsk.Start();
                return string.Format("Y: {0:000.000}\nZ: {1:000.000}", y, z);
            }
        }

        public static EM_STA status = EM_STA.UNKNOW;
        /// <summary>
        /// 获取工站数据
        /// </summary>
        /// <param name="ws">工站</param>
        public static EM_RES GetListMdDat(WS ws)
        {
          
            if(ws.list_md.Count==0)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}没有模组信息",ws.disc));
                return EM_RES.PARA_ERR;
            }
            List_md.Clear();
            foreach (WS.MdDat md in ws.list_md)
            {
                List_md.Add(md.Clone());
                md.res = -2;
                md.bardcode = "";
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 确认有没有可放物料
        /// </summary>
        /// <returns></returns>
        public static bool GetPlaceMdStatus()
        {
            if (List_md == null) return false;
            foreach (WS.MdDat md in List_md)
            {
                //if (VAR.gsys_set.isChkMode && VAR.ChkPC != md.PC_ID) continue;
                if (md.benable  && md.res >-1)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查是否有没有吸到料的吸头
        /// </summary>
        /// <returns> true :存在 false: 不存在</returns>
        public static bool ChkZkHavNo()
        {
            foreach (WS.MdDat md in List_md)
            {
                if (md.Num - 1 < List_CLD_HD_HD.Count)
                {
                    if (!List_CLD_HD_HD[md.Num - 1].isOFFByChkSen)
                    {
                        return true;
                    }

                }

            }
            return false;
        }

        public static EM_RES MovSafePoint()
        {
            EM_RES res = EM_RES.OK;
            foreach (Cylinder cy in List_CLD_UD_HD)
            {
                cy.SetOff();
            }

            //提起Z轴
            bool bq = false;
            res= ax_z.MoveTo(ref bq, 0, 5000);
            if (res == EM_RES.OK)
            {
                //移动到安全位置
                res= MT.Move(ref bquit, ref ax_y,
                    ax_y.fenc_pos > (Dwload_Ysafe - 1) ? (Dwload_Ysafe - 10) : ax_y.fenc_pos);
               
            }
            return res;
        }

        /// <summary>
        /// 下料流程
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static EM_RES DownloadModleAct(ref bool bquit, WS ws)
        {
            EM_RES res;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("下料轴取{0}模组",ws.disc));
            res = PickMod(ref bquit, ws);
            if (res != EM_RES.OK) return res;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("下料轴取料完成更新{0}模组信息!", ws.disc));
            res = GetListMdDat(ws);
            if (res != EM_RES.OK) return res;
            res = Place_Md(ref bquit);
            if (res != EM_RES.OK) return res;

            return EM_RES.OK;
        }

        //取料
        public static EM_RES PickMod(ref bool bquit,WS ws,int delay=500)
        {
            //判断当前工站是否有可做物料
            EM_RES res;
            int cnt = 0;

            //try
            {
                //提取md
                List_md.Clear();
                foreach (WS.MdDat md in ws.list_md)
                {
                    //if (VAR.gsys_set.isChkMode && VAR.ChkPC != md.PC_ID) continue;
                    if (!md.benable || md.res > -1)
                    {                       
                        List_md.Add(md.Clone());
                    }
                }

                if (List_md.Count==0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}没有物料退出下料线程!", ws.disc));
                    return EM_RES.END;
                }

                //提起Z轴
                res = MT.Move(ref bquit, ref ax_z, 0);
                if (res != EM_RES.OK)
                {
                    List_md.Clear();
                    return res;
                }

                //关闭全部上下气缸
                Cld_Hd_Action(false);
                //关闭全部上下气缸
                Cld_Ud_Action(ref bquit, false);
                //确认安全
                if (!isUp)
                {
                    List_md.Clear();
                    return EM_RES.ERR;
                }

                
                //跑到取料位
                res = MT.Move(ref bquit, ref ax_y, ws.pos_Dwload_Pick.y);
                if (res != EM_RES.OK)
                {
                    List_md.Clear();
                    return res;
                }

                if (ws.FeedStatus != WS.EM_STA.REDAYFORDOWNLOAD)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}下料时，状态异常，{1}!", ws.disc, ws.FeedStatus.ToString()));
                    List_md.Clear();
                    return EM_RES.ERR;
                }

                //松开吸气
                foreach (WS.MdDat md in List_md)
                {
                    if (md.Num - 1 < List_CLD_HD_HD.Count)
                    {
                       List_CLD_HD_HD[md.Num - 1].SetOff();
                    }
                }

                //下气缸
                //foreach(WS.MdDat md in List_md)
                //{
                //    if (md.Num - 1 < List_CLD_UD_HD.Count)
                //    {
                //        List_CLD_UD_HD[md.Num -1].SetOn();
                //    }
                //}

                //下降
                res = MT.Move(ref bquit, ref ax_z, ws.pos_Dwload_Pick.z);
                if (res != EM_RES.OK)
                {
                    List_md.Clear();
                    return res;
                }

                //等到位
                //foreach (WS.MdDat md in List_md)
                //{
                //    if (md.Num - 1 < List_CLD_UD_HD.Count)
                //    {
                //        res = List_CLD_UD_HD[md.Num - 1].SetOn(ref bquit,2000);
                //        if (res != EM_RES.OK)
                //        {
                //            List_md.Clear();
                //            return res;
                //        }                        
                //    }
                //}

                //Thread.Sleep(200);

                //开真空
                foreach (WS.MdDat md in List_md)
                {
                    if (md.Num - 1 < List_CLD_HD_HD.Count)
                    {
                        List_CLD_HD_HD[md.Num - 1].SetOn();
                     
                    }
                }

                //更新工装状态
                foreach (WS.MdDat md in ws.list_md)
                {
                    //if(VAR.gsys_set.isChkMode && VAR.ChkPC != md.PC_ID)continue;
                    if (!md.benable || md.res > -1)
                    {
                        md.res = -2;
                        md.bardcode =null;
                    }

                }

                Thread.Sleep(delay);

                res = MT.Move(ref bquit, ref ax_z, ws.pos_Dwload_Pick.z - 3);
                if (res != EM_RES.OK) return res;

                res = MT.Move(ref bquit, ref ax_z, ws.pos_Dwload_Pick.z);
                if (res != EM_RES.OK) return res;

                Thread.Sleep(delay-200);
                ////打开所有气缸
                //res = Cld_Ud_Action(ref bquit, true);
                //if (res != EM_RES.OK) return res;
                //Thread.Sleep(200);
                //Cld_Hd_Action(true);
                //Thread.Sleep(delay);


                //foreach (WS.MdDat md in List_md)
                //{
                //    if (md.Num - 1 < List_CLD_UD_HD.Count)
                //    {
                //        List_CLD_UD_HD[md.Num - 1].SetOff();
                //    }
                //}

                //抬气缸
                Cld_Ud_Action(ref bquit, false);

                //提起Z轴
                res = ax_z.MoveTo(ref bquit, 0, 5000);
                if (res != EM_RES.OK) return res;

                //检测真空
                bool ZC_Cnt = false;
                ZC_Cnt = ChkZkHavNo();
                if (ZC_Cnt)
                {
                    //下降气缸
                    foreach (WS.MdDat md in List_md)
                    {
                        if (md.Num - 1 < List_CLD_HD_HD.Count)
                        {
                            if (!List_CLD_HD_HD[md.Num - 1].isOFFByChkSen)
                            {
                                List_CLD_UD_HD[md.Num - 1].SetOn();
                            }

                        }

                    }
                    Thread.Sleep(300);
                    //下降Z轴
                    res = ax_z.MoveTo(ref bquit, ws.pos_Dwload_Pick.z - cy_high+2, 5000);
                    if (res != EM_RES.OK) return res;

                    Thread.Sleep(delay);
                    //抬气缸
                    Cld_Ud_Action(ref bquit, false);

                    //提起Z轴
                    res = ax_z.MoveTo(ref bquit, 0, 5000);
                    if (res != EM_RES.OK) return res;
                }

                //检查气缸
                foreach (WS.MdDat md in List_md)
                {
                    if (md.Num - 1 < List_CLD_UD_HD.Count)
                    {
                        res = List_CLD_UD_HD[md.Num - 1].SetOff(ref bquit,2000);
                        if (res != EM_RES.OK) 
                            return res;
                    }

                }

                ////提起Z轴
                //res = ax_z.MoveTo(ref bquit, 0);
                //if (res != EM_RES.OK) return res;
                ////提升气缸
                //res = Cld_Ud_Action(ref bquit, false);
                //if (res != EM_RES.OK) return res;
                ////等待定位结束
                //res = ax_z.WaitForMoveDone(ref bquit, 0, 10000);
                //if (res != EM_RES.OK) return res;

               
            }
            //finally
            //{
            //    //抬气缸
            //    foreach (Cylinder cy in List_CLD_UD_HD)
            //    {
            //        cy.SetOff();
            //    }
            //    //提起Z轴
            //    bool bq = false;
            //    res = ax_z.MoveTo(ref bq, 0, 5000);
            //    if (res == EM_RES.OK)
            //    {
            //        //移动到安全位置
            //        res = MT.Move(ref bquit, ref ax_y, ax_y.fenc_pos > (Dwload_Ysafe - 1) ? (Dwload_Ysafe - 10) : ax_y.fenc_pos);
            //    }
            //}
            return res;
        }

        public static EM_RES MovPlace(ref bool bquit, WS.MdDat md, TrayBox traybox)
        {
            ST_XYZ ofs_md;
            EM_RES res;
            Product.MdDat pMddat = new Product.MdDat();
            ST_XYZ pos = new ST_XYZ();
           if(traybox.disc == traybox_ng.disc)
            List_traypos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY,md.res);
           else if(traybox.disc == traybox_ok.disc)
            List_traypos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            if (List_traypos.Count > 0)
            {
                ofs_md = md.st_pos[0] -COM.list_ws[md.WS_ID].list_md[0].st_pos[0];
                pos.x = List_traypos[0].Pos[0].x + ofs_md.x;
                pos.y = List_traypos[0].Pos[0].y - ofs_md.y;
                pos.z = List_traypos[0].Pos[0].z;
                // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("下料夹爪{0}放料于{1}当前料盘位置{2},坐标X：{3:f3},Y：{4:f3}", md.Num, traybox.disc, List_traypos[0].idx, posInf.x, posInf.y));
                res = Place(ref  bquit, traybox, pos, md.Num - 1);
                if (res == EM_RES.OK)
                {
                    pMddat.res = md.res;
                    pMddat.bardcode = md.bardcode;
                    traybox.tray_cur.Push(pMddat, List_traypos[0].idx);
                    md.res = -2;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("下料轴放料->夹爪:{0} 仓储:{1} 料盘:{2} 位置:{3} 模组结果{4} BC:{5}", md.Num, traybox.disc, traybox.tray_idx, List_traypos[0].idx, pMddat.res, pMddat.bardcode));
                }
                else
                    return res;
                if (List_traypos.Count == 1)
                    return EM_RES.END;
            }
            else
            {
                return EM_RES.PARA_OUTOFRANG;
            }
            return EM_RES.OK;
        }


        public static EM_RES Tray_Place_Md(ref bool bquit, TrayBox traybox)
        {
            EM_RES res;
            if (!traybox.IsReady) return EM_RES.ABORT;
            if (traybox.tray_cur == null) return EM_RES.PARA_OUTOFRANG;
             List_traypos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            if (List_traypos.Count==0) return EM_RES.END;
            foreach (WS.MdDat md in List_md)
            {
                // if (bquit) return EM_RES.QUIT;
                if (!md.benable)
                {
                    md.res = -2;
                    List_CLD_HD_HD[md.Num-1].SetOff();
                    continue;
                }
                if (traybox.disc == traybox_ok.disc && md.res != 0) continue;
                if (traybox.disc == traybox_ng.disc && md.res <1) continue;               
                res = MovPlace(ref bquit, md, traybox);
                if (res != EM_RES.OK) return res;
                           
            }
            return EM_RES.OK;
        }

        
        /// <summary>
        /// 放料
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public static EM_RES Place_Md(ref bool bquit)
        {
            EM_RES res;
            int cnt=0;
            if (List_md.Count == 0) return EM_RES.OK;
            while (!bquit)
            {
                //仓储进出料
                res = Tray_Place_Md(ref bquit, traybox_ok);
                if (res == EM_RES.PARA_OUTOFRANG||res==EM_RES.END)
                {
                    if(traybox_ok.IsReady)
                    {
                        traybox_ok.IsReady = false;
                        Task tskok = new Task(() =>
                        {
                            traybox_ok.Tray_Action(false);
                        });
                        tskok.Start();
                    }                  
                   
                }
                else if(res!=EM_RES.OK&&res!=EM_RES.ABORT)
                {
                    return res;
                }


                res = Tray_Place_Md(ref bquit, traybox_ng);
                if (res == EM_RES.PARA_OUTOFRANG || res == EM_RES.END)
                {
                    UploadModle.bWaitforUpload = true;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("请更换下料盘======================="));
                    while (UploadModle.bWaitforUpload == true && !bquit)
                    {
                        Thread.Sleep(100);
                    }   

                    if (traybox_ng.IsReady)
                    {
                        traybox_ng.IsReady = false;
                        Task tskng = new Task(() =>
                        {
                            traybox_ng.Tray_Action(false);
                        });
                        tskng.Start();
                    }
                }
                else if (res != EM_RES.OK && res != EM_RES.ABORT)
                {
                    return res;
                }

                //确认物料完成
                cnt = 0;
                foreach(WS.MdDat md in List_md)
                {
                    if (md.res == -2) cnt++;
                }
                if (cnt == List_md.Count)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料轴放料完成!");
                    List_md.Clear();
                    break;
                }
            }
           
           
            return EM_RES.OK;
        } 
        //放料
        public static EM_RES Place(ref bool bquit,TrayBox traybox, ST_XYZ pos_Dwload_Place,int num, int delay = 200)
        {
            try
            {
                EM_RES res;
                if (num > List_CLD_UD_HD.Count)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "放料吸头num大于吸头个数!");
                    return EM_RES.PARA_ERR;
                }
                //关闭全部上下气缸
                Cld_Ud_Action(ref bquit, false);

                res = MT.Move(ref bquit, ref ax_z, Dwload_Zsafe);
                if (res != EM_RES.OK) return res;
                //Thread.Sleep(300);
                
                ////确认安全
                Dwload_Zsafe_IsOn = true;
                //if (!isUp)
                //    return EM_RES.ERR;
                //料盘进仓储位置确认
                if (pos_Dwload_Place.x > traybox.fd_safe_x)
                {
                    res = traybox.PosTrayMovIn(ref bquit);
                    if (res != EM_RES.OK) return res;
                }
                //跑到取料位
               
                res = MT.Move(ref bquit, ref traybox.ax_x, pos_Dwload_Place.x, ref ax_y, pos_Dwload_Place.y);
                if (res != EM_RES.OK) return res;

                //打开对应上下气缸
                res = List_CLD_UD_HD[num].SetOn();
                if (res != EM_RES.OK) return res;

                //下降
                res = MT.Move(ref bquit, ref ax_z, pos_Dwload_Place.z,3000);
                if (res != EM_RES.OK) return res;

                //打开对应上下气缸
                //res = List_CLD_UD_HD[num].SetOn();
               // if (res != EM_RES.OK) return res;                
                Thread.Sleep(300);
                //关闭对应夹紧气缸
                List_CLD_HD_HD[num].SetOff();
                Thread.Sleep(delay);

                //提升气缸
                res = List_CLD_UD_HD[num].SetOff(ref bquit, 2000);
                if (res != EM_RES.OK) return res;
                res = ax_z.MoveTo(ref bquit, Dwload_Zsafe,3000);
                if (res != EM_RES.OK) return res;
                return EM_RES.OK;
            }
            finally
            {
                Dwload_Zsafe_IsOn = false;
            }
        }
        //检查是否抬起
        public static bool isUp
        {
            get
            {                
                //check Cylinder
                foreach (Cylinder cy in List_CLD_UD_HD)
                {
                    if (!cy.isOFFByChkSen)
                    {
                        Thread.Sleep(2);
                        if (!cy.isOFFByChkSen)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 气缸未抬起!", cy.io_out1.disc));
                            return false;
                        }
                    }
                }


                //确保Z原点感应
                //if (!ax_z.isORG)
                //{
                //    Thread.Sleep(300);
                //    if (!ax_z.isORG)
                //    {
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 未在原点处，有撞机风险!", ax_z.disc));
                //        return false;
                //    }
                //}
                if((Math.Abs(ax_z.fenc_pos)>1&&!Dwload_Zsafe_IsOn)||((Math.Abs(ax_z.fenc_pos)>Dwload_Zsafe+0.1)&&Dwload_Zsafe_IsOn))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 当前坐标>1或大于安全高度，移动Y有撞机风险!", ax_z.disc));
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 同时打开与关闭全部夹紧气缸
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        public static EM_RES Cld_Hd_Action(bool OnAll=true)
        {
            
            foreach (Cylinder cyhd in List_CLD_HD_HD)
            {
                if (OnAll)
                {
                    cyhd.SetOn();
                }
                else
                {
                    cyhd.SetOff();
                  
                   
                }
            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 同时打开与关闭全部上下气缸
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        public static EM_RES Cld_Ud_Action(ref bool bquit, bool OnAll = true)
        {
            EM_RES res;
            foreach (Cylinder cyud in List_CLD_UD_HD)
            {
                if (OnAll)
                {
                    res = cyud.SetOn(ref bquit, 2000);
                    if (res != EM_RES.OK) return res;
                }
                else
                {
                    cyud.SetOff();
              
                }
            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 下料模块复位
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public static EM_RES Home(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;

            if (bquit) return EM_RES.QUIT;

            //确保上料X轴已复位，且位置安全
            if (UploadModle.ax_x.home_status != AXIS.HOME_STA.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 未复位，有撞机风险!", UploadModle.ax_x.disc));
                return EM_RES.ERR;
            }
            if (Math.Abs(UploadModle.ax_x.fenc_pos) > 10)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 未在原点附近(<10),X={1:F3}，有撞机风险!", UploadModle.ax_x.disc, UploadModle.ax_x.fenc_pos));
                return EM_RES.ERR;
            }

            //气缸抬升
            foreach (Cylinder cy in List_CLD_UD_HD) cy.SetOff();

            //先抬升
            res = MT.AxisHome(ref bquit, ax_z);
            if (res != EM_RES.OK) return res;

            //检查是否已经抬起
            if (!isUp) return EM_RES.MOVE_PROTECT;

            //Y复位
            res = MT.AxisHome(ref bquit, ax_y, traybox_ok.ax_x, traybox_ng.ax_x);
            if (res != EM_RES.OK) return res;


            res = MT.AxisHome(ref bquit, traybox_ok.ax_z, traybox_ng.ax_z);
            if (res != EM_RES.OK) return res;

            return EM_RES.OK;
        }
        /// <summary>
        /// 停止轴运动，停止Home动作
        /// </summary>
        public static void Stop()
        {
            ax_z.bhomequit = true;
            ax_z.Stop();

            ax_y.bhomequit = true;
            ax_y.Stop();

            traybox_ok.ax_x.bhomequit = true;
            traybox_ok.ax_x.Stop();

            traybox_ok.ax_z.bhomequit = true;
            traybox_ok.ax_z.Stop();

            traybox_ng.ax_x.bhomequit = true;
            traybox_ng.ax_x.Stop();

            traybox_ng.ax_z.bhomequit = true;
            traybox_ng.ax_z.Stop();
        }

        #region 下料线程
        public static Task DownloadTask = null;
        public static bool brun = false;
        public static bool bquit = false;
        public static void RunTask()
        {
            if (DownloadTask == null || DownloadTask != null && DownloadTask.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "创建上料线程");
                if (DownloadTask != null) DownloadTask.Dispose();
                DownloadTask = new Task(Download);
                brun = true;
                DownloadTask.Start();
                status = EM_STA.BUSY;
            }
        }
        public static EM_RES WaitTask(ref bool bquit)
        {
            if (DownloadTask == null) return EM_RES.PARA_ERR;

            while (!bquit)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                if (DownloadTask.IsCompleted) break;
                if (!brun) break;
            }
            if (status == EM_STA.READY)
                return EM_RES.OK;
            else
                return EM_RES.ERR;
        }
        public static void Download()
        {
            EM_RES res = EM_RES.OK,res1=EM_RES.OK;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料线程开始...");
            bquit = false;
            int tick = System.Environment.TickCount;
            bool IsHavMt = false;
            while (!VAR.gsys_set.bquit && bquit == false)
            {
                status = EM_STA.WAIT;
                brun = true;
                IsHavMt = false;
                //准备好料盘
                //todo

                //获取当前转盘位置
                if (DownloadProcess == EM_STA.PICK)
                {


                    res = EM_RES.OK;
                    WS ws = Turntable.GetWSOnFeedPos;
                    if (ws == null)
                    {
                        Thread.Sleep(200);
                        continue;
                    }

                    IsHavMt = GetPlaceMdStatus();
                    if (!IsHavMt)
                    {
                       
                        //等待下料
                        if (ws.FeedStatus != WS.EM_STA.REDAYFORDOWNLOAD)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        //检查当前工站是否有物料
                        if (ws.TestStatus == WS.EM_TEST_STA.EMPTY)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("{0} 下料线程检测到没有模组!", ws.disc));
                            //通知上料
                            ws.FeedStatus = WS.EM_STA.REDAYFORUPLOAD;
                            continue;
                        }

                        //确保当前工站准备好下料
                        if (!ws.isInFeedPos)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 下料线程检测到不在上料位!", ws.disc));
                            res = EM_RES.ERR;
                            ws.TestStatus = WS.EM_TEST_STA.ERROR;
                            break;
                        }

                        //下料
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "下料线程准备下料...");

                        while (bquit == false && VAR.gsys_set.bquit == false)
                        {
                            if (Monitor.TryEnter(COM.UDLockObj, 1000)) break;
                            Thread.Sleep(10);
                        }

                        tick = Environment.TickCount;
                        status = EM_STA.PICK;
                        res = PickMod(ref VAR.gsys_set.bquit, ws);
                        {
                            //抬气缸
                            //foreach (Cylinder cy in List_CLD_UD_HD)
                            //{
                            //    cy.SetOff();
                            //}

                            ////提起Z轴
                            //bool bq = false;
                            //res1 = ax_z.MoveTo(ref bq, 0, 5000);
                            //if (res1 == EM_RES.OK)
                            //{
                            //    //移动到安全位置
                            //    res1 = MT.Move(ref bquit, ref ax_y,
                            //        ax_y.fenc_pos > (Dwload_Ysafe - 1) ? (Dwload_Ysafe - 10) : ax_y.fenc_pos);
                            //}
                            res1 = MovSafePoint();
                            if (res1 != EM_RES.OK)
                                res = res1;
                        }
                        ////检测真空
                        bool Chk_Zk = false;
                        Chk_Zk = ChkZkHavNo();
                        if (Chk_Zk)
                        {
                            string msg = string.Empty;
                            foreach (WS.MdDat md in List_md)
                            {
                                if (md.Num - 1 < List_CLD_HD_HD.Count)
                                {
                                    if (!List_CLD_HD_HD[md.Num - 1].isOFFByChkSen)
                                    {
                                        msg = VAR.IsChinese ? msg + List_CLD_HD_HD[md.Num - 1].io_sen_off.str_disc + ",": msg + List_CLD_HD_HD[md.Num - 1].io_sen_off.english_disc + ",";
                                    }

                                }

                            }
                           
                            if (msg != string.Empty)
                            {
                                double uploadZpos=0;
                                if (UploadModle.traybox_fd.IsReady)
                                {
                                    uploadZpos = UploadModle.traybox_fd.ax_z.fcmd_pos;
                                }                               
                                MT.Move(ref bquit, ref UploadModle.traybox_fd.ax_z, 0);
                                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese?"下料异常!":"DwLoad ERR!", 20, true);
                                warning fr_DLwarn = new warning();
                                fr_DLwarn.TopMost = true;
                                fr_DLwarn.btn_ok.Text = VAR.IsChinese?"继续运行": "Keep running";
                                fr_DLwarn.btn_cancle.Text = VAR.IsChinese ? "停止运行": "Stop running";
                                fr_DLwarn.btn_ok.Visible = true;
                                fr_DLwarn.btn_cancle.Visible = true;
                                fr_DLwarn.BackColor = Color.Yellow;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?"下料有物料没有吸起报警!": "Alarm!No material sucked up when download!");
                                fr_DLwarn.lb_msg.Text = VAR.IsChinese?"提示:下料" + msg + "没检测到物料，请确认!\r\n  1.如果工站上没有物料,请按继续运行键!\r\n  " +
                                 "2.如有物料请按停止运行键退出运行，待界面左上角显示就绪后再按运行键!": "Tip: Download" + msg + "No material detected, please confirm!\r\n  1.If there is no material on the station, press the 'Keep running' button!\r\n  " +
                                                                        "2.If there is any material, please press the 'Stop running' button to exit the operation. After the 'ready' is displayed in the upper left corner of the interface, press the run button again!";
                               DialogResult logres=fr_DLwarn.ShowDialog();
                               if (DialogResult.Cancel == logres)
                               {
                                   //VAR.gsys_set.bquit = true;
                                   //UploadModle.bquit = true;
                                   //bquit = true;
                                   res = EM_RES.ERR;
                               }
                               else
                               {
                                   VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行":"RUN", 0, true);
                                    MT.OnlyOnelightON(0);
                                }

                               if (uploadZpos!= 0)
                               MT.Move(ref bquit, ref UploadModle.traybox_fd.ax_z, uploadZpos);
                            }
                        }

                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "上料线程，取料结束");
                        Monitor.Exit(COM.UDLockObj);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料线程，exit wait");
                        //检查真空

                        if (res != EM_RES.OK && res != EM_RES.END)
                        {
                            if (List_md.Count == 0)
                                break;
                        }

                        ws.TestStatus = WS.EM_TEST_STA.EMPTY;

                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR,
                            string.Format("下料线程{0}取料完成,T={1:F2}s", ws.disc, (tick - Environment.TickCount) / 1000.0));
                    }

                    if (ax_y.fenc_pos > (Dwload_Ysafe - 1))
                    {
                        res1 = MovSafePoint();
                        if (res1 != EM_RES.OK)
                            res = res1;
                    }
                    if (res != EM_RES.OK && res != EM_RES.END)
                    {
                        break;
                    }
                    //通知上料
                    bool Isupload = false;
                    if(VAR.gsys_set.isChkMode)
                    {
                        foreach (WS.MdDat md in List_md)
                        {
                            if (VAR.ChkPC != md.PC_ID) continue;
                            if (md.benable)
                                Isupload = true;
                                
                        }
                    }

                    if ((!VAR.ClearMt) && (!VAR.gsys_set.isChkMode || (VAR.gsys_set.isChkMode && Isupload)))
                    {
                        ws.FeedStatus = WS.EM_STA.REDAYFORUPLOAD;
                    }
                    else
                    {
                        //上完闭合
                        //Task tsk = new Task(() =>
                        //  {
                        //      ws.SetupForTest(ref VAR.gsys_set.bquit);
                        //      //测试
                           ws.FeedStatus = WS.EM_STA.REDAYFORTEST;
                        //  });
                        //tsk.Start();

                    }
                    DownloadProcess = EM_STA.PLACE;
                    
                   
                 

                  
                }

                //摆放到料盘
                if (DownloadProcess == EM_STA.PLACE)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "下料线程摆放...");
                    tick = Environment.TickCount;
                    status = EM_STA.PLACE;
                    res = Place_Md(ref VAR.gsys_set.bquit);
                    //提起Z轴
                    bool bq = false;
                    ax_z.MoveTo(ref bq, 0, 5000);
                    if(res!=EM_RES.OK)
                     break;
                    Ct = (Environment.TickCount - tick) / 1000.0;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("下料线程摆放完成,T={0:F2}s", Ct));
                    DownloadProcess = EM_STA.PICK;
                   
                }

                
                
            }

            //下料机构复位
            //todo

            if (res != EM_RES.OK && res!=EM_RES.END)
            {
                status = EM_STA.ERR;
            }
            else
            {
                status = EM_STA.READY;
            }
            brun = false;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "下料线程结束!");
        }
        #endregion
    }
}