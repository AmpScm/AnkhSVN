// $Id$
using System.Windows.Forms;
using Ankh.Solution;
using System;

namespace Ankh
{
    public interface IContext
    {
        /// <summary>
        /// Raised when Ankh is unloading.
        /// </summary>
        event EventHandler Unloading;

        /// <summary>
        /// The top level automation object.
        /// </summary>
        EnvDTE._DTE DTE { get; }

        /// <summary>
        /// The addin object.
        /// </summary>
        EnvDTE.AddIn AddIn { get; }

        /// <summary>
        /// The SolutionExplorer object.
        /// </summary>
        Explorer SolutionExplorer { get; }

        /// <summary>
        /// The output pane.
        /// </summary>
        OutputPaneWriter OutputPane { get; }

        //        public RepositoryExplorer.Controller RepositoryController
        //        {
        //            [System.Diagnostics.DebuggerStepThrough]
        //            get{ return this.repositoryController; }
        //        }



        /// <summary>
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        SvnClient Client { get; }

        /// <summary>
        /// The repository explorer control.
        /// </summary>
        RepositoryExplorer.Controller RepositoryExplorer { get; }

        EnvDTE.Window RepositoryExplorerWindow { get; }

        /// <summary>
        /// Whether a solution is open.
        /// </summary>
        bool SolutionIsOpen { get; }

        bool AnkhLoadedForSolution { get; }

        /// <summary>
        /// The Ankh configuration.
        /// </summary>
        Ankh.Config.Config Config { get; }

        /// <summary>
        /// The error handler.
        /// </summary>
        IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// The configloader.
        /// </summary>
        Ankh.Config.ConfigLoader ConfigLoader { get; }

        /// <summary>
        /// The status cache.
        /// </summary>
        StatusCache StatusCache { get; }

        bool OperationRunning { get; }

        /// <summary>
        /// An IWin32Window to be used for parenting dialogs.
        /// </summary>
        IWin32Window HostWindow { get; }

        /// <summary>
        /// Manage issues related to conflicts.
        /// </summary>
        ConflictManager ConflictManager { get; }

        /// <summary>
        /// Event handler for the SolutionOpened event. Can also be called at
        /// addin load time, or if Ankh is enabled for a solution.
        /// </summary>
        void SolutionOpened();

        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        void SolutionClosing();

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        void StartOperation( string description );

        /// <summary>
        ///  called at the end of any lengthy operation
        /// </summary>
        void EndOperation();

        /// <summary>
        /// Miscellaneous cleanup stuff goes here.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Sets up event handlers.
        /// </summary>
        void SetUpEvents();

        /// <summary>
        /// try to load the configuration file
        /// </summary>
        void LoadConfig();

        bool CheckWhetherAnkhShouldLoad();
        bool QueryWhetherAnkhShouldLoad( string solutionDir );
        void CreateRepositoryExplorer();
    }
}
