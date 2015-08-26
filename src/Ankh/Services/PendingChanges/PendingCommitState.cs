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
using SharpSvn;
using Ankh.Scc;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ankh.VS;
using Ankh.Commands;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.Services.PendingChanges
{
    sealed class PendingCommitState : AnkhService, IDisposable
    {
        readonly HybridCollection<PendingChange> _changes = new HybridCollection<PendingChange>();
        readonly HybridCollection<string> _commitPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        readonly SortedDictionary<string, string> _customProperties = new SortedDictionary<string, string>();
        SvnClient _client;
        bool _keepLocks;
        bool _keepChangeLists;
        string _logMessage;
        string _issueText;
        bool _skipIssueVerify;

        public PendingCommitState(IAnkhServiceProvider context, IEnumerable<PendingChange> changes)
            : base(context)
        {
            if (changes == null)
                throw new ArgumentNullException("changes");

            _changes.UniqueAddRange(changes);

            foreach (PendingChange pc in _changes)
            {
                if (!_commitPaths.Contains(pc.FullPath))
                    _commitPaths.Add(pc.FullPath);
            }
        }

        public SvnClient Client
        {
            get
            {
                if (_client == null)
                    _client = GetService<ISvnClientPool>().GetNoUIClient();

                return _client;
            }
        }

        public HybridCollection<PendingChange> Changes
        {
            get { return _changes; }
        }

        public HybridCollection<string> CommitPaths
        {
            get { return _commitPaths; }
        }

        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        public string IssueText
        {
            get { return _issueText; }
            set { _issueText = value; }
        }

        public bool KeepLocks
        {
            get { return _keepLocks; }
            set { _keepLocks = value; }
        }

        public bool KeepChangeLists
        {
            get { return _keepChangeLists; }
            set { _keepChangeLists = value; }
        }

        [DebuggerStepThrough]
        public new T GetService<T>()
            where T : class
        {
            return base.GetService<T>();
        }

        [DebuggerStepThrough]
        public new T GetService<T>(Type serviceType)
            where T : class
        {
            return base.GetService<T>(serviceType);
        }

        AnkhMessageBox _mb;
        public AnkhMessageBox MessageBox
        {
            get { return _mb ?? (_mb = new AnkhMessageBox(this)); }
        }

        IFileStatusCache _cache;
        public IFileStatusCache Cache
        {
            get { return _cache ?? (_cache = GetService<IFileStatusCache>()); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            FlushState();
        }

        #endregion


        internal void FlushState()
        {
            // This method assumes giving back the SvnClient instance flushes the state to the FileState cache
            if (_client != null)
            {
                IDisposable cl = _client;
                _client = null;
                cl.Dispose();
            }
        }

        public bool SkipIssueVerify 
        {
            get { return _skipIssueVerify; }
            set { _skipIssueVerify = value; }
        }

        public SortedDictionary<string, string> CustomProperties
        {
            get { return _customProperties; }
        }
    }
}
