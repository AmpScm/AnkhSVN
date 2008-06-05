using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceManuallyRecordPageControl : MergeSourceManuallyRecordPageControlBase
    {
        public MergeSourceManuallyRecordPageControl()
        {
            InitializeComponent();
        }
    }

    public class MergeSourceManuallyRecordPageControlBase : MergeSourceBasePageControl<MergeSourceManuallyRecordPageControl>
    {
    }
}
