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

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    partial class AnkhSccProvider : IVsQueryEditQuerySave2
    {
        readonly SortedList<string, int> _unreloadable = new SortedList<string, int>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Creates a batch of a sequence of documents before attempting to save them to disk.
        /// </summary>
        /// <returns></returns>
        public int BeginQuerySaveBatch()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Ends the batch started by the BeginQuerySaveBatch method and 
        /// displays any user interface (UI) generated within the batch
        /// </summary>
        /// <returns></returns>
        public int EndQuerySaveBatch()
        {
            return VSConstants.S_OK;
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
            MarkDirty(pszMkDocument, true);

            return VSConstants.S_OK;
        }


        /// <summary>
        /// Gets the SvnItem of the document file and all subdocument files (SccSpecial files)
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        IEnumerable<SvnItem> GetAllDocumentItems(string document)
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

            HybridCollection<string> mustLockFiles = null;
            List<SvnItem> mustLockItems = null;
            if (rgpszMkDocuments != null)
            {
                for (int i = 0; i < cFiles; i++)
                {
                    string file = rgpszMkDocuments[i];

                    if (!SvnItem.IsValidPath(file) || file.StartsWith(TempPathWithSeparator, StringComparison.OrdinalIgnoreCase))
                        continue; // Just allow temporary files; don't look them up in our tables

                    foreach (SvnItem item in GetAllDocumentItems(rgpszMkDocuments[i]))
                    {
                        if (!item.IsVersioned)
                            continue;

                        if (item.ReadOnlyMustLock)
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
            }
            if (mustLockItems != null)
            {
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();

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
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="rgf">The RGF.</param>
        /// <param name="pFileInfo">The p file info.</param>
        /// <param name="pdwQSResult">The PDW QS result.</param>
        /// <returns></returns>
        public int QuerySaveFile(string pszMkDocument, uint rgf, VSQEQS_FILE_ATTRIBUTE_DATA[] pFileInfo, out uint pdwQSResult)
        {
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;
            if (!string.IsNullOrEmpty(pszMkDocument) &&
                StatusCache.IsValidPath(pszMkDocument) && !pszMkDocument.StartsWith(TempPathWithSeparator))
            {
                SvnItem item = StatusCache[pszMkDocument];
                if (item != null)
                {
                    if (item.ReadOnlyMustLock)
                    {
                        IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                        cmdSvc.DirectlyExecCommand(AnkhCommand.Lock, new SvnItem[] { item });
                        pdwQSResult = item.ReadOnlyMustLock ? (uint)tagVSQuerySaveResult.QSR_NoSave_Cancel : (uint)tagVSQuerySaveResult.QSR_SaveOK;
                        return VSConstants.S_OK;
                    }
                }

                MarkDirty(pszMkDocument, true);
            }

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
        /// <returns></returns>
        public int QuerySaveFiles(uint rgfQuerySave, int cFiles, string[] rgpszMkDocuments, uint[] rgrgf, VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo, out uint pdwQSResult)
        {
            pdwQSResult = (uint)tagVSQuerySaveResult.QSR_SaveOK;

            if (rgpszMkDocuments != null)
                for (int i = 0; i < cFiles; i++)
                    MarkDirty(rgpszMkDocuments[i], true);

            return VSConstants.S_OK;
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
