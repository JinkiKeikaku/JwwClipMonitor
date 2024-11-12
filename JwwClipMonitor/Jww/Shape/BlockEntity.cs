using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Jww.Shape
{
    /// <summary>
    /// ブロック図形の実態。必要最低限。
    /// </summary>
    class BlockEntity
    {
        public int ID { get; }
        public List<IShape> Shapes { get; } = new();
        public BlockEntity(int id) { ID = id; }
    }
}

