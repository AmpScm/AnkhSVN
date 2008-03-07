using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using System.IO;
using System.Collections;


using System.Reflection;
using System.Text;
using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a solution to a Subversion repository
    /// by checking out the repository directory to the solution directory, then
    /// recursively adding all solution items.
    /// </summary>
    [VSNetCommand(AnkhCommand.AddSolutionToRepository,
		"AddSolutionToRepository",
         Text = "Add Sol&ution to Subversion repository...",
         Tooltip = "Add this solution to a Subversion repository.",
         Bitmap = ResourceBitmaps.AddSolutionToRepository),
         VSNetControl( "Solution." + VSNetControlAttribute.AnkhSubMenu, Position = 1 ),
         VSNetControl( "File", Position = 14 )]
    public class AddSolutionToRepositoryCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Context.AnkhLoadedForSolution ||
                !File.Exists(e.Context.DTE.Solution.FullName))
            {
                e.Enabled = false;
            }
            else if (!e.Context.SolutionIsOpen)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            SaveAllDirtyDocuments( context );

            string url;

            IList vsProjects = this.GetVSProjects( context );
            ArrayList notUnderSolutionRoot = CheckForProjectsNotUnderTheSolutionRoot(vsProjects);
            
            // any projects not under the solution root, allow the user to bail out now
            if ( notUnderSolutionRoot.Count > 0)
            {
                string[] projectNames = (string[])notUnderSolutionRoot.ToArray(typeof(string));
                string projectNamesString = this.FormatProjectNames( projectNames );
                if ( context.UIShell.ShowMessageBox("The following project(s) are not under the solution root and\r\n" + 
                    "will not be imported into the repository if you choose to continue.\r\n\r\n" + 
                    projectNamesString + "\r\n" +
                    "Do you want to continue anyway?\r\n", 
                    "Project(s) not under the solution root.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) ==
                    DialogResult.No)
                {
                    return;
                }
            }

            // user wants to go on anyway.

            using( AddSolutionDialog dlg = new AddSolutionDialog() )
            {
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                url = dlg.BaseUrl;

                // do we need to create a new repository directory?
                if ( dlg.CreateSubDirectory )
                {
                    context.StartOperation( "Creating repository directory" );
                    try
                    {
                        url = UriUtils.Combine(url, dlg.SubDirectoryName);
                        MakeDirWorker makeDirWorker = new MakeDirWorker(url,
                                   dlg.LogMessage, context);
                        {
                            context.UIShell.RunWithProgressDialog(makeDirWorker,
                                "Creating directory");
                        }
                    }
                    finally
                    {
                        context.EndOperation();
                    }
                }
            }

            // now check out the repository directory into the solution dir
            context.StartOperation( "Checking out repository directory" );
            string solutionDir = Path.GetDirectoryName( 
                context.DTE.Solution.FullName );
            try
            {
                // check out the repository directory specified               
                CheckoutRunner checkoutRunner = new CheckoutRunner(  
                    solutionDir, SvnRevision.Head, new Uri(url) );
                context.UIShell.RunWithProgressDialog( checkoutRunner, "Checking out" );
            }
            finally
            {
                context.EndOperation();
            }

            // walk the tree and add all the files
            context.StartOperation( "Adding files" );
            try
            {
                // the solution dir is already a wc                
                this.paths = new ArrayList();
                this.paths.Add( solutionDir );
                    
                context.Client.Add( context.DTE.Solution.FullName, SvnDepth.Empty );
                this.paths.Add( context.DTE.Solution.FullName );
                this.AddProjects( context, vsProjects );  
            }
            catch( Exception )
            {
                // oops, bad stuff happened
                context.StartOperation( "Error: Reverting changes" );
                try
                {
                    SvnRevertArgs args = new SvnRevertArgs();
                    args.Depth = SvnDepth.Infinity;
                    context.Client.Revert(new string[] { solutionDir }, args);
                    PathUtils.RecursiveDelete(
                        Path.Combine(solutionDir, SvnClient.AdministrativeDirectoryName));
                }
                finally
                {
                    context.EndOperation();
                }
                throw;                
            }
            finally
            {
                context.EndOperation();
            }

            // now commit the added files
            CommitOperation operation = new CommitOperation( new SimpleProgressWorker( 
                        new SimpleProgressWorkerCallback(this.DoCommit)), this.paths, context );

            if ( !operation.ShowLogMessageDialog( ) )
            {
                // oops - after all this work, the user cancelled
                context.StartOperation( "Aborted - reverting" );
                try
                {
                    SvnRevertArgs args = new SvnRevertArgs();
                    args.Depth = SvnDepth.Infinity;
                    context.Client.Revert(new string[] { solutionDir }, args);
                    PathUtils.RecursiveDelete(
                        Path.Combine(solutionDir, SvnClient.AdministrativeDirectoryName));
                }
                finally
                {
                    context.EndOperation();
                }
            }
            else
            {  
                // go ahead with the commit
                context.StartOperation( "Committing added files" );
                try
                {
                    bool completed = operation.Run( "Committing" ); 

                    if ( !completed )
                        return;
                }
                finally
                {
                    context.EndOperation();
                }
                        
                // we want ankh to get enabled right away
                context.DTE.ExecuteCommand( "Ankh.ToggleAnkh", "" );

                // Make sure the URL typed gets remembered.
                RegistryUtils.CreateNewTypedUrl( url );
                    
            }
        }

        #endregion

        private string FormatProjectNames( string[] projectNames )
        {
            StringBuilder builder = new StringBuilder();
            foreach ( string name in projectNames )
            {
                builder.AppendFormat( "\u2219 {0}{1}", name, Environment.NewLine );
            }
            return builder.ToString();
        }

        private static ArrayList CheckForProjectsNotUnderTheSolutionRoot( IList vsProjects )
        {
            ArrayList notUnderSolutionRoot = new ArrayList();
            ArrayList toRemove = new ArrayList();
            foreach ( VSProject vsProject in vsProjects )
            {
                if ( !vsProject.IsUnderSolutionRoot )
                {
                    notUnderSolutionRoot.Add( vsProject.Name );
                    toRemove.Add( vsProject );
                }
            }

            // we can't import these, no matter what
            foreach(VSProject vsProject in toRemove)
            {
                vsProjects.Remove( vsProject );
            }

            return notUnderSolutionRoot;
        }

        private IList GetVSProjects(IContext context)
        {
            ArrayList list = new ArrayList();
            foreach ( Project project in Enumerators.EnumerateProjects( context.DTE ) ) 
            {
                VSProject vsProject = VSProject.FromProject( context, project );
                if ( !vsProject.IsSolutionFolder )
                {
                    list.Add( vsProject );
                }
                else
                {
                    list.AddRange( vsProject.GetSubProjects(false) );
                }
            }
            return list;
        }

        /// <summary>
        /// Adds projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="context"></param>
        private void AddProjects( IContext context, IList vsProjects )
        {
            foreach ( VSProject vsProject in vsProjects )
            {
                this.paths.AddRange( vsProject.AddProjectToSvn() ); 
            }
        }



        private void DoCommit(IContext context)
        {
            string[] paths = (string[])(new ArrayList(this.paths).ToArray(typeof(string)));
            SvnCommitArgs args = new SvnCommitArgs();
            args.Depth = SvnDepth.Empty;
            context.Client.Commit(paths, args);
        }

        /// <summary>
        /// A progress runner for creating repository directories.
        /// </summary>
        private class MakeDirWorker : IProgressWorker
        {
            public MakeDirWorker( string url, string logMessage, IContext context ) 
            {
                this.url = url;
                this.logMessage = logMessage;
                this.context = context;
            }

            public void Work(IContext context)
            {
                SvnCreateDirectoryArgs args = new SvnCreateDirectoryArgs();
                args.LogMessage = this.logMessage;
                args.MakeParents = true;
                context.Client.RemoteCreateDirectory(new Uri(this.url), args);
            }

            private IContext context;
            private string url;
            private string logMessage;
            #region IDisposable Members

            
            #endregion
        }

        private ArrayList paths;
    }
}
