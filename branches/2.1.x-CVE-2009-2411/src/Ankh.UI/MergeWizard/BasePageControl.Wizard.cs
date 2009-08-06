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
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{    
    public partial class BasePage
    {
        protected BasePage(Wizard wizard, string name)
            : base(name)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");

            Wizard = wizard;
            InitializeComponent();
        }

        protected BasePage(Wizard wizard, string name, Image image)
            : base(name, image)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");

            Wizard = wizard;
            InitializeComponent();
        }

        Wizard _wizard;
        /// <summary>
        /// Gets or sets the wizard.
        /// </summary>
        /// <value>The wizard.</value>
        public new Wizard Wizard
        {
            get { return _wizard ?? (_wizard = (Wizard)base.Wizard); }
            set
            {
                _wizard = value;
                base.Wizard = value;

                // HACK: Should not have intimate knowledge about mergewizard, refactor
                MergeWizard mergeWizard = value as MergeWizard;
                if(mergeWizard != null)
                {
                    mergeWizard.WizardDialog.PageChanged += new EventHandler<WizardPageChangedEventArgs>(WizardDialog_PageChangeEvent);
                    mergeWizard.WizardDialog.PageChanging += new EventHandler<WizardPageChangingEventArgs>(WizardDialog_PageChangingEvent);
                }
            }
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
