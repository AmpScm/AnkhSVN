using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.PendingChanges
{
    partial class PendingConflictsPage : PendingChangesPage
    {
        public PendingConflictsPage()
        {
            InitializeComponent();
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingConflictsPage);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            conflictView.Context = Context;
            conflictEditSplitter.SplitterDistance += conflictEditSplitter.Panel2.Height - resolveBottomLabel.Bottom - 2;

            IAnkhVSColor clr = Context.GetService<IAnkhVSColor>();
            Color c;
            if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_PANEL_TITLEBAR, out c))
            {
                resolvePannel.BackColor = c;
            }

            if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_PANEL_TITLEBAR_TEXT, out c))
            {
                resolvePannel.ForeColor = c;
            }
        }

        private void conflictEditSplitter_Panel2_MouseEnter(object sender, EventArgs e)
        {
            IAnkhVSColor clr = Context.GetService<IAnkhVSColor>();
            Color c;
            if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_PANEL_TITLEBAR, out c))
            {
                conflictEditSplitter.BackColor = c;// System.Drawing.SystemColors.ActiveCaption;
                //flowLayoutPanel1.BackColor = conflictEditSplitter.BackColor;
            }
        }
    }
}
