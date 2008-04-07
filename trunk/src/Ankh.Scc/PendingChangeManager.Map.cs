﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;

namespace Ankh.Scc
{
    partial class PendingChangeManager
    {
        readonly PendingChangeCollection _pendingChanges = new PendingChangeCollection();
        readonly Dictionary<string, string> _extraFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        void InnerRefresh()
        {
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
                    if (pc.Refresh(item, isDirty) && !wasClean)
                    {
                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);

                            // No need to check wasClean or external files; not possible in this case
                            OnRemoved(new PendingChangeEventArgs(this, pc));

                            //continue;
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(item, isDirty, out pc))
                {
                    _pendingChanges.Add(pc);
                    if (!wasClean)
                        OnAdded(new PendingChangeEventArgs(this, pc));
                }
                else
                    continue;

                mapped.Add(pc, pc);
            }

            foreach (string file in _extraFiles.Keys)
            {
                SvnItem item = cache[file];

                if (item == null)
                    continue;

                bool isDirty = !item.IsVersioned || Tracker.IsDocumentDirty(file);

                PendingChange pc;
                if (_pendingChanges.TryGetValue(file, out pc))
                {
                    if (pc.Refresh(item, isDirty) && !wasClean)
                    {
                        if (pc.IsClean)
                        {
                            _pendingChanges.Remove(file);

                            // No need to check wasClean or external files; not possible in this case
                            OnRemoved(new PendingChangeEventArgs(this, pc));

                            continue;
                        }
                        else if (!wasClean)
                            OnChanged(new PendingChangeEventArgs(this, pc));
                    }
                }
                else if (PendingChange.CreateIfPending(item, isDirty, out pc))
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
                OnInitialUpdate(new PendingChangeEventArgs(this, null));
        }

        public event EventHandler<PendingChangeEventArgs> InitialUpdate;

        private void OnInitialUpdate(PendingChangeEventArgs e)
        {
            if (InitialUpdate != null)
                InitialUpdate(this, e);
        }
    }
}
