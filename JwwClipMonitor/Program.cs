using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JwwClipMonitor
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す    

        private static Mutex mutex = null!;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            mutex = new Mutex(true, "JwwClipMonitor", out var createdNew);
            try
            {
                if (!createdNew)
                {
                    var previousProcess = GetPreviousProcess();
                    if (previousProcess != null)
                    {
                        // 既存のプロセスのメインウィンドウを前面に表示
                        var hWnd = previousProcess.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            // 最小化されている場合は元のサイズに戻す
                            if (IsIconic(hWnd)) ShowWindowAsync(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        else
                        {
                            MessageBox.Show("既に実行中のウィンドウが見つかりませんでした。");
                        }
                    }
                    return;
                }
                SelectDll();
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
            finally
            {
                if (createdNew) mutex.ReleaseMutex();
            }
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

        static Process? GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            var currentPath = curProcess.MainModule?.FileName;
            if (currentPath == null) return null;
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (var checkProcess in allProcesses)
            {
                if (checkProcess.Id != curProcess.Id)
                {
                    string? checkPath;

                    try
                    {
                        checkPath = checkProcess.MainModule?.FileName;
                        if (checkPath == null) continue;
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        // アクセス権限がない場合は無視
                        continue;
                    }

                    // プロセスのフルパス名を比較して同じアプリケーションか検証
                    if (String.Compare(checkPath, currentPath, true) == 0) return checkProcess;
                }
            }
            return null;
        }
    }
}