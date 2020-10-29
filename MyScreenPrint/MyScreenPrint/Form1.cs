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

        FloatForm floatForm = new FloatForm();//创建悬浮窗

        CheckBox isStartUp = new CheckBox();//创建开机启动按钮

        CheckBox isStartService = new CheckBox();//创建是否开机启动服务按钮

        Form setting = new Form();//更多设置窗体


        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        static String Interval;//定时时间
        System.Timers.Timer t= new System.Timers.Timer();
        // 截图窗口
        Cutter cutter = null;

        // 截得的图片
        public static Bitmap catchBmp = null;

        // 绘图参数
        enum Tools { Pen, Text};
        Graphics catchBmpGraphics = null;  // 图形设备
        Color color = Color.White;  // 选择的颜色

        public Form1()
        {
            InitializeComponent();

            // 双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                           ControlStyles.AllPaintingInWmPaint,
                           true);
            this.UpdateStyles();
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
            // 显示所截得的图片
            //UpdateScreen();

            // 获取截图图片的图形设备
            //catchBmpGraphics = Graphics.FromImage(catchBmp);
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr= MessageBox.Show("这是测试按钮！点击后会根据坐标截取指定区域的图片，文件将生成在桌面上。","提示",MessageBoxButtons.YesNo);
            
            Hide();
            Thread.Sleep(200);
            if (dr == DialogResult.Yes)
            {
                SaveImg(true);
            }
            else
            {
                SaveImg(false);
            }

            Show();
            MessageBox.Show("识别成功！");
        }


        // 矩形起点
        private int rectX;
        private int rectY;
        // 矩形宽高
        private int width;
        private int height;
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszoutput, IntPtr lpdate);
        [DllImport("gdi32.dll")]
        public static extern BootMode BitBlt(IntPtr hdcDest, int x, int y, int widht, int hight, IntPtr hdcsrc, int xsrc, int ysrc, System.Int32 dw);
        public string SaveImg(Boolean flag=false)
        {
            string ret=string.Empty;
            IntPtr dc1 = CreateDC("display", null, null, (IntPtr)null);
            Graphics g1 = Graphics.FromHdc(dc1);
            Bitmap my = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, g1);
            Graphics g2 = Graphics.FromImage(my);
            IntPtr dc3 = g1.GetHdc();
            IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, dc3, 0, 0, 13369376);
            g1.ReleaseHdc(dc3);
            g2.ReleaseHdc(dc2);

            int i = 0;

            #region 获取坐标
            if (ReadZB.point.Count > 0)
            {
                foreach (string line in ReadZB.point)
                {
                    i++;
                    rectX = Convert.ToInt32(line.Split(',')[0]);
                    rectY = Convert.ToInt32(line.Split(',')[1]);
                    width = Convert.ToInt32(line.Split(',')[2]);
                    height = Convert.ToInt32(line.Split(',')[3]);

                    // 保存图片到图片框
                    Bitmap bmp = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(my, new Rectangle(0, 0, width, height), new Rectangle(rectX, rectY, width, height), GraphicsUnit.Pixel);

                    if (flag)
                    {
                        bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToFileTime().ToString() + ".png");                    
                    }
                    

                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);
                    byte[] picdata = ms.GetBuffer();//StreamToBytes(ms);
                    //BytesToImage(picdata);
                    string response = CreatePostData(ReadZB._URL, DateTime.Now.ToFileTime().ToString(), picdata);
                    PICResponse pir = JsonConvert.DeserializeObject<PICResponse>(response);
                    textBox2.Text += //string.IsNullOrEmpty(pir.data) ?"该坐标无法识别出数字:"+ line+"   ": 
                        DateTime.Now.ToString()+" : "+response + "\r\n";
                    ms.Close();
                    try
                    {
                        if (!string.IsNullOrEmpty(pir.data) && decimal.Parse(pir.data) != 0)//判断是否为空
                        {
                        
                                //尝试将内容转为数字
                                //if (decimal.Parse(pir.data) == 0) { continue; }
                                ret = JsonConvert.SerializeObject(pir);
                                floatForm.FloatLabel.Text = "金额:" + pir.data;
                                break;//跳出循环
                        

                        }
                        if (i== ReadZB.point.Count && string.IsNullOrEmpty(pir.data))//如果是最后一个坐标并且还没数据
                        {
                            floatForm.FloatLabel.Text = "金额:0";
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;//继续循环
                    }
                }
                textBox2.Text += "\r\n";
            }
            else {
                t.Stop();
                timebtn.Text = "启动";
                MessageBox.Show("请设置坐标！","提示");
                floatForm.FloatLabel.Text = "金额:0";
                ret = "{\"msg\":\"读取坐标失败!\",\"code\":500,\"data\":\"\"}";
            }
            if (string.IsNullOrEmpty(ret))
            {
                ret = "{\"msg\":\"未读取到数据!\",\"code\":500,\"data\":\"\"}";
            }
            return ret;
            #endregion
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
            label1.Text = "设置坐标个数:" + ReadZB.point.Count;
            Interval = ConfigurationManager.AppSettings["Time"];
            timetext.Text = Interval;
            if (ReadZB.point.Count > 0)
            {
                foreach (string line in ReadZB.point)
                {
                    textBox1.Text += line+"\r\n";
                }
            }


            
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）

            floatForm.Show();

            bool isStartService = bool.Parse(config.AppSettings.Settings["isStartService"].Value);
            if (isStartService)
            {
                //最小化窗口
                WindowState = FormWindowState.Minimized;
                Thread.Sleep(200);
                t.Enabled = true; //是否触发Elapsed事件
                t.Start();
                timebtn.Text = "停止";
            }
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

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        public string CreatePostData(string url, string filename, byte[] data)
        {

            Stream fileStream = new MemoryStream(data);

            BinaryReader br = new BinaryReader(fileStream);

            byte[] buffer = br.ReadBytes(Convert.ToInt32(fileStream.Length));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            //请求
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            //组织表单数据
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + filename + "\";");
            sb.Append("\r\n");
            sb.Append("Content-Type: image/png");
            sb.Append("\r\n\r\n");
            string head = sb.ToString();
            byte[] form_data = Encoding.UTF8.GetBytes(head);
            //结尾
            byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            //post总长度
            long length = form_data.Length + fileStream.Length + foot_data.Length;
            req.ContentLength = length;
            Stream requestStream = req.GetRequestStream();
            //这里要注意一下发送顺序，先发送form_data > buffer > foot_data
            //发送表单参数
            requestStream.Write(form_data, 0, form_data.Length);
            //发送文件内容
            requestStream.Write(buffer, 0, buffer.Length);
            //结尾
            requestStream.Write(foot_data, 0, foot_data.Length);
            requestStream.Close();
            fileStream.Close();
            fileStream.Dispose();
            br.Close();
            // br.Dispose();
            //响应
            WebResponse pos = req.GetResponse();
            StreamReader sr = new StreamReader(pos.GetResponseStream(), Encoding.UTF8);
            string html = sr.ReadToEnd().Trim();
            sr.Close();
            sr.Dispose();
            if (pos != null)
            {
                pos.Close();
                pos = null;
            }
            if (req != null)
            {
                req = null;
            }
            return html;
        }
        private void timebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Interval) || !Interval.Equals(timetext.Text))
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Time"].Value = timetext.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    Interval = timetext.Text;
                }
                t.Interval = Convert.ToDouble(Interval);
                if (timebtn.Text == "启动")
                {
                    //最小化窗口
                    WindowState = FormWindowState.Minimized;
                    Thread.Sleep(200);
                    t.Enabled = true; //是否触发Elapsed事件
                    t.Start();
                    timebtn.Text = "停止";
                }
                else
                {
                    t.Stop();
                    timebtn.Text = "启动";
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),"警告！");
            }
            
        }

        private void Timer_TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            //到达指定时间截取屏幕指定区域并且识别内容
                      
            SaveImg();
        }

        private void cleanlog_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
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
            setting.Width = 230;
            setting.Text = "更多设置";
            
            isStartUp.CheckedChanged += isStartUp_CheckedChanged;
            isStartUp.Checked = bool.Parse(config.AppSettings.Settings["isStartUp"].Value);
            isStartUp.Text = "开机启动";
            isStartUp.Location = new Point(50, 20);
            setting.Controls.Add(isStartUp);


            isStartService.CheckedChanged += isStartService_CheckedChanged;
            isStartService.Checked = bool.Parse(config.AppSettings.Settings["isStartService"].Value);
            isStartService.Text = "开机启动实时识别";
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
