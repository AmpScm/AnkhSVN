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
using Ankh.UI.WizardFramework;
using System.Collections.Generic;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSourceManuallyRemovePage : MergeSourceBasePage
    {
		public const string PAGE_NAME = "Merge Source Manually Remove";

		/// <summary>
		/// Constructor.
		/// </summary>
		public MergeSourceManuallyRemovePage()
		{
			Name = PAGE_NAME;

			IsPageComplete = false;
			Text = MergeStrings.MergeSourceHeaderTitle;
			Description = MergeStrings.MergeSourceManuallyRemovePageHeaderMessage;

			EnableSelectButton(false);
			InitializeComponent();
		}

		/// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
		internal override MergeWizard.MergeType MergeType
		{
			get { return MergeWizard.MergeType.ManuallyRemove; }
		}

		protected override void OnPageChanging(WizardPageChangingEventArgs e)
		{
			base.OnPageChanging(e);

			((MergeWizard)Wizard).LogMode = Ankh.UI.SvnLog.LogMode.MergesMerged;
		}

		internal override ICollection<Uri> GetMergeSources(SvnItem target)
		{
			SvnMergeItemCollection items = ((MergeWizard)Wizard).MergeUtils.GetAppliedMerges(target);

			List<Uri> rslt = new List<Uri>(items == null ? 0 : items.Count);

			if (items != null)
			{
				foreach (SvnMergeItem i in items)
					rslt.Add(i.Uri);
			}

			return rslt;
		}
    }
}
