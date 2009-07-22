using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

/* 
 * IWizardPage.cs
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
    /// Interface for a wizard page.
    /// </summary>
    /// <para><code>WizardPage</code> is an implementation
    /// of this interface.  Feel free to implement this interface if
    /// <code>WizardPage</code> does not suit your needs.</para>
    public interface IWizardPage : IDisposable
    {
        /// <summary>
        /// Returns whether the next page can be displayed.
        /// </summary>
        /// <returns>Returns <code>true</code> if the next page can
        /// be displayed or <code>false</code> otherwise.</returns>
        bool CanFlipToNextPage { get; }

        /// <summary>
        /// Returns whether the current page is complete.
        /// </summary>
        /// <para>This information is used by the wizard to decide when
        /// it is okay to finish.</para>
        /// <returns>Returns <code>true</code> if the page is complete
        /// or <code>false</code> otherwise.</returns>
        bool IsPageComplete { get; set; }

        /// <summary>
        /// Get the successor for the given page.
        /// </summary>
        /// <para>This method is typically called by a wizard page.</para>
        /// <param name="page">The page.</param>
        /// <returns>The next page or <code>null</code> if there is none.</returns>
        IWizardPage NextPage { get; }

        /// <summary>
        /// Gets/sets the previous page.
        /// </summary>
        /// <para>This method is typically called by a wizard page.s</para>
        /// <param name="page">The page.</param>
        /// <returns>The previous page or <code>null</code> if there is none.</returns>
        IWizardPage PreviousPage { get; set; }

        /// <summary>
        /// Get the name of this wizard page.
        /// </summary>
        string PageName { get; }

        /// <summary>
        /// Gets/Sets the wizard that this page is part of.
        /// </summary>
        IWizard Wizard { get; set; }

        /// <summary>
        /// Gets/Sets the description of the wizard page.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets/Sets the message for this wizard page.
        /// </summary>
        WizardMessage Message { get; set;  }

        /// <summary>
        /// Get/Sets the actual UserControl of the page.
        /// </summary>
        UserControl Control { get; }

        /// <summary>
        /// Get the image for the wizard dialog header.
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// Gets/Sets the title of the wizard page.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Sets the control's visibility.
        /// </summary>
        bool Visible { set; }
    }
}