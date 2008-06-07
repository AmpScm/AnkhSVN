// $Id$
using System;

using System.Diagnostics;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Collections;
using SharpSvn;
using System.Collections.ObjectModel;
using Ankh.Ids;
using System.Collections.Generic;
using Ankh.UI;
using Ankh.UI.SvnLog;
using Ankh.Selection;
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the change log for the selected item.
    /// </summary>
    [Command(AnkhCommand.Log)]
    [Command(AnkhCommand.ProjectHistory)]
    [Command(AnkhCommand.SolutionHistory)]
    public class LogCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            IAnkhPackage package = e.Context.GetService<IAnkhPackage>();


            List<string> selected = new List<string>();

            switch (e.Command)
            {
                case AnkhCommand.Log:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
                    {
                        if (i.IsVersioned)
                            selected.Add(i.FullPath);
                    }
                    break;
                case AnkhCommand.ProjectHistory:
                case AnkhCommand.SolutionHistory:
                    if (e.Selection.IsSolutionSelected)
                    {
                        IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                        selected.Add(settings.ProjectRoot);
                    }
                    else
                    {
                        IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                        foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                        {
                            ISvnProjectInfo info = mapper.GetProjectInfo(p);

                            if (info != null)
                                selected.Add(info.ProjectDirectory);
                        }
                    }
                    break;
            }

            package.ShowToolWindow(AnkhToolWindow.Log);
            LogToolWindowControl logToolControl = e.Context.GetService<LogToolWindowControl>();
            logToolControl.Start(selected);
        }
    }
}
