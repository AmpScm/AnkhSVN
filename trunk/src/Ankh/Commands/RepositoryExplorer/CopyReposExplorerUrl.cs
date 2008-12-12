// $Id$
using System;
using Ankh.UI;
using Clipboard = System.Windows.Forms.Clipboard;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.Scc;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to copy the URL of this item to the clipboard in Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.CopyReposExplorerUrl)]
    class CopyReposExplorerUrl : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            int n = 0;
            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                n++;
                if (n > 1 || i.Uri == null)
                {
                    e.Enabled = false;
                    return;
                }
            }
            if (n == 1)
                return;

            foreach(SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                n++;
                if(n > 1 || item.Status.Uri == null)
                {
                    e.Enabled = false;
                    return;
                }
            }

            if (n != 1)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (i.Uri != null)
                    Clipboard.SetText(i.Uri.AbsoluteUri);

                return;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.Status.Uri != null)
                    Clipboard.SetText(item.Status.Uri.AbsoluteUri);

                return;
            }
        }
    }
}
