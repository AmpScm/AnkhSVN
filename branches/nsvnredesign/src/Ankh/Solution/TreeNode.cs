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

                this.currentStatus = this.NodeStatus();
                this.CheckChildStatuses();

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

        

        abstract protected string Directory
        {
            get;
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

        public StatusKind CurrentStatus
        {
            get{ return this.currentStatus; }
        }


        /// <summary>
        /// Gets the status of the resources belonging to one specific node, 
        /// not including children.
        /// </summary>
        /// <returns></returns>
        protected virtual StatusKind NodeStatus()
        {
            return StatusKind.None;
        }

        protected virtual void OnChanged()
        {
            if ( this.Changed != null )
                this.Changed( this, EventArgs.Empty );
        }

        protected virtual void ChildChanged( object sender, EventArgs args )
        {
            this.CheckChildStatuses();
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
        protected StatusKind GenerateStatus(Status status)
        {  
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

                    if ( child.Name != SvnUtils.WC_ADMIN_AREA )
                    {                    
                        TreeNode childNode = TreeNode.CreateNode( child, childItem, this.explorer,
                            this );
                        if (childNode != null )
                        {
                            childNode.Changed += new StatusChanged(this.ChildChanged);                            
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

        private void CheckChildStatuses()
        {
            StatusKind newStatus = this.currentStatus;
            foreach( TreeNode node in this.children )
            {
                newStatus = node.CurrentStatus;
                if ( newStatus != this.CurrentStatus )
                {
                    this.currentStatus = newStatus;
                    this.OnChanged();
                    break;
                }
            }
        }
        
        private UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private StatusKind currentStatus;
        private static IDictionary statusMap = new Hashtable();
    }
}
