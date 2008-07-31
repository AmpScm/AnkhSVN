using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Ankh.UI.Services;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell;
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.WorkingCopyExplorer;
using SharpSvn;

namespace Ankh.UI.WorkingCopyExplorer
{
    public interface IWorkingCopyExplorerSubControl
    {
        Point GetSelectionPoint();
        IFileSystemItem[] GetSelectedItems();
    }

    public partial class WorkingCopyExplorerControl : AnkhToolWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingCopyExplorerControl"/> class.
        /// </summary>
        public WorkingCopyExplorerControl()
        {
            this.InitializeComponent();

            this.folderTree.SelectedItemChanged += new EventHandler(treeView_SelectedItemChanged);
            this.fileList.CurrentDirectoryChanged += new EventHandler(listView_CurrentDirectoryChanged);

            this.folderTree.MouseDown += new MouseEventHandler(HandleMouseDown);
            this.fileList.MouseDown += new MouseEventHandler(HandleMouseDown);
        }

        /// <summary>
        /// Called when the frame is created
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowSite.CommandContext = AnkhId.SccExplorerContextGuid;
            ToolWindowSite.KeyboardContext = AnkhId.SccExplorerContextGuid;

            folderTree.Context = Context;
            fileList.Context = Context;

            folderTree.SelectionPublishServiceProvider = Context;
            fileList.SelectionPublishServiceProvider = Context;

            fileList.StateImageList = Context.GetService<IStatusImageMapper>().StatusImageList;
        }

        public void AddRoot(IFileSystemItem root)
        {
            this.folderTree.AddRoot(root);
        }

        public void RemoveRoot(IFileSystemItem root)
        {
            this.folderTree.RemoveRoot(root);
        }

        public void RefreshItem(IFileSystemItem item)
        {
            throw new System.NotImplementedException();
        }

        public IFileSystemItem[] GetSelectedItems()
        {
            if (this.SelectedControl != null)
            {
                this.selection = this.SelectedControl.GetSelectedItems();
            }

            // if none are focused, whatever selection was there before is probably still valid

            return this.selection;
        }

        protected IWorkingCopyExplorerSubControl SelectedControl
        {
            get
            {
                if (this.folderTree.Focused)
                {
                    return this.folderTree;
                }
                else if (this.fileList.Focused)
                {
                    return this.fileList;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Handle the key combinations for "right click menu" here.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData == (Keys)(Keys.F10 | Keys.Shift) || keyData == Keys.Apps)
            {
                if (this.SelectedControl != null)
                {
                    Point point = this.SelectedControl.GetSelectionPoint();
                    this.ShowContextMenu(point);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void treeView_SelectedItemChanged(object sender, EventArgs e)
        {
            IFileSystemItem item = this.folderTree.SelectedItem;
            this.fileList.SetDirectory(item);
        }

        void listView_CurrentDirectoryChanged(object sender, EventArgs e)
        {
            this.folderTree.SelectedItemChanged -= new EventHandler(this.treeView_SelectedItemChanged);
            try
            {
                this.folderTree.SelectedItem = this.fileList.CurrentDirectory;
            }
            finally
            {
                this.folderTree.SelectedItemChanged += new EventHandler(this.treeView_SelectedItemChanged);
            }
        }

        void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Control c = sender as Control;
                if (c == null)
                {
                    return;
                }
                Point screen = c.PointToScreen(new Point(e.X, e.Y));

                ShowContextMenu(screen);
                return;
            }
        }        

        private void ShowContextMenu(Point point)
        {
            ToolWindowSite.ShowContextMenu(AnkhCommandMenu.WorkingCopyExplorerContextMenu, point.X, point.Y);
        }

        public bool IsWcRootSelected()
        {
            return false;
        }

        public void RemoveRoot()
        {
            //
        }

        IFileStatusCache _cache;
        protected internal IFileStatusCache StatusCache
        {
            get { return _cache ?? (_cache = Context.GetService<IFileStatusCache>()); }
        }

        public void BrowsePath(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item = StatusCache[path];

            if(item == null)
                return;

            SvnWorkingCopy wc = item.WorkingCopy;

            string root;
            if (wc != null)
                root = wc.FullPath;
            else
                root = item.FullPath;

            

            FileSystemRootItem rt = CreateRoot(root);

            AddRoot(rt);
        }

        private FileSystemRootItem CreateRoot(string directory)
        {
            StatusCache.UpdateStatus(directory, SvnDepth.Infinity);
            SvnItem item = StatusCache[directory];
            FileSystemRootItem root = new FileSystemRootItem(this, item);
            return root;
        }

        internal void OpenItem(IAnkhServiceProvider context, string p)
        {
            throw new NotImplementedException();
        }
    }
}
