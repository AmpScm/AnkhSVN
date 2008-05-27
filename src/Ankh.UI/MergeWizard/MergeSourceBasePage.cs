using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;

namespace Ankh.UI.MergeWizard
{
    public abstract class MergeSourceBasePage : WizardPage
    {
        protected MergeSourceBasePage(String name)
            : base(name)
        {
            ((MergeSourceBasePageControl)Control).WizardPage = this;
        }
        protected MergeSourceBasePage(String name, Image image)
            : base(name, image)
        {
            ((MergeSourceBasePageControl)Control).WizardPage = this;
        }

        public override bool IsPageComplete
        {
            get
            {
                if (!((MergeSourceBasePageControl)Control).HasMergeSource)
                    return false;

                return base.IsPageComplete;
            }
        }

        public abstract MergeWizard.MergeType MergeType { get; }
    }
}
