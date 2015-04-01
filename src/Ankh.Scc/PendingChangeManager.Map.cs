// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
        readonly HybridCollection<string> _extraFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        IFileIconMapper _iconMap;
        ISvnStatusCache _cache;
        IProjectFileMapper _mapper;
        IAnkhOpenDocumentTracker _tracker;

        protected IFileIconMapper IconMap
        {
            get { return _iconMap ?? (_iconMap = GetService<IFileIconMapper>()); }
        }

        protected ISvnStatusCache Cache
        {
            get { return _cache ?? (_cache = GetService<ISvnStatusCache>()); }
        }

        protected IProjectFileMapper Mapper
        {
            get { return _mapper ?? (_mapper = GetService<IProjectFileMapper>()); }
        }

        protected IAnkhOpenDocumentTracker Tracker
        {
            get { return _tracker ?? (_tracker = GetService<IAnkhOpenDocumentTracker>()); }
        }

        PendingChange.RefreshContext _refreshContext;
        protected PendingChange.RefreshContext RefreshContext
        {
            get { return _refreshContext ?? (_refreshContext = new PendingChange.RefreshContext(Context)); }
        }

        void InnerRefresh()
        {
            using (BatchStartedEventArgs br = BatchRefresh())
            {
                Dictionary<string, PendingChange> mapped = new Dictionary<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

                ISvnStatusCache cache = Cache;

                foreach (string file in Mapper.GetAllFilesOfAllProjects())
                {
                    br.Tick();
                    _extraFiles.Remove(file); // If we find it here; it is no longer 'extra'!

                    SvnItem item = cache[file];

                    if (item == null)
                        continue;

                    PendingChange pc = UpdatePendingChange(item);

                    if (pc != null)
                        mapped[pc.FullPath] = pc;
                }

                foreach (string file in new List<string>(_extraFiles))
                {
                    br.Tick();
                    SvnItem item = cache[file];

                    if (item == null)
                    {
                        _extraFiles.Remove(file);
                        continue;
                    }

                    PendingChange pc = UpdatePendingChange(item);

                    if (pc != null)
                        mapped[pc.FullPath] = pc;
                }

                for (int i = 0; i < _pendingChanges.Count; i++)
                {
                    br.Tick();
                    PendingChange pc = _pendingChanges[i];

                    if (mapped.ContainsKey(pc.FullPath))
                        continue;

                    _pendingChanges.RemoveAt(i--);
                }
            }
        }

        private PendingChange UpdatePendingChange(SvnItem item)
        {
            PendingChange pc;
            string file = item.FullPath;
            if (_pendingChanges.TryGetValue(file, out pc))
            {
                if (pc.Refresh(RefreshContext, item))
                {
                    if (pc.IsClean)
                    {
                        _pendingChanges.Remove(file);
                        _extraFiles.Remove(file);
                        pc = null;
                    }
                    else
                        OnChanged(new PendingChangeEventArgs(this, pc));
                }
            }
            else if (PendingChange.CreateIfPending(RefreshContext, item, out pc))
            {
                Debug.Assert(!_pendingChanges.Contains(pc), "Insane race condition triggered");

                if (!_pendingChanges.Contains(pc))
                    _pendingChanges.Add(pc);
            }
            
            return pc;
        }

        private void ItemRefresh(string file)
        {
            SvnItem item = Cache[file];
            file = item.FullPath; // Use existing normalization

            bool inProject = item.InSolution;
            bool inExtra = _extraFiles.Contains(file);
            PendingChange pc;

            if (!inProject && !inExtra)
            {
                _pendingChanges.Remove(file);

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
                    }
                    else
                        OnChanged(new PendingChangeEventArgs(this, pc));
                }
            }
            else if (PendingChange.CreateIfPending(RefreshContext, item, out pc))
            {
                _pendingChanges.Add(pc);
            }
        }

        public event EventHandler<PendingChangeEventArgs> InitialUpdate;

        private void OnInitialUpdate(PendingChangeEventArgs e)
        {
            if (InitialUpdate != null)
                InitialUpdate(this, e);
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
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            foreach (PendingChange pc in _pendingChanges)
            {
                if (pc.IsBelowPath(path))
                {
                    yield return pc;
                }
            }
        }

        public bool Contains(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            return _pendingChanges.Contains(path);
        }

        const string IgnoreOnCommit = "ignore-on-commit";
        public IEnumerable<string> GetSuggestedChangeLists()
        {
            Dictionary<string, string> usedNames = new Dictionary<string, string>();

            foreach (PendingChange pc in new List<PendingChange>(_pendingChanges))
            {
                string cl = pc.SvnItem.Status.ChangeList;

                if (!string.IsNullOrEmpty(cl) && !string.Equals(cl, IgnoreOnCommit))
                {
                    if (usedNames.ContainsKey(cl))
                        continue;

                    usedNames.Add(cl, cl);

                    yield return cl;
                }
            }
        }
    }
}
