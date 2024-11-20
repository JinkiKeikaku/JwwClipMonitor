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
        private const int SW_RESTORE = 9;  // ��ʂ����̑傫���ɖ߂�    

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
                        // �����̃v���Z�X�̃��C���E�B���h�E��O�ʂɕ\��
                        var hWnd = previousProcess.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            // �ŏ�������Ă���ꍇ�͌��̃T�C�Y�ɖ߂�
                            if (IsIconic(hWnd)) ShowWindowAsync(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        else
                        {
                            MessageBox.Show("���Ɏ��s���̃E�B���h�E��������܂���ł����B");
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
                        // �A�N�Z�X�������Ȃ��ꍇ�͖���
                        continue;
                    }

                    // �v���Z�X�̃t���p�X�����r���ē����A�v���P�[�V����������
                    if (String.Compare(checkPath, currentPath, true) == 0) return checkProcess;
                }
            }
            return null;
        }
    }
}