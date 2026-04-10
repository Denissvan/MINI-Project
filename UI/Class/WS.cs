using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using ControlzEx.Standard;
using DevReport;
using MotionCtrl;
using UI.Class;
using static SerialCommander;
using static UI.LightBox;

namespace UI
{
    public class WS
    {
        #region 参数
        public int num;
        public string disc
        {
            get { return VAR.IsChinese ? string.Format("工站{0}", num + 1) : string.Format("WS{0} ", num + 1); }
        }

        #region PCID_TESTBOXID_SN(不能更改)
        public static int[,] pc_id = new int[4, 16] {{1,1,1,1,1,1,1,1,5,5,5,5,5,5,5,5},
                                                          {2,2,2,2,2,2,2,2,6,6,6,6,6,6,6,6},
                                                          {3,3,3,3,3,3,3,3,7,7,7,7,7,7,7,7},
                                                          {4,4,4,4,4,4,4,4,8,8,8,8,8,8,8,8}};

        public static int[,] testbox_id = new int[4, 16] {{0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,1},
                                                          {0,0,0,0,1,1,1,1,0,0,0,0,1,1,1,1},
                                                          {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1},
                                                          {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1}};

        public static int[,] md_sn = new int[4, 16] {{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15},
                                                          {1,2,3,4,5,6,7,8,1,2,3,4,5,6,7,8},
                                                          {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15},
                                                          {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}};
        #endregion
        //状态检测gy0123
        public enum Md_RES
        {
            [Description("OK")]
            OK = 0,
            [Description("开图NG1")]
            NG_IMAGE = 258,
            [Description("开图NG2")]
            NG_IMAGE2 = 266,
            [Description("OS异常")]
            NG_OS = 270,
            [Description("图像丢桢")]
            NG_MISS_PIX = 274,
            [Description("曝光NG")]
            NG_EXPOSURE = 512,
            [Description("IIC读写NG")]
            NG_IIC = 271,
            [Description("单号异常")]
            NG_ORDER = 3333,
            [Description("内存申请失败")]
            NG_NeiCun = 257,
            [Description("点检时间到")]
            NGLightChk = 279,
            [Description("点检继续")]
            GoOnChk = 289,
            [Description("点检时间到2")]
            NGLightChk2 = 276,

        }
        public bool bjigSan = true;//夹具二维码扫码gy0123
        //当前位置
        public int mpos_idx;
        public int tmr;
        public static bool Demo = false;
        public static bool bquit = false;
        public static bool bpause = false;
        //public int AllCnt = 0;
        //public int NgCnt = 0;
        //public int OkCnt = 0;
        public int ReTest = 0;
        public bool bRetest = false;
        public bool BoxCheckFinsh = false;
        //订单号防呆
        public bool Iserrfirstbox = true;

        public bool waitopen = false;     //等待上下料面夹具完全打开才进行测试
        public int waittime=1000;//夹具打开等待时间
        public Stopwatch Swtime = new Stopwatch();//测试时间的统计
        public Stopwatch Swtimemode = new Stopwatch();//测试分段时间的统计
        public Stopwatch AllSwtime = new Stopwatch();//测试总时间的统计
        public Stopwatch UdSwtime = new Stopwatch();//上下料的统计
        private static volatile GyroCheckState _gyroState = GyroCheckState.Disabled;
        private static bool IsGyroCheckAllowed()
        {
            return _gyroState >= GyroCheckState.Idle
                && _gyroState <= GyroCheckState.Running;
        }


        public int pos_idx
        {
            get
            {
                Turntable.EM_STA sta = Turntable.GetWSSta(num);
                if (sta > Turntable.EM_STA.POS3 || sta < Turntable.EM_STA.POS0) mpos_idx = (int)Turntable.EM_STA.UNKNOW;
                else mpos_idx = (int)sta;
                return mpos_idx;
            }
        }
        public bool breschanged = true;

        public int TestDelay = 30000;
        public enum EM_STA
        {
            [Description("未知")]
            UNKNOWN,
            [Description("就绪")]
            REDAY,
            [Description("待测")]
            UNTEST,
            [Description("测试中")]
            TEST,
            [Description("待转位")]
            NEXT,
            [Description("上料中")]
            UPLOAD,
            [Description("下料中")]
            DOWNLOAD,
            [Description("上下料中")]
            UPDOWNLOAD,
            [Description("准备测试")]
            REDAYFORTEST,
            [Description("准备下料")]
            REDAYFORDOWNLOAD,
            [Description("准备上料")]
            REDAYFORUPLOAD,
            [Description("准备上下料")]
            REDAYFORUPDOWNLOAD,
            [Description("复位中")]
            HOME,
            [Description("联机异常")]
            LINKERR,
            [Description("错误")]
            ERR,
            [Description("退出")]
            QUIT,
        }
        public EM_STA Status;
        //上下料状态
        public EM_STA FeedStatus;
        public bool isEmpty = true;
        public bool isFourRows=false;//是否是四行的夹具
        //位置设定
        public const int cnt = 2;
        float pos_open_fr;
        float pos_close_fr;
        float pos_open_bk;
        float pos_close_bk;
        float pos_test_u;
        float pos_feed_u;
        public ST_YZ pos_Dwload_Pick;
        //立柱拍照位置ey 
        public ST_XY pos_CapLiZhu;
        public double Cap_LiZhu_Limit;
        //首次开始标志位
        public bool IsFirst = true;
        //测试模组行列
        public int cm_row = 2;
        public int cm_col = 8;
        //每工装测试模组
        public int cm_per_box = 2;
        //测试状态，true为已经测试，fa
        public enum EM_TEST_STA { EMPTY, UNTEST, NEXT, COMPLETED, ERROR };
        public EM_TEST_STA TestStatus = EM_TEST_STA.UNTEST;
        //GRR测试流程参数
        public int GrrTestCnt = 0;
        public int GrrUDLcnt = 0;
        //点检测试次数
        public int ChkCnt = 0;
        /// <summary>
        /// 当前工站的NG码
        /// </summary>
        public  List<string> CurWSNGCode = new List<string>();
        public class MdDat
        {
            //编号
            public int Num;
            //放料位置
            public ST_XYZ[] st_pos = new ST_XYZ[cnt];
            //吸头取料位置
            public ST_XYZ[] st_pickpos = new ST_XYZ[cnt];
            //工站编号
            public int WS_ID;
            //对应测试电脑
            public int PC_ID;
            public string PC_IP;
            //当前测试位置
            public int test_idx;
            //工装编号
            public int TestBox_ID;
            //序号
            public int SN;
            //上次结果
            public int last_res;
            //NG相同结果次数
            public int NgSameRes_cnt;
            //结果
            public int res;
            //用时
            public int ct;
            //二维码
            public string bardcode;
            //马达二维码
            public string motor_barcode;
            //使用
            public bool _benable;
            public bool bAutoChkOk;
            //使用
            public bool benable
            {
                get
                {
                    if (VAR.isAutoChkMode && !VAR.ClearMt)
                    {
                        bool bAutoSfrCheck = false;
                        if (ListChkTemp != null && ListChkTemp.Count > 0 && AutoChkCnt < ListChkTemp.Count && AutoChkCnt >= 0)
                        {
                            bAutoSfrCheck = NoChangCheckModList.Contains(ListChkTemp[AutoChkCnt]);  // 
                        }
                        var maxen = PT_SET.AutoChkMaxMdEn;
                        var minen = PT_SET.AutoChkSmallMdEn;
                        if (bAutoSfrCheck)
                        {
                            maxen = PT_SET.issmall ? 0 : 255;
                            minen = 255;
                        }
                        if (PT_SET.AutoChkSelectWs != WS_ID)
                            return false;
                        else if (!PT_SET.issmall && ((PT_SET.bitOpenMode == 1 && Num % 2 == 0) ||
                             (PT_SET.bitOpenMode == 2 && Num % 2 == 1) ||
                             (PT_SET.bitOpenMode == 3 && Num < 9) ||
                             (PT_SET.bitOpenMode == 4 && Num > 8)))
                        {
                            return false;
                        }
                        else if (Num <= 8)
                        {
                            var mm = (1 << (Num - 1));
                            var obj = (int)(minen & mm);
                            return obj != 0;
                        }
                        else
                        {
                            var mm = 1 << (Num - 9);
                            var obj = (int)(maxen & mm);
                            return obj != 0;
                        }
                    }
                    else
                        return _benable;

                }
                set
                {
                    _benable = value;
                }
            }
            //类型
            public bool IsNormal;
            //gy0123夹具生产信息统计
            //夹具拍照位置gy0123
            public ST_XYZ[] st_jigpos = new ST_XYZ[cnt];
            //夹具二维码gy0123
            public string jig_ID;
            //ok数量
            public int cnt_ok;
            //统计近期20个中Ng个数用来报警
            public string cntNgRateFor20;

            //开图NG
            public int cnt_ng_image;
            //os异常
            public int cnt_ng_OS;
            //图像丢帧
            public int cnt_ng_miss_pix;
            //曝光NG
            public int cnt_ng_exposure;
            //IIC读写NG
            public int cnt_ng_iic;
            //其他NG
            public int cnt_ng_other;
            //新增回检拍二维码位
            public ST_XYZ[] st_CapQrcodePos = new ST_XYZ[cnt];


            public MdDat Clone()
            {
                MdDat md = new MdDat();
                md.Num = Num;
                md.WS_ID = WS_ID;
                md.PC_ID = PC_ID;
                md.PC_IP = PC_IP;
                md.test_idx = test_idx;
                md.TestBox_ID = TestBox_ID;
                md.SN = SN;
                md.res = res;
                md.ct = ct;
                md.bardcode = bardcode;
                md.benable = benable;
                md.IsNormal = IsNormal;
                //gy0123夹具生产信息统计
                md.jig_ID = jig_ID;
                md.cnt_ng_exposure = cnt_ng_exposure;
                md.cnt_ng_iic = cnt_ng_iic;
                md.cnt_ng_image = cnt_ng_image;
                md.cnt_ng_miss_pix = cnt_ng_miss_pix;
                md.cnt_ng_OS = cnt_ng_OS;
                md.cnt_ng_other = cnt_ng_other;
                md.cnt_ok = cnt_ok;
                md.motor_barcode = motor_barcode;
                for (int i = 0; i < cnt; i++)
                {
                    md.st_pos[i] = st_pos[i].clone();
                    md.st_jigpos[i] = st_jigpos[i].clone();//gy0123
                    md.st_CapQrcodePos[i] = st_CapQrcodePos[i].clone();
                }

                return md;
            }
        };
        public List<MdDat> list_md = new List<MdDat>();

        public enum EM_PC_STA
        {
            [Description("电脑禁用")]
            DISABLE,
            [Description("联机错误")]
            LINK_ERR,
            [Description("就绪")]
            READY,
            [Description("测试中")]
            TEST,
            [Description("测试结束")]
            ENDOFTEST,
            [Description("等待下站")]
            WAIT,
            [Description("测试不同步")]
            NOT_SAME_TESTIDX,
            [Description("上下料")]
            UP_DOWN_LOAD
        };
        public class PCDat
        {
            //编号
            public int ID;
            //状态
            public EM_PC_STA status;
            //测试位置
            public int test_idx;
            //循环数
            public int temp;
            public int cnt;
        };
        public List<PCDat> list_pc_dat = new List<PCDat>();

        //硬件相关
        //public AXIS ax_fr = null;
        //public AXIS ax_bk = null;
        public AXIS ax_u = null;
        public List<Cylinder> list_cld_fr = null;
        public List<Cylinder> list_cld_bk = null;
        public List<Cylinder> list_cld = null;
        public GPIO gpio_out_gz_power = null;
        public GPIO gpio_out_gz_wind = null;
        public List<GPIO> list_gpio_zk = null;
        //是否在上下料位置
        public bool bOnUpDnPos = false;
        public bool bUpDnPosGoOnTest = false;//在上下料位置需要继续测试
        public bool bUpDnAddTestWaitUnload = false;//401附加测试已完成，等待下料消费
        public bool bResultWaitUnload = false;//本轮结果已完整返回，等待下料消费

        double fr = 0, bk = 0, u = 0;
        bool btsk = false;
        public string StrOfPos
        {
            get
            {
                Task tsk = new Task(() =>
                {
                    if (btsk) return;
                    try
                    {
                        btsk = true;
                        //fr = ax_fr != null ? ax_fr.fcmd_pos : 0;
                        //bk = ax_bk != null ? ax_bk.fcmd_pos : 0;
                        u = ax_u != null ? ax_u.fcmd_pos : 0;
                    }
                    finally
                    {
                        btsk = false;
                    }
                });
                tsk.Start();
                //return string.Format("F: {0:000.000}\nB: {1:000.000}\nU: {2:000.000}", fr, bk, u);
                return string.Format("U: {0:000.000}", u);
            }
        }

        public string StrOfPosA
        {
            get
            {
                // return string.Format("F:{0:000.0} B:{1:000.0} U:{2:000.0}", ax_fr != null ? ax_fr.fcmd_pos : 0, ax_bk != null ? ax_bk.fcmd_pos : 0, ax_u != null ? ax_u.fcmd_pos : 0);
                return string.Format("U:{0:000.0}", ax_u != null ? ax_u.fcmd_pos : 0);
            }
        }

        #endregion

        #region 模组信息
        public List<List<MdDat>> list_list_md = new List<List<MdDat>>();
        /// <summary>
        /// 按PC_ID分类,按序号排序。在加载/保存时候调用。存储于list_list_md
        /// </summary>
        /// <returns></returns>
        public EM_RES SortMdDat()
        {
            List<MdDat> list_md_temp = new List<MdDat>();
            foreach (MdDat md in list_md) list_md_temp.Add(md);
            ////移除
            //list_md_temp.RemoveAll(delegate (MdDat x) { return !x.benable; });

            //按PC_ID排序            
            list_md_temp.Sort(delegate (MdDat x, MdDat y) { return x.PC_ID.CompareTo(y.PC_ID); });
            //提取            
            List<MdDat> list_pc = new List<MdDat>();
            int id_temp = -1;
            list_list_md.Clear();
            foreach (MdDat md in list_md_temp)
            {
                if (md.PC_ID < 1) continue;
                if (id_temp != md.PC_ID)
                {
                    list_pc = new List<MdDat>();
                    list_list_md.Add(list_pc);
                }
                list_pc.Add(md);
                id_temp = md.PC_ID;
            }

            //按序号排序
            foreach (List<MdDat> list in list_list_md)
            {
                list.Sort(delegate (MdDat x, MdDat y) { return x.SN.CompareTo(y.SN); });
            }

            //初始化PC信息
            list_pc_dat.Clear();
            foreach (List<MdDat> list in list_list_md)
            {
                if (list.Count == 0) continue;
                PCDat pc = new PCDat();
                pc.ID = list[0].PC_ID;
                pc.status = EM_PC_STA.READY;
                MdDat md = list.Find(s => { return s.benable == true; });
                if (md == null) pc.status = EM_PC_STA.DISABLE;
                list_pc_dat.Add(pc);
            }

            return EM_RES.OK;
        }

        /// <summary>
        /// 复位所有模组状态到待测(-1)
        /// </summary>
        /// <returns></returns>
        public int ResetResultOfMd()
        {
            string strres = "";
            foreach (MdDat md in list_md)
            {
                if (md.res != -2)//空料不需要重置
                {
                    md.res = -1;
                    strres += "1@";
                }
                else
                {
                    strres += "0@";
                }
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("PC{0} reset:{1}", list_md[0].PC_ID, strres));
            bResultWaitUnload = false;
            breschanged = true;
            return 0;
        }

        /// <summary>
        /// 返回指定状态的模组列表
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public List<MdDat> GetMdByRes(int res = -1)
        {
            return list_md.FindAll(s => { return s.res == res; });
        }

        /// <summary>
        /// 取出指定编号指定状态的模组
        /// </summary>
        /// <param name="idx">取指定编号，且不为空的模组</param>
        /// <param name="res">取不指定编号，第一个满足指定状态的模组</param>
        /// <returns></returns>
        public EM_RES Pull(ref MdDat md, int idx = -1, int res = -2)
        {
            MdDat md_temp = list_md.Find(s =>
            {
                //取指定编号且不为空的模组
                if (idx != -1 && s.Num == idx && res != -2) return true;
                //取不指定编号，第一个满足状态的模组
                if (idx == -1 && s.res == res) return true;
                return false;
            });
            if (md_temp == null) return EM_RES.PARA_OUTOFRANG;
            //复制
            md = md_temp.Clone();
            //清空
            md_temp.res = -2;
            md_temp.bardcode = "";
            return EM_RES.OK;
        }
        /// <summary>
        /// 放置模组到测试工位
        /// </summary>
        /// <param name="md">模组包含二维码bardcode，要放置的位置Num(-1为第一空位置），</param>
        /// <returns></returns>
        public EM_RES Push(MdDat md)
        {
            MdDat md_temp = list_md.Find(s =>
            {
                //为空
                if (s.res == -2)
                {
                    if (md.Num == -1 || md.Num == s.Num)
                        return true;
                    else return false;
                }
                return false;
            });
            if (md_temp == null) return EM_RES.PARA_OUTOFRANG;

            md_temp.res = md.res;
            md_temp.bardcode = md.bardcode;
            md_temp.ct = 0;
            md_temp.test_idx = 0;
            return EM_RES.OK;
        }
        #endregion

