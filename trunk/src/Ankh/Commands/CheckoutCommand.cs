// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;

using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to checkout a Subversion repository.
    /// </summary>
    [Command(AnkhCommand.Checkout)]
    public class CheckoutCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (CheckoutDialog dlg = new CheckoutDialog())
            {
                if (dlg.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                    return;

                using (context.StartOperation("Checking out"))
                {
                    CheckoutRunner runner = new CheckoutRunner(
                        dlg.LocalPath, dlg.Revision, new Uri(dlg.Url), dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Empty);

                    e.GetService<IProgressRunner>().Run("Checking out", runner.Work);
                }
            }
        }

        #endregion
    }
}