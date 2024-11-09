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
            btnToJww = new Button();
            listPaste = new ListBox();
            pictureBox1 = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            btnSave = new Button();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(btnSave);
            panel2.Controls.Add(btnToJww);
            panel2.Controls.Add(listPaste);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 211);
            panel2.Name = "panel2";
            panel2.Size = new Size(339, 100);
            panel2.TabIndex = 1;
            // 
            // btnToJww
            // 
            btnToJww.Location = new Point(220, 11);
            btnToJww.Name = "btnToJww";
            btnToJww.Size = new Size(75, 23);
            btnToJww.TabIndex = 1;
            btnToJww.Text = "→Jw_cad";
            btnToJww.UseVisualStyleBackColor = true;
            btnToJww.Click += btnToJww_Click;
            // 
            // listPaste
            // 
            listPaste.FormattingEnabled = true;
            listPaste.ItemHeight = 15;
            listPaste.Location = new Point(12, 11);
            listPaste.Name = "listPaste";
            listPaste.Size = new Size(202, 79);
            listPaste.TabIndex = 0;
            listPaste.SelectedIndexChanged += listPaste_SelectedIndexChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveBorder;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(339, 211);
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
            // btnSave
            // 
            btnSave.Location = new Point(220, 40);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save...";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(339, 311);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panel2.ResumeLayout(false);
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
    }
}
