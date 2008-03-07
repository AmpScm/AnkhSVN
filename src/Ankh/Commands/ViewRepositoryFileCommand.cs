// $Id$
using System;
using System.Collections;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    [VSNetCommand(AnkhCommand.ViewRepositoryFile,
		"ViewRepositoryFile", Tooltip="View this file", Text = "In VS.NET" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    public abstract class ViewRepositoryFileCommand : CommandBase
    {
        #region ICommand Members
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Context.RepositoryExplorer.SelectedNode == null ||
                e.Context.RepositoryExplorer.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            }
        }
        #endregion
      
    }
}



