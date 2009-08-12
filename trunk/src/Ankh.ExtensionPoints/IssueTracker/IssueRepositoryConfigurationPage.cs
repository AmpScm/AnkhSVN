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

using System.Windows.Forms;
using System;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for Issue Repository configuration page
    /// </summary>
    public abstract class IssueRepositoryConfigurationPage : IIssueRepositoryConfigurationPageEvents
    {
        private IssueRepositorySettings _settings;

        /// <summary>
        /// Gets or sets the current repository settings.
        /// </summary>
        public virtual IssueRepositorySettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Gets the IWin32Window instance for issue repository configuration UI.
        /// </summary>
        /// <remarks>Default implementation returns this as IWin32Window.</remarks>
        public virtual IWin32Window Window
        {
            get
            {
                return this as IWin32Window;
            }
        }

        /// <summary>
        /// Notifies ragistered OnPageEvent handlers.
        /// </summary>
        public virtual void ConfigurationPageChanged(ConfigPageEventArgs e)
        {
            if (OnPageEvent != null)
            {
                OnPageEvent(this, e);
            }
        }

        #region IIssueRepositoryConfigurationPageEvents Members

        /// <summary>
        /// Raised on a configuration page event 
        /// </summary>
        public event System.EventHandler<ConfigPageEventArgs> OnPageEvent;

        #endregion
    }

    /// <summary>
    /// This interface exposes the config page events raised by connector configuration logic.
    /// </summary>
    public interface IIssueRepositoryConfigurationPageEvents
    {
        /// <summary>
        /// Raised on a config page event
        /// </summary>
        event EventHandler<ConfigPageEventArgs> OnPageEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConfigPageEventArgs : EventArgs
    {
        private bool _isComplete;
        private Exception _exception;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is complete.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}
