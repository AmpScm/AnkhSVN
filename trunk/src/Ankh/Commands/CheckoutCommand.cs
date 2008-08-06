// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;

using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to checkout a Subversion repository.
    /// </summary>
    [Command(AnkhCommand.Checkout)]
    class CheckoutCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool first = false;

            foreach (Ankh.Scc.ISvnRepositoryItem i in e.Selection.GetSelection<Ankh.Scc.ISvnRepositoryItem>())
            {
                if (first)
                {
                    e.Enabled = false;
                    return;
                }
                first = true;
            }

            if (!first)
                e.Enabled = false;
        }


        public override void OnExecute(CommandEventArgs e)
        {
            Uri uri = null;
            SharpSvn.SvnRevision rev = null;
            string name = null;

            foreach (Ankh.Scc.ISvnRepositoryItem i in e.Selection.GetSelection<Ankh.Scc.ISvnRepositoryItem>())
            {
                if (uri != null)
                    return;

                name = i.Name;
                uri = i.Uri;
                rev = i.Revision;               
            }

            Ankh.VS.IAnkhSolutionSettings ss = e.GetService<Ankh.VS.IAnkhSolutionSettings>();

            if (uri == null)
                return;


            IUIService ui = e.GetService<IUIService>();

            using (CheckoutDialog dlg = new CheckoutDialog())
            {
                dlg.Uri = uri;
                dlg.LocalPath = System.IO.Path.Combine(ss.NewProjectLocation, name);
                

                if (ui.ShowDialog(dlg) != DialogResult.OK)
                    return;

                e.GetService<IProgressRunner>().Run("Checking Out", 
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnCheckOutArgs args = new SvnCheckOutArgs();
                        args.Revision = dlg.Revision;
                        args.Depth = dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Children;

                        a.Client.CheckOut(dlg.Uri, dlg.LocalPath, args);
                    });
            }
        }
    }
}