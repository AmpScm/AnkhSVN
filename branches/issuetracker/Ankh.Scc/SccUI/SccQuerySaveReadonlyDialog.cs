using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ankh.UI;

namespace Ankh.Scc.SccUI
{
    public partial class SccQuerySaveReadonlyDialog : VSDialogForm
    {
        string _file;

        public SccQuerySaveReadonlyDialog()
        {
            InitializeComponent();
        }

        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblFile.Text = string.Format(SccManagementResources.ReadonlyFileXCannotBeSaved, Path.GetFileName(File));
        }
    }
}
