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
        /// Shows the commit dialog, blocking until the user hits cancel or commit.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public CommitContext ShowCommitDialogModal( CommitContext ctx )
        {
            if ( this.commitDialogWindow == null ) 
                this.CreateCommitDialog();
            Debug.Assert( this.commitDialog != null );

            this.commitDialog.CommitItems = ctx.CommitItems;
            this.commitDialog.UrlPaths = ctx.UrlPaths;
            this.commitDialog.LogMessageTemplate = ctx.LogMessageTemplate;
            if ( ctx.LogMessage != null )
                this.commitDialog.LogMessage = ctx.LogMessage;

            // we want to preserve the original state.
            bool originalVisibility = this.commitDialog.Visible;

            this.commitDialogWindow.Visible = true;
            this.commitDialogModal = true;

            // Fired when user clicks Commit or Cancel.
            this.commitDialog.Proceed += new EventHandler( this.ProceedCommit );

            // we need the buttons enabled now, since it's pseudo-modal.
            this.commitDialog.ButtonsEnabled = true;
            this.commitDialog.Initialize();

            while ( this.commitDialogModal && this.commitDialogWindow.Visible ) 
            {
                Application.DoEvents();
            }

            ctx.LogMessage = this.commitDialog.LogMessage;
            ctx.RawLogMessage = this.commitDialog.RawLogMessage;
            ctx.CommitItems = this.commitDialog.CommitItems;

            if ( this.commitDialog.CommitDialogResult != CommitDialogResult.Cancel )
            {
                ctx.Cancelled = false;
            }

            // restore the pre-modal state.
            this.commitDialog.ButtonsEnabled = false;
            this.commitDialogWindow.Visible = originalVisibility;
            this.commitDialog.CommitItems = new object[]{};

            return ctx;
        }

        public void ToggleCommitDialog( bool show )
        {
            if ( this.commitDialogWindow == null )
                this.CreateCommitDialog();

            Debug.Assert( this.commitDialogWindow != null );

            this.commitDialogWindow.Visible = show;
            this.commitDialog.ButtonsEnabled = this.commitDialogModal;
        }



        public void ResetCommitDialog()
        {
            this.commitDialog.Reset();
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

        private void CreateCommitDialog()
        {
            Debug.WriteLine( "Creating commit dialog user control", "Ankh" );
            object control = null;
            this.commitDialogWindow = this.context.DTE.Windows.CreateToolWindow( 
                this.context.AddIn, "AnkhUserControlHost.AnkhUserControlHostCtl", 
                "Commit", CommitDialogGuid, ref control );
            
            this.commitDialogWindow.Visible = true;
            this.commitDialogWindow.Visible = false;

            this.commitDialogWindow.Caption = "Commit";
            
            AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl 
                objControl = (AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl)control;
            
            this.commitDialog = new CommitDialog();
            objControl.HostUserControl( this.commitDialog );
            
            System.Diagnostics.Debug.Assert( this.commitDialog != null, 
                "Could not create tool window" );
            
        }

        private void ProceedCommit( object sender, EventArgs e )
        {
            this.commitDialog.Proceed -= new EventHandler( this.ProceedCommit );
            this.commitDialogModal = false;
            this.commitDialogWindow.Visible = false;
        }

        private RepositoryExplorerControl repositoryExplorerControl;        
        private Window repositoryExplorerWindow;
        private CommitDialog commitDialog;
        private Window commitDialogWindow;
        private IContext context;
        private bool commitDialogModal;
        
        public const string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
        private const string CommitDialogGuid = 
            "{08BD45A4-7716-49b0-BB41-CFEBCD098728}";


    
        
    }
}
