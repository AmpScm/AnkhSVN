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

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for the DiffLocalItem and CreatePatch commands
    /// </summary>
    public abstract class LocalDiffCommandBase : CommandBase
    {
        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetExe(ISelectionContext selection, IContext context)
        {
            if (!context.Config.ChooseDiffMergeManual)
                return context.Config.DiffExePath;
            else
                return null;
        }

        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(ISelectionContext selection, IContext context)
        {
            bool useExternalDiff = GetExe(selection, context) != null;

            // We use VersionedFilter here to allow diffs between arbitrary revisions
            ArrayList checkedResources = new ArrayList();
            foreach (SvnItem item in selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned && item.IsFile)
                    checkedResources.Add(item);
            }

            IList resources = new ArrayList(checkedResources);

            // are we shifted?
            PathSelectorInfo info = new PathSelectorInfo("Select items for diffing",
                resources, checkedResources);
            info.RevisionStart = SvnRevision.Base;
            info.RevisionEnd = SvnRevision.Working;

            // "Recursive" doesn't make much sense if using an external diff
            info.EnableRecursive = !useExternalDiff;
            info.Depth = useExternalDiff ? SvnDepth.Empty : SvnDepth.Infinity;

            // default to textbase vs wc diff                
            SvnRevision revisionStart = SvnRevision.Base;
            SvnRevision revisionEnd = SvnRevision.Working;

            // should we show the path selector?
            if (!CommandBase.Shift && resources.Count != 1)
            {
                info = context.UIShell.ShowPathSelector(info);

                if (info == null)
                    return null;
            }

            if (useExternalDiff)
            {
                return DoExternalDiff(info, selection, context);
            }
            else
            {
                return DoInternalDiff(info, selection, context);
            }
        }

        private string DoInternalDiff(PathSelectorInfo info, ISelectionContext selection, IContext context)
        {
            string curdir = Environment.CurrentDirectory;

            // we go to the solution directory so that the diff paths will be relative 
            // to that directory
            string slndir = Path.GetDirectoryName(selection.SolutionFilename);

            try
            {
                // switch to the solution dir, so we can get relative paths.
                // if a solution isn't open, don't bother
                if (slndir != null)
                {
                    Environment.CurrentDirectory = slndir;
                }

                MemoryStream stream = new MemoryStream();
                using (SvnClient client = context.ClientPool.GetClient())
                {
                    foreach (SvnItem item in info.CheckedItems)
                    {
                        // try to get a relative path to the item from the solution directory
                        string path = null;

                        if (slndir != null)
                        {
                            path = Utils.Win32.Win32.PathRelativePathTo(slndir,
                                            Utils.Win32.FileAttribute.Directory, item.Path,
                                            Utils.Win32.FileAttribute.Normal);
                        }

                        // We can't use a path with more than two .. relative paths as input to svn diff (see svn issue #2448)
                        if (path == null || path.IndexOf(@"..\..\..") >= 0)
                        {
                            path = item.Path;
                        }

                        SvnDiffArgs args = new SvnDiffArgs();
                        args.IgnoreAncestry = true;
                        args.NoDeleted = false;
                        args.Depth = info.Depth;
                        client.Diff(path, new SvnRevisionRange(info.RevisionStart, info.RevisionEnd), args, stream);
                    }
                }

                return System.Text.Encoding.Default.GetString(stream.ToArray());
            }
            finally
            {
                Environment.CurrentDirectory = curdir;
            }

        }

        private string DoExternalDiff(PathSelectorInfo info, ISelectionContext selection, IContext context)
        {
            foreach (SvnItem item in info.CheckedItems)
            {
                // skip unmodified for a diff against the textbase
                if (info.RevisionStart == SvnRevision.Base &&
                    info.RevisionEnd == SvnRevision.Working && !item.IsModified)
                    continue;

                string quotedLeftPath = GetPath(info.RevisionStart, item, selection, context);
                string quotedRightPath = GetPath(info.RevisionEnd, item, selection, context);
                string diffString = this.GetExe(selection, context);
                diffString = diffString.Replace("%base", quotedLeftPath);
                diffString = diffString.Replace("%mine", quotedRightPath);

                // We can't use System.Diagnostics.Process here because we want to keep the
                // program path and arguments together, which it doesn't allow.
                Utils.Exec exec = new Utils.Exec();
                exec.ExecPath(diffString);
            }

            return null;
        }

        private string GetPath(SvnRevision revision, SvnItem item, ISelectionContext selection, IContext context)
        {
            // is it local?
            if (revision == SvnRevision.Base)
            {
                if (item.Status.LocalContentStatus == SvnStatus.Added)
                {
                    string empty = Path.GetTempFileName();
                    File.Create(empty).Close();
                    return empty;
                }
                else
                    using (SvnClient client = context.ClientPool.GetClient())
                    {
                        SvnWorkingCopyState result;
                        client.GetWorkingCopyState(item.Path, out result);
                        return result.WorkingCopyBasePath;
                    }
                
            }
            else if (revision == SvnRevision.Working)
            {
                return item.Path;
            }

            // we need to get it from the repos
            CatRunner runner = new CatRunner(revision, item.Status.Uri);
            context.UIShell.RunWithProgressDialog(runner, "Retrieving file for diffing");
            //			runner.Work( context );
            return runner.Path;
        }
    }
}
