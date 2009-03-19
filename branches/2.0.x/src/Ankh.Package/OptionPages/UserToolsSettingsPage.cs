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
using Microsoft.VisualStudio.Shell;
using Ankh.UI;
using Ankh.UI.OptionsPages;

namespace Ankh.VSPackage
{
    class UserToolsSettingsPage : DialogPage
    {
        UserToolSettingsControl _control;
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
                return Control;
            }
        }

        UserToolSettingsControl Control
        {
            get
            {
                return _control ?? (_control = CreateControl());
            }
        }

        UserToolSettingsControl CreateControl()
        {
            UserToolSettingsControl control = new UserToolSettingsControl();
            IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));

            if (sp != null)
                control.Context = sp;

            return control;
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            Control.LoadSettings();
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            Control.SaveSettings();

        }

        public override void ResetSettings()
        {
            base.ResetSettings();

            IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
            if (sp != null)
            {
                IAnkhConfigurationService cfgSvc = sp.GetService<IAnkhConfigurationService>();
                cfgSvc.LoadDefaultConfig();
            }
        }
    }
}
