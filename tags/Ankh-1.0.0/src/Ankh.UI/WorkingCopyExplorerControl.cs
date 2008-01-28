using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using Utils.Win32;
using System.ComponentModel;

namespace Ankh.UI
{
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
            if ( this.treeView.Focused )
            {
                this.selection = this.treeView.GetSelectedItems();
            }
            else if (this.listView.Focused )
            {
                this.selection = this.listView.GetSelectedItems();
            }

            // if none are focused, whatever selection was there before is probably still valid

            return this.selection;
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
            if ( e.Button == MouseButtons.Right && this.CustomContextMenu != null )
            {
                Control c = sender as Control;
                if ( c == null )
                {
                    return;
                }

                Point screen = c.PointToScreen( new Point(e.X, e.Y) );
                this.CustomContextMenu.Show( screen.X, screen.Y );
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
            this.splitter = new System.Windows.Forms.Splitter();
            this.topPanel = new System.Windows.Forms.Panel();
            this.addWorkingCopyLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.newRootTextBox = new System.Windows.Forms.TextBox();
            this.listView = new Ankh.UI.FileSystemDetailsView();
            this.treeView = new Ankh.UI.FileSystemTreeView();
            this.explorerPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // explorerPanel
            // 
            this.explorerPanel.Controls.Add( this.listView );
            this.explorerPanel.Controls.Add( this.splitter );
            this.explorerPanel.Controls.Add( this.treeView );
            this.explorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerPanel.Location = new System.Drawing.Point( 0, 29 );
            this.explorerPanel.Name = "explorerPanel";
            this.explorerPanel.Size = new System.Drawing.Size( 902, 604 );
            this.explorerPanel.TabIndex = 1;
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point( 207, 0 );
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size( 3, 604 );
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
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
            this.addWorkingCopyLabel.Size = new System.Drawing.Size( 96, 13 );
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
            this.newRootTextBox.Size = new System.Drawing.Size( 240, 21 );
            this.newRootTextBox.TabIndex = 0;
            this.newRootTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler( this.newRootTextBox_KeyDown );
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point( 210, 0 );
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size( 692, 604 );
            this.listView.TabIndex = 2;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point( 0, 0 );
            this.treeView.Name = "treeView";
            this.treeView.SelectedItem = null;
            this.treeView.Size = new System.Drawing.Size( 207, 604 );
            this.treeView.TabIndex = 0;
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
            this.ResumeLayout( false );

        }
        #endregion

        private Panel explorerPanel;
        private FileSystemTreeView treeView;
        private FileSystemDetailsView listView;
        private Splitter splitter;

        private IFileSystemItem[] selection = new IFileSystemItem[] { };
        private ArrayList roots;
        private Panel topPanel;
        private Label addWorkingCopyLabel;
        private Button addButton;
        private TextBox newRootTextBox;

        private IContextMenu customContextMenu;

        

       
    }
}
