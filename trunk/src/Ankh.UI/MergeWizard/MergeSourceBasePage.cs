using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Abstract class allowing for extension to <code>WizardPage</code>,
    /// to be used by all merge source pages except for the "Two Different Trees"
    /// merge source page.
    /// </summary>
    public abstract class MergeSourceBasePage : BasePage
    {
        /// <summary>
        /// Constructor with name.
        /// </summary>
        protected MergeSourceBasePage(MergeWizard wizard, BasePageControl control, string name)
            : base(wizard, control, name)
        {
            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Constructor with name and image.
        /// </summary>
        protected MergeSourceBasePage(MergeWizard wizard, BasePageControl control, string name, Image image)
            : base(wizard, control, name, image)
        {
            PageControl.WizardPage = this;
        }

        /// <see cref="WizardFramework.IWizardPage.IsPageComplete" />
        public override bool IsPageComplete
        {
            get
            {
                if (!((MergeSourceBasePageControlImpl)PageControl).IsMergeURLValid)
                    return false;

                return base.IsPageComplete;
            }
        }

        protected override void OnPageChanging(WizardPageChangingEventArgs e)
        {
            base.OnPageChanging(e);

            // Set the MergeSource before the page changes
            ((MergeWizard)Wizard).MergeSource = MergeSource;
        }

        internal SvnOrigin MergeSource
        {
            get { return ((MergeSourceBasePageControlImpl)PageControl).MergeSource; }
        }

        /// <summary>
        /// Returns the merge type for the subclass' page.
        /// </summary>
        public abstract MergeWizard.MergeType MergeType { get; }
    }
}
