using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing.Imaging;

namespace MyScreenPrint
{
    public partial class Form1 : Form
    {
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
            label1.Text = Program.point.Count.ToString();
            // 显示所截得的图片
            //UpdateScreen();

            // 获取截图图片的图形设备
            //catchBmpGraphics = Graphics.FromImage(catchBmp);
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            SaveImg();
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
        public void SaveImg()
        {
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
            if (Program.point.Count > 0)
            {
                foreach (string line in Program.point)
                {
                    rectX = Convert.ToInt32(line.Split(',')[0]);
                    rectY = Convert.ToInt32(line.Split(',')[1]);
                    width = Convert.ToInt32(line.Split(',')[2]);
                    height = Convert.ToInt32(line.Split(',')[3]);

                    // 保存图片到图片框
                    Bitmap bmp = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(my, new Rectangle(0, 0, width, height), new Rectangle(rectX, rectY, width, height), GraphicsUnit.Pixel);

                    bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +"\\"+DateTime.Now.ToFileTime().ToString()+".jpg");

                    //stream = BytesToStream(BitmapToBytes(bmp));
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);

                    byte[] buffur = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin); // 此处需要将流重置，不然下面的代码读取不到数据。
                    ms.Read(buffur, 0, Convert.ToInt32(ms.Length));


                    var formDatas = new List<FormItemModel>();
                    formDatas.Add(new FormItemModel()
                    {
                        Key = "file",
                        Value = "",
                        FileName = "pic.Png",
                        FileContent = ms
                    });
                    //string Response = PostWebRequest("http://118.25.1.155:9527/ocr","",new Dictionary<string, string>(),"POST",true,data);
                    string Response= PostForm("http://118.25.1.155:9527/ocr", formDatas);
                    textBox2.Text = Response;
                    bmp.Dispose();
                    ms.Dispose();
                }
            }
            else {
                MessageBox.Show("请设置坐标！","提示");
            }

            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "设置坐标个数：" + Program.point.Count;
            if (Program.point.Count > 0)
            {
                foreach (string line in Program.point)
                {
                    textBox1.Text += line+"\r\n";
                }
            }
        }

        private void Clean_Point_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + @"/Point.txt", FileMode.Open, FileAccess.Write);
            System.IO.File.SetAttributes(Environment.CurrentDirectory + @"/Point.txt", FileAttributes.Hidden);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine("");//开始写入值
            sr.Close();
            fs.Close();
            Program.point.Clear();
            textBox1.Text = "";
            MessageBox.Show("已清空！");
        }

        public string PostWebRequest(string postUrl, string paramData, Dictionary<string, string> headers, string RequestType = "POST", bool isFile = false, byte[] stream = null)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                request.Method = RequestType;
                // 这个可以是改变的，也可以是下面这个固定的字符串
                // 创建request对象
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                request.ContentType = "application/json;charset=utf-8;";
                request.Accept = "application/json";

                //添加消息头
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                Stream newStream = null;
                if (RequestType != "GET")
                {
                    if (isFile)
                    {
                        string boundary = "—————————7d930d1a850658";
                        request.ContentType = "multipart/form-data; boundary=" + boundary;
                        request.ContentLength = stream.Length;
                        newStream = request.GetRequestStream();
                        newStream.Write(stream, 0, stream.Length);
                        newStream.Close();
                    }
                    else
                    {
                        request.ContentLength = byteArray.Length;
                        newStream = request.GetRequestStream();
                        newStream.Write(byteArray, 0, byteArray.Length); //写入参数
                        newStream.Close();
                    }
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                    ret = sr.ReadToEnd();
                }
            }
            return ret;
        }

        public static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {   // 总是接受    
            return true;
        }

        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formItems">Post表单内容</param>
        /// <param name="cookieContainer"></param>
        /// <param name="timeOut">默认20秒</param>
        /// <param name="encoding">响应内容的编码类型（默认utf-8）</param>
        /// <returns></returns>
        public static string PostForm(string url, List<FormItemModel> formItems, CookieContainer cookieContainer = null, string refererUrl = null, Encoding encoding = null, int timeOut = 20000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            #region 初始化请求对象
            request.Method = "POST";
            request.Timeout = timeOut;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            if (!string.IsNullOrEmpty(refererUrl))
                request.Referer = refererUrl;
            if (cookieContainer != null)
                request.CookieContainer = cookieContainer;
            #endregion

            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            //请求流
            var postStream = new MemoryStream();
            #region 处理Form表单请求内容
            //是否用Form上传文件
            var formUploadFile = formItems != null && formItems.Count > 0;
            if (formUploadFile)
            {
                //文件数据模板
                string fileFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                    "\r\nContent-Type: application/octet-stream" +
                    "\r\n\r\n";
                //文本数据模板
                string dataFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                    "\r\n\r\n{1}";
                foreach (var item in formItems)
                {
                    string formdata = null;
                    if (item.IsFile)
                    {
                        //上传文件
                        formdata = string.Format(
                            fileFormdataTemplate,
                            item.Key, //表单键
                            item.FileName);
                    }
                    else
                    {
                        //上传文本
                        formdata = string.Format(
                            dataFormdataTemplate,
                            item.Key,
                            item.Value);
                    }

                    //统一处理
                    byte[] formdataBytes = null;
                    //第一行不需要换行
                    if (postStream.Length == 0)
                        formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                    else
                        formdataBytes = Encoding.UTF8.GetBytes(formdata);
                    postStream.Write(formdataBytes, 0, formdataBytes.Length);

                    //写入文件内容
                    if (item.FileContent != null && item.FileContent.Length > 0)
                    {
                        using (var stream = item.FileContent)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                postStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                //结尾
                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);

            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            #endregion

            request.ContentLength = postStream.Length;

            #region 输入二进制流
            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                ////debug
                //postStream.Seek(0, SeekOrigin.Begin);
                //StreamReader sr = new StreamReader(postStream);
                //var postStr = sr.ReadToEnd();
                postStream.Close();//关闭文件访问
            }
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }

        //图片转byte[]   
        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        /// <summary>  
        /// 将 byte[] 转成 Stream  
        /// </summary>  
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }

    /// <summary>
    /// 表单数据项
    /// </summary>
    public class FormItemModel
    {
        /// <summary>
        /// 表单键，request["key"]
        /// </summary>
        public string Key { set; get; }
        /// <summary>
        /// 表单值,上传文件时忽略，request["key"].value
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0)
                    return false;

                if (FileContent != null && FileContent.Length > 0 && string.IsNullOrEmpty(FileName))
                    throw new Exception("上传文件时 FileName 属性值不能为空");
                return true;
            }
        }
        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// 上传的文件内容
        /// </summary>
        public Stream FileContent { set; get; }
    }
}
