using System;
using EnvDTE;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.Collections;
using Utils.Win32;
using System.Diagnostics;

using C = Utils.Win32.Constants;

namespace Ankh
{
    /// <summary>
    /// Represents an item in the treeview.
    /// </summary>
    internal class TreeNode
    {
        private TreeNode( UIHierarchyItem item, IntPtr hItem, 
            SolutionExplorer explorer )
        {                
            this.hItem = hItem;
            this.explorer = explorer;
                
            this.FindChildren( item );  
        }

        internal void VisitResources( ILocalResourceVisitor visitor )
        {            
            this.DoVisitResources( visitor );
            foreach( TreeNode node in this.Children )
                node.VisitResources( visitor );
        }

        
        
        public static TreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            SolutionExplorer explorer )
        {
            TreeNode node = null;
            // what kind of node is this?
            if ( item.Object is Project || item.Object is Solution )
                node = new ProjectNode( item, hItem, explorer );
            else if ( item.Object is ProjectItem )
                node = new ProjectItemNode( item, hItem, explorer );
            else
                node = new TreeNode( item, hItem, explorer );

            // make sure it has the correct status
            node.UpdateStatus();

            return node;
        }

        public static TreeNode CreateSolutionNode( UIHierarchyItem item, IntPtr hItem,
            SolutionExplorer explorer )
        {
            TreeNode node = new ProjectNode( item, hItem, explorer );
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
            if ( status != StatusKind.Normal )
                this.SetStatusImage( status );            
            else
            {
                // whats the status on the children?
                ModifiedVisitor v = new ModifiedVisitor();
                this.VisitResources( v );
                if ( v.Modified )
                    this.SetStatusImage( StatusKind.Modified );
                else
                    // no non-normal statuses found
                    this.SetStatusImage( StatusKind.Normal );
            }

            
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
        /// Visit the resources belonging to one specific node, not 
        /// including children
        /// </summary>
        /// <param name="visitor"></param>
        protected virtual void DoVisitResources( ILocalResourceVisitor visitor )
        {}

        

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        private void FindChildren( UIHierarchyItem item )
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
                    

                this.children.Add( TreeNode.CreateNode( child, childItem, this.explorer ) );

                // and the next child
                childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                    C.TVGN_NEXT, childItem );                    
            }

            item.UIHierarchyItems.Expanded = isExpanded;
        }

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
        /// Represents a node containing subnodes, such as a project or a solution.
        /// </summary>
        private class ProjectNode : TreeNode
        {
            public ProjectNode( UIHierarchyItem item, IntPtr hItem, SolutionExplorer explorer ) : 
                base( item, hItem, explorer )
            {
                if ( item.Object is Project )
                {
                    this.resource = SvnResource.FromLocalPath(((Project)item.Object).FullName);
                    explorer.AddResource( (Project)item.Object, this );                    
                }
                else
                {
                    this.resource = SvnResource.FromLocalPath(explorer.DTE.Solution.FullName);
                    explorer.AddResource( explorer.DTE.Solution, this );                    
                }
                this.resource.Context = explorer.Context;
            }
            
            protected override StatusKind GetStatus()
            {
                if ( this.resource.Status.TextStatus != StatusKind.Normal ) 
                    return this.resource.Status.TextStatus;
                else if ( this.resource.Status.PropertyStatus != StatusKind.Normal &&
                    this.resource.Status.PropertyStatus != StatusKind.None )
                    return this.resource.Status.PropertyStatus;
                else
                    return StatusKind.Normal;
            }
            
            protected override void DoVisitResources( ILocalResourceVisitor visitor )
            {
                this.resource.Accept( visitor );
            }            

            private ILocalResource resource;
        }        

        private class ProjectItemNode : TreeNode
        {
            public ProjectItemNode( UIHierarchyItem item, IntPtr hItem, SolutionExplorer explorer ) :
                base( item, hItem, explorer )
            {
                ProjectItem pitem = (ProjectItem)item.Object;
                this.resources = new ArrayList();
                for( short i = 1; i <= pitem.FileCount; i++ ) 
                {
                    ILocalResource res = SvnResource.FromLocalPath( pitem.get_FileNames(i) );
                    res.Context = explorer.Context;
                    this.resources.Add( res );
                }

                explorer.AddResource( pitem, this );
            }

            protected override StatusKind GetStatus()
            {
                // go through the resources belonging to this node
                foreach( ILocalResource resource in this.resources )
                {
                    if ( resource.Status.TextStatus != StatusKind.Normal )
                        return resource.Status.TextStatus;
                    else if ( resource.Status.PropertyStatus != StatusKind.Normal &&
                        resource.Status.PropertyStatus != StatusKind.None )
                        return resource.Status.PropertyStatus;                    
                }
                                
                return StatusKind.Normal;            
            }

            protected override void DoVisitResources( ILocalResourceVisitor visitor )
            {
                foreach( ILocalResource resource in this.resources )
                    resource.Accept( visitor );
            }

            private IList resources;
        }        

        private IntPtr hItem;
        private IList children;
        private SolutionExplorer explorer;
        private static IDictionary statusMap = new Hashtable();
    }
}
