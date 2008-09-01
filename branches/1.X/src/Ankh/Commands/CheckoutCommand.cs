// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using NSvn.Common;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to checkout a Subversion repository.
    /// </summary>
    [VSNetCommand("Checkout",
         Text = "Chec&kout a Repository...",
         Tooltip="Checkout a Subversion repository.", 
         Bitmap = ResourceBitmaps.CheckoutDirectory),
         VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class CheckoutCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            using(CheckoutDialog dlg = new CheckoutDialog())
            {
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                context.StartOperation( "Checking out" );
                try
                {
                    CheckoutRunner runner = new CheckoutRunner(
                        dlg.LocalPath, dlg.Revision, dlg.Url, dlg.Recursive ? Recurse.Full : Recurse.None );
                    context.UIShell.RunWithProgressDialog( runner, "Checking out" );

                    // make sure it's remembered
                    RegistryUtils.CreateNewTypedUrl( dlg.Url );
                }
                finally
                {
                    context.EndOperation();
                }
            }
        }

        #endregion
    }
}