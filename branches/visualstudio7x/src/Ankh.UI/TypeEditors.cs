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
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService( typeof( IWindowsFormsEditorService ) );
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
