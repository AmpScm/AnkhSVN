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
using System.Diagnostics;
using Ankh.VS;

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
                IAnkhWebBrowser wb = GetService<IAnkhWebBrowser>();
                AnkhBrowserArgs ba = new AnkhBrowserArgs();
                ba.External = true;

                wb.Navigate(ub.Uri, ba);
            }
            catch (Exception e)
            {
                IAnkhErrorHandler r = GetService<IAnkhErrorHandler>();

                if (r != null)
                    r.OnError(e);
                else
                    throw;
            }
        }

        #endregion
    }
}
