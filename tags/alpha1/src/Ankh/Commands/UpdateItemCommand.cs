// $Id$
using System;
using EnvDTE;
using NSvn;
using Ankh.UI;
using System.Collections;
using System.Diagnostics;
using Ankh.Solution;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that updates an item.
	/// </summary>
	[VSNetCommand("UpdateItem", Text = "Update", Tooltip = "Updates the local item",
         Bitmap = ResourceBitmaps.Update),
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
            context.SolutionExplorer.VisitSelectedItems( v, false );
            
            if ( v.IsVersioned )
                return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusUnsupported;
        }

        public override void Execute(AnkhContext context)
        {
            context.OutputPane.StartActionText("Updating");
            // we assume by now that all items are working copy resources.
            UpdateVisitor v = new UpdateVisitor();
            context.SolutionExplorer.VisitSelectedNodes( v );
            v.Update();
            context.SolutionExplorer.UpdateSelectionStatus();
            context.OutputPane.EndActionText();
        }    
        #endregion

        #region UpdateVisitor
        private class UpdateVisitor : LocalResourceVisitorBase, INodeVisitor
        {
            public void Update()
            {
                foreach( WorkingCopyResource resource in this.resources )
                {
                    Trace.WriteLine( "Updating " + resource.Path, "Ankh" );
                    resource.Update();
                }
            }

            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                this.resources.Add( resource );
            }

            public void VisitProject(Ankh.Solution.ProjectNode node)
            {
                // some project types dont necessarily have a project folder
                if ( node.ProjectFolder != SvnResource.Unversionable )
                    this.resources.Add( node.ProjectFolder );
                else
                    node.VisitResources( this, true );
            }           

            public void VisitProjectItem(Ankh.Solution.ProjectItemNode node)
            {
                node.VisitResources( this, false );
            } 

            public void VisitSolutionNode(Ankh.Solution.SolutionNode node)
            {
                string solutionPath = ";"; // illegal in a path
                if ( node.SolutionFolder != SvnResource.Unversionable )
                {
                    node.SolutionFolder.Accept( this );
                    solutionPath = node.SolutionFolder.Path.ToLower();
                }

                // update all projects whose folder is not under the solution root
                foreach( TreeNode n in node.Children )
                {
                    ProjectNode pNode = n as ProjectNode;
                    if ( pNode != null && ( 
                        pNode.ProjectFolder == SvnResource.Unversionable ||
                        pNode.ProjectFolder.Path.ToLower().IndexOf( 
                        solutionPath ) != 0 ) )
                    {
                        pNode.Accept( this );
                    }
                }
            }

            private ArrayList resources = new ArrayList();
        }            
        #endregion
    }
}



