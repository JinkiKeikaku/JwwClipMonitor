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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Math;

namespace JwwClipMonitor
{
    public partial class Form1 : Form
    {
        private List<ClipboardFormat> mPasteSupportFormatList = new List<ClipboardFormat>();
        public BindingList<ClipboardFormat> PasteFormatList { get; } = new();
        private IntPtr mLastImageHandle = IntPtr.Zero;

        public Form1()
        {
            InitializeComponent();
            this.Text = Resources.Title_App;
            mPasteSupportFormatList.Add(new("DIB", ClipboardUtility.CF_DIB));
            mPasteSupportFormatList.Add(new("Bitmap", ClipboardUtility.CF_BITMAP));
            mPasteSupportFormatList.Add(new("PNG", ClipboardImageToJww.PngId));
            mPasteSupportFormatList.Add(new("Enhanced Meta File", ClipboardUtility.CF_ENHMETAFILE));
            listPaste.DataSource = PasteFormatList;
            cmbScale.DataSource = Scales;
            cmbScale.ValueMember = "Scale";
            cmbScale.DisplayMember = "Name";
            cmbScale.SelectedValue = 1.0;
        }

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

        private void Form1_Load(object sender, EventArgs e)
        {
            ChangeButtonState();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            UpdatePasteFormatList();
            UpdatePreview();
            ChangeButtonState();
            timer1.Start();
        }

        private void btnToJww_Click(object sender, EventArgs e)
        {
            var s = listPaste.SelectedItem;
            if (s is not ClipboardFormat cf) return;
            var scale = (cmbScale.SelectedItem as PaperScale)?.Scale ?? 1.0;
            ClipboardImageToJww.ConvertToJww(this.Handle, cf.FormatId, scale);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var item = listPaste.SelectedItem as ClipboardFormat;
            if (item == null) return;
            Save(item);
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
                d.FilterIndex = 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    using var image = ClipboardImageToJww.GetBitmapImageFromClipboard(item.FormatId);
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
                return;
            }
            if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE)
            {
                d.Filter = "Enhanced Meta File (*.emf)|*.emf";
                d.FilterIndex = 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    using var ms = ClipboardImageToJww.GetEmfFromClipboard(Handle);
                    if (ms == null) return;
                    File.WriteAllBytes(d.FileName, ms.ToArray());
                }
                return;
            }

        }

        private bool UpdatePasteFormatList()
        {
            var list = new List<ClipboardFormat>();
            foreach (var s in mPasteSupportFormatList)
            {
                if (ClipboardUtility.IsClipboardFormatAvailable(s.FormatId)) list.Add(s);
            }
            var changed = false;
            for (var i = PasteFormatList.Count - 1; i >= 0; i--)
            {
                if (!list.Contains(PasteFormatList[i]))
                {
                    PasteFormatList.RemoveAt(i);
                    changed = true;
                }
            }
            for (var i = 0; i < list.Count; i++)
            {
                if (!PasteFormatList.Contains(list[i]))
                {
                    PasteFormatList.Add(list[i]);
                    changed = true;
                }
            }
            return changed;
        }

        private void UpdatePreview()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: 
                    PreviewImage();
                    break;
                case 1:
                    PreviewJww();
                    break;
            }
        }

        private void PreviewJww()
        {
            using var data = Clipboard.GetData("Jw_win") as MemoryStream;
            if (data == null)
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
                return;
            }

            var jwr = new Jww.JwwReader();
            jwr.Read(data);
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;
            var doc = jwr.Document;
            if (doc.Bounds.IsNull()) return;
            var drawer = new JwwDrawer();
            var w = (int)Ceiling(doc.Bounds.Width() * 96 / 2.54 + 0.5);
            var h = (int)Ceiling(doc.Bounds.Height() * 96 / 2.54 + 0.5);
            var bmp = new Bitmap(w, h);
            using var g = Graphics.FromImage(bmp);
            drawer.Scale = 96 / 2.54f;
            drawer.OnDraw(g, jwr.Document);
            pictureBox1.Image = bmp;
        }

        private void PreviewImage() { 
            if (listPaste.Items.Count == 0)
            {
                if (mLastImageHandle != IntPtr.Zero) pictureBox1.Image = null;
                return;
            }
            var item = listPaste.SelectedItem as ClipboardFormat;
            if (item == null) return;
            if (ClipboardUtility.OpenClipboard(Handle))
            {
                mLastImageHandle = ClipboardUtility.GetClipboardData(item.FormatId);
            }
            ClipboardUtility.CloseClipboard();
            if (item.FormatId == ClipboardUtility.CF_ENHMETAFILE)
            {
                if (ClipboardUtility.OpenClipboard(Handle))
                {
                    var ptr = ClipboardUtility.GetClipboardData(ClipboardUtility.CF_ENHMETAFILE);
                    if (!ptr.Equals(IntPtr.Zero))
                    {
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = new Metafile(ptr, true);
                    }
                }
                ClipboardUtility.CloseClipboard();
            }
            else
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = ClipboardImageToJww.GetBitmapImageFromClipboard(item.FormatId);
            }
        }

        private void ChangeButtonState()
        {
            btnToJww.Enabled = listPaste.SelectedIndex >= 0;
            btnSave.Enabled = listPaste.SelectedIndex >= 0;
        }

    }
}
