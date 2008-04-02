// $Id$
using System.IO;
using Ankh.UI;
using EnvDTE;
using AnkhSvn.Ids;
using Ankh.VS;
using System;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Shows differences compared to local text base.
    /// </summary>
    [Command(AnkhCommand.DiffLocalItem)]
    [Command(AnkhCommand.ItemCompareBase)]
    [Command(AnkhCommand.ItemCompareCommitted)]
    [Command(AnkhCommand.ItemCompareHead)]
    [Command(AnkhCommand.ItemComparePrevious)]
    [Command(AnkhCommand.ItemCompareSpecific)]
    public class DiffLocalItem : LocalDiffCommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned && (item.Status.CombinedStatus != SvnStatus.Added || item.Status.IsCopied))
                    return;
            }
            e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments(e.Selection, context);

            using (context.StartOperation("Diffing"))
            {
                SvnRevisionRange revRange = null;
                switch (e.Command)
                {
                    case AnkhCommand.ItemCompareBase:
                        revRange = new SvnRevisionRange(SvnRevision.Base, SvnRevision.Working);
                        break;
                    case AnkhCommand.ItemCompareCommitted:
                        revRange = new SvnRevisionRange(SvnRevision.Committed, SvnRevision.Working);
                        break;
                    case AnkhCommand.ItemCompareHead:
                        revRange = new SvnRevisionRange(SvnRevision.Head, SvnRevision.Working);
                        break;
                    case AnkhCommand.ItemComparePrevious:
                        revRange = new SvnRevisionRange(SvnRevision.Previous, SvnRevision.Working);
                        break;
                }
                string diff = this.GetDiff(e.Selection, context, revRange);
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
    }
}