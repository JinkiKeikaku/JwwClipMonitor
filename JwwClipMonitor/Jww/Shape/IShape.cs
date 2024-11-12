using CadMath2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    interface IShape
    {
        void OnDraw(Graphics g, DrawContext d);
        CadRect GetExtent();
    }
}
