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
    public abstract class TreeNode
    {

        public event StatusChanged Changed;

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
        
        public static TreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer, TreeNode parent )
        {
            Project project = item.Object as Project;
            // what kind of node is this?
            if ( project != null )
            {
                switch (project.Kind)
                {
                    case DteUtils.SolutionItemsKind:
                        return new SolutionFolderItem(item, hItem, explorer, parent, project);
                    default:
                        return new ProjectNode(item, hItem, explorer, parent, project);
                }
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
                    return new ProjectNode(item, hItem, explorer, parent, projectItem.Object as Project );
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

        

        abstract public string Directory
        {
            get;
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
                            childNode.Changed += new StatusChanged(this.ChildOrResourceChanged);
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
        protected void AddDeletions( string dir, IList list, StatusChanged del )
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


        /// <summary>
        /// Used for merging several NodeStatuses into a single NodeStatus.
        /// </summary>
        protected struct StatusMerger
        {
            public void NewStatus( NodeStatus status )
            {
                if ( this.currentStatus.Kind == NodeStatusKind.None || this.currentStatus.Kind == 0 )
                    this.currentStatus.Kind = status.Kind;
                else if ( status.Kind != NodeStatusKind.None && 
                    status.Kind != NodeStatusKind.Ignored &&
                    status.Kind != 0 )
                {
                    if ( this.currentStatus.Kind == NodeStatusKind.Normal )
                    {
                        this.currentStatus.Kind = status.Kind;
                    }
                    else if ( status.Kind != NodeStatusKind.Normal &&
                        this.currentStatus.Kind != status.Kind )
                    {
                        this.currentStatus.Kind = NodeStatusKind.IndividualStatusesConflicting;
                    }
                }

                if ( status.ReadOnly )
                    this.currentStatus.ReadOnly = true;

                if ( status.Locked )
                    this.currentStatus.Locked = true;
                                                    
            }

            public NodeStatus CurrentStatus
            {
                get
                {                    
                    return this.currentStatus;
                }
                set{ this.currentStatus = value; }
            }
            private NodeStatus currentStatus;
        }

        /// <summary>
        /// Describes the status of a tree node.
        /// </summary>
        protected struct NodeStatus
        {
            public NodeStatusKind Kind;
            public bool ReadOnly;
            public bool Locked;

            public NodeStatus( NodeStatusKind kind, bool readOnly, bool locked )
            {
                this.Kind = kind;
                this.ReadOnly = readOnly;
                this.Locked = locked;
            }

            public static bool operator==( NodeStatus s1, NodeStatus s2 )
            {
                return s1.Kind == s2.Kind && 
                        s1.Locked == s2.Locked && 
                        s1.ReadOnly == s2.ReadOnly;
            }

            public static bool operator!=( NodeStatus s1, NodeStatus s2 )
            {
                return !(s1 == s2);
            }

            public override bool Equals(object obj)
            {
                return this == (NodeStatus)obj;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode ();
            }



            public static readonly NodeStatus None = new NodeStatus( NodeStatusKind.None, false, false );
        }

        protected enum NodeStatusKind
        {
            None = StatusKind.None,
            Normal = StatusKind.Normal,
            Added = StatusKind.Added,
            Deleted = StatusKind.Deleted,
            Conflicted = StatusKind.Conflicted,
            Unversioned = StatusKind.Unversioned,
            Modified = StatusKind.Modified,
            Ignored = StatusKind.Ignored,
            Replaced = StatusKind.Replaced,
            IndividualStatusesConflicting
        }

        protected UIHierarchyItem uiItem;
        private TreeNode parent;
        private IntPtr hItem;
        private IList children;
        private Explorer explorer;
        private NodeStatus currentStatus;
        private static readonly IDictionary statusMap = new Hashtable();
    }
}
