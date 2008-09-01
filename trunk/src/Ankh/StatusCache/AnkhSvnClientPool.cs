﻿using System;
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

namespace Ankh
{
    [GlobalService(typeof(ISvnClientPool))]
    sealed class AnkhSvnClientPool : AnkhService, ISvnClientPool
    {
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
        readonly NotificationHandler _notifyHandler;
        readonly Control _syncher;
        const int MaxPoolSize = 10;

        public AnkhSvnClientPool(IAnkhServiceProvider context)
            : base(context)
        {
            _notifyHandler = context.GetService<NotificationHandler>();

            if (_notifyHandler == null)
            {
                _notifyHandler = new NotificationHandler(context);
                GetService<IServiceContainer>().AddService(typeof(NotificationHandler), _notifyHandler);
            }

            _syncher = new Control();
            _syncher.Visible = false;
            _syncher.Text = "AnkhSVN Synchronizer";
            _syncher.CreateControl();
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

        internal bool ReturnClient(SvnPoolClient poolClient, HybridCollection<string> changedPaths, IList<string> deleted)
        {
            bool ok = ReturnClient(poolClient);

            if (deleted != null && deleted.Count > 0)
            {
                if (changedPaths == null)
                    changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                IFileStatusCache cache = GetService<IFileStatusCache>();

                if (cache != null)
                {
                    foreach (SvnItem item in cache.GetCachedBelow(deleted))
                    {
                        if(!changedPaths.Contains(item.FullPath))
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

            return ok;
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
                if (!((AnkhSvnClientPool)SvnClientPool).ReturnClient(this, paths, deleted))
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
