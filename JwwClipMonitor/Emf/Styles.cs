using EmfHelper;
using JwwClipMonitor.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Emf
{


    class TextStyle
    {
        public string FontName;
        public double FontHeight;
        public double Angle;
        public bool IsItalic;
        public bool IsBold;
        public TextStyle(string fontName, double fontHeight, double angle, bool isItalic, bool isBold)
        {
            FontName = ShapeHelper.GetValidFontName(fontName);
            FontHeight = fontHeight;
            Angle = angle;
            IsItalic = isItalic;
            IsBold = isBold;
        }

        public TextStyle Copy()
        {
            return new TextStyle(FontName, FontHeight, Angle, IsItalic, IsBold);
        }

        public static TextStyle GetSystemTextStyle()
        {
            var fontName = SystemFonts.DefaultFont.Name;
            var fontHeight = SystemFonts.DefaultFont.Height;
            return new TextStyle(fontName, fontHeight, 0.0, false, false);
        }
    }

    class LineStyle
    {
        public bool IsNull;
        public int Width;
        public Color Color;
        public float[]? Pattern;
        public LineStyle(int width, Color color, float[]? pattern)
        {
            Width = width;
            Color = color;
            Pattern = pattern;
        }
        private LineStyle() { IsNull = true; }
        public static LineStyle NullStyle = new LineStyle();
    }

    class FillStyle
    {
        public bool IsNull;
        public Color Color;
        public FillStyle(Color color)
        {
            Color = color;
        }
        private FillStyle() { IsNull = true; }
        public static FillStyle NullLStyle = new FillStyle();
    }

}
