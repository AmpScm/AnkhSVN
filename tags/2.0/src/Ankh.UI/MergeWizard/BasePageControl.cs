using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    public partial class BasePageControl : UserControl
    {
        public BasePageControl()
        {
            InitializeComponent();
        }

        BasePage _page;
        internal BasePage WizardPage
        {
            get { return _page; }
            set { _page = value; }
        }
    }
}
