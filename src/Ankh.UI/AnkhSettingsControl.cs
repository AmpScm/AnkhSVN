// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Ankh.Commands;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms.Design;
using Ankh.UI.UITypeEditors;
using Ankh.Configuration;

namespace Ankh.UI
{
    public partial class AnkhSettingsControl : UserControl
    {
        IAnkhServiceProvider _context;
        public AnkhSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
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

        public void LoadSettings()
        {
            IAnkhConfigurationService cfgSvc = Context.GetService<IAnkhConfigurationService>();

            cfgSvc.LoadConfig(); // Load most recent settings from registry
            _config = null; 

            diffExeBox.Text = Config.DiffExePath;
            mergeExeBox.Text = Config.MergeExePath;
            interactiveMergeOnConflict.Checked = Config.InteractiveMergeOnConflict;
        }


        public void SaveSettings()
        {
            Config.DiffExePath = string.IsNullOrEmpty(diffExeBox.Text) ? null : diffExeBox.Text;
            Config.MergeExePath = string.IsNullOrEmpty(mergeExeBox.Text) ? null : mergeExeBox.Text;
            Config.InteractiveMergeOnConflict = interactiveMergeOnConflict.Checked;

            IAnkhConfigurationService cfgSvc = Context.GetService<IAnkhConfigurationService>();
            try
            {
                cfgSvc.SaveConfig(Config);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = Context.GetService<IAnkhErrorHandler>();

                if (handler != null)
                {
                    handler.OnError(ex);
                    return;
                }

                throw;
            }
        }

        private void btnDiffExePath_Click(object sender, EventArgs e)
        {
            DiffExeTypeEditor typeEditor = new DiffExeTypeEditor();
            diffExeBox.Text = (string)typeEditor.EditValue(Context, Config.DiffExePath);
        }

        private void btnMergePath_Click(object sender, EventArgs e)
        {
            MergeExeTypeEditor typeEditor = new MergeExeTypeEditor();
            mergeExeBox.Text = (string)typeEditor.EditValue(Context, Config.MergeExePath);
        }
    }
}
