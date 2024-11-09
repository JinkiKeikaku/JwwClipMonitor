using EmfHelper;
using JwwClipMonitor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JwwClipMonitor.Emf
{
    class EmfObjectTable
    {
        private Dictionary<uint, object?> mObjectMap = new();
        public void Clear()
        {
            mObjectMap.Clear();
            mObjectMap[(uint)StockObjects.BLACK_PEN] =
                new LineStyle(0, Color.Black, null);
            mObjectMap[(uint)StockObjects.WHITE_PEN] =
                new LineStyle(0, Color.White, null);
            mObjectMap[(uint)StockObjects.WHITE_BRUSH] =
                new FillStyle(Color.White);
            mObjectMap[(uint)StockObjects.BLACK_BRUSH] =
                new FillStyle(Color.Black);
            mObjectMap[(uint)StockObjects.GRAY_BRUSH] =
                new FillStyle(Color.FromArgb(128,128,128));
            mObjectMap[(uint)StockObjects.LTGRAY_BRUSH] =
                new FillStyle(Color.FromArgb(192, 192, 192));
            mObjectMap[(uint)StockObjects.DKGRAY_BRUSH] =
                new FillStyle(Color.FromArgb(64, 64, 64));
            mObjectMap[(uint)StockObjects.HOLLOW_BRUSH] =
                FillStyle.NullLStyle;
            mObjectMap[(uint)StockObjects.NULL_BRUSH] =
                FillStyle.NullLStyle;
            mObjectMap[(uint)StockObjects.DC_BRUSH] =
                new FillStyle(Color.White);
            mObjectMap[(uint)StockObjects.SYSTEM_FONT] = TextStyle.GetSystemTextStyle();
        }

        public void CreatePen(EmrCreatePen pen)
        {
            mObjectMap[pen.IhPen] = GetLineStyle(pen.LogPen);
        }

        public void CreatePen(EmrExtCreatePen pen)
        {
            mObjectMap[pen.IhPen] = GetLineStyle(pen.LogPen);
        }

        public void CreateBrush(EmrCreateBrushIndirect ppl)
        {
            mObjectMap[ppl.IhBrush] = GetFillStyle(ppl.LogBrush);
        }

        public void CreateFont(EmrExtCreateFontIndirectW ppl)
        {
            mObjectMap[ppl.IhFont] = GetTextStyle(ppl.LogFont);
        }

        private TextStyle GetTextStyle(LogFont lf)
        {
            var fontName = EmfUtility.CharsToString(lf.Facename);
            var angle = lf.Escapement * 0.1;
            var height = lf.Height != 0 ? Math.Abs(lf.Height) : TextStyle.GetSystemTextStyle().FontHeight; ;

            //s.IsFontItalic = lf.Italic == 0x01;
            //s.IsFontBold = lf.Weight >= 700;
            //s.IsFontStrikeout = lf.StrikeOut == 0x01;
            return new TextStyle(fontName, height, angle, lf.Italic == 0x01, lf.Weight >= 700);
        }

        public object? GetStyleFromId(uint id)
        {
            if (!mObjectMap.ContainsKey(id)) return null;
            return mObjectMap[id];
        }

        public void DeleteObject(uint id)
        {
            if (!mObjectMap.Remove(id))
            {
                Debug.WriteLine($"DeleteObject(): id={id} not found in table.", GetType().Name);
            }
        }



        private LineStyle? GetLineStyle(LogPen logPen)
        {
            var c = logPen.ColorRef.ToColor();
            var w = logPen.Width.X;
            switch (logPen.Style)
            {
                case PenStyle.PS_NULL:
                    return LineStyle.NullStyle;
                case PenStyle.PS_DOT:
                    return new LineStyle(w,c, null);
                case PenStyle.PS_DASH:
                    return new LineStyle(w,c,null);
                case PenStyle.PS_DASHDOT:
                    return new LineStyle(w, c, null);
                case PenStyle.PS_SOLID:
                default:
                    return new LineStyle(w, c, null);
            }
        }

        private LineStyle? GetLineStyle(LogPenEx logPen)
        {
            var c = logPen.ColorRef.ToColor();
            var w = (int)logPen.Width;
            switch (logPen.Style)
            {
                case PenStyle.PS_NULL:
                    return LineStyle.NullStyle;
                case PenStyle.PS_DOT:
                    return new LineStyle(w, c, null);
                case PenStyle.PS_DASH:
                    return new LineStyle(w, c, null);
                case PenStyle.PS_DASHDOT:
                    return new LineStyle(w, c, null);
                case PenStyle.PS_SOLID:
                default:
                    return new LineStyle(w, c, null);
            }
        }


        private FillStyle? GetFillStyle(LogBrushEx logBrush)
        {
            switch ((BrushStyle)logBrush.Style)
            {
                case BrushStyle.BS_SOLID:
                {
                    var c = logBrush.ColorRef.ToColor();
                    return new FillStyle(c);
                }
                case BrushStyle.BS_NULL:
                default:
                    return FillStyle.NullLStyle;
            }
        }
    }
}
