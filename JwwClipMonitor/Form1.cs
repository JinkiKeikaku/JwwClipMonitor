using JwwClipMonitor.Emf;
using JwwClipMonitor.Jww;
using JwwClipMonitor.Properties;
using JwwClipMonitor.Utility;
using JwwHelper;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Math;
using static CadMath2D.CadMath;
namespace JwwClipMonitor
{
    public partial class Form1 : Form
    {
        internal static readonly PaperScale[] Scales = new PaperScale[] {
            new PaperScale("5:1", 5.0),
            new PaperScale("2:1", 2.0),
            new PaperScale("1:1", 1.0),
            new PaperScale("1:2", 0.5),
            new PaperScale("1:5", 0.2),
            new PaperScale("1:10", 0.1),
            new PaperScale("1:20", 0.05),
            new PaperScale("1:50", 0.02),
            new PaperScale("1:100", 0.01),
            new PaperScale("1:200", 0.005),
            new PaperScale("1:500", 0.002),
            new PaperScale("1:1000", 0.001),
        };

        private List<ClipboardFormat> mToJwwFormatList = new();
        private List<ClipboardFormat> mFromJwwFormatList = new();
        private ClipboardViewer mClipboardViewer;
        private Point mMouseDragPosition = new();
        private int mConvertMode = Settings.Default.ConvertMode;
        private ClipboardFormat? mLastToJwwFormat = null;
        private ClipboardFormat? mLastFromJwwFormat = null;

