using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceManuallyRemovePageControl : MergeSourceManuallyRemovePageControlBase
    {
        public MergeSourceManuallyRemovePageControl()
        {
            InitializeComponent();
        }
    }

    public class MergeSourceManuallyRemovePageControlBase : MergeSourceBasePageControl<MergeSourceManuallyRemovePageControl>
    { }
}
