using System;
using EnvDTE;
using Utils;
using NSvn;
using NSvn.Core;
using NSvn.Common;
using Utils.Win32;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

using C = Utils.Win32.Constants;
using DteConstants = EnvDTE.Constants;
using Swf = System.Windows.Forms;


namespace Ankh
{
	/// <summary>
	/// Represents the Solution Explorer window in the VS.NET IDE
	/// </summary>
	internal class SolutionExplorer
	{
		public SolutionExplorer( _DTE dte )
		{
			this.dte = dte;
            this.resources = new Hashtable( new ItemHashCodeProvider(),
                new ItemComparer() );
            this.SetUpTreeview();
            this.SyncWithTreeView();
		}

        /// <summary>
        /// Gets the selected items in the Solution Explorer.
        /// </summary>
        /// <returns></returns>
        public ILocalResource[] GetSelectedItems()
        {
            ArrayList list = new ArrayList();
            foreach( SelectedItem item in this.dte.SelectedItems )
                list.Add( ((TreeNode)this.resources[item.ProjectItem]).Resource );

            return (ILocalResource[])list.ToArray( typeof(ILocalResource) );
        }

        /// <summary>
        /// Visits all the selected items.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitSelectedItems( ILocalResourceVisitor visitor )
        {
            foreach( SelectedItem item in this.dte.SelectedItems )
                ((TreeNode)this.resources[item.ProjectItem]).Resource.Accept( visitor );
        }

        /// <summary>
        /// Updates the status of selected items.
        /// </summary>
        public void UpdateSelectionStatus()
        {
            foreach( SelectedItem item in this.dte.SelectedItems )
               ((TreeNode)this.resources[item.ProjectItem]).UpdateStatus(); 
        }

        /// <summary>
        /// Updates the status of the given item.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStatus( ProjectItem item )
        {
            ((TreeNode)this.resources[item]).UpdateStatus();
        }

        /// <summary>
        /// Updates the resource associated with the given item 
        /// </summary>

        public void UpdateItem( ILocalResource oldResource, ILocalResource newResource )
        {
            // HACK: fix this
            ProjectItem item = null;
            foreach( DictionaryEntry entry in this.resources )
            {
                if (((TreeNode)entry.Value).Resource == oldResource)
                {
                    item = (ProjectItem)entry.Key;
                    break;
                }                       
            }
            ((TreeNode)this.resources[item]).Resource = newResource;
        }

        public void SyncWithTreeView()
        {
            this.resources.Clear();
            // find the root in the treeview
            IntPtr root = (IntPtr)Win32.SendMessage( this.treeview, Msg.TVM_GETNEXTITEM,
                C.TVGN_ROOT, IntPtr.Zero );
            

            // and the uihierarchy root
            UIHierarchy hierarchy = (UIHierarchy)this.dte.Windows.Item( 
                DteConstants.vsWindowKindSolutionExplorer ).Object;           

            // we assume there is a single root node
            this.root = new TreeNode( hierarchy.UIHierarchyItems.Item(1), root, this );
        }


        private void SetUpTreeview()
        {
            // TODO: error checking here
            // find the solution explorer window
            // Get the caption of the solution explorer
            string slnExplorerCaption = this.dte.Windows.Item(
                EnvDTE.Constants.vsWindowKindSolutionExplorer).Caption;
//            string vsnetCaption = this.dte.MainWindow.C
            IntPtr vsnet = (IntPtr)this.dte.MainWindow.HWnd;//Win32.FindWindow( VSNETWINDOW, null );
            IntPtr slnExplorer = Win32.FindWindowEx( vsnet, IntPtr.Zero, GENERICPANE, 
                slnExplorerCaption );
            IntPtr uiHierarchy = Win32.FindWindowEx( slnExplorer, IntPtr.Zero, 
                UIHIERARCHY, null );
            this.treeview = Win32.FindWindowEx( uiHierarchy, IntPtr.Zero, TREEVIEW, 
                null );         
 
            if ( this.treeview == IntPtr.Zero )
                throw new ApplicationException( 
                "Could not attach to solution explorer treeview" );

            // load the status images image strip
            Bitmap statusImages = (Bitmap)Image.FromStream( 
                this.GetType().Assembly.GetManifestResourceStream( STATUS_IMAGES ) );

            statusImages.MakeTransparent( statusImages.GetPixel(0,0) );

            this.statusImageList = new Swf.ImageList();
            this.statusImageList.ImageSize = new Size(7, 16);
            this.statusImageList.Images.AddStrip( statusImages );    
        
            // and assign it to the tree
            Win32.SendMessage( this.treeview, Msg.TVM_SETIMAGELIST, C.TVSIL_STATE,
                this.statusImageList.Handle );
        }

        

        private void AddResource( ProjectItem key, TreeNode node )
        {
            this.resources[key] = node;
        }

