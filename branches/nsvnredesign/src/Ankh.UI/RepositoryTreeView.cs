// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    public interface IRepositoryTreeController
    {
        event EventHandler RootChanged;

        /// <summary>
        /// The text to put in the root.
        /// </summary>
        string RootText
        {
            get;
        }

        void SetRepository( string url, Revision revision );

        /// <summary>
        /// The IRepositoryTreeView associated with this controller.
        /// </summary>
        IRepositoryTreeView TreeView
        {
            set;
        }
           

        IRepositoryTreeNode RootNode
        {
            get;
        }
    }

    /// <summary>
    /// Represents a node in the tree.
    /// </summary>
    public interface IRepositoryTreeNode
    {
        /// <summary>
        /// The filename.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Whether the node is a directory.
        /// </summary>
        bool IsDirectory
        {
            get;
        }

        /// <summary>
        /// The child nodes - IRepositoryTreeNode objects.
        /// </summary>
        ICollection GetChildren();


        /// <summary>
        /// An arbitrary piece of data associated with the node.
        /// </summary>
        object Tag
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Treeview that shows the layout of a SVN repository
    /// </summary>
    public class RepositoryTreeView : System.Windows.Forms.TreeView, IRepositoryTreeView
    {
        public RepositoryTreeView()
        {
            this.GetSystemImageList();       
        }    
    
        /// <summary>
        /// Sets or gets the IRepositoryTreeController associated with this control.
        /// </summary>
        public IRepositoryTreeController Controller
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.controller; }

            set
            {               
                this.controller = value;   
                if ( this.controller != null )
                {
                    this.controller.RootChanged += new EventHandler( this.RootChanged );

                    this.RootChanged( null, EventArgs.Empty );
                }
            }
        }

        /// <summary>
        /// The number of selected nodes.
        /// </summary>
        public int SelectionCount
        {
            get{ return this.SelectedNode != null ? 1 : 0; }
        }

        /// <summary>
        /// Returns the selected nodes.
        /// </summary>
        public IRepositoryTreeNode[] SelectedNodes
        {
            get
            { 
                if ( this.SelectedNode != null )
                    return new IRepositoryTreeNode[]{ (IRepositoryTreeNode)this.SelectedNode.Tag };
                else
                    return new IRepositoryTreeNode[]{};
            }
        }

        /// <summary>
        /// Start loading the tree.
        /// </summary>
        public void Go()
        {
            if ( this.controller == null ) 
                throw new ApplicationException( "No repository controller set" );

            this.BuildSubTree( this.Nodes[0].Nodes, this.controller.RootNode );
        }
        

        /// <summary>
        /// Retrieve the system image list and assign it to this treeview.
        /// </summary>
        private void GetSystemImageList()
        {
            // get the system image list
            SHFILEINFO fileinfo = new SHFILEINFO();;
            IntPtr sysImageList = Win32.SHGetFileInfo( "", 0, ref fileinfo, 
                (uint)Marshal.SizeOf(fileinfo), Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON );

            // assign it to this treeview
            Win32.SendMessage( this.Handle, Msg.TVM_SETIMAGELIST, Constants.TVSIL_NORMAL,
                sysImageList );

            // get the open folder icon
            Win32.SHGetFileInfo( "", Constants.FILE_ATTRIBUTE_DIRECTORY, ref fileinfo, 
                (uint)Marshal.SizeOf(fileinfo), Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON | Constants.SHGFI_OPENICON |
                Constants.SHGFI_USEFILEATTRIBUTES );
            this.openFolderIndex = fileinfo.iIcon.ToInt32();

            // get the closed folder icon
            Win32.SHGetFileInfo( "", Constants.FILE_ATTRIBUTE_DIRECTORY, ref fileinfo, 
                (uint)Marshal.SizeOf(fileinfo), Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON | 
                Constants.SHGFI_USEFILEATTRIBUTES );
            this.closedFolderIndex = fileinfo.iIcon.ToInt32();


        }        

        /// <summary>
        /// Event handler for the Expand event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnBeforeExpand(
            System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand( e );
            // is this uninitialized?
            if ( e.Node.Nodes[0].Tag == DUMMY_NODE )
            {
                IRepositoryTreeNode dir = (IRepositoryTreeNode)e.Node.Tag;
                this.BuildSubTree( e.Node.Nodes, dir );
            }
            // switch to the open folder icon
            e.Node.ImageIndex = this.openFolderIndex;
            e.Node.SelectedImageIndex = this.openFolderIndex;
           
        } 

        /// <summary>
        /// Handle the collapse event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCollapse( 
            System.Windows.Forms.TreeViewEventArgs e )
        {
            base.OnAfterCollapse( e );

            // switch to the closed folder icon
            e.Node.ImageIndex = this.closedFolderIndex;
            e.Node.SelectedImageIndex = this.closedFolderIndex;
        }

        private void RootChanged( object o, EventArgs e )
        {
            this.Nodes.Clear();

            TreeNode root = new TreeNode( this.controller.RootText, this.openFolderIndex, this.openFolderIndex );
            root.Tag = this.controller.RootNode;
            root.Expand();

            this.Nodes.Add( root );
        }


        private void BuildSubTree( TreeNodeCollection nodes, IRepositoryTreeNode node )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // we have set a new root, so get rid of any existing nodes
                nodes.Clear();

                ArrayList nodeList = new ArrayList( node.GetChildren() );
                nodeList.Sort( RepositoryTreeView.Comparer );

                foreach( IRepositoryTreeNode child in nodeList )
                {
                    TreeNode newNode;

                    if ( child.IsDirectory )
                    {
                        TreeNode dummy = new TreeNode( "" );
                        dummy.Tag = DUMMY_NODE;

                        newNode = new TreeNode( child.Name, 
                            new TreeNode[]{ dummy } );

                        // start with the closed folder icon
                        newNode.ImageIndex = this.closedFolderIndex;
                        newNode.SelectedImageIndex = this.closedFolderIndex;
                    }
                    else
                    {
                        newNode = new TreeNode( child.Name);

                        // set the icon
                        SHFILEINFO fi = new SHFILEINFO();
                        Win32.SHGetFileInfo( child.Name, 0, ref fi, (uint)Marshal.SizeOf(fi),
                            Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                            Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON |
                            Constants.SHGFI_USEFILEATTRIBUTES );
                        newNode.ImageIndex = fi.iIcon.ToInt32(); 
                        newNode.SelectedImageIndex = fi.iIcon.ToInt32();

                    }

                    newNode.Tag = child;
                    nodes.Add( newNode );

                } // foreach

            }
            catch( ApplicationException )
            {
                this.Nodes.Clear();
                this.Nodes.Add( new TreeNode( "An error occurred",  this.openFolderIndex, this.openFolderIndex ) );
            }
            finally
            {           
                this.Cursor = Cursors.Default;
            }
        }

        private class NodeComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                IRepositoryTreeNode n1 = (IRepositoryTreeNode)x;
                IRepositoryTreeNode n2 = (IRepositoryTreeNode)y;
                
                if( (n1.IsDirectory && n2.IsDirectory) || 
                    (!n1.IsDirectory && !n2.IsDirectory) )
                    return n1.Name.CompareTo(n2.Name);
                else if ( n1.IsDirectory && !n2.IsDirectory )
                    return -1;
                else 
                    return 1;
            }
            #endregion
        }



        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private static readonly object DUMMY_NODE = new object();
        private int openFolderIndex;
        private int closedFolderIndex;

        private static readonly NodeComparer Comparer = new NodeComparer();

        private IRepositoryTreeController controller;
    }

    
}
