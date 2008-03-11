using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using System.Diagnostics;
using System.Reflection;

namespace Ankh.Commands
{
    public class CommandMapItem
    {
        readonly AnkhCommand _command;
        ICommandHandler _handler;

        public event EventHandler<CommandEventArgs> Execute;
        public event EventHandler<CommandUpdateEventArgs> Update;

        public CommandMapItem(AnkhCommand command)
        {
            _command = command;
        }

        public AnkhCommand Command
        {
            get { return _command; }
        }

        public ICommandHandler ICommand
        {
            get { return _handler; }
            set { _handler = value; }
        }

        protected internal void OnExecute(CommandEventArgs e)
        {
            if (ICommand != null)
                ICommand.OnExecute(e);

            if (Execute != null)
                Execute(this, e);
        }

        protected internal void OnUpdate(CommandUpdateEventArgs e)
        {
            if (ICommand != null)
                ICommand.OnUpdate(e);

            if (Update != null)
                Update(this, e);
        }

        public bool IsHandled
        {
            get { return (Execute != null) || (ICommand != null); }
        }
    }

    public sealed class CommandMapper
    {
        readonly IServiceProvider _context;
        readonly Dictionary<AnkhCommand, CommandMapItem> _map;

        public CommandMapper(IServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
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
                    IAnkhErrorHandler handler = (IAnkhErrorHandler)_context.GetService(typeof(IAnkhErrorHandler));

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
                    IAnkhErrorHandler handler = (IAnkhErrorHandler)_context.GetService(typeof(IAnkhErrorHandler));

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
                else if (command <= AnkhCommand.CommandFirst)
                    return null;
                else if (Enum.IsDefined(typeof(AnkhCommand), command))
                {
                    item = new CommandMapItem(command);

                    _map.Add(command, item);

                    return item;
                }
                else
                    return null;
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
                                instance = (ICommandHandler)Activator.CreateInstance(type);

                            Debug.Assert(item.ICommand == null || item.ICommand == instance, string.Format("No previous ICommand registered on the CommandMapItem for {0}", cmdAttr.Command));

                            item.ICommand = instance; // hooks all events in compatibility mode
                        }
                    }
                }
            }
        }
    }
}
