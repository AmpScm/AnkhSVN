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

namespace Ankh.Solution
{
    /// <summary>
    /// Represents an item in the treeview.
    /// </summary>
    internal abstract class TreeNode
    {
        protected TreeNode( UIHierarchyItem item, IntPtr hItem, 
            Explorer explorer, TreeNode parent )
        {     
            this.uiItem = item;
            this.hItem = hItem;
            this.explorer = explorer;
            this.parent = parent;
                
            this.FindChildren();  
        }

        public abstract void VisitResources( ILocalResourceVisitor visitor, bool recursive );        
        
        public static TreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer, TreeNode parent )
        {
            TreeNode node = null;
            // what kind of node is this?
            if ( item.Object is Project )
                node = new ProjectNode( item, hItem, explorer, parent );
            else if ( item.Object is ProjectItem )
                node = new ProjectItemNode( item, hItem, explorer, parent );

            // make sure it has the correct status
            if ( node != null )
                node.UpdateStatus();

            return node;
        }

        public static TreeNode CreateSolutionNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer )
        {
            TreeNode node = new SolutionNode( item, hItem, explorer );
            node.UpdateStatus();
            
            return node;
        }

        public virtual void Refresh()
        {
            this.FindChildren( );
        }

        abstract public void Accept( INodeVisitor visitor );



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
        public void VisitChildResources( ILocalResourceVisitor visitor )
        {
            foreach( TreeNode node in this.Children )
                node.VisitResources( visitor, true );
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public TreeNode Parent
        {
            get{ return this.parent; }
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
        protected void FindChildren()
        {
            // retain the original expansion state
            bool isExpanded = this.uiItem.UIHierarchyItems.Expanded;

            // get the treeview child
            IntPtr childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                C.TVGN_CHILD, this.hItem );

            // a node needs to be expanded at least once in order to have child nodes
            if ( childItem == IntPtr.Zero && this.uiItem.UIHierarchyItems.Count > 0 )
            {
                this.uiItem.UIHierarchyItems.Expanded = true;
                childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                    C.TVGN_CHILD, this.hItem );
            }

            // iterate over the ui items and the treeview items in parallell
            this.children = new ArrayList();
            foreach( UIHierarchyItem child in this.uiItem.UIHierarchyItems )
            {
                Debug.Assert( childItem != IntPtr.Zero, 
                    "Could not get treeview item" );
                    
                TreeNode childNode = TreeNode.CreateNode( child, childItem, this.explorer,
                    this );
                if (childNode != null )
                    this.children.Add( childNode );

                // and the next child
                childItem = (IntPtr)Win32.SendMessage( this.explorer.TreeView, Msg.TVM_GETNEXTITEM,
                    C.TVGN_NEXT, childItem );                    
            }

            this.uiItem.UIHierarchyItems.Expanded = isExpanded;
        }

        #region ProjectNode
        /// <summary>
        /// Represents a node containing subnodes, such as a project or a solution.
        /// </summary>
        #endregion
   
        #region SolutionNode

            

            
        #endregion

        #region ProjectItemNode
        #endregion

        private UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private static IDictionary statusMap = new Hashtable();
    }
}
