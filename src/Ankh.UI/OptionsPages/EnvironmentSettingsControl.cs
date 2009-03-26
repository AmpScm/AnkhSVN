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

        private IAnkhConfigurationService _configSvc;
        IAnkhConfigurationService ConfigSvc
        {
            get { return _configSvc ?? (_configSvc = Context.GetService<IAnkhConfigurationService>()); }
        }
        AnkhConfig _config;
        AnkhConfig Config
        {
            get
            {
                return _config ?? (_config = ConfigSvc.Instance);
            }
        }

        public override void SaveSettings()
        {
            base.SaveSettings();

            Config.InteractiveMergeOnConflict = interactiveMergeOnConflict.Checked;
            Config.AutoAddEnabled = autoAddFiles.Checked;
            Config.FlashWindowWhenOperationCompletes = flashWindowAfterOperation.Checked;

            try
            {
                ConfigSvc.SaveConfig(Config);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = Context.GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                {
                    eh.OnError(ex);
                    return;
                }

                throw;
            }
        }

        public override void LoadSettings()
        {
            base.LoadSettings();

            ConfigSvc.LoadConfig();

            interactiveMergeOnConflict.Checked = Config.InteractiveMergeOnConflict;
            autoAddFiles.Checked = Config.AutoAddEnabled;
            flashWindowAfterOperation.Checked = Config.FlashWindowWhenOperationCompletes;
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
