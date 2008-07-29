using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.Scc.UI;

namespace Ankh.UI.DiffWindow
{
    public partial class MergeToolWindowControl : AnkhToolWindowControl
    {
        public MergeToolWindowControl()
        {
            InitializeComponent();
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowSite.CommandContext = AnkhId.DiffMergeContextGuid;
            ToolWindowSite.KeyboardContext = AnkhId.DiffMergeContextGuid;
        }
    }
}
