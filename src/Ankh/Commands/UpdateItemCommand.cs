// $Id$
using System;
using EnvDTE;
using NSvn;
using Ankh.UI;
using System.Collections;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that updates an item.
	/// </summary>
	[VSNetCommand("UpdateItem", Text = "Update", Tooltip = "Updates the local item"),
     VSNetControl( "Item", Position = 2 ),
     VSNetControl( "Project", Position = 2 ),
     VSNetControl( "Solution", Position = 2 ),
     VSNetControl( "Folder", Position = 2 )]
	internal class UpdateItem : CommandBase
	{		
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // all items must be versioned if we are going to run update.
            VersionedVisitor v = new VersionedVisitor();
            context.SolutionExplorer.VisitSelectedItems( v );
            
            if ( v.IsVersioned )
                return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusUnsupported;
        }

        public override void Execute(AnkhContext context)
        {
            // we assume by now that all items are working copy resources.
            UpdateVisitor v = new UpdateVisitor();
            context.SolutionExplorer.VisitSelectedItems( v );
            v.Update();
            context.SolutionExplorer.UpdateSelectionStatus();
        }    
        #endregion

        #region UpdateVisitor
        private class UpdateVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource )
            {
                this.resources.Add( resource );
            }           

            public void Update()
            {
                IList list = this.Filter();
                foreach( WorkingCopyResource resource in list )
                    resource.Update();
            }

            /// <summary>
            /// Filter out paths that are subcomponents of other paths.
            /// </summary>
            /// <returns></returns>
            private IList Filter()
            {
                ArrayList list = new ArrayList();
                foreach( WorkingCopyResource outer in this.resources )
                {
                    bool found = false;
                    foreach( WorkingCopyResource inner in this.resources )
                    {
                        if ( outer.Path.IndexOf( inner.Path ) == 0 && outer.Path != inner.Path )
                        {
                            found = true;
                            break;
                        }
                    }

                    if ( !found )
                        list.Add( outer );
                }

                return list;
            }


            private ArrayList resources = new ArrayList();
        }
        #endregion
    }
}



