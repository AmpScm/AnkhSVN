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
using SharpSvn;
using System.IO;
using System.Diagnostics;

namespace Ankh.Scc
{
    partial class ProjectTracker
    {
        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            if (rgszMkNewNames == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            RegisterForSccCleanup(); // Clear the origins
            _collectHints = true;

            for (int i = 0; i < cFiles; i++)
            {
                if (rgResults != null)
                    rgResults[i] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK;

                string newDoc = rgszMkNewNames[i];
                string origDoc = rgszMkOldNames[i];

                if (!SccProvider.IsSafeSccPath(newDoc))
                    continue;
                else if (!SccProvider.IsSafeSccPath(origDoc))
                    continue;

                // Save the origins of the to be added files as they are not available in the added event
                newDoc = SvnTools.GetNormalizedFullPath(newDoc);
                origDoc = SvnTools.GetNormalizedFullPath(origDoc);

                if (newDoc != origDoc)
                    _fileOrigins[newDoc] = origDoc;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK;

            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            if (rgszMkNewNames == null || rgpProjects == null || rgszMkOldNames == null || rgszMkOldNames.Length != rgszMkNewNames.Length)
                return VSConstants.E_POINTER;

            // TODO: C++ projects do not send directory renames; but do send OnAfterRenameFile() events
            //       for all files (one at a time). We should detect that case here and fix up this dirt!

            for (int i = 0; i < cFiles; i++)
            {
                string s = rgszMkOldNames[i];
                if (SvnItem.IsValidPath(s))
                    StatusCache.MarkDirty(s);

                s = rgszMkNewNames[i];
                if (SvnItem.IsValidPath(s))
                    StatusCache.MarkDirty(s);
            }

            if (!SccProvider.IsActive)
                return VSConstants.S_OK;

            ProcessRenames(rgszMkOldNames, rgszMkNewNames);

            for (int iProject = 0, iFile = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                if (rgpProjects[iProject] != null)
                {
                    IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                    bool track = SccProvider.TrackProjectChanges(sccProject);

                    for (; iFile < iLastFileThisProject; iFile++)
                    {
                        if (sccProject == null || !track)
                            continue; // Not handled by our provider

                        if (!SvnItem.IsValidPath(rgszMkOldNames[iFile]))
                            continue;

                        string oldName = SvnTools.GetNormalizedFullPath(rgszMkOldNames[iFile]);
                        string newName = SvnTools.GetNormalizedFullPath(rgszMkNewNames[iFile]);

                        if (oldName == newName)
                            continue;

                        SccProvider.OnProjectRenamedFile(sccProject, oldName, newName, rgFlags[iFile]);
                    }
                }
                else
                {
                    // Renaming something in the solution (= solution file itself)
                    for (; iFile < iLastFileThisProject; iFile++)
                    {
                        if (!SvnItem.IsValidPath(rgszMkOldNames[iFile]))
                            continue;

                        string oldName = SvnTools.GetNormalizedFullPath(rgszMkOldNames[iFile]);
                        string newName = SvnTools.GetNormalizedFullPath(rgszMkNewNames[iFile]);

                        if (oldName == newName)
                            continue;

                        SccProvider.OnSolutionRenamedFile(oldName, newName);
                    }
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Fixes working copies which are invalidated by a rename operation
        /// </summary>
        /// <param name="rgszMkOldNames"></param>
        /// <param name="rgszMkNewNames"></param>
        private void ProcessRenames(string[] rgszMkOldNames, string[] rgszMkNewNames)
        {
            if (rgszMkNewNames == null || rgszMkOldNames == null || rgszMkOldNames.Length != rgszMkNewNames.Length)
                return;

            for (int i = 0; i < rgszMkOldNames.Length; i++)
            {
                string oldName = rgszMkOldNames[i];
                string newName = rgszMkNewNames[i];

                if (!SvnItem.IsValidPath(oldName))
                    continue;
                if (!SvnItem.IsValidPath(newName))
                    continue;

                oldName = SvnTools.GetNormalizedFullPath(oldName);
                newName = SvnTools.GetNormalizedFullPath(newName);

                if (!MaybeProcessAncestorRename(oldName, newName))
                    ProcessRename(oldName, newName);
            }
        }

        private bool MaybeProcessAncestorRename(string oldName, string newName)
        {
            string oldNode = SvnTools.GetNormalizedDirectoryName(oldName);
            string newNode = SvnTools.GetNormalizedDirectoryName(newName);

            if (oldNode == null || newNode == null)
                return false;

            if (_alreadyProcessed.Contains(newNode))
                return true;

            if (MaybeProcessAncestorRename(oldNode, newNode))
                return true;

            if (!_fileOrigins.ContainsKey(newNode))
                return false;

            return ProcessRename(oldNode, newNode);
        }

        private bool ProcessRename(string oldName, string newName)
        {
            if (_alreadyProcessed.Contains(newName))
                return true;

            SvnItem old = StatusCache[oldName];
            SvnItem nw = StatusCache[newName];

            if (old.IsVersioned && !old.IsDeleteScheduled
                && nw.IsVersionable
                && (!nw.IsVersioned || nw.IsDeleteScheduled))
            {
                SvnItem opp = nw.Parent;

                using (SvnSccContext ctx = new SvnSccContext(this))
                {
                    if (opp != null && !opp.IsVersioned)
                        ctx.AddParents(newName);

                    ctx.MarkAsMoved(oldName, newName);
                    StatusCache.MarkDirtyRecursive(newName);
                    StatusCache.MarkDirtyRecursive(oldName);

                    _alreadyProcessed.Add(newName);

                    return true;
                }
            }
            return false;
        }


        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            if (rgszMkNewNames == null || pProject == null || rgszMkOldNames == null)
                return VSConstants.E_POINTER;

            RegisterForSccCleanup(); // Clear the origins
            _collectHints = true;

            for (int i = 0; i < cDirs; i++)
            {
                if (rgResults != null)
                    rgResults[i] = VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK;

                string newDoc = rgszMkNewNames[i];
                string origDoc = rgszMkOldNames[i];

                if (!SccProvider.IsSafeSccPath(newDoc))
                    continue;
                else if (!SccProvider.IsSafeSccPath(origDoc))
                    continue;

                // Save the origins of the to be added files as they are not available in the added event
                newDoc = SvnTools.GetNormalizedFullPath(newDoc);
                origDoc = SvnTools.GetNormalizedFullPath(origDoc);

                if (newDoc != origDoc)
                    _fileOrigins[newDoc] = origDoc;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK;

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

                bool track = SccProvider.TrackProjectChanges(sccProject);

                for (; iDirectory < iLastDirectoryThisProject; iDirectory++)
                {
                    if (sccProject == null || !track)
                        continue; // Not handled by our provider

                    SccProvider.OnProjectDirectoryRenamed(sccProject,
                        SvnTools.GetNormalizedFullPath(rgszMkOldNames[iDirectory]),
                        SvnTools.GetNormalizedFullPath(rgszMkNewNames[iDirectory]), rgFlags[iDirectory]);
                }
            }

            return VSConstants.S_OK;
        }
    }
}
