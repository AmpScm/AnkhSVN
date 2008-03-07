// $Id$
using System;
using System.IO;
using System.Windows.Forms;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you create a patch.
    /// </summary>
    [VSNetCommand(AnkhCommand.CreatePatch,
		"CreatePatch",
         Text = "Create &Patch...", 
         Tooltip = "Create a patch file of changes.", 
         Bitmap = ResourceBitmaps.CreatePatch),
        VSNetItemControl(VSNetControlAttribute.AnkhSubMenu, Position = 9)]
    public class CreatePatchCommand : LocalDiffCommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            this.SaveAllDirtyDocuments( context );

            context.StartOperation( "Creating patch" );
            try
            {
                string diff = this.GetDiff( context );
                
                if ( diff == null )
                {
                    MessageBox.Show( context.HostWindow, "Nothing to diff here. Move along." );
                    return;
                }

                using( SaveFileDialog dlg = new SaveFileDialog() )
                {
                    dlg.Filter = "Patch files(*.patch)|*.patch|Diff files(*.diff)|*.diff|" +
                        "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                    dlg.AddExtension = true;

                    if ( dlg.ShowDialog( context.HostWindow ) == DialogResult.OK )
                    {
                        using( StreamWriter w = File.CreateText(dlg.FileName) )
                            w.Write( diff );
                    }
                }
            }
            finally
            {
                context.EndOperation();
            }
        } // Execute

        #endregion
    } 
}
