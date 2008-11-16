using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Selection;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.Commands;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.SolutionUpdateHead)]
    [Command(AnkhCommand.SolutionUpdateSpecific)]
    [Command(AnkhCommand.ProjectUpdateHead)]
    [Command(AnkhCommand.ProjectUpdateSpecific)]
    class SolutionUpdateCommand : CommandBase
    {
        bool IsSolutionCommand(AnkhCommand command)
        {
            switch (command)
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

        IEnumerable<SvnProject> GetSelectedProjects(BaseCommandEventArgs e)
        {
            bool foundOne = false;
            foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
            {
                foundOne = true;
                yield return p;
            }

            if (foundOne)
                yield break;

            foreach (SvnProject p in e.Selection.GetOwnerProjects(false))
            {
                yield return p;
            }
        }

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (IsSolutionCommand(e.Command))
            {
                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
                if (settings == null || string.IsNullOrEmpty(settings.ProjectRoot))
                {
                    e.Enabled = false;
                    return;
                }

                if (!settings.ProjectRootSvnItem.IsVersioned)
                    e.Enabled = false;
            }
            else
            {
                IProjectFileMapper pfm = null;
                IFileStatusCache fsc = null;

                Uri rootUrl = null;
                foreach (SvnProject p in GetSelectedProjects(e))
                {                    
                    if(pfm == null)
                        pfm = e.GetService<IProjectFileMapper>();

                    ISvnProjectInfo pi = pfm.GetProjectInfo(p);

                    if(pi == null || pi.ProjectDirectory == null)
                        continue;                    

                    if(fsc == null)
                        fsc = e.GetService<IFileStatusCache>();

                    SvnItem rootItem = fsc[pi.ProjectDirectory];

                    if(!rootItem.IsVersioned)
                        continue;

                    if (IsHeadCommand(e.Command))
                        return; // Ok, we can update

                    if(rootUrl == null)
                        rootUrl = rootItem.WorkingCopy.RepositoryRoot;
                    else if(rootUrl != rootItem.WorkingCopy.RepositoryRoot)
                    {
                        // Multiple repositories selected; can't choose uniform version
                        e.Enabled = false; 
                        return;
                    }
                }

                if(rootUrl == null)
                    e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            ILastChangeInfo ci = e.GetService<ILastChangeInfo>();

            if (ci != null)
                ci.SetLastChange(null, null);

            SvnRevision rev;
            bool allowUnversionedObstructions = false;
            bool updateExternals = true;

            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

            if (IsHeadCommand(e.Command) || e.DontPrompt)
                rev = SvnRevision.Head;
            else if (IsSolutionCommand(e.Command))
            {
                SvnItem projectItem = settings.ProjectRootSvnItem;

                Debug.Assert(projectItem != null, "Has item");

                using (UpdateDialog ud = new UpdateDialog())
                {
                    ud.ItemToUpdate = projectItem;
                    ud.Revision = SvnRevision.Head;

                    if (ud.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    rev = ud.Revision;
                    allowUnversionedObstructions = ud.AllowUnversionedObstructions;
                    updateExternals = ud.UpdateExternals;
                }
            }
            else
            {
                // We checked there was only a single repository to select a revision 
                // from in OnUpdate, so we can suffice with only calculate the path

                SvnItem si = null;
                SvnOrigin origin = null;
                foreach (SvnProject p in GetSelectedProjects(e))
                {
                    ISvnProjectInfo pi = mapper.GetProjectInfo(p);
                    if (pi == null || pi.ProjectDirectory == null)
                        continue;

                    SvnItem item = cache[pi.ProjectDirectory];
                    if (!item.IsVersioned)
                        continue;

                    if (si == null && origin == null)
                    {
                        si = item;
                        origin = new SvnOrigin(item);
                    }
                    else
                    {
                        si = null;
                        string urlPath1 = origin.Uri.AbsolutePath;
                        string urlPath2 = item.Status.Uri.AbsolutePath;

                        int i = 0;
                        while (i < urlPath1.Length && i < urlPath2.Length
                            && urlPath1[i] == urlPath2[i])
                        {
                            i++;
                        }

                        while (i > 0 && urlPath1[i-1] != '/')
                            i--;

                        origin = new SvnOrigin(new Uri(origin.Uri, urlPath1.Substring(0, i)), origin.RepositoryRoot);
                    }
                }

                Debug.Assert(origin != null);

                using (UpdateDialog ud = new UpdateDialog())
                {
                    if (si != null)
                        ud.ItemToUpdate = si;
                    else
                    {
                        ud.SvnOrigin = origin;
                        ud.SetMultiple(true);
                    }

                    ud.Revision = SvnRevision.Head;

                    if (ud.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    rev = ud.Revision;
                    allowUnversionedObstructions = ud.AllowUnversionedObstructions;
                    updateExternals = ud.UpdateExternals;
                }
            }

            List<string> paths = new List<string>();

            if (IsSolutionCommand(e.Command))
            {

                if (settings == null)
                    return;
                else if (string.IsNullOrEmpty(settings.ProjectRoot))
                    return;

                paths.Add(settings.ProjectRoot);
            }
            else
            {
                

                if (mapper == null)
                    return;

                List<SvnProject> projects = new List<SvnProject>();

                foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                {
                    if (!projects.Contains(project))
                        projects.Add(project);
                }

                if (projects.Count == 0)
                    foreach (SvnProject project in e.Selection.GetOwnerProjects(false))
                    {
                        if (!projects.Contains(project))
                            projects.Add(project);
                    }

                foreach (SvnProject project in projects)
                {
                    ISvnProjectInfo info = mapper.GetProjectInfo(project);

                    if (info != null && !string.IsNullOrEmpty(info.ProjectDirectory))
                    {
                        string path = info.ProjectDirectory;

                        if (string.IsNullOrEmpty(path))
                            continue;

                        if (!paths.Contains(path))
                            paths.Add(path);
                    }
                }
            }

            if (rev == null)
                rev = SvnRevision.Head;

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();
            foreach (string path in paths)
            {
                foreach (string file in documentTracker.GetDocumentsBelow(path))
                {
                    if (!lockPaths.Contains(file))
                        lockPaths.Add(file);
                }
            }

            documentTracker.SaveDocuments(lockPaths); // Make sure all files are saved before merging!

            using (DocumentLock lck = documentTracker.LockDocuments(lockPaths, DocumentLockType.NoReload))
            {
                lck.MonitorChanges();

                // TODO: Monitor conflicts!!

                UpdateRunner ur = new UpdateRunner(paths, rev, updateExternals, allowUnversionedObstructions);

                e.GetService<IProgressRunner>().Run(
                    string.Format("Updating {0}", IsSolutionCommand(e.Command) ? "Solution" : "Project"),
                    ur.Work);

                if (ci != null && ur.LastResult != null && IsSolutionCommand(e.Command))
                {
                    ci.SetLastChange("Updated to:", ur.LastResult.Revision.ToString());
                }


                lck.ReloadModified();
            }
        }

        class UpdateRunner
        {
            SvnRevision _rev;
            List<string> _paths;
            SvnUpdateResult _result;
            bool _updateExternals;
            bool _allowUnversionedObstructions;

            public UpdateRunner(List<string> paths, SvnRevision rev, bool updateExternals, bool allowUnversionedObstructions)
            {
                if (paths == null)
                    throw new ArgumentNullException("paths");
                else if (rev == null)
                    throw new ArgumentNullException("rev");

                _paths = paths;
                _rev = rev;
                _updateExternals = updateExternals;
                _allowUnversionedObstructions = allowUnversionedObstructions;
            }

            public SvnUpdateResult LastResult
            {
                get { return _result; }
            }

            #region IProgressWorker Members

            public void Work(object sender, ProgressWorkerArgs e)
            {
                SvnUpdateArgs ua = new SvnUpdateArgs();
                ua.Revision = _rev;
                ua.ThrowOnError = false;
                ua.AllowObstructions = _allowUnversionedObstructions;
                ua.IgnoreExternals = !_updateExternals;
                e.Context.GetService<IConflictHandler>().RegisterConflictHandler(ua, e.Synchronizer);

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

                    if (!e.Client.Update(now, ua, out _result) && ua.LastException != null)
                    {
                        e.Exception = ua.LastException;
                        return;
                    }

                    if (ua.IsLastInvocationCanceled)
                        return;
                }
            }
            #endregion
        }
    }
}
