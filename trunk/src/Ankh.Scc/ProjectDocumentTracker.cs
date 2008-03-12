using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
    interface IAnkhProjectDocumentTracker
    {
        void Hook(bool enable);
    }

    //[CLSCompliant(false)]
    partial class ProjectDocumentTracker : IAnkhProjectDocumentTracker, IVsTrackProjectDocumentsEvents2, IVsTrackProjectDocumentsEvents3
    {
        readonly AnkhContext _context;
        bool _hooked;
        uint _projectCookie;
        uint _documentCookie;
        readonly AnkhSccProvider _sccProvider;

        public ProjectDocumentTracker(AnkhContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _sccProvider = context.GetService<AnkhSccProvider>();

            Hook(true);
        }

        public void Hook(bool enable)
        {
            if (enable == _hooked)
                return;
            else if (enable)
            {
                IVsTrackProjectDocuments2 tracker = (IVsTrackProjectDocuments2)_context.GetService(typeof(SVsTrackProjectDocuments));

                if (tracker != null)
                    Marshal.ThrowExceptionForHR(tracker.AdviseTrackProjectDocumentsEvents(this, out _projectCookie));

                _hooked = true;

                IVsSolution solution = (IVsSolution)_context.GetService(typeof(SVsSolution));

                if (solution != null)
                    solution.AdviseSolutionEvents(this, out _documentCookie);
            }
            else 
            {
                IVsTrackProjectDocuments2 tracker = (IVsTrackProjectDocuments2)_context.GetService(typeof(SVsTrackProjectDocuments));

                if (tracker != null)
                    tracker.UnadviseTrackProjectDocumentsEvents(_projectCookie);
                _hooked = false;

                IVsSolution solution = (IVsSolution)_context.GetService(typeof(SVsSolution));

                if (solution != null)
                    solution.UnadviseSolutionEvents(_documentCookie);
            }
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
    }
}
