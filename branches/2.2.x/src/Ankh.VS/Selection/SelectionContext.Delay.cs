// $Id$
//
// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ankh.Commands;
using Microsoft.VisualStudio;

namespace Ankh.VS.Selection
{
    sealed class DelayData
    {
        readonly IVsTextViewEx _textView;
        readonly IntPtr _windowHandle;

        public DelayData(IVsTextViewEx textView)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            _textView = textView;
        }

        public DelayData(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
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
            if (_delayedDirty)
                return;

            DelayData dm = ShouldInstallDelayHandler();

            if (dm == null)
                return;

            InstallDelayHandler(dm);
        }

        DelayData ShouldInstallDelayHandler()
        {
            IVsWindowFrame frame = ActiveFrame;

            if (frame != null)
            {
                IVsTextViewEx textView = GetTextView(frame) as IVsTextViewEx;

                if (textView != null)
                {
                    if (textView.IsCompletorWindowActive() == 0 || textView.IsExpansionUIActive() == 0)
                        return new DelayData(textView);

                    return null;
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
                    for (Control c = Control.FromHandle(focus); c != null; c = c.Parent)
                    {
                        DataGridView dataGrid = c as DataGridView;
                        if (dataGrid != null)
                        {
                            DataGridViewCell cell = dataGrid.CurrentCell;
                            if (cell != null && cell.IsInEditMode)
                                return true;
                        }
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

        private IVsTextView GetTextView(IVsWindowFrame windowFrame)
        {
            if (windowFrame == null)
                throw new ArgumentException("windowFrame");

            try
            {
                object pvar;
                int hr = windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out pvar);
                if (!ErrorHandler.Succeeded(hr))
                    return null;

                IVsTextView textView = pvar as IVsTextView;
                if (textView == null)
                {
                    IVsCodeWindow codeWin = pvar as IVsCodeWindow;
                    if (codeWin != null)
                    {
                        hr = codeWin.GetPrimaryView(out textView);

                        if (!ErrorHandler.Succeeded(hr))
                            return null;
                    }
                }
                return textView;
            }
            catch /* Handle broken frame implementations that throw errors instead of 
                   * returning null or an error HResult */
            {
                return null;
            }
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
