// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.VS;

namespace Ankh.UI
{
    public class AnkhMessageBox : AnkhService
    {
        public AnkhMessageBox(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public DialogResult Show(string text)
        {
            return Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }
        public DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }
        public DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }
        public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0);
        }
        public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Show(text, caption, buttons, icon, defaultButton, 0);
        }
        public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            else if (caption == null)
                throw new ArgumentNullException("caption");
            IVsUIShell shell = GetService<IVsUIShell>(typeof(SVsUIShell));

            if (shell == null)
                return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);

            OLEMSGBUTTON oleButton = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    break;
                case MessageBoxButtons.OKCancel:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_ABORTRETRYIGNORE;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
                    break;
                case MessageBoxButtons.YesNo:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                    break;
                case MessageBoxButtons.RetryCancel:
                    oleButton = OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("buttons");
            }
            OLEMSGDEFBUTTON oleDefButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            switch (defaultButton)
            {
                case MessageBoxDefaultButton.Button1:
                    oleDefButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    break;
                case MessageBoxDefaultButton.Button2:
                    oleDefButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND;
                    break;
                case MessageBoxDefaultButton.Button3:
                    oleDefButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_THIRD;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("defaultButton");
            }

            OLEMSGICON oleIcon = OLEMSGICON.OLEMSGICON_NOICON;
            switch (icon)
            {
                // Many more values are handles, as they are all aliases of each other
                case MessageBoxIcon.None:
                    oleIcon = OLEMSGICON.OLEMSGICON_NOICON;
                    break;
                case MessageBoxIcon.Error:
                    oleIcon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    break;
                case MessageBoxIcon.Question:
                    oleIcon = OLEMSGICON.OLEMSGICON_QUERY;
                    break;
                case MessageBoxIcon.Warning:
                    oleIcon = OLEMSGICON.OLEMSGICON_WARNING;
                    break;
                case MessageBoxIcon.Information:
                    oleIcon = OLEMSGICON.OLEMSGICON_INFO;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("icon");
            }

            Guid g0 = Guid.Empty;
            int pnResult;
            if (0 != shell.ShowMessageBox(0, ref g0, caption, text, null, 0, oleButton, 
                oleDefButton, oleIcon, 
                ((options & MessageBoxOptions.ServiceNotification) != 0) ? 1 : 0, out pnResult))
            {
                return DialogResult.Cancel;
            }

            switch (pnResult)
            {
                case 1: // IDOK
                    return DialogResult.OK;
                case 2: // IDCANCEL
                    return DialogResult.Cancel;
                case 3: // IDABORT
                    return DialogResult.Abort;
                case 4: // IDRETRY
                    return DialogResult.Retry;
                case 5:
                    return DialogResult.Ignore;
                case 6:
                    return DialogResult.Yes;
                case 7:
                    return DialogResult.No;
                default:
                    return DialogResult.None;
            }
        }
    }
}
