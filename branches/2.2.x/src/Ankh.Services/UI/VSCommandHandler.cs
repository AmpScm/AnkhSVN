// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using System.ComponentModel.Design;
using Ankh.Commands;

namespace Ankh.UI
{
    public static class VSCommandHandler
    {
        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="updateHandler">The update handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, CommandID command, EventHandler<CommandEventArgs> handler, EventHandler<CommandUpdateEventArgs> updateHandler)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (control == null)
                throw new ArgumentNullException("control");
            else if (command == null)
                throw new ArgumentNullException("command");

            IAnkhCommandHandlerInstallerService svc = context.GetService<IAnkhCommandHandlerInstallerService>();

            if (svc != null)
                svc.Install(control, command, handler, updateHandler);
        }

        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="updateHandler">The update handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, AnkhCommand command, EventHandler<CommandEventArgs> handler, EventHandler<CommandUpdateEventArgs> updateHandler)
        {
            Install(context, control, new CommandID(AnkhId.CommandSetGuid, (int)command), handler, updateHandler);
        }

        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, CommandID command, EventHandler<CommandEventArgs> handler)
        {
            Install(context, control, command, handler, null);
        }

        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, AnkhCommand command, EventHandler<CommandEventArgs> handler)
        {
            Install(context, control, command, handler, null);
        }

        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, CommandID command, ICommandHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            Install(context, control, command,
                delegate(object sender, CommandEventArgs e)
                {
                    handler.OnExecute(e);
                },
                delegate(object sender, CommandUpdateEventArgs e)
                {
                    handler.OnUpdate(e);
                });
        }

        /// <summary>
        /// Installs a visual studio command handler for the specified control
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="control">The control.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        public static void Install(IAnkhServiceProvider context, Control control, AnkhCommand command, ICommandHandler handler)
        {
            Install(context, control, command,
                delegate(object sender, CommandEventArgs e)
                {
                    handler.OnExecute(e);
                },
                delegate(object sender, CommandUpdateEventArgs e)
                {
                    handler.OnUpdate(e);
                });
        }
    }

    public interface IAnkhCommandHandlerInstallerService
    {
        void Install(Control control, CommandID command, EventHandler<CommandEventArgs> handler, EventHandler<CommandUpdateEventArgs> updateHandler);
    }

    public interface IAnkhCommandHookAccessor
    {
        AnkhCommandHook CommandHook { get; set; }        
    }

    public abstract class AnkhCommandHook : AnkhService
    {
        readonly Control _control;
        protected AnkhCommandHook(IAnkhServiceProvider context, Control control)
            : base(context)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            _control = control;
        }

        public Control Control
        {
            get { return _control; }
        }

        public abstract void Install(Control control, CommandID command, EventHandler<CommandEventArgs> handler, EventHandler<CommandUpdateEventArgs> updateHandler);
    }
}
