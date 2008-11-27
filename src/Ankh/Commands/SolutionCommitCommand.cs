using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI.SccManagement;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectCommit)]
    [Command(AnkhCommand.SolutionCommit)]
    class SolutionCommitCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (PendingChange pc in GetChanges(e))
            {
                return;
            }
            e.Enabled = false;            
        }

        public override void OnExecute(CommandEventArgs e)
        {
            using(ProjectCommitDialog dlg = new ProjectCommitDialog())
            {
                dlg.Context = e.Context;

                dlg.LoadChanges(GetChanges(e));

                if (dlg.ShowDialog(e.Context) == DialogResult.OK)
                {
                    PendingChangeCommitArgs pca = new PendingChangeCommitArgs();
                    pca.StoreMessageOnError = true;
                    // TODO: Commit it!
                    List<PendingChange> toCommit = new List<PendingChange>(dlg.GetSelection());
                    dlg.FillArgs(pca);

                    e.GetService<IPendingChangeHandler>().Commit(toCommit, pca);
                }
            }
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
                        if (pc.IsBelowPath(f))
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
