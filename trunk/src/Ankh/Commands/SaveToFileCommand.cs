using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// Lets the user cat a file from a repos and have Windows open it.
    /// </summary>
    [VSNetCommand("SaveToFile", Tooltip="Save the file to disk.", Text = "Save to file" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    internal class SaveToFileCommand : ViewRepositoryFileCommand
    {
        public override void Execute(AnkhContext context)
        {
            context.StartOperation( "Saving" );
            try
            {
                SaveCatVisitor v = new SaveCatVisitor( context );     
                context.RepositoryController.VisitSelectedNodes( v );
            }
            finally
            {
                context.EndOperation();
            }
        }        

        // override the CatVisitor class so we can pop up a save file dialog.
        protected class SaveCatVisitor : CatVisitor
        {
            public SaveCatVisitor( AnkhContext context ) : base( context )
            {}

            protected override string GetPath(string filename)
            {
                using( SaveFileDialog sfd = new SaveFileDialog() )
                {
                    sfd.FileName = filename;
                    if ( sfd.ShowDialog() == DialogResult.OK )
                        return sfd.FileName;
                    else
                        return null;
                }
            }
        }
    }
}
