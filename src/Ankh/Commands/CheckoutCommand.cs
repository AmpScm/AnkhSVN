// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;


namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you check out a repository directory.
    /// </summary>
    [VSNetCommand("Checkout", Tooltip="Checkout a repository directory", 
         Text = "Checkout a repository directory..." ),
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
                }
                finally
                {
                    context.EndOperation();
                }
            }
        }
    }
}
