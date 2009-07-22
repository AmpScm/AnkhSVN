using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

/* 
 * WizardPage.cs
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
    /// An abstract implementation of a wizard page.
    /// </summary>
    public abstract class WizardPage : Component, IWizardPage
    {
        bool _disposed;
        /// <summary>
        /// Constructor for a named wizard page.
        /// </summary>
        /// <param name="name"></param>
        protected WizardPage(string name)
        {
            name_prop = name;
        }

        /// <summary>
        /// Create a named page with a non-default page image.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <param name="image">The page image.</param>
        protected WizardPage(string name, Image image)
        {
            name_prop = name;
            image_prop = image;
        }
 
        #region IWizardPage Members

        /// <see cref="WizardFramework.IWizardPage.CanFlipToNextPage" />
        public virtual bool CanFlipToNextPage
        {
            get
            {
                return IsPageComplete && NextPage != null;
            }
        }

        /// <see cref="WizardFramework.IWizardPage.IsPageComplete" />
        public virtual bool IsPageComplete
        {
            get { return isPageComplete_prop; }
            set
            {
                isPageComplete_prop = value;
                if (this.IsCurrentPage)
                {
                    this.Container.UpdateButtons();
                }
            }
        }  

        /// <summary>
        /// Handle disposing the UI stuff maintained in this wizard page.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if(_disposed)
                    return;

                if(disposing)
                {
                    if (Control != null && !Control.IsDisposed)
                    {
                        Control.Dispose();
                    }

                    if (image_prop != null)
                    {
                        image_prop.Dispose();
                        image_prop = null;
                    }
                }
            }
            finally
            {
                _disposed = true;

                base.Dispose(disposing);
            }
        }

        /// <see cref="WizardFramework.IWizardPage.NextPage" />
        public virtual IWizardPage NextPage
        {
            get
            {
                if (wizard_prop == null) return null;

                return wizard_prop.GetNextPage(this);
            }
        }

        /// <see cref="WizardFramework.IWizardPage.PreviousPage" />
        public virtual IWizardPage PreviousPage
        {
            get
            {
                if (previousPage_prop != null) return previousPage_prop;

                if (wizard_prop == null) return null;

                return wizard_prop.GetPreviousPage(this);
            }
            set
            {
                previousPage_prop = value;
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Name" />
        public virtual string PageName
        {
            get { return name_prop; }
        }

        /// <see cref="WizardFramework.IWizardPage.Wizard" />
        public virtual IWizard Wizard
        {
            get
            {
                return wizard_prop;
            }
            set
            {
                wizard_prop = value;
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Description" />
        public virtual string Description
        {
            get
            {
                return description_prop;
            }
            set
            {
                description_prop = value;

                if (IsCurrentPage) Container.UpdateTitleBar();
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Message" />
        public virtual WizardMessage Message
        {
            get
            {
                return message_prop;
            }
            set
            {
                message_prop = value;

                if (Form != null)
                    ((WizardDialog)Form).UpdateMessage();
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public abstract UserControl Control { get; }

        /// <see cref="WizardFramework.IWizardPage.Image" />
        /// <para>If the page hasn't explicitly created an image
        /// for this page, return the wizard's default page image.</para>
        public virtual Image Image
        {
            get
            {
                if (image_prop == null) return wizard_prop.DefaultPageImage;
                else return image_prop;
            }

            set { image_prop = value; }
        }

        /// <see cref="WizardFramework.IWizardPage.Title" />
        public virtual string Title
        {
            get
            {
                return title_prop;
            }
            set
            {
                title_prop = value;
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Visible" />
        public bool Visible
        {
            set { Control.Visible = Control.Enabled = value; }
            get { return Control.Visible; }
        }

        /// <summary>
        /// Returns the container for the wizard that this
        /// page belongs to.
        /// </summary>
        protected new virtual IWizardContainer Container
        {
            get
            {
                if (wizard_prop == null) return null;

                return wizard_prop.Container;
            }
        }

        /// <summary>
        /// Returns the Form that contains the wizard
        /// that this page belongs to.
        /// </summary>
        public virtual Form Form
        {
            get
            {
                if (Container == null) return null;

                return Container.Form;
            }
        }

        /// <summary>
        /// Returns whether or not this page is the current page
        /// being displayed in the wizard.
        /// </summary>
        protected virtual bool IsCurrentPage
        {
            get
            {
                return (Container != null && this == Container.CurrentPage);
            }
        }

        private string title_prop = null;
        private WizardMessage message_prop = null;
        private string description_prop = null;
        private bool isPageComplete_prop = false;
        private IWizardPage previousPage_prop = null;
        private Image image_prop = null;
        private IWizard wizard_prop = null;
        private string name_prop = null;
        #endregion
    }
}