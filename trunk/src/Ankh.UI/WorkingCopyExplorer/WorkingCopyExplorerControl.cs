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

namespace Ankh.UI
{
    public interface IWorkingCopyExplorerSubControl
    {
        Point GetSelectionPoint();
        IFileSystemItem[] GetSelectedItems();
    }

    public partial class WorkingCopyExplorerControl : UserControl
    {
        public event CancelEventHandler ValidatingNewRoot;

        public event EventHandler WantNewRoot;

        public WorkingCopyExplorerControl()
        {
            this.InitializeComponent();

            this.treeView.SelectedItemChanged += new EventHandler( treeView_SelectedItemChanged );
            this.listView.CurrentDirectoryChanged += new EventHandler( listView_CurrentDirectoryChanged );

            this.treeView.MouseDown += new MouseEventHandler( HandleMouseDown );
            this.listView.MouseDown += new MouseEventHandler( HandleMouseDown );

            this.newRootTextBox.TextChanged += new EventHandler( newRootTextBox_TextChanged );
        }

        public string NewRootPath
        {
            get { return this.newRootTextBox.Text; }
        }


        public ImageList StateImages
        {
            get { return this.listView.StateImageList; }
            set { this.listView.StateImageList = value; }
        }


        
     
        public void AddRoot( IFileSystemItem root )
        {
            this.treeView.AddRoot( root );
        }

        public void RemoveRoot( IFileSystemItem root )
        {
            this.treeView.RemoveRoot( root );
        }

        public void RefreshItem( IFileSystemItem item )
        {
            throw new System.NotImplementedException();
        }

        public IFileSystemItem[] GetSelectedItems()
        {
            if ( this.SelectedControl != null )
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
                if ( this.treeView.Focused )
                {
                    return this.treeView;
                }
                else if ( this.listView.Focused )
                {
                    return this.listView;
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
        protected override bool ProcessCmdKey( ref System.Windows.Forms.Message msg, Keys keyData )
        {
            if ( keyData == (Keys)( Keys.F10 | Keys.Shift ) || keyData == Keys.Apps )
            {
                if ( this.SelectedControl != null )
                {
                    Point point = this.SelectedControl.GetSelectionPoint();
                    this.ShowContextMenu( point );
                    return true;
                }
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }

        void newRootTextBox_TextChanged( object sender, EventArgs e )
        {
            if ( this.ValidatingNewRoot != null )
            {
                CancelEventArgs args = new CancelEventArgs( true );
                this.ValidatingNewRoot( this, args );

                this.addButton.Enabled = ! args.Cancel;
            }
        }

        void treeView_SelectedItemChanged( object sender, EventArgs e )
        {
            IFileSystemItem item = this.treeView.SelectedItem;
            this.listView.SetDirectory( item );
        }

        void listView_CurrentDirectoryChanged( object sender, EventArgs e )
        {
            this.treeView.SelectedItemChanged -= new EventHandler( this.treeView_SelectedItemChanged );
            try
            {
                this.treeView.SelectedItem = this.listView.CurrentDirectory;
            }
            finally
            {
                this.treeView.SelectedItemChanged += new EventHandler( this.treeView_SelectedItemChanged );
            }
        }

        void HandleMouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Right )
            {
                Control c = sender as Control;
                if ( c == null )
                {
                    return;
                }
                Point screen = c.PointToScreen( new Point( e.X, e.Y ) );

                ShowContextMenu( screen );
                return;
            }
        }

        IAnkhUISite _uiSite;
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;
                if (value is IAnkhUISite)
                {
                    _uiSite = value as IAnkhUISite;
                    OnUISiteChanged(EventArgs.Empty);
                }
            }
        }

        private void OnUISiteChanged(EventArgs eventArgs)
        {
            treeView.SelectionPublishServiceProvider = UISite;
            listView.SelectionPublishServiceProvider = UISite;
            treeView.RetrieveSelection += new EventHandler<Ankh.UI.VSSelectionControls.TreeViewWithSelection<TreeNode>.RetrieveSelectionEventArgs>(treeView_RetrieveSelection);
            listView.RetrieveSelection += new EventHandler<Ankh.UI.VSSelectionControls.ListViewWithSelection<ListViewItem>.RetrieveSelectionEventArgs>(listView_RetrieveSelection);
            treeView.Context = UISite;
            listView.Context = UISite;
        }

        void listView_RetrieveSelection(object sender, Ankh.UI.VSSelectionControls.ListViewWithSelection<ListViewItem>.RetrieveSelectionEventArgs e)
        {
            IFileSystemItem ii = e.Item.Tag as IFileSystemItem;

            if (ii == null)
            {
                e.SelectionItem = null;
                return;
            }

            SvnItem item = ii.SvnItem;

            if (item != null)
                e.SelectionItem = new SvnItemData(UISite, item);
            else
                e.SelectionItem = null;
        }

        void treeView_RetrieveSelection(object sender, Ankh.UI.VSSelectionControls.TreeViewWithSelection<TreeNode>.RetrieveSelectionEventArgs e)
        {
            IFileSystemItem ii = e.Item.Tag as IFileSystemItem;

            if (ii == null)
            {
                e.SelectionItem = null;
                return;
            }

            SvnItem item = ii.SvnItem;

            if (item != null)
                e.SelectionItem = new SvnItemData(UISite, item);
            else
                e.SelectionItem = null;
        }

        [CLSCompliant(false)]
        protected IAnkhUISite UISite
        {
            get { return _uiSite; }
        }

        private void ShowContextMenu( Point point)
        {
            _uiSite.ShowContextMenu(AnkhCommandMenu.WorkingCopyExplorerContextMenu, point.X, point.Y);
        }

        private void newRootTextBox_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter && this.addButton.Enabled )
            {
                this.AddNewRoot();
            }
        }

        private void addButton_Click( object sender, EventArgs e )
        {
            AddNewRoot();
        }

        private void AddNewRoot()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                if ( this.WantNewRoot != null )
                {
                    this.WantNewRoot( this, EventArgs.Empty );
                }

                this.newRootTextBox.Text = "";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }        
    }
}
