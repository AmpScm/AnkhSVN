using System;
using Ankh.UI;
using System.Collections;
using System.Windows.Forms;


namespace Ankh
{
	/// <summary>
	/// Encapsulates the details of an operation requiring a log message
	/// </summary>
	public class CommitOperation  
	{
		public CommitOperation( IProgressWorker worker, IList items, IContext context )
		{
			this.worker = worker;
            this.context = context;
            this.items = items;
		}

        public CommitContext CommitContext
        {
            get{ return this.commitContext; }
        }

        public bool UrlPaths
        {
            get { return this.urlPaths; }
            set{ this.urlPaths = value; }
        }

        public string LogMessage
        {
            get{ return this.logMessage; }
            set{ this.logMessage = value; }
        }

        public bool ShowLogMessageDialog()
        {
            string templateText = this.context.Config.LogMessageTemplate != null ? 
                context.Config.LogMessageTemplate : "";;
            LogMessageTemplate template = new LogMessageTemplate( templateText );

            this.commitContext = new CommitContext( template, this.items, this.UrlPaths );            

            // is there a previous log message?
            if ( this.LogMessage != null )
            {
                if ( this.context.Config.AutoReuseComment ||
                    context.UIShell.ShowMessageBox(  
                    "The previous commit did not complete." + Environment.NewLine + 
                    "Do you want to reuse the log message?", 
                    "Previous log message", MessageBoxButtons.YesNo ) ==
                    DialogResult.Yes )

                    this.commitContext.LogMessage = this.LogMessage;
            }

            this.commitContext = context.UIShell.ShowCommitDialogModal( this.commitContext );
            if ( commitContext.Cancelled )
            {
                this.logMessage = this.commitContext.RawLogMessage;
                return false;                
            }
            else
            {
                this.logMessage = commitContext.LogMessage;
                return true;                
            }
        }

        /// <summary>
        /// Get the show on the road.
        /// </summary>
        public bool Run( string caption )
        {
            try
            {
                this.context.Client.LogMessage += new NSvn.Core.LogMessageDelegate(this.LogMessageWanted);
                return this.context.UIShell.RunWithProgressDialog( this.worker, caption );
            }
            finally
            {
                this.context.Client.LogMessage -= new NSvn.Core.LogMessageDelegate(this.LogMessageWanted);
            }
        } 

        private IProgressWorker worker;
        private IContext context;
        private CommitContext commitContext;
        private bool urlPaths;
        private IList items;
        private string logMessage;

        private void LogMessageWanted(object sender, NSvn.Core.LogMessageEventArgs args)
        {
            args.Message = this.LogMessage;
        }
    }
}
