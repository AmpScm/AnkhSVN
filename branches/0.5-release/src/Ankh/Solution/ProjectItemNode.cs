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
    internal class ProjectItemNode : TreeNode
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
                Debug.WriteLine( "Adding resources from node " + 
                    this.projectItem.Name, "Ankh" );
                Debug.Indent();

                StatusChanged del = new StatusChanged( this.ChildOrResourceChanged );
                this.AddResourcesFromProjectItem( this.projectItem, del );

                // is this a childless tree node? it might have hidden children after all
                if ( this.Children.Count == 0 )
                    this.AddSubItems( this.projectItem, del );
               
            }
            catch( NullReferenceException )
            {
                Debug.WriteLine( "NullReferenceException thrown in ProjectItemNode" );
                //swallow
            }   
            catch( System.Runtime.InteropServices.SEHException sex )
            {
                Debug.WriteLine( "SEHException thrown: " + this.projectItem.Name );
                System.Windows.Forms.MessageBox.Show( "SEHException: " + this.projectItem.Name + sex.Message );
            }
            finally
            {
                Debug.Unindent();
                Debug.WriteLine( "Finished adding resources from node", "Ankh" );
            }
        }

        // recursively adds subitems of this projectitem.
        private void AddSubItems( ProjectItem item, StatusChanged del )
        {
            // some object models might throw when accessing the .ProjectItems property
            try
            {
                Debug.WriteLine( "Adding subitems for " + item.Name, "Ankh" );
                Debug.Indent();

                foreach( ProjectItem subItem in item.ProjectItems )
                {
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
                Debug.Unindent();
                Debug.WriteLine( "Finished adding subitems for " + item.Name, 
                    "Ankh" );
            }
        }

        private void AddResourcesFromProjectItem( ProjectItem item, StatusChanged del )
        {
            for( short i = 1; i <= item.FileCount; i++ ) 
            {
                string path = item.get_FileNames(i);
                Debug.WriteLine( "Adding path from ProjectItem: " + path, "Ankh" );
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
