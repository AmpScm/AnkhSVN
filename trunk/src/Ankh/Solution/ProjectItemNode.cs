// $Id$
using System;
using System.Collections;

using NSvn.Core;
using NSvn.Common;
using EnvDTE;
using System.Diagnostics;
using System.IO;

namespace Ankh.Solution
{
    /// <summary>
    /// Represents a node containing a project item.
    /// </summary>
    public class ProjectItemNode : TreeNode
    {
        public ProjectItemNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            TreeNode parent ) :
            base( item, hItem, explorer, parent )
        {
            this.projectItem = (ProjectItem)item.Object;
            
            this.FindResources();                      
        }

        public override void GetResources(IList list, bool getChildItems, 
            ResourceFilterCallback filter )
        {
            foreach( SvnItem item in this.resources )
            {
                if ( filter == null || filter(item) )
                    list.Add(item);
            }
            this.GetChildResources( list, getChildItems, filter );
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitProjectItem(this);
        }



        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override NodeStatus ThisNodeStatus()
        {
            return this.MergeStatuses( this.resources );
        }

        /// <summary>
        /// The folder path if this is a folder item, or the directory
        /// in which the file resides if it is a file.
        /// </summary>
        public override string Directory
        {
            get
            {
                // is one of the resources a directory?
                foreach( SvnItem item in this.resources )
                {
                    if ( System.IO.Directory.Exists( item.Path ) )
                        return item.Path;
                }

                // just return the dir component of the first item
                if ( resources.Count > 0 )
                    return Path.GetDirectoryName( ((SvnItem)this.resources[0]).Path );
                else
                {
                    // oops, get the direectory from the parent then?
                    if ( this.Parent != null )
                        return this.Parent.Directory;
                    else
                        throw new ApplicationException( 
                            "Could not determine directory for item. Should not happen" );
                }
            }
        }


        protected void FindResources()
        {
            this.Explorer.AddResource( this.projectItem, this ); 

            this.resources = new ArrayList();
            try
            {
                StatusChanged del = new StatusChanged( this.ChildOrResourceChanged );
                this.AddResourcesFromProjectItem( this.projectItem, del );

                // is this a childless tree node? it might have hidden children after all
                if ( this.Children.Count == 0 )
                    this.AddSubItems( this.projectItem, del );
               
            }
            catch( NullReferenceException )
            {
                Trace.WriteLine( "NullReferenceException thrown in ProjectItemNode" );
                //swallow
            }   
            catch( System.Runtime.InteropServices.SEHException )
            {
                Trace.WriteLine( "SEHException thrown: " + this.projectItem.Name );
            }
            finally
            {
            }
        }

        // recursively adds subitems of this projectitem.
        private void AddSubItems( ProjectItem item, StatusChanged del )
        {
            if ( item.ProjectItems == null ) 
                return;

            // some object models might throw when accessing the .ProjectItems property
            try
            {
                for( int i = 1; i <= item.ProjectItems.Count; i++ )
                {
                    ProjectItem subItem = item.ProjectItems.Item(i);
                    if ( subItem.Name != Client.AdminDirectoryName )
                    {
                        this.AddResourcesFromProjectItem( subItem, del );
                        this.AddSubItems( subItem, del );
                    }
                }               
            }
            catch( InvalidCastException )
            {
                // empty
            }
            finally
            {
            }
        }

        private void AddResourcesFromProjectItem( ProjectItem item, StatusChanged del )
        {
            if ( item.FileCount == 0 )
                return;

            // HACK: figure out if we're dealing with a 0 or 1 based collection
            short startIndex = 1;
            
            // try to access the high bound - if that doesn't fail, we're using a 1-based idx
            try
            {
                item.get_FileNames(item.FileCount);
            }
            catch( ArgumentException )
            {
                // oops, must be a zero-based collection
                startIndex = 0;
            }
            for( short i = startIndex; i < item.FileCount + startIndex; i++ ) 
            {
                string path = null;
                try
                {
                    path = item.get_FileNames(i);
                }
                catch( ArgumentException )
                {
                    this.Explorer.Context.OutputPane.WriteLine( 
                        "Could not retrieve filename for project item" );
                    continue;
                }

                if ( File.Exists( path ) || System.IO.Directory.Exists( path ) )
                {
                    SvnItem svnItem = this.Explorer.Context.StatusCache[path];
                    this.resources.Add( svnItem );
                    svnItem.Changed += del;

                    // if its a dir, we want the deleted paths too
                    if ( System.IO.Directory.Exists( path ) )
                    {
                        this.AddDeletions( path, this.resources, del );
                    }
                }                    
            }
        }

        private ProjectItem projectItem;
        private IList resources;
    }    

}
