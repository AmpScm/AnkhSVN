﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class ProjectTracker
    {
        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            if (rgszMkNewNames == null || pProject == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            IVsSccProject2 sccProject = pProject as IVsSccProject2;
            bool track = _sccProvider.TrackProjectChanges(sccProject);

            if(track)
                for (int i = 0; i < cFiles; i++)
                {
                    string s = rgszMkNewNames[i];
                    if (!string.IsNullOrEmpty(s))
                        StatusCache.MarkDirty(s);
                }

            bool allOk = true;
            for (int i = 0; i < cFiles; i++)
            {
                bool ok = true;

                if(track)
                    _sccProvider.OnBeforeProjectRenameFile(sccProject, rgszMkOldNames[i], rgszMkNewNames[i], rgFlags[i], out ok);

                if (rgResults != null)
                    rgResults[i] = ok ? VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK : VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;

                if (!ok)
                    allOk = false;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = allOk ? VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK : VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;

            if (!allOk)
                _batchOk = false;

            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            if (rgszMkNewNames == null || rgpProjects == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            int iFile = 0;

            for (int i = 0; i < cFiles; i++)
            {
                string s = rgszMkOldNames[i];
                if (!string.IsNullOrEmpty(s))
                    StatusCache.MarkDirty(s);

                s = rgszMkNewNames[i];
                if (!string.IsNullOrEmpty(s))
                    StatusCache.MarkDirty(s);
            }

            for (int iProject = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool track = _sccProvider.TrackProjectChanges(sccProject);

                for (; iFile < iLastFileThisProject; iFile++)
                {
                    if (sccProject == null || !track)
                        continue; // Not handled by our provider

                    _sccProvider.OnProjectRenamedFile(sccProject, rgszMkOldNames[iFile], rgszMkNewNames[iFile], rgFlags[iFile]);
                }
            }

            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            if (rgszMkNewNames == null || pProject == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            IVsSccProject2 sccProject = pProject as IVsSccProject2;
            bool track = _sccProvider.TrackProjectChanges(sccProject);

            if(track)
                for (int i = 0; i < cDirs; i++)
                {
                    string s = rgszMkNewNames[i];
                    if (!string.IsNullOrEmpty(s))
                        StatusCache.MarkDirty(s);
                }

            bool allOk = true;
            for (int i = 0; i < cDirs; i++)
            {
                bool ok = true;

                if(track)
                    _sccProvider.OnBeforeProjectDirectoryRename(sccProject, rgszMkOldNames[i], rgszMkNewNames[i], rgFlags[i], out ok);

                if (rgResults != null)
                    rgResults[i] = ok ? VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK : VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK;

                if (!ok)
                    allOk = false;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = allOk ? VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK : VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK;

            if (!allOk)
                _batchOk = false;

            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            if (rgszMkNewNames == null || rgpProjects == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            int iDirectory = 0;

            for (int i = 0; i < cDirs; i++)
            {
                string s = rgszMkOldNames[i];
                if (!string.IsNullOrEmpty(s))
                    StatusCache.MarkDirty(s);

                s = rgszMkNewNames[i];
                if (!string.IsNullOrEmpty(s))
                    StatusCache.MarkDirty(s);
            }

            for (int iProject = 0; (iProject < cProjects) && (iDirectory < cDirs); iProject++)
            {
                int iLastDirectoryThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cDirs;
                
                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool track = _sccProvider.TrackProjectChanges(sccProject);

                for (; iDirectory < iLastDirectoryThisProject; iDirectory++)
                {
                    if (sccProject == null || !track)
                        continue; // Not handled by our provider

                    _sccProvider.OnProjectDirectoryRenamed(sccProject, rgszMkOldNames[iDirectory], rgszMkNewNames[iDirectory], rgFlags[iDirectory]);
                }
            }

            return VSConstants.S_OK;
        }
    }
}
