// $Id$
using EnvDTE;
using System.Collections;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;

namespace Ankh.Commands
{
    /// <summary>
    /// Adds an unversioned item to a working copy
    /// </summary>
    [VSNetCommand("AddItem", Text = "Add", Tooltip = "Adds selected item to a working copy",
         Bitmap = ResourceBitmaps.Add),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetFolderNodeControl( "Ankh", Position = 1),
    VSNetControl( "Solution.Ankh", Position = 1)]    
    public class AddItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {
            AddFilter filter = new AddFilter();
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback(filter.Filter)).Count > 0 )
            {
                return Enabled;
            }
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters )
        {
            AddFilter filter = new AddFilter();
            IList resources = context.SolutionExplorer.GetSelectionResources( false,
                new ResourceFilterCallback(filter.Filter) );

            bool recursive = false;

            // are we shifted?
            if ( CommandBase.Shift )
            {
                using( PathSelector sel = CommandBase.GetPathSelector( "Select items to add" ) )
                {
                    sel.EnableRecursive = true;
                    sel.Recursive = false;
                    sel.CheckedItems = sel.Items = resources;

                    if ( sel.ShowDialog() != DialogResult.OK )
                        return;

                    resources = sel.CheckedItems;
                    recursive = sel.Recursive;
                }
            }

            context.StartOperation( "Adding" );
            try
            {

                foreach( SvnItem item in resources )
                {
                    context.Client.Add( item.Path, recursive );
                }
                context.SolutionExplorer.RefreshSelection();
            }
            finally
            {
                context.EndOperation();
            }
        }
        #endregion

        /// <summary>
        /// This class is used to ensure that you can add f.ex a project folder
        /// and a file in the same operation. It assumes that the folder is 
        /// always visited first, and stores the path to the folder if it is versionable. 
        /// When the file is visited, it checks whether the parent dir of that file
        /// has already been visited.
        /// </summary>
        private class AddFilter
        {
            private Hashtable paths = 
                System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable();

            public bool Filter( SvnItem item )
            {
                if ( item.IsVersioned )
                    return false;

                if ( item.IsDirectory )
                {
                    if ( item.IsVersionable )
                    {
                        // this could be the parent item of some item to come later on
                        this.paths.Add( NormalizePath(item.Path), null );
                        return true;
                    }
                    else
                        return false;
                }
                else 
                {
                    // must be a file
                    if ( item.IsVersionable )
                        return true;
                    else
                    {
                        // have we already visited the parent?
                        string dir = NormalizePath(Path.GetDirectoryName( item.Path ));
                        return this.paths.ContainsKey( dir );
                    }
                }
            }

            private string NormalizePath( string dir )
            {
                if ( dir.EndsWith( "\\" ) )
                    return dir.Substring(0, dir.Length-1);
                else 
                    return dir;                
            }
        }
    }
}



