// $Id$
using System;
using EnvDTE;

using Ankh.UI;
using System.Collections;
using System.Diagnostics;
using Ankh.Solution;
using NSvn.Core;
using System.Windows.Forms;

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
                context.StartOperation( "Updating" );

                if ( this.Shift )
                {
                    IList resources = context.SolutionExplorer.GetSelectionResources( true,
                        new ResourceFilterCallback(CommandBase.VersionedFilter) );
                
                    this.form = new Form();
                    this.tree = new PathSelectionTreeView();
                    form.Controls.Add( tree );
                    tree.Dock = DockStyle.Top;
                    tree.Height = 400;
                    tree.GetPathInfo +=new GetPathInfoDelegate(tree_GetPathInfo);
                    tree.UrlPaths = false;
                    tree.Items = resources;

                    this.check = new CheckBox();
                    this.check.Text = "Recursive";
                    form.Controls.Add( check );
                    form.Height = 500;
                    check.Top = 420;
                    check.CheckedChanged += new EventHandler(check_CheckedChanged);

                    if ( form.ShowDialog() != DialogResult.Ignore )
                    {
                        MessageBox.Show( String.Join( "\r\n", SvnItem.GetPaths( tree.CheckedItems ) ) );
                        return;
                    }
                }
                else if ( !this.Shift )
                    return;
                    

                // save all files
                context.DTE.Documents.SaveAll();

                // we assume by now that all items are working copy resources.

                // run this on another thread
                UpdateRunner visitor = new UpdateRunner( context);
                context.SolutionExplorer.VisitSelectedNodes( visitor );
                visitor.Start( "Updating" );
            }
            finally
            {
                context.EndOperation();
            }
        }    

        private CheckBox check;
        private Form form;
        private PathSelectionTreeView tree;
        #endregion

        #region UpdateVisitor
        private class UpdateRunner : ProgressRunner, INodeVisitor
        {
            public UpdateRunner( AnkhContext context ) : base(context)
            {}

            protected override void DoRun()
            {
                foreach( SvnItem item in this.resources )
                {
                    Debug.WriteLine( "Updating " + item.Path, "Ankh" );
                    this.Context.Client.Update( item.Path, Revision.Head, true );                    
                }

                this.Context.SolutionExplorer.RefreshSelection();
            }

            public void VisitProject(Ankh.Solution.ProjectNode node)
            {
                // some project types dont necessarily have a project folder
                SvnItem folder = 
                    this.Context.SolutionExplorer.StatusCache[node.Directory];
                if ( folder != SvnItem.Unversionable )
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
                    this.Context.SolutionExplorer.StatusCache[node.Directory];
                if ( folder != SvnItem.Unversionable )
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
                            this.Context.SolutionExplorer.StatusCache[pNode.Directory];
                        if ( folder2 == SvnItem.Unversionable ||
                            folder2.Path.IndexOf( 
                            solutionPath ) != 0 )
                        {
                            pNode.Accept( this );
                        }
                    }
                }
            }

            private ArrayList resources = new ArrayList();
        }            
        #endregion

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            this.tree.Recursive = this.check.Checked;
        }

        private void tree_GetPathInfo(object sender, GetPathInfoEventArgs args)
        {
            SvnItem item = (SvnItem)args.Item;
            args.IsDirectory = item.IsDirectory;
            args.Path = item.Path;
        }
    }
}



