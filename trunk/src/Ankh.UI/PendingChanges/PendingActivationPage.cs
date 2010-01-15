using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Commands;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;

namespace Ankh.UI.PendingChanges
{
    partial class PendingActivationPage : PendingChangesPage
    {
        public PendingActivationPage()
        {
            InitializeComponent();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            openSccSelectorLink.Font = new Font(Font, FontStyle.Bold);
        }

        private void openSccSelectorLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
            cs.PostExecCommand(new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.ToolsOptions), "53544C4D-1D2D-44BD-8566-4FC149E23AAF");
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingActivationPage);
            }
        }
    }
}
