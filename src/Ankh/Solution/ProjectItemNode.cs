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
            TreeNode parent, ParsedSolutionItem parsedItem ) :
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
                foreach( TreeNode node in this.Children )
                    node.Accept( visitor );
            }
        }



        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override NodeStatus ThisNodeStatus()
        {
            return this.MergeStatuses(this.MergeStatuses( this.resources ),
                this.MergeStatuses(this.deletedResources));
        }

        protected override void CheckForSvnDeletions()
        {
            // only check *our* resources here, we already know that the deleted resources are, uhm, deleted.
            foreach(SvnItem item in this.resources)
            {
                if ( item.IsDeleted )
                {
                    this.SvnDelete();
                }
            }
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            if ( !AllResourcesDeleted() )
            {
                return false;
            }

            // we need to be absolutely sure this project item is up to date, since it may have been renamed
            this.FindResources();

            // If everything's deleted, I have no more reason to live.
            if ( !AllResourcesDeleted() )
            {
                return false;
            }

            // get us off the hook
            this.Dispose();
            this.RemoveSelf();

            bool removed = false;
            try
            {   
                // VC files can only be removed with VCProject.RemoveFile.
                VCProject vcProject = this.ProjectItem.ContainingProject.Object as VCProject;
                VCProjectItem vcItem = this.ProjectItem.Object as VCProjectItem;
                if ( vcProject != null && vcItem != null )
                {
                    vcProject.RemoveFile( vcItem );
                    removed = true;
                }
            }
            catch ( Exception )
            {
                // ignore
            }
            try
            {
                // Try just removing them from the project.
                if ( !removed )
                {
                    this.ProjectItem.Remove();
                    removed = true; 
                }
            }
            catch(Exception)
            {
            }
            try
            {
                // New-style ASP.NET files can only be deleted from projects.
                if ( !removed )
                {
                    this.ProjectItem.Delete();
                    removed = true; 
                }
            }
            catch ( Exception )
            {
            }

            if ( removed )
            {
                this.Parent.Refresh();
            }
            return removed;
        }

        protected override void DoDispose()
        {
            this.UnhookEvents( this.resources, new StatusChanged( this.ChildOrResourceChanged ) );
            this.UnhookEvents( this.resources, new StatusChanged( this.DeletedItemStatusChanged ) );
        }

        private bool AllResourcesDeleted()
        {
            IList versionedResources = new ArrayList();
            this.GetResources( versionedResources, true, new ResourceFilterCallback( SvnItem.NotDeletedFilter ) );

            if ( versionedResources.Count > 0 )
            {
                return false;
            }
            else
            {
                return true;
            }
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
                    if ( this.Parent != null )
                        return this.Parent.Directory;
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
            this.Explorer.AddResource( this.projectItem, this.parsedProjectItem, this ); 

            this.resources = new ArrayList();
            this.deletedResources = new ArrayList();
            try
            {
                StatusChanged del = new StatusChanged( this.ChildOrResourceChanged );
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
                    this.AddDeletions( item.Path, this.deletedResources, new StatusChanged( this.DeletedItemStatusChanged ) );
                }
            }
        }

        // recursively adds subitems of this projectitem.
        private void AddSubItems( ProjectItem item, StatusChanged del )
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
        private void AddSubItems( ParsedSolutionItem item, StatusChanged del )
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

        private void AddResourcesFromProjectItem( ProjectItem item, StatusChanged del )
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
                        this.AddDeletions( path, this.deletedResources, new StatusChanged(this.DeletedItemStatusChanged) );
                    }
                }                    
            }
        }

        private void AddResourcesFromProjectItem( ParsedSolutionItem item, StatusChanged del )
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
                    this.AddDeletions( svnItem.Path, this.deletedResources, new StatusChanged(this.DeletedItemStatusChanged) );
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