        #region class TreeNode
        /// <summary>
        /// Represents an item in the treeview.
        /// </summary>
        private class TreeNode
        {
            public TreeNode( UIHierarchyItem item, IntPtr hItem, 
                SolutionExplorer outer )
            {                
                this.hItem = hItem;
                this.outer = outer;
                
                this.FindChildren( item );   
           
                // is this a file or at least something thats likely
                // to be versioned?
                ProjectItem pitem = item.Object as ProjectItem;
                if ( pitem != null && pitem.FileCount > 0 )
                {
                    try
                    {
                        this.resource = SvnResource.FromLocalPath(
                            pitem.get_FileNames(1) );
                        if ( this.resource != null )
                        {
                            this.outer.AddResource( pitem, this );
                            
                            this.UpdateStatus();
                        }
                            
                    }
                
                    catch( NullReferenceException ex ) 
                    {
//                        Swf.MessageBox.Show( ex.GetType().ToString() + ": " + 
//                            ex.Message + Environment.NewLine + 
//                            ex.StackTrace + Environment.NewLine + 
//                            item.Name );
                    }
                }
                else
                    this.SetStatusImage( hItem, 0 );
            }

            static TreeNode()
            {
                statusMap[ StatusKind.Normal ]      = 1;
                statusMap[ StatusKind.Added ]       = 2;
                statusMap[ StatusKind.Deleted ]     = 3;
                statusMap[ StatusKind.Conflicted ]  = 6;
                statusMap[ StatusKind.Unversioned ] = 8;
                statusMap[ StatusKind.Modified ]    = 9;
            }
        

            /// <summary>
            /// Child nodes of this node
            /// </summary>
            public IList Children
            {
                get { return this.children;  }
            }

            /// <summary>
            /// Updates the status icon of this node.
            /// </summary>
            public void UpdateStatus(  )
            {
                if ( this.resource != null )
                {
                    StatusKind status = 
                        this.resource.Status.TextStatus;
                    int statusImage = 6;
                    if ( statusMap.Contains(status) )
                        statusImage = (int)statusMap[status];

                    this.SetStatusImage( this.hItem, statusImage );
                }
                else 
                    this.SetStatusImage( this.hItem, 0 );
            }

            /// <summary>
            /// The local resource associated with this item.
            /// </summary>
            public ILocalResource Resource
            {
                get{ return this.resource; }
                set
                {
                    this.resource = value; 
                    this.UpdateStatus();
                }
            }

            /// <summary>
            /// Finds the child nodes of this node.
            /// </summary>
            private void FindChildren( UIHierarchyItem item )
            {
                // retain the original expansion state
                bool isExpanded = item.UIHierarchyItems.Expanded;

                // get the treeview child
                IntPtr childItem = (IntPtr)Win32.SendMessage( this.outer.treeview, Msg.TVM_GETNEXTITEM,
                    C.TVGN_CHILD, this.hItem );

                // a node needs to be expanded at least once in order to have child nodes
                if ( childItem == IntPtr.Zero && item.UIHierarchyItems.Count > 0 )
                {
                    item.UIHierarchyItems.Expanded = true;
                    childItem = (IntPtr)Win32.SendMessage( this.outer.treeview, Msg.TVM_GETNEXTITEM,
                        C.TVGN_CHILD, this.hItem );
                }

                this.children = new ArrayList();
                foreach( UIHierarchyItem child in item.UIHierarchyItems )
                {
                    Debug.Assert( childItem != IntPtr.Zero, 
                        "Could not get treeview item" );
                    

                    this.children.Add( new TreeNode( child, childItem, this.outer ) );

                    // and the next child
                    childItem = (IntPtr)Win32.SendMessage( this.outer.treeview, Msg.TVM_GETNEXTITEM,
                        C.TVGN_NEXT, childItem );                    
                }

                item.UIHierarchyItems.Expanded = isExpanded;
            }

            private void SetStatusImage( IntPtr item, int status )
            {
                TVITEMEX tvitem = new TVITEMEX();
                tvitem.mask = C.TVIF_STATE | C.TVIF_HANDLE;
                tvitem.hItem = item;
                // bits 12-15 indicate the state image
                tvitem.state = (uint)(status << 12);
                tvitem.stateMask = C.TVIS_STATEIMAGEMASK;

                int retval = Win32.SendMessage( this.outer.treeview, Msg.TVM_SETITEM, IntPtr.Zero, 
                    tvitem ).ToInt32();
                Debug.Assert( Convert.ToBoolean( retval ), 
                    "Could not set treeview state image" );
                
            }

            private ILocalResource resource;
            private IntPtr hItem;
            private IList children;
            private SolutionExplorer outer;
            private static IDictionary statusMap = new Hashtable();
        }
        #endregion

        #region class ItemHashCodeProvider
        private class ItemHashCodeProvider : IHashCodeProvider
        {        
            #region Implementation of IHashCodeProvider
            public int GetHashCode(object obj)
            {
                return ((ProjectItem)obj).get_FileNames(1).GetHashCode();
            }
        
            #endregion
        }
        #endregion

        #region class ItemComparer
        private class ItemComparer : IComparer
        {        
            #region Implementation of IComparer
            public int Compare(object x, object y)
            {
                return ((ProjectItem)x).get_FileNames(0).CompareTo(
                    ((ProjectItem)y).get_FileNames(0) );
            }
        
            #endregion
        }
        #endregion


        private _DTE dte;
        private IntPtr treeview;
        private TreeNode root;
        private const string VSNETWINDOW = "wndclass_desked_gsk";
        private const string GENERICPANE = "GenericPane";
        private const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        private const string TREEVIEW = "SysTreeView32";
        private IDictionary resources;
        private Swf.ImageList statusImageList;

        private const string STATUS_IMAGES = "Ankh.status_icons.bmp";
	}
}
