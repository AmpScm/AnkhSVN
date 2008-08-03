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
                e.Enabled = false;
            else if (e.Command == AnkhCommand.ItemOpenFolderInRepositoryExplorer && parent.Status.Uri != null)
            { }
            else if (e.Command == AnkhCommand.ItemOpenFolderInWorkingCopyExplorer && parent.Exists)
            { }
            else
                e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
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

            IAnkhCommandService cmd = e.GetService<IAnkhCommandService>();
            switch (e.Command)
            {
                case AnkhCommand.ItemOpenFolderInRepositoryExplorer:
                    if (parent == null || parent.Status.Uri == null)
                        return;

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.RepositoryBrowse, parent.Status.Uri);
                    break;
                case AnkhCommand.ItemOpenFolderInWorkingCopyExplorer:
                    if (parent == null || !parent.Exists)
                        return;

                    if (cmd != null)
                        cmd.DirectlyExecCommand(AnkhCommand.WorkingCopyBrowse, parent.FullPath);
                    break;
            }
        }
    }
}
