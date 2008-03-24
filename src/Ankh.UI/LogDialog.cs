using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI
{
	public partial class LogDialog : Ankh.UI.PathSelector
	{
		public LogDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

        /// <summary>
        /// Whether to stop on a copy.
        /// </summary>
        public bool StopOnCopy
        {
            get{ return this.stopOnCopyCheckBox.Checked; }
            set{ this.stopOnCopyCheckBox.Checked = value; }
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
	}
}

