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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Commands;

namespace Ankh.Scc
{
    interface IAnkhProjectDocumentTracker
    {
        void Hook(bool enable);
    }

    //[CLSCompliant(false)]
    [GlobalService(typeof(IAnkhProjectDocumentTracker))]
    partial class ProjectTracker : AnkhService, IAnkhProjectDocumentTracker, IVsTrackProjectDocumentsEvents2, IVsTrackProjectDocumentsEvents3
    {
        bool _hooked;
        uint _projectCookie;
        uint _documentCookie;
        AnkhSccProvider _sccProvider;
        AnkhSccSettingStorage _sccStore;
        bool _collectHints;
        bool _solutionLoaded;
        readonly HybridCollection<string> _fileHints = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        readonly SortedList<string, string> _fileOrigins;

        public ProjectTracker(IAnkhServiceProvider context)
            : base(context)
        {
            _fileOrigins = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Hook(true);
            LoadInitial();
        }

        AnkhSccProvider SccProvider
        {
            [DebuggerStepThrough]
            get { return _sccProvider ?? (_sccProvider = GetService<AnkhSccProvider>()); }
        }

        AnkhSccSettingStorage SccStore
        {
            [DebuggerStepThrough]
            get { return _sccStore ?? (_sccStore = GetService<AnkhSccSettingStorage>(typeof(ISccSettingsStore))); }
        }

        private void LoadInitial()
        {
            IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));

            if (solution == null)
                return;

            string dir, file, user;
            if (!ErrorHandler.Succeeded(solution.GetSolutionInfo(out dir, out file, out user))
                || string.IsNullOrEmpty(file))
            {
                return; // No solution loaded, nothing to load
            }

            Guid none = Guid.Empty;
            IEnumHierarchies hierEnum;
            if (!ErrorHandler.Succeeded(solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref none, out hierEnum)))
                return;

            IVsHierarchy[] hiers = new IVsHierarchy[32];
            uint nFetched;
            while (ErrorHandler.Succeeded(hierEnum.Next((uint)hiers.Length, hiers, out nFetched)))
            {
                if (nFetched == 0)
                    break;
                for (int i = 0; i < nFetched; i++)
                {
                    IVsSccProject2 p2 = hiers[i] as IVsSccProject2;

                    if (p2 != null)
                        SccProvider.OnProjectOpened(p2, false);
                }
            }

            _solutionLoaded = true;
            SccProvider.OnSolutionOpened(true);
        }

        public void Hook(bool enable)
        {
            if (enable == _hooked)
                return;

            IVsTrackProjectDocuments2 tracker = (IVsTrackProjectDocuments2)Context.GetService(typeof(SVsTrackProjectDocuments));
            IVsSolution solution = (IVsSolution)Context.GetService(typeof(SVsSolution));
            if (enable)
            {
                if (tracker != null)
                    Marshal.ThrowExceptionForHR(tracker.AdviseTrackProjectDocumentsEvents(this, out _projectCookie));

                _hooked = true;

                if (solution != null)
                    solution.AdviseSolutionEvents(this, out _documentCookie);
            }
            else
            {
                if (tracker != null)
                    tracker.UnadviseTrackProjectDocumentsEvents(_projectCookie);

                _hooked = false;

                if (solution != null)
                    solution.UnadviseSolutionEvents(_documentCookie);
            }
        }

        IFileStatusCache StatusCache
        {
            get { return GetService<IFileStatusCache>(); }
        }

        #region IVsTrackProjectDocumentsEvents2 Members

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        #endregion

        /// <summary>
        /// Accesses a specified set of files and asks all implementers of this method to release any locks that may exist on those files.
        /// </summary>
        /// <param name="grfRequiredAccess">[in] A value from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__HANDSOFFMODE"></see> enumeration, indicating the type of access requested. This can be used to optimize the locks that actually need to be released.</param>
        /// <param name="cFiles">[in] The number of files in the rgpszMkDocuments array.</param>
        /// <param name="rgpszMkDocuments">[in] If there are any locks on this array of file names, the caller wants them to be released.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int HandsOffFiles(uint grfRequiredAccess, int cFiles, string[] rgpszMkDocuments)
        {
            if (_collectHints && rgpszMkDocuments != null)
            {
                // Some projects call HandsOffFiles of files they want to add. Use that to collect extra origin information
                foreach (string file in rgpszMkDocuments)
                {
                    if (!SccProvider.IsSafeSccPath(file))
                        continue;

                    string fullFile = SvnTools.GetNormalizedFullPath(file);
                    if (!_fileHints.Contains(fullFile))
                        _fileHints.Add(fullFile);
                }
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called when a project has completed operations on a set of files.
        /// </summary>
        /// <param name="cFiles">[in] Number of file names given in the rgpszMkDocuments array.</param>
        /// <param name="rgpszMkDocuments">[in] An array of file names.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int HandsOnFiles(int cFiles, string[] rgpszMkDocuments)
        {
            return VSConstants.S_OK;
        }

        readonly List<string> _delayedDeletes = new List<string>();

        bool _registeredSccCleanup;
        internal void OnSccCleanup(CommandEventArgs e)
        {
            _registeredSccCleanup = false;
            _collectHints = false;

            _fileHints.Clear();
            _fileOrigins.Clear();

            if (_delayedDeletes.Count > 0)
            {
                using (SvnSccContext svn = new SvnSccContext(Context))
                {
                    foreach (string d in _delayedDeletes.ToArray())
                    {
                        _delayedDeletes.Remove(d);
                        svn.WcDelete(d);
                    }
                }
            }
        }

        void RegisterForSccCleanup()
        {
            if (_registeredSccCleanup)
                return;

            Context.GetService<IAnkhCommandService>().PostTickCommand(ref _registeredSccCleanup, AnkhCommand.SccFinishTasks);
        }
    }
}
