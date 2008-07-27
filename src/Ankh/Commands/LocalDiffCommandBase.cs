// $Id$
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using SharpSvn;
using Ankh.Selection;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ankh.Scc;
using System.CodeDom.Compiler;
using Ankh.VS;
using Ankh.Scc.UI;

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
            return GetDiff(context, selection, null, false);
        }
        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection, SvnRevisionRange revisions, bool forceExternal)
        {
            if (selection == null)
                throw new ArgumentNullException("selection");
            else if (context == null)
                throw new ArgumentNullException("context");

            IUIShell uiShell = context.GetService<IUIShell>();

            bool useExternalDiff = true;

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
            info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned; };
            if (foundModified)
                info.CheckedFilter += delegate(SvnItem item) { return item.IsFile && (item.IsModified || item.IsDocumentDirty); };

            info.RevisionStart = revisions == null ? SvnRevision.Base : revisions.StartRevision;
            info.RevisionEnd = revisions == null ? SvnRevision.Working : revisions.EndRevision;

            // "Recursive" doesn't make much sense if using an external diff
            info.EnableRecursive = !useExternalDiff;
            info.Depth = useExternalDiff ? SvnDepth.Empty : SvnDepth.Infinity;

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

            if (useExternalDiff)
            {
                return DoExternalDiff(context, result, selection);
            }
            else
            {
                return DoInternalDiff(context, result, selection);
            }
        }

        private string DoInternalDiff(IAnkhServiceProvider context, PathSelectorResult info, ISelectionContext selection)
        {
            Ankh.VS.IAnkhSolutionSettings ss = context.GetService<Ankh.VS.IAnkhSolutionSettings>();
            string slndir = ss.ProjectRoot;

            SvnDiffArgs args = new SvnDiffArgs();
            args.IgnoreAncestry = true;
            args.NoDeleted = false;
            args.Depth = info.Depth;

            string slndirP = slndir + "\\";
            
            SvnRevisionRange range = new SvnRevisionRange(info.RevisionStart, info.RevisionEnd);

            using (MemoryStream stream = new MemoryStream())
            using (StreamReader reader = new StreamReader(stream))
            using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
            {
                foreach (SvnItem item in info.Selection)
                {
                    SvnWorkingCopy wc;
                    if (!string.IsNullOrEmpty(slndir) &&
                        item.FullPath.StartsWith(slndirP, StringComparison.OrdinalIgnoreCase))
                        args.RelativeToPath = slndir;
                    else if ((wc = item.WorkingCopy) != null)
                        args.RelativeToPath = wc.FullPath;
                    else
                        args.RelativeToPath = null;

                    client.Diff(item.FullPath, range, args, stream);
                }
                stream.Position = 0;

                return reader.ReadToEnd();
            }
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

            string tempFile = Path.GetFileNameWithoutExtension(item.Name) + "." + revision.ToString() + Path.GetExtension(item.Name);
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