        #region 构造/参数存取
        public WS(int num, AXIS ax_u, List<Cylinder> list_cld_fr, List<Cylinder> list_cld_bk, List<Cylinder> list_cld, GPIO gpio_out_gz_power, List<GPIO> list_gpio_zk, GPIO gpio_out_gz_wind, int cm_row = 1, int cm_col = 2, int cm_per_box = 2)
        {
            this.num = num;
            this.cm_col = cm_col;
            this.cm_row = cm_row;
            this.cm_per_box = cm_per_box;
            //this.ax_fr = ax_fr;
            //this.ax_bk = ax_bk;
            this.ax_u = ax_u;
            this.list_cld_fr = list_cld_fr;
            this.list_cld_bk = list_cld_bk;
            this.list_cld = list_cld;
            this.gpio_out_gz_power = gpio_out_gz_power;
            this.gpio_out_gz_wind = gpio_out_gz_wind;
            this.list_gpio_zk = list_gpio_zk;
            //for debug
            Random rdm = new Random();
            list_md.Clear();
            for (int m = 0; m < 16; m++)
            {
                MdDat md = new MdDat();
                md.res = rdm.Next(-1, 2);
                if (md.res > 0) md.res += 10000;
                list_md.Add(md);
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
                filename = string.Format("{0}\\product\\{1}\\WsCfg\\WS{2}Cfg.ini", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, num + 1);

            if (!Directory.Exists(Path.GetDirectoryName(filename))) return EM_RES.PARA_ERR;

            IniFile inf = new IniFile(filename);
            string str_section = "";
            bool ChkParm = false;

            TestDelay = inf.ReadInteger("OTHER", "TEST_DELAY", 90000);
            waittime = inf.ReadInteger("OTHER", "waittime", 2000);
            pos_CapLiZhu.x = inf.ReadDouble("OTHER", "POS_CAP_LIZHU_X", 0);
            pos_CapLiZhu.y = inf.ReadDouble("OTHER", "POS_CAP_LIZHU_Y", 0);
            Cap_LiZhu_Limit = inf.ReadDouble("OTHER", "POS_CAP_LIZHU_LIMIT", 0.1);
            isFourRows = inf.ReadBool("OTHER", "bisfourrows", false);
            bjigSan = inf.ReadBool("OTHER", "bJigSan", true);

            waitopen = inf.ReadBool("OTHER", "waitopen", false);

            //pos_Dwload_Pick.y = inf.ReadDouble("OTHER", "POS_DWLOAD_PICK_Y", 0);
            //pos_Dwload_Pick.z = inf.ReadDouble("OTHER", "POS_DWLOAD_PICK_Z", 0);

            List<MdDat> list_temp = new List<MdDat>();
            // list_md.Clear();
            for (int n = 1; n <= 32; n++)
            {
                MdDat md = new MdDat();
                str_section = string.Format("MD{0}", n);
                md.Num = inf.ReadInteger(str_section, "NUM", -1);
                if (md.Num < 0) break;

                for (int i = 0; i < cnt; i++)
                {
                    md.st_pos[i].x = inf.ReadDouble(str_section, "X" + (i + 1).ToString(), double.MaxValue);
                    md.st_pos[i].y = inf.ReadDouble(str_section, "Y" + (i + 1).ToString(), double.MaxValue);
                    md.st_pos[i].z = inf.ReadDouble(str_section, "Z" + (i + 1).ToString(), double.MaxValue);

                    md.st_pickpos[i].x = inf.ReadDouble(str_section, "PX" + (i + 1).ToString(), double.MaxValue);
                    md.st_pickpos[i].y = inf.ReadDouble(str_section, "PY" + (i + 1).ToString(), double.MaxValue);
                    md.st_pickpos[i].z = inf.ReadDouble(str_section, "PZ" + (i + 1).ToString(), double.MaxValue);
                    //夹具拍照位置 gy0123
                    md.st_jigpos[i].x = inf.ReadDouble(str_section, "JX" + (i + 1).ToString(), double.MaxValue);
                    md.st_jigpos[i].y = inf.ReadDouble(str_section, "JY" + (i + 1).ToString(), double.MaxValue);
                    md.st_jigpos[i].z = inf.ReadDouble(str_section, "JZ" + (i + 1).ToString(), double.MaxValue);

                    md.st_CapQrcodePos[i].x = inf.ReadDouble(str_section, "CapQrcodeX" + (i + 1).ToString(), double.MaxValue);
                    md.st_CapQrcodePos[i].y = inf.ReadDouble(str_section, "CapQrcodeY" + (i + 1).ToString(), double.MaxValue);
                    md.st_CapQrcodePos[i].z = inf.ReadDouble(str_section, "CapQrcodeZ" + (i + 1).ToString(), double.MaxValue);
                }

               // md.cntNgRateFor20 = inf.ReadString(str_section, "cntNgRateFor20", "");
                //夹具二维码gy0123
                md.jig_ID = inf.ReadString(str_section, "JIG_ID", "null");
                
                md.cnt_ok = inf.ReadInteger(str_section, "CNT_OK", 0);
                md.cnt_ng_exposure = inf.ReadInteger(str_section, "CNT_EXSPOSURE", 0);
                md.cnt_ng_iic = inf.ReadInteger(str_section, "CNT_IIC", 0);
                md.cnt_ng_image = inf.ReadInteger(str_section, "CNT_IMAGE", 0);
                md.cnt_ng_miss_pix = inf.ReadInteger(str_section, "CNT_MISS_PIX", 0);
                md.cnt_ng_OS = inf.ReadInteger(str_section, "CNT_OS", 0);
                md.cnt_ng_other = inf.ReadInteger(str_section, "CNT_OTHER", 0);
                // md.st_pos.a = inf.ReadDouble(str_section, "U", double.MaxValue);
                md.PC_ID = pc_id[num, n - 1];
                md.TestBox_ID = testbox_id[num, n - 1];
                md.SN = md_sn[num, n - 1];
               
                if (md.PC_ID != inf.ReadInteger(str_section, "PC_ID", -1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}第{1}模组PC_ID信息有误,软件:{2},读取:{3}", disc, n, md.PC_ID, inf.ReadInteger(str_section, "PC_ID", -1)) : string.Format("{0} mod {1} PC_ID err! PC_ID:{2},but now PC_ID:{3}             ({0}第{1}模组PC_ID信息有误,软件:{2},读取:{3})", disc, n, md.PC_ID, inf.ReadInteger(str_section, "PC_ID", -1)));
                    ChkParm = true;
                }

                if (md.TestBox_ID != inf.ReadInteger(str_section, "TESTBOX_ID", -1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}第{1}模组TESTBOX_ID信息有误,软件:{2},读取:{3}", disc, n, md.TestBox_ID, inf.ReadInteger(str_section, "TESTBOX_ID", -1)) : string.Format("WS{0} Mod {1} TESTBOX_ID err,TestBox_ID:{2},but now TestBox_ID:{3}                ({0}第{1}模组TESTBOX_ID信息有误,软件:{2},读取:{3})", disc, n, md.TestBox_ID, inf.ReadInteger(str_section, "TESTBOX_ID", -1)));
                    ChkParm = true;
                }

                if (md.SN != inf.ReadInteger(str_section, "SN", md.SN))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}第{1}模组SN信息有误,软件:{2},读取:{3}", disc, n, md.SN, inf.ReadInteger(str_section, "SN", md.SN)) : string.Format("WS{0} Mod{1} SN err, SN:{2},but now SN:{3}            ({0}第{1}模组SN信息有误,软件:{2},读取:{3})", disc, n, md.SN, inf.ReadInteger(str_section, "SN", md.SN)));
                    ChkParm = true;
                }

                //md.PC_ID = inf.ReadInteger(str_section, "PC_ID", -1);
                //if (md.PC_ID < 0) break;
                //md.TestBox_ID = inf.ReadInteger(str_section, "TESTBOX_ID", -1);
                //if (md.PC_ID < 0) break;
                //md.SN = inf.ReadInteger(str_section, "SN", md.SN);
                //if (md.SN < 0) break;

                md.PC_IP = inf.ReadString(str_section, "PC_IP", "");

                //bool ischange = false;
                //if (!PT_SET.LbEn && (num == 0 || num == 2))
                //{
                //    inf.WriteBool(str_section, "EN", false,ref ischange,false);
                //    md.benable = false;
                //}
                //else
                //    md.benable = inf.ReadBool(str_section, "EN", true);
                //if (!PT_SET.issmall)
                //{
                //    if (PT_SET.issingle && n % 2 == 0)
                //    {
                //        inf.WriteBool(str_section, "EN", false, ref ischange, false);
                //        md.benable = false;
                //    }
                //    else if (!PT_SET.issingle && n % 2 == 1)
                //    {
                //        inf.WriteBool(str_section, "EN", false, ref ischange, false);
                //        md.benable = false;
                //    }
                //    else
                //    {
                //        md.benable = inf.ReadBool(str_section, "EN", true);
                //    }
                //}
                //else
                md.benable = inf.ReadBool(str_section, "EN", true);
                md.WS_ID = num;
                if (IsFirst) md.res = -1;
                else md.res = list_md[n - 1].res;
                md.bardcode = list_md[n - 1].bardcode;
                list_temp.Add(md);
            }

