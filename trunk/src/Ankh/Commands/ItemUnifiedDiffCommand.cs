using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Ids;
using Ankh.UI;
using System.IO;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Commands
{
    class ItemUnifiedDiffCommand : LocalDiffCommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            List<string> selectedFiles = new List<string>();
            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.IsModified)
                    selectedFiles.Add(i.FullPath);
            }

            SvnRevisionRange revRange = null;
            //bool forceExternal = false;
            switch (e.Command)
            {
                case AnkhCommand.DiffExternalLocalItem:
                    //forceExternal = true;
                    goto case AnkhCommand.DiffLocalItem;
                case AnkhCommand.DiffLocalItem:
                case AnkhCommand.ItemCompareBase:
                case AnkhCommand.ItemShowChanges:
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
            string diff = this.GetDiff(e.Context, e.Selection, revRange, false);
            if (diff != null)
            {
                // convert it to HTML and store in a temp file
                DiffHtmlModel model = new DiffHtmlModel(diff);
                string html = model.GetHtml();
                string htmlFile = Path.GetTempFileName();
                TempFileCollection.AddFile(htmlFile, false);
                using (StreamWriter w = new StreamWriter(htmlFile, false, System.Text.Encoding.Default))
                    w.Write(html);

                IAnkhWebBrowser browser = e.Context.GetService<IAnkhWebBrowser>();
                BrowserArgs args = new BrowserArgs();
                args.CreateFlags = __VSCREATEWEBBROWSER.VSCWB_AutoShow |
                    __VSCREATEWEBBROWSER.VSCWB_NoHistory |
                    __VSCREATEWEBBROWSER.VSCWB_StartCustom |
                    __VSCREATEWEBBROWSER.VSCWB_OptionDisableStatusBar;
                args.BaseCaption = selectedFiles.Count == 1 ? Path.GetFileName(selectedFiles[0]) : "Subversion";
                browser.Navigate(new Uri("file:///" + htmlFile), args);
            }
        }
    }
}
