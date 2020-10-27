using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace MyScreenPrint
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory+@"\test.txt", FileMode.Open, FileAccess.Write);
            StreamWriter sr = new StreamWriter(fs);
            try
            {
                //0106add  一次打开一个应用程序
                Process instance = RunningInstance();
                if (instance != null)
                {
                    if (instance.MainWindowHandle.ToInt32() == 0) //是否托盘化
                    {
                        MessageBox.Show("程序已打开并托盘化");
                        return;
                    }
                    //1.2 已经有一个实例在运行
                    HandleRunningInstance(instance);
                    return;
                }

                //////判断是否需要开机启动
                //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //bool isStartUp = bool.Parse(config.AppSettings.Settings["isStartUp"].Value);
                //RootClass.AutoStart(isStartUp);

                ReadZB zb = new ReadZB();
                zb.readpoint();
                sr.WriteLine("开机启动！");//开始写入值
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);                                                            
                Application.Run(new Form1());
            }catch(Exception ex)
            {
                sr.WriteLine(ex.ToString());//开始写入值
            }
            finally
            {
                sr.Close();
                fs.Close();
            }
        }


        #region 确保程序只运行一个实例
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表 
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程 
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回已经存在的进程
                        return process;
                    }
                }
            }
            return null;
        }
        //3.已经有了就把它激活，并将其窗口放置最前端
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, 1); //调用api函数，正常显示窗口
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);
        #endregion
    }


}
