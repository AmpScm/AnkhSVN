// $Id$
using System;
using Ankh.UI;
using EnvDTE;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ankh
{
	/// <summary>
	/// Summary description for UIShell.
	/// </summary>
	public class UIShell : IUIShell
	{
		public UIShell()
		{
			//
			// TODO: Add constructor logic here
			//
        }
        #region IUIShell Members
        public RepositoryExplorerControl RepositoryExplorer
        {
            get
            { 
                if( this.repositoryExplorerControl == null )
                    this.CreateRepositoryExplorer();
                return this.repositoryExplorerControl;
            }
        }

        public IContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.context = value; }
        }

        public DialogResult QueryWhetherAnkhShouldLoad()
        {
            string nl = Environment.NewLine;
            string msg = "Ankh has detected that the solution file for this solution " + 
                "is in a Subversion working copy." + nl + 
                "Do you want to enable Ankh for this solution?" + nl +
                "(If you select Cancel, Ankh will not be enabled, " + nl +
                "but you will " +
                "be asked this question again the next time you open the solution)";

            //TODO: The UIShell should be responsible for maintaining the hostwindow
            return MessageBox.Show( 
                this.context.HostWindow, msg, "Ankh", 
                MessageBoxButtons.YesNoCancel );
        }

        public void ShowRepositoryExplorer(bool show)
        {
            this.repositoryExplorerWindow.Visible = show;

        }
    
        public void SetRepositoryExplorerSelection(object[] selection)
        {
            this.repositoryExplorerWindow.SetSelectionContainer( ref selection );
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons )
        {
            return MessageBox.Show( this.Context.HostWindow, text, caption,
                buttons );
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons, MessageBoxIcon icon )
        {
            return MessageBox.Show( this.Context.HostWindow, text, caption,
                buttons, icon );
        }

        #endregion

        private void CreateRepositoryExplorer()
        {   
            Debug.WriteLine( "Creating repository explorer", "Ankh" );
            object control = null;
            this.repositoryExplorerWindow = this.context.DTE.Windows.CreateToolWindow( 
                this.context.AddIn, "AnkhUserControlHost.AnkhUserControlHostCtl", 
                "Repository Explorer", REPOSEXPLORERGUID, ref control );
            
            this.repositoryExplorerWindow.Visible = true;
            this.repositoryExplorerWindow.Caption = "Repository Explorer";
            
            AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl 
                objControl = (AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl)control;
            
            this.repositoryExplorerControl = new RepositoryExplorerControl();
            objControl.HostUserControl( this.repositoryExplorerControl );
            
            System.Diagnostics.Debug.Assert( this.repositoryExplorerControl != null, 
                "Could not create tool window" );
        }

        private RepositoryExplorerControl repositoryExplorerControl;
        private Window repositoryExplorerWindow;
        private IContext context;
        
        public const string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
    
        
    }
}
