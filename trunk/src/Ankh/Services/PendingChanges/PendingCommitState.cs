// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

namespace Ankh.PendingChanges
{
    class PendingCommitState : AnkhService, IDisposable
    {
        SvnClient _client;
        bool _keepLocks;
        bool _keepChangeLists;
        HybridCollection<PendingChange> _changes = new HybridCollection<PendingChange>();
        HybridCollection<string> _commitPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        string _logMessage;

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
                    _client = GetService<ISvnClientPool>().GetClient();

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

        bool IsDirectory(SvnItem item)
        {
            return item.IsDirectory || item.NodeKind == SvnNodeKind.Directory;
        }

        public SvnDepth CalculateCommitDepth()
        {
            SvnDepth depth = SvnDepth.Empty;
            bool requireInfinity = false;
            bool noDepthInfinity = false;

            foreach (string path in CommitPaths)
            {
                SvnItem item = Cache[path];

                if (IsDirectory(item))
                {
                    if (item.IsDeleteScheduled)
                    {
                        // Infinity = OK
                        requireInfinity = true;
                    }
                    else
                        noDepthInfinity = true;
                }
            }

            if (requireInfinity && !noDepthInfinity)
                depth = SvnDepth.Infinity;

            if (requireInfinity && noDepthInfinity)
            {
                // Houston we have a problem.
                // - Directory deletes require depth infinity
                // - There is another directory commit

                // Let's see if committing with depth infinity would go wrong
                bool hasOther = false;
                using (SvnClient cl = GetService<ISvnClientPool>().GetNoUIClient())
                {
                    bool cancel = false;
                    SvnStatusArgs sa = new SvnStatusArgs();
                    sa.ThrowOnError = false;
                    sa.ThrowOnCancel = false;
                    sa.RetrieveIgnoredEntries = false;
                    sa.Depth = SvnDepth.Infinity;
                    sa.Cancel += delegate(object sender, SvnCancelEventArgs ee) { if (cancel) ee.Cancel = true; };

                    foreach (string path in CommitPaths)
                    {
                        SvnItem item = Cache[path];

                        if (!IsDirectory(item) || item.IsDeleteScheduled)
                            continue; // Only check not to be deleted directories

                        if (!cl.Status(path, sa,
                            delegate(object sender, SvnStatusEventArgs ee)
                            {
                                if (!CommitPaths.Contains(ee.FullPath))
                                {
                                    hasOther = true;
                                    cancel = true; // Cancel via the cancel hook
                                }
                            }))
                        {
                            hasOther = true;
                        }

                        if (hasOther)
                            break;
                    }
                }

                if (!hasOther)
                {
                    // Ok; it is safe to commit with depth infinity; all items that would be committed
                    // with infinity would have been committed anyway

                    depth = SvnDepth.Infinity;
                }
                else
                {
                    MessageBox.Show("Subversion does not allow to commit this combination of files and directories at once. (Directory deletions and modifications collide)", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return SvnDepth.Unknown;
                }

            }

            // Returns SvnDepth.Infinity if there are directories scheduled for commit 
            // and all directories scheduled for commit are to be deleted
            //
            // Returns SvnDepth.Empty in all other cases
            return depth;
        }

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
    }
}
