// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;

using SharpSvn;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Scc;
using Ankh.Selection;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to switch current item to a different URL.
    /// </summary>
    [Command(AnkhCommand.SwitchItem)]
    [Command(AnkhCommand.SolutionSwitchDialog)]
    [Command(AnkhCommand.SwitchProject)]
    class SwitchItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.SolutionSwitchDialog)
            {
                if (string.IsNullOrEmpty(e.Selection.SolutionFilename))
                    e.Enabled = false;
                return;
            }

            bool foundOne = false, error = false;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned && !foundOne)
                    foundOne = true;
                else
                {
                    error = true;
                    break;
                }
            }

            e.Enabled = foundOne && !error;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem theItem = null;
            string path;

            string projectRoot = e.GetService<IAnkhSolutionSettings>().ProjectRoot;

            if (e.Command == AnkhCommand.SolutionSwitchDialog)
                path = projectRoot;
            else if (e.Command == AnkhCommand.SwitchProject)
            {
                IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                path = null;

                foreach (SvnProject item in e.Selection.GetSelectedProjects(true))
                {
                    ISvnProjectInfo pi = mapper.GetProjectInfo(item);

                    if (pi == null)
                        continue;

                    path = pi.ProjectDirectory;
                    break;
                }

                if (string.IsNullOrEmpty(path))
                    return;
            }
            else
            {
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsVersioned)
                    {
                        theItem = item;
                        break;
                    }
                    return;
                }
                path = theItem.FullPath;
            }

            Uri uri;

            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                uri = cl.GetUriFromWorkingCopy(path);
            }

            SvnUriTarget target;

            if (e.Argument is string)
                target = SvnUriTarget.FromString((string)e.Argument);
            else
            {
                using (SwitchDialog dlg = new SwitchDialog())
                {
                    dlg.Context = e.Context;

                    dlg.LocalPath = path;
                    dlg.SwitchToUri = uri;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    target = dlg.SwitchToUri;
                }
            }

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            foreach (string file in documentTracker.GetDocumentsBelow(path))
            {
                if (!lockPaths.Contains(file))
                    lockPaths.Add(file);
            }

            documentTracker.SaveDocuments(lockPaths); // Make sure all files are saved before merging!

            using (DocumentLock lck = documentTracker.LockDocuments(lockPaths, DocumentLockType.NoReload))
            {
                lck.MonitorChanges();

                // TODO: Monitor conflicts!!

                e.GetService<IProgressRunner>().Run(
                    "Switching",
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnSwitchArgs args = new SvnSwitchArgs();

                        e.GetService<IConflictHandler>().RegisterConflictHandler(args, a.Synchronizer);                        
                        a.Client.Switch(path, target, args);
                    });


                // This fixes the PC 'Working on' combo 
                string solution = e.GetService<IAnkhSolutionSettings>().SolutionFilename;
                IFileStatusCache cache = e.GetService<IFileStatusCache>();

                if (!string.IsNullOrEmpty(solution))
                    cache.MarkDirty(solution);
                if (!string.IsNullOrEmpty(projectRoot))
                    cache.MarkDirty(projectRoot);
                // Working on fix


                lck.ReloadModified();
            }
        }        
    }
}