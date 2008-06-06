// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;


using SharpSvn;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to switch current item to a different URL.
    /// </summary>
    [Command(AnkhCommand.SwitchItem)]
    [Command(AnkhCommand.SolutionSwitchDialog)]
    public class SwitchItemCommand : CommandBase
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

            if (e.Command == AnkhCommand.SolutionSwitchDialog)
                path = e.GetService<IAnkhSolutionSettings>().ProjectRoot;
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

            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();


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
                        a.Client.Switch(path, target);
                    });
                    
                lck.ReloadModified();
            }
        } 
    }
}