// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Ankh.UI;
using Ankh.UI.Services;
using SharpSvn;
using Utils;
using Ankh.Scc;
using IServiceProvider = System.IServiceProvider;
using Ankh.Selection;
using Ankh.EventSinks;
using System.Collections.Generic;
using Ankh.VS;
using Ankh.ContextServices;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    public class OldAnkhContext : IContext, IDTEContext, IAnkhServiceProvider
    {
        /// <summary>
        /// Fired when the addin is unloading.
        /// </summary>
        public event EventHandler Unloading;

        public OldAnkhContext(IAnkhPackage package)
            : this(package, new UIShell(package))
        {
            Trace.Assert(package != null);
        }

        public OldAnkhContext(IAnkhPackage package, IUIShell uiShell)
        {
            this.package = package;

            this.dte = (EnvDTE._DTE)package.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            this.uiShell = uiShell;
            this.uiShell.Context = this;

            this.config = package.GetService<IAnkhConfigurationService>();

            this.LoadConfig();

            this.outputPane = new OutputPaneWriter(this, "AnkhSVN");

            this.progressDialog = new ProgressDialog();

            //this.SetUpEvents();    

            this.conflictManager = new ConflictManager(this);

            //GC.KeepAlive(this.solutionExplorerWindow.TreeWindow);

            this.repositoryController =
                new RepositoryExplorer.Controller(this);
            this.workingCopyExplorer =
                new Ankh.WorkingCopyExplorer.WorkingCopyExplorer(this);
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
        public ISvnClientPool ClientPool
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.clientPool ?? (this.clientPool = GetService<ISvnClientPool>()); }
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
        /// The configloader.
        /// </summary>
        public IAnkhConfigurationService Configuration
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.config; }
        }

        public bool OperationRunning
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.operationRunning; }
        }

        /// <summary>
        /// Manage issues related to conflicts.
        /// </summary>
        public ConflictManager ConflictManager
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.conflictManager; }
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
        /// Should be called before starting any lengthy operation
        /// </summary>
        public IDisposable StartOperation(string description)
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

            this.progressDialog.Caption = description;

            return new OperationCompleter(this, this.OutputPane.StartActionText(description));
        }

        class OperationCompleter : IDisposable
        {
            OldAnkhContext _context;
            IDisposable _disp2;
            
            public OperationCompleter(OldAnkhContext context, IDisposable disp2)
            {
                _context = context;
                _disp2 = disp2;
            }

            public void Dispose()
            {
                _context.EndOperation();
                _context = null;
                _disp2.Dispose();
                _disp2 = null;
            }
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

        /// <summary>
        /// try to load the configuration file
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                this.config.LoadConfig();
            }
            catch (Ankh.Configuration.ConfigException ex)
            {
                MessageBox.Show(GetService<IAnkhDialogOwner>().DialogOwner,
                    "There is an error in your configuration file:" +
                    Environment.NewLine + Environment.NewLine +
                    ex.Message + Environment.NewLine + Environment.NewLine +
                    "Please edit the " + this.config.UserConfigurationPath +
                    " file and correct the error." + Environment.NewLine +
                    "Ankh will now load a default configuration.", "Configuration error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // fall back on the default configuration
                this.config.LoadDefaultConfig();
            }

            SetupFromConfig();

            this.Configuration.ConfigFileChanged += new EventHandler(ConfigLoader_ConfigFileChanged);

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
                this.Configuration.LoadConfig();
                SetupFromConfig();
                this.OutputPane.WriteLine("Configuration reloaded.");
            }
            catch (Ankh.Configuration.ConfigException ex)
            {
                this.OutputPane.WriteLine("Configuration file has errors: " + ex.Message);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);
                else
                    throw;                
            }
        }

        private void SetupFromConfig()
        {
            using (SvnClient client = ClientPool.GetClient())
            {
                
            }
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

        private EnvDTE._DTE dte;

        private WorkingCopyExplorer.WorkingCopyExplorer workingCopyExplorer;
        private RepositoryExplorer.Controller repositoryController;

        private OutputPaneWriter outputPane;

        private bool operationRunning;

        private ConflictManager conflictManager;

        private ProgressDialog progressDialog;
        private ISvnClientPool clientPool;

        private IUIShell uiShell;

        private IAnkhConfigurationService config;

        readonly IAnkhPackage package;

        #region IAnkhServiceProvider Members

        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return GetService(typeof(T)) as T;
        }

        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return GetService(serviceType) as T;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        #endregion
    }
}
