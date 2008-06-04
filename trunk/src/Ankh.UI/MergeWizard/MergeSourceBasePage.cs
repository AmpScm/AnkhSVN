using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Abstract class allowing for extension to <code>WizardPage</code>,
    /// to be used by all merge source pages except for the "Two Different Trees"
    /// merge source page.
    /// </summary>
    public abstract class MergeSourceBasePage<TControl> : BasePage<MergeWizard, TControl>
        where TControl : MergeSourceBasePageControl<TControl>, new()
    {
        /// <summary>
        /// Constructor with name.
        /// </summary>
        protected MergeSourceBasePage(MergeWizard wizard, String name)
            : base(wizard, name)
        {
            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Constructor with name and image.
        /// </summary>
        protected MergeSourceBasePage(MergeWizard wizard, String name, Image image)
            : base(wizard, name, image)
        {
            PageControl.WizardPage = this;
        }

        /// <see cref="WizardFramework.IWizardPage.IsPageComplete" />
        public override bool IsPageComplete
        {
            get
            {
                if (!PageControl.IsMergeURLValid)
                    return false;

                return base.IsPageComplete;
            }
        }

        internal string MergeSource
        {
            get { return PageControl.MergeSource; }
        }

        /// <summary>
        /// Returns the merge type for the subclass' page.
        /// </summary>
        public abstract MergeWizard.MergeType MergeType { get; }
    }
}
