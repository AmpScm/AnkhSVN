using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI
{
	public partial class LockDialog : Ankh.UI.PathSelector
	{
		public LockDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
            this.RevisionPickerStart.Visible = false;
            this.RevisionPickerEnd.Visible = false;
            this.revisionStartGroupBox.Visible = false;
            this.revisionEndGroupBox.Visible = false;
            this.recursiveCheckBox.Visible = false;
            this.Options = PathSelectorOptions.NoRevision;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	
        public string Message
        {
            get { return this.messageTextBox.Text; }
            set { this.messageTextBox.Text = value; }
        }

        public bool StealLocks
        {
            get { return this.stealLocksCheckBox.Checked; }
            set { this.stealLocksCheckBox.Checked = value; }
        }
    }
}

