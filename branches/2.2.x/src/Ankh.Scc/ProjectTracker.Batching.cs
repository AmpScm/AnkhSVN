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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class ProjectTracker
    {
        readonly List<String> _batchErrors = new List<string>();
        bool _batchOk;
        bool _inBatch;
        
        /// <summary>
        /// Indicates that a project is about start a batch query process.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnBeginQueryBatch()
        {
            _inBatch = _batchOk = true;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method is called to indicate that a batch query process has been canceled.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnCancelQueryBatch()
        {
            _inBatch = _batchOk = false;
            _batchErrors.Clear();
            RegisterForSccCleanup();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines whether it is okay to proceed with the actual batch operation after successful completion of a batch query process.
        /// </summary>
        /// <param name="pfActionOK">[out] Returns nonzero if it is okay to continue with the proposed batch process. Returns zero if the proposed batch process should not proceed.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnEndQueryBatch(out int pfActionOK)
        {
            if (_inBatch)
            {
                ShowQueryErrorDialog();
                pfActionOK = _batchOk ? 1 : 0;
                _inBatch = _batchOk = false;
            }
            else
                pfActionOK = 1; // What batch?

            RegisterForSccCleanup();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ShowQueryErrorDialog()
        {
            try
            {
                if (_batchErrors.Count > 0)
                {
                    Ankh.UI.AnkhMessageBox mb = new Ankh.UI.AnkhMessageBox(Context);

                    StringBuilder sb = new StringBuilder();

                    foreach (string message in _batchErrors)
                        sb.AppendLine(message);

                    mb.Show(sb.ToString());
                }
            }
            finally
            {
                _batchErrors.Clear();
            }
        }

        void AddBatchError(string message)
        {
            if (!_batchErrors.Contains(message))
                _batchErrors.Add(message);
        }
    }
}
