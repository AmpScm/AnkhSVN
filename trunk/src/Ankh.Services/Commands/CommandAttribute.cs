﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
    public sealed class CommandAttribute : Attribute
    {
        readonly AnkhCommand _command;
        readonly AnkhCommandContext _context;
        AnkhCommand _lastCommand;

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

        public AnkhCommand LastCommand
        {
            get { return _lastCommand; }
            set { _lastCommand = value; }
        }

        bool _showWhenDisabled;
        public bool HideWhenDisabled
        {
            get { return !_showWhenDisabled; }
            set { _showWhenDisabled = !value; }
        }

        bool _alwaysAvailable;

        /// <summary>
        /// Gets or sets a boolean indicating whether this command might be enabled if AnkhSVN is not the current SCC provider
        /// </summary>
        /// <remarks>If set to false the command is disabled (and when <see cref="HideWhenDisabled"/> also hidden)</remarks>
        public bool AlwaysAvailable
        {
            get { return _alwaysAvailable; }
            set { _alwaysAvailable = value; }
        }

        string _argumentDefinition;
        /// <summary>
        /// Gets or sets the argument definition string
        /// </summary>
        /// <remarks>
        ///     ‘~’ - No autocompletion for this parameter.
        ///     ‘$’ - This parameter is the rest of the input line (no autocompletion).
        ///     ‘a’ – An alias.
        ///     ‘c’ – The canonical name of a command.
        ///     ‘d’ – A filename from the file system.
        ///     ‘p’ – The filename from a project in the current solution.
        ///     ‘u’ – A URL.
        ///     ‘|’ – Combines two parameter types for the same parameter.
        ///     ‘*’ – Indicates zero or more occurrences of the previous parameter.
        ///     
        /// Some examples:
        ///     "d|p *" filenames or projects
        ///     
        /// “p p” – Command accepts two filenames 
        /// “u d” – Command accepts one URL and one filename argument.
        /// “u *” – Command accepts zero or more URL arguments.
        /// </remarks>
        public string ArgumentDefinition
        {
            get { return _argumentDefinition; }
            set { _argumentDefinition = value; }
        }

        internal IEnumerable<AnkhCommand> GetAllCommands()
        {
            if (LastCommand == AnkhCommand.None)
                yield return Command;
            else if(LastCommand < Command || ((int)LastCommand - (int)Command) > 256)
                throw new InvalidOperationException("Command range larger then 256 on range starting with" + Command.ToString());
            else
                for (AnkhCommand c = Command; c <= LastCommand; c++)
                {
                    yield return c;
                }
        }
    }
}
