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
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;

using Ankh.Commands;
using Ankh.Scc.ProjectMap;
using Ankh.Scc.SccUI;
using Ankh.Services;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    partial class SvnSccProvider : IVsQueryEditQuerySave2
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
            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
        }


        /// <summary>
        /// Gets the SvnItem of the document file and all subdocument files (SccSpecial files)
        /// </summary>
        /// <param name="documentName">The document.</param>
        /// <returns></returns>
        public override IEnumerable<string> GetAllDocumentFiles(string documentName)
        {
            if (string.IsNullOrEmpty(documentName))
                throw new ArgumentNullException("document");

            SccProjectFile pf;
            if (!ProjectMap.TryGetFile(documentName, out pf))
                yield break;

            foreach(string path in pf.GetAllFiles())
            {
                SvnItem item = StatusCache[documentName];

                if (item != null)
                    yield return item.FullPath; // Use true path
            }
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
        sealed class QueryEditQueues
        {
            public HybridCollection<string> MustLockFiles;
            public List<SvnItem> MustLockItems;
            public HybridCollection<string> ReadOnlyFiles;
            public List<SvnItem> ReadOnlyItems;
        }

        public int QueryEditFiles(uint rgfQueryEdit, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pfEditVerdict, out uint prgfMoreInfo)
        {
            tagVSQueryEditFlags queryFlags = (tagVSQueryEditFlags)rgfQueryEdit;
            pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            prgfMoreInfo = 0;

            if (rgpszMkDocuments == null)
                return VSErr.E_POINTER;

            try
            {
                if ((queryFlags & tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting) != 0)
                    return QueryEditForceWritable(rgpszMkDocuments);

                bool? allowReadOnlyNonSccWrites = null;
                QueryEditQueues queues = new QueryEditQueues();

                if (!CollectQueryEditItems(
                        cFiles,
                        rgpszMkDocuments,
                        QueryEditLogic.AllowsUI(queryFlags),
                        ref allowReadOnlyNonSccWrites,
                        queues,
                        out pfEditVerdict,
                        out prgfMoreInfo))
                {
                    return VSErr.S_OK;
                }

                if (!TryLockQueryEditItems(queues))
                {
                    pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                    prgfMoreInfo = (uint)(
                        tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed
                        | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc);
                }

                if (!TryMakeReadOnlyQueryEditItemWritable(queues))
                {
                    pfEditVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                    prgfMoreInfo = (uint)(
                        tagVSQueryEditResultFlags.QER_InMemoryEditNotAllowed
                        | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc);
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

            return VSErr.S_OK;
        }

        bool CollectQueryEditItems(
            int cFiles,
            string[] documents,
            bool allowUI,
            ref bool? allowReadOnlyNonSccWrites,
            QueryEditQueues queues,
            out uint editVerdict,
            out uint moreInfo)
        {
            editVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            moreInfo = 0;

            for (int i = 0; i < cFiles; i++)
            {
                string file = documents[i];
                if (!IsSafeSccPath(file))
                    continue;

                SvnItem item = StatusCache[file];
                Monitor.ScheduleDirtyCheck(item);

                bool allowReadOnlyWrites = true;
                if (QueryEditLogic.NeedsReadOnlyNonSccPolicy(
                        item.IsReadOnlyMustLock,
                        item.IsDirectory,
                        item.IsReadOnly))
                {
                    if (!allowReadOnlyNonSccWrites.HasValue)
                        allowReadOnlyNonSccWrites = AllowReadOnlyNonSccWrites();

                    allowReadOnlyWrites = allowReadOnlyNonSccWrites.Value;
                }

                QueryEditFileAction action = QueryEditLogic.GetFileAction(
                    item.IsReadOnlyMustLock,
                    item.IsDirectory,
                    item.IsReadOnly,
                    allowUI,
                    allowReadOnlyWrites);

                if (!ApplyQueryEditAction(
                        action,
                        item,
                        queues,
                        out editVerdict,
                        out moreInfo))
                {
                    return false;
                }
            }

            return true;
        }

        static bool ApplyQueryEditAction(
            QueryEditFileAction action,
            SvnItem item,
            QueryEditQueues queues,
            out uint editVerdict,
            out uint moreInfo)
        {
            editVerdict = (uint)tagVSQueryEditResult.QER_EditOK;
            moreInfo = 0;

            switch (action)
            {
                case QueryEditFileAction.RejectMustLock:
                    editVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                    moreInfo = (uint)(
                        tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc
                        | tagVSQueryEditResultFlags.QER_NoisyCheckoutRequired);
                    return false;

                case QueryEditFileAction.QueueMustLock:
                    AddQueryEditItem(
                        item,
                        ref queues.MustLockFiles,
                        ref queues.MustLockItems);
                    return true;

                case QueryEditFileAction.RejectReadOnly:
                    editVerdict = (uint)tagVSQueryEditResult.QER_EditNotOK;
                    moreInfo = (uint)(
                        tagVSQueryEditResultFlags.QER_InMemoryEditNotAllowed
                        | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc
                        | tagVSQueryEditResultFlags.QER_NoisyPromptRequired);
                    return false;

                case QueryEditFileAction.QueueReadOnly:
                    AddQueryEditItem(
                        item,
                        ref queues.ReadOnlyFiles,
                        ref queues.ReadOnlyItems);
                    return true;

                default:
                    return true;
            }
        }

        static void AddQueryEditItem(
            SvnItem item,
            ref HybridCollection<string> files,
            ref List<SvnItem> items)
        {
            if (items == null)
            {
                files = new HybridCollection<string>(
                    StringComparer.OrdinalIgnoreCase);
                items = new List<SvnItem>();
            }

            if (files.Contains(item.FullPath))
                return;

            files.Add(item.FullPath);
            items.Add(item);
        }

        bool TryLockQueryEditItems(QueryEditQueues queues)
        {
            if (queues.MustLockItems == null)
                return true;

            List<SvnItem> mustBeLocked =
                new List<SvnItem>(queues.MustLockItems);

            ExpandQueryEditLockItems(queues);

            CommandService.DirectlyExecCommand(
                AnkhCommand.SccLock,
                queues.MustLockItems,
                CommandPrompt.DoDefault);

            foreach (SvnItem item in mustBeLocked)
            {
                if (item.IsReadOnlyMustLock)
                    return false;
            }

            return true;
        }

        void ExpandQueryEditLockItems(QueryEditQueues queues)
        {
            foreach (string lockFile in new List<string>(queues.MustLockFiles))
            {
                foreach (string file in GetAllDocumentFiles(lockFile))
                {
                    if (queues.MustLockFiles.Contains(file))
                        continue;

                    queues.MustLockFiles.Add(file);
                    queues.MustLockItems.Add(StatusCache[file]);
                }
            }
        }

        bool TryMakeReadOnlyQueryEditItemWritable(QueryEditQueues queues)
        {
            if (queues.ReadOnlyItems == null)
                return true;

            CommandResult result =
                CommandService.DirectlyExecCommand(
                    AnkhCommand.MakeNonSccFileWriteable,
                    queues.ReadOnlyItems[0],
                    CommandPrompt.DoDefault);

            return result.Result is bool && (bool)result.Result;
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
            return VSErr.S_OK;
        }

        bool AllowReadOnlyNonSccWrites()
        {
            IVsSccToolsOptions sccToolsOptions = GetService<IVsSccToolsOptions>(typeof(SVsSccToolsOptions));
            if (sccToolsOptions == null)
                return true;

            object o;
            if (!VSErr.Succeeded(
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
                return VSErr.E_POINTER;

            if (_querySaveBatchCancel)
            {
                pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                return VSErr.S_OK;
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
                            return VSErr.S_OK;
                        }
                        toBeSvnLocked.Add(item);
                        continue;
                    }
                    else if (!item.IsReadOnly)
                        continue;
                    else if (silent)
                    {
                        pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_NoisyPromptRequired;
                        return VSErr.S_OK;
                    }

                    tagVSQuerySaveResult rslt = QueryReadOnlyFile(item);
                    switch (rslt)
                    {
                        case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel;
                            if (IsInSaveBatch)
                                _querySaveBatchCancel = true;
                            return VSErr.S_OK;
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

                return VSErr.S_OK;
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

                DialogResult result = dlg.ShowDialog(this);
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
            return VSErr.S_OK;
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
            return VSErr.S_OK;
        }
#endif
    }
}
