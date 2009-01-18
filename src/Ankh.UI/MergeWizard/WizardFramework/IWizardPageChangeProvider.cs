using System;
using System.Collections.Generic;
using System.Text;

/* 
 * IWizardPageChangeProvider.cs
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
    /// Minimal implementation of a page change provider.  Useful for
    /// dialogs that host wizards with more than one page.
    /// </summary>
    public interface IWizardPageChangeProvider
    {
        /// <summary>
        /// Event for handling wizard page change events.
        /// </summary>
        event EventHandler<WizardPageChangeEventArgs> PageChanged;

        /// <summary>
        /// Event for when a page is changing.
        /// </summary>
        event EventHandler<WizardPageChangingEventArgs> PageChanging;

        /// <summary>
        /// Returns the currently selected page in the wizard or returns
        /// <code>null</code> if no page is selected.
        /// </summary>
        IWizardPage SelectedPage { get; }
    }
}