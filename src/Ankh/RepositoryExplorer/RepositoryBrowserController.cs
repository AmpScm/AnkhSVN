using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using SharpSvn;
using EnvDTE;
using Utils;

using Ankh.ContextServices;
using Ankh.UI;
using Ankh.VS;
using Thread = System.Threading.Thread;
using Ankh.WorkingCopyExplorer;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Responsible for controlling the repository explorer.
    /// </summary>
    public class RepositoryBrowserController : AnkhService
    {
        RepositoryExplorerControl _repositoryExplorer;

        public RepositoryBrowserController(IAnkhServiceProvider context)
            : base(context)
        {
            if(Shell != null && Shell.RepositoryExplorer != null)
                SetControl(Shell.RepositoryExplorer);
        }

        IExplorersShell _shell;
        IExplorersShell Shell
        {
            get { return _shell ?? (_shell = GetService<IExplorersShell>()); }
        }

        internal void SetControl(RepositoryExplorerControl value)
        {
            this._repositoryExplorer = Shell.RepositoryExplorer;
        }

        /// <summary>
        /// Add a new root node to the repository explorer.
        /// </summary>
        /// <param name="info"></param>
        public void AddRoot(Uri uri)
        {
            _repositoryExplorer.AddRoot(uri);
        }

        public Uri SelectedUri
        {
            get
            {
                if (_repositoryExplorer != null)
                    return _repositoryExplorer.SelectedUri;

                return null;
            }
        }
   }
}
