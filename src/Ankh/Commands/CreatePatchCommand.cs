using System;
using EnvDTE;
using System.Windows.Forms;
using System.IO;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you create a patch.
    /// </summary>
    [VSNetCommand( "CreatePatch", Text="Create patch...", 
         Tooltip="Create a patch file.", 
         Bitmap = ResourceBitmaps.Diff),
    VSNetControl( "Item.Ankh", Position=1 ),
    VSNetControl( "Project.Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1 )]
    internal class CreatePatchCommand : CommandBase
    {       
    
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // always allow - worst case, he gets an empty file
            return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
        }
    
        public override void Execute(AnkhContext context)
        {
            context.StartOperation( "Creating patch" );
            try
            {
                DiffVisitor v = new DiffVisitor();
                context.SolutionExplorer.VisitSelectedItems( v, true );

                if ( v.Diff.Trim() == String.Empty )
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
                            w.Write( v.Diff );
                    }
                }
            }
            finally
            {
                context.EndOperation();
            }
        } // Execute
    } 
}
