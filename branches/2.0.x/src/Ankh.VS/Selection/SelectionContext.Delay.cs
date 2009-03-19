using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ankh.Commands;

namespace Ankh.VS.Selection
{
    sealed class DelayData
    {
        readonly IVsTextViewEx _textView;
        readonly IntPtr _windowHandle;        

        public DelayData(IVsTextViewEx textView)
        {
            if(textView == null)
                throw new ArgumentNullException("textView");
            _textView = textView;
        }

        public DelayData(IntPtr windowHandle)
        {
            if(windowHandle == IntPtr.Zero)
                throw new ArgumentNullException("windowHandle");

            _windowHandle = windowHandle;
        }

        public IVsTextViewEx TextView
        {
            get { return _textView; }
        }

        public IntPtr WindowHandle
        {
            get { return _windowHandle; }
        }
    }

    partial class SelectionContext
    {
        bool _delayedDirty;

        public void MaybeInstallDelayHandler()
        {
            if(_delayedDirty)
                return;

            DelayData dm = ShouldInstallDelayHandler();
            
            if(dm == null)
                return;

            InstallDelayHandler(dm);
        }

        DelayData ShouldInstallDelayHandler()
        {
            IVsWindowFrame frame = ActiveFrame;

            if (frame != null)
            {
                IVsTextViewEx textView = VsShellUtilities.GetTextView(frame) as IVsTextViewEx;

                if (textView != null)
                {
                    if (textView.IsCompletorWindowActive() == 0 || textView.IsExpansionUIActive() == 0)
                        return new DelayData(textView);
                }                                
            }

            IntPtr handle;

            if (IsEditBoxActive(out handle))
                return new DelayData(handle);

            return null;
        }

        private bool IsEditBoxActive(out IntPtr handle)
        {
            handle = IntPtr.Zero;

            IntPtr focus = NativeMethods.GetFocus();
            handle = focus;
            if (focus != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(32);
                NativeMethods.GetClassName(focus, sb, 32);

                string cls = sb.ToString();

                if (cls.StartsWith("WindowsForms"))
                {
                    Control c = Control.FromHandle(focus);

                    while (c != null)
                    {
                        if (c is TextBox)
                        {
                            if (c is DataGridViewTextBoxEditingControl)
                                return true; // At least true in the String Resource

                        }
                        else if (c is DataGridView)
                            return true;

                        c = c.Parent;
                    }

                    return false;
                }
                else
                    switch (cls)
                    {
                        //case "Edit":
                        //	return true;
                        // Filter what?
                        default:
                            break;
                    }
            }
            handle = IntPtr.Zero;
            return false;
        }

        void InstallDelayHandler(DelayData dd)
        {
            GetService<IAnkhCommandService>().DelayPostCommands(
                delegate
                {
                    bool cont;
                    if (dd.TextView != null)
                    {
                        cont = dd.TextView.IsCompletorWindowActive() == 0
                            || dd.TextView.IsExpansionUIActive() == 0;
                    }
                    else
                        cont = NativeMethods.GetFocus() == dd.WindowHandle;

                    if (!cont)
                    {
                        dd = ShouldInstallDelayHandler();

                        if (dd == null)
                        {
                            _delayedDirty = false;
                            return false;
                        }
                    }
                    
                    return true;
                });
        }
      
        static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetFocus();

            [DllImport("user32.dll")]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder sb, int nMaxCount);
        }
    }
}
