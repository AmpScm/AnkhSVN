using System;
using System.Collections.Generic;
using System.Text;

/* 
 * WizardPageChangeEventArgs.cs
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
    public sealed class WizardPageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The page change provider.</param>
        /// <param name="currPage">The current page displayed in the wizard.</param>
        internal WizardPageChangedEventArgs(IWizardPageChangeProvider provider, IWizardPage currPage)
        {
            currPage_prop = currPage;
            provider_prop = provider;
        }

        readonly IWizardPage currPage_prop;
        readonly IWizardPageChangeProvider provider_prop;

        /// <summary>
        /// Returns the page currently being displayed.
        /// </summary>
        public IWizardPage CurrentPage
        {
            get { return currPage_prop; }
        }

        /// <summary>
        /// Gets the page change provider.
        /// </summary>
        public IWizardPageChangeProvider Provider
        {
            get { return provider_prop; }
        }
    }
}