using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;
using DevReport;

namespace UI
{
    public static class MT
    {
        #region 板卡定义
        public static CARD CARD_ECI2400_0 = new CARD(0, "192.168.0.100", 4, 24, 8, CARD.BRAND.ZMOTION, CARD.TYPE.MOTION, "ECI2400", "转台", "Turn Table");
        public static CARD CARD_ECI2600_1 = new CARD(1, "192.168.0.101", 6, 24, 8, CARD.BRAND.ZMOTION, CARD.TYPE.MOTION, "ECI2600", "左光箱", "Left LightBox");
        public static CARD CARD_ECI2400_2 = new CARD(2, "192.168.0.102", 6, 24, 8, CARD.BRAND.ZMOTION, CARD.TYPE.MOTION, "ECI2400", "右光箱", "Right  LightBox");
        public static CARD CARD_ECI2400_3 = new CARD(3, "192.168.0.103", 4, 24, 8, CARD.BRAND.ZMOTION, CARD.TYPE.MOTION, "ECI2400", "上下料", "UpDownLoad");
        // public static CARD CARD_ECI2600_3 = new CARD(3, "192.168.0.103", 6, 24, 8, CARD.BRAND.ZMOTION, CARD.TYPE.MOTION, "ECI2600", "下料");
        // public static CARD CARD_ECI0064_4 = new CARD(4, "192.168.0.104", 0, 32, 32, CARD.BRAND.ZMOTION, CARD.TYPE.IO, "ECI0064", "下料");
        //public static CARD CARD_DMC3800_5 = new CARD(5, 0, 8, 16, 16, CARD.BRAND.LEADSHINE, CARD.TYPE.MOTION, "DMC3800", "上料");
        public static CARD CARD_DMC3C00_4 = new CARD(4, 0, 12, 16, 16, CARD.BRAND.LEADSHINE, CARD.TYPE.MOTION, "DMC3C00", "上下料", "UpDownLoad");

        public static CARD CARD_ORIENTAL485_5 = new CARD(5, "COM5", 115200, CARD.BRAND.ORIENTALMOTOR, CARD.TYPE.MOTION, "ORIENTAL", "工装平台", "Fixture Platform");
        public static CARD CARD_ECI0064_6 = new CARD(6, "192.168.0.106", 0, 32, 32, CARD.BRAND.ZMOTION, CARD.TYPE.IO, "ECI0064", "工装平台1", "Fixture Platform1");
        public static CARD CARD_ECI0064_7 = new CARD(7, "192.168.0.105", 0, 32, 32, CARD.BRAND.ZMOTION, CARD.TYPE.IO, "ECI0064", "工装平台2", "Fixture Platform2");
        public static List<CARD> CardList1 = new List<CARD> { CARD_ECI2400_0, CARD_ECI2400_3, CARD_DMC3C00_4, CARD_ORIENTAL485_5, CARD_ECI0064_6, CARD_ECI0064_7 };
        public static List<CARD> CardList = new List<CARD> { CARD_ECI2400_0, CARD_ECI2600_1, CARD_ECI2400_2, CARD_ECI2400_3, CARD_DMC3C00_4, CARD_ORIENTAL485_5, CARD_ECI0064_6, CARD_ECI0064_7 };
        //public static List<CARD> CardList = new List<CARD> { CARD_ECI2400_0, CARD_ECI2400_1, CARD_ECI2400_2, CARD_ORIENTAL485_5, CARD_ECI0064_6, CARD_ECI0064_7 };
        #endregion

        #region 轴定义 
        //工位1
        //public static AXIS AXIS_WS1_F = new AXIS(1, CARD_ORIENTAL485_5, "工站1前排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_WS1_B = new AXIS(2, CARD_ORIENTAL485_5, "工站1后排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_WS1_U = new AXIS(1, CARD_ORIENTAL485_5, "工站1旋转", "WS1 Rotate", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //工位1
        //public static AXIS AXIS_WS2_F = new AXIS(4, CARD_ORIENTAL485_5, "工站2前排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_WS2_B = new AXIS(5, CARD_ORIENTAL485_5, "工站2后排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_WS2_U = new AXIS(2, CARD_ORIENTAL485_5, "工站2旋转", "WS2 Rotate", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //工位1
        //public static AXIS AXIS_WS3_F = new AXIS(7, CARD_ORIENTAL485_5, "工站3前排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_WS3_B = new AXIS(8, CARD_ORIENTAL485_5, "工站3后排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_WS3_U = new AXIS(3, CARD_ORIENTAL485_5, "工站3旋转", "WS3 Rotate", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //工位1
        //public static AXIS AXIS_WS4_F = new AXIS(10, CARD_ORIENTAL485_5, "工站4前排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_WS4_B = new AXIS(11, CARD_ORIENTAL485_5, "工站4后排", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_WS4_U = new AXIS(4, CARD_ORIENTAL485_5, "工站4旋转", "WS4 Rotate", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //OTP光箱
        public static AXIS AXIS_BOX_OTP_Z = new AXIS(0, CARD_ECI2400_0, "OTP_Z", "OTP_Z", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);


