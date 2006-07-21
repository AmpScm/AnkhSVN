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
    public abstract class TreeNode : IDisposable
    {

        public event EventHandler Changed;

        protected TreeNode( UIHierarchyItem item, IntPtr hItem, 
            Explorer explorer, TreeNode parent )
        {     
            this.uiItem = item;
            this.hItem = hItem;
            this.explorer = explorer;
            this.parent = parent;
        }


        public Explorer Explorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.explorer; }
        }

        public virtual SolutionNode Solution
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.Parent.Solution; }
        }
        
        public static TreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer, TreeNode parent )
        {
            Project project = item.Object as Project;
            // what kind of node is this?
            if ( project != null )
            {
                return GetTreeNodeForProject( item, hItem, explorer, parent, project );
            }
            else if ( item.Object is ProjectItem )
            {
                ProjectItem projectItem = item.Object as ProjectItem;
                // Check if we have a subproject inside an Enterprise Template project
                if ( projectItem.Kind == DteUtils.EnterpriseTemplateProjectItemKind && 
                    parent.uiItem.Object is Project &&
                    ((Project)parent.uiItem.Object).Kind == DteUtils.EnterpriseTemplateProjectKind )
                {
                    return new ProjectNode( item, hItem, explorer, parent, projectItem.SubProject );
                }
                else if ( projectItem.Object is Project )
                {
                    return GetTreeNodeForProject( item, hItem, explorer, parent, projectItem.Object as Project );
                }
                else  //normal project item
                {
                    return new ProjectItemNode(item, hItem, explorer, parent, null);
                }
            }
            else if ( parent is SolutionNode ) //deal with unmodeled projects (database)
            {
                foreach(Project p in Enumerators.EnumerateProjects(explorer.DTE))
                {
                    if(p.Name==item.Name)
                    {
                        return new ProjectNode( item, hItem, explorer, parent, p );
                    }
                }
            }
            else if ( parent is ProjectNode && !((ProjectNode)parent).Modeled ) //deal with items in unmodeled projects
            {
                ProjectNode projectNode=(ProjectNode)parent;
                ParsedSolutionItem parsedItem=((SolutionNode)projectNode.parent).Parser.GetProjectItem(projectNode.Name, item.Name);
                return new ProjectItemNode( item, hItem, explorer, parent, parsedItem );  
            }
            else if ( parent is ProjectItemNode ) //deal with sub items in unmodeled projects
            {
                ProjectItemNode parentNode=(ProjectItemNode)parent;
                if ( parentNode.ParsedItem != null )
                {
                    ParsedSolutionItem parsedItem = parentNode.ParsedItem.GetChild( item.Name );
                    return new ProjectItemNode( item, hItem, explorer, parent, parsedItem );
                }
            }

            return null;
        }

        private static TreeNode GetTreeNodeForProject( UIHierarchyItem item, IntPtr hItem, Explorer explorer, TreeNode parent, Project project )
        {
            switch ( project.Kind )
            {
                case DteUtils.SolutionItemsKind:
                    return new SolutionFolderNode( item, hItem, explorer, parent, project );
                default:
                    return new ProjectNode( item, hItem, explorer, parent, project );
            }
        }

        public static TreeNode CreateSolutionNode( UIHierarchyItem item, 
            Explorer explorer )
        {
            if ( explorer.DTE.Solution.FullName != string.Empty )
            {
                Debug.WriteLine( "Creating solution node " + item.Name, "Ankh" );
                TreeNode node = new SolutionNode( item, explorer.TreeView.GetRoot(), 
                    explorer );
                node.Refresh( false );
                return node;
            }
            else
            {
                Debug.WriteLine( "No solution found", "Ankh" );
                return null;
            }
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
        private void DisposeChildren()
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
                    if (this.Directory != null && this.Directory.Length > 0)
                        this.explorer.Context.StatusCache.Status(this.Directory);

                    this.DisposeChildren();
                    this.FindChildren( );
                    this.RescanHook();
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

        

        /// <summary>
        /// Thedirectory represented by this node.
        /// </summary>
        abstract public string Directory
        {
            get;
        }
        
        /// <summary>
        /// A list of deleted resources belonging to this node.
        /// </summary>
        abstract protected IList DeletedItems
        {
            get;
        }

        /// <summary>
        /// Handles status change events from deleted items belonging to a node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void DeletedItemStatusChanged( object sender, EventArgs args )
        {
            SvnItem item = sender as SvnItem;
            if ( item != null && !item.IsDeleted && this.DeletedItems.Contains( item ) )
            {
                item.Changed -= new EventHandler( this.DeletedItemStatusChanged );
                this.DeletedItems.Remove( item );
                this.ChildOrResourceChanged(sender, args);
            }
        }



        static TreeNode()
        {
            statusMap[ NodeStatusKind.Normal ]      = 1;
            statusMap[ NodeStatusKind.Added ]       = 2;
            statusMap[ NodeStatusKind.Deleted ]     = 3;
            statusMap[ NodeStatusKind.Replaced ]    = 4;
            statusMap[ NodeStatusKind.IndividualStatusesConflicting ] = 7;
            statusMap[ NodeStatusKind.Conflicted ]  = 6;
            statusMap[ NodeStatusKind.Unversioned ] = 8;
            statusMap[ NodeStatusKind.Modified ]    = 9;
            
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
            try
            {
                if ( !this.isRefreshing )
                {
                    this.isRefreshing = true;

                    this.SetStatusImage( this.CurrentStatus );
                    if ( this.Changed != null )
                        this.Changed( this, EventArgs.Empty );
                }
            }
            finally
            {
                this.isRefreshing = false;
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
            if ( !this.isDeleting )
            {
                this.CheckForSvnDeletions();
                if ( CheckForDeletedTreeNode() )
                {
                    return;
                }
            }

            NodeStatus newStatus = this.MergeStatuses( this.ThisNodeStatus(), 
                this.CheckChildStatuses() );
            if ( newStatus != this.currentStatus )
            {
                this.currentStatus = newStatus;
                this.OnChanged();
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
                this.isDeleting = true;

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
                this.isDeleting = false;
            }
        }

        /// <summary>
        /// Override this to "kill" yourself if all resources belonging to you are deleted.
        /// </summary>
        /// <returns></returns>
        protected abstract bool RemoveTreeNodeIfResourcesDeleted();

        protected abstract void CheckForSvnDeletions();

        /// <summary>
        /// Deletes all resources belonging to this node and its children.
        /// </summary>
        protected void SvnDelete()
        {
            try
            {
                this.isDeleting = true;

                IList resources = new ArrayList();
                this.GetResources( resources, true, new ResourceFilterCallback( SvnItem.VersionedFilter ) );

                ArrayList resourcePaths = new ArrayList();
                foreach ( SvnItem item in resources )
                {
                    if ( !item.IsDeleted )
                    {
                        resourcePaths.Add( item.Path );
                    }
                }

                this.explorer.Context.Client.Delete( (string[])resourcePaths.ToArray( typeof( string ) ), true );

                foreach ( SvnItem item in resources )
                {
                    item.Refresh( this.explorer.Context.Client );
                }
            }
            finally
            {
                this.isDeleting = false;
            }

        }

        protected void FilterResources( IList inList, IList outList, ResourceFilterCallback filter )
        {
            foreach ( SvnItem item in inList )
            {
                if ( filter == null || filter(item) )
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

        private void Remove( TreeNode treeNode )
        {
            treeNode.Changed -= new EventHandler( this.ChildOrResourceChanged );
            this.Children.Remove( treeNode );
        }

        protected void UnhookEvents( IList svnItems, EventHandler del )
        {
            foreach ( SvnItem item in svnItems )
            {
                item.Changed -= del;
            }
        }

        /// <summary>
        /// Sets the status image on this node.
        /// </summary>
        /// <param name="status">The status on this node.</param>
        protected void SetStatusImage( NodeStatus status )
        {
            int statusImage = 0;
            int overlay = 0;
            if ( statusMap.Contains(status.Kind) )
                statusImage = (int)statusMap[status.Kind];

            if ( status.ReadOnly && status.Locked )
                overlay = Explorer.LockReadonlyOverlay;
            else if ( status.ReadOnly )
                overlay = Explorer.ReadonlyOverlay;
            else if ( status.Locked )
               overlay = Explorer.LockOverlay;

            this.explorer.TreeView.SetStateAndOverlayImage( this.hItem, overlay, statusImage );                
        }

        /// <summary>
        /// Returns a NodeStatus from a Status, taking into account both text status and 
        /// property status.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        protected NodeStatus GenerateStatus(SvnItem item)
        {  
            Status status = item.Status;
            NodeStatus newStatus = new NodeStatus();
            if ( status.TextStatus != StatusKind.Normal )
                newStatus.Kind = (NodeStatusKind)status.TextStatus;
            else if ( status.PropertyStatus != StatusKind.Normal &&
                status.PropertyStatus != StatusKind.None )
                newStatus.Kind = (NodeStatusKind)status.PropertyStatus;  
            else
                newStatus.Kind = NodeStatusKind.Normal;

            newStatus.ReadOnly = item.IsReadOnly;
            newStatus.Locked = item.IsLocked;

            return newStatus;
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
                statusMerger.NewStatus( this.GenerateStatus(item) );

            return statusMerger.CurrentStatus;
        }

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        protected void FindChildren()
        {
            try
            {                
                this.children = new ArrayList();

                // retain the original expansion state
                bool isExpanded = this.uiItem.UIHierarchyItems.Expanded;

                // get the treeview child
                IntPtr childItem = this.explorer.TreeView.GetChild( this.hItem );

                // a node needs to be expanded at least once in order to have child nodes
                if ( childItem == IntPtr.Zero && this.uiItem.UIHierarchyItems.Count > 0 )
                {
                    this.uiItem.UIHierarchyItems.Expanded = true;
                    childItem = this.explorer.TreeView.GetChild( this.hItem );
                }
                
                // iterate over the ui items and the treeview items in parallell
                foreach( UIHierarchyItem child in this.uiItem.UIHierarchyItems )
                {
                    Debug.Assert( childItem != IntPtr.Zero, 
                        "Could not get treeview item" );

                    if ( child.Name != Client.AdminDirectoryName )
                    {                    
                        TreeNode childNode = TreeNode.CreateNode( child, childItem, this.explorer,
                            this );
                        if (childNode != null )
                        {
                            childNode.Changed += new EventHandler(this.ChildOrResourceChanged);
                            this.children.Add( childNode );
                            childNode.Refresh( false );
                        }
                    }

                    // and the next child
                    childItem = this.explorer.TreeView.GetNextSibling( childItem );               
                }
                
                this.uiItem.UIHierarchyItems.Expanded = isExpanded;
            }
            catch( ArgumentException )
            {
                // thrown some times if the uiitem is invalid for some reason
                this.explorer.Context.OutputPane.StartActionText( "ERROR" );
                this.explorer.Context.OutputPane.WriteLine( 
                    "ERROR: ArgumentException thrown by automation object. " + 
                    "This project may not function correctly." );
                this.explorer.Context.OutputPane.WriteLine(
                    "(Is this a third party project type?)" );
                this.explorer.Context.OutputPane.EndActionText();
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
        /// Add the deleted items from "dir" to "list".
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="list"></param>
        /// <param name="del"></param>
        protected void AddDeletions( string dir, IList list, EventHandler del )
        {
            IList deletions = this.Explorer.Context.StatusCache.GetDeletions( 
                dir );
            foreach( SvnItem deletedItem in deletions )
            {
                list.Add( deletedItem );
                deletedItem.Changed += del;
            } 
        }


        /// <summary>
        /// Called as part of a rescan of the current node.
        /// </summary>
        protected virtual void RescanHook()
        {
            // empty
        }
        
        protected UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private NodeStatus currentStatus;
        private bool isDeleting = false;
        private bool isRefreshing = false;
        private static readonly IDictionary statusMap = new Hashtable();

#if DEBUG
        internal bool IsOrphaned()
        {
            if ( this.Parent == null )
            {
                return this.explorer.SolutionNode != this;
            }
            else
            {
                return ( !this.Parent.Children.Contains( this ) ) ||
                    this.Parent.IsOrphaned();
            }
        }
#endif

       
    }

#if DEBUG

    [VSNetCommand("CheckForOrphanedTreeNodes", Text="Check for orphaned tree nodes", 
        Bitmap=ResourceBitmaps.Default, 
        Tooltip="Check for orphaned tree nodes")]
    [VSNetControl( "Tools.AnkhSVN", Position=1 )]
    public class CheckForOrphanedTreeNodes : Ankh.Commands.CommandBase
    {
        public override vsCommandStatus QueryStatus( IContext context )
        {
            return Enabled;
        }

        public override void Execute( IContext context, string parameters )
        {
            context.StatusCache.ScanForOrphanedTreeNodes(context.OutputPane);
        }
    }
#endif
}
