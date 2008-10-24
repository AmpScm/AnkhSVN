using System;
using System.Collections.Generic;
using System.Text;

/* 
 * WizardPageChangingEventArgs.cs
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
    public class WizardPageChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentPage">The current page displayed in the wizard.</param>
        /// <param name="targetPage">The target page attempting to be switched to.</param>
        internal WizardPageChangingEventArgs(IWizardPage currentPage, IWizardPage targetPage)
        {
            this.currPage_prop = currentPage;
            this.tarPage_prop = targetPage;
        }

        #region WizardPageChangeEventArgs Properties
        private IWizardPage currPage_prop;
        private IWizardPage tarPage_prop;
        #endregion

        #region WizardPageChangeEventArgs Property Implementations
        /// <summary>
        /// Returns the page currently being displayed.
        /// </summary>
        public IWizardPage CurrentPage
        {
            get { return currPage_prop; }
        }

        /// <summary>
        /// Retuns the page attempting to be changed to.
        /// </summary>
        public IWizardPage TargetPage
        {
            get { return tarPage_prop; }
        }

        /// <summary>
        /// Public member used to signify if the page will change successfully.
        /// </summary>
        public bool DoIt = true;
        #endregion
    }
}
