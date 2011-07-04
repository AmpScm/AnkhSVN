using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.SvnInfoGrid
{
	public partial class SvnInfoGridControl : AnkhToolWindowControl
	{
		public SvnInfoGridControl()
		{
			InitializeComponent();
		}

        protected override void OnLoad(EventArgs e)
        {
            ToolStripRenderer renderer = null;
            System.Windows.Forms.Design.IUIService ds = Context.GetService<System.Windows.Forms.Design.IUIService>();
            if (ds != null)
            {
                renderer = ds.Styles["VsToolWindowRenderer"] as ToolStripRenderer;
            }

            if (renderer != null)
                grid.ToolStripRenderer = renderer;

            base.OnLoad(e);
        }
	}
}
