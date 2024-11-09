using JwwClipMonitor.Emf;
using JwwClipMonitor.Properties;
using JwwClipMonitor.Utility;
using JwwHelper;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace JwwClipMonitor
{
    public partial class Form1 : Form
    {
        public class ClipboardFormat
        {
            public string FormatName;
            public int Formatid;

            public ClipboardFormat(string formatName, int formatid)
            {
                FormatName = formatName;
                Formatid = formatid;
            }

            public override string ToString() => FormatName;
        }
        private List<ClipboardFormat> mPasteSupportFormatList = new List<ClipboardFormat>();
        public BindingList<ClipboardFormat> PasteFormatList { get; } = new();

        private int PngId = -1;
        private IntPtr mLastImageHandle = IntPtr.Zero;

        public Form1()
        {
            InitializeComponent();
            this.Text = Resources.Title_App;
            PngId = ClipboardUtility.RegisterClipboardFormat("PNG");
            mPasteSupportFormatList.Add(new("DIB", ClipboardUtility.CF_DIB));
            mPasteSupportFormatList.Add(new("Bitmap", ClipboardUtility.CF_BITMAP));
            mPasteSupportFormatList.Add(new("PNG", PngId));
            mPasteSupportFormatList.Add(new("Enhanced Meta File", ClipboardUtility.CF_ENHMETAFILE));
            listPaste.DataSource = PasteFormatList;
        }

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

        private bool UpdatePasteFormatList()
        {
            var list = new List<ClipboardFormat>();
            var old = listPaste.SelectedItem as ClipboardFormat;
            foreach (var s in mPasteSupportFormatList)
            {
                if (ClipboardUtility.IsClipboardFormatAvailable(s.Formatid)) list.Add(s);
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
            if (listPaste.Items.Count == 0)
            {
                if (mLastImageHandle != IntPtr.Zero) pictureBox1.Image = null;
                return;
            }
            var item = listPaste.SelectedItem as ClipboardFormat;
            if (item == null) return;
            var imageChanged = false;
            if (ClipboardUtility.OpenClipboard(Handle))
            {
                imageChanged = ClipboardUtility.GetClipboardData(item.Formatid) != mLastImageHandle;
                mLastImageHandle = ClipboardUtility.GetClipboardData(item.Formatid);
            }
            ClipboardUtility.CloseClipboard();
            if (!imageChanged) return;
            if (item.Formatid == ClipboardUtility.CF_ENHMETAFILE)
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
                pictureBox1.Image = GetBitmapImage(item.Formatid);
            }
        }

        private void btnToJww_Click(object sender, EventArgs e)
        {
            var s = listPaste.SelectedItem;
            if (s is not ClipboardFormat cf) return;
            ConvertToJww(cf.Formatid);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var item = listPaste.SelectedItem as ClipboardFormat;
            if (item == null) return;
            var d = new SaveFileDialog();
            d.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (item.Formatid == ClipboardUtility.CF_BITMAP || item.Formatid == ClipboardUtility.CF_DIB || item.Formatid == PngId)
            {
                d.Filter = "Bitmap file (*.bmp)|*.bmp|PNG file (*.png)|*.png";
                d.FilterIndex = 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    using var image = GetBitmapImage(item.Formatid);
                    if(image != null){
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
            if(item.Formatid == ClipboardUtility.CF_ENHMETAFILE)
            {
                d.Filter = "Enhanced Meta File (*.emf)|*.emf";
                d.FilterIndex = 1;
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    using var ms = GetEmfFromClipboard();
                    if (ms == null) return;
                    File.WriteAllBytes(d.FileName, ms.ToArray());
                }
                return;
            }
        }

        private void listPaste_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ChangeButtonState()
        {
            btnToJww.Enabled = listPaste.SelectedIndex >= 0;
            btnSave.Enabled = listPaste.SelectedIndex >= 0;
        }

        private Image? GetBitmapImage(int formatId)
        {
            if (formatId == PngId) return GetPngFromClipboard();
            if (formatId == ClipboardUtility.CF_BITMAP) return GetBitmapFromClipboard();
            if (formatId == ClipboardUtility.CF_DIB) return GetDibFromClipboard();
            return null;
        }

        private void ConvertToJww(int formatId)
        {
            if (formatId == ClipboardUtility.CF_ENHMETAFILE)
            {
                using var ms = GetEmfFromClipboard();
                if (ms != null) EmfToJww(ms);
                return;
            }
            using var image = GetBitmapImage(formatId);
            if (image != null) ImageToJww(image);
            return;
        }

        private Image? GetPngFromClipboard()
        {
            var d = System.Windows.Forms.Clipboard.GetData("PNG");
            if (d is not MemoryStream ms) return null;
            using var image = Bitmap.FromStream(ms);
            ms.Dispose();
            return To24bitBitmap(image);
        }

        private Image? GetBitmapFromClipboard()
        {
            using var bmp = Clipboard.GetData(DataFormats.Bitmap) as Bitmap;
            return To24bitBitmap(bmp);
        }

        private Image? GetDibFromClipboard()
        {
            var d = Clipboard.GetData(DataFormats.Dib);
            var ds = d as MemoryStream;
            var dib = ds?.ToArray();
            ds?.Dispose();
            if (dib == null) return null;
            using var ms = new MemoryStream();
            ms.WriteByte(0x42);
            ms.WriteByte(0x4D);
            var fileSize = 14 + dib.Length;
            ms.WriteByte((byte)(fileSize & 0xff));
            ms.WriteByte((byte)((fileSize >> 8) & 0xff));
            ms.WriteByte((byte)((fileSize >> 16) & 0xff));
            ms.WriteByte((byte)((fileSize >> 24) & 0xff));
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            var offset = 14 + dib[0] + (dib[1] << 8) + (dib[2] << 16) + (dib[3] << 24);
            ms.WriteByte((byte)(offset & 0xff));
            ms.WriteByte((byte)((offset >> 8) & 0xff));
            ms.WriteByte((byte)((offset >> 16) & 0xff));
            ms.WriteByte((byte)((offset >> 24) & 0xff));
            ms.Write(dib);
            ms.Position = 0;
            using var bmp = Bitmap.FromStream(ms);
            return To24bitBitmap(bmp);
        }

        private Bitmap? To24bitBitmap(Image? bmp)
        {
            if (bmp == null) return null;
            // 24ビットに変換
            var image = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(image);
            g.Clear(Color.White);
            g.DrawImage(bmp, 0, 0);
            return image;
        }

        private MemoryStream? GetEmfFromClipboard()
        {
            if (ClipboardUtility.OpenClipboard(Handle))
            {
                try
                {
                    var h = ClipboardUtility.GetClipboardData(ClipboardUtility.CF_ENHMETAFILE);
                    if (h != IntPtr.Zero)
                    {
                        uint bufferSize = ClipboardUtility.GetEnhMetaFileBits(h, 0, null);
                        var dataArray = new byte[bufferSize];
                        ClipboardUtility.GetEnhMetaFileBits(h, bufferSize, dataArray);
                        return new MemoryStream(dataArray);
                    }
                }
                finally
                {
                    ClipboardUtility.CloseClipboard();
                }
            }
            return null;
        }

        private void ImageToJww(Image originalImage)
        {
            var pictFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (pictFolder == null) return;
            var folder = pictFolder + "\\JwwClipMonitor";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var dt = DateTime.Now.ToString("yyyyMMdd_HHmmss_FFFFFFF") + ".bmp";
            var tfn = Path.Combine(folder, dt);

            // 32ビットビットマップだとjw_cadの背景色透過が働かないので24ビットに変換
            using var image = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(image);
            g.DrawImage(originalImage, 0, 0);

            var width = image.Width * 24.4 / image.HorizontalResolution;
            var height = image.Height * 25.4 / image.VerticalResolution;
            image.Save(tfn, ImageFormat.Bmp);
            var name = $"^@BM{tfn},{width},{height},0,0,1,0";
            var moji = new JwwMoji();
            moji.m_string = name;
            moji.m_dSizeX = 2;
            moji.m_dSizeY = 2;
            moji.m_start_x = 0;
            moji.m_start_y = 0;
            moji.m_end_x = name.Length * moji.m_dSizeX;
            moji.m_end_y = 0;
            moji.m_strFontName = "ＭＳ ゴシック";//決め打ち
            var cw = new JwwClipWriter();
            cw.AddData(moji);
            //            cw.Write();
            IntPtr h = cw.Write();
            if (h == IntPtr.Zero) return;
            ClipboardUtility.OpenClipboard(this.Handle);
            ClipboardUtility.EmptyClipboard();
            var id = ClipboardUtility.RegisterClipboardFormat("Jw_win");
            if (id != 0) ClipboardUtility.SetClipboardData(id, h);
            ClipboardUtility.CloseClipboard();
        }

        void EmfToJww(MemoryStream ms)
        {
            var r = new EmfReader();
            r.Read(ms);
            var cw = new JwwClipWriter();
            foreach (var d in r.Shapes) cw.AddData(d);
            IntPtr h = cw.Write();
            if (h == IntPtr.Zero) return;
            ClipboardUtility.OpenClipboard(this.Handle);
            ClipboardUtility.EmptyClipboard();
            var id = ClipboardUtility.RegisterClipboardFormat("Jw_win");
            if (id != 0) ClipboardUtility.SetClipboardData(id, h);
            ClipboardUtility.CloseClipboard();
        }

    }
}
