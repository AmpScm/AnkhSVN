// $Id$
using EnvDTE;
using System.Collections;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;

using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add selected items to the working copy.
    /// </summary>
    [VSNetCommand(AnkhCommand.AddItem,
		"AddItem",
        Text = "A&dd...",
        Tooltip = "Add selected items to the working copy.",
        Bitmap = ResourceBitmaps.Add),
    VSNetItemControl(VSNetControlAttribute.AnkhSubMenu, Position = 1)]
    public class AddItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            AddFilter filter = new AddFilter();
            if (e.Context.Selection.GetSelectionResources(true,
                new ResourceFilterCallback(filter.Filter)).Count == 0)
            {
                e.Enabled = e.Visible = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            AddFilter filter = new AddFilter();
            IList resources = context.Selection.GetSelectionResources( true,
                new ResourceFilterCallback(filter.Filter) );

            // are we shifted?
            if ( !CommandBase.Shift )
            {                
                PathSelectorInfo info = new PathSelectorInfo( "Select items to add",
                    resources, resources );
                info.EnableRecursive = false;

                info = context.UIShell.ShowPathSelector( info );

                if ( info == null )
                    return;

                resources = info.CheckedItems;
            }

            context.StartOperation( "Adding" );
            try
            {
                SvnAddArgs args = new SvnAddArgs();
                args.ThrowOnError = false;
                args.Depth = SvnDepth.Empty;
                foreach (SvnItem item in resources)
                {
                    context.Client.Add(item.Path, args);
                }
                context.Selection.RefreshSelection();
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

                if ( ! (File.Exists( item.Path ) || Directory.Exists( item.Path ) ) )
                    return false;

                if ( item.IsDirectory )
                {
                    if ( item.IsVersionable )
                    {
                        // this could be the parent item of some item to come later on
                        string normalizedPath = NormalizePath(item.Path);
                        if ( !this.paths.ContainsKey(normalizedPath) )
                        {
                            this.paths.Add( normalizedPath, null );
                        }
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