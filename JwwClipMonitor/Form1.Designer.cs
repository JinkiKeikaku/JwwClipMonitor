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
            panel2 = new Panel();
            panel1 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            label1 = new Label();
            cmbScale = new ComboBox();
            btnToJww = new Button();
            btnSave = new Button();
            listPaste = new ListBox();
            tabPage2 = new TabPage();
            btnConvert = new Button();
            listCopy = new ListBox();
            pictureBox1 = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            toolTip1 = new ToolTip(components);
            panel2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(panel1);
            panel2.Controls.Add(tabControl1);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 211);
            panel2.Name = "panel2";
            panel2.Size = new Size(396, 100);
            panel2.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(344, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(52, 100);
            panel1.TabIndex = 3;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.ShowToolTips = true;
            tabControl1.Size = new Size(396, 100);
            tabControl1.TabIndex = 3;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(cmbScale);
            tabPage1.Controls.Add(btnToJww);
            tabPage1.Controls.Add(btnSave);
            tabPage1.Controls.Add(listPaste);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(388, 72);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "画像からJw_cad";
            tabPage1.ToolTipText = "クリップボードにある画像をJw_cadの形式に変換します。";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(190, 10);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 4;
            label1.Text = "縮尺";
            toolTip1.SetToolTip(label1, "EMFに文字が含まれている場合はここで縮尺を設定する必要があります。");
            // 
            // cmbScale
            // 
            cmbScale.FormattingEnabled = true;
            cmbScale.Location = new Point(230, 6);
            cmbScale.Name = "cmbScale";
            cmbScale.Size = new Size(86, 23);
            cmbScale.TabIndex = 3;
            // 
            // btnToJww
            // 
            btnToJww.Location = new Point(186, 41);
            btnToJww.Name = "btnToJww";
            btnToJww.Size = new Size(60, 23);
            btnToJww.TabIndex = 1;
            btnToJww.Text = "変換";
            btnToJww.UseVisualStyleBackColor = true;
            btnToJww.Click += btnToJww_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(257, 41);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(60, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "保存...";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // listPaste
            // 
            listPaste.FormattingEnabled = true;
            listPaste.ItemHeight = 15;
            listPaste.Location = new Point(0, 0);
            listPaste.Name = "listPaste";
            listPaste.Size = new Size(180, 64);
            listPaste.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnConvert);
            tabPage2.Controls.Add(listCopy);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(388, 72);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Jw_cadから画像";
            tabPage2.ToolTipText = "Jw_cadの図形をビットマップなどに変換します。";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(186, 41);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(60, 23);
            btnConvert.TabIndex = 1;
            btnConvert.Text = "変換";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // listCopy
            // 
            listCopy.FormattingEnabled = true;
            listCopy.ItemHeight = 15;
            listCopy.Location = new Point(0, 0);
            listCopy.Name = "listCopy";
            listCopy.Size = new Size(180, 64);
            listCopy.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveBorder;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(396, 211);
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 311);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private ListBox listPaste;
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private Button btnToJww;
        private Button btnSave;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label1;
        private ComboBox cmbScale;
        private Button btnConvert;
        private ListBox listCopy;
        private ToolTip toolTip1;
        private Panel panel1;
    }
}
