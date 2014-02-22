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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;
using SharpSvn;

using Ankh.Scc;
using Ankh.Scc.UI;

namespace Ankh.UI.SvnLog
{
    public sealed partial class LogToolWindowControl : AnkhToolWindowControl, ILogControl
    {
        string _originalText;
        IList<SvnOrigin> _origins;
        public LogToolWindowControl()
        {
            InitializeComponent();
            LogSource = logControl.LogSource;
        }

        public LogToolWindowControl(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        internal LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            _originalText = Text;

            ToolWindowHost.CommandContext = AnkhId.LogContextGuid;
            ToolWindowHost.KeyboardContext = AnkhId.LogContextGuid;
            logControl.Context = Context;
        }

        protected override void OnLoad(EventArgs e)
        {
            IServiceContainer container = Context.GetService<IServiceContainer>();

            if (container != null)
            {
                if (null == container.GetService(typeof(LogToolWindowControl)))
                    container.AddService(typeof(LogToolWindowControl), this);
            }

            base.OnLoad(e);
        }

        public IList<SvnOrigin> Origins
        {
            get
            {
                if (_origins == null)
                    return null;

                return new List<SvnOrigin>(_origins);
            }
        }

        Uri _baseUri;
        public Uri CommonBaseUri
        {
            get
            {
                if (_baseUri == null && _origins != null && _origins.Count > 0)
                {
                    Uri baseUri;
                    Uri[] uris = new Uri[_origins.Count];
                    int n = 0;
                    foreach(SvnOrigin o in _origins)
                        uris[n++] = o.Uri;

                    if (SvnTools.TryGetUriAncestor(uris, out baseUri))
                    {
                        _baseUri = baseUri;
                    }
                }
                return _baseUri;
            }
        }

        void UpdateTitle()
        {
            Text = _originalText;
            if (_origins == null)
                return;

            StringBuilder sb = new StringBuilder(Text);
            int n = 0;

            foreach (SvnOrigin origin in _origins)
            {
                sb.Append((n++ == 0) ? " - " : ", ");
                    
                sb.Append(origin.Target.FileName);
            }

            Text = sb.ToString();
        }

        public void StartLog(SvnOrigin target, SvnRevision start, SvnRevision end)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            StartLog(new SvnOrigin[] { target }, start, end);
        }

        SvnRevision _rqStart;
        SvnRevision _rqEnd;

        public SvnRevision LastStartRevision
        {
            get { return _rqStart; }
        }

        public SvnRevision LastEndRevision
        {
            get { return _rqEnd; }
        }

        public void StartLog(ICollection<SvnOrigin> targets, SvnRevision start, SvnRevision end)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            _origins = new List<SvnOrigin>(targets);
            _baseUri = null;
            _rqStart = start;
            _rqEnd = end;

            UpdateTitle();

            logControl.StartLog(_origins, start, end);
        }

        public void StartMergesEligible(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            SvnOrigin origin = new SvnOrigin(target);
            _origins = new SvnOrigin[] { origin };
            _baseUri = null;
            UpdateTitle();
            logControl.StartMergesEligible(context, origin, source);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            SvnOrigin origin = new SvnOrigin(target);
            _origins = new SvnOrigin[] { origin };
            _baseUri = null;
            UpdateTitle();
            logControl.StartMergesMerged(context, origin, source);
        }

        public bool LogMessageVisible
        {
            [DebuggerStepThrough]
            get { return logControl.ShowLogMessage; }
            [DebuggerStepThrough]
            set { logControl.ShowLogMessage = value; }
        }

        public bool ChangedPathsVisible
        {
            [DebuggerStepThrough]
            get { return logControl.ShowChangedPaths; }
            [DebuggerStepThrough]
            set { logControl.ShowChangedPaths = value; }
        }

        public bool IncludeMerged
        {
            [DebuggerStepThrough]
            get { return logControl.IncludeMergedRevisions; }
            [DebuggerStepThrough]
            set { logControl.IncludeMergedRevisions = value; }
        }

        [DebuggerStepThrough]
        public void FetchAll()
        {
            logControl.FetchAll();
        }

        [DebuggerStepThrough]
        public void Restart()
        {
            logControl.Restart();
        }

        #region ILogControl Members

        public bool ShowChangedPaths
        {
            get
            {
                return logControl.ShowChangedPaths;
            }
            set
            {
                logControl.ShowChangedPaths = value;
            }
        }

        public bool ShowLogMessage
        {
            get
            {
                return logControl.ShowLogMessage;
            }
            set
            {
                logControl.ShowLogMessage = value;
            }
        }

        public bool IncludeMergedRevisions
        {
            get { return logControl.IncludeMergedRevisions; }
            set
            {
                if (value != logControl.IncludeMergedRevisions)
                {
                    logControl.IncludeMergedRevisions = value;
                    logControl.Restart();
                }
            }
        }

        public bool StopOnCopy
        {
            [DebuggerStepThrough]
            get { return logControl.StopOnCopy; }
            set
            {
                if (value != StopOnCopy)
                {
                    logControl.StopOnCopy = value;
                    logControl.Restart();
                }
            }
        }

        #endregion
    }
}
