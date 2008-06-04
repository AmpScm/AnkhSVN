// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;


using SharpSvn;
using Ankh.Ids;
using Ankh.VS;

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
            
            using (SwitchDialog dlg = new SwitchDialog())
            {
                dlg.Context = e.Context;

                dlg.LocalPath = path;
                dlg.SwitchToUri = uri;                

                dlg.ShowDialog(e.Context);
            }

            IContext context = e.Context.GetService<IContext>();

            /*SaveAllDirtyDocuments(e.Selection, context);

            IList resources = context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback(SvnItem.VersionedFilter));

            if (resources.Count == 0)
                return;

            SwitchDialogInfo info = new SwitchDialogInfo(resources,
                new object[] { resources[0] });

            info = context.UIShell.ShowSwitchDialog(info);

            if (info == null)
                return;

            using (context.StartOperation("Switching"))
            {
                SwitchRunner runner = new SwitchRunner(info.Path, new Uri(info.SwitchToUrl),
                    info.RevisionStart, info.Depth);
                context.UIShell.RunWithProgressDialog(runner, "Switching");
            }*/
        }

        /// <summary>
        /// A progress runner that runs the switch operation.
        /// </summary>
        private class SwitchRunner
        {
            public SwitchRunner(string path, Uri url, SvnRevision revision,
                SvnDepth depth)
            {
                this.path = path;
                this.url = url;
                this.revision = revision;
                this.depth = depth;
            }

            public void Work(object sender, ProgressWorkerArgs e)
            {
                SvnSwitchArgs args = new SvnSwitchArgs();
                args.Revision = revision;
                args.Depth = depth;
                e.Client.Switch(this.path, this.url, args);
            }

            private string path;
            private Uri url;
            private SvnRevision revision;
            private SvnDepth depth;
        }
    }
}