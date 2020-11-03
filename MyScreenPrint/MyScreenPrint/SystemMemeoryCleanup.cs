using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MyScreenPrint
{
    class SystemMemeoryCleanup
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //Process[] processes = Process.GetProcesses();
            Process processes = Process.GetProcessById(Process.GetCurrentProcess().Id);
            EmptyWorkingSet(processes.Handle);
            //foreach (Process process in processes)
            //{
            //    //以下系统进程没有权限，所以跳过，防止出错影响效率。  
            //    if ((process.ProcessName == "System") && (process.ProcessName == "Idle"))
            //        continue;
            //    try
            //    {
            //        EmptyWorkingSet(process.Handle);
            //    }
            //    catch
            //    {
            //    }
            //}
        }

    }
}
