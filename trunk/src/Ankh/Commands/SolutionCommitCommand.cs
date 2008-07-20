//#define TEST_ProjectCommit
using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Selection;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectCommit)]
    [Command(AnkhCommand.SolutionCommit)]
    class SolutionCommitCommand : CommandBase
    {
#if !TEST_ProjectCommit
        CommitItemCommand itemCommit = new CommitItemCommand();
#endif

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
#if !TEST_ProjectCommit
            itemCommit.OnUpdate(e);
#else
            foreach (PendingChange pc in GetChanges(e))
            {
                return;
            }
            e.Enabled = false;            
#endif
        }

        public override void OnExecute(CommandEventArgs e)
        {
#if !TEST_ProjectCommit
            itemCommit.OnExecute(e);
#else
            throw new NotImplementedException();
#endif
        }

        class ProjectListFilter
        {
            readonly HybridCollection<string> files = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly HybridCollection<string> folders = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly IProjectFileMapper _mapper;

            public ProjectListFilter(IAnkhServiceProvider context, IEnumerable<SvnProject> projects)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (projects == null)
                    throw new ArgumentNullException("projects");

                _mapper = context.GetService<IProjectFileMapper>();
                List<SvnProject> projectList = new List<SvnProject>(projects);

                files.AddRange(_mapper.GetAllFilesOf(projectList));

                foreach (SvnProject p in projectList)
                {
                    ISvnProjectInfo pi = _mapper.GetProjectInfo(p);

                    if (pi == null)
                        continue; // Ignore solution and non scc projects

                    string dir = pi.ProjectDirectory;

                    if (!string.IsNullOrEmpty(dir) && !folders.Contains(dir))
                        folders.Add(dir);
                }
            }

            public bool ShowChange(PendingChange pc)
            {
                if (files.Contains(pc.FullPath))
                    return true;
                else if (!_mapper.ContainsPath(pc.FullPath))
                {
                    foreach (string f in folders)
                    {
                        if (pc.FullPath.StartsWith(f) && f.Length <= pc.FullPath.Length || pc.FullPath[f.Length] == '\\')
                        {
                            // Path is not contained in any other project but below one of the project roots
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        IEnumerable<PendingChange> GetChanges(BaseCommandEventArgs e)
        {
            IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();
            if (e.Command == AnkhCommand.SolutionCommit)
            {
                foreach (PendingChange pc in pcm.GetAll())
                {
                    yield return pc;
                }
            }
            else
            {
                ProjectListFilter plf = new ProjectListFilter(e.Context, e.Selection.GetSelectedProjects(false));

                foreach (PendingChange pc in pcm.GetAll())
                {
                    if (plf.ShowChange(pc))
                        yield return pc;
                }
            }
        }
    }
}
