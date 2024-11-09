using EmfHelper;
using JwwClipMonitor.Emf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Utility
{
    internal static class ShapeHelper
    {
        private static Bitmap sDummyBitmap = new(16, 16);

        public static Color ToColor(this ColorRef c)
        {
            return Color.FromArgb(c.Red, c.Green, c.Blue);
        }

        public static Font GetDefaultFont() => System.Drawing.SystemFonts.DefaultFont;

        public static string GetDefaultFontName() => GetDefaultFont().Name;

        public static string GetValidFontName(string name)
        {
            var f = new Font(name, 12);
            if (f.Name == name) return name;
            return GetDefaultFontName();
        }

        public static (SizeF size, double ry) GetStringSize(string s, TextStyle ts, double h)
        {
            var font = new Font(ts.FontName, (float)h, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(sDummyBitmap);
            // 謎の空白付きの文字列のサイズを計測。
            var graphicsSize = graphics.MeasureString(s, font);
            FontFamily ff = font.FontFamily;
            float lineSpace = ff.GetLineSpacing(font.Style);
            float ascent = ff.GetCellAscent(font.Style);
            float emh = ff.GetEmHeight(font.Style);
            float des = ff.GetCellDescent(font.Style);
            var ry = des / (ascent + des);// des / emh; //des / lineSpace;//des / (ascent + des);// 1.0 - ascent / (ascent + des);////1.0 - ascent / emh;// des / emh;// (1.0f - ascent / lineSpace);
            return (graphics.MeasureString(s, font, (int)graphicsSize.Width, StringFormat.GenericTypographic), ry);
        }
    }
}
