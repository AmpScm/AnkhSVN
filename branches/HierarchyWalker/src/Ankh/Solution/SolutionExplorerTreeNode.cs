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
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Solution
{
    /// <summary>
    /// Represents an item in the treeview.
    /// </summary>
    public abstract class SolutionExplorerTreeNode : TreeNode, IDisposable
    {


        protected SolutionExplorerTreeNode( uint itemID, 
            Explorer explorer, SolutionExplorerTreeNode parent ) : base(parent)
        {
            this.itemID = itemID;
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

        public static SolutionExplorerTreeNode CreateSolutionNode( UIHierarchyItem item, 
            Explorer explorer )
        {
            return null;
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

                this.SetStatusImage( this.Hierarchy, this.CurrentStatus );

                RefreshChildren();

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

        private void RefreshChildren()
        {
            foreach ( SolutionExplorerTreeNode node in this.Children )
            {
                node.Refresh( false );
            }
        }

        public void AddChild( SolutionExplorerTreeNode node )
        {
            this.Children.Add( node );
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

        abstract public IVsHierarchy Hierarchy
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
        protected void SetStatusImage( IVsHierarchy hierarchy, NodeStatus status )
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


           try
           {
               //Marshal.ThrowExceptionForHR(
               //      hierarchy.SetProperty( this.itemID, (int)__VSHPROPID.VSHPROPID_StateIconIndex, statusImage ) );
           }
           catch ( Exception )
           {
               Console.WriteLine("HI");
           }
            //this.explorer.TreeView.SetStateAndOverlayImage( this.hItem, overlay, statusImage );                
        }

        

        /// <summary>
        /// Finds the child nodes of this node.
        /// </summary>
        protected void FindChildren()
        {

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

            this.SetStatusImage( this.Hierarchy, this.CurrentStatus );
        }


        /// <summary>
        /// Called as part of a rescan of the current node.
        /// </summary>
        protected virtual void RescanHook()
        {
            // empty
        }
        
        protected UIHierarchyItem uiItem;
        private uint itemID;
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
