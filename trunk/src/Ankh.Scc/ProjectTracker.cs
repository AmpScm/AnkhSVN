﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Ankh.Commands;
using Ankh.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using System.IO;

namespace Ankh.Scc
{
    interface IAnkhProjectDocumentTracker
    {
        void Hook(bool enable);
    }

    //[CLSCompliant(false)]
    partial class ProjectTracker : AnkhService, IAnkhProjectDocumentTracker, IVsTrackProjectDocumentsEvents2, IVsTrackProjectDocumentsEvents3
    {
        bool _hooked;
        uint _projectCookie;
        uint _documentCookie;
        readonly AnkhSccProvider _sccProvider;
        bool _collectHints;
        readonly List<string> _fileHints = new List<string>();
        readonly SortedList<string, string> _fileOrigins;

        public ProjectTracker(AnkhContext context)
            : base(context)
        {
            _sccProvider = context.GetService<AnkhSccProvider>();
            _fileOrigins = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);

            Hook(true);
            LoadInitial();
        }

        private void LoadInitial()
        {
            IVsSolution solution = (IVsSolution)Context.GetService(typeof(SVsSolution));

            if(solution == null)
                return;

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

                    if(p2 != null)
                        _sccProvider.OnProjectOpened(p2, false);
                }                
            }

            _sccProvider.OnSolutionOpened(true);
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
                    string fullFile = Path.GetFullPath(file);
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

            IAnkhCommandService cmd = Context.GetService<IAnkhCommandService>();

            if (cmd != null && cmd.PostExecCommand(AnkhCommand.SccFinishTasks))
                _registeredSccCleanup = true;
        }
    }
}
