using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyScreenPrint
{
    class Log
    {
        static string FilePath = Environment.CurrentDirectory + @"/Log.txt";
        public static void Save(string text)
        {
            if (!File.Exists(FilePath))//判断文件是否存在
            {
                File.Create(FilePath).Close();
            }
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Write);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine("================================================");//开始写入值
            sr.WriteLine(DateTime.Now.ToString()+":  "+text);//开始写入值
            sr.WriteLine("================================================");//开始写入值
            sr.Close();
            fs.Close();
        }
    }
}
