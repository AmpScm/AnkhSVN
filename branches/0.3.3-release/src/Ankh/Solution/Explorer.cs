// $Id$
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
using System.IO;

using C = Utils.Win32.Constants;
using DteConstants = EnvDTE.Constants;
using Swf = System.Windows.Forms;


namespace Ankh.Solution
{
    /// <summary>
    /// Represents the Solution Explorer window in the VS.NET IDE
    /// </summary>
    internal class Explorer
    {
        public Explorer( _DTE dte, SvnContext context )
        {
            this.dte = dte;
            this.context = context;
            this.projectItems = new Hashtable( new ItemHashCodeProvider(), 
                new ItemComparer() );
            this.projects = new Hashtable( new ProjectHashCodeProvider(), 
                new ProjectComparer() );
            
            // get the uihierarchy root
            this.uiHierarchy = (UIHierarchy)this.dte.Windows.Item( 
                DteConstants.vsWindowKindSolutionExplorer ).Object; 
            this.solutionNode = null;            
        }

        /// <summary>
        ///  To be called when a solution is loaded.
        /// </summary>
        public void Load()
        {
            this.Unload();
            this.SetUpTreeview();
            this.SyncWithTreeView();
        }

        /// <summary>
        ///  Resets the state of the object. To be called when a solution is unloaded.
        /// </summary>
        public void Unload()
        {
            this.projectItems.Clear();
            this.projects.Clear();
            this.solutionNode = null;
        }

        

        /// <summary>
        /// Visits all the selected items.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitSelectedItems( ILocalResourceVisitor visitor, bool recursive )
        {
            //foreach( SelectedItem item in items )
            object o = this.uiHierarchy.SelectedItems;
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )
            {
                TreeNode node = this.GetNode( item );
                if ( node != null )
                    node.VisitResources( visitor, recursive );
            }
        }

