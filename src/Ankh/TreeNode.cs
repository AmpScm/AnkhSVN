using System;
using EnvDTE;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.Collections;
using Utils.Win32;
using System.Diagnostics;
using System.IO;

using C = Utils.Win32.Constants;

namespace Ankh
{
    /// <summary>
    /// Represents an item in the treeview.
    /// </summary>
    internal abstract class TreeNode
    {
        private TreeNode( UIHierarchyItem item, IntPtr hItem, 
            SolutionExplorer explorer )
        {                
            this.hItem = hItem;
            this.explorer = explorer;
                
            this.FindChildren( item );  
        }

        public abstract void VisitResources( ILocalResourceVisitor visitor );        
        
        public static TreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            SolutionExplorer explorer )
        {
            TreeNode node = null;
            // what kind of node is this?
            if ( item.Object is Project )
                node = new ProjectNode( item, hItem, explorer );
            else if ( item.Object is ProjectItem )
                node = new ProjectItemNode( item, hItem, explorer );

            // make sure it has the correct status
            if ( node != null )
                node.UpdateStatus();

            return node;
        }

        public static TreeNode CreateSolutionNode( UIHierarchyItem item, IntPtr hItem,
            SolutionExplorer explorer )
        {
            TreeNode node = new SolutionNode( item, hItem, explorer );
            node.UpdateStatus();
            
            return node;
        }



        static TreeNode()
        {
            statusMap[ StatusKind.Normal ]      = 1;
            statusMap[ StatusKind.Added ]       = 2;
            statusMap[ StatusKind.Deleted ]     = 3;
            statusMap[ StatusKind.Conflicted ]  = 6;
            statusMap[ StatusKind.Unversioned ] = 8;
            statusMap[ StatusKind.Modified ]    = 9;
        }
        

        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public IList Children
        {
            get { return this.children;  }
        }

        /// <summary>
        /// Updates the status icon of this node.
        /// </summary>
        public void UpdateStatus()
        {
            // update status on the children first
            foreach( TreeNode node in this.Children )
                node.UpdateStatus();

            // text or property changes on the project file itself?
            StatusKind status = this.GetStatus();
            this.SetStatusImage( status );
        }

        /// <summary>
        /// Visits the children of this node.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitChildren( ILocalResourceVisitor visitor )
        {
            foreach( TreeNode node in this.Children )
                node.VisitResources( visitor );
        }

        /// <summary>
        /// Gets the status of the resources belonging to one specific node, 
        /// not including children.
        /// </summary>
        /// <returns></returns>
        protected virtual StatusKind GetStatus()
        {
            return StatusKind.None;
        }

        /// <summary>
        /// Sets the status image on this node.
        /// </summary>
        /// <param name="status">The status on this node.</param>
        protected void SetStatusImage( StatusKind status )
        {
            int statusImage = 0;
            if ( statusMap.Contains(status) )
                statusImage = (int)statusMap[status];

            TVITEMEX tvitem = new TVITEMEX();
            tvitem.mask = C.TVIF_STATE | C.TVIF_HANDLE;
            tvitem.hItem = this.hItem;
            // bits 12-15 indicate the state image
            tvitem.state = (uint)(statusImage << 12);
            tvitem.stateMask = C.TVIS_STATEIMAGEMASK;

            int retval = Win32.SendMessage( this.explorer.TreeView, Msg.TVM_SETITEM, IntPtr.Zero, 
                tvitem ).ToInt32();
            Debug.Assert( Convert.ToBoolean( retval ), 
                "Could not set treeview state image" );
                
        }

        /// <summary>
        /// Returns a status from a resource, taking into account both text status and 
        /// property status.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected static StatusKind StatusFromResource( ILocalResource resource )
        {
            if ( resource.Status.TextStatus != StatusKind.Normal )
                return resource.Status.TextStatus;
            else if ( resource.Status.PropertyStatus != StatusKind.Normal &&
                resource.Status.PropertyStatus != StatusKind.None )
                return resource.Status.PropertyStatus;  
            else
                return StatusKind.Normal;
        }

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        protected void FindChildren( UIHierarchyItem item )
        {
            // retain the original expansion state
            bool isExpanded = item.UIHierarchyItems.Expanded;

            // get the treeview child
            IntPtr childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                C.TVGN_CHILD, this.hItem );

