using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class ProjectDocumentTracker
    {
        /// <summary>
        /// This method notifies the client when a project has requested to add files.
        /// </summary>
        /// <param name="pProject"></param>
        /// <param name="cFiles"></param>
        /// <param name="rgpszMkDocuments"></param>
        /// <param name="rgFlags"></param>
        /// <param name="pSummaryResult"></param>
        /// <param name="rgResults"></param>
        /// <returns></returns>
        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            // For now, we allow adding all files as is
            // We might propose moving files to within a managed root

            for (int i = 0; i < cFiles; i++)
            {
                rgResults[i] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
            }

            pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK; // All ok

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
        /// <remarks>Deny a query only if allowing the operation would compromise your stable state</remarks>
        public int OnQueryAddFilesEx(IVsProject pProject, int cFiles, string[] rgpszNewMkDocuments, string[] rgpszSrcMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            // For now, we allow adding all files as is
            // We might propose moving files to within a managed root

            for (int i = 0; i < cFiles; i++)
            {
                rgResults[i] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
            }

            pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK; // All ok
            
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method notifies the client after a project has added files.
        /// </summary>
        /// <param name="cProjects">[in] Number of projects to which files were added.</param>
        /// <param name="cFiles">[in] Number of files that were added.</param>
        /// <param name="rgpProjects">[in] Array of projects to which files were added.</param>
        /// <param name="rgFirstIndices">[in] Array of first indices identifying which project each file belongs to. For more information, see IVsTrackProjectDocumentsEvents2.</param>
        /// <param name="rgpszMkDocuments">[in] Array of paths for the files that were processed. This is the same size as cFiles.</param>
        /// <param name="rgFlags">[in] Array of flags. For a list of rgFlags values, see <see cref="VSADDFILEFLAGS" />.</param>
        /// <returns></returns>
        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            int iFile = 0;

            for (int iProject = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                IVsProject project = rgpProjects[iProject];
                IVsSccProject2 sccProject = project as IVsSccProject2;

                for (; iFile < iLastFileThisProject; iFile++)
                {
                    if(sccProject == null)
                        continue; // Not handled by our provider

                    _sccProvider.OnProjectFileAdded(sccProject, rgpszMkDocuments[iFile], rgFlags[iFile]);
                }
            }
            return VSConstants.S_OK;
        }        

        /// <summary>
        /// This method notifies the client when a project has requested to add directories.
        /// </summary>
        /// <param name="pProject">[in] Project to which the directories will be added.</param>
        /// <param name="cDirectories">[in] Number of directories to add.</param>
        /// <param name="rgpszMkDocuments">[in] Array of paths for the directories to add.</param>
        /// <param name="rgFlags">[in] Flags identifying information about each directory. For a list of rgFlags values, see <see cref=">VSQUERYADDDIRECTORYFLAGS"/.</param>
        /// <param name="pSummaryResult">[out] Summary result object. This object is a summation of the yes and no results for the array of directories passed in rgpszMkDocuments.
        /// If the result for a single directory is no, then pSummaryResult is equal to VSQUERYADDDIRECTORYRESULTS_AddNotOK; if the results for all directories are yes, then pSummaryResult is equal to VSQUERYADDDIRECTORYRESULTS_AddOK. For a list of pSummaryResult values, see VSQUERYADDDIRECTORYRESULTS.</param>
        /// <param name="rgResults">[out] Array of results. For a list of rgResults values, see VSQUERYADDDIRECTORYRESULTS.</param>
        /// <returns></returns>
        /// <remarks>Deny a query only if allowing the operation would compromise your stable state</remarks>
        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            for (int i = 0; i < cDirectories; i++)
            {
                rgResults[i] = VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK;
            }
            
            pSummaryResult[0] = VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK; // All ok

            return VSConstants.S_OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cProjects"></param>
        /// <param name="cDirectories"></param>
        /// <param name="rgpProjects"></param>
        /// <param name="rgFirstIndices"></param>
        /// <param name="rgpszMkDocuments"></param>
        /// <param name="rgFlags"></param>
        /// <returns></returns>
        /// <remarks>Deny a query only if allowing the operation would compromise your stable state</remarks>
        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            int iDirectory = 0;

            for (int iProject = 0; (iProject < cProjects) && (iDirectory < cDirectories); iProject++)
            {
                int iLastDirectoryThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cDirectories;

                IVsProject project = rgpProjects[iProject];
                IVsSccProject2 sccProject = project as IVsSccProject2;

                for (; iDirectory < iLastDirectoryThisProject; iDirectory++)
                {
                    if (sccProject == null)
                        continue; // Not handled by our provider

                    _sccProvider.OnProjectDirectoryAdded(sccProject, rgpszMkDocuments[iDirectory], rgFlags[iDirectory]);
                }
            }

            return VSConstants.S_OK;
        }        
    }
}
