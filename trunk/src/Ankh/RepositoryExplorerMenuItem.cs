// $Id$
using System;
using System.Windows.Forms;
using EnvDTE;

namespace Ankh
{
    /// <summary>
    /// A MenuItem that invokes an ICommand.
    /// </summary>
    internal class RepositoryExplorerMenuItem : MenuItem
    {
        public RepositoryExplorerMenuItem( AnkhContext context, ICommand command )
        {	
            this.context = context;
            this.command = command;
            
        }

        internal void RegisterWithParent()
        {
            this.Parent.GetContextMenu().Popup += new EventHandler( this.OnPopup );
        }

        private void OnPopup( object sender, EventArgs e )
        {
            this.Enabled = this.command.QueryStatus( this.context ) == ENABLEDMASK;
        }

        protected override void OnClick( EventArgs e )
        {
            base.OnClick( e );
            this.command.Execute( this.context );
        }


        private AnkhContext context;
        private ICommand command;
        private const vsCommandStatus ENABLEDMASK = vsCommandStatus.vsCommandStatusEnabled |
            vsCommandStatus.vsCommandStatusSupported;



    }
}
