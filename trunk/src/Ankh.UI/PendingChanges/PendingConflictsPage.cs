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

        bool _loaded;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            conflictView.Context = Context;

            if (!SystemInformation.HighContrast)
            {
                IAnkhVSColor clr = Context.GetService<IAnkhVSColor>();
                Color c;
                if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TITLEBAR_INACTIVE, out c))
                {
                    resolvePannel.BackColor = c;
                }

                if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TITLEBAR_INACTIVE_TEXT, out c))
                {
                    resolvePannel.ForeColor = c;
                }
            }

            ResizeToFit();
            _loaded = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_loaded)
                ResizeToFit();
        }

        private void ResizeToFit()
        {
            conflictEditSplitter.SplitterDistance += conflictEditSplitter.Panel2.Height - resolveLinkLabel.Bottom - resolveLinkLabel.Margin.Bottom;
        }
    }
}
