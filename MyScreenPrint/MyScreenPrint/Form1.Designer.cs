namespace MyScreenPrint
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.operationPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.timebtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.timetext = new System.Windows.Forms.TextBox();
            this.Clean_Point = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.printScrBtn = new System.Windows.Forms.Button();
            this.splitter_1 = new System.Windows.Forms.Label();
            this.picturePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operationPanel.SuspendLayout();
            this.picturePanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // operationPanel
            // 
            this.operationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.operationPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.operationPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.operationPanel.Controls.Add(this.button1);
            this.operationPanel.Controls.Add(this.timebtn);
            this.operationPanel.Controls.Add(this.label4);
            this.operationPanel.Controls.Add(this.timetext);
            this.operationPanel.Controls.Add(this.Clean_Point);
            this.operationPanel.Controls.Add(this.label1);
            this.operationPanel.Controls.Add(this.button2);
            this.operationPanel.Controls.Add(this.printScrBtn);
            this.operationPanel.Controls.Add(this.splitter_1);
            this.operationPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.operationPanel.Location = new System.Drawing.Point(443, 12);
            this.operationPanel.Name = "operationPanel";
            this.operationPanel.Size = new System.Drawing.Size(113, 316);
            this.operationPanel.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(2, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 48);
            this.button1.TabIndex = 10;
            this.button1.Text = "更多设置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timebtn
            // 
            this.timebtn.Location = new System.Drawing.Point(2, 276);
            this.timebtn.Name = "timebtn";
            this.timebtn.Size = new System.Drawing.Size(75, 23);
            this.timebtn.TabIndex = 9;
            this.timebtn.Text = "启动";
            this.timebtn.UseVisualStyleBackColor = true;
            this.timebtn.Click += new System.EventHandler(this.timebtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(75, 252);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "毫秒";
            // 
            // timetext
            // 
            this.timetext.Location = new System.Drawing.Point(2, 249);
            this.timetext.Name = "timetext";
            this.timetext.Size = new System.Drawing.Size(67, 21);
            this.timetext.TabIndex = 7;
            // 
            // Clean_Point
            // 
            this.Clean_Point.Location = new System.Drawing.Point(2, 57);
            this.Clean_Point.Name = "Clean_Point";
            this.Clean_Point.Size = new System.Drawing.Size(102, 48);
            this.Clean_Point.TabIndex = 6;
            this.Clean_Point.Text = "清空坐标";
            this.Clean_Point.UseVisualStyleBackColor = true;
            this.Clean_Point.Click += new System.EventHandler(this.Clean_Point_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 234);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(2, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 48);
            this.button2.TabIndex = 4;
            this.button2.Text = "识别测试";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // printScrBtn
            // 
            this.printScrBtn.Location = new System.Drawing.Point(2, 3);
            this.printScrBtn.Name = "printScrBtn";
            this.printScrBtn.Size = new System.Drawing.Size(102, 48);
            this.printScrBtn.TabIndex = 3;
            this.printScrBtn.Text = "设置坐标";
            this.printScrBtn.UseVisualStyleBackColor = true;
            this.printScrBtn.Click += new System.EventHandler(this.printScrBtn_Click);
            // 
            // splitter_1
            // 
            this.splitter_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter_1.Location = new System.Drawing.Point(4, 112);
            this.splitter_1.Name = "splitter_1";
            this.splitter_1.Size = new System.Drawing.Size(100, 1);
            this.splitter_1.TabIndex = 2;
            // 
            // picturePanel
            // 
            this.picturePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picturePanel.AutoScroll = true;
            this.picturePanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picturePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picturePanel.Controls.Add(this.label2);
            this.picturePanel.Controls.Add(this.textBox1);
            this.picturePanel.Location = new System.Drawing.Point(13, 12);
            this.picturePanel.Name = "picturePanel";
            this.picturePanel.Size = new System.Drawing.Size(424, 316);
            this.picturePanel.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "坐标日志:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(3, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(416, 279);
            this.textBox1.TabIndex = 0;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "图片数字识别工具";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // 显示ToolStripMenuItem
            // 
            this.显示ToolStripMenuItem.Name = "显示ToolStripMenuItem";
            this.显示ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.显示ToolStripMenuItem.Text = "显示";
            this.显示ToolStripMenuItem.Click += new System.EventHandler(this.显示ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 340);
            this.Controls.Add(this.picturePanel);
            this.Controls.Add(this.operationPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "图片数字识别工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.operationPanel.ResumeLayout(false);
            this.operationPanel.PerformLayout();
            this.picturePanel.ResumeLayout(false);
            this.picturePanel.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel operationPanel;
        private System.Windows.Forms.Panel picturePanel;
        private System.Windows.Forms.Label splitter_1;
        private System.Windows.Forms.Button printScrBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Clean_Point;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox timetext;
        private System.Windows.Forms.Button timebtn;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

