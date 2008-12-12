// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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
using System.IO;

namespace Ankh.UI
{
    public partial class AddWorkingCopyExplorerRootDialog : Form
    {


        public AddWorkingCopyExplorerRootDialog()
        {
            this.InitializeComponent();
        }

        public string NewRoot
        {
            get { return this.workingCopyRootTextBox.Text; }
        }


        private void workingCopyRootTextBox_TextChanged(object sender, EventArgs e)
        {
            this.okButton.Enabled = Directory.Exists(this.workingCopyRootTextBox.Text);
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				folderBrowser.ShowNewFolderButton = false;

				if (folderBrowser.ShowDialog(this) == DialogResult.OK)
				{
					this.workingCopyRootTextBox.Text = folderBrowser.SelectedPath;
				}
			}
        }
    }
}
