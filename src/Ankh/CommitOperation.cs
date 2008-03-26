using System;
using Ankh.UI;
using System.Collections;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Configuration;
using Ankh.ContextServices;
using Ankh.VS;

namespace Ankh
{
    /// <summary>
    /// Encapsulates the details of an operation requiring a log message
    /// </summary>
    public class CommitOperation  
    {
        readonly IProgressWorker worker;
        readonly IAnkhServiceProvider _context;
        readonly IList items;
        readonly SvnCommitArgs _args;
        bool urlPaths;
        string logMessage;

        public CommitOperation(SvnCommitArgs args, IProgressWorker worker, IList items, IAnkhServiceProvider context)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (context == null)
                throw new ArgumentNullException("context");

            _args = args;
            _context = context;
            this.worker = worker;
            
            this.items = items;
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
            IAnkhConfigurationService configSvc = _context.GetService<IAnkhConfigurationService>();
            IAnkhDialogOwner ownerSvc = _context.GetService<IAnkhDialogOwner>();

            AnkhConfig config = configSvc.Instance;

            string templateText = config.LogMessageTemplate != null ? 
                config.LogMessageTemplate : "";
            LogMessageTemplate template = new LogMessageTemplate(_context, templateText );


            // is there a previous log message?
            if ( this.LogMessage != null )
            {
                if ( config.AutoReuseComment ||
                    MessageBox.Show( ownerSvc.DialogOwner, 
                    "The previous commit did not complete." + Environment.NewLine + 
                    "Do you want to reuse the log message?", 
                    "Previous log message", MessageBoxButtons.YesNo ) ==
                    DialogResult.Yes )

                    _args.LogMessage = this.LogMessage;
            }

            // don't show the dialog if shift is down.
            if (!(Commands.CommandBase.Shift))
            {
                using (CommitDialog dialog = new CommitDialog())
                {
                    dialog.LogMessageTemplate = template;
                    dialog.CommitItems = items;
                    if (dialog.ShowDialog(ownerSvc.DialogOwner) != DialogResult.OK)
                    {
                        logMessage = dialog.RawLogMessage;
                        return false;
                    }

                    _args.LogMessage = dialog.LogMessage;
                }
            }

            //this.items = commitContext.CommitItems;
            this.logMessage = _args.LogMessage;
            return true;                
        }

        /// <summary>
        /// Get the show on the road.
        /// </summary>
        public bool Run( string caption )
        {
            ProgressRunner runner = new ProgressRunner(_context, worker);
            runner.Start(caption);

            return !runner.Cancelled;
        } 


    }
}
