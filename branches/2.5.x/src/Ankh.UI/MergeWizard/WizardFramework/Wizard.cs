// $Id: MergeConflictHandler.cs 6697 2009-04-24 20:58:59Z rhuijben $
//
// Copyright 2008-2010 The AnkhSVN Project.
// Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net.
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.UI.WizardFramework
{
    /// <summary>
    /// A dialog to display a wizard to the end user.
    /// </summary>
    public partial class Wizard : VSContainerForm
    {
        readonly string _nextText;
        readonly string _finishText;
        readonly WizardPageCollection _pages;
        Image _defaultImage;
        bool _isMovingToPreviousPage;

        protected Wizard()
        {
            InitializeComponent();

            _pages = new WizardPageCollection(this);
            _nextText = nextButton.Text;
            _finishText = finishButton.Text;
        }

        #region WizardDialog Members
        /// <summary>
        /// Shows the starting page of the wizard.
        /// </summary>
        private void ShowStartingPage()
        {
            ShowPage(StartingPage);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
                InitializeDialog();
        }

        /// <summary>
        /// Performance any pre-display initialization for the dialog,
        /// the wizard and the framework.
        /// </summary>
        private void InitializeDialog()
        {
            AddPages();
            ShowStartingPage();
        }

        /// <summary>
        /// Update the receiver for the new page.
        /// </summary>
        /// <param name="page">New <c>IWizardPage</c></param>
        private void UpdateForPage(WizardPage page)
        {
            // ensure this page belongs to the current wizard
            if (!Pages.Contains(page))
                Pages.Add(page);

            // Make the page invisible so the new page can be displayed
            if (CurrentPage != null && CurrentPage != page)
            {
                CurrentPage.Visible = false;
            }

            // Only add pages if they are not already added
            if (!PageContainer.Controls.Contains(page))
            {
                page.Size = PageContainer.Size;
                page.Dock = DockStyle.Fill;
                PageContainer.Controls.Add(page);
            }

            _curPage = page;

            page.Visible = true;
            UpdateUI();
        }

        /// <summary>
        /// Method will update the UI components (form title, wizard title, wizard message,
        /// wizard image and buttons.)
        /// </summary>
        private void UpdateUI()
        {
            // Update the Page title
            UpdateTitleBar();
            // Update the Page message
            UpdateMessage();
            // Update the buttons
            UpdateButtons();
        }

        #endregion

        #region IWizardContainer Members
        /// <see cref="WizardFramework.IWizardContainer.CurrentPage" />
        public WizardPage CurrentPage
        {
            get { return _curPage; }
        }

        /// <see cref="WizardFramework.IWizardContainer.ShowPage" />
        public void ShowPage(WizardPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (!_isMovingToPreviousPage)
                page.PreviousPage = this.CurrentPage; // Help going back

            _isMovingToPreviousPage = false;
            WizardPageChangingEventArgs pageChangingEventArgs = new WizardPageChangingEventArgs(page);

            OnPageChanging(pageChangingEventArgs);

            // Evaluate if changing is an option
            if (pageChangingEventArgs.Cancel)
                return;

            UpdateForPage(page);

            OnPageChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Allows the wizard and its pages to toggle the
        /// enablement of the page and buttons.
        /// </summary>
        public void EnablePageAndButtons(bool enabled)
        {
            EnablePage(enabled);
            EnableButtons(enabled);
        }

        /// <summary>
        /// Allows the wizard and its pages to toggle the
        /// enablement of the page.
        /// </summary>
        public void EnablePage(bool enabled)
        {
            PageContainer.Enabled = enabled;
        }

        /// <summary>
        /// Allows the wizard and its pages to toggle the
        /// enablement of the buttons.
        /// </summary>
        public void EnableButtons(bool enabled)
        {
            NavigationContainer.Enabled = enabled;
        }

        public Panel NavigationContainer
        {
            get
            {
                return controlPanel;
            }
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateButtons" />
        public void UpdateButtons()
        {
            bool isFinish = NextIsFinish;
            nextButton.Text = isFinish ? _finishText : _nextText;
            backButton.Enabled = _curPage.PreviousPage != null;
            nextButton.Enabled = isFinish ? _curPage.IsPageComplete : _curPage.CanFlipToNextPage;
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateTitleBar" />
        public void UpdateTitleBar()
        {
            if (_curPage == null)
                return;

            if (_curPage.Text != null)
            {
                string newText = _curPage.Text ?? "";
                if (newText != headerTitle.Text)
                    headerTitle.Text = newText;
            }

            if (_curPage.Description != null)
            {
                string newText = _curPage.Description ?? "";
                if (newText != headerDescription.Text)
                    headerDescription.Text = newText;
            }

            if (_curPage.Image != null && headerImage.Image != _curPage.Image)
                headerImage.Image = _curPage.Image ?? DefaultPageImage;
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateMessage" />
        public void UpdateMessage()
        {
            if (_curPage == null)
                return;

            WizardMessage message = _curPage.Message;

            if (message != null && !string.IsNullOrEmpty(message.Message))
            {
                Image newImg;
                // Display the message panel
                // Bora: statusIcon images are set be read from the MergeWizard resources.
                // otherwise, runtime throws an exception and quits the Wizard.
                switch (message.Type)
                {
                    case WizardMessage.MessageType.Error:
                        newImg = Ankh.UI.MergeWizard.MergeStrings.ErrorIcon;
                        break;
                    case WizardMessage.MessageType.Information:
                        newImg = Ankh.UI.MergeWizard.MergeStrings.InfoIcon;
                        break;
                    case WizardMessage.MessageType.Warning:
                        newImg = Ankh.UI.MergeWizard.MergeStrings.WarningIcon;
                        break;
                    case WizardMessage.MessageType.None:
                    default:
                        newImg = null;
                        break;
                }

                if (statusIcon.Image != newImg)
                    statusIcon.Image = newImg;

                if (message.Message != null && statusMessage.Text != message.Message)
                    statusMessage.Text = message.Message;

                statusPanel.Visible = true;
            }
            else
            {
                // Hide the message panel
                statusPanel.Visible = false;
            }
        }

        /// <see cref="WizardFramework.IWizardContainer.PageContainer" />
        public Panel PageContainer
        {
            get { return this.wizardPagePanel; }
        }

        private WizardPage _curPage = null;
        #endregion

        #region IWizardPageChangeProvider Members
        /// <summary>
        /// Fires an event signifying a page change.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPageChanged(EventArgs e)
        {
            if (PageChanged != null)
                PageChanged(this, e);
        }

        /// <summary>
        /// Fires an event signifying a page is changing event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPageChanging(WizardPageChangingEventArgs e)
        {
            if (PageChanging != null)
                PageChanging(this, e);
        }

        public event EventHandler PageChanged;
        public event EventHandler<WizardPageChangingEventArgs> PageChanging;
        #endregion

        #region WizardDialog UI Event Handling
        /// <summary>
        /// Handles the clicking of the back button.
        /// </summary>
        private void backButton_Click(object sender, EventArgs e)
        {
            WizardPage page = _curPage.PreviousPage;

            if (page == null) return; // Should never happen if the back button is enabled

            _isMovingToPreviousPage = true;

            ShowPage(page); // Show the page
        }

        /// <summary>
        /// Handles the clicking of the Next button.
        /// </summary>
        private void nextButton_Click(object sender, EventArgs e)
        {
            if (NextIsFinish)
            {
                finishButton_Click(sender, e);
            }
            else
            {
                WizardPage page = _curPage.NextPage;

                if (page == null)
                    return; // Should never happen if the next button is enabled

                ShowPage(page); // Show the page
            }
        }

        /// <summary>
        /// Handles the clicking of the Finish button.
        /// </summary>
        /// <para>The wizard framework allows for nested wizards using
        /// the set Wizard accessor.  Since the current wizard is always
        /// the last wizard in the stack, it's <code>PerformFinish</code>
        /// is called manually to allow for cancelation, work and/or state
        /// management.  Then each nested wizard's <code>PerformFinish</code>
        /// is called to allow for it do do work and/or save state.</para>
        private void finishButton_Click(object sender, EventArgs e)
        {
            CancelEventArgs ce = new CancelEventArgs();
            OnFinish(ce);
            if (!ce.Cancel)
            {
                for (int i = 0; i < nestedWizards.Count - 1; i++)
                {
                    nestedWizards[i].OnFinish(ce);
                }
            }
        }

        /// <summary>
        /// Handles the clicking of the Cancel button.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private List<Wizard> nestedWizards = new List<Wizard>();

        #endregion

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            headerTitle.Font = new Font(Font, FontStyle.Bold);
        }

        /// <summary>
        /// 
        /// </summary>
        public WizardPageCollection Pages
        {
            get { return _pages; }
        }

        /// <summary>
        /// This method does nothing for this wizard.  Wizards subclassing
        /// this can implement this method themselves when necessary.
        /// </summary>
        /// <see cref="WizardFramework.IWizard.AddPages" />
        public virtual void AddPages()
        {
        }

        /// <see cref="WizardFramework.IWizard.CanFinish" />
        public virtual bool NextIsFinish
        {
            get
            {
                foreach (WizardPage page in Pages)
                {
                    if (!page.IsPageComplete)
                        return false;
                }

                return true;
            }
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public virtual WizardPage GetNextPage(WizardPage page)
        {
            int index = Pages.IndexOf(page);

            if (index + 1 >= Pages.Count || index < 0)
                return null;

            return Pages[index + 1];
        }

        /// <see cref="WizardFramework.IWizard.GetPage" />
        public virtual WizardPage GetPage(string pageName)
        {
            if (Pages.Contains(pageName))
                return Pages[pageName];

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPage<T>() where T : WizardPage
        {
            foreach (WizardPage wp in Pages)
            {
                T twp = wp as T;

                if (twp != null)
                    return twp;
            }

            return null;
        }

        /// <see cref="WizardFramework.IWizard.PageCount" />
        public virtual int PageCount
        {
            get { return Pages.Count; }
        }

        /// <see cref="WizardFramework.IWizard.GetPreviousPage" />
        public virtual WizardPage GetPreviousPage(WizardPage page)
        {
            int index = Pages.IndexOf(page);

            if (index <= 0)
                return null;

            return Pages[index - 1];
        }

        /// <see cref="WizardFramework.IWizard.StartingPage" />
        public virtual WizardPage StartingPage
        {
            get
            {
                if (Pages.Count == 0)
                    return null;

                return Pages[0];
            }
        }

        /// <see cref="WizardFramework.IWizard.PerformCancel" />
        /// <para>Wizard does nothing here.  Subclasses should override
        /// if they need to perform any custom cancel steps.</para>
        public virtual void OnCancel(CancelEventArgs e)
        {
        }

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public virtual void OnFinish(CancelEventArgs e)
        {
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        /// <para>Returns null for now.</para>
        [DefaultValue(null)]
        public virtual Image DefaultPageImage
        {
            get { return _defaultImage; }
            set { _defaultImage = value; }
        }
    }
}
