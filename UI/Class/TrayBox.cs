using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MotionCtrl;
using System.Drawing;
using DevReport;
using Win32Lib;
using UI.Class;

namespace UI
{
    public class TrayBox
    {
        //料盘
        public List<Product.Tray> list_tray = new List<Product.Tray>();
        public List<EM_STA> list_sta = new List<EM_STA>();
        public Product.Tray tray_cur = null;
        public string strCfgPath
        {
            get
            {
                return string.Format("{0}\\product\\{1}\\TrayBoxCfg\\{2}.inf", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, name);
            }
        }

        //数据变更，用于更新绘图
        public bool bchanged = true;

        //格数
        int m_tray_cnt;
        public int tray_cnt
        {
            get { return m_tray_cnt; }
            set
            {
                if (m_tray_cnt != value)
                {
                    if (list_tray == null) list_tray = new List<Product.Tray>();
                    Product.Tray tray = new Product.Tray(strCfgPath);
                    list_tray.Clear();
                    for (int n = 0; n < m_tray_cnt; n++)
                    {
                        list_tray.Add(tray.Clone());
                        list_sta.Add(EM_STA.EMPTY);
                    }
                }
                m_tray_cnt = value;
            }
        }
        //当前格
        int m_tray_idx;
        public int tray_idx
        {
            get
            {
                if (m_tray_idx < 0 || m_tray_idx >= list_sta.Count) m_tray_idx = 0;
                return m_tray_idx;
            }
            set
            {
                if (value < 0 || value >= list_sta.Count) m_tray_idx = 0;
                else m_tray_idx = value;
            }
        }

        public string name;
        public string disc;
        public bool IsReady = true;
        //TRAY参数
        /// <summary>
        /// 行数
        /// </summary>
        public int tray_row = 6;
        /// <summary>
        /// 列数
        /// </summary>
        public int tray_col = 10;
        /// <summary>
        /// 左上角
        /// </summary>
        public ST_XYZA tray_tl;
        /// <summary>
        /// 左下角
        /// </summary>
        public ST_XYZA tray_bl;
        /// <summary>
        /// 右上角
        /// </summary>
        public ST_XYZA tray_tr;

        /// <summary>
        /// NG默认O度标准位
        /// </summary>
        public bool IsNg=true;

        //料仓参数
        //第一格位置
        public ST_XZ st_first_tray_pos;
        //每格高度
        public double tray_heigh;
        //取放料脱离高度
        public double tray_feed_ofs_h;
        //取料X位置
        public double fd_pos_x;
        //放料松开气缸X位置
        public double fd_openCy_pos_x;
        //料盘二维码拍照位
        public double tray_barcode_x1;
        public double tray_barcode_y1;
        public double tray_barcode_x2;
        public double tray_barcode_y2;
        public double tray_barcode_a1;
        public double tray_barcode_a2;
        public bool ischangetray = false;
        //安区X位置
        public double fd_safe_x;
        //检测高度差
        public double fd_chk_high_z;
        public EM_DIR direction;
        //0 吸头1Y 1吸头3Y
        public double[] motor_barcode_pos = new double[4];
        //硬件
        public AXIS ax_z;
        public AXIS ax_x;
        //料夹感应
        public GPIO in_box_sen;
        //料盘感应
        public GPIO in_tray_sen;
        //料盘顶紧气缸
        public GPIO out_tray_hd;
        //料盘顶紧气缸感应
        public GPIO in_tray_hd;
        //夹紧气缸
        public Cylinder cy_hd;
        //状态
        public EM_STA status;
        //是否运动
        public bool runin = false;
        //位置更新
        public bool runUpdate = false;
        //空运行
        public bool[] MoveToLastPos = new bool[2];
        //更换物料标志
        public bool ChgML = false;

        public enum EM_STA
        {
            [Description("未知")]
            UNKNOWN,
            [Description("就绪")]
            STANBY,
            [Description("空仓")]
            EMPTY,
            [Description("满仓")]
            FULL,
            [Description("待测")]
            UNTEST,
            [Description("完成")]
            DONE,
            [Description("复位中")]
            HOME,
            [Description("无料仓")]
            NOBOX,
            [Description("错误")]
            ERR
        }
        public enum EM_DIR
        {
            [Description("只进")]
            ONLY_IN,
            [Description("只出")]
            ONLY_OUT,
            [Description("进/出")]
            IN_OUT,
        }

