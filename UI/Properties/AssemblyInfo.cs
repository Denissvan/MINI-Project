using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("WLTmini")]
[assembly: AssemblyDescription("3.0.0.1测试版")]
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
//2410 增加复测品单独计数，计算除了复测品的良率数据。
//2410  增加取料后复检真空异常次数超过弹窗报警。增加放料后在放偏检测流程中增加二维码检测对比上料QrCodeChkShow
//2412增加任务监控和位置
//增加相机误触发处理，记录输入图像数量  cam.inputImageErrCnt  inputImageCnt++;//输入图像次数计数，防止午误触发！
//相机取图函数增加出结果后判断防止再次误触发更改
//优化2417版本打印信息，增加上料二维码检测功能设置和频率设置2418
//增加257内存失败提示测试软件重新启动
//2420优化英文版本稼动率异常（运行和run区分状态导致），增加越南语版本
//增加稼动率按小时导出功能
//2422增加mes控制，MT.IsAllowStartByTray
//2421增加稼动率导出功能DatatableToCSV
//2423增加下拍二维码位置bDwAddCapQrcode
//2430增加康耐视扫码bmotorphoto
//屏蔽工位也取料HaveWsPickMd和GetWsPosTeam对屏蔽修改
//2433增加ng比例PT_SET.CntWsNgRateShow
//2434封装工站放偏检测函数
//增加参数表格保存
//2436增加单独在工站扫码，必须是上相机扫码开启二维码回检。bGetOrcodOnWs
//使用编码器位置监控轴定位偏差，解决光箱轴定位偏差导致的偶发风险
//2437优化定位偏差等待时间至50ms到100ms
//2438优化ng比例部分异常+消息打印，注销工位NG连续记录数据保存功能
//2439增加UserNormalSet.bClearSetOffWs清料后关闭工装
//2440增加设置工站取料偏移 NoneRunPosInfo.UserNormalSet.bPickWsDis
//2441运行开始增加secs程序启动检测，增加数据超过5万自动清零功能，运行停止生产数据自动保存，开放工站检二维码关闭功能
//2443 优化马达扫码
//2444 优化设备状态上报逻辑，停机超过五分中上报停机，运行超过10分钟没产出上报停机
// 2445 增加放工站前偏移，增加相机飞拍结果数量监控
// 2446 增加自动点检，增加放料盘吸嘴顺序
//2447 霍尔测试提前翻转
//2448 增加开图延时时间

//[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.0.1")]
[assembly: AssemblyFileVersion("3.0.0.1")]
