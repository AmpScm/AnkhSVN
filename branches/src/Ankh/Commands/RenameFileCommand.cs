using System;
using EnvDTE;
using Ankh.UI;
using NSvn.Core;
using Utils;

using System.IO;
using System.Windows.Forms;
using Ankh.Solution;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for RenameCommand.
    /// </summary>
    [VSNetCommand("RenameFile", Text = "Rename item...", Tooltip = "Rename this item...", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1 )]
    public class RenameFileCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we can only rename unmodified files or folders
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback(RenameFileCommand.UnmodifiedItemFilter) ).Count == 1 )
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
                new ResourceFilterCallback(CommandBase.UnmodifiedItemFilter) );
            
            // should only ever be 1 
            System.Diagnostics.Debug.Assert( items.Count == 1, "Should be 1" );

            context.StartOperation( "Renaming" );
            try
            {
                foreach( SvnItem item in items )
                {
                    this.RenameItem( item, context );
                }
            }
            finally
            {
                context.EndOperation();
            }
            
        }

        private void RenameItem( SvnItem item, IContext context )
        {
            string parent = PathUtils.GetParent( item.Path );
            string name = PathUtils.GetName( item.Path );

            using( RenameDialog dialog = new RenameDialog( name ) )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    string newPath = Path.Combine( parent, dialog.NewName );
                    context.OutputPane.WriteLine( "Renaming {0} to {1}", item.Path, newPath );
                    context.Client.Move( item.Path, Revision.Unspecified, newPath, true );

                    // remove the old file and add the new to the project
                    ProjectItem prjItem = context.SolutionExplorer.GetSelectedProjectItem();

                    Project project = prjItem.ContainingProject;
                    prjItem.Remove();

                    if ( item.IsDirectory )
                        project.ProjectItems.AddFromDirectory( newPath );
                    else                            
                        project.ProjectItems.AddFromFile( newPath );

                    project.Save( null );
                }
            }
        }

    }
}
