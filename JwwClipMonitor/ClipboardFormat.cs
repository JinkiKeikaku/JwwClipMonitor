using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor
{
    public class ClipboardFormat
    {
        public string FormatName;
        public int FormatId;
        public bool ToJww;
        public ClipboardFormat(string formatName, int formatid, bool toJww)
        {
            FormatName = formatName;
            FormatId = formatid;
            ToJww = toJww;
        }

        public override string ToString() => FormatName;
    }
}
