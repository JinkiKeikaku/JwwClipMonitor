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

        public ClipboardFormat(string formatName, int formatid)
        {
            FormatName = formatName;
            FormatId = formatid;
        }

        public override string ToString() => FormatName;
    }
}
