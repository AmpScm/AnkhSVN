// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Commits an item.
	/// </summary>
	[VSNetCommand("CommitItem", Text = "Commit", Tooltip = "Commits an item"),
     VSNetControl( "Item", Position = 2 )]
	internal class CommitItem : CommandBase
	{	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            ILocalResource[] resources = context.SolutionExplorer.GetSelectedItems();
            foreach( ILocalResource resource in resources )
            {
                if ( (resource.Status.TextStatus & commitCandidates) != 0 ||
                    (resource.Status.PropertyStatus & commitCandidates) != 0 )
                    return vsCommandStatus.vsCommandStatusEnabled | 
                        vsCommandStatus.vsCommandStatusSupported;
            }
            
            return vsCommandStatus.vsCommandStatusEnabled;
        }
        public override void Execute(Ankh.AnkhContext context)
        {
            ILocalResource[] resources = context.SolutionExplorer.GetSelectedItems();
            WorkingCopyResource[] wcResources = new WorkingCopyResource[ resources.Length ];
            resources.CopyTo( wcResources, 0 );

            WorkingCopyResource.Commit( wcResources, false );

            context.SolutionExplorer.UpdateSelectionStatus();
        }
        
        #endregion

        

        private const StatusKind commitCandidates = StatusKind.Added | 
            StatusKind.Modified;
    }
}



