using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;

namespace Ankh.UI
{
	/// <summary>
	/// This is a control that shows a treeview of a remote repository
	/// </summary>
	public class RepositoryExplorer : System.Windows.Forms.UserControl
	{       

		public RepositoryExplorer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

        /// <summary>
        /// The root folder of the repository to be explored by this control.
        /// </summary>
        public RepositoryDirectory RepositoryRoot
        {
            get{ return this.root; }
            set
            { 
                this.root = value;
                this.BuildSubTree( this.repositoryTree.Nodes, this.root );
            }
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.repositoryTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // repositoryTree
            // 
            this.repositoryTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repositoryTree.ImageIndex = -1;
            this.repositoryTree.Name = "repositoryTree";
            this.repositoryTree.SelectedImageIndex = -1;
            this.repositoryTree.Size = new System.Drawing.Size(150, 150);
            this.repositoryTree.Sorted = true;
            this.repositoryTree.TabIndex = 0;
            this.repositoryTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ExpandNode);
            // 
            // RepositoryExplorer
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.repositoryTree});
            this.Name = "RepositoryExplorer";
            this.ResumeLayout(false);

        }
		#endregion

        

        private void BuildSubTree( TreeNodeCollection nodes, RepositoryDirectory dir )
        {
            // we have set a new root, so get rid of any existing nodes
            nodes.Clear();

            if( this.root == null )
                return;

            ResourceVisitor visitor = new ResourceVisitor( nodes );

            // get the children of the root
            RepositoryResourceDictionary dict = dir.GetChildren();
            foreach( RepositoryResource res in dict.Values )
                res.Accept( visitor );
        }

        #region ResourceVisitor
        /// <summary>
        /// Inner class to traverse a list of RepositoryResource objects.
        /// </summary>
        private class ResourceVisitor : IRepositoryResourceVisitor
        {
            public ResourceVisitor( TreeNodeCollection nodes )
            {
                this.nodes = nodes; 
            }
            
            public void VisitFile( RepositoryFile file )
            {
                TreeNode node = new TreeNode( file.Name );
                node.Tag = file;
                this.nodes.Add( node );
            }

            public void VisitDirectory( RepositoryDirectory dir )
            {
                TreeNode dummy = new TreeNode( "" );
                dummy.Tag = DUMMY_NODE;

                TreeNode node = new TreeNode( dir.Name, 
                    new TreeNode[]{ dummy } );
                node.Tag = dir;
                this.nodes.Add( node );
            }

            private TreeNodeCollection nodes;

        }
        #endregion

        /// <summary>
        /// Event handler for the Expand event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandNode(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
                       
            // is this uninitialized?
            if ( e.Node.Nodes[0].Tag == DUMMY_NODE )
            {
                RepositoryDirectory dir = (RepositoryDirectory)e.Node.Tag;
                this.BuildSubTree( e.Node.Nodes, dir );
            }
        } 
        private System.Windows.Forms.TreeView repositoryTree;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private RepositoryDirectory root;
        private static readonly object DUMMY_NODE = new object();

        
	}
}
