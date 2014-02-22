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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for IssueRepository implementation
    /// </summary>
    public abstract class IssueRepository : IssueRepositorySettings
    {
        string _reValue;
        Regex _re;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectorName">Unique connector name(as registered with the registry)</param>
        protected IssueRepository(string connectorName)
            : base(connectorName)
        {
        }

        /// <summary>
        /// Label for the issue repository.
        /// </summary>
        public abstract string Label { get; }

        /// <summary>
        /// Handles the request to open the issue.
        /// </summary>
        /// <param name="issueId">Issue identifier</param>
        public virtual void NavigateTo(string issueId)
        {
        }

        /// <summary>
        /// Gets a boolean indicating whether the issue repository implements
        /// <see cref="NavigateToRevision"/>
        /// </summary>
        public virtual bool CanNavigateToRevision { get; protected set; }

        /// <summary>
        /// Called when a user uses 'Open Revision' on a log message that refers to
        /// some revision when <see cref="CanNavigateToRevision"/> is true.
        /// </summary>
        /// <param name="revision"></param>
        public virtual void NavigateToRevision(long revision)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called just before the changes are committed.
        /// </summary>
        public virtual void PreCommit(PreCommitArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
        }

        /// <summary>
        /// Called just after the changes are committed.
        /// </summary>
        /// <remarks>Provider is responsible for any post-commit issue related operations</remarks>
        public virtual void PostCommit(PostCommitArgs args)
        {
        }

        /// <summary>
        /// Obsolete: Gets the RegEx pattern to recognize issues in a text.
        /// </summary>
        [Obsolete("Please return a (compiled) regex from IssueIdRegex")]
        public virtual string IssueIdPattern
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets the pre-compiled issue id regex or NULL if there is no issue regex
        /// </summary>
        /// <remarks>The default implementation caches a regex built from IssueIdPattern</remarks>
        public virtual Regex IssueIdRegex
        {
            get
            {
                // Disable Obsolete warning
#pragma warning disable 618
                string pattern = IssueIdPattern;
#pragma warning restore 618
                // /Disable Obsolete warning

                if (string.Equals(pattern, _reValue))
                {
                    return _re ?? (_re = new Regex(_reValue, RegexOptions.CultureInvariant));
                }
                else if (!string.IsNullOrEmpty(pattern))
                {
                    try
                    {
                        Regex re = new Regex(pattern, RegexOptions.CultureInvariant);
                        _re = re;
                        _reValue = pattern;
                        return re;
                    }
                    catch
                    { }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the IWin32Window instance for issue presentation
        /// </summary>
        /// <remarks>Default implementation returns this as IWin32Window.</remarks>
        public virtual IWin32Window Window
        {
            get { return this as IWin32Window; }
        }
    }
}