        double x1 = 0, z1 = 0;
        bool btsk = false;
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
                    x1 = ax_x != null ? ax_x.fenc_pos : 0;
                    z1 = ax_z != null ? ax_z.fenc_pos : 0;
                    btsk = false;
                });
                tsk.Start();
                return string.Format("X: {0:000.000}\nZ: {1:000.000}", x1, z1);
            }
        }

        //初始化
        public TrayBox(string name = "未定义", string disc = "料仓", EM_DIR dir = EM_DIR.IN_OUT, int cnt = 10, AXIS ax_x = null, AXIS ax_z = null, GPIO in_box_sen = null, GPIO in_tray_sen = null, GPIO out_tray_hd = null, GPIO in_tray_hd = null, Cylinder cy_hd = null)
        {
            this.direction = dir;
            this.name = name;
            this.disc = disc;
            this.m_tray_cnt = cnt;
            this.ax_x = ax_x;
            this.ax_z = ax_z;
            this.in_box_sen = in_box_sen;
            this.in_tray_sen = in_tray_sen;
            this.out_tray_hd = out_tray_hd;
            this.in_tray_hd = in_tray_hd;
            this.MoveToLastPos[0] = true;
            this.MoveToLastPos[1] = true;
            this.ChgML = false;
            this.cy_hd = cy_hd;
            LoadCfg();

            tray_idx = 0;
            list_sta.Clear();
            list_tray.Clear();
            Random rdm = new Random();
            for (int n = 0; n < cnt; n++)
            {
                Thread.Sleep(1);
                list_sta.Add((EM_STA)rdm.Next(0, 5));

                Product.Tray tray = new Product.Tray(tray_row, tray_col, Product.EM_CM_RES.RANDOM);
                list_tray.Add(tray);
            }
        }
        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public EM_RES LoadCfg(string filename = "")
        {

            if (filename.Length < 3)
                filename = strCfgPath;

            if (!File.Exists(filename)) return EM_RES.PARA_ERR;
            IniFile inf = new IniFile(filename);

            string str_section = "TRAY_BOX";
            tray_cnt = inf.ReadInteger(str_section, "TRAY_CNT", tray_cnt);
            // st_first_tray_pos.x = inf.ReadDouble(str_section, "FIRST_TRAY_POS_X", st_first_tray_pos.x);          
            st_first_tray_pos.z = inf.ReadDouble(str_section, "FIRST_TRAY_POS_Z", 0);
            tray_feed_ofs_h = inf.ReadDouble(str_section, "TRAY_FEED_OFS_H", 0);

            tray_heigh = inf.ReadDouble(str_section, "TRAY_HEIGH", 0);
            fd_pos_x = inf.ReadDouble(str_section, "TRAY_FEED_X", 0);
            fd_openCy_pos_x = inf.ReadDouble(str_section, "OPENCY_X", fd_pos_x - 20);
            fd_safe_x = inf.ReadDouble(str_section, "TRAY_FEED_SAFE_X", 0);
            fd_chk_high_z = inf.ReadDouble(str_section, "FD_CHK_HIGH_Z", 0);
            tray_barcode_x1 = inf.ReadDouble(str_section, "TRAY_BARCODE_X1", 0);
            tray_barcode_y1 = inf.ReadDouble(str_section, "TRAY_BARCODE_Y1", 0);
            tray_barcode_x2 = inf.ReadDouble(str_section, "TRAY_BARCODE_X2", 0);
            tray_barcode_y2 = inf.ReadDouble(str_section, "TRAY_BARCODE_Y2", 0);
            tray_barcode_a1 = inf.ReadDouble(str_section, "TRAY_BARCODE_A1", 0);
            tray_barcode_a2 = inf.ReadDouble(str_section, "TRAY_BARCODE_A2", 0);
            motor_barcode_pos[0] = inf.ReadDouble(str_section, "MOTOR_BARCODE_POS1", 0);
            motor_barcode_pos[1] = inf.ReadDouble(str_section, "MOTOR_BARCODE_POS2", 0);
            motor_barcode_pos[2] = inf.ReadDouble(str_section, "MOTOR_BARCODE_POS3", 0);
            motor_barcode_pos[3] = inf.ReadDouble(str_section, "MOTOR_BARCODE_POS4", 0);
            //str_section = "TRAY";
            //tray_row = inf.ReadInteger(str_section, "ROW", tray_row);
            //tray_col = inf.ReadInteger(str_section, "COL", tray_col);
            //tray_tl.x = inf.ReadDouble(str_section, "TL_X", tray_tl.x);
            //tray_tl.y = inf.ReadDouble(str_section, "TL_Y", tray_tl.y);
            //tray_tl.z = inf.ReadDouble(str_section, "TL_Z", tray_tl.z);
            //tray_tr.x = inf.ReadDouble(str_section, "TR_X", tray_tr.x);
            //tray_tr.y = inf.ReadDouble(str_section, "TR_Y", tray_tr.y);
            //tray_tr.z = inf.ReadDouble(str_section, "TR_Z", tray_tr.z);
            //tray_bl.x = inf.ReadDouble(str_section, "BL_X", tray_bl.x);
            //tray_bl.y = inf.ReadDouble(str_section, "BL_Y", tray_bl.y);
            //tray_bl.z = inf.ReadDouble(str_section, "BL_Z", tray_bl.z);

            return EM_RES.OK;
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public EM_RES SaveCfg(string filename = "")
        {
            EM_RES res = EM_RES.OK;
            if (filename.Length < 3)
                filename = strCfgPath;

            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;

            IniFile inf = new IniFile(filename);

            string str_section = "TRAY_BOX";
            inf.WriteInteger(str_section, "TRAY_CNT", tray_cnt, ref ischange, true, filename);
            //inf.WriteDouble(str_section, "FIRST_TRAY_POS_X", st_first_tray_pos.x);
            inf.WriteDouble(str_section, "FIRST_TRAY_POS_Z", st_first_tray_pos.z, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_HEIGH", tray_heigh, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_FEED_OFS_H", tray_feed_ofs_h, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_FEED_X", fd_pos_x, ref ischange, true, filename);
            inf.WriteDouble(str_section, "OPENCY_X", fd_openCy_pos_x, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_FEED_SAFE_X", fd_safe_x, ref ischange, true, filename);
            inf.WriteDouble(str_section, "FD_CHK_HIGH_Z", fd_chk_high_z, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_X1", tray_barcode_x1, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_Y1", tray_barcode_y1, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_X2", tray_barcode_x2, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_Y2", tray_barcode_y2, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_A1", tray_barcode_a1, ref ischange, true, filename);
            inf.WriteDouble(str_section, "TRAY_BARCODE_A2", tray_barcode_a2, ref ischange, true, filename);


            inf.WriteDouble(str_section, "MOTOR_BARCODE_POS1", motor_barcode_pos[0], ref ischange, true, filename);
            inf.WriteDouble(str_section, "MOTOR_BARCODE_POS2", motor_barcode_pos[1], ref ischange, true, filename);
            inf.WriteDouble(str_section, "MOTOR_BARCODE_POS3", motor_barcode_pos[2], ref ischange, true, filename);
            inf.WriteDouble(str_section, "MOTOR_BARCODE_POS4", motor_barcode_pos[3], ref ischange, true, filename);
            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name);
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //第二层
                backup_filename = backup_filename + "\\TrayBoxCfg";
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //文件
                string[] str = filename.Split('\\');
                res = SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]), backup, backup_filename);
                if (res != EM_RES.OK) return res;
            }
            //str_section = "TRAY";
            //inf.WriteInteger(str_section, "ROW", tray_row);
            //inf.WriteInteger(str_section, "COL", tray_col);
            //inf.WriteDouble(str_section, "TL_X", tray_tl.x);
            //inf.WriteDouble(str_section, "TL_Y", tray_tl.y);
            //inf.WriteDouble(str_section, "TL_Z", tray_tl.z);
            //inf.WriteDouble(str_section, "TR_X", tray_tr.x);
            //inf.WriteDouble(str_section, "TR_Y", tray_tr.y);
            //inf.WriteDouble(str_section, "TR_Z", tray_tr.z);
            //inf.WriteDouble(str_section, "BL_X", tray_bl.x);
            //inf.WriteDouble(str_section, "BL_Y", tray_bl.y);
            //inf.WriteDouble(str_section, "BL_Z", tray_bl.z);

            return EM_RES.OK;
        }
        public bool isSafe
        {
            get
            {
                if (ax_x.status == AXIS.AX_STA.ALM || ax_x.status == AXIS.AX_STA.UNKOWN || ax_x.status == AXIS.AX_STA.HOMEING) return false;
                if (ax_x.fcmd_pos < 0) return false;
                return true;
            }
        }

        public EM_RES NewBox(Product.EM_CM_RES res = Product.EM_CM_RES.UNTEST)
        {
            bchanged = true;
            if (list_tray == null || list_tray.Count != tray_cnt) list_tray = new List<Product.Tray>();
            list_tray.Clear();
            list_sta.Clear();
            NGCodeDef NGDef = new NGCodeDef();
            NGDef.LoadCfg(VAR.Isretestzone);
            if (name != "TrayBox_FD") res = Product.EM_CM_RES.EMPTY;
            for (int n = 0; n < tray_cnt; n++)
            {
                Product.Tray tray = new Product.Tray(strCfgPath, res);
                tray.NGDef = NGDef;
                list_tray.Add(tray);
                list_sta.Add(EM_STA.FULL);
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 移动料仓到指定位置编号
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="idx">指定位置编号</param>
        /// <param name="btrayin">True：后续动作为TRAY盘入仓，定位自动降低ofs_z。</param>
        /// <returns></returns>
        public EM_RES BoxMoveToPosIdx(ref bool bquit, bool Demo, int idx = -1, bool btrayin = true)
        {
            //bquit
            if (bquit) return EM_RES.QUIT;

            bchanged = true;

            //current idx
            if (idx == -1) idx = tray_idx;
            //check idx
            if (idx > list_sta.Count - 1 || idx < 0) return EM_RES.PARA_OUTOFRANG;

            //calc posInf
            double pos = st_first_tray_pos.z + idx * tray_heigh - Math.Abs(fd_chk_high_z);// -(btrayin ? tray_feed_ofs_h : 0);

            //move
            if (ax_z != null)
            {
                EM_RES res = ax_z.MoveTo(ref bquit, pos, 10000, false);
                if (res != EM_RES.OK) return res;
            }

            //update idx
            tray_idx = idx;

            //check sensor
            if (!Demo)
            {
                if (btrayin && in_tray_sen.AssertON())
                {
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 入仓感应已有物料,层数:{1}!", disc, idx));
                    return EM_RES.NEXT;
                }
                else if (!btrayin && in_tray_sen.AssertOFF())
                {
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} 出仓感应没有物料,层数:{1}!", disc, idx));
                    return EM_RES.NEXT;
                }
            }


            //Z offset
            pos = st_first_tray_pos.z + idx * tray_heigh - (btrayin ? tray_feed_ofs_h : 0);
            //move
            if (ax_z != null)
            {
                EM_RES res = ax_z.MoveTo(ref bquit, pos, 10000, false);
                if (res != EM_RES.OK) return res;
            }


            return EM_RES.OK;
        }
        public EM_RES SetSta(EM_STA sta)
        {
            for (int n = 0; n < list_sta.Count; n++)
            {
                list_sta[n] = sta;
            }
            bchanged = true;
            return EM_RES.OK;
        }
        //确认当前料轴Z可在仓储里移动的高度
        public EM_RES PosTrayMovIn(ref bool bquit)
        {
            EM_RES res;
            if (Math.Abs(ax_z.fenc_pos - (st_first_tray_pos.z + tray_idx * tray_heigh - tray_feed_ofs_h)) > 1 && ax_x.fenc_pos > (fd_safe_x + 1))
            {
                res = MT.Move(ref bquit, ref ax_x, fd_safe_x);
                if (res != EM_RES.OK) return res;
            }

            if (Math.Abs(ax_z.fenc_pos - (st_first_tray_pos.z + tray_idx * tray_heigh - tray_feed_ofs_h)) > 1 && ax_x.fenc_pos < (fd_safe_x + 1))
            {
                res = MT.Move(ref bquit, ref ax_z, st_first_tray_pos.z + tray_idx * tray_heigh - tray_feed_ofs_h);
                if (res != EM_RES.OK) return res;

            }
            return EM_RES.OK;
        }
        public EM_RES TrayBoxFull(ref bool bquit)
        {

            try
            {
                EM_RES res;
                //先定位到零位
                res = MT.Move(ref bquit, ref ax_z, 0);
                if (res != EM_RES.OK) return res;
                ChgML = true;
                ShowErrMsg Code;
                Code = disc.Contains("供") ? ShowErrMsg.ChangeFedBox : disc.Contains("OK") ? ShowErrMsg.ChangeOKBox : ShowErrMsg.ChangeSenNGBox;
                //界面提示
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "更换仓储" : "Change Tray", 10, true, ErrCode: Code);

                if (NewSysInf.UserParams.bAGVCallBox && (disc.Contains("供") || disc.Contains("OK")))
                {
                    res = TrayBoxFullCallAgv(ref bquit);
                    if (res != EM_RES.OK)
                    {
                        return res;
                    }
                }
                else
                {
                    MT.ST_WARN warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "tiếp tục chạy");
                    warn.ws = null;//增加语言
                    warn.msg = MultiLanguage.TxtSelct(
                        disc + "的物料已完成,请更换仓储后按确定继续!",
                        name + "The materials have been completed, please change the tray box and press 'Keep running' key to continue!",
                        disc + "Nguyên liệu đã hoàn thành, bạn hãy đổi kho và nhấn OK để tiếp tục nhé!");
                    warn.lb_msg = MultiLanguage.TxtSelct(
                        disc + "的物料已完成,请更换仓储后按确定继续!",
                        name + "The materials have been completed, please change the tray box and press 'Keep running' key to continue!",
                        disc + "Nguyên liệu đã hoàn thành, bạn hãy đổi kho và nhấn OK để tiếp tục nhé!");
                    warn.title = MultiLanguage.TxtSelct("提示:更换仓储", "Tip:Change tray box", "Mẹo: Thay đổi bộ nhớ");
                    MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.Null);
                }
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                // MT.Display_frwarn(Color.Yellow, "提示:当前仓储的物料已完成,请更换仓储后按确定继续!");
                tray_idx = 0;
                res = NewBox();
                if (res != EM_RES.OK) return res;
                return EM_RES.OK;
            }
            finally
            {
                ChgML = false;
            }

        }

        object LockAGvCall = new object();
        public EM_RES TrayBoxFullCallAgv(ref bool bquit)
        {

            try
            {
                EM_RES res;

                lock (LockAGvCall)
                {
                    bool bUntestBox = disc.Contains("供") ? true : false;
                    var gpiscall = MT.GPIO_CallAGVBox;
                    var gpioAction = bUntestBox ? MT.GPIO_AGV_ChangeUnTestBox : MT.GPIO_AGV_ChangeOkBox;
                    var gpioDone = bUntestBox ? MT.GPIO_AGV_ChangeUnTestBoxDone : MT.GPIO_AGV_ChangeOkBoxDone;
                    gpiscall.SetOn();
                    DateTime nowtime = DateTime.Now;
                    gpioAction.SetOn();
                    nowtime = DateTime.Now;
                    bool boxSensChange = false;
                    bool boxSenState = in_box_sen.AssertOFF();

                    while (true)
                    {
                        if (bquit)
                            return EM_RES.QUIT;
                        var secmods = (DateTime.Now - nowtime).TotalSeconds;
                        if (gpioDone.AssertON())
                        {
                            break;
                        }
                        if (boxSenState != in_box_sen.AssertOFF())
                        {
                            boxSensChange = true;
                        }

                        if (secmods % 5 == 0)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "等待AGV上料中，时间" + secmods);
                        }
                        if (secmods > NewSysInf.UserParams.WaitAGVToSeconds)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "等待AGV上料动作超时（秒）" + secmods);
                            return EM_RES.TIMEOUT;
                        }
                    }

                    if (!boxSensChange)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "AGV上料动作后没有料仓信号变化，上料失败！");
                        return EM_RES.ERR;
                    }

                    if (in_box_sen.AssertOFF())
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, disc + "AGV上料动作后没有感应到料仓，上料失败！");
                        return EM_RES.ERR;
                    }

                    gpiscall.SetOff();
                    gpioAction.SetOff();

                }
                return EM_RES.OK;
            }
            finally
            {
                ChgML = false;
            }

        }

        private void preCallAgv()
        {
            if (NewSysInf.UserParams.bAGVCallBox && (disc.Contains("供") || disc.Contains("OK")))
            {
                var precnt = NewSysInf.UserParams.AgvPreCallBox;
                if (precnt >= 1 && precnt <= tray_cnt - 2)
                {
                    if (tray_idx >= tray_cnt - precnt)
                    {
                        MT.GPIO_CallAGVBox.SetOn();
                    }
                }
            }
        }
        //仓储
        public EM_RES Tray_Action(bool Demo, EM_DIR _dir = EM_DIR.IN_OUT)
        {
            EM_RES res = EM_RES.OK;
            try
            {
                IsReady = false;
                //确认ax_x停止
                int n = 0;
                while (runin || !ax_x.isStop)
                {
                    if (n++ > 500) break;
                    if (WS.bquit || VAR.gsys_set.bquit) return EM_RES.QUIT;
                    Thread.Sleep(10);
                }
                MoveToLastPos[0] = true;
                MoveToLastPos[1] = true;
                if (n > 500)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位超时5S", ax_x.disc) : string.Format("{0} Move timeout 5S     ({0}定位超时5S)", ax_x.disc), DReport.EmErrCode.MoveError, (int)DReport.EmHareware.UpDownLoad, ERR_ALM.EmErrItem.TimeOut);
                    MT.BeQuitEn(true, false);
                    return EM_RES.ERR;
                }

                if (_dir != EM_DIR.ONLY_IN)
                {
                    if (tray_cur == null)
                    {
                        res = Tray_InOut(ref UpDownLoad.bquit, EM_DIR.ONLY_OUT, Demo);
                    }
                    else
                    {
                        res = Tray_InOut(ref UpDownLoad.bquit, EM_DIR.IN_OUT, Demo);
                    }
                }
                else
                {
                    res = Tray_InOut(ref UpDownLoad.bquit, EM_DIR.ONLY_IN, Demo);
                }

                if (res == EM_RES.OK)
                {
                    //Task Zhome = new Task(() =>
                    //{
                    //    MT.Move(ref VAR.gsys_set.bquit, ref this.ax_z, 0);
                    //});
                    //Zhome.Start();
                    if (_dir != EM_DIR.ONLY_IN) ischangetray = true;
                }
                else
                {


                    if (res == EM_RES.MOVE_PROTECT || res == EM_RES.MOVE_ERR || res == EM_RES.MOVE_TIMEOUT || res == EM_RES.MOVE_PARA_ERR)
                    {
                        MT.BeQuitEn(true, true);
                    }
                    else if (res == EM_RES.SAFE_PROTECT)
                    {
                        MT.BeQuitEn(true);
                    }


                }
                return res;
            }
            finally
            {
                //if(res== EM_RES.OK)
                IsReady = true;
            }
        }

        //仓储动作
        public EM_RES Tray_InOut(ref bool bquit, EM_DIR dir, bool Demo)
        {
            EM_RES res;
            if (dir == EM_DIR.ONLY_IN)
            {
                res = In(ref bquit, Demo);
                if (res != EM_RES.OK) return res;
            }
            else if (dir == EM_DIR.ONLY_OUT)
            {
                preCallAgv();
                if (tray_idx == tray_cnt - 1)
                {
                    res = TrayBoxFull(ref bquit);
                    if (res != EM_RES.OK) return res;
                    //return EM_RES.END;
                }
                res = Out(ref bquit, Demo);
                if (res != EM_RES.OK) return res;
            }
            else if (dir == EM_DIR.IN_OUT)
            {
                res = In(ref bquit, Demo);
                if (res != EM_RES.OK) return res;
                preCallAgv();
                if (tray_idx++ == tray_cnt - 1)
                {

                    res = TrayBoxFull(ref bquit);
                    if (res != EM_RES.OK) return res;
                    //return EM_RES.END;
                }
             

                res = Out(ref bquit, Demo);
                if (res != EM_RES.OK) return res;
            }
            return EM_RES.OK;
        }


        public EM_RES TrayOut(ref bool bquit, bool Demo, int idx = -2, EM_STA sta = EM_STA.UNTEST)
        {
            EM_RES res = EM_RES.OK;
            try
            {
                //check box
                if (in_box_sen.isOFF && !Demo)
                {
                    Thread.Sleep(30);
                    if (in_box_sen.isOFF)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}出料时料仓未感应!", disc) : string.Format("{0} The magazine is not sensing when tray out!      ({1}出料时料仓未感应!)", name, disc));
                        if (VAR.gsys_set.status == EM_SYS_STA.RUN)
                        {
                            //界面提示         增加语言        
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "没有仓储" : "No tray", 10, true);
                            MT.ST_WARN warn = new MT.ST_WARN();
                            warning fr_warn = new warning();
                            warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "tiếp tục chạy");
                            warn.ws = null;
                            warn.title = MultiLanguage.TxtSelct("提示:没有仓储", "Tip: no tray box", "Gợi ý: không nhập kho");
                            warn.msg = MultiLanguage.TxtSelct
                                (disc + "没有检测到,请确认放好后按确定继续!",
                                name + "No detection, please confirm it and press OK to continue!",
                                disc + "Nếu nó không được phát hiện, vui lòng xác nhận và nhấn OK để tiếp tục!");
                            warn.lb_msg = MultiLanguage.TxtSelct
                                (disc + "没有检测到,请确认放好后按确定继续!",
                                name + "No detection, please confirm it and press OK to continue!",
                                disc + "Nếu nó không được phát hiện, vui lòng xác nhận và nhấn OK để tiếp tục!");
                            MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.TrayBoxAbnormal);
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        }
                        status = EM_STA.NOBOX;
                        return EM_RES.ERR;
                    }
                }




                bchanged = true;

                //check cur tray
                if (tray_cur != null)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 出料时轨道已有料盘!", disc) : string.Format("{0}There is already a tray in the track when tray out!      ({1} 出料时轨道已有料盘!)", name, disc));
                    return EM_RES.ERR;
                }

                //if (!PT_SET.IsMesLocal&&disc== COM.traybox_fd.disc)
                //{

                //    Msg.secsManager.Send(new BaseInfo() { Id = 5 }, 2);
                //    MT.IsAllowStartUpdateByTray = false;
                //    MT.IsAllowStartByTray= false;

                //    Msg.secsManager.Send(new BaseInfo() { Id = 1, Value = Convert.ToInt32(DReport.EmStatus.Run).ToString() });
                //    Msg.secsManager.Send(new BaseInfo() { Id = 3 }, 2);
                //    Task mm =new Task(() =>
                //    {
                //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, "正在等待MES上位指令!");
                //        SpinWait.SpinUntil(() => MT.IsAllowStartUpdateByTray, 10000);
                //    });

                //    mm.Start();
                //    mm.Wait();
                //    //fr?.Close();
                //    if (!MT.IsAllowStartByTray)
                //    {
                //       FrRun. Dialog(Color.Yellow, "警告", "被MES禁止继续加工！请联系相关人员。");
                //       return EM_RES.ERR;
                //    }
                //}


                //check idx
                if (idx < 0 || idx >= list_sta.Count) idx = tray_idx;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0} 出料,层数:{1}!", disc, idx) : string.Format("{0} tray out,layer:{2}!   ({1} 出料,层数:{2}!)", name, disc, idx));
                //search
                if (list_sta[idx] == EM_STA.EMPTY)
                {
                    for (int n = 0; n < list_sta.Count; n++)
                    {
                        //search down
                        if ((idx + n < list_sta.Count) && list_sta[idx + n] != EM_STA.EMPTY)
                        {
                            idx = idx + n;
                            break;
                        }

                        //search up
                        if ((idx - n >= 0) && list_sta[idx - n] != EM_STA.EMPTY)
                        {
                            idx = idx - n;
                            break;
                        }
                    }
                }

                //check status
                if (list_sta[idx] == EM_STA.EMPTY)
                {
                    status = EM_STA.EMPTY;
                    return EM_RES.ERR;
                }
                out_tray_hd.SetOff();
                if (ax_x.fenc_pos > fd_safe_x)
                {
                    res = MT.Move(ref bquit, ref ax_x, fd_safe_x);
                    if (res != EM_RES.OK) return res;
                }
                //move to posInf 
                res = BoxMoveToPosIdx(ref bquit, Demo, idx, false);
                if (res == EM_RES.NEXT)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 出料未感应物料,层数:{1}!", disc, idx + 1) : string.Format("{0}The magazine is not sensing when tray out! ,layer:{2}!     ({1} 出料未感应物料,层数:{2}!)", name, disc, idx + 1));
                    list_sta[idx] = EM_STA.EMPTY;
                    return EM_RES.NEXT;
                }
                if (res != EM_RES.OK) return res;

                //X in
                res = MT.Move(ref bquit, ref ax_x, fd_pos_x);
                if (res != EM_RES.OK) return res;

                //Z down
                //ax_x.MoveTo(ref bquit, ax_x.fcmd_pos - tray_feed_ofs_h);
                res = MT.Move(ref bquit, ref ax_z, ax_z.fcmd_pos - tray_feed_ofs_h);
                if (res != EM_RES.OK) return res;


                //Thread.Sleep(300);
                //X out
                //ax_x.MoveTo(ref bquit, fd_safe_x);
                res = MT.Move(ref bquit, ref ax_x, fd_pos_x - 30);
                if (res != EM_RES.OK) return res;

                if (!Demo) cy_hd.SetOn(ref bquit, 2000);
                else cy_hd.SetOn();

                res = MT.Move(ref bquit, ref ax_x, fd_safe_x);
                if (res != EM_RES.OK) return res;

                //out_tray_hd.SetOn();

                // Thread.Sleep(2000);

                //update status
                list_sta[idx] = EM_STA.EMPTY;
                tray_cur = list_tray[idx];

                //check status
                for (int n = 0; n < list_sta.Count; n++)
                {
                    if (list_sta[n] != EM_STA.EMPTY)
                    {
                        status = EM_STA.STANBY;
                        return EM_RES.OK;
                    }
                }

                status = EM_STA.EMPTY;
                return EM_RES.OK;
            }
            finally
            {
                bool bq = false;
                res = MT.Move(ref bq, ref ax_x, fd_safe_x);
                
            }
           
        }
        public EM_RES TrayIn(ref bool bquit, bool Demo, int idx = -2, EM_STA sta = EM_STA.UNTEST)
        {
            ////check box
            if (in_box_sen.isOFF && !Demo)
            {
                Thread.Sleep(30);
                if (in_box_sen.isOFF)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0}出料时料仓未感应!", disc) : string.Format("{0}The magazine is not sensing when tray out!       ({1}出料时料仓未感应!)", name, disc));
                    if (VAR.gsys_set.status == EM_SYS_STA.RUN)
                    {
                        //界面提示      增加语言           
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "没有仓储" : "No tray", 10, true);
                        MT.ST_WARN warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "tiếp tục chạy");
                        warn.ws = null;
                        warn.title = MultiLanguage.TxtSelct("提示:没有仓储", "Tip:No tray box", "Gợi ý: không nhập kho");
                        warn.msg = MultiLanguage.TxtSelct(
                            disc + "没有检测到,请确认放好后按确定继续!",
                            name + "No detection, please confirm it and press OK to continue!",
                            disc + "Nếu nó không được phát hiện, vui lòng xác nhận và nhấn OK để tiếp tục!");
                        warn.lb_msg = MultiLanguage.TxtSelct(
                            disc + "没有检测到,请确认放好后按确定继续!",
                            name + "No detection, please confirm it and press OK to continue!",
                            disc + "Nếu nó không được phát hiện, vui lòng xác nhận và nhấn OK để tiếp tục!");
                        MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.TrayBoxAbnormal);
                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                    }
                    status = EM_STA.NOBOX;
                    return EM_RES.ERR;
                }
            }
            EM_RES res;
            bchanged = true;


            if (tray_cur == null)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 进料时轨道无料盘!", disc) : string.Format("{0} There is no tray when tray in      ({1} 进料时轨道无料盘!)", name, disc));
                return EM_RES.ERR;
            }

            //check idx
            if (idx < 0 || idx >= list_sta.Count) idx = tray_idx;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0} 进料,层数:{1}!", disc, idx + 1) : string.Format("{0} tray in,layer:{2}!        ({1} 进料,层数:{2}!)", name, disc, idx + 1));
            //search
            if (list_sta[idx] != EM_STA.EMPTY)
            {
                for (int n = 0; n < list_sta.Count; n++)
                {
                    //search down
                    if ((idx + n < list_sta.Count) && list_sta[idx + n] == EM_STA.EMPTY)
                    {
                        idx = idx + n;
                        break;
                    }

                    //search up
                    if ((idx - n >= 0) && list_sta[idx - n] == EM_STA.EMPTY)
                    {
                        idx = idx - n;
                        break;
                    }
                }
            }

            //check status
            if (list_sta[idx] != EM_STA.EMPTY)
            {
                status = EM_STA.FULL;
                return EM_RES.ERR;
            }
            if (ax_x.fenc_pos > fd_safe_x)
            {
                res = MT.Move(ref bquit, ref ax_x, fd_safe_x);
                if (res != EM_RES.OK) return res;
            }
            //move to posInf 
            res = BoxMoveToPosIdx(ref bquit, Demo, idx);
            if (res == EM_RES.NEXT)
            {
                ShowErrMsg Code;
                Code = disc.Contains("供") ? ShowErrMsg.FdBoxHaveTray : disc.Contains("OK") ? ShowErrMsg.OkBoxHaveTray : ShowErrMsg.NgBoxHaveTray;
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "料仓错误" : "TrayBox ERR", 10, true, ErrCode: Code);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "tiếp tục chạy");
                warn.ws = null;
                warn.title = MultiLanguage.TxtSelct("提示:料仓错误", "Tip:Tray box error", "Mẹo: Lỗi Silo");//增加语言
                warn.msg = MultiLanguage.TxtSelct(
                    disc + "进料感应已有物料,层数:" + (idx + 1).ToString(),
                    name + "Induction of existing materials, the number of layers:" + (idx + 1).ToString(),
                    disc + "Nguồn cấp dữ liệu cảm biến vật liệu hiện có, lớp:" + (idx + 1).ToString());
                warn.lb_msg = MultiLanguage.TxtSelct(
                    "提示:" + warn.msg + "请把当前层的料盘拿掉,按确认键继续运行!",
                    "Tip: " + warn.msg + "\r\nPlease remove the tray of the current layer and press the confirmation key to continue running!",
                    "dấu hiệu:" + warn.msg + "Vui lòng tháo khay vật liệu của lớp hiện tại và nhấn nút OK để tiếp tục chạy!");
                MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.TrayBoxAbnormal);
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                return EM_RES.NEXT;
            }
            if (res != EM_RES.OK) return res;

            //X in
            // res=MT.Move(ref bquit, ref ax_x, fd_pos_x);
            //  if (res != EM_RES.OK) return res;
            if (fd_openCy_pos_x < fd_pos_x - 20 || fd_openCy_pos_x > fd_pos_x)
                fd_openCy_pos_x = fd_pos_x - 20;
            res = MT.Move(ref bquit, ref ax_x, fd_openCy_pos_x);
            if (res != EM_RES.OK) return res;
            out_tray_hd.SetOff();
            cy_hd.SetOff(ref bquit, 2000);
            //  Thread.Sleep(300);
            res = MT.Move(ref bquit, ref ax_x, fd_pos_x);
            if (res != EM_RES.OK) return res;
            //Z up
            res = MT.Move(ref bquit, ref ax_z, ax_z.fcmd_pos + tray_feed_ofs_h);
            if (res != EM_RES.OK) return res;

            //X out
            res = MT.Move(ref bquit, ref ax_x, fd_safe_x);
            if (res != EM_RES.OK) return res;

            //Z down
            //res = MT.Move(ref bquit, ref ax_z, ax_z.fcmd_pos - tray_feed_ofs_h);
            //if (res != EM_RES.OK) return res;
            //update status
            list_sta[idx] = sta;
            list_tray[idx] = tray_cur;
            tray_cur = null;

            //check status
            for (int n = 0; n < list_sta.Count; n++)
            {
                if (list_sta[n] == EM_STA.EMPTY)
                {
                    status = EM_STA.STANBY;
                    return EM_RES.OK;
                }
            }

            status = EM_STA.FULL;
            return EM_RES.OK;
        }
        /// <summary>
        /// 抬升到指定位置编号
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="idx">指定位置编号，-1为当前位置的上一位置</param>
        /// <returns></returns>
        public EM_RES Up(ref bool bquit, bool Demo)
        {
            return BoxMoveToPosIdx(ref bquit, Demo, tray_idx + 1, false);
        }
        public EM_RES Down(ref bool bquit, bool Demo)
        {
            return BoxMoveToPosIdx(ref bquit, Demo, tray_idx - 1, false);
        }
        public EM_RES Out(ref bool bquit, bool Demo)
        {
            return TrayOut(ref bquit, Demo);
        }
        public EM_RES In(ref bool bquit, bool Demo)
        {
            return TrayIn(ref bquit, Demo);
        }

        //出料
        //复位
        public EM_RES Home(ref bool bquit)
        {
            //check
            if (ax_x == null || ax_z == null) return EM_RES.PARA_ERR;

            //检查安全性
            //上下料Z轴是否抬起
            //送料X轴确保不在料仓中
            //if(!isSafe)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR,string.Format("{0}回零时，{1}有撞击风险!确保其处于安全区域再回零!",));
            //}

            //开始回零
            status = EM_STA.HOME;

            bchanged = true;
            EM_RES res = EM_RES.OK;

            try
            {
                //送料X轴先回零
                res = MT.AxisHome(ref VAR.gsys_set.bquit, ax_x);
                if (res != EM_RES.OK) return res;

                //送料Z轴回零
                res = MT.AxisHome(ref VAR.gsys_set.bquit, ax_z);
                if (res != EM_RES.OK) return res;

            }
            finally
            {
                if (res == EM_RES.OK) status = EM_STA.STANBY;
                else status = EM_STA.ERR;
            }

            return EM_RES.OK;
        }
    }
}