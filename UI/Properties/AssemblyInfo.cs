using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("WLTmini")]
[assembly: AssemblyDescription("LiDayuan&LiZhiCheng")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Sunny")]
[assembly: AssemblyProduct("WLTmini")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//将 ComVisible 设置为 false 将使此程序集中的类型
//对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("77c148f5-418a-4fb4-af50-deb5934664e3")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: :新基地专用,取消清空功能
//增加二维码识别治具位置“_1”表示左"_2"表示右
//2.5.5  增加夹具扫码相机参数设置
//2.3.0.6优化保存测试时间和夹具扫码开启
//2.3.0.7优化WS类的运行函数，解决停机导致的测试结果异常，优化报警上报
//2.3.0.8优化三色灯状态显示，停机或待料显示黄灯
//2309 优化压伤处理，优化相机参数设置导致的卡
//2.3.1.0 后开图，优化反转后停止，开始重复上料
//2312增加远程设备看板数据上传在数据库类增加
//2322增加模组idx，优化拍照NG放回料盘,增加开机上报夹具
//统一版本2400，增加远程监控版本
//2401取料盘尝试次数增加到15
//[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.4.0.1")]
[assembly: AssemblyFileVersion("2.4.0.1")]
