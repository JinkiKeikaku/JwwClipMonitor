namespace JwwClipMonitor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            toolTip1 = new ToolTip(components);
            label1 = new Label();
            listFormat = new ListBox();
            btnSave = new Button();
            btnConvert = new Button();
            cmbScale = new ComboBox();
            panel2 = new Panel();
            panelDirection = new Panel();
            radioImageToJww = new RadioButton();
            radioJwwToImage = new RadioButton();
            btnSettings = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            panelDirection.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveBorder;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(350, 202);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(153, 34);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 9;
            label1.Text = "縮尺";
            toolTip1.SetToolTip(label1, "Jw_cadへ変換する場合は貼り付けるレイヤグループの縮尺を設定する必要があります。");
            // 
            // listFormat
            // 
            listFormat.FormattingEnabled = true;
            listFormat.ItemHeight = 15;
            listFormat.Location = new Point(5, 33);
            listFormat.Name = "listFormat";
            listFormat.Size = new Size(142, 64);
            listFormat.TabIndex = 5;
            listFormat.SelectedIndexChanged += listFormat_SelectedIndexChanged;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(219, 59);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(60, 23);
            btnSave.TabIndex = 7;
            btnSave.Text = "保存...";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(153, 59);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(60, 23);
            btnConvert.TabIndex = 6;
            btnConvert.Text = "変換";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // cmbScale
            // 
            cmbScale.FormattingEnabled = true;
            cmbScale.Location = new Point(190, 31);
            cmbScale.Name = "cmbScale";
            cmbScale.Size = new Size(86, 23);
            cmbScale.TabIndex = 8;
            // 
            // panel2
            // 
            panel2.Controls.Add(panelDirection);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(cmbScale);
            panel2.Controls.Add(btnConvert);
            panel2.Controls.Add(btnSettings);
            panel2.Controls.Add(btnSave);
            panel2.Controls.Add(listFormat);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 202);
            panel2.Name = "panel2";
            panel2.Size = new Size(350, 100);
            panel2.TabIndex = 1;
            // 
            // panelDirection
            // 
            panelDirection.Controls.Add(radioImageToJww);
            panelDirection.Controls.Add(radioJwwToImage);
            panelDirection.Location = new Point(5, 0);
            panelDirection.Name = "panelDirection";
            panelDirection.Size = new Size(229, 30);
            panelDirection.TabIndex = 11;
            // 
            // radioImageToJww
            // 
            radioImageToJww.AutoSize = true;
            radioImageToJww.Location = new Point(5, 6);
            radioImageToJww.Name = "radioImageToJww";
            radioImageToJww.Size = new Size(105, 19);
            radioImageToJww.TabIndex = 10;
            radioImageToJww.TabStop = true;
            radioImageToJww.Tag = "ImageToJww";
            radioImageToJww.Text = "画像からJw_cad";
            radioImageToJww.UseVisualStyleBackColor = true;
            // 
            // radioJwwToImage
            // 
            radioJwwToImage.AutoSize = true;
            radioJwwToImage.Location = new Point(116, 6);
            radioJwwToImage.Name = "radioJwwToImage";
            radioJwwToImage.Size = new Size(105, 19);
            radioJwwToImage.TabIndex = 10;
            radioJwwToImage.TabStop = true;
            radioJwwToImage.Tag = "JwwToImage";
            radioJwwToImage.Text = "Jw_cadから画像";
            radioJwwToImage.UseVisualStyleBackColor = true;
            // 
            // btnSettings
            // 
            btnSettings.Location = new Point(285, 59);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(60, 23);
            btnSettings.TabIndex = 7;
            btnSettings.Text = "設定...";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(350, 302);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(360, 330);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panelDirection.ResumeLayout(false);
            panelDirection.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private ToolTip toolTip1;
        private ListBox listFormat;
        private Button btnSave;
        private Button btnConvert;
        private ComboBox cmbScale;
        private Label label1;
        private Panel panel2;
        private Button btnSettings;
        private RadioButton radioJwwToImage;
        private RadioButton radioImageToJww;
        private Panel panelDirection;
    }
}
