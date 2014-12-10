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

namespace Ankh.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class GitCommandAttribute : CommandAttribute
    {
        /// <summary>
        /// Defines the class or function as a handler of the specified <see cref="AnkhCommand"/>
        /// </summary>
        /// <param name="command">The command.</param>
        public GitCommandAttribute(AnkhCommand command)
            : base(command)
        {
            Availability = CommandAvailability.GitActive;
        }

        public GitCommandAttribute(AnkhCommand command, AnkhCommandContext context)
            : base(command, context)
        {
            Availability = CommandAvailability.GitActive;
        }
    }
}
