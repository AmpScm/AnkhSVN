using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;
using System.IO;
using System.Diagnostics;
using SharpSvn;

namespace Ankh.Scc
{
    partial class PendingChangeManager
    {
        readonly PendingChangeCollection _pendingChanges = new PendingChangeCollection();
        readonly HybridCollection<string> _extraFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        IFileIconMapper _iconMap;
        IFileStatusCache _cache;
        IProjectFileMapper _mapper;
        IAnkhOpenDocumentTracker _tracker;

        protected IFileIconMapper IconMap
        {
            get { return _iconMap ?? (_iconMap = GetService<IFileIconMapper>()); }
        }

        protected IFileStatusCache Cache
        {
            get { return _cache ?? (_cache = GetService<IFileStatusCache>()); }
        }

        protected IProjectFileMapper Mapper
        {
            get { return _mapper ?? (_mapper = GetService<IProjectFileMapper>()); }
        }

        protected IAnkhOpenDocumentTracker Tracker
        {
            get { return _tracker ?? (_tracker = GetService<IAnkhOpenDocumentTracker>()); }
        }

        public IEnumerable<PendingChange> GetAll()
        {
            return new List<PendingChange>(_pendingChanges);
        }

        PendingChange.RefreshContext _refreshContext;
        protected PendingChange.RefreshContext RefreshContext
        {
            get { return _refreshContext ?? (_refreshContext = new PendingChange.RefreshContext(Context)); }
        }

        void InnerRefresh()
        {
            PendingChangeEventArgs pceMe = new PendingChangeEventArgs(this, null);

            OnRefreshStarted(pceMe);

            bool wasClean = (_pendingChanges.Count == 0);
            Dictionary<PendingChange, PendingChange> mapped = new Dictionary<PendingChange, PendingChange>();

            IFileStatusCache cache = Cache;

            foreach (string file in Mapper.GetAllFilesOfAllProjects())
            {
                _extraFiles.Remove(file); // If we find it here; it is no longer 'extra'!

                SvnItem item = cache[file];

                if (item == null)
                    continue;

                PendingChange pc;
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    if (pc.Refresh(RefreshContext, item) && !wasClean)
                    {
                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);
                            _extraFiles.Remove(file);

                            // No need to check wasClean or external files; not possible in this case
                            OnRemoved(new PendingChangeEventArgs(this, pc));
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(RefreshContext, item, out pc))
                {
                    _pendingChanges.Add(pc);
                    if (!wasClean)
                        OnAdded(new PendingChangeEventArgs(this, pc));
                }
                else
                    continue;

                mapped.Add(pc, pc);
            }

            foreach (string file in new List<string>(_extraFiles))
            {
                SvnItem item = cache[file];

                if (item == null)
                {
                    _extraFiles.Remove(file);
                    continue;
                }

                PendingChange pc;
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    if (pc.Refresh(RefreshContext, item) && !wasClean)
                    {
                        Debug.Assert(pc.IsClean == !PendingChange.IsPending(item));

                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);
                            _extraFiles.Remove(file);

                            // No need to check wasClean
                            OnRemoved(new PendingChangeEventArgs(this, pc));

                            continue;
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(RefreshContext, item, out pc))
                {
                    _pendingChanges.Add(pc);
                    if (!wasClean)
                        OnAdded(new PendingChangeEventArgs(this, pc));
                }
                else
                    continue;

                // All collisions where just removed in the loop above
                mapped.Add(pc, pc);
            }

            for (int i = 0; i < _pendingChanges.Count; i++)
            {
                PendingChange pc = _pendingChanges[i];

                if (mapped.ContainsKey(pc))
                    continue;

                _pendingChanges.RemoveAt(i--);
                if (!wasClean)
                {
                    OnRemoved(new PendingChangeEventArgs(this, pc));
                }
            }

            if (wasClean && _pendingChanges.Count > 0)
                OnInitialUpdate(pceMe);

            OnRefreshCompleted(pceMe);
        }

        private void ItemRefresh(string file)
        {
            SvnItem item = Cache[file];

            bool inProject = item.InSolution;
            bool inExtra = _extraFiles.Contains(file);
            PendingChange pc;

            if (!inProject && !inExtra)
            {
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    _pendingChanges.Remove(file);
                    OnRemoved(new PendingChangeEventArgs(this, pc));
                }

                return;
            }
            else if (inProject && inExtra)
                _extraFiles.Remove(file);

            if (item == null)
                return;

            if (_pendingChanges.TryGetValue(file, out pc))
            {
                if (pc.Refresh(RefreshContext, item))
                {
                    if (pc.IsClean)
                    {
                        _pendingChanges.Remove(file);
                        _extraFiles.Remove(file);

                        // No need to check wasClean or external files; not possible in this case
                        OnRemoved(new PendingChangeEventArgs(this, pc));
                    }
                    else
                        OnChanged(new PendingChangeEventArgs(this, pc));
                }
            }
            else if (PendingChange.CreateIfPending(RefreshContext, item, out pc))
            {
                _pendingChanges.Add(pc);

                OnAdded(new PendingChangeEventArgs(this, pc));
            }
        }

        public event EventHandler<PendingChangeEventArgs> InitialUpdate;

        private void OnInitialUpdate(PendingChangeEventArgs e)
        {
            if (InitialUpdate != null)
                InitialUpdate(this, e);
        }

        public event EventHandler<PendingChangeEventArgs> RefreshStarted;

        protected void OnRefreshStarted(PendingChangeEventArgs e)
        {
            if (RefreshStarted != null)
                RefreshStarted(this, e);
        }

        public event EventHandler<PendingChangeEventArgs> RefreshCompleted;

        protected void OnRefreshCompleted(PendingChangeEventArgs e)
        {
            if (RefreshCompleted != null)
                RefreshCompleted(this, e);
        }

        /// <summary>
        /// Tries to get a matching file from the specified text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        /// <remarks>Called from the log message editor in an attempt to provide a mouse over</remarks>
        public bool TryMatchFile(string text, out PendingChange change)
        {
            change = null;
            if (string.IsNullOrEmpty(text))
                return false;

            lock (_toRefresh)
            {
                text = text.Replace(Path.DirectorySeparatorChar, '/');
                foreach (PendingChange pc in _pendingChanges)
                {
                    if (pc.RelativePath == text)
                    {
                        change = pc;
                        return true;
                    }
                }

                int liSlash = text.LastIndexOf('/');
                if (liSlash > 0)
                {
                    text = text.Substring(liSlash + 1);
                }

                foreach (PendingChange pc in _pendingChanges)
                {
                    if (text == pc.Name || text == Path.GetFileNameWithoutExtension(pc.Name))
                    {
                        change = pc;
                        return true;
                    }

                }

                return false;
            }
        }

        public IEnumerable<PendingChange> GetAllBelow(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            foreach (PendingChange pc in _pendingChanges)
            {
                if (pc.FullPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                    && path.Length == pc.FullPath.Length || (pc.FullPath.Length > path.Length && pc.FullPath[path.Length] == Path.DirectorySeparatorChar))
                {
                    // SR: Added pc.FullPath.Length > path.Length to avoid index out of bounds exception
                    yield return pc;
                }
            }
        }

        public bool ContainsPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            return _pendingChanges.Contains(path);
        }
    }
}
