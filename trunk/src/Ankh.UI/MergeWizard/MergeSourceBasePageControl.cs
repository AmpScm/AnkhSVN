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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;

using SharpSvn;
using WizardFramework;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.RepositoryOpen;
using System.IO;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    
    public partial class MergeSourceBasePageControl: BasePageControl
        
    {
        private readonly WizardMessage INVALID_FROM_URL = new WizardMessage(Resources.InvalidFromUrl,
            WizardMessage.MessageType.Error);

        readonly MergeSources _retrieveMergeSources;
        readonly BindingList<string> suggestedSources = new BindingList<string>();
        readonly BindingSource bindingSource;
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceBasePageControl()
        {
            InitializeComponent();
            _retrieveMergeSources = new MergeSources(RetrieveMergeSources);
            
            bindingSource = new BindingSource(suggestedSources, "");
            mergeFromComboBox.DataSource = bindingSource;
        }

        /// <summary>
        /// Enables/Disables the Select button.
        /// </summary>
        internal void EnableSelectButton(bool enabled)
        {
            selectButton.Enabled = enabled;
            selectButton.Visible = enabled;
        }

        internal string MergeSourceText
        {
            get { return mergeFromComboBox.Text.Trim(); }
        }

        internal SvnOrigin MergeSource
        {
            get
            {
                SvnOrigin mergeSource = null;
                string mergeFrom = mergeFromComboBox.Text.Trim();
                if (!string.IsNullOrEmpty(mergeFrom))
                {
                    Uri mergeFromUri;
                    if (Uri.TryCreate(mergeFrom, UriKind.Absolute, out mergeFromUri))
                    {
                        try
                        {
                            mergeSource = new SvnOrigin(WizardPage.Context, mergeFromUri, null);
                        }
                        catch
                        {
                            mergeSource = null;
                        }
                    }
                }
                return mergeSource;
            }
        }

        MergeSourceBasePage _wizardPage;
        /// <summary>
        /// Gets/Sets the wizard page associated with this UserControl.
        /// </summary>
        internal new MergeSourceBasePage WizardPage
        {
            get { return _wizardPage ?? (_wizardPage = (MergeSourceBasePage)base.WizardPage); }
            set
            { 
                _wizardPage = value;
                base.WizardPage = value;
            }
        }

        IAsyncResult _mergeRetrieveResult;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            mergeFromComboBox.Text = MergeTarget.Status.Uri.ToString();

            _mergeRetrieveResult = _retrieveMergeSources.BeginInvoke(new AsyncCallback(MergesRetrieved), null);
            
        }

        /// <summary>
        /// Sets the merge sources for the mergeFromComboBox.
        /// </summary>
        private void SetMergeSources(ICollection<Uri> mergeSources)
        {
            if (InvokeRequired)
            {
                SetMergeSourcesCallBack c = new SetMergeSourcesCallBack(SetMergeSources);
                BeginInvoke(c, new object[] { mergeSources });
                return;
            }
            MergeWizard wizard = (MergeWizard)WizardPage.Wizard;

            bool containsAtLeastOne = false;

            foreach (Uri u in mergeSources)
            {
                containsAtLeastOne = true;
                suggestedSources.Add(u.ToString());
            }
            
            if(containsAtLeastOne)
            {
                UIUtils.ResizeDropDownForLongestEntry(mergeFromComboBox);
            }
            else if (WizardPage.MergeType == MergeWizard.MergeType.ManuallyRemove)
            {
                WizardPage.Message = new WizardMessage(Resources.NoRevisionsToUnblock, WizardMessage.MessageType.Error);
                mergeFromComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }

            ((WizardDialog)WizardPage.Form).EnablePageAndButtons(true);
        }

        /// <summary>
        /// Retrieves the merge sources and adds them to the <code>ComboBox</code>.
        /// </summary>
        ICollection<Uri> RetrieveMergeSources()
        {
            return WizardPage.GetMergeSources(MergeTarget);
        }

        SvnItem MergeTarget
        {
            get { return ((MergeWizard)WizardPage.Wizard).MergeTarget; }
        }

        void MergesRetrieved(IAsyncResult result)
        {
            ICollection<Uri> mergeSources = 
                _retrieveMergeSources.EndInvoke(_mergeRetrieveResult);
            SetMergeSources(mergeSources);
        }

        delegate void SetMergeSourcesCallBack(ICollection<Uri> mergeSources);
        delegate ICollection<Uri> MergeSources();

        /// <summary>
        /// Displays the Repository Folder Dialog
        /// </summary>
        void selectButton_Click(object sender, EventArgs e)
        {
            Uri uri = UIUtils.DisplayBrowseDialogAndGetResult(
                WizardPage,
                MergeTarget,
                MergeTarget.Status.Uri);

            if (uri != null)
                mergeFromComboBox.Text = uri.ToString();
        }
    }
}
