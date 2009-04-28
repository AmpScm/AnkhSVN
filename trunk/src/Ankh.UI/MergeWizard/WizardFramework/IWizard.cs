using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

/* 
 * IWizard.cs
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
    /// Interface for a wizard.  A wizard maintains a list of wizard pages
    /// in a stack.
    /// </summary>
    /// <para>The <code>Wizard</code> is an abstract implementation of this
    /// interface.  Feel free to implement this interface if the 
    /// <code>Wizard</code> does not fill your needs.</para>
    public interface IWizard : IDisposable
    {
        /// <summary>
        /// Adds pages to the wizard's stack.
        /// </summary>
        /// <para>This method is called just before the wizard becomes
        /// visible.</para>
        void AddPages();

        /// <summary>
        /// Gets whether the next button should be shown as finish on this page
        /// </summary>
        /// <returns>Returns <code>true</code> if the wizard finishes on nect
        /// or <code>false</code> if it is not.</returns>
        bool NextIsFinish { get; }

        /// <summary>
        /// Returns the successor for the given page.
        /// </summary>
        /// <para>This method is typically called by a wizard page.</para>
        /// <param name="page">The page.</param>
        /// <returns>The next page or <code>null</code> if there is none.</returns>
        IWizardPage GetNextPage(IWizardPage page);

        /// <summary>
        /// Returns the wizard page with the given name.
        /// </summary>
        /// <param name="pageName">The page's name.</param>
        /// <returns>The page corresponding with the given name.</returns>
        IWizardPage GetPage(string pageName);

        /// <summary>
        /// Returns the the predecessor of the given page.
        /// </summary>
        /// <para>This method is typically called by a wizard page.</para>
        /// <param name="page">The page.</param>
        /// <returns>The previous page or <code>null</code> if there is none.</returns>
        IWizardPage GetPreviousPage(IWizardPage page);

        /// <summary>
        /// Perform any necessary actions corresponding with the user clicking
        /// Cancel or refusing if cancel is not allowed.
        /// </summary>
        /// <returns>Returns <code>true</code> if cancel is allowed and successful
        /// or <code>false</code> otherwise.</returns>
        bool PerformCancel();

        /// <summary>
        /// Perform any necessary actions corresponding with the user clicking
        /// Finish or refuse if finishing is not an available now.
        /// </summary>
        /// <returns>Returns <code>true</code> if finish is allowed and successful
        /// or <code>false</code> otherwise.</returns>
        bool PerformFinish();

        /// <summary>
        /// Gets/Sets the container for this wizard.
        /// </summary>
        IWizardContainer Container { get; set; }

        /// <summary>
        /// Gets the window title for this wizard.
        /// </summary>
        string WindowTitle { get; }

        /// <summary>
        /// Gets the first page of the wizard.
        /// </summary>
        IWizardPage StartingPage { get; }

        /// <summary>
        /// Get the number of pages for this wizard.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Get the list of pages for this wizard.
        /// </summary>
        List<IWizardPage> Pages { get; }

        /// <summary>
        /// Get the default image for this wizard.
        /// </summary>
        Image DefaultPageImage { get; }
    }
}