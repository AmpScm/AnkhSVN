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
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;

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
                return true;
            }
        }

        protected override void OnPageChanging(WizardPageChangingEventArgs e)
        {
            base.OnPageChanging(e);

            // Set the MergeSource before the page changes
            ((MergeWizard)Wizard).MergeSource = MergeSource;

            MergeSourceBasePageControlImpl control = (MergeSourceBasePageControlImpl)PageControl;


            // Do not validate since this field isn't editable and its contents are
            // retrieved directly from mergeinfo.
            if (MergeType == MergeWizard.MergeType.ManuallyRemove)
                return;


            // Do not show an error while the resources are retrieved.
            //if (mergeFromComboBox.Text == Resources.LoadingMergeSources)
            //    return true;

            if (string.IsNullOrEmpty(control.MergeSourceText))
            {
                Message = MergeUtils.NO_FROM_URL;
                e.Cancel = true;
                return;
            }

            Uri tmpUri;
            if (!Uri.TryCreate(control.MergeSourceText, UriKind.Absolute, out tmpUri))
            {
                Message = MergeUtils.INVALID_FROM_URL;
                e.Cancel = true;
                return;
            }
            
            Message = null;
        }

        SvnOrigin MergeSource
        {
            get { return ((MergeSourceBasePageControlImpl)PageControl).MergeSource; }
        }

        /// <summary>
        /// Returns the merge type for the subclass' page.
        /// </summary>
        internal abstract MergeWizard.MergeType MergeType { get; }

        internal virtual IEnumerable<Uri> GetMergeSources(SvnItem target)
        {
            SvnMergeSourcesCollection sources = ((MergeWizard)Wizard).MergeUtils.GetSuggestedMergeSources(target);
            if (sources == null)
                yield break;
            foreach (SvnMergeSource s in sources)
                yield return s.Uri;
        }
    }
}
