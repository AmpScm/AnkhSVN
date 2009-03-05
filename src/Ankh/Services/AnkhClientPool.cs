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
using System.ComponentModel.Design;
using System.Text;
using SharpSvn;

using Ankh.Scc;
using Ankh.VS;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using SharpSvn.UI;
using System.Windows.Forms.Design;
using Ankh.UI;
using System.Diagnostics;

namespace Ankh.Services
{
    [GlobalService(typeof(ISvnClientPool))]
    sealed class AnkhClientPool : AnkhService, ISvnClientPool
    {
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
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
            {
                // Let SharpSvnUI handle login and SSL dialogs
                SvnUIBindArgs bindArgs = new SvnUIBindArgs();
                bindArgs.ParentWindow = new OwnerWrapper(DialogOwner);
                bindArgs.UIService = GetService<IUIService>();
                bindArgs.Synchronizer = _syncher;

                SvnUI.Bind(client, bindArgs);
            }

            return client;
        }

        internal void NotifyChanges(HybridCollection<string> changedPaths, IList<string> fullRefresh)
        {
            if (changedPaths != null && changedPaths.Count > 0)
                StatusMonitor.ScheduleMonitor(changedPaths); // Adds the files to the to-commit list while they are not in a project

            if (fullRefresh != null && fullRefresh.Count > 0)
            {
                if (changedPaths == null)
                    changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                foreach (SvnItem item in StatusCache.GetCachedBelow(fullRefresh))
                {
                    if (!changedPaths.Contains(item.FullPath))
                        changedPaths.Add(item.FullPath);
                }
            }

            if (changedPaths != null && changedPaths.Count > 0)
            {
                StatusMonitor.ScheduleSvnStatus(changedPaths);
            }
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

            lock (_uiClients)
            {
                while (_uiClients.Count > 0)
                    _uiClients.Pop().Dispose();
            }

            lock (_clients)
            {
                while (_clients.Count > 0)
                    _clients.Pop().Dispose();
            }
        }

        class AnkhSvnPoolClient : SvnPoolClient
        {
            readonly HybridCollection<string> _touchedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly HybridCollection<string> _fullRefresh = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
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

                AddTouchedPath(e.FullPath, e.Action == SvnNotifyAction.CommitDeleted || e.Action == SvnNotifyAction.Revert);
            }

            void AddTouchedPath(string path, bool fullRefresh)
            {
                if (!string.IsNullOrEmpty(path) && !_touchedPaths.Contains(path))
                {
                    _touchedPaths.Add(path);
                }

                if (fullRefresh && !_fullRefresh.Contains(path))
                    _fullRefresh.Add(path);
            }

            protected override void ReturnClient()
            {
                HybridCollection<string> paths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                List<string> deleted = null;

                if (_fullRefresh.Count > 0)
                    deleted = new List<string>(_fullRefresh);

                paths.AddRange(_touchedPaths);
                _touchedPaths.Clear();
                _fullRefresh.Clear();

                AnkhClientPool pool = (AnkhClientPool)SvnClientPool;
                SvnClientPool = null;

                if (pool == null)
                {
                    Debug.Assert(false, "Returning pool client a second time");
                    return;
                }

                pool.NotifyChanges(paths, deleted);

                if (base.IsCommandRunning || base.IsDisposed)
                {
                    Debug.Assert(!IsCommandRunning, "Returning pool client while it is running");
                    Debug.Assert(!IsDisposed, "Returning pool client while it is disposed");

                    return; // No return on these errors.. Leave it to the GC to clean it up eventually
                }
                else if (!pool.ReturnClient(this))
                    InnerDispose(); // The pool wants to get rid of us
            }

            public int ReturnCookie
            {
                get { return _returnCookie; }
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
