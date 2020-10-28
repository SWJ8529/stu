using Microsoft.Win32;
using MyScreenPrint.Utils;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyScreenPrint
{
    public class RootClass
    {

        /// <summary>
        /// 修改程序在注册表中的键值
        /// </summary>
        /// <param name="flag">1:开机启动</param>
        public static void AutoStart(bool isAuto)
        {
            try
            {
                if (isAuto == true)
                {
                    RegistryKey R_local = Registry.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.SetValue("Print", Application.ExecutablePath);
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    R_run.DeleteValue("Print", false);
                    R_run.Close();
                    R_local.Close();
                }


                //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
            }
            catch (Exception)
            {
                //MessageBoxDlg dlg = new MessageBoxDlg();
                //dlg.InitialData("您需要管理员权限修改", "提示", MessageBoxButtons.OK, MessageBoxDlgIcon.Error);
                //dlg.ShowDialog();
                MessageBox.Show("您需要管理员权限修改", "提示");
            }
        }

        /// <summary>
        /// 使用计划任务实现开机启动
        /// </summary>
        public static void AutoStart(bool isAuto,bool istask=true)
        {
            try
            {
                if (isAuto)
                {
                    TaskSchedulerTool.Create("ScreenPrint", Application.ExecutablePath, "Print", "图片识别程序自启动计划任务");
                }
                else
                {
                    TaskSchedulerTool.Delete("ScreenPrint");
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
