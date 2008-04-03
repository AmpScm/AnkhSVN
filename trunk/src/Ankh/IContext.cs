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
    public interface IContext : IAnkhServiceProvider
    {
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
		//[Obsolete("Use .SelectionContext wherever possible")]
        RepositoryExplorer.Controller RepositoryExplorer { get; }

        /// <summary>
        /// The configloader.
        /// </summary>
        IAnkhConfigurationService Configuration { get; }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        IDisposable StartOperation( string description );

        //[Obsolete("Use .SelectionContext wherever possible")]
        IWorkingCopyExplorer WorkingCopyExplorer
        {
            get;
        }
    }
}
