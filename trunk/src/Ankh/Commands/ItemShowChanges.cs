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
using Ankh.Scc.UI;
using Ankh.Scc;

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
    [Command(AnkhCommand.DiffExternalLocalItem)]
    public sealed class DiffLocalItem : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.DiffExternalLocalItem)
            {
                e.Enabled = false; // Unsupported for now
                return;
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
            List<SvnItem> selectedFiles = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.IsVersioned || (item.Status.CombinedStatus == SvnStatus.Added && !item.Status.IsCopied))
                    continue;

                if (e.Command == AnkhCommand.ItemCompareBase || e.Command == AnkhCommand.ItemShowChanges)
                {
                    if (!(item.IsModified || item.IsDocumentDirty))
                        continue;
                }

                selectedFiles.Add(item);
            }

            SvnRevisionRange revRange = null;
            switch (e.Command)
            {
                case AnkhCommand.DiffExternalLocalItem:
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

            if (selectedFiles.Count > 1 || revRange == null)
            {
                PathSelectorInfo info = new PathSelectorInfo("Select item for Diff", selectedFiles);
                info.SingleSelection = false;
                info.RevisionStart = revRange == null ? SvnRevision.Base : revRange.StartRevision;
                info.RevisionEnd = revRange == null ? SvnRevision.Working : revRange.EndRevision;

                info.EnableRecursive = false;
                info.Depth = SvnDepth.Infinity;

                PathSelectorResult result;
                // should we show the path selector?
                if (e.PromptUser && !CommandBase.Shift)
                {
                    IUIShell ui = e.GetService<IUIShell>();

                    result = ui.ShowPathSelector(info);
                    if (!result.Succeeded)
                        return;

                    if (info == null)
                        return;
                }
                else
                    result = info.DefaultResult;

                selectedFiles.Clear();
                selectedFiles.AddRange(result.Selection);
            }

            List<AnkhDiffArgs> diffArgs = new List<AnkhDiffArgs>();

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            string tempDir = e.GetService<IAnkhTempDirManager>().GetTempDir();

            if (revRange.EndRevision.RevisionType == SvnRevisionType.Working ||
                revRange.StartRevision.RevisionType == SvnRevisionType.Working)
            {
                // Save only the files needed

                IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();
                if (tracker != null)
                    tracker.SaveDocuments(SvnItem.GetPaths(selectedFiles));
            }

            foreach (SvnItem item in selectedFiles)
            {
                AnkhDiffArgs da = new AnkhDiffArgs();

                da.BaseFile = GetPath(e.Context, revRange.StartRevision, item, tempDir);
                da.MineFile = GetPath(e.Context, revRange.EndRevision, item, tempDir);

                da.BaseTitle = item.Name + " - " + revRange.StartRevision.ToString();
                da.MineTitle = item.Name + " - " + revRange.EndRevision.ToString();
                

                diff.RunDiff(da);
            }
        }

        private string GetPath(IAnkhServiceProvider context, SvnRevision revision, SvnItem item, string tempDir)
        {
            if (revision == SvnRevision.Working)
            {
                return item.FullPath;
            }

            string tempFile = Path.GetFileNameWithoutExtension(item.Name) + "." + revision.ToString() + Path.GetExtension(item.Name);
            tempFile = Path.Combine(tempDir, tempFile);
            // we need to get it from the repos
            context.GetService<IProgressRunner>().Run("Retrieving file for diffing", delegate(object o, ProgressWorkerArgs ee)
            {
                SvnTarget target;

                switch (revision.RevisionType)
                {
                    case SvnRevisionType.Head:
                    case SvnRevisionType.Number:
                    case SvnRevisionType.Time:
                        target = new SvnUriTarget(item.Status.Uri);
                        break;
                    default:
                        target = new SvnPathTarget(item.FullPath);
                        break;
                }
                SvnWriteArgs args = new SvnWriteArgs();
                args.Revision = revision;
                args.SvnError += delegate(object sender, SvnErrorEventArgs eea)
                {
                    if (eea.Exception is SvnClientUnrelatedResourcesException)
                        eea.Cancel = true;
                };

                using (FileStream stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    ee.Client.Write(target, stream, args);
                }
            });

            return tempFile;
        }
    }
}