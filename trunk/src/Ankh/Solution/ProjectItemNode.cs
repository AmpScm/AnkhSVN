// $Id$
using System;
using System.Collections;

using NSvn.Core;
using NSvn.Common;
using EnvDTE;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Ankh.Solution
{
    /// <summary>
    /// Represents a node containing a project item.
    /// </summary>
    public class ProjectItemNode : SolutionExplorerTreeNode
    {
        public ProjectItemNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            SolutionExplorerTreeNode parent, ParsedSolutionItem parsedItem ) :
            base( item, hItem, explorer, parent )
        {
            this.projectItem = item.Object;
            this.parsedProjectItem=parsedItem;
                
            this.FindChildren();  
            
            this.FindResources();                      
        }

        public override void GetResources(IList list, bool getChildItems, 
            ResourceFilterCallback filter )
        {
            this.FilterResources( this.resources, list, filter );
            this.FilterResources( this.deletedResources, list, filter );
            this.GetChildResources( list, getChildItems, filter );
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitProjectItem(this);

            // if this isn't a directory node, visit children as well
            if ( !
                (this.resources.Count == 1 && 
                ((SvnItem)this.resources[0]).IsDirectory) )
            {
                foreach( SolutionExplorerTreeNode node in this.Children )
                    node.Accept( visitor );
            }
        }



        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override NodeStatus ThisNodeStatus()
        {
            return MergeStatuses(MergeStatuses( this.resources ),
                MergeStatuses(this.deletedResources));
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            try
            {
                if ( this.ProjectItem != null )
                {
                    int fileCount = this.ProjectItem.FileCount;
                    return fileCount < 0;
                }
                else
                {
                    return false;
                }
            }
            catch ( COMException )
            {
                this.RemoveSelf();
                return true;
            }

        }

        protected override void UnhookEvents()
        {
            UnhookEvents( this.resources, new EventHandler( this.ChildOrResourceChanged ) );
            UnhookEvents( this.resources, new EventHandler( this.DeletedItemStatusChanged ) );
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
                    if ( item.IsDirectory )
                        return item.Path;
                }

                // just return the dir component of the first item
                if ( resources.Count > 0 )
                    return Path.GetDirectoryName( ((SvnItem)this.resources[0]).Path );
                else
                {
                    // oops, get the direectory from the parent then?
                    if ( this.SolutionExplorerParent != null )
                        return this.SolutionExplorerParent.Directory;
                    else
                        throw new ApplicationException( 
                            "Could not determine directory for item. Should not happen" );
                }
            }
        }

        protected override IList DeletedItems
        {
            get { return this.deletedResources; }
        }


        protected void FindResources()
        {
            this.resources = new ArrayList();
            this.deletedResources = new ArrayList();
            try
            {
                EventHandler del = new EventHandler( this.ChildOrResourceChanged );
                this.AddResourcesFromProjectItem( this.ProjectItem, del );
                this.AddResourcesFromProjectItem( this.parsedProjectItem, del );

                // is this a childless tree node? it might have hidden children after all
                if ( this.Children.Count == 0 )
                {
                    this.AddSubItems( this.ProjectItem, del );
                    this.AddSubItems( this.parsedProjectItem, del );
                }
               
            }
            catch( NullReferenceException )
            {
                Trace.WriteLine( "NullReferenceException thrown in ProjectItemNode" );
                //swallow
            }   
            catch( System.Runtime.InteropServices.SEHException )
            {
                Trace.WriteLine( "SEHException thrown: " + this.ProjectItem.Name );
            }
            finally
            {
            }
        }

        protected override void RescanHook()
        {
            this.deletedResources = new ArrayList();
            foreach ( SvnItem item in this.resources )
            {
                if ( item.IsDirectory )
                {
                    this.AddDeletions( item.Path, this.deletedResources, new EventHandler( this.DeletedItemStatusChanged ) );
                }
            }
        }

        // recursively adds subitems of this projectitem.
        private void AddSubItems( ProjectItem item, EventHandler del )
        {
            if ( item==null || item.ProjectItems == null ) 
                return;

            // some object models might throw when accessing the .ProjectItems property
            try
            {
                foreach( ProjectItem subItem in Enumerators.EnumerateProjectItems(item.ProjectItems))
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
            }
        }

        // recursively adds subitems of this projectitem.
        private void AddSubItems( ParsedSolutionItem item, EventHandler del )
        {
            if ( item==null || item.Children.Count == 0 ) 
                return;

            foreach( ParsedSolutionItem subItem in item.Children )
            {
                if ( subItem.Name != Client.AdminDirectoryName )
                {
                    this.AddResourcesFromProjectItem( subItem, del );
                    this.AddSubItems( subItem, del );
                }
            } 
        }

        private void AddResourcesFromProjectItem( ProjectItem item, EventHandler del )
        {
            if ( item==null || item.FileCount == 0 )
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

                SvnItem svnItem = this.Explorer.Context.StatusCache[path];
                if ( svnItem.IsFile || svnItem.IsDirectory )
                {
                    this.resources.Add( svnItem );
                    svnItem.Changed += del;

                  
                   // if its a dir, we want the deleted paths too
                    if ( svnItem.IsDirectory )
                    {
                        this.AddDeletions( path, this.deletedResources, new EventHandler(this.DeletedItemStatusChanged) );
                    }
                }                    
            }
        }

        private void AddResourcesFromProjectItem( ParsedSolutionItem item, EventHandler del )
        {
            if(item==null)
                return;

            SvnItem svnItem = this.Explorer.Context.StatusCache[item.FileName];
            if ( svnItem.IsFile || svnItem.IsDirectory )
            {
                this.resources.Add( svnItem );
                svnItem.Changed += del;

                // if its a dir, we want the deleted paths too
                if ( svnItem.IsDirectory )
                {
                    this.AddDeletions( svnItem.Path, this.deletedResources, new EventHandler(this.DeletedItemStatusChanged) );
                }
            }  
        }

		/// <summary>
		/// The item for an unmodeled project
		/// </summary>
        public ParsedSolutionItem ParsedItem
        {
            get{ return this.parsedProjectItem; }
        }

		/// <summary>
		/// The modeled Project Item
		/// </summary>
		public ProjectItem ProjectItem
		{
			get{ return projectItem as ProjectItem; }
		}

        private ParsedSolutionItem parsedProjectItem;
        private object projectItem;
        private IList resources;
        private IList deletedResources;

    }    

}
