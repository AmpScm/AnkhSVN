// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Ankh.UI
{
	/// <summary>
	/// Represents a TreeView that can be used to choose from a set of paths.
	/// </summary>
	public class PathSelectionTreeView : PathTreeView
	{
        /// <summary>
        /// Fired when the treeview needs information about a path.
        /// </summary>
        public event GetPathInfoDelegate GetPathInfo;

		public PathSelectionTreeView()
		{
			this.CheckBoxes = true;
            this.SingleCheck = false;
            this.Recursive = false;
            this.paths = new object[]{};
		}

        public IList Paths
        {
            get{ return this.paths; }
            set
            {
                this.paths = value;
                this.BuildTree();
            }
        }

        /// <summary>
        /// Whether the paths used are URLs.
        /// </summary>
        public bool UrlPaths
        {
            get{ 
                return this.PathSeparator == "/"; }
            set{ this.PathSeparator = value ? "/" : "\\"; }
        }

        /// <summary>
        /// Whether there should be only one single check. Default is false.
        /// </summary>
        public bool SingleCheck
        {
            get{ return this.singleCheck; }
            set{ this.singleCheck = value; }
        }

        /// <summary>
        /// Whether a check is supposed to be perceived as recursive.
        /// </summary>
        public bool Recursive
        {
            get{ return this.recursive; }
            set
            { 
                this.recursive = value;
                this.UncheckChildren( this.Nodes, value );
            }
        }

        public IList CheckedItems
        {
            get
            { 
                IList list = new ArrayList();
                return this.GetCheckedItems( this.Nodes, list ); 
            }
            
            set
            { 
                this.SetCheckedItems( this.Nodes, value ); 
            }
        }

        /// <summary>
        /// Keep track of the last checked node and make sure only
        /// one node is checked at one time if singleCheck is true.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck (e);
            
            // unchecking?
            if ( !e.Node.Checked )
            {
                this.checkedNode = null;
                // reenable children?
                if ( this.Recursive )
                    this.ToggleChildren( e.Node, true );
                return;
            }

            // make sure only one node is checked if SingleCheck is true
            if ( this.SingleCheck && this.checkedNode != null )
            {
                this.checkedNode.Checked = false;
            }

            // keep track of the last checked node
            this.checkedNode = e.Node;

            // disable children?
            if ( this.Recursive )
                this.ToggleChildren( e.Node, false );


        }

        /// <summary>
        /// Make sure you can't check disabled nodes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            base.OnBeforeCheck (e);
            e.Cancel = e.Node.ForeColor == DisabledColor;
        }

        protected void OnGetPathInfo( TreeNode node )
        {
            if ( this.GetPathInfo != null )
            {
                GetPathInfoEventArgs args = new GetPathInfoEventArgs( node.Tag );
                this.GetPathInfo( this, args );
                if ( args.IsDirectory )
                {
                    node.SelectedImageIndex = this.ClosedFolderIndex;
                    node.ImageIndex = this.ClosedFolderIndex;
                }
                else
                    this.SetIcon( node, args.Path );
            }
        }

       

        /// <summary>
        /// Retrieves a list of the checked items.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList GetCheckedItems( TreeNodeCollection nodes, IList list )
        {
            foreach( TreeNode node in nodes )
            {
                if ( node.Checked )
                    list.Add( node.Tag );
                this.GetCheckedItems( node.Nodes, list );
            }

            return list;
        }

        /// <summary>
        /// Sets the checked items.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="items"></param>
        private void SetCheckedItems( TreeNodeCollection nodes, IList items )
        {
            foreach( TreeNode node in nodes )
            {
                if ( items.Contains( node.Tag ) )
                    node.Checked = true;
                else
                    node.Checked = false;

                this.SetCheckedItems( node.Nodes, items );
            }            
        }


        /// <summary>
        /// Recursively uncheck and disable the children of checked nodes
        /// </summary>
        /// <param name="nodes"></param>
        private void UncheckChildren( TreeNodeCollection nodes, bool recursive )
        {
            foreach( TreeNode node in nodes )
            {
                if ( node.Checked )
                    this.ToggleChildren( node, !recursive );
                else
                    this.UncheckChildren( node.Nodes, recursive );
            }
        }

        /// <summary>
        /// Enables or disables the children of a specific node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="enabled"></param>
        private void ToggleChildren( TreeNode parent, bool enabled )
        {
            this.BeginUpdate();
            this.DoToggleChildren( parent, enabled );
            this.EndUpdate();            
        }

        /// <summary>
        /// Helper for above.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="enabled"></param>
        private void DoToggleChildren( TreeNode parent, bool enabled )
        {
            foreach( TreeNode node in parent.Nodes )
            {
                node.Checked = enabled ? node.Checked : false;
                node.ForeColor = enabled ? EnabledColor : DisabledColor;

                this.ToggleChildren( node, enabled );
            }
        }


        /// <summary>
        /// Builds the treeview from the objects in this.paths
        /// </summary>
        private void BuildTree()
        {
            this.BeginUpdate();
            this.Nodes.Clear();

            if ( this.paths.Count == 0 )
                return;            

            foreach( object item in this.paths )
                this.AddNode( item );

            this.ExpandAll();
            this.EndUpdate();
        }

        /// <summary>
        /// Adds a node to the right place in the tree.
        /// </summary>
        /// <param name="nodeName"></param>
        private void AddNode( object item )
        {
            TreeNodeCollection nodes = this.Nodes;

            string nodeName = item.ToString();

            // get rid of any trailing path separators
            if ( nodeName[nodeName.Length-1] == this.PathSeparator[0] )
                nodeName = nodeName.Substring(0, nodeName.Length-1);

            string[] components = nodeName.Split( this.PathSeparator[0]);

            TreeNode node = null;
            foreach( string component in components )
            {
                node = this.GetNode( nodes, component );
                nodes = node.Nodes;
            }

            // leaf nodes should be black and enabled
            if ( node != null )
            {
                node.ForeColor = EnabledColor;
                node.Tag = item;
                this.OnGetPathInfo( node );
            }
        }

        private TreeNode GetNode( TreeNodeCollection nodes, string pathComponent )
        {
            foreach( TreeNode node in nodes )
            {
                if ( node.Text == pathComponent )
                    return node;
            }

            TreeNode newNode = nodes.Add( pathComponent );

            // non-leaf nodes default to gray and are disabled
            newNode.ForeColor = DisabledColor;
            return newNode;
        }

        private IList paths;
        private TreeNode checkedNode;
        private bool singleCheck;
        private bool recursive;

        private static readonly Color EnabledColor = Color.Black;
        private static readonly Color DisabledColor = Color.Gray;

       
        public static void Main()
        {
            Form form = new Form();
            PathSelectionTreeView tree = new PathSelectionTreeView();
            form.Controls.Add( tree );
            tree.Dock = DockStyle.Fill;
            tree.UrlPaths = true;

            tree.Paths = new string[]{ 
                                         "trunk/src/Ankh/Commands/AddItemCommand.cs",
                                         "trunk/src/Ankh/Commands/CheckoutFolderCommand.cs",
                                         "trunk/src/Ankh/Commands/CheckoutSolutionCommand.cs",
                                         "trunk/src/Ankh/Commands/CleanupCommand.cs",
                                         "trunk/src/Ankh/Commands/CommandBase.cs",
                                         "trunk/src/Ankh/Commands/CommitItemCommand.cs",
                                         "trunk/src/Ankh/Commands/CopyReposExplorerUrl.cs",
                                         "trunk/src/Ankh/Commands/LocalDiffCommandBase.cs",
                                         "trunk/src/Ankh/Commands/RefreshCommand.cs",
                                         "trunk/src/Ankh/Commands/RenameFileCommand.cs",
                                         "trunk/src/Ankh/Commands/ResolveConflictCommand.cs",
                                         "trunk/src/Ankh/Commands/RevertItemCommand.cs",
                                         "trunk/src/Ankh/Commands/RunSvnCommand.cs",
                                         "trunk/src/Ankh/Commands/ShowRepositoryExplorerCommand.cs",
                                         "trunk/src/Ankh/Commands/ToggleAnkhCommand.cs",
                                         "trunk/src/Ankh/Commands/UpdateItemCommand.cs",
                                         "trunk/src/Ankh/Commands/ViewRepositoryFileCommand.cs"
                                     };
            Application.Run( form );

        }
	}

    public class GetPathInfoEventArgs
    {
        public GetPathInfoEventArgs( object item )
        {
            this.item = item;
        }

        public object Item
        {
            get{ return this.item; }
        }

        public bool IsDirectory
        {
            get{ return this.isDirectory; }
            set{ this.isDirectory = value; }
        }

        public string Path
        {
            get{ return this.path; }
            set{ this.path = value; }
        }
        private bool isDirectory = false;
        private string path = "";
        private object item;
    }

    public delegate void GetPathInfoDelegate( 
        object sender, GetPathInfoEventArgs args );

   
}
