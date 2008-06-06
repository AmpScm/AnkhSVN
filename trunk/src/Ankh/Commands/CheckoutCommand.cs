// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;

using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;

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
            IUIService ui = e.GetService<IUIService>();

            using (CheckoutDialog dlg = new CheckoutDialog())
            {
                if (ui.ShowDialog(dlg) != DialogResult.OK)
                    return;

                CheckoutRunner runner = new CheckoutRunner(
                        dlg.LocalPath, dlg.Revision, new Uri(dlg.Url), dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Empty);

                e.GetService<IProgressRunner>().Run("Checking out", runner.Work);

            }
        }

        #endregion
    }
}