using System;
using EnvDTE;
using Ankh.UI;
using NSvn;
using System.IO;
using System.Windows.Forms;
using Ankh.Solution;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for RenameCommand.
    /// </summary>
    [VSNetCommand("RenameFile", Text = "Rename file...", Tooltip = "Rename this file...", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetControl( "Item.Ankh", Position = 1 )]
    internal class RenameFileCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            RenamableVisitor v = new RenamableVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, false );
            return v.Renamable ? 
                (   vsCommandStatus.vsCommandStatusEnabled | 
                    vsCommandStatus.vsCommandStatusSupported ) 
                : vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(AnkhContext context)
        {
            context.StartOperation( "Renaming" );
            try
            {
                RenameVisitor v = new RenameVisitor( context );
                context.SolutionExplorer.VisitSelectedNodes( v );
            }
            finally
            {
                context.EndOperation();
            }
        }

        private class RenameVisitor : INodeVisitor
        {
            public RenameVisitor( AnkhContext context )
            {
                this.context = context;
            }

            public void VisitProjectItem( ProjectItemNode node )
            {
                // we can only rename a single item
                if ( node.Resources.Count > 1 || node.Resources.Count == 0 )
                    return;

                WorkingCopyFile file = (WorkingCopyFile)node.Resources[0];

                string filename = Path.GetFileName( file.Path );
                string dirname = Path.GetDirectoryName( file.Path );
                using( RenameDialog dialog = new RenameDialog( filename ) )
                {
                    if ( dialog.ShowDialog() == DialogResult.OK )
                    {
                        string newPath = Path.Combine( dirname, dialog.NewName );
                        context.OutputPane.WriteLine( "Renaming {0} to {1}", file.Path, newPath );
                        file.Move( newPath, true );

                        // remove the old file and add the new to the project
                        Project project = node.ProjectItem.ContainingProject;
                        node.ProjectItem.Remove();
                        project.ProjectItems.AddFromFile( newPath );
                    }
                }
            }

            public void VisitProject( ProjectNode node )
            {
                // empty
            }

            public void VisitSolutionNode( SolutionNode node )
            {
                // empty
            }

            private AnkhContext context;
        }
    }
}
