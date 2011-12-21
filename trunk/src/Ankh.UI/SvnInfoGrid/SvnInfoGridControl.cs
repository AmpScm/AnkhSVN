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
