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
using System.IO;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;
using Ankh.Selection;
using Clipboard = System.Windows.Forms.Clipboard;
using IDataObject = System.Windows.Forms.IDataObject;

namespace Ankh.Scc
{
    partial class ProjectTracker
    {
        ISelectionContext _selectionContext;
        ISelectionContext SelectionContext
        {
            [DebuggerStepThrough]
            get { return _selectionContext ?? (_selectionContext = GetService<ISelectionContext>()); }
        }

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
            if (rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            RegisterForSccCleanup(); // Clear the origins
            _collectHints = true; // Some projects call HandsOff(file) on which files they wish to import. Use that to get more information

            for (int i = 0; i < cFiles; i++)
            {
                if (rgResults != null)
                    rgResults[i] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;

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

            RegisterForSccCleanup(); // Clear the origins table after adding
            _collectHints = true; // Some projects call HandsOff(file) on which files they wish to import. Use that to get more information

            for (int i = 0; i < cFiles; i++)
            {
                rgResults[i] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;

                string newDoc = rgpszNewMkDocuments[i];
                string origDoc = rgpszSrcMkDocuments[i];

                if (!SccProvider.IsSafeSccPath(newDoc))
                    continue;
                else if (!SccProvider.IsSafeSccPath(origDoc))
                    continue;

                // Save the origins of the to be added files as they are not available in the added event
                newDoc = SvnTools.GetNormalizedFullPath(newDoc);
                origDoc = SvnTools.GetNormalizedFullPath(origDoc);

                // Some additions (e.g. drag&drop in C# project report the same locations. We use the hints later, but collect the targets anyway
                if (newDoc != origDoc)
                    _fileOrigins[newDoc] = origDoc;
                else if (!_fileOrigins.ContainsKey(newDoc))
                    _fileOrigins[newDoc] = null;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK;

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
            RegisterForSccCleanup(); // Clear the origins table after adding

            SortedList<string, string> copies = null;

            bool sccActive = SccProvider.IsActive;

            for (int iProject = 0, iFile = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool trackCopies;
                bool track = SccProvider.TrackProjectChanges(sccProject, out trackCopies);

                for (; iFile < iLastFileThisProject; iFile++)
                {
                    if (!track)
                        continue; // Not handled by our provider

                    string origin = null;
                    string newName = rgpszMkDocuments[iFile];

                    if (!SvnItem.IsValidPath(newName))
                        continue;

                    newName = SvnTools.GetNormalizedFullPath(newName);

                    if (sccActive && _solutionLoaded)
                    {
                        StatusCache.MarkDirty(newName);
                        TryFindOrigin(newName, out origin);
                    }

                    // We do this before the copies to make sure a failed copy doesn't break the project
                    SccProvider.OnProjectFileAdded(sccProject, newName, origin, rgFlags[iFile]);

                    if (sccActive && trackCopies &&
                        !string.IsNullOrEmpty(origin) &&
                        StatusCache[origin].HasCopyableHistory)
                    {
                        if (copies == null)
                            copies = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);

                        copies[newName] = origin;
                    }
                }
            }

            if (copies != null)
                using (SvnSccContext svn = new SvnSccContext(Context))
                {
                    foreach (KeyValuePair<string, string> kv in copies)
                    {
                        svn.AddParents(kv.Key);
                        svn.MetaCopy(kv.Value, kv.Key, true);
                    }
                }

            return VSConstants.S_OK;
        }

        private void TryFindOrigin(string newName, out string origin)
        {
            if (_fileOrigins.TryGetValue(newName, out origin))
            {
                if (origin != null)
                    return;
            }
            else
                _fileOrigins.Add(newName, null);

            // We haven't got the project file origin for free via OnQueryAddFilesEx
            // So:
            //  1 - The file is really new or
            //  2 - The file is drag&dropped into the project from the solution explorer or
            //  3 - The file is copy pasted into the project from an other project or
            //  4 - The file is added via add existing item or
            //  5 - The file is added via drag&drop from another application (OLE drop)
            //
            // The only way to determine is walking through these options
            SortedList<string, string> nameToItem = new SortedList<string, string>();

            nameToItem[Path.GetFileName(newName)] = newName;
            foreach (KeyValuePair<string, string> kv in _fileOrigins)
            {
                if (kv.Value != null)
                    continue;

                nameToItem[Path.GetFileName(kv.Key)] = kv.Key;
            }

            // 2 -  If the file is drag&dropped in the solution explorer
            //      the current selection is still the original selection

            // **************** Check the current selection *************
            // Checks for drag&drop actions. The selection contains the original list of files
            // BH: resx files are not correctly included if we don't retrieve this list recursive
            foreach (string file in SelectionContext.GetSelectedFiles(true))
            {
                if (_fileOrigins.ContainsKey(file))
                    continue;

                string name = Path.GetFileName(file);
                if (nameToItem.ContainsKey(name))
                {
                    string item = nameToItem[name];

                    CheckForMatch(item, file);
                }
            }

            // **************** Check the clipboard *********************
            // 3 - Copy & Paste in the solution explorer:
            //     The original hierarchy information is still on the clipboard
            IDataObject dataObject;
            string projectItemType;
            if (null != (dataObject = Clipboard.GetDataObject()) && SolutionExplorerClipboardItem.CanRead(dataObject, out projectItemType))
            {
                IVsSolution solution = GetService<IVsSolution>(typeof(SVsSolution));
                ISccProjectWalker walker = GetService<ISccProjectWalker>();

                foreach (string projref in SolutionExplorerClipboardItem.DecodeProjectItemData(dataObject, projectItemType))
                {
                    IVsHierarchy project;
                    uint itemid;

                    {
                        string updatedRef;
                        VSUPDATEPROJREFREASON[] updateReason = new VSUPDATEPROJREFREASON[1];
                        if (!ErrorHandler.Succeeded(solution.GetItemOfProjref(projref, out project, out itemid, out updatedRef, updateReason)))
                            continue;
                    }

                    foreach(string rawFile in walker.GetSccFiles(project, itemid, ProjectWalkDepth.AllDescendantsInHierarchy, null))
                    {
                        if (!SvnItem.IsValidPath(rawFile))
                            continue;

                        string file = SvnTools.GetNormalizedFullPath(rawFile);

                        if (_fileOrigins.ContainsKey(file))
                            continue;

                        string name = Path.GetFileName(file);
                        if (nameToItem.ContainsKey(name))
                        {
                            string item = nameToItem[name];

                            CheckForMatch(item, file);
                        }
                    }
                }
            }


            // **************** Check external hints ********************
            // Checks for HandsOff events send by the project system
            foreach (string file in _fileHints)
            {
                if (_fileOrigins.ContainsKey(file))
                    continue;

                string name = Path.GetFileName(file);
                if (nameToItem.ContainsKey(name))
                {
                    string item = nameToItem[name];

                    CheckForMatch(item, file);
                }
            }

            // The clipboard seems to have some other format which might contain other info
            origin = _fileOrigins[newName];

            if (origin == null)
            {
                bool first = true;
                string path = null;

                foreach (KeyValuePair<string, string> kv in _fileOrigins)
                {
                    if (kv.Value == null)
                        continue;

                    if (SvnItem.IsBelowRoot(kv.Key, newName))
                    {
                        string itemRoot = kv.Value.Substring(0, kv.Value.Length - kv.Key.Length + newName.Length);
                        if (first)
                        {
                            path = itemRoot;
                            first = false;
                        }
                        else if (path != itemRoot)
                        {
                            origin = null;
                            return;
                        }
                    }
                }
                origin = path;
            }
        }

        void CheckForMatch(string newItem, string maybeFrom)
        {
            string v;
            if (_fileOrigins.TryGetValue(newItem, out v) && v != null)
                return;

            FileInfo newInfo = new FileInfo(newItem);
            FileInfo maybeInfo = new FileInfo(maybeFrom);

            if (maybeInfo.Exists && newInfo.Exists && maybeInfo.Length == newInfo.Length)
            {
                // BH: Don't verify filedates, etc; as they shouldn't be copied

                if (FileContentsEquals(maybeInfo.FullName, newInfo.FullName))
                {
                    _fileOrigins[newItem] = maybeFrom;
                }
            }
            else if (!maybeInfo.Exists)
            {
                // Handles Delete followed by add. Triggered from Cut&Paste
                SvnItem svnItem = StatusCache[maybeInfo.FullName];

                if (svnItem.IsVersioned)
                {
                    _fileOrigins[newItem] = maybeFrom;
                }
            }
        }

        /// <summary>
        /// Compares two files to make sure they are equal
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private bool FileContentsEquals(string file1, string file2)
        {
            if (string.IsNullOrEmpty(file1))
                throw new ArgumentNullException("file1");
            else if (string.IsNullOrEmpty(file2))
                throw new ArgumentNullException("file2");
            // We assume 
            // - Filelengths are equal
            // - Both files exist
            byte[] buffer1 = new byte[8192];
            byte[] buffer2 = new byte[8192];
            using (Stream f1 = File.OpenRead(file1))
            using (Stream f2 = File.OpenRead(file2))
            {
                int r;
                while (0 < (r = f1.Read(buffer1, 0, buffer1.Length)))
                {
                    if (r != f2.Read(buffer2, 0, buffer2.Length))
                        return false; // Should never happen on disk files

                    for (int i = 0; i < r; i++)
                    {
                        if (buffer1[i] != buffer2[i])
                            return false; // Files are different
                    }
                }
            }

            return true;
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
            if (pProject == null || rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            RegisterForSccCleanup(); // Clear the origins table after adding
            _collectHints = true;

            for (int i = 0; i < cDirectories; i++)
            {
                if (rgResults != null)
                    rgResults[i] = VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK;

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
            if (rgpProjects == null || rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            bool sccActive = SccProvider.IsActive;

            for (int iProject = 0, iDir = 0; (iProject < cProjects) && (iDir < cDirectories); iProject++)
            {
                int iLastDirectoryThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cDirectories;

                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool trackCopies;
                bool track = SccProvider.TrackProjectChanges(sccProject, out trackCopies);

                for (; iDir < iLastDirectoryThisProject; iDir++)
                {
                    if (!track)
                        continue;

                    string dir = rgpszMkDocuments[iDir];

                    if (!SvnItem.IsValidPath(dir))
                        continue;

                    dir = SvnTools.GetNormalizedFullPath(dir);
                    string origin = null;

                    if (sccActive && _solutionLoaded)
                    {
                        StatusCache.MarkDirty(dir);
                        TryFindOrigin(dir, out origin);
                    }

                    if (sccProject != null)
                        SccProvider.OnProjectDirectoryAdded(sccProject, dir, origin);

                    if (sccActive && trackCopies &&
                        !string.IsNullOrEmpty(origin) &&
                        StatusCache[origin].HasCopyableHistory)
                    {
                        using (SvnSccContext svn = new SvnSccContext(this))
                        {
                            svn.AddParents(dir);
                            svn.MetaCopy(origin, dir, true);
                        }
                    }
                }
            }

            return VSConstants.S_OK;
        }

        internal void OnDocumentSaveAs(string oldName, string newName)
        {
            _fileOrigins[newName] = oldName;
            RegisterForSccCleanup();
        }
    }
}
