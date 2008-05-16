using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for handling the merge type
    /// selection of AnkhSVN's merge capabilities
    /// </summary>
    class MergeTypePage : WizardPage
    {
        public MergeTypePage() : base("Merge Type")
        {
            IsPageComplete = true;

            Title = Resources.MergeTypePageHeaderTitle;
            this.Message = new WizardMessage(Resources.MergeTypePageHeaderMessage);

            control_prop.WizardPage = this;
        }

        /// <summary>
        /// Returns whether or not to show the best practices page.
        /// </summary>
        public bool ShowBestPracticesPage
        {
            get { return control_prop.IsPerformBestPracticesChecked; }
        }

        /// <see cref="WizardFramework.WizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        public MergeType SelectedMergeType
        {
            get
            {
                return mergeType_prop;
            }

            set
            {
                mergeType_prop = value;
            }
        }

        public enum MergeType
        {
            RangeOfRevisions,
            Reintegrate,
            TwoDifferentTrees,
            ManuallyRecord,
            ManuallyRemove
        }

        private MergeType mergeType_prop = MergeType.RangeOfRevisions;
        private MergeTypePageControl control_prop = new MergeTypePageControl();
    }
}
