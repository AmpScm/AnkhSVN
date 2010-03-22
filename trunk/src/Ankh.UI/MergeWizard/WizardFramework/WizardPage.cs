using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Ankh;

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
namespace Ankh.UI.WizardFramework
{
    /// <summary>
    /// An abstract implementation of a wizard page.
    /// </summary>
    public class WizardPage : UserControl
    {
        Image _image;
        string _message;
        string _description;
        WizardMessage.MessageType _messageType;

        private bool _pageComplete = false;
        private WizardPage _prevPage = null;
        private string name_prop = null;
        bool _disposed;

        protected WizardPage()
        {
        }

		[Browsable(false)]
		public new string Name
		{
			get
			{
				string name = base.Name;

				if (!string.IsNullOrEmpty(name))
					return name;
				else
					return GetType().FullName;
			}
			set
			{
				base.Name = value;
			}
		}

		protected bool ShouldSerializeName()
		{
			return Name != GetType().FullName;
		}

        /// <summary>
        /// Create a named page with a non-default page image.
        /// </summary>
        /// <param name="name">The page name.</param>
        /// <param name="image">The page image.</param>
        protected WizardPage(string name, Image image)
            : this()
        {
            Name = name;
            Image = image;
        }

        IAnkhServiceProvider _context;
        public virtual IAnkhServiceProvider Context
        {
            get { return _context ?? FindContext(); }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

		private IAnkhServiceProvider FindContext()
		{
			throw new NotImplementedException();
		}

        protected virtual void OnContextChanged(EventArgs e)
        {
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
            get { return _pageComplete; }
            set
            {
                _pageComplete = value;
                if (Wizard != null && Wizard.Form != null)
                    Wizard.Form.UpdateButtons();
            }
        }  

        /// <see cref="WizardFramework.IWizardPage.NextPage" />
        public virtual WizardPage NextPage
        {
            get
            {
                if (Wizard == null) return null;

                return Wizard.GetNextPage(this);
            }
        }

        /// <see cref="WizardFramework.IWizardPage.PreviousPage" />
        public virtual WizardPage PreviousPage
        {
            get
            {
                if (_prevPage != null)
                    return _prevPage;

                if (Wizard != null)
					return Wizard.GetPreviousPage(this);
					
				return null;
            }
            set { _prevPage = value; }
        }

        /// <see cref="WizardFramework.IWizardPage.Description" />
		[DefaultValue("")]
        public virtual string Description
        {
            get { return _description ?? ""; }
            set
            {
                _description = value;

                if (Wizard != null && Wizard.Form != null)
                    Wizard.Form.UpdateTitleBar();
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Message" />
		[Browsable(false)]
        public virtual WizardMessage Message
        {
            get { return new WizardMessage(MessageText, MessageType); }
            set 
            {
				if (value == null)
				{
					MessageText = "";
					MessageType = WizardMessage.MessageType.None;
				}
				else
				{
					MessageText = value.Message;
					MessageType = value.Type;
				}
            }
        }

		[DefaultValue("")]
		public string MessageText
		{
			get { return _message ?? ""; }
			set { _message = value; }
		}

		[DefaultValue(WizardMessage.MessageType.None)]
		public WizardMessage.MessageType MessageType
		{
			get { return _messageType; }
			set { _messageType = value; }
		}

        /// <see cref="WizardFramework.IWizardPage.Image" />
        /// <para>If the page hasn't explicitly created an image
        /// for this page, return the wizard's default page image.</para>
        public virtual Image Image
        {
            get
            {
				if (_image != null)
					return _image;
				else if (Wizard != null)
					return Wizard.DefaultPageImage;

				return null;
            }

            set { _image = value; }
        }

		bool ShouldSerializeImage()
		{
			return _image != null;
		}

        /// <see cref="WizardFramework.IWizardPage.Title" />
		[Obsolete("Please use .Text")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Title
        {
            get { return Text; }
            set { Text = value; }
        }

		[Bindable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		public new Wizard Container
		{
			get
			{
				if (_collection != null)
					return _collection.Wizard;
				else
					return null;
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
				if (Wizard != null && Wizard.Form != null)
					return Wizard.Form.CurrentPage == this;
				else
					return false;
            }
        }
        
        #endregion

        bool _removing;
        WizardPageCollection _collection;

		protected internal virtual void OnBeforeAdd(WizardPageCollection collection)
		{
			if (!_removing && _collection != null)
				throw new InvalidOperationException("Can be part of only one wizard at once");
		}

		protected internal virtual void OnAfterAdd(WizardPageCollection collection)
		{
			_collection = collection;
		}

		protected internal virtual void OnBeforeRemove(WizardPageCollection collection)
		{
			_removing = true;
		}

		protected internal virtual void OnAfterRemove(WizardPageCollection wizardPageCollection)
		{
			_removing = false;
			_collection = null;
		}

		/// <summary>
		/// Gets the Wizard containing this page
		/// </summary>
		public Wizard Wizard
		{
			get
			{
				if (_collection != null)
					return _collection.Wizard;

				return null;
			}
		}
	}
}
