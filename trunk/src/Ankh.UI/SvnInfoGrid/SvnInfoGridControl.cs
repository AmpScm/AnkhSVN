using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Commands;
using System.ComponentModel.Design;

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

            IServiceContainer container = Context.GetService<IServiceContainer>();

            if (container != null)
            {
                if (null == container.GetService(typeof(SvnInfoGridControl)))
                    container.AddService(typeof(SvnInfoGridControl), this);
            }

            base.OnLoad(e);
        }

        internal InfoPropertyGrid Grid
        {
            get { return grid; }
        }
	}
}
