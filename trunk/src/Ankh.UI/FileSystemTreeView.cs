using System;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    class FileSystemTreeView : PathTreeView
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

        internal IFileSystemItem[] GetSelectedItems()
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
            // get rid of the dummy node
            treeNode.Nodes.RemoveAt( 0 );

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
            TreeNode node = nodes.Add( child.Text );
            node.Tag = child;

            node.SelectedImageIndex = this.ClosedFolderIndex;
            node.ImageIndex = this.ClosedFolderIndex;

            TreeNode dummy = node.Nodes.Add( "DUMMY" );
            dummy.Tag = DummyTag;
        }

        private static readonly object DummyTag = new object();

   }
}
