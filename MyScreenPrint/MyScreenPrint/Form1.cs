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
        Form FloatForm = new Form();//创建悬浮窗
        Label FloatLabel = new Label();//创建悬浮窗上的文本
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
            
            label1.Text = "设置坐标个数：" + ReadZB.point.Count;
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


            #region 获取坐标
            if (ReadZB.point.Count > 0)
            {
                foreach (string line in ReadZB.point)
                {
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
                    textBox2.Text += //string.IsNullOrEmpty(pir.data) ?"该坐标无法识别出数字："+ line+"   ": 
                        DateTime.Now.ToString()+" : "+response + "\r\n";
                    ms.Close();
                    if (!string.IsNullOrEmpty(pir.data))//判断是否为空
                    {
                        try
                        {
                            decimal.Parse(pir.data);//尝试将内容转为数字
                            ret = JsonConvert.SerializeObject(pir);
                            FloatLabel.Text = "金额：" + pir.data;
                            break;//跳出循环
                        }
                        catch (Exception ex)
                        {
                            continue;//继续循环
                        }

                    }
                    else
                    {
                        FloatLabel.Text = "金额：0";
                    }
                }
                textBox2.Text += "\r\n";
            }
            else {
                MessageBox.Show("请设置坐标！","提示");
                FloatLabel.Text = "金额：0";
                ret = "{\"msg\":\"读取坐标失败!\",\"code\":500,\"data\":\"\"}";
            }
            return ret;
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            label1.Text = "设置坐标个数：" + ReadZB.point.Count;
            Interval = ConfigurationManager.AppSettings["Time"];
            timetext.Text = Interval;
            if (ReadZB.point.Count > 0)
            {
                foreach (string line in ReadZB.point)
                {
                    textBox1.Text += line+"\r\n";
                }
            }


            m_aeroEnabled = false;
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            FloatForm.FormBorderStyle = FormBorderStyle.None;
            FloatForm.TopMost = true;//设置窗口永远为屏幕前面
            FloatForm.ShowInTaskbar = false;//不在任务栏中显示以免误关
            FloatForm.Width=80;
            FloatForm.Height = 30;
            FloatForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            FloatForm.MaximizeBox = false;
            FloatForm.MinimizeBox = false;
            int x = (SystemInformation.WorkingArea.Width - FloatForm.Size.Width) / 2;
            int y = (SystemInformation.WorkingArea.Height - FloatForm.Size.Height) / 2;
            FloatForm.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            FloatForm.Location = (Point)new Size(x, 0);         //窗体的起始位置为(x,y)
            
            FloatLabel.Location = new Point(0,10);
            FloatLabel.AutoSize = true;
            FloatLabel.Width = 78;
            FloatLabel.Height = 28;
            FloatLabel.Font= new Font(this.Font.FontFamily, 18);

            FloatLabel.Text = "金额：0";
            FloatForm.Controls.Add(FloatLabel);
            FloatForm.Show();
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
            label1.Text= "设置坐标个数：" + ReadZB.point.Count;
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

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
    (
        int nLeftRect, // x-coordinate of upper-left corner
        int nTopRect, // y-coordinate of upper-left corner
        int nRightRect, // x-coordinate of lower-right corner
        int nBottomRect, // y-coordinate of lower-right corner
        int nWidthEllipse, // height of ellipse
        int nHeightEllipse // width of ellipse
     );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

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
