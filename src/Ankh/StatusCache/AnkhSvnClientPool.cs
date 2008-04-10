using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using SharpSvn;

using Ankh.Scc;
using Ankh.VS;
using System.IO;

namespace Ankh
{
    public class AnkhSvnClientPool : ISvnClientPool
    {
        readonly IAnkhServiceProvider _context;
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
        readonly NotificationHandler _notifyHandler;
        const int MaxPoolSize = 10;

        public AnkhSvnClientPool(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _notifyHandler = context.GetService<NotificationHandler>();

            if (_notifyHandler == null)
            {
                _notifyHandler = new NotificationHandler(context);
                context.GetService<IServiceContainer>().AddService(typeof(NotificationHandler), _notifyHandler);
            }

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

        private SvnPoolClient CreateClient(bool hookUI)
        {
            IAnkhDialogOwner owner = _context.GetService<IAnkhDialogOwner>();

            if (owner == null)
                hookUI = false;

            AnkhSvnPoolClient client = new AnkhSvnPoolClient(this, hookUI);

            ////// should we use a custom configuration directory?
            //if (this.config.Subversion.ConfigDir != null)
            //    this.client.LoadConfiguration(
            //        Environment.ExpandEnvironmentVariables(this.config.Subversion.ConfigDir));

            if (hookUI)
            {
                    // Let SharpSvnUI handle login and SSL dialogs
                    SharpSvn.UI.SharpSvnUI.Bind(client, owner.DialogOwner);
            }

            return client;
        }

        internal bool ReturnClient(SvnPoolClient poolClient, IList<string> changedPaths)
        {
            bool ok = ReturnClient(poolClient);

            if (changedPaths != null && changedPaths.Count > 0)
            {
                // TODO: Marshal to UI thread if we are not there!!!

                IFileStatusMonitor monitor = _context.GetService<IFileStatusMonitor>();

                if (monitor != null)
                {
                    monitor.ScheduleStatusUpdate(changedPaths);
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
            readonly Dictionary<string, string> _touchedPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

                AddTouchedPath(e.FullPath);

                // *************** See subversion issue #3168 ************************
                if (e.CommandType == SvnCommandType.Commit && e.Action.ToString().StartsWith("Commit"))
                {
                    if (!File.Exists(e.FullPath) && !Directory.Exists(e.FullPath) && !Directory.Exists(Path.GetDirectoryName(e.FullPath)))
                    {
                        // e.FullPath should have been ok, as all files to commit should exist (or its parent directory should)

                        // As a workaround:
                        //   We just add all possible variations of a path that might be committed to the list of files to refresh

                        string dir = Environment.CurrentDirectory;

                        while (!string.IsNullOrEmpty(dir))
                        {
                            AddTouchedPath(Path.GetFullPath(Path.Combine(dir, e.Path))); // Not e.FullPath, as that is absolute (and invalid)
                            dir = Path.GetDirectoryName(dir);
                        }
                    }
                }
            }

            void AddTouchedPath(string path)
            {
                if (!string.IsNullOrEmpty(path) && !_touchedPaths.ContainsKey(path))
                    _touchedPaths[path] = path;
            }

            protected override void ReturnClient()
            {
                List<string> paths = new List<string>(_touchedPaths.Values);
                _touchedPaths.Clear();
                if (!((AnkhSvnClientPool)SvnClientPool).ReturnClient(this, paths))
                {
                    InnerDispose();
                }
            }
        }
    }
}
