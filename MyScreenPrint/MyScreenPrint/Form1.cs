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
        static string url;
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

            textBox1.Text = cutter.sb.ToString();
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
                    string response = CreatePostData(url, DateTime.Now.ToFileTime().ToString(), picdata);
                    PICResponse pir = JsonConvert.DeserializeObject<PICResponse>(response);
                    textBox2.Text += //string.IsNullOrEmpty(pir.data) ?"该坐标无法识别出数字："+ line+"   ": 
                        DateTime.Now.ToString()+" : "+response + "\r\n";
                    ms.Close();
                    if (!string.IsNullOrEmpty(pir.data))
                    {
                        ret = JsonConvert.SerializeObject(pir);
                    }
                }
                textBox2.Text += "\r\n";
            }
            else {
                MessageBox.Show("请设置坐标！","提示");
                ret= "{\"msg\":\"读取坐标失败!\",\"code\":500,\"data\":\"\"}";
            }
            return ret;
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            url = ConfigurationManager.AppSettings["URL"];
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
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
        }

        private void Clean_Point_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + @"/Point.txt", FileMode.Open, FileAccess.Write);
            System.IO.File.SetAttributes(Environment.CurrentDirectory + @"/Point.txt", FileAttributes.Hidden);
            StreamWriter sr = new StreamWriter(fs);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
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
        ///// <summary>
        ///// 图片识别请求
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //private string PICRequest(byte[] data,string filename) {
        //    using (var client = new HttpClient())
        //    using (var content = new MultipartFormDataContent())
        //    {
        //        client.BaseAddress = new Uri("http://118.25.1.155:9527/ocr");
        //        var filecontent1 = new ByteArrayContent(data);
        //        filecontent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        //        {
        //            Name = "\"file\"",
        //            FileName = filename+".png"
        //        };
        //        filecontent1.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        //        //content.headers.add("content-tpye","image/png");
        //        content.Add(filecontent1);
        //        //content.add(datacontent);
        //        var result = client.PostAsync("", content).Result;
        //        return result.Content.ReadAsStringAsync().Result;
        //    }
        //}
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
