using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ankh.UI.SvnInfoGrid
{
    class InfoPropertyGrid : PropertyGrid
    {
        [Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripRenderer ToolStripRenderer
        {
            get { return base.ToolStripRenderer; }
            set { base.ToolStripRenderer = value; }
        }
    }
}
