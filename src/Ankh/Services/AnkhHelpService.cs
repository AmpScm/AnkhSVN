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
using Ankh.UI;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace Ankh.Services
{
    [GlobalService(typeof(IAnkhDialogHelpService))]
    class AnkhHelpService : AnkhService, IAnkhDialogHelpService
    {
        public AnkhHelpService(IAnkhServiceProvider context)
            : base(context)
        {
        }
        #region IAnkhDialogHelpService Members

        public void RunHelp(VSDialogForm form)
        {
            UriBuilder ub = new UriBuilder("http://svc.ankhsvn.net/svc/go/");
            ub.Query = string.Format("t=dlgHelp&v={0}&l={1}&dt={2}", GetService<IAnkhPackage>().UIVersion, CultureInfo.CurrentUICulture.LCID, Uri.EscapeUriString(form.DialogHelpTypeName));

            try
            {
                bool showHelpInBrowser = true;
                IVsHelpSystem help = GetService<IVsHelpSystem>(typeof(SVsHelpService));
                if (help != null)
                    showHelpInBrowser = !ErrorHandler.Succeeded(help.DisplayTopicFromURL(ub.Uri.AbsoluteUri, (uint)VHS_COMMAND.VHS_Default));

                if (showHelpInBrowser)
                    Help.ShowHelp(form, ub.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion
    }
}
