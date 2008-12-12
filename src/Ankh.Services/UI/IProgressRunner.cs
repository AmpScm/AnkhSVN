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
using SharpSvn;
using System.ComponentModel;

namespace Ankh
{
    public class ProgressWorkerArgs : EventArgs
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnClient _client;
        readonly ISynchronizeInvoke _sync;
        Exception _exception;

        public ProgressWorkerArgs(IAnkhServiceProvider context, SvnClient client, ISynchronizeInvoke sync)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _client = client;
            _sync = sync;
        }

        public SvnClient Client
        {
            get { return _client; }
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public ISynchronizeInvoke Synchronizer
        {
            get { return _sync; }
        }
    }

    public class ProgressRunnerResult
    {
        readonly bool _succeeded;
        readonly Exception _ex;

        public ProgressRunnerResult(bool succeeded)
        {
            _succeeded = succeeded;
        }

        public ProgressRunnerResult(bool succeeded, Exception e)
        {
            _ex = e;
        }

        public bool Succeeded
        {
            get { return _succeeded; }
        }

        public Exception Exception
        {
            get { return _ex; }
        }
    }

    public class ProgressWorkerDoneArgs : EventArgs
    {
        readonly ProgressRunnerResult _result;
        public ProgressWorkerDoneArgs(ProgressRunnerResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            _result = result;
        }

        public ProgressRunnerResult Result
        {
            get { return _result; }
        }
    }

    public interface IProgressRunner
    {
        /// <summary>
        /// Runs the specified action.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        ProgressRunnerResult RunModal(string caption, EventHandler<ProgressWorkerArgs> action);

        /// <summary>
        /// Runs the specified action and when the action completes completer. (Currently implemented synchronously!)
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="action">The action.</param>
        /// <param name="completer">The completer.</param>
        void RunNonModal(string caption, EventHandler<ProgressWorkerArgs> action, EventHandler<ProgressWorkerDoneArgs> completer);
    }
}
