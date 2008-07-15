using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemOpenFolderInWorkingCopyExplorer)]
    [Command(AnkhCommand.ItemOpenFolderInRepositoryExplorer)]
    class OpenInXExplorer : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.ItemOpenFolderInWorkingCopyExplorer)
                e.Enabled = e.Visible = false;
            else
            {
                SvnItem parent = null;
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    SvnItem p;
                    if (item.IsDirectory)
                        p = item;
                    else
                        p = item.Parent;

                    if(parent == null)
                        parent = p;
                    else if (parent != null && parent != p)
                    {
                        parent = null;
                        break;
                    }
                }

                if (parent == null)
                    e.Enabled = e.Visible = false;
            }
        }
        public override void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.ItemOpenFolderInWorkingCopyExplorer)
                return;

            SvnItem parent = null;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                SvnItem p;
                if (item.IsDirectory)
                    p = item;
                else
                    p = item.Parent;

                if (parent == null)
                    parent = p;
                else if (parent != null && parent != p)
                {
                    parent = null;
                    break;
                }
            }

            if (parent == null)
                return;

            IAnkhCommandService cmd = e.GetService<IAnkhCommandService>();
            if (cmd.DirectlyExecCommand(AnkhCommand.ShowRepositoryExplorer))
                cmd.DirectlyExecCommand(AnkhCommand.AddRepositoryRoot, parent.Status.Uri);
        }
    }
}