        public Form1()
        {
            InitializeComponent();
            this.Text = $"{Resources.Title_App} ver.{SystemUtility.GetAppVersion()}";
            mToJwwFormatList.Add(new("DIB->Jw_cad", ClipboardUtility.CF_DIB, true));
            mToJwwFormatList.Add(new("Bitmap->Jw_cad", ClipboardUtility.CF_BITMAP, true));
            mToJwwFormatList.Add(new("PNG->Jw_cad", ClipboardImageToJww.PngId, true));
            mToJwwFormatList.Add(new("EMF->Jw_cad", ClipboardUtility.CF_ENHMETAFILE, true));
            mFromJwwFormatList.Add(new("Jw_cad->Bitmap", ClipboardUtility.CF_BITMAP, false));
            mFromJwwFormatList.Add(new("Jw_cad->PNG", ClipboardImageToJww.PngId, false));
            mFromJwwFormatList.Add(new("Jw_cad->EMF", ClipboardUtility.CF_ENHMETAFILE, false));
            listFormat.DataSource = FormatList;
            cmbScale.DataSource = Scales;
            cmbScale.ValueMember = "Scale";
            cmbScale.DisplayMember = "Name";
            cmbScale.SelectedValue = 1.0;
            mClipboardViewer = new ClipboardViewer(this, () =>
            {
                UpdateFormatList();
                UpdatePreview();
            });
        }
        public BindingList<ClipboardFormat> FormatList { get; } = new();
        private void Form1_Load(object sender, EventArgs e)
        {
            ChangeButtonState();
            TopMost = Settings.Default.IsTopMost;
            pictureBox1.MouseDown += (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    //位置を記憶する
                    mMouseDragPosition = new Point(e.X, e.Y);
                }
            };
            pictureBox1.MouseMove += (sender, e) =>
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    this.Left += e.X - mMouseDragPosition.X;
                    this.Top += e.Y - mMouseDragPosition.Y;
                }
            };
            var radios = panelDirection.Controls.OfType<RadioButton>();

            foreach (var radio in radios)
            {
                radio.CheckedChanged += (object? sender, EventArgs e) =>
                {
                    if (sender is RadioButton r)
                    {
                        if (r.Checked)
                        {
                            int.TryParse(r.Tag.ToString(), out mConvertMode);
                            UpdateFormatList();
                            UpdatePreview();
                            ChangeButtonState();
                            Settings.Default.ConvertMode = mConvertMode;
                        }
                    }
                };
                if (int.TryParse(radio.Tag.ToString(), out var x) && x == mConvertMode) radio.Checked = true;
            }
            var i = Array.FindIndex(Scales, x => FloatEQ(x.Scale, Settings.Default.Scale));
            if (i < 0) i = 0;
            cmbScale.SelectedIndex = i;
            cmbScale.SelectedIndexChanged += (object? sender, EventArgs e) =>
            {
                var i = cmbScale.SelectedIndex;
                if (i >= 0 && i < Scales.Length) Settings.Default.Scale = Scales[i].Scale;
            };
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            ChangeButtonState();
            timer1.Start();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var item = listFormat.SelectedItem as ClipboardFormat;
            if (item != null) Save(item);
        }

        /// <summary>
        /// 変換ボタンの処理
        /// </summary>
        private void btnConvert_Click(object sender, EventArgs e)
        {
            var item = listFormat.SelectedItem as ClipboardFormat;
            if (item == null) return;
            if (item.ToJww)
            {
                var scale = Settings.Default.Scale;// Validate (cmbScale.SelectedItem as PaperScale)?.Scale ?? 1.0;
                ClipboardImageToJww.ConvertToJww(this.Handle, item.FormatId, scale);
            }
            else
            {
                if (item.FormatId == ClipboardImageToJww.PngId) CopyJwwAsPng();
                else if (item.FormatId == ClipboardUtility.CF_BITMAP) CopyJwwAsBitmap();
                else if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE) CopyJwwAsEmf();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var d = new SettingsForm();
            TopMost = false;
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                UpdatePreview();
            }
            TopMost = Settings.Default.IsTopMost;
        }

        private void ChangeButtonState()
        {
            var item = listFormat.SelectedItem as ClipboardFormat;
            btnConvert.Enabled = item != null;
            btnSave.Enabled = item != null;
            cmbScale.Enabled = mConvertMode == 0;// item?.ToJww == true;// && item?.FormatId == ClipboardUtility.CF_ENHMETAFILE;
        }

        private void CopyJwwAsBitmap()
        {
            var bg = Settings.Default.BgColor;
            var bmp = ClipboardJwwToImage.JwwToImage(bg, false);
            if (bmp == null) return;
            Clipboard.SetImage(bmp);
        }
        private void CopyJwwAsPng()
        {
            var bg = Settings.Default.IsPngTransparent ? Color.Transparent : Settings.Default.BgColor;
            var bmp = ClipboardJwwToImage.JwwToImage(bg, true);
            if (bmp == null) return;

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Flush();
            if (ClipboardUtility.OpenClipboard(Handle))
            {
                ClipboardUtility.EmptyClipboard();
                ClipboardUtility.SetClipboardData(ClipboardUtility.RegisterClipboardFormat("PNG"), ms.ToArray());
                ClipboardUtility.CloseClipboard();
            }
        }
        private void CopyJwwAsEmf()
        {
            var mf = ClipboardJwwToImage.JwwToMetafile();
            if (mf == null) return;
            var h = mf.GetHenhmetafile();
            if (ClipboardUtility.OpenClipboard(Handle))
            {
                ClipboardUtility.EmptyClipboard();
                ClipboardUtility.SetClipboardData(ClipboardUtility.CF_ENHMETAFILE, h);
                ClipboardUtility.CloseClipboard();
            }
        }

        private void Save(ClipboardFormat item)
        {
            var d = new SaveFileDialog();
            d.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (item.FormatId == ClipboardUtility.CF_BITMAP ||
                item.FormatId == ClipboardUtility.CF_DIB ||
                item.FormatId == ClipboardImageToJww.PngId)
            {
                d.Filter = "Bitmap file (*.bmp)|*.bmp|PNG file (*.png)|*.png";
                d.FilterIndex = item.FormatId == ClipboardImageToJww.PngId ? 2 : 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    Image? image = null;
                    try
                    {
                        if (item.ToJww)
                        {
                            image = ClipboardImageToJww.GetBitmapImageFromClipboard(item.FormatId);
                        }
                        else
                        {
                            if (item.FormatId == ClipboardImageToJww.PngId)
                            {
                                var bg = Settings.Default.IsPngTransparent ? Color.Transparent : Settings.Default.BgColor;
                                image = ClipboardJwwToImage.JwwToImage(bg, true);
                            }
                            else
                            {
                                var bg = Settings.Default.BgColor;
                                image = ClipboardJwwToImage.JwwToImage(bg, false);
                            }
                        }
                        if (image != null)
                        {
                            if (d.FilterIndex == 1)
                            {
                                image.Save(d.FileName, ImageFormat.Bmp);
                                return;
                            }
                            if (d.FilterIndex == 2)
                            {
                                image.Save(d.FileName, ImageFormat.Png);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        image?.Dispose();
                    }
                }
                return;
            }
            if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE)
            {
                d.Filter = "Enhanced Meta File (*.emf)|*.emf";
                d.FilterIndex = 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    if (item.ToJww)
                    {
                        using var ms = ClipboardImageToJww.GetEmfFromClipboard(Handle);
                        if (ms == null) return;
                        File.WriteAllBytes(d.FileName, ms.ToArray());
                    }
                    else
                    {
                        var mf = ClipboardJwwToImage.JwwToMetafile();
                        if (mf != null)
                        {
                            var h = mf.GetHenhmetafile();
                            if (h != IntPtr.Zero)
                            {
                                uint bufferSize = ClipboardUtility.GetEnhMetaFileBits(h, 0, null);
                                var dataArray = new byte[bufferSize];
                                ClipboardUtility.GetEnhMetaFileBits(h, bufferSize, dataArray);
                                File.WriteAllBytes(d.FileName, dataArray);
                                ClipboardUtility.DeleteEnhMetaFile(h);
                            }
                        }
                    }
                }
                return;
            }

        }

        private bool mListChanging = false;
        private bool UpdateFormatList()
        {
            try
            {
                mListChanging = true;
                var list = new List<ClipboardFormat>();
                if (mConvertMode == 1)
                {
                    if (ClipboardUtility.IsClipboardFormatAvailable(ClipboardJwwToImage.JwwId))
                    {
                        list.AddRange(mFromJwwFormatList);
                    }
                }
                if (mConvertMode == 0)
                {
                    foreach (var s in mToJwwFormatList)
                    {
                        if (ClipboardUtility.IsClipboardFormatAvailable(s.FormatId)) list.Add(s);
                    }
                }
                var changed = false;
                for (var i = FormatList.Count - 1; i >= 0; i--)
                {
                    if (!list.Contains(FormatList[i]))
                    {
                        FormatList.RemoveAt(i);
                        changed = true;
                    }
                }
                for (var i = 0; i < list.Count; i++)
                {
                    if (!FormatList.Contains(list[i]))
                    {
                        FormatList.Add(list[i]);
                        changed = true;
                    }
                }

                if (mConvertMode == 0)
                {
                    if (mLastToJwwFormat != null)
                    {
                        var i = FormatList.IndexOf(mLastToJwwFormat);
                        if (i >= 0) listFormat.SelectedIndex = i;
                    }
                }
                else if (mConvertMode == 1)
                {
                    if (mLastFromJwwFormat != null)
                    {
                        var i = FormatList.IndexOf(mLastFromJwwFormat);
                        if (i >= 0)
                        {
                            listFormat.SelectedIndex = i;
                        }
                    }
                }
                return changed;
            }
            finally
            {
                mListChanging = false;
            }
        }

        private void UpdatePreview()
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;
            if (listFormat.Items.Count == 0) return;
            var item = listFormat.SelectedItem as ClipboardFormat;
            if (item == null) return;
            if (item.ToJww)
            {
                if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE)
                {
                    if (!ClipboardUtility.OpenClipboard(Handle)) return;
                    var ptr = ClipboardUtility.GetClipboardData(ClipboardUtility.CF_ENHMETAFILE);
                    if (!ptr.Equals(IntPtr.Zero)) pictureBox1.Image = new Metafile(ptr, true);
                    ClipboardUtility.CloseClipboard();
                }
                else
                {
                    pictureBox1.Image = ClipboardImageToJww.GetBitmapImageFromClipboard(item.FormatId);
                }
            }
            else
            {
                var bg = Settings.Default.BgColor;
                if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE ||
                    (item.FormatId == ClipboardImageToJww.PngId && Settings.Default.IsPngTransparent))
                {
                    bg = Color.Transparent;
                }
                var bmp = ClipboardJwwToImage.JwwToImage(bg, true);
                if (bmp != null) pictureBox1.Image = bmp;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }

        private void listFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mListChanging)
            {
                if (mConvertMode == 0)
                {
                    var a = listFormat.SelectedItem as ClipboardFormat;
                    if (a != null) mLastToJwwFormat = a;
                }
                else if (mConvertMode == 1)
                {
                    var a = listFormat.SelectedItem as ClipboardFormat;
                    if (a != null) mLastFromJwwFormat = a;
                }
            }
            UpdatePreview();
        }

        private void Form1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PreCAD_windows_manual.html");
            if (File.Exists(s)) SystemUtility.OpenURL(s);
        }
    }
}
