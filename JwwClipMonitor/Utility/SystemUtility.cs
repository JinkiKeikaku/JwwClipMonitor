using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JwwClipMonitor.Utility
{
    public static class SystemUtility
    {
        public static string GetAppVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            //バージョンの取得
            var ver = asm.GetName().Version!;
            return ver.ToString();
        }
        public static Font GetDefaultFont() => System.Drawing.SystemFonts.DefaultFont;
        public static string GetDefaultFontName() => GetDefaultFont().Name;
        internal static void OpenURL(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //Windowsのとき  
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c  \"{url}\"") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //Linuxのとき  
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    //Macのとき  
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
