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
    public abstract class SolutionExplorerTreeNode : TreeNode, IDisposable
    {


        protected SolutionExplorerTreeNode( UIHierarchyItem item, IntPtr hItem, 
            Explorer explorer, SolutionExplorerTreeNode parent ) : base(parent)
        {     
            this.uiItem = item;
            this.hItem = hItem;
            this.explorer = explorer;
        }


        public Explorer Explorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.explorer; }
        }

        public SolutionExplorerTreeNode SolutionExplorerParent
        {
            get { return this.Parent as SolutionExplorerTreeNode; }
        }

        public virtual SolutionNode Solution
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.SolutionExplorerParent != null ? this.SolutionExplorerParent.Solution : null; }
        }
        
        public static SolutionExplorerTreeNode CreateNode( UIHierarchyItem item, IntPtr hItem,
            Explorer explorer, SolutionExplorerTreeNode parent )
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

                object projectItemObject = DteUtils.GetProjectItemObject( projectItem );

                if ( projectItem.Kind == DteUtils.EnterpriseTemplateProjectItemKind && 
                    parent.uiItem.Object is Project &&
                    ((Project)parent.uiItem.Object).Kind == DteUtils.EnterpriseTemplateProjectKind )
                {
                    return new ProjectNode( item, hItem, explorer, parent, projectItem.SubProject );
                }
                else if ( projectItemObject is Project )
                {
                    return GetTreeNodeForProject( item, hItem, explorer, parent, projectItemObject as Project );
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
                ParsedSolutionItem parsedItem=((SolutionNode)projectNode.Parent).Parser.GetProjectItem(projectNode.Name, item.Name);
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

        private static SolutionExplorerTreeNode GetTreeNodeForProject( UIHierarchyItem item, IntPtr hItem, Explorer explorer, SolutionExplorerTreeNode parent, Project project )
        {
            switch ( project.Kind )
            {
                case DteUtils.SolutionItemsKind:
                    return new SolutionFolderNode( item, hItem, explorer, parent, project );
                default:
                    return new ProjectNode( item, hItem, explorer, parent, project );
            }
        }

        public static SolutionExplorerTreeNode CreateSolutionNode( UIHierarchyItem item, 
            Explorer explorer )
        {
            if ( explorer.DTE.Solution.FullName != string.Empty )
            {
                Debug.WriteLine( "Creating solution node " + item.Name, "Ankh" );
                SolutionExplorerTreeNode node = new SolutionNode( item, explorer.TreeView.GetRoot(), 
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

        

        public override void Refresh( bool rescan )
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
                this.CurrentStatus = MergeStatuses( this.ThisNodeStatus(), 
                    this.CheckChildStatuses() );

                this.SetStatusImage( this.CurrentStatus );

            }
            catch( SvnException )
            {
                // try refreshing the parent
                if ( this.Parent != null )
                    this.Parent.Refresh();
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

        protected override void DoDispose()
        {
            this.UnhookEvents();

            this.Explorer.RemoveUIHierarchyResource( this.uiItem );
        }

        protected abstract void UnhookEvents();


        /// <summary>
        /// Sets the status image on this node.
        /// </summary>
        /// <param name="status">The status on this node.</param>
        protected void SetStatusImage( NodeStatus status )
        {
            int statusImage = 0;
            int overlay = 0;
            statusImage = StatusImages.GetStatusImageForNodeStatus ( status );

            if ( status.ReadOnly && status.Locked )
                overlay = Explorer.LockReadonlyOverlay;
            else if ( status.ReadOnly )
                overlay = Explorer.ReadonlyOverlay;
            else if ( status.Locked )
               overlay = Explorer.LockOverlay;

            this.explorer.TreeView.SetStateAndOverlayImage( this.hItem, overlay, statusImage );                
        }

        

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        protected void FindChildren()
        {
            try
            {
                this.Children.Clear();

                // retain the original expansion state
                bool isExpanded = this.uiItem.UIHierarchyItems.Expanded;

                this.uiItem.UIHierarchyItems.Expanded = true;

                // get the treeview child
                IntPtr childItem = this.explorer.TreeView.GetChild( this.hItem );

                //// a node needs to be expanded at least once in order to have child nodes
                //if ( childItem == IntPtr.Zero && this.uiItem.UIHierarchyItems.Count > 0 )
                //{
                //    this.uiItem.UIHierarchyItems.Expanded = true;
                //    childItem = this.explorer.TreeView.GetChild( this.hItem );
                //}
                
                // iterate over the ui items and the treeview items in parallell
                foreach( UIHierarchyItem child in this.uiItem.UIHierarchyItems )
                {
                    Debug.Assert( childItem != IntPtr.Zero, 
                        "Could not get treeview item" );

                    if ( child.Name != Client.AdminDirectoryName )
                    {                    
                        SolutionExplorerTreeNode childNode = SolutionExplorerTreeNode.CreateNode( child, childItem, this.explorer,
                            this );
                        if (childNode != null )
                        {
                            childNode.Changed += new EventHandler(this.ChildOrResourceChanged);
                            this.Children.Add( childNode );
                            childNode.Refresh( false );

                            this.explorer.AddUIHierarchyItemResource( child, childNode );

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

        protected override void OnChanged()
        {
            base.OnChanged();

            this.SetStatusImage( this.CurrentStatus );
        }


        /// <summary>
        /// Called as part of a rescan of the current node.
        /// </summary>
        protected virtual void RescanHook()
        {
            // empty
        }
        
        protected UIHierarchyItem uiItem;
        private IntPtr hItem;
        private Explorer explorer;

#if DEBUG
        internal bool IsOrphaned()
        {
            if ( this.SolutionExplorerParent == null )
            {
                return this.explorer.SolutionNode != this;
            }
            else
            {
                return ( !this.SolutionExplorerParent.Children.Contains( this ) ) ||
                    this.SolutionExplorerParent.IsOrphaned();
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
