using System;
using EnvDTE;
using Ankh.UI;
using NSvn.Core;

using System.IO;
using System.Windows.Forms;
using Ankh.Solution;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for RenameCommand.
    /// </summary>
    [VSNetCommand("RenameFile", Text = "Rename file...", Tooltip = "Rename this file...", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetControl( "Item.Ankh", Position = 1 )]
    public class RenameFileCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we can only rename a single unmodified file
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback(RenameFileCommand.UnmodifiedSingleFileFilter) ).Count == 1 )
            {
                return Enabled;
            }
            else
            {
                return Disabled;
            }
        }

        public override void Execute(IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            IList items = context.SolutionExplorer.GetSelectionResources( false,
                new ResourceFilterCallback(CommandBase.UnmodifiedSingleFileFilter) );
            
            // should only ever be 1
            System.Diagnostics.Debug.Assert( items.Count == 1, "Should only be 1" );
            SvnItem item = (SvnItem)items[0];

            context.StartOperation( "Renaming" );
            try
            {
                string filename = Path.GetFileName( item.Path );
                string dirname = Path.GetDirectoryName( item.Path );
                using( RenameDialog dialog = new RenameDialog( filename ) )
                {
                    if ( dialog.ShowDialog() == DialogResult.OK )
                    {
                        string newPath = Path.Combine( dirname, dialog.NewName );
                        context.OutputPane.WriteLine( "Renaming {0} to {1}", item.Path, newPath );
                        context.Client.Move( item.Path, Revision.Unspecified, newPath, true );

                        // remove the old file and add the new to the project
                        ProjectItem prjItem = context.SolutionExplorer.GetSelectedProjectItem();

                        Project project = prjItem.ContainingProject;
                        prjItem.Remove();
                        project.ProjectItems.AddFromFile( newPath );
                    }
                }
                                
            }
            finally
            {
                context.EndOperation();
            }
        }
    }
}
