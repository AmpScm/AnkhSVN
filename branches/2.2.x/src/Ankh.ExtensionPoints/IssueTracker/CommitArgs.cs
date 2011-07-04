// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommitArgs
    {
        private readonly ICollection<Uri> _paths;
        private readonly long _revision;
        string _commitMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitArgs"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="commitMessage">The commit message.</param>
        protected CommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
        {
            _paths = paths;
            _revision = revision;
            _commitMessage = commitMessage;
        }

        // ### BH: I think this should be a list of string, not Uri.
        /// <summary>
        /// Gets the paths.
        /// </summary>
        /// <value>The paths.</value>
        public ICollection<Uri> Paths
        {
            get { return _paths; }
        }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public long Revision
        {
            get { return _revision; }
        }

        /// <summary>
        /// Gets or sets the internal commit message.
        /// </summary>
        /// <value>The internal commit message.</value>
        protected string InternalCommitMessage
        {
            get { return _commitMessage; }
            set { _commitMessage = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PreCommitArgs : CommitArgs
    {
        bool _cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreCommitArgs"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="revision">The revision.</param>
        public PreCommitArgs(ICollection<Uri> paths, long revision)
            : this(paths, revision, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreCommitArgs"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="commitMessage">The commit message.</param>
        public PreCommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
            : base(paths, revision, commitMessage)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PreCommitArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        /// <summary>
        /// Gets or sets the commit message.
        /// </summary>
        /// <value>The commit message.</value>
        public string CommitMessage
        {
            get { return InternalCommitMessage; }
            set { InternalCommitMessage = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PostCommitArgs : CommitArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostCommitArgs"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="revision">The revision.</param>
        public PostCommitArgs(ICollection<Uri> paths, long revision)
            : this(paths, revision, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostCommitArgs"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="commitMessage">The commit message.</param>
        public PostCommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
            : base(paths, revision, commitMessage)
        {
        }

        /// <summary>
        /// Gets the commit message.
        /// </summary>
        /// <value>The commit message.</value>
        public string CommitMessage
        {
            get { return InternalCommitMessage; }
        }
    }
}
