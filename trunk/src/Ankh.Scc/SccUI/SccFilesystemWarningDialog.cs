using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.Scc.SccUI
{
    public partial class SccFilesystemWarningDialog : VSDialogForm
    {
        public SccFilesystemWarningDialog()
        {
            InitializeComponent();
        }

        public bool DontWarnAgain
        {
            get { return dontWarnAgain.Checked; }
            set { dontWarnAgain.Checked = value; }
        }
    }
}
