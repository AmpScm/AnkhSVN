using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Ankh;
using System.Collections.ObjectModel;

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
namespace Ankh.UI.WizardFramework
{
	public class WizardPageCollection : KeyedCollection<string, WizardPage>
	{
		Wizard _wizard;
		public WizardPageCollection(Wizard wizard)
		{
			if (wizard == null)
				throw new ArgumentNullException("wizard");

			_wizard = wizard;
		}

		protected override string GetKeyForItem(WizardPage item)
		{
			return item.Name;
		}

		protected override void InsertItem(int index, WizardPage item)
		{
			item.OnBeforeAdd(this);
			base.InsertItem(index, item);
			item.OnAfterAdd(this);
		}

		protected override void SetItem(int index, WizardPage item)
		{
			WizardPage oldItem = this[index];
			oldItem.OnBeforeRemove(this);
			item.OnBeforeAdd(this);
			base.SetItem(index, item);
			oldItem.OnAfterRemove(this);
			item.OnAfterAdd(this);
		}

		protected override void RemoveItem(int index)
		{
			WizardPage oldItem = this[index];
			oldItem.OnBeforeRemove(this);
			base.RemoveItem(index);
			oldItem.OnBeforeRemove(this);
		}

		public Wizard Wizard
		{
			get { return _wizard; }
		}
	}


    /// <summary>
    /// An abstract implementation of a wizard.
    /// </summary>
    /// <para>The most common scenario is that
    /// you will subclass this to implement your own wizard.</para>
    public class Wizard : Component
    {
        readonly WizardPageCollection _pages;
		WizardDialog _container;
        IAnkhServiceProvider _context;
        Image _defaultImage;
		string _text;

        protected Wizard() 
        { 
        }

        protected Wizard(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _pages = new WizardPageCollection(this);
            _context = context;
        }

		/// <summary>
		/// 
		/// </summary>
        public WizardPageCollection Pages
        {
            get { return _pages; }
        }

		public WizardDialog Form
		{
			get { return _container; }
			set { _container = value; }
		}

        #region IWizard Members

		bool _disposed;
        /// <summary>
        /// Handle disposing the UI stuff maintained in this wizard.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                }
            }
            finally
            {
                _disposed = true;
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        /// <summary>
        /// This method does nothing for this wizard.  Wizards subclassing
        /// this can implement this method themselves when necessary.
        /// </summary>
        /// <see cref="WizardFramework.IWizard.AddPages" />
        public virtual void AddPages()
		{ 
		}

        /// <see cref="WizardFramework.IWizard.CanFinish" />
        public virtual bool NextIsFinish
        {
            get
            {
                foreach (WizardPage page in Pages)
                {
                    if (!page.IsPageComplete)
                        return false;
                }

                return true;
            }
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public virtual WizardPage GetNextPage(WizardPage page)
        {
            int index = Pages.IndexOf(page);

            if (index+1 >= Pages.Count || index < 0) 
                return null;

            return Pages[index + 1];
        }

        /// <see cref="WizardFramework.IWizard.GetPage" />
        public virtual WizardPage GetPage(string pageName)
        {
            if (Pages.Contains(pageName))
                return Pages[pageName];

            return null;
        }

        /// <see cref="WizardFramework.IWizard.PageCount" />
        public virtual int PageCount
        {
            get { return Pages.Count; }
        }

        /// <see cref="WizardFramework.IWizard.GetPreviousPage" />
        public virtual WizardPage GetPreviousPage(WizardPage page)
        {
            int index = Pages.IndexOf(page);

            if (index <= 0)
                return null;

            return Pages[index - 1];
        }

        /// <see cref="WizardFramework.IWizard.StartingPage" />
        public virtual WizardPage StartingPage
        {
            get
            {
                if (Pages.Count == 0) return null;

                return Pages[0];
            }
        }

        /// <see cref="WizardFramework.IWizard.PerformCancel" />
        /// <para>Wizard does nothing here.  Subclasses should override
        /// if they need to perform any custom cancel steps.</para>
        public virtual void OnCancel(CancelEventArgs e)
        {
        }

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public virtual void OnFinish(CancelEventArgs e)
        {
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        /// <para>Returns null for now.</para>
		[DefaultValue(null)]
        public virtual Image DefaultPageImage
        {
            get { return _defaultImage; }
            set { _defaultImage = value; }
        }

        #endregion

		[Localizable(true)]
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		[Obsolete("Use .Text")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string WindowTitle
		{
			get { return Text; }
			set { Text = value; }
		}
    }
}