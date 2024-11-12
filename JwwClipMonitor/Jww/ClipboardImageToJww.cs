using JwwClipMonitor.Emf;
using JwwClipMonitor.Utility;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww
{
    internal static class ClipboardImageToJww
    {
        public static int PngId => ClipboardUtility.RegisterClipboardFormat("PNG");

        public static string GetImageFolder()
        {
            var pictFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (pictFolder == null) throw new DirectoryNotFoundException("Not found MyPictures folder."); ;
            return pictFolder + "\\JwwClipMonitor";
        }
        public static void ConvertToJww(IntPtr hwnd, int formatId, double scale)
        {
            if (formatId == ClipboardUtility.CF_ENHMETAFILE)
            {
                using var ms = GetEmfFromClipboard(hwnd);
                if (ms != null) EmfToJww(hwnd, ms, scale);
                return;
            }
            using var image = GetBitmapImageFromClipboard(formatId);
            if (image != null) ImageToJww(hwnd, image, scale);
            return;
        }

        public static void ImageToJww(IntPtr hwnd, Image originalImage, double scale)
        {
            var folder = GetImageFolder();
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
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
            IntPtr h = cw.Write();
            if (h == IntPtr.Zero) return;
            ClipboardUtility.OpenClipboard(hwnd);
            ClipboardUtility.EmptyClipboard();
            var id = ClipboardUtility.RegisterClipboardFormat("Jw_win");
            if (id != 0) ClipboardUtility.SetClipboardData(id, h);
            ClipboardUtility.CloseClipboard();
        }

        public static void EmfToJww(IntPtr hwnd, MemoryStream ms, double scale)
        {
            var r = new EmfReader();
            r.Read(ms);
            var cw = new JwwClipWriter();
            cw.Header.m_Scales[0] = 1.0 / scale;
            foreach (var d in r.Shapes) cw.AddData(d);
            IntPtr h = cw.Write();
            if (h == IntPtr.Zero) return;
            ClipboardUtility.OpenClipboard(hwnd);
            ClipboardUtility.EmptyClipboard();
            var id = ClipboardUtility.RegisterClipboardFormat("Jw_win");
            if (id != 0) ClipboardUtility.SetClipboardData(id, h);
            ClipboardUtility.CloseClipboard();
        }

        public static Image? GetBitmapImageFromClipboard(int formatId)
        {
            if (formatId == PngId) return GetPngFromClipboard();
            if (formatId == ClipboardUtility.CF_BITMAP) return GetBitmapFromClipboard();
            if (formatId == ClipboardUtility.CF_DIB) return GetDibFromClipboard();
            return null;
        }

        public static MemoryStream? GetEmfFromClipboard(IntPtr hwnd)
        {
            if (ClipboardUtility.OpenClipboard(hwnd))
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

        private static Image? GetPngFromClipboard()
        {
            var d = Clipboard.GetData("PNG");
            if (d is not MemoryStream ms) return null;
            using var image = Image.FromStream(ms);
            ms.Dispose();
            return To24bitBitmap(image);
        }

        private static Image? GetBitmapFromClipboard()
        {
            using var bmp = Clipboard.GetData(DataFormats.Bitmap) as Bitmap;
            return To24bitBitmap(bmp);
        }

        private static Image? GetDibFromClipboard()
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
            ms.WriteByte((byte)(fileSize >> 8 & 0xff));
            ms.WriteByte((byte)(fileSize >> 16 & 0xff));
            ms.WriteByte((byte)(fileSize >> 24 & 0xff));
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            var offset = 14 + dib[0] + (dib[1] << 8) + (dib[2] << 16) + (dib[3] << 24);
            ms.WriteByte((byte)(offset & 0xff));
            ms.WriteByte((byte)(offset >> 8 & 0xff));
            ms.WriteByte((byte)(offset >> 16 & 0xff));
            ms.WriteByte((byte)(offset >> 24 & 0xff));
            ms.Write(dib);
            ms.Position = 0;
            using var bmp = Image.FromStream(ms);
            return To24bitBitmap(bmp);
        }

        private static Bitmap? To24bitBitmap(Image? bmp)
        {
            if (bmp == null) return null;
            // 24ビットに変換
            var image = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(image);
            g.Clear(Color.White);
            g.DrawImage(bmp, 0, 0);
            return image;
        }
    }
}
