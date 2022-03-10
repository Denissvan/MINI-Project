using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.InteropServices;
using DevReport;
using System.Data;

namespace MotionCtrl
{
    #region 基本结构

    public struct ST_XY
    {
        public double x;
        public double y;

        public ST_XY(double x = 0, double y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public ST_XY(string str)
        {
            x = 0;
            y = 0;
            FrString(str);
        }

        public ST_XY clone()
        {
            ST_XY xy = new ST_XY();
            xy.x = x;
            xy.y = y;
            return xy;
        }

        public ST_XYA ToXYA(double a = 0)
        {
            ST_XYA xya = new ST_XYA();
            xya.x = x;
            xya.y = y;
            xya.a = a;
            return xya;
        }

        public ST_XYZ ToXYZ(double z = 0)
        {
            ST_XYZ xyz = new ST_XYZ();
            xyz.x = x;
            xyz.y = y;
            xyz.z = z;
            return xyz;
        }

        public ST_XYZA ToXYZA(double z = 0, double a = 0)
        {
            ST_XYZA xyza = new ST_XYZA();
            xyza.x = x;
            xyza.y = y;
            xyza.z = z;
            xyza.a = a;
            return xyza;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Y{1:F3} ", x, y);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'X', ref x)) n++;
            if (Utility.GetDoubleFrStr(str, 'Y', ref y)) n++;
            return n;
        }

        public static ST_XY operator +(ST_XY lhs, ST_XY rhs)
        {
            return new ST_XY(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static ST_XY operator -(ST_XY lhs, ST_XY rhs)
        {
            return new ST_XY(lhs.x - rhs.x, lhs.y - rhs.y);
        }
    }

    public struct ST_XZ
    {
        public double x;
        public double z;

        public ST_XZ(double x = 0, double z = 0)
        {
            this.z = z;
            this.x = x;
        }

        public ST_XZ(string str)
        {
            x = 0;
            z = 0;
            FrString(str);
        }

        public ST_XZ clone()
        {
            ST_XZ xz = new ST_XZ();
            xz.x = x;
            xz.z = z;
            return xz;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Z{1:F3} ", x, z);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'X', ref x)) n++;
            if (Utility.GetDoubleFrStr(str, 'Z', ref z)) n++;
            return n;
        }
    }

    public struct ST_YZ
    {
        public double y;
        public double z;

        public ST_YZ(double y = 0, double z = 0)
        {
            this.z = z;
            this.y = y;
        }

        public ST_YZ(string str)
        {
            z = 0;
            y = 0;
            FrString(str);
        }

        public ST_YZ clone()
        {
            ST_YZ yz = new ST_YZ();
            yz.y = y;
            yz.z = z;
            return yz;
        }

        public override string ToString()
        {
            return string.Format(" Y{0:F3},Z{1:F3} ", y, z);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'Z', ref z)) n++;
            if (Utility.GetDoubleFrStr(str, 'Y', ref y)) n++;
            return n;
        }
    }

    public struct ST_XYZ
    {
        public double x;
        public double y;
        public double z;

        public ST_XYZ(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ST_XYZ(string str)
        {
            x = 0;
            y = 0;
            z = 0;
            FrString(str);
        }

        public ST_XYZ clone()
        {
            ST_XYZ xyz = new ST_XYZ();
            xyz.x = x;
            xyz.y = y;
            xyz.z = z;
            return xyz;
        }

        public ST_XY ToXY()
        {
            ST_XY xy = new ST_XY();
            xy.x = x;
            xy.y = y;
            return xy;
        }

        public ST_XYA ToXYA()
        {
            ST_XYA xya = new ST_XYA();
            xya.x = x;
            xya.y = y;
            xya.a = z;
            return xya;
        }

        public ST_XYZA ToXYZA(double a = 0)
        {
            ST_XYZA xyza = new ST_XYZA();
            xyza.x = x;
            xyza.y = y;
            xyza.z = z;
            xyza.a = a;
            return xyza;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Y{1:F3},Z{2:F3} ", x, y, z);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'X', ref x)) n++;
            if (Utility.GetDoubleFrStr(str, 'Y', ref y)) n++;
            if (Utility.GetDoubleFrStr(str, 'Z', ref z)) n++;
            return n;
        }

        public static ST_XYZ operator +(ST_XYZ lhs, ST_XYZ rhs)
        {
            return new ST_XYZ(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static ST_XYZ operator -(ST_XYZ lhs, ST_XYZ rhs)
        {
            return new ST_XYZ(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
    }

    public struct ST_XYA
    {
        public double x;
        public double y;
        public double a;

        public ST_XYA(double x = 0, double y = 0, double a = 0)
        {
            this.x = x;
            this.y = y;
            this.a = a;
        }

        public ST_XYA(string str)
        {
            x = 0;
            y = 0;
            a = 0;
            FrString(str);
        }

        public ST_XYA clone()
        {
            ST_XYA xya = new ST_XYA();
            xya.x = x;
            xya.y = y;
            xya.a = a;
            return xya;
        }

        public ST_XY ToXY()
        {
            ST_XY xy = new ST_XY();
            xy.x = x;
            xy.y = y;
            return xy;
        }

        public ST_XYZ ToXYZ()
        {
            ST_XYZ xyz = new ST_XYZ();
            xyz.x = x;
            xyz.y = y;
            xyz.z = a;
            return xyz;
        }

        public ST_XYZA ToXYZA(double z = 0)
        {
            ST_XYZA xyza = new ST_XYZA();
            xyza.x = x;
            xyza.y = y;
            xyza.z = z;
            xyza.a = a;
            return xyza;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Y{1:F3},A{2:F3} ", x, y, a);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'X', ref x)) n++;
            if (Utility.GetDoubleFrStr(str, 'Y', ref y)) n++;
            if (Utility.GetDoubleFrStr(str, 'A', ref a)) n++;
            return n;
        }

        public static ST_XYA operator +(ST_XYA lhs, ST_XYA rhs)
        {
            return new ST_XYA(lhs.x + rhs.x, lhs.y + rhs.y, lhs.a + rhs.a);
        }

        public static ST_XYA operator -(ST_XYA lhs, ST_XYA rhs)
        {
            return new ST_XYA(lhs.x - rhs.x, lhs.y - rhs.y, lhs.a - rhs.a);
        }
    }

    //[TypeConvert(typeof(ExpandableObjectConverter))]
    public struct ST_XYZA
    {
        public double x;
        public double y;
        public double z;
        public double a;

        public ST_XYZA(double x = 0, double y = 0, double z = 0, double a = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.a = a;
        }

        public ST_XYZA(string str)
        {
            x = 0;
            y = 0;
            z = 0;
            a = 0;
            FrString(str);
        }

        public ST_XYZA clone()
        {
            ST_XYZA xyza = new ST_XYZA();
            xyza.x = x;
            xyza.y = y;
            xyza.z = z;
            xyza.a = a;
            return xyza;
        }

        public ST_XY ToXY()
        {
            ST_XY xy = new ST_XY();
            xy.x = x;
            xy.y = y;
            return xy;
        }

        public ST_XYZ ToXYZ()
        {
            ST_XYZ xyz = new ST_XYZ();
            xyz.x = x;
            xyz.y = y;
            xyz.z = z;
            return xyz;
        }

        public ST_XYA ToXYA()
        {
            ST_XYA xya = new ST_XYA();
            xya.x = x;
            xya.y = y;
            xya.a = a;
            return xya;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Y{1:F3},Z{2:F3},A{3:F3} ", x, y, z, a);
        }

        public int FrString(string str)
        {
            int n = 0;
            if (Utility.GetDoubleFrStr(str, 'X', ref x)) n++;
            if (Utility.GetDoubleFrStr(str, 'Y', ref y)) n++;
            if (Utility.GetDoubleFrStr(str, 'Z', ref z)) n++;
            if (Utility.GetDoubleFrStr(str, 'A', ref a)) n++;
            return n;
        }

        public static ST_XYZA operator +(ST_XYZA lhs, ST_XYZA rhs)
        {
            return new ST_XYZA(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.a + rhs.a);
        }

        public static ST_XYZA operator -(ST_XYZA lhs, ST_XYZA rhs)
        {
            return new ST_XYZA(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.a - rhs.a);
        }
        //public ST_XYZA Clone()
        //{
        //    return new ST_XYZA(x,y,z,a);
        //}

    }

    public struct ST_XYN
    {
        public double x;
        public double y;
        public int n;

        public ST_XYN(double x = 0, double y = 0, int n = 0)
        {
            this.x = x;
            this.y = y;
            this.n = n;
        }

        public ST_XYN clone()
        {
            ST_XYN xyn = new ST_XYN();
            xyn.x = x;
            xyn.y = y;
            xyn.n = n;
            return xyn;
        }

        public override string ToString()
        {
            return string.Format(" X{0:F3},Y{1:F3},N{2:F0} ", x, y, n);
        }
    }

    #endregion

    #region 设备信息

    public class MCInf
    {
        /// <summary>
        /// 供应商信息
        /// Manufacturer information
        /// </summary>
        public class MFInf
        {
            public string CompName; //供应商名称
            public string Contact; //联系人
            public string ContactMode; //联系方式
            public string MFD; //制造日期
            public string Exp; //保修截止时间
            public string ps; //备注
        }

        /// <summary>
        /// 基本参数
        /// NominalParam
        /// </summary>
        public class Param
        {
            public string Name; //设备名
            public string SN; //序列号
            public string AssetsCOde; //资产编号
            public double Lenght; //长(m)
            public double Width; //宽(m)
            public double Height; //高(m)
            public int Power; //额定功率(W)
            public int Voltage; //额定电压(V);
            public string ps; //其他备注;
        }

        /// <summary>
        /// 使用信息
        /// </summary>
        public class UserInf
        {
            public string AreaCode; //厂区编号
            public string WorkShopCode; //车间编号
            public string LineCode; // 产线编号
            public string WorkStationNum; //测试站编号
            public string SubWorkStationNum; //测试站子编号
            public string UPH; // 预定UPH
            public string Contact; //联系人
            public string ContactMode; //联系方式
            public string ps; //其他备注;
        }

        /// <summary>
        /// 设备状态
        /// </summary>
        public class Status
        {
            public string status; // 运行/停止/待料/警报/维护
            public string product; // 产品名称，规格
            public int run_time_min; //运行时间(min)
            public int stop_time_min; //停止时间(min)
            public int wait_time_min; //待料时间(min)
            public int error_time_min; //警报时间(min)
            public int maintenance_time_min; // 维护时间(min)
        }

        /// <summary>
        /// 设备维护
        /// maintenance, repair and operation
        /// </summary>
        public class MRO
        {
            public string Type; //维护类别
            public string Discription; // 实施内容
            public string Operator; // 实施人
            public string Datetime; // 实施时间
            public string Taketime; //损耗工时(h)
        }

        /// <summary>
        /// 设备功能
        /// </summary>
        public class MCFunc
        {
            public int StationNum; //工站编号
            public int TextBoxNum; // 工装编号
            public string TestFunc; // 测试项目
        }

        /// <summary>
        /// 测试结果
        /// </summary>
        public class TestResult
        {
            public int StationNum; //工站编号
            public int TextBoxNum; // 工装编号
            public string TestFunc; // 测试项目
            public int NGCode; // 测试结果代码
            public int CT; //测试用时(sec)
            public string ps; //附加结果信息
        }

        /// <summary>
        /// 测试命令
        /// </summary>
        public class TestCmd
        {
            public int StationNum; //工站编号
            public int TextBoxNum; //工装编号
            public string TestFunc; //测试项目
            public string Barcode; //模组二维码
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <returns></returns>
        public int Initrial()
        {
            //从配置文件加载参数进行初始化
            //建立一个客户端链接中控服务器，进行设备状态上传
            //建立一个服务端或客户端，进行机台间通信
            return 0;
        }

        /// <summary>
        /// 上传设备信息数据到中控
        /// </summary>
        /// <returns></returns>
        public int SendMCInfToCentraCtrl()
        {
            return 0;
        }

        /// <summary>
        /// 上传设备状态到中控
        /// </summary>
        /// <returns></returns>
        public int SendMCStatusToCentraCtrl()
        {
            return 0;
        }

        /// <summary>
        /// 上传设备维护记录到中控
        /// </summary>
        /// <returns></returns>
        public int SendMMROToCentraCtrl()
        {
            return 0;
        }

        /// <summary>
        /// 接受中控信息事件时执行
        /// </summary>
        /// <returns></returns>
        public int ReceFromCentraCtrl()
        {
            //根据命令响应
            return 0;
        }

        /// <summary>
        /// 发送数据给联机设备
        /// </summary>
        /// <returns></returns>
        public int SendTestResult()
        {
            return 0;
        }

        public int SendStatus()
        {
            return 0;
        }

        public int SendCommand()
        {
            return 0;
        }

        /// <summary>
        /// 接受联机设备信息事件时执行
        /// </summary>
        /// <returns></returns>
        public int Receve()
        {
            //根据命令响应
            return 0;
        }
    }

    #endregion

    #region 结果定义

    public enum EM_RES
    {
        //RESEULT
        [Description("成功")] OK,
        [Description("错误")] ERR,
        [Description("急停")] EMG,
        [Description("超时")] TIMEOUT,
        [Description("取消")] QUIT,
        [Description("退出")] ABORT,

        //FOLLOW
        [Description("BUSY")] BUSY,
        [Description("WAIT")] WAIT,
        [Description("NEXT")] NEXT,
        [Description("RETRY")] RETRY,
        [Description("END")] END,

        //PARAM
        [Description("参数异常")] PARA_ERR,
        [Description("参数超范围")] PARA_OUTOFRANG,

        //CAM
        [Description("相机错误")] CAM_ERR,
        [Description("相机未初始化")] CAM_INIT_ERR,
        [Description("相机连接错误")] CAM_LINK_ERR,
        [Description("相机参数错误")] CAM_PARA_ERR,
        [Description("相机任务加载错误")] CAM_TASK_LOAD_ERR,
        [Description("相机重测错误")] CAM_RECHK_ERR,
        [Description("CAM_REMARK")] CAM_REMARK,
        [Description("飞拍缺少结果")] CAM_LACKRES,
        [Description("拍照失败放回")] CAM_ERR_PLACE_BACK,
        [Description("马达拍照失败放回")] MOTOR_CAM_ERR_PLACE_BACK,

        //FUCTION
        [Description("取料错误")] PICK_ERR,
        [Description("放料错误")] PLACE_ERR,

        //MOVE
        [Description("移动错误")] MOVE_ERR,
        [Description("移动超时")] MOVE_TIMEOUT,
        [Description("参数异常")] MOVE_PARA_ERR,

        //PROTECT
        [Description("移动保护")] MOVE_PROTECT,
        [Description("安全保护")] SAFE_PROTECT,
        [Description("IO保护")] GPIO_PROTECT,
        [Description("旋转保护")] ROL_PROTECT
    }

    #endregion

    #region 运行模式

    public enum EM_SYS_MODE
    {
        NORMAL = 0x01,
        DEMO = 0x02,
        CHK = 0x04, //点检模式
        STEP = 0x10,
        ONE = 0x20,
        CONTINUE = 0x40,
    }

    #endregion

    #region 系统状态

    public enum EM_SYS_STA
    {
        UNKOWN,
        STANDBY,
        RUN,
        WARNING,
        ERR,
        PAUSE,
        RESET,
        EMG,
        REPAIR,
    }

    #endregion

    #region 报警信息

    public enum EM_ALM_STA
    {
        NOR_GREEN,
        NOR_GREEN_FLASH,
        NOR_BLUE,
        NOR_BLUE_FLASH,
        WAR_YELLOW,
        WAR_YELLOW_FLASH,
        WAR_RED,
        WAR_RED_FLASH,
    }

    #endregion

    #region 视觉数据

    public class VS_DAT
    {
        public int cs; //对应流程编号
        public int id; //目标编号
        public ST_XYZA st_cap_pos; //拍照位置
        public ST_XYA st_pix; //像素坐标
        public ST_XYA st_mm; //mm坐标
        public ST_XY st_center_ofs; //与画面中心偏差
        public ST_XYZA st_temp; //备用
        public string str_barcode; //读取二维码数据
        public bool bOK; //结果
        public bool bupdate; //更新标志

        public int ct_ms; //CT时间
        //public ICogImage outputImage;//输出图像
        //public CogGraphicCollection GraphicCollection;//输出界面绘图
        //public CogCompositeShape GraphicsPMAlignTool;//输出PMAlignTool匹配图形
        //public List<CogPointMarker> ListTopCamera;
    }

    #endregion

    #region 系统信息

    public class SYS_SET
    {
        //机型设置
        public int mc_sel;
        public int hw_ver;
        public String mc_name;
        public String mc_disc;

        //当前产品名
        public String cur_product_name;

        //使能蜂鸣器
        public bool beep_en;

        //蜂鸣器时间
        public int beep_tmr;

        //使能触摸屏操作
        public bool touch_en;

        //当前模式
        public EM_SYS_MODE mode;

        //当前状态
        public EM_SYS_STA status;

        //退出标记
        public bool bquit;

        //程序关闭
        public bool bclose;

        //程序暂停
        public bool bpause;

        //当前IP
        public string str_cur_ip;

        //前机IP
        public string str_pre_ip;

        //后机IP
        public string str_next_ip;

        //信息设置
        public int log_cfg;

        public bool blight_always_on;

        public bool isChkMode
        {
            get { return ((mode & EM_SYS_MODE.CHK) == EM_SYS_MODE.CHK); }
        }

        public bool isDemoMode
        {
            get { return ((mode & EM_SYS_MODE.DEMO) == EM_SYS_MODE.DEMO); }
        }

        public bool isStepMode
        {
            get { return ((mode & EM_SYS_MODE.STEP) == EM_SYS_MODE.STEP); }
        }

        public bool isContinueMode
        {
            get { return ((mode & EM_SYS_MODE.CONTINUE) == EM_SYS_MODE.CONTINUE); }
        }

        public bool isNormalMode
        {
            get { return ((mode & EM_SYS_MODE.NORMAL) == EM_SYS_MODE.NORMAL); }
        }

        public bool isStandby
        {
            get { return (status == EM_SYS_STA.STANDBY); }
        }

        public string GetSysCfgPath
        {
            get { return string.Format("{0}\\syscfg\\", Path.GetFullPath("..")); }
        }

        public string GetSysProductPath
        {
            get { return string.Format("{0}\\product\\", Path.GetFullPath("..")); }
        }

        public string GetCurProductPath
        {
            get { return string.Format("{0}\\product\\{1}\\", Path.GetFullPath(".."), cur_product_name); }
        }

        #region 系统设置

        public void LoadSysCfg(string filename = "")
        {
            if (filename.Length < 3) filename = Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";
            IniFile inf = new IniFile(filename);

            //当前产品ID
            cur_product_name = inf.ReadString("PRODUCT_SET", "CUR_PRODCUT_NAME", "");

            //信息处理
            log_cfg = inf.ReadInteger("OTHER_SET", "MSG_CFG", 0);

            mc_sel = inf.ReadInteger("MC_SEL", "MC", 0);
            mc_name = inf.ReadString("MC_SEL", "NAME", "");
            mc_disc = inf.ReadString("MC_SEL", "DISC", "");
            hw_ver = inf.ReadInteger("MC_SEL", "HW_VER", 0);
        }

        public void SaveSysCfg(string filename = "")
        {
            if (filename.Length < 3) filename = Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";
            IniFile inf = new IniFile(filename);
            bool ischange = false;
            //当前产品ID
            inf.WriteString("PRODUCT_SET", "CUR_PRODCUT_NAME", cur_product_name, ref ischange, true, filename);
            //信息处理
            inf.WriteInteger("OTHER_SET", "MSG_CFG", log_cfg, ref ischange, true, filename);
        }

        #endregion
    }

    #endregion

    #region 产品设置

    public static class PT_SET
    {
        public enum BAR_SCAN
        {
            [Description("上相机扫码")] UP_SCAN,
            [Description("下相机扫码")] DW_SCAN,
            [Description("不扫码")] NO_SCAN,
        }

        public enum RUN_MD
        {
            [Description("两模块同时工作")] BOTH_WORK,
            [Description("模块1工作")] MD1_WORK,
            [Description("模块2工作")] MD2_WORK,
        }

        public enum RUN_PATTERN
        {
            [Description("正常模式")] RUN_NORMAL,
            [Description("正常上下料")] RUN_UPDW,
            [Description("空运行模式")] RUN_EMPTY,
        }

        #region 参数

        //上下料门禁使能
        public static bool bEnUpDownDr = false;

        //转盘门禁使能
        public static bool bEnTrayDr = false;

        //左光箱门禁使能
        public static bool bEnLBoxDr = false;

        //右光箱门禁使能
        public static bool bEnRBoxDr = false;

        //二维码扫码方式 0---上相机扫码  1---上相机扫码
        public static int BarcodeMode = 0;

        //运行模式 0--两个模块同时工作  1---模块1工作  2---模块2工作
        public static int UpDownRunMode = 0;

        //视觉回检
        public static bool bEnVsFB = false;

        //料盘检查-(包括XY允许偏差与R允许偏差)
        public static bool bEnVsTray = false;
        public static double Vs_XYofs;

        public static double Vs_Rofs;

        //运行方式  0---正常运行  1---正常上下料   2---空运行
        public static int RunPattern = 0;

        //上下料移动保护(针对转盘) 
        public static bool bUdMovSafe = false;

        //GRR流程测试
        public static bool bGrrFlow = false;
        public static int GRRTestCnt;

        public static int GRRUdlCnt;

        //模组带起
        public static bool bModPasteUp = false;
        public static int PlaceDly;

        public static int MovUpDly;

        //Y1起用
        public static bool Y1En = false;

        //右光箱Y1起用
        public static bool RY1En = false;

        //左右光箱起用
        public static bool LbEn = false;

        //提前开图
        public static bool turnon = false;
        public static int OpenDly;

        //料盘二维码拍照设置
        public static bool TrayBarcodeEn = false;

        //吸头1先运动设置
        public static bool xt1firsten = false;

        //小模组
        public static bool issmall = false;

        //单:1 双:2 1-8位下排:3  9-16位上排:4
        public static int bitOpenMode = 1;
        //霍尔测试
        public static bool HallEn = false;

        //取放料角度
        public static bool isopen_degree = false;
        public static int degree;

        //二维码回检
        public static bool bBarcodeCamBackEn = false;

        //OK品下料回检
        public static bool bOkCheck = false;

        //工站视觉模板增加有无测试
        public static bool bWsVsAddCheckEn = false;

        //高透模偏移检测
        public static bool bGTMCheck = false;
        public static double GTMOfs;

        //同工位连续同NG提示
        public static bool bSameNGTip;
        public static int SameNGTipCnt;
        //工站同排连续同NG提示
        public static bool bSameRowNGTip;
        public static int SameRowNGTipCnt;

        //保养
        public static int FixtrueMT;
        public static int EquipmentMT;
        //数据上传设置
        public static bool bUpdateSoft = true;
        public static bool bUploadData = true;
        //工站二次取料
        public static bool bWsPickAgain = false;
        //回检失败继续运行设置
        public static bool bBackerrcontinue = false;
        //夹具双工位非水平设置
        public static bool bNonparallel = false;
        //夹具清洗设置
        public static bool bCleanen = false;
        public static double Cleaninterval = 0;
        //上一次清洗时间
        public static string Lastcleantime = "";
        //相机参数设置
        public static double[] TrayExposure = new double[2];
        public static double[] WsExposure = new double[2];
        public static double[] JigExposure = new double[2];
        public static double[] TrayBrightness = new double[2];
        public static double[] WsBrightness = new double[2];
        public static double[] JigBrightness = new double[2];
        public static double[] TrayContrast = new double[2];
        public static double[] WsContrast = new double[2];
        public static double[] JigContrast = new double[2];
        public static bool BCamcfgset = false;

        //点检时间
        public static string CheckTimeMorning = "";
        public static string CheckTimeEvening = "";

        //上下料周期保存设置
        public static bool bCycle = false;

        //降温设置
        public static bool bCool = false;

        //DCC测试移至上下料
        public static bool bDelayTest = false;

        //NG管控设置
        public static int ngCode = 0;
        public static double ngScale = 0;
        public static bool bNgControl = false;

        //持续报警设置
        public static bool bContinuousAlarm = false;
        //轩仕佳光源
        public static bool bG4C;

        public static bool bNgWarn;
        public static double OkRate;

        public static bool IsMesLocal = true;
        //夹具扫码设置gy0123
        public static bool bJigSan = false;
        public static int TestTime = 30000;//测试等待最大时间30s
        public static int JigCntSend = 30;//夹具生产数据上报EAP，间隔时间30分钟gy0123

        //吸头个数设置
        public static bool bUpDn1XtOnlyOne = false;
        public static bool bUpDn2XtOnlyOne = false;

        //新增上拍二维码位置
        public static bool bAddCapQrcode;

        //回检针座管控
        public static double LeftArea;
        public static double RightArea;
        public static double UpArea;
        public static double DownArea;
        //允许误差
        public static double Area;
        //回检针座管控
        public static double LeftArea2;
        public static double RightArea2;
        public static double UpArea2;
        public static double DownArea2;
        //允许误差
        public static double Area2;
        //针座回检使能
        public static bool bConnectorCheck;
        //设备信息
        public static string EqpSN;
        public static string EqpPos;
        #endregion

        #region 参数存取

        public static EM_RES LoadPtCfg(string productname)
        {
            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载异常对应产品名{1}(<3个字符)!", "LoadPtCfg", productname) : string.Format("ERROR:{0} Load {1}(less than three characters)      ({0}加载异常对应产品名{1}(<3个字符)!)", "LoadPtCfg", productname), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\ptcfg.ini", Path.GetFullPath(".."), productname);

            if (!File.Exists(filename))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载异常对应产品名{1}配置文件不存在!", "LoadPtCfg", productname) : string.Format("{0}ERROR:Load {1},configuration file does not exist!   ({0}加载异常对应产品名{1}配置文件不存在!)", "LoadPtCfg", productname), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }

            IniFile inf = new IniFile(filename);
            //门禁
            bEnUpDownDr = inf.ReadBool("DOOR_SET", "UPDOWN_DOOR", true);
            bEnTrayDr = inf.ReadBool("DOOR_SET", "TRAY_DOOR", true);
            bEnLBoxDr = inf.ReadBool("DOOR_SET", "LBOX_DOOR", true);
            bEnRBoxDr = inf.ReadBool("DOOR_SET", "RBOX_DOOR", true);
            //其它
            bEnVsFB = inf.ReadBool("OTHER_SET", "VS_FB", false);
            bUdMovSafe = inf.ReadBool("OTHER_SET", "UD_MOV_SAFE", false);
            bEnVsTray = inf.ReadBool("OTHER_SET", "VS_TRAY", false);
            Vs_XYofs = inf.ReadDouble("OTHER_SET", "VS_XY_OFS", 0.5);
            Vs_Rofs = inf.ReadDouble("OTHER_SET", "VS_R_OFS", 0.5);
            BarcodeMode = inf.ReadInteger("OTHER_SET", "BARCODE_SCAN", 0);
            UpDownRunMode = inf.ReadInteger("OTHER_SET", "RUN_MODE", 0);
            RunPattern = inf.ReadInteger("OTHER_SET", "RUN_PATTERN", 0);
            bGrrFlow = inf.ReadBool("OTHER_SET", "GRR_FLOW", false);
            GRRTestCnt = inf.ReadInteger("OTHER_SET", "GRR_TEST_CNT", 11);
            GRRUdlCnt = inf.ReadInteger("OTHER_SET", "GRR_UDL_CNT", 10);
            bModPasteUp = inf.ReadBool("OTHER_SET", "MODPASTEUP", false);
            PlaceDly = inf.ReadInteger("OTHER_SET", "PLACEDLY", 50);
            MovUpDly = inf.ReadInteger("OTHER_SET", "MOVUPDLY", 50);
            Y1En = inf.ReadBool("OTHER_SET", "Y1EN", false);
            RY1En = inf.ReadBool("OTHER_SET", "RY1EN", false);
            LbEn = inf.ReadBool("OTHER_SET", "LbEN", true);
            TrayBarcodeEn = inf.ReadBool("OTHER_SET", "TrayBarcodeEn", false);
            xt1firsten = inf.ReadBool("OTHER_SET", "XT1FIRSTEN", false);
            turnon = inf.ReadBool("OTHER_SET", "TurnOn", true);
            OpenDly = inf.ReadInteger("OTHER_SET", "OpenDly", 500);
            HallEn = inf.ReadBool("OTHER_SET", "HallEN", false);
            issmall = inf.ReadBool("OTHER_SET", "ISsmall", true);
            //issingle = inf.ReadBool("OTHER_SET", "ISsingle", false);
            bitOpenMode = inf.ReadInteger("OTHER_SET", "BITOPENMODE", 1);
            isopen_degree = inf.ReadBool("OTHER_SET", "ISopen_degree", false);
            degree = inf.ReadInteger("OTHER_SET", "Degree", 90);
            bBarcodeCamBackEn = inf.ReadBool("OTHER_SET", "BARCODE_CAMBACK", true);
            bOkCheck = inf.ReadBool("OTHER_SET", "BOKCHECK", true);
            bWsVsAddCheckEn = inf.ReadBool("OTHER_SET", "WSVS_ADDCHECK", false);
            bGTMCheck = inf.ReadBool("OTHER_SET", "GTM_CHECK", true);
            GTMOfs = inf.ReadDouble("OTHER_SET", "GTM_OFS", 0.5);
            FixtrueMT = inf.ReadInteger("OTHER_SET", "FIXTRUE_MT", 0);
            EquipmentMT = inf.ReadInteger("OTHER_SET", "EQUIPMENT_MT", 0);
            bWsPickAgain = inf.ReadBool("OTHER_SET", "WSPICKAGAIN", true);
            bSameNGTip = inf.ReadBool("OTHER_SET", "BSAMENGTIP", false);
            SameNGTipCnt = inf.ReadInteger("OTHER_SET", "SAMENGTIP_CNT", 4);
            bSameRowNGTip = inf.ReadBool("OTHER_SET", "BSAMEROWNGTIP", false);
            SameRowNGTipCnt = inf.ReadInteger("OTHER_SET", "SAMEROWNGTIP_CNT", 5);



            bUpdateSoft = inf.ReadBool("OTHER_SET", "BUPDATESOFT", true);
            bUploadData = inf.ReadBool("OTHER_SET", "BUPLOADDATA", true);
            bBackerrcontinue = inf.ReadBool("OTHER_SET", "BBACKERRCONTINUE", false);
            bNonparallel = inf.ReadBool("OTHER_SET", "BNONPARALLEL", false);
            bCleanen = inf.ReadBool("OTHER_SET", "BCLEANEN", false);
            Cleaninterval = inf.ReadDouble("OTHER_SET", "CLEANINTERVAL", 120);
            Lastcleantime = inf.ReadString("OTHER_SET", "LASTCLEANTIME", DateTime.Now.ToString());
            BCamcfgset = inf.ReadBool("OTHER_SET", "BCAMCFGSET", false);
            bCycle = inf.ReadBool("OTHER_SET", "BCYCLE", true);
            bCool = inf.ReadBool("OTHER_SET", "BCOOL", false);
            bDelayTest = inf.ReadBool("OTHER_SET", "BDELAYTEST", false);
            ngCode = inf.ReadInteger("OTHER_SET", "NGCODE", 0);
            ngScale = inf.ReadDouble("OTHER_SET", "NGSCALE", 0);
            bNgControl = inf.ReadBool("OTHER_SET", "BNGCONTROL", false);//bContinuousAlarm
            bNgWarn = inf.ReadBool("OTHER_SET", "BNGWARN", true);
            IsMesLocal = inf.ReadBool("OTHER_SET", "ISMESLOCAL", true);
            OkRate = inf.ReadDouble("OTHER_SET", "OKRATE", 0);
            bContinuousAlarm = inf.ReadBool("OTHER_SET", "BCONTINUOUSALARM", false);
            bG4C = inf.ReadBool("OTHER_SET", "BG4C", false);//OTP光源是G4C或者其他
            bJigSan = inf.ReadBool("OTHER_SET", "bJigSan", true);
            TestTime = inf.ReadInteger("OTHER_SET", "TestTime", 30000);
            JigCntSend = inf.ReadInteger("OTHER_SET", "JigCntSend", 30);
            bAddCapQrcode = inf.ReadBool("OTHER_SET", "bAddCapQrcode", false);


            //吸头个数设置
            bUpDn1XtOnlyOne = inf.ReadBool("OTHER_SET", "bUpDn1XtOnlyOne", false);
            bUpDn2XtOnlyOne = inf.ReadBool("OTHER_SET", "bUpDn2XtOnlyOne", false);
            //
            LeftArea = inf.ReadDouble("OTHER_SET", "LeftArea", 0.5);
            RightArea = inf.ReadDouble("OTHER_SET", "RightArea", 0.5);
            UpArea = inf.ReadDouble("OTHER_SET", "UpArea", 0.5);
            DownArea = inf.ReadDouble("OTHER_SET", "DownArea", 0.5);
            Area = inf.ReadDouble("OTHER_SET", "Area", 0.5);

            LeftArea2 = inf.ReadDouble("OTHER_SET", "LeftArea2", 0.5);
            RightArea2 = inf.ReadDouble("OTHER_SET", "RightArea2", 0.5);
            UpArea2 = inf.ReadDouble("OTHER_SET", "UpArea2", 0.5);
            DownArea2 = inf.ReadDouble("OTHER_SET", "DownArea2", 0.5);
            Area2 = inf.ReadDouble("OTHER_SET", "Area2", 0.5);
            bConnectorCheck = inf.ReadBool("OTHER_SET", "bConnectorCheck", false);
            for (int i = 0; i < 2; i++)
            {
                TrayExposure[i] = inf.ReadDouble("OTHER_SET", string.Format("TRAYEXPOSURE{0}", i), 1);
                TrayContrast[i] = inf.ReadDouble("OTHER_SET", string.Format("TRAYCONTRAST{0}", i), 0);
                TrayBrightness[i] = inf.ReadDouble("OTHER_SET", string.Format("TRAYBRIGHTNESS{0}", i), 0);
                JigExposure[i] = inf.ReadDouble("OTHER_SET", string.Format("JigEXPOSURE{0}", i), 1);
                JigContrast[i] = inf.ReadDouble("OTHER_SET", string.Format("JigCONTRAST{0}", i), 0);
                JigBrightness[i] = inf.ReadDouble("OTHER_SET", string.Format("JigBRIGHTNESS{0}", i), 0);
                WsExposure[i] = inf.ReadDouble("OTHER_SET", string.Format("WSEXPOSURE{0}", i), 1);
                WsBrightness[i] = inf.ReadDouble("OTHER_SET", string.Format("WSBRIGHTNESS{0}", i), 0);

                WsContrast[i] = inf.ReadDouble("OTHER_SET", string.Format("WSCONTRAST{0}", i), 0);
            }
            CheckTimeMorning = inf.ReadString("OTHER_SET", "CHECKTIMEMORNING", "08:00:00");
            CheckTimeEvening = inf.ReadString("OTHER_SET", "CHECKTIMEEVENING", "20:00:00");

            EqpPos = inf.ReadString("OTHER_SET", "EqpPos", "新基地4号楼标杆车间");
            EqpSN = inf.ReadString("OTHER_SET", "EqpSN", "123456");
            return EM_RES.OK;
        }

        public static EM_RES SavePtCfg(string productname)
        {
            EM_RES res = EM_RES.OK;
            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}保存异常对应产品名{1}(<3个字符)!", "SavePtCfg", productname) : string.Format("ERROR:{0} save {1}(less than three characters)!    ({0}保存异常对应产品名{1}(<3个字符)!)", "SavePtCfg", productname), DReport.EmErrCode.SetParamError);
                return EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\ptcfg.ini", Path.GetFullPath(".."), productname);


            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;
            //if (!File.Exists(filename))
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}保存异常对应产品名{1}配置文件不存在!", "SavePtCfg", productname));
            //    return EM_RES.PARA_ERR;
            //}
            IniFile inf = new IniFile(filename);
            //门禁
            inf.WriteBool("DOOR_SET", "UPDOWN_DOOR", bEnUpDownDr, ref ischange, true, filename);
            inf.WriteBool("DOOR_SET", "TRAY_DOOR", bEnTrayDr, ref ischange, true, filename);
            inf.WriteBool("DOOR_SET", "LBOX_DOOR", bEnLBoxDr, ref ischange, true, filename);
            inf.WriteBool("DOOR_SET", "RBOX_DOOR", bEnRBoxDr, ref ischange, true, filename);
            //其它
            inf.WriteBool("OTHER_SET", "VS_FB", bEnVsFB, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "VS_TRAY", bEnVsTray, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "VS_XY_OFS", Vs_XYofs, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "VS_R_OFS", Vs_Rofs, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "UD_MOV_SAFE", bUdMovSafe, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "BARCODE_SCAN", BarcodeMode, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "RUN_MODE", UpDownRunMode, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "RUN_PATTERN", RunPattern, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "GRR_FLOW", bGrrFlow, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "GRR_TEST_CNT", GRRTestCnt, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "GRR_UDL_CNT", GRRUdlCnt, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "MODPASTEUP", bModPasteUp, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "PLACEDLY", PlaceDly, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "MOVUPDLY", MovUpDly, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "Y1EN", Y1En, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "RY1EN", RY1En, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "LbEN", LbEn, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "TurnOn", turnon, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "OpenDly", OpenDly, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "ISsmall", issmall, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "HallEN", HallEn, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "XT1FIRSTEN", xt1firsten, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "TrayBarcodeEn", TrayBarcodeEn, ref ischange, true, filename);
            //inf.WriteBool("OTHER_SET", "ISsingle", issingle, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "BITOPENMODE", bitOpenMode, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "ISopen_degree", isopen_degree, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "Degree", degree, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BOKCHECK", bOkCheck, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BARCODE_CAMBACK", bBarcodeCamBackEn, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "WSVS_ADDCHECK", bWsVsAddCheckEn, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "GTM_CHECK", bGTMCheck, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "GTM_OFS", GTMOfs, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "FIXTRUE_MT", FixtrueMT, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "EQUIPMENT_MT", EquipmentMT, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "WSPICKAGAIN", bWsPickAgain, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BSAMENGTIP", bSameNGTip, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "SAMENGTIP_CNT", SameNGTipCnt, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "TestTime", TestTime, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "JigCntSend", JigCntSend, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BSAMEROWNGTIP", bSameRowNGTip, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "SAMEROWNGTIP_CNT", SameRowNGTipCnt, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BUPDATESOFT", bUpdateSoft, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BUPLOADDATA", bUploadData, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BBACKERRCONTINUE", bBackerrcontinue, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BNONPARALLEL", bNonparallel, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "CLEANINTERVAL", Cleaninterval, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BCLEANEN", bCleanen, ref ischange, true, filename);
            inf.WriteString("OTHER_SET", "LASTCLEANTIME", Lastcleantime, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BCAMCFGSET", BCamcfgset, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BCOOL", bCool, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BCYCLE", bCycle, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BDELAYTEST", bDelayTest, ref ischange, true, filename);
            inf.WriteInteger("OTHER_SET", "NGCODE", ngCode, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "NGSCALE", ngScale, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BNGCONTROL", bNgControl, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BCONTINUOUSALARM", bContinuousAlarm, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BNGWARN", bNgWarn, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "OKRATE", OkRate, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "ISMESLOCAL", IsMesLocal, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "BG4C", bG4C, ref ischange, true, filename);//轩仕佳光源
            inf.WriteBool("OTHER_SET", "bJigSan", bJigSan, ref ischange, true, filename);

            //吸头个数设置
            inf.WriteBool("OTHER_SET", "bUpDn1XtOnlyOne", bUpDn1XtOnlyOne, ref ischange, true, filename);
            inf.WriteBool("OTHER_SET", "bUpDn2XtOnlyOne", bUpDn2XtOnlyOne, ref ischange, true, filename);

            //
            inf.WriteDouble("OTHER_SET", "LeftArea", LeftArea, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "RightArea", RightArea, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "UpArea", UpArea, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "DownArea", DownArea, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "Area", Area, ref ischange, true, filename);

            inf.WriteDouble("OTHER_SET", "LeftArea2", LeftArea2, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "RightArea2", RightArea2, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "UpArea2", UpArea2, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "DownArea2", DownArea2, ref ischange, true, filename);
            inf.WriteDouble("OTHER_SET", "Area2", Area2, ref ischange, true, filename);

            inf.WriteBool("OTHER_SET", "bConnectorCheck", bConnectorCheck, ref ischange, true, filename);
            for (int i = 0; i < 2; i++)
            {
                inf.WriteDouble("OTHER_SET", string.Format("JigEXPOSURE{0}", i), JigExposure[i], ref ischange, true, filename);
                inf.WriteDouble("OTHER_SET", string.Format("JigCONTRAST{0}", i), JigContrast[i], ref ischange, true, filename);
                inf.WriteDouble("OTHER_SET", string.Format("JigBRIGHTNESS{0}", i), JigBrightness[i], ref ischange, true, filename);

                inf.WriteDouble("OTHER_SET", string.Format("TRAYEXPOSURE{0}", i), TrayExposure[i], ref ischange, true, filename);
                inf.WriteDouble("OTHER_SET", string.Format("TRAYCONTRAST{0}", i), TrayContrast[i], ref ischange, true, filename);
                inf.WriteDouble("OTHER_SET", string.Format("TRAYBRIGHTNESS{0}", i), TrayBrightness[i], ref ischange, true, filename);

                inf.WriteDouble("OTHER_SET", string.Format("WSEXPOSURE{0}", i), WsExposure[i], ref ischange, true, filename);
                inf.WriteDouble("OTHER_SET", string.Format("WSBRIGHTNESS{0}", i), WsBrightness[i], ref ischange, true, filename);

                inf.WriteDouble("OTHER_SET", string.Format("WSCONTRAST{0}", i), WsContrast[i], ref ischange, true, filename);
            }
            inf.WriteString("OTHER_SET", "CHECKTIMEMORNING", CheckTimeMorning, ref ischange, true, filename);
            inf.WriteString("OTHER_SET", "CHECKTIMEEVENING", CheckTimeEvening, ref ischange, true, filename);
            inf.WriteString("OTHER_SET", "EqpPos", EqpPos, ref ischange, true, filename);
            inf.WriteString("OTHER_SET", "EqpSN", EqpSN, ref ischange, true, filename);
      

            inf.WriteBool("OTHER_SET", "bAddCapQrcode", bAddCapQrcode, ref ischange, true, filename);
            if (ischange)
            {
                //创建backup
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), productname);
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                res = SYS_PUD.FileWriteLine("ptcfg.ini", backup, backup_filename);
                if (res != EM_RES.OK) return res;
            }
            
            return EM_RES.OK;
        }

        #endregion
    }

    #endregion

    #region 产品新增

    public static class SYS_PUD
    {
        public static EM_RES CopyFile(String filename, String to_prodcut_name = "", String fr_prodcut_name = "")
        {
            //文件夹
            if (Path.GetExtension(filename).Length < 2)
            {
                if (filename.Length < 3)
                {
                    MessageBox.Show(VAR.IsChinese ? "生成文件夹时，产品名字或文件夹名字小于3字符！" : "When creating a folder, the product name or folder name is less than 3 characters!\r\n(生成文件夹时，产品名字或文件夹名字小于3字符！)");
                    return EM_RES.ERR;
                }

                String dir_name;
                dir_name = Path.GetFullPath("..") + "\\" + filename;
                //check exist 
                if (Directory.Exists(dir_name))
                {
                    MessageBox.Show(VAR.IsChinese ? "对应产品名或文件夹已经存在!\r\n" + dir_name : "The corresponding product name or folder already exists!\r\n(对应产品名或文件夹已经存在!)\r\n" + dir_name);
                    return EM_RES.ERR;
                }

                try
                {
                    Directory.CreateDirectory(dir_name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" + ex.Message + "\r\n" + dir_name : "Error creating folder:" + ex.Message + "\r\n" + dir_name + "\r\n建立文件夹出错：" + ex.Message + "\r\n" + dir_name);
                    return EM_RES.ERR;
                }
            }
            //文件
            else
            {
                if (fr_prodcut_name == "") fr_prodcut_name = VAR.gsys_set.cur_product_name;

                if (to_prodcut_name.Length < 3 || fr_prodcut_name.Length < 3)
                {
                    MessageBox.Show(VAR.IsChinese ? "复制文件时，产品名字小于3字符" : "Product name is less than 3 characters when copying files\r\n(复制文件时，产品名字小于3字符)");
                    return EM_RES.ERR;
                }

                String fr_file_name;
                fr_file_name = Path.GetFullPath("..") + "\\product\\" + fr_prodcut_name + "\\" + filename;
                String to_file_name;
                to_file_name = Path.GetFullPath("..") + "\\product\\" + to_prodcut_name + "\\" + filename;
                try
                {
                    if (!File.Exists(fr_file_name)) return EM_RES.ABORT;
                    //if(File.Exists(to_file_name))File.Delete(to_file_name);
                    File.Copy(fr_file_name, to_file_name, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "复制文件错： " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name : "Copy file error: " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name + "\r\n复制文件错： " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name);
                    return EM_RES.ERR;
                }
            }

            return EM_RES.OK;
        }

        public static EM_RES CopyFile2(String filename, String to_filename = "", String fr_filename = "")
        {
            //文件夹
            if (Path.GetExtension(filename).Length < 2)
            {
                if (filename.Length < 3)
                {
                    MessageBox.Show(VAR.IsChinese ? "生成文件夹时，产品名字或文件夹名字小于3字符！" : "When creating a folder, the product name or folder name is less than 3 characters!\r\n(生成文件夹时，产品名字或文件夹名字小于3字符！)");
                    return EM_RES.ERR;
                }

                String dir_name;
                // dir_name = Path.GetFullPath("..") + "\\" + filename;
                dir_name = filename;
                //check exist 
                if (Directory.Exists(dir_name))
                {
                    return EM_RES.OK;
                }

                try
                {
                    Directory.CreateDirectory(dir_name);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" + ex.Message + "\r\n" + dir_name : "Error creating folder:" + ex.Message + "\r\n" + dir_name + "\r\n建立文件夹出错：" + ex.Message + "\r\n" + dir_name);
                    return EM_RES.ERR;
                }
            }
            //文件
            else
            {

                if (to_filename.Length < 3 || to_filename.Length < 3)
                {
                    MessageBox.Show(VAR.IsChinese ? "复制文件时，产品名字小于3字符" : "Product name is less than 3 characters when copying files\r\n复制文件时，产品名字小于3字符");
                    return EM_RES.ERR;
                }

                String fr_file_name;
                fr_file_name = fr_filename;
                String to_file_name;
                to_file_name = to_filename + "\\" + filename;
                try
                {
                    if (!File.Exists(fr_file_name)) return EM_RES.ABORT;
                    if (File.Exists(to_file_name)) File.Delete(to_file_name);
                    File.Copy(fr_file_name, to_file_name, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "复制文件出错： " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name : "Error copying files: " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name + "\r\n复制文件出错： " + ex.Message + "\r\nFrom:" + fr_file_name + "\r\nTo:" + to_file_name);
                    return EM_RES.ERR;
                }
            }

            return EM_RES.OK;
        }

        public static EM_RES FileWriteLine(String filename, string[] wl, String to_filename = "")
        {
            if (to_filename.Length < 3 || to_filename.Length < 3)
            {
                MessageBox.Show(VAR.IsChinese ? "复制文件时，产品名字小于3字符" : "Product name is less than 3 characters when copying files\r\n(复制文件时，产品名字小于3字符)");
                return EM_RES.ERR;
            }

            String to_file_name;
            to_file_name = to_filename + "\\" + filename;
            try
            {
                if (File.Exists(to_file_name)) File.Delete(to_file_name);
                File.WriteAllLines(to_file_name, wl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(VAR.IsChinese ? "复制文件出错： " + ex.Message + "\r\n To:" + to_file_name : "Error copying files: " + ex.Message + "\r\n To:" + to_file_name + "\r\n复制文件出错： " + ex.Message + "\r\n To:" + to_file_name);
                return EM_RES.ERR;
            }

            return EM_RES.OK;
        }

        public static EM_RES CfgCopyFile(string product_new, string product_temp, string FirName = "", string SecName = "", int FileCnt = 1)
        {
            string dir_name;
            EM_RES res;
            dir_name = string.Format("{0}\\product\\{1}{2}{3}\\", Path.GetFullPath(".."), product_temp, FirName != "" ? "\\" + FirName : "", SecName != "" ? "\\" + SecName : "");
            var files = Directory.EnumerateFiles(dir_name);
            if (FileCnt >= 0 && files.Count() < FileCnt)
            {
                MessageBox.Show(VAR.IsChinese ? dir_name + "配置文件缺失，请检查！" : dir_name + "Configuration file is missing, please check!\r\n" + dir_name + "配置文件缺失，请检查！");
                return EM_RES.ERR;
            }
            foreach (var ff in files)
            {
                res = CopyFile(string.Format("{0}{1}{2}", FirName != "" ? FirName + "\\" : "", SecName != "" ? SecName + "\\" : "", Path.GetFileName(ff)), product_new, product_temp);
                if (res == EM_RES.ABORT) continue;
                else if (res != EM_RES.OK) return res;
            }
            return EM_RES.OK;
        }
        public static EM_RES AddProduct(string product_new, string product_temp)
        {
            EM_RES ret;
            string[] FirDegFile = new string[8] { "Camera", "image", "CsvData", "DataBase", "LightBoxCfg", "Shape", "TrayBoxCfg", "WsCfg" };
            string[] SecDegFile = new string[4] { "CamDw1", "CamDw2", "CamUp1", "CamUp2" };
            //check source
            string dir_name;
            dir_name = Path.GetFullPath("..") + "\\product\\" + product_temp;
            //check product_temp exist 
            if (!Directory.Exists(dir_name))
            {
                MessageBox.Show(VAR.IsChinese ? "找不到模板文件夹，请重新选择模板？" : "Can't find template folder, please select a new template?\r\n找不到模板文件夹，请重新选择模板？", VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return EM_RES.ERR;
            }
            //check product_new exist 
            dir_name = Path.GetFullPath("..") + "\\product\\" + product_new;
            if (Directory.Exists(dir_name))
            {
                if (DialogResult.Cancel == MessageBox.Show(VAR.IsChinese ? "对应产品文件夹已经存在，是否要覆盖？" : "The corresponding product folder already exists. Do you want to overwrite it?\r\n对应产品文件夹已经存在，是否要覆盖？", VAR.IsChinese ? "警告" : "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    return EM_RES.OK;
            }

            //创建产品文件夹
            ret = CopyFile("product\\" + product_new);
            if (ret == EM_RES.ERR) return ret;
            for (int i = 0; i < FirDegFile.Length; i++)
            {
                //创建产品/视觉
                ret = CopyFile("product\\" + product_new + "\\" + FirDegFile[i]);
                if (ret == EM_RES.ERR) return ret;
                if (FirDegFile[i] == "Camera" || FirDegFile[i] == "image")
                {
                    for (int j = 0; j < SecDegFile.Length; j++)
                    {
                        ret = CopyFile("product\\" + product_new + "\\" + FirDegFile[i] + "\\" + SecDegFile[j]);
                        if (ret == EM_RES.ERR) return ret;
                    }
                }
            }
            //创建文件
            for (int i = 0; i < SecDegFile.Length; i++)
            {
                ret = CfgCopyFile(product_new, product_temp, "Camera", SecDegFile[i], SecDegFile[i].Contains("CamDw") ? 5 : 6);
                if (ret != EM_RES.OK) return ret;
            }

            ret = CfgCopyFile(product_new, product_temp, "LightBoxCfg", "", 3);
            if (ret != EM_RES.OK) return ret;

            ret = CfgCopyFile(product_new, product_temp, "Shape", "", -1);
            if (ret != EM_RES.OK) return ret;

            ret = CfgCopyFile(product_new, product_temp, "TrayBoxCfg", "", 3);
            if (ret != EM_RES.OK) return ret;

            ret = CfgCopyFile(product_new, product_temp, "WsCfg", "", 4);
            if (ret != EM_RES.OK) return ret;
            //复制文件
            ret = CopyFile("ngcode.xml", product_new, product_temp);
            if (ret != EM_RES.OK) return ret;
            ret = CopyFile("ptcfg.ini", product_new, product_temp);
            if (ret != EM_RES.OK) return ret;
            ret = CopyFile("xtcfg.ini", product_new, product_temp);
            if (ret != EM_RES.OK) return ret;
            ret = CopyFile("xtcfg.ini", product_new, product_temp);
            if (ret != EM_RES.OK) return ret;
            ret = CopyFile("ZoneDef.ini", product_new, product_temp);
            if (ret != EM_RES.OK) return ret;
            return EM_RES.OK;
        }
    }


    #endregion

    #region 错误分类
    public class ERR_ALM
    {
        public enum EmErrItem
        {
            [Description("NULL")]
            Null = 0,
            [Description("运动失败")]
            MoveError,
            [Description("参数错误")]
            ParmError,
            [Description("初始化错误")]
            InitFailed,
            [Description("板卡错误")]
            CardError,
            [Description("等待超时")]
            TimeOut,
            [Description("限位保护")]
            LimitProtect,
            [Description("移动保护")]
            MoveProtect,
            [Description("门禁保护")]
            DoorProtect,
            [Description("急停按下")]
            EmgStop,
            [Description("连接失败")]
            ConnectError,
            [Description("切换失败")]
            ChangeError,
            [Description("拍照异常")]
            CaptureAbnomal,
            [Description("状态异常")]
            StautsAbnomal,
            [Description("测试异常")]
            TestAbnormal,
            [Description("上下料异常")]
            UpDownLoadAbnormal,
            [Description("物料放偏")]
            MaterialPosErr,
            [Description("仓储异常")]
            TrayBoxAbnormal,
            [Description("二维码异常")]
            BarcodeAbnormal,

        }

        public EmErrItem ErrItem;
        public string ErrStr;

        public ERR_ALM()
        {
            ErrItem = EmErrItem.Null;
            ErrStr = string.Empty;
        }

        public void Clear()
        {
            ErrItem = EmErrItem.Null;
            ErrStr = string.Empty;
        }
    }
    #endregion

    #region 统计数据
    public static class COUNT_DATA
    {
        public static int[] allcnt = new int[4]; //工站投料数
        public static int[] ngcnt = new int[4]; //产出数据
        public static int[] okcnt = new int[4];  //NG数据

        public static double runtime;         //运行时间sec
        public static double waittime;        //空闲时间sec
        public static double waitwltime;      //待料时间
        public static DateTime dt;            //清零日期
        //gy0123
        public static int openimagerate;  //开图率
        public static int cnt_ng_image;  //开图NG数量
        public static int cnt_ng_OS;  //os异常
        public static int cnt_ng_miss_pix;  //图像丢帧
        public static int cnt_ng_exposure;  //曝光NG
        public static int cnt_ng_iic;  //IIC读写NG
        public static int cnt_ng_other;  //其他NG


        //保养—生产次数
        public static int CurFixtrueMT;
        public static int CurEquipmentMT;
        public static int SuctionAllcnt;  //吸料总数
        public static int SuctionErrcnt;  //吸不起数量

        //NG管控
        public static int ngctrlallcnt = 0;
        public static int ngctrlngcnt = 0;

        #region  加载
        public static EM_RES LoadCountCfg(string productname)
        {
            //if (!File.Exists(Path.GetFullPath("..") + "\\product\\" + VAR.gst_sys_set.cur_product_name + "\\barcode.csv"))
            //    File.Create(Path.GetFullPath("..") + "\\product\\" + VAR.gst_sys_set.cur_product_name + "\\barcode.csv");
            //if (!Directory.Exists(Path.GetFullPath("..") + "\\product\\" + VAR.gst_sys_set.cur_product_name + "\\Image\\"))
            //    Directory.CreateDirectory(Path.GetFullPath("..") + "\\product\\" + VAR.gst_sys_set.cur_product_name + "\\Image\\");



            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载异常对应产品名{1}(<3个字符)!", "LoadCountCfg", productname) : string.Format("ERROR:{0} load {1}(Less than three characters)!   ({0}加载异常对应产品名{1}(<3个字符)!)", "LoadCountCfg", productname), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\countcfg.ini", Path.GetFullPath(".."), productname);

            if (!File.Exists(filename))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载异常对应产品名{1}配置文件不存在!", "LoadCountCfg", productname) : string.Format("ERROR:{0} load {1}(configuration file does not exist!)    ({0}加载异常对应产品名{1}配置文件不存在!)", "LoadCountCfg", productname), DReport.EmErrCode.SetParamError);
                return EM_RES.PARA_ERR;
            }
            IniFile inf = new IniFile(filename);

            for (int i = 0; i < 4; i++)
            {
                allcnt[i] = inf.ReadInteger("COUNT", "ALLCNT" + i.ToString(), 0);
                ngcnt[i] = inf.ReadInteger("COUNT", "NGCNT" + i.ToString(), 0);
                okcnt[i] = inf.ReadInteger("COUNT", "OKCNT" + i.ToString(), 0);
            }
            SuctionAllcnt = inf.ReadInteger("COUNT", "SUCTIONALLTIME", 0);
            SuctionErrcnt = inf.ReadInteger("COUNT", "SUCTIONERRTIME", 0);
            runtime = inf.ReadDouble("CNT", "RUNTIME", runtime);
            waittime = inf.ReadDouble("CNT", "WAITTIME", waittime);
            waitwltime = inf.ReadDouble("CNT", "WAITWLTIME", 0);
            CurFixtrueMT = inf.ReadInteger("OTHER_SET", "CUR_FIXTRUE_MT", 0);
            CurEquipmentMT = inf.ReadInteger("OTHER_SET", "CUR_EQUIPMENT_MT", 0);
            openimagerate = inf.ReadInteger("OTHER_SET", "OPENIMAGE_RATE", 0);

            dt = Convert.ToDateTime(inf.ReadString("CNT", "DT", dt.ToString()));
            ngctrlallcnt = inf.ReadInteger("COUNT", "NGCTRLALLCNT", 0);
            ngctrlngcnt = inf.ReadInteger("COUNT", "NGCTRLNGCNT", 0);

            return EM_RES.OK;
        }
        #endregion
        #region  保存
        public static EM_RES SaveCountCfg(string productname, bool bsave = false)
        {
            //if (!bsave && (cnt_pcs_temp == cnt_pcs) && (runtime - timer_temp) < 60) return;
            //cnt_pcs_temp = cnt_pcs;
            //timer_temp = runtime;


            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}加载异常对应产品名{1}(<3个字符)!", "SaveCountCfg", productname) : string.Format("ERROR:{0} load {1}(less than three characters)!   ({0}加载异常对应产品名{1}(<3个字符)!)", "SaveCountCfg", productname), DReport.EmErrCode.SetParamError);
                return EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\countcfg.ini", Path.GetFullPath(".."), productname);

            //if (!File.Exists(filename))
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}加载异常对应产品名{1}配置文件不存在!", "SaveCountCfg", productname));
            //    return EM_RES.PARA_ERR;
            //}
            bool ischange = false;
            IniFile inf = new IniFile(filename);
            for (int i = 0; i < 4; i++)
            {
                inf.WriteInteger("COUNT", "ALLCNT" + i.ToString(), allcnt[i], ref ischange, false);
                inf.WriteInteger("COUNT", "NGCNT" + i.ToString(), ngcnt[i], ref ischange, false);
                inf.WriteInteger("COUNT", "OKCNT" + i.ToString(), okcnt[i], ref ischange, false);
            }
            inf.WriteInteger("COUNT", "SUCTIONALLTIME", SuctionAllcnt, ref ischange, false);
            inf.WriteInteger("COUNT", "SUCTIONERRTIME", SuctionErrcnt, ref ischange, false);
            inf.WriteDouble("CNT", "RUNTIME", runtime, ref ischange, false);
            inf.WriteDouble("CNT", "WAITTIME", waittime, ref ischange, false);
            inf.WriteDouble("CNT", "WAITWLTIME", waitwltime, ref ischange, false);
            inf.WriteInteger("OTHER_SET", "CUR_FIXTRUE_MT", CurFixtrueMT, ref ischange, false);
            inf.WriteInteger("OTHER_SET", "CUR_EQUIPMENT_MT", CurEquipmentMT, ref ischange, false);
            inf.WriteInteger("OTHER_SET", "OPENIMAGE_RATE", openimagerate, ref ischange, false);
            inf.WriteString("CNT", "DT", dt.ToString(), ref ischange, false);
            inf.WriteInteger("COUNT", "NGCTRLALLCNT", ngctrlallcnt, ref ischange, false);
            inf.WriteInteger("COUNT", "NGCTRLNGCNT", ngctrlngcnt, ref ischange, false);
            return EM_RES.OK;
        }
        #endregion
        #region 清零
        public static void Clear()
        {
            double Activation = 0;
            Activation = COUNT_DATA.runtime + COUNT_DATA.waittime;
            if (Activation == 0) Activation = 1;
            Activation = 1 - COUNT_DATA.waittime / Activation;
            Activation = Activation * 100;
            for (int i = 0; i < 4; i++)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("工站:{0},投入:{1},产出:{2},不良:{3}", i, allcnt[i], okcnt[i], ngcnt[i]) : string.Format("WS:{0},ALLCNT:{1},OKCNT:{2},NGCNT:{3}", i, allcnt[i], okcnt[i], ngcnt[i]));
            }

            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("上次清零时间:{0},运行时间:{1:0.0}h,待机时间:{2:0.0}h,稼动率:{3}", dt.ToString(), (double)(runtime / 3600), (double)(waittime / 3600), Activation.ToString("f2") + "%") : string.Format("Last clear time:{0},Run time:{1:0.0}h,Waittime:{2:0.0}h,Activation:{3}", dt.ToString(), (double)(runtime / 3600), (double)(waittime / 3600), Activation.ToString("f2") + "%"));
            for (int i = 0; i < 4; i++)
            {
                allcnt[i] = 0;
                ngcnt[i] = 0;
                okcnt[i] = 0;
            }
            runtime = 0;
            waittime = 0;
            waitwltime = 0;
            SuctionAllcnt = 0;
            SuctionErrcnt = 0;
            openimagerate = 0;
            dt = System.DateTime.Now;
        }

        #endregion
    }
    #endregion

    #region  设备监控
    public static class DRpt
    {

        public static readonly DReport dr = new DReport();

        public static void Report_CreateTable()
        {
            if (!PT_SET.bUploadData) return;
            try
            {
                dr.CreateTable();
            }
            catch (Exception e)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            }
        }
        public static void Report_NgProduct(int ngcode, int fixnum, string ngdesc, bool brework = false)
        {
            /*if (!PT_SET.bUploadData) */
            return;
            //try
            //{
            //    dr.ReportNgProduct(ngcode, fixnum, ngdesc,brework);
            //}
            //catch (Exception e)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            //}
        }

        public static void Report_OkProduct(int num, bool brework = false)
        {
            /*if (!PT_SET.bUploadData || num <= 0)*/
            return;
            //try
            //{
            //    dr.ReportOkProduct(num,brework);
            //}
            //catch (Exception e)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            //}
        }

        public static void Report_Status(DReport.EmStatus emsta, DReport.EmHareware emhar, string str)
        {
            /*if (!PT_SET.bUploadData)*/
            return;
            //try
            //{
            //    dr.ReportStatus(emsta, emhar, str);
            //}
            //catch (Exception e)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            //}
        }

        public static void Report_Product(string productname, bool brework)
        {
            /*if (!PT_SET.bUploadData)*/
            return;
            //try
            //{
            //    dr.ReportProduct(productname, brework);
            //}
            //catch (Exception e)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            //}
        }

        public static void Report_Error(DReport.EmErrCode emerr, int relevanthw, string str)
        {
            /*if (!PT_SET.bUploadData)*/
            return;
            //try
            //{
            //    dr.ReportError(emerr, relevanthw, str);
            //}
            //catch (Exception e)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            //}
        }

        public static void Report_Opration(int keycode, int tgcode, string opdesc)
        {
            if (!PT_SET.bUploadData || keycode != 3000) return;
            try
            {
                dr.ReportOpration(keycode, tgcode, opdesc);
            }
            catch (Exception e)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, e.ToString());
            }
        }
    }


    #endregion

}