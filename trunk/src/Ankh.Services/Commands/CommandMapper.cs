using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

namespace Ankh.Commands
{
    public sealed class CommandMapper : AnkhService
    {
        readonly Dictionary<AnkhCommand, CommandMapItem> _map;

        public CommandMapper(IAnkhServiceProvider context)
            : base(context)
        {
            _map = new Dictionary<AnkhCommand, CommandMapItem>();
        }

        public bool PerformUpdate(AnkhCommand command, CommandUpdateEventArgs e)
        {
            EnsureLoaded();
            CommandMapItem item;

            if (_map.TryGetValue(command, out item))
            {
                try
                {
                    item.OnUpdate(e);
                }
                catch (Exception ex)
                {
                    IAnkhErrorHandler handler = Context.GetService<IAnkhErrorHandler>();

                    if (handler != null)
                    {
                        handler.OnError(ex);
                        return false;
                    }

                    throw;
                }

                return item.IsHandled;
            }

            return false;
        }

        public bool Execute(AnkhCommand command, CommandEventArgs e)
        {
            EnsureLoaded();
            CommandMapItem item;

            if (_map.TryGetValue(command, out item))
            {
                try
                {
                    CommandUpdateEventArgs u = new CommandUpdateEventArgs(command, e.Context);
                    item.OnUpdate(u);
                    if (u.Enabled)
                    {
                        item.OnExecute(e);
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    IAnkhErrorHandler handler = Context.GetService<IAnkhErrorHandler>();

                    if (handler != null)
                    {
                        handler.OnError(ex);
                        return false;
                    }

                    throw;

                }

                return item.IsHandled;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="CommandMapItem"/> for the specified command
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The <see cref="CommandMapItem"/> or null if the command is not valid</returns>
        public CommandMapItem this[AnkhCommand command]
        {
            get
            {
                CommandMapItem item;

                if (_map.TryGetValue(command, out item))
                    return item;
                else
                {
                    item = new CommandMapItem(command);

                    _map.Add(command, item);

                    return item;
                }
            }
        }

        readonly List<Assembly> _assembliesToLoad = new List<Assembly>();
        readonly List<Assembly> _assembliesLoaded = new List<Assembly>();

        public void LoadFrom(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (!_assembliesToLoad.Contains(assembly) && !_assembliesLoaded.Contains(assembly))
                _assembliesToLoad.Add(assembly);
        }

        private void EnsureLoaded()
        {
            if(_assembliesToLoad.Count == 0)
                return;

            while (_assembliesToLoad.Count > 0)
            {
                Assembly asm = _assembliesToLoad[0];
                _assembliesToLoad.RemoveAt(0);
                _assembliesLoaded.Add(asm);
                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;

                    if (!typeof(ICommandHandler).IsAssignableFrom(type))
                        continue;

                    ICommandHandler instance = null;

                    foreach (CommandAttribute cmdAttr in type.GetCustomAttributes(typeof(CommandAttribute), false))
                    {
                        CommandMapItem item = this[cmdAttr.Command];

                        if (item != null)
                        {
                            if (instance == null)
                            {
                                instance = (ICommandHandler)Activator.CreateInstance(type);

                                IComponent component = instance as IComponent;

                                if (component != null)
                                    component.Site = CommandSite;
                            }

                            Debug.Assert(item.ICommand == null || item.ICommand == instance, string.Format("No previous ICommand registered on the CommandMapItem for {0}", cmdAttr.Command));

                            item.ICommand = instance; // hooks all events in compatibility mode
                        }
                    }
                }
            }
        }

        CommandMapperSite _commandMapperSite;
        CommandMapperSite CommandSite
        {
            get { return _commandMapperSite ?? (_commandMapperSite = new CommandMapperSite(this)); }
        }

        sealed class CommandMapperSite : AnkhService, ISite
        {
            readonly CommandMapper _mapper;
            readonly Container _container = new Container();

            public CommandMapperSite(CommandMapper context)
                : base(context)
            {
                _mapper = context;
            }

            public IComponent Component
            {
                get { return _mapper; }
            }

            public IContainer Container
            {
                get { return _container; }
            }

            public bool DesignMode
            {
                get { return false; }
            }

            public string Name
            {
                get { return "CommandMapper"; }
                set { throw new InvalidOperationException();  }
            }            
        }
    }
}
