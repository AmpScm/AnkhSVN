using System;
using System.Text;
using System.Collections;
using NSvn.Core;

namespace Ankh
{
    public abstract class TreeNode : IDisposable
    {
        public event EventHandler Changed;


        public TreeNode( TreeNode parent )
        {
            this.parent = parent;
            this.children = new ArrayList();
        }

        /// <summary>
        /// Make sure we unhook from all events pointing to us.
        /// </summary>
        public void Dispose()
        {
            this.DoDispose();
            this.DisposeChildren();
        }

        public void Remove()
        {
            this.RemoveSelf();
            this.Dispose();
        }

        /// <summary>
        /// Calls Dispose on all Children of this node.
        /// </summary>
        protected void DisposeChildren()
        {
            foreach ( TreeNode node in this.Children )
            {
                node.Dispose();
            }
        }

        /// <summary>
        /// Must be overridden in derived classes to unhook from events and other cleanup.
        /// </summary>
        protected abstract void DoDispose();
        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public IList Children
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.children; }
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public TreeNode Parent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.parent; }
        }

        /// <summary>
        /// Derived classes implement this method to append their resources
        /// to the list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void GetResources( IList list, bool getChildItems,
            ResourceFilterCallback filter );

        public void Refresh()
        {
            this.Refresh( true );
        }

        public abstract void Refresh( bool rescan );

        /// <summary>
        /// Override this to "kill" yourself if all resources belonging to you are deleted.
        /// </summary>
        /// <returns></returns>
        protected abstract bool RemoveTreeNodeIfResourcesDeleted();

        protected abstract void CheckForSvnDeletions();

        protected abstract void SvnDelete();

        protected NodeStatus CurrentStatus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.currentStatus; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.currentStatus = value; }
        }

        protected bool IsDeleting
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.isDeleting; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.isDeleting = value; }
        }

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
        /// Returns a NodeStatus from a Status, taking into account both text status and 
        /// property status.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected static NodeStatus GenerateStatus( SvnItem item )
        {
            Status status = item.Status;
            NodeStatusKind kind;
            if ( status.TextStatus != StatusKind.Normal )
            {
                kind = (NodeStatusKind)status.TextStatus;
            }
            else if ( status.PropertyStatus != StatusKind.Normal &&
                status.PropertyStatus != StatusKind.None )
            {
                kind = (NodeStatusKind)status.PropertyStatus;
            }
            else
            {
                kind = NodeStatusKind.Normal;
            }

            return new NodeStatus( kind, item.IsReadOnly, item.IsLocked );
        }

        /// <summary>
        /// Merges the statuses of the passed items into a single NodeStatus.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected static NodeStatus MergeStatuses( params SvnItem[] items )
        {
            return MergeStatuses( ( (IList)items ) );
        }

        /// <summary>
        /// Merges the passed NodeStatuses into a single NodeStatus.
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        protected static NodeStatus MergeStatuses( params NodeStatus[] statuses )
        {
            NodeStatus newStatus = new NodeStatus();
            foreach ( NodeStatus status in statuses )
                newStatus = newStatus.Merge( status );
            return newStatus;
        }

        /// <summary>
        /// Merges the statuses of the passed SvnItems into
        /// a single NodeStatus.
        /// </summary>
        /// <param name="items">An IList of SvnItem instances.</param>
        /// <returns></returns>
        protected static NodeStatus MergeStatuses( IList items )
        {
            NodeStatus newStatus = new NodeStatus();
            foreach ( SvnItem item in items )
                newStatus = newStatus.Merge( GenerateStatus( item ) );

            return newStatus;
        }

        /// <summary>
        /// Gets the merged NodeStatus from the child nodes.
        /// </summary>
        /// <returns></returns>
        protected NodeStatus CheckChildStatuses()
        {
            NodeStatus status = new NodeStatus();
            foreach ( TreeNode node in this.Children )
            {
                status = status.Merge( node.CurrentStatus );
            }
            return status;
        }



        protected void GetChildResources( System.Collections.IList list, bool getChildItems,
            ResourceFilterCallback filter )
        {
            if ( getChildItems )
            {
                foreach ( TreeNode node in this.Children )
                    node.GetResources( list, getChildItems, filter );
            }
        }

        protected void FilterResources( IList inList, IList outList, ResourceFilterCallback filter )
        {
            foreach ( SvnItem item in inList )
            {
                if ( filter == null || filter( item ) )
                {
                    outList.Add( item );
                }
            }
        }

        protected void RemoveSelf()
        {
            if ( this.Parent != null )
            {
                this.Parent.Remove( this );
            }
        }

        /// <summary>
        /// Event handler for change events in child nodes or resources belonging
        /// to this node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void ChildOrResourceChanged( object sender, EventArgs args )
        {
            if ( !this.IsDeleting )
            {
                this.CheckForSvnDeletions();
                if ( CheckForDeletedTreeNode() )
                {
                    return;
                }
            }

            NodeStatus newStatus = this.ThisNodeStatus().Merge( this.CheckChildStatuses() );
            if ( newStatus != this.CurrentStatus )
            {
                this.CurrentStatus = newStatus;
                this.OnChanged();
            }
        }

        protected static void UnhookEvents( IList svnItems, EventHandler del )
        {
            foreach ( SvnItem item in svnItems )
            {
                item.Changed -= del;
            }
        }

        /// <summary>
        /// Check if this treenode is deleted.
        /// </summary>
        /// <returns></returns>
        protected bool CheckForDeletedTreeNode()
        {
            try
            {
                this.IsDeleting = true;

                // If the parent is deleted as well, no point in deleting us.
                if ( this.Parent != null )
                {
                    if ( this.Parent.CheckForDeletedTreeNode() )
                    {
                        return true;
                    }
                }

                return RemoveTreeNodeIfResourcesDeleted();
            }
            finally
            {
                this.IsDeleting = false;
            }
        }

        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        protected virtual void OnChanged()
        {
            try
            {
                if ( !this.isRefreshing )
                {
                    this.isRefreshing = true;

                    if ( this.Changed != null )
                        this.Changed( this, EventArgs.Empty );
                }
            }
            finally
            {
                this.isRefreshing = false;
            }
        }

        private void Remove( TreeNode treeNode )
        {
            treeNode.Changed -= new EventHandler( this.ChildOrResourceChanged );
            this.Children.Remove( treeNode );
        }

        private bool isDeleting;
        private NodeStatus currentStatus;
        private TreeNode parent;
        private IList children;
        private bool isRefreshing;

    }
}
