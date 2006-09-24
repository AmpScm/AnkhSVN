// $Id$
using System;
using System.IO;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you create a patch.
    /// </summary>
    [VSNetCommand( "CreatePatch", Text="Create patch...", 
         Tooltip="Create a patch file.", 
         Bitmap = ResourceBitmaps.CreatePatch),
    VSNetItemControl( "", Position = 1 )]
    public class CreatePatchCommand : LocalDiffCommandBase
    {
        public CreatePatchCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override void Execute(IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            using(OperationManager.RunOperation( "Creating patch" ))
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
        } // Execute
    } 
}
