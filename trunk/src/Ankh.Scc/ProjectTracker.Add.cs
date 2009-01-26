// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Ankh.Selection;
using System.IO;
using SharpSvn;
using System.Diagnostics;

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

            bool allOk = true;

            bool track = SccProvider.TrackProjectChanges(pProject as IVsSccProject2);

            for (int i = 0; i < cFiles; i++)
            {
                bool ok = true;

                if (string.IsNullOrEmpty(rgpszMkDocuments[i]) || !SvnItem.IsValidPath(rgpszMkDocuments[i]))
                    continue;

                // TODO: Verify the new names do not give invalid Subversion state (Double casings, etc.)
                if (track && !SvnCanAddPath(SvnTools.GetNormalizedFullPath(rgpszMkDocuments[i]), SvnNodeKind.File))
                    ok = false;

                if (rgResults != null)
                    rgResults[i] = ok ? VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK : VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddNotOK;

                if (!ok)
                    allOk = false;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = allOk ? VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK : VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddNotOK;

            if (!allOk && !_inBatch)
                ShowQueryErrorDialog();

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

            _collectHints = true; // Some projects call HandsOff(file) on which files they wish to import. Use that to get more information
            bool allOk = true;

            bool track = SccProvider.TrackProjectChanges(pProject as IVsSccProject2);

            for (int i = 0; i < cFiles; i++)
            {
                bool ok = true;

                if (string.IsNullOrEmpty(rgpszNewMkDocuments[i]) || !SvnItem.IsValidPath(rgpszNewMkDocuments[i]))
                    continue;

                // Save the origins of the to be added files as they are not available in the added event

                string newDoc = SvnTools.GetNormalizedFullPath(rgpszNewMkDocuments[i]);
                string origDoc = rgpszSrcMkDocuments[i];

                if (origDoc != null)
                    origDoc = SvnTools.GetNormalizedFullPath(origDoc);

                if (!string.IsNullOrEmpty(newDoc) && !string.IsNullOrEmpty(origDoc)
                    && !string.Equals(newDoc, origDoc, StringComparison.OrdinalIgnoreCase)
                    && !origDoc.StartsWith(SccProvider.TempPathWithSeparator)) // VS tries to add the file
                {
                    _fileOrigins[newDoc] = origDoc;
                }

                if (track && !SvnCanAddPath(newDoc, SvnNodeKind.File))
                    ok = false;

                if (rgResults != null)
                    rgResults[i] = ok ? VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK : VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddNotOK;

                if (!ok)
                    allOk = false;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = allOk ? VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK : VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddNotOK;

            if (!allOk)
            {
                RegisterForSccCleanup(); // Clear the origins table after adding

                if (!_inBatch)
                    ShowQueryErrorDialog();
            }

            return VSConstants.S_OK;
        }

        protected bool SvnCanAddPath(string fullpath, SvnNodeKind nodeKind)
        {
            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                // Determine if we could add fullname
                if (!svn.CouldAdd(fullpath, nodeKind))
                {
                    if (svn.BelowAdminDir(fullpath))
                        _batchErrors.Add(string.Format(Resources.SvnPathXBlocked, fullpath));
                    else
                        _batchErrors.Add(string.Format(Resources.PathXBlocked, fullpath));
                    return false;
                }
                else
                    return true;
            }
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
            RegisterForSccCleanup(); // Clear the origins table after adding

            List<string> selectedFiles = null;
            SortedList<string, string> copies = null;

            for (int i = 0; i < cFiles; i++)
            {
                string s = rgpszMkDocuments[i];
                if (!string.IsNullOrEmpty(s) && SvnItem.IsValidPath(s))
                {
                    StatusCache.MarkDirty(s);
                }
            }

            for (int iProject = 0; (iProject < cProjects) && (iFile < cFiles); iProject++)
            {
                int iLastFileThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cFiles;

                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool track = SccProvider.TrackProjectChanges(sccProject);

                for (; iFile < iLastFileThisProject; iFile++)
                {
                    if (!track)
                        continue; // Not handled by our provider

                    string origin = null;
                    string newName = rgpszMkDocuments[iFile];

                    if (string.IsNullOrEmpty(newName) || !SvnItem.IsValidPath(newName))
                        continue;

                    newName = SvnTools.GetNormalizedFullPath(rgpszMkDocuments[iFile]);

                    if ((!_fileOrigins.TryGetValue(newName, out origin) || (origin == null)))
                    {
                        // We haven't got the project file origin for free via OnQueryAddFilesEx
                        // So:
                        //  1 - The file is really new or
                        //  2 - The file is drag&dropped into the project from the solution explorer or
                        //  3 - The file is copy pasted into the project from an other project or
                        //  4 - The file is added via add existing item or
                        //  5 - The file is added via drag&drop from another application (OLE drop)
                        //
                        // The only way to determine is walking through these options

                        FileInfo newInfo = new FileInfo(newName);

                        // 2 -  If the file is drag&dropped in the solution explorer
                        //      the current selection is still the original selection
                        if (selectedFiles == null)
                        {
                            if (SelectionContext != null)
                            {
                                // BH: resx files are not correctly included if we don't retrieve this list recursive
                                selectedFiles = new List<string>(SelectionContext.GetSelectedFiles(true));
                            }
                            else
                                selectedFiles = new List<string>();
                        }


                        // **************** Check the current selection *************
                        // Checks for drag&drop actions. The selection contains the original list of files
                        foreach (string file in selectedFiles)
                        {
                            if (Path.GetFileName(file) == newInfo.Name && !string.Equals(file, newInfo.FullName, StringComparison.OrdinalIgnoreCase))
                            {
                                FileInfo orgInfo = new FileInfo(file);

                                if (orgInfo.Exists && newInfo.Exists && orgInfo.Length == newInfo.Length)
                                {
                                    // BH: Don't verify filedates, etc; as they shouldn't be copied
                                    // We can be reasonably be sure its the same file. Same name and same length

                                    if (FileContentsEquals(orgInfo.FullName, newInfo.FullName))
                                    {
                                        // TODO: Determine if we should verify the contents (BH: We probably should to be 100% sure; but perf impact)
                                        _fileOrigins[newName] = origin = file;

                                        break;
                                    }
                                }
                            }
                        }

                        // **************** Check external hints ********************
                        // Checks for HandsOff events send by the project system
                        if (origin == null)
                        {
                            foreach (string file in _fileHints)
                            {
                                if (Path.GetFileName(file) == newInfo.Name && !string.Equals(file, newInfo.FullName, StringComparison.OrdinalIgnoreCase))
                                {
                                    FileInfo orgInfo = new FileInfo(file);

                                    if (orgInfo.Exists && newInfo.Exists && orgInfo.Length == newInfo.Length)
                                    {
                                        // BH: Don't verify filedates, etc; as they shouldn't be copied

                                        if (FileContentsEquals(orgInfo.FullName, newInfo.FullName))
                                        {
                                            // TODO: Determine if we should verify the contents (BH: We probably should to be 100% sure; but perf impact)
                                            _fileOrigins[newName] = origin = SvnTools.GetNormalizedFullPath(file);

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // **************** Check the clipboard *********************
                        // 3 - Copy & Paste in the solution explorer:
                        //     The original paths are still on the clipboard
                        if (origin == null && System.Windows.Forms.Clipboard.ContainsText())
                        {
                            // TODO: BH: Look into using IVsUIHierWinClipboardHelper to parse the clipboard data!

                            // In some cases (Websites) the solution explorer just dumps a bunch of file:// Url's on the clipboard

                            string text = System.Windows.Forms.Clipboard.GetText();

                            if (!string.IsNullOrEmpty(text))
                            {
                                foreach (string part in text.Split('\n'))
                                {
                                    string s = part.Trim();
                                    Uri uri;

                                    if (!Uri.TryCreate(s, UriKind.Absolute, out uri))
                                        break;

                                    if (uri.IsFile)
                                    {
                                        string file = SvnTools.GetNormalizedFullPath(uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped));

                                        if (Path.GetFileName(file) == newInfo.Name && !string.Equals(file, newInfo.FullName, StringComparison.OrdinalIgnoreCase))
                                        {
                                            FileInfo orgInfo = new FileInfo(file);

                                            if (orgInfo.Exists && newInfo.Exists && orgInfo.Length == newInfo.Length)
                                            {
                                                // BH: Don't verify filedates, etc; as they shouldn't be copied

                                                if (FileContentsEquals(orgInfo.FullName, newInfo.FullName))
                                                {
                                                    // TODO: Determine if we should verify the contents (BH: We probably should to be 100% sure; but perf impact)
                                                    _fileOrigins[newName] = origin = SvnTools.GetNormalizedFullPath(file);

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        // The clipboard seems to have some other format which might contain other info
                    }

                    if (sccProject != null)
                        SccProvider.OnProjectFileAdded(sccProject, newName,
                            origin, rgFlags[iFile]);

                    if (!string.IsNullOrEmpty(origin) && SccProvider.IsActive)
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
                    while (copies.Count > 0)
                    {
                        string toFile = copies.Keys[0];
                        string fromFile = copies.Values[0];
                        string dir = Path.GetDirectoryName(copies.Keys[0]);

                        copies.RemoveAt(0);
                        Guid addGuid;

                        if (!svn.TryGetRepositoryId(dir, out addGuid))
                        {
                            continue; // No repository to fix up
                        }

                        Guid fileGuid;

                        if (!svn.TryGetRepositoryId(fromFile, out fileGuid) || fileGuid != addGuid)
                            continue; // Can't fix history for this file

                        if (string.Equals(Path.GetFileName(fromFile), Path.GetFileName(toFile), StringComparison.OrdinalIgnoreCase))
                        {
                            // If the names are the same we can handle all files to the same directory
                            // without a sleep penalty             
                            SortedList<string, string> now = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
                            now.Add(toFile, fromFile);

                            for (int i = 0; i < copies.Count; i++)
                            {
                                string fl = copies.Keys[i];
                                string tl = copies.Values[i];

                                if (string.Equals(Path.GetDirectoryName(fl), dir, StringComparison.OrdinalIgnoreCase) &&
                                    string.Equals(Path.GetFileName(fl), Path.GetFileName(tl), StringComparison.OrdinalIgnoreCase))
                                {
                                    Guid fromGuid;
                                    if (svn.TryGetRepositoryId(tl, out fromGuid) && (fromGuid == addGuid))
                                        now.Add(fl, tl); // We can copy this item at the same time
                                    // else 
                                    // This copy comes from another repository, no history to save

                                    copies.RemoveAt(i--);
                                }
                            }

                            // Now contains all the files we are receiving in a single directory
                            if (now.Count > 0)
                                svn.SafeWcCopyToDirFixup(now, dir);
                            else
                                svn.SafeWcCopyFixup(fromFile, toFile);
                        }
                        else
                            svn.SafeWcCopyFixup(fromFile, toFile);
                    }
                }

            return VSConstants.S_OK;
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

            bool track = SccProvider.TrackProjectChanges(pProject as IVsSccProject2);

            bool allOk = true;
            for (int i = 0; i < cDirectories; i++)
            {
                bool ok = true;

                string dir = rgpszMkDocuments[i];

                if (string.IsNullOrEmpty(dir) || !SvnItem.IsValidPath(dir))
                    continue;

                dir = SvnTools.GetNormalizedFullPath(dir);

                if (track && !SvnCanAddPath(dir, SvnNodeKind.Directory))
                    ok = false;

                if (rgResults != null)
                    rgResults[i] = ok ? VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK : VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddNotOK;

                if (!ok)
                    allOk = false;
            }

            if (pSummaryResult != null)
                pSummaryResult[0] = allOk ? VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddOK : VSQUERYADDDIRECTORYRESULTS.VSQUERYADDDIRECTORYRESULTS_AddNotOK;

            if (!allOk && !_inBatch)
                ShowQueryErrorDialog();

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

            int iDirectory = 0;

            for (int i = 0; i < cDirectories; i++)
            {
                string dir = rgpszMkDocuments[i];

                if (string.IsNullOrEmpty(dir) || !SvnItem.IsValidPath(dir))
                    continue;

                dir = SvnTools.GetNormalizedFullPath(dir);

                StatusCache.MarkDirty(dir);

                SvnItem item = StatusCache[dir];

                if (!item.Exists || !item.IsDirectory || !SvnTools.IsManagedPath(dir))
                    continue;

                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                if (!dirInfo.Exists || (DateTime.Now - dirInfo.CreationTime) > new TimeSpan(0, 1, 0))
                    continue; // Directory is older than one minute.. Not just copied

                using (SvnSccContext svn = new SvnSccContext(Context))
                {
                    // Ok; we have a 'new' directory here.. Lets check if VS broke the subversion working copy
                    SvnStatusEventArgs status = svn.SafeGetStatusViaParent(dir);

                    if (status == null)
                    {
                        string pd = Path.GetDirectoryName(dir);
                        if (pd == null || SvnTools.IsManagedPath(pd))
                            continue;
                    }
                    else if (status.LocalContentStatus != SvnStatus.NotVersioned && status.LocalContentStatus != SvnStatus.Ignored)
                        continue;

                    svn.UnversionRecursive(dir);
                }
            }

            for (int iProject = 0; (iProject < cProjects) && (iDirectory < cDirectories); iProject++)
            {
                int iLastDirectoryThisProject = (iProject < cProjects - 1) ? rgFirstIndices[iProject + 1] : cDirectories;

                IVsSccProject2 sccProject = rgpProjects[iProject] as IVsSccProject2;

                bool track = SccProvider.TrackProjectChanges(sccProject);

                for (; iDirectory < iLastDirectoryThisProject; iDirectory++)
                {
                    if (!track)
                        continue;

                    string dir = rgpszMkDocuments[iDirectory];

                    if (string.IsNullOrEmpty(dir) || !SvnItem.IsValidPath(dir))
                        continue;

                    dir = SvnTools.GetNormalizedFullPath(dir);

                    if (sccProject != null)
                        SccProvider.OnProjectDirectoryAdded(sccProject, dir, rgFlags[iDirectory]);
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
