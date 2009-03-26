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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Configuration;

namespace Ankh.UI.OptionsPages
{
    public partial class EnvironmentSettingsControl : AnkhOptionsPageControl
    {
        public EnvironmentSettingsControl()
        {
            InitializeComponent();
        }

        private void authenticationEdit_Click(object sender, EventArgs e)
        {
            using (SvnAuthenticationCacheEditor editor = new SvnAuthenticationCacheEditor())
            {
                editor.ShowDialog(Context);
            }
        }

        AnkhConfig _config;
        AnkhConfig Config
        {
            get
            {
                if (_config == null)
                {
                    IAnkhConfigurationService configurationSvc = Context.GetService<IAnkhConfigurationService>();
                    _config = configurationSvc.Instance;
                }
                return _config;
            }
        }

        public override void SaveSettings()
        {
            base.SaveSettings();

            Config.InteractiveMergeOnConflict = interactiveMergeOnConflict.Checked;
            Config.AutoAddEnabled = autoAddFiles.Checked;
            Config.FlashWindowWhenOperationCompletes = flashWindowAfterOperation.Checked;
        }

        public override void LoadSettings()
        {
            base.LoadSettings();

            interactiveMergeOnConflict.Checked = Config.InteractiveMergeOnConflict;
            autoAddFiles.Checked = Config.AutoAddEnabled;
            flashWindowAfterOperation.Checked = Config.FlashWindowWhenOperationCompletes;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void proxyEdit_Click(object sender, EventArgs e)
        {
            using (SvnProxyEditor editor = new SvnProxyEditor())
            {
                editor.ShowDialog(Context);
            }
        }
    }
}
