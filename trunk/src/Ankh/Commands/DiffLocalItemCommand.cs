// $Id$
using System.IO;
using Ankh.UI;
using Ankh.Ids;
using Ankh.VS;
using System;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;
using System.Collections.Generic;
using Ankh.Configuration;

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
    [Command(AnkhCommand.ItemShowChanges)]
    public sealed class DiffLocalItem : LocalDiffCommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.DiffExternalLocalItem)
            {
                IAnkhConfigurationService cs = e.GetService<IAnkhConfigurationService>();

                AnkhConfig config = cs.Instance;

                // Allow external diff if enabled in config file
                if (!config.ChooseDiffMergeManual || config.DiffExePath == null)
                {
                    e.Enabled = false;
                    return;
                }
            }

            bool noConflictDiff = e.Command == AnkhCommand.ItemShowChanges;

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned && (item.Status.CombinedStatus != SvnStatus.Added || item.Status.IsCopied))
                {
                    if (e.Command == AnkhCommand.ItemCompareBase || e.Command == AnkhCommand.ItemShowChanges)
                    {
                        if (!(item.IsModified || item.IsDocumentDirty))
                            continue;
                    }

                    if (noConflictDiff && item.IsConflicted)
                    {
                        // Use advanced diff to get a diff, or 'Edit Conflict' to resolve it
                        continue;
                    }

                    return;
                }
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            List<string> selectedFiles = new List<string>();
            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.IsModified)
                    selectedFiles.Add(i.FullPath);
            }

            SvnRevisionRange revRange = null;
            bool forceExternal = false;
            switch (e.Command)
            {
                case AnkhCommand.DiffExternalLocalItem:
                    forceExternal = true;
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
            string diff = this.GetDiff(context, e.Selection, revRange, forceExternal);
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