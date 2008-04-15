// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Ankh.UI
{
    public enum CommitDialogResult
    {
        Cancel,
        Commit
    }

    /// <summary>
    /// Dialog that lets a user enter a log message for a commit.
    /// </summary>
    public partial class CommitDialog : Form
    {
        public event EventHandler Proceed;
        IAnkhServiceProvider _context;

        public CommitDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.commitItemsTree.AfterCheck += new TreeViewEventHandler(ItemChecked);

            // Support Ctrl-A to select everything.
            this.logMessageBox.KeyDown += new KeyEventHandler(logMessageBox_KeyDown);
            this.logMessageBox.KeyPress += new KeyPressEventHandler(logMessageBox_KeyPress);

            // HACK: since there is no KeyPreview on a UserControl
            this.HookUpKeyEvent(this);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raises the ContextChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnContextChanged(EventArgs e)
        {
            commitItemsTree.Context = Context;
        }

        /// <summary>
        /// The log message to be used for this commit.
        /// </summary>
        public string LogMessage
        {
            get
            {
                return this.LogMessageTemplate.PostProcess(this.logMessageBox.Text);
            }
            set
            {
                this.logMessageBox.Text = value;
            }
        }

        /// <summary>
        /// The raw log message, with commented lines still embedded.
        /// </summary>
        public string RawLogMessage
        {
            get
            {
                return this.logMessageBox.Text;
            }
            set
            {
                this.logMessageBox.Text = value;
            }
        }

        /// <summary>
        /// The template to use for log messages.
        /// </summary>
        public LogMessageTemplate LogMessageTemplate
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.logMessageTemplate; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.logMessageTemplate = value; }
        }

        public ICollection<SvnItem> Items
        {
            get { return commitItemsTree.Items; }
            set { commitItemsTree.Items = value; }
        }

        public IEnumerable<SvnItem> CommitItems
        {
            get { return this.commitItemsTree.CheckedItems; }
            //set
            //{ 
            //    this.commitItemsTree.Items = value;
            //    this.commitItemsTree.CheckedItems = value;

            //}
        }

        public event Predicate<SvnItem> CommitFilter
        {
            add { commitItemsTree.CheckedFilter += value; }
            remove { commitItemsTree.CheckedFilter -= value; }
        }

        public bool UrlPaths
        {
            get
            { return this.commitItemsTree.UrlPaths; }
            set
            {
                this.commitItemsTree.UrlPaths = value;
            }
        }

        public CommitDialogResult CommitDialogResult
        {
            get { return this.dialogResult; }
        }

        /// <summary>
        /// Whether the Commit/Cancel buttons should be enabled.
        /// </summary>
        public bool ButtonsEnabled
        {
            get { return this.commitButton.Enabled || this.cancelButton.Enabled; }
            set
            {
                this.commitButton.Enabled = this.cancelButton.Enabled = value;
            }
        }

        public bool KeepLocks
        {
            get { return this.keepLocksCheckBox.Checked; }
            set { this.keepLocksCheckBox.Checked = value; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialize the log message in the text box.
        /// </summary>
        public void Initialize()
        {
            if (this.logMessageBox.Text.Trim() == String.Empty)
            {
                ArrayList arr = new ArrayList();
                foreach (object item in this.commitItemsTree.CheckedItems)
                    arr.Add(item.ToString());
                this.LogMessageTemplate.UrlPaths = this.commitItemsTree.UrlPaths;
                this.logMessageBox.Text = this.LogMessageTemplate.PreProcess(arr);


            }

            this.logMessageBox.Focus();
            this.logMessageBox.SelectionStart = this.logMessageBox.SelectionStart + this.logMessageBox.SelectionLength;
            this.logMessageBox.SelectionLength = 0;

            this.dialogResult = CommitDialogResult.Cancel;

            this.loaded = true;
        }

        /// <summary>
        /// Reset the log message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reset()
        {
            this.logMessageBox.Text = "";
            this.dialogResult = CommitDialogResult.Cancel;
        }

        private void ItemChecked(object sender, TreeViewEventArgs e)
        {
            // don't bother if we haven't been loaded
            if (!this.loaded)
                return;

            if (e.Node.Checked)
            {
                this.logMessageBox.Text = this.LogMessageTemplate.AddItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString());
            }
            else
            {
                this.logMessageBox.Text = this.logMessageTemplate.RemoveItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString());
            }
        }

        private void RaiseProceed(object sender, System.EventArgs e)
        {
            if (sender == this.cancelButton)
                this.dialogResult = CommitDialogResult.Cancel;
            else if (sender == this.commitButton)
                this.dialogResult = CommitDialogResult.Commit;

            if (this.Proceed != null)
                this.Proceed(this, EventArgs.Empty);

            this.loaded = false;
        }

        void logMessageBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // suppress Ctrl-A, which is ASCII 1 for some reason...
            if (e.KeyChar == (char)1)
            {
                e.Handled = true;
            }
        }

        void logMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                this.logMessageBox.SelectAll();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle Ctrl-Enter to commit and Esc to cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommitDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.commitButton.Enabled && e.Control && e.KeyCode == Keys.Enter)
            {
                this.RaiseProceed(this.commitButton, EventArgs.Empty);
                e.Handled = true;
            }
            else if (this.cancelButton.Enabled && e.KeyCode == Keys.Escape)
            {
                this.RaiseProceed(this.cancelButton, EventArgs.Empty);
                e.Handled = true;
            }
        }

        /// <summary>
        /// This method is necessary to ensure we get this event from all controls, since there
        /// is no KeyPreview on a user control.
        /// </summary>
        /// <param name="control"></param>
        private void HookUpKeyEvent(Control control)
        {
            control.KeyDown += new KeyEventHandler(CommitDialog_KeyDown);
            foreach (Control child in control.Controls)
            {
                this.HookUpKeyEvent(child);
            }
        }
        private LogMessageTemplate logMessageTemplate;

        private bool loaded = false;

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}



