using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IssueZilla
{
    public partial class BackgroundWorkerForm : Form
    {
        public BackgroundWorkerForm(DoWorkEventHandler worker) : 
            this(new DefaultBackgroundOperation(worker))
        {

        }

        public BackgroundWorkerForm( IBackgroundOperation operation )
        {
            InitializeComponent();

            this.operation = operation;

            this.backgroundWorker.DoWork += delegate { operation.Work(); };
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( backgroundWorker_RunWorkerCompleted );

        }        

        public string Caption
        {
            get { return this.label.Text; }
            set { this.label.Text = value; }
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            this.backgroundWorker.RunWorkerAsync();

        }

        void backgroundWorker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( e.Error != null )
            {
                MessageBox.Show( this, e.Error.ToString(), "Error", MessageBoxButtons.OK );
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.operation.WorkCompleted( e );
                this.DialogResult = DialogResult.OK;
            }
        }

        private class DefaultBackgroundOperation : IBackgroundOperation
        {
            public DefaultBackgroundOperation( DoWorkEventHandler worker )
            {
                this.worker = worker;
            }

            public void Work()
            {
                this.worker( this, null );
            }

            public void WorkCompleted( RunWorkerCompletedEventArgs e )
            {
            }

            private DoWorkEventHandler worker;
        }

        private IBackgroundOperation operation;


    }
}