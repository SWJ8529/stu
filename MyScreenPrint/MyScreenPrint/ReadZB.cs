using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyScreenPrint
{
    public class ReadZB
    {
        public static List<string> point = new List<string>();

        public void readpoint()
        {
            #region 读取文件坐标
            var lines = File.ReadAllLines(Environment.CurrentDirectory + @"/Point.txt");

            string PointJson = "{\"point\":[";

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    PointJson += line;
                }
            }

            PointJson += "]}";
            ReadPoint rp = JsonConvert.DeserializeObject<ReadPoint>(PointJson);
            point = rp.point;
            #endregion
        }
        public class ReadPoint
        {
            /// <summary>
            /// 
            /// </summary>
            public List<string> point { get; set; }
        }
    }
}
