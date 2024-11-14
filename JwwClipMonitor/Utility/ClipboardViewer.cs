using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace JwwClipMonitor.Utility
{
    internal class ClipboardViewer : NativeWindow
    {
        [DllImport("user32")]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32")]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030D;
        private IntPtr mNextHandle;
        private Action mAction;
        private Form mParent;
        public ClipboardViewer(Form f, Action action)
        {
            mParent = f;
            mAction = action;
            mParent.HandleCreated += new EventHandler(this.OnHandleCreated);
            mParent.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
        }

        internal void OnHandleCreated(object? sender, EventArgs e)
        {
            if (sender is Form form)
            {
                AssignHandle(form.Handle);
                mNextHandle = SetClipboardViewer(this.Handle);
            }
        }

        internal void OnHandleDestroyed(object? sender, EventArgs e)
        {
            ChangeClipboardChain(this.Handle, mNextHandle);
            ReleaseHandle();
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    mAction();
                    if ((int)mNextHandle != 0)
                        SendMessage(mNextHandle, msg.Msg, msg.WParam, msg.LParam);
                    break;

                // クリップボード・ビューア・チェーンが更新された
                case WM_CHANGECBCHAIN:
                    if (msg.WParam == mNextHandle)
                    {
                        mNextHandle = (IntPtr)msg.LParam;
                    }
                    else if ((int)mNextHandle != 0)
                        SendMessage(mNextHandle, msg.Msg, msg.WParam, msg.LParam);
                    break;
            }
            base.WndProc(ref msg);
        }
    }
}
