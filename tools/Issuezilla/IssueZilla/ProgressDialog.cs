using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IssueZilla
{
    public partial class ProgressDialog : Form, IProgressDialog
    {
        public ProgressDialog(Form parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.parent.Enabled = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose(); 
                }

                this.parent.Enabled = true;
            }
            base.Dispose( disposing );

        }

        #region IProgressDialog Members
        public void ShowError( Exception exception )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        private Form parent;
    }
}