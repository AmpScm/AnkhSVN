using System;
using Ankh.UI;
using System.Collections;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Configuration;

namespace Ankh
{
    /// <summary>
    /// Encapsulates the details of an operation requiring a log message
    /// </summary>
    public class CommitOperation  
    {
        public CommitOperation(IProgressWorker worker, IList items, IContext context)
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

        public IList Items
        {
            get { return this.items; }
        }

        public bool ShowLogMessageDialog()
        {
            Config config = this.context.Configuration.Instance;

            string templateText = config.LogMessageTemplate != null ? 
                context.Configuration.Instance.LogMessageTemplate : "";;
            LogMessageTemplate template = new LogMessageTemplate(context, templateText );

            this.commitContext = new CommitContext( template, this.items, this.UrlPaths );            

            // is there a previous log message?
            if ( this.LogMessage != null )
            {
                if ( config.AutoReuseComment ||
                    context.UIShell.ShowMessageBox(  
                    "The previous commit did not complete." + Environment.NewLine + 
                    "Do you want to reuse the log message?", 
                    "Previous log message", MessageBoxButtons.YesNo ) ==
                    DialogResult.Yes )

                    this.commitContext.LogMessage = this.LogMessage;
            }

            // don't show the dialog if shift is down.
            if (!(Commands.CommandBase.Shift))
            {
                if (context.UIShell.ShowCommitDialogModal(this.commitContext) != DialogResult.OK)
                    return false;
            }

            if ( commitContext.Cancelled )
            {
                this.logMessage = this.commitContext.RawLogMessage;
                return false;                
            }
            else
            {
                this.items = commitContext.CommitItems;
                this.logMessage = commitContext.LogMessage;
                return true;                
            }
        }

        /// <summary>
        /// Get the show on the road.
        /// </summary>
        public bool Run( string caption )
        {
            return this.context.UIShell.RunWithProgressDialog( this.worker, caption );
        } 

        private IProgressWorker worker;
        private IContext context;
        private CommitContext commitContext;
        private bool urlPaths;
        private IList items;
        private string logMessage;
    }
}
