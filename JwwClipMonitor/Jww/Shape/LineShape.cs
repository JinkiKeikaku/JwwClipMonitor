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

        public LineShape(StyleConverter sc, JwwSen s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            P1.Set(s.m_end_x, s.m_end_y);
            LineStyle.Set(sc, s);
        }

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
    }
}
