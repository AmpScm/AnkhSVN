using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text;
using SharpSvn;
using Ankh.Ids;
using System.Collections.Generic;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a solution to a Subversion repository
    /// by checking out the repository directory to the solution directory, then
    /// recursively adding all solution items.
    /// </summary>
    [Command(AnkhCommand.AddSolutionToRepository)]
    public class AddSolutionToRepositoryCommand : CommandBase
    {
        SvnCommitArgs _args = null;
        List<SvnItem> paths;
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Selection.SolutionFilename))
                e.Enabled = false;
            else if (!File.Exists(e.Selection.SolutionFilename))
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IFileStatusCache statusCache = e.Context.GetService<IFileStatusCache>();
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments(e.Selection, context);

            using (SvnClient client = context.ClientPool.GetClient())
            {
                string url;

                IList vsProjects = this.GetVSProjects(context);
                ArrayList notUnderSolutionRoot = CheckForProjectsNotUnderTheSolutionRoot(vsProjects);

                // any projects not under the solution root, allow the user to bail out now
                if (notUnderSolutionRoot.Count > 0)
                {
                    string[] projectNames = (string[])notUnderSolutionRoot.ToArray(typeof(string));
                    string projectNamesString = this.FormatProjectNames(projectNames);
                    if (context.UIShell.ShowMessageBox("The following project(s) are not under the solution root and\r\n" +
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

                using (AddSolutionDialog dlg = new AddSolutionDialog())
                {
                    if (dlg.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                        return;

                    url = dlg.BaseUrl;

                    // do we need to create a new repository directory?
                    if (dlg.CreateSubDirectory)
                    {
                        using (context.StartOperation("Creating repository directory"))
                        {
                            url = UriUtils.Combine(url, dlg.SubDirectoryName);
                            MakeDirWorker makeDirWorker = new MakeDirWorker(url,
                                       dlg.LogMessage, context);
                            
                            e.GetService<IProgressRunner>().Run(
                                    "Creating directory",
                                    makeDirWorker.Work);
                        }
                    }
                }
                string solutionDir = Path.GetDirectoryName(
                        e.Selection.SolutionFilename);


                // now check out the repository directory into the solution dir
                using (context.StartOperation("Checking out repository directory"))
                {


                    // check out the repository directory specified               
                    CheckoutRunner checkoutRunner = new CheckoutRunner(
                        solutionDir, SvnRevision.Head, new Uri(url));

                    e.GetService<IProgressRunner>().Run(
                        "Checking out",
                        checkoutRunner.Work);
                }

                // walk the tree and add all the files
                using (context.StartOperation("Adding files"))

                    try
                    {
                        // the solution dir is already a wc                
                        this.paths.Add(statusCache[solutionDir]);

                        client.Add(e.Selection.SolutionFilename, SvnDepth.Empty);
                        this.paths.Add(statusCache[e.Selection.SolutionFilename]);
                        this.AddProjects(context, vsProjects);
                    }
                    catch (Exception)
                    {
                        // oops, bad stuff happened
                        using (context.StartOperation("Error: Reverting changes"))
                        {
                            SvnRevertArgs args = new SvnRevertArgs();
                            args.Depth = SvnDepth.Infinity;
                            client.Revert(new string[] { solutionDir }, args);
                            PathUtils.RecursiveDelete(
                                Path.Combine(solutionDir, SvnClient.AdministrativeDirectoryName));
                        }
                        throw;
                    }

                // now commit the added files
                CommitOperation operation = new CommitOperation(e.Context, this.paths, _args, DoCommit);

                if (!operation.ShowLogMessageDialog())
                {
                    // oops - after all this work, the user cancelled
                    using (context.StartOperation("Aborted - reverting"))
                    {
                        SvnRevertArgs args = new SvnRevertArgs();
                        args.Depth = SvnDepth.Infinity;
                        client.Revert(new string[] { solutionDir }, args);
                        PathUtils.RecursiveDelete(
                            Path.Combine(solutionDir, SvnClient.AdministrativeDirectoryName));
                    }
                }
                else
                {
                    // go ahead with the commit
                    using (context.StartOperation("Committing added files"))
                    {
                        bool completed = operation.Run("Committing");

                        if (!completed)
                            return;
                    }
                }
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
            /*ArrayList toRemove = new ArrayList();
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
            }*/

            return notUnderSolutionRoot;
        }

        private IList GetVSProjects(IContext context)
        {
            ArrayList list = new ArrayList();
            /*foreach ( Project project in Enumerators.EnumerateProjects( context.DTE ) ) 
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
            }*/
            return list;
        }

        /// <summary>
        /// Adds projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="context"></param>
        private void AddProjects( IContext context, IList vsProjects )
        {
            /*foreach ( VSProject vsProject in vsProjects )
            {
                this.paths.AddRange( vsProject.AddProjectToSvn() ); 
            }*/
        }



        private void DoCommit(object sender, ProgressWorkerArgs e)
        {
            SvnCommitArgs args = new SvnCommitArgs();
            args.Depth = SvnDepth.Empty;
            e.Client.Commit(SvnItem.GetPaths(paths), args);
        }

        /// <summary>
        /// A progress runner for creating repository directories.
        /// </summary>
        private class MakeDirWorker
        {
            public MakeDirWorker( string url, string logMessage, IContext context ) 
            {
                this.url = url;
                this.logMessage = logMessage;
                this.context = context;
            }

            public void Work(object sender, ProgressWorkerArgs e)
            {
                SvnCreateDirectoryArgs args = new SvnCreateDirectoryArgs();
                args.LogMessage = this.logMessage;
                args.MakeParents = true;
                e.Client.RemoteCreateDirectory(new Uri(this.url), args);
            }

            private IContext context;
            private string url;
            private string logMessage;
            #region IDisposable Members

            
            #endregion
        }

        
    }
}
