using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

/* 
 * IWizardContainer.cs
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
    /// Interface for containers that can host a wizard.
    /// </summary>
    /// <para><code>WizardDialog</code>
    /// is a fully-functional implementation of this interface that should suffice
    /// for most implementations.  Feel free to implement this interface if the
    /// <code>WizardDialog</code> does not suit your needs.</para>
    public interface IWizardContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void ShowPage(IWizardPage page);

        /// <summary>
        /// Updates the state of the Back, Next, Finish and Cancel buttons.
        /// </summary>
        /// <para>This method is called by the container whenever the user clicks
        /// a button changing the page.  It can also be called directly by a page
        /// to manually update the button state.</para>
        void UpdateButtons();

        /// <summary>
        /// Updates the title bar, which contains a title, description
        /// and image, to reflect the state of the current page.
        /// </summary>
        void UpdateTitleBar();

        /// <summary>
        /// Updates the form title and image to reflect the state of the currently
        /// active page.
        /// </summary>
        /// <para>This method is called by the container whenever the user clicks
        /// a button changing the page.  It can also be called directly by a page
        /// to manually update the button state.</para>
        void UpdateFormTitle();

        /// <summary>
        /// Updates the message label to reflect the current page's state.
        /// </summary>
        void UpdateMessage();

        /// <summary>
        /// Get the form containing the given wizard.
        /// </summary>
        Form Form { get; }

        /// <summary>
        /// Get the current wizard page for this container.
        /// </summary>
        /// <see cref="ShowPage" />
        IWizardPage CurrentPage { get; }

        /// <summary>
        /// Get the panel that will contain the wizard pages.
        /// </summary>
        Panel PageContainer { get; }
    }
}