            // a node needs to be expanded at least once in order to have child nodes
            if ( childItem == IntPtr.Zero && item.UIHierarchyItems.Count > 0 )
            {
                item.UIHierarchyItems.Expanded = true;
                childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                    C.TVGN_CHILD, this.hItem );
            }

            this.children = new ArrayList();
            foreach( UIHierarchyItem child in item.UIHierarchyItems )
            {
                Debug.Assert( childItem != IntPtr.Zero, 
                    "Could not get treeview item" );
                    
                TreeNode childNode = TreeNode.CreateNode( child, childItem, this.explorer );
                if (childNode != null )
                    this.children.Add( childNode );

                // and the next child
                childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                    C.TVGN_NEXT, childItem );                    
            }

            item.UIHierarchyItems.Expanded = isExpanded;
        }

        /// <summary>
        /// Represents a node containing subnodes, such as a project or a solution.
        /// </summary>
        private class ProjectNode : TreeNode
        {
            public ProjectNode( UIHierarchyItem item, IntPtr hItem, SolutionExplorer explorer ) : 
                base( item, hItem, explorer )
            {
                Project project = (Project)item.Object;

                // find the directory containing the project
                string fullname = project.FullName;
                // the Solution Items project has no path
                if ( fullname != string.Empty )
                {
                    string parentPath = Path.GetDirectoryName( fullname );
                    this.projectFolder = SvnResource.FromLocalPath( parentPath );
                    explorer.AddResource( project, this );                    
                }
                if ( this.projectFolder != null )
                    this.projectFolder.Context = explorer.Context;
            }

            public override void VisitResources( ILocalResourceVisitor visitor )
            {
                if ( this.projectFolder != null )
                    this.projectFolder.Accept( visitor );
            } 
            
            protected override StatusKind GetStatus()
            {
                if ( this.projectFolder == null )
                    return StatusKind.None;               
                else
                    return StatusFromResource( this.projectFolder );
            }                    

            private ILocalResource projectFolder;
        }     
   
        /// <summary>
        /// A node representing a solution.
        /// </summary>
        private class SolutionNode : TreeNode
        {
            public SolutionNode( UIHierarchyItem item, IntPtr hItem, SolutionExplorer explorer )
                : base( item, hItem, explorer )
            {
                Solution solution = explorer.DTE.Solution;
                this.solutionFile = SvnResource.FromLocalPath( solution.FullName );
            }

            public override void VisitResources( ILocalResourceVisitor visitor )
            {
                this.solutionFile.Accept( visitor );
                this.VisitChildren( visitor );
            } 

            protected override StatusKind GetStatus()
            {
                if ( this.solutionFile == null )
                    return StatusKind.None;               
                else
                {
                    StatusKind status = StatusFromResource( this.solutionFile );
                    if ( status != StatusKind.Normal )
                    {
                        // check the status on the projects
                        ModifiedVisitor v = new ModifiedVisitor();
                        this.VisitChildren( v );
                        if ( v.Modified )
                            status = StatusKind.Modified;
                    }

                    return status;
                }
            }

            

            private ILocalResource solutionFile;
        }

        /// <summary>
        /// Represents a node containing a project item.
        /// </summary>
        private class ProjectItemNode : TreeNode
        {
            public ProjectItemNode( UIHierarchyItem item, IntPtr hItem, SolutionExplorer explorer ) :
                base( item, hItem, explorer )
            {
                ProjectItem pitem = (ProjectItem)item.Object;
                this.resources = new ArrayList();
                try
                {
                    for( short i = 1; i <= pitem.FileCount; i++ ) 
                    {
                    
                        ILocalResource res = SvnResource.FromLocalPath( pitem.get_FileNames(i) );
                        // does this resource exist?
                        if ( res != null )
                        {
                            res.Context = explorer.Context;
                            this.resources.Add( res );
                        }
                    }
                    explorer.AddResource( pitem, this );                    
                }
                catch( NullReferenceException )
                {
                    //swallow
                }                
            }

            protected override StatusKind GetStatus()
            {
                // go through the resources belonging to this node
                foreach( ILocalResource resource in this.resources )
                {
                    StatusKind status = StatusFromResource( resource );
                    if ( status != StatusKind.Normal )
                        return status;
                }
                                
                return StatusKind.Normal;            
            }

            public override void VisitResources( ILocalResourceVisitor visitor )
            {
                foreach( ILocalResource resource in this.resources )
                    resource.Accept( visitor );

                this.VisitChildren( visitor );
            }

            private IList resources;
        }        

        private IntPtr hItem;
        private IList children;
        private SolutionExplorer explorer;
        private static IDictionary statusMap = new Hashtable();
    }
}
