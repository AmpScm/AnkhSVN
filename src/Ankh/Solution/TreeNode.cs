// $Id$
using System;
using EnvDTE;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.Collections;
using Utils.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
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


        public Explorer Explorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.explorer; }
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

            return node;
        }

        public static TreeNode CreateSolutionNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer )
        {
            if ( explorer.DTE.Solution.FullName != string.Empty )
            {
                TreeNode node = new SolutionNode( item, hItem, explorer );
                node.UpdateStatus( true, false );
                return node;
            }
            else
                return null;
        }

        public virtual void Refresh()
        {
            try
            {
                this.FindChildren( );
                this.UpdateStatus();
            }
            catch( SvnException )
            {
                // try refreshing the parent
                if ( this.parent != null )
                    parent.Refresh();
                else
                    throw;
            }
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
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.children;  }
        }

        public void UpdateStatus()
        {
            this.UpdateStatus( true, true );
        }


        /// <summary>
        /// Updates the status icon of this node and parents.
        /// </summary>
        protected void UpdateStatus( bool recursive, bool propagate )
        {      
            // update status on the children first           
            if ( recursive )
            {
                this.propagatedStatus = StatusKind.None;
                foreach( TreeNode node in this.Children )
                    node.UpdateStatus( true, false );
            }

            // text or property changes on the project file itself?
            try
            {
                // should be set either by the recursion above or by an explicit propagation
                if ( this.propagatedStatus != StatusKind.None )
                    this.currentStatus = this.propagatedStatus;
                else
                    this.currentStatus = this.GetStatus();
            }
            catch( StatusException stex )
            {
                Debug.WriteLine( stex.Message );
                this.currentStatus = StatusKind.Deleted;
            }

            this.SetStatusImage( this.currentStatus );

            // propagate to the parent nodes.
            if ( this.currentStatus != StatusKind.Normal && this.Parent != null )
                this.Parent.PropagateStatus( this.currentStatus );
            if ( propagate && this.parent != null )
                this.parent.UpdateStatus( false, true );

            this.propagatedStatus = StatusKind.None;
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
            [System.Diagnostics.DebuggerStepThrough]
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
        /// Intended to be called from child nodes if their status has changed.
        /// </summary>
        /// <param name="status"></param>
        protected virtual void PropagateStatus( StatusKind status )
        {
            if ( status == StatusKind.Added || status == StatusKind.Modified )
                this.propagatedStatus = StatusKind.Modified;
            else if ( status == StatusKind.None )
                this.propagatedStatus = StatusKind.Normal;
            else
                this.propagatedStatus = status;
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
        protected StatusKind StatusFromResource( ILocalResource resource )
        {
            
            Status status;

            // is it cached?
            if ( (status = this.Explorer.GetCachedStatus( resource.Path ) ) == null )
                status = resource.Status;

            if ( status.TextStatus != StatusKind.Normal )
                return status.TextStatus;
            else if ( status.PropertyStatus != StatusKind.Normal &&
                status.PropertyStatus != StatusKind.None )
                return status.PropertyStatus;  
            else
                return StatusKind.Normal;
        }

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        protected void FindChildren()
        {
            try
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
            catch( ArgumentException ex )
            {
                // thrown some times if the uiitem is invalid for some reason
                throw new SvnException( "Invalid UIHierarchyItem", ex );
            }
        }
        
        private UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private StatusKind currentStatus;
        private StatusKind propagatedStatus;
        private static IDictionary statusMap = new Hashtable();
    }
}
