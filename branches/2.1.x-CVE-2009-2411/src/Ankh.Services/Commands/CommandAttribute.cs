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
using System.Text;

namespace Ankh.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CommandAttribute : Attribute
    {
        readonly AnkhCommand _command;
        readonly AnkhCommandContext _context;
        AnkhCommand _lastCommand;
        CommandTarget _target;

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

        /// <summary>
        /// Gets or sets the last command.
        /// </summary>
        /// <value>The last command.</value>
        public AnkhCommand LastCommand
        {
            get { return _lastCommand; }
            set { _lastCommand = value; }
        }

        /// <summary>
        /// Gets or sets the command target.
        /// </summary>
        /// <value>The command target.</value>
        public CommandTarget CommandTarget
        {
            get { return _target; }
            set { _target = value; }
        }

        bool _showWhenDisabled;
        /// <summary>
        /// Gets or sets a value indicating whether [hide when disabled].
        /// </summary>
        /// <value><c>true</c> if [hide when disabled]; otherwise, <c>false</c>.</value>
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
            else if (LastCommand < Command || ((int)LastCommand - (int)Command) > 256)
                throw new InvalidOperationException("Command range larger then 256 on range starting with" + Command.ToString());
            else
                for (AnkhCommand c = Command; c <= LastCommand; c++)
                {
                    yield return c;
                }
        }
    }
}
