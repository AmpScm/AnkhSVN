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
    [Command(AnkhCommand.ReposExplorerLog)]
    public class LogCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            switch (e.Command)
            {
                case AnkhCommand.Log:
                case AnkhCommand.ProjectHistory:
                case AnkhCommand.SolutionHistory:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                    {
                        if (item.IsVersioned)
                            return;
                    }
                    break;
                case AnkhCommand.ReposExplorerLog:
                    int i = 0;
                    foreach (ISvnRepositoryItem item in e.Selection.GetSelection<ISvnRepositoryItem>())
                    {
                        if (item == null || item.Uri == null)
                            continue;
                        i++;
                        if (i > 1)
                            break;
                    }
                    if (i == 1)
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            


            List<string> selected = new List<string>();

            switch (e.Command)
            {
                case AnkhCommand.Log:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
                    {
                        if (i.IsVersioned)
                            selected.Add(i.FullPath);
                    }
                    LocalLog(e.Context, selected);
                    break;
                case AnkhCommand.ProjectHistory:
                case AnkhCommand.SolutionHistory:
                    if (e.Selection.IsSolutionSelected)
                    {
                        IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                        selected.Add(settings.ProjectRoot);

                        LocalLog(e.Context, selected);
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

                        LocalLog(e.Context, selected);
                    }
                    break;
                case AnkhCommand.ReposExplorerLog:
                    ISvnRepositoryItem item = null;
                    foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
                    {
                        if(i!= null && i.Uri != null)
                            item = i;
                        break;
                    }

                    if (item != null)
                        RemoteLog(e.Context, item.Uri);
                    break;
            }
        }

        static void LocalLog(IAnkhServiceProvider context, ICollection<string> targets)
        {
            IAnkhPackage package = context.GetService<IAnkhPackage>();

            package.ShowToolWindow(AnkhToolWindow.Log);
            LogToolWindowControl logToolControl = context.GetService<LogToolWindowControl>();
            logToolControl.StartLocalLog(context, targets);
        }

        static void RemoteLog(IAnkhServiceProvider context, Uri target)
        {
            IAnkhPackage package = context.GetService<IAnkhPackage>();

            package.ShowToolWindow(AnkhToolWindow.Log);
            LogToolWindowControl logToolControl = context.GetService<LogToolWindowControl>();
            logToolControl.StartRemoteLog(context, target); // TODO: revision support
        }
    }
}
