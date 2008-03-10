// $Id$
using System.Windows.Forms;
using System;

using IServiceProvider = System.IServiceProvider;
using SharpSvn;
using Ankh.UI.Services;
using Ankh.Selection;

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

    public interface IContext
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
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        SvnClient Client { get; }

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
        IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// The configloader.
        /// </summary>
        Ankh.Config.ConfigLoader ConfigLoader { get; }

        /// <summary>
        /// The status cache.
        /// </summary>
        StatusCache StatusCache { get; }

        /// <summary>
        /// Whether an operation is currently running.
        /// </summary>
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


        [Obsolete("Please use .SelectionContext instead")]
        IAnkhSelectionContainer Selection
        {
            get;
        }

		ISelectionContext SelectionContext
		{
			get;
		}

		System.ComponentModel.ISynchronizeInvoke SynchronizingObject
		{
			get;
		}
    }
}
