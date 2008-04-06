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
            if (!context.Configuration.Instance.ChooseDiffMergeManual)
                return context.Configuration.Instance.DiffExePath;
            else
                return null;
        }

        protected virtual string GetDiff(ISelectionContext selection, IContext context)
        {
            return GetDiff(selection, context, null);
        }
        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff(ISelectionContext selection, IContext context, SvnRevisionRange revisions)
        {
            if (selection == null)
                throw new ArgumentNullException("selection");
            if (context == null)
                throw new ArgumentNullException("context");

            bool useExternalDiff = GetExe(selection, context) != null;

            Dictionary<string, SvnItem> items = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            foreach (SvnItem item in selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified && !items.ContainsKey(item.FullPath))
                {
                    items.Add(item.FullPath, item);
                }
            }

			List<SvnItem> resources = new List<SvnItem>(selection.GetSelectedSvnItems(true));


            Predicate<SvnItem> checkFilter;
            if (items.Count == 0)
            {
                // Always check something when there are no modified items to diff
                checkFilter = delegate { return true; };
            }
            else
            {
                checkFilter = delegate(SvnItem item)
                {
                    return item.IsFile && item.IsModified;
                };
            }

			PathSelectorInfo info = new PathSelectorInfo("Select items for diffing",
				resources, 
				checkFilter
				);

            info.RevisionStart = revisions == null ? SvnRevision.Base : revisions.StartRevision;
            info.RevisionEnd = revisions == null ? SvnRevision.Working : revisions.EndRevision;

            // "Recursive" doesn't make much sense if using an external diff
            info.EnableRecursive = !useExternalDiff;
            info.Depth = useExternalDiff ? SvnDepth.Empty : SvnDepth.Infinity;

            // should we show the path selector?
            if (!CommandBase.Shift && (revisions == null || items.Count != 1))
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
            Ankh.VS.IAnkhSolutionSettings ss = context.GetService<Ankh.VS.IAnkhSolutionSettings>();
            string slndir = ss.ProjectRoot;

            SvnDiffArgs args = new SvnDiffArgs();
            args.IgnoreAncestry = true;
            args.NoDeleted = false;
            args.Depth = info.Depth;
            if (slndir != null)
                args.RelativeToPath = slndir;
            SvnRevisionRange range = new SvnRevisionRange(info.RevisionStart, info.RevisionEnd);
            
            using(MemoryStream stream = new MemoryStream())
            using(StreamReader reader = new StreamReader(stream))
            using (SvnClient client = context.ClientPool.GetClient())
            {
                foreach (SvnItem item in info.CheckedItems)
                {
                    client.Diff(item.FullPath, range, args, stream);
                }
                stream.Position = 0;

                return reader.ReadToEnd();
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

                // We must split the line in program and arguments before running
                diffString = diffString.TrimStart();
                string program ;
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
                    while(n >= 0)
                    {
                        program = diffString.Substring(0, n);

                        if(File.Exists(program))
                        {
                            args = diffString.Substring(n + 1).TrimStart();
                            break;
                        }
                        else
                            n = diffString.IndexOfAny(spacers, n + 1);
                    }

                    if(n < 0)
                    {
                        program = diffString.Trim();
                    }
                }

                Process.Start(program, args);
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
                {
                    // BH: We should use Export here instead.. This will give us keyword expansion for free
                    using (SvnClient client = context.ClientPool.GetClient())
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

            // we need to get it from the repos
            CatRunner runner = new CatRunner(revision, item.Status.Uri);
            context.UIShell.RunWithProgressDialog(runner, "Retrieving file for diffing");
            //			runner.Work( context );
            return runner.Path;
        }
    }
}
