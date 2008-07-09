#region Copyright And Revision History

/*---------------------------------------------------------------------------

	FindDlg.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.27.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Diff
{
    public interface IFindDlg
    {
        bool Execute(IWin32Window Owner, FindData Data);
    }

    /// <summary>
    /// A standard Find dialog.
    /// </summary>
    public partial class FindDlg : System.Windows.Forms.Form, IFindDlg
    {
        private System.Windows.Forms.Label lblFind;
        public System.Windows.Forms.TextBox edtText;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.CheckBox chkMatchCase;
        public System.Windows.Forms.CheckBox chkSearchUp;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public FindDlg()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
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

        public bool Execute(IWin32Window owner, FindData Data)
        {
            edtText.Text = Data.Text;
            chkMatchCase.Checked = Data.MatchCase;
            chkSearchUp.Checked = Data.SearchUp;
            if (ShowDialog(owner) == DialogResult.OK)
            {
                Data.Text = edtText.Text;
                Data.MatchCase = chkMatchCase.Checked;
                Data.SearchUp = chkSearchUp.Checked;
                return true;
            }

            return false;
        }

        private void edtText_TextChanged(object sender, System.EventArgs e)
        {
            btnOK.Enabled = edtText.TextLength > 0;
        }
    }
}
