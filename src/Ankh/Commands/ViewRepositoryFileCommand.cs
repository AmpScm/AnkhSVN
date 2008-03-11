// $Id$
using System;
using System.Collections;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    public abstract class ViewRepositoryFileCommand : CommandBase
    {
        #region ICommand Members
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            /*if (e.Context.RepositoryExplorer.SelectedNode == null ||
                e.Context.RepositoryExplorer.SelectedNode.IsDirectory)*/
            {
                e.Enabled = false;
            }
        }
        #endregion
      
    }
}



