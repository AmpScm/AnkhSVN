// $Id$
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.Selection;
using Ankh.UI;
using Ankh.VS;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for the DiffLocalItem and CreatePatch commands
    /// </summary>
    public abstract class LocalDiffCommandBase : CommandBase
    {
        readonly TempFileCollection _tempFileCollection = new TempFileCollection();

        /// <summary>
        /// Gets the temp file collection.
        /// </summary>
        /// <value>The temp file collection.</value>
        protected TempFileCollection TempFileCollection
        {
            get { return _tempFileCollection; }
        }        

        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection)
        {
            return GetDiff(
                context, 
                selection, 
                null);
        }
        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection, SvnRevisionRange revisions)
        {
            return GetDiff(
                context, 
                selection, 
                revisions, 
                delegate(SvnItem item) 
                { 
                    return item.IsVersioned; 
                });
        }
        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection, SvnRevisionRange revisions, Predicate<SvnItem> visibleFilter)
        {
            if (selection == null)
                throw new ArgumentNullException("selection");
            else if (context == null)
                throw new ArgumentNullException("context");

            IUIShell uiShell = context.GetService<IUIShell>();

            bool foundModified = false;
            foreach (SvnItem item in selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified || item.IsDocumentDirty)
                {
                    foundModified = true;
                    break; // no need (yet) to keep searching
                }
            }

            List<SvnItem> resources = new List<SvnItem>(selection.GetSelectedSvnItems(true));

            PathSelectorInfo info = new PathSelectorInfo("Select items for diffing", selection.GetSelectedSvnItems(true));
            info.VisibleFilter += visibleFilter;
            if (foundModified)
                info.CheckedFilter += delegate(SvnItem item) { return item.IsFile && (item.IsModified || item.IsDocumentDirty); };

            info.RevisionStart = revisions == null ? SvnRevision.Base : revisions.StartRevision;
            info.RevisionEnd = revisions == null ? SvnRevision.Working : revisions.EndRevision;

            PathSelectorResult result;
            // should we show the path selector?
            if (!CommandBase.Shift && (revisions == null || !foundModified))
            {
                result = uiShell.ShowPathSelector(info);
                if (!result.Succeeded)
                    return null;

                if (info == null)
                    return null;
            }
            else
                result = info.DefaultResult;

            if (!result.Succeeded)
                return null;

            SaveAllDirtyDocuments(selection, context);

            return DoExternalDiff(context, result, selection);
        }

        private string DoExternalDiff(IAnkhServiceProvider context, PathSelectorResult info, ISelectionContext selection)
        {
            foreach (SvnItem item in info.Selection)
            {
                // skip unmodified for a diff against the textbase
                if (info.RevisionStart == SvnRevision.Base &&
                    info.RevisionEnd == SvnRevision.Working && !item.IsModified)
                    continue;

                string tempDir = context.GetService<IAnkhTempDirManager>().GetTempDir();

                AnkhDiffArgs da = new AnkhDiffArgs();

                da.BaseFile = GetPath(context, info.RevisionStart, item, selection, tempDir);
                da.MineFile = GetPath(context, info.RevisionEnd, item, selection, tempDir);


                context.GetService<IAnkhDiffHandler>().RunDiff(da);
            }

            return null;
        }

        private string GetPath(IAnkhServiceProvider context, SvnRevision revision, SvnItem item, ISelectionContext selection, string tempDir)
        {
            if (revision == SvnRevision.Working)
            {
                return item.FullPath;
            }

            string strRevision;
            if (revision.RevisionType == SvnRevisionType.Time)
                strRevision = revision.Time.ToLocalTime().ToString("yyyyMMdd_hhmmss");
            else
                strRevision = revision.ToString();
            string tempFile = Path.GetFileNameWithoutExtension(item.Name) + "." + strRevision + Path.GetExtension(item.Name);
            tempFile = Path.Combine(tempDir, tempFile);
            // we need to get it from the repos
            context.GetService<IProgressRunner>().Run("Retrieving file for diffing", delegate(object o, ProgressWorkerArgs ee)
            { 
                SvnTarget target;

                switch(revision.RevisionType)
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
