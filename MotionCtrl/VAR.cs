using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;

namespace MotionCtrl
{
    public class VAR
    {
        //系统设置/状态
        public static SYS_SET gsys_set = new SYS_SET();
        public static readonly object lockthis = new object();

        //信息
        public static Msg msg = new Msg();        

        //系统提示
        public static SYS_INF sys_inf = new SYS_INF();
        //清料标志位
        public static bool ClearMt = false;
        //点检PC
        public static int  ChkPC = 1;
        //点检测试次数
        public static int  ChkCnt = 1;
        //是否正常品
        public static bool  Isnormal = true;
        //暂时取消同NG超数据提示
        public static bool bSameNGTip_Temp = true;
        //目前操作员
        public static string CurUserName = "";
        //系统错误信息
        public static ERR_ALM SysErrAlm=new ERR_ALM();
        //错误弹窗报警标志
        public static bool IsErrAlm = false;
        //语言
        public static bool IsChinese = false;
        //复测分区
        public static bool Isretestzone = false;
        //设备运行时间监控
        public static DataTable dttt = new DataTable();

        static bool _isAutoChkMode;
        /// <summary>
        /// 是否在自动点检生产模式
        /// </summary>
        public static bool isAutoChkMode
        {
            get
            {
                if (!PT_SET.AutoChkEn)
                    _isAutoChkMode = false;
                return _isAutoChkMode;
            }
            set
            {
                if (!PT_SET.AutoChkEn)
                    _isAutoChkMode = false;
                else
                    _isAutoChkMode = value;
            }
        }


    }
}
