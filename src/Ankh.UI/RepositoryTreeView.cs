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
    public class RepositoryTreeView : PathTreeView
    {
        public RepositoryTreeView()
        {
        }   
     
        public void AddRoot( IRepositoryTreeNode node, string url )
        {
            TreeNode root = new TreeNode( url, this.OpenFolderIndex, this.OpenFolderIndex );

            root.Tag = node;
            node.Tag = root;          

            this.Nodes.Add( root );

            TreeNode dummy = new TreeNode("");
            dummy.Tag = DUMMY_NODE;

            root.Nodes.Add( dummy );
        }
        
        /// <summary>
        /// Refresh the contents of this node.
        /// </summary>
        /// <param name="n"></param>
        public void RefreshNode( IRepositoryTreeNode n )
        {
            // get rid of the subnodes
            TreeNode node = (TreeNode)n.Tag;
            node.Nodes.Clear();

            // now add the dummy child.
            TreeNode dummy = new TreeNode();
            dummy.Tag = DUMMY_NODE;
            node.Nodes.Add( dummy );

            // make sure it gets refilled.
            node.Collapse();
            node.Expand();
        }

        public void AddChildren( IRepositoryTreeNode parent, IList childNodes )
        {
            TreeNode parentNode = (TreeNode)parent.Tag;
            this.BuildSubTree( parentNode.Nodes, childNodes );
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
                        newNode.ImageIndex = this.ClosedFolderIndex;
                        newNode.SelectedImageIndex = this.ClosedFolderIndex;
                    }
                    else
                    {
                        newNode = new TreeNode( child.Name);

                        // set the icon
                        this.SetIcon( newNode, child.Name );
                    }

                    newNode.Tag = child;
                    child.Tag = newNode;
                    nodes.Add( newNode );

                } // foreach

            }
            catch( ApplicationException )
            {
                this.Nodes.Clear();
                this.Nodes.Add( new TreeNode( "An error occurred",  
                    this.OpenFolderIndex, this.OpenFolderIndex ) );
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
    }

    
}
