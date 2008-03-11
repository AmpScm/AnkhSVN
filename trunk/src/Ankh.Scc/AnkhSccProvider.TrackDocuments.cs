using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
	partial class AnkhSccProvider : IVsTrackProjectDocuments2, IVsTrackProjectDocumentsEvents3
	{
        /// <summary>
        /// Advises the track project documents events.
        /// </summary>
        /// <param name="pEventSink">The p event sink.</param>
        /// <param name="pdwCookie">The PDW cookie.</param>
        /// <returns></returns>
        public int AdviseTrackProjectDocumentsEvents(IVsTrackProjectDocumentsEvents2 pEventSink, out uint pdwCookie)
        {
            pdwCookie = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int BeginBatch()
        {
            return VSConstants.S_OK;
        }

        public int EndBatch()
        {
            return VSConstants.S_OK;
        }

        public int Flush()
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectoriesEx(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFile(IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags, out int pfRenameCanContinue)
        {
            pfRenameCanContinue = 1;
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int UnadviseTrackProjectDocumentsEvents(uint dwCookie)
        {
            return VSConstants.E_NOTIMPL;
        }

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

        /// <summary>
        /// Indicates that a project is about start a batch query process.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnBeginQueryBatch()
        {
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
            pfActionOK = 1;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines if it is okay to add a collection of files (possibly from source control) whose final destination may be different from a source location.
        /// </summary>
        /// <param name="pProject">[in] Project making the request about adding files.</param>
        /// <param name="cFiles">[in] The number of files represented in the rgpszNewMkDocuments, rgpszSrcMkDocuments, rgFlags, and rgResults arrays.</param>
        /// <param name="rgpszNewMkDocuments">[in] An array of file names that indicate the files' final destination.</param>
        /// <param name="rgpszSrcMkDocuments">[in] An array of file names specifying the source location of the files.</param>
        /// <param name="rgFlags">[in] An array of values, one element for each file, from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILEFLAGS"></see> enumeration.</param>
        /// <param name="pSummaryResult">[out] Returns an overall status for all files as a value from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS"></see> enumeration.</param>
        /// <param name="rgResults">[out] An array that is to be filled in with the status of each file. Each status is a value from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS"></see> enumeration.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnQueryAddFilesEx(IVsProject pProject, int cFiles, string[] rgpszNewMkDocuments, string[] rgpszSrcMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }
    }
}
