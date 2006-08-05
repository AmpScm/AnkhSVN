using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Ankh.UI
{
    public class WorkingCopyExplorerControl : UserControl
    {
        public WorkingCopyExplorerControl()
        {
            this.InitializeComponent();

            this.treeView.SelectedItemChanged += new EventHandler( treeView_SelectedItemChanged );
            this.listView.CurrentDirectoryChanged += new EventHandler( listView_CurrentDirectoryChanged );

            this.treeView.MouseDown += new MouseEventHandler( HandleMouseDown );
            this.listView.MouseDown += new MouseEventHandler( HandleMouseDown );
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
            throw new System.NotImplementedException();
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

        #region InitializeComponent
        private void InitializeComponent()
        {
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.listView = new Ankh.UI.FileSystemDetailsView();
            this.splitter = new System.Windows.Forms.Splitter();
            this.treeView = new Ankh.UI.FileSystemTreeView();
            this.topPanel = new System.Windows.Forms.Panel();
            this.explorerPanel.SuspendLayout();
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
            this.explorerPanel.TabIndex = 0;
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Location = new System.Drawing.Point( 210, 0 );
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size( 692, 604 );
            this.listView.TabIndex = 2;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point( 207, 0 );
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size( 3, 604 );
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Location = new System.Drawing.Point( 0, 0 );
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size( 207, 604 );
            this.treeView.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point( 0, 0 );
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size( 902, 29 );
            this.topPanel.TabIndex = 1;
            // 
            // WorkingCopyExplorerControl
            // 
            this.Controls.Add( this.explorerPanel );
            this.Controls.Add( this.topPanel );
            this.Name = "WorkingCopyExplorerControl";
            this.Size = new System.Drawing.Size( 902, 633 );
            this.explorerPanel.ResumeLayout( false );
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

        private IContextMenu customContextMenu;

        static void Main( )
        {
            WorkingCopyExplorerControl wce = new WorkingCopyExplorerControl();
            wce.AddRoot( new FileSystemDirectoryMock( @"D:\tmp" ) );

            using ( Form form = new Form() )
            {
                form.Size = new System.Drawing.Size( 1100, 670 );
                form.Controls.Add( wce );
                wce.Dock = DockStyle.Fill;
                Application.Run( form );
            }
        }

        private abstract class FileSystemMockBase
        {

            [TextProperty( "Modified", Order=0 )]
            public DateTime Modified
            {
                get { return this.Info.LastWriteTime; }
            }

            [TextProperty("Accessed", Order=1)]
            public DateTime Accessed
            {
                get { return this.Info.LastAccessTime; }
                set { this.Info.LastAccessTime = value; }
            }
	

            protected abstract FileSystemInfo Info { get; }

        }

        private class FileSystemFileMock : FileSystemMockBase, IFileSystemItem
        {

            public FileSystemFileMock( string path )
            {
                if ( File.Exists(path) )
                {
                    this.info = new FileInfo( path );
                }
                else
                {
                    throw new ArgumentException( "Path must refer to existing file", "path" );
                }
            }
            #region IFileSystemItem Members

            public event EventHandler Changed;

            public bool IsContainer
            {
                get
                {
                    return false;
                }
            }

            public string Text
            {
                get
                {
                    return this.info.Name;
                }
            }

            public IFileSystemItem[] GetChildren()
            {
                throw new InvalidOperationException("Cannot get children from file node");
            }

            #endregion

            protected override FileSystemInfo Info
            {
                get { return this.info; }
            }

            private FileInfo info;

            #region IFileSystemItem Members


            public void Open()
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            #endregion
        }

        private class FileSystemDirectoryMock : FileSystemMockBase, IFileSystemItem
        {
            public FileSystemDirectoryMock( string path )
            {
                if ( Directory.Exists( path ) )
                {
                    this.info = new DirectoryInfo( path );
                }
                else
                {
                    throw new ArgumentException( "path must refer to existing directory", path );
                }
            }
            #region IFileSystemItem Members

            public event EventHandler Changed;

            public bool IsContainer
            {
                get
                {
                    return true;
                }
            }

            public string Text
            {
                get
                {
                    return this.info.Name;
                }
            }

            public IFileSystemItem[] GetChildren()
            {
                ArrayList children = new ArrayList();
                foreach( DirectoryInfo child in this.info.GetDirectories())
                {
                    children.Add(new FileSystemDirectoryMock(child.FullName));
                }
                foreach ( FileInfo child in this.info.GetFiles() )
                {
                    children.Add( new FileSystemFileMock( child.FullName ) );
                }

                return ((IFileSystemItem[])children.ToArray(typeof(IFileSystemItem)));
            }

            #endregion

            protected override FileSystemInfo Info
            {
                get { return this.info; }
            }

            private DirectoryInfo info;

            #region IFileSystemItem Members


            public void Open()
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            #endregion
        }
    }
}
