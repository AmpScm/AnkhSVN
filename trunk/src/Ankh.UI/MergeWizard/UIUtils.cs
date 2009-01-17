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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SharpSvn;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.RepositoryOpen;
using WizardFramework;
using System.IO;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This class contains utility methods for the UI.
    /// </summary>
    static class UIUtils
    {
        /// <summary>
        /// Resizes a <code>System.Windows.Forms.ComboBox</code>'s DropDownWidth
        /// based on the longest string in the list.
        /// </summary>
        public static void ResizeDropDownForLongestEntry(ComboBox comboBox)
        {
            int width = comboBox.DropDownWidth;
            using (Graphics g = comboBox.CreateGraphics())
            {
                Font font = comboBox.Font;
                int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;
                int newWidth;

                foreach (object o in comboBox.Items)
                {
                    string s = "";

                    if (o is string)
                        s = (string)o;
                    else if (o is KeyValuePair<SvnDepth, string>)
                    {
                        s = ((KeyValuePair<SvnDepth, string>)o).Value;
                    }
                    else
                    {
                        return;
                    }

                    newWidth = (int)g.MeasureString(s, font).Width
                        + vertScrollBarWidth;
                    if (width < newWidth)
                    {
                        width = newWidth;
                    }
                }
                comboBox.DropDownWidth = width;
            }
        }
        public static Uri DisplayBrowseDialogAndGetResult(WizardPage page, SvnItem target, string baseUri)
        {
            Uri u;
            if(Uri.TryCreate(baseUri, UriKind.Absolute, out u))
                return u;
            
            page.Message = MergeUtils.INVALID_FROM_URL;

            return null;
        }
        public static Uri DisplayBrowseDialogAndGetResult(WizardPage page, SvnItem target, Uri baseUri)
        {
            IAnkhServiceProvider context = ((MergeWizard)page.Wizard).Context;

            if (((MergeWizard)page.Wizard).MergeTarget.IsDirectory)
            {
                using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
                {
                    dlg.SelectedUri = baseUri;

                    if (dlg.ShowDialog(context) == DialogResult.OK)
                    {
                        return dlg.SelectedUri;
                    }
                }
            }
            else
            {
                using (RepositoryOpenDialog dlg = new RepositoryOpenDialog())
                {
                    string fileName = Path.GetFileName(target.FullPath);

                    dlg.Context = context;
                    dlg.Filter = fileName + "|" + fileName + "|All Files (*.*)|*";


                    dlg.SelectedUri = baseUri;

                    if (dlg.ShowDialog(context) == DialogResult.OK)
                        return dlg.SelectedUri;
                }
            }

            return null;
        }
    }
}
