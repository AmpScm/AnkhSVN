using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class ProjectDocumentTracker
    {
        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            IVsSccProject2 sccProject = pProject as IVsSccProject2;

            if (sccProject == null)
                return VSConstants.S_OK; // Not for us

            bool allOk = true;
            for (int i = 0; i < cFiles; i++)
            {
                bool ok;
                
                _sccProvider.OnBeforeProjectRenameFile(sccProject, rgszMkOldNames[i], rgszMkNewNames[i], rgFlags[i], out ok);

                rgResults[i] = ok ? VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK : VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;

                if (!ok)
                    allOk = false;
            }

            pSummaryResult[0] = allOk ? VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK : VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;

            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
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

                    _sccProvider.OnProjectRenameFile(sccProject, rgszMkOldNames[iFile], rgszMkNewNames[iFile], rgFlags[iFile]);
                }
            }

            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            IVsSccProject2 sccProject = pProject as IVsSccProject2;

            if (sccProject == null)
                return VSConstants.S_OK; // Not for us

            bool allOk = true;
            for(int i = 0; i < cDirs; i++)
            {
                bool ok;
                
                _sccProvider.OnBeforeProjectDirectoryRename(sccProject, rgszMkOldNames[i], rgszMkNewNames[i], rgFlags[i], out ok);

                rgResults[i] = ok ? VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK : VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK;

                if (!ok)
                    allOk = false;
            }

            pSummaryResult[0] = allOk ? VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK : VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK;

            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            int iDirectory = 0;

            for (int iProject = 0; (iProject < cProjects) && (iDirectory < cDirs); iProject++)
            {
                int iLastDirectoryThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cDirs;

                IVsProject project = rgpProjects[iProject];
                IVsSccProject2 sccProject = project as IVsSccProject2;

                for (; iDirectory < iLastDirectoryThisProject; iDirectory++)
                {
                    if (sccProject == null)
                        continue; // Not handled by our provider

                    _sccProvider.OnProjectDirectoryRename(sccProject, rgszMkOldNames[iDirectory], rgszMkNewNames[iDirectory], rgFlags[iDirectory]);
                }
            }

            return VSConstants.S_OK;
        }
    }
}
