using System;
using System.Collections.Generic;
using System.Text;
using Ankh.ContextServices;

namespace Ankh
{
    public class AnkhSvnClientPool : ISvnClientPool
    {
        readonly IAnkhServiceProvider _context;
        readonly Stack<SvnPoolClient> _clients = new Stack<SvnPoolClient>();
        readonly Stack<SvnPoolClient> _uiClients = new Stack<SvnPoolClient>();
        const int MaxPoolSize = 10;

        public AnkhSvnClientPool(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
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
            AnkhSvnPoolClient client = new AnkhSvnPoolClient(this, hookUI);

            if (hookUI)
            {
                IAnkhDialogOwner owner = _context.GetService<IAnkhDialogOwner>();

                if (owner != null)
                    SharpSvn.UI.SharpSvnUI.Bind(client, owner.DialogOwner);
            }

            return client;
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
        }
    }
}
