// $Id$
using System;
using EnvDTE;
using Utils;

using NSvn.Core;
using NSvn.Common;
using Utils.Win32;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.IO;

using C = Utils.Win32.Constants;
using DteConstants = EnvDTE.Constants;
using System.Windows.Forms;


namespace Ankh.Solution
{

    /// <summary>
    /// Represents the Solution Explorer window in the VS.NET IDE
    /// </summary>
    public class Explorer : ISolutionExplorer
    {
        public Explorer( _DTE dte, IContext context )
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
            this.context.ProjectFileWatcher.AddFile( this.DTE.Solution.FullName );
            this.SetUpTreeview();
            this.SyncAll();

            Trace.WriteLine( String.Format( "Cache hit rate: {0}%", 
                this.context.StatusCache.CacheHitSuccess ), "Ankh" );
        }

        /// <summary>
        ///  Resets the state of the object. To be called when a solution is unloaded.
        /// </summary>
        public void Unload()
        {
            Debug.WriteLine( "Unloading existing solution information", "Ankh" );
            this.projectItems.Clear();
            this.projects.Clear();
            if ( this.treeview != null )
                this.treeview.ClearStatusImages();
            if ( this.originalImageList != IntPtr.Zero )
            {
                this.treeview.StatusImageList = originalImageList;
                originalImageList = IntPtr.Zero;
            }
            this.context.ProjectFileWatcher.Clear();
            this.solutionNode = null;
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
                if ( node != null )
                    node.Refresh();
            }
        }


        /// <summary>
        /// Updates the status of the given item.
        /// </summary>
        /// <param name="item"></param>
        public void Refresh( ProjectItem item )
        {
            TreeNode node = (TreeNode)this.projectItems[item];
            if ( node != null )
                node.Refresh();
        }       

        public void SyncAll()
        {
            // build the whole tree anew
            Debug.WriteLine( "Synchronizing with treeview", "Ankh" );

            this.projectItems.Clear();
            this.projects.Clear();
            
            // store the original image list
            this.originalImageList = this.treeview.StatusImageList;
            
            // and assign the status image list to the tree
            this.treeview.StatusImageList = statusImageList.Handle;

            SolutionLoadStrategy.GetStrategy( dte.Version ).Load( this );            
 
            Debug.WriteLine( "Created solution node", "Ankh" );
        }

        /// <summary>
        /// Returns the SvnItem resources associated with the selected items
        /// in the solution explorer.
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>        /// 
        /// <returns>A list of SvnItem instances.</returns>
        public IList GetSelectionResources( bool getChildItems )
        {
            return this.GetSelectionResources( getChildItems, null );
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

        


        /// <summary>
        /// Returns the SvnItem resources associated with the selected items
        /// in the solution explorer.
        /// </summary>
        /// <param name="getChildItems">Whether children of the items in 
        /// question should be included.</param>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        public IList GetSelectionResources( bool getChildItems, 
            ResourceFilterCallback filter )
        {
            ArrayList list = new ArrayList();

            object o = this.uiHierarchy.SelectedItems;	 	
            foreach( UIHierarchyItem item in (Array)this.uiHierarchy.SelectedItems )	 	
            {	 	
                TreeNode node = this.GetNode( item );	 	
                if ( node != null )	 	
                    node.GetResources( list, getChildItems, filter );	 	
            }

            return list;
        }

        /// <summary>
        /// Returns all  the SvnItem resources from root
        /// </summary>
        /// <param name="filter">A callback used to filter the items
        /// that are added.</param>
        /// <returns>A list of SvnItem instances.</returns>
        public IList GetAllResources( ResourceFilterCallback filter )
        {
            ArrayList list = new ArrayList();

            TreeNode node = solutionNode; 	
            if ( node != null )	 	
                node.GetResources( list, true, filter );	 	

            return list;
        }

        /// <summary>
        /// Retrieves the resources associated with a project item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public IList GetItemResources( ProjectItem item, bool recursive )
        {
            ArrayList list = new ArrayList();

            TreeNode node = this.GetNode(item);
            if ( node != null )
                node.GetResources( list, recursive, null );

            return list;
        }

        public ProjectItem GetSelectedProjectItem()
        {
            Array array = (Array)this.uiHierarchy.SelectedItems;
            UIHierarchyItem uiItem = (UIHierarchyItem)array.GetValue(0);
            return uiItem.Object as ProjectItem;
        }

       

        internal TreeView TreeView
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.treeview; }
        }

        internal IContext Context
        {
            get{ return this.context; }
        }

        internal _DTE DTE
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.dte; }
        }

        /// <summary>
        /// Retrieves the window handle to the solution explorer treeview and uses it
        /// to replace it's imagelist with our own.
        /// </summary>
        internal void SetUpTreeview()
        {
            // dragons be here - modify with care
            Debug.WriteLine( "Setting up treeview", "Ankh" );
            Window solutionExplorerWindow = this.dte.Windows.Item(
                EnvDTE.Constants.vsWindowKindSolutionExplorer);

            // we need to make sure its not hidden and that it is dockable
            bool linkable = solutionExplorerWindow.Linkable;
            bool hidden = solutionExplorerWindow.AutoHides;
            bool isFloating = solutionExplorerWindow.IsFloating;

            // these two operations need to be done in an exact order, 
            // depending on whether it is initially hidden
            if ( hidden )
            {
                solutionExplorerWindow.AutoHides = false;
                solutionExplorerWindow.IsFloating = false;
                solutionExplorerWindow.Linkable = true;
            }
            else
            {
                solutionExplorerWindow.IsFloating = false;
                solutionExplorerWindow.Linkable = true;
                solutionExplorerWindow.AutoHides = false;
            }
            
            // find the solution explorer window
            // Get the caption of the solution explorer            
            string slnExplorerCaption = solutionExplorerWindow.Caption;
            Debug.WriteLine( "Caption of solution explorer window is " + slnExplorerCaption, 
                "Ankh" );
            //            string vsnetCaption = this.dte.MainWindow.C
            IntPtr vsnet = (IntPtr)this.dte.MainWindow.HWnd;//Win32.FindWindow( VSNETWINDOW, null );

            // first try finding it as a child of the main VS.NET window
            IntPtr slnExplorer = Win32.FindWindowEx( vsnet, IntPtr.Zero, GENERICPANE, 
                slnExplorerCaption );

            // not there? Try looking for a floating palette. These are toplevel windows for 
            // some reason
            if ( slnExplorer == IntPtr.Zero )
            {
                Debug.WriteLine( "Solution explorer not a child of VS.NET window. " + 
                    "Searching floating windows", "Ankh" );
                // we need to search for the caption of any of the potentially linked windows
                IntPtr floatingPalette = IntPtr.Zero;
                foreach( Window win in solutionExplorerWindow.LinkedWindowFrame.LinkedWindows )
                {
                    floatingPalette = Win32.FindWindow( VBFLOATINGPALETTE, 
                        win.Caption );
                    if ( floatingPalette != IntPtr.Zero )
                        break;
                }
                
                // the solution explorer should be a direct child of the palette
                slnExplorer = Win32.FindWindowEx( floatingPalette, IntPtr.Zero, GENERICPANE,
                    slnExplorerCaption );
            }

            IntPtr uiHierarchy = Win32.FindWindowEx( slnExplorer, IntPtr.Zero, 
                UIHIERARCHY, null );
            IntPtr treeHwnd = Win32.FindWindowEx( uiHierarchy, IntPtr.Zero, TREEVIEW, 
                null );         
 
            if ( treeHwnd == IntPtr.Zero )
                throw new ApplicationException( 
                    "Could not attach to solution explorer treeview. If the solution explorer " + 
                    "window is on a secondary monitor, " +
                    "try moving it to the primary during solution loading." );

            this.treeview = new TreeView( treeHwnd );

            // reset back to the original hiding-state and dockable state            
            solutionExplorerWindow.Linkable = linkable;
            solutionExplorerWindow.IsFloating = isFloating;
            if ( solutionExplorerWindow.Linkable )
                solutionExplorerWindow.AutoHides = hidden;

            // load the status images image strip
            Debug.WriteLine( "Loading status images", "Ankh" );
            Bitmap statusImages = (Bitmap)Image.FromStream( 
                this.GetType().Assembly.GetManifestResourceStream( STATUS_IMAGES ) );

            statusImages.MakeTransparent( statusImages.GetPixel(0,0) );

            this.statusImageList = new ImageList();
            this.statusImageList.ImageSize = new Size(7, 16);
            this.statusImageList.Images.AddStrip( statusImages );    
        
        }

        

        /// <summary>
        /// Adds a new resource to the tree.
        /// </summary>
        internal void AddResource( ProjectItem key, TreeNode node )
        {
            this.projectItems[key] = node;
        }

        internal void AddResource( Project key, TreeNode node, string projectFile )
        {
            this.projects[key] = node;
            this.context.ProjectFileWatcher.AddFile( projectFile );
        }

        internal void SetSolution( TreeNode node )
        {
            // we assume theres only one of these
            this.solutionNode = node;
        }

        private TreeNode GetNode(UIHierarchyItem item)
        {
            if ( item.Object == null )
                return null;

            if ( item.Object is ProjectItem && this.projectItems.Contains( item.Object ) )
                return ((TreeNode)this.projectItems[item.Object]);
            else if ( item.Object is Project && this.projects.Contains(item.Object) )
                return ((TreeNode)this.projects[item.Object]); 
            else if ( item == this.uiHierarchy.UIHierarchyItems.Item(1) )
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

        #region SolutionLoadStrategy
        /// <summary>
        /// Encapsulates the details of how to handle a solution load.
        /// </summary>
        private class SolutionLoadStrategy
        {
            protected SolutionLoadStrategy()
            {
            }

            public virtual void Load( Explorer outer )
            {
                DateTime startTime = DateTime.Now;
                outer.context.StartOperation( "Synchronizing with solution explorer");
                

                // avoid lots of flickering while we walk the tree
                outer.treeview.LockWindowUpdate(true);
                try
                {
                    // we assume there is a single root node
                    outer.solutionNode = TreeNode.CreateSolutionNode(
                        outer.uiHierarchy.UIHierarchyItems.Item(1), outer);

                    // and we're done
                    outer.context.OutputPane.WriteLine( "Time: {0}", DateTime.Now - startTime );
                }
                finally
                {
                    // done
                    outer.treeview.LockWindowUpdate(false);
                    outer.context.EndOperation();
                }
            }

            public virtual void CancelLoad()
            {
                // no-op, since it will never be called
            }

            public static SolutionLoadStrategy GetStrategy( string version )
            {
                // just load right away if we're on vs7.x, poll on a 
                // thread if we're on 8(+).x
                if ( strategy == null )
                {
                    if ( version[0] == '7' )
                        strategy = new SolutionLoadStrategy();
                    else
                        strategy = new SolutionLoadStrategy2005();
                }

                return strategy;
            }

            private static SolutionLoadStrategy strategy;
        }


        private class SolutionLoadStrategy2005 : SolutionLoadStrategy
        {
            public override void Load( Explorer outer )
            {
                this.outer = outer;

                // create a thread to poll the solution explorer
                System.Threading.Thread thread = new System.Threading.Thread(
                    new System.Threading.ThreadStart(this.ThreadProc) );
                thread.Start();
            }

            public override void CancelLoad()
            {
                this.done = true;
            }


            private void ThreadProc()
            {
                try
                {
                    this.done = false;
                    DateTime startTime = DateTime.Now;

                    // loop until all the items have been loaded in the solution explorer
                    UIHierarchyItem item = outer.uiHierarchy.UIHierarchyItems.Item(1);

                    // if there is a Misc Items project in the solution, 
                    // it doesn't necessarily appear as an UIHierarchyItem
                    int targetCount = outer.DTE.Solution.Projects.Count;
                    targetCount = this.HasMiscItems() ? targetCount - 1 : targetCount;

                    while( !done && DateTime.Now - startTime < TimeOut )
                    {
                        Trace.WriteLine( String.Format("UIHierarchyItems: {0}, Projects.Count: {1}",
                            item.UIHierarchyItems.Count, outer.dte.Solution.Projects.Count),
                            "Ankh" );
 
                        if ( item.UIHierarchyItems.Count >= targetCount )
                        {
                            // make sure this is invoked on the main GUI thread
                            Trace.WriteLine( "Found all UIHierarchyItems, loading", "Ankh" );
                            this.outer.Context.Client.SynchronizingObject.Invoke( 
                                new LoadDelegate( this.DoLoad ), 
                                new object[]{} );
                            done = true;
                        }
                        System.Threading.Thread.Sleep( 250 );
                    }

                    if ( !done )
                    {
                        // if we have discovered some, load anyway
                        if ( item.UIHierarchyItems.Count > 0 )
                        {
                            Trace.WriteLine( "UIHierarchyItems for all projects not found, " + 
                                "loading those present", "Ankh" );
                            Trace.WriteLine(  String.Format("UIHierarchyItems: {0}, Projects.Count: {1}",
                                item.UIHierarchyItems.Count, outer.dte.Solution.Projects.Count),
                                "Ankh" );
                            this.outer.Context.Client.SynchronizingObject.Invoke( 
                                new LoadDelegate( this.DoLoad ), 
                                new object[]{} );
                        }
                        else
                        {
                            Trace.WriteLine( "No UIHierarchyItems found during solution load", "Ankh" );

                            this.outer.Context.Client.SynchronizingObject.Invoke(
                                new LoadDelegate( this.ShowTimeOutMessage ),
                                new object[]{} );                        
                        }
                    }
                }
                catch( Exception ex )
                {
                    Debug.WriteLine( ex );
                }
            }

            private void ShowTimeOutMessage()
            {
                outer.Context.UIShell.ShowMessageBox( 
                    "Solution is under version control, but UIHiearchyItems " + 
                    "in the solution explorer failed to appear", 
                    "Solution load timed out", MessageBoxButtons.OK );
            }

            private void DoLoad()
            {
                try
                {
                    base.Load( this.outer );
                }
                catch( Exception ex )
                {
                    this.outer.Context.ErrorHandler.Handle( ex );
                }
            }

            private bool HasMiscItems()
            {
                foreach (Project project in this.outer.DTE.Solution.Projects)
                {
                    if (project.Kind == MiscItemsKind)
                        return true;
                }
                return false;
            }


            private const string MiscItemsKind = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";
            private readonly static TimeSpan TimeOut = new TimeSpan(0, 0, 10);
            private delegate void LoadDelegate( );
            private Explorer outer;
            private bool done;
        }
        #endregion

        #region class ItemHashCodeProvider
        private class ItemHashCodeProvider : IHashCodeProvider
        {        
            public int GetHashCode(object obj)
            {
                try
                {                    
                    string str = ItemComparer.GetProjectName(obj) + "|" + 
                        ItemComparer.GetFileName(obj);
                    return str.GetHashCode();
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
                    string str_a = GetProjectName(x) + "|" + GetFileName(x);
                    string str_b = GetProjectName(y) + "|" + GetFileName(y);
                    return str_a.CompareTo(str_b);
                }
                catch( Exception )
                {
                    return -1;
                }
            }

            internal static string GetFileName( object obj )
            {
                string filename = null;
                try
                {
                    filename = ((ProjectItem)obj).get_FileNames(1);
                }
                catch( Exception )
                {
                    // hack for those project types which use a 0-based index
                    // (GRRRR)
                    filename = ((ProjectItem)obj).get_FileNames(0);
                }

                return filename;
            }

            internal static string GetProjectName( object obj )
            {
                string projectName = null;
                try
                {
                    projectName = ((ProjectItem)obj).ContainingProject.FullName;
                }
                catch ( Exception )
                {
                    projectName = "";
                }

                return projectName;
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


        private _DTE dte;
        private UIHierarchy uiHierarchy;
        private const string VSNETWINDOW = "wndclass_desked_gsk";
        private const string GENERICPANE = "GenericPane";
        private const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        private const string TREEVIEW = "SysTreeView32";
        private const string VBFLOATINGPALETTE = "VBFloatingPalette";
        private IDictionary projectItems;
        private IDictionary projects;
        private TreeNode solutionNode;
        private ImageList statusImageList;
        private IContext context;
        private TreeView treeview;
        private IntPtr originalImageList = IntPtr.Zero;

        private const string STATUS_IMAGES = "Ankh.status_icons.bmp";
    }
}
