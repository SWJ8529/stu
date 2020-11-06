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
                ReadZB zb = new ReadZB();
                zb.readpoint();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                #region 异常处理
                //应用程序处理异常方式
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                //当未捕获线程异常时,进行如下处理
                Application.ThreadException += (sender, e) =>
                {
                    Log.Save(e.ToString());
                    MessageBox.Show("程序运行错误:" + e.Exception.Message);
                };
                //当异常未捕获时进行如下处理
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is System.Exception)
                    {
                        System.Exception exc = e.ExceptionObject as System.Exception;
                        Log.Save(e.ToString());
                        MessageBox.Show("程序运行错误:" + exc.Message);
                    }
                };
                #endregion

                Application.Run(new Form1());
            }catch(Exception ex)
            {
                MessageBox.Show("程序启动失败！"+ex.ToString());
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
