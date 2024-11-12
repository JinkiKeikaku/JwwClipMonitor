using CadMath2D;
using JwwClipMonitor.Jww.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww
{
    internal class Document
    {
        public List<BlockEntity> BlockEntities = new();
        public List<IShape> Shapes = new();
        public CadRect Bounds = new CadRect();

    }
}
