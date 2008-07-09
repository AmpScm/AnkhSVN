﻿using System;
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
            Graphics g = comboBox.CreateGraphics();
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

        public static Uri DisplayBrowseDialogAndGetResult(WizardPage page, SvnItem target, string baseUri)
        {
            Uri result = null;

            if (((MergeWizard)page.Wizard).MergeTarget.IsDirectory)
            {
                using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
                {
                    Uri uri;

                    if (!Uri.TryCreate(baseUri, UriKind.Absolute, out uri))
                    {
                        page.Message = new WizardMessage(Resources.InvalidFromRevision, WizardMessage.MessageType.ERROR);
                    }
                    else
                    {
                        dlg.SelectedUri = uri;

                        if (dlg.ShowDialog(((MergeWizard)page.Wizard).Context) == DialogResult.OK)
                        {
                            result = dlg.SelectedUri;
                        }
                    }
                }
            }
            else
            {
                using (RepositoryOpenDialog dlg = new RepositoryOpenDialog())
                {
                    MergeWizard wizard = ((MergeWizard)page.Wizard);
                    Uri uri;
                    string fileName = Path.GetFileName(target.FullPath);

                    dlg.Context = wizard.Context;
                    dlg.Filter = fileName + "|" + fileName + "|All Files (*.*)|*";

                    if (!Uri.TryCreate(baseUri, UriKind.Absolute, out uri))
                    {
                        page.Message = new WizardMessage(Resources.InvalidFromRevision, WizardMessage.MessageType.ERROR);
                    }
                    else
                    {
                        dlg.SelectedUri = uri;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            result = dlg.SelectedUri;
                        }
                    }
                }
            }

            return result;
        }
    }
}
