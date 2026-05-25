using MotionCtrl;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UI
{
    public class TestPC
    {
        public struct DeviceStruct
        {
            public int angle;      //下旋角度	【测试软件提供】			
            public int lenpitch;   //螺距		【测试软件提供】			
            public int spaceAngle; //半间隙角度  【测试软件提供】			
        }
        public enum EM_RES
        {
            [Description("成功")]
            OK = 0,
            [Description("错误")]
            ERR = -1,
            [Description("参数错误")]
            ERR_PARAM = -2,
            [Description("取消")]
            ERR_QUIT = -3,
            [Description("超时")]
            ERR_TIMEOUT = -4,
            [Description("失联")]
            ERR_LINK = -5,
        }
        public struct InfoData
        {
            public int slaveid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public char[] ip;
            public int s;
            public int start_mark;
            public int result_mark;
            public int pos_idx;
            public int res_num;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public UInt16[] res;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (32 * 32 + 32 + 1))]
            public char[] barcode;
            public int status;
        };

        [DllImport("dllforcomv8.dll", EntryPoint = "StartTestFlow", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartTestFlow(int id, int status, char[] barcode);
        [DllImport("dllforcomv8.dll", EntryPoint = "WaitTestResultA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WaitTestResultA(int id, ref int status, int[] res, ref int NumofResult, ref DeviceStruct pDeviceParam, ref bool bquit, int delay, char[] barcode);
        [DllImport("dllforcomv8.dll", EntryPoint = "WaitTestResult", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WaitTestResult(int id, ref int status, int[] res, ref int NumofResult, ref bool bquit, int delay);
        [DllImport("dllforcomv8.dll", EntryPoint = "NextTest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NextTest(int id, int status, int delay, ref bool bquit);
        [DllImport("dllforcomv8.dll", EntryPoint = "GetInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetInfo(int id, ref InfoData dat);

        [DllImport("dllforcomv8.dll", EntryPoint = "GetStatus", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetStatus(int id, ref int status);

        [DllImport("SFC_Control.dll", EntryPoint = "UploadRiskCode", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]

        public static extern int UploadRiskCode(string codes, int netType);
    }


}
