using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Utils.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    public interface IWorkingCopyExplorerSubControl
    {
        Point GetSelectionPoint();
        IFileSystemItem[] GetSelectedItems();
    }

    [ComVisible(true)]
    public class WorkingCopyExplorerControl : UserControl
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

            Win32.SHAutoComplete( this.newRootTextBox.Handle, Shacf.FileSysDirs );


        }

        

        public IContextMenu CustomContextMenu
        {
            get
            {
                return this.customContextMenu;
            }
            set
            {
                this.customContextMenu = value;
            }
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


        private void ShowContextMenu( Point point)
        {
            if ( this.CustomContextMenu != null && point != Point.Empty )
            {
                this.CustomContextMenu.Show( point.X, point.Y );
            }
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

        #region InitializeComponent
        private void InitializeComponent()
        {
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.listView = new Ankh.UI.FileSystemDetailsView();
            this.treeView = new Ankh.UI.FileSystemTreeView();
            this.topPanel = new System.Windows.Forms.Panel();
            this.addWorkingCopyLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.newRootTextBox = new System.Windows.Forms.TextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.explorerPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // explorerPanel
            // 
            this.explorerPanel.Controls.Add( this.splitContainer );
            this.explorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerPanel.Location = new System.Drawing.Point( 0, 29 );
            this.explorerPanel.Name = "explorerPanel";
            this.explorerPanel.Size = new System.Drawing.Size( 902, 604 );
            this.explorerPanel.TabIndex = 1;
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point( 0, 0 );
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size( 598, 604 );
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point( 0, 0 );
            this.treeView.Name = "treeView";
            this.treeView.SelectedItem = null;
            this.treeView.Size = new System.Drawing.Size( 300, 604 );
            this.treeView.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add( this.addWorkingCopyLabel );
            this.topPanel.Controls.Add( this.addButton );
            this.topPanel.Controls.Add( this.newRootTextBox );
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point( 0, 0 );
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size( 902, 29 );
            this.topPanel.TabIndex = 0;
            // 
            // addWorkingCopyLabel
            // 
            this.addWorkingCopyLabel.AutoSize = true;
            this.addWorkingCopyLabel.Location = new System.Drawing.Point( 3, 8 );
            this.addWorkingCopyLabel.Name = "addWorkingCopyLabel";
            this.addWorkingCopyLabel.Size = new System.Drawing.Size( 95, 13 );
            this.addWorkingCopyLabel.TabIndex = 2;
            this.addWorkingCopyLabel.Text = "Add working copy:";
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point( 351, 2 );
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size( 65, 23 );
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.Click += new System.EventHandler( this.addButton_Click );
            // 
            // newRootTextBox
            // 
            this.newRootTextBox.Location = new System.Drawing.Point( 105, 5 );
            this.newRootTextBox.Name = "newRootTextBox";
            this.newRootTextBox.Size = new System.Drawing.Size( 240, 20 );
            this.newRootTextBox.TabIndex = 0;
            this.newRootTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler( this.newRootTextBox_KeyDown );
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point( 0, 0 );
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add( this.treeView );
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add( this.listView );
            this.splitContainer.Size = new System.Drawing.Size( 902, 604 );
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.TabIndex = 0;
            // 
            // WorkingCopyExplorerControl
            // 
            this.Controls.Add( this.explorerPanel );
            this.Controls.Add( this.topPanel );
            this.Name = "WorkingCopyExplorerControl";
            this.Size = new System.Drawing.Size( 902, 633 );
            this.explorerPanel.ResumeLayout( false );
            this.topPanel.ResumeLayout( false );
            this.topPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout( false );
            this.splitContainer.Panel2.ResumeLayout( false );
            this.splitContainer.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        private Panel explorerPanel;
        private FileSystemTreeView treeView;
        private FileSystemDetailsView listView;

        private IFileSystemItem[] selection = new IFileSystemItem[] { };
        private Panel topPanel;
        private Label addWorkingCopyLabel;
        private Button addButton;
        private TextBox newRootTextBox;
        private SplitContainer splitContainer;

        private IContextMenu customContextMenu;
    }
}
