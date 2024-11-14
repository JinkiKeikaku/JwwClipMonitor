using CadMath2D;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JwwClipMonitor.Jww.Shape
{
    internal class LineShape : IShape
    {

        public CadPoint P0=new();
        public CadPoint P1 = new();
        public LineStyle LineStyle = new();

        public LineShape(CadPoint p0, CadPoint p1, LineStyle lineStyle)
        {
            P0.Set(p0);
            P1.Set(p1);
            LineStyle.Set(lineStyle);
        }

        public LineShape(StyleConverter sc, JwwSen s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            P1.Set(s.m_end_x, s.m_end_y);
            LineStyle.Set(sc, s);
        }

        public IShape Clone()=> new LineShape(P0, P1, LineStyle);

        public CadRect GetExtent()
        {
            var r = new CadRect(P0, P1);
            r.Inflate(LineStyle.Width, LineStyle.Width);
            return r;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var p1 = d.DocToCanvas(P0);
            var p2 = d.DocToCanvas(P1);
            LineStyle.DrawLine(g, d, p1, p2);
        }

        public void Offset(double dx, double dy)
        {
            P0.Offset(dx, dy);
            P1.Offset(dx, dy);
        }
        public void Rotate(CadPoint p0, double angleRad)
        {
            P0.Rotate(p0, angleRad);
            P1.Rotate(p0, angleRad);
        }
        public void Scale(CadPoint p0, double mx, double my)
        {
            P0.Magnify(p0, mx, my);
            P1.Magnify(p0, mx, my);
        }
    }
}
