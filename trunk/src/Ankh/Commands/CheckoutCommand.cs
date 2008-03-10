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
    [VSNetCommand(AnkhCommand.Checkout,
		"Checkout",
         Text = "Chec&kout a Repository...",
         Tooltip="Checkout a Subversion repository.", 
         Bitmap = ResourceBitmaps.CheckoutDirectory),
         VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class CheckoutCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            using (CheckoutDialog dlg = new CheckoutDialog())
            {
                if (dlg.ShowDialog(context.HostWindow) != DialogResult.OK)
                    return;

                using (context.StartOperation("Checking out"))
                {
                    CheckoutRunner runner = new CheckoutRunner(
                        dlg.LocalPath, dlg.Revision, new Uri(dlg.Url), dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Empty);
                    context.UIShell.RunWithProgressDialog(runner, "Checking out");

                    // make sure it's remembered
                    RegistryUtils.CreateNewTypedUrl(dlg.Url);
                }
            }
        }

        #endregion
    }
}