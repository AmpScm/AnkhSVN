// $Id$
using System;
using EnvDTE;
using System.Windows.Forms;
using Ankh.UI;
using System.Collections;
using System.Diagnostics;
using Ankh.Solution;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [VSNetCommand("UpdateItem", Text = "Update...", Tooltip = "Updates the local item",
         Bitmap = ResourceBitmaps.Update),
    VSNetControl( "Item", Position = 2 ),
    VSNetControl( "Project Node", Position = 2 ),
    VSNetControl( "Cab Project Node", Position = 2 ),
    VSNetControl( "Project", Position = 2 ),
    VSNetControl( "Solution", Position = 2 ),
    VSNetControl( "Folder", Position = 2 )]
    internal class UpdateItem : CommandBase
    {		
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // all items must be versioned if we are going to run update.
            IList resources = context.SolutionExplorer.GetSelectionResources( true,
                new ResourceFilterCallback(CommandBase.VersionedFilter) );
            if ( resources.Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            try
            {
                // save all files
                this.SaveAllDirtyDocuments( context );

                context.StartOperation( "Updating" );

                // we assume by now that all items are working copy resources.                
                UpdateRunner visitor = new UpdateRunner( context );
                context.SolutionExplorer.VisitSelectedNodes( visitor );
                if ( !visitor.MaybeShowUpdateDialog() )
                    return;

                // run the actual update on another thread
                visitor.Start( "Updating" );

                // this *must* happen on the primary thread.
                if ( !visitor.Cancelled )
                    context.SolutionExplorer.RefreshSelection();
            }
            finally
            {
                context.EndOperation();
            }
        }    
        #endregion

        #region UpdateVisitor
        private class UpdateRunner : ProgressRunner, INodeVisitor
        {
            public UpdateRunner( AnkhContext context ) : base(context)
            {}

            /// <summary>
            /// Show the update dialog if wanted.
            /// </summary>
            /// <returns></returns>
            public bool MaybeShowUpdateDialog()
            {
                this.recursive = false;
                this.revision = Revision.Head;

                // is Shift down?
                if ( !CommandBase.Shift )
                {
                    using(UpdateDialog d = new UpdateDialog())
                    {
                        d.GetPathInfo += new GetPathInfoDelegate(CommandBase.GetPathInfo);
                        d.Items = this.resources;
                        d.CheckedItems = this.resources;
                        d.Recursive = true;
                        if ( d.ShowDialog( this.Context.HostWindow ) != DialogResult.OK )
                            return false;
                        recursive = d.Recursive;
                        this.resources = d.CheckedItems;
                        this.revision = d.Revision;
                    }
                }

                // the user hasn't cancelled the update
                return true;
            }

            /// <summary>
            /// The actual updating happens here.
            /// </summary>
            protected override void DoRun()
            {   
                foreach( SvnItem item in this.resources )
                {
                    Debug.WriteLine( "Updating " + item.Path, "Ankh" );
                    this.Context.Client.Update( item.Path, revision, recursive );                    
                }
            }

            public void VisitProject(Ankh.Solution.ProjectNode node)
            {
                // some project types dont necessarily have a project folder
                SvnItem folder = 
                    this.Context.StatusCache[node.Directory];
                if ( folder.IsVersioned )
                    this.resources.Add( folder );
            }           

            public void VisitProjectItem(Ankh.Solution.ProjectItemNode node)
            {
                node.GetResources( this.resources, false, 
                    new ResourceFilterCallback(CommandBase.VersionedFilter) );
            } 

            public void VisitSolutionNode(Ankh.Solution.SolutionNode node)
            {
                string solutionPath = ";"; // illegal in a path
                SvnItem folder = 
                    this.Context.StatusCache[node.Directory];
                if ( folder.IsVersioned )
                {
                    this.resources.Add( folder );
                    solutionPath = folder.Path;
                }

                // update all projects whose folder is not under the solution root
                foreach( Ankh.Solution.TreeNode n in node.Children )
                {
                    ProjectNode pNode = n as ProjectNode;

                    if ( pNode != null )
                    {
                        SvnItem folder2 = 
                            this.Context.StatusCache[pNode.Directory];
                        if ( !folder2.IsVersioned ||
                            folder2.Path.IndexOf( 
                            solutionPath ) != 0 )
                        {
                            pNode.Accept( this );
                        }
                    }
                }
            }

            private IList resources = new ArrayList();
            private Revision revision;
            private bool recursive;
        }            
        #endregion
    }
}



