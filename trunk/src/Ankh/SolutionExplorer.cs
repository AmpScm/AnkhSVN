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
		public SolutionExplorer( _DTE dte, SvnContext context )
		{
			this.dte = dte;
            this.context = context;
            this.projectItems = new Hashtable( new ItemHashCodeProvider(), 
                new ItemComparer() );
            this.projects = new Hashtable( new ProjectHashCodeProvider(), 
                new ProjectComparer() );
            this.solutionNode = null;
            this.SetUpTreeview();
            this.SyncWithTreeView();
		}

        /// <summary>
        /// Gets the selected items in the Solution Explorer.
        /// </summary>
        /// <returns></returns>
        public ILocalResource[] GetSelectedItems()
        {
//            ArrayList list = new ArrayList();
//            foreach( SelectedItem item in this.dte.SelectedItems )
//                list.Add( ((TreeNode)this.projectItems[item.ProjectItem]).Resource );
//
//            return (ILocalResource[])list.ToArray( typeof(ILocalResource) );
            return null;
        }

        /// <summary>
        /// Visits all the selected items.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitSelectedItems( ILocalResourceVisitor visitor )
        {
            foreach( SelectedItem item in this.dte.SelectedItems )
            {
                if ( item.ProjectItem != null && this.projectItems.Contains(item.ProjectItem) )
                    ((TreeNode)this.projectItems[item.ProjectItem]).VisitResources( visitor );
                else if ( item.Project != null && this.projects.Contains(item.Project) )
                    ((TreeNode)this.projects[item.Project]).VisitResources( visitor );                
            }
        }

        /// <summary>
        /// Updates the status of selected items.
        /// </summary>
        public void UpdateSelectionStatus()
        {
            //TODO: this can be done slightly faster, with only a single lookup
            foreach( SelectedItem item in this.dte.SelectedItems )
            {
                if ( item.ProjectItem != null && this.projectItems.Contains(item.ProjectItem) )
                    ((TreeNode)this.projectItems[item.ProjectItem]).UpdateStatus();
                else if ( item.Project != null && this.projects.Contains(item.Project) )
                    ((TreeNode)this.projects[item.Project]).UpdateStatus();    
            }
        }

        /// <summary>
        /// Updates the status of the given item.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStatus( ProjectItem item )
        {
            ((TreeNode)this.projectItems[item]).UpdateStatus();
        }

        /// <summary>
        /// Updates the resource associated with the given item 
        /// </summary>

        public void UpdateItem( ILocalResource oldResource, ILocalResource newResource )
        {
//            // HACK: fix this
//            ProjectItem item = null;
//            foreach( DictionaryEntry entry in this.projectItems )
//            {
//                if (((TreeNode)entry.Value).Resource == oldResource)
//                {
//                    item = (ProjectItem)entry.Key;
//                    break;
//                }                       
//            }
//            ((TreeNode)this.projectItems[item]).Resource = newResource;
        }

        public void SyncWithTreeView()
        {
            this.projectItems.Clear();
            this.projects.Clear();
            // find the root in the treeview
            IntPtr root = (IntPtr)Win32.SendMessage( this.treeview, Msg.TVM_GETNEXTITEM,
                C.TVGN_ROOT, IntPtr.Zero );
            

            // and the uihierarchy root
            UIHierarchy hierarchy = (UIHierarchy)this.dte.Windows.Item( 
                DteConstants.vsWindowKindSolutionExplorer ).Object;           

            // we assume there is a single root node
            this.root = TreeNode.CreateSolutionNode( 
                hierarchy.UIHierarchyItems.Item(1), root, this );
        }

        internal IntPtr TreeView
        {
            get{ return this.treeview; }
        }

        internal NSvnContext Context
        {
            get{ return this.context; }
        }

        internal _DTE DTE
        {
            get{ return this.dte; }
        }

        internal void SetUpTreeview()
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

        

        /// <summary>
        /// Adds a new resource to the tree.
        /// </summary>
        internal void AddResource( ProjectItem key, TreeNode node )
        {
            this.projectItems[key] = node;
        }

        internal void AddResource( Project key, TreeNode node )
        {
            this.projects[key] = node;
        }

        internal void AddResource( Solution key, TreeNode node )
        {
            // we assume theres only one of these
            this.solutionNode = node;
        }


        #region class TreeNode
        
        #endregion

        #region class ItemHashCodeProvider
        private class ItemHashCodeProvider : IHashCodeProvider
        {        
            public int GetHashCode(object obj)
            {
                return ((ProjectItem)obj).get_FileNames(1).GetHashCode();
            }
        }
        #endregion

        #region class ItemComparer
        private class ItemComparer : IComparer
        {        
            public int Compare(object x, object y)
            {
                return ((ProjectItem)x).get_FileNames(0).CompareTo(
                    ((ProjectItem)y).get_FileNames(0) );
            }
        }
        #endregion
        #region class ProjectHashCodeProvider
        private class ProjectHashCodeProvider : IHashCodeProvider
        {        
            public int GetHashCode(object obj)
            {
                return ((Project)obj).FullName.GetHashCode();
            }
        }
        #endregion

        #region class ProjectComparer
        private class ProjectComparer : IComparer
        {        
            public int Compare(object x, object y)
            {
                return ((Project)x).FullName.CompareTo(
                    ((Project)y).FullName );
            }
        }
        #endregion


        private _DTE dte;
        private IntPtr treeview;
        private TreeNode root;
        private const string VSNETWINDOW = "wndclass_desked_gsk";
        private const string GENERICPANE = "GenericPane";
        private const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        private const string TREEVIEW = "SysTreeView32";
        private IDictionary projectItems;
        private IDictionary projects;
        private TreeNode solutionNode;
        private Swf.ImageList statusImageList;
        private SvnContext context;

        private const string STATUS_IMAGES = "Ankh.status_icons.bmp";
	}
}
