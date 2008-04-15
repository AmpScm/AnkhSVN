using System;
using Ankh.UI;
using System.Collections;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Configuration;
using Ankh.ContextServices;
using Ankh.VS;
using System.Collections.Generic;

namespace Ankh
{
    /// <summary>
    /// Encapsulates the details of an operation requiring a log message
    /// </summary>
    class CommitOperation
    {
        readonly IProgressWorker worker;
        readonly IAnkhServiceProvider _context;
        readonly ICollection<SvnItem> _items;
        readonly SvnCommitArgs _args;
        bool urlPaths;
        string logMessage;

        public CommitOperation(
            SvnCommitArgs args,
            IProgressWorker worker,
            ICollection<SvnItem> items,
            IAnkhServiceProvider context)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (items == null)
                throw new ArgumentNullException("items");
            if (context == null)
                throw new ArgumentNullException("context");

            _args = args;
            _context = context;
            this.worker = worker;

            _items = items;
        }

        public bool UrlPaths
        {
            get { return this.urlPaths; }
            set { this.urlPaths = value; }
        }

        public string LogMessage
        {
            get { return this.logMessage; }
            set { this.logMessage = value; }
        }

        public ICollection<SvnItem> Items
        {
            get { return this._items; }
        }

        public bool ShowLogMessageDialog()
        {
            IAnkhConfigurationService configSvc = _context.GetService<IAnkhConfigurationService>();
            IAnkhDialogOwner ownerSvc = _context.GetService<IAnkhDialogOwner>();

            AnkhConfig config = configSvc.Instance;

            string templateText = config.LogMessageTemplate != null ?
                config.LogMessageTemplate : "";
            LogMessageTemplate template = new LogMessageTemplate(_context, templateText);

            string savedLogMessage = "";
            // is there a previous log message?
            if (!string.IsNullOrEmpty(this.LogMessage))
            {
                if (config.AutoReuseComment ||
                    MessageBox.Show(ownerSvc.DialogOwner,
                    "The previous commit did not complete." + Environment.NewLine +
                    "Do you want to reuse the log message?",
                    "Previous log message", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)

                    savedLogMessage = LogMessage;
            }

            // don't show the dialog if shift is down.
            if (!(Commands.CommandBase.Shift))
            {
                using (CommitDialog dialog = new CommitDialog())
                {
                    dialog.Context = _context;
                    dialog.LogMessage = savedLogMessage;
                    dialog.LogMessageTemplate = template;
                    dialog.Items = _items;
                    dialog.CommitFilter += delegate { return true; };
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
        public bool Run(string caption)
        {
            ProgressRunner runner = new ProgressRunner(_context, worker);
            runner.Start(caption);

            return !runner.Cancelled;
        }


    }
}
