// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    /// <summary>
    /// Treeview that shows the layout of a SVN repository
    /// </summary>
    public class RepositoryTreeView : System.Windows.Forms.TreeView
    {
        public RepositoryTreeView()
        {
            this.GetSystemImageList();
            
            this.ContextMenu = new ContextMenu();
        }        

        /// <summary>
        /// The root folder of the repository to be explored by this treeview.
        /// </summary>
        public RepositoryDirectory RepositoryRoot
        {
            get{ return this.root; }
            set
            { 
                this.root = value;               
                this.Nodes.Clear();
                
                if( this.root != null )
                {
                    this.Nodes.Add( new TreeNode( this.root.Url ) );
                    this.BuildSubTree( this.Nodes[0].Nodes, this.root );
                    this.Nodes[0].Expand();
                }
            }
        }

        private void BuildSubTree( TreeNodeCollection nodes, RepositoryDirectory dir )
        {
            // set the wait cursor
            //Cursor current = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            // we have set a new root, so get rid of any existing nodes
            nodes.Clear();

            if( this.root == null )
                return;

            ResourceVisitor visitor = new ResourceVisitor( nodes, this );

            // get the children of the root
            RepositoryResourceDictionary dict = dir.GetChildren();
            foreach( RepositoryResource res in dict.Values )
            {
                res.Context = dir.Context;
                res.Accept( visitor );
            }

            this.Cursor = Cursors.Default;
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


        #region ResourceVisitor
        /// <summary>
        /// Inner class to traverse a list of RepositoryResource objects.
        /// </summary>
        private class ResourceVisitor : IRepositoryResourceVisitor
        {
            public ResourceVisitor( TreeNodeCollection nodes, RepositoryTreeView tree )
            {
                this.nodes = nodes; 
                this.tree = tree;
            }
            
            public void VisitFile( RepositoryFile file )
            {
                TreeNode node = new TreeNode( file.Name );
                node.Tag = file;
                this.nodes.Add( node );

                // set the icon
                SHFILEINFO fi = new SHFILEINFO();
                Win32.SHGetFileInfo( file.Name, 0, ref fi, (uint)Marshal.SizeOf(fi),
                    Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                    Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON |
                    Constants.SHGFI_USEFILEATTRIBUTES );
                node.ImageIndex = fi.iIcon.ToInt32(); 
                node.SelectedImageIndex = fi.iIcon.ToInt32();
            }

            public void VisitDirectory( RepositoryDirectory dir )
            {
                TreeNode dummy = new TreeNode( "" );
                dummy.Tag = DUMMY_NODE;

                TreeNode node = new TreeNode( dir.Name, 
                    new TreeNode[]{ dummy } );
                node.Tag = dir;
                this.nodes.Add( node );

                // start with the closed folder icon
                node.ImageIndex = this.tree.closedFolderIndex;
                node.SelectedImageIndex = this.tree.closedFolderIndex;
            }

            private TreeNodeCollection nodes;
            private RepositoryTreeView tree;

        }
        #endregion

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
                RepositoryDirectory dir = (RepositoryDirectory)e.Node.Tag;
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private RepositoryDirectory root;
        private static readonly object DUMMY_NODE = new object();
        private int openFolderIndex;
        private int closedFolderIndex;
    }
}
