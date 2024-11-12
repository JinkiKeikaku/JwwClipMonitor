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

        public DotShape(StyleConverter sc, JwwTen s)
        {
            P0.Set(s.m_start_x, s.m_start_y);
            DotStyle.Set(sc, s);
        }

        public CadRect GetExtent()
        {
            return new CadRect(P0, Radius);
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var p1 = d.DocToCanvas(P0);
            var r = d.DocToCanvas(Radius);
            DotStyle.DrawDot(g, d, p1, r);
        }




    }
}
