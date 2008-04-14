using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Ankh.VS;
using Ankh.Selection;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.Commands
{
    [Command(AnkhCommand.SolutionUpdateHead)]
    //[Command(AnkhCommand.SolutionUpdateSpecific)]
    [Command(AnkhCommand.ProjectUpdateHead)]    
    //[Command(AnkhCommand.ProjectUpdateSpecific)]
    class SolutionUpdateCommand : CommandBase
    {

        bool IsSolutionCommand(AnkhCommand command)
        {
            switch(command)
            {
                case AnkhCommand.SolutionUpdateHead:
                case AnkhCommand.SolutionUpdateSpecific:
                    return true;
                default:
                    return false;
            }            
        }

        bool IsHeadCommand(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.SolutionUpdateHead:
                case AnkhCommand.ProjectUpdateHead:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (IsSolutionCommand(e.Command))
            {
                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                if (settings == null || string.IsNullOrEmpty(settings.SolutionFilename) || string.IsNullOrEmpty(settings.ProjectRootWithSeparator))
                {
                    e.Enabled = false;
                }
            }
            else
            {
                foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                {
                    return; // We have a project
                }

                foreach (SvnProject project in e.Selection.GetOwnerProjects(false))
                {
                    return; // We have a project
                }

                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnRevision rev;

            if (IsHeadCommand(e.Command))
                rev = SvnRevision.Head;
            else
                rev = null;

            List<string> paths = new List<string>();

            if (IsSolutionCommand(e.Command))
            {
                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                if (settings == null)
                    return;
                else if (string.IsNullOrEmpty(settings.ProjectRootWithSeparator))
                    return;

                paths.Add(settings.ProjectRootWithSeparator);                
            }
            else
            {
                IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

                if (mapper == null)
                    return;

                List<SvnProject> projects = new List<SvnProject>();

                foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                {
                    if(!projects.Contains(project))
                        projects.Add(project);
                }

                if(projects.Count == 0)
                    foreach(SvnProject project in e.Selection.GetOwnerProjects(false))
                    {
                        if(!projects.Contains(project))
                            projects.Add(project);
                    }

                foreach (SvnProject project in projects)
                {
                    ISvnProjectInfo info = mapper.GetProjectInfo(project);

                    if (info != null && !string.IsNullOrEmpty(info.ProjectDirectory))
                    {
                        string path = info.ProjectDirectory;

                        if (!paths.Contains(path))
                            paths.Add(path);
                    }
                }
            }

            if (rev == null)
                rev = SvnRevision.Head;

            IContext context = e.GetService<IContext>();

            e.GetService<IProgressRunner>().Run(
                string.Format("Updating {0}", IsSolutionCommand(e.Command) ? "Solution" : "Project"),
                new UpdateRunner(paths, rev).Work);
        }

        class UpdateRunner : IProgressWorker
        {
            SvnRevision _rev;
            List<string> _paths;

            public UpdateRunner(List<string> paths, SvnRevision rev)
            {
                if (paths == null)
                    throw new ArgumentNullException("paths");
                else if (rev == null)
                    throw new ArgumentNullException("rev");

                _paths = paths;
                _rev = rev;
            }

            #region IProgressWorker Members

            public void Work(object sender, ProgressWorkerArgs e)
            {
                SvnUpdateArgs ua = new SvnUpdateArgs();
                ua.Revision = _rev;
                ua.ThrowOnError = false;

                while (_paths.Count > 0)
                {
                    List<string> now = new List<string>();

                    now.Add(_paths[0]);
                    _paths.RemoveAt(0);

                    if (_paths.Count > 0)
                    {
                        // Find all other paths with the same guid and root
                        Guid reposGuid = Guid.Empty;
                        Uri reposRoot = null;
                        SvnInfoArgs ia = new SvnInfoArgs();
                        ia.ThrowOnError = false;

                        e.Client.Info(new SvnPathTarget(now[0]), ia,
                            delegate(object s, SvnInfoEventArgs ee)
                            {
                                reposGuid = ee.RepositoryId;
                                reposRoot = ee.RepositoryRoot;
                            });


                        if (ia.IsLastInvocationCanceled)
                            return;


                        for (int i = 0; i < _paths.Count; i++)
                        {
                            e.Client.Info(new SvnPathTarget(_paths[i]), ia,
                            delegate(object s, SvnInfoEventArgs ee)
                            {
                                if (reposGuid == ee.RepositoryId &&
                                    reposRoot == ee.RepositoryRoot)
                                {
                                    now.Add(_paths[i]);
                                    _paths.RemoveAt(i);
                                    i--;
                                }
                            });

                            if (ia.IsLastInvocationCanceled)
                                return;
                        }
                    }

                    if (!e.Client.Update(now, ua) && ua.LastException != null)
                    {
                        e.Exception = ua.LastException;
                        return;
                    }

                    if(ua.IsLastInvocationCanceled)
                        return;
                }
            }
            #endregion
        }
    }
}
