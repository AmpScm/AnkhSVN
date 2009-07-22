using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

/* 
 * Wizard.cs
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
    /// An abstract implementation of a wizard.
    /// </summary>
    /// <para>The most common scenario is that
    /// you will subclass this to implement your own wizard.</para>
    public abstract class Wizard : Component,IWizard
    {
        bool _disposed;
        #region IWizard Members

        /// <summary>
        /// Handle disposing the UI stuff maintained in this wizard.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed)
                    return;

                if(disposing)
                {
                    foreach (IWizardPage page in pages_prop)
                    {
                        page.Dispose();
                    }
                }
            }
            finally
            {
                _disposed = true;
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// This method does nothing for this wizard.  Wizards subclassing
        /// this can implement this method themselves when necessary.
        /// </summary>
        /// <see cref="WizardFramework.IWizard.AddPages" />
        public virtual void AddPages() { }

        /// <see cref="WizardFramework.IWizard.CanFinish" />
        public virtual bool CanFinish
        {
            get
            {
                foreach (IWizardPage page in pages_prop)
                {
                    if (!page.IsPageComplete) return false;
                }

                return true;
            }
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public virtual IWizardPage GetNextPage(IWizardPage page)
        {
            int index = pages_prop.IndexOf(page);

            if (index == pages_prop.Count - 1 || index == -1) return null;

            return pages_prop[index + 1];
        }

        /// <see cref="WizardFramework.IWizard.GetPage" />
        public virtual IWizardPage GetPage(string pageName)
        {
            foreach (IWizardPage page in pages_prop)
            {
                if (page.PageName == pageName) return page;
            }

            return null;
        }

        /// <see cref="WizardFramework.IWizard.PageCount" />
        public virtual int PageCount
        {
            get { return pages_prop.Count; }
        }

        /// <see cref="WizardFramework.IWizard.Pages" />
        public virtual List<IWizardPage> Pages
        {
            get { return pages_prop; }
        }

        /// <see cref="WizardFramework.IWizard.GetPreviousPage" />
        public virtual IWizardPage GetPreviousPage(IWizardPage page)
        {
            int index = pages_prop.IndexOf(page);

            if (index == 0 || index == -1) return null;

            return pages_prop[index - 1];
        }

        /// <see cref="WizardFramework.IWizard.StartingPage" />
        public virtual IWizardPage StartingPage
        {
            get
            {
                if (pages_prop.Count == 0) return null;

                return pages_prop[0];
            }
        }

        /// <see cref="WizardFramework.IWizard.PerformCancel" />
        /// <para>Wizard does nothing here.  Subclasses should override
        /// if they need to perform any custom cancel steps.</para>
        public virtual bool PerformCancel()
        {
            return true;
        }

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public abstract bool PerformFinish();

        /// <see cref="WizardFramework.IWizard.Container" />
        public new virtual IWizardContainer Container
        {
            get
            {
                return container_prop;
            }
            set
            {
                container_prop = value;
            }
        }

        /// <see cref="WizardFramework.IWizard.WindowTitle" />
        public virtual string WindowTitle
        {
            get { return windowTitle_prop; }
            set
            {
                windowTitle_prop = value;

                if (container_prop != null) container_prop.Form.Text = windowTitle_prop;
            }
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        /// <para>Returns null for now.</para>
        public virtual Image DefaultPageImage
        {
            get
            {
                return null; // TODO: Change to a default image later.
            }
        }

        private IWizardContainer container_prop = null;
        private List<IWizardPage> pages_prop = new List<IWizardPage>();
        private string windowTitle_prop = null;
        #endregion

        #region Wizard Members
        protected Wizard() { }

        /// <summary>
        /// Adds a new page to this wizard.  The page is inserted at
        /// the end of the page list.
        /// </summary>
        /// <param name="page">The page.</param>
        public virtual void AddPage(IWizardPage page)
        {
            if (page == null) return;

            if (GetPage(page.PageName) == null) this.pages_prop.Add(page);

            page.Wizard = this;
        }

        /// <see cref="WizardFramework.IWizard.Form" />
        public virtual Form Form
        {
            get
            {
                if (container_prop != null)
                    return container_prop.Form;
                else
                    return null;
            }
        }
        #endregion
    }
}