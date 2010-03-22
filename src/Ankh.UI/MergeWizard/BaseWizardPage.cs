// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using Ankh.UI.WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class BaseWizardPage : WizardPage
    {
        protected BaseWizardPage()
        {
            InitializeComponent();
        }

		protected internal override void OnAfterAdd(WizardPageCollection collection)
		{
			base.OnAfterAdd(collection);

			Wizard.Form.PageChanged += new EventHandler<WizardPageChangedEventArgs>(WizardDialog_PageChangeEvent);
			Wizard.Form.PageChanging += new EventHandler<WizardPageChangingEventArgs>(WizardDialog_PageChangingEvent);
		}


		void WizardDialog_PageChangingEvent(object sender, WizardPageChangingEventArgs e)
		{
			if (e.CurrentPage == this)
				OnPageChanging(e);
		}
		void WizardDialog_PageChangeEvent(object sender, WizardPageChangedEventArgs e)
		{
			if (e.CurrentPage.PreviousPage == this)
				OnPageChanged(e);
		}

		protected virtual void OnPageChanging(WizardPageChangingEventArgs e)
		{
		}

		protected virtual void OnPageChanged(WizardPageChangedEventArgs e)
		{
		}

    }
}
