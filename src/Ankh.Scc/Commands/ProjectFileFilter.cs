using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Selection;

namespace Ankh.Scc.Commands
{
    [Command((AnkhCommand)AnkhCommandMenu.ProjectFileScc)]
    class ProjectFileFilter : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
            {
                ISvnProjectInfo pi = e.GetService<IProjectFileMapper>().GetProjectInfo(p);

                if (p == null || string.IsNullOrEmpty(pi.ProjectFile))
                {
                    break; // No project file
                }

                if (!string.IsNullOrEmpty(pi.ProjectDirectory) &&
                    string.Equals(pi.ProjectDirectory, pi.ProjectFile, StringComparison.OrdinalIgnoreCase))
                {
                    break; // Project file is directory
                }

                SvnItem item = e.GetService<IFileStatusCache>()[pi.ProjectFile];

                if (item != null && item.IsDirectory)
                    break; // Project file is not file

                return; // Show the menu
            }

            e.Enabled = e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            throw new InvalidOperationException(); // Never reached; not a real command
        }

        #endregion
    }
}
