// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;


namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you check out a repository directory.
    /// </summary>
    [VSNetCommand("Checkout", Tooltip="Checkout a repository directory", 
         Text = "Checkout a repository directory...", 
         Bitmap = ResourceBitmaps.CheckoutDirectory),
    VSNetControl( "MenuBar.Tools.AnkhSVN", Position = 1 ) ]
    internal class CheckoutCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return Enabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            using(CheckoutDialog dlg = new CheckoutDialog())
            {
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                context.StartOperation( "Checking out" );
                try
                {
                    CheckoutRunner runner = new CheckoutRunner( context, 
                        dlg.LocalPath, dlg.Revision, dlg.Url, !dlg.NonRecursive );
                    runner.Start( "Checking out" );

                    // make sure it's remembered
                    RegistryUtils.CreateNewTypedUrl( dlg.Url );
                }
                finally
                {
                    context.EndOperation();
                }
            }
        }
    }
}
