// $Id$
using System.IO;
using Ankh.UI;
using EnvDTE;
using AnkhSvn.Ids;
using Ankh.VS;
using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Commands
{
    /// <summary>
    /// Shows differences compared to local text base.
    /// </summary>
    [Command(AnkhCommand.DiffLocalItem)]
    public class DiffLocalItem : LocalDiffCommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments(e.Selection, context);

            using (context.StartOperation("Diffing"))
            {
                string diff = this.GetDiff(e.Selection, context);
                if (diff != null)
                {
                    // convert it to HTML and store in a temp file
                    DiffHtmlModel model = new DiffHtmlModel(diff);
                    string html = model.GetHtml();
                    string htmlFile = Path.GetTempFileName();
                    using (StreamWriter w = new StreamWriter(htmlFile, false, System.Text.Encoding.Default))
                        w.Write(html);

                    IAnkhWebBrowser browser = e.Context.GetService<IAnkhWebBrowser>();
                    BrowserArgs args = new BrowserArgs();
                    args.CreateFlags = __VSCREATEWEBBROWSER.VSCWB_AutoShow | 
                        __VSCREATEWEBBROWSER.VSCWB_NoHistory | 
                        __VSCREATEWEBBROWSER.VSCWB_StartCustom |
                        __VSCREATEWEBBROWSER.VSCWB_OptionDisableStatusBar;
                    args.BaseCaption = "Subversion";
                    browser.Navigate(new Uri("file:///" + htmlFile), args);
                }
            }
        }


        #endregion
    }
}