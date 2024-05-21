using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevReport;
using MotionCtrl;
using UI.Class;

namespace UI
{
    public class LightBox
    {
        /// <summary>
        /// ID
        /// </summary>
        public string name;
        /// <summary>
        /// 描述
        /// </summary>
        public string disc;
        /// <summary>
        /// 英文描述
        /// </summary>
        public string english_disc;
        #region 位置
        /// <summary>
        /// X1与转台的安全距离,太靠近转台时先后退再Z上下切换，然后再往前靠
        /// </summary>
        public double pos_safe_xz;
        /// <summary>
        /// X1-X2的安全距离
        /// </summary>
        public double pos_safe_xx;

        public double pos_safe_xz2;

        public double laser_x1=260;

        public double laser_y1=43;

        public double laser_z2=-200;

        public double md_h=10;

        public double fsm_h=24;

        public double scale=1;
        /// <summary>
        /// 位置补偿
        /// </summary>
        public class PosCmpDef
        {
            public double X1;
            public double X2;
            public double Y1;
            public double Z1;
            public double Z2;
        }
        /// <summary>
        /// 位置定义
        /// </summary>
        /// 
        public class PosDef
        {
            public int ID;
            public string Name;
            public double ActualDistanceParam;
            public double DistanceThreshold;
            public double Comp;
            public double TeleComp;
            public double ActualLuxParam;
            public double LuxThreshold;
            public double ActualCctParam;
            public double CctThreshold;
            public double X1;
            public double X2;
            public double Y1;
            public double Z1;
            public double Z2;
            public int Delay;
            public int Channel;
            public bool IsUse=true;
            /// <summary>
            /// 对应工站补偿
            /// </summary>
            public PosCmpDef[] Cmp = new PosCmpDef[4];
            /// <summary>
            /// 提取工站补偿
            /// </summary>
            /// <param name="cmp_idx">工站号</param>
            /// <returns></returns>
            public PosCmpDef GetCmp(int cmp_idx)
            {
                if (cmp_idx >= 0 && cmp_idx < Cmp.Length && Cmp[cmp_idx] != null)
                {
                    PosCmpDef cmp = new PosCmpDef();
                    cmp.X1 = Math.Abs(Cmp[cmp_idx].X1) < 1000 ? Cmp[cmp_idx].X1 : 0;
                    cmp.X2 = Math.Abs(Cmp[cmp_idx].X2) < 1000 ? Cmp[cmp_idx].X2 : 0;
                    cmp.Y1 = Math.Abs(Cmp[cmp_idx].Y1) < 1000 ? Cmp[cmp_idx].Y1 : 0;
                    cmp.Z1 = Math.Abs(Cmp[cmp_idx].Z1) < 1000 ? Cmp[cmp_idx].Z1 : 0;
                    cmp.Z2 = Math.Abs(Cmp[cmp_idx].Z2) < 1000 ? Cmp[cmp_idx].Z2 : 0;
                    if (cmp.X1 != 0 || cmp.Y1 != 0 || cmp.X2 != 0 || cmp.Z1 != 0 || cmp.Z2 != 0)
                    {
                        return cmp;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 定义位置
        /// </summary>
        public List<PosDef> ListPos = new List<PosDef>();
        /// <summary>
        /// 根据当前位置获取当前位置定义，找不到对应位置则返回NULL
        /// </summary>
        public PosDef CurPosDef
        {
            get
            {
                if (ax_z_otp != null)
                {
                    return ListPos.Find(s => (Math.Abs(ax_z_otp.fenc_pos - s.Z1) < 0.05));
                }

                PosDef pos = new PosDef();
                pos.X1 = ax_x1 != null ? ax_x1.fenc_pos : 0;
                pos.X2 = ax_x2 != null ? ax_x2.fenc_pos : 0;
                pos.Y1 = ax_y1 != null ? ax_y1.fenc_pos : 0;
                pos.Z1 = ax_z1 != null ? ax_z1.fenc_pos : 0;
                pos.Z2 = ax_z2 != null ? ax_z2.fenc_pos : 0;

                return ListPos.Find(s => (Math.Abs(s.X1 - pos.X1) < 0.05 && Math.Abs(s.X2 - pos.X2) < 0.05 && Math.Abs(s.Y1 - pos.Y1) < 0.05 && Math.Abs(s.Z1 - pos.Z1) < 0.05 && Math.Abs(s.Z2 - pos.Z2) < 0.05));
            }
        }

        /// <summary>
        /// 获取与当前位置一样的个数
        /// </summary>
        
        public int GetCurPosCnt
        {
          
            get 
            {
                int cnt=0;
                PosDef pos = new PosDef();
                pos.X1 = ax_x1 != null ? ax_x1.fenc_pos : 0;
                pos.X2 = ax_x2 != null ? ax_x2.fenc_pos : 0;
                pos.Y1 = ax_y1 != null ? ax_y1.fenc_pos : 0;
                pos.Z1 = ax_z1 != null ? ax_z1.fenc_pos : 0;
                pos.Z2 = ax_z2 != null ? ax_z2.fenc_pos : 0;
                foreach(PosDef pd in ListPos)
                {
                   if (ax_z_otp != null)
                    {
                       if(Math.Abs(ax_z_otp.fenc_pos - pd.Z1) < 0.05 && pd.Z1<1000)
                        cnt++;
                    }
                   else
                   {
                       if ((Math.Abs(pd.X1 - pos.X1) < 0.05 && Math.Abs(pd.X2 - pos.X2) < 0.05 && Math.Abs(pd.Y1 - pos.Y1) < 0.05  && Math.Abs(pd.Z1 - pos.Z1) < 0.05 && Math.Abs(pd.Z2 - pos.Z2) < 0.05) && (pd.X1 < 1000 || pd.X2 < 1000 || pd.Y1 < 1000 || pos.Z1 < 1000 || pd.Z2 < 1000))
                        cnt++;
                   }
                     
                }
               return cnt;
            }
        }
        #endregion
        #region 硬件
        /// <summary>
        /// X1小光源
        /// </summary>
        public AXIS ax_x1 = null;
        /// <summary>
        /// X2大光源
        /// </summary>
        public AXIS ax_x2 = null;
        /// <summary>
        /// 增距镜移动轴
        /// </summary>
        public AXIS ax_y1 = null;
        /// <summary>
        /// Z轴(前):近焦光源
        /// </summary>
        public AXIS ax_z1 = null;
        /// <summary>
        /// Z轴(后):左：增距镜(下)/暗态(上)，右：增距镜(下)/污坏点(上)
        /// </summary>
        public AXIS ax_z2 = null;
        /// <summary>
        /// Z轴:OTP
        /// </summary>
        public AXIS ax_z_otp = null;

        EM_RES safe_chk(int id, double target_pos = double.MaxValue)
        {
            EM_RES ret=EM_RES.OK;

            //右光箱
            if (name== "RightLightBox")
            {
                ret = MT.ChkAllSafeSen(0x04);
                if (ret != EM_RES.OK) return ret;
            }
            //左光箱
            if (name == "LeftLightBox")
            {
                ret = MT.ChkAllSafeSen(0x08);
                if (ret != EM_RES.OK) return ret;
            }

            if (Math.Abs(target_pos) > 10000) return EM_RES.OK;

            //check X1 target posInf
            if (ax_x1 != null && id == ax_x1.id)
            {
                if (ax_x2 != null && target_pos < ((ax_x2.fenc_pos + pos_safe_xx) - 0.5))
                {
                    Thread.Sleep(50);
                    if (ax_x2 != null && target_pos < ((ax_x2.fenc_pos + pos_safe_xx) - 0.5))
                    {
                       // double a = (ax_x2.fenc_pos + pos_safe_xx - 0.5);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese?string.Format("{0}目标位置与{1}干涉，禁止移动!", ax_x1.disc, ax_x2.disc): string.Format("{0}'s target position interferes with {1},forbidden to move!         ({0}目标位置与{1}干涉，禁止移动!)",  ax_x1.disc, ax_x2.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }
            }
            //check X2 target posInf
            if (ax_x2 != null && id == ax_x2.id)
            {
                if (ax_x1 != null && target_pos > ((ax_x1.fenc_pos - pos_safe_xx) + 0.5))
                {
                   Thread.Sleep(50);
                    if (ax_x1 != null && target_pos > ((ax_x1.fenc_pos - pos_safe_xx) + 0.5))
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese?string.Format("{0}，{1}，禁止移动!", target_pos, (ax_x1.fenc_pos - pos_safe_xx) + 0.5): string.Format("{0}，{1}，forbidden to move!     ({0}，{1}，禁止移动!)", target_pos, (ax_x1.fenc_pos - pos_safe_xx) + 0.5), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese ? string.Format("{0}目标位置与{1}干涉，禁止移动!", ax_x1.disc, ax_x2.disc) : string.Format("{0}'s target position interferes with {1},forbidden to move!         ({0}目标位置与{1}干涉，禁止移动!)",  ax_x2.disc, ax_x1.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }
            }
            //check Y1 target posInf
            if (ax_y1 != null && id == ax_y1.id)
            {
                if (Math.Abs(target_pos - ax_y1.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                {
                    Thread.Sleep(50);
                    if (Math.Abs(target_pos - ax_y1.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                    {                      
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}在当前位置({1}={2})与测试平台干涉，禁止移动!", ax_y1.disc, ax_x1.disc, ax_x1.fenc_pos): string.Format("{0} at current position({1}={2}),interfering with the test platform, forbidden to move!     ({0}在当前位置({1}={2})与测试平台干涉，禁止移动!)",  ax_y1.disc, ax_x1.disc, ax_x1.fenc_pos), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                        
                }
            }
            //check Z1 target posInf
            if (ax_z1 != null && id == ax_z1.id)
            {
                if (Math.Abs(target_pos - ax_z1.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                {
                    Thread.Sleep(50);
                    if (Math.Abs(target_pos - ax_z1.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese ? string.Format("{0}在当前位置({1}={2})与测试平台干涉，禁止移动!", ax_z1.disc, ax_x1.disc, ax_x1.fenc_pos): string.Format("{0} at current position({1}={2}),interfering with the test platform, forbidden to move!      ({0}在当前位置({1}={2})与测试平台干涉，禁止移动!)",  ax_z1.disc, ax_x1.disc, ax_x1.fenc_pos), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }
            }
         
            //check Z2 target posInf
            if (ax_z2 != null && id == ax_z2.id)
            {
                if (Math.Abs(target_pos - ax_z2.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                {
                    Thread.Sleep(50);
                    if (Math.Abs(target_pos - ax_z2.fenc_pos) > 0.05 && (ax_x1 != null && ax_x1.fenc_pos > (pos_safe_xz + 2)))
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese?string.Format("{0}在当前位置({1}={2})与测试平台干涉，禁止移动!", ax_z2.disc, ax_x1.disc, ax_x1.fenc_pos): string.Format("{0} at current position({1}={2}),interfering with the test platform, forbidden to move!      ({0}在当前位置({1}={2})与测试平台干涉，禁止移动!)",  ax_z2.disc, ax_x1.disc, ax_x1.fenc_pos), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }
            }
            return EM_RES.OK;
        }
        EM_RES opt_safe_chk(int id, double target_pos = double.MaxValue)
        {
            EM_RES ret = MT.ChkAllSafeSen(0x02);
            if (ret != EM_RES.OK) return ret;
            WS ws = COM.list_ws.Find(delegate (WS x) { return x.pos_idx == (int)Turntable.EM_STA.POS2; });
            if (ws == null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese?string.Format("转盘状态异常，OTP轴禁止动作!", disc): string.Format("The turntable is in an abnormal state, and the OTP axis is prohibited from moving!       (转盘状态异常，OTP轴禁止动作!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }
            try
            {
                if (!ws.isUInTestPos || ws.isFrUp || ws.isBkUp)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese?string.Format("{0}不在测试位置，OTP轴禁止动作!", ws.disc): string.Format("{0} is not in the test position, the OTP axis is prohibited from moving!   ({0}不在测试位置，OTP轴禁止动作!)", ws.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, ex.Message);
            }
            return EM_RES.OK;
        }

        #endregion
        #region 状态
        public enum EM_STA
        {
            [Description("未知")]
            UNKNOW,
            [Description("忙")]
            BUSY,
            [Description("回零中")]
            HOME,
            [Description("就绪")]
            READY,
            [Description("远焦")]
            FAF,
            [Description("近焦")]
            NAF,
            [Description("污坏点")]
            DUST,
            [Description("暗态")]
            DARK,
            [Description("OTP")]
            OTP,
            [Description("错误")]
            ERR
        }
        public EM_STA status = EM_STA.UNKNOW;
        #endregion
        #region 初始化
        /// <summary>
        /// 光箱初始化
        /// </summary>
        /// <param name="ax_y_naf">Y轴:近焦/增距镜/污坏点/暗态/</param>
        /// <param name="ax_y_faf">Y轴:远焦</param>
        /// <param name="ax_z_dust">Z轴(后):左：增距镜(下)/暗态(上)，右：增距镜(下)/污坏点(上)</param>
        /// <param name="ax_z_naf">Z轴(前):近焦光源</param>
        /// <param name="ax_otp">Z轴:OTP</param>
        public LightBox(string name, string disc,string english_disc, AXIS x1 = null, AXIS x2 = null,AXIS y1=null, AXIS z1 = null, AXIS z2 = null, AXIS ax_z_otp = null)
        {
            this.name = name;
            this.disc = disc;
            this.english_disc = english_disc;
            this.ax_x1 = x1;
            this.ax_x2 = x2;
            this.ax_y1 = y1;
            this.ax_z1 = z1;
            this.ax_z2 = z2;
            this.ax_z_otp = ax_z_otp;
            status = EM_STA.UNKNOW;
            LoadCfg();

            if (x1 != null && x2 != null && z1 != null && z2 != null)
            {
                if (pos_safe_xx == 0)
                {
                    pos_safe_xx = ax_x1.slp - ax_x2.slp;
                }

                if (pos_safe_xz == 0)
                {
                    pos_safe_xz = ax_x1.slp - 100;
                }
                if (pos_safe_xz2 == 0)
                {
                    pos_safe_xz2 = ax_x1.slp - 90;
                }
            }

            if (ax_x1 != null) ax_x1.ChkSafePos = safe_chk;
            if (ax_x2 != null) ax_x2.ChkSafePos = safe_chk;
            if (ax_y1 != null) ax_y1.ChkSafePos = safe_chk;
            if (ax_z1 != null) ax_z1.ChkSafePos = safe_chk;
            if (ax_z2 != null) ax_z2.ChkSafePos = safe_chk;
            if (ax_z_otp != null) ax_z_otp.ChkSafePos = opt_safe_chk;
        }
        #endregion

        double x1 = 0, x2 = 0, y1 = 0, z1 = 0, z2 = 0, zotp = 0;
        bool btsk = false;
        /// <summary>
        /// 每日点检次数
        /// </summary>
        public int AutoCheckCount;
        /// <summary>
        /// 当前位置字符串
        /// </summary>
        public string StrOfPos
        {
            get
            {
                Task tsk = new Task(() =>
                {
                    if (btsk) return;
                    btsk = true;
                    x1 = ax_x1 != null ? ax_x1.fenc_pos : 0;
                    x2 = ax_x2 != null ? ax_x2.fenc_pos : 0;
                    y1 = ax_y1 != null ? ax_y1.fenc_pos : 0;
                    z1 = ax_z1 != null ? ax_z1.fenc_pos : 0;
                    z2 = ax_z2 != null ? ax_z2.fenc_pos : 0;
                    zotp = ax_z_otp != null ? ax_z_otp.fenc_pos : 0;
                    btsk = false;
                });
                tsk.Start();
                if (ax_z_otp != null) return string.Format("\nZ1(OTP): {0:000.000}", zotp);
                else
                {
                    if(ax_y1!=null)  return string.Format("X1: {0:000.000}  X2: {1:000.000}\nZ1: {2:000.000}  Z2: {3:000.000}\nY1: {4:000.000}", x1, x2, z1, z2,y1);
                    else return string.Format("X1: {0:000.000}  X2: {1:000.000}\nZ1: {2:000.000}  Z2: {3:000.000}", x1, x2, z1, z2);

                }
            }
        }
        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="filename">配置文件路径，为空时默认为..\\syscfg\\syscfg.ini</param>
        /// <returns></returns>
        public EM_RES SaveCfg(string filename = "")
        {
            EM_RES res = EM_RES.OK;
            if (filename.Length < 3)
                filename = string.Format("{0}\\product\\{1}\\LightBoxCfg\\{2}.ini", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, name);
            if (!Directory.Exists(Path.GetDirectoryName(filename))) return EM_RES.PARA_ERR;
            string[] backup = File.ReadAllLines(filename);
            IniFile inf = new IniFile(filename);
            string str_section = "";
            bool ischange = false;
            //delete
            if (this == COM.LeftLightBox)
            {
                str_section = "LASER";
                inf.WriteDouble(str_section, "Laser_x1", laser_x1, ref ischange, true, filename);
                inf.WriteDouble(str_section, "Laser_y1", laser_y1, ref ischange, true, filename);
                inf.WriteDouble(str_section, "Laser_z2", laser_z2, ref ischange, true, filename);
                inf.WriteDouble(str_section, "Scale", scale, ref ischange, true, filename);
                inf.WriteDouble(str_section, "Md_h", md_h, ref ischange, true, filename);
                inf.WriteDouble(str_section, "Fsm_h", fsm_h, ref ischange, true, filename);
            }

        
            for (int n = 0; n < 500; n++)
            {
                str_section = string.Format("POS{0}", n);
                PosDef pos = ListPos.Find(s => s.ID == n);
                if (null != pos)
                {
                    inf.WriteString(str_section, "Name", pos.Name,ref ischange, true, filename);
                    inf.WriteInteger(str_section, "ID", pos.ID, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "ActualDistanceParam", pos.ActualDistanceParam, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "DistanceThreshold", pos.DistanceThreshold, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Comp", pos.Comp, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "TeleComp", pos.TeleComp, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "ActualLuxParam", pos.ActualLuxParam, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "DistanceThreshold", pos.DistanceThreshold, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "ActualCctParam", pos.ActualCctParam, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "CctThreshold", pos.CctThreshold, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "LuxThreshold", pos.LuxThreshold, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "X1", pos.X1, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "X2", pos.X2, ref ischange, true, filename);
                    inf.WriteBool(str_section, "Isuse", pos.IsUse, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Y1", pos.Y1, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Z1", pos.Z1, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Z2", pos.Z2, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Channel", pos.Channel, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Delay", pos.Delay, ref ischange, true, filename);
                    //补偿
                    for (int m = 0; m < 4; m++)
                    {
                        inf.WriteDouble(str_section, string.Format("CMP{0}_X1", m), pos.Cmp[m] != null ? pos.Cmp[m].X1 : 10000, ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CMP{0}_X2", m), pos.Cmp[m] != null ? pos.Cmp[m].X2 : 10000, ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CMP{0}_Y1", m), pos.Cmp[m] != null ? pos.Cmp[m].Y1 : 10000, ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CMP{0}_Z1", m), pos.Cmp[m] != null ? pos.Cmp[m].Z1 : 10000, ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CMP{0}_Z2", m), pos.Cmp[m] != null ? pos.Cmp[m].Z2 : 10000, ref ischange, true, filename);
                    }
                }
                else
                {
                    inf.WriteString(str_section, null, null, ref ischange, false);
                }
            }

            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name);
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //第二层
                backup_filename = backup_filename + "\\LightBoxCfg";
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //文件
               // File.WriteAllLines(backup_filename);
                 SYS_PUD.FileWriteLine(string.Format("{0}.ini", name), backup, backup_filename);
                //if (res != EM_RES.OK) return res;
            }
            // inf = new IniFile(Path.GetFullPath("..") + "\\syscfg\\syscfg.ini");
            //inf.WriteDouble(name, "SAFE_ZX", pos_safe_xz);
            //inf.WriteDouble(name, "SAFE_XX", pos_safe_xx);
            //inf.WriteDouble(name, "SAFE_ZX2", pos_safe_xz2);

            return EM_RES.OK;
        }
        /// <summary>
        /// 参数加载
        /// </summary>
        /// <param name="filename">配置文件路径，为空时默认为..\\syscfg\\syscfg.ini</param>
        /// <returns></returns>
        public EM_RES LoadCfg(string filename = "")
        {
            //
            IniFile inf = new IniFile(Path.GetFullPath("..") + "\\syscfg\\syscfg.ini");
            pos_safe_xz = inf.ReadDouble(name, "SAFE_ZX", 0);
            pos_safe_xx = inf.ReadDouble(name, "SAFE_XX", 0);
            pos_safe_xz2 = inf.ReadDouble(name, "SAFE_ZX2", 0);
            string cur_product_name = VAR.gsys_set.cur_product_name == null ? "" : VAR.gsys_set.cur_product_name;
            if (cur_product_name.Length < 1)
                cur_product_name = inf.ReadString("PRODUCT_SET", "CUR_PRODCUT_NAME", "");

            if (filename.Length < 3)
                filename = string.Format("{0}\\product\\{1}\\LightBoxCfg\\{2}.ini", Path.GetFullPath(".."), cur_product_name, name);
            if (!File.Exists(filename)) return EM_RES.PARA_ERR;

            inf = new IniFile(filename);

            ListPos.Clear();
            string str_section = "";
            if (this == COM.LeftLightBox)
            {
                str_section = "LASER";
                laser_x1 = inf.ReadDouble(str_section, "Laser_x1", 240);
                laser_y1 = inf.ReadDouble(str_section, "Laser_y1", 43);
                laser_z2 = inf.ReadDouble(str_section, "Laser_z2", -200);
                scale = inf.ReadDouble(str_section, "Scale", 1);
                md_h = inf.ReadDouble(str_section, "Md_h", 10);
                fsm_h = inf.ReadDouble(str_section, "Fsm_h", 24);
            }

        
            for (int n = 0; n < 500; n++)
            {
                PosDef pos = new PosDef();
                str_section = string.Format("POS{0}", n);
                //ID
                pos.ID = inf.ReadInteger(str_section, "ID", -1);
                if (pos.ID < 0) continue;
                pos.ID = n;
                //Name
                pos.Name = inf.ReadString(str_section, "Name", "");
                if (pos.Name.Length < 1) continue;
                pos.IsUse = inf.ReadBool(str_section, "Isuse",true);
                pos.ActualDistanceParam = inf.ReadDouble(str_section, "ActualDistanceParam", 0);
                pos.DistanceThreshold = inf.ReadDouble(str_section, "DistanceThreshold", 0);
                pos.Comp = inf.ReadDouble(str_section, "Comp", 0);
                pos.TeleComp = inf.ReadDouble(str_section, "TeleComp", 0);
                pos.ActualLuxParam = inf.ReadDouble(str_section, "ActualLuxParam", 0);
                pos.LuxThreshold = inf.ReadDouble(str_section, "LuxThreshold", 0);
                pos.ActualCctParam = inf.ReadDouble(str_section, "ActualCctParam", 0);
                pos.CctThreshold = inf.ReadDouble(str_section, "CctThreshold", 0);
                pos.X1 = inf.ReadDouble(str_section, "X1", double.MaxValue);
                pos.X2 = inf.ReadDouble(str_section, "X2", double.MaxValue);
                pos.Y1 = inf.ReadDouble(str_section, "Y1", double.MaxValue);
                pos.Z1 = inf.ReadDouble(str_section, "Z1", double.MaxValue);
                pos.Z2 = inf.ReadDouble(str_section, "Z2", double.MaxValue);
                pos.Channel = inf.ReadInteger(str_section, "Channel", int.MaxValue);
                pos.Delay = inf.ReadInteger(str_section, "Delay", 0);

                //补偿
                for (int m = 0; m < 4; m++)
                {
                    pos.Cmp[m] = new PosCmpDef();
                    pos.Cmp[m].X1 = inf.ReadDouble(str_section, string.Format("CMP{0}_X1", m), 10000);
                    pos.Cmp[m].X2 = inf.ReadDouble(str_section, string.Format("CMP{0}_X2", m), 10000);
                    pos.Cmp[m].Y1 = inf.ReadDouble(str_section, string.Format("CMP{0}_Y1", m), 10000);
                    pos.Cmp[m].Z1 = inf.ReadDouble(str_section, string.Format("CMP{0}_Z1", m), 10000);
                    pos.Cmp[m].Z2 = inf.ReadDouble(str_section, string.Format("CMP{0}_Z2", m), 10000);
                }

                ListPos.Add(pos);
            }

            return EM_RES.OK;
        }
        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES Home(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;

            if (status == EM_STA.HOME) return res;
            try
            {
                //otp
                if (ax_z_otp != null)
                {
                    res = MT.AxisHome(ref bquit, ax_z_otp);
                    return res;
                }
                //stop first
                if (status == EM_STA.BUSY)
                {
                    Stop();
                    Thread.Sleep(500);
                }

                status = EM_STA.HOME;
                //先退出远焦
                res = MT.AxisHome(ref bquit, ax_x2);
                if (res != EM_RES.OK) return res;

                //再往外复位近焦
                res = MT.AxisHome(ref bquit, ax_x1);
                if (res != EM_RES.OK) return res;

                //然后Z与Y1复位
                res = MT.AxisHome(ref bquit, ax_z1, ax_z2,ax_y1);
                if (res != EM_RES.OK) return res;

                return EM_RES.OK;
            }
            finally
            {
                if (res == EM_RES.OK) status = EM_STA.READY;
                else status = EM_STA.UNKNOW;
            }
        }
        /// <summary>
        /// 停止轴运动，停止Home动作
        /// </summary>
        public void Stop()
        {
            if (ax_x1 != null)
            {
                ax_x1.bhomequit = true;
                ax_x1.Stop();
            }

            if (ax_x2 != null)
            {
                ax_x2.bhomequit = true;
                ax_x2.Stop();
            }

            if (ax_z1 != null)
            {
                ax_z1.bhomequit = true;
                ax_z1.Stop();
            }

            if (ax_z2 != null)
            {
                ax_z2.bhomequit = true;
                ax_z2.Stop();
            }

            if (ax_y1 != null)
            {
                ax_y1.bhomequit = true;
                ax_y1.Stop();
            }

            if (ax_z_otp != null)
            {
                ax_z_otp.bhomequit = true;
                ax_z_otp.Stop();
            }
        }
        /// <summary>
        /// 定位到指定位置
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="idx">位置编号</param>
        /// <param name="cmp_idx">补偿的工站号</param>
        /// <returns></returns>
        public EM_RES MoveTo(ref bool bquit, int idx, int cmp_idx = -1)
        {
            //if (idx < 0 || idx >= ListPos.Count) return EM_RES.END;
            int t = Environment.TickCount;
            PosDef pos = ListPos.Find(delegate (PosDef x) { return x.ID == idx; });
            if (pos == null) return EM_RES.END;
 

            if (!pos.IsUse)
            {
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();//增加语言
                warn.ok_txt = MultiLanguage.TxtSelct("继续", "GoOn", "Đi tiếp");
                warn.cancle_txt = MultiLanguage.TxtSelct("停止", "Stop", "Ngừng lại");
                warn.title = MultiLanguage.TxtSelct("提示:测试项匹配异常", "Tip: The test item does not match properly", "Mẫu thử khớp với dị thường");
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "测试项匹配异常" : " match err", 10, true);
                warn.msg = MultiLanguage.TxtSelct("测试项匹配异常!", " match err", "Mẫu thử khớp với dị thường");
                warn.lb_msg = MultiLanguage.TxtSelct($"测试项匹配异常，检测到测试软件发来的测试序号{idx}，该测试项未启用，请检查");
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                if (logres == DialogResult.OK)
                {
                    return EM_RES.ERR;
                }
                else
                {
                    return EM_RES.ERR;
                }

            }
            if ((this == COM.LeftLightBox && pos.X1 > MT.AXIS_BOX_L_X1.fenc_pos && pos.Z1 > 10000)||(this==COM.RightLightBox&&pos.X1> MT.AXIS_BOX_R_X1.fenc_pos&&pos.Z1>10000)) pos.Z1 = 0;
            EM_RES res = MoveTo(ref bquit, pos, cmp_idx);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0}定位到 [{1}]{2} 用时:{3:F1}s", disc, pos.ID, pos.Name, (Environment.TickCount-t)/1000.0): string.Format("{0} Move to [{2}]{3} for {4:F1}s       ({1}定位到 [{2}]{3} 用时:{4:F1}s)", english_disc, disc, pos.ID, pos.Name, (Environment.TickCount - t) / 1000.0));
            if (res != EM_RES.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}定位到 [{1}]{2} 出错,{3}", disc, pos.ID, pos.Name, Utility.GetDescription(res,VAR.IsChinese)): string.Format("{0} move to [{2}]{3} err,{4}        ({1}定位到 [{2}]{3} 出错,{4})", english_disc, disc, pos.ID, pos.Name, Utility.GetDescription(res, VAR.IsChinese)), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveError);
            }


            return res;
        }
        /// <summary>
        /// 定位到指定位置
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="pos">位置信息</param>
        /// <param name="cmp_idx">补偿的工站号</param>
        /// <returns></returns>
        public EM_RES MoveTo(ref bool bquit, PosDef pos, int cmp_idx = -1)
        {
            //切换光源
            if (pos.Channel >= 0 && pos.Channel < 10)
            {
                if (pos.Channel > 3)
                {
                    bool ret;
                    //如果非G4C，设置COM6对应通道（456），设置COM7对应通道（789）
                    //如果设置选择了G4C光源，串口使用COM2名字，通道设置大于3
                    if (!PT_SET.bG4C && pos.Channel <= 6)//轩十佳光源OTP
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "进入轩仕佳");
                        MT.COM6.XSJSatrt();
                        Thread.Sleep(200);
                        Application.DoEvents();
                        ret = MT.COM6.SetChannelXSJ(pos.Channel);

                    }
                    else if (!PT_SET.bG4C && pos.Channel <= 9)//轩十佳光源OTP
                    {
                        var idx = pos.Channel - 3;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "进入轩仕佳");
                        MT.COM7.XSJSatrt();
                        Thread.Sleep(200);
                        Application.DoEvents();
                        ret = MT.COM7.SetChannelXSJ(idx);

                    }
                    else
                        ret = MT.COM2.SetChannel(pos.Channel);

                    if (ret != true)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}切换光源[id={1},ch={2}]时异常!", disc, pos.ID, pos.Channel) : string.Format("{0} Switch light source [id={2},ch={3}] ERR!    ({1}切换光源[id={2},ch={3}]时异常!)", english_disc, disc, pos.ID, pos.Channel));
                        return EM_RES.ERR;
                    }
                }
                else
                {
                    int iresult;
                    byte[] cmdbuffAck = new byte[2048];
                    IntPtr handle = (IntPtr)0;
                    if (ax_x1 != null && ax_x1.card != null) handle = ax_x1.card.handle;
                    else if (ax_x2 != null && ax_x2.card != null) handle = ax_x2.card.handle;
                    else if (ax_z1 != null && ax_z1.card != null) handle = ax_z1.card.handle;
                    else if (ax_y1 != null && ax_y1.card != null) handle = ax_y1.card.handle;
                    else if (ax_z2 != null && ax_z1.card != null) handle = ax_z2.card.handle;
                    else if (ax_z_otp != null && ax_z_otp.card != null) handle = ax_z_otp.card.handle;
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}切换光源[id={1},ch={2}]时，handle=null", disc, pos.ID, pos.Channel) : string.Format("{0} Switch light source [id={2},ch={3}]，handle=null)        ({1}切换光源[id={2},ch={3}]时，handle=null)", english_disc, disc, pos.ID, pos.Channel), DReport.EmErrCode.ChangeFailed, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.ChangeError);
                        return EM_RES.PARA_ERR;
                    }
                    string str = string.Format("chset={0:0}", pos.Channel);
                    iresult = zmcaux.ZAux_Execute(handle, string.Format("chset={0:0}", pos.Channel), cmdbuffAck, 2048);

                    if (zmcaux.ERR_OK != iresult)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}切换光源[id={1},ch={2}]时，ret={3}", disc, pos.ID, pos.Channel, iresult) : string.Format("{0} Switch light source [id={2},ch={3}]，ret={4}  ({1}切换光源[id={2},ch={3}]时，ret={4})", english_disc, disc, pos.ID, pos.Channel, iresult), DReport.EmErrCode.ChangeFailed, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.ChangeError);
                        return EM_RES.ERR;
                    }

                    if (0 == cmdbuffAck.ToString().Length)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}切换光源[id={1},ch={2}]时无响应!", disc, pos.ID, pos.Channel) : string.Format("{0}  Switch light source [id={2},ch={3}] No response!    ({1}切换光源[id={2},ch={3}]时无响应!)", english_disc, disc, pos.ID, pos.Channel), DReport.EmErrCode.ChangeFailed, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.ChangeError);
                        return EM_RES.ERR;
                    }
                }
            }

            EM_RES res = EM_RES.OK;
            if ((this == COM.LeftLightBox && pos.X1 > MT.AXIS_BOX_L_X1.fenc_pos && pos.Z1 > 10000) || (this == COM.RightLightBox && pos.X1 > MT.AXIS_BOX_R_X1.fenc_pos && pos.Z1 > 10000)) pos.Z1 = 0;
            //补偿
            PosCmpDef cmp = pos.GetCmp(cmp_idx);

            if (cmp != null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}/{1}补偿:dx1={2},dx2={3},dz1={4},dz2={5},dy1={6}", disc, pos.ID, cmp.X1, cmp.X2, cmp.Z1, cmp.Z2, cmp.Y1) : string.Format("{0}/{2} cmp:dx1={3},dx2={4},dz1={5},dz2={6},dy1={7}   ({1}/{2}补偿:dx1={3},dx2={4},dz1={5},dz2={6},dy1={7})", english_disc, disc, pos.ID, cmp.X1, cmp.X2, cmp.Z1, cmp.Z2, cmp.Y1));
                res = MoveTo(ref bquit, pos.X1 + cmp.X1, pos.X2 + cmp.X2, pos.Z1 + cmp.Z1, pos.Z2 + cmp.Z2, pos.Y1 + cmp.Y1);
            }
            else res = MoveTo(ref bquit, pos.X1, pos.X2, pos.Z1, pos.Z2, pos.Y1);
            if (res != EM_RES.OK)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位到 [{1}]{2} 出错,{3}", disc, pos.ID, pos.Name, Utility.GetDescription(res, VAR.IsChinese)) : string.Format("{0} move to [{2}]{3} err,{4}     ({1}定位到 [{2}]{3} 出错,{4})", english_disc, disc, pos.ID, pos.Name, Utility.GetDescription(res, VAR.IsChinese)), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveError);
            }

