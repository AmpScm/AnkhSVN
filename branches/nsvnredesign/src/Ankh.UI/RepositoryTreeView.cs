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

    public class NodeExpandingEventArgs : EventArgs
    {
        public NodeExpandingEventArgs( IRepositoryTreeNode parentNode )
        {
            this.parentNode = parentNode;
            this.children = new IRepositoryTreeNode[]{};
        }

        /// <summary>
        /// The node being expanded.
        /// </summary>
        public IRepositoryTreeNode Node
        {
            get{ return this.parentNode; }
        }

        /// <summary>
        /// Event handlers set this to a list of IRepositoryTreeNode 
        /// instances.
        /// </summary>
        public IList Children
        {
            get{ return this.children; }
            set{ this.children = value; }
        }

        private IList children;
        private IRepositoryTreeNode parentNode;
    }

    public delegate void NodeExpandingDelegate( object sender, NodeExpandingEventArgs args );
    

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

        bool IsDirectory
        {
            get;
        }


        object Tag
        {
            get;
            set;
        }
        
    }

    /// <summary>
    /// Treeview that shows the layout of a SVN repository
    /// </summary>
    public class RepositoryTreeView : System.Windows.Forms.TreeView
    {
        public RepositoryTreeView()
        {
            this.GetSystemImageList();       
        }   
     
        public void AddRoot( IRepositoryTreeNode node, string url )
        {
            TreeNode root = new TreeNode( url, this.openFolderIndex, this.openFolderIndex );

            root.Tag = node;
            node.Tag = root;          

            this.Nodes.Add( root );

            TreeNode dummy = new TreeNode("");
            dummy.Tag = DUMMY_NODE;

            root.Nodes.Add( dummy );

            root.Expand();
        }

        public void AddChildren( IRepositoryTreeNode parent, IList childNodes )
        {
            TreeNode parentNode = (TreeNode)parent.Tag;
            this.BuildSubTree( parentNode.Nodes, childNodes );
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

        


        private void BuildSubTree( TreeNodeCollection nodes, IList nodeList )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // we have set a new root, so get rid of any existing nodes
                nodes.Clear();

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
                    child.Tag = newNode;
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

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        public static readonly object DUMMY_NODE = new object();
        private int openFolderIndex;
        private int closedFolderIndex;
    }

    
}
