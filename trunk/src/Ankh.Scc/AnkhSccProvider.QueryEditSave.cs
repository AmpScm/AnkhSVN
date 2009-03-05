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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.Selection;
using Ankh.Commands;
using Ankh.Scc.ProjectMap;
using System.IO;
using SharpSvn;
using System.Diagnostics;
using Ankh.Scc.SccUI;
using System.Windows.Forms;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    partial class AnkhSccProvider : IVsQueryEditQuerySave2
    {
        readonly SortedList<string, int> _unreloadable = new SortedList<string, int>(StringComparer.OrdinalIgnoreCase);

        int _saveBatchingState;
        /// <summary>
        /// Creates a batch of a sequence of documents before attempting to save them to disk.
        /// </summary>
        /// <returns></returns>
        public int BeginQuerySaveBatch()
        {
            _saveBatchingState++;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Ends the batch started by the BeginQuerySaveBatch method and 
        /// displays any user interface (UI) generated within the batch
        /// </summary>
        /// <returns></returns>
        public int EndQuerySaveBatch()
        {
            _saveBatchingState--;

            // _batchingState shouldn't go negative
            Debug.Assert(_saveBatchingState >= 0);

            if (_saveBatchingState == 0)
            {
                // Reset the cancel flag
                _querySaveBatchCancel = false;
            }

            return VSConstants.S_OK;
        }

        bool IsInSaveBatch
        {
            get { return _saveBatchingState > 0; }
        }

        /// <summary>
        /// States that a file will be reloaded if it changes on disk.
        /// </summary>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="rgf">The RGF.</param>
        /// <param name="pFileInfo">The p file info.</param>
        /// <returns></returns>
        public int DeclareReloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            if (!string.IsNullOrEmpty(pszMkDocument))
                lock (_unreloadable)
                {
                    int n;

                    if (!_unreloadable.TryGetValue(pszMkDocument, out n))
                        n = 0;

                    n--;

                    if (n != 0)
                        _unreloadable[pszMkDocument] = n;
                    else
                        _unreloadable.Remove(pszMkDocument);
                }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// States that a file will not be reloaded if it changes on disk
        /// </summary>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="rgf">The RGF.</param>
        /// <param name="pFileInfo">The p file info.</param>
        /// <returns></returns>
        public int DeclareUnreloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            if (!string.IsNullOrEmpty(pszMkDocument))
                lock (_unreloadable)
                {
                    int n;

                    if (!_unreloadable.TryGetValue(pszMkDocument, out n))
                        n = 0;

                    n++;

                    if (n != 0)
                        _unreloadable[pszMkDocument] = n;
                    else
                        _unreloadable.Remove(pszMkDocument);
                }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines whether the specified PSZ mk document is reloadable.
        /// </summary>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="pbResult">The pb result.</param>
        /// <returns></returns>
        public int IsReloadable(string pszMkDocument, out int pbResult)
        {
            lock (_unreloadable)
            {
                int n;

                if (_unreloadable.TryGetValue(pszMkDocument, out n))
                    pbResult = (n != 0) ? 1 : 0;
                else
                    pbResult = 1;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Synchronizes or refreshes the file date and size after an editor saves an unreloadable file.
        /// </summary>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="rgf">The RGF.</param>
        /// <param name="pFileInfo">The p file info.</param>
        /// <returns></returns>
        public int OnAfterSaveUnreloadableFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo)
        {
            if (IsSafeSccPath(pszMkDocument))
                MarkDirty(pszMkDocument);

            return VSConstants.S_OK;
        }


        /// <summary>
        /// Gets the SvnItem of the document file and all subdocument files (SccSpecial files)
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        internal IEnumerable<SvnItem> GetAllDocumentItems(string document)
        {
            if (string.IsNullOrEmpty(document))
                throw new ArgumentNullException("document");

            SvnItem item = StatusCache[document];

            if (item == null)
                yield break;

            yield return item;

            SccProjectFile pf;
            if (_fileMap.TryGetValue(item.FullPath, out pf))
            {
                HybridCollection<string> subFiles = null;

                if (pf.FirstReference != null)
                    foreach (string path in pf.FirstReference.GetSubFiles())
                    {
                        if (subFiles == null)
                        {
                            subFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                            subFiles.Add(item.FullPath);
                        }

                        if (subFiles.Contains(path))
                            continue;

                        item = StatusCache[path];
                        if (item != null)
                            yield return item;

                        subFiles.Add(item.FullPath);
                    }
            }
        }

        string _tempPath;
        internal string TempPathWithSeparator
        {
            get
            {
                if (_tempPath == null)
                {
                    string p = System.IO.Path.GetTempPath();

                    if (p.Length > 0 && p[p.Length - 1] != Path.DirectorySeparatorChar)
                        p += Path.DirectorySeparatorChar;

                    _tempPath = p;
                }
                return _tempPath;
            }
        }

        bool IsSafeSccPath(string file)
        {
            if (string.IsNullOrEmpty(file))
                return false;
            else if (!SvnItem.IsValidPath(file))
                return false;
            else if (file.StartsWith(TempPathWithSeparator, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Called by projects and editors before modifying a file
        /// The function allows the source control systems to take the necessary actions (checkout, flip attributes)
        /// to make the file writable in order to allow the edit to continue
        ///
        /// There are a lot of cases to deal with during QueryEdit/QuerySave. 
        /// - called in commmand line mode, when UI cannot be displayed
        /// - called during builds, when save shoudn't probably be allowed
        /// - called during projects migration, when projects are not open and not registered yet with source control
        /// - checking out files may bring new versions from vss database which may be reloaded and the user may lose in-memory changes; some other files may not be reloadable
        /// - not all editors call QueryEdit when they modify the file the first time (buggy editors!), and the files may be already dirty in memory when QueryEdit is called
        /// - files on disk may be modified outside IDE and may have attributes incorrect for their scc status
        /// - checkouts may fail
        /// </summary>
        /// <param name="rgfQueryEdit">The RGF query edit.</param>
        /// <param name="cFiles">The c files.</param>
        /// <param name="rgpszMkDocuments">The RGPSZ mk documents.</param>
        /// <param name="rgrgf">The RGRGF.</param>
        /// <param name="rgFileInfo">The rg file info.</param>
        /// <param name="pfEditVerdict">The pf edit verdict.</param>
        /// <param name="prgfMoreInfo">The PRGF more info.</param>
        /// <returns></returns>
        public int QueryEditFiles(uint rgfQueryEdit, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pfEditVerdict, out uint prgfMoreInfo)
        {
            tagVSQueryEditFlags queryFlags = (tagVSQueryEditFlags)rgfQueryEdit;
            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            prgfMoreInfo = (uint)(tagVSQueryEditResultFlags)0;

            if (rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            HybridCollection<string> mustLockFiles = null;
            List<SvnItem> mustLockItems = null;


            for (int i = 0; i < cFiles; i++)
            {
                string file = rgpszMkDocuments[i];

                if (!IsSafeSccPath(file))
                    continue; // Skip non scc paths

                file = SvnTools.GetNormalizedFullPath(file);

                Monitor.ScheduleDirtyCheck(file, true);

                foreach (SvnItem item in GetAllDocumentItems(file))
                {
                    if (item.ReadOnlyMustLock && !item.IsDirectory)
                    {
                        if ((queryFlags & tagVSQueryEditFlags.QEF_ReportOnly) != 0)
                        {
                            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                            prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_MaybeCheckedout
                                | tagVSQueryEditResultFlags.QER_EditNotPossible
                                | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc);

                            return VSConstants.S_OK;
                        }

                        if (mustLockItems == null)
                        {
                            mustLockFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                            mustLockItems = new List<SvnItem>();
                        }

                        if (!mustLockFiles.Contains(item.FullPath))
                        {
                            mustLockFiles.Add(item.FullPath);
                            mustLockItems.Add(item);
                        }
                    }
                }
            }
            if (mustLockItems != null)
            {
                IAnkhCommandService cmdSvc = GetService<IAnkhCommandService>();

                cmdSvc.DirectlyExecCommand(AnkhCommand.Lock, mustLockItems, CommandPrompt.Always);

                foreach (SvnItem i in mustLockItems)
                {
                    if (i.ReadOnlyMustLock)
                    {
                        // User has probably canceled the lock operation, or it failed.
                        pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                        prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed
                            | tagVSQueryEditResultFlags.QER_EditNotPossible
                            | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc);
                        break;
                    }
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies the environment that a file is about to be saved.
        /// </summary>
        /// <param name="pszMkDocument">The document that wants to be saved</param>
        /// <param name="rgf">Valid file attributes?</param>
        /// <param name="pFileInfo">File attributes</param>
        /// <param name="pdwQSResult">Result</param>
        /// <returns></returns>
        public int QuerySaveFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult)
        {
            if (_querySaveBatchCancel)
            {
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                return VSConstants.S_OK;
            }

            // TODO: Handle batch mode
            if (!IsSafeSccPath(pszMkDocument))
            {
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                return VSConstants.S_OK;
            }

            SvnItem item = StatusCache[pszMkDocument];
            if (item == null)
            {
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                return VSConstants.S_OK;
            }

            if (rgf == (uint)tagVSQEQSFlags.VSQEQS_FileInfo)
                Debug.Assert(pFileInfo.Length == 1);

            if (item.ReadOnlyMustLock)
            {
                // This readonly case can be fixed by Subversion
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.DirectlyExecCommand(AnkhCommand.Lock, new SvnItem[] { item });
                
                if(item.ReadOnlyMustLock)
                {
                    // User cancelled
                    pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;

                    if (IsInSaveBatch)
                    {
                        // Cancel all next questions in this batch
                        _querySaveBatchCancel = true;
                    }
                }
                else
                {
                    pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                }
                
                return VSConstants.S_OK;
            }

            bool validFileInfo = rgf == (uint)tagVSQEQSFlags.VSQEQS_FileInfo;

            VSQEQS_FILE_ATTRIBUTE_DATA? fileAttrData = null;
            if (validFileInfo && pFileInfo != null && pFileInfo.Length > 0)
                fileAttrData = pFileInfo[0];

            tagVSQuerySaveResult rslt;
            QuerySaveSingleFile(
                false /* Single file is never in silent mode */, 
                pszMkDocument,
                item,
                validFileInfo,
                fileAttrData,
                out rslt);

            pdwQSResult = (uint)rslt;

            return VSConstants.S_OK;
        }

        bool _querySaveBatchCancel;

        /// <summary>
        /// Notifies the environment that multiple files are about to be saved.
        /// </summary>
        /// <param name="rgfQuerySave">The RGF query save.</param>
        /// <param name="cFiles">The c files.</param>
        /// <param name="rgpszMkDocuments">The RGPSZ mk documents.</param>
        /// <param name="rgrgf">The RGRGF.</param>
        /// <param name="rgFileInfo">The rg file info.</param>
        /// <param name="pdwQSResult">The PDW QS result.</param>
        /// <returns></returns>
        public int QuerySaveFiles(uint rgfQuerySave, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult)
        {
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            bool silent = rgfQuerySave == (uint)tagVSQuerySaveFlags.QSF_SilentMode;

            List<SvnItem> toBeSvnLocked = new List<SvnItem>();

            if (rgpszMkDocuments == null)
                return VSConstants.S_OK;

            for (int i = 0; i < cFiles; i++)
            {
                string file = rgpszMkDocuments[i];

                if (!IsSafeSccPath(file))
                    continue;

                file = SvnTools.GetNormalizedFullPath(file);
                MarkDirty(file);

                SvnItem item = StatusCache[file];
                if (item.ReadOnlyMustLock)
                {
                    toBeSvnLocked.Add(item);
                    continue;
                }

                bool validFileInfo = rgrgf != null && rgrgf.Length > i && rgrgf[i] == (uint)tagVSQEQSFlags.VSQEQS_FileInfo;

                VSQEQS_FILE_ATTRIBUTE_DATA? fileAttrData = null;
                if (validFileInfo && rgFileInfo != null && rgFileInfo.Length > i)
                    fileAttrData = rgFileInfo[i];

                // Handle readonly files without svn:needs-lock
                tagVSQuerySaveResult rslt;
                QuerySaveSingleFile(
                    silent,
                    file,
                    item,
                    validFileInfo,
                    fileAttrData,
                    out rslt);

                // TODO: check rslt, set pdwQSResult accordingly
            }

            if (toBeSvnLocked.Count > 0)
            {
                if (silent)
                {
                    // We need to show UI, but aren't allowed because of silent mode
                    pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_NoisyPromptRequired;
                    return VSConstants.S_OK;
                }

                // File(s) need to be locked
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.DirectlyExecCommand(AnkhCommand.Lock, toBeSvnLocked.ToArray());

                foreach (SvnItem item in toBeSvnLocked)
                {
                    if (item.ReadOnlyMustLock)
                    {
                        // Use cancel here, because the user must have either cancelled the lock dialog
                        // or didn't lock all files
                        pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;

                        if (IsInSaveBatch)
                        {
                            // Cancel all next UI for this batch
                            _querySaveBatchCancel = true;
                        }

                        return VSConstants.S_OK;
                    }
                }

                // All items locked
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            }

            return VSConstants.S_OK;
        }

        void QuerySaveSingleFile(bool silent, string pszMkDocument, SvnItem item, bool validFileInfo, VSQEQS_FILE_ATTRIBUTE_DATA? pFileInfo, out tagVSQuerySaveResult pdwQSResult)
        {
            // If rgf is FileInfo, pFileInfo contains valid file attributes
            FileAttributes attrs;
            if (validFileInfo)
            {
                Debug.Assert(pFileInfo.HasValue);
                attrs = (FileAttributes)pFileInfo.Value.dwFileAttributes;
            }
            else
            {
                attrs = File.GetAttributes(item.FullPath);
            }

            MarkDirty(pszMkDocument);

            if ((attrs & FileAttributes.ReadOnly) > 0)
            {
                // We're just read-only, not svn:needs-lock
                Debug.Assert(!item.ReadOnlyMustLock);

                if (silent)
                {
                    // we have to show a dialog (save as/overwrite/cancel), but we're not allowed
                    pdwQSResult = tagVSQuerySaveResult.QSR_NoSave_NoisyPromptRequired;
                    return;
                }

                // We're allowed to show UI
                Debug.Assert(!silent);

                // We can also be dealing with a non svn:needs-lock read-only file
                // Now we have to ask the user wether to overwrite, or to save as
                using (SccQuerySaveReadonlyDialog dlg = new SccQuerySaveReadonlyDialog())
                {
                    dlg.File = item.Name;

                    DialogResult result = dlg.ShowDialog(Context);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            // Force the caller to show a save-as dialog for this file
                            pdwQSResult = tagVSQuerySaveResult.QSR_ForceSaveAs;
                            return;

                        case DialogResult.No:
                            // User wants to overwrite existing file
                            File.SetAttributes(item.FullPath, attrs & ~FileAttributes.ReadOnly);

                            // it's no longer read-only, so save is OK
                            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
                            return;

                        case DialogResult.Cancel:
                            // User cancelled
                            pdwQSResult = tagVSQuerySaveResult.QSR_NoSave_UserCanceled;

                            if (IsInSaveBatch)
                            {
                                // Cancel all coming QuerySaveFile(s) calls until batching is done
                                _querySaveBatchCancel = true;
                            }

                            return;

                        default:
                            throw new InvalidOperationException("Dialog returned unexpected DialogResult");
                    } // switch(dialogResult)
                } // using dialog
            } // if readonly

            // File wasn't read-only so save is ok
            pdwQSResult = tagVSQuerySaveResult.QSR_SaveOK;
        }

#if VS2008_PLUS
        // TODO: Implement IVsQueryEditQuerySave3 extra's. 
        /// <summary>
        /// Notifies the environment that a file is about to be saved.
        /// </summary>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="rgf">The RGF.</param>
        /// <param name="pFileInfo">The p file info.</param>
        /// <param name="pdwQSResult">The PDW QS result.</param>
        /// <param name="prgfMoreInfo">The PRGF more info.</param>
        /// <returns></returns>
        public int QuerySaveFile2(string pszMkDocument, uint[] rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult, out uint prgfMoreInfo)
        {
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            prgfMoreInfo = (uint)tagVSQuerySaveResultFlags.QSR_DefaultFlag;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies the environment that multiple files are about to be saved.
        /// </summary>
        /// <param name="rgfQuerySave">The RGF query save.</param>
        /// <param name="cFiles">The c files.</param>
        /// <param name="rgpszMkDocuments">The RGPSZ mk documents.</param>
        /// <param name="rgrgf">The RGRGF.</param>
        /// <param name="rgFileInfo">The rg file info.</param>
        /// <param name="pdwQSResult">The PDW QS result.</param>
        /// <param name="prgfMoreInfo">The PRGF more info.</param>
        /// <returns></returns>
        public int QuerySaveFiles2(uint[] rgfQuerySave, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult, out uint prgfMoreInfo)
        {
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            prgfMoreInfo = (uint)tagVSQuerySaveResultFlags.QSR_DefaultFlag;
            return VSConstants.S_OK;
        }
#endif
    }
}
