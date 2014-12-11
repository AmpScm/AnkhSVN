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

namespace Ankh
{
    public class LastChangedEventArgs : EventArgs
    {
        readonly string _caption;
        readonly string _value;

        public LastChangedEventArgs(string caption, string value)
        {
            _caption = caption;
            _value = value;
        }

        public string Caption
        {
            get { return _caption; }
        }

        public string Value
        {
            get { return _value; }
        }
    }
    public interface IAnkhServiceEvents
    {
        /// <summary>
        /// Raises the <see cref="E:RuntimeLoaded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnRuntimeLoaded(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:RuntimeStarted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnRuntimeStarted(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:SccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnSccProviderActivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:SccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnSccProviderDeactivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnGitSccProviderActivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnGitSccProviderDeactivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnDocumentTrackerActivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnDocumentTrackerDeactivated(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:SolutionOpened"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnSolutionOpened(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:SolutionClosed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnSolutionClosed(EventArgs e);

        /// <summary>
        /// Raises the <see cref="UIShellActivate"/> event.
        /// </summary>
        /// <param name="e"></param>
        void OnUIShellActivate(EventArgs e);

        /// <summary>
        /// Raises the <see cref="ThemeChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        void OnThemeChanged(EventArgs e);

        /// <summary>
        /// Raises the <see cref="E:LastChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        void OnLastChanged(LastChangedEventArgs e);
    }

    /// <summary>
    /// Ankh service events container
    /// </summary>
    public class AnkhServiceEvents : AnkhService, IAnkhServiceEvents
    {
        internal protected AnkhServiceEvents(IAnkhServiceProvider context)
            : base(context)
        {
        }

        /// <summary>
        /// Occurs when the AnkhSVN Runtime is started (after the OnInitialize of modules and services)
        /// </summary>
        /// <remarks>Called before all other events on this context object</remarks>
        public event EventHandler RuntimeLoaded;

        /// <summary>
        /// Occurs when the AnkhSVN Runtime is started (after the OnInitialize of modules and services)
        /// </summary>
        /// <remarks>Some handlers of this event call other events!</remarks>
        public event EventHandler RuntimeStarted;

        /// <summary>
        /// Occurs when our SCC provider is activated
        /// </summary>
        public event EventHandler SccProviderActivated;

        /// <summary>
        /// Occurs when our SCC provider is deactivated
        /// </summary>
        public event EventHandler SccProviderDeactivated;

        /// <summary>
        /// Occurs when our SCC provider is activated
        /// </summary>
        public event EventHandler GitSccProviderActivated;

        /// <summary>
        /// Occurs when our SCC provider is deactivated
        /// </summary>
        public event EventHandler GitSccProviderDeactivated;

        /// <summary>
        /// Occurs when the document tracker is activated
        /// </summary>
        public event EventHandler DocumentTrackerActivated;

        /// <summary>
        /// Occurs when the document tracker is deactivated.
        /// </summary>
        public event EventHandler DocumentTrackerDeactivated;

        /// <summary>
        /// Occurs when a solution is opened
        /// </summary>
        /// <remarks>Also occurs if AnkhSVN is loaded when a solution is already open</remarks>
        public event EventHandler SolutionOpened;

        /// <summary>
        /// Occurs when an open solution is closed
        /// </summary>
        public event EventHandler SolutionClosed;

        /// <summary>
        /// When the SCC provider was activated before the UI Shell was available, this event is raised
        /// to allow initiating services that really need the UI Shell
        /// </summary>
        public event EventHandler UIShellActivate;

        /// <summary>
        /// Raised when the Visual Studio or Windows theme changes
        /// </summary>
        public event EventHandler ThemeChanged;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LastChangedEventArgs> LastChanged;

        /// <summary>
        /// Raises the <see cref="E:RuntimeLoaded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal void OnRuntimeLoaded(EventArgs e)
        {
            if (RuntimeLoaded != null)
                RuntimeLoaded(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:RuntimeStarted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected internal void OnRuntimeStarted(EventArgs e)
        {
            if (RuntimeStarted != null)
                RuntimeStarted(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnSccProviderActivated(EventArgs e)
        {
            if (SccProviderActivated != null)
                SccProviderActivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnSccProviderDeactivated(EventArgs e)
        {
            if (SccProviderDeactivated != null)
                SccProviderDeactivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnGitSccProviderActivated(EventArgs e)
        {
            if (GitSccProviderActivated != null)
                GitSccProviderActivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnGitSccProviderDeactivated(EventArgs e)
        {
            if (GitSccProviderDeactivated != null)
                GitSccProviderDeactivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnDocumentTrackerActivated(EventArgs e)
        {
            if (DocumentTrackerActivated != null)
                DocumentTrackerActivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnDocumentTrackerDeactivated(EventArgs e)
        {
            if (DocumentTrackerDeactivated != null)
                DocumentTrackerDeactivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SolutionOpened"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnSolutionOpened(EventArgs e)
        {
            if (SolutionOpened != null)
                SolutionOpened(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SolutionClosed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnSolutionClosed(EventArgs e)
        {
            if (SolutionClosed != null)
                SolutionClosed(this, e);
        }

        /// <summary>
        /// Raises the <see cref="UIShellAvailable"/>
        /// </summary>
        /// <param name="e"></param>
        private void OnUIShellActivate(EventArgs e)
        {
            if (UIShellActivate != null)
                UIShellActivate(this, e);
        }

        private void OnThemeChanged(EventArgs e)
        {
            if (ThemeChanged != null)
                ThemeChanged(this, e);
        }

        protected void OnLastChanged(LastChangedEventArgs e)
        {
            if (LastChanged != null)
                LastChanged(this, e);
        }

        #region IAnkhServiceEvents Members

        /// <summary>
        /// Raises the <see cref="E:RuntimeLoaded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnRuntimeLoaded(EventArgs e)
        {
            OnRuntimeLoaded(e);
        }

        /// <summary>
        /// Raises the <see cref="E:RuntimeStarted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnRuntimeStarted(EventArgs e)
        {
            OnRuntimeStarted(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnSccProviderActivated(EventArgs e)
        {
            OnSccProviderActivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnSccProviderDeactivated(EventArgs e)
        {
            OnSccProviderDeactivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnGitSccProviderActivated(EventArgs e)
        {
            OnGitSccProviderActivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:GitSccProviderDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnGitSccProviderDeactivated(EventArgs e)
        {
            OnGitSccProviderDeactivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnDocumentTrackerActivated(EventArgs e)
        {
            OnDocumentTrackerActivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DocumentTrackerDeactivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnDocumentTrackerDeactivated(EventArgs e)
        {
            OnDocumentTrackerDeactivated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SolutionOpened"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnSolutionOpened(EventArgs e)
        {
            try
            {
                OnSolutionOpened(e);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SolutionClosed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void IAnkhServiceEvents.OnSolutionClosed(EventArgs e)
        {
            try
            {
                OnSolutionClosed(e);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        void IAnkhServiceEvents.OnUIShellActivate(EventArgs e)
        {
            try
            {
                OnUIShellActivate(e);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        void IAnkhServiceEvents.OnThemeChanged(EventArgs e)
        {
            try
            {
                OnThemeChanged(e);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        void IAnkhServiceEvents.OnLastChanged(LastChangedEventArgs e)
        {
            try
            {
                OnLastChanged(e);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion
    }
}
