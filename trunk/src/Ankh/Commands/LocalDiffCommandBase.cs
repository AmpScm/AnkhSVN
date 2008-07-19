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

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetExe(IAnkhServiceProvider context, ISelectionContext selection)
        {
            IAnkhConfigurationService cs = context.GetService<IAnkhConfigurationService>();
            
            if (!cs.Instance.ChooseDiffMergeManual)
                return cs.Instance.DiffExePath;
            else
                return null;
        }

        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection)
        {
            return GetDiff(context, selection, null);
        }
        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(IAnkhServiceProvider context, ISelectionContext selection, SvnRevisionRange revisions)
        {
            if (selection == null)
                throw new ArgumentNullException("selection");
            else if (context == null)
                throw new ArgumentNullException("context");

            IUIShell uiShell = context.GetService<IUIShell>();

            bool useExternalDiff = GetExe(context, selection) != null;

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
            info.VisibleFilter += delegate(SvnItem item) { return true; };
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
            if (slndir != null)
                args.RelativeToPath = slndir;
            SvnRevisionRange range = new SvnRevisionRange(info.RevisionStart, info.RevisionEnd);

            using (MemoryStream stream = new MemoryStream())
            using (StreamReader reader = new StreamReader(stream))
            using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
            {
                foreach (SvnItem item in info.Selection)
                {
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

                string quotedLeftPath = GetPath(context, info.RevisionStart, item, selection);
                string quotedRightPath = GetPath(context, info.RevisionEnd, item, selection);
                string diffString = this.GetExe(context, selection);
                diffString = diffString.Replace("%base", quotedLeftPath);
                diffString = diffString.Replace("%mine", quotedRightPath);

                // We must split the line in program and arguments before running
                diffString = diffString.TrimStart();
                string program;
                string args;

                if (diffString.Length > 0 && diffString[0] == '\"')
                {
                    int n = diffString.IndexOf('\"', 1);

                    if (n > 0)
                    {
                        program = diffString.Substring(1, n - 1).Trim();
                        args = diffString.Substring(n + 1).TrimStart();
                    }
                    else
                    {
                        program = diffString;
                        args = "";
                    }
                }
                else
                {
                    char[] spacers = new char[] { ' ', '\t' };

                    int n = diffString.IndexOfAny(spacers);
                    program = "";
                    args = "";

                    // We use the algorithm as documented by CreateProcess() in MSDN
                    // http://msdn2.microsoft.com/en-us/library/ms682425(VS.85).aspx
                    while (n >= 0)
                    {
                        program = diffString.Substring(0, n);

                        if (File.Exists(program))
                        {
                            args = diffString.Substring(n + 1).TrimStart();
                            break;
                        }
                        else
                            n = diffString.IndexOfAny(spacers, n + 1);
                    }

                    if (n < 0)
                    {
                        program = diffString.Trim();
                    }
                }

                Process.Start(program, args);
            }

            return null;
        }

        private string GetPath(IAnkhServiceProvider context, SvnRevision revision, SvnItem item, ISelectionContext selection)
        {
            // is it local?
            if (revision == SvnRevision.Base)
            {
                if (item.Status.LocalContentStatus == SvnStatus.Added)
                {
                    string empty = Path.GetTempFileName();
                    TempFileCollection.AddFile(empty, false);
                    File.Create(empty).Close();
                    return empty;
                }
                else
                {
                    // BH: We should use Export here instead.. This will give us keyword expansion for free
                    using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
                    {
                        SvnWorkingCopyState result;
                        client.GetWorkingCopyState(item.FullPath, out result);
                        return result.WorkingCopyBasePath;
                    }
                }

            }
            else if (revision == SvnRevision.Working)
            {
                return item.FullPath;
            }

            string tempFile = context.GetService<IAnkhTempFileManager>().GetTempFile(Path.GetExtension(item.FullPath));
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
