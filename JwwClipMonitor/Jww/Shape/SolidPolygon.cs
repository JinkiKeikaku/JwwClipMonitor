using CadMath2D;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CadMath2D.CadPoint;
using static CadMath2D.CadMath;

namespace JwwClipMonitor.Jww.Shape
{
    internal class SolidPolygon:IShape
    {
        Color Color;
        List<CadPoint> Points = new();



        public SolidPolygon(StyleConverter sc, JwwSolid s)
        {
            Points.Add(new CadPoint(s.m_start_x, s.m_start_y));
            Points.Add(new CadPoint(s.m_end_x, s.m_end_y));
            Points.Add(new CadPoint(s.m_DPoint2_x, s.m_DPoint2_y));
            Points.Add(new CadPoint(s.m_DPoint3_x, s.m_DPoint3_y));
            if (PointEQ(Points[0], Points[3])) Points.RemoveAt(3);
            if (PointEQ(Points[0], Points[2])) Points.RemoveAt(2);
            Color = s.m_nPenColor == 10 ? s.m_Color.ToColor(): sc.PenToColor(s.m_nPenColor);
        }

        public SolidPolygon(Color color, IReadOnlyList<CadPoint> points)
        {
            Color = color;
            PolylineHelper.CopyPoints(points);
        }

        public IShape Clone()
        {
            return new SolidPolygon(Color, Points);
        }

        public CadRect GetExtent() => CadRect.GetBounds(Points);
        
        public void OnDraw(Graphics g, DrawContext d)
        {
            var pts = new PointF[Points.Count];
            for (int i = 0;i < pts.Length; i++) pts[i] = d.DocToCanvas(Points[i]);
            if(Points.Count == 2)
            {
                d.Pen.Color = Color;
                d.Pen.Width = LineStyle.DefaultLineWidth;
                d.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                g.DrawLine(d.Pen, pts[0], pts[1]);
            }
            else
            {
                using var b = new SolidBrush(Color);
                g.FillPolygon(b, pts);
            }
        }
        public void Offset(double dx, double dy)
        {
            foreach(var p in Points) p.Offset(dx, dy);
        }
        public void Rotate(CadPoint p0, double angleRad)
        {
            foreach (var p in Points) p.Rotate(p0, angleRad);
        }
        public void Scale(CadPoint p0, double mx, double my)
        {
            foreach (var p in Points) p.Magnify(p0, mx, my);
        }

    }
}
