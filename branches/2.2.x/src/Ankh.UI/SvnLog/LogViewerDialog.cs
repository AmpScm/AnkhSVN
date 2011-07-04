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
using Ankh.Scc;
using System.Diagnostics;
using Ankh.Scc.UI;

namespace Ankh.UI.SvnLog
{
    public sealed partial class LogViewerDialog : VSContainerForm, ILogControl
    {
        private SvnOrigin _logTarget;

        public LogViewerDialog()
        {
            InitializeComponent();
        }

        public LogViewerDialog(SvnOrigin target)
            : this()
        {
            LogTarget = target;
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            logViewerControl.Context = Context;
        }

        /// <summary>
        /// Gets an instance of the <code>LogControl</code>.
        /// </summary>
        internal LogControl LogControl
        {
            get { return logViewerControl; }
        }

        /// <summary>
        /// The target of the log.
        /// </summary>
        public SvnOrigin LogTarget
        {
            [DebuggerStepThrough]
            get { return _logTarget; }
            set { _logTarget = value; }
        }

        public IEnumerable<ISvnLogItem> SelectedItems
        {
            get { return logViewerControl.SelectedItems; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;
        
            if (LogTarget == null)
                throw new InvalidOperationException("Log target is null");

            logViewerControl.StartLog(new SvnOrigin[] { LogTarget }, null, null);
        }

        #region ILogControl Members

        public bool ShowChangedPaths
        {
            get { return LogControl.ShowChangedPaths; }
            set { LogControl.ShowChangedPaths = value; }
        }

        public bool ShowLogMessage
        {
            get { return LogControl.ShowLogMessage; }
            set { LogControl.ShowLogMessage = value; }
        }

        public bool StrictNodeHistory
        {
            get { return LogControl.StrictNodeHistory; }
            set { LogControl.StrictNodeHistory = value; }
        }

        public bool IncludeMergedRevisions
        {
            get { return LogControl.IncludeMergedRevisions; }
            set { LogControl.IncludeMergedRevisions = value; }
        }

        public void FetchAll()
        {
            LogControl.FetchAll();
        }

        public void Restart()
        {
            LogControl.Restart();
        }

        public IList<SvnOrigin> Origins
        {
            get { return new SvnOrigin[] { LogTarget }; }
        }

        #endregion
    }
}