using JwwClipMonitor.Properties;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    internal class DotStyle
    {
        public Color Color;
        public float Width = 0.25f;
        public bool IsHojo;

        public void Set(DotStyle src)
        {
            Color = src.Color;
            Width = src.Width;
            IsHojo = src.IsHojo;
        }

        public void Set(StyleConverter sc, JwwData s)
        {
            Color = sc.PenToColor(s.m_nPenColor);
            IsHojo = LineStyle.IsHojoDotText(s);
            Width = Settings.Default.LineWidth;
        }
        public void DrawDot(Graphics g, DrawContext d, PointF p0, float radius)
        {
            if (Settings.Default.HiedHojo && IsHojo) return;
            d.ApplyDotStyle(this);
            g.DrawEllipse(d.Pen, p0.X - radius, p0.Y - radius, radius * 2, radius * 2);
        }


    }
}
