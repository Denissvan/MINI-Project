using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

namespace UI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
       static void Main(string[] args)
        {
            //检测版本或更新
            string udPath = AppDomain.CurrentDomain.BaseDirectory + "update.exe";
            if ((args == null || args.Length == 0) && File.Exists(udPath))
            {
                try
                {
                    File.Copy(udPath, AppDomain.CurrentDomain.BaseDirectory + "temp.exe", true);
                    string curVer =
                        string.Format(@"{0}.{1}", Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);
                    string progressName = Assembly.GetExecutingAssembly().GetName().Name;
                    string lauchPath = Assembly.GetExecutingAssembly().Location;

                    //{ sn,hostIp,curVer,launch,progressName,mode,prjname}
                    string param = String.Format(",,{0},{1},{2},,{3}", curVer, lauchPath, progressName, progressName);
                    var p = Process.Start(AppDomain.CurrentDomain.BaseDirectory + "temp.exe", param);
                    if (p != null) p.WaitForExit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "temp.exe");
                }
            }
            Process[] procs = Process.GetProcesses();
            var app = procs.FirstOrDefault(i => i.ProcessName.Contains("SecsApp"));
            if(app!=null)
            {
                app.CloseMainWindow();
                app.Kill();//WaitForExit()
            }
        
            Thread.Sleep(200);
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Release\SecsApp.exe");
           
            //启动程序
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            if (ps.Length <= 1)
            {
                ImageSaveQueue.gInit();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrMain());
            }
            else
            {
                MessageBox.Show("程序已运行!");
            }
        }
    }
}
