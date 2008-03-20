using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class ProjectTracker
    {
        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            if (rgResults != null)
                for (int i = 0; i < cFiles; i++)
                {
                    rgResults[i] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;
                }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            int iFile = 0;

            for (int iProject = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                IVsProject project = rgpProjects[iProject];
                IVsSccProject2 sccProject = project as IVsSccProject2;

                for (; iFile < iLastFileThisProject; iFile++)
                {
                    if (sccProject == null)
                        continue; // Not handled by our provider

                    string file = rgpszMkDocuments[iFile];

                    bool wasDeleted = System.IO.File.Exists(file);

                    _sccProvider.OnProjectFileRemoved(sccProject, file, wasDeleted, rgFlags[iFile]);
                }
            }
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            if (rgResults != null)
                for (int i = 0; i < cDirectories; i++)
                {
                    rgResults[i] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;
                }
            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
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

                    _sccProvider.OnProjectDirectoryRemoved(sccProject, rgpszMkDocuments[iDirectory], rgFlags[iDirectory]);
                }
            }

            return VSConstants.S_OK;
        }
    }
}
