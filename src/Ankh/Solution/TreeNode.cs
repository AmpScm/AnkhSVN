// $Id$
using System;
using EnvDTE;

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

        public event StatusChanged Changed;

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
                node.Refresh( false );
                //node.UpdateStatus( true, false );
                return node;
            }
            else
                return null;
        }

        public abstract void Accept( INodeVisitor visitor );

        public void Refresh()
        {
            this.Refresh( true );
        }

        public virtual void Refresh( bool rescan )
        {
            try
            {                
                if ( rescan )
                {
                    this.explorer.StatusCache.Status( this.Directory );
                    this.FindChildren( );
                }
                this.currentStatus = this.MergeStatuses( this.ThisNodeStatus(), 
                    this.CheckChildStatuses() );

                this.SetStatusImage( this.currentStatus );

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

        

        abstract public string Directory
        {
            get;
        }



        static TreeNode()
        {
            statusMap[ NodeStatus.Normal ]      = 1;
            statusMap[ NodeStatus.Added ]       = 2;
            statusMap[ NodeStatus.Deleted ]     = 3;
            statusMap[ NodeStatus.IndividualStatusesConflicting ] = 7;
            statusMap[ NodeStatus.Conflicted ]  = 6;
            statusMap[ NodeStatus.Unversioned ] = 8;
            statusMap[ NodeStatus.Modified ]    = 9;
            
        }
        

        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public IList Children
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.children;  }
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public TreeNode Parent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.parent; }
        }

        protected NodeStatus CurrentStatus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.currentStatus; }
        }

        /// <summary>
        /// Derived classes implement this method to append their resources
        /// to the list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void GetResources( IList list, bool getChildItems, 
            ResourceFilterCallback filter );
        

       /// <summary>
        /// Gets the status of the resources belonging to one specific node, 
        /// not including children.
        /// </summary>
        /// <returns></returns>
        protected virtual NodeStatus ThisNodeStatus()
        {
            return NodeStatus.None;
        }

        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        protected virtual void OnChanged()
        {
            this.SetStatusImage( this.CurrentStatus );
            if ( this.Changed != null )
                this.Changed( this, EventArgs.Empty );
        }

        /// <summary>
        /// Event handler for change events in child nodes or resources belonging
        /// to this node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void ChildOrResourceChanged( object sender, EventArgs args )
        {
            NodeStatus newStatus = this.MergeStatuses( this.ThisNodeStatus(), 
                this.CheckChildStatuses() );
            if ( newStatus != this.currentStatus )
            {
                this.currentStatus = newStatus;
                this.OnChanged();
            }
        }

        /// <summary>
        /// Sets the status image on this node.
        /// </summary>
        /// <param name="status">The status on this node.</param>
        protected void SetStatusImage( NodeStatus status )
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
        /// Returns a NodeStatus from a Status, taking into account both text status and 
        /// property status.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected NodeStatus GenerateStatus(Status status)
        {  
            if ( status.TextStatus != StatusKind.Normal )
                return (NodeStatus)status.TextStatus;
            else if ( status.PropertyStatus != StatusKind.Normal &&
                status.PropertyStatus != StatusKind.None )
                return (NodeStatus)status.PropertyStatus;  
            else
                return NodeStatus.Normal;
        }

        /// <summary>
        /// Merges the statuses of the passed items into a single NodeStatus.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected NodeStatus MergeStatuses( params SvnItem[] items )
        {
            return this.MergeStatuses( ((IList)items) );
        }

        /// <summary>
        /// Merges the passed NodeStatuses into a single NodeStatus.
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        protected NodeStatus MergeStatuses( params NodeStatus[] statuses )
        {
            StatusMerger merger = new StatusMerger();
            foreach( NodeStatus status in statuses )
                merger.NewStatus( status );
            return merger.CurrentStatus;
        }

        /// <summary>
        /// Merges the statuses of the passed SvnItems into
        /// a single NodeStatus.
        /// </summary>
        /// <param name="items">An IList of SvnItem instances.</param>
        /// <returns></returns>
        protected NodeStatus MergeStatuses( IList items )
        {   
            StatusMerger statusMerger = new StatusMerger();
            foreach( SvnItem item in items )
                statusMerger.NewStatus( this.GenerateStatus(item.Status) );

            return statusMerger.CurrentStatus;
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

                    if ( child.Name != SvnUtils.WC_ADMIN_AREA )
                    {                    
                        TreeNode childNode = TreeNode.CreateNode( child, childItem, this.explorer,
                            this );
                        if (childNode != null )
                        {
                            childNode.Changed += new StatusChanged(this.ChildOrResourceChanged);
                            this.children.Add( childNode );
                            childNode.Refresh( false );
                        }
                    }

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

        /// <summary>
        /// Gets the merged NodeStatus from the child nodes.
        /// </summary>
        /// <returns></returns>
        protected NodeStatus CheckChildStatuses()
        {            
            StatusMerger statusMerger = new StatusMerger();
            foreach( TreeNode node in this.children )
            {
                statusMerger.NewStatus( node.CurrentStatus );                
            }
            return statusMerger.CurrentStatus;
        }

        protected void GetChildResources(System.Collections.IList list, bool getChildItems,
            ResourceFilterCallback filter )
        {
            if ( getChildItems )
            {
                foreach( TreeNode node in this.Children )
                    node.GetResources( list, getChildItems, filter );
            }
        }


        /// <summary>
        /// Used for merging several NodeStatuses into a single NodeStatus.
        /// </summary>
        protected struct StatusMerger
        {
            public void NewStatus( NodeStatus status )
            {
                if ( this.CurrentStatus == NodeStatus.None )
                    this.CurrentStatus = status;
                else if ( status != NodeStatus.None )
                {
                    if ( this.CurrentStatus == NodeStatus.Normal )
                    {
                        this.CurrentStatus = status;
                    }
                    else if ( status != NodeStatus.Normal &&
                        this.CurrentStatus != status )
                    {
                        this.CurrentStatus = NodeStatus.IndividualStatusesConflicting;
                    }
                }
                                                    
            }

            public NodeStatus CurrentStatus
            {
                get
                {
                    if ( this.currentStatus == (NodeStatus)0 )
                        this.currentStatus = NodeStatus.None;
                    return this.currentStatus;
                }
                set{ this.currentStatus = value; }
            }
            private NodeStatus currentStatus;
        }

        /// <summary>
        /// Describes the status of a tree node.
        /// </summary>
        protected enum NodeStatus
        {
            None = StatusKind.None,
            Normal = StatusKind.Normal,
            Added = StatusKind.Added,
            Deleted = StatusKind.Deleted,
            Conflicted = StatusKind.Conflicted,
            Unversioned = StatusKind.Unversioned,
            Modified = StatusKind.Modified,
            IndividualStatusesConflicting
        }

        private UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private NodeStatus currentStatus;
        private static readonly IDictionary statusMap = new Hashtable();
    }
}
