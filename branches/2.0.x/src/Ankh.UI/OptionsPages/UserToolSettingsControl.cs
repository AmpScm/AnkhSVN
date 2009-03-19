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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms.Design;
using Ankh.Configuration;
using Ankh.Scc.UI;

namespace Ankh.UI.OptionsPages
{
    public partial class UserToolSettingsControl : AnkhOptionsPageControl
    {
        public UserToolSettingsControl()
        {
            InitializeComponent();
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

        public override void LoadSettings()
        {
            base.LoadSettings();
            IAnkhConfigurationService cfgSvc = Context.GetService<IAnkhConfigurationService>();

            cfgSvc.LoadConfig(); // Load most recent settings from registry
            _config = null;

            IAnkhDiffHandler diff = Context.GetService<IAnkhDiffHandler>();

            LoadBox(diffExeBox, Config.DiffExePath, diff.DiffToolTemplates);
            LoadBox(mergeExeBox, Config.MergeExePath, diff.MergeToolTemplates);
            LoadBox(patchExeBox, Config.PatchExePath, diff.PatchToolTemplates);

            interactiveMergeOnConflict.Checked = Config.InteractiveMergeOnConflict;
        }

        class OtherTool
        {
            string _title;
            public OtherTool(string title)
            {
                _title = string.IsNullOrEmpty(title) ? "Other..." : title;
            }
            public override string ToString()
            {
                return _title;
            }

            public string Title
            {
                get { return _title; }
            }

            public string DisplayName
            {
                get { return Title; }
            }
        }

        void LoadBox(ComboBox combo, string value, IList<AnkhDiffTool> tools)
        {
            if (combo == null)
                throw new ArgumentNullException("combo");

            combo.DropDownStyle = ComboBoxStyle.DropDown;
            combo.Items.Clear();

            string selectedName = string.IsNullOrEmpty(value) ? null : AnkhDiffTool.GetToolNameFromTemplate(value);
            bool search = !string.IsNullOrEmpty(selectedName);
            bool found = false;
            foreach (AnkhDiffTool tool in tools)
            {
                // Items are presorted
                combo.Items.Add(tool);

                if (search && string.Equals(tool.Name, selectedName, StringComparison.OrdinalIgnoreCase))
                {
                    search = false;
                    found = true;
                    combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    combo.SelectedItem = tool;
                }
            }

            combo.Items.Add(new OtherTool(null));

            if (!found)
            {
                combo.SelectedItem = null;
                combo.Text = value ?? "";
            }
        }


        public override void SaveSettings()
        {
            base.LoadSettings();

            Config.DiffExePath = SaveBox(diffExeBox);
            Config.MergeExePath = SaveBox(mergeExeBox);
            Config.PatchExePath = SaveBox(patchExeBox);
            Config.InteractiveMergeOnConflict = interactiveMergeOnConflict.Checked;

            IAnkhConfigurationService cfgSvc = Context.GetService<IAnkhConfigurationService>();
            try
            {
                cfgSvc.SaveConfig(Config);
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

        string SaveBox(ComboBox box)
        {
            if (box == null)
                throw new ArgumentNullException("box");

            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            if (tool != null)
                return tool.ToolTemplate;

            return box.Text;
        }

        void BrowseCombo(ComboBox box)
        {
            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            string line;
            if (tool != null)
            {
                line = string.Format("\"{0}\" {1}", tool.Program, tool.Arguments);
            }
            else
                line = box.Text;

            using (ToolArgumentDialog dlg = new ToolArgumentDialog())
            {
                dlg.Value = line;
                dlg.SetTemplates(Context.GetService<IAnkhDiffHandler>().ArgumentDefinitions);

                if (DialogResult.OK == dlg.ShowDialog(Context))
                {
                    string newValue = dlg.Value;

                    if (!string.IsNullOrEmpty(newValue) && newValue != line)
                    {
                        box.DropDownStyle = ComboBoxStyle.DropDown;
                        box.SelectedItem = null;
                        box.Text = newValue;
                    }
                }
            }
        }

        private void diffExeBox_TextChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            if (box.DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (box.SelectedItem == null && !string.IsNullOrEmpty(box.Text))
                {
                    box.Tag = box.Text;
                }
            }
        }
        
        private void tool_selectionCommitted(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            if (tool != null)
            {
                box.DropDownStyle = ComboBoxStyle.DropDownList;
                box.Tag = tool.ToolTemplate;
            }
            else
            {
                box.DropDownStyle = ComboBoxStyle.DropDown;
                if (box.SelectedItem != null)
                    box.SelectedItem = null;
                if (box.Tag is string)
                    box.Text = (string)box.Tag;
                else if (box.Tag is AnkhDiffTool)
                    box.Text = ((AnkhDiffTool)box.Tag).ToolTemplate;

                if (!string.IsNullOrEmpty(box.Text))
                {
                    box.SelectionStart = 0;
                    box.SelectionLength = box.Text.Length;
                }
            }
        }

        private void diffBrowseBtn_Click(object sender, EventArgs e)
        {
            BrowseCombo(diffExeBox);
        }        

        private void mergeBrowseBtn_Click(object sender, EventArgs e)
        {
            BrowseCombo(mergeExeBox);
        }

        private void patchBrowseBtn_Click(object sender, EventArgs e)
        {
            BrowseCombo(patchExeBox);
        }        
    }
}
