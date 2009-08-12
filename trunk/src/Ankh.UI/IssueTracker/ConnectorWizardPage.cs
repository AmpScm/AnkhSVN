// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Windows.Forms;

using WizardFramework;

using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizardPage : WizardPage
    {
        UserControl _pageControl;
        IssueRepositoryConfigurationPage _configPage;

        public ConnectorWizardPage(string name, IssueRepositoryConfigurationPage configPage)
            : base(name)
        {
            _configPage = configPage;
            if (_configPage != null)
            {
                _configPage.OnPageEvent += new EventHandler<ConfigPageEventArgs>(_configPage_OnPageEvent);
            }
        }

        void _configPage_OnPageEvent(object sender, ConfigPageEventArgs e)
        {
            if (e.IsComplete)
            {
                IsPageComplete = true;
                Message = null;
            }
            else
            {
                IsPageComplete = false;
                if (e.Exception != null)
                {
                    Message = new WizardMessage(e.Exception.Message, WizardMessage.MessageType.Error);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // pass the current settings to the config page for editing
            if (_configPage != null)
            {
                _configPage.Settings = Wizard.SolutionSettings;
            }
        }

        public override UserControl Control
        {
            get
            {
                UserControl control = base.Control;
                if (_pageControl == null
                    && _configPage != null
                    && _configPage.Window != null)
                {
                    try
                    {
                        IWin32Window window = _configPage.Window;
                        _pageControl = UserControl.FromHandle(window.Handle) as UserControl;
                        if (control != null)
                        {
                            control.Controls.Clear();
                            _pageControl.Dock = DockStyle.Fill;
                            control.Controls.Add(_pageControl);
                        }
                    }
                    catch { }
                }
                return control;
            }
        }

        public new ConnectorWizard Wizard
        {
            get
            {
                return base.Wizard as ConnectorWizard;
            }
            set
            {
                base.Wizard = value;
            }
        }

    }
}
