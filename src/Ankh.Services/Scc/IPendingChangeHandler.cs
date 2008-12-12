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

namespace Ankh.Scc
{
    public interface IPendingChangeHandler
    {
        bool Commit(IEnumerable<PendingChange> changes, PendingChangeCommitArgs args);
    }

    public class PendingChangeCommitArgs
    {
        string _logMessage;
        bool _keepLocks;
        bool _keepChangeLists;
        bool _storeMessageOnError;

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        /// <value>The log message.</value>
        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [keep locks].
        /// </summary>
        /// <value><c>true</c> if [keep locks]; otherwise, <c>false</c>.</value>
        public bool KeepLocks
        {
            get { return _keepLocks; }
            set { _keepLocks = value; }
        }

        public bool KeepChangeLists
        {
            get { return _keepChangeLists; }
            set { _keepChangeLists = value; }
        }

        public bool StoreMessageOnError
        {
            get { return _storeMessageOnError; }
            set { _storeMessageOnError = value; }
        }
    }
}
