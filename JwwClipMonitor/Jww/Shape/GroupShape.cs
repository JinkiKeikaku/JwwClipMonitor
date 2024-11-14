using CadMath2D;
using CadMath2D.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    internal class GroupShape : IShape
    {
        public List<IShape> Shapes=new();
        
        public void Add(IShape s) { Shapes.Add(s); }

        public IShape Clone()
        {
            var g = new GroupShape();
            foreach(var s in Shapes) { g.Shapes.Add(s.Clone()); }
            return g;
        }

        public CadRect GetExtent()
        {
            if(Shapes.Count == 0) return new CadRect();
            var r = Shapes[0].GetExtent();
            for(var i = 0; i < Shapes.Count; i++) r.Spread(Shapes[i].GetExtent());
            return r;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            foreach(var shape in Shapes) shape.OnDraw(g, d);
        }
        public void Offset(double dx, double dy)
        {
            foreach (var shape in Shapes) shape.Offset(dx, dy);
        }
        public void Rotate(CadPoint p0, double angleRad)
        {
            foreach (var shape in Shapes) shape.Rotate(p0, angleRad);
        }
        public void Scale(CadPoint p0, double mx, double my)
        {
            foreach (var shape in Shapes) shape.Scale(p0, mx, my);
        }

    }
}
