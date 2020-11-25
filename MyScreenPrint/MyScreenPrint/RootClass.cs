using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MyScreenPrint
{
    class RootClass
    {

        ///// <summary>
        ///// 修改程序在注册表中的键值
        ///// </summary>
        ///// <param name="flag">1:开机启动</param>
        //public static void AutoStart(bool isAuto)
        //{
        //    try
        //    {
        //        if (isAuto == true)
        //        {
        //            RegistryKey R_local = Registry.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
        //            RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        //            R_run.SetValue("Print", Application.ExecutablePath);
        //            R_run.Close();
        //            R_local.Close();
        //        }
        //        else
        //        {
        //            RegistryKey R_local = Registry.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
        //            RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        //            R_run.DeleteValue("Print", false);
        //            R_run.Close();
        //            R_local.Close();
        //        }


        //        //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
        //    }
        //    catch (Exception)
        //    {
        //        //MessageBoxDlg dlg = new MessageBoxDlg();
        //        //dlg.InitialData("您需要管理员权限修改", "提示", MessageBoxButtons.OK, MessageBoxDlgIcon.Error);
        //        //dlg.ShowDialog();
        //        MessageBox.Show("您需要管理员权限修改", "提示");
        //    }
        //}

        /// <summary>
        /// 创建桌面快捷方式并开机启动的方法
        /// </summary>
        //public static void ShortcutAndStartup(bool isAuto)
        //{
        //    //获取当前系统用户启动目录
        //    string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        //    //获取当前系统用户桌面目录
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    FileInfo fileStartup = new FileInfo(startupPath + "\\图片识别插件.lnk");
        //    FileInfo fileDesktop = new FileInfo(desktopPath + "\\图片识别插件.lnk");

        //    if (isAuto)
        //    {
        //        if (!fileDesktop.Exists)
        //        {
        //            WshShell shell = new WshShell();
        //            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(
        //                  Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +
        //                   "\\" + "图片识别插件.lnk"
        //                   );
        //            shortcut.TargetPath = Application.StartupPath + "\\" + "图片识别插件.exe";//启动更新程序★
        //            shortcut.WorkingDirectory = System.Environment.CurrentDirectory;
        //            shortcut.WindowStyle = 1;
        //            shortcut.Description = "图片识别插件";
        //            shortcut.IconLocation = Application.ExecutablePath;
        //            shortcut.Save();
        //        }

        //        if (!fileStartup.Exists)
        //        {
        //            //获取可执行文件快捷方式的全部路径
        //            string exeDir = desktopPath + "\\图片识别插件.lnk";
        //            //把程序快捷方式复制到启动目录
        //            System.IO.File.Copy(exeDir, startupPath + "\\图片识别插件.lnk", true);
        //        }
        //    }
        //    else
        //    {
        //        if (fileStartup.Exists)
        //        {
        //            System.IO.File.Delete(startupPath + "\\图片识别插件.lnk");
        //        }
        //    }

        //}


    }
}
