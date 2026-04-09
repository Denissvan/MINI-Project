using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MotionCtrl
{
    public class CONST
    {
        #region 吸嘴定义
        public const ushort FLY_COUNT = 3;
       // public static double[] fly_trg_spd = new double[CONST.FLY_COUNT] { 300, 600, 1000 };
        #endregion

        #region 拍照流程定义
        //下相机流程
        public  static string[] XtDwFw = new string[2] { "Xt1_Shp", "Xt2_Shp" };//空吸头流程
        public  static string[] ModDwFw = new string[2] { "ModDw1_Shp", "ModDw2_Shp" };//下相机流程
        public static string[] DwFindQrCodeFw = new string[2] { "FindQrCode1", "FindQrCode2" };//下相机流程
        //上相机流程
        public static string OvenCheckFw = "OvenUp_Shp";//模组拍照流程
        public static string  ModUpFw = "ModUp_Shp";//模组拍照流程
        public static string  WsModUpFw = "WsMod_Shp";//工站上模组拍照流程
        public static string  WsModUpFw2 = "WsMod_Shp2";//工站上模组拍照流程
        public static string WsModAfterCloseFw = "WsModAfterClose_Shp";//夹具合上后工站上模组拍照流程
        public static string WsUpFw = "WsUp_Shp";//工站拍照流程
        public static string TrayUpFw = "TrayUp_Shp";//料盘拍照流程
        public static string LiZhuFw = "LiZhu_Shp";//
        public static string JigSanFw = "Jig_Shp";//夹具拍照流程gy0123
        public static string BoxCheck1 = "BoxCheck1_Shp";//夹具安装检测
        public static string BoxCheck2 = "BoxCheck2_Shp";//夹具安装检测
        public static string FindQrCodeFw = "FindQrCode";//拍二维码流程
        public static string WsModQrcode_Shp = "WsModQrcode_Shp";//回检拍二维码流程
        #endregion


    }
}
