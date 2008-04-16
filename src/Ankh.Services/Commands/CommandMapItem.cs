using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// 
    /// </summary>
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
}
