using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Text;

namespace MyScreenPrint
{
    public partial class Form1 : Form
    {
        // 加载截图和图片识别类
        ImageRecognition image_ecognition = new ImageRecognition();
        //// 创建悬浮窗上的文本
        //Label FloatLabel = new Label();
        // 创建悬浮窗
        FloatForm floatFormNew = new FloatForm();
        // 创建是否开机启动按钮
        CheckBox isStartUp = new CheckBox();
        // 创建是否开机启动识别服务按钮
        CheckBox isStartService = new CheckBox();
        // 更多设置窗体
        Form setting = new Form();
        // 读取app.config配置文件
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        // 定时时间
        static String Interval;
        // 截图窗口
        Cutter cutter = null;
        // 绘图参数
        enum Tools { Pen, Text };

        public Form1()
        {
            InitializeComponent();
            // 双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                           ControlStyles.AllPaintingInWmPaint,
                           true);
            this.UpdateStyles();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化坐标数据
            ReadZB zb = new ReadZB();
            zb.readpoint();

            label1.Text = "设置坐标个数:" + ReadZB.point.Count;
            Interval = ConfigurationManager.AppSettings["Time"];
            timetext.Text = Interval;
            if (ReadZB.point.Count > 0)
            {
                foreach (string line in ReadZB.point)
                {
                    textBox1.Text += line + "\r\n";
                }
            }


            floatFormNew.Show();
            //floatForm.Show();

            bool isStartService = bool.Parse(config.AppSettings.Settings["isStartService"].Value);
            if (isStartService)
            {
                //最小化窗口
                WindowState = FormWindowState.Minimized;
                Thread.Sleep(200);
                //防止重复执行
                if (backgroundWorker1.IsBusy)
                {
                    return;
                }
                backgroundWorker1.RunWorkerAsync();
                timebtn.Text = "停止";
            }
        }

        /// <summary>
        /// 截图和识别功能
        /// </summary>
        /// <param name="flag">true根据坐标截图到桌面，false不生成</param>
        private void changeLabelText(bool flag)
        {
            floatFormNew.FloatLabel.BeginInvoke(new Action(() => {
                PICResponse response = JsonConvert.DeserializeObject<PICResponse>(image_ecognition.SaveImgNew(flag));
                if (string.IsNullOrEmpty(response.data)) 
                {
                    floatFormNew.FloatLabel.Text = "金额:0";
                }
                else
                {
                    floatFormNew.FloatLabel.Text = "金额:"+ response.data;
                }
            }));
        }



        #region 按钮事件

        // 点击按钮开始捕捉屏幕
        private void printScrBtn_Click(object sender, EventArgs e)
        {
            // 新建一个截图窗口
            cutter = new Cutter();
            // 隐藏原窗口
            Hide();
            Thread.Sleep(200);

            // 设置截图窗口的背景图片
            Bitmap bmp = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(bmp.Width, bmp.Height));
            cutter.BackgroundImage = bmp;
            // 显示原窗口
            Show();
            // 显示截图窗口
            cutter.WindowState = FormWindowState.Maximized;
            cutter.ShowDialog();
            textBox1.Text = "";
            foreach (string p in ReadZB.point)
            {
                textBox1.Text += p+"\r\n";
            }
            
            label1.Text = "设置坐标个数:" + ReadZB.point.Count;
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr= MessageBox.Show("这是测试按钮！点击后会根据坐标截取指定区域的图片，文件将生成在桌面上。","提示",MessageBoxButtons.YesNo);
            // 隐藏当前窗口防止需要识别的内容被遮挡
            Hide();
            Thread.Sleep(200);
            if (dr == DialogResult.Yes)
            {
                //生成图片
                changeLabelText(true);
                //image_ecognition.SaveImgNew(true);
            }
            else
            {
                changeLabelText(false);
            }
            Show();
            MessageBox.Show("识别成功！");
        }




        




        private void Clean_Point_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(ReadZB.FilePath, FileMode.Open, FileAccess.Write);
            System.IO.File.SetAttributes(ReadZB.FilePath, FileAttributes.Hidden);
            StreamWriter sr = new StreamWriter(fs);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
            ReadZB.point.Clear();
            sr.WriteLine(JsonConvert.SerializeObject(new ReadZB.ReadPoint() { point=ReadZB.point,url=ReadZB._URL}));//开始写入值
            sr.Close();
            fs.Close();
            ReadZB.point.Clear();
            textBox1.Text = "";
            label1.Text= "设置坐标个数:" + ReadZB.point.Count;
            MessageBox.Show("已清空！");
        }

        private void timebtn_Click(object sender, EventArgs e)
        {
            //更新定时时间
            if (string.IsNullOrEmpty(Interval) || !Interval.Equals(timetext.Text))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Time"].Value = timetext.Text;
                config.Save(ConfigurationSaveMode.Modified);
                Interval = timetext.Text;
            }
            if (timebtn.Text == "启动")
            {
                //最小化窗口
                WindowState = FormWindowState.Minimized;
                //防止重复执行
                if (backgroundWorker1.IsBusy)
                {
                    return;
                }
                backgroundWorker1.RunWorkerAsync();
                timebtn.Text = "停止";
            }
            else
            {
                //取消执行
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.CancelAsync();
                timebtn.Text = "启动";
            }
            return;
            
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setting.FormClosing += setting_FormClosing;
            setting.MaximizeBox = false;
            setting.MinimizeBox = false;
            setting.Height = 200;
            setting.Width = 250;
            setting.Text = "更多设置";
            
            isStartUp.CheckedChanged += isStartUp_CheckedChanged;
            isStartUp.Checked = bool.Parse(config.AppSettings.Settings["isStartUp"].Value);
            isStartUp.Text = "开机启动";
            isStartUp.Location = new Point(50, 20);
            setting.Controls.Add(isStartUp);


            isStartService.CheckedChanged += isStartService_CheckedChanged;
            isStartService.Checked = bool.Parse(config.AppSettings.Settings["isStartService"].Value);
            isStartService.Text = "启动时开启实时识别";
            isStartService.AutoSize=true;
            isStartService.Location = new Point(50, 60);
            setting.Controls.Add(isStartService);
            setting.Show();
        }

        private void isStartUp_CheckedChanged(object sender, EventArgs e)
        {
            if (isStartUp.Checked)
            {
                config.AppSettings.Settings["isStartUp"].Value = "true";
                
                RootClass.ShortcutAndStartup(true);//开启开机启动
            }
            else
            {
                config.AppSettings.Settings["isStartUp"].Value = "false";
                config.AppSettings.Settings["isStartService"].Value = "false";
                isStartService.Checked = false;
                RootClass.ShortcutAndStartup(false);//关闭开机启动

            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void isStartService_CheckedChanged(object sender, EventArgs e)
        {
            if (isStartService.Checked)
            {
                config.AppSettings.Settings["isStartService"].Value = "true";

            }
            else
            {
                config.AppSettings.Settings["isStartService"].Value = "false";
            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            setting.Visible = false;
            e.Cancel = true;
        }
        
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                // 判断是否执行取消操作
                if (backgroundWorker1.CancellationPending)
                {
                    return;
                }
                Thread.Sleep(int.Parse(Interval));
                changeLabelText(false);
                GC.Collect();
            }
        }
    }

    public class PICResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string data { get; set; }
    }

}
