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
using Ankh.UI;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
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

        bool _isInQuerySaveBatch;
        /// <summary>
        /// Creates a batch of a sequence of documents before attempting to save them to disk.
        /// </summary>
        /// <returns></returns>
        public int BeginQuerySaveBatch()
        {
            _isInQuerySaveBatch = true;
            _querySaveBatchCancel = false; // Just to be sure
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Ends the batch started by the BeginQuerySaveBatch method and 
        /// displays any user interface (UI) generated within the batch
        /// </summary>
        /// <returns></returns>
        public int EndQuerySaveBatch()
        {
            _isInQuerySaveBatch = false;

            // Reset the cancel flag
            _querySaveBatchCancel = false;

            return VSConstants.S_OK;
        }

        bool IsInSaveBatch
        {
            get { return _isInQuerySaveBatch; }
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
        string TempPathWithSeparator
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

        internal bool IsSafeSccPath(string file)
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
            prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_InMemoryEdit | tagVSQueryEditResultFlags.QER_MaybeChanged);

            bool allowUI = (queryFlags & (tagVSQueryEditFlags.QEF_SilentMode | tagVSQueryEditFlags.QEF_ReportOnly | tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting)) == 0;

            bool? allowReadOnlyNonSccWrites = null;

            if (rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            try
            {
                // If the editor asks us to do anything in our power to make it editable
                // without prompting, just make those files writable.
                if ((queryFlags & tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting) != 0)
                    return QueryEditForceWritable(rgpszMkDocuments);

                HybridCollection<string> mustLockFiles = null;
                HybridCollection<string> readOnlyEditFiles = null;
                List<SvnItem> mustLockItems = null;
                List<SvnItem> readOnlyItems = null;

                for (int i = 0; i < cFiles; i++)
                {
                    string file = rgpszMkDocuments[i];

                    if (!IsSafeSccPath(file))
                        continue; // Skip non scc paths (Includes %TEMP%\*)

                    file = SvnTools.GetNormalizedFullPath(file);

                    Monitor.ScheduleDirtyCheck(file);

                    SvnItem item = StatusCache[file];

                    if (item.IsReadOnlyMustLock && !item.IsDirectory)
                    {
                        if (!allowUI)
                        {
                            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                            prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_MaybeCheckedout
                                                   | tagVSQueryEditResultFlags.QER_EditNotPossible
                                                   | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc
                                                   | tagVSQueryEditResultFlags.QER_NoisyCheckoutRequired);

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
                    else if (item.IsReadOnly)
                    {
                        if (!allowReadOnlyNonSccWrites.HasValue)
                            allowReadOnlyNonSccWrites = AllowReadOnlyNonSccWrites();

                        if (!allowReadOnlyNonSccWrites.Value)
                        {
                            if (!allowUI)
                            {
                                pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                                prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_EditNotPossible
                                                       | tagVSQueryEditResultFlags.QER_InMemoryEditNotAllowed
                                                       | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc
                                                       | tagVSQueryEditResultFlags.QER_NoisyPromptRequired);

                                return VSConstants.S_OK;
                            }

                            if (readOnlyEditFiles == null)
                            {
                                readOnlyEditFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                                readOnlyItems = new List<SvnItem>();
                            }

                            if (!readOnlyEditFiles.Contains(item.FullPath))
                            {
                                readOnlyEditFiles.Add(item.FullPath);
                                readOnlyItems.Add(item);
                            }
                        }
                        else
                        {
                            // What?
                            // Deny? or Make the file writable?
                            // Fallthrough = OK to edit
                        }
                    }
                }
                if (mustLockItems != null)
                {
                    List<SvnItem> mustBeLocked = new List<SvnItem>(mustLockItems);

                    // Look at all subfiles of the must be locked document and add these to the dialog
                    // to make it easier to lock them too
                    foreach (string lockFile in new List<string>(mustLockFiles))
                    {
                        foreach (SvnItem item in GetAllDocumentItems(lockFile))
                        {
                            if (!mustLockFiles.Contains(item.FullPath))
                            {
                                mustLockFiles.Add(item.FullPath);
                                mustLockItems.Add(item);
                            }
                        }
                    }

                    CommandService.DirectlyExecCommand(AnkhCommand.SccLock, mustLockItems, CommandPrompt.DoDefault);
                    // Only check the original list; the rest of the items in mustLockItems is optional
                    foreach (SvnItem i in mustBeLocked)
                    {
                        if (i.IsReadOnlyMustLock)
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
                if (readOnlyItems != null)
                {
                    // TODO: Handle multiple items correctly
                    // Is this only for non-scc items or also for scc items that are readonly for other reasons?
                    CommandResult result =
                        CommandService.DirectlyExecCommand(AnkhCommand.MakeNonSccFileWriteable, readOnlyItems[0], CommandPrompt.DoDefault);

                    bool allowed = result.Result is bool ? (bool)result.Result : false;

                    if (!allowed)
                    {
                        pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                        prgfMoreInfo = (uint)(tagVSQueryEditResultFlags.QER_EditNotPossible
                                               | tagVSQueryEditResultFlags.QER_InMemoryEditNotAllowed
                                               | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc // TODO: Specialize to SCC?
                                               );
                    }
                }
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }

            return VSConstants.S_OK;
        }

        private int QueryEditForceWritable(string[] rgpszMkDocuments)
        {
            // Force all real files to be writable
            foreach (string file in rgpszMkDocuments)
            {
                if (SvnItem.IsValidPath(file))
                {
                    SvnItem item = StatusCache[file];

                    if (item.IsReadOnly)
                    {
                        try
                        {
                            FileAttributes attrs = File.GetAttributes(item.FullPath);
                            File.SetAttributes(item.FullPath, attrs & ~FileAttributes.ReadOnly);
                        }
                        catch
                        { }
                    }
                }
            }
            return VSConstants.S_OK;
        }

        bool AllowReadOnlyNonSccWrites()
        {
            IVsSccToolsOptions sccToolsOptions = GetService<IVsSccToolsOptions>(typeof(SVsSccToolsOptions));
            if (sccToolsOptions == null)
                return true;

            object o;
            if (!ErrorHandler.Succeeded(
                sccToolsOptions.GetSccToolsOption(SccToolsOptionsEnum.ksctoAllowReadOnlyFilesNotUnderSccToBeEdited,
                                                  out o)))
                return true;

            if (!(o is bool))
                return true;

            return (bool)o;
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
            return QuerySaveFiles(
                (uint)tagVSQuerySaveFlags.QSF_DefaultOperation,
                1,
                new string[] { pszMkDocument },
                null,
                null,
                out pdwQSResult);
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
            bool silent = (rgfQuerySave & (uint)tagVSQuerySaveFlags.QSF_SilentMode) != 0;

            List<SvnItem> toBeSvnLocked = new List<SvnItem>();

            if (rgpszMkDocuments == null)
                return VSConstants.E_POINTER;

            if (_querySaveBatchCancel)
            {
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                return VSConstants.S_OK;
            }

            try
            {
                bool saveAs = false;
                bool saveOk = false;

                for (int i = 0; i < cFiles; i++)
                {
                    string file = rgpszMkDocuments[i];

                    if (!IsSafeSccPath(file))
                        continue;

                    file = SvnTools.GetNormalizedFullPath(file);

                    SvnItem item = StatusCache[file];
                    if (item.IsReadOnlyMustLock)
                    {
                        if (silent)
                        {
                            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_NoisyPromptRequired;
                            return VSConstants.S_OK;
                        }
                        toBeSvnLocked.Add(item);
                        continue;
                    }
                    else if (!item.IsReadOnly)
                        continue;
                    else if (silent)
                    {
                        pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_NoisyPromptRequired;
                        return VSConstants.S_OK;
                    }

                    tagVSQuerySaveResult rslt = QueryReadOnlyFile(item);
                    switch (rslt)
                    {
                        case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                            if (IsInSaveBatch)
                                _querySaveBatchCancel = true;
                            return VSConstants.S_OK;
                        case tagVSQuerySaveResult.QSR_ForceSaveAs:
                            saveAs = true;
                            break;
                        case tagVSQuerySaveResult.QSR_SaveOK:
                            saveOk = true;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

                if (saveAs && !saveOk)
                    pdwQSResult = (uint)tagVSQuerySaveResult.QSR_ForceSaveAs;
                else
                    pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;

                if (toBeSvnLocked.Count > 0)
                {
                    // File(s) need to be locked
                    CommandService.DirectlyExecCommand(AnkhCommand.SccLock, toBeSvnLocked.ToArray());

                    bool notWritable = false;
                    foreach (SvnItem item in toBeSvnLocked)
                    {
                        if (item.IsReadOnlyMustLock)
                            notWritable = true;
                    }

                    if (notWritable)
                        pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                }

                return VSConstants.S_OK;
            }
            finally
            {
                for (int i = 0; i < cFiles; i++)
                {
                    string file = rgpszMkDocuments[i];

                    if (!IsSafeSccPath(file))
                        continue;

                    MarkDirty(SvnTools.GetNormalizedFullPath(file));
                }
            }
        }

        tagVSQuerySaveResult QueryReadOnlyFile(SvnItem item)
        {
            Debug.Assert(item.IsReadOnly && !item.IsReadOnlyMustLock, "item.IsReadOnly && !item.IsReadOnlyMustLock");

            // Now we have to ask the user wether to overwrite, or to save as
            using (SccQuerySaveReadonlyDialog dlg = new SccQuerySaveReadonlyDialog())
            {
                dlg.File = item.Name;

                DialogResult result = dlg.ShowDialog(Context);
                switch (result)
                {
                    case DialogResult.Yes:
                        // Force the caller to show a save-as dialog for this file
                        return tagVSQuerySaveResult.QSR_ForceSaveAs;

                    case DialogResult.No:
                        // User wants to overwrite existing file
                        try
                        {
                            FileAttributes attrs = File.GetAttributes(item.FullPath);
                            File.SetAttributes(item.FullPath, attrs & ~FileAttributes.ReadOnly);
                        }
                        catch (IOException) // Includes PathTooLongException
                        { }
                        catch (SystemException) // Includes UnauthorizedAccessException
                        { }

                        // it's no longer read-only, so save is OK
                        return tagVSQuerySaveResult.QSR_SaveOK;

                    case DialogResult.Cancel:
                        return tagVSQuerySaveResult.QSR_NoSave_Cancel;
                    default:
                        throw new InvalidOperationException("Dialog returned unexpected DialogResult");
                } // switch(dialogResult)
            } // using dialog
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
