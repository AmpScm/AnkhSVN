using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.Commands
{
    public partial class UpdateDialog : VSContainerForm
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        SvnItem _item;
        public SvnItem ItemToUpdate
        {
            get { return _item; }
            set
            {
                _item = value;
                if (value != null)
                {
                    projectRootBox.Text = value.FullPath;
                    urlBox.Text = _item.Status.Uri.ToString();
                    versionBox.SvnOrigin = new Ankh.Scc.SvnOrigin(value);                    
                }
            }
        }

        public void SetMultiple(bool multiple)
        {
            projectRootLabel.Enabled = !multiple;
            if (multiple)
                projectRootBox.Text = "-";
        }

        public SvnOrigin SvnOrigin
        {
            get { return versionBox.SvnOrigin; }
            set
            {
                versionBox.SvnOrigin = value;
                if (value != null)
                    urlBox.Text = value.Uri.ToString();
            }
        }

        public SvnRevision Revision
        {
            get { return versionBox.Revision; }
            set { versionBox.Revision = value; }
        }

        public bool AllowUnversionedObstructions
        {
            get { return allowObstructions.Checked; }
            set { allowObstructions.Checked = value; }
        }

        public bool UpdateExternals
        {
            get { return updateExternals.Checked; }
            set { updateExternals.Checked = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Context != null)
                versionBox.Context = Context;
        }        
    }
}
