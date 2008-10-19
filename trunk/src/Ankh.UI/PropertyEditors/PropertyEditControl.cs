using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Ankh.UI.PropertyEditors
{
    class PropertyEditControl : UserControl
    {
        public PropertyEditControl()
        {
            Size = new Size(348, 196);
        }

        [Localizable(true), DefaultValue(typeof(Size), "348;196")]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }
    }
}
