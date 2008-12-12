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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// Interface for a dialog that allows the user to edit a string.
    /// </summary>
    public interface IStringEditorDialog : IDisposable
    {
        Form Dialog { get; }
        string Value { get; set; }
    }

    /// <summary>
    /// A type editor for a string.
    /// </summary>
    public class StringTypeEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported( System.ComponentModel.ITypeDescriptorContext context )
        {
            return false;
        }

        public override UITypeEditorEditStyle GetEditStyle( System.ComponentModel.ITypeDescriptorContext context )
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            IUIService svc = (IUIService)provider.GetService( typeof( IUIService ) );
            using ( IStringEditorDialog dlg = this.GetStringEditorDialog() )
            {
                dlg.Value = (string)value;
                if ( this.title != null )
                {
                    dlg.Dialog.Text = this.title;
                }
                if ( svc.ShowDialog( dlg.Dialog ) == System.Windows.Forms.DialogResult.OK )
                {
                    return dlg.Value;
                }
                else
                {
                    return value;
                }
            }
        }

        protected void SetTitle( string title )
        {
            this.title = title;
        }

        protected virtual IStringEditorDialog GetStringEditorDialog()
        {
            return new MultiLineStringTypeEditorDialog();
        }

        private string title;
    }

    /// <summary>
    /// A type editor for strings that allow templates.
    /// </summary>
    public abstract class StringTypeEditorWithTemplates : StringTypeEditor
    {
        protected override IStringEditorDialog GetStringEditorDialog()
        {
            MultiLineStringEditorWithTemplates dlg = new MultiLineStringEditorWithTemplates();
            dlg.SetTemplates( this.GetTemplates() );

            return dlg;
        }

        protected abstract StringEditorTemplate[] GetTemplates();
    }
}