        //上下料
        public static AXIS AXIS_UDL1_X = new AXIS(1, CARD_DMC3C00_4, "上下料1_X", "UpDwLoad1_X", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL1_Y = new AXIS(0, CARD_DMC3C00_4, "上下料1_Y", "UpDwLoad1_Y", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL1_Z = new AXIS(2, CARD_DMC3C00_4, "上下料1_Z", "UpDwLoad1_Z", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL1_U2 = new AXIS(9, CARD_DMC3C00_4, "上下料1_U2", "UpDwLoad1_U2", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL1_U1 = new AXIS(8, CARD_DMC3C00_4, "上下料1_U1", "UpDwLoad1_U1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);

        public static AXIS AXIS_UDL2_X = new AXIS(4, CARD_DMC3C00_4, "上下料2_X", "UpDwLoad2_X", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL2_Y = new AXIS(3, CARD_DMC3C00_4, "上下料2_Y", "UpDwLoad2_Y", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL2_Z = new AXIS(5, CARD_DMC3C00_4, "上下料2_Z", "UpDwLoad2_Z", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL2_U2 = new AXIS(11, CARD_DMC3C00_4, "上下料2_U2", "UpDwLoad2_U2", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL2_U1 = new AXIS(10, CARD_DMC3C00_4, "上下料2_U1", "UpDwLoad2_U1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);

        public static AXIS AXIS_UDL_FD_X = new AXIS(0, CARD_ECI2400_3, "供料X", "FEED_X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL_FD_Z = new AXIS(3, CARD_ECI2400_3, "供料Z", "FEED_Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL_OK_X = new AXIS(1, CARD_ECI2400_3, "OK料X", "OK_X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL_OK_Z = new AXIS(6, CARD_DMC3C00_4, "OK料Z", "OK_Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL_NG_X = new AXIS(2, CARD_ECI2400_3, "NG料X", "NG_X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_UDL_NG_Z = new AXIS(7, CARD_DMC3C00_4, "NG料Z", "NG_Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //上下料
        //public static AXIS AXIS_UL_X = new AXIS(0, null, "上下料X", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_UL_Y = new AXIS(1, null, "上下料Y", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_UL_Z = new AXIS(2, null, "上下料Z", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);

        //public static AXIS AXIS_UL_U2 = new AXIS(3, null, "上下料U2", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_UL_U1 = new AXIS(4, null, "上下料U1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);

        //public static AXIS AXIS_UL_FD_X = new AXIS(5, null, "供料X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_UL_FD_Z = new AXIS(6, null, "供料Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_DL_OK_X = new AXIS(2, null, "OK料X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_DL_OK_Z = new AXIS(3, null, "OK料Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_DL_NG_X = new AXIS(4, null, "NG料X", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_DL_NG_Z = new AXIS(5, null, "NG料Z", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        ////下料
        //public static AXIS AXIS_DL_Y = new AXIS(0, null, "下料Y", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_DL_Z = new AXIS(1, null, "下料Z", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);


        //左光箱
        public static AXIS AXIS_BOX_L_X1 = new AXIS(0, CARD_ECI2600_1, "左光箱X1", "LB_X1", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_L_X2 = new AXIS(1, CARD_ECI2600_1, "左光箱X2", "LB_X2", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_L_Z1 = new AXIS(2, CARD_ECI2600_1, "左光箱Z1", "LB_Z1", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_L_Z2 = new AXIS(3, CARD_ECI2600_1, "左光箱Z2", "LB_Z2", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        //public static AXIS AXIS_BOX_L_Y1 = new AXIS(4, CARD_ECI2600_1, "左光箱Y1", AXIS.MT_TYPE.STEP, AXIS.ENC_TYPE.NO, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_L_Y1 = null;
        //右光箱
        public static AXIS AXIS_BOX_R_X1 = new AXIS(0, CARD_ECI2400_2, "右光箱X1", "RB_X1", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_R_X2 = new AXIS(1, CARD_ECI2400_2, "右光箱X2", "RB_X2", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_R_Z1 = new AXIS(2, CARD_ECI2400_2, "右光箱Z1", "RB_Z1", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_R_Z2 = new AXIS(3, CARD_ECI2400_2, "右光箱Z2", "RB_Z2", AXIS.MT_TYPE.SEVER, AXIS.ENC_TYPE.YES, GPIO.IO_STA.OUT_ON);
        public static AXIS AXIS_BOX_R_Y1 = null;
        //工站
        public static List<AXIS> AxList_WS = new List<AXIS> { AXIS_WS1_U, AXIS_WS2_U, AXIS_WS3_U, AXIS_WS4_U };
        public static List<AXIS> AxList_WS1 = new List<AXIS> { AXIS_WS1_U };
        public static List<AXIS> AxList_WS2 = new List<AXIS> { AXIS_WS2_U };
        public static List<AXIS> AxList_WS3 = new List<AXIS> { AXIS_WS3_U };
        public static List<AXIS> AxList_WS4 = new List<AXIS> { AXIS_WS4_U };
        //光箱
        public static List<AXIS> AxList_BOX_OPT = new List<AXIS> { AXIS_BOX_OTP_Z };
        public static List<AXIS> AxList_BOX_LEFT;//= new List<AXIS> { AXIS_BOX_L_X1, AXIS_BOX_L_X2, AXIS_BOX_L_Y1, AXIS_BOX_L_Z1, AXIS_BOX_L_Z2};
        public static List<AXIS> AxList_BOX_RIGHT;// = new List<AXIS> { AXIS_BOX_R_X1, AXIS_BOX_R_X2,AXIS_BOX_R_Y1,, AXIS_BOX_R_Z1, AXIS_BOX_R_Z2 };
                                                  //上料
                                                  // public static List<AXIS> AxList_UL = new List<AXIS> { AXIS_UL_X, AXIS_UL_Y, AXIS_UL_Z, AXIS_UL_U1, AXIS_UL_U2, AXIS_UL_FD_X, AXIS_UL_FD_Z };
                                                  //下料
                                                  //public static List<AXIS> AxList_DL = new List<AXIS> { AXIS_DL_Y, AXIS_DL_Z, AXIS_DL_OK_X, AXIS_DL_OK_Z, AXIS_DL_NG_X, AXIS_DL_NG_Z };
                                                  //上下料1
        public static List<AXIS> Axlist_UDL1 = new List<AXIS> { AXIS_UDL1_X, AXIS_UDL1_Y, AXIS_UDL1_Z, AXIS_UDL1_U1, AXIS_UDL1_U2, AXIS_UDL_FD_X, AXIS_UDL_OK_X, AXIS_UDL_NG_X };
        public static List<AXIS> Axlist_UDL1_ExpLC = new List<AXIS> { AXIS_UDL1_X, AXIS_UDL1_Y, AXIS_UDL1_Z, AXIS_UDL1_U1, AXIS_UDL1_U2 };
        //上下料2
        public static List<AXIS> Axlist_UDL2 = new List<AXIS> { AXIS_UDL2_X, AXIS_UDL2_Y, AXIS_UDL2_Z, AXIS_UDL2_U1, AXIS_UDL2_U2, AXIS_UDL_FD_X, AXIS_UDL_OK_X, AXIS_UDL_NG_X };
        public static List<AXIS> Axlist_UDL2_ExpLC = new List<AXIS> { AXIS_UDL2_X, AXIS_UDL2_Y, AXIS_UDL2_Z, AXIS_UDL2_U1, AXIS_UDL2_U2 };
        //料仓
        public static List<AXIS> Axlist_UDL_LC = new List<AXIS> { AXIS_UDL_FD_X, AXIS_UDL_OK_X, AXIS_UDL_NG_X, AXIS_UDL_FD_Z, AXIS_UDL_OK_Z, AXIS_UDL_NG_Z };
        #endregion

        #region IO 定义
        public static GPIO GPIO_OUT_ON_NULL = new GPIO(0, (CARD)null, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.NULL, "OUT_ON_NULL", "OUT_ON_NULL", GPIO.IO_STA.OUT_ON);
        public static GPIO GPIO_IN_ON_NULL = new GPIO(0, (CARD)null, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.NULL, "IN_ON_NULL", "IN_ON_NULL", GPIO.IO_STA.IN_ON);
        public static GPIO GPIO_OUT_OFF_NULL = new GPIO(0, (CARD)null, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.NULL, "OUT_OFF_NULL", "OUT_OFF_NULL", GPIO.IO_STA.OUT_OFF);
        public static GPIO GPIO_IN_OFF_NULL = new GPIO(0, (CARD)null, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.NULL, "IN_OFF_NULL", "IN_OFF_NULL", GPIO.IO_STA.IN_OFF);
        #region OUT  
        //上料
        //public static GPIO GPIO_OUT_UL_Z_RESET = new GPIO(0, CARD_DMC3800_5, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料Z轴复位");
        public static GPIO GPIO_OUT_UDL1_ZK_N1 = new GPIO(0, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料1吸头真空1", "XT1 vacuum");
        public static GPIO GPIO_OUT_UDL1_ZK_N2 = new GPIO(1, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料1吸头真空2", "XT2 vacuum");
        public static GPIO GPIO_OUT_UDL2_ZK_N1 = new GPIO(4, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料2吸头真空1", "XT3 vacuum");
        public static GPIO GPIO_OUT_UDL2_ZK_N2 = new GPIO(5, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料2吸头真空2", "XT4 vacuum");

        public static GPIO GPIO_OUT_UDL1_PZK_N1 = new GPIO(2, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料1吸头破真空1", "XT1 vacuum break");
        public static GPIO GPIO_OUT_UDL1_PZK_N2 = new GPIO(3, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料1吸头破真空2", "XT2 vacuum break");
        public static GPIO GPIO_OUT_UDL2_PZK_N1 = new GPIO(6, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料2吸头破真空1", "XT3 vacuum break");
        public static GPIO GPIO_OUT_UDL2_PZK_N2 = new GPIO(7, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上料2吸头破真空2", "XT4 vacuum break");

        public static GPIO GPIO_OUT_UL_FD_TRAY = new GPIO(4, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "供料料盘夹紧", "Feed Tray clamp");

        //按键
        public static GPIO GPIO_OUT_KL_START = new GPIO(8, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "开始按键灯", "Start Ctrl-DEL");
        public static GPIO GPIO_OUT_KL_STOP = new GPIO(10, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "停止按键灯", "Stop Ctrl-DEL");
        public static GPIO GPIO_OUT_KL_RESET = new GPIO(9, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "复位按键灯", "Reset Ctrl-DEL");
        //警报
        public static GPIO GPIO_OUT_ALM_RED = new GPIO(0, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "红色塔灯", "Red Alarm Light");
        public static GPIO GPIO_OUT_ALM_YELLOW = new GPIO(1, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "黄色塔灯", "Yellow Alarm Light");
        public static GPIO GPIO_OUT_ALM_GREEN = new GPIO(2, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "绿色塔灯", "Green Alarm Light");
        public static GPIO GPIO_OUT_ALM_BEEPER = new GPIO(3, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "蜂鸣器", "Alarm Beeper");
        //相机
        public static GPIO GPIO_OUT_UL_CAM_UP1 = new GPIO(12, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上相机1触发", "UpCam1 trigger");
        public static GPIO GPIO_OUT_UL_CAM_DW1 = new GPIO(13, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "下相机1触发", "DwCam1 trigger");
        public static GPIO GPIO_OUT_UL_CAM_UP2 = new GPIO(14, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "上相机2触发", "UpCam2 trigger");
        public static GPIO GPIO_OUT_UL_CAM_DW2 = new GPIO(15, CARD_DMC3C00_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "下相机2触发", "DwCam2 trigger");
        //下料
        public static GPIO GPIO_OUT_DL_OK_TRAY = new GPIO(5, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "OK料盘夹紧", "OK Tray clamp");
        public static GPIO GPIO_OUT_DL_NG_TRAY = new GPIO(6, CARD_ECI2400_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "NG料盘夹紧", "NG Tray clamp");
        ////夹爪
        //public static GPIO GPIO_OUT_DL_HD_HD1 = new GPIO(0, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪1");
        //public static GPIO GPIO_OUT_DL_HD_HD2 = new GPIO(1, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪2");
        //public static GPIO GPIO_OUT_DL_HD_HD3 = new GPIO(2, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪3");
        //public static GPIO GPIO_OUT_DL_HD_HD4 = new GPIO(3, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪4");
        //public static GPIO GPIO_OUT_DL_HD_HD5 = new GPIO(4, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪5");
        //public static GPIO GPIO_OUT_DL_HD_HD6 = new GPIO(5, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪6");
        //public static GPIO GPIO_OUT_DL_HD_HD7 = new GPIO(6, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪7");
        //public static GPIO GPIO_OUT_DL_HD_HD8 = new GPIO(7, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪8");
        //public static GPIO GPIO_OUT_DL_HD_HD9 = new GPIO(8, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪9");
        //public static GPIO GPIO_OUT_DL_HD_HD10 = new GPIO(9, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪10");
        //public static GPIO GPIO_OUT_DL_HD_HD11 = new GPIO(10, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪11");
        //public static GPIO GPIO_OUT_DL_HD_HD12 = new GPIO(11, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪12");
        //public static GPIO GPIO_OUT_DL_HD_HD13 = new GPIO(12, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪13");
        //public static GPIO GPIO_OUT_DL_HD_HD14 = new GPIO(13, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪14");
        //public static GPIO GPIO_OUT_DL_HD_HD15 = new GPIO(14, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪15");
        //public static GPIO GPIO_OUT_DL_HD_HD16 = new GPIO(15, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪16");
        ////夹爪上下
        //public static GPIO GPIO_OUT_DL_UD_HD1 = new GPIO(16, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下1");
        //public static GPIO GPIO_OUT_DL_UD_HD2 = new GPIO(17, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下2");
        //public static GPIO GPIO_OUT_DL_UD_HD3 = new GPIO(18, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下3");
        //public static GPIO GPIO_OUT_DL_UD_HD4 = new GPIO(19, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下4");
        //public static GPIO GPIO_OUT_DL_UD_HD5 = new GPIO(20, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下5");
        //public static GPIO GPIO_OUT_DL_UD_HD6 = new GPIO(21, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下6");
        //public static GPIO GPIO_OUT_DL_UD_HD7 = new GPIO(22, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下7");
        //public static GPIO GPIO_OUT_DL_UD_HD8 = new GPIO(23, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下8");
        //public static GPIO GPIO_OUT_DL_UD_HD9 = new GPIO(24, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下9");
        //public static GPIO GPIO_OUT_DL_UD_HD10 = new GPIO(25, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下10");
        //public static GPIO GPIO_OUT_DL_UD_HD11 = new GPIO(26, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下11");
        //public static GPIO GPIO_OUT_DL_UD_HD12 = new GPIO(27, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下12");
        //public static GPIO GPIO_OUT_DL_UD_HD13 = new GPIO(28, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下13");
        //public static GPIO GPIO_OUT_DL_UD_HD14 = new GPIO(29, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下14");
        //public static GPIO GPIO_OUT_DL_UD_HD15 = new GPIO(30, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下15");
        //public static GPIO GPIO_OUT_DL_UD_HD16 = new GPIO(31, CARD_ECI0064_4, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "下料夹爪上下16");
        //料盘
        //public static GPIO GPIO_OUT_DL_ZK_NG_TRAY = new GPIO(0, CARD_ECI2600_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "NG料盘真空");
        //public static GPIO GPIO_OUT_DL_ZK_OK_TRAY = new GPIO(1, CARD_ECI2600_3, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "OK料盘真空");

        //转台
        public static GPIO GPIO_OUT_TT_FWD = new GPIO(0, CARD_ECI2400_0, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "转台顺时针(FWD)信号", "Turn Table FWD");
        public static GPIO GPIO_OUT_TT_REV = new GPIO(1, CARD_ECI2400_0, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "转台逆时针(REV)信号", "Turn Table REV");
        public static GPIO GPIO_OUT_TT_RESET = new GPIO(2, CARD_ECI2400_0, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "转台复位(X1)信号", "Turn Table RESET");
        public static GPIO GPIO_OUT_TT_STOP = new GPIO(3, CARD_ECI2400_0, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "转台停止(X3)信号", "Turn Table STOP");

        //工站开压盖WS1
        public static GPIO GPIO_OUT_WS1_OPEN_FR1 = new GPIO(0, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排开盖1", "WS1 fixture1 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_FR2 = new GPIO(2, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排开盖2", "WS1 fixture2 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_FR3 = new GPIO(4, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排开盖3", "WS1 fixture3 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_FR4 = new GPIO(6, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排开盖4", "WS1 fixture4 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_BK1 = new GPIO(1, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排开盖5", "WS1 fixture5 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_BK2 = new GPIO(3, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排开盖6", "WS1 fixture6 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_BK3 = new GPIO(5, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排开盖7", "WS1 fixture7 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_OPEN_BK4 = new GPIO(7, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排开盖8", "WS1 fixture8 open", GPIO.IO_STA.NULL);


        public static GPIO GPIO_OUT_WS1_CLOSE_FR1 = new GPIO(8, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排压盖1", "WS1 fixture1 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_FR2 = new GPIO(10, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排压盖2", "WS1 fixture2 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_FR3 = new GPIO(12, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排压盖3", "WS1 fixture3 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_FR4 = new GPIO(14, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1前排压盖4", "WS1 fixture4 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_BK1 = new GPIO(9, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排压盖5", "WS1 fixture5 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_BK2 = new GPIO(11, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排压盖6", "WS1 fixture6 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_BK3 = new GPIO(13, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排压盖7", "WS1 fixture7 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_CLOSE_BK4 = new GPIO(15, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站1后排压盖8", "WS1 fixture8 close", GPIO.IO_STA.NULL);

        //工站开压盖WS2
        public static GPIO GPIO_OUT_WS2_OPEN_FR1 = new GPIO(16, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排开盖1", "WS2 fixture1 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_FR2 = new GPIO(18, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排开盖2", "WS2 fixture2 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_FR3 = new GPIO(20, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排开盖3", "WS2 fixture3 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_FR4 = new GPIO(22, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排开盖4", "WS2 fixture4 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_BK1 = new GPIO(17, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排开盖5", "WS2 fixture5 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_BK2 = new GPIO(19, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排开盖6", "WS2 fixture6 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_BK3 = new GPIO(21, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排开盖7", "WS2 fixture7 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_OPEN_BK4 = new GPIO(23, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排开盖8", "WS2 fixture8 open", GPIO.IO_STA.NULL);


        public static GPIO GPIO_OUT_WS2_CLOSE_FR1 = new GPIO(24, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排压盖1", "WS2 fixture1 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_FR2 = new GPIO(26, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排压盖2", "WS2 fixture2 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_FR3 = new GPIO(28, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排压盖3", "WS2 fixture3 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_FR4 = new GPIO(30, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2前排压盖4", "WS2 fixture4 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_BK1 = new GPIO(25, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排压盖5", "WS2 fixture5 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_BK2 = new GPIO(27, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排压盖6", "WS2 fixture6 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_BK3 = new GPIO(29, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排压盖7", "WS2 fixture7 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_CLOSE_BK4 = new GPIO(31, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站2后排压盖8", "WS2 fixture8 close", GPIO.IO_STA.NULL);

        //工站开压盖WS3
        public static GPIO GPIO_OUT_WS3_OPEN_FR1 = new GPIO(0, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排开盖1", "WS3 fixture1 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_FR2 = new GPIO(2, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排开盖2", "WS3 fixture2 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_FR3 = new GPIO(4, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排开盖3", "WS3 fixture3 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_FR4 = new GPIO(6, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排开盖4", "WS3 fixture4 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_BK1 = new GPIO(1, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排开盖5", "WS3 fixture5 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_BK2 = new GPIO(3, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排开盖6", "WS3 fixture6 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_BK3 = new GPIO(5, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排开盖7", "WS3 fixture7 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_OPEN_BK4 = new GPIO(7, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排开盖8", "WS3 fixture8 open", GPIO.IO_STA.NULL);


        public static GPIO GPIO_OUT_WS3_CLOSE_FR1 = new GPIO(8, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排压盖1", "WS3 fixture1 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_FR2 = new GPIO(10, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排压盖2", "WS3 fixture2 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_FR3 = new GPIO(12, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排压盖3", "WS3 fixture3 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_FR4 = new GPIO(14, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3前排压盖4", "WS3 fixture4 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_BK1 = new GPIO(9, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排压盖5", "WS3 fixture5 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_BK2 = new GPIO(11, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排压盖6", "WS3 fixture6 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_BK3 = new GPIO(13, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排压盖7", "WS3 fixture7 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_CLOSE_BK4 = new GPIO(15, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站3后排压盖8", "WS3 fixture8 close", GPIO.IO_STA.NULL);

        //工站开压盖WS4
        public static GPIO GPIO_OUT_WS4_OPEN_FR1 = new GPIO(16, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排开盖1", "WS4 fixture1 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_FR2 = new GPIO(18, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排开盖2", "WS4 fixture2 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_FR3 = new GPIO(20, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排开盖3", "WS4 fixture3 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_FR4 = new GPIO(22, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排开盖4", "WS4 fixture4 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_BK1 = new GPIO(17, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排开盖5", "WS4 fixture5 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_BK2 = new GPIO(19, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排开盖6", "WS4 fixture6 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_BK3 = new GPIO(21, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排开盖7", "WS4 fixture7 open", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_OPEN_BK4 = new GPIO(23, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排开盖8", "WS4 fixture8 open", GPIO.IO_STA.NULL);


        public static GPIO GPIO_OUT_WS4_CLOSE_FR1 = new GPIO(24, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排压盖1", "WS4 fixture1 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_FR2 = new GPIO(26, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排压盖2", "WS4 fixture2 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_FR3 = new GPIO(28, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排压盖3", "WS4 fixture3 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_FR4 = new GPIO(30, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4前排压盖4", "WS4 fixture4 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_BK1 = new GPIO(25, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排压盖5", "WS4 fixture5 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_BK2 = new GPIO(27, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排压盖6", "WS4 fixture6 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_BK3 = new GPIO(29, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排压盖7", "WS4 fixture7 close", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_CLOSE_BK4 = new GPIO(31, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工站4后排压盖8", "WS4 fixture8 close", GPIO.IO_STA.NULL);

        ////工装电源
        //public static GPIO GPIO_OUT_WS1_GZ_POWER = new GPIO(2, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1电源", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_GZ_POWER = new GPIO(2, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2电源", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS3_GZ_POWER = new GPIO(2, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3电源", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_GZ_POWER = new GPIO(2, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4电源", GPIO.IO_STA.NULL);
        //工装电源
        public static GPIO GPIO_OUT_WS1_GZ_POWER = new GPIO(2, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1电源", "WS1 fixture power", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_GZ_POWER = new GPIO(2, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2电源", "WS2 fixture power", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_GZ_POWER = new GPIO(2, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3电源", "WS3 fixture power", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_GZ_POWER = new GPIO(2, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4电源", "WS4 fixture power", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS1_Wind = new GPIO(3, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1吹气", "WS1 wind", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS2_Wind = new GPIO(3, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2吹气", "WS2 wind", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS3_Wind = new GPIO(3, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3吹气", "WS3 wind", GPIO.IO_STA.NULL);
        public static GPIO GPIO_OUT_WS4_Wind = new GPIO(3, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4吹气", "WS4 wind", GPIO.IO_STA.NULL);

        //public static GPIO GPIO_OUT_WS1_GZ_POWER_FR1 = new GPIO(10, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装1前排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS1_GZ_POWER_FR2 = new GPIO(11, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装1前排电源2", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS1_GZ_POWER_BK1 = new GPIO(12, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装1后排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS1_GZ_POWER_BK2 = new GPIO(13, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装1后排电源2", GPIO.IO_STA.NULL);
        //public static List<GPIO> List_GPIO_OUT_WS_GZ_POWER1 = new List<GPIO> { GPIO_OUT_WS1_GZ_POWER_FR1, GPIO_OUT_WS1_GZ_POWER_FR2, GPIO_OUT_WS1_GZ_POWER_BK1, GPIO_OUT_WS1_GZ_POWER_BK2 };

        //public static GPIO GPIO_OUT_WS2_GZ_POWER_FR1 = new GPIO(24, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装2前排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_GZ_POWER_FR2 = new GPIO(25, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装2前排电源2", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_GZ_POWER_BK1 = new GPIO(26, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装2后排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_GZ_POWER_BK2 = new GPIO(27, CARD_ECI0064_6, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装2后排电源2", GPIO.IO_STA.NULL);
        //public static List<GPIO> List_GPIO_OUT_WS_GZ_POWER2 = new List<GPIO> { GPIO_OUT_WS2_GZ_POWER_FR1, GPIO_OUT_WS2_GZ_POWER_FR2, GPIO_OUT_WS2_GZ_POWER_BK1, GPIO_OUT_WS2_GZ_POWER_BK2 };

        //public static GPIO GPIO_OUT_WS3_GZ_POWER_FR1 = new GPIO(10, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装3前排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS3_GZ_POWER_FR2 = new GPIO(11, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装3前排电源2", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS3_GZ_POWER_BK1 = new GPIO(12, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装3后排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS3_GZ_POWER_BK2 = new GPIO(13, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装3后排电源2", GPIO.IO_STA.NULL);
        //public static List<GPIO> List_GPIO_OUT_WS_GZ_POWER3 = new List<GPIO> { GPIO_OUT_WS3_GZ_POWER_FR1, GPIO_OUT_WS3_GZ_POWER_FR2, GPIO_OUT_WS3_GZ_POWER_BK1, GPIO_OUT_WS3_GZ_POWER_BK2 };

        //public static GPIO GPIO_OUT_WS4_GZ_POWER_FR1 = new GPIO(24, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装4前排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_GZ_POWER_FR2 = new GPIO(25, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装4前排电源2", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_GZ_POWER_BK1 = new GPIO(26, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装4后排电源1", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_GZ_POWER_BK2 = new GPIO(27, CARD_ECI0064_7, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.IO_CARD, "工装4后排电源2", GPIO.IO_STA.NULL);
        //public static List<GPIO> List_GPIO_OUT_WS_GZ_POWER4 = new List<GPIO> { GPIO_OUT_WS4_GZ_POWER_FR1, GPIO_OUT_WS4_GZ_POWER_FR2, GPIO_OUT_WS4_GZ_POWER_BK1, GPIO_OUT_WS4_GZ_POWER_BK2 };

        ////前排（外）真空
        //public static GPIO GPIO_OUT_WS1_ZK_FR = new GPIO(1, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1外真空", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_ZK_FR = new GPIO(1, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2外真空", GPIO.IO_STA.NULL,true);
        //public static GPIO GPIO_OUT_WS3_ZK_FR = new GPIO(1, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3外真空", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_ZK_FR = new GPIO(1, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4外真空", GPIO.IO_STA.NULL);

        ////后排（里）真空
        //public static GPIO GPIO_OUT_WS1_ZK_BK = new GPIO(0, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1里真空", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS2_ZK_BK = new GPIO(0, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2里真空", GPIO.IO_STA.NULL, true);
        //public static GPIO GPIO_OUT_WS3_ZK_BK = new GPIO(0, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3里真空", GPIO.IO_STA.NULL);
        //public static GPIO GPIO_OUT_WS4_ZK_BK = new GPIO(0, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4里真空", GPIO.IO_STA.NULL);
        //前排（外）真空
        public static GPIO GPIO_OUT_WS1_ZK_FR = new GPIO(1, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1外真空", "WS1 outside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS2_ZK_FR = new GPIO(1, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2外真空", "WS1 outside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS3_ZK_FR = new GPIO(1, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3外真空", "WS1 outside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS4_ZK_FR = new GPIO(1, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4外真空", "WS1 outside vacuum", GPIO.IO_STA.NULL, true);

        //后排（里）真空
        public static GPIO GPIO_OUT_WS1_ZK_BK = new GPIO(0, AXIS_WS1_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装1里真空", "WS1 inside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS2_ZK_BK = new GPIO(0, AXIS_WS2_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装2里真空", "WS1 inside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS3_ZK_BK = new GPIO(0, AXIS_WS3_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装3里真空", "WS1 inside vacuum", GPIO.IO_STA.NULL, true);
        public static GPIO GPIO_OUT_WS4_ZK_BK = new GPIO(0, AXIS_WS4_U, GPIO.IO_DIR.OUT, GPIO.IO_TYPE.MT_CARD, "工装4里真空", "WS1 inside vacuum", GPIO.IO_STA.NULL, true);
        public static List<GPIO> List_GPIO_OUT_WS1_ZK = new List<GPIO> { GPIO_OUT_WS1_ZK_FR, GPIO_OUT_WS1_ZK_BK };
        public static List<GPIO> List_GPIO_OUT_WS2_ZK = new List<GPIO> { GPIO_OUT_WS2_ZK_FR, GPIO_OUT_WS2_ZK_BK };
        public static List<GPIO> List_GPIO_OUT_WS3_ZK = new List<GPIO> { GPIO_OUT_WS3_ZK_FR, GPIO_OUT_WS3_ZK_BK };
        public static List<GPIO> List_GPIO_OUT_WS4_ZK = new List<GPIO> { GPIO_OUT_WS4_ZK_FR, GPIO_OUT_WS4_ZK_BK };

        #endregion
        #region IN
        //急停
        public static GPIO GPIO_IN_EMG0 = new GPIO(0, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "急停键(转台)", "EMG(Turn Table)");
        public static GPIO GPIO_IN_EMG1 = new GPIO(0, CARD_ECI2600_1, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "急停键(左光箱)", "EMG(Left LightBox)");
        public static GPIO GPIO_IN_EMG2 = new GPIO(0, CARD_ECI2400_2, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "急停键(右光箱)", "EMG(Right LightBox)");
        public static GPIO GPIO_IN_EMG3 = new GPIO(0, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "急停键(上下料)", "EMG(UpDwLoad)");
        //public static GPIO GPIO_IN_EMG5 = new GPIO(13, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "急停键（上料）");
        //上料
        // public static GPIO GPIO_IN_UL_Z_RSTOK = new GPIO(1, CARD_DMC3800_5, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上料Z轴复位完成");
        public static GPIO GPIO_IN_UDL1_ZK_N1 = new GPIO(6, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上料1吸头真空感应1", "XT1 vacuum sense");
        public static GPIO GPIO_IN_UDL1_ZK_N2 = new GPIO(7, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上料1吸头真空感应2", "XT2 vacuum sense");
        public static GPIO GPIO_IN_UDL2_ZK_N1 = new GPIO(8, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上料2吸头真空感应1", "XT3 vacuum sense");
        public static GPIO GPIO_IN_UDL2_ZK_N2 = new GPIO(9, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上料2吸头真空感应2", "XT4 vacuum sense");
        //public static GPIO GPIO_IN_UL_INP_FD_TRAYBOX = new GPIO(13,CARD_DMC3800_5, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料仓在位感应");
        //public static GPIO GPIO_IN_UL_RDY_FD_TRAY = new GPIO(13,CARD_ECI2600_3 , GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料仓有料感应");
        public static GPIO GPIO_IN_UDL_FD_TRAY_ON = new GPIO(10, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料盘夹紧感应", "Feed Tray clamp sense");
        public static GPIO GPIO_IN_UDL_FD_TRAY_OFF = new GPIO(11, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料盘缩回感应", "Feed Tray draw back sense");
        //按键
        public static GPIO GPIO_IN_KEY_START = new GPIO(10, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "开始键", "Start");
        public static GPIO GPIO_IN_KEY_STOP = new GPIO(12, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "停止键", "Stop");
        public static GPIO GPIO_IN_KEY_RESET = new GPIO(11, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "复位键", "Reset");
        //门禁        
        public static GPIO GPIO_IN_UDL_CC_DOOR = new GPIO(1, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上下料仓储门禁", "UDL storage entrance Guard");
        public static GPIO GPIO_IN_UDL_FR_DOOR = new GPIO(14, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上下料前门门禁", "UDL front door entrance Guard");
        public static GPIO GPIO_IN_UDL_VIEW_DOOR = new GPIO(16, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上下料参观门禁", "UDL view entrance Guard");
        public static GPIO GPIO_IN_UDL_BR_DOOR = new GPIO(17, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上下料后门门禁", "UDL behind door entrance Guard");
        public static List<GPIO> List_UDL_DOOR = new List<GPIO> { GPIO_IN_UDL_FR_DOOR, GPIO_IN_UDL_VIEW_DOOR, GPIO_IN_UDL_BR_DOOR };

        //下料
        public static GPIO GPIO_IN_UDL_OK_TRAY_ON = new GPIO(12, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "OK料盘夹紧感应", "OK Tray clamp sense");
        public static GPIO GPIO_IN_UDL_OK_TRAY_OFF = new GPIO(13, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "OK料盘缩回感应", "OK Tray draw back sense");
        public static GPIO GPIO_IN_UDL_NG_TRAY_ON = new GPIO(14, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "NG料盘夹紧感应", "NG Tray clamp sense");
        public static GPIO GPIO_IN_UDL_NG_TRAY_OFF = new GPIO(15, CARD_ECI2400_3, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "NG料盘缩回感应", "NG Tray draw back sense");
        //夹爪夹位感应
        //public static GPIO GPIO_IN_DL_HD_HD1 = new GPIO(0, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应1");
        //public static GPIO GPIO_IN_DL_HD_HD2 = new GPIO(1, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应2");
        //public static GPIO GPIO_IN_DL_HD_HD3 = new GPIO(2, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应3");
        //public static GPIO GPIO_IN_DL_HD_HD4 = new GPIO(3, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应4");
        //public static GPIO GPIO_IN_DL_HD_HD5 = new GPIO(4, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应5");
        //public static GPIO GPIO_IN_DL_HD_HD6 = new GPIO(5, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应6");
        //public static GPIO GPIO_IN_DL_HD_HD7 = new GPIO(6, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应7");
        //public static GPIO GPIO_IN_DL_HD_HD8 = new GPIO(7, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应8");
        //public static GPIO GPIO_IN_DL_HD_HD9 = new GPIO(8, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应9");
        //public static GPIO GPIO_IN_DL_HD_HD10 = new GPIO(9, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应10");
        //public static GPIO GPIO_IN_DL_HD_HD11 = new GPIO(10, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应11");
        //public static GPIO GPIO_IN_DL_HD_HD12 = new GPIO(11, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应12");
        //public static GPIO GPIO_IN_DL_HD_HD13 = new GPIO(12, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应13");
        //public static GPIO GPIO_IN_DL_HD_HD14 = new GPIO(13, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应14");
        //public static GPIO GPIO_IN_DL_HD_HD15 = new GPIO(14, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应15");
        //public static GPIO GPIO_IN_DL_HD_HD16 = new GPIO(15, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料真空感应16");
        ////夹爪下位感应
        //public static GPIO GPIO_IN_DL_DW_HD1 = new GPIO(16, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应1");
        //public static GPIO GPIO_IN_DL_DW_HD2 = new GPIO(17, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应2");
        //public static GPIO GPIO_IN_DL_DW_HD3 = new GPIO(18, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应3");
        //public static GPIO GPIO_IN_DL_DW_HD4 = new GPIO(19, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应4");
        //public static GPIO GPIO_IN_DL_DW_HD5 = new GPIO(20, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应5");
        //public static GPIO GPIO_IN_DL_DW_HD6 = new GPIO(21, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应6");
        //public static GPIO GPIO_IN_DL_DW_HD7 = new GPIO(22, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应7");
        //public static GPIO GPIO_IN_DL_DW_HD8 = new GPIO(23, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应8");
        //public static GPIO GPIO_IN_DL_DW_HD9 = new GPIO(24, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应9");
        //public static GPIO GPIO_IN_DL_DW_HD10 = new GPIO(25, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应10");
        //public static GPIO GPIO_IN_DL_DW_HD11 = new GPIO(26, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应11");
        //public static GPIO GPIO_IN_DL_DW_HD12 = new GPIO(27, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应12");
        //public static GPIO GPIO_IN_DL_DW_HD13 = new GPIO(28, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应13");
        //public static GPIO GPIO_IN_DL_DW_HD14 = new GPIO(29, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应14");
        //public static GPIO GPIO_IN_DL_DW_HD15 = new GPIO(30, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应15");
        //public static GPIO GPIO_IN_DL_DW_HD16 = new GPIO(31, CARD_ECI0064_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.IO_CARD, "下料夹爪下位感应16");

        //料夹在位
        public static GPIO GPIO_IN_UL_INP_FD_TRAYBOX = new GPIO(13, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料仓在位感应", "Feed TrayBox INP");
        public static GPIO GPIO_IN_DL_INP_OK_TRAYBOX = new GPIO(1, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "OK料仓在位感应", "OK TrayBox INP");
        public static GPIO GPIO_IN_DL_INP_NG_TRAYBOX = new GPIO(2, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "NG料仓在位感应", "NG TrayBox INP");

        //料夹有料
        public static GPIO GPIO_IN_UL_RDY_FD_TRAY = new GPIO(3, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "供料仓有料感应", "Feed TrayBox Ready");
        public static GPIO GPIO_IN_DL_RDY_OK_TRAY = new GPIO(4, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "OK料仓有料感应", "OK TrayBox Ready");
        public static GPIO GPIO_IN_DL_RDY_NG_TRAY = new GPIO(5, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "NG料仓有料感应", "NG TrayBox Ready");
        //料盘真空
        //public static GPIO GPIO_IN_DL_ZK_NG_TRAY = new GPIO(15, CARD_DMC3800_5, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "NG料盘真空感应");
        //public static GPIO GPIO_IN_DL_ZK_OK_TRAY = new GPIO(14, CARD_DMC3800_5, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "OK料盘真空感应");
        //public static GPIO GPIO_IN_UD_DOOR = new GPIO(14, CARD_DMC3C00_4, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "上下料门感应");

        //转台        
        public static GPIO GPIO_IN_TT_ALM = new GPIO(3, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台报警(30B)信号", "Turn Table Alarm");
        public static GPIO GPIO_IN_TT_INP = new GPIO(4, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台到位(X2)信号", "Turn Table INP");
        public static GPIO GPIO_IN_TT_SEN90 = new GPIO(6, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台90°信号", "Turn Table sen90");
        public static GPIO GPIO_IN_TT_SEN0 = new GPIO(5, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台0°信号", "Turn Table sen0");
        public static GPIO GPIO_IN_TT_SEN270 = new GPIO(7, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台270°信号", "Turn Table sen270");
        public static GPIO GPIO_IN_TT_MOVE = new GPIO(8, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台转动(Y1)信号", "Turn Table Move");

        //转盘门禁
        public static GPIO GPIO_IN_TT_DOOR = new GPIO(9, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台门后门门禁", "Turn Table behind door entrance Guard");
        public static GPIO GPIO_IN_TT_LEFT_DOOR = new GPIO(11, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台门左门门禁", "Turn Table left door entrance Guard");
        public static GPIO GPIO_IN_TT_RIGHT_DOOR = new GPIO(12, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台门右门门禁", "Turn Table right door entrance Guard");
        public static GPIO GPIO_IN_TT_LIGHT = new GPIO(10, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "转台门光幕", "Turn Table light curtain entrance Guard");
        public static List<GPIO> List_TT_DOOR1 = new List<GPIO> { GPIO_IN_TT_DOOR, GPIO_IN_TT_LIGHT, GPIO_IN_TT_LEFT_DOOR, GPIO_IN_TT_RIGHT_DOOR };
        public static List<GPIO> List_TT_DOOR2 = new List<GPIO> { GPIO_IN_TT_DOOR, GPIO_IN_TT_LIGHT };

        //工站开盖WS1
        public static GPIO GPIO_IN_WS1_OPEN_FR1 = new GPIO(8, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排开盖1", "WS1 fixture1 open");
        public static GPIO GPIO_IN_WS1_OPEN_FR2 = new GPIO(9, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排开盖2", "WS1 fixture2 open");
        public static GPIO GPIO_IN_WS1_OPEN_FR3 = new GPIO(10, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排开盖3", "WS1 fixture3 open");
        public static GPIO GPIO_IN_WS1_OPEN_FR4 = new GPIO(11, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排开盖4", "WS1 fixture4 open");
        public static GPIO GPIO_IN_WS1_OPEN_BK1 = new GPIO(12, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排开盖5", "WS1 fixture5 open");
        public static GPIO GPIO_IN_WS1_OPEN_BK2 = new GPIO(13, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排开盖6", "WS1 fixture6 open");
        public static GPIO GPIO_IN_WS1_OPEN_BK3 = new GPIO(14, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排开盖7", "WS1 fixture7 open");
        public static GPIO GPIO_IN_WS1_OPEN_BK4 = new GPIO(15, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排开盖8", "WS1 fixture8 open");

        //工站开盖WS2
        public static GPIO GPIO_IN_WS2_OPEN_FR1 = new GPIO(24, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排开盖1", "WS2 fixture1 open");
        public static GPIO GPIO_IN_WS2_OPEN_FR2 = new GPIO(25, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排开盖2", "WS2 fixture2 open");
        public static GPIO GPIO_IN_WS2_OPEN_FR3 = new GPIO(26, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排开盖3", "WS2 fixture3 open");
        public static GPIO GPIO_IN_WS2_OPEN_FR4 = new GPIO(27, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排开盖4", "WS2 fixture4 open");
        public static GPIO GPIO_IN_WS2_OPEN_BK1 = new GPIO(28, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排开盖5", "WS2 fixture5 open");
        public static GPIO GPIO_IN_WS2_OPEN_BK2 = new GPIO(29, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排开盖6", "WS2 fixture6 open");
        public static GPIO GPIO_IN_WS2_OPEN_BK3 = new GPIO(30, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排开盖7", "WS2 fixture7 open");
        public static GPIO GPIO_IN_WS2_OPEN_BK4 = new GPIO(31, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排开盖8", "WS2 fixture8 open");

        //工站开盖WS3
        public static GPIO GPIO_IN_WS3_OPEN_FR1 = new GPIO(8, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排开盖1", "WS3 fixture1 open");
        public static GPIO GPIO_IN_WS3_OPEN_FR2 = new GPIO(9, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排开盖2", "WS3 fixture2 open");
        public static GPIO GPIO_IN_WS3_OPEN_FR3 = new GPIO(10, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排开盖3", "WS3 fixture3 open");
        public static GPIO GPIO_IN_WS3_OPEN_FR4 = new GPIO(11, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排开盖4", "WS3 fixture4 open");
        public static GPIO GPIO_IN_WS3_OPEN_BK1 = new GPIO(12, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排开盖5", "WS3 fixture5 open");
        public static GPIO GPIO_IN_WS3_OPEN_BK2 = new GPIO(13, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排开盖6", "WS3 fixture6 open");
        public static GPIO GPIO_IN_WS3_OPEN_BK3 = new GPIO(14, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排开盖7", "WS3 fixture7 open");
        public static GPIO GPIO_IN_WS3_OPEN_BK4 = new GPIO(15, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排开盖8", "WS3 fixture8 open");

        //工站开盖WS4
        public static GPIO GPIO_IN_WS4_OPEN_FR1 = new GPIO(24, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排开盖1", "WS4 fixture1 open");
        public static GPIO GPIO_IN_WS4_OPEN_FR2 = new GPIO(25, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排开盖2", "WS4 fixture2 open");
        public static GPIO GPIO_IN_WS4_OPEN_FR3 = new GPIO(26, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排开盖3", "WS4 fixture3 open");
        public static GPIO GPIO_IN_WS4_OPEN_FR4 = new GPIO(27, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排开盖4", "WS4 fixture4 open");
        public static GPIO GPIO_IN_WS4_OPEN_BK1 = new GPIO(28, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排开盖5", "WS4 fixture5 open");
        public static GPIO GPIO_IN_WS4_OPEN_BK2 = new GPIO(29, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排开盖6", "WS4 fixture6 open");
        public static GPIO GPIO_IN_WS4_OPEN_BK3 = new GPIO(30, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排开盖7", "WS4 fixture7 open");
        public static GPIO GPIO_IN_WS4_OPEN_BK4 = new GPIO(31, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排开盖8", "WS4 fixture8 open");

        //工站关盖WS1
        public static GPIO GPIO_IN_WS1_CLOSE_FR1 = new GPIO(0, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排关盖1", "WS1 fixture1 close");
        public static GPIO GPIO_IN_WS1_CLOSE_FR2 = new GPIO(1, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排关盖2", "WS1 fixture2 close");
        public static GPIO GPIO_IN_WS1_CLOSE_FR3 = new GPIO(2, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排关盖3", "WS1 fixture3 close");
        public static GPIO GPIO_IN_WS1_CLOSE_FR4 = new GPIO(3, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1前排关盖4", "WS1 fixture4 close");
        public static GPIO GPIO_IN_WS1_CLOSE_BK1 = new GPIO(4, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排关盖5", "WS1 fixture5 close");
        public static GPIO GPIO_IN_WS1_CLOSE_BK2 = new GPIO(5, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排关盖6", "WS1 fixture6 close");
        public static GPIO GPIO_IN_WS1_CLOSE_BK3 = new GPIO(6, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排关盖7", "WS1 fixture7 close");
        public static GPIO GPIO_IN_WS1_CLOSE_BK4 = new GPIO(7, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站1后排关盖8", "WS1 fixture8 close");

        //工站关盖WS2
        public static GPIO GPIO_IN_WS2_CLOSE_FR1 = new GPIO(16, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排关盖1", "WS2 fixture1 close");
        public static GPIO GPIO_IN_WS2_CLOSE_FR2 = new GPIO(17, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排关盖2", "WS2 fixture2 close");
        public static GPIO GPIO_IN_WS2_CLOSE_FR3 = new GPIO(18, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排关盖3", "WS2 fixture3 close");
        public static GPIO GPIO_IN_WS2_CLOSE_FR4 = new GPIO(19, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2前排关盖4", "WS2 fixture4 close");
        public static GPIO GPIO_IN_WS2_CLOSE_BK1 = new GPIO(20, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排关盖5", "WS2 fixture5 close");
        public static GPIO GPIO_IN_WS2_CLOSE_BK2 = new GPIO(21, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排关盖6", "WS2 fixture6 close");
        public static GPIO GPIO_IN_WS2_CLOSE_BK3 = new GPIO(22, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排关盖7", "WS2 fixture7 close");
        public static GPIO GPIO_IN_WS2_CLOSE_BK4 = new GPIO(23, CARD_ECI0064_6, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站2后排关盖8", "WS2 fixture8 close");

        //工站关盖WS3
        public static GPIO GPIO_IN_WS3_CLOSE_FR1 = new GPIO(0, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排关盖1", "WS3 fixture1 close");
        public static GPIO GPIO_IN_WS3_CLOSE_FR2 = new GPIO(1, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排关盖2", "WS3 fixture2 close");
        public static GPIO GPIO_IN_WS3_CLOSE_FR3 = new GPIO(2, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排关盖3", "WS3 fixture3 close");
        public static GPIO GPIO_IN_WS3_CLOSE_FR4 = new GPIO(3, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3前排关盖4", "WS3 fixture4 close");
        public static GPIO GPIO_IN_WS3_CLOSE_BK1 = new GPIO(4, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排关盖5", "WS3 fixture5 close");
        public static GPIO GPIO_IN_WS3_CLOSE_BK2 = new GPIO(5, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排关盖6", "WS3 fixture6 close");
        public static GPIO GPIO_IN_WS3_CLOSE_BK3 = new GPIO(6, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排关盖7", "WS3 fixture7 close");
        public static GPIO GPIO_IN_WS3_CLOSE_BK4 = new GPIO(7, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站3后排关盖8", "WS3 fixture8 close");

        //工站关盖WS4
        public static GPIO GPIO_IN_WS4_CLOSE_FR1 = new GPIO(16, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排关盖1", "WS4 fixture1 close");
        public static GPIO GPIO_IN_WS4_CLOSE_FR2 = new GPIO(17, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排关盖2", "WS4 fixture2 close");
        public static GPIO GPIO_IN_WS4_CLOSE_FR3 = new GPIO(18, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排关盖3", "WS4 fixture3 close");
        public static GPIO GPIO_IN_WS4_CLOSE_FR4 = new GPIO(19, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4前排关盖4", "WS4 fixture4 close");
        public static GPIO GPIO_IN_WS4_CLOSE_BK1 = new GPIO(20, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排关盖5", "WS4 fixture5 close");
        public static GPIO GPIO_IN_WS4_CLOSE_BK2 = new GPIO(21, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排关盖6", "WS4 fixture6 close");
        public static GPIO GPIO_IN_WS4_CLOSE_BK3 = new GPIO(22, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排关盖7", "WS4 fixture7 close");
        public static GPIO GPIO_IN_WS4_CLOSE_BK4 = new GPIO(23, CARD_ECI0064_7, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "工站4后排关盖8", "WS4 fixture8 close");

        //左光箱门禁 
        public static GPIO GPIO_IN_LLB_LEFT_DOOR = new GPIO(1, CARD_ECI2600_1, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "左光箱后门门禁", "Left LightBox behind door entrance Guard");
        public static GPIO GPIO_IN_LLB_BACK_DOOR = new GPIO(2, CARD_ECI2600_1, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "左光箱前门门禁", "Left LightBox front door entrance Guard");
        public static GPIO GPIO_IN_LLB_FRONT_DOOR = new GPIO(3, CARD_ECI2600_1, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "左光箱电控板门禁", "Left LightBox left box entrance Guard");
        public static List<GPIO> List_LLB_DOOR = new List<GPIO> { GPIO_IN_LLB_LEFT_DOOR, GPIO_IN_LLB_BACK_DOOR, GPIO_IN_LLB_FRONT_DOOR };

        //右光箱门禁
        public static GPIO GPIO_IN_RLB_LEFT_DOOR = new GPIO(1, CARD_ECI2400_2, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "右光箱前门门禁", "Right LightBox front door entrance Guard");
        public static GPIO GPIO_IN_RLB_BACK_DOOR = new GPIO(2, CARD_ECI2400_2, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "右光箱后门门禁", "Right LightBox behind door entrance Guard");
        public static GPIO GPIO_IN_RLB_FRONT_DOOR = new GPIO(3, CARD_ECI2400_2, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "右光箱电控板门禁", "Right LightBox tight box entrance Guard");
        public static List<GPIO> List_RLB_DOOR = new List<GPIO> { GPIO_IN_RLB_LEFT_DOOR, GPIO_IN_RLB_BACK_DOOR, GPIO_IN_RLB_FRONT_DOOR };

        //压盖检测光幕
        public static GPIO GPIO_IN_LIGHT = new GPIO(13, CARD_ECI2400_0, GPIO.IO_DIR.IN, GPIO.IO_TYPE.MT_CARD, "压盖检测光幕", "Close fixture check light curtain");
        #endregion

        #region 串口定义
        //public static BarcodeScanner COM3 = new BarcodeScanner("模块1扫码器", "COM3", 115200);
        //public static BarcodeScanner COM4 = new BarcodeScanner("模块2扫码器", "COM4", 115200);
        public static IlluminationController COM2 = new IlluminationController("右光箱其他光源", "COM2", 57600);
        public static IlluminationController COM6 = new IlluminationController("轩十佳OTP光源串口", "COM6", 19200);
        public static IlluminationController COM7 = new IlluminationController("轩十佳右光箱窗串口", "COM7", 19200);

        public static BarcodeScanner COM3 = new BarcodeScanner("模块1扫码器", "COM3", 115200);
        public static BarcodeScanner COM4 = new BarcodeScanner("模块2扫码器", "COM4", 115200);

        #endregion

        public static void BeQuitEn(bool ben = false, bool sysbquit = false)
        {
            if (sysbquit) VAR.gsys_set.bquit = ben;
            if (!WS.bpause)
                WS.bquit = ben;
            UpDownLoad.bquit = ben;
            // WS.bpause = ben;
        }
        #region 气缸定义
        //上下料
        public static Cylinder CLD_UDL1_N1 = new Cylinder(GPIO_OUT_UDL1_ZK_N1, GPIO_IN_UDL1_ZK_N1);
        public static Cylinder CLD_UDL1_N2 = new Cylinder(GPIO_OUT_UDL1_ZK_N2, GPIO_IN_UDL1_ZK_N2);
        public static Cylinder CLD_UDL2_N1 = new Cylinder(GPIO_OUT_UDL2_ZK_N1, GPIO_IN_UDL2_ZK_N1);
        public static Cylinder CLD_UDL2_N2 = new Cylinder(GPIO_OUT_UDL2_ZK_N2, GPIO_IN_UDL2_ZK_N2);
        //料盘
        public static Cylinder CLD_UDL_FDTRAY_HD = new Cylinder(GPIO_OUT_UL_FD_TRAY, GPIO_IN_UDL_FD_TRAY_ON, GPIO_IN_UDL_FD_TRAY_OFF);

        //下料
        public static Cylinder CLD_UDL_OKTRAY_HD = new Cylinder(GPIO_OUT_DL_OK_TRAY, GPIO_IN_UDL_OK_TRAY_ON, GPIO_IN_UDL_OK_TRAY_OFF);
        public static Cylinder CLD_UDL_NGTRAY_HD = new Cylinder(GPIO_OUT_DL_NG_TRAY, GPIO_IN_UDL_NG_TRAY_ON, GPIO_IN_UDL_NG_TRAY_OFF);
        ////夹爪
        //public static Cylinder CLD_DL_HD_HD1 = new Cylinder(GPIO_OUT_DL_HD_HD1, null, GPIO_IN_DL_HD_HD1);
        //public static Cylinder CLD_DL_HD_HD2 = new Cylinder(GPIO_OUT_DL_HD_HD2, null, GPIO_IN_DL_HD_HD2);
        //public static Cylinder CLD_DL_HD_HD3 = new Cylinder(GPIO_OUT_DL_HD_HD3, null, GPIO_IN_DL_HD_HD3);
        //public static Cylinder CLD_DL_HD_HD4 = new Cylinder(GPIO_OUT_DL_HD_HD4, null, GPIO_IN_DL_HD_HD4);
        //public static Cylinder CLD_DL_HD_HD5 = new Cylinder(GPIO_OUT_DL_HD_HD5, null, GPIO_IN_DL_HD_HD5);
        //public static Cylinder CLD_DL_HD_HD6 = new Cylinder(GPIO_OUT_DL_HD_HD6, null, GPIO_IN_DL_HD_HD6);
        //public static Cylinder CLD_DL_HD_HD7 = new Cylinder(GPIO_OUT_DL_HD_HD7, null, GPIO_IN_DL_HD_HD7);
        //public static Cylinder CLD_DL_HD_HD8 = new Cylinder(GPIO_OUT_DL_HD_HD8, null, GPIO_IN_DL_HD_HD8);
        //public static Cylinder CLD_DL_HD_HD9 = new Cylinder(GPIO_OUT_DL_HD_HD9, null, GPIO_IN_DL_HD_HD9);
        //public static Cylinder CLD_DL_HD_HD10 = new Cylinder(GPIO_OUT_DL_HD_HD10, null, GPIO_IN_DL_HD_HD10);
        //public static Cylinder CLD_DL_HD_HD11 = new Cylinder(GPIO_OUT_DL_HD_HD11, null, GPIO_IN_DL_HD_HD11);
        //public static Cylinder CLD_DL_HD_HD12 = new Cylinder(GPIO_OUT_DL_HD_HD12, null, GPIO_IN_DL_HD_HD12);
        //public static Cylinder CLD_DL_HD_HD13 = new Cylinder(GPIO_OUT_DL_HD_HD13, null, GPIO_IN_DL_HD_HD13);
        //public static Cylinder CLD_DL_HD_HD14 = new Cylinder(GPIO_OUT_DL_HD_HD14, null, GPIO_IN_DL_HD_HD14);
        //public static Cylinder CLD_DL_HD_HD15 = new Cylinder(GPIO_OUT_DL_HD_HD15, null, GPIO_IN_DL_HD_HD15);
        //public static Cylinder CLD_DL_HD_HD16 = new Cylinder(GPIO_OUT_DL_HD_HD16, null, GPIO_IN_DL_HD_HD16);
        //public static List<Cylinder> List_CLD_HD_HD = new List<Cylinder> { CLD_DL_HD_HD1, CLD_DL_HD_HD2, CLD_DL_HD_HD3, CLD_DL_HD_HD4, CLD_DL_HD_HD5, CLD_DL_HD_HD6, CLD_DL_HD_HD7, CLD_DL_HD_HD8,
        //    CLD_DL_HD_HD9, CLD_DL_HD_HD10, CLD_DL_HD_HD11, CLD_DL_HD_HD12, CLD_DL_HD_HD13, CLD_DL_HD_HD14, CLD_DL_HD_HD15, CLD_DL_HD_HD16};
        ////夹爪上下
        //public static Cylinder CLD_DL_UD_HD1 = new Cylinder(GPIO_OUT_DL_UD_HD1, null, GPIO_IN_DL_DW_HD1);
        //public static Cylinder CLD_DL_UD_HD2 = new Cylinder(GPIO_OUT_DL_UD_HD2, null, GPIO_IN_DL_DW_HD2);
        //public static Cylinder CLD_DL_UD_HD3 = new Cylinder(GPIO_OUT_DL_UD_HD3, null, GPIO_IN_DL_DW_HD3);
        //public static Cylinder CLD_DL_UD_HD4 = new Cylinder(GPIO_OUT_DL_UD_HD4, null, GPIO_IN_DL_DW_HD4);
        //public static Cylinder CLD_DL_UD_HD5 = new Cylinder(GPIO_OUT_DL_UD_HD5, null, GPIO_IN_DL_DW_HD5);
        //public static Cylinder CLD_DL_UD_HD6 = new Cylinder(GPIO_OUT_DL_UD_HD6, null, GPIO_IN_DL_DW_HD6);
        //public static Cylinder CLD_DL_UD_HD7 = new Cylinder(GPIO_OUT_DL_UD_HD7, null, GPIO_IN_DL_DW_HD7);
        //public static Cylinder CLD_DL_UD_HD8 = new Cylinder(GPIO_OUT_DL_UD_HD8, null, GPIO_IN_DL_DW_HD8);
        //public static Cylinder CLD_DL_UD_HD9 = new Cylinder(GPIO_OUT_DL_UD_HD9, null, GPIO_IN_DL_DW_HD9);
        //public static Cylinder CLD_DL_UD_HD10 = new Cylinder(GPIO_OUT_DL_UD_HD10, null, GPIO_IN_DL_DW_HD10);
        //public static Cylinder CLD_DL_UD_HD11 = new Cylinder(GPIO_OUT_DL_UD_HD11, null, GPIO_IN_DL_DW_HD11);
        //public static Cylinder CLD_DL_UD_HD12 = new Cylinder(GPIO_OUT_DL_UD_HD12, null, GPIO_IN_DL_DW_HD12);
        //public static Cylinder CLD_DL_UD_HD13 = new Cylinder(GPIO_OUT_DL_UD_HD13, null, GPIO_IN_DL_DW_HD13);
        //public static Cylinder CLD_DL_UD_HD14 = new Cylinder(GPIO_OUT_DL_UD_HD14, null, GPIO_IN_DL_DW_HD14);
        //public static Cylinder CLD_DL_UD_HD15 = new Cylinder(GPIO_OUT_DL_UD_HD15, null, GPIO_IN_DL_DW_HD15);
        //public static Cylinder CLD_DL_UD_HD16 = new Cylinder(GPIO_OUT_DL_UD_HD16, null, GPIO_IN_DL_DW_HD16);
        //public static List<Cylinder> List_CLD_UD_HD = new List<Cylinder> { CLD_DL_UD_HD1, CLD_DL_UD_HD2, CLD_DL_UD_HD3, CLD_DL_UD_HD4, CLD_DL_UD_HD5, CLD_DL_UD_HD6, CLD_DL_UD_HD7, CLD_DL_UD_HD8,
        //    CLD_DL_UD_HD9, CLD_DL_UD_HD10, CLD_DL_UD_HD11, CLD_DL_UD_HD12, CLD_DL_UD_HD13, CLD_DL_UD_HD14, CLD_DL_UD_HD15, CLD_DL_UD_HD16};
        //料盘
        //public static Cylinder CLD_DL_HD_TRAY_NG = new Cylinder(GPIO_OUT_DL_NG_TRAY, GPIO_IN_DL_NG_TRAY);
        //public static Cylinder CLD_DL_HD_TRAY_OK = new Cylinder(GPIO_OUT_DL_OK_TRAY, GPIO_IN_DL_OK_TRAY);

        //工站WS1
        public static Cylinder CLD_WS1_FR1 = new Cylinder(GPIO_OUT_WS1_OPEN_FR1, GPIO_IN_WS1_OPEN_FR1, GPIO_IN_WS1_CLOSE_FR1, GPIO_OUT_WS1_CLOSE_FR1);
        public static Cylinder CLD_WS1_FR2 = new Cylinder(GPIO_OUT_WS1_OPEN_FR2, GPIO_IN_WS1_OPEN_FR2, GPIO_IN_WS1_CLOSE_FR2, GPIO_OUT_WS1_CLOSE_FR2);
        public static Cylinder CLD_WS1_FR3 = new Cylinder(GPIO_OUT_WS1_OPEN_FR3, GPIO_IN_WS1_OPEN_FR3, GPIO_IN_WS1_CLOSE_FR3, GPIO_OUT_WS1_CLOSE_FR3);
        public static Cylinder CLD_WS1_FR4 = new Cylinder(GPIO_OUT_WS1_OPEN_FR4, GPIO_IN_WS1_OPEN_FR4, GPIO_IN_WS1_CLOSE_FR4, GPIO_OUT_WS1_CLOSE_FR4);
        //public static Cylinder CLD_CLOSE_WS1_FR1 = new Cylinder(GPIO_OUT_WS1_CLOSE_FR1, GPIO_IN_WS1_CLOSE_FR1, GPIO_IN_WS1_OPEN_FR1);
        //public static Cylinder CLD_CLOSE_WS1_FR2 = new Cylinder(GPIO_OUT_WS1_CLOSE_FR2, GPIO_IN_WS1_CLOSE_FR2, GPIO_IN_WS1_OPEN_FR2);
        //public static Cylinder CLD_CLOSE_WS1_FR3 = new Cylinder(GPIO_OUT_WS1_CLOSE_FR3, GPIO_IN_WS1_CLOSE_FR3, GPIO_IN_WS1_OPEN_FR3);
        //public static Cylinder CLD_CLOSE_WS1_FR4 = new Cylinder(GPIO_OUT_WS1_CLOSE_FR4, GPIO_IN_WS1_CLOSE_FR4, GPIO_IN_WS1_OPEN_FR4);

        public static Cylinder CLD_WS1_BK1 = new Cylinder(GPIO_OUT_WS1_OPEN_BK1, GPIO_IN_WS1_OPEN_BK1, GPIO_IN_WS1_CLOSE_BK1, GPIO_OUT_WS1_CLOSE_BK1);
        public static Cylinder CLD_WS1_BK2 = new Cylinder(GPIO_OUT_WS1_OPEN_BK2, GPIO_IN_WS1_OPEN_BK2, GPIO_IN_WS1_CLOSE_BK2, GPIO_OUT_WS1_CLOSE_BK2);
        public static Cylinder CLD_WS1_BK3 = new Cylinder(GPIO_OUT_WS1_OPEN_BK3, GPIO_IN_WS1_OPEN_BK3, GPIO_IN_WS1_CLOSE_BK3, GPIO_OUT_WS1_CLOSE_BK3);
        public static Cylinder CLD_WS1_BK4 = new Cylinder(GPIO_OUT_WS1_OPEN_BK4, GPIO_IN_WS1_OPEN_BK4, GPIO_IN_WS1_CLOSE_BK4, GPIO_OUT_WS1_CLOSE_BK4);
        //public static Cylinder CLD_CLOSE_WS1_BK1 = new Cylinder(GPIO_OUT_WS1_CLOSE_BK1, GPIO_IN_WS1_CLOSE_BK1, GPIO_IN_WS1_OPEN_BK1);
        //public static Cylinder CLD_CLOSE_WS1_BK2 = new Cylinder(GPIO_OUT_WS1_CLOSE_BK2, GPIO_IN_WS1_CLOSE_BK2, GPIO_IN_WS1_OPEN_BK2);
        //public static Cylinder CLD_CLOSE_WS1_BK3 = new Cylinder(GPIO_OUT_WS1_CLOSE_BK3, GPIO_IN_WS1_CLOSE_BK3, GPIO_IN_WS1_OPEN_BK3);
        //public static Cylinder CLD_CLOSE_WS1_BK4 = new Cylinder(GPIO_OUT_WS1_CLOSE_BK4, GPIO_IN_WS1_CLOSE_BK4, GPIO_IN_WS1_OPEN_BK4);

        public static List<Cylinder> List_CLD_WS1_FR = new List<Cylinder> { CLD_WS1_FR1, CLD_WS1_FR2, CLD_WS1_FR3, CLD_WS1_FR4 };
        public static List<Cylinder> List_CLD_WS1_BK = new List<Cylinder> { CLD_WS1_BK1, CLD_WS1_BK2, CLD_WS1_BK3, CLD_WS1_BK4 };
        public static List<Cylinder> List_CLD_WS1 = new List<Cylinder> { CLD_WS1_FR1, CLD_WS1_FR2, CLD_WS1_FR3, CLD_WS1_FR4, CLD_WS1_BK1, CLD_WS1_BK2, CLD_WS1_BK3, CLD_WS1_BK4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS1_FR = new List<Cylinder> { CLD_CLOSE_WS1_FR1, CLD_CLOSE_WS1_FR2, CLD_CLOSE_WS1_FR3, CLD_CLOSE_WS1_FR4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS1_BK = new List<Cylinder> { CLD_CLOSE_WS1_BK1, CLD_CLOSE_WS1_BK2, CLD_CLOSE_WS1_BK3, CLD_CLOSE_WS1_BK4 };

        //工站WS2
        public static Cylinder CLD_WS2_FR1 = new Cylinder(GPIO_OUT_WS2_OPEN_FR1, GPIO_IN_WS2_OPEN_FR1, GPIO_IN_WS2_CLOSE_FR1, GPIO_OUT_WS2_CLOSE_FR1);
        public static Cylinder CLD_WS2_FR2 = new Cylinder(GPIO_OUT_WS2_OPEN_FR2, GPIO_IN_WS2_OPEN_FR2, GPIO_IN_WS2_CLOSE_FR2, GPIO_OUT_WS2_CLOSE_FR2);
        public static Cylinder CLD_WS2_FR3 = new Cylinder(GPIO_OUT_WS2_OPEN_FR3, GPIO_IN_WS2_OPEN_FR3, GPIO_IN_WS2_CLOSE_FR3, GPIO_OUT_WS2_CLOSE_FR3);
        public static Cylinder CLD_WS2_FR4 = new Cylinder(GPIO_OUT_WS2_OPEN_FR4, GPIO_IN_WS2_OPEN_FR4, GPIO_IN_WS2_CLOSE_FR4, GPIO_OUT_WS2_CLOSE_FR4);
        //public static Cylinder CLD_CLOSE_WS2_FR1 = new Cylinder(GPIO_OUT_WS2_CLOSE_FR1, GPIO_IN_WS2_CLOSE_FR1, GPIO_IN_WS2_OPEN_FR1);
        //public static Cylinder CLD_CLOSE_WS2_FR2 = new Cylinder(GPIO_OUT_WS2_CLOSE_FR2, GPIO_IN_WS2_CLOSE_FR2, GPIO_IN_WS2_OPEN_FR2);
        //public static Cylinder CLD_CLOSE_WS2_FR3 = new Cylinder(GPIO_OUT_WS2_CLOSE_FR3, GPIO_IN_WS2_CLOSE_FR3, GPIO_IN_WS2_OPEN_FR3);
        //public static Cylinder CLD_CLOSE_WS2_FR4 = new Cylinder(GPIO_OUT_WS2_CLOSE_FR4, GPIO_IN_WS2_CLOSE_FR4, GPIO_IN_WS2_OPEN_FR4);

        public static Cylinder CLD_WS2_BK1 = new Cylinder(GPIO_OUT_WS2_OPEN_BK1, GPIO_IN_WS2_OPEN_BK1, GPIO_IN_WS2_CLOSE_BK1, GPIO_OUT_WS2_CLOSE_BK1);
        public static Cylinder CLD_WS2_BK2 = new Cylinder(GPIO_OUT_WS2_OPEN_BK2, GPIO_IN_WS2_OPEN_BK2, GPIO_IN_WS2_CLOSE_BK2, GPIO_OUT_WS2_CLOSE_BK2);
        public static Cylinder CLD_WS2_BK3 = new Cylinder(GPIO_OUT_WS2_OPEN_BK3, GPIO_IN_WS2_OPEN_BK3, GPIO_IN_WS2_CLOSE_BK3, GPIO_OUT_WS2_CLOSE_BK3);
        public static Cylinder CLD_WS2_BK4 = new Cylinder(GPIO_OUT_WS2_OPEN_BK4, GPIO_IN_WS2_OPEN_BK4, GPIO_IN_WS2_CLOSE_BK4, GPIO_OUT_WS2_CLOSE_BK4);
        //public static Cylinder CLD_CLOSE_WS2_BK1 = new Cylinder(GPIO_OUT_WS2_CLOSE_BK1, GPIO_IN_WS2_CLOSE_BK1, GPIO_IN_WS2_OPEN_BK1);
        //public static Cylinder CLD_CLOSE_WS2_BK2 = new Cylinder(GPIO_OUT_WS2_CLOSE_BK2, GPIO_IN_WS2_CLOSE_BK2, GPIO_IN_WS2_OPEN_BK2);
        //public static Cylinder CLD_CLOSE_WS2_BK3 = new Cylinder(GPIO_OUT_WS2_CLOSE_BK3, GPIO_IN_WS2_CLOSE_BK3, GPIO_IN_WS2_OPEN_BK3);
        //public static Cylinder CLD_CLOSE_WS2_BK4 = new Cylinder(GPIO_OUT_WS2_CLOSE_BK4, GPIO_IN_WS2_CLOSE_BK4, GPIO_IN_WS2_OPEN_BK4);

        public static List<Cylinder> List_CLD_WS2_FR = new List<Cylinder> { CLD_WS2_FR1, CLD_WS2_FR2, CLD_WS2_FR3, CLD_WS2_FR4 };
        public static List<Cylinder> List_CLD_WS2_BK = new List<Cylinder> { CLD_WS2_BK1, CLD_WS2_BK2, CLD_WS2_BK3, CLD_WS2_BK4 };
        public static List<Cylinder> List_CLD_WS2 = new List<Cylinder> { CLD_WS2_FR1, CLD_WS2_FR2, CLD_WS2_FR3, CLD_WS2_FR4, CLD_WS2_BK1, CLD_WS2_BK2, CLD_WS2_BK3, CLD_WS2_BK4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS2_FR = new List<Cylinder> { CLD_CLOSE_WS2_FR1, CLD_CLOSE_WS2_FR2, CLD_CLOSE_WS2_FR3, CLD_CLOSE_WS2_FR4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS2_BK = new List<Cylinder> { CLD_CLOSE_WS2_BK1, CLD_CLOSE_WS2_BK2, CLD_CLOSE_WS2_BK3, CLD_CLOSE_WS2_BK4 };

        //工站WS3
        public static Cylinder CLD_WS3_FR1 = new Cylinder(GPIO_OUT_WS3_OPEN_FR1, GPIO_IN_WS3_OPEN_FR1, GPIO_IN_WS3_CLOSE_FR1, GPIO_OUT_WS3_CLOSE_FR1);
        public static Cylinder CLD_WS3_FR2 = new Cylinder(GPIO_OUT_WS3_OPEN_FR2, GPIO_IN_WS3_OPEN_FR2, GPIO_IN_WS3_CLOSE_FR2, GPIO_OUT_WS3_CLOSE_FR2);
        public static Cylinder CLD_WS3_FR3 = new Cylinder(GPIO_OUT_WS3_OPEN_FR3, GPIO_IN_WS3_OPEN_FR3, GPIO_IN_WS3_CLOSE_FR3, GPIO_OUT_WS3_CLOSE_FR3);
        public static Cylinder CLD_WS3_FR4 = new Cylinder(GPIO_OUT_WS3_OPEN_FR4, GPIO_IN_WS3_OPEN_FR4, GPIO_IN_WS3_CLOSE_FR4, GPIO_OUT_WS3_CLOSE_FR4);
        //public static Cylinder CLD_CLOSE_WS3_FR1 = new Cylinder(GPIO_OUT_WS3_CLOSE_FR1, GPIO_IN_WS2_CLOSE_FR1, GPIO_IN_WS2_OPEN_FR1);
        //public static Cylinder CLD_CLOSE_WS3_FR2 = new Cylinder(GPIO_OUT_WS3_CLOSE_FR2, GPIO_IN_WS2_CLOSE_FR2, GPIO_IN_WS2_OPEN_FR2);
        //public static Cylinder CLD_CLOSE_WS3_FR3 = new Cylinder(GPIO_OUT_WS3_CLOSE_FR3, GPIO_IN_WS2_CLOSE_FR3, GPIO_IN_WS2_OPEN_FR3);
        //public static Cylinder CLD_CLOSE_WS3_FR4 = new Cylinder(GPIO_OUT_WS3_CLOSE_FR4, GPIO_IN_WS2_CLOSE_FR4, GPIO_IN_WS2_OPEN_FR4);

        public static Cylinder CLD_WS3_BK1 = new Cylinder(GPIO_OUT_WS3_OPEN_BK1, GPIO_IN_WS3_OPEN_BK1, GPIO_IN_WS3_CLOSE_BK1, GPIO_OUT_WS3_CLOSE_BK1);
        public static Cylinder CLD_WS3_BK2 = new Cylinder(GPIO_OUT_WS3_OPEN_BK2, GPIO_IN_WS3_OPEN_BK2, GPIO_IN_WS3_CLOSE_BK2, GPIO_OUT_WS3_CLOSE_BK2);
        public static Cylinder CLD_WS3_BK3 = new Cylinder(GPIO_OUT_WS3_OPEN_BK3, GPIO_IN_WS3_OPEN_BK3, GPIO_IN_WS3_CLOSE_BK3, GPIO_OUT_WS3_CLOSE_BK3);
        public static Cylinder CLD_WS3_BK4 = new Cylinder(GPIO_OUT_WS3_OPEN_BK4, GPIO_IN_WS3_OPEN_BK4, GPIO_IN_WS3_CLOSE_BK4, GPIO_OUT_WS3_CLOSE_BK4);
        //public static Cylinder CLD_CLOSE_WS3_BK1 = new Cylinder(GPIO_OUT_WS3_CLOSE_BK1, GPIO_IN_WS3_CLOSE_BK1, GPIO_IN_WS3_OPEN_BK1);
        //public static Cylinder CLD_CLOSE_WS3_BK2 = new Cylinder(GPIO_OUT_WS3_CLOSE_BK2, GPIO_IN_WS3_CLOSE_BK2, GPIO_IN_WS3_OPEN_BK2);
        //public static Cylinder CLD_CLOSE_WS3_BK3 = new Cylinder(GPIO_OUT_WS3_CLOSE_BK3, GPIO_IN_WS3_CLOSE_BK3, GPIO_IN_WS3_OPEN_BK3);
        //public static Cylinder CLD_CLOSE_WS3_BK4 = new Cylinder(GPIO_OUT_WS3_CLOSE_BK4, GPIO_IN_WS3_CLOSE_BK4, GPIO_IN_WS3_OPEN_BK4);

        public static List<Cylinder> List_CLD_WS3_FR = new List<Cylinder> { CLD_WS3_FR1, CLD_WS3_FR2, CLD_WS3_FR3, CLD_WS3_FR4 };
        public static List<Cylinder> List_CLD_WS3_BK = new List<Cylinder> { CLD_WS3_BK1, CLD_WS3_BK2, CLD_WS3_BK3, CLD_WS3_BK4 };
        public static List<Cylinder> List_CLD_WS3 = new List<Cylinder> { CLD_WS3_FR1, CLD_WS3_FR2, CLD_WS3_FR3, CLD_WS3_FR4, CLD_WS3_BK1, CLD_WS3_BK2, CLD_WS3_BK3, CLD_WS3_BK4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS3_FR = new List<Cylinder> { CLD_CLOSE_WS3_FR1, CLD_CLOSE_WS3_FR2, CLD_CLOSE_WS3_FR3, CLD_CLOSE_WS3_FR4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS3_BK = new List<Cylinder> { CLD_CLOSE_WS3_BK1, CLD_CLOSE_WS3_BK2, CLD_CLOSE_WS3_BK3, CLD_CLOSE_WS3_BK4 };

        //工站WS4
        public static Cylinder CLD_WS4_FR1 = new Cylinder(GPIO_OUT_WS4_OPEN_FR1, GPIO_IN_WS4_OPEN_FR1, GPIO_IN_WS4_CLOSE_FR1, GPIO_OUT_WS4_CLOSE_FR1);
        public static Cylinder CLD_WS4_FR2 = new Cylinder(GPIO_OUT_WS4_OPEN_FR2, GPIO_IN_WS4_OPEN_FR2, GPIO_IN_WS4_CLOSE_FR2, GPIO_OUT_WS4_CLOSE_FR2);
        public static Cylinder CLD_WS4_FR3 = new Cylinder(GPIO_OUT_WS4_OPEN_FR3, GPIO_IN_WS4_OPEN_FR3, GPIO_IN_WS4_CLOSE_FR3, GPIO_OUT_WS4_CLOSE_FR3);
        public static Cylinder CLD_WS4_FR4 = new Cylinder(GPIO_OUT_WS4_OPEN_FR4, GPIO_IN_WS4_OPEN_FR4, GPIO_IN_WS4_CLOSE_FR4, GPIO_OUT_WS4_CLOSE_FR4);
        //public static Cylinder CLD_CLOSE_WS4_FR1 = new Cylinder(GPIO_OUT_WS4_CLOSE_FR1, GPIO_IN_WS4_CLOSE_FR1, GPIO_IN_WS4_OPEN_FR1);
        //public static Cylinder CLD_CLOSE_WS4_FR2 = new Cylinder(GPIO_OUT_WS4_CLOSE_FR2, GPIO_IN_WS4_CLOSE_FR2, GPIO_IN_WS4_OPEN_FR2);
        //public static Cylinder CLD_CLOSE_WS4_FR3 = new Cylinder(GPIO_OUT_WS4_CLOSE_FR3, GPIO_IN_WS4_CLOSE_FR3, GPIO_IN_WS4_OPEN_FR3);
        //public static Cylinder CLD_CLOSE_WS4_FR4 = new Cylinder(GPIO_OUT_WS4_CLOSE_FR4, GPIO_IN_WS4_CLOSE_FR4, GPIO_IN_WS4_OPEN_FR4);

        public static Cylinder CLD_WS4_BK1 = new Cylinder(GPIO_OUT_WS4_OPEN_BK1, GPIO_IN_WS4_OPEN_BK1, GPIO_IN_WS4_CLOSE_BK1, GPIO_OUT_WS4_CLOSE_BK1);
        public static Cylinder CLD_WS4_BK2 = new Cylinder(GPIO_OUT_WS4_OPEN_BK2, GPIO_IN_WS4_OPEN_BK2, GPIO_IN_WS4_CLOSE_BK2, GPIO_OUT_WS4_CLOSE_BK2);
        public static Cylinder CLD_WS4_BK3 = new Cylinder(GPIO_OUT_WS4_OPEN_BK3, GPIO_IN_WS4_OPEN_BK3, GPIO_IN_WS4_CLOSE_BK3, GPIO_OUT_WS4_CLOSE_BK3);
        public static Cylinder CLD_WS4_BK4 = new Cylinder(GPIO_OUT_WS4_OPEN_BK4, GPIO_IN_WS4_OPEN_BK4, GPIO_IN_WS4_CLOSE_BK4, GPIO_OUT_WS4_CLOSE_BK4);
        //public static Cylinder CLD_CLOSE_WS4_BK1 = new Cylinder(GPIO_OUT_WS4_CLOSE_BK1, GPIO_IN_WS4_CLOSE_BK1, GPIO_IN_WS4_OPEN_BK1);
        //public static Cylinder CLD_CLOSE_WS4_BK2 = new Cylinder(GPIO_OUT_WS4_CLOSE_BK2, GPIO_IN_WS4_CLOSE_BK2, GPIO_IN_WS4_OPEN_BK2);
        //public static Cylinder CLD_CLOSE_WS4_BK3 = new Cylinder(GPIO_OUT_WS4_CLOSE_BK3, GPIO_IN_WS4_CLOSE_BK3, GPIO_IN_WS4_OPEN_BK3);
        //public static Cylinder CLD_CLOSE_WS4_BK4 = new Cylinder(GPIO_OUT_WS4_CLOSE_BK4, GPIO_IN_WS4_CLOSE_BK4, GPIO_IN_WS4_OPEN_BK4);

        public static List<Cylinder> List_CLD_WS4_FR = new List<Cylinder> { CLD_WS4_FR1, CLD_WS4_FR2, CLD_WS4_FR3, CLD_WS4_FR4 };
        public static List<Cylinder> List_CLD_WS4_BK = new List<Cylinder> { CLD_WS4_BK1, CLD_WS4_BK2, CLD_WS4_BK3, CLD_WS4_BK4 };
        public static List<Cylinder> List_CLD_WS4 = new List<Cylinder> { CLD_WS4_FR1, CLD_WS4_FR2, CLD_WS4_FR3, CLD_WS4_FR4, CLD_WS4_BK1, CLD_WS4_BK2, CLD_WS4_BK3, CLD_WS4_BK4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS4_FR = new List<Cylinder> { CLD_CLOSE_WS4_FR1, CLD_CLOSE_WS4_FR2, CLD_CLOSE_WS4_FR3, CLD_CLOSE_WS4_FR4 };
        //public static List<Cylinder> List_CLD_CLOSE_WS4_BK = new List<Cylinder> { CLD_CLOSE_WS4_BK1, CLD_CLOSE_WS4_BK2, CLD_CLOSE_WS4_BK3, CLD_CLOSE_WS4_BK4 };


        #endregion
        #endregion

        #region 错误框显示
        //private static  object lockdisplay = new object();       
        //  public static warning fr_warn= new warning();
        public static string DoorAlarmMsg = string.Empty;
        public static string DoorAlarmMsgTemp = string.Empty;
        //public static bool IsErrAlm = false;
        public class ST_WARN
        {
            public string ok_txt;
            public string cancle_txt;
            public string abort_txt;
            public string msg;
            public List<int> ws_num;
            public string lb_msg;
            public WS ws = null;
            public string title;

            public ST_WARN(string ok_txt = "确定", WS ws = null, List<int> ws_num = null, string cancle_txt = "不用", string abort_txt = "不用", string msg = "信息", string lb_msg = "信息", string title = "")
            {
                this.ok_txt = ok_txt;
                this.cancle_txt = cancle_txt;
                this.abort_txt = abort_txt;
                this.msg = msg;
                this.ws_num = ws_num;
                this.lb_msg = lb_msg;
                this.ws = ws;
                this.title = title;
            }
        }

        public static DialogResult Display_frwarn(warning fr_warn, ST_WARN warn, ERR_ALM.EmErrItem erritem, bool IsYellow = true)
        {
            //lock (lockdisplay)
            //{
            try
            {
                fr_warn.btn_ok.Text = warn.ok_txt;
                fr_warn.btn_cancle.Text = warn.cancle_txt;
                fr_warn.btn_ok.Visible = true;
                fr_warn.btn_abort.Text = warn.abort_txt;
                fr_warn.ws = warn.ws;
                if (fr_warn.ws == null) fr_warn.pnl_ws.Visible = false;
                else fr_warn.pnl_ws.Visible = true;
                fr_warn.ws_num = warn.ws_num;
                if (fr_warn.ws_num == null || fr_warn.ws_num.Count <= 0) fr_warn.pnl_wspos.Visible = false;
                else fr_warn.pnl_wspos.Visible = true;
                if (fr_warn.btn_cancle.Text != "不用") fr_warn.btn_cancle.Visible = true;
                else fr_warn.btn_cancle.Visible = false;
                if (fr_warn.btn_abort.Text != "不用") fr_warn.btn_abort.Visible = true;
                else fr_warn.btn_abort.Visible = false;
                fr_warn.BackColor = IsYellow ? Color.Yellow : Color.Red;
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, warn.msg, DReport.EmErrCode.Null, 0, erritem);
                fr_warn.lb_msg.Text = warn.lb_msg;
                fr_warn.title = warn.title;
                fr_warn.TopMost = true;
                if (!VAR.sys_inf.info.Contains(VAR.IsChinese ? "更换仓储" : "Change Tray")) 
                    VAR.IsErrAlm = true;
                DialogResult logres = fr_warn.ShowDialog();
                return logres;
            }
            finally
            {
                //IsErrAlm = false;
            }

            //}


        }

        //门禁提示
        public static void ErrAlmTip()
        {
            MT.ST_WARN st_warn = new MT.ST_WARN();
            warning fr_warn = new warning();
            st_warn.ok_txt = MultiLanguage.TxtSelct("确认", "OK", "Đảm bảo");// VAR.IsChinese ? "确认" : "OK";
            st_warn.ws = null;
            if (MT.DoorAlarmMsg != string.Empty)
            {
                st_warn.msg = MT.DoorAlarmMsg;//增加语言
                //DRpt.Report_Status(DReport.EmStatus.Ready, DReport.EmHareware.Null, DReport.EmStatus.Ready.GetDescription());
                if (MT.DoorAlarmMsg != string.Format("急停键按下，请确认,系统需重新复位!") || MT.DoorAlarmMsg != "Press the emergency stop button, please confirm, the system needs to be reset!")
                {
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "门禁打开!" : "Door Alarm!", 20, true);
                    st_warn.title = MultiLanguage.TxtSelct("提示:门禁打开!", "Tip: The entrance guard is open!", "Gợi ý: Cửa đang mở!");
                    st_warn.lb_msg = MultiLanguage.TxtSelct("门禁打开提示:\r\n" + MT.DoorAlarmMsg + "请把相关的门关好",
                                                            "Access control opening reminder:\r\n" + MT.DoorAlarmMsg + "Please close the relevant doors.",
                                                            "Gợi ý: Cửa đang mở!:\r\n" + MT.DoorAlarmMsg + "Vui lòng đóng cánh cửa liên quan");
                }
                else
                {
                    VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "急停按下!" : "EMG", 20, true);
                    st_warn.title = MultiLanguage.TxtSelct("提示:急停按下!", "Tip:EMG button was pressed", "Mẹo: Nhấn dừng khẩn cấp!");
                    st_warn.lb_msg = MT.DoorAlarmMsg;
                }
                MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.Null);
            }
            else if (VAR.SysErrAlm.ErrItem != ERR_ALM.EmErrItem.Null && VAR.sys_inf.info.Contains(VAR.IsChinese ? "运行" : "RUN"))
            {
                // DRpt.Report_Status(DReport.EmStatus.Error, DReport.EmHareware.Null, DReport.EmStatus.Error.GetDescription());
                st_warn.msg = VAR.SysErrAlm.ErrStr;
                st_warn.lb_msg = VAR.SysErrAlm.ErrStr;
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, Utility.GetDescription(VAR.SysErrAlm.ErrItem, VAR.IsChinese), 20, true);
                MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.Null);
            }
            VAR.SysErrAlm.ErrItem = ERR_ALM.EmErrItem.Null;
            VAR.SysErrAlm.ErrStr = string.Empty;
            MT.DoorAlarmMsg = string.Empty;
            MT.DoorAlarmMsgTemp = string.Empty;
        }

        #endregion

        #region 安全监测
        public static void SetSafeFunc()
        {
            foreach (AXIS ax in Axlist_UDL1_ExpLC)
            {
                ax.ChkSafeSen = ChkSafeSen;
                ax.ChkSafePos = ChkSafePos;
            }

            foreach (AXIS ax in Axlist_UDL2_ExpLC)
            {
                ax.ChkSafeSen = ChkSafeSen;
                ax.ChkSafePos = ChkSafePos;
            }

            foreach (AXIS ax in Axlist_UDL_LC)
            {
                ax.ChkSafeSen = ChkSafeSen;
                ax.ChkSafePos = ChkSafePos;
            }

        }

        public const double xsafedis = 89;
        public static EM_RES ChkSafePos(int id, double targe_pos = double.MaxValue)
        {
            EM_RES ret = EM_RES.OK;
            UpDownLoad Other_ud;
            //门禁保护
            //ret = ChkSafeSen(id);
            //if (ret != EM_RES.OK) return ret;
            //位置保护
            foreach (UpDownLoad ud in COM.List_UDLoad)
            {
                if (id == ud.ax_x.id || id == ud.ax_y.id)
                {
                    if (ud.ax_z.home_status == AXIS.HOME_STA.OK)
                    {
                        if (Math.Abs(ud.ax_z.fenc_pos) > 3)
                        {
                            Thread.Sleep(20);
                            if (Math.Abs(ud.ax_z.fenc_pos) > 3)
                            {
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}->{1}当前坐标>3，禁止移动{2}或{3}!", ud.disc, ud.ax_z.disc, ud.ax_x.disc, ud.ax_y.disc) : string.Format("{0}->{2} Current coordinates>3,forbidden {3} or {4} to move     ({1}->{2}当前坐标>3，禁止移动{3}或{4}!)", ud.englishdisc, ud.disc, ud.ax_z.disc, ud.ax_x.disc, ud.ax_y.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                                return EM_RES.MOVE_PROTECT;
                            }
                        }
                    }
                    else
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}{1}未复位 ，禁止移动{2}或{3}!", ud.disc, ud.ax_z.disc, ud.ax_x.disc, ud.ax_y.disc) : string.Format("{0} {2} is not reset,forbidden {3} or {4} to move     ({1}{2}未复位 ，禁止移动{3}或{4}!)", ud.englishdisc, ud.disc, ud.ax_z.disc, ud.ax_x.disc, ud.ax_y.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }

                }

                if (PT_SET.bUdMovSafe && id == ud.ax_y.id && ud.ax_y.home_status == AXIS.HOME_STA.OK && VAR.gsys_set.status != EM_SYS_STA.RUN && targe_pos > ud.list_xt[1].cap_spd)
                {
                    if (!Turntable.GetWSOnFeedPos.isInTestPos && !Turntable.GetWSOnFeedPos.isInFeedPos)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("上料工站不在上料与测试位，禁止移动{0}->{1}!", ud.disc, ud.ax_y.disc) : string.Format("The loading station is not in the loading and testing position. It is forbidden {0} {2} to move !      (上料工站不在上料与测试位，禁止移动{1}->{2}!)", ud.englishdisc, ud.disc, ud.ax_y.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }

                if (PT_SET.bUdMovSafe && id == ud.ax_x.id && ud.ax_x.home_status == AXIS.HOME_STA.OK && VAR.gsys_set.status != EM_SYS_STA.RUN && Math.Abs(targe_pos) > 10)
                {
                    if (!Turntable.GetWSOnFeedPos.isInTestPos && !Turntable.GetWSOnFeedPos.isInFeedPos)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("上料工站不在上料与测试位，禁止移动{0}->{1}!", ud.disc, ud.ax_x.disc) : string.Format("The loading station is not in the loading and testing position. It is forbidden {0} {2} to move !         (上料工站不在上料与测试位，禁止移动{1}->{2}!)", ud.englishdisc, ud.disc, ud.ax_x.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }

                }

                if (id == ud.ax_x.id)
                {
                    if (ud.id == 0) Other_ud = COM.UDLoad2;
                    else Other_ud = COM.UDLoad1;
                    if (Math.Abs(targe_pos) + Math.Abs(Other_ud.ax_x.fenc_pos) + xsafedis > UpDownLoad.DisAxisX && COM.UDLoad1.ax_x.home_status == AXIS.HOME_STA.OK && COM.UDLoad2.ax_x.home_status == AXIS.HOME_STA.OK)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}的目标坐标为{1}，{2}的当前坐标为{3}，禁止{4}移动因移动后两X轴距离小于安全距离{5},总距离{6}", ud.disc, targe_pos, Other_ud.ax_x.disc, Other_ud.ax_x.fenc_pos, ud.disc, xsafedis, UpDownLoad.DisAxisX) : string.Format("{0}'s target coordinates is {2},{3}'s Current coordinates is {4},forbidden {0} to move cause the distance between the two X axises is less than the safety distance({6}) after moving,total distance is {7}       ({1}的目标坐标为{2}，{3}的当前坐标为{4}，禁止{5}移动因移动后两X轴距离小于安全距离{6},总距离{7})", ud.englishdisc, ud.disc, targe_pos, Other_ud.ax_x.disc, Other_ud.ax_x.fenc_pos, ud.disc, xsafedis, UpDownLoad.DisAxisX), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                    else if (COM.UDLoad1.ax_x.home_status != AXIS.HOME_STA.OK && COM.UDLoad1.status_ud != UpDownLoad.EM_STA.HOME)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}未复位,{1}禁止移动!", COM.UDLoad1.ax_x.disc, ud.disc) : string.Format("{1} is not reset,forbidden {0} to move!    ({1}未复位,{2}禁止移动!)", ud.englishdisc, COM.UDLoad1.ax_x.disc, ud.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                    else if (COM.UDLoad2.ax_x.home_status != AXIS.HOME_STA.OK && COM.UDLoad2.status_ud != UpDownLoad.EM_STA.HOME)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}未复位,{1}禁止移动!", COM.UDLoad2.ax_x.disc, ud.disc) : string.Format("{1} is not reset,forbidden {0} to move!      ({1}未复位,{2}禁止移动!)", ud.englishdisc, COM.UDLoad2.ax_x.disc, ud.disc), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                        return EM_RES.MOVE_PROTECT;
                    }
                }

            }


            #region old
            //if (MT.AXIS_DL_Y.home_status == AXIS.HOME_STA.OK && MT.AXIS_UL_X.home_status == AXIS.HOME_STA.OK)
            //{
            //    if (id == MT.AXIS_DL_Y.id)
            //    {
            //        if (!DownloadModle.isUp)
            //            return EM_RES.MOVE_PROTECT;
            //        if ((targe_pos > (DownloadModle.Dwload_Ysafe+0.1)) && MT.AXIS_UL_X.fenc_pos >0.1)
            //        {
            //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("下料Y定位位置{0}大于安全位置{1},且上料X轴的当前位置不在原点,禁止下料轴移动!", targe_pos, DownloadModle.Dwload_Ysafe));
            //            return EM_RES.MOVE_PROTECT;
            //        }
            //    }

            //    if (id == MT.AXIS_UL_X.id)
            //    {
            //        if ((MT.AXIS_DL_Y.fenc_pos > (DownloadModle.Dwload_Ysafe+0.1)) && targe_pos >0.1)
            //        {
            //            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("下料Y轴当前位置大于安全位置{0}，上料X轴禁止移动!", DownloadModle.Dwload_Ysafe));
            //            return EM_RES.MOVE_PROTECT;
            //        }

            //    }
            //}


            #endregion



            if (id == MT.AXIS_UDL_FD_Z.id)
            {
                if ((Math.Abs(MT.AXIS_UDL_FD_X.fenc_pos) > COM.traybox_fd.fd_safe_x + 1) && (Math.Abs(targe_pos - MT.AXIS_UDL_FD_Z.fenc_pos) > COM.traybox_fd.tray_feed_ofs_h + 0.1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("供料仓X轴插入OK料仓，供料Z轴移动距离{0}超过{1}!", (Math.Abs(targe_pos - MT.AXIS_UDL_FD_Z.fenc_pos)), COM.traybox_fd.tray_feed_ofs_h) : string.Format("FeedTray Box x axis insert in to FeedTray Box，FeedTray Box z axis's moving distance{0} over {1}!       (供料仓X轴插入OK料仓，供料Z轴移动距离{0}超过{1}!)", (Math.Abs(targe_pos - MT.AXIS_UDL_FD_Z.fenc_pos)), COM.traybox_fd.tray_feed_ofs_h), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }

            if (id == MT.AXIS_UDL_NG_Z.id)
            {
                if ((Math.Abs(MT.AXIS_UDL_NG_X.fenc_pos) > COM.traybox_ng.fd_safe_x + 1) && (Math.Abs(targe_pos - MT.AXIS_UDL_NG_Z.fenc_pos) > COM.traybox_ng.tray_feed_ofs_h + 0.1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("NG料仓X轴插入NG料仓，NG料Z轴移动距离{0}大于{1}!", (Math.Abs(targe_pos - MT.AXIS_UDL_NG_Z.fenc_pos)), COM.traybox_ng.tray_feed_ofs_h) : string.Format("NG Box x axis insert in to NG Box，NG Box z axis's moving distance{0} over {1}!      (NG料仓X轴插入NG料仓，NG料Z轴移动距离{0}大于{1}!)", (Math.Abs(targe_pos - MT.AXIS_UDL_NG_Z.fenc_pos)), COM.traybox_ng.tray_feed_ofs_h), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }

            if (id == MT.AXIS_UDL_OK_Z.id)
            {
                if ((Math.Abs(MT.AXIS_UDL_OK_X.fenc_pos) > COM.traybox_ok.fd_safe_x + 1) && (Math.Abs(targe_pos - MT.AXIS_UDL_OK_Z.fenc_pos) > COM.traybox_ok.tray_feed_ofs_h + 0.1))
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("OK料仓X轴插入OK料仓，OK料Z轴移动距离{0}大于{1}!", (Math.Abs(targe_pos - MT.AXIS_UDL_OK_Z.fenc_pos)), COM.traybox_ok.tray_feed_ofs_h) : string.Format("OK Box x axis insert in to OK Box，OK Box z axis's moving distance{0} over {1}!         (OK料仓X轴插入OK料仓，OK料Z轴移动距离{0}大于{1}!)", (Math.Abs(targe_pos - MT.AXIS_UDL_OK_Z.fenc_pos)), COM.traybox_ok.tray_feed_ofs_h), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.Axis + id, ERR_ALM.EmErrItem.MoveProtect);
                    return EM_RES.MOVE_PROTECT;
                }
            }

            //检查料仓物料
            //if (id == MT.AXIS_UL_FD_X.id)
            //{
            //    if ((Math.Abs(MT.AXIS_UL_FD_X.fenc_pos) > COM.traybox_fd.fd_safe_x) && MT.GPIO_IN_UL_INP_FD_TRAYBOX.isON)
            //    {
            //        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("供料仓X轴插入OK料仓，OK料Z轴移动距离{0}超过{1}!", (Math.Abs(targe_pos - MT.AXIS_UL_FD_Z.fenc_pos)), COM.traybox_fd.tray_feed_ofs_h));
            //        return EM_RES.MOVE_PROTECT;
            //    }
            //}


            #region old
            //if (AXIS_Z.id == id) return EM_RES.OK;

            //Z在原点且坐标接近0
            //if ((!AXIS_Z.isORG || (AXIS_Z.home_status != AXIS.HOME_STA.OK && Math.Abs(AXIS_Z.fcmd_pos) > 1)))
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "Z不在原点，禁止移动载台!");
            //    return CONST.RES_MOVE_PROTECT;
            //}
            //if (!AXIS_Z.isORG && (AXIS_Z.home_status != AXIS.HOME_STA.OK && Math.Abs(AXIS_Z.fcmd_pos) > (TD.dt_pos_up + 0.1)))
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "Z不在原点，禁止移动载台!");
            //    return EM_RES.MOVE_PROTECT;
            //}

            ////FDH在上位
            //if ((GPIO_IN_FDH_L_U.isOFF && GPIO_IN_FDH_L_L.isOFF) || (GPIO_IN_FDH_R_U.isOFF && GPIO_IN_FDH_R_R.isOFF))
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "搬料未抬起，禁止移动载台!");
            //    return EM_RES.MOVE_PROTECT;
            //}

            ////R限制
            //if (AXIS_R.id == id && Math.Abs(AXIS_R.fenc_pos - targe_pos) > 3)
            //{
            //    if (AXIS_Y.fenc_pos > (AXIS_Y.slp - 50))
            //    {
            //        double[] posInf = { -80, -100, -260, -290 };
            //        for (int n = 0; n < posInf.Length; n++)
            //        {
            //            if (Math.Min(AXIS_R.fenc_pos, targe_pos) < posInf[n] && posInf[n] < Math.Max(AXIS_R.fenc_pos, targe_pos))
            //                return EM_RES.MOVE_PROTECT;
            //        }
            //    }
            //}
            #endregion
            return EM_RES.OK;
        }

        public static EM_RES ChkSafeSen(int id = 0, double target_pos = double.MaxValue)
        {
            EM_RES ret = EM_RES.OK;
            //上下料门禁
            ret = ChkAllSafeSen(0x01);
            //if (GPIO_IN_EMG.isOFF) return EM_RES.EMG;
            return ret;
        }

        public static bool isSafeSen
        {
            get
            {
                if (ChkSafeSen() == EM_RES.OK) return true;
                return false;
            }
        }

        public static EM_RES ChkAllSafeSen(int ChkEn = 0xff)
        {
            //上下料门禁
            if (PT_SET.bEnUpDownDr && (ChkEn & 0x01) == 0x01)
            {
                foreach (GPIO dr in MT.List_UDL_DOOR)
                {
                    if (dr.isON) continue;

                    if (dr.AssertOFF())
                    {
                        MT.DoorAlarmMsg = VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!", dr.english_disc);
                        if (MT.DoorAlarmMsg != MT.DoorAlarmMsgTemp)
                        {
                            MT.DoorAlarmMsgTemp = MT.DoorAlarmMsg;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}打开!",
                                dr.str_disc) : string.Format("{0} turn on!        ({1}打开!)", dr.english_disc, dr.str_disc), DReport.EmErrCode.HarewareProtect,
                                (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.DoorProtect, ErrCode: ShowErrMsg.UpDnDoorOpen);
                        }
                        return EM_RES.SAFE_PROTECT;
                    }
                }
            }


            List<GPIO> List_Temp_DOOR = new List<GPIO> { };
            if (PT_SET.LbEn)
            {
                List_Temp_DOOR = MT.List_TT_DOOR2;
            }
            else
            {
                List_Temp_DOOR = MT.List_TT_DOOR1;
            }

            //转盘
            if (PT_SET.bEnTrayDr && (ChkEn & 0x02) == 0x02)
            {
                foreach (GPIO dr in List_Temp_DOOR)
                {
                    if (dr.isON) continue;
                    if (dr.AssertOFF())
                    {
                        MT.DoorAlarmMsg = VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!", dr.english_disc);
                        if (MT.DoorAlarmMsg != MT.DoorAlarmMsgTemp)
                        {
                            MT.DoorAlarmMsgTemp = MT.DoorAlarmMsg;
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!    ({1}打开!)", dr.english_disc, dr.str_disc), DReport.EmErrCode.HarewareProtect, (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.DoorProtect, ErrCode: ShowErrMsg.WsDoorOpen);
                        }
                        return EM_RES.SAFE_PROTECT;
                    }
                }
            }

            if (PT_SET.LbEn)
            {
                //右光箱
                if (PT_SET.bEnRBoxDr && (ChkEn & 0x04) == 0x04)
                {
                    foreach (GPIO dr in MT.List_RLB_DOOR)
                    {
                        if (dr.isON) continue;
                        if (dr.AssertOFF())
                        {
                            MT.DoorAlarmMsg = VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!", dr.english_disc);
                            if (MT.DoorAlarmMsg != MT.DoorAlarmMsgTemp)
                            {
                                MT.DoorAlarmMsgTemp = MT.DoorAlarmMsg;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!   {1}打开!", dr.english_disc, dr.str_disc), DReport.EmErrCode.HarewareProtect, (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.DoorProtect, ErrCode: ShowErrMsg.RLightDoorOpen);
                            }
                            return EM_RES.SAFE_PROTECT;
                        }
                    }
                }

                //左光箱
                if (PT_SET.bEnLBoxDr && (ChkEn & 0x08) == 0x08)
                {
                    foreach (GPIO dr in MT.List_LLB_DOOR)
                    {
                        if (dr.isON) continue;
                        if (dr.AssertOFF())
                        {
                            MT.DoorAlarmMsg = VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!", dr.english_disc);
                            if (MT.DoorAlarmMsg != MT.DoorAlarmMsgTemp)
                            {
                                MT.DoorAlarmMsgTemp = MT.DoorAlarmMsg;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}打开!", dr.str_disc) : string.Format("{0} turn on!     ({1}打开!)", dr.english_disc, dr.str_disc), DReport.EmErrCode.HarewareProtect, (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.DoorProtect, ErrCode: ShowErrMsg.LLightDoorOpen);
                            }
                            return EM_RES.SAFE_PROTECT;
                        }
                    }
                }
            }

            if (GPIO_IN_EMG0.isOFF || GPIO_IN_EMG1.isOFF || GPIO_IN_EMG2.isOFF || GPIO_IN_EMG3.isOFF)
            {
                Thread.Sleep(20);
                if (GPIO_IN_EMG0.isOFF || GPIO_IN_EMG1.isOFF || GPIO_IN_EMG2.isOFF || GPIO_IN_EMG3.isOFF)
                {
                    MT.DoorAlarmMsg = VAR.IsChinese ? "急停键按下，请确认,系统需重新复位!" : "Press the emergency stop button, please confirm, the system needs to be reset!          (急停键按下，请确认,系统需重新复位!)";
                    WS.bquit = true;
                    UpDownLoad.bquit = true;
                    VAR.gsys_set.bquit = true;
                    MT.GPIO_OUT_ALM_RED.SetOn();
                    if (MT.DoorAlarmMsg != MT.DoorAlarmMsgTemp)
                    {
                        MT.DoorAlarmMsgTemp = MT.DoorAlarmMsg;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("急停键按下，请确认,系统需重新复位!") : string.Format("Press the emergency stop button, please confirm, the system needs to be reset!          (急停键按下，请确认,系统需重新复位!)"), DReport.EmErrCode.EmgStop, (int)DReport.EmHareware.Safty, ERR_ALM.EmErrItem.EmgStop);
                    }

                    return EM_RES.EMG;
                }
            }
            return EM_RES.OK;
        }

        public static bool isAllSafeSen
        {
            get
            {
                if (ChkAllSafeSen() == EM_RES.OK) return true;
                return false;
            }
        }
        #endregion

        #region 板卡初始化
        public static bool isReady
        {
            get
            {
                List<CARD> TempCardList;
                if (PT_SET.LbEn)
                {
                    TempCardList = CardList;

                }
                else
                {
                    TempCardList = CardList1;
                }
                foreach (CARD card in TempCardList)
                {
                    if (!card.isReady) return false;
                }
                return true;
            }
        }
        public static EM_RES Init(String filename = "", int timeout = 15000)
        {
            if (isReady) return EM_RES.OK;

            //start init
            List<Task> ListTask = new List<Task>();
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = CardList;
            }
            else
            {
                TempCardList = CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                if (!card.isReady)
                {
                    Task TaskHWInit = new Task(() =>
                    {
                        card.Init();
                    });
                    ListTask.Add(TaskHWInit);
                    TaskHWInit.Start();
                }
            }

            if (timeout == 0) return EM_RES.OK;

            //wait
            bool bEnd = true;
            int time = 0;
            do
            {
                //timer
                time += 10;
                if (time >= timeout) return EM_RES.TIMEOUT;

                //check task
                bEnd = true;
                foreach (Task task in ListTask)
                {
                    if (!task.IsCompleted) bEnd = false;
                }
                if (bEnd) break;

                //delay
                Thread.Sleep(10);
                Application.DoEvents();

            } while (true);

            if (isReady) return EM_RES.OK;
            else return EM_RES.ERR;
        }
        #endregion

        #region 检查/重连
        /// <summary>
        /// 检查所有板卡是否在线，否在重连
        /// </summary>
        /// <returns></returns>
        public static EM_RES ChkAndReConnect(string filename = "")
        {
            EM_RES res = EM_RES.OK;
            bool bok = true;
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = CardList;
            }
            else
            {
                TempCardList = CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                res = card.ChkOnline(filename);
                if (res != EM_RES.OK) bok = false;
            }
            if (bok) return EM_RES.OK;
            else return EM_RES.ERR;
        }
        #endregion

        #region 关闭控制卡
        public static EM_RES Close()
        {
            EM_RES res = EM_RES.OK;
            bool bok = true;
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = CardList;
            }
            else
            {
                TempCardList = CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                res = card.Close();
                if (res != EM_RES.OK) bok = false;
            }
            if (bok) return EM_RES.OK;
            else return EM_RES.ERR;
        }
        #endregion

        #region 设置轴到工作速度
        public static EM_RES SetAllAxToWorkSpd(double persent = 1.0)
        {
            EM_RES ret = EM_RES.OK;
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = CardList;
            }
            else
            {
                TempCardList = CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                if (card.AxList == null) continue;
                foreach (AXIS ax in card.AxList)
                {
                    ret = ax.SetToWorkSpd(persent);
                    if (ret != EM_RES.OK) return ret;
                }
            }
            return ret;
        }
        public static EM_RES SetAllAxToManualSpd()
        {
            EM_RES ret = EM_RES.OK;
            List<CARD> TempCardList;
            if (PT_SET.LbEn)
            {
                TempCardList = CardList;
            }
            else
            {
                TempCardList = CardList1;
            }
            foreach (CARD card in TempCardList)
            {
                if (card.AxList == null) continue;
                foreach (AXIS ax in card.AxList)
                {
                    ret = ax.SetToManualHighSpd();
                    if (ret != EM_RES.OK) return ret;
                }
            }
            return ret;
        }
        //public static EM_RES SetMainAxToCaliSpd()
        //{
        //    EM_RES ret = EM_RES.OK;
        //    foreach (AXIS ax in AxisListMain)
        //    {
        //        ret = ax.SetSpeed(ax.spd_start, ax.spd_work / 3, ax.spd_stop, 1, 1);
        //        if (ret != EM_RES.OK) return ret;
        //    }
        //    return ret;
        //}
        #endregion

        #region 所有轴停止
        public static void AllAxStop(List<AXIS> list_ax = null)
        {
            if (list_ax == null)
            {
                List<CARD> TempCardList;
                if (PT_SET.LbEn)
                {
                    TempCardList = CardList;
                }
                else
                {
                    TempCardList = CardList1;
                }
                foreach (CARD card in TempCardList)
                {
                    if (card.AxList == null) continue;
                    card.AllCardStop();
                }
            }
            else
            {
                foreach (AXIS ax in list_ax)
                {
                    ax.Stop();
                }
            }

        }
        public static bool AllAxSvrOn()
        {
            bool all_on = true;
            foreach (CARD card in CardList)
            {
                if (card.AxList == null) continue;
                foreach (AXIS ax in card.AxList)
                {
                    if (ax.mt_type != AXIS.MT_TYPE.VIRTUAL && !ax.isSVRON)
                    {
                        all_on = false;
                        ax.SVRON = true;
                    }
                }
            }
            return all_on;
        }
        #endregion

        #region 先升Z再定位
        public static EM_RES ZupMove(ref bool bquit, ref AXIS ax_x, double xpos, bool OtherMov = false, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return ZupMove(ref bquit, ref ax_x, xpos, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, false, time_out_ms, bdoevent);
        }
        public static EM_RES ZupMove(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, bool OtherMov = false, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return ZupMove(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_null, 0, ref ax_null, 0, false, time_out_ms, bdoevent);
        }
        public static EM_RES ZupMove(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, bool OtherMov = false, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return ZupMove(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_z, zpos, ref ax_null, 0, false, time_out_ms, bdoevent);
        }
        public static EM_RES ZupMove(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, ref AXIS ax_r, double rpos, bool OtherMov = false, int time_out_ms = 20000, bool bdoevent = false)
        {
            EM_RES ret;
            bool bz1_up = false;
            bool bz2_up = false;
            double gopos = 0;

            if (ax_x != null)
            {
                if (ax_x.m_id == MT.AXIS_UDL1_X.m_id || ax_x.m_id == MT.AXIS_UDL1_Y.m_id || ax_x.m_id == MT.AXIS_UDL1_U1.m_id || ax_x.m_id == MT.AXIS_UDL1_U2.m_id) bz1_up = true;
                if (ax_x.m_id == MT.AXIS_UDL2_X.m_id || ax_x.m_id == MT.AXIS_UDL2_Y.m_id || ax_x.m_id == MT.AXIS_UDL2_U1.m_id || ax_x.m_id == MT.AXIS_UDL2_U2.m_id) bz2_up = true;
            }

            if (ax_x == null && ax_y != null)
            {
                if (ax_y.m_id == MT.AXIS_UDL1_X.m_id || ax_y.m_id == MT.AXIS_UDL1_Y.m_id || ax_y.m_id == MT.AXIS_UDL1_U1.m_id || ax_y.m_id == MT.AXIS_UDL1_U2.m_id) bz1_up = true;
                if (ax_y.m_id == MT.AXIS_UDL2_X.m_id || ax_y.m_id == MT.AXIS_UDL2_Y.m_id || ax_y.m_id == MT.AXIS_UDL2_U1.m_id || ax_y.m_id == MT.AXIS_UDL2_U2.m_id) bz2_up = true;
            }

            if (ax_x == null && ax_y == null && ax_z != null)
            {
                if (ax_z.m_id == MT.AXIS_UDL1_X.m_id || ax_z.m_id == MT.AXIS_UDL1_Y.m_id || ax_z.m_id == MT.AXIS_UDL1_U1.m_id || ax_z.m_id == MT.AXIS_UDL1_U2.m_id) bz1_up = true;
                if (ax_z.m_id == MT.AXIS_UDL2_X.m_id || ax_z.m_id == MT.AXIS_UDL2_Y.m_id || ax_z.m_id == MT.AXIS_UDL2_U1.m_id || ax_z.m_id == MT.AXIS_UDL2_U2.m_id) bz2_up = true;
            }

            if (ax_x == null && ax_y == null && ax_z == null && ax_r != null)
            {
                if (ax_r.m_id == MT.AXIS_UDL1_X.m_id || ax_r.m_id == MT.AXIS_UDL1_Y.m_id || ax_r.m_id == MT.AXIS_UDL1_U1.m_id || ax_r.m_id == MT.AXIS_UDL1_U2.m_id) bz1_up = true;
                if (ax_r.m_id == MT.AXIS_UDL2_X.m_id || ax_r.m_id == MT.AXIS_UDL2_Y.m_id || ax_r.m_id == MT.AXIS_UDL2_U1.m_id || ax_r.m_id == MT.AXIS_UDL2_U2.m_id) bz2_up = true;
            }

            if (ax_x != null && (ax_x.m_id == MT.AXIS_UDL1_X.m_id || ax_x.m_id == MT.AXIS_UDL2_X.m_id)) gopos = xpos;
            else if (ax_y != null && (ax_y.m_id == MT.AXIS_UDL1_X.m_id || ax_y.m_id == MT.AXIS_UDL2_X.m_id)) gopos = ypos;
            else if (ax_z != null && (ax_z.m_id == MT.AXIS_UDL1_X.m_id || ax_z.m_id == MT.AXIS_UDL2_X.m_id)) gopos = zpos;
            else if (ax_r != null && (ax_r.m_id == MT.AXIS_UDL1_X.m_id || ax_r.m_id == MT.AXIS_UDL2_X.m_id)) gopos = rpos;

            if (bz1_up)
            {
                if (Math.Abs(MT.AXIS_UDL1_Z.fenc_pos) > 3)
                {
                    ret = Move(ref bquit, ref MT.AXIS_UDL1_Z, 0, time_out_ms, bdoevent);
                    if (ret != EM_RES.OK) return ret;
                }
                if ((ax_x != null && ax_x.m_id == MT.AXIS_UDL1_X.m_id) || (ax_y != null && ax_y.m_id == MT.AXIS_UDL1_X.m_id) || (ax_z != null && ax_z.m_id == MT.AXIS_UDL1_X.m_id) || (ax_r != null && ax_r.m_id == MT.AXIS_UDL1_X.m_id))
                {
                    if (Math.Abs(gopos) + Math.Abs(MT.AXIS_UDL2_X.fenc_pos) + xsafedis > UpDownLoad.DisAxisX)
                    {
                        ret = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_z, 0);
                        if (ret != EM_RES.OK) return ret;
                        ret = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad2.ax_x, 0, ref COM.UDLoad2.ax_y, 0);
                        if (ret != EM_RES.OK) return ret;
                    }
                }
            }

            else if (bz2_up)
            {
                if (Math.Abs(MT.AXIS_UDL2_Z.fenc_pos) > 3)
                {
                    ret = Move(ref bquit, ref MT.AXIS_UDL2_Z, 0, time_out_ms, bdoevent);
                    if (ret != EM_RES.OK) return ret;
                }
                if ((ax_x != null && ax_x.m_id == MT.AXIS_UDL2_X.m_id) || (ax_y != null && ax_y.m_id == MT.AXIS_UDL2_X.m_id) || (ax_z != null && ax_z.m_id == MT.AXIS_UDL2_X.m_id) || (ax_r != null && ax_r.m_id == MT.AXIS_UDL2_X.m_id))
                {
                    if (Math.Abs(gopos) + Math.Abs(MT.AXIS_UDL1_X.fenc_pos) + xsafedis > UpDownLoad.DisAxisX)
                    {
                        if (VAR.gsys_set.status != EM_SYS_STA.RUN)
                        {
                            ret = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_z, 0);
                            if (ret != EM_RES.OK) return ret;
                            ret = MT.Move(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, 0, ref COM.UDLoad1.ax_y, 0);
                            if (ret != EM_RES.OK) return ret;
                        }
                        else
                        {
                            int n = 0;
                            while (true)
                            {
                                if (Math.Abs(gopos) + Math.Abs(MT.AXIS_UDL1_X.fenc_pos) + xsafedis <= UpDownLoad.DisAxisX) break;
                                Thread.Sleep(100);
                                if (bquit) return EM_RES.QUIT;
                                if (n++ > 100)
                                {
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("上下料模块2等待上下料模块1 X方向进入安全位置超时10S") : string.Format("Updwload2 wait for updwload 1 x axis moves to safe location timeout (10s)         (上下料模块2等待上下料模块1 X方向进入安全位置超时10S)"), DReport.EmErrCode.SoftwareProtect, (int)DReport.EmHareware.UpDownLoad, ERR_ALM.EmErrItem.MoveProtect);
                                    return EM_RES.ERR;
                                }

                            }
                        }

                    }
                }
            }


            ret = Move(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_z, zpos, ref ax_r, rpos, time_out_ms, bdoevent);
            return ret;
        }
        #endregion

        #region 定位
        public static EM_RES MoveHandle(ref bool bquit, bool bEnX, bool bEnY, bool bEnZ, bool bEnA, bool bIsU1, ref ST_XYZA StPos, string CamName = "", int Delay = 200)
        {
            EM_RES ret;
            AXIS ax_x = null, ax_y = null, ax_z = null, ax_u = null, ax_null = null;
            AXIS ax_zz = MT.AXIS_UDL1_Z;
            if (CamName == "CamUp2" || CamName == "CamDw2")
            {
                if (bEnX) ax_x = MT.AXIS_UDL2_X;
                if (bEnY) ax_y = MT.AXIS_UDL2_Y;
                if (bEnZ) ax_z = MT.AXIS_UDL2_Z;
                if (bEnA && bIsU1) ax_u = MT.AXIS_UDL2_U1;
                if (bEnA && !bIsU1) ax_u = MT.AXIS_UDL2_U2;
                ax_zz = MT.AXIS_UDL2_Z;
            }
            else
            {
                if (bEnX) ax_x = MT.AXIS_UDL1_X;
                if (bEnY) ax_y = MT.AXIS_UDL1_Y;
                if (bEnZ) ax_z = MT.AXIS_UDL1_Z;
                if (bEnA && bIsU1) ax_u = MT.AXIS_UDL1_U1;
                if (bEnA && !bIsU1) ax_u = MT.AXIS_UDL1_U2;

            }

            ret = Move(ref bquit, ref ax_null, 0, ref ax_null, 0, ref ax_zz, 0, ref ax_null, 0);
            if (ret != EM_RES.OK) return ret;
            //定位
            ret = Move(ref bquit, ref ax_x, StPos.x, ref ax_y, StPos.y, ref ax_null, 0, ref ax_u, StPos.a);
            if (ret != EM_RES.OK) return ret;
            //Z下降
            //ret = Move(ref bquit, ref ax_null,0, ref ax_null,0, ref ax_z, StPos.z, ref ax_null,0);
            //if (ret != EM_RES.OK) return ret;
            Thread.Sleep(Delay);
            if (ax_x != null) StPos.x = ax_x.fenc_pos;
            if (ax_y != null) StPos.y = ax_y.fenc_pos;
            //if (ax_z != null) StPos.z = ax_z.fenc_pos;
            return EM_RES.OK;
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return Move(ref bquit, ref ax_x, xpos, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, time_out_ms, bdoevent);
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, int time_out_ms = 20000, bool bdoevent = false,bool bchkps=true)
        {
            AXIS ax_null = null;
            return Move(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, time_out_ms, bdoevent, bchkps);
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return Move(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_z, zpos, ref ax_null, 0, ref ax_null, 0, ref ax_null, 0, time_out_ms, bdoevent);
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, ref AXIS ax_r, double rpos, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return Move(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_z, zpos, ref ax_r, rpos, ref ax_null, 0, ref ax_null, 0, time_out_ms, bdoevent);
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, ref AXIS ax_r, double rpos, ref AXIS ax_w, double wpos, int time_out_ms = 20000, bool bdoevent = false)
        {
            AXIS ax_null = null;
            return Move(ref bquit, ref ax_x, xpos, ref ax_y, ypos, ref ax_z, zpos, ref ax_r, rpos, ref ax_w, wpos, ref ax_null, 0, time_out_ms, bdoevent);
        }
        public static EM_RES Move(ref bool bquit, ref AXIS ax_x, double xpos, ref AXIS ax_y, double ypos, ref AXIS ax_z, double zpos, ref AXIS ax_r, double rpos, ref AXIS ax_w, double wpos, ref AXIS ax_u, double upos, int time_out_ms = 20000, bool bdoevent = false,bool bchkps=true)
        {
            EM_RES ret = EM_RES.OK;
            //start move
            if (ax_x != null)
            {
                ret = ax_x.MoveTo(ref bquit, xpos, bchkpos: bchkps);
                if (ret != EM_RES.OK)
                    goto MOVE_END;
            }
            if (ax_y != null)
            {
                ret = ax_y.MoveTo(ref bquit, ypos, bchkpos: bchkps);
                if (ret != EM_RES.OK) goto MOVE_END;

            }
            if (ax_z != null)
            {
                ret = ax_z.MoveTo(ref bquit, zpos, bchkpos: bchkps);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_r != null)
            {
                ret = ax_r.MoveTo(ref bquit, rpos);
                if (ret != EM_RES.OK) goto MOVE_END;
            }

            if (ax_w != null)
            {
                ret = ax_w.MoveTo(ref bquit, wpos);
                if (ret != EM_RES.OK) goto MOVE_END;
            }

            if (ax_u != null)
            {
                ret = ax_u.MoveTo(ref bquit, upos);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            //wait
            if (ax_x != null)
            {
                ret = ax_x.WaitForMoveDone(ref bquit, xpos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_y != null)
            {
                ret = ax_y.WaitForMoveDone(ref bquit, ypos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_z != null)
            {
                ret = ax_z.WaitForMoveDone(ref bquit, zpos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_r != null)
            {
                ret = ax_r.WaitForMoveDone(ref bquit, rpos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_w != null)
            {
                ret = ax_w.WaitForMoveDone(ref bquit, wpos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            if (ax_u != null)
            {
                ret = ax_u.WaitForMoveDone(ref bquit, upos, time_out_ms, bdoevent);
                if (ret != EM_RES.OK) goto MOVE_END;
            }
            return EM_RES.OK;

        MOVE_END:
            if (ax_x != null) ax_x.Stop();
            if (ax_y != null) ax_y.Stop();
            if (ax_z != null) ax_z.Stop();
            if (ax_r != null) ax_r.Stop();
            if (ax_w != null) ax_w.Stop();
            if (ax_u != null) ax_u.Stop();
            return ret;
        }
        #endregion

        #region 按键灯
        public static void OnlyKeyLOn(GPIO keyio = null)
        {
            MT.GPIO_OUT_KL_START.SetOff();
            MT.GPIO_OUT_KL_STOP.SetOff();
            MT.GPIO_OUT_KL_RESET.SetOff();
            if (keyio != null)
                keyio.SetOn();
        }

        #endregion

        #region 蜂鸣器
        public static EM_RES Beeper(int tmr)
        {
            Task beep = new Task(() =>
            {
                if (tmr > 0)
                {
                    EM_RES ret = GPIO_OUT_ALM_BEEPER.SetOn();
                    if (ret == EM_RES.OK)
                    {
                        Thread.Sleep(tmr);
                        ret = GPIO_OUT_ALM_BEEPER.SetOff();
                    }
                }
            }
            );
            beep.Start();
            return EM_RES.OK;
        }
        #endregion

        #region 轴复位
        public static EM_RES AxisHome(ref bool bquit, params AXIS[] axs)
        {
            //home task start
            foreach (AXIS ax in axs)
            {
                if (ax != null) ax.HomeTask(10000);
            }

            //wait end
            bool bok = true;
            while (true)
            {
                bok = true;
                foreach (AXIS ax in axs)
                {
                    if (ax != null && !ax.HomeTaskisEnd) bok = false;
                }
                if (bok) break;

                Application.DoEvents();
                Thread.Sleep(10);

                //quit
                if (bquit)
                {
                    foreach (AXIS ax in axs)
                    {
                        if (ax != null && !ax.HomeTaskisEnd) ax.Stop();
                    }
                    return EM_RES.ERR;
                }
            }

            //check result
            bok = true;
            foreach (AXIS ax in axs)
            {
                if (ax != null && ax.home_status != AXIS.HOME_STA.OK) bok = false;
            }
            if (bok == false)
            {
                foreach (AXIS ax in axs)
                {
                    if (ax != null && !ax.HomeTaskisEnd) ax.Stop();
                }
                return EM_RES.ERR;
            }

            return EM_RES.OK;
        }
        public static void AxisHomeQuit(params AXIS[] axs)
        {
            foreach (AXIS ax in axs)
            {
                if (ax != null)
                {
                    ax.bhomequit = true;
                    ax.Stop();
                }
            }
        }
        public static void AxisHomeQuit(List<AXIS> axs)
        {
            foreach (AXIS ax in axs)
            {
                if (ax != null)
                {
                    ax.bhomequit = true;
                    ax.Stop();
                }
            }
        }
        #endregion

        #region 归位或避让
        public static EM_RES gotopos(bool breset = true, bool brun = false)
        {
            EM_RES ret = EM_RES.OK;

            ////检查是否需要移动
            //bool bneedmove = false;
            //if (breset && ((Math.Abs(MT.AXIS_X.fenc_pos) > 0.05))) bneedmove = true;
            //if (breset && ((Math.Abs(MT.AXIS_Y.fenc_pos) > 0.05))) bneedmove = true;
            //if (breset && ((Math.Abs(MT.AXIS_R.fenc_pos) > 0.05))) bneedmove = true;
            //if (!bneedmove) return CONST.RES_OK;

            ////safe check
            //ret = ChkSafeSen();
            //if (ret != CONST.RES_OK) return ret;

            ////正在进出料，不能移动
            //if (!brun && FDH.binuse)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "出料中gotopos定位");
            //    Thread.Sleep(10);
            //    if (FDH.binuse) return CONST.RES_MOVE_PROTECT;
            //}

            ////运行中，不能移动
            //if (COM.MounterGetRunStatus() == CONST.SYS_STATUS_RUN)
            //{
            //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "运行中gotopos定位");
            //    return CONST.RES_ERR;
            //}

            //if (brun || VAR.gsys_set.status == CONST.SYS_STATUS_STANDBY || VAR.gsys_set.status == CONST.SYS_STATUS_PAUSE)
            //{
            //    VAR.gsys_set.bquit = false;
            //    ret = MT.ChkSafeSen();
            //    if (ret != CONST.RES_OK) return ret;

            //    if (VAR.gsys_set.status == CONST.SYS_STATUS_STANDBY) VAR.sys_inf.Set(CONST.EM_ALM_STA.NOR_GREEN, "就绪", -1, true);
            //    else if (VAR.gsys_set.status == CONST.SYS_STATUS_PAUSE) VAR.sys_inf.Set(CONST.EM_ALM_STA.NOR_GREEN, "暂停", -1, true);

            //    //set to workspd
            //    foreach (AXIS ax in AxisListExceptFd) ax.SetToWorkSpd();

            //    try
            //    {
            //        bgotopos = true;
            //        if (breset)
            //        {
            //            ret = ZupMove(ref VAR.gsys_set.bquit, ref AXIS_X, 0, ref AXIS_Y, 0, ref AXIS_R, 0, 10000, true);
            //            if (ret != CONST.RES_OK) return CONST.RES_ERR;
            //        }
            //        else
            //        {
            //            ret = ZupMove(ref VAR.gsys_set.bquit, ref AXIS_X, FDH.st_pos_ready.x, ref AXIS_Y, COM.MTER1.pos_br_y, ref AXIS_R, 0, 10000, true);
            //            if (ret != CONST.RES_OK) return CONST.RES_ERR;
            //        }
            //    }
            //    finally
            //    {
            //        bgotopos = false;
            //    }
            //}
            //else
            //{
            //    if (breset) VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "非待机状态禁止复位！");
            //    else VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "非待机状态禁止定位避让位!");
            //    return CONST.RES_ERR;
            //}
            return ret;
        }
        #endregion

        #region 相机触发
        public static EM_RES CamUp1Triger(int dly = 100)
        {
            //EM_RES res = GPIO_OUT_UL_CAM_UP1.SetOn();
            //Thread.Sleep(dly);
            //res = GPIO_OUT_UL_CAM_UP1.SetOff();          
            EM_RES res = GPIO_OUT_UL_CAM_UP1.SetOff();
            Thread.Sleep(dly);
            GPIO_OUT_UL_CAM_UP1.SetOn();

            return res;
        }
        public static EM_RES CamUp2Triger(int dly = 100)
        {
            //EM_RES res = GPIO_OUT_UL_CAM_UP2.SetOn();
            //Thread.Sleep(dly);
            //res = GPIO_OUT_UL_CAM_UP2.SetOff();    
            EM_RES res = GPIO_OUT_UL_CAM_UP2.SetOff();
            Thread.Sleep(dly);
            GPIO_OUT_UL_CAM_UP2.SetOn();

            return res;
        }
        public static EM_RES CamDw1Triger(int dly = 100)
        {
            //EM_RES res = GPIO_OUT_UL_CAM_DW1.SetOn();
            //Thread.Sleep(dly);
            //res = GPIO_OUT_UL_CAM_DW1.SetOff();   
            EM_RES res = GPIO_OUT_UL_CAM_DW1.SetOff();
            Thread.Sleep(dly);
            res = GPIO_OUT_UL_CAM_DW1.SetOn();

            return res;
        }
        public static EM_RES CamDw2Triger(int dly = 100)
        {
            //EM_RES res = GPIO_OUT_UL_CAM_DW2.SetOn();
            //Thread.Sleep(dly);
            //res = GPIO_OUT_UL_CAM_DW2.SetOff();
            EM_RES res = GPIO_OUT_UL_CAM_DW2.SetOff();
            Thread.Sleep(dly);
            res = GPIO_OUT_UL_CAM_DW2.SetOn();
            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "CamDown Triger");
            return res;
        }
        #endregion

        #region SecsParam
        public static bool IsAllowStart = false;
        public static bool IsAllowStartByTray = false;

        public static bool IsAllowStartUpdate = false;
        public static bool IsAllowStartUpdateByTray = false;

        #endregion
    }
}