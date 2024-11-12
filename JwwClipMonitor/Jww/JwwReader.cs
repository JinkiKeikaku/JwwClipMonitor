using CadMath2D;
using JwwClipMonitor.Jww.Shape;
using JwwHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww
{
    internal class JwwReader
    {
        public Document Document = new();
        public void Read(MemoryStream ms)
        {
            Document = new();
            var jwr = new JwwHelper.JwwClipReader();
            jwr.Read(ms.ToArray(), Completed);
        }

        private void Completed(JwwClipReader clipReader)
        {
            var sc = new StyleConverter();
            foreach (var js in clipReader.DataList)
            {
                var s = Convert(sc, js);
                if (s != null)
                {
                    Document.Shapes.Add(s);
                }
            }
            if (Document.Shapes.Count > 0)
            {
                Document.Bounds.Set(Document.Shapes[0].GetExtent());
            }
            for (var i = 1; i < Document.Shapes.Count; i++)
            {
                Document.Bounds.Spread(Document.Shapes[i].GetExtent());
            }
        }
        private IShape? Convert(StyleConverter sc, JwwData js)
        {
            switch (js)
            {
                case JwwSen s:
                    return new LineShape(sc, s);
                case JwwEnko s:
                    return s.m_bZenEnFlg == 1 ? new CircleShape(sc, s) : new ArcShape(sc, s);
                case JwwTen s:
                    return new DotShape(sc, s);
                case JwwMoji s:
                {
                    return new TextShape(sc, s);
                }
                case JwwSolid s:
                {
                    if(s.m_nPenStyle < 101) return new SolidPolygon(sc, s);
                    return new SolidCircle(sc, s);
                }
                break;

            }
            return null;
        }
    }
}
