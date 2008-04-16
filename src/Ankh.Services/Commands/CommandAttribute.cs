using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
    public sealed class CommandAttribute : Attribute
    {
        readonly AnkhCommand _command;
        readonly AnkhCommandContext _context;

        /// <summary>
        /// Defines the class or function as a handler of the specified <see cref="AnkhCommand"/>
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandAttribute(AnkhCommand command)
        {
            _command = command;
        }

        public CommandAttribute(AnkhCommand command, AnkhCommandContext context)
            : this(command)
        {
            _context = context;
        }


        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public AnkhCommand Command
        {
            get { return _command; }
        }

        public AnkhCommandContext Context
        {
            get { return _context; }
        }
    }
}
