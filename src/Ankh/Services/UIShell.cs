// $Id$
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

using SharpSvn;

using Ankh.UI;
using Ankh.VS;
using Ankh.UI.SccManagement;
using Ankh.UI.PathSelector;

namespace Ankh
{
    /// <summary>
    /// Summary description for UIShell.
    /// </summary>
    [GlobalService(typeof(IUIShell))]
    sealed class UIShell : AnkhService, IUIShell
    {
        public UIShell(IAnkhServiceProvider context)
            : base(context)
        {

        }        

        public void DisplayHtml(string caption, string html, bool reuse)
        {
            IAnkhWebBrowser browser = Context.GetService<IAnkhWebBrowser>();

            string htmlFile = Path.GetTempFileName();
            using (StreamWriter w = new StreamWriter(htmlFile, false, System.Text.Encoding.UTF8))
                w.Write(html);

            // have it show the html
            Uri url = new Uri("file://" + htmlFile);
            BrowserArgs args = new BrowserArgs();
            args.BaseCaption = caption;

            //if(reuse)
            // args.CreateFlags |= __VSCREATEWEBBROWSER.VSCWB_ReuseExisting;

            browser.Navigate(url, args);
        }

        public PathSelectorResult ShowPathSelector(PathSelectorInfo info)
        {
            IUIService uiService = GetService<IUIService>();

            using (PathSelector selector = new PathSelector(info))
            {
                selector.Context = Context;

                bool succeeded = uiService.ShowDialog(selector) == DialogResult.OK;
                PathSelectorResult result = new PathSelectorResult(succeeded, selector.CheckedItems);
                result.Depth = selector.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                result.RevisionStart = selector.RevisionStart;
                result.RevisionEnd = selector.RevisionEnd;
                return result;
            }
        }

        #region IUIShell Members

        public bool EditEnlistmentState(Ankh.Scc.EnlistmentState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            IUIService uiService = GetService<IUIService>();

            using (SccEditEnlistment editor = new SccEditEnlistment(state))
            {
                return editor.ShowDialog(Context) == DialogResult.OK;
            }
        }

        public Uri ShowAddRepositoryRootDialog()
        {
            using (AddRepositoryRootDialog dlg = new AddRepositoryRootDialog(Context))
            {
                IUIService ui = GetService<IUIService>();


                if (ui.ShowDialog(dlg) != DialogResult.OK || dlg.Uri == null)
                    return null;

                return dlg.Uri;
            }
        }

        public string ShowAddWorkingCopyExplorerRootDialog()
        {
            using (AddWorkingCopyExplorerRootDialog dlg = new AddWorkingCopyExplorerRootDialog())
            {
                IUIService ui = GetService<IUIService>();

                DialogResult dr;
                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();


                if (dr != DialogResult.OK || string.IsNullOrEmpty(dlg.NewRoot))
                    return null;

                return dlg.NewRoot;
            }
        }

        #endregion

    }
}
