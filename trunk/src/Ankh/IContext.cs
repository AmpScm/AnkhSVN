// $Id$
using System.Windows.Forms;
using System;

using IServiceProvider = System.IServiceProvider;
using SharpSvn;
using Ankh.UI.Services;
using Ankh.Selection;
using Ankh.UI;

namespace Ankh
{
    [CLSCompliant(false)]
    public interface IDTEContext
    {
        /// <summary>
        /// The top level automation object.
        /// </summary>
        EnvDTE._DTE DTE { get; }
    }

    public interface IContext : IAnkhServiceProvider
    {
        IAnkhPackage Package { get; }        

        /// <summary>
        /// The UI shell.
        /// </summary>
        IUIShell UIShell { get; }

        /// <summary>
        /// The output pane.
        /// </summary>
        OutputPaneWriter OutputPane { get; }

        /// <summary>
        /// The ISvnClientPool object used by the NSvn objects.
        /// </summary>
        ISvnClientPool ClientPool { get; }

        /// <summary>
        /// The repository explorer controller.
        /// </summary>
		[Obsolete("Use .SelectionContext wherever possible")]
        RepositoryExplorer.Controller RepositoryExplorer { get; }

        /// <summary>
        /// Whether a solution is open.
        /// </summary>
        bool SolutionIsOpen { get; }

        /// <summary>
        /// Whether Ankh is loaded for the current solution.
        /// </summary>
        bool AnkhLoadedForSolution { get; }

        /// <summary>
        /// The Ankh configuration.
        /// </summary>
        Ankh.Config.Config Config { get; }

        /// <summary>
        /// The error handler.
        /// </summary>
        IAnkhErrorHandler ErrorHandler { get; }

        /// <summary>
        /// The configloader.
        /// </summary>
        Ankh.Config.ConfigLoader ConfigLoader { get; }

        /// <summary>
        /// Whether an operation is currently running.
        /// </summary>
        bool OperationRunning { get; }

        /// <summary>
        /// Manage issues related to conflicts.
        /// </summary>
        ConflictManager ConflictManager { get; }

        /// <summary>
        /// The OLE Serviceprovider.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        IDisposable StartOperation( string description );

        [Obsolete("Use .SelectionContext wherever possible")]
        IWorkingCopyExplorer WorkingCopyExplorer
        {
            get;
        }
    }
}