            if (list_temp.Count > 0)
            {
                list_md.Clear();
                list_md = list_temp;
                SortMdDat();
                if (ChkParm)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("二维码绑定数据有误!") : "The barcode binding data is wrong!         (二维码绑定数据有误!)");
                    MessageBox.Show(VAR.IsChinese ? "警告:二维码绑定数据检验有误,请联系工程人员确认!" : "Warning: The verification of the barcode binding data is incorrect, please contact the engineering staff to confirm!\r\n警告:二维码绑定数据检验有误,请联系工程人员确认!)", VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return EM_RES.ERR;
                }
                return EM_RES.OK;
            }
            else return EM_RES.ERR;
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public EM_RES SaveCfg(bool enable = true, string filename = "")
        {
            EM_RES res = EM_RES.OK;
            if (filename.Length < 3)
                filename = string.Format("{0}\\product\\{1}\\WsCfg\\WS{2}Cfg.ini", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, num + 1);

            if (!Directory.Exists(Path.GetDirectoryName(filename))) return EM_RES.PARA_ERR;

            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;

            IniFile inf = new IniFile(filename);
            string str_section = "";
            inf.WriteDouble("OTHER", "POS_CAP_LIZHU_X", pos_CapLiZhu.x, ref ischange, true, filename);
            inf.WriteDouble("OTHER", "POS_CAP_LIZHU_Y", pos_CapLiZhu.y, ref ischange, true, filename);
            inf.WriteDouble("OTHER", "POS_CAP_LIZHU_LIMIT", Cap_LiZhu_Limit, ref ischange, true, filename);
            inf.WriteInteger("OTHER", "TEST_DELAY", TestDelay, ref ischange, true, filename);
            inf.WriteInteger("OTHER", "waittime", waittime, ref ischange, true, filename);
            inf.WriteBool("OTHER", "bJigSan", bjigSan, ref ischange, true, filename);
            inf.WriteBool("OTHER", "waitopen", waitopen, ref ischange, true, filename);
            inf.WriteBool("OTHER", "bisfourrows", isFourRows, ref ischange, true, filename);

            //delete
            foreach (MdDat md in list_md)
            {
                str_section = string.Format("MD{0}", md.Num);
                inf.WriteInteger(str_section, "NUM", md.Num, ref ischange, true, filename);
                for (int i = 0; i < cnt; i++)
                {
                    inf.WriteDouble(str_section, "X" + (i + 1).ToString(), md.st_pos[i].x, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Y" + (i + 1).ToString(), md.st_pos[i].y, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "Z" + (i + 1).ToString(), md.st_pos[i].z, ref ischange, true, filename);

                    inf.WriteDouble(str_section, "PX" + (i + 1).ToString(), md.st_pickpos[i].x, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "PY" + (i + 1).ToString(), md.st_pickpos[i].y, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "PZ" + (i + 1).ToString(), md.st_pickpos[i].z, ref ischange, true, filename);
                    //夹具拍照位置gy0123
                    inf.WriteDouble(str_section, "JX" + (i + 1).ToString(), md.st_jigpos[i].x, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "JY" + (i + 1).ToString(), md.st_jigpos[i].y, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "JZ" + (i + 1).ToString(), md.st_jigpos[i].z, ref ischange, true, filename);

                    inf.WriteDouble(str_section, "CapQrcodeX" + (i + 1).ToString(), md.st_CapQrcodePos[i].x, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "CapQrcodeY" + (i + 1).ToString(), md.st_CapQrcodePos[i].y, ref ischange, true, filename);
                    inf.WriteDouble(str_section, "CapQrcodeZ" + (i + 1).ToString(), md.st_CapQrcodePos[i].z, ref ischange, true, filename);
                }

               // inf.WriteString(str_section, "cntNgRateFor20", md.cntNgRateFor20, ref ischange, true, filename);
                //夹具二维码gy0123和夹具统计
                inf.WriteString(str_section, "JIG_ID", md.jig_ID, ref ischange, true, filename);     
                inf.WriteInteger(str_section, "CNT_OK", md.cnt_ok, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_EXSPOSURE", md.cnt_ng_exposure, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_IIC", md.cnt_ng_iic, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_IMAGE", md.cnt_ng_image, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_MISS_PIX", md.cnt_ng_miss_pix, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_OS", md.cnt_ng_OS, ref ischange, true, filename);
                inf.WriteInteger(str_section, "CNT_OTHER", md.cnt_ng_other, ref ischange, true, filename);


                inf.WriteInteger(str_section, "PC_ID", md.PC_ID, ref ischange, true, filename);
                inf.WriteString(str_section, "PC_IP", md.PC_IP, ref ischange, true, filename);
                inf.WriteInteger(str_section, "TESTBOX_ID", md.TestBox_ID, ref ischange, true, filename);
                inf.WriteInteger(str_section, "SN", md.SN, ref ischange, true, filename);

                

               
                //if (!PT_SET.LbEn && (num == 0 || num == 2))
                //    inf.WriteBool(str_section, "EN", false, ref ischange, true, filename);
                //else
                //    inf.WriteBool(str_section, "EN", md.benable, ref ischange, true, filename);
                if (!PT_SET.LbEn && (num == 0 || num == 2))
                {
                    inf.WriteBool(str_section, "EN", false, ref ischange, true, filename);
                    md.benable = false;
                }
                else
                    inf.WriteBool(str_section, "EN", md.benable, ref ischange, true, filename);
                if (!PT_SET.issmall)
                {
                    if ((PT_SET.bitOpenMode == 1 && md.Num % 2 == 0) || (PT_SET.bitOpenMode == 2 && md.Num % 2 == 1) ||
                        (PT_SET.bitOpenMode == 3 && md.Num < 9) || (PT_SET.bitOpenMode == 4 && md.Num > 8))
                    {
                        inf.WriteBool(str_section, "EN", false, ref ischange, true, filename);
                        md.benable = false;
                    }
                    else
                    {
                        inf.WriteBool(str_section, "EN", md.benable, ref ischange, true, filename);
                    }
                }
                else
                    inf.WriteBool(str_section, "EN", md.benable, ref ischange, true, filename);
            }

            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name);
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //第二层
                backup_filename = backup_filename + "\\WsCfg";
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //文件
                string[] str = filename.Split('\\');
                res = SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]), backup, backup_filename);
                if (res != EM_RES.OK) return res;
            }
            SortMdDat();
            return EM_RES.OK;
        }
        #endregion

        #region 状态检测
        public bool _isFrUp;
        /// <summary>
        /// 检查是否所有前排气缸打开
        /// </summary>
        public bool isFrUp
        {
            get
            {
                foreach (Cylinder cy in list_cld_fr)
                {
                    if (cy.isOFFByChkSen) return _isFrUp = false;
                }
                return _isFrUp = true;
            }
        }

        public bool _isBkUp;
        /// <summary>
        /// 检查是否所有后排气缸打开
        /// </summary>
        public bool isBkUp
        {
            get
            {
                foreach (Cylinder cy in list_cld_bk)
                {
                    if (cy.isOFFByChkSen) return _isBkUp = false;
                }
                return _isBkUp = true;
            }
        }

        public bool _isAllUp;
        /// <summary>
        /// 检查是否所有气缸打开
        /// </summary>
        public bool isAllUp
        {
            get
            {
                foreach (Cylinder cy in list_cld)
                {
                    if (cy.isOFFByChkSen) return _isAllUp = false;
                }
                return _isAllUp = true;
            }
        }

        public bool _isFrDown;
        /// <summary>
        /// 检查是否所有前排气缸关闭
        /// </summary>
        public bool isFrDown
        {
            get
            {
                foreach (Cylinder cy in list_cld_fr)
                {
                    if (cy.isONByChkSen) return _isFrDown = false;
                }
                return _isFrDown = true;
            }
        }

        public bool _isBkDown;
        /// <summary>
        /// 检查是否所有后排气缸打开
        /// </summary>
        public bool isBkDown
        {
            get
            {
                foreach (Cylinder cy in list_cld_bk)
                {
                    if (cy.isONByChkSen) return _isBkDown = false;
                }
                return _isBkDown = true;
            }
        }



        public bool _isAllDown;
        /// <summary>
        /// 检查是否所有气缸打开
        /// </summary>
        public bool isAllDown
        {
            get
            {
                foreach (Cylinder cy in list_cld)
                {
                    if (cy.isONByChkSen) return _isAllDown = false;
                }
                return _isAllDown = true;
            }
        }


        public bool _isFrOpen = false;
        public bool isFrOpen
        {
            get
            {
                return _isFrOpen = true;
            }
        }

        public bool _isFrClose = false;
        public bool isFrClose
        {
            get
            {
                return _isFrClose = true;
            }
        }

        public bool _isBkOpen = false;
        public bool isBkOpen
        {
            get
            {
                return _isBkOpen = true;
            }
        }

        public bool _isBkClose = false;
        public bool isBkClose
        {
            get
            {
                return _isBkClose = true;
            }
        }

        public bool _isUInTestPos = false;
        public bool isUInTestPos
        {
            get
            {
                return _isUInTestPos = Math.Abs(ax_u.fenc_pos - ax_u.pos0) < 0.5;
            }
        }

        public bool _isUInFeedPos = false;
        public bool isUInFeedPos
        {
            get
            {
                return _isUInFeedPos = Math.Abs(ax_u.fenc_pos - ax_u.pos1) < 0.5;
            }
        }

        public bool isInTestPos
        {
            get
            {
                // if (isUInTestPos && !isFrUp && !isBkUp && isFrClose && isBkClose) return true;
                if (isUInTestPos && !isFrUp && !isBkUp && isFrDown && isBkDown) return true;
                else return false;
            }
        }
        public bool isInFeedPos
        {
            get
            {
                //if (isUInFeedPos && isFrUp && isBkUp && isFrOpen && isBkOpen) return true;
                if (isUInFeedPos && isFrUp && isBkUp) return true;
                else return false;
            }
        }

        /// <summary>
        /// 获取指定状态的气缸列表
        /// </summary>
        /// <param name="bON_STA">True:打开状态，False:关闭状态</param>
        /// <param name="sel">-1:所有，0:前排，1:后排</param>
        /// <returns></returns>
        public List<Cylinder> GetCyByStatus(bool bON_STA = true, int sel = -1)
        {
            List<Cylinder> list_cy = new List<Cylinder>();
            list_cy.Clear();

            if (list_cld_fr != null)
            {
                foreach (Cylinder cy in list_cld_fr)
                {
                    if (bON_STA && !cy.isONByChkSen || !bON_STA && !cy.isOFFByChkSen) list_cy.Add(cy);
                }
            }

            if (list_cld_bk != null)
            {
                foreach (Cylinder cy in list_cld_bk)
                {
                    if (bON_STA && !cy.isONByChkSen || !bON_STA && !cy.isOFFByChkSen) list_cy.Add(cy);
                }
            }
            return list_cy;
        }

        #endregion

        #region 动作执行
        /// <summary>
        /// 回零
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES Home(ref bool bquit)
        {
            //U home
            VAR.gsys_set.bquit = false;
            ax_u.HomeTask(20000);
            while (true)
            {
                if (ax_u.HomeTaskisEnd) break;
                Thread.Sleep(10);
                Application.DoEvents();
            }
            if (ax_u.HomeTaskRet != EM_RES.OK)
            {
                ax_u.Stop();
            }

            //all up
            EM_RES res = FrCyUp(ref bquit, 2000, true, -1, false);
            if (res != EM_RES.OK) return res;

            res = BkCyUp(ref bquit, 2000, true, -1, false);
            if (res != EM_RES.OK) return res;

            //home

            //检查气缸状态
            if (isFrUp)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "前排气缸未全压盖，禁止U复位!" : "The front cylinder is not fully glanded, U axis is prohibited from resetting!      (前排气缸未全压盖，禁止U复位!)", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }
            if (isBkUp)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "后排气缸未全压盖，禁止U复位!" : "The rear cylinder is not fully glanded, U axis is prohibited from resetting!  (后排气缸未全压盖，禁止U复位!)", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            //U 复位
            //打开才能前后回零
            return EM_RES.OK;
        }
        /// <summary>
        /// 旋转到上料位置
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES TurnToFeed(ref bool bquit, bool ChkUpDownPos = false)
        {
            //检查气缸状态
            if (isFrUp && !isFrDown)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "前排气缸未全压盖，禁止U旋转!" : "The front cylinder is not fully glanded, U axis is prohibited from spinning!      (前排气缸未全压盖，禁止U旋转!", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }
            if (isBkUp && !isBkDown)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "后排气缸未全压盖，禁止U旋转!" : "The rear cylinder is not fully glanded, U axis is prohibited from spinning!      (后排气缸未全压盖，禁止U旋转!", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            //检测上下料是否安全
            //todo

            //检查位置，不在上料位不能翻转
            if (pos_idx != (int)Turntable.EM_STA.POS0 && pos_idx != (int)Turntable.EM_STA.POS2)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}不在上料/OTP位置，禁止翻转!", ax_u.disc) : string.Format("{0} is not in the upload/OTP,it is forbidden from fliping!          ({0}不在上料/OTP位置，禁止翻转!)", ax_u.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            if (ChkUpDownPos && pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
            {
                Thread.Sleep(300);
                if (ChkUpDownPos && pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止翻转", disc) : string.Format("Updownload did not return to the safe position, {0} is prohibited from turning!       (上下料未回安全位置,{0}禁止翻转)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }

            if (pos_idx == (int)Turntable.EM_STA.POS2 && !MT.AXIS_BOX_OTP_Z.isORG)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}在OPT位置，OTP光源不在原点，禁止翻转!", ax_u.disc) : string.Format("{0} is in the OTP,but OTP is not in the origin,it is prohibited from turning!        ({0}在OPT位置，OTP光源不在原点，禁止翻转!", ax_u.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1);
                return EM_RES.MOVE_PROTECT;
            }
            return ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.P);              //P去上料位
        }
        /// <summary>
        /// 旋转到测试位置
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES TurnToTest(ref bool bquit, bool ChkUpDownPos = false)
        {
            //检查位置，不在上料位不能翻转
            if (pos_idx != (int)Turntable.EM_STA.POS0 && pos_idx != (int)Turntable.EM_STA.POS2)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}不在上料/OTP位置，禁止翻转!", ax_u.disc) : string.Format("{0} is not in the upload/OTP,it is prohibited from turning!            ({0}不在上料/OTP位置，禁止翻转!)", ax_u.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            if (ChkUpDownPos && pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
            {
                Thread.Sleep(300);
                if (ChkUpDownPos && pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y+3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y+3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止翻转", disc) : string.Format("Updownload is not in the safe pos,{0} is prohibited from turning!         (上下料未回安全位置,{0}禁止翻转)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }

            if (pos_idx == (int)Turntable.EM_STA.POS2 && !MT.AXIS_BOX_OTP_Z.isORG)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}在OTP位置，OTP光源不在原点，禁止翻转!", ax_u.disc) : string.Format("{0} is in the OTP,but OTP is not in the origin,{0} is prohibited from turning!          ({0}在OTP位置，OTP光源不在原点，禁止翻转!)", ax_u.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            //检查气缸状态
            if (isFrUp && !isFrDown)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "前排气缸未全压盖，禁止U旋转到测试位!" : "The front cylinders are not fully glanded, forbid U axis to rotate to the test position!    (前排气缸未全压盖，禁止U旋转到测试位!)", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }
            if (isBkUp && !isBkDown)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? "后排气缸未全压盖，禁止U旋转到测试位!" : "The rear cylinder is not fully glanded, and U axis is not allowed to rotate to the test position!         (后排气缸未全压盖，禁止U旋转到测试位!)", DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }
            ////检查位置
            //if (isFrOpen)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "前排气缸未在压盖位，禁止U旋转到测试位!");
            //    return EM_RES.MOVE_PROTECT;
            //}
            //if (isBkOpen)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "后排气缸未在压盖位，禁止U旋转到测试位!");
            //    return EM_RES.MOVE_PROTECT;
            //}
            return ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.N);
        }

        public EM_RES FrCyUp(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;

            //check param

            //make sure U is on FEEDR position
            if (bprotect && !isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}前排不在水平位置，禁止抬升压盖!", disc) : string.Format("{0} The front row is not in the horizontal position. Lifting the cover is prohibited!          ({0}前排不在水平位置，禁止抬升压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            ////make sure ax is in position
            //if (Math.Abs(ax_fr.fenc_pos) > 0.1)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}前排不在压盖位置，禁止上升!", disc));
            //    return EM_RES.GPIO_PROTECT;
            //}
            if (PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOff();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld_fr)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOn();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld_fr.Count)
            {
                res = list_cld_fr.ElementAt(idx).SetOn();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                if (idx == -1 && isFrUp) break;
                if (idx >= 0 && list_cld_fr.ElementAt(idx).isONByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},抬升超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} Lifted timeout ({2} ms)             ({0}气缸{1},抬升超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.TIMEOUT;
                }
            }
            return res;
        }

        public EM_RES AllCyUp(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;

            //check param

            //make sure U is on FEEDR position
            if (bprotect && !isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}不在水平位置，禁止抬升压盖!", disc) : string.Format("{0} is not in a horizontal position. Lifting the cover is prohibited!          ({0}不在水平位置，禁止抬升压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            if (PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOff();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOn();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld.Count)
            {
                res = list_cld.ElementAt(idx).SetOn();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                if (idx == -1 && isAllUp) break;
                if (idx >= 0 && list_cld.ElementAt(idx).isONByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},抬升超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} Lifted timeout ({2} ms)           ({0}气缸{1},抬升超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.TIMEOUT;
                }
            }
            //保证开盖到位
            if (dly_temp > 0) Thread.Sleep(300);
            return res;
        }
        public EM_RES FrCyDown(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1)
        {
            EM_RES res = EM_RES.OK;

            //check param

            //make sure U is on FEEDR position
            //if (Math.Abs(ax_u.fcmd_pos - pos_feed_u) <1) return EM_RES.MOVE_PROTECT;
            //make sure U is on FEEDR position
            if (!isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}前排不在水平位置，禁止压盖!", disc) : string.Format("The front row of {0} is not in a horizontal position, and glands are prohibited!         ({0}前排不在水平位置，禁止压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            //make sure ax is in position
            //if (!isFrClose)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}前排不在压盖位置，禁止下压!", disc));
            //    return EM_RES.GPIO_PROTECT;
            //}

            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld_fr)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOff();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld_fr.Count)
            {
                res = list_cld_fr.ElementAt(idx).SetOff();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                if (idx == -1 && isFrDown) break;
                if (idx >= 0 && list_cld_fr.ElementAt(idx).isOFFByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} lowered timeout ({2} ms)           ({0}气缸{1},下压超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect, ErrCode: ShowErrMsg.WsCylTimeOut);
                    return EM_RES.TIMEOUT;
                }
            }
            //保证压盖到位
            if (dly_temp > 0) Thread.Sleep(500);
            if (isBkDown && isFrDown && PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOn();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            return res;
        }

        //public EM_RES FrCyDown1(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1)
        //{
        //    EM_RES res = EM_RES.OK;

        //    //check param

        //    //make sure U is on FEEDR position
        //    //if (Math.Abs(ax_u.fcmd_pos - pos_feed_u) <1) return EM_RES.MOVE_PROTECT;
        //    //make sure U is on FEEDR position

        //    //make sure ax is in position
        //    //if (!isFrClose)
        //    //{
        //    //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}前排不在压盖位置，禁止下压!", disc));
        //    //    return EM_RES.GPIO_PROTECT;
        //    //}

        //    //Up Cylinder
        //    if (idx == -1)
        //    {
        //        foreach (Cylinder cy in list_cld_fr)
        //        {
        //            if (bquit) return EM_RES.QUIT;
        //            res = cy.SetOff();
        //            if (res != EM_RES.OK) return res;
        //        }
        //    }
        //    else if (idx >= 0 && idx < list_cld_fr.Count)
        //    {
        //        res = list_cld_fr.ElementAt(idx).SetOff();
        //        if (res != EM_RES.OK) return res;
        //    }

        //    //等待传感器
        //    if (dly <= 0) return EM_RES.OK;
        //    int dly_temp = dly;
        //    while (true)
        //    {
        //        if (bquit) return EM_RES.QUIT;

        //        //check sensor
        //        if (idx == -1 && isFrDown) break;
        //        if (idx >= 0 && list_cld_fr.ElementAt(idx).isOFFByChkSen) break;

        //        //timeout
        //        if (dly > 0)
        //        {
        //            Thread.Sleep(10);
        //            if (dly > 0) dly -= 10;
        //        }
        //        if (dly <= 0)
        //        {
        //            List<Cylinder> list_cy = GetCyByStatus(false);
        //            string str = string.Empty;
        //            foreach (Cylinder cy in list_cy) str = str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",";
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp));
        //            return EM_RES.TIMEOUT;
        //        }
        //    }
        //    //保证压盖到位
        //    if (dly_temp > 0) Thread.Sleep(500);

        //    return res;
        //}

        public EM_RES AllCyDown(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;

            //check param
            if (bprotect && !isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}不在水平位置，禁止压盖!", disc) : string.Format("{0} is not in a horizontal position, and glands are prohibited!           ({0}不在水平位置，禁止压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                //return EM_RES.MOVE_PROTECT;
            }


            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOff();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld.Count)
            {
                res = list_cld.ElementAt(idx).SetOff();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                //if (idx == -1 && isAllDown) break;
                if (isAllDown) break;
                if (idx >= 0 && list_cld.ElementAt(idx).isOFFByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} lowered timeout ({2} ms)           ({0}气缸{1},下压超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.TIMEOUT;
                }
            }
            //保证压盖到位
            if (dly_temp > 0) Thread.Sleep(100);
            if (isAllDown && PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOn();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            return res;
        }

        public EM_RES BkCyUp(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;

            //check param

            //make sure U is on FEEDR position
            if (bprotect && !isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}后排不在水平位置，禁止抬升压盖!", disc) : string.Format("The back row of {0} is not in a horizontal position. Lifting the lift cover is prohibited!           ({0}后排不在水平位置，禁止抬升压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            ////make sure ax is in position
            //if (Math.Abs(ax_bk.fenc_pos) > 0.1)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}后排不在压盖位置，禁止上升!", disc));
            //    return EM_RES.GPIO_PROTECT;
            //}
            if (PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOff();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld_bk)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOn();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld_bk.Count)
            {
                res = list_cld_bk.ElementAt(idx).SetOn();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                if (idx == -1 && isBkUp) break;
                if (idx >= 0 && list_cld_bk.ElementAt(idx).isONByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} lowered timeout ({2} ms)           ({0}气缸{1},下压超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect, ErrCode: ShowErrMsg.WsCylTimeOut);
                    return EM_RES.TIMEOUT;
                }
            }
            return res;
        }
        public EM_RES BkCyDown(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1)
        {
            EM_RES res = EM_RES.OK;

            //check param

            //make sure U is on FEEDR position
            //if (Math.Abs(ax_u.fcmd_pos - pos_feed_u) <1 ) return EM_RES.MOVE_PROTECT;
            if (!isUInFeedPos)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}前排不在水平位置，禁止压盖!", disc) : string.Format("The front row of the {0} is not in a horizontal position, and glands are prohibited!        ({0}前排不在水平位置，禁止压盖!)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                return EM_RES.MOVE_PROTECT;
            }

            //make sure ax is in position
            //if (!isBkClose)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}后排不在压盖位置，禁止下压!", disc));
            //    return EM_RES.GPIO_PROTECT;
            //}

            //Up Cylinder
            if (idx == -1)
            {
                foreach (Cylinder cy in list_cld_bk)
                {
                    if (bquit) return EM_RES.QUIT;
                    res = cy.SetOff();
                    if (res != EM_RES.OK) return res;
                }
            }
            else if (idx >= 0 && idx < list_cld_bk.Count)
            {
                res = list_cld_bk.ElementAt(idx).SetOff();
                if (res != EM_RES.OK) return res;
            }

            //等待传感器
            if (dly <= 0) return EM_RES.OK;
            int dly_temp = dly;
            while (true)
            {
                if (bquit) return EM_RES.QUIT;

                //check sensor
                if (idx == -1 && isBkDown) break;
                if (idx >= 0 && list_cld_bk.ElementAt(idx).isOFFByChkSen) break;

                //timeout
                if (dly > 0)
                {
                    Thread.Sleep(10);
                    if (dly > 0) dly -= 10;
                }
                if (dly <= 0)
                {
                    List<Cylinder> list_cy = GetCyByStatus(false);
                    string str = string.Empty;
                    foreach (Cylinder cy in list_cy) str = VAR.IsChinese ? (str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",") : (str + cy.io_out1.english_disc + "," + cy.io_out2.english_disc + ",");
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp) : string.Format("{0} cylinder{1} lowered timeout ({2} ms)         ({0}气缸{1},下压超时({2}ms))", disc, str, dly_temp), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect, ErrCode: ShowErrMsg.WsCylTimeOut);
                    return EM_RES.TIMEOUT;
                }
            }
            if (isBkDown && isFrDown && PT_SET.bCool)
            {
                this.gpio_out_gz_wind.SetOn();
                if (gpio_out_gz_wind.res != EM_RES.OK) return gpio_out_gz_wind.res;
            }
            return res;
        }

        //public EM_RES BkCyDown1(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1)
        //{
        //    EM_RES res = EM_RES.OK;

        //    //check param

        //    //make sure U is on FEEDR position
        //    //if (Math.Abs(ax_u.fcmd_pos - pos_feed_u) <1 ) return EM_RES.MOVE_PROTECT;

        //    //make sure ax is in position
        //    //if (!isBkClose)
        //    //{
        //    //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}后排不在压盖位置，禁止下压!", disc));
        //    //    return EM_RES.GPIO_PROTECT;
        //    //}

        //    //Up Cylinder
        //    if (idx == -1)
        //    {
        //        foreach (Cylinder cy in list_cld_bk)
        //        {
        //            if (bquit) return EM_RES.QUIT;
        //            res = cy.SetOff();
        //            if (res != EM_RES.OK) return res;
        //        }
        //    }
        //    else if (idx >= 0 && idx < list_cld_bk.Count)
        //    {
        //        res = list_cld_bk.ElementAt(idx).SetOff();
        //        if (res != EM_RES.OK) return res;
        //    }

        //    //等待传感器
        //    if (dly <= 0) return EM_RES.OK;
        //    int dly_temp = dly;
        //    while (true)
        //    {
        //        if (bquit) return EM_RES.QUIT;

        //        //check sensor
        //        if (idx == -1 && isBkDown) break;
        //        if (idx >= 0 && list_cld_bk.ElementAt(idx).isOFFByChkSen) break;

        //        //timeout
        //        if (dly > 0)
        //        {
        //            Thread.Sleep(10);
        //            if (dly > 0) dly -= 10;
        //        }
        //        if (dly <= 0)
        //        {
        //            List<Cylinder> list_cy = GetCyByStatus(false);
        //            string str = string.Empty;
        //            foreach (Cylinder cy in list_cy) str = str + cy.io_out1.str_disc + "," + cy.io_out2.str_disc + ",";
        //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}气缸{1},下压超时({2}ms)", disc, str, dly_temp));
        //            return EM_RES.TIMEOUT;
        //        }
        //    }
        //    return res;
        //}

        //public EM_RES FrMove(ref bool bquit, bool bopen, int dly = 3000, bool bdoevent = false)
        //{
        //    if (!isFrUp)
        //    {
        //        MessageBox.Show(string.Format("{0}前排气缸未全部打开，不能移动！", disc), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return EM_RES.MOVE_PROTECT;
        //    }
        //    bool bres = ax_fr.AzdMotor.MoveToSelDat(ref VAR.gsys_set.bquit, (byte)(bopen ? 1 : 0));
        //    if (bres) return EM_RES.ERR;
        //    else return EM_RES.OK;
        //}
        //public EM_RES BkMove(ref bool bquit, bool bopen, int dly = 3000, bool bdoevent = false)
        //{
        //    if (!isBkUp)
        //    {
        //        MessageBox.Show(string.Format("{0}后排气缸未全部打开，不能移动！", disc), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return EM_RES.MOVE_PROTECT;
        //    }
        //    bool bres = ax_bk.AzdMotor.MoveToSelDat(ref VAR.gsys_set.bquit, (byte)(bopen ? 1 : 0));
        //    if (bres) return EM_RES.ERR;
        //    else return EM_RES.OK;
        //}

        public EM_RES PowerOn(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;
            this.gpio_out_gz_power.SetOff();
            if (gpio_out_gz_power.res != EM_RES.OK) return gpio_out_gz_power.res;
            return res;
        }

        public EM_RES PowerOff(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;
            this.gpio_out_gz_power.SetOn();
            if (gpio_out_gz_power.res != EM_RES.OK) return gpio_out_gz_power.res;
            return res;
        }

        public EM_RES ZKOff(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;
            foreach (GPIO gpio in list_gpio_zk)
            {
                gpio.SetOn();
                if (gpio.res != EM_RES.OK) return gpio.res;
            }
            return res;
        }

        public EM_RES ZKOn(ref bool bquit, int dly = 1000, bool bdoevent = false, int idx = -1, bool bprotect = true)
        {
            EM_RES res = EM_RES.OK;
            foreach (GPIO gpio in list_gpio_zk)
            {
                gpio.SetOff();
                if (gpio.res != EM_RES.OK) return gpio.res;
            }
            return res;
        }

        #endregion

        #region 上下料
        /// <summary>
        /// 准备上下料，旋转至水平，打开压盖
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES SetupForFeed(ref bool bquit)
        {
            EM_RES res = EM_RES.OK;
            //已经在上下料位置
            if (isInFeedPos) return EM_RES.OK;//!COM.UDLoad1.ax_x.isORG || !COM.UDLoad2.ax_x.isORG
            if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos>3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y+3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y+3)))
            {
                Thread.Sleep(300);
                if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止合盖与翻转", disc) : string.Format("Updownload is not in the safe pos,{0} is forbidden to close and flip!         (上下料未回安全位置,{0}禁止合盖与翻转)", disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.MoveProtect);
                    
                    return EM_RES.ERR;
                }                   
            }

            try
            {
                res = ZKOn(ref VAR.gsys_set.bquit);
                if (res != EM_RES.OK) return res;
                //转到水平
                //EM_RES res = ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.P);
                res = TurnToFeed(ref bquit);
                if (res != EM_RES.OK) return res;

                //这里加入其它面不可以进行测试的指令
                COM.Cannottest = true;
                res = AllCyUp(ref bquit,waittime);
                COM.Cannottest = false;
                if (res != EM_RES.OK)
                {
                    return res;
                }
                if (isFrDown || isBkDown || !isInFeedPos)
                {
                    res = EM_RES.ERR;
                    return res;
                }
            }
            finally
            {
                ZKOff(ref VAR.gsys_set.bquit);
            }

            //if (res != EM_RES.OK) return res;

            //开
            //bool bres = ax_fr.AzdMotor.MoveToSelDat(ref bquit, 0, 0);
            //if (!bres) return EM_RES.ERR;
            //bres = ax_bk.AzdMotor.MoveToSelDat(ref bquit, 0, 3000);
            //if (!bres) return EM_RES.ERR;

            return EM_RES.OK;
        }
        /// <summary>
        /// 准备测试，关闭压盖，旋转测试位
        /// </summary>
        /// <param name="bquit"></param>
        /// <returns></returns>
        public EM_RES SetupForTest(ref bool bquit, bool IfDelay = true)
        {
            EM_RES res = EM_RES.OK;
            //已经在测试位置
            if (isInTestPos) return EM_RES.OK;
            if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
            {
                Thread.Sleep(300);
                if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止合盖与翻转", disc) : string.Format("Updownload is not in the safe pos,{0} is forbidden to close and flip!         (上下料未回安全位置,{0}禁止合盖与翻转)", disc));
                    return EM_RES.ERR;
                }
            }

            //if (!isFrUp && isFrOpen)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "前排气缸未抬升，禁止气缸移动!");
            //    return EM_RES.MOVE_PROTECT;
            //}
            //if (!isBkUp && isBkOpen)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "后排气缸未抬升，禁止气缸移动!");
            //    return EM_RES.MOVE_PROTECT;
            //}

            ////合盖
            //bool bres = ax_fr.AzdMotor.MoveToSelDat(ref bquit, 1, 0);
            //if (!bres) return EM_RES.ERR;
            //bres = ax_bk.AzdMotor.MoveToSelDat(ref bquit, 1, 3000);
            //if (!bres) return EM_RES.ERR;

            //压盖
            //res = FrCyDown(ref VAR.gsys_set.bquit);
            //if (res != EM_RES.OK) return res;
            //res = BkCyDown(ref VAR.gsys_set.bquit);
            //if (res != EM_RES.OK) return res;
            res = AllCyDown(ref VAR.gsys_set.bquit);
            if (res != EM_RES.OK) return res;
            //res = ZKOn(ref VAR.gsys_set.bquit);
            //if (res != EM_RES.OK) return res;
            //foreach (GPIO gpio in MT.List_GPIO_OUT_WS_ZK_IN)
            //{
            //    gpio.SetOn();
            //}

            if (isFrUp || !isFrDown || isBkUp || !isBkDown)
            {
                res = EM_RES.ERR;
                return res;
            }

            //等待压盖到位
            if (IfDelay)
                Thread.Sleep(400);

            //测试位
            res = TurnToTest(ref bquit);
            //res = ax_u.JOG_Step(ref VAR.gsys_set.bquit, AXIS.AX_DIR.N);
            if (res != EM_RES.OK) return res;

            return EM_RES.OK;
        }

        public EM_RES AllCyClose(ref bool bquit, bool IfDelay = true)
        {
            EM_RES res = EM_RES.OK;

            if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
            {
                Thread.Sleep(300);
                if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止合盖与翻转", disc) : string.Format("Updownload is not in the safe pos,{0} is forbidden to close and flip!         (上下料未回安全位置,{0}禁止合盖与翻转)", disc));
                    return EM_RES.ERR;
                }
            }

            res = AllCyDown(ref VAR.gsys_set.bquit);
            if (res != EM_RES.OK) return res;

            if (isFrUp || !isFrDown || isBkUp || !isBkDown)
            {
                res = EM_RES.ERR;
                return res;
            }

            //等待压盖到位
            if (IfDelay)
                Thread.Sleep(400);

            return EM_RES.OK;
        }


        public EM_RES TurnToFeedSafe(ref bool bquit, bool IfDelay = true)
        {
            EM_RES res = EM_RES.OK;
            //已经在上下料位置
            if (isInFeedPos) return EM_RES.OK;
            if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
            {
                Thread.Sleep(300);
                if (pos_idx == (int)Turntable.EM_STA.POS0 && ((!COM.UDLoad1.ax_x.isORG && COM.UDLoad1.ax_x.fenc_pos > 3) || (!COM.UDLoad2.ax_x.isORG && COM.UDLoad2.ax_x.fenc_pos < -3) || COM.UDLoad1.ax_y.fenc_pos > (COM.UDLoad1.list_xt[1].st_cap_pos.y + 3) || COM.UDLoad2.ax_y.fenc_pos > (COM.UDLoad2.list_xt[1].st_cap_pos.y + 3)))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("目前上下料1X的位置：{0}，上下料2X的位置：{1}，上下料1Y的位置：{2}，上下料2Y的位置：{3}", COM.UDLoad1.ax_x.fenc_pos, COM.UDLoad2.ax_x.fenc_pos, COM.UDLoad1.ax_y.fenc_pos, COM.UDLoad2.ax_y.fenc_pos));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("上下料未回安全位置,{0}禁止合盖与翻转", disc) : string.Format("Updownload is not in the safe pos,{0} is forbidden to close and flip!         (上下料未回安全位置,{0}禁止合盖与翻转)", disc));
                    return EM_RES.ERR;
                }
            }

            res = ZKOn(ref VAR.gsys_set.bquit);
            if (res != EM_RES.OK) return res;
            //转到水平
            //EM_RES res = ax_u.JOG_Step(ref bquit, AXIS.AX_DIR.P);
            res = TurnToFeed(ref bquit);
            if (res != EM_RES.OK) return res;

            return EM_RES.OK;
        }
        #endregion

        #region 测试通信
        public EM_RES StartTestFlow(int status = 0, bool demo = false)
        {
            EM_RES res = EM_RES.OK;
            if (NewSysInf.UserParams.bBeforeTestReset)
            {
                //关掉当前测试                    
                res = NextTest(-1, Demo);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("在StartTestFlow中关闭当前测试") : "Close current test.   (关闭当前测试)");
                if (res != EM_RES.OK)
                {
                    TestStatus = EM_TEST_STA.ERROR;
                    return EM_RES.ERR;
                }
                Thread.Sleep(100);
            }
            foreach (List<MdDat> list in list_list_md)
            {
                if (list.Count == 0) continue;
                //全禁用
                if (list.Find(delegate (MdDat x) { return x.benable; }) == null) continue;

                int ret = 0;
                try
                {
                    //整合二维码
                    string str_barcode = "";
                    foreach (MdDat m in list)
                    {
                        if (PT_SET.bmotorphoto || PT_SET.bdownqr)
                        {
                            if (m.benable)
                            {
                                if (!VAR.ClearMt)
                                {
                                    str_barcode += (m.bardcode == null ? "NOBARCODE" : m.bardcode) + ";" +
                                                   (m.motor_barcode == null ? "REEORCODE" : m.motor_barcode) + "#";
                                }
                                else
                                {
                                    str_barcode += (m.bardcode == null ? "NULL" : m.bardcode) + ";" +
                                                   (m.motor_barcode == null ? "NULL" : m.motor_barcode) + "#";
                                }
                            }
                            else
                                str_barcode += "NULL" + ";" + "NULL" + "#";
                        }
                        else
                        {
                            if (m.benable)
                            {
                                if (!VAR.ClearMt)
                                {
                                    str_barcode += (m.bardcode == null ? "NOBARCODE" : m.bardcode) + "#";
                                }
                                else
                                {
                                    str_barcode += (m.bardcode == null ? "NULL" : m.bardcode) + "#";
                                }
                            }
                            else
                                str_barcode += "NULL" + "#";
                        }
                    }

                    foreach (MdDat m in list)
                    {
                        // if (m.benable && (!VAR.gsys_set.isChkMode|| (VAR.gsys_set.isChkMode && VAR.ChkPC == m.PC_ID)))
                        if (m.benable && m.res == (int)Product.EM_CM_RES.UNTEST)//没有模组不需要测试。
                        {
                            str_barcode += "1" + "@";
                        }
                        else
                            str_barcode += "0" + "@";
                    }
                  
                   if( PT_SET.BarcodeMode != (int)PT_SET.BAR_SCAN.NO_SCAN  )
                   {
                        #region   二维码防呆
                        List<string> numbers = new List<string>();
                        List<string> lastnumbers = new List<string>();


                        numbers.Clear();
                        foreach (MdDat m in list)
                        {
                            if (m.benable && m.res == (int)Product.EM_CM_RES.UNTEST)//没有模组不需要测试。
                            {
                                numbers.Add(m.bardcode);
                            }
                        }
                        bool Err = false;
                        if (lastnumbers.Count > 0)
                        {
                            var differentElements = lastnumbers.Except(numbers).ToList();

                            if (!differentElements.Any())
                            {
                                Err = true;
                            }
                        }

                        lastnumbers.Clear();
                        lastnumbers = numbers;

                        var distinctNumbers = numbers.Distinct().ToList();

                        // 如果原始列表和不重复元素集合的长度不同，说明存在重复元素  
                        if (numbers.Count != distinctNumbers.Count || Err)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "启动时，存在重复二维码或者和上一轮存在重复二维码");
                            MT.ST_WARN st_warn = new MT.ST_WARN();
                            warning fr_warn = new warning();
                            st_warn.ok_txt = VAR.IsChinese ? "继续运行" : "Give up";
                            st_warn.abort_txt = VAR.IsChinese ? "停止运行" : "Try again";
                            st_warn.ws = null;
                            st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                            st_warn.title = VAR.IsChinese ? "提示:存在重复二维码!" : "Tip: Abnormal suction!";
                            st_warn.msg = VAR.IsChinese ? "提示:存在重复二维码!" : "Tip: Abnormal suction!";
                            st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.启动时，存在重复二维码!\r\n  " +
                                "2.点击继续运行则继续运行!\r\n  " +
                                "3.点击停止将停止运行!\r\n  ";

                            DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            if (DialogResult.OK == logres)
                            {

                            }
                            else
                            {
                                return EM_RES.QUIT; ;
                            }
                        }
                        #endregion
                    }


                    //发送启动命令
                    if (demo)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("PC{0} barcode:{1}", list[0].PC_ID, str_barcode));
                        return EM_RES.OK;
                    }
                    ret = TestPC.StartTestFlow(list[0].PC_ID, status, str_barcode.ToArray());
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("PC{0} barcode:{1}", list[0].PC_ID, str_barcode));
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("PC{0} StartFlow,ret={1},sta={2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), status));
                    if (ret != (int)TestPC.EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("PC{0} 重发 StartFlow,ret={1},sta={2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), status) : string.Format("PC{0} retry StartFlow,ret={1},sta={2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), status));
                        Thread.Sleep(1000);
                        ret = TestPC.StartTestFlow(list[0].PC_ID, status, str_barcode.ToArray());
                        if (ret != (int)TestPC.EM_RES.OK)
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("PC{0} 重发 StartFlow,ret={1},sta={2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), status) : string.Format("PC{0} retry StartFlow,ret={1},sta={2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), status), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                        return EM_RES.ERR;
                    }
                }
                catch (Exception ex)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("StartTestFlow,ID={0}, {1},{2}", list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), ex.Message), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                    return EM_RES.ERR;
                }
            }
            return EM_RES.OK;
        }

        public EM_RES WaitBeforeOpenImage(ref bool bquit)
        {
            int waitMs = Math.Max(0, NewSysInf.UserParams.BeforeOpenImageWaitTime);
            bool stopOtherAxes = NewSysInf.UserParams.bBeforeOpenImageStopAxis;

            if (!stopOtherAxes && waitMs <= 0)
            {
                return EM_RES.OK;
            }

            if (stopOtherAxes)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc} 上料位提前开图前停止其他轴");
                VAR.bBeforeOpenImageAxisStatic = true;
                MT.AllAxStop();
            }

            try
            {
                if (waitMs <= 0)
                {
                    return EM_RES.OK;
                }

                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc} 上料后延迟开图时间{waitMs}");
                int elapsed = 0;
                while (elapsed < waitMs)
                {
                    if (bquit || VAR.gsys_set.bquit)
                    {
                        return EM_RES.QUIT;
                    }

                    Thread.Sleep(50);
                    elapsed += 50;
                }

                return EM_RES.OK;
            }
            finally
            {
                if (stopOtherAxes)
                {
                    VAR.bBeforeOpenImageAxisStatic = false;
                }
            }
        }
        int DemoPos = 0;
        public EM_RES WaitTestResult(ref int status, int delay = 30000, bool demo = false, LightBox lb = null)
        {
            int sta = -1;
            PCDat pc = null;
            status = 0;
            foreach (List<MdDat> list in list_list_md)
            {
                if (list.Count == 0) continue;
                //全禁用
                if (list.Find(delegate (MdDat x) { return x.benable; }) == null) continue;

                //发送命令
                int num = 0;
                TestPC.DeviceStruct pDeviceParam = new TestPC.DeviceStruct();
                int[] res = new int[16];
                if (!demo)
                    res = new int[16] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                char[] md_barcode = new char[16 * 32];
                int ret = 0;
                try
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} WaitTestResult,ID={1}", disc, list[0].PC_ID));
                    if (!demo)
                    {
                        //ret = TestPC.WaitTestResult(list[0].PC_ID, ref sta, res, ref num, ref VAR.gsys_set.bquit, delay);
                        ret = TestPC.WaitTestResultA(list[0].PC_ID, ref sta, res, ref num, ref pDeviceParam, ref VAR.gsys_set.bquit, delay, md_barcode);
                        if (ret != (int)TestPC.EM_RES.OK && ret != (int)TestPC.EM_RES.ERR_QUIT)
                        {
                            Thread.Sleep(1000);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 重发WaitTestResult,ID={1}", disc, list[0].PC_ID) : string.Format("{0} retry WaitTestResult,ID={1}", disc, list[0].PC_ID));
                            ret = TestPC.WaitTestResultA(list[0].PC_ID, ref sta, res, ref num, ref pDeviceParam, ref VAR.gsys_set.bquit, delay, md_barcode);
                            if (ret != (int)TestPC.EM_RES.OK && ret != (int)TestPC.EM_RES.ERR_QUIT)
                            {
                                Thread.Sleep(1000);
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 再次重发WaitTestResult,ID={1}", disc, list[0].PC_ID) : string.Format("{0} retry WaitTestResult,ID={1}", disc, list[0].PC_ID));
                                ret = TestPC.WaitTestResultA(list[0].PC_ID, ref sta, res, ref num, ref pDeviceParam, ref VAR.gsys_set.bquit, delay, md_barcode);
                                if (ret != (int)TestPC.EM_RES.OK && ret != (int)TestPC.EM_RES.ERR_QUIT)
                                {
                                    Thread.Sleep(1000);
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 重发WaitTestResult,ID={1}", disc, list[0].PC_ID) : string.Format("{0} retry WaitTestResult,ID={1}", disc, list[0].PC_ID));
                                    ret = TestPC.WaitTestResultA(list[0].PC_ID, ref sta, res, ref num, ref pDeviceParam, ref VAR.gsys_set.bquit, delay, md_barcode);
                                }
                            }
                        }
                    }
                    else
                    {
                        int id_num, samecnt = 0;
                        lb.ListPos.Sort((a, b) =>
                        {
                            if (a.ID > b.ID) return 1;
                            else return -1;
                        });

                        //确认是否有相同的位置
                        samecnt = lb.GetCurPosCnt;
                        if (samecnt == 0)
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("samecnt:{0}", samecnt));
                        if (list_list_md.IndexOf(list) == 0)
                        {
                            foreach (LightBox.PosDef pd in lb.ListPos)
                            {

                                if ((lb.CurPosDef == null && pd.ID == DemoPos) || (lb.CurPosDef != null && pd.ID == lb.CurPosDef.ID && samecnt == 1) || (((lb.CurPosDef != null && DemoPos == 0 && pd.ID == lb.CurPosDef.ID) || pd.ID == DemoPos) && samecnt > 1))
                                {

                                    id_num = lb.ListPos.IndexOf(pd) + 1;
                                    num = 1;
                                    if (id_num < lb.ListPos.Count && id_num < 3)
                                    {
                                        sta = lb.ListPos[id_num].ID;
                                        DemoPos = sta;
                                    }
                                    else
                                    {
                                        DemoPos = 0;
                                        if (!PT_SET.LbEn)
                                        {
                                            if (lb.name == COM.OTPLightBox.name)
                                            {
                                                sta = 0;
                                                num = list.Count;
                                                for (int i = 0; i < list.Count * 2; i++)
                                                {
                                                    res[i] = 266;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (lb.name == COM.RightLightBox.name)
                                            {
                                                sta = 0;
                                                num = list.Count;
                                                for (int i = 0; i < list.Count * 2; i++)
                                                {
                                                    res[i] = 266;
                                                }
                                            }
                                            else
                                            {
                                                if (lb.name == COM.LeftLightBox.name)
                                                {
                                                    sta = COM.OTPLightBox.ListPos[1].ID;
                                                }
                                                else if (lb.name == COM.OTPLightBox.name)
                                                {
                                                    sta = COM.RightLightBox.ListPos[1].ID;
                                                }


                                            }
                                        }

                                    }
                                    break;
                                }

                            }
                        }
                        else
                        {
                            if (sta == 0)
                                num = list.Count;
                        }

                    }
                    if (ret == (int)TestPC.EM_RES.OK)
                    {
                        //其中一个未完成，则取未完成状态
                        status = status >= (int)sta ? status : (int)sta;
                        //sta 光箱的位置  
                        string str = string.Format("{0} PC{1},sta={2},n={3}", disc, list[0].PC_ID, sta, num);


                        if (PT_SET.AutoChkEn)
                        {
                            foreach (var mres in res)
                            {
                                if ((mres == (int)WS.Md_RES.NGLightChk || mres == (int)WS.Md_RES.NGLightChk2) && !VAR.isAutoChkMode)
                                {
                                    COUNT_DATA.cnt_ng_other++;
                                    var msg = string.Format("工站点检时间到, \r\n 请确认是否开始清料后进行自动点检!");
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg);
                                   // VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, msg, 20, true);
                                    DialogResult dr = FrRun.Dialog(Color.Yellow, "警告", msg, "确定", "取消");
                                    if (dr == DialogResult.OK)
                                    {
                                        VAR.ClearMt = true;
                                        VAR.isAutoChkMode = true;
                                        WS.AutoChkCnt = 0;
                                        foreach (var ws in COM.list_ws)
                                        {
                                            WS.TempAutoChkCnt = -1;
                                            WS.ListChkTemp.Clear();
                                            foreach (var md in ws.list_md)
                                            {
                                                md.bAutoChkOk = false;
                                            }

                                        }
                                    }
                                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                    break;
                                }
                            }
                        }
                        //测试结束，更新结果
                        if (sta == 0)
                        {
                            string br = new string(md_barcode);
                            string[] lsstr = br.Split('#');
                            List<string> rowSnapshot = new List<string>();
                            for (int n = 0; n < num && n < res.Length; n++)
                            {
                                if (list[n].res == -2)//空料无需更新结果
                                {
                                    list[n].res = -2;
                                }
                                else
                                {
                                    list[n].res = res[n];
                                }
                                //list[n].res = res[n];
                                list[n].test_idx = 0;
                                str = str + string.Format(",{0}-{1}", res[n], lsstr.Length > n && lsstr[n][0] != '\0' ? lsstr[n] : "");
                                rowSnapshot.Add(string.Format("工位{0}:res={1},bc={2}", list[n].Num, res[n], lsstr.Length > n && lsstr[n].Length > 0 ? lsstr[n] : "EMPTY"));
                                CurWSNGCode.Add(str);
                                //if (list[n].benable && (lsstr.Length<=n ||list[n].bardcode != lsstr[n] || lsstr[n] == ""))
                                //{
                                //    list[n].res = 111;
                                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} PC{1},发送:{2},回传{3}->发送与回传barcode不符,结果置为111", disc, list[0].PC_ID, list[n].bardcode, lsstr[n]));
                                //}

                            }
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} PC{1}最终结果快照->{2}", disc, list[0].PC_ID, string.Join(" | ", rowSnapshot)));
                            //确认个数
                            if (!demo)
                            {
                                int encnt = 0;
                                bRetest = false;
                                for (int i = 0; i < list.Count; i++) { if (list[i].benable) encnt++; }
                                if (encnt > num) { bRetest = true; }
                                if (bRetest)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} PC{1},sta={2},n={3}->接收结果缺失，重置待测!", disc, list[0].PC_ID, sta, num) : string.Format("{0} PC{1},sta={2},n={3}->lost result,reardy to retest!          ({0} PC{1},sta={2},n={3}->接收结果缺失，重置待测!)", disc, list[0].PC_ID, sta, num));
                                    List<string> missingSnapshot = new List<string>();
                                    for (int m = 0; m < list.Count; m++)
                                    {
                                        if (list[m].benable)
                                        {
                                            missingSnapshot.Add(string.Format("工位{0}:旧res={1},test_idx={2},bc={3}", list[m].Num, list[m].res, list[m].test_idx, string.IsNullOrWhiteSpace(list[m].bardcode) ? "EMPTY" : list[m].bardcode));
                                        }
                                    }
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, string.Format("{0} PC{1}结果缺失详情,应收={2},实收={3},当前快照->{4}", disc, list[0].PC_ID, encnt, num, string.Join(" | ", missingSnapshot)));
                                    for (int m = 0; m < list.Count; m++)
                                    {
                                        if (list[m].benable)
                                        {
                                            list[m].res = -1;
                                            list[m].test_idx = 0;
                                        }

                                    }
                                }
                            }

                            breschanged = true;
                        }

                        //更新PC数据
                        pc = list_pc_dat.Find(s => { return s.ID == list[0].PC_ID; });
                        if (pc != null)
                        {
                            pc.test_idx = sta;
                            if (sta == 0) pc.status = EM_PC_STA.READY;
                        }

                        //更新测试位置
                        foreach (MdDat md in list)
                        {
                            md.test_idx = sta;
                        }

                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, str);
                    }
                    else
                    {
                        if (!IsFirst)
                        {
                            for (int m = 0; m < list.Count; m++)
                            {
                                if (list[m].benable)
                                {
                                    list[m].res = -1;
                                    list[m].test_idx = 0;
                                }

                            }

                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WaitTestResult,ID={1},sta={2},ret={3}", disc, list[0].PC_ID, status, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese)), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1);
                            if (ret != (int)TestPC.EM_RES.ERR_QUIT) return EM_RES.QUIT;
                            else return EM_RES.ERR;
                        }

                    }
                }
                catch (Exception ex)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WaitTestResult,ID={1},ret={2},{3}", disc, list[0].PC_ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), ex.Message), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1);
                    return EM_RES.ERR;
                }
            }

            //更新测试状态,所有电脑站完成才算完成
            pc = list_pc_dat.Find(s => { return (s.status == EM_PC_STA.TEST || s.status == EM_PC_STA.WAIT); });
            if (pc != null) TestStatus = EM_TEST_STA.NEXT;
            else TestStatus = EM_TEST_STA.COMPLETED;

            //检测同步性
            PCDat pc_temp = null;
            foreach (PCDat p in list_pc_dat)
            {
                if (num == 0 || p.test_idx == 0 || p.status == EM_PC_STA.READY || p.status == EM_PC_STA.DISABLE) continue;
                if (pc_temp == null) pc_temp = p;
                if (p.test_idx != pc_temp.test_idx)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,
                        VAR.IsChinese ? string.Format("测试位置不同步,PC[{0}]/测试位[{1}]/[{2}],PC[{3}]/测试位[{4}/[{5}]]!", pc_temp.ID,
                            pc_temp.test_idx, pc_temp.status.ToString(), p.ID, p.test_idx, p.status.ToString()) : string.Format("Test locations are out of sync,PC[{0}]/Test locations[{1}]/[{2}],PC[{3}]/Test locations[{4}/[{5}]]!            (测试位置不同步,PC[{0}]/测试位[{1}]/[{2}],PC[{3}]/测试位[{4}/[{5}]]!)", pc_temp.ID,
                            pc_temp.test_idx, pc_temp.status.ToString(), p.ID, p.test_idx, p.status.ToString()), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                    return EM_RES.PARA_ERR;
                }
            }

            //检查结果一致性
            foreach (List<MdDat> list in list_list_md)
            {
                if (list.Count == 0) continue;
                //非禁用模组是否同一测试位
                MdDat md = list.Find(delegate (MdDat x) { return x.benable && (x.test_idx != list[0].test_idx); });
                if (md != null)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} WaitTestResult,结果不一致,PC={1},idx={2}!", disc, md.PC_ID, md.test_idx) : string.Format("{0} WaitTestResult,Inconsistent results,PC={1},idx={2}!         ({0} WaitTestResult,结果不一致,PC={1},idx={2}!)", disc, md.PC_ID, md.test_idx), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                    return EM_RES.PARA_ERR;
                }
            }
            return EM_RES.OK;
        }
        public EM_RES NextTest(int status, bool demo = false)
        {
            //if (status >= 0)
            {
                int ret = 0;
                foreach (List<MdDat> list in list_list_md)
                {
                    if (list.Count == 0) continue;
                    //全禁用
                    if (list.Find(delegate (MdDat x) { return x.benable; }) == null) continue;

                    //检查当前转盘位置/光箱位置是否满足再发送
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} NextTest,ID={1}, sta={2}", disc, list[0].PC_ID, status));
                    //空运行
                    if (demo) continue;
                    try
                    {
                        ret = TestPC.NextTest(list[0].PC_ID, status, 3000, ref VAR.gsys_set.bquit);
                        if (ret != (int)TestPC.EM_RES.OK && ret != (int)TestPC.EM_RES.ERR_QUIT)
                        {
                            Thread.Sleep(1000);
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese ? string.Format("{0} 重发 NextTest,ID={1}, sta={2},ret={3}", disc, list[0].PC_ID, status, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese)) : string.Format("{0} retry NextTest,ID={1}, sta={2},ret={3}       ({0} 重发 NextTest,ID={1}, sta={2},ret={3})", disc, list[0].PC_ID, status, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese)));
                            ret = TestPC.NextTest(list[0].PC_ID, status, 3000, ref VAR.gsys_set.bquit);
                            if (ret != (int)TestPC.EM_RES.OK)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} NextTest,ID={1}, sta={2},ret={3}", disc, list[0].PC_ID, status, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese)), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                                return EM_RES.ERR;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} NextTest,ID={1}, sta={2},ret={3},{4}", disc, list[0].PC_ID, status, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), ex.Message), DReport.EmErrCode.ResultNg, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                        return EM_RES.ERR;
                    }
                }
            }

            return EM_RES.OK;
        }
        public EM_RES GetTestInfo(ref List<TestPC.InfoData> list_info, bool demo = false)
        {
            if (demo) return EM_RES.OK;
            list_info.Clear();
            foreach (PCDat pc in list_pc_dat)
            {
                if (pc.status != EM_PC_STA.DISABLE)
                {
                    TestPC.InfoData Info = new TestPC.InfoData();
                    int ret = 0;
                    try
                    {
                        ret = TestPC.GetInfo(pc.ID, ref Info);
                        if (ret != (int)TestPC.EM_RES.OK) return EM_RES.ERR;
                        else list_info.Add(Info);
                    }
                    catch (Exception ex)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("GetTestInfo,ID={0}, ret={1},{2}", pc.ID, Utility.GetDescription((TestPC.EM_RES)ret, VAR.IsChinese), ex.Message));
                    }
                }
            }
            return EM_RES.OK;
        }
        public EM_PC_STA GetStatus
        {
            get
            {
                foreach (PCDat pc in list_pc_dat)
                {
                    if (pc.status != EM_PC_STA.DISABLE)
                    {
                        try
                        {
                            int sta = -1;
                            int ret = TestPC.GetStatus(pc.ID, ref sta);
                            if (ret == (int)TestPC.EM_RES.OK)
                            {
                                //检查联机
                                int t = sta & 0xFF;
                                if (pc.temp != t) pc.cnt = 0;
                                else pc.cnt++;
                                pc.temp = t;

                                //检查状态
                                t = sta >> 8;
                                if (t == 1) pc.status = EM_PC_STA.READY;
                                else if (t == 2) pc.status = EM_PC_STA.TEST;
                                else if (t == 3) pc.status = EM_PC_STA.WAIT;
                            }
                            else pc.cnt++;

                            if (pc.cnt > 5) pc.status = EM_PC_STA.LINK_ERR;
                        }
                        catch (Exception ex)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "GetStatus," + ex.Message);
                        }
                    }
                }

                bool bdis = true;
                bool berr = false;
                bool bready = true;
                bool bsametest = true;
                bool bwait = true;
                int temp_idx = -10000;
                foreach (PCDat pc in list_pc_dat)
                {
                    if (pc.status != EM_PC_STA.DISABLE) bdis = false;
                    if (pc.status == EM_PC_STA.LINK_ERR) berr = true;
                    if (pc.status != EM_PC_STA.READY) bready = false;
                    if (pc.status != EM_PC_STA.WAIT) bwait = false;

                    //是否同一位置
                    if (temp_idx == -10000) temp_idx = pc.test_idx;
                    if (pc.test_idx != temp_idx) bsametest = false;
                    temp_idx = pc.test_idx;
                }

                if (bdis) return EM_PC_STA.DISABLE;
                if (berr) return EM_PC_STA.LINK_ERR;
                if (bwait && !bsametest) return EM_PC_STA.NOT_SAME_TESTIDX;
                if (bwait) return EM_PC_STA.WAIT;
                if (bready) return EM_PC_STA.READY;
                else return EM_PC_STA.TEST;
            }
        }
        #endregion

        #region 测试线程
        Task TestTask = null;
        bool brun = false;
        public void RunTestTask()
        {
            if (TestStatus == EM_TEST_STA.EMPTY || TestStatus == EM_TEST_STA.COMPLETED)
            {
                brun = false;
                return;
            }
            if (TestTask == null || TestTask != null && TestTask.IsCompleted)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? "创建运行线程" : "Create RunTestTask     (创建运行线程)");
                if (TestTask != null) TestTask.Dispose();
                TestTask = new Task(RunTest);
                brun = true;
                TestTask.Start();
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? (disc + "测试线程未退出无法创建!") : (disc + "The test thread cannot be created without exiting!            (测试线程未退出无法创建!)"), ErrCode: ShowErrMsg.TestThreadExistErr);
            }
        }
        private  string lastData = string.Empty;

        public bool CompareData(string currentData)
        {

            if (currentData != lastData && lastData != string.Empty)
            {
                lastData = currentData;
                return true;

            }
            else
            {
                lastData = currentData;
                return false;
            }

            

        }
        public EM_RES WaitTestTask(ref bool bquit)
        {
            if (TestTask == null) return EM_RES.PARA_ERR;

            while (!bquit)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                if (TestTask.IsCompleted) break;
                if (!brun) break;
            }
            return EM_RES.OK;
        }
        //是否上下料开始
        public bool bUpDnStart = false;
        public static DateTime CurOutProductTime = DateTime.Now;
        private static volatile bool _quitFlag = false;
        public static class ComPortGuard
        {
            private static readonly Dictionary<string, SemaphoreSlim> _guards
                = new Dictionary<string, SemaphoreSlim>();

            private static readonly object _lock = new object();

            public static SemaphoreSlim Get(string comName)
            {
                lock (_lock)
                {
                    if (!_guards.ContainsKey(comName))
                        _guards[comName] = new SemaphoreSlim(1, 1);

                    return _guards[comName];
                }
            }
        }

        public static class GyroscopeMonitor
        {
            private static Task _worker;
            private static readonly object _lock = new object();
            public static void SetState(GyroCheckState state)
            {
                _gyroState = state;
            }
            /// <summary>
            /// 启动陀螺仪后台监控（每 1 分钟检测一次）
            /// </summary>
            public static void Start(
                string comName)
            {
                lock (_lock)
                {
                    // 防止重复启动
                    if (_worker != null && !_worker.IsCompleted)
                        return;

                    _worker = Task.Run(() =>
                    {
                        RunLoop(comName);
                    });
                }
            }

            /// <summary>
            /// 后台循环主体
            /// </summary>
            private static void RunLoop(string comName)
            {
                var guard = ComPortGuard.Get(comName);

                while (!_quitFlag)
                {
                    // ① 检测条件不满足，直接空转
                    if (!IsGyroCheckAllowed())
                    {
                        SleepWithQuitCheck(500); // 低频轮询
                        continue;
                    }

                    bool acquired = false;

                    try
                    {
                        // ② 低优先级尝试获取串口
                        acquired = guard.Wait(0);
                        if (!acquired)
                        {
                            VAR.msg.AddMsg(
                                Msg.EM_MSGTYPE.DBG,
                                "[Gyro] COM busy, yield");
                        }
                        else
                        {
                            using (var comm = new SerialCommander(comName))
                            {
                                AutoInspectionParameter
                                    .CheckGyroscopeJitter(comm, comName);
                            }
                        }
                    }
                    finally
                    {
                        if (acquired)
                            guard.Release();
                    }

                    // ③ 产线要求：4 秒周期
                    SleepWithQuitCheck(4_000);
                }
            }




            /// <summary>
            /// 带退出检查的 Sleep（避免 Thread.Sleep 卡死）
            /// </summary>
            private static bool SleepWithQuitCheck(
                int totalMs)
            {
                int step = 200; // 200ms 检查一次
                int elapsed = 0;

                while (elapsed < totalMs)
                {
                    if (_quitFlag)
                        return false;

                    Thread.Sleep(step);
                    elapsed += step;
                }

                return true;
            }
            public static void SyncQuitFlag(bool bquit)
            {
                _quitFlag = bquit;
            }
            public static void Stop()
            {
                _quitFlag = true;
            }
        }

        void RunTest()
        {
            LightBox lb = null;
            EM_RES res = EM_RES.OK;

            try
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 测试线程开始", disc) : string.Format("{0} Test thread start!        ({0} 测试线程开始)", disc));
                Swtime.Restart();
                Swtime.Start();
                while (true)
                {
                    brun = true;
                    while (COM.Cannottest && waitopen && !bquit)
                    {
                        Thread.Sleep(100);
                        Application.DoEvents();
                    }

                    //获取当前转盘位置
                    Turntable.EM_STA pos = Turntable.GetWSSta(num);
                    switch (pos)
                    {
                        default:
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 测试时，转盘状态未知!", disc) : string.Format("when {0} is testing,turntable status unknown!       ({0} 测试时，转盘状态未知!)", disc));
                            break;
                        //上料位
                        case Turntable.EM_STA.POS0:
                            lb = null;
                            break;
                        //左光箱
                        case Turntable.EM_STA.POS1:
                            bUpDnStart = false;
                            bOnUpDnPos = false;
                            bUpDnPosGoOnTest = false;
                            lb = COM.LeftLightBox;
                            if (PT_SET.afcclose)
                            {
                                int n = 0;
                                Task tsk = new Task(() =>
                                {
                                    while (true)
                                    {

                                        Thread.Sleep(100);
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到AFC面，关闭吹气", disc));
                                        this.gpio_out_gz_wind.SetOff();
                                        n++;
                                        if (n >= PT_SET.afcclosetime * 10)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 关闭时间到，打开吹气", disc));
                                            this.gpio_out_gz_wind.SetOn();
                                            break;
                                        }
                                    }

                                });
                                tsk.Start();

                            }

                            break;
                        //OTP光箱
                        case Turntable.EM_STA.POS2:
                            bUpDnStart = false;
                            bOnUpDnPos = false;
                            bUpDnPosGoOnTest = false;
                            lb = COM.OTPLightBox;
                            if (PT_SET.afcclose)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到OTP光箱，打开吹气", disc));
                                this.gpio_out_gz_wind.SetOn();
                            }
                            if (PT_SET.otpclose)
                            {
                                int n = 0;
                                Task tsk = new Task(() =>
                                {
                                    while (true)
                                    {

                                        Thread.Sleep(100);
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到OTP面，关闭吹气", disc));
                                        this.gpio_out_gz_wind.SetOff();
                                        n++;
                                        if (n >= PT_SET.otpclosetime * 10)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 关闭时间到，打开吹气", disc));
                                            this.gpio_out_gz_wind.SetOn();
                                            break;
                                        }
                                    }

                                });
                                tsk.Start();

                            }
                            break;
                        //右光箱
                        case Turntable.EM_STA.POS3:
                            bUpDnStart = false;
                            bOnUpDnPos = false;
                            bUpDnPosGoOnTest = false;
                            lb = COM.RightLightBox;
                            if (PT_SET.otpclose)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到右光箱，打开吹气", disc));
                                this.gpio_out_gz_wind.SetOn();
                            }
                            break;
                    }
                    if (!PT_SET.LbEn && lb != COM.OTPLightBox)
                    {
                        lb = COM.LightBox;
                    }

                    string ip1 = list_md[0].PC_IP;
                    string ip2 = list_md[10].PC_IP;
                    UI.Class.PUCFFactoryMode ProductMod1;
                    UI.Class.PUCFFactoryMode ProductMod2;
                    int Temper1;
                    int Temper2;
                    if (PT_SET.AutoChkEn && !VAR.ClearMt && VAR.isAutoChkMode && num == (int)PT_SET.AutoChkSelectWs)
                    {
                        var bok = GetPcChkMod(out ProductMod1, out ProductMod2, out Temper1, out Temper2); // 获取点检模式
                        if (!bok)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}WS内获取点检模式失败");
                            return;
                        }
                        int[] temp1List = new int[16], temp2List = new int[16];
                        bok = GetPcChkInfo(out var pUCFFac1, out var pUCFFac2, temp1List, temp2List);  // 获取点检要求
                        if (!bok)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}WS内获取点检信息失败");
                            return;
                        }
                        bok = AutoChkCntOkShow(); // 判断点检是否ok
                        if (!bok)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}WS内获取点检信息失败");
                            return;
                        }
                        if (!VAR.isAutoChkMode)
                        {
                            continue;
                        }
                        bok = PcIsChkModShow(ProductMod1, ProductMod2); // 判断点检模式
                        if (!bok)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}WS内PcIsChkModShow弹窗后停机");
                            return;
                        }

                        var bOkTemper = AutoChkTemperShow(Temper1, Temper2, AutoChkCnt); // 判断色温是否对
                        if (!bOkTemper)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}WS内色温比对失败");
                            return;
                        }
                    }


                    //获取测试软件状态
                    List<TestPC.InfoData> list_info = new List<TestPC.InfoData>();
                    res = GetTestInfo(ref list_info, Demo);


                    ////上下料
                    //if (posInf == Turntable.EM_STA.POS0)
                    //{
                    //if (TestStatus == EM_TEST_STA.UNTEST)
                    //{
                    //    res = SetupForTest(ref VAR.gsys_set.bquit);
                    //    if (res != EM_RES.OK) break;
                    //    TestStatus = EM_TEST_STA.NEXT;
                    //    Status = EM_STA.TEST;
                    //}
                    //else if (TestStatus == EM_TEST_STA.COMPLETED)
                    //{
                    //    res = SetupForFeed(ref VAR.gsys_set.bquit);
                    //    if (res != EM_RES.OK) break;
                    //    Status = EM_STA.DOWNLOAD;
                    //    break;
                    //}
                    //break;                   
                    //}

                    //if (VAR.gsys_set.bquit||bquit)
                    //{
                    //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0} 取消", disc): string.Format("{0} cancel!        ({0} 取消)", disc));
                    //    res = EM_RES.QUIT;
                    //    break;
                    //}

                    if (IsFirst)
                    {
                        if (lb != null && lb != COM.LightBox)
                        {
                            res = lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                            if (res != EM_RES.OK) break;
                        }

                        //关掉当前测试                    
                        res = NextTest(-1, Demo);
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("关闭当前测试") : "Close current test.   (关闭当前测试)");
                        if (res != EM_RES.OK)
                        {
                            TestStatus = EM_TEST_STA.ERROR;
                            break;
                        }
                        //Thread.Sleep(1000);
                        ////复位当前测试  
                        //res = NextTest(0);
                        //if (res != EM_RES.OK)
                        //{
                        //    TestStatus = EM_TEST_STA.ERROR;
                        //    break;
                        //}
                        if (!Demo)
                        {
                            int _sta = 0;
                            WaitTestResult(ref _sta, PT_SET.TestTime);
                        }
                        res = EM_RES.OK;
                        Status = EM_STA.REDAY;
                        TestStatus = EM_TEST_STA.COMPLETED;
                        foreach (MdDat md in list_md)
                        {
                            md.res = 1;
                        }



                        IsFirst = false;
                        break;
                    }
                    DateTime startTime = DateTime.Now;
                    //开始测试,只有在上料位置才开图
                    if (pos == Turntable.EM_STA.POS0 && PT_SET.turnon || (pos == Turntable.EM_STA.POS1 && !PT_SET.turnon))
                    {
                        //已完成/测试错误/空料 不开图
                        if (TestStatus == EM_TEST_STA.COMPLETED || TestStatus == EM_TEST_STA.ERROR || TestStatus == EM_TEST_STA.EMPTY)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese ? string.Format("{0} 物料状态异常，不开图，测试结束！", disc) : string.Format("{0} mod status err,can't start test,test over!        ({0} 物料状态异常，不开图，测试结束！)", disc));
                            break;
                        }

                        if (!bUpDnPosGoOnTest)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 启动测试", disc) : string.Format("{0} StartTestFlow       ({0} 启动测试)", disc));
                            if (pos == Turntable.EM_STA.POS0 && PT_SET.turnon)
                            {
                                res = WaitBeforeOpenImage(ref VAR.gsys_set.bquit);
                                if (res != EM_RES.OK)
                                {
                                    Status = EM_STA.LINKERR;
                                    break;
                                }
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }

                            res = StartTestFlow(0, Demo);
                            startTime = DateTime.Now;

                            if (res != EM_RES.OK)
                            {
                                Thread.Sleep(1500);
                                res = StartTestFlow();
                                startTime = DateTime.Now;
                                if (res != EM_RES.OK)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} StartTestFlow err", disc));
                                    Status = EM_STA.LINKERR;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} bUpDnPosGoOnTest不能启动测试", disc) : string.Format("{0}bUpDnPosGoOnTest  Cannot StartTestFlow       ({0} 不能启动测试)", disc));
                        }

                    }

                    //等待测试结果
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 等待结果", disc) : string.Format("{0} wait test result        ({0} 等待结果)", disc));
                    int sta = 0;

                    Status = EM_STA.TEST;

                    if (lb == COM.LightBox)
                    {
                        TestStatus = EM_TEST_STA.NEXT;
                        break;
                    }

                    Thread.Sleep(PT_SET.OpenDly);// 开图延时等待
                    if (bUpDnStart && !PT_SET.turnon)//后开图且已经上料停止启动测试
                    {
                        res = EM_RES.OK;
                        Status = EM_STA.REDAY;
                        TestStatus = EM_TEST_STA.COMPLETED;
                        break;
                    }

                    if (bOnUpDnPos && !PT_SET.turnon)//不提前开图且还在上下料位置。停止启动测试
                    {
                        res = EM_RES.OK;
                        Status = EM_STA.REDAY;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("后开图且还在上料位置，停止启动测试,设置工站状态{0}completed", disc));
                        TestStatus = EM_TEST_STA.COMPLETED;
                        break;
                    }

                    res = WaitTestResult(ref sta, PT_SET.TestTime, Demo, lb);
                    DateTime endTime = DateTime.Now;
                    TimeSpan timeDifference = endTime - startTime;
                    double seconds = timeDifference.TotalSeconds;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("所花时间{0}", seconds) : string.Format("spend {0}", seconds));
                    if (res == EM_RES.OK && sta == 0 && seconds < 2 && !Demo)
                    {
                      
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("开图到收到结果时间过短"));

                        MT.ST_WARN st_warn = new MT.ST_WARN();
                        warning fr_warn = new warning();
                        st_warn.ok_txt = VAR.IsChinese ? "忽略" : "Go on";
                        st_warn.abort_txt = VAR.IsChinese ? "继续" : "give up";
                        st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                        st_warn.title = VAR.IsChinese ? $"提示:开图到收到结果时间过短!实际该工站的NG码为{CurWSNGCode}" : "Tip: Receieve reason too fast";
                        st_warn.msg = VAR.IsChinese ? string.Format("开图到收到结果时间过短") : string.Format("Receieve reason too fast");
                        st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.点击忽略将保留本次NG结果并继续运行!\r\n  " +
                            "2.点击继续则本次结果全置为2222，并继续!\r\n  " + "3.点击停止则NG代码置为2222!，并停止\r\n  ";
                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "开图到收到结果时间过短!" : "Receieve reason too fast", 20, true);
                        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        CurWSNGCode.Clear();
                        if (DialogResult.OK == logres)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("开图到收到结果时间过短,点击忽略"));

                        }
                        else if(DialogResult.Abort == logres)
                        {
                            foreach (WS.MdDat md in list_md)
                            {
                                md.res = 2222;
                            }
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("开图到收到结果时间过短,点击继续"));
                        }
                        else
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("开图到收到结果时间过短,点击停止"));
                            Status = EM_STA.ERR;
                            break;
                        }
                    }
                    if (res == EM_RES.PARA_ERR || res == EM_RES.QUIT)
                    {
                        //不同步等异常
                        break;
                    }
                    else if (res != EM_RES.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WaitTestResult err", disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                        Status = EM_STA.LINKERR;
                        break;
                    }

                    if (sta == 0)
                    {
                        if (!VAR.ClearMt && VAR.isAutoChkMode && num == (int)PT_SET.AutoChkSelectWs)
                        {
                            bool bAllOk = true;
                            List<MdDat> NgMdList = new List<MdDat>();
                            List<MdDat> OkMdList = new List<MdDat>();

                            var resok = GetPcChkResult(out var temp1, out var temp2);
                            if (!resok)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}获取工位点检结果失败");
                                return;
                            }
                            bAllOk = temp1 == 0 && temp2 == 0;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}测试软件判断结果{bAllOk}");
                            if (!bAllOk)
                            {
                                string NgMdNums = "";
                                foreach (WS.MdDat md in list_md)
                                {
                                    var curtempMod = ListChkTemp[AutoChkCnt];
                                    int nextChkCnt = AutoChkCnt + 1;

                                    bool NoChang = NoChangCheckModList.Contains(curtempMod);
                                    NoChang = false;
                                    if (NoChang)
                                    {
                                        if (temp1 == 0 && md.PC_IP == list_md[0].PC_IP)
                                        {
                                            if (md.res != -2)
                                            {
                                                md.res = -1;
                                            }
                                        }
                                        if (temp2 == 0 && md.PC_IP == list_md[9].PC_IP)
                                        {
                                            if (md.res != -2)
                                            {
                                                md.res = -1;
                                            }
                                        }
                                    }

                                    if (!md.benable) continue;
                                    if (md.res == 0)
                                    {
                                        if (!NoChang)
                                            md.bAutoChkOk = true;
                                    }
                                    else
                                    {
                                        if (!NoChang)
                                        {
                                            if (temp1 == 0 && md.PC_IP == list_md[0].PC_IP)
                                            {
                                                md.bAutoChkOk = true;
                                            }
                                            if (temp2 == 0 && md.PC_IP == list_md[9].PC_IP)
                                            {
                                                md.bAutoChkOk = true;
                                            }
                                        }
                                        if (!md.bAutoChkOk)
                                            NgMdNums += md.Num + "_";
                                    }
                                }
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + "自动点检失败模组编号：" + NgMdNums);
                            }
                            else
                            {
                                var curtempMod = ListChkTemp[AutoChkCnt];
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"自动点检一次成功色温{curtempMod}，计数{AutoChkCnt}");
                                AutoChkCnt++;
                                //var NextTempMod = ListChkTemp[AutoChkCnt];
                                //if (NoChangCheckModList.Contains(curtempMod) && NoChangCheckModList.Contains(NextTempMod))
                                //{
                                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"当前点检色温{curtempMod}下次点检色温{NextTempMod}不需要上料");
                                //    // 不需要更换物料,非空的物料改为待测
                                //    foreach (var md in list_md)
                                //    {
                                //        if (md.res != -2)
                                //        {
                                //            md.res = -1;
                                //        }
                                //    }
                                //}

                                foreach (WS.MdDat md in list_md)
                                    md.bAutoChkOk = false;
                                var bok = AutoChkCntOkShow();
                                if (!bok) return;
                                if (VAR.isAutoChkMode)
                                {
                                    var bSetOk = SetPcAutoChk(AutoChkCnt);
                                    if (!bSetOk)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"当前点检色温{AutoChkCnt}下次点检色温异常");
                                        return;
                                    }
                                }
                            }

                        }
                        else
                        {


                            if (PT_SET.bGrrFlow || VAR.gsys_set.isChkMode)
                            {
                                if (PT_SET.bGrrFlow) GrrTestCnt++;
                                if (VAR.gsys_set.isChkMode) ChkCnt++;
                            }
                            //夹具使用计数
                            if (num == 0 && PT_SET.FixtrueMT != 0) COUNT_DATA.CurEquipmentMT++;

                            if (!VAR.gsys_set.isChkMode)
                            {
                                //保存结果
                                int t = System.Environment.TickCount - tmr;
                                foreach (MdDat md in list_md)
                                {
                                    md.ct = t;
                                    md.test_idx = 0;
                                }
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 存储结果", disc) : string.Format("{0} save result       ({0} 存储结果)", disc));
                                //gy0123夹具数据记录
                                JigProductionCnt(ref list_md);
                                SQLData.TestDataAdd(this);
                                CurOutProductTime = DateTime.Now;

                                //foreach (WS.MdDat md in list_md)
                                //{

                                //}
                            }
                        }

                        if (lb != null && lb != COM.LightBox)
                        {
                            ;
                            //if (PT_SET.otpclose &&lb == COM.OTPLightBox)
                            //{
                            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到OTP面，关闭吹气", disc));
                            //    this.gpio_out_gz_wind.SetOff();
                            //}
                            //else
                            //{
                            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 转到非OTP面，打开吹气", disc));
                            //    this.gpio_out_gz_wind.SetOn();
                            //}

                            GyroscopeMonitor.SetState(GyroCheckState.Stop);
                            res = lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                            if (res != EM_RES.OK) break;

      
                            if (res != EM_RES.OK)
                                {
                                    TestStatus = EM_TEST_STA.ERROR;
                                    break;
                                }


                            if (Iserrfirstbox && !VAR.gsys_set.isChkMode && !VAR.isAutoChkMode)
                            {

                                Iserrfirstbox = false;
                                //同排工站同样NG是否超范围
                                if (PT_SET.bSameRowNGTip && VAR.bSameNGTip_Temp)
                                {
                                    List<WS.MdDat> lmds = new List<WS.MdDat>();
                                    WS.MdDat[] _md = new WS.MdDat[2];
                                    int[] num = new int[2] { 0, 0 };
                                    foreach (List<WS.MdDat> list_md in list_list_md)
                                    {
                                        foreach (WS.MdDat md in list_md)
                                        {
                                            lmds.Clear();
                                            if (md.benable && md.res > 1)
                                            {
                                                lmds = list_md.FindAll(delegate (WS.MdDat a) { return md.res > 1 && a.res == md.res || !a.benable; });
                                                if (lmds.Count >= PT_SET.SameRowNGTipCnt && lmds.Count > num[list_list_md.IndexOf(list_md)])
                                                {
                                                    _md[list_list_md.IndexOf(list_md)] = md;
                                                    num[list_list_md.IndexOf(list_md)] = lmds.Count;
                                                }
                                            }
                                        }
                                    }

                                    if (num[0] > 0 || num[1] > 0)
                                    {
                                        string[] ngstr = new string[2];
                                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "工站同排异常!" : "Same row NG", 20, true, ErrCode: ShowErrMsg.WsRow1SameErr);
                                        MT.ST_WARN st_warn = new MT.ST_WARN();
                                        warning fr_warn = new warning();//增加语言
                                        st_warn.ok_txt = MultiLanguage.TxtSelct("继续运行", "Keep running", "Tiếp tục chạy");
                                        st_warn.ws = null;
                                        st_warn.title = MultiLanguage.TxtSelct("提示:工站同排异常!", "Tip: The station is abnormal in the same row!", "Mẹo: Trạm bất thường trong cùng một hàng!");
                                        if (num[0] > 0)
                                            ngstr[0] = MultiLanguage.TxtSelct(
                                                $"第一排(1-8工位):NG数量(加上屏蔽工位):{num[0]},NG代码:{_md[0].res}!",
                                                $"First row (1-8 stations): NG quantity (plus shielding stations): {num[0]}, NG code: {_md[0].res}",
                                                $"Hàng đầu tiên (1-8 trạm): Số lượng NG (cộng với các trạm che chắn): {num[0]}, Mã NG: {_md[0].res}");
                                        else
                                            ngstr[0] = String.Empty;

                                        if (num[1] > 0)
                                            ngstr[1] = MultiLanguage.TxtSelct(
                                                $"第二排(9-16工位):NG数量(加上屏蔽工位):{num[1]},NG代码:{_md[1].res}",
                                                $"Second row (9-16 stations): NG quantity (plus shielding station): {num[1]}, NG code: {_md[1].res}!",
                                                $"Hàng thứ hai (9-16 trạm): Số lượng NG (cộng với trạm che chắn): {num[1]}, Mã NG: {_md[1].res}!");
                                        else
                                            ngstr[1] = String.Empty;

                                        st_warn.msg = MultiLanguage.TxtSelct(
                                            $"{disc}同排工位连续出现相同NG达到设定数量[{PT_SET.SameRowNGTipCnt}]!{ngstr[0]}{ngstr[1]}",
                                            $"{disc} The same NG appears continuously in the same row of stations to reach the set number [{PT_SET.SameRowNGTipCnt}]! {ngstr[0]}{ngstr[1]}",
                                            $"{disc} Cùng một NG xuất hiện liên tục trong cùng một hàng trạm để đạt đến số đã đặt [{PT_SET.SameRowNGTipCnt}]! {ngstr[0]}{ngstr[1]}");

                                        st_warn.lb_msg = MultiLanguage.TxtSelct(
                                            $"提示: {st_warn.msg}请确认!\r\n" +
                                            $"1.按'继续运行'键则继续运行!\r\n" +
                                            $"2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!",

                                            $"Tip:{st_warn.msg} Please confirm!\r\n" +
                                            $"1.Press 'Keep running' to keep running!\r\n" +
                                            $"2.If you need to confirm the problem, press the 'Stop Running' button to exit the operation." +
                                            $"After the ready in the upper left corner of the interface, press the 'Run' button.!",

                                            $"Mẹo: {st_warn.msg} Vui lòng xác nhận!\r\n" +
                                            $"1.Nhấn 'Tiếp tục chạy' để tiếp tục chạy!\r\n" +
                                            $"2.Nếu bạn cần xác nhận sự cố, hãy nhấn nút 'Dừng Chạy' để thoát khỏi hoạt động." +
                                            $"Sau khi đã sẵn sàng ở góc trên bên trái của giao diện, nhấn nút 'Chạy'.");
                                        DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                    }
                                }
                            }
                            //复位
                            //
                            GyroscopeMonitor.SetState(GyroCheckState.Running);
                            res = NextTest(sta, Demo);
                            if (res != EM_RES.OK)
                            {
                                TestStatus = EM_TEST_STA.ERROR;
                                break;
                            }
                            res = EM_RES.OK;
                            Status = EM_STA.REDAY;
                            bResultWaitUnload = true;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 最终结果已返回，等待下料消费", disc));
                            if (bUpDnPosGoOnTest)
                            {
                                bUpDnAddTestWaitUnload = true;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} 401附加测试完成，等待下料消费", disc));
                            }
                            bUpDnPosGoOnTest = false;
                            TestStatus = EM_TEST_STA.COMPLETED;
                            break;
                        }
                        else
                        {
                            if (lb == COM.LightBox)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? "本站测试完成" : "Test completed on this site      (本站测试完成)");

                                //本站测试完成
                                TestStatus = EM_TEST_STA.NEXT;
                                Status = EM_STA.TEST;
                                res = EM_RES.OK;
                                break;
                            }
                            if (VAR.gsys_set.bquit || bquit)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel!        ({0} 取消)", disc));
                                res = EM_RES.QUIT;
                                break;
                            }

                            if (lb != null)
                            {
                                if (sta == 401 && PT_SET.bUpDnAddTest)
                                {
                                    bUpDnPosGoOnTest = true;
                                }
                                //光箱定位
                                String str1, str2;

                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} {1}定位{2}", disc, lb.disc, sta) : string.Format("{0}  {1} move {3}         ({0} {2}定位{3})", disc, lb.english_disc, lb.disc, sta));
                                //if(CompareData(sta.ToString()))
                                //{
                                //    Swtimemode.Stop();
                                //    var times = Swtimemode.ElapsedMilliseconds;
                                //    str2 = "当前工站：" + disc + "当前光箱：" + lb.disc + "测试项序号：" + sta + "测试用时：" + times;
                                //    Utility.WriteStrToCSVPre(str2);
                                //    Swtimemode.Restart();
                                //}
                                try
                                {
                                    res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);
                                    if (res == EM_RES.END)
                                    {
                                        res = lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                                        if (res != EM_RES.OK) break;

                                        //本站测试完成
                                        TestStatus = EM_TEST_STA.NEXT;
                                        Status = EM_STA.TEST;
                                        res = EM_RES.OK;
                                        break;
                                    }
                                    if (res != EM_RES.OK) break;
                                }
                                catch (Exception ex)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}", ex.Message));
                                }
                            }
                            else
                            {
                                if (sta == 401 && PT_SET.bUpDnAddTest)
                                {
                                    //转到水平
                                    res = TurnToFeed(ref bquit);
                                    if (res != EM_RES.OK)
                                    {
                                        bquit = true; break;
                                    }
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "当前工站" + disc + "收到反转代码，反转后执行测试");

                                    //通知测试
                                    if (VAR.gsys_set.bquit || bquit)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel       ({0} 取消)", disc));
                                        res = EM_RES.QUIT;
                                        break;
                                    }

                                    Status = EM_STA.TEST;
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试", disc) : string.Format("{0}  notice test!        ({0} 通知测试)", disc));
                                    res = NextTest(sta, Demo);
                                    if (res != EM_RES.OK)
                                    {
                                        res = NextTest(sta, Demo);
                                        if (res != EM_RES.OK)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("通知测试软件失败设置工站状态{0}error", disc));
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", disc) : string.Format("{0} ERROR: notice test!         ({0} 通知测试出错!)", disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                                            TestStatus = EM_TEST_STA.ERROR;
                                            break;
                                        }

                                    }

                                    continue;
                                }

                                if (sta < 401 && PT_SET.bDelayTest)
                                {
                                    //本站测试完成
                                    TestStatus = EM_TEST_STA.NEXT;
                                    Status = EM_STA.TEST;
                                    res = EM_RES.OK;
                                    break;
                                }
                                else break;
                            }
                            //通知测试
                            if (VAR.gsys_set.bquit || bquit)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel       ({0} 取消)", disc));
                                res = EM_RES.QUIT;
                                break;
                            }

                            Status = EM_STA.TEST;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试", disc) : string.Format("{0}  notice test!        ({0} 通知测试)", disc));
                            res = NextTest(sta, Demo);
                            if (res != EM_RES.OK)
                            {
                                res = NextTest(sta);
                                if (res != EM_RES.OK)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", disc) : string.Format("{0} ERROR: notice test!         ({0} 通知测试出错!)", disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                                    TestStatus = EM_TEST_STA.ERROR;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lb == COM.LightBox)
                        {

                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? "本站测试完成" : "Test completed on this site      (本站测试完成)");

                            //本站测试完成
                            TestStatus = EM_TEST_STA.NEXT;
                            Status = EM_STA.TEST;
                            res = EM_RES.OK;
                            break;
                        }
                        if (VAR.gsys_set.bquit || bquit)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel!        ({0} 取消)", disc));
                            res = EM_RES.QUIT;
                            break;
                        }

                        if (lb != null)
                        {
                            if (sta == 401 && PT_SET.bUpDnAddTest)
                            {
                                bUpDnPosGoOnTest = true;
                            }
                            //光箱定位
                            String str1, str2;

                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} {1}定位{2}", disc, lb.disc, sta) : string.Format("{0}  {1} move {3}         ({0} {2}定位{3})", disc, lb.english_disc, lb.disc, sta));
                            //if(CompareData(sta.ToString()))
                            //{
                            //    Swtimemode.Stop();
                            //    var times = Swtimemode.ElapsedMilliseconds;
                            //    str2 = "当前工站：" + disc + "当前光箱：" + lb.disc + "测试项序号：" + sta + "测试用时：" + times;
                            //    Utility.WriteStrToCSVPre(str2);
                            //    Swtimemode.Restart();
                            //}
                            if (sta == PT_SET.stanum&& AutoInspectionParameter.ifcheckontime && (num + 1 == PT_SET.CheckWs))
                            {
                                AutoInspectionParameter.ifcheckontime_enabled = true;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"sta为{sta}，下一轮进入点检");
                            }
                            if (AutoInspectionParameter.ifcheckontime_enabled && (num+1==PT_SET.CheckWs))
                            {
  

                                    res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);
                                    Thread.Sleep(1000);
                                if (res == EM_RES.END)
                                {
                                    res = lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                                    if (res != EM_RES.OK) break;

                                    //本站测试完成
                                    TestStatus = EM_TEST_STA.NEXT;
                                    Status = EM_STA.TEST;
                                    res = EM_RES.OK;
                                    break;
                                }
                                if (res != EM_RES.OK) break;
                                #region 自动点检部分

                                AutoInspectionParameter AutoCheck = new AutoInspectionParameter();


                                if (lb == COM.LeftLightBox && sta != 0)
                                {
                                    PosDef pos1 = COM.LeftLightBox.ListPos.Find(delegate (PosDef x) { return x.ID == sta; });
                                    ST_XYZ distance1 = PT_SET.ApointposAFC;
                                    ST_XYZ distance2 = PT_SET.BpointposAFC;
                                    ST_XYZ distance3 = PT_SET.CpointposAFC;
                                    if (PT_SET.AFC_distance_check_open)
                                    {
                                        if(res== EM_RES.OK)
                                        {
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "AFC面进行距离点检");
                                            res=AutoCheck.AutoCheckPass(lb, sta);
                                            if (res != EM_RES.OK)
                                            {
                                                res = EM_RES.QUIT;
                                                break;
                                            }
                                        }

                               
                                    }

                                    if (PT_SET.AFC_luxcct_check_open && pos1.Name.Contains("色温"))
                                    {
                                        Thread.Sleep(2000);
                                        AutoCheck.RunFullLightScan3x3(ref bquit, MT.AXIS_BOX_L_X1, MT.AXIS_BOX_L_Z2, lb, sta, distance1, distance2, distance3);
                                        res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);
                                    }
                                }
                                if (lb == COM.RightLightBox && sta != 0)
                                {
                                    PosDef pos1 = COM.RightLightBox.ListPos.Find(delegate (PosDef x) { return x.ID == sta; });
                                    ST_XYZ distance1 = PT_SET.ApointposDCC;
                                    ST_XYZ distance2 = PT_SET.BpointposDCC;
                                    ST_XYZ distance3 = PT_SET.CpointposDCC;
                                    if (PT_SET.DCC_distance_check_open)
                                    {
                                        if (res == EM_RES.OK)
                                        {
                                            Thread.Sleep(5000);
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "DCC面进行距离点检");
                                            res = AutoCheck.AutoCheckPass(lb, sta);
                                            if (res != EM_RES.OK)
                                            {
                                                res = EM_RES.QUIT;
                                                break;
                                            }
                                        }




                                    }
                                    if (PT_SET.DCC_luxcct_check_open && pos1.Name.Contains("色温"))
                                    {
                                        Thread.Sleep(2000);
                                        AutoCheck.RunFullLightScan3x3(ref bquit, MT.AXIS_BOX_R_X1, MT.AXIS_BOX_R_Z2, lb, sta, distance1, distance2, distance3);
                                        res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);

                                    }
                                }
                                if (lb == COM.OTPLightBox && sta != 0)
                                {
                                    PosDef pos1 = COM.OTPLightBox.ListPos.Find(delegate (PosDef x) { return x.ID == sta; });
                                    ST_XYZ distance1 = PT_SET.ApointposOTP;
                                    ST_XYZ distance2 = PT_SET.BpointposOTP;
                                    ST_XYZ distance3 = PT_SET.CpointposOTP;
                                    if (PT_SET.OTP_distance_check_open)
                                    {
                                        if (res == EM_RES.OK)
                                        {
                                            Thread.Sleep(5000);
                                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "OTP面进行距离点检");
                                            res=AutoCheck.AutoCheckPass(lb, sta);
                                            if (res != EM_RES.OK)
                                            {
                                                res = EM_RES.QUIT;
                                                break;
                                            }
                                        }
         
                                      
                                    }
                                    if (PT_SET.OTP_luxcct_check_open && pos1.Name.Contains("色温"))
                                    {
                                        Thread.Sleep(2000);
                                        AutoCheck.RunFullLightScan3x3(ref bquit, null, MT.AXIS_BOX_OTP_Z, lb, sta, distance1, distance2, distance3);
                                        res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);
                                    }
                                }

                                #endregion
                                AutoInspectionParameter.ifcheck = true;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "完成单次点检");

                            }
                            else
                            {
                                // 主流程里

                                GyroscopeMonitor.Start(PT_SET.COM_1);


                                // 每次主循环同步一次
                                GyroscopeMonitor.SyncQuitFlag(bquit);
                                GyroscopeMonitor.SetState(GyroCheckState.Stop);
                                res = lb.MoveTo(ref VAR.gsys_set.bquit, sta, num);
                         
      
                                if (res == EM_RES.END)
                                {
                                    res = lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                                    if (res != EM_RES.OK) break;
                                    GyroscopeMonitor.SetState(GyroCheckState.Stop);
                                    //本站测试完成
                                    TestStatus = EM_TEST_STA.NEXT;
                                    Status = EM_STA.TEST;
                                    res = EM_RES.OK;
                                    break;
                                }
                                if (res != EM_RES.OK) break;
                            }
                        }
                        else
                        {
                            if (sta == 401 && PT_SET.bUpDnAddTest)
                            {
                                //转到水平
                                res = TurnToFeed(ref bquit);
                                if (res != EM_RES.OK)
                                {
                                    bquit = true; break;
                                }
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "当前工站" + disc + "收到反转代码，反转后执行测试");

                                //通知测试
                                if (VAR.gsys_set.bquit || bquit)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel       ({0} 取消)", disc));
                                    res = EM_RES.QUIT;
                                    break;
                                }

                                Status = EM_STA.TEST;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试", disc) : string.Format("{0}  notice test!        ({0} 通知测试)", disc));
                                res = NextTest(sta, Demo);
                                if (res != EM_RES.OK)
                                {
                                    res = NextTest(sta, Demo);
                                    if (res != EM_RES.OK)
                                    {
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("通知测试软件失败设置工站状态{0}error", disc));
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", disc) : string.Format("{0} ERROR: notice test!         ({0} 通知测试出错!)", disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                                        TestStatus = EM_TEST_STA.ERROR;
                                        break;
                                    }

                                }

                                continue;
                            }

                            if (sta < 401 && PT_SET.bDelayTest)
                            {
                                //本站测试完成
                                TestStatus = EM_TEST_STA.NEXT;
                                Status = EM_STA.TEST;
                                res = EM_RES.OK;
                                break;
                            }
                            else break;
                        }
                        //通知测试
                        if (VAR.gsys_set.bquit || bquit)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 取消", disc) : string.Format("{0} cancel       ({0} 取消)", disc));
                            res = EM_RES.QUIT;
                            break;
                        }
                        GyroscopeMonitor.SetState(GyroCheckState.Running);
                        Status = EM_STA.TEST;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 通知测试", disc) : string.Format("{0}  notice test!        ({0} 通知测试)", disc));
                        res = NextTest(sta, Demo);
                        if (res != EM_RES.OK)
                        {
                            res = NextTest(sta);
                            if (res != EM_RES.OK)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 通知测试出错!", disc) : string.Format("{0} ERROR: notice test!         ({0} 通知测试出错!)", disc), DReport.EmErrCode.TestFailed, (int)DReport.EmHareware.TurnTable + num + 1, ERR_ALM.EmErrItem.TestAbnormal);
                                TestStatus = EM_TEST_STA.ERROR;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 测试线程 {1} 结束,{2}", disc, res != EM_RES.OK ? "异常" : "正常", ex.Message) : string.Format("{0} test thread {1} end,{3}       ({0} 测试线程 {2} 结束,{3})", disc, res != EM_RES.OK ? "ERR" : "NORMAL", res != EM_RES.OK ? "异常" : "正常", ex.Message));
                res = EM_RES.ERR;
            }
            finally
            {
                //光箱归位
                if (lb != null && lb != COM.LightBox) lb.MoveTo(ref VAR.gsys_set.bquit, 0);
                //NextTest(-1, Demo);
                if (res != EM_RES.OK)
                {
                    TestStatus = EM_TEST_STA.ERROR;
                    Status = EM_STA.ERR;
                }

                brun = false;
                Swtime.Stop();
                String str1, str2;
                int lbid = -1;//定义光箱ID
                int wsid = num + 1;
                str2 = wsid + "," + lb.disc + "," + Swtime.ElapsedMilliseconds + ",";
                Utility.WriteStrToCSVPre(str2);
                if (lb.disc.Contains("左"))
                {
                    lbid = 1;
                    PT_SET.lefttime = (Swtime.ElapsedMilliseconds/1000);
                }
                else if (lb.disc.Contains("右"))
                {
                    lbid = 2;
                    PT_SET.righttime = (Swtime.ElapsedMilliseconds / 1000);
                }
                else
                {
                    lbid = 3;
                    PT_SET.otptime= (Swtime.ElapsedMilliseconds / 1000);
                }
                var timetemp = (Swtime.ElapsedMilliseconds / 1000);
                SQLData.TestDataAddTime(wsid, lbid, timetemp);
               // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0} 测试线程 {1} 结束", disc, res != EM_RES.OK ? "异常" : "正常") : string.Format("{0} test thread {1} end          ({0} 测试线程 {2} 结束)", disc, res != EM_RES.OK ? "ERR" : "NORMAL", res != EM_RES.OK ? "异常" : "正常"));
            }
        }
        #endregion

        /// <summary>
        /// 夹具生产数据统计gy0123
        /// </summary>
        /// <param name="Mlist"></param>
        /// <returns></returns>
        private void JigProductionCnt(ref List<MdDat> Mlist)
        {
            foreach (var md in Mlist)
            {
                if (!md.benable) continue;//屏蔽的夹具不统计
                switch (md.res)
                {
                    case (int)Md_RES.OK:
                        md.cnt_ok++;
                        break;
                    case (int)Md_RES.NG_IMAGE:
                        md.cnt_ng_image++;
                        break;
                    case (int)Md_RES.NG_EXPOSURE:
                        md.cnt_ng_exposure++;
                        break;
                    case (int)Md_RES.NG_IIC:
                        md.cnt_ng_iic++;
                        break;
                    case (int)Md_RES.NG_IMAGE2:
                        md.cnt_ng_image++;
                        break;
                    case (int)Md_RES.NG_MISS_PIX:
                        md.cnt_ng_miss_pix++;
                        break;
                    case (int)Md_RES.NG_OS:
                        md.cnt_ng_OS++;
                        break;
                    case -1://待测
                        break;
                    case -2://空料
                        break;
                    default:
                        md.cnt_ng_other++;
                        break;
                }
            }
            SaveCfg();//保存数据

        }

        #region 自动点检
        /// <summary>
        /// 点检模组色温要求列表
        /// </summary>
        public static List<int> ListChkTemp = new List<int>();
        //自动点检测试次数
        public static int AutoChkCnt;
        public static int TempAutoChkCnt;
        private bool Pc1Enable => (list_md.Find(s => s.benable && (s.PC_IP == list_md[0].PC_IP))) != null;

        private bool Pc2Enable => (list_md.Find(s => s.benable && (s.PC_IP == list_md[9].PC_IP))) != null;

        /// <summary>
        /// 判断自动点检次数，并弹窗提示是否重新点检
        /// </summary>
        /// <param name="ip1"></param>
        /// <param name="ip2"></param>
        /// <returns></returns>
        public bool AutoChkCntOkShow()
        {
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;
            if (AutoChkCnt >= ListChkTemp.Count)
            {
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "自动点检提示" : "AutoChkShowmsg", 0);
                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.cancle_txt = VAR.IsChinese ? "重新点检" : "Keep running";
                warn.ok_txt = VAR.IsChinese ? "停止点检" : "Stop running";
                warn.ws = null;
                warn.title = VAR.IsChinese ? "提示:自动点检" : "Tip: Data File  Err";
                warn.msg = "当前点检次数达到要求，是否继续点检或停止点检开始清料!";
                warn.lb_msg = warn.msg + "请确认!\r\n  1.如果继续点检,请按重新点检!\r\n  " +
                                              "2.如果停止点检开始清料请按停止点检!";
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                if (DialogResult.OK == logres)
                {
                    // 停止点检
                    SetPcPrductMod();
                    int errid = 111;
                    foreach (var md in list_md)
                    {
                        md.bAutoChkOk = false;
                        if (md.benable)
                        {
                            md.benable = true;//设置_enbale
                        }
                        if (md.res != -2)//空料
                        {
                            md.res = errid;
                            errid++;
                        }
                    }
                    VAR.isAutoChkMode = false;
                    ListChkTemp.Clear();
                    VAR.ClearMt = true;
                   
                }
                else
                {
                    AutoChkCnt = 0;
                    TempAutoChkCnt = -1;
                    int i = 0;
                    foreach (var md in list_md)
                    {
                        md.bAutoChkOk = false;
                        i++;
                        if (md.benable)
                            md.res = 222 + i;
                    }
                }
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);

            }
            return true;
        }
        /// <summary>
        /// 判断测试软件的色温是否正常
        /// </summary>
        /// <param name="Temper1"></param>
        /// <param name="Temper2"></param>
        /// <returns></returns>
        public bool AutoChkTemperShow(int Temper1, int Temper2, int TestCnt = 0)
        {
            bool TemperErr = false;
            int CurTemper = 0, res1 = 0, res2 = 0;
            if (ListChkTemp == null || ListChkTemp.Count < 1)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "测试软件点检色温列表为空");
                return false;
            }
            else
            {
                if (TestCnt <= ListChkTemp.Count - 1)
                {
                    if ((Pc1Enable && Temper1 != ListChkTemp[TestCnt]) || (Pc2Enable && Temper2 != ListChkTemp[TestCnt]))
                    {
                        TemperErr = true;
                        CurTemper = ListChkTemp[TestCnt];
                    }

                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "当前点检次数达到要求");
                    return false;

                }

            }

            if (TemperErr)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "测试软件色温参数异常");
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "测试软件异常!" : "Test Soft NG", 20, true, ErrCode: ShowErrMsg.WaitTestErr);
                MT.ST_WARN st_warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                st_warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                st_warn.ws = null;
                st_warn.title = VAR.IsChinese ? "点检色温参数异常" : "Tip: The  PC Test Softerware is abnormal in the ProductMod!";
                st_warn.msg = string.Format($"当前测试软件色温{Temper1}和{Temper2}参数和电控记录参数{CurTemper}不一致");
                st_warn.lb_msg = "提示:" + st_warn.msg + $"请确认!\r\n  1.按'继续运行'键则切换测试软件到色温{CurTemper}继续运行!\r\n  " +
                                 "2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!\r\n";
                DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                if (logres == DialogResult.OK)
                {
                    var bSetOk = SetPcAutoChk(TestCnt);
                    if (!bSetOk) return false;
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                }
                else
                {
                    VAR.gsys_set.bquit = true;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断测试电脑是否在生产模式，弹窗提示
        /// </summary>
        /// <param name="ProductMod1"></param>
        /// <param name="ProductMod2"></param>
        /// <returns></returns>
        public bool PcIsProductModShow(Class.PUCFFactoryMode ProductMod1, Class.PUCFFactoryMode ProductMod2)
        {
            var bPc1Err = Pc1Enable && ProductMod1 != Class.PUCFFactoryMode.PUCFFactoryMode_Product;
            var bPc2Err = Pc2Enable && ProductMod2 != Class.PUCFFactoryMode.PUCFFactoryMode_Product;
            if (bPc1Err || bPc2Err)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "测试软件在非生产模式");
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "测试软件异常!" : "Test Soft NG", 20, true, ErrCode: ShowErrMsg.WaitTestErr);
                MT.ST_WARN st_warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                st_warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                st_warn.ws = null;
                st_warn.title = VAR.IsChinese ? "测试软件模式异常" : "Tip: The  PC Test Softerware is abnormal in the ProductMod!";
                st_warn.msg = string.Format("当前在生产模式，但是测试软件在非生产模式");
                st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.按'继续运行'键则切换测试软件到生产模式继续运行!\r\n  " +
                                 "2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!\r\n";
                DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                if (logres == DialogResult.OK)
                {
                    var bSetOk = SetPcPrductMod();
                    if (!bSetOk)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "PcIsProductModShow切换测试软件到生产模式失败");
                        return false;
                    }
                    MT.OnlyOnelightON(0);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                }
                else
                {
                    return false;
                }

            }
            return true;
        }
        /// <summary>
        /// 判断测试软件是否在点检模式
        /// </summary>
        /// <param name="ProductMod1"></param>
        /// <param name="ProductMod2"></param>
        /// <returns></returns>
        public bool PcIsChkModShow(Class.PUCFFactoryMode ProductMod1, Class.PUCFFactoryMode ProductMod2)
        {
            int res1 = 0, res2 = 0;
            bool bPc1ErrShow = Pc1Enable && ProductMod1 != Class.PUCFFactoryMode.PUCFFactoryMode_Check;
            bool bPc2ErrShow = Pc2Enable && ProductMod2 != Class.PUCFFactoryMode.PUCFFactoryMode_Check;
            if (bPc1ErrShow || bPc2ErrShow)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + "测试软件在非点检模式");
                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "测试软件异常!" : "Test Soft NG", 20, true, ErrCode: ShowErrMsg.WaitTestErr);
                MT.ST_WARN st_warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                st_warn.ok_txt = VAR.IsChinese ? "继续运行" : "Keep running";
                st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                st_warn.ws = null;
                st_warn.title = VAR.IsChinese ? "测试软件模式异常" : "Tip: The  PC Test Softerware is abnormal in the ProductMod!";
                st_warn.msg = string.Format($"当前在点检模式，但是测试软件在非点检模式Pc1State={ProductMod1},Pc2State={ProductMod2}");
                st_warn.lb_msg = "提示:" + st_warn.msg + "请确认!\r\n  1.按'继续运行'键则切换测试软件到点检模式继续运行!\r\n  " +
                                 "2.如需确认问题请按'停止运行'键退出运行，待界面左上角显示就绪后再按'运行'键!\r\n";
                DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                if (logres == DialogResult.OK)
                {
                    AutoChkCnt = 0;
                    TempAutoChkCnt = -1;
                    var bSetOk = SetPcAutoChk(0);
                    if (!bSetOk) return false;
                    foreach (var md in list_md)
                    {
                        md.bAutoChkOk = false;
                    }
                    MT.OnlyOnelightON(0);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                }
                else
                {
                   
                    return false;
                }
            }
            return true;
        }
        public bool SetPcPrductMod(bool bcheckEnable = true)
        {
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;
            var curMod = Class.PUCFFactoryMode.PUCFFactoryMode_Product;
            int CurTemper = 0;
            ListChkTemp.Clear();

            int res1 = 0, res2 = 0;
            string actionname = "切换生产模式";
            if (Pc1Enable || !bcheckEnable)
            {
                var pcname = "pc1";
                res1 = UI.Class.AutoChkDLL.iPUCFTestServiceSetFactoryMode(ip1, curMod, CurTemper);
                if (res1 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"与测试软件通信{pcname}{actionname}成功");
                }

            }

            if (Pc2Enable || !bcheckEnable)
            {
                res2 = UI.Class.AutoChkDLL.iPUCFTestServiceSetFactoryMode(ip2, curMod, CurTemper);
                var pcname = "pc2";
                if (res2 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"与测试软件通信{pcname}{actionname}成功");
                }
            }
            return true;

        }
        public bool GetPcChkInfo(out int ProductMod1, out int ProductMod2, int[] Temper1, int[] Temper2, bool benableChk = true)
        {
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;
            ProductMod1 = 0; ProductMod2 = 0;
            Temper1 = new int[16];
            Temper2 = new int[16];
            int res1 = 0, res2 = 0;
            string actionname = "获取测试信息";
            if (Pc1Enable || !benableChk)
            {
                var pcname = "pc1";
                res1 = UI.Class.AutoChkDLL.iPUCFTestServiceGetAutoCheckInfo(ip1, out ProductMod1, Temper1);
                if (res1 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    if (ProductMod1 > 0)
                    {
                        string tempinfo = "";
                        WS.ListChkTemp.Clear();
                        foreach (var m in Temper1)
                        {
                            if (m > 0)
                            {
                                WS.ListChkTemp.Add(m);
                                tempinfo += m + ",";
                            }
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}{pcname}{actionname}点检次数{ProductMod1}");
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}获取{pcname}{actionname}色温列表{tempinfo}");
                    }

                }

            }

            if (Pc2Enable || !benableChk)
            {
                res2 = UI.Class.AutoChkDLL.iPUCFTestServiceGetAutoCheckInfo(ip2, out ProductMod2, Temper2);
                var pcname = "pc2";
                if (res2 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    if (ProductMod2 > 0)
                    {
                        string tempinfo = "";
                        WS.ListChkTemp.Clear();
                        foreach (var m in Temper2)
                        {
                            if (m > 0)
                            {
                                WS.ListChkTemp.Add(m);
                                tempinfo += m + ",";
                            }
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}{pcname}{actionname}点检次数{ProductMod2}");
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}{pcname}{actionname}色温列表{tempinfo}");
                    }

                }
            }
            return true;
        }
        public bool GetPcChkResult(out int Temper1, out int Temper2, bool benableChk = true)
        {
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;
            int res1 = 0, res2 = 0;
            Temper1 = 0;
            Temper2 = 0;
            string actionname = "获取测试结果";
            if (Pc1Enable || !benableChk)
            {
                var pcname = "pc1";
                res1 = UI.Class.AutoChkDLL.iPUCFTestServiceGetCheckModeResult(ip1, out Temper1);
                if (res1 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}获取{pcname}第{AutoChkCnt}{actionname}结果{Temper1 == 0}");
                }

            }

            if (Pc2Enable || !benableChk)
            {
                res2 = UI.Class.AutoChkDLL.iPUCFTestServiceGetCheckModeResult(ip2, out Temper2);
                var pcname = "pc2";
                if (res2 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}获取测试结果失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}获取{pcname}第{AutoChkCnt}次点检结果{Temper2 == 0}");
                }
            }
            return true;
        }
        public bool GetPcChkMod(out UI.Class.PUCFFactoryMode ProductMod1, out UI.Class.PUCFFactoryMode ProductMod2, out int Temper1, out int Temper2, bool bchkenable = true)
        {
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;

            ProductMod1 = Class.PUCFFactoryMode.PUCFFactoryMode_Product;
            ProductMod2 = Class.PUCFFactoryMode.PUCFFactoryMode_Product;
            int res1 = 0, res2 = 0;
            Temper1 = 0;
            Temper2 = 0;
            string actionname = "获取点检模式";
            if (Pc1Enable || !bchkenable)
            {
                var pcname = "pc1";
                res1 = UI.Class.AutoChkDLL.iPUCFTestServiceGetFactoryMode(ip1, out ProductMod1, out Temper1);
                if (res1 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    bool bchk = ProductMod1 == Class.PUCFFactoryMode.PUCFFactoryMode_Check;
                    string msg = bchk ? "点检模式" : "非点检模式";
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}{pcname}{actionname}结果{msg}");
                }

            }

            if (Pc2Enable || !bchkenable)
            {
                res2 = UI.Class.AutoChkDLL.iPUCFTestServiceGetFactoryMode(ip2, out ProductMod2, out Temper2);
                var pcname = "pc2";
                if (res2 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    bool bchk = ProductMod2 == Class.PUCFFactoryMode.PUCFFactoryMode_Check;
                    string msg = bchk ? "点检模式" : "非点检模式";
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, $"{disc}{pcname}{actionname}结果{msg}");
                }
            }

            return true;
        }
        /// <summary>
        /// 设置测试软件工作模式
        /// </summary>
        /// <param name="CurTemper"></param>
        /// <param name="bSetChk"></param>
        /// <returns></returns>
        public bool SetPcAutoChk(int TestCnt = 0, bool bSetChk = true, bool bcheckEnable = true)
        {
            int[] temp1List = new int[16], temp2List = new int[16];
            var res1 = GetPcChkInfo(out var pUCFFactoryMode1, out var pUCFFactoryMode2, temp1List, temp2List, bcheckEnable);
            if (!res1)
            {
                return false;
            }

            return mSetPcAutoChk(TestCnt, bcheckEnable);
        }
        bool mSetPcAutoChk(int TestCnt = 0, bool bcheckEnable = true)
        {
            int res1, res2;
            string ip1 = list_md[0].PC_IP;
            string ip2 = list_md[10].PC_IP;
            UI.Class.PUCFFactoryMode curMod = Class.PUCFFactoryMode.PUCFFactoryMode_Check;
            string actionname = "切换到点检模式";
            if (!VAR.isAutoChkMode)
                return true;
            int CurTemper = 0;
            if (ListChkTemp == null || ListChkTemp.Count < 1)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"mSetPcAutoChk当前色温列表数据为空{actionname}失败");
                return false;
            }
            else
            {
                if (TestCnt < ListChkTemp.Count)
                    CurTemper = ListChkTemp[TestCnt];
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"mSetPcAutoChk当前点检色温序号参数大于色温列表数量{actionname}失败");
                    return false;
                }
            }
            if (Pc1Enable || !bcheckEnable)
            {
                var pcname = "pc1";
                res1 = UI.Class.AutoChkDLL.iPUCFTestServiceSetFactoryMode(ip1, curMod, CurTemper);
                if (res1 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"与测试软件通信{pcname}{actionname}成功");
                }

            }

            if (Pc2Enable || !bcheckEnable)
            {
                res2 = UI.Class.AutoChkDLL.iPUCFTestServiceSetFactoryMode(ip2, curMod, CurTemper);
                var pcname = "pc2";
                if (res2 < 0)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, disc + $"与测试软件通信{pcname}{actionname}失败");
                    return false;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, disc + $"与测试软件通信{pcname}{actionname}成功");
                }
            }

            return true;
        }

        private string GetChkModName
        {
            get
            {
                if (AutoChkCnt < ListChkTemp.Count)
                {
                    int curtemp = ListChkTemp[AutoChkCnt];
                    switch (curtemp)
                    {
                        case 3000:
                            return "3000K色温点检";
                        case 4000:
                            return "4000K色温点检";
                        case 5000:
                            return "5000K色温点检";
                        case 10001:
                            return "AFC近焦点检";
                        case 10002:
                            return "AFC中距点检";
                        case 10003:
                            return "AFC远焦点检";
                        case 20001:
                            return "OTP光源点检数据生成";
                        case 20002:
                            return "OTP光源点检";
                    }

                }
                return "";
            }

        }
        /// <summary>
        /// 放料前提示准备好点检模组
        /// </summary>
        /// <param name="bShowMsg"></param>
        /// <returns></returns>
        public bool PutModShowMsg(bool bShowMsg = true)
        {
            if (!VAR.isAutoChkMode)
                return true;

            if (AutoChkCnt < ListChkTemp.Count && bShowMsg)
            {
                if(GetChkModName.Length<2)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}PutModShowMsg当前点检参数异常GetChkModName{AutoChkCnt}信息异常实际色温{ListChkTemp[AutoChkCnt]}");
                    return false;
                }
                MT.ST_WARN warn = new MT.ST_WARN();
               // VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "自动点检提示" : "AutoChkShowMsg", 0);
                warning fr_warn = new warning();
                warn.ok_txt = VAR.IsChinese ? "点检模式继续" : "Keep running";
                warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                warn.abort_txt = VAR.IsChinese ? "生产模式继续" : "Stop running";
                warn.ws = null;
                warn.title = VAR.IsChinese ? "提示:自动点检开始" : "Tip: Data File  Err";

                warn.msg = $"请确认上料是专用色温{GetChkModName}的点检模组!";
                warn.lb_msg = warn.msg + "请确认!\r\n  1.如果确认点检，请上料专用的点检模组,请按继续运行键!\r\n  " +
                                              "2.如果上料错误或其他确认，请停止运行，!\r\n " +
                                              "3.如果想切换生产模式运行，请按切换生产按钮";
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
                if (DialogResult.Cancel == logres)
                {
                    VAR.gsys_set.bquit = true;
                    return false;
                }
                else if (DialogResult.OK == logres)
                {
                    var bSetOk = SetPcAutoChk(AutoChkCnt);
                    if (!bSetOk)
                    {
                        VAR.gsys_set.bquit = true;
                        return false;
                    }
                    MT.OnlyOnelightON(0);
                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                }
                else
                {
                    VAR.isAutoChkMode = false;
                    var bSetOk = SetPcPrductMod();
                    if (!bSetOk)
                    {
                        bquit = true;
                        return false;
                    }

                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                }
            }else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, $"{disc}PutModShowMsg当前点检参数异常AutoChkCnt{AutoChkCnt}大于点检要求数量{ListChkTemp.Count}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 不需要更换模组的色温
        /// </summary>
        public static List<int> NoChangCheckModList => new List<int>() { 10001, 10002, 10003 };


        #endregion
    }
}
