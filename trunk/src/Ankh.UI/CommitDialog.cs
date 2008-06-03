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
    /// <summary>
    /// Dialog that lets a user enter a log message for a commit.
    /// </summary>
    public partial class CommitDialog : VSContainerForm
    {
        LogMessageTemplate _logMessageTemplate;
        bool _loaded = false;

        public CommitDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.commitItemsTree.AfterCheck += new TreeViewEventHandler(ItemChecked);

            ContainerMode = VSContainerMode.UseTextEditorScope;
        }
        
        /// <summary>
        /// Raises the ContextChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            commitItemsTree.Context = Context;
            logMessageBox.Init(Context, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddCommandTarget(logMessageBox.CommandTarget);
            AddWindowPane(logMessageBox.WindowPane);
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
            get { return this._logMessageTemplate; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this._logMessageTemplate = value; }
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

            this.logMessageBox.Select();

            _loaded = true;
        }

        private void ItemChecked(object sender, TreeViewEventArgs e)
        {
            // don't bother if we haven't been loaded
            if (!this._loaded)
                return;

            if (e.Node.Checked)
            {
                this.logMessageBox.Text = this.LogMessageTemplate.AddItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString());
            }
            else
            {
                this.logMessageBox.Text = this._logMessageTemplate.RemoveItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString());
            }
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="msg">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the Win32 message to process.</param>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>
        /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Return | Keys.Control))
            {
                DialogResult = DialogResult.OK;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }     
    }
}



