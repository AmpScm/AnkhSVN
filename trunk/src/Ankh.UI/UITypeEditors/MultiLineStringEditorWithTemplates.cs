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
using System.Text;
using System.Drawing;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that lets the user edit a string, with templates.
    /// </summary>
    public partial class MultiLineStringEditorWithTemplates : MultiLineStringTypeEditorDialog
    {
        PopUpListForm _templatePopup;

        public MultiLineStringEditorWithTemplates()
        {
            this.InitializeComponent();
        }

        public void SetTemplates(StringEditorTemplate[] templates)
        {
            _templatePopup = new PopUpListForm();
            _templatePopup.ValueMember = "Value";
            _templatePopup.DisplayMember = "Text";
            _templatePopup.ToolTipMember = "ToolTip";
            _templatePopup.DataSource = templates;
            _templatePopup.SelectionCommitted += new EventHandler(templatePopup_SelectionCommitted);
        }

        private void MultiLineStringEditorWithTemplates_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.T && e.Control)
            {
                this.ShowTemplatePopup();
            }
        }

        private void ShowTemplatePopup()
        {
            this._templatePopup.Location = this.textBox.PointToScreen(
                this.textBox.GetPositionFromCharIndex(this.textBox.SelectionStart));

            using (Graphics g = this.textBox.CreateGraphics())
            {
                SizeF size = g.MeasureString("A", this.textBox.Font);

                this._templatePopup.Top += (int)(size.Height);
                this._templatePopup.Left -= (int)(size.Width);
            }
            this._templatePopup.Show();
        }

        void templatePopup_SelectionCommitted(object sender, EventArgs e)
        {
            if (this._templatePopup.SelectedValue != null)
            {
                this.textBox.AppendText((string)this._templatePopup.SelectedValue);
            }
        }        
    }

    /// <summary>
    /// A template in the dialog above.
    /// </summary>
    public class StringEditorTemplate
    {
        readonly string _text, _value, _toolTip;

        public string Text
        {
            get { return this._text; }
        }
        public string Value
        {
            get { return this._value; }
        }
        public string ToolTip
        {
            get { return this._toolTip; }
        }

        public StringEditorTemplate(string value, string toolTip, string text)
        {
            this._text = text;
            this._value = value;
            this._toolTip = toolTip;
        }

        public StringEditorTemplate(string value, string toolTip)
            : this(value, toolTip, value)
        {
        }

        public StringEditorTemplate(string value)
            : this(value, value, value)
        {

        }
    }
}
