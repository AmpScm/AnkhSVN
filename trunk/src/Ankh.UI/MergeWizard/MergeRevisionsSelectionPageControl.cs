using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPageControl : BasePageControl
    {
        public MergeRevisionsSelectionPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the merge source.
        /// </summary>
        /// <value>The merge source.</value>
        public string MergeSource
        {
            get { return ((MergeWizard)WizardPage.Wizard).MergeSource; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            logToolControl1.Site = Site;
            logToolControl1.Mode = Ankh.UI.SvnLog.LogMode.Remote;
            if(MergeSource != null)
                logToolControl1.Start(WizardPage.Context, new string[]{MergeSource});
        }
    }
}
