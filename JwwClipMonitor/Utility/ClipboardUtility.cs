using System.Runtime.InteropServices;
using System.Text;

namespace JwwClipMonitor.Utility
{
    public static class ClipboardUtility
    {
        public const int CF_TEXT = 1;
        public const int CF_BITMAP = 2;
        public const int CF_DIB = 8;
        public const int CF_UNICODETEXT = 13;
        public const int CF_ENHMETAFILE = 14;

        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(int wFormat);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(int wFormat);
        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();
        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RegisterClipboardFormat(string lpszFormat);

        [DllImport("user32.dll")]
        public static extern int GetClipboardFormatName(uint format, StringBuilder lpszFormatName, int cchMaxCount);

        [DllImport("gdi32")]
        public static extern uint GetEnhMetaFileBits(IntPtr hemf, uint cbBuffer, byte[]? lpbBuffer);
        [DllImport("gdi32.dll")]
        public static extern int DeleteEnhMetaFile(IntPtr hemf);

        public static bool SetClipboardData(int formatId, byte[] data)
        {
            if (formatId == 0) return false;
            IntPtr h = Marshal.AllocHGlobal((int)data.Length);
            Marshal.Copy(data, 0, h, (int)data.Length);
            return ClipboardUtility.SetClipboardData(formatId, h) != IntPtr.Zero;
        }

        public static string GetClipboardFormatName(short format)
        {
            var buffer = new StringBuilder();
            for (int l = 256; ; l += 256)
            {
                var copied = GetClipboardFormatName(
                    (uint)format, buffer, l);
                if (Marshal.GetLastWin32Error() == 0) return buffer.ToString(0, copied);
                return null;
            }
        }
    }
}
