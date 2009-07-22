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
        string _argumentDefinition;
        bool _dynamicMenuEnd;
        CommandTarget _target;

        public event EventHandler<CommandEventArgs> Execute;
        public event EventHandler<CommandUpdateEventArgs> Update;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMapItem"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandMapItem(AnkhCommand command)
        {
            _command = command;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public AnkhCommand Command
        {
            get { return _command; }
        }

        /// <summary>
        /// Gets or sets the I command.
        /// </summary>
        /// <value>The I command.</value>
        public ICommandHandler ICommand
        {
            get { return _handler; }
            set { _handler = value; }
        }

        /// <summary>
        /// Gets or sets the argument definition.
        /// </summary>
        /// <value>The argument definition.</value>
        public string ArgumentDefinition
        {
            get { return _argumentDefinition; }
            internal set { _argumentDefinition = value; }
        }

        /// <summary>
        /// Gets or sets the command target.
        /// </summary>
        /// <value>The command target.</value>
        public CommandTarget CommandTarget
        {
            get { return _target; }
            internal set { _target = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this items marks a dynamic menu end
        /// </summary>
        /// <value><c>true</c> if [dynamic menu end]; otherwise, <c>false</c>.</value>
        public bool DynamicMenuEnd
        {
            get { return _dynamicMenuEnd; }
            internal set { _dynamicMenuEnd = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:Execute"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Commands.CommandEventArgs"/> instance containing the event data.</param>
        protected internal void OnExecute(CommandEventArgs e)
        {
            if (ICommand != null)
                ICommand.OnExecute(e);

            if (Execute != null)
                Execute(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Update"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Commands.CommandUpdateEventArgs"/> instance containing the event data.</param>
        protected internal void OnUpdate(CommandUpdateEventArgs e)
        {
            if (ICommand != null)
                ICommand.OnUpdate(e);

            if (Update != null)
                Update(this, e);
        }

        bool _alwaysAvailable;
        /// <summary>
        /// Gets or sets a value indicating whether this command is available when AnkhSVN is not the active SCC
        /// </summary>
        /// <value><c>true</c> if [always available]; otherwise, <c>false</c>.</value>
        public bool AlwaysAvailable
        {
            get { return _alwaysAvailable; }
            internal set { _alwaysAvailable = value; }
        }

        bool _hideWhenDisabled;
        /// <summary>
        /// Gets or sets a value indicating whether the command is hidden when disabled
        /// </summary>
        /// <value><c>true</c> if [hide when disabled]; otherwise, <c>false</c>.</value>
        public bool HiddenWhenDisabled
        {
            get { return _hideWhenDisabled; }
            internal set { _hideWhenDisabled = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this command is handled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this command is handled; otherwise, <c>false</c>.
        /// </value>
        public bool IsHandled
        {
            get { return (Execute != null) || (ICommand != null); }
        }
    }
}
