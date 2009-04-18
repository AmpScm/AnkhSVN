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
    /// <summary>
    /// 
    /// </summary>
    public class PendingChangeEventArgs : EventArgs
    {
        readonly IPendingChangesManager _context;
        readonly PendingChange _change;
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChangeEventArgs"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="change">The change.</param>
        public PendingChangeEventArgs(IPendingChangesManager service, PendingChange change)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _change = change; // change can be null
            _context = service;
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public IPendingChangesManager Manager
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the change.
        /// </summary>
        /// <value>The change.</value>
        public PendingChange Change
        {
            get { return _change; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>This service is only available in the UI thread</remarks>
    public interface IPendingChangesManager
    {
        /// <summary>
        /// Gets a boolean indicating whether the pending changes manager is active
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets a list of all current pending changes
        /// </summary>
        /// <returns></returns>
        IEnumerable<PendingChange> GetAll();

        /// <summary>
        /// Gets a list of all current pending changes below a specific path
        /// </summary>
        /// <returns></returns>
        IEnumerable<PendingChange> GetAllBelow(string path);

        /// <summary>
        /// Schedules a refresh of all pending change state
        /// </summary>
        /// <param name="clearStateCache"></param>
        void FullRefresh(bool clearStateCache);

        /// <summary>
        /// Schedules a refresh the pending change state for the specified path
        /// </summary>
        void Refresh(string path);
        /// <summary>
        /// Schedules a refresh of thepending change state for the specified paths
        /// </summary>
        /// <param name="paths"></param>
        void Refresh(IEnumerable<string> paths);

        /// <summary>
        /// Gets the pending change information for the specified path or <c>null</c> if none is available
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        PendingChange this[string fullPath] { get; }

        /// <summary>
        /// Raised when a pending change item has been added
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        event EventHandler<PendingChangeEventArgs> Added;
        /// <summary>
        /// Raised when a pending change item has been removed
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        event EventHandler<PendingChangeEventArgs> Removed;
        /// <summary>
        /// Raised when the properties of a pending change have changed
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        event EventHandler<PendingChangeEventArgs> Changed;
        /// <summary>
        /// Raised when the complete pending change state has been flushed; All listeners should
        /// use GetAll() to get a new initial state
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        event EventHandler<PendingChangeEventArgs> ListFlushed;

        /// <summary>
        /// Occurs when [initial update].
        /// </summary>
        event EventHandler<PendingChangeEventArgs> InitialUpdate;

        /// <summary>
        /// Raised when the pending changes manager is activated or disabled
        /// </summary>
        event EventHandler<PendingChangeEventArgs> IsActiveChanged;

        /// <summary>
        /// Occurs when starting a refresh of more than a few items (This event allows you to batch a series of updates)
        /// </summary>
        event EventHandler<PendingChangeEventArgs> RefreshStarted;

        /// <summary>
        /// Occurs when completing a refresh of more than a few items (This event allows you to batch a series of updates)
        /// </summary>
        event EventHandler<PendingChangeEventArgs> RefreshCompleted;

        /// <summary>
        /// Clears all state; called on solution close
        /// </summary>
        void Clear();

        /// <summary>
        /// Tries to get a matching file from the specified text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        /// <remarks>Called from the log message editor in an attempt to provide a mouse over</remarks>
        bool TryMatchFile(string text, out PendingChange change);

        /// <summary>
        /// Determines whether the list of pending changes contains the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the specified path contains path; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(string path);

        IEnumerable<string> GetSuggestedChangeLists();
    }
}
