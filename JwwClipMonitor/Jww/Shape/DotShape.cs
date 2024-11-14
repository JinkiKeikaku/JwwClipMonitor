using CadMath2D;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    internal class DotShape:IShape
    {
        public CadPoint P0 = new();
        public double Radius = 0.75;
        public DotStyle DotStyle = new();

        public DotShape(CadPoint p0, double radius, DotStyle dotStyle)
        {
            P0.Set(p0);
            Radius = radius;
            DotStyle.Set(dotStyle);
        }

        public DotShape(StyleConverter sc, JwwTen s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            DotStyle.Set(sc, s);
        }

        public IShape Clone() => new DotShape(P0, Radius, DotStyle);

        public CadRect GetExtent()
        {
            var r = new CadRect(P0, Radius);
            r.Inflate(DotStyle.Width, DotStyle.Width);
            return r;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var p1 = d.DocToCanvas(P0);
            var r = d.DocToCanvas(Radius);
            DotStyle.DrawDot(g, d, p1, r);
        }

        public void Offset(double dx, double dy)
        {
            P0.Offset(dx, dy);
        }
        public void Rotate(CadPoint p0, double angleRad)
        {
            P0.Rotate(p0, angleRad);
        }
        public void Scale(CadPoint p0, double mx, double my)
        {
            P0.Magnify(p0, mx, my);
        }

    }
}
