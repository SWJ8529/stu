using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MyScreenPrint
{
    public class ImageRecognition
    {

        // 矩形起点
        private int rectX;
        private int rectY;
        // 矩形宽高
        private int width;
        private int height;
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszoutput, IntPtr lpdate);
        [DllImport("gdi32.dll")]
        private static extern BootMode BitBlt(IntPtr hdcDest, int x, int y, int widht, int hight, IntPtr hdcsrc, int xsrc, int ysrc, System.Int32 dw);
        public string SaveImg(Boolean flag = false)
        {
            string ret = string.Empty;
            try
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
                        ms.Close();
                        g.Dispose();
                        bmp.Dispose();
                        bmp =null;
                        try
                        {
                            if (!string.IsNullOrEmpty(pir.data) && decimal.Parse(pir.data) != 0)//判断是否为空
                            {
                                ret = JsonConvert.SerializeObject(pir);
                                break;//跳出循环


                            }
                            if (i == ReadZB.point.Count && string.IsNullOrEmpty(pir.data))//如果是最后一个坐标并且还没数据
                            {
                                ret = "{\"msg\":\"没获取到坐标中的数据!\",\"code\":500,\"data\":\"\"}";
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Save(ex.ToString());
                            continue;//继续循环
                        }
                    }
                }
                else
                {
                    ret = "{\"msg\":\"读取坐标失败!\",\"code\":500,\"data\":\"\"}";
                }
                if (string.IsNullOrEmpty(ret))
                {
                    ret = "{\"msg\":\"未读取到数据!\",\"code\":500,\"data\":\"\"}";
                }

                g1.Dispose();
                g2.Dispose();
                my.Dispose();
                my=null;
            }
            catch (Exception ex)
            {
                SystemMemeoryCleanup.ClearMemory();//清理缓存(类似360加速球的效果)
                Log.Save(ex.ToString());
            }
            return ret;
            #endregion
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
            req.Proxy = null;//不使用代理提高执行效率
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
            br.Dispose();
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
    }
}
