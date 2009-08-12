// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
    partial class MergeOptionsPage
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
            : base(wizard, PAGE_NAME)
        {
            IsPageComplete = true;

            Title = MergeStrings.MergeOptionsHeaderTitle;
            this.Description = MergeStrings.MergeOptionsHeaderMessage;
            InitializeComponent();
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
