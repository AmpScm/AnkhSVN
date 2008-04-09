using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;

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

            foreach(string file in Mapper.GetAllFilesOfAllProjects())
            {
                _extraFiles.Remove(file); // If we find it here; it is no longer 'extra'!

                SvnItem item = cache[file];

                if (item == null)
                    continue;

                bool isDirty = !item.IsVersioned || Tracker.IsDocumentDirty(file);

                PendingChange pc;
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    if (pc.Refresh(RefreshContext, item, isDirty) && !wasClean)
                    {
                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);

                            // No need to check wasClean or external files; not possible in this case
                            OnRemoved(new PendingChangeEventArgs(this, pc));
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(RefreshContext, item, isDirty, out pc))
                {
                    _pendingChanges.Add(pc);
                    if (!wasClean)
                        OnAdded(new PendingChangeEventArgs(this, pc));
                }
                else
                    continue;

                mapped.Add(pc, pc);
            }

            foreach (string file in _extraFiles)
            {
                SvnItem item = cache[file];

                if (item == null)
                    continue;

                bool isDirty = !item.IsVersioned || Tracker.IsDocumentDirty(file);

                PendingChange pc;
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    if (pc.Refresh(RefreshContext, item, isDirty) && !wasClean)
                    {
                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);

                            // No need to check wasClean
                            OnRemoved(new PendingChangeEventArgs(this, pc));

                            continue;
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(RefreshContext, item, isDirty, out pc))
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

            if(wasClean && _pendingChanges.Count > 0)
                OnInitialUpdate(pceMe);

            OnRefreshCompleted(pceMe);
        }

        private void ItemRefresh(string file)
        {
            bool inProject = Mapper.ContainsPath(file);
            bool inExtra = _extraFiles.Contains(file);

            if (!inProject && !inExtra)
                return;
            else if (inProject && inExtra)
                _extraFiles.Remove(file);

            SvnItem item = Cache[file];

            if (item == null)
                return;

            bool isDirty = !item.IsVersioned || Tracker.IsDocumentDirty(file);

            PendingChange pc;
            if (_pendingChanges.TryGetValue(file, out pc))
            {
                if (pc.Refresh(RefreshContext, item, isDirty))
                {
                    if (pc.IsClean)
                    {
                        _pendingChanges.Remove(file);

                        // No need to check wasClean or external files; not possible in this case
                        OnRemoved(new PendingChangeEventArgs(this, pc));
                    }
                    else
                        OnChanged(new PendingChangeEventArgs(this, pc));
                }
            }
            else if (PendingChange.CreateIfPending(RefreshContext, item, isDirty, out pc))
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
    }
}
