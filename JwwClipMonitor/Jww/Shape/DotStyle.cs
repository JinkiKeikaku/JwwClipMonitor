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

        public void Set(DotStyle src)
        {
            Color = src.Color;
            Width = src.Width;
        }

        public void Set(StyleConverter sc, JwwData s)
        {
            Color = sc.PenToColor(s.m_nPenColor);
        }
        public void DrawDot(Graphics g, DrawContext d, PointF p0, float radius)
        {
            d.ApplyDotStyle(this);
            g.DrawEllipse(d.Pen, p0.X - radius, p0.Y - radius, radius * 2, radius * 2);
        }


    }
}
