using System.Diagnostics;
using System.Reflection;

namespace JwwClipMonitor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            SelectDll();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static void SelectDll()
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyDir == null) return;
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                if (e.Name.StartsWith("JwwHelper,", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = Path.Combine(assemblyDir,
                    string.Format("JwwHelper_{0}.dll", (IntPtr.Size == 4) ? "x86" : "x64"));
                    Debug.WriteLine($"LoadAssembly::{fileName}");
                    return Assembly.LoadFile(fileName);
                }
                return null;
            };
        }

    }
}