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
using Ankh.Selection;
using System.Diagnostics;

namespace Ankh.Commands
{
    public class BaseCommandEventArgs : EventArgs
    {
        readonly AnkhCommand _command;
        readonly AnkhContext _context;
        CommandMapItem _mapItem;

        public BaseCommandEventArgs(AnkhCommand command, AnkhContext context)
        {
            _command = command;
            _context = context;
        }

        public AnkhCommand Command
        {
            [DebuggerStepThrough]
            get { return _command; }
        }

        public AnkhContext Context
        {
            [DebuggerStepThrough]
            get { return _context; }
        }

        ISelectionContext _selection;
        IAnkhCommandStates _state;
        /// <summary>
        /// Gets the Visual Studio selection
        /// </summary>
        /// <value>The selection.</value>        
        public ISelectionContext Selection
        {
            [DebuggerStepThrough]
            get { return _selection ?? (_selection = GetService<ISelectionContext>()); }
        }

        /// <summary>
        /// Gets the command states.
        /// </summary>
        /// <value>The state.</value>
        public IAnkhCommandStates State
        {
            [DebuggerStepThrough]
            get { return _state ?? (_state = GetService<IAnkhCommandStates>()); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in automation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in automation; otherwise, <c>false</c>.
        /// </value>
        public bool IsInAutomation
        {
            get
            {
                IAnkhRuntimeInfo runtimeInfo = Context.GetService<IAnkhRuntimeInfo>();

                if (runtimeInfo != null)
                    return runtimeInfo.IsInAutomation;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return Context.GetService<T>();
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return Context.GetService<T>(serviceType);
        }

        internal void Prepare(CommandMapItem item)
        {
            _mapItem = item;
        }
    }
}
