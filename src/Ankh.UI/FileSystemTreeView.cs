using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ankh.UI
{
    class FileSystemTreeView : PathTreeView, IWorkingCopyExplorerSubControl
    {
        public event EventHandler SelectedItemChanged;

        public FileSystemTreeView()
        {
            this.HideSelection = false;
        }
       
    
        public IFileSystemItem SelectedItem
        {
            get
            {
                return this.SelectedNode != null ? this.SelectedNode.Tag as IFileSystemItem : null;
            }
            set
            {
                // Assume for now that it can only change to a direct child of the current node
                if ( this.SelectedItem != null )
                {
                    this.SelectedNode.Expand();

                    foreach ( TreeNode childNode in this.SelectedNode.Nodes)
                    {
                        if ( childNode.Tag.Equals( value ) )
                        {
                            this.SelectedNode = childNode;
                        }
                    }
                }
            }
        }
    
        public void AddRoot( IFileSystemItem rootItem )
        {
            this.AddNode( this.Nodes, rootItem );

            // select this node if it's the first one added
            if ( this.Nodes.Count == 1 )
            {
                this.SelectedNode = this.Nodes[0];
            }
        }

        public System.Drawing.Point GetSelectionPoint()
        {
            if ( this.SelectedNode != null )
            {
                int offset = this.SelectedNode.Bounds.Height / 3;
                return this.PointToScreen(new Point( this.SelectedNode.Bounds.X + offset, 
                    this.SelectedNode.Bounds.Y + offset ));
            }
            else
            {
                return Point.Empty;
            }
        }

        public IFileSystemItem[] GetSelectedItems()
        {
            if ( this.SelectedItem != null )
            {
                return new IFileSystemItem[] { this.SelectedItem };
            }
            else
            {
                return new IFileSystemItem[] { };
            }
        }

        internal void RemoveRoot( IFileSystemItem root )
        {
            TreeNode nodeToRemove = null;
            foreach ( TreeNode node in this.Nodes )
            {
                if ( node.Tag == root )
                {
                    nodeToRemove = node;
                    break;
                }
            }

            if ( nodeToRemove != null )
            {
                nodeToRemove.Remove();
            }
        }
 

        protected override void OnMouseDown( MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Right )
            {
                this.SelectedNode = this.GetNodeAt( e.X, e.Y );
            }

            base.OnMouseDown( e );
        }

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            base.OnAfterSelect( e );

            if ( this.SelectedItemChanged != null )
            {
                this.SelectedItemChanged( this, EventArgs.Empty );
            }
        }

        protected override void OnBeforeExpand( TreeViewCancelEventArgs e )
        {
            base.OnBeforeExpand( e );

            if ( e.Node.Nodes.Count > 0 && e.Node.Nodes[ 0 ].Tag == DummyTag )
            {
                this.FillNode( e.Node );
            }
        }

        private void FillNode( TreeNode treeNode )
        {
            // get rid of the dummy node or existing nodes
            treeNode.Nodes.Clear();

            IFileSystemItem item = treeNode.Tag as IFileSystemItem;

            foreach ( IFileSystemItem child in item.GetChildren() )
            {
                if ( child.IsContainer )
                {
                    this.AddNode( treeNode.Nodes, child );
                }
            }
        }

        private void AddNode( TreeNodeCollection nodes, IFileSystemItem child )
        {
            child.ItemChanged += new EventHandler<ItemChangedEventArgs>( child_ItemChanged );
            TreeNode node = nodes.Add( child.Text );
            node.Tag = child;

            node.SelectedImageIndex = this.ClosedFolderIndex;
            node.ImageIndex = this.ClosedFolderIndex;

            TreeNode dummy = node.Nodes.Add( "DUMMY" );
            dummy.Tag = DummyTag;
        }

        void child_ItemChanged( object sender, ItemChangedEventArgs e )
        {
            IFileSystemItem item = sender as IFileSystemItem;
            if ( e.ItemChangedType == ItemChangedType.ChildrenInvalidated && item != null )
            {
                this.HandleItemChanged( item );
            }
        }

        private void HandleItemChanged( IFileSystemItem item )
        {
            TreeNode node = this.SearchForNodeRecursivelyByTag( this.Nodes, item );
            if ( node != null)
            {
                this.RecursivelyUnhookFromEvents( node.Nodes );
                this.FillNode( node );
            }
        }

        private void RecursivelyUnhookFromEvents( TreeNodeCollection nodes )
        {
            foreach ( TreeNode node in nodes )
            {
                IFileSystemItem item = node.Tag as IFileSystemItem;
                if ( item != null )
                {
                    item.ItemChanged -= new EventHandler<ItemChangedEventArgs>( this.child_ItemChanged );
                }

                RecursivelyUnhookFromEvents( node.Nodes );
            }
        }

        private TreeNode SearchForNodeRecursivelyByTag( TreeNodeCollection coll, object tag )
        {
            foreach ( TreeNode node in coll )
            {
                if ( node.Tag == tag )
                {
                    return node;
                }
                TreeNode foundNode = SearchForNodeRecursivelyByTag( node.Nodes, tag );
                if ( foundNode != null )
                {
                    return foundNode;
                }
            }
            return null;
        }

        private static readonly object DummyTag = new object();



    }
}