        /// <summary>
        /// Visits all the selected nodes.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitSelectedNodes( INodeVisitor visitor )
        {
            //foreach( SelectedItem item in items )
            object o = this.uiHierarchy.SelectedItems;
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )
            {
                TreeNode node = this.GetNode( item );
                if ( node != null )
                    node.Accept( visitor );
            }
        }

        public void VisitResources( ProjectItem item, ILocalResourceVisitor visitor,
            bool recursive )
        {
            TreeNode node = this.GetNode( item );
            if ( node != null )
                node.VisitResources( visitor, recursive );
        }

        public void VisitResources( Project project, ILocalResourceVisitor visitor, 
            bool recursive )
        {
            TreeNode node = this.GetNode( project );
            if ( node != null )
                node.VisitResources( visitor, recursive );
        }

        /// <summary>
        /// Updates the status of selected items.
        /// </summary>
        public void UpdateSelectionStatus()
        {
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )
            {
                TreeNode node = this.GetNode( item );
                if ( node != null )
                    node.UpdateStatus();
            }
        }

        

        /// <summary>
        /// Refreshes the parents of the selected items.
        /// </summary>
        public void RefreshSelectionParents()
        {
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )
            {
                TreeNode node = this.GetNode( item );
                if ( node != null )
                {
                    if ( node == this.solutionNode )
                        node.Refresh();
                    else
                        node.Parent.Refresh();
                }
            }
        }

        /// <summary>
        /// Refreshes all subnodes of a specific project.
        /// </summary>
        /// <param name="project"></param>
        public void Refresh( Project project )
        {
            TreeNode node = this.GetNode( project );
            if ( node != null )
                node.Refresh();
        }

        /// <summary>
        /// Refreshes the current selection.
        /// </summary>
        public void RefreshSelection()
        {
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )
            {
                TreeNode node = this.GetNode( item );
                // special care for the solution
                if ( node == solutionNode )
                    this.SyncWithTreeView();
                else if ( node != null )
                    node.Refresh();
            }
        }


        /// <summary>
        /// Updates the status of the given item.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStatus( ProjectItem item )
        {
            TreeNode node = (TreeNode)this.projectItems[item];
            if ( node != null )
                node.UpdateStatus();
        }

        /// <summary>
        /// Checks if the status of an item is cached.
        /// </summary>
        /// <param name="path">The path to check status for</param>
        /// <returns>A status object, or null if its not cached</returns>
        public Status GetCachedStatus( string path )
        {
            Debug.WriteLine( "Checking for cached status for " + path, "Ankh" );
            if ( this.statusCache != null ) 
            {
                Status status; 

                if ( (status = this.statusCache[ path ]) != null )
                {
                    Debug.WriteLine( "Found cached status for " + path, "Ankh" );
                    return status;
                }
                else
                    return null;
            }
            else
                return null;
        }

 

        public void SyncWithTreeView()
        {
            // no point in doing anything if the solution dir isn't a wc
            string solutionPath = this.dte.Solution.FullName;
            if ( solutionPath == String.Empty || 
                !SvnUtils.IsWorkingCopyPath( Path.GetDirectoryName( solutionPath ) ) )
                return;

            this.projectItems.Clear();
            this.projects.Clear();
            // find the root in the treeview
            IntPtr root = (IntPtr)Win32.SendMessage( this.treeview, Msg.TVM_GETNEXTITEM,
                C.TVGN_ROOT, IntPtr.Zero );
            
            // generate a status cache
            this.GenerateStatusCache( this.dte.Solution.FullName );
            
            this.solutionItem = this.uiHierarchy.UIHierarchyItems.Item(1);

            // we assume there is a single root node
            this.root = TreeNode.CreateSolutionNode( 
                this.solutionItem, root, this );

            // we don't want to maintain the cache after initial load.
            this.statusCache = null;

            //this.Hook();
        }

        internal IntPtr TreeView
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.treeview; }
        }

        internal NSvnContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }
        }

        internal _DTE DTE
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.dte; }
        }

        internal void SetUpTreeview()
        {
            Window solutionExplorerWindow = this.dte.Windows.Item(
                EnvDTE.Constants.vsWindowKindSolutionExplorer);

            // we need to make sure its not hidden
            bool hidden = solutionExplorerWindow.AutoHides;
            solutionExplorerWindow.AutoHides = false;
            
            // find the solution explorer window
            // Get the caption of the solution explorer            
            string slnExplorerCaption = solutionExplorerWindow.Caption;
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

            // reset back to the original hiding-state(!?)
            solutionExplorerWindow.AutoHides = hidden;

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

        internal void SetSolution( TreeNode node )
        {
            // we assume theres only one of these
            this.solutionNode = node;
        }

        private void GenerateStatusCache( string solutionPath )
        {
            DebugTimer t = DebugTimer.Start();
            string solutionDir = Path.GetDirectoryName( solutionPath );

            int youngest;
            Debug.WriteLine( "Getting status cache", "Ankh" );
            
            this.statusCache = new StatusCache();
            Client.Status( out youngest, solutionDir, Revision.Unspecified, 
                new StatusCallback(this.statusCache.StatusFunc), 
                true, true, false, true, new ClientContext() );

            t.End( "Got status cache", "Ankh" );
        }

        private TreeNode GetNode(UIHierarchyItem item)
        {
            if ( item.Object == null )
                return null;

            if ( item.Object is ProjectItem && this.projectItems.Contains( item.Object ) )
                return ((TreeNode)this.projectItems[item.Object]);
            else if ( item.Object is Project && this.projects.Contains(item.Object) )
                return ((TreeNode)this.projects[item.Object]); 
            else if ( item == this.solutionItem )
                return this.solutionNode;
            else
                return null;
        }
        
        private TreeNode GetNode( ProjectItem item )
        {
            if ( item == null )
                return null;

            if (this.projectItems.Contains( item ) )
                return ((TreeNode)this.projectItems[item]);
            else
                return null;
        }

        private TreeNode GetNode( Project project )
        {
            if ( project == null )
                return null;

            if ( this.projects.Contains( project ) )
                return ((TreeNode)this.projects[project]);
            else
                return null;
        }

        


        #region class ItemHashCodeProvider
        private class ItemHashCodeProvider : IHashCodeProvider
        {        
            public int GetHashCode(object obj)
            {
                try
                {
                    return ((ProjectItem)obj).get_FileNames(1).GetHashCode();
                }
                catch( Exception )
                {
                    return obj.GetHashCode();
                }
            }
        }
        #endregion

        #region class ItemComparer
        private class ItemComparer : IComparer
        {        
            public int Compare(object x, object y)
            {
                try
                {
                    return ((ProjectItem)x).get_FileNames(1).CompareTo(
                        ((ProjectItem)y).get_FileNames(1) );
                }
                catch( Exception )
                {
                    return -1;
                }
            }
        }
        #endregion
        #region class ProjectHashCodeProvider
        private class ProjectHashCodeProvider : IHashCodeProvider
        {        
            public int GetHashCode(object obj)
            {
                try
                {
                    return ((Project)obj).FullName.GetHashCode();
                }
                catch( Exception )
                {
                    return obj.GetHashCode();
                }
            }
        }
        #endregion

        #region class ProjectComparer
        private class ProjectComparer : IComparer
        {        
            public int Compare(object x, object y)
            {
                try
                {
                    return ((Project)x).FullName.CompareTo(
                        ((Project)y).FullName );
                }
                catch( Exception )
                {
                    return -1;
                }
            }
        }
        #endregion

        #region StatusCache
        /// <summary>
        ///  used to accumulate and persist state from status callbacks when
        ///  generating status cache.
        /// </summary>
        private class StatusCache
        {
            public Status this[ string path ]
            {
                get{ return (Status)dict[path]; }
            }

            public void StatusFunc( string path, Status status )
            {
                dict[path] = status;

            }

            private IDictionary dict = new Hashtable();
        }
        #endregion


        private _DTE dte;
        private IntPtr treeview;
        private UIHierarchyItem solutionItem;
        private TreeNode root;
        private UIHierarchy uiHierarchy;
        private const string VSNETWINDOW = "wndclass_desked_gsk";
        private const string GENERICPANE = "GenericPane";
        private const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        private const string TREEVIEW = "SysTreeView32";
        private IDictionary projectItems;
        private IDictionary projects;
        private TreeNode solutionNode;
        private Swf.ImageList statusImageList;
        private SvnContext context;
        private StatusCache statusCache;

        private const string STATUS_IMAGES = "Ankh.status_icons.bmp";
    }
}
