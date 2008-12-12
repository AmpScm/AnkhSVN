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

namespace Ankh.Services
{
    [GlobalService(typeof(ISvnClientPool))]
    sealed class AnkhSvnClientPool : AnkhService, ISvnClientPool
    {
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
        readonly Control _syncher;
        const int MaxPoolSize = 10;

        public AnkhSvnClientPool(IAnkhServiceProvider context)
            : base(context)
        {
            _syncher = new Control();
            _syncher.Visible = false;
            _syncher.Text = "AnkhSVN Synchronizer";
            GC.KeepAlive(_syncher.Handle); // Ensure the window is created
        }

        bool _ensuredNames;
        void EnsureNames()
        {
            if(_ensuredNames)
                return;
            _ensuredNames = true;

            SvnClient.AddClientName("VisualStudio", GetService<IAnkhSolutionSettings>().VisualStudioVersion);
            SvnClient.AddClientName("AnkhSVN", new System.Reflection.AssemblyName(typeof(AnkhSvnClientPool).Assembly.FullName).Version);            
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

            IAnkhDialogOwner owner = GetService<IAnkhDialogOwner>();

            if (owner == null)
                hookUI = false;

            AnkhSvnPoolClient client = new AnkhSvnPoolClient(this, hookUI);

            if (hookUI)
            {
                // Let SharpSvnUI handle login and SSL dialogs
                SvnUIBindArgs bindArgs = new SvnUIBindArgs();
                bindArgs.ParentWindow = new OwnerWrapper(owner);
                bindArgs.UIService = GetService<IUIService>();
                bindArgs.Synchronizer = _syncher;

                SvnUI.Bind(client, bindArgs);
            }

            return client;
        }

        internal void NotifyChanges(HybridCollection<string> changedPaths, IList<string> deleted)
        {
            if (deleted != null && deleted.Count > 0)
            {
                if (changedPaths == null)
                    changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                IFileStatusCache cache = GetService<IFileStatusCache>();

                if (cache != null)
                {
                    foreach (SvnItem item in cache.GetCachedBelow(deleted))
                    {
                        if (!changedPaths.Contains(item.FullPath))
                            changedPaths.Add(item.FullPath);
                    }
                }
            }

            if (changedPaths != null && changedPaths.Count > 0)
            {
                IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

                if (monitor != null)
                {
                    monitor.ScheduleMonitor(changedPaths); // Adds the files to the to-commit list while they are not in a project
                    monitor.ScheduleSvnStatus(changedPaths);
                }
            }
        }

        public bool ReturnClient(SvnPoolClient poolClient)
        {
            AnkhSvnPoolClient pc = poolClient as AnkhSvnPoolClient;

            if (pc != null)
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

        class AnkhSvnPoolClient : SvnPoolClient
        {
            readonly HybridCollection<string> _touchedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly HybridCollection<string> _deleted = new HybridCollection<string>();
            readonly bool _uiEnabled;
            public AnkhSvnPoolClient(AnkhSvnClientPool pool, bool uiEnabled)
                : base(pool)
            {
                _uiEnabled = uiEnabled;
            }

            public bool UIEnabled
            {
                get { return _uiEnabled; }
            }

            protected override void OnNotify(SvnNotifyEventArgs e)
            {
                base.OnNotify(e);

                AddTouchedPath(e.FullPath, e.Action == SvnNotifyAction.CommitDeleted);
            }

            void AddTouchedPath(string path, bool deleted)
            {
                if (!string.IsNullOrEmpty(path) && !_touchedPaths.Contains(path))
                {
                    _touchedPaths.Add(path);
                }

                if (deleted && !_deleted.Contains(path))
                    _deleted.Add(path);
            }

            protected override void ReturnClient()
            {
                HybridCollection<string> paths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);                
                List<string> deleted = null;
                
                if(_deleted.Count > 0)
                    deleted = new List<string>(_deleted);

                paths.AddRange(_touchedPaths);
                _touchedPaths.Clear();
                _deleted.Clear();

                AnkhSvnClientPool pool = (AnkhSvnClientPool)SvnClientPool;

                pool.NotifyChanges(paths, deleted);

                if (base.IsCommandRunning || base.IsDisposed || !pool.ReturnClient(this))
                {
                    InnerDispose();
                }
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
