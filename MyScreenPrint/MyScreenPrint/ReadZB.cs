using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyScreenPrint
{
    public class ReadZB
    {
        public static List<string> point = new List<string>();
        public static string FilePath = "";
        public static string _URL = "";
        public string test;
        public void readpoint()
        {
            FilePath = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf('\\')) + @"/Point.txt";
            #region 读取文件坐标
            if (!File.Exists(FilePath))//判断文件是否存在
            {
                File.Create(FilePath).Close();
            }
            //Thread.Sleep(200);
            var lines = File.ReadAllLines(FilePath);

            string PointJson = string.Empty;

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    PointJson += line;
                }
            }
            ReadPoint rp = JsonConvert.DeserializeObject<ReadPoint>(PointJson);
            if(rp != null)
            {
                point = rp.point;
                if(rp.url!= ConfigurationManager.AppSettings["URL"])
                {
                    rp.url = ConfigurationManager.AppSettings["URL"];
                }
                _URL = rp.url;
            }
            else
            {
                FileStream fs = new FileStream(ReadZB.FilePath, FileMode.Open, FileAccess.Write);
                System.IO.File.SetAttributes(ReadZB.FilePath, FileAttributes.Hidden);
                StreamWriter sr = new StreamWriter(fs);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                ReadZB.point.Clear();
                sr.WriteLine(JsonConvert.SerializeObject(new ReadZB.ReadPoint() { point = ReadZB.point, url = ConfigurationManager.AppSettings["URL"] }));//开始写入值
                sr.Close();
                fs.Close();
                _URL = ConfigurationManager.AppSettings["URL"];
            }
            
            #endregion
        }
        public class ReadPoint
        {
            /// <summary>
            /// 
            /// </summary>
            public List<string> point { get; set; }
            public string url { get; set; }
        }
    }
}
