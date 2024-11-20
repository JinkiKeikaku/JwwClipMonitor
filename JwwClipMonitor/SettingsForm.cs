using JwwClipMonitor.Properties;
using JwwClipMonitor.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace JwwClipMonitor
{
    public partial class SettingsForm : Form
    {
        public int[] Dpis = { 96, 150, 200, 300 };
        public float[] LineWidths = { 0.125f, 0.25f, 0.35f, 0.5f };
        public SettingsForm()
        {
            InitializeComponent();
            chkTopMost.Checked = Settings.Default.IsTopMost;
            chkPngTransparent.Checked = Settings.Default.IsPngTransparent;
            btnColor.BackColor = Settings.Default.BgColor;
            cmbDpi.DataSource = Dpis;
            cmbDpi.SelectedItem = Settings.Default.BitmapDpi;
            chkBlack.Checked = Settings.Default.DrawLineBlack;
            chkHideHojo.Checked = Settings.Default.HiedHojo;
            chkAntiAlias.Checked = Settings.Default.AntiAlias;
            cmbLineWidth.DataSource = LineWidths;
            cmbLineWidth.SelectedItem = Settings.Default.LineWidth;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Settings.Default.IsTopMost = chkTopMost.Checked;
            Settings.Default.BgColor = btnColor.BackColor;
            Settings.Default.IsPngTransparent = chkPngTransparent.Checked;
            Settings.Default.DrawLineBlack = chkBlack.Checked;
            Settings.Default.HiedHojo = chkHideHojo.Checked;
            Settings.Default.AntiAlias = chkAntiAlias.Checked;
            Settings.Default.BitmapDpi = GetSelectedComboBoxValue<int>(cmbDpi, Dpis[0]);
            Settings.Default.LineWidth = GetSelectedComboBoxValue<float>(cmbLineWidth, LineWidths[0]);

            //if (int.TryParse(cmbDpi.SelectedItem.ToString(), out var result) )
            //{
            //    Settings.Default.BitmapDpi = result;
            //}
            //else
            //{
            //    Settings.Default.BitmapDpi = Dpis[0];
            //}
        }

        private T GetSelectedComboBoxValue<T>(ComboBox cb, T defaultValue)
        {
            T result = default(T)!;
            return TryParse<T>(cb.SelectedItem?.ToString() ?? "", ref result) ? result : defaultValue;
        }

        public bool TryParse<T>(string input, ref T result)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    //ConvertFromString(string text)の戻りは object なので T型でキャストする
                    var t = converter.ConvertFromString(input);
                    if (t == null) return false;
                    result = (T)t;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            var d = new ColorDialog();
            d.Color = Color.White;
            if (d.ShowDialog() == DialogResult.OK)
            {
                btnColor.BackColor = d.Color;
            }
        }

        private void linkLabelMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            if (link == null) return;
            var subject = $@"{Resources.MailSubject} {SystemUtility.GetAppVersion()}({RuntimeInformation.OSDescription})";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @$"{link.Text}?subject={subject}",
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }

        private void linkLabelWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            if (link == null) return;
            var processStartInfo = new ProcessStartInfo
            {
                FileName = link.Text,// @$"https://junkbulk.com/",
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
    }
}
