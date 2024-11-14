using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace JwwClipMonitor.Jww
{
    internal static class ClipboardJwwToImage
    {
        public static Bitmap? JwwToImage(Color bgColor, bool useAlpha)
        {
            var doc = CreateDocument();
            if(doc == null) return null;
            if (doc.Bounds.IsNull()) return null;
            var w = (int)Ceiling(doc.Bounds.Width() * 96 / 25.4 + 0.5);
            var h = (int)Ceiling(doc.Bounds.Height() * 96 / 25.4 + 0.5);
            if (w == 0 || h == 0) return null;
            var scale = 1.0;
            //ビットマップサイズの上限が32ビットの場合1GBなので変換。
            if (((double)w) * ((double)h) > 256_000_000)
            {
                scale = Sqrt(256_000_000 / 4 / (((double)w) * ((double)h)));
                w = (int)(w * scale);
                h = (int)(h * scale);
            }
            var bmp = new Bitmap(w, h, useAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bmp);
            g.Clear(bgColor);
            var drawer = new JwwDrawer();
            drawer.Scale = 96 / 25.4f * (float)scale;
            drawer.OnDraw(g, doc);
            return bmp;
        }

        public static Metafile? JwwToMetafile()
        {
            var doc = CreateDocument();
            if (doc == null) return null;
            var scale = 1.0;
            using var bmp = new Bitmap(4, 4);
            var grfx = Graphics.FromImage(bmp);
            IntPtr ipHdc = grfx.GetHdc();
            var mf = new Metafile(ipHdc, EmfType.EmfOnly);
            grfx.ReleaseHdc(ipHdc);
            using var g = Graphics.FromImage(mf);
            var drawer = new JwwDrawer();
            drawer.Scale =  96 / 25.4f * (float)scale;
            drawer.OnDraw(g, doc);
            return mf;
        }

        private static Document? CreateDocument()
        {
            using var data = Clipboard.GetData("Jw_win") as MemoryStream;
            if (data == null) return null;
            var jwr = new Jww.JwwReader();
            jwr.Read(data);
            return jwr.Document;
        }
    }
}
