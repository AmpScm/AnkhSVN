// $Id$
using System;
using System.Windows.Forms;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    /// <summary>
    /// A treeview that displays the system icons for paths.
    /// </summary>
    public class PathTreeView : TreeView
    {
        public PathTreeView()
        {
            this.GetSystemImageList();
        }

        public int ClosedFolderIndex
        {
            get{ return this.closedFolderIndex; }
        }

        public int OpenFolderIndex
        {
            get{ return this.openFolderIndex; }
        }



        /// <summary>
        /// Set the icon for a given node based on it's path.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        public void SetIcon( TreeNode node, string path )
        {
            SHFILEINFO fi = new SHFILEINFO();
            Win32.SHGetFileInfo( path, 0, ref fi, (uint)Marshal.SizeOf(fi),
                Constants.SHGFI_ICON | Constants.SHGFI_SHELLICONSIZE | 
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON |
                Constants.SHGFI_USEFILEATTRIBUTES );
            node.ImageIndex = fi.iIcon.ToInt32(); 
            node.SelectedImageIndex = fi.iIcon.ToInt32();
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
            e.Node.ImageIndex = this.OpenFolderIndex;
            e.Node.SelectedImageIndex = this.OpenFolderIndex;           
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



        private int closedFolderIndex;
        private int openFolderIndex;
    }
}
