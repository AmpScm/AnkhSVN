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
using Ankh.Scc.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the change log for the selected item.
    /// </summary>
    [Command(AnkhCommand.Log)]
    [Command(AnkhCommand.ProjectHistory)]
    [Command(AnkhCommand.SolutionHistory)]
    [Command(AnkhCommand.ReposExplorerLog)]
    [Command(AnkhCommand.BlameShowLog)]
    class LogCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
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
                case AnkhCommand.BlameShowLog:

                    IBlameControl blameWindow = e.Selection.ActiveDialogOrFrameControl as IBlameControl;

                    if ((blameWindow == null) || !blameWindow.HasWorkingCopyItems)
                    {
                        e.Enabled = false;
                        return;
                    }

                    int j = 0;
                    foreach (IBlameSection section in e.Selection.GetSelection<IBlameSection>())
                    {
                        if (section == null)
                            continue;
                        j++;
                    }

                    if (j == 1)
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> selected = new List<SvnItem>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.Log:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
                    {
                        if (i.IsVersioned)
                            selected.Add(i);
                    }
                    LocalLog(e.Context, selected);
                    break;
                case AnkhCommand.ProjectHistory:
                case AnkhCommand.SolutionHistory:
                    if (e.Selection.IsSolutionSelected)
                    {
                        IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                        selected.Add(cache[settings.ProjectRoot]);

                        LocalLog(e.Context, selected);
                    }
                    else
                    {
                        IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                        foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                        {
                            ISvnProjectInfo info = mapper.GetProjectInfo(p);

                            if (info != null)
                                selected.Add(cache[info.ProjectDirectory]);
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
                case AnkhCommand.BlameShowLog:

                    IBlameControl blameWindow = e.Selection.ActiveDialogOrFrameControl as IBlameControl;

                    if ((blameWindow == null) || !blameWindow.HasWorkingCopyItems)
                        return;

                    SvnItem firstItem = null;
                    foreach (SvnItem i in blameWindow.WorkingCopyItems)
                        firstItem = i;

                    IBlameSection section = null;
                    foreach (IBlameSection s in e.Selection.GetSelection<IBlameSection>())
                    {
                        section = s;
                        break;
                    }
                    if (section == null)
                        return;

                    LocalLog(e.Context, new SvnItem[]{firstItem}, section.Revision, null);
                    
                    break;
            }
        }

        static void LocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets)
        {
            LocalLog(context, targets, null, null);
        }
        static void LocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets, SvnRevision start, SvnRevision end)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (targets == null)
                throw new ArgumentNullException("targets");

            IAnkhPackage package = context.GetService<IAnkhPackage>();

            package.ShowToolWindow(AnkhToolWindow.Log);

            LogToolWindowControl logToolControl = context.GetService<ISelectionContext>().ActiveFrameControl as LogToolWindowControl;
            if(logToolControl != null)
                logToolControl.StartLocalLog(context, targets, start, end);
        }

        static void RemoteLog(IAnkhServiceProvider context, Uri target)
        {
            IAnkhPackage package = context.GetService<IAnkhPackage>();

            package.ShowToolWindow(AnkhToolWindow.Log);
            LogToolWindowControl logToolControl = context.GetService<ISelectionContext>().ActiveFrameControl as LogToolWindowControl;
            if (logToolControl != null) 
                logToolControl.StartRemoteLog(context, target); // TODO: revision support
        }
    }
}
