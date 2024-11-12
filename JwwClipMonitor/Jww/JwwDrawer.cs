using CadMath2D;
using JwwClipMonitor.Jww.Shape;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww
{
    internal class JwwDrawer
    {
        public JwwDrawer() 
        {
        }

        public float Scale = 1.0f;

        public void OnDraw(Graphics g, Document doc)
        {
            var dc = new DrawContext(doc.Bounds, doc.BlockEntities);
            var p0 = doc.Bounds.TopLeft();
            var org = dc.DocToCanvas(p0);
            g.ScaleTransform(Scale, Scale);
            g.TranslateTransform(-org.X, -org.Y);
            foreach(var s in doc.Shapes)
            {
                s.OnDraw(g, dc);
            }
        }


    }
}
