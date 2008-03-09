// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Ankh.SolutionExplorer;
using Ankh.UI;
using Ankh.UI.Services;
using SharpSvn;
using Utils;
using Utils.Services;
using IServiceProvider = System.IServiceProvider;
using Ankh.Selection;
using Ankh.EventSinks;
using System.Collections.Generic;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    public class AnkhContext : IContext, IDTEContext
    {
        /// <summary>
        /// Fired when the addin is unloading.
        /// </summary>
        public event EventHandler Unloading;

        public AnkhContext(IAnkhPackage package)
            : this(package, new UIShell())
        {
            Trace.Assert(package != null);
        }

        public AnkhContext(IAnkhPackage package, IUIShell uiShell)
        {
            this.package = package;

            this.dte = (EnvDTE._DTE)package.GetService(typeof(EnvDTE._DTE));
            this.uiShell = uiShell;
            this.uiShell.Context = this;

            this.errorHandler = new ErrorHandler(dte.Version, this);

            this.hostWindow = new Win32Window(new IntPtr(dte.MainWindow.HWnd));

            this.configLoader = new Ankh.Config.ConfigLoader();

            this.LoadConfig();

            this.projectFileWatcher = new FileWatcher(this.client);

            this.outputPane = new OutputPaneWriter(this, "AnkhSVN");

            this.progressDialog = new ProgressDialog();

            //this.SetUpEvents();    

            this.conflictManager = new ConflictManager(this);

            this.statusCache = new StatusCache(this.client);

            this.solutionExplorerWindow = new SolutionExplorerWindow(package, SynchronizingObject);
            this.selectionContext = new SelectionContext(package, statusCache, solutionExplorerWindow);
            

            //GC.KeepAlive(this.solutionExplorerWindow.TreeWindow);

            this.repositoryController =
                new RepositoryExplorer.Controller(this);
            this.workingCopyExplorer =
                new Ankh.WorkingCopyExplorer.WorkingCopyExplorer(this);

            SolutionEventsSink solutionEvents = new SolutionEventsSink(this);
            eventSinks.Add(solutionEvents);
            eventSinks.Add(new TrackProjectDocumentsEventSink(this));

            AnkhServices.AddService(typeof(IWorkingCopyOperations), new WorkingCopyOperations(client));
            NotificationHandler.GetHandler(this);

            solutionEvents.AfterOpenSolution += new EventHandler(OnAfterOpenSolution);
            solutionEvents.BeforeCloseSolution += new EventHandler(OnBeforeCloseSolution);
        }

        void OnAfterOpenSolution(object sender, EventArgs e)
        {
            solutionExplorerWindow.EnableAnkhIcons(true);
        }

        void OnBeforeCloseSolution(object sender, EventArgs e)
        {
            solutionExplorerWindow.EnableAnkhIcons(false);
        }

        public IAnkhPackage Package
        {
            get { return package; }
        }

        /// <summary>
        /// The top level automation object.
        /// </summary>
        [CLSCompliant(false)]
        public EnvDTE._DTE DTE
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.dte; }
        }

        /// <summary>
        /// The UI shell.
        /// </summary>
        public IUIShell UIShell
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.uiShell; }
        }

        /// <summary>
        /// The output pane.
        /// </summary>
        public OutputPaneWriter OutputPane
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.outputPane; }
        }

        //        public RepositoryExplorer.Controller RepositoryController
        //        {
        //            [System.Diagnostics.DebuggerStepThrough]
        //            get{ return this.repositoryController; }
        //        }



        /// <summary>
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        public SvnClient Client
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.client; }
        }

        /// <summary>
        /// The repository explorer control.
        /// </summary>
        public RepositoryExplorer.Controller RepositoryExplorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.repositoryController; }
        }

        /// <summary>
        /// Whether a solution is open.
        /// </summary>
        public bool SolutionIsOpen
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.dte.Solution.IsOpen; }
        }

        public bool AnkhLoadedForSolution
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                IAnkhSccService scc = (IAnkhSccService)Package.GetService(typeof(IAnkhSccService));

                if (scc == null)
                    return false;
                else
                    return scc.IsActive;
            }
        }


        /// <summary>
        /// The Ankh configuration.
        /// </summary>
        public Ankh.Config.Config Config
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.config; }
        }

        /// <summary>
        /// The error handler.
        /// </summary>
        public IErrorHandler ErrorHandler
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.errorHandler; }
        }

        /// <summary>
        /// The configloader.
        /// </summary>
        public Ankh.Config.ConfigLoader ConfigLoader
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.configLoader; }
        }

        /// <summary>
        /// The status cache.
        /// </summary>
        public StatusCache StatusCache
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.statusCache; }
        }

        public bool OperationRunning
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.operationRunning; }
        }

        public string SolutionDirectory
        {
            get
            {
                if (!this.SolutionIsOpen)
                    return null;

                // no point in doing anything if the solution dir doesn't exist
                string solutionPath = SelectionContext.SolutionFilename;
                if (solutionPath == String.Empty || !File.Exists(solutionPath))
                    return null;

                return Path.GetDirectoryName(solutionPath);
            }
        }


        /// <summary>
        /// An IWin32Window to be used for parenting dialogs.
        /// </summary>
        public IWin32Window HostWindow
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.hostWindow;
            }
        }

        /// <summary>
        /// Manage issues related to conflicts.
        /// </summary>
        public ConflictManager ConflictManager
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.conflictManager; }
        }

        /// <summary>
        /// Watches the project and solution files.
        /// </summary>
        public FileWatcher ProjectFileWatcher
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.projectFileWatcher; }
        }

        public IServiceProvider ServiceProvider
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return Package;
            }
        }

        public IWorkingCopyExplorer WorkingCopyExplorer
        {
            [DebuggerStepThrough]
            get { return this.workingCopyExplorer; }
        }

        /// <summary>
        /// Event handler for the SolutionOpened event. Can also be called at
        /// addin load time, or if Ankh is enabled for a solution.
        /// </summary>
        public bool EnableAnkhForLoadedSolution()
        {
            return true;
        }

        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        public void SolutionClosing()
        {
            this.conflictManager.RemoveAllTaskItems();
        }

        /// <summary>
        /// Reloads the current solution.
        /// </summary>
        /// <returns>True if the solution has been reloaded.</returns>
        public bool ReloadSolutionIfNecessary()
        {
            if (this.projectFileWatcher.HasDirtyFiles && !this.Config.DisableSolutionReload)
            {
                if (MessageBox.Show(this.HostWindow,
                    "One or more of your project files have changed as a result of a " +
                    "Subversion operation." + Environment.NewLine +
                    "It is recommended that you reload the solution now. " + Environment.NewLine +
                    "Do you want to reload the solution?", "Project files changed",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string filename = this.dte.Solution.FullName;
                    this.dte.Solution.Close(true);
                    this.dte.Solution.Open(filename);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        public void StartOperation(string description)
        {
            //TODO: maybe refactor this?
            this.operationRunning = true;
            try
            {
                this.DTE.StatusBar.Text = description + "...";
                this.DTE.StatusBar.Animate(true, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
            }
            catch (Exception)
            {
                // Swallow, not critical
            }

            this.OutputPane.StartActionText(description);

            this.progressDialog.Caption = description;
        }

        /// <summary>
        ///  called at the end of any lengthy operation
        /// </summary>
        public void EndOperation()
        {
            if (this.operationRunning)
            {
                try
                {
                    this.DTE.StatusBar.Text = "Ready";
                    this.DTE.StatusBar.Animate(false, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
                }
                catch (Exception)
                {
                    // swallow, not critical
                }

                this.OutputPane.EndActionText();
                this.operationRunning = false;
            }
        }

        /// <summary>
        /// Miscellaneous cleanup stuff goes here.
        /// </summary>
        public void Shutdown()
        {
            if (this.Unloading != null)
                this.Unloading(this, EventArgs.Empty);
        }

        public IAnkhSelectionContainer Selection
        {
            get { return new NullSelectionContainer(); }
        }

        /// <summary>
        /// Gets the selection context.
        /// </summary>
        /// <value>The selection context.</value>
        public Ankh.Selection.ISelectionContext SelectionContext
        {
            get { return selectionContext; }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get { return progressDialog; }
        }

        /// <summary>
        /// try to load the configuration file
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                this.config = this.configLoader.LoadConfig();
            }
            catch (Ankh.Config.ConfigException ex)
            {
                MessageBox.Show(this.HostWindow,
                    "There is an error in your configuration file:" +
                    Environment.NewLine + Environment.NewLine +
                    ex.Message + Environment.NewLine + Environment.NewLine +
                    "Please edit the " + this.configLoader.ConfigPath +
                    " file and correct the error." + Environment.NewLine +
                    "Ankh will now load a default configuration.", "Configuration error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // fall back on the default configuration
                this.config = this.configLoader.LoadDefaultConfig();
            }

            SetupFromConfig();

            this.ConfigLoader.ConfigFileChanged += new EventHandler(ConfigLoader_ConfigFileChanged);

        }

        /// <summary>
        /// Seems someone has updated the config file on disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConfigLoader_ConfigFileChanged(object sender, EventArgs e)
        {
            try
            {
                this.config = this.ConfigLoader.LoadConfig();
                SetupFromConfig();
                this.OutputPane.WriteLine("Configuration reloaded.");
            }
            catch (Ankh.Config.ConfigException ex)
            {
                this.OutputPane.WriteLine("Configuration file has errors: " + ex.Message);
            }
            catch (Exception ex)
            {
                this.ErrorHandler.Handle(ex);
            }
        }

        private void SetupFromConfig()
        {
            this.client = new SvnClient();
            //// should we use a custom configuration directory?
            if (this.config.Subversion.ConfigDir != null)
                this.client.LoadConfiguration(
                    Environment.ExpandEnvironmentVariables(this.config.Subversion.ConfigDir));
            else
                this.client.LoadConfigurationDefault();

            // Let SharpSvnUI handle login and SSL dialogs
            SharpSvn.UI.SharpSvnUI.Bind(client, this.HostWindow);
        }

        private bool CheckWhetherAnkhShouldLoad()
        {
            string solutionDir = this.SolutionDirectory;

            // if we don't have a valid solution directory, no point in going on
            if (solutionDir == null)
                return false;

            // maybe this solution has never been loaded before with Ankh?
            if (File.Exists(Path.Combine(solutionDir, "Ankh.Load")))
            {
                Debug.WriteLine("Found Ankh.Load", "Ankh");
                return true;
            }
            //  user has expressly specified that this solution should load?
            else if (File.Exists(Path.Combine(solutionDir, "Ankh.NoLoad")))
            {
                Debug.WriteLine("Found Ankh.NoLoad", "Ankh");
                return false;
            }
            else
            {
                Debug.WriteLine("Found neither Ankh.Load nor Ankh.NoLoad", "Ankh");

                // is this a wc?
                // the user must manually enable Ankh if soln dir is not vc
                if (AnkhServices.GetService<IWorkingCopyOperations>().IsWorkingCopyPath(solutionDir))
                    return this.QueryWhetherAnkhShouldLoad(solutionDir);
                else
                    return false;
            }
        }

        private bool QueryWhetherAnkhShouldLoad(string solutionDir)
        {
            DialogResult res = this.uiShell.QueryWhetherAnkhShouldLoad();

            if (res == DialogResult.Yes)
            {
                Debug.WriteLine("Creating Ankh.Load", "Ankh");
                string ankhLoad = Path.Combine(solutionDir, "Ankh.Load");
                File.Create(ankhLoad).Close();
                File.SetAttributes(ankhLoad, FileAttributes.Hidden);
                return true;
            }
            else if (res == DialogResult.No)
            {
                Debug.WriteLine("Creating Ankh.NoLoad", "Ankh");
                string ankhNoLoad = Path.Combine(solutionDir, "Ankh.NoLoad");
                File.Create(ankhNoLoad).Close();
                File.SetAttributes(ankhNoLoad, FileAttributes.Hidden);
            }

            return false;
        }

        #region Win32Window class
        private class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr handle)
            {
                this.handle = handle;
            }
            #region IWin32Window Members

            public System.IntPtr Handle
            {
                get
                {
                    return this.handle;
                }
            }

            #endregion
            private IntPtr handle;

        }
        #endregion


        private class NullSelectionContainer : IAnkhSelectionContainer
        {
            public void RefreshSelectionParents()
            {
            }

            public void RefreshSelection()
            {
            }

            public void SyncAll()
            {
            }

            public IList GetSelectionResources(bool getChildItems)
            {
                return EmptyList;
            }

            public IList GetSelectionResources(bool getChildItems, ResourceFilterCallback filter)
            {
                return EmptyList;
            }

            public IList GetAllResources(ResourceFilterCallback filter)
            {
                return EmptyList;
            }
            public static readonly NullSelectionContainer Instance = new NullSelectionContainer();
            private SvnItem[] EmptyList = new SvnItem[] { };
        }


        SelectionContext selectionContext;
        SolutionExplorerWindow solutionExplorerWindow;

        private EnvDTE._DTE dte;
        private IWin32Window hostWindow;

        private IAnkhSelectionContainer selectionContainer = NullSelectionContainer.Instance;

        private WorkingCopyExplorer.WorkingCopyExplorer workingCopyExplorer;
        private RepositoryExplorer.Controller repositoryController;

        private OutputPaneWriter outputPane;

        //required to ensure events will still fire
        private List<EventSink> eventSinks = new List<EventSink>();

        private Ankh.Config.Config config;

        private StatusCache statusCache;

        private bool operationRunning;

        private ConflictManager conflictManager;
        private IErrorHandler errorHandler;

        private FileWatcher projectFileWatcher;

        private ProgressDialog progressDialog;
        private SvnClient client;

        private IUIShell uiShell;

        private Ankh.Config.ConfigLoader configLoader;

        readonly IAnkhPackage package;
    }
}
