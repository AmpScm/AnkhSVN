using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of <code>WizardFramework.WizardPage</code> for handling
    /// conflict resolution.
    /// </summary>
    class MergeOptionsPage : BasePage
    {
        public const string PAGE_NAME = "Merge Options Page";
        public enum ConflictResolutionOption
        {
            PROMPT,
            MARK,
            MINE,
            THEIRS,
            BASE
        }

        public MergeOptionsPage(MergeWizard wizard)
            : base(wizard, new MergeOptionsPageControl(), PAGE_NAME)
        {
            IsPageComplete = true;

            Title = Resources.MergeOptionsHeaderTitle;
            this.Description = Resources.MergeOptionsHeaderMessage;

            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Gets/Sets how automatic conflicts
        /// for binary files should be handled.
        /// </summary>
        public ConflictResolutionOption BinaryConflictResolution
        {
            get { return _binaryConflictResolution; }
            set { _binaryConflictResolution = value; }
        }

        /// <summary>
        /// Gets/Sets how automatic conflicts
        /// for text files should be handled.
        /// </summary>
        public ConflictResolutionOption TextConflictResolution
        {
            get { return _textConflictResolution; }
            set { _textConflictResolution = value; }
        }

        /// <summary>
        /// Gets/Sets whether or not ancestry is ignored.
        /// </summary>
        public bool IgnoreAncestry
        {
            get { return _ignoreAncestry; }
            set { _ignoreAncestry = value; }
        }

        /// <summary>
        /// Gets/Sets whether or not unversioned obstructions
        /// are allowed.
        /// </summary>
        public bool AllowUnversionedObstructions
        {
            get { return _allowUnversionedObstructions; }
            set { _allowUnversionedObstructions = value; }
        }

        /// <summary>
        /// Gets/Sets the depth.
        /// </summary>
        public SvnDepth Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        private SvnDepth _depth = SvnDepth.Unknown;
        private bool _allowUnversionedObstructions = false;
        private bool _ignoreAncestry = false;
        private ConflictResolutionOption _binaryConflictResolution = ConflictResolutionOption.PROMPT;
        private ConflictResolutionOption _textConflictResolution = ConflictResolutionOption.PROMPT;
    }
}
