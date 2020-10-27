using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace MyScreenPrint
{
    class Cutter : Form
    {
        static List<string> list = ReadZB.point;
        ReadZB.ReadPoint rp = new ReadZB.ReadPoint();
        // 是否开始截图
        private bool isCatchStart = false;
        
        // 截图起点
        private Point startPoint;
        // 矩形起点
        private int rectX;
        private int rectY;
        // 矩形宽高
        private int width;
        private int height;

        // 确认按钮
        private Button OK_btn = null;
        //取消按钮
        private Button No_btn = null;

        // 截图窗口构造
        public Cutter() : base()
        {
            InitializeComponent();

            // 最大化截图窗口并隐藏边框
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;

            // 双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                           ControlStyles.AllPaintingInWmPaint,
                           true);
            this.UpdateStyles();

            // 鼠标样式
            Cursor = Cursors.Cross;
        }

        // 控件初始化
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Cutter
            // 
            this.ClientSize = new System.Drawing.Size(837, 513);
            this.Name = "Cutter";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Cutter_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Cutter_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Cutter_MouseUp);
            this.ResumeLayout(false);

        }

        #region 鼠标事件
        // 鼠标按下
        private void Cutter_MouseDown(object sender, MouseEventArgs e)
         {
            // 如果按下左键且还没开始开始截图，记录截图起点
            if(e.Button == MouseButtons.Left)
            {
                if(!isCatchStart)
                {
                    isCatchStart = true;

                    startPoint = new Point(e.X, e.Y);
                }
            }
        }

        
        // 鼠标移动
        private void Cutter_MouseMove(object sender, MouseEventArgs e)
        {
            if(isCatchStart)
            {
                
                // 初始化矩形区域
                rectX = Math.Min(startPoint.X, e.X);
                rectY = Math.Min(startPoint.Y, e.Y);
                width = Math.Abs(e.X - startPoint.X);
                height = Math.Abs(e.Y - startPoint.Y);
                
               Rectangle rect = new Rectangle(rectX, rectY, width, height);
               Pen pen = new Pen(Color.Red, 1);

               Invalidate();
               Update();
               Graphics g = this.CreateGraphics();
               g.DrawRectangle(pen, rect);

            }
        }
        // 鼠标松开
        private void Cutter_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                isCatchStart = false;

                Update();

                //防错判断如果长宽为0则默认为1
                if (width==0)
                {
                    width = 1;
                }
                if (height == 0)
                {
                    height = 1;
                }
                // 保存图片到图片框
                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(BackgroundImage, new Rectangle(0, 0, width, height), new Rectangle(rectX, rectY, width, height), GraphicsUnit.Pixel);




                // 确定按钮
                OK_btn = new Button();
                OK_btn.Location = new Point(e.X, e.Y);
                OK_btn.Size = new Size(100, 50);
                OK_btn.Text = "确认！";
                OK_btn.Click += (sende, ee) => DialogResult = DialogResult.OK;
                OK_btn.MouseClick+= OK_btn_Click;
                Controls.Add(OK_btn);

                // 取消按钮
                No_btn = new Button();
                No_btn.Location = new Point(e.X, e.Y-50);
                No_btn.Size = new Size(100, 50);
                No_btn.Text = "取消！";
                No_btn.Click += (sende, ee) => DialogResult = DialogResult.OK;
                Controls.Add(No_btn);
                Update();
                // 绘制矩形区域
                Rectangle rect = new Rectangle(rectX, rectY, width, height);
                Pen pen = new Pen(Color.Red, 5);
                g = this.CreateGraphics();
                g.DrawRectangle(pen, rect);                       
            }
        }

        private void OK_btn_Click(object sender, System.EventArgs e)
        {
            list.Add(rectX + "," + rectY + "," + width + "," + height);

            rp.point = list;
            rp.url = ReadZB._URL;

            FileStream fs = new FileStream(ReadZB.FilePath, FileMode.Open, FileAccess.Write);
            System.IO.File.SetAttributes(ReadZB.FilePath, FileAttributes.Hidden);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine(JsonConvert.SerializeObject(rp));//开始写入值
            sr.Close();
            fs.Close();
            ReadZB.point = list;
        }


        #endregion


    }
}
