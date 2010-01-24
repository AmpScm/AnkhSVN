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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using SharpSvn;
using SharpSvn.Remote;
using SharpSvn.UI;

using Ankh.Scc;
using Ankh.UI;
using Ankh.VS;

namespace Ankh.Services
{
    [GlobalService(typeof(ISvnClientPool))]
    sealed class AnkhClientPool : AnkhService, ISvnClientPool
    {
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
        readonly List<SvnPoolRemoteSession> _remoteSessions = new List<SvnPoolRemoteSession>();
        readonly Control _syncher;
        const int MaxPoolSize = 10;
        int _returnCookie;

        public AnkhClientPool(IAnkhServiceProvider context)
            : base(context)
        {
            _syncher = new Control();
            _syncher.Visible = false;
            _syncher.Text = "AnkhSVN Synchronizer";
            GC.KeepAlive(_syncher.Handle); // Ensure the window is created
        }

        IAnkhDialogOwner _dialogOwner;
        IAnkhDialogOwner DialogOwner
        {
            get { return _dialogOwner ?? (_dialogOwner = GetService<IAnkhDialogOwner>()); }
        }

        IFileStatusCache _cache;
        IFileStatusCache StatusCache
        {
            get { return _cache ?? (_cache = GetService<IFileStatusCache>()); }
        }

        IFileStatusMonitor _monitor;
        IFileStatusMonitor StatusMonitor
        {
            get { return _monitor ?? (_monitor = GetService<IFileStatusMonitor>()); }
        }

        bool _ensuredNames;
        void EnsureNames()
        {
            if (_ensuredNames)
                return;
            _ensuredNames = true;

            SvnClient.AddClientName("VisualStudio", GetService<IAnkhSolutionSettings>().VisualStudioVersion);
            SvnClient.AddClientName("AnkhSVN", GetService<IAnkhPackage>().UIVersion);
        }

        public SvnPoolClient GetClient()
        {
            lock (_uiClients)
            {
                if (_uiClients.Count > 0)
                    return _uiClients.Pop();

                return CreateClient(true);
            }
        }

        public SvnPoolClient GetNoUIClient()
        {
            lock (_clients)
            {
                if (_clients.Count > 0)
                    return _clients.Pop();

                return CreateClient(false);
            }
        }

        public SvnWorkingCopyClient GetWcClient()
        {
            return new SvnWorkingCopyClient();
        }

        private SvnPoolClient CreateClient(bool hookUI)
        {
            EnsureNames();

            if (DialogOwner == null)
                hookUI = false;

            AnkhSvnPoolClient client = new AnkhSvnPoolClient(this, hookUI, _returnCookie);

            if (hookUI)
                HookUI(client);

            return client;
        }

        // Use separate function to delay loading the SharpSvn.UI.dll
        private void HookUI(AnkhSvnPoolClient client)
        {
            // Let SharpSvnUI handle login and SSL dialogs
            SvnUIBindArgs bindArgs = new SvnUIBindArgs();
            bindArgs.ParentWindow = new OwnerWrapper(DialogOwner);
            bindArgs.UIService = GetService<IUIService>();
            bindArgs.Synchronizer = _syncher;

            SvnUI.Bind(client, bindArgs);
        }

        internal void NotifyChanges(IDictionary<string, SvnClientAction> actions)
        {
            StatusMonitor.HandleSvnResult(actions);
        }

