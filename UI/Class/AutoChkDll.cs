using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UI.Class
{
  public  enum PUCFFactoryMode
    {
        PUCFFactoryMode_UnKnown = -1,//未知
        PUCFFactoryMode_Product = 0,//生产模式
        PUCFFactoryMode_Check = 1,//点检模式
        PUCFFactoryMode_Create = 2,//点检生成模式
    };
    public static class AutoChkDLL
    {
        //功能：获取测试软件模式
        //输入参数：IP：测试软件IP地址
        //输出参数：iFactoryMode：测试软件模式
        //输出参数：iCheckModeTemp：如果获取到的模式是点检模式，此参数表示当前色温，目前支持3000,4000,5000，后续可扩展
        //返回值：0：OK，其他：失败
          [DllImport("PUCFTestServiceCall.dll", EntryPoint = "iPUCFTestServiceGetFactoryMode")]
           public static extern int iPUCFTestServiceGetFactoryMode( string IP, out PUCFFactoryMode iFactoryMode, out int iCheckModeTemp);

        //public static int iPUCFTestServiceGetFactoryMode(ref string IP, out PUCFFactoryMode iFactoryMode, out int iCheckModeTemp)
        //{
        //    iFactoryMode = PUCFFactoryMode.PUCFFactoryMode_Check;
        //    iCheckModeTemp = 22;
        //    return 0;
        //}
        //功能：设置测试软件模式
        //输入参数：IP：测试软件IP地址
        //输入参数：iFactoryMode：测试软件模式
        //输入参数：iCheckModeTemp：如果设置的模式是点检模式，此参数表示设置色温，目前支持3000,4000,5000，后续可扩展
        //返回值：0：OK，其他：失败
        [DllImport("PUCFTestServiceCall.dll", EntryPoint = "iPUCFTestServiceSetFactoryMode")]
        public static extern int iPUCFTestServiceSetFactoryMode( string IP, PUCFFactoryMode iFactoryMode, int iCheckModeTemp);
        //public static int iPUCFTestServiceSetFactoryMode(ref string IP, PUCFFactoryMode iFactoryMode, int iCheckModeTemp)
        //{
        //    return 0;
        //}

        //获取测试软件有几轮点检，每轮点检的色温分别是多少
        //总共3000,4000,5000,10001,10002,10003,20001,20002 ，8种
        //3000-----3000K色温点检
        //4000-----4000K色温点检
        //5000-----5000K色温点检
        //10001-----AFC近焦点检
        //10002-----AFC中距点检
        //10003-----AFC远焦点检
        //20001-----OTP光源点检数据生成
        //20002-----OTP光源点检
        [DllImport("PUCFTestServiceCall.dll", EntryPoint = "iPUCFTestServiceGetAutoCheckInfo")]
        public static extern int iPUCFTestServiceGetAutoCheckInfo(string IP, out int iCheckProcessNum,  int[] iCheckModeTemp);
        //获取本轮点检流程测试最终结果，需要在测试结束后30s以内调用，超时后无法获取
        [DllImport("PUCFTestServiceCall.dll", EntryPoint = "iPUCFTestServiceGetCheckModeResult")]
        public static extern int  iPUCFTestServiceGetCheckModeResult(string IP, out int iCheckModeResult);





    }
}
