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
    public abstract class MergeSourceBasePage : WizardPage
    {
        /// <summary>
        /// Constructor with name.
        /// </summary>
        protected MergeSourceBasePage(String name)
            : base(name)
        {
            ((MergeSourceBasePageControl)Control).WizardPage = this;
        }

        /// <summary>
        /// Constructor with name and image.
        /// </summary>
        protected MergeSourceBasePage(String name, Image image)
            : base(name, image)
        {
            ((MergeSourceBasePageControl)Control).WizardPage = this;
        }

        /// <see cref="WizardFramework.IWizardPage.IsPageComplete" />
        public override bool IsPageComplete
        {
            get
            {
                if (!((MergeSourceBasePageControl)Control).IsMergeURLValid)
                    return false;

                return base.IsPageComplete;
            }
        }

        /// <summary>
        /// Returns the merge type for the subclass' page.
        /// </summary>
        public abstract MergeWizard.MergeType MergeType { get; }
    }
}
