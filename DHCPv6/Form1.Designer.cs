namespace DHCPv6
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.ckbPwd = new System.Windows.Forms.CheckBox();
            this.ckbMini = new System.Windows.Forms.CheckBox();
            this.ckbStart = new System.Windows.Forms.CheckBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.panelOn = new System.Windows.Forms.Panel();
            this.rbeID = new System.Windows.Forms.RadioButton();
            this.rbNID = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbError = new System.Windows.Forms.Label();
            this.picBtn = new System.Windows.Forms.PictureBox();
            this.loadingCircle1 = new DHCPv6.LoadingCircle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbInMID = new System.Windows.Forms.RadioButton();
            this.rbInNID = new System.Windows.Forms.RadioButton();
            this.rbInSID = new System.Windows.Forms.RadioButton();
            this.panelOff = new System.Windows.Forms.Panel();
            this.picCutbtn = new System.Windows.Forms.PictureBox();
            this.loadingCircle2 = new DHCPv6.LoadingCircle();
            this.lbUser = new System.Windows.Forms.Label();
            this.lbNetworkTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbTime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.picClose = new System.Windows.Forms.PictureBox();
            this.picMin = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelOn.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBtn)).BeginInit();
            this.panel3.SuspendLayout();
            this.panelOff.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCutbtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMin)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "登陆名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(9, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "密    码：";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(72, 17);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(203, 21);
            this.txtUser.TabIndex = 4;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(72, 58);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(203, 21);
            this.txtPwd.TabIndex = 5;
            // 
            // ckbPwd
            // 
            this.ckbPwd.AutoSize = true;
            this.ckbPwd.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbPwd.Location = new System.Drawing.Point(15, 86);
            this.ckbPwd.Name = "ckbPwd";
            this.ckbPwd.Size = new System.Drawing.Size(75, 21);
            this.ckbPwd.TabIndex = 6;
            this.ckbPwd.Text = "记住密码";
            this.ckbPwd.UseVisualStyleBackColor = true;
            this.ckbPwd.CheckedChanged += new System.EventHandler(this.ckbPwd_CheckedChanged);
            // 
            // ckbMini
            // 
            this.ckbMini.AutoSize = true;
            this.ckbMini.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbMini.Location = new System.Drawing.Point(98, 86);
            this.ckbMini.Name = "ckbMini";
            this.ckbMini.Size = new System.Drawing.Size(87, 21);
            this.ckbMini.TabIndex = 7;
            this.ckbMini.Text = "登录最小化";
            this.ckbMini.UseVisualStyleBackColor = true;
            this.ckbMini.CheckedChanged += new System.EventHandler(this.ckbMini_CheckedChanged);
            // 
            // ckbStart
            // 
            this.ckbStart.AutoSize = true;
            this.ckbStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ckbStart.Location = new System.Drawing.Point(200, 86);
            this.ckbStart.Name = "ckbStart";
            this.ckbStart.Size = new System.Drawing.Size(75, 21);
            this.ckbStart.TabIndex = 8;
            this.ckbStart.Text = "自动启动";
            this.ckbStart.UseVisualStyleBackColor = true;
            this.ckbStart.CheckedChanged += new System.EventHandler(this.ckbStart_CheckedChanged);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "网络连接客户端";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // panelOn
            // 
            this.panelOn.Controls.Add(this.rbeID);
            this.panelOn.Controls.Add(this.rbNID);
            this.panelOn.Controls.Add(this.tableLayoutPanel1);
            this.panelOn.Controls.Add(this.txtPwd);
            this.panelOn.Controls.Add(this.picBtn);
            this.panelOn.Controls.Add(this.loadingCircle1);
            this.panelOn.Controls.Add(this.txtUser);
            this.panelOn.Controls.Add(this.ckbStart);
            this.panelOn.Controls.Add(this.ckbMini);
            this.panelOn.Controls.Add(this.label1);
            this.panelOn.Controls.Add(this.ckbPwd);
            this.panelOn.Controls.Add(this.label2);
            this.panelOn.Controls.Add(this.panel3);
            this.panelOn.Location = new System.Drawing.Point(122, 63);
            this.panelOn.Name = "panelOn";
            this.panelOn.Size = new System.Drawing.Size(286, 220);
            this.panelOn.TabIndex = 9;
            // 
            // rbeID
            // 
            this.rbeID.AutoSize = true;
            this.rbeID.Location = new System.Drawing.Point(155, 126);
            this.rbeID.Name = "rbeID";
            this.rbeID.Size = new System.Drawing.Size(65, 16);
            this.rbeID.TabIndex = 17;
            this.rbeID.Text = "eID登录";
            this.rbeID.UseVisualStyleBackColor = true;
            this.rbeID.CheckedChanged += new System.EventHandler(this.rbeID_CheckedChanged);
            // 
            // rbNID
            // 
            this.rbNID.AutoSize = true;
            this.rbNID.Checked = true;
            this.rbNID.Location = new System.Drawing.Point(72, 126);
            this.rbNID.Name = "rbNID";
            this.rbNID.Size = new System.Drawing.Size(71, 16);
            this.rbNID.TabIndex = 17;
            this.rbNID.TabStop = true;
            this.rbNID.Text = "直接登录";
            this.rbNID.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lbError, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(46, 143);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 25);
            this.tableLayoutPanel1.TabIndex = 16;
            // 
            // lbError
            // 
            this.lbError.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbError.AutoSize = true;
            this.lbError.ForeColor = System.Drawing.Color.Red;
            this.lbError.Location = new System.Drawing.Point(82, 6);
            this.lbError.Name = "lbError";
            this.lbError.Size = new System.Drawing.Size(35, 12);
            this.lbError.TabIndex = 10;
            this.lbError.Text = "error";
            this.lbError.Visible = false;
            this.lbError.Click += new System.EventHandler(this.lbError_Click);
            // 
            // picBtn
            // 
            this.picBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picBtn.Image = global::DHCPv6.Properties.Resources.log;
            this.picBtn.Location = new System.Drawing.Point(13, 168);
            this.picBtn.Name = "picBtn";
            this.picBtn.Size = new System.Drawing.Size(262, 32);
            this.picBtn.TabIndex = 15;
            this.picBtn.TabStop = false;
            this.picBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle1.InnerCircleRadius = 8;
            this.loadingCircle1.Location = new System.Drawing.Point(125, 190);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 12;
            this.loadingCircle1.OuterCircleRadius = 11;
            this.loadingCircle1.RotationSpeed = 100;
            this.loadingCircle1.Size = new System.Drawing.Size(43, 33);
            this.loadingCircle1.SpokeThickness = 6;
            this.loadingCircle1.StylePreset = DHCPv6.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle1.TabIndex = 9;
            this.loadingCircle1.Text = "loadingCircle1";
            this.loadingCircle1.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbInMID);
            this.panel3.Controls.Add(this.rbInNID);
            this.panel3.Controls.Add(this.rbInSID);
            this.panel3.Location = new System.Drawing.Point(25, 99);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(250, 26);
            this.panel3.TabIndex = 15;
            // 
            // rbInMID
            // 
            this.rbInMID.AutoSize = true;
            this.rbInMID.Location = new System.Drawing.Point(157, 8);
            this.rbInMID.Name = "rbInMID";
            this.rbInMID.Size = new System.Drawing.Size(83, 16);
            this.rbInMID.TabIndex = 18;
            this.rbInMID.Text = "手机号登录";
            this.rbInMID.UseVisualStyleBackColor = true;
            this.rbInMID.CheckedChanged += new System.EventHandler(this.rbInMID_CheckedChanged);
            // 
            // rbInNID
            // 
            this.rbInNID.AutoSize = true;
            this.rbInNID.Checked = true;
            this.rbInNID.Location = new System.Drawing.Point(3, 8);
            this.rbInNID.Name = "rbInNID";
            this.rbInNID.Size = new System.Drawing.Size(65, 16);
            this.rbInNID.TabIndex = 18;
            this.rbInNID.TabStop = true;
            this.rbInNID.Text = "NID登录";
            this.rbInNID.UseVisualStyleBackColor = true;
            this.rbInNID.CheckedChanged += new System.EventHandler(this.rbInNID_CheckedChanged);
            // 
            // rbInSID
            // 
            this.rbInSID.AutoSize = true;
            this.rbInSID.Location = new System.Drawing.Point(80, 8);
            this.rbInSID.Name = "rbInSID";
            this.rbInSID.Size = new System.Drawing.Size(71, 16);
            this.rbInSID.TabIndex = 18;
            this.rbInSID.Text = "学号登录";
            this.rbInSID.UseVisualStyleBackColor = true;
            this.rbInSID.CheckedChanged += new System.EventHandler(this.rbInSID_CheckedChanged);
            // 
            // panelOff
            // 
            this.panelOff.Controls.Add(this.picCutbtn);
            this.panelOff.Controls.Add(this.loadingCircle2);
            this.panelOff.Controls.Add(this.lbUser);
            this.panelOff.Controls.Add(this.lbNetworkTime);
            this.panelOff.Controls.Add(this.label7);
            this.panelOff.Controls.Add(this.label6);
            this.panelOff.Controls.Add(this.label3);
            this.panelOff.Location = new System.Drawing.Point(119, 75);
            this.panelOff.Name = "panelOff";
            this.panelOff.Size = new System.Drawing.Size(297, 182);
            this.panelOff.TabIndex = 10;
            // 
            // picCutbtn
            // 
            this.picCutbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCutbtn.Image = global::DHCPv6.Properties.Resources.cutBtn;
            this.picCutbtn.Location = new System.Drawing.Point(19, 134);
            this.picCutbtn.Name = "picCutbtn";
            this.picCutbtn.Size = new System.Drawing.Size(262, 32);
            this.picCutbtn.TabIndex = 16;
            this.picCutbtn.TabStop = false;
            this.picCutbtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // loadingCircle2
            // 
            this.loadingCircle2.Active = false;
            this.loadingCircle2.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle2.InnerCircleRadius = 8;
            this.loadingCircle2.Location = new System.Drawing.Point(123, 153);
            this.loadingCircle2.Name = "loadingCircle2";
            this.loadingCircle2.NumberSpoke = 12;
            this.loadingCircle2.OuterCircleRadius = 11;
            this.loadingCircle2.RotationSpeed = 100;
            this.loadingCircle2.Size = new System.Drawing.Size(43, 33);
            this.loadingCircle2.SpokeThickness = 6;
            this.loadingCircle2.StylePreset = DHCPv6.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle2.TabIndex = 15;
            this.loadingCircle2.Text = "loadingCircle2";
            this.loadingCircle2.Visible = false;
            // 
            // lbUser
            // 
            this.lbUser.AutoSize = true;
            this.lbUser.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbUser.ForeColor = System.Drawing.Color.Red;
            this.lbUser.Location = new System.Drawing.Point(96, 7);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(100, 21);
            this.lbUser.TabIndex = 17;
            this.lbUser.Text = "0000000000";
            // 
            // lbNetworkTime
            // 
            this.lbNetworkTime.AutoSize = true;
            this.lbNetworkTime.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNetworkTime.ForeColor = System.Drawing.Color.Red;
            this.lbNetworkTime.Location = new System.Drawing.Point(153, 91);
            this.lbNetworkTime.Name = "lbNetworkTime";
            this.lbNetworkTime.Size = new System.Drawing.Size(88, 25);
            this.lbNetworkTime.TabIndex = 15;
            this.lbNetworkTime.Text = "00:00:00";
            this.lbNetworkTime.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(19, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 21);
            this.label7.TabIndex = 14;
            this.label7.Text = "您本次上网时间：";
            this.label7.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(19, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 21);
            this.label6.TabIndex = 13;
            this.label6.Text = "欢迎使用教育网IPv6网络";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(19, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 22);
            this.label3.TabIndex = 11;
            this.label3.Text = "登录成功";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 101);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 126);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(111)))), ((int)(((byte)(180)))));
            this.panel2.BackgroundImage = global::DHCPv6.Properties.Resources.topbg;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.lbTime);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 288);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(425, 28);
            this.panel2.TabIndex = 13;
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.BackColor = System.Drawing.Color.Transparent;
            this.lbTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTime.ForeColor = System.Drawing.Color.White;
            this.lbTime.Location = new System.Drawing.Point(294, 7);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(126, 17);
            this.lbTime.TabIndex = 1;
            this.lbTime.Text = "2015-12-12 12:12:12";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(15, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "版本：version 1.0.8";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(107)))), ((int)(((byte)(177)))));
            this.panel1.BackgroundImage = global::DHCPv6.Properties.Resources.bottombg;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.picClose);
            this.panel1.Controls.Add(this.picMin);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 41);
            this.panel1.TabIndex = 11;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // picClose
            // 
            this.picClose.BackColor = System.Drawing.Color.Transparent;
            this.picClose.Image = ((System.Drawing.Image)(resources.GetObject("picClose.Image")));
            this.picClose.Location = new System.Drawing.Point(392, 11);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(27, 27);
            this.picClose.TabIndex = 14;
            this.picClose.TabStop = false;
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            // 
            // picMin
            // 
            this.picMin.BackColor = System.Drawing.Color.Transparent;
            this.picMin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picMin.Image = ((System.Drawing.Image)(resources.GetObject("picMin.Image")));
            this.picMin.Location = new System.Drawing.Point(359, 11);
            this.picMin.Name = "picMin";
            this.picMin.Size = new System.Drawing.Size(27, 25);
            this.picMin.TabIndex = 1;
            this.picMin.TabStop = false;
            this.picMin.Click += new System.EventHandler(this.picMin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 22);
            this.label4.TabIndex = 0;
            this.label4.Text = "网络连接客户端";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 316);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelOn);
            this.Controls.Add(this.panelOff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络连接客户端";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panelOn.ResumeLayout(false);
            this.panelOn.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBtn)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panelOff.ResumeLayout(false);
            this.panelOff.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCutbtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.CheckBox ckbPwd;
        private System.Windows.Forms.CheckBox ckbMini;
        private System.Windows.Forms.CheckBox ckbStart;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Panel panelOn;
        private System.Windows.Forms.Panel panelOff;
        private System.Windows.Forms.Label label3;
        private LoadingCircle loadingCircle1;
        private System.Windows.Forms.Label lbError;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picClose;
        private System.Windows.Forms.PictureBox picMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbNetworkTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox picBtn;
        private System.Windows.Forms.PictureBox picCutbtn;
        private System.Windows.Forms.Label lbUser;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton rbeID;
        private System.Windows.Forms.RadioButton rbNID;
        private System.Windows.Forms.RadioButton rbInMID;
        private System.Windows.Forms.RadioButton rbInSID;
        private System.Windows.Forms.RadioButton rbInNID;
        private System.Windows.Forms.Panel panel3;
        private LoadingCircle loadingCircle2;
    }
}

