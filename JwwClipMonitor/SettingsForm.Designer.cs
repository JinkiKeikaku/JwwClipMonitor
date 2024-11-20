namespace JwwClipMonitor
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            chkTopMost = new CheckBox();
            btnOk = new Button();
            btnCancel = new Button();
            chkPngTransparent = new CheckBox();
            btnColor = new Button();
            toolTip1 = new ToolTip(components);
            cmbDpi = new ComboBox();
            label1 = new Label();
            chkBlack = new CheckBox();
            chkHideHojo = new CheckBox();
            groupBox1 = new GroupBox();
            label2 = new Label();
            cmbLineWidth = new ComboBox();
            chkAntiAlias = new CheckBox();
            linkLabelMail = new LinkLabel();
            linkLabelWeb = new LinkLabel();
            label3 = new Label();
            label4 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // chkTopMost
            // 
            chkTopMost.AutoSize = true;
            chkTopMost.Location = new Point(12, 12);
            chkTopMost.Name = "chkTopMost";
            chkTopMost.Size = new Size(95, 19);
            chkTopMost.TabIndex = 0;
            chkTopMost.Text = "最上面に表示";
            chkTopMost.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(179, 241);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(260, 241);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "CANCEL";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkPngTransparent
            // 
            chkPngTransparent.AutoSize = true;
            chkPngTransparent.Location = new Point(12, 79);
            chkPngTransparent.Name = "chkPngTransparent";
            chkPngTransparent.Size = new Size(74, 19);
            chkPngTransparent.TabIndex = 2;
            chkPngTransparent.Text = "PNG透過";
            toolTip1.SetToolTip(chkPngTransparent, "PNG保存時、背景色を使いません。");
            chkPngTransparent.UseVisualStyleBackColor = true;
            // 
            // btnColor
            // 
            btnColor.Location = new Point(14, 52);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(75, 23);
            btnColor.TabIndex = 3;
            btnColor.Text = "背景色";
            btnColor.UseVisualStyleBackColor = true;
            btnColor.Click += btnColor_Click;
            // 
            // cmbDpi
            // 
            cmbDpi.FormattingEnabled = true;
            cmbDpi.Location = new Point(136, 31);
            cmbDpi.Name = "cmbDpi";
            cmbDpi.Size = new Size(85, 23);
            cmbDpi.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 34);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 5;
            label1.Text = "ビットマップ解像度(dpi)";
            // 
            // chkBlack
            // 
            chkBlack.AutoSize = true;
            chkBlack.Location = new Point(11, 22);
            chkBlack.Name = "chkBlack";
            chkBlack.Size = new Size(105, 19);
            chkBlack.TabIndex = 0;
            chkBlack.Text = "線を黒色で表示";
            chkBlack.UseVisualStyleBackColor = true;
            // 
            // chkHideHojo
            // 
            chkHideHojo.AutoSize = true;
            chkHideHojo.Location = new Point(138, 22);
            chkHideHojo.Name = "chkHideHojo";
            chkHideHojo.Size = new Size(107, 19);
            chkHideHojo.TabIndex = 0;
            chkHideHojo.Text = "補助線を非表示";
            chkHideHojo.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chkBlack);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(cmbLineWidth);
            groupBox1.Controls.Add(chkHideHojo);
            groupBox1.Location = new Point(9, 102);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(279, 81);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Jw_cad";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 50);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 5;
            label2.Text = "線幅";
            // 
            // cmbLineWidth
            // 
            cmbLineWidth.FormattingEnabled = true;
            cmbLineWidth.Location = new Point(68, 47);
            cmbLineWidth.Name = "cmbLineWidth";
            cmbLineWidth.Size = new Size(85, 23);
            cmbLineWidth.TabIndex = 4;
            // 
            // chkAntiAlias
            // 
            chkAntiAlias.AutoSize = true;
            chkAntiAlias.Location = new Point(101, 79);
            chkAntiAlias.Name = "chkAntiAlias";
            chkAntiAlias.Size = new Size(97, 19);
            chkAntiAlias.TabIndex = 2;
            chkAntiAlias.Text = "アンチエイリアス";
            chkAntiAlias.UseVisualStyleBackColor = true;
            // 
            // linkLabelMail
            // 
            linkLabelMail.AutoSize = true;
            linkLabelMail.Location = new Point(48, 190);
            linkLabelMail.Name = "linkLabelMail";
            linkLabelMail.Size = new Size(135, 15);
            linkLabelMail.TabIndex = 7;
            linkLabelMail.TabStop = true;
            linkLabelMail.Text = "mailto:ai@junkbulk.com";
            linkLabelMail.LinkClicked += linkLabelMail_LinkClicked;
            // 
            // linkLabelWeb
            // 
            linkLabelWeb.AutoSize = true;
            linkLabelWeb.Location = new Point(48, 215);
            linkLabelWeb.Name = "linkLabelWeb";
            linkLabelWeb.Size = new Size(295, 15);
            linkLabelWeb.TabIndex = 7;
            linkLabelWeb.TabStop = true;
            linkLabelWeb.Text = "https://www.junkbulk.com/windows/JwwClipMonitor/";
            linkLabelWeb.LinkClicked += linkLabelWeb_LinkClicked;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 190);
            label3.Name = "label3";
            label3.Size = new Size(30, 15);
            label3.TabIndex = 5;
            label3.Text = "Mail";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 215);
            label4.Name = "label4";
            label4.Size = new Size(31, 15);
            label4.TabIndex = 5;
            label4.Text = "Web";
            // 
            // SettingsForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(356, 268);
            Controls.Add(linkLabelWeb);
            Controls.Add(linkLabelMail);
            Controls.Add(groupBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(cmbDpi);
            Controls.Add(btnColor);
            Controls.Add(chkAntiAlias);
            Controls.Add(chkPngTransparent);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(chkTopMost);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "設定";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkTopMost;
        private Button btnOk;
        private Button btnCancel;
        private CheckBox chkPngTransparent;
        private Button btnColor;
        private ToolTip toolTip1;
        private ComboBox cmbDpi;
        private Label label1;
        private CheckBox chkBlack;
        private CheckBox chkHideHojo;
        private GroupBox groupBox1;
        private CheckBox chkAntiAlias;
        private Label label2;
        private ComboBox cmbLineWidth;
        private LinkLabel linkLabelMail;
        private LinkLabel linkLabelWeb;
        private Label label3;
        private Label label4;
    }
}