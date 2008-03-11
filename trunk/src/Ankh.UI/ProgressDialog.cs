using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog used for long-running operations.
    /// </summary>
    public partial class ProgressDialog : System.Windows.Forms.Form
    {
        /// <summary>
        /// Fired at regular intervals.
        /// </summary>
        public event EventHandler<ProgressStatusEventArgs> ProgressStatus;

        /// <summary>
        /// Loader Form
        /// </summary>
        /// <param name="inText">Text to be printed in the form.</param>
        public ProgressDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public string Caption
        {
            get
            {
                return textLabel.Text;
            }
            set
            {
                textLabel.Text = value;
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void ProgressDialog_Load(object sender, System.EventArgs e)
        {
            this.timer.Enabled = true;
        }

        private void TimerTick(object sender, System.EventArgs e)
        {
            if ( this.ProgressStatus != null )
                this.ProgressStatus( this, this.args );

            this.progressBar.PerformStep();
            if ( this.progressBar.Value >= this.progressBar.Maximum )
                this.progressBar.Value = this.progressBar.Minimum;

            if ( this.args.Done )
                this.Close();

            this.timer.Enabled = true;
        }

        private void CancelClick(object sender, System.EventArgs e)
        {
            this.args.SetCancelled( true );
            this.cancelButton.Text = "Cancelling...";
            this.cancelButton.Enabled = false;
        }
        
        private ProgressStatusEventArgs args = new ProgressStatusEventArgs();        
    }

    /// <summary>
    /// An event args class used by the ProgressDialog.ProgressStatus event.
    /// </summary>
    public class ProgressStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Event handlers can set this to true if the operation is finished.
        /// </summary>
        public bool Done
        {
            get{ return this.done; }
            set{ this.done = value; }
        }

        /// <summary>
        /// The dialog uses this to indicate that the user has clicked 
        /// Cancel. Event handlers should detect this and attempt to 
        /// cancel the ongoing operation.
        /// </summary>
        public bool Cancelled
        {
            get{ return this.cancelled; }
        }
        
        internal void SetCancelled( bool val )
        {
            this.cancelled = val; 
        }

        private bool done;
        private bool cancelled;
    }
}