            //delay
            for (int dly = pos.Delay; dly > 0; dly -= 100)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
           
            string str2 = "(位置打印)测试项：" + "," + pos?.Name + "," + ax_x1?.disc + "," + ax_x1?.fenc_pos + "," + ax_x2?.disc + "," + ax_x2?.fenc_pos
                + "," + ax_y1?.disc + "," + ax_y1?.fenc_pos + "," + ax_z1?.disc + "," + ax_z1?.fenc_pos + "," + ax_z2?.disc + "," + ax_z2?.fenc_pos
                + "," + ax_z_otp?. disc + "," + ax_z_otp?.fenc_pos;


            Utility.WriteStrToCSVTest(str2);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, str2);
            return res;
        }

        //检查到位
        public EM_RES ChkAxisPos(ref AXIS ax,double pos)
        {
            if (Math.Abs(pos - ax.fenc_pos) > 0.3)
            {
                Thread.Sleep(50);
                if (Math.Abs(pos - ax.fenc_pos) > 0.3)
                {
                    Thread.Sleep(100);
                    if (Math.Abs(pos - ax.fenc_pos) > 0.3)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                            VAR.IsChinese?string.Format("{0}当前位置与目标位置偏差超0.3mm,目标位置:{1}mm,实际位置:{2}mm", ax.str_disc, pos, ax.fenc_pos): string.Format("{0}The deviation between the current position and the target position is more than 0.3mm, the target position: {2} mm, the actual position: {3} mm    ({1}当前位置与目标位置偏差超0.3mm,目标位置:{2}mm,实际位置:{3}mm)", ax.english_disc, ax.str_disc, pos, ax.fenc_pos), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveError);
                        return EM_RES.ERR;
                    }
                }

            }
            return EM_RES.OK;
        }
        /// <summary>
        /// 定位到指定位置
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="x1">X1位置，小于10000时候有效</param>
        /// <param name="x2">X2位置，小于10000时候有效</param>
        /// <param name="z1">Z1位置，小于10000时候有效</param>
        /// <param name="z2">Z2位置，小于10000时候有效</param>
        /// <returns></returns>
        public EM_RES MoveTo(ref bool bquit, double x1, double x2 = double.MaxValue, double z1 = double.MaxValue, double z2 = double.MaxValue,double y1= double.MaxValue)
        {
            EM_RES res = EM_RES.OK;

            //确保转台在位置
            //确保对应测试工站在测试位置

            //OPT光源
            if (ax_x1 == null && ax_x2 == null || ax_z1 == null && ax_z2 == null)
            {
                if (ax_z_otp == null) return EM_RES.PARA_ERR;
                if (bquit) return EM_RES.QUIT;

                if (Math.Abs(z1) < 10000 && Math.Abs(z1 - ax_z_otp.fenc_pos) > 0.05)
                {
                    res = MT.Move(ref bquit, ref ax_z_otp, z1);
                    if (res != EM_RES.OK) return res;
                }
                
                //OTP是否到位
                Thread.Sleep(30);
                res = ChkAxisPos(ref ax_z_otp, z1);
                return res;
            }

            //Z动作
            if (Math.Abs(z1) < 10000 && Math.Abs(z1 - ax_z1.fenc_pos) > 0.3 || Math.Abs(z2) < 10000 && Math.Abs(z2 - ax_z2.fenc_pos) > 0.3 || Math.Abs(y1) < 10000 && (ax_y1 == null || (ax_y1 != null && Math.Abs(y1 - ax_y1.fenc_pos) > 0.3)))
            {
                //如果X1太靠近先后退
                if (ax_x1.fenc_pos > pos_safe_xz)
                {                    
                    if (ax_x2.fenc_pos > (pos_safe_xz - pos_safe_xx))
                    {
                        //如果X2目标位置安全，直接定位X2
                        double temp_x2 = pos_safe_xz - pos_safe_xx - 50;
                        if (Math.Abs(x2) < 10000 && x2 < temp_x2)
                        {
                            temp_x2 = x2;
                        }
                        res = MT.Move(ref bquit, ref ax_x2, temp_x2);
                        if (res != EM_RES.OK) return res;
                    }

                    //如果X1目标位置安全，直接定位X1
                    double temp_x1 = pos_safe_xz;
                    if (Math.Abs(x1) < 10000 && Math.Abs(x1 - ax_x1.fenc_pos) > 0.05 && ax_x2.fenc_pos < (x1 - pos_safe_xx) && x1 < pos_safe_xz)
                    {
                        temp_x1 = x1;
                    }

                    res = MT.Move(ref bquit, ref ax_x1, temp_x1);
                    if (res != EM_RES.OK) return res;
                }

                //提前动X2
                AXIS axx2 = null;
                double pos_x2 = ax_x2.fenc_pos;
                //X2目标位在X1之后的安全位
                if (Math.Abs(x2) < 10000 && Math.Abs(x2 - ax_x2.fenc_pos) > 0.05 && x2 < ax_x1.fenc_pos - pos_safe_xx)
                {
                    pos_x2 = x2;
                    axx2 = ax_x2;
                }
                //X1动作，提前移动X2
                if (Math.Abs(x1) < 10000 && Math.Abs(x1 - ax_x1.fenc_pos) > 0.05 && ax_x2.fenc_pos > (x1 - pos_safe_xx))
                {
                    //X2需要移动，取移动位置与安全位最小值(远离位置)
                    pos_x2 = Math.Abs(x2) < 10000 && x2 < x1 - pos_safe_xx ? x2 : x1 - pos_safe_xx;
                    axx2 = ax_x2;
                }

                //提前动X1
                AXIS axx1 = null;
                if (axx2 == null && Math.Abs(x1) < 10000 && Math.Abs(x1 - ax_x1.fenc_pos) > 0.05 && ax_x2.fenc_pos < (x1 - pos_safe_xx) && x1 < pos_safe_xz)
                {
                    axx1 = ax_x1;
                }

                //Z
                AXIS axz1 = null;
                AXIS axz2 = null;
                if (Math.Abs(z1) < 10000) axz1 = ax_z1;
                if (Math.Abs(z2) < 10000) axz2 = ax_z2;
                //y1
                AXIS axy1 = null;
                if (ax_y1!=null && Math.Abs(y1) < 10000) axy1 = ax_y1;
                res = MT.Move(ref bquit, ref axz1, z1, ref axz2, z2, ref axx2, pos_x2, ref axx1, x1,ref axy1,y1);
                if (res != EM_RES.OK) return res;
            }

            //两轴同时定位
            if (Math.Abs(x1) < 10000 && Math.Abs(x2) < 10000)
            {
                //检查X1/X2安全间距                
                if (x1 - x2 < pos_safe_xx)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("目标位置X1={0}/X2={1}，X1/X2间距{2}过小(<{3})，可能撞机", x1, x2, x1 - x2, pos_safe_xx): string.Format("Target location X1={0}/X2={1},X1/X2 Pitch {2} is too small (<{3}), and may collide     (目标位置X1={0}/X2={1}，X1/X2间距{2}过小(<{3})，可能撞机)", x1, x2, x1 - x2, pos_safe_xx), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }

                //res = MT.Move(ref bquit, ref ax_x1, x1, ref ax_x2, x2);
                //if (res != EM_RES.OK) return res;

                if (x2 < ax_x2.fenc_pos)
                {
                    res = MT.Move(ref bquit, ref ax_x2, x2);
                    if (res != EM_RES.OK) return res;
                    res = MT.Move(ref bquit, ref ax_x1, x1);
                    if (res != EM_RES.OK) return res;
                }
                else
                {
                    if (x1 > pos_safe_xz2)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("目标位置X1={0}过大(>{1})，可能撞机", x1, pos_safe_xz2): string.Format("Target position X1 = {0} is too large (>{1}), it may collide      (目标位置X1={0}过大(>{1})，可能撞机)", x1, pos_safe_xz2), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                    res = MT.Move(ref bquit, ref ax_x1, x1);
                    if (res != EM_RES.OK) return res;
                    res = MT.Move(ref bquit, ref ax_x2, x2);
                    if (res != EM_RES.OK) return res;
                }

            }
            //只动X1
            else if (Math.Abs(x1) < 10000 && Math.Abs(x1 - ax_x1.fenc_pos) > 0.05)
            {
                if (ax_x2.fenc_pos > (x1 - pos_safe_xx))
                {
                    //res = MT.Move(ref bquit, ref ax_x1, pos_safe_xz, ref ax_x2, x1 - XSafeDis);
                    res = MT.Move(ref bquit, ref ax_x2, x1 - pos_safe_xx-50);
                    if (res != EM_RES.OK) return res;
                }
                if (x1 > pos_safe_xz2)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("目标位置X1={0}过大(>{1})，可能撞机", x1, pos_safe_xz2) : string.Format("Target position X1 = {0} is too large (>{1}), it may collide      (目标位置X1={0}过大(>{1})，可能撞机)", x1, pos_safe_xz2), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
                res = MT.Move(ref bquit, ref ax_x1, x1);
                if (res != EM_RES.OK) return res;
            }
            //只动X2
            else if (Math.Abs(x2) < 10000 && Math.Abs(x2 - ax_x2.fenc_pos) > 0.05)
            {
                if (ax_x1.fenc_pos < x2 + pos_safe_xx)
                {
                    //res = MT.Move(ref bquit, ref ax_x1, pos_safe_xz, ref ax_x2, x1 - XSafeDis);
                    res = MT.Move(ref bquit, ref ax_x1, x2 + pos_safe_xx);
                    if (res != EM_RES.OK) return res;
                }
                res = MT.Move(ref bquit, ref ax_x2, x2);
                if (res != EM_RES.OK) return res;
            }
            //位置检查
            Thread.Sleep(30);
            if (ax_x1 != null && (Math.Abs(x1) < 10000))
            {
                res = ChkAxisPos(ref ax_x1, x1);
                if (res != EM_RES.OK) return res;
            }
            if (ax_x2 != null && (Math.Abs(x2) < 10000))
            {
                res = ChkAxisPos(ref ax_x2, x2);
                if (res != EM_RES.OK) return res;
            }
            if (ax_z1 != null && (Math.Abs(z1) < 10000))
            {
                res = ChkAxisPos(ref ax_z1, z1);
                if (res != EM_RES.OK) return res;
            }
            if (ax_z2 != null && (Math.Abs(z2) < 10000))
            {
                res = ChkAxisPos(ref ax_z2, z2);
                if (res != EM_RES.OK) return res;
            }
            ////ax_y1为步进
            //if (ax_y1 != null && (Math.Abs(y1) < 10000))
            //{
            //    res = ChkAxisPos(ref ax_y1, y1);
            //    if (res != EM_RES.OK) return res;
            //}
            return EM_RES.OK;
        }

        /// <summary>
        /// 检查对转盘是否安全
        /// </summary>
        public bool isInSafePos
        {
            get
            {
                if (ax_z_otp != null)
                {
                    if (!ax_z_otp.isORG)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}不在原点，与转台可能有干涉，请先回零!", COM.LeftLightBox.ax_x1.disc): string.Format("{0} is not at the origin, there may be interference with the turntable, please return to origin first!      ({0}不在原点，与转台可能有干涉，请先回零!)",  COM.LeftLightBox.ax_x1.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return false;
                    }
                }
                else
                {
                    if (ax_x1.home_status != AXIS.HOME_STA.OK || ax_x1.fenc_pos > pos_safe_xz || pos_safe_xz == 0)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}与转台可能有干涉，请先回零/避让!", COM.LeftLightBox.ax_x1.disc): string.Format("There may be interference between {0} and the turntable, please return to origin or dodge first!    ({0}与转台可能有干涉，请先回零/避让!)",  COM.LeftLightBox.ax_x1.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.LightBox, ERR_ALM.EmErrItem.MoveProtect);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}