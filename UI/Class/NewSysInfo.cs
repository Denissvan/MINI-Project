using MotionCtrl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Class
{
    public class NewSysInf
    {

        // 类型设置
        public class UserSet
        {
            [Description("红灯亮蜂鸣响")]
            public bool RedLightSund;
            [Description("仅在工站扫二维码")]
            public bool bGetOrcodOnWs;
            [Description("NG比例仅特定代码")]
            public bool bNgRateBySet;
            [Description("NG比例代码(用英文逗号间隔)")]
            public string NgRateCodes;
            [Description("清料后关闭工装")]
            public bool bClearSetOffWs;
            [Description("工站取料偏移")]
            public bool bPickWsDis;
            [Description("取料偏移Z")]
            public double PickWsDisZ;
            [Description("取料偏移X")]
            public double PickWsDisX;
            [Description("取料偏移Y")]
            public double PickWsDisY;

            [Description("工站放料偏移")]
            public bool bPlaceWsDis;
            [Description("放料偏移Z")]
            public double PlaceWsDisZ;
            [Description("放料偏移X")]
            public double PlaceWsDisX;
            [Description("放料偏移Y")]
            public double PlaceWsDisY;

            [Description("关闭工站上检二维码")]
            public bool bUpWsChkQrCodeOff;
            [Description("飞拍照片数量检测")]
            public bool bPicCntForSafe;

            [Description("AGV叫料")]
            public bool bAGVCallBox;
            [Description("等待Agv到位时间(秒)")]
            public int WaitAGVToSeconds;
            [Description("提前几盒叫料")]
            public int AgvPreCallBox;

            [Description("复测物料不统计上传mes")]
            public bool bDoubleTestNotCnt;
            [Description("放料盘吸头先大后小")]
            public bool bXtPrePlaceTrayByBigId;
            [Description("启动测试前执行关闭")]
            public bool bBeforeTestReset;
            [Description("上料后开图前停止其他轴")]
            public bool bBeforeOpenImageStopAxis;
            [Description("马达二维码位数校验")]
            public bool bCheckMotoCodeLength;
            [Description("上料后延迟开图时间(毫秒)")]
            public int  BeforeOpenImageWaitTime;
            [Description("上料位开图后闭气时间(毫秒)")]
            public int BeforeOpenImageWindOffTime;


        }
        public class NoneRunAll
        {
            [Description("常见设置开启关闭")]
            public UserSet UserNormalSet = new UserSet();

        }

        [Description("空跑位置参数")]
        public static NoneRunAll NoneRunPosInfo = new NoneRunAll();

        public static UserSet UserParams => NoneRunPosInfo.UserNormalSet;
        public static bool LoadSysInfCfg(out string errMsg)
        {
            errMsg = "";
            try
            {
                //产品参数
                string filename = string.Format("{0}\\product\\NewSysInf.ini", Path.GetFullPath(".."));

                if (!File.Exists(filename))
                {
                    errMsg = string.Format("{0}加载NewSysInf配置文件不存在!", "LoadSysInfCfg");
                    return false;
                }

                IniFile inf = new IniFile(filename);

                var AllPosStr = inf.ReadString("OTHER_SET", "NoneRunPosInfo", "");
                if (AllPosStr.Length > 10)
                {
                    NewSysInf.NoneRunPosInfo = JsonConvert.DeserializeObject<NewSysInf.NoneRunAll>(AllPosStr);
                    return true;
                }

                return false;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public static bool SaveSysInf(out string errmsg)
        {
            errmsg = "";
            try
            {
                EM_RES res = EM_RES.OK;
                //产品参数
                string filename = string.Format("{0}\\product\\NewSysInf.ini", Path.GetFullPath(".."));

                if (!File.Exists(filename))
                {
                    new FileStream(filename, FileMode.Create);
                }

                string[] backup = File.ReadAllLines(filename);
                bool ischange = false;
                //if (!File.Exists(filename))
                //{
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}保存异常对应产品名{1}配置文件不存在!", "SavePtCfg", productname));
                //    return EM_RES.PARA_ERR;
                //}
                IniFile inf = new IniFile(filename);
                //门禁

                var EqpPos = JsonConvert.SerializeObject(NewSysInf.NoneRunPosInfo);
                inf.WriteString("OTHER_SET", "NoneRunPosInfo", EqpPos, ref ischange, true, filename);
                if (ischange)
                {
                    //创建backup
                    string backup_filename = string.Format("{0}\\product\\backup", Path.GetFullPath(".."));
                    res = SYS_PUD.CopyFile2(backup_filename);
                    if (res != EM_RES.OK) return false;
                    res = SYS_PUD.FileWriteLine("NewSysInf.ini", backup, backup_filename);
                    if (res != EM_RES.OK) return false;
                }

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }
    }
}
