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
