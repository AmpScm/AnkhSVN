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

}