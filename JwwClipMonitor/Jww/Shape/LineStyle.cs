using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    internal class LineStyle
    {
        public static float DefaultLineWidth = 0.25f;
        public Color Color;
        public float Width = DefaultLineWidth;
        public float[]? Pattern;

        public void Set(StyleConverter sc, JwwData s)
        {
            Color = sc.PenToColor(s.m_nPenColor);
        }

        public void DrawLine(Graphics g, DrawContext d, PointF p0, PointF p1)
        {
            d.ApplyLineStyle(this);
            g.DrawLine(d.Pen, p0, p1);
        }
        public void DrawLines(Graphics g, DrawContext d, PointF[] points, bool isClosed)
        {
            d.ApplyLineStyle(this);
            if (isClosed)
            {
                g.DrawPolygon(d.Pen, points);
            }
            else
            {
                g.DrawLines(d.Pen, points);
            }
        }
    }
}