        public bool ReturnClient(SvnPoolClient poolClient)
        {
            AnkhSvnPoolClient pc = poolClient as AnkhSvnPoolClient;

            if (pc != null && pc.ReturnCookie == _returnCookie)
            {
                Stack<SvnPoolClient> stack = pc.UIEnabled ? _uiClients : _clients;

                lock (stack)
                {
                    if (stack.Count < MaxPoolSize)
                    {
                        stack.Push(pc);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Flushes all clients to read settings again
        /// </summary>
        public void FlushAllClients()
        {
            _returnCookie++;

            List<IDisposable> toDispose = new List<IDisposable>();

            lock (_uiClients)
            {
                while (_uiClients.Count > 0)
                    toDispose.Add(_uiClients.Pop());
            }

            lock (_clients)
            {
                while (_clients.Count > 0)
                    toDispose.Add(_clients.Pop());
            }

            lock (_remoteSessions)
            {
                foreach (SvnPoolRemoteSession rs in _remoteSessions)
                    toDispose.Add(rs);
                _remoteSessions.Clear();
            }

            foreach (IDisposable d in toDispose)
            {
                d.Dispose();
            }
        }

        #region ISvnClientPool Members


        public SvnPoolRemoteSession GetRemoteSession(Uri sessionUri, bool parentOk)
        {
            if (sessionUri == null)
                throw new ArgumentNullException("sessionUri");

            SvnPoolRemoteSession reuse = null;
            lock (_remoteSessions)
            {
                if (_remoteSessions.Count > 0)
                {
                    foreach (SvnPoolRemoteSession rs in _remoteSessions)
                    {
                        if (rs.SessionUri == sessionUri)
                        {
                            reuse = rs;
                            _remoteSessions.Remove(rs);
                            break;
                        }
                    }

                    if (reuse == null)
                    {
                        string schemeAndServer = sessionUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
                        foreach (SvnPoolRemoteSession rs in _remoteSessions)
                        {
                            Uri reposUri = rs.RepositoryRootUri ?? rs.SessionUri;

                            if (reposUri == null || reposUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped) != schemeAndServer)
                                continue;

                            if (sessionUri.AbsolutePath.StartsWith(reposUri.AbsolutePath, StringComparison.Ordinal))
                            {
                                reuse = rs;
                            }
                        }
                    }

                    if (reuse != null)
                        _remoteSessions.Remove(reuse);
                }
            }

            if (reuse != null)
            {
                    if (sessionUri != reuse.SessionUri)
                    {
                        if (!parentOk || !sessionUri.AbsolutePath.StartsWith(reuse.SessionUri.AbsolutePath))
                        {
                            SvnRemoteCommonArgs rca = new SvnRemoteCommonArgs();
                            rca.ThrowOnError = false;

                            if (!reuse.Reparent(sessionUri, rca))
                                reuse = null;
                        }
                    }

                if (reuse != null)
                    return reuse;

                // else -> GC will cleanup
            }

            AnkhSvnPoolRemoteSession session = new AnkhSvnPoolRemoteSession(this, true, _returnCookie);
            session.Open(sessionUri);
            return session;
        }

        public bool ReturnClient(SvnPoolRemoteSession session)
        {
            AnkhSvnPoolRemoteSession pc = session as AnkhSvnPoolRemoteSession;

            if (pc != null && pc.ReturnCookie == _returnCookie && pc.SessionUri != null)
            {
                pc.ReturnTime = DateTime.Now;

                lock (_remoteSessions)
                {
                    _remoteSessions.Insert(0, pc);

                    ScheduleDisposeSessions();
                    return true;
                }
            }
            return false;
        }

        IAnkhScheduler _scheduler;
        IAnkhScheduler Scheduler
        {
            get { return _scheduler ?? (_scheduler = GetService<IAnkhScheduler>()); }
        }


        DateTime? nextCleanup;
        private void ScheduleDisposeSessions()
        {
            if (nextCleanup.HasValue)
            {
                TimeSpan ts = nextCleanup.Value - DateTime.Now;

                if (ts.Minutes < 5 && ts.Minutes > -1)
                    return;
            }

            DateTime nextTime = DateTime.Now + new TimeSpan(0, 2, 0);
            nextCleanup = nextTime;

            Scheduler.ScheduleAt(nextTime, OnCleanup);
        }

        void OnCleanup()
        {
            List<SvnPoolRemoteSession> toDispose = null;
            bool left = false;
            lock (_remoteSessions)
            {
                DateTime now = DateTime.Now;

                foreach (AnkhSvnPoolRemoteSession rs in _remoteSessions)
                {
                    bool dispose = false;
                    switch (rs.SessionUri.Scheme)
                    {
                        case "svn":
                            dispose = (now - rs.ReturnTime) > new TimeSpan(0, 3, 0);
                            break;
                        case "https":
                        case "http":
                            dispose = (now - rs.ReturnTime) > new TimeSpan(0, 5, 0);
                            break;
                        default:
                            dispose = true;
                            break;
                    }

                    if (!dispose)
                        left = true;
                    else
                    {
                        if (toDispose == null)
                            toDispose = new List<SvnPoolRemoteSession>();

                        toDispose.Add(rs);
                    }
                }

                if (toDispose != null)
                    foreach (SvnPoolRemoteSession rs in toDispose)
                        _remoteSessions.Remove(rs);
            }


            if (toDispose != null)
                foreach (AnkhSvnPoolRemoteSession rs in toDispose)
                    rs.Dispose();

            if (left)
                ScheduleDisposeSessions();
        }

        #endregion

        sealed class AnkhSvnPoolClient : SvnPoolClient
        {
            readonly SortedDictionary<string, SvnClientAction> _changes = new SortedDictionary<string, SvnClientAction>(StringComparer.OrdinalIgnoreCase);
            readonly bool _uiEnabled;
            readonly int _returnCookie;
            public AnkhSvnPoolClient(AnkhClientPool pool, bool uiEnabled, int returnCookie)
                : base(pool)
            {
                _uiEnabled = uiEnabled;
                _returnCookie = returnCookie;
            }

            public bool UIEnabled
            {
                get { return _uiEnabled; }
            }

            protected override void OnNotify(SvnNotifyEventArgs e)
            {
                base.OnNotify(e);

                string path = e.FullPath;

                if (string.IsNullOrEmpty(path))
                    return;

                SvnClientAction action;
                if (!_changes.TryGetValue(path, out action))
                    _changes.Add(path, action = new SvnClientAction(path));

                switch (e.Action)
                {
                    case SvnNotifyAction.CommitDeleted:
                    case SvnNotifyAction.Revert:
                    case SvnNotifyAction.TreeConflict:
                        action.Recursive = true;
                        break;
                    case SvnNotifyAction.UpdateDelete:
                        action.Recursive = true;
                        action.AddOrRemove = true;
                        break;
                    case SvnNotifyAction.UpdateReplace:
                    case SvnNotifyAction.UpdateAdd:
                        action.AddOrRemove = true;
                        break;
                    case SvnNotifyAction.UpdateUpdate:
                        action.OldRevision = e.OldRevision;
                        break;
                }
            }

            protected override void OnCommitting(SvnCommittingEventArgs e)
            {
                base.OnCommitting(e);

                if (e.CurrentCommandType != SvnCommandType.Commit)
                    return;

                foreach (SvnCommitItem item in e.Items)
                {
                    string fp = item.FullPath;

                    if (fp == null) // Non local operation
                        return;

                    SvnClientAction action;

                    if (!_changes.TryGetValue(fp, out action))
                        _changes.Add(fp, action = new SvnClientAction(fp));
                }
            }

            protected override void ReturnClient()
            {
                AnkhClientPool pool = (AnkhClientPool)SvnClientPool;
                SvnClientPool = null;

                if (pool == null)
                {
                    Debug.Assert(false, "Returning pool client a second time");
                    return;
                }

                try
                {
                    if (_changes.Count > 0)
                        pool.NotifyChanges(_changes);
                }
                finally
                {
                    _changes.Clear();
                }

                if (base.IsCommandRunning || base.IsDisposed)
                {
                    Debug.Assert(!IsCommandRunning, "Returning pool client while it is running");
                    Debug.Assert(!IsDisposed, "Returning pool client while it is disposed");

                    return; // No return on these errors.. Leave it to the GC to clean it up eventually
                }
                else if (!pool.ReturnClient(this))
                    InnerDispose(); // The pool wants to get rid of us
                else
                    SvnClientPool = pool; // Reinstated
            }

            public int ReturnCookie
            {
                get { return _returnCookie; }
            }
        }

        sealed class AnkhSvnPoolRemoteSession : SvnPoolRemoteSession
        {
            readonly bool _uiEnabled;
            readonly int _returnCookie;
            DateTime _returnTime;
            public AnkhSvnPoolRemoteSession(AnkhClientPool pool, bool uiEnabled, int returnCookie)
                : base(pool)
            {
                _uiEnabled = uiEnabled;
                _returnCookie = returnCookie;
            }

            public bool UIEnabled
            {
                get { return _uiEnabled; }
            }

            protected override void ReturnClient()
            {
                AnkhClientPool pool = (AnkhClientPool)SvnClientPool;
                SvnClientPool = null;

                if (pool == null)
                {
                    Debug.Assert(false, "Returning pool client a second time");
                    return;
                }

                if (base.IsCommandRunning || base.IsDisposed)
                {
                    Debug.Assert(!IsCommandRunning, "Returning pool client while it is running");
                    Debug.Assert(!IsDisposed, "Returning pool client while it is disposed");

                    return; // No return on these errors.. Leave it to the GC to clean it up eventually
                }
                else if (!pool.ReturnClient(this))
                    InnerDispose(); // The pool wants to get rid of us
                else
                    SvnClientPool = pool; // Reinstated
            }

            public int ReturnCookie
            {
                get { return _returnCookie; }
            }

            public DateTime ReturnTime
            {
                get { return _returnTime.ToLocalTime(); }
                set { _returnTime = value.ToUniversalTime(); }
            }
        }
    }

    sealed class OwnerWrapper : IWin32Window
    {
        IAnkhDialogOwner _owner;

        public OwnerWrapper(IAnkhDialogOwner owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            _owner = owner;
        }

        public IntPtr Handle
        {
            get
            {
                IWin32Window window = _owner.DialogOwner;

                if (window != null)
                {
                    ISynchronizeInvoke invoker = window as ISynchronizeInvoke;

                    if (invoker != null && invoker.InvokeRequired && Control.CheckForIllegalCrossThreadCalls)
                    {
                        Control.CheckForIllegalCrossThreadCalls = false;
                        try
                        {
                            return window.Handle;
                        }
                        finally
                        {
                            Control.CheckForIllegalCrossThreadCalls = true;
                        }
                    }
                    else
                        return window.Handle;
                }
                else
                    return IntPtr.Zero;
            }
        }
    }
}
