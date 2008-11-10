using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

/* 
 * WizardDialog.cs
 * 
 * Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net,
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/
namespace WizardFramework
{
    /// <summary>
    /// A dialog to display a wizard to the end user.
    /// </summary>
    public partial class WizardDialog : Form, IWizardContainer, IWizardPageChangeProvider
    {
        /// <summary>
        /// Default parameterless constructor required by VS.NET Designer.
        /// Do not use this constructor when trying to instantiate a
        /// WizardDialog.
        /// </summary>
        protected WizardDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The preferred constructor for creating the WizardDialog with an
        /// associated wizard.
        /// </summary>
        /// <param name="wizard">The default wizard.</param>
        public WizardDialog(IWizard wizard) : this()
        {
            this.wizard_prop = wizard;
            this.wizard_prop.Container = this;

            InitializeDialog();
        }

        #region WizardDialog Members
        /// <summary>
        /// Shows the starting page of the wizard.
        /// </summary>
        private void ShowStartingPage()
        {
            ShowPage(this.wizard_prop.StartingPage);
        }

        /// <summary>
        /// Performance any pre-display initialization for the dialog,
        /// the wizard and the framework.
        /// </summary>
        private void InitializeDialog()
        {
            if (this.wizard_prop != null)
            {
                this.wizard_prop.AddPages();
            }

            ShowStartingPage();
        }

        /// <summary>
        /// Update the receiver for the new page.
        /// </summary>
        /// <param name="page">New <c>IWizardPage</c></param>
        private void UpdateForPage(IWizardPage page)
        {
            // ensure this page belongs to the current wizard
            if (this.Wizard != page.Wizard)
            {
                this.Wizard = page.Wizard;
            }

            // Make the page invisible so the new page can be displayed
            if (this.CurrentPage != null)
            {
                this.CurrentPage.Visible = false;
            }

            // Only add pages if they are not already added
            if (!PageContainer.Controls.Contains(page.Control))
            {
                PageContainer.Controls.Add(page.Control);
            }

            currPage_prop = page;

            page.Control.Visible = true;
            UpdateUI();
        }

        /// <summary>
        /// Method will update the UI components (form title, wizard title, wizard message,
        /// wizard image and buttons.)
        /// </summary>
        private void UpdateUI()
        {
            // Update the Form title
            UpdateFormTitle();
            // Update the Page title
            UpdateTitleBar();
            // Update the Page message
            UpdateMessage();
            // Update the buttons
            UpdateButtons();

            FirePageChangeEvent(new WizardPageChangeEventArgs(this, currPage_prop));
        }

        /// <summary>
        /// The current wizard for this dialog.
        /// </summary>
        protected IWizard Wizard
        {
            get { return wizard_prop; }
            set
            {
                wizard_prop = value;

                wizard_prop.Container = this;

                if (!createdWizards.Contains(wizard_prop))
                {
                    createdWizards.Add(wizard_prop);
                    nestedWizards.Add(wizard_prop);  // Add new wizards to the end of the nested list
                }
                else
                {
                    int size = nestedWizards.Count;

                    // We have already seen this wizard.  Remove it from the list if
                    // we are going back to view it.
                    if (size >= 2 && nestedWizards[size - 2] == wizard_prop) nestedWizards.Remove(nestedWizards[size - 1]);
                    // Assume we are going forward to revisit this wizard
                    else nestedWizards.Add(wizard_prop);
                }

                InitializeDialog();
            }
        }

        /// <summary>
        /// Disposes all wizards and nullifies their controls.
        /// </summary>
        private void HardClose()
        {
            foreach (IWizard wizard in createdWizards)
            {
                wizard.Dispose();

                wizard.Container = null;
            }

            base.Dispose();
        }

        private List<IWizard> createdWizards = new List<IWizard>();
        private IWizard wizard_prop = null;
        #endregion

        #region IWizardContainer Members
        /// <see cref="WizardFramework.IWizardContainer.CurrentPage" />
        public IWizardPage CurrentPage
        {
            get { return currPage_prop; }
        }

        /// <see cref="WizardFramework.IWizardContainer.ShowPage" />
        public void ShowPage(IWizardPage page)
        {
            if (page == null || page.Control == null)
            {
                return;
            }
            if (!this.isMovingToPreviousPage)
            {
                // remember my previous page.
                page.PreviousPage = this.CurrentPage;
            }
            else
            {
                this.isMovingToPreviousPage = false;
            }
            WizardPageChangingEventArgs pageChangingEventArgs = new WizardPageChangingEventArgs(currPage_prop, page);

            FirePageChangingEvent(pageChangingEventArgs);

            // Evaluate if changing is an option
            if (!pageChangingEventArgs.DoIt) return;

            UpdateForPage(page);
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
            bool canFinish = wizard_prop.CanFinish;
            backButton.Enabled = currPage_prop.PreviousPage != null;
            nextButton.Enabled = currPage_prop.CanFlipToNextPage;
            finishButton.Enabled = canFinish;

            if (finishButton.Enabled)
                this.AcceptButton = finishButton;
            else if (nextButton.Enabled)
                this.AcceptButton = nextButton;
            else
                this.AcceptButton = cancelButton;
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateTitleBar" />
        public void UpdateTitleBar()
        {
            if (currPage_prop == null) return;

            if (currPage_prop.Title != null && currPage_prop.Title != headerTitle.Text)
                headerTitle.Text = currPage_prop.Title;

            if (currPage_prop.Image != null && headerImage.Image != currPage_prop.Image)
                headerImage.Image = currPage_prop.Image;

            if (currPage_prop.Description != null && headerDescription.Text != currPage_prop.Description)
                headerDescription.Text = currPage_prop.Description;
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateFormTitle" />
        public void UpdateFormTitle()
        {
            if (wizard_prop.WindowTitle == null) return;

            if (wizard_prop.WindowTitle != this.Text) this.Text = wizard_prop.WindowTitle;
        }

        /// <see cref="WizardFramework.IWizardContainer.UpdateMessage" />
        public void UpdateMessage()
        {
            if (currPage_prop == null) return;

            WizardMessage message = currPage_prop.Message;

            if (message != null && message.Message != null)
            {
                // Display the message panel
                // Bora: statusIcon images are set be read from the MergeWizard resources.
                // otherwise, runtime throws an exception and quits the Wizard.
                if (message.Type == WizardMessage.MessageType.NONE)
                    statusIcon.Image = null;

                if (message.Type == WizardMessage.MessageType.ERROR && statusIcon.Image != Ankh.UI.MergeWizard.Resources.ErrorIcon)
                    statusIcon.Image = Ankh.UI.MergeWizard.Resources.ErrorIcon;

                if (message.Type == WizardMessage.MessageType.INFORMATION && statusIcon.Image != Ankh.UI.MergeWizard.Resources.InfoIcon)
                    statusIcon.Image = Ankh.UI.MergeWizard.Resources.InfoIcon;

                if (message.Type == WizardMessage.MessageType.WARNING && statusIcon.Image != Ankh.UI.MergeWizard.Resources.WarningIcon)
                    statusIcon.Image = Ankh.UI.MergeWizard.Resources.WarningIcon;

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

        /// <see cref="WizardFramework.IWizardContainer.Form" />
        public Form Form
        {
            get { return this; }
        }

        /// <see cref="WizardFramework.IWizardContainer.PageContainer" />
        public Panel PageContainer
        {
            get { return this.wizardPagePanel; }
        }

        private IWizardPage currPage_prop = null;
        #endregion

        #region IWizardPageChangeProvider Members
        /// <summary>
        /// Fires an event signifying a page change.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void FirePageChangeEvent(WizardPageChangeEventArgs e)
        {
            if (PageChangeEvent == null) return;

            PageChangeEvent(this, e);
        }

        /// <summary>
        /// Fires an event signifying a page is changing event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void FirePageChangingEvent(WizardPageChangingEventArgs e)
        {
            if (PageChangingEvent == null) return;

            PageChangingEvent(this, e);
        }

        /// <see cref="WizardFramework.IWizardPageChangeProvider.SelectedPage" />
        public IWizardPage SelectedPage
        {
            get { return selectedPage_prop; }
        }

        private IWizardPage selectedPage_prop = null;
        public event EventHandler<WizardPageChangeEventArgs> PageChangeEvent;
        public event EventHandler<WizardPageChangingEventArgs> PageChangingEvent;
        #endregion

        #region WizardDialog UI Event Handling
        /// <summary>
        /// Handles the clicking of the back button.
        /// </summary>
        private void backButton_Click(object sender, EventArgs e)
        {
            IWizardPage page = currPage_prop.PreviousPage;

            if (page == null) return; // Should never happen if the back button is enabled

            isMovingToPreviousPage = true;

            ShowPage(page); // Show the page
        }

        /// <summary>
        /// Handles the clicking of the Next button.
        /// </summary>
        private void nextButton_Click(object sender, EventArgs e)
        {
            IWizardPage page = currPage_prop.NextPage;

            if (page == null) return; // Should never happen if the next button is enabled

            ShowPage(page); // Show the page
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
            if (wizard_prop.PerformFinish())
            {
                for (int i = 0; i < nestedWizards.Count - 1; i++)
                {
                    nestedWizards[i].PerformFinish();
                }

                HardClose();
            }
        }

        /// <summary>
        /// Handles the clicking of the Cancel button.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            HardClose();
        }

        private List<IWizard> nestedWizards = new List<IWizard>();
        private bool isMovingToPreviousPage = false;
        #endregion

    }
}
