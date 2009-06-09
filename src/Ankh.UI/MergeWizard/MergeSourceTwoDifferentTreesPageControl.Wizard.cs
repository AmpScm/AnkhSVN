// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for retrieving the merge source
    /// information for merging two different trees together.
    /// </summary>
    partial class MergeSourceTwoDifferentTreesPage
    {
        public const string PAGE_NAME = "Merge Source Two Different Trees";

        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceTwoDifferentTreesPage(MergeWizard wizard) 
            : base(wizard, PAGE_NAME)
        {
            IsPageComplete = false;
            Title = MergeStrings.MergeSourceHeaderTitle;
            Description = MergeStrings.MergeSourceTwoDifferentTreesPageHeaderMessage;
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Sets the first merge source.
        /// </summary>
        public string MergeSourceOne
        {
            get { return _mergeSourceOne; }
            set { _mergeSourceOne = value; }
        }

        /// <summary>
        /// Gets/Sets the second merge source.
        /// </summary>
        public string MergeSourceTwo
        {
            get { return _mergeSourceTwo; }
            set { _mergeSourceTwo = value; }
        }

        /// <summary>
        /// Gets/Sets whether or not merge source one and
        /// merge source two have the same url.
        /// </summary>
        public bool HasSecondMergeSourceUrl
        {
            get { return _hasSecondMergeSourceUrl; }
            set { _hasSecondMergeSourceUrl = value; }
        }

        /// <summary>
        /// Gets/Sets the 'From' merge revision.
        /// </summary>
        public long MergeFromRevision
        {
            get { return _mergeFromRevision; }
            set { _mergeFromRevision = value; }
        }

        /// <summary>
        /// Gets/Sets the 'To' merge revision.
        /// </summary>
        public long MergeToRevision
        {
            get { return _mergeToRevision; }
            set { _mergeToRevision = value; }
        }

        private long _mergeFromRevision;
        private long _mergeToRevision;
        private bool _hasSecondMergeSourceUrl = false;
        private string _mergeSourceOne;
        private string _mergeSourceTwo;
    }
}
