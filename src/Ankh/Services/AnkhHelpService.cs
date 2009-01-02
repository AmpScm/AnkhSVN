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
            ub.Query = string.Format("t=dlgHelp&v={0}&l={1}&t={2}", GetService<IAnkhPackage>().UIVersion, CultureInfo.CurrentUICulture.LCID, Uri.EscapeUriString(form.DialogHelpTypeName));

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
