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
using SharpSvn;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Ankh.Scc
{
    /// <summary>
    /// Container of Svn/SharpSvn helper tools which should be refactored to a better location
    /// in a future version, but which functionality is required to get file tracking working
    /// </summary>
    sealed class SvnSccContext : AnkhService
    {
        readonly SvnClient _client;
        readonly IFileStatusCache _statusCache;
        bool _disposed;

        public SvnSccContext(IAnkhServiceProvider context)
            : base(context)
        {
            _client = context.GetService<ISvnClientPool>().GetNoUIClient();
            _statusCache = GetService<IFileStatusCache>();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                ((IDisposable)_client).Dispose();
            }
            base.Dispose(disposing);
        }

        IFileStatusCache StatusCache
        {
            get { return _statusCache; }
        }

        /// <summary>
        /// Gets the working copy entry information on the specified path from its parent
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public SvnWorkingCopyEntryEventArgs SafeGetEntry(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            // We only have to look in the parent.
            // If the path is the working copy root, the name doesn't matter!

            string dir = SvnTools.GetNormalizedDirectoryName(path);

            using (SvnWorkingCopyClient wcc = GetService<ISvnClientPool>().GetWcClient())
            {
                SvnWorkingCopyEntryEventArgs entry = null;
                SvnWorkingCopyEntriesArgs wa = new SvnWorkingCopyEntriesArgs();
                wa.ThrowOnError = false;
                wa.ThrowOnCancel = false;
                wcc.ListEntries(dir, wa,
                    delegate(object sender, SvnWorkingCopyEntryEventArgs e)
                    {
                        if (entry == null && path == e.FullPath)
                        {
                            e.Detach();
                            entry = e;
                            e.Cancel = true;
                        }
                    });

                return entry;
            }
        }

        public void MarkAsMoved(string oldName, string newName)
        {
            using (SvnWorkingCopyClient wcc = GetService<ISvnClientPool>().GetWcClient())
            {
                SvnWorkingCopyMoveArgs ma = new SvnWorkingCopyMoveArgs();
                ma.ThrowOnError = false;
                ma.MetaDataOnly = true;

                wcc.Move(oldName, newName);
            }
        }

        /// <summary>
        /// Tries to get the repository Guid of the specified path when it would be added to subversion
        /// </summary>
        /// <param name="path"></param>
        /// <param name="wcGuid"></param>
        /// <returns></returns>
        public bool TryGetRepositoryId(string path, out Guid repositoryId)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            if (File.Exists(path))
                path = SvnTools.GetNormalizedDirectoryName(path);

            if (Directory.Exists(path))
                path = SvnTools.GetTruePath(path); // Resolve casing issues

            while (!string.IsNullOrEmpty(path))
            {
                if (SvnTools.IsManagedPath(path))
                {
                    return _client.TryGetRepositoryId(path, out repositoryId);
                }

                path = SvnTools.GetNormalizedDirectoryName(path);
            }

            repositoryId = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Reverts the specified path if the path specifies a file replaced with itself
        /// </summary>
        /// <param name="path"></param>
        void MaybeRevertReplaced(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item = StatusCache[path];
            item.MarkDirty();

            if (!item.IsFile || item.Status.LocalNodeStatus != SvnStatus.Replaced)
                return;

            SvnInfoEventArgs info = null;
            SvnInfoArgs ia = new SvnInfoArgs();
            ia.ThrowOnError = false;
            ia.Depth = SvnDepth.Empty;

            if (!_client.Info(path, ia,
                delegate(object sender, SvnInfoEventArgs e)
                {
                    e.Detach();
                    info = e;
                }))
            {
                return;
            }

            if (info == null)
                return;

            if ((info.CopyFromUri != null) && (info.Uri != info.CopyFromUri))
                return;
            else if (info.CopyFromRevision >= 0 && info.CopyFromRevision != info.Revision)
                return;

            // Ok, the file was copied back to its original location!

            SvnRevertArgs ra = new SvnRevertArgs();
            ra.Depth = SvnDepth.Empty;
            ra.ThrowOnError = false;

            // Our callers should move away the file, but we can't be to sure here
            using (MoveAway(path, true))
            {
                _client.Revert(path, ra);
            }
        }

        /// <summary>
        /// Safes the wc copy to dir fixup.
        /// </summary>
        /// <param name="files">The files a dictionary mapping the result to the origin</param>
        /// <param name="toDir">To dir.</param>
        /// <returns></returns>
        /// <remarks>
        /// Files can't be renamed by this action!
        /// </remarks>
        public bool SafeWcCopyToDirFixup(IDictionary<string, string> files, string toDir)
        {
            if (files == null)
                throw new ArgumentNullException("files");
            else if (string.IsNullOrEmpty(toDir))
                throw new ArgumentNullException("toDir");

            List<string> from = new List<string>(files.Values);
            List<string> tryMove = new List<string>();

            foreach (string f in from)
            {
                if (!File.Exists(f))
                    tryMove.Add(f);
            }

            using (MarkIgnoreFiles(files.Keys))
            using (MarkIgnoreFiles(tryMove))
            using (TempRevertForCopy(tryMove))
            {
                List<string> setReadOnly = null;
                using (MoveAwayFiles(files.Keys, true))
                {
                    EnsureAdded(toDir);

                    List<SvnPathTarget> toCopy = new List<SvnPathTarget>();
                    List<string> toMove = new List<string>();
                    foreach (string f in files.Values)
                    {
                        if (tryMove.Contains(f))
                            toMove.Add(f);
                        else
                            toCopy.Add(f);
                    }

                    if (toCopy.Count > 0)
                    {
                        SvnCopyArgs ca = new SvnCopyArgs();
                        ca.ThrowOnError = false;
                        ca.AlwaysCopyAsChild = true;

                        _client.Copy(toCopy, toDir, ca);
                    }

                    if (toMove.Count > 0)
                    {
                        SvnMoveArgs ma = new SvnMoveArgs();
                        ma.ThrowOnError = false;
                        ma.AlwaysMoveAsChild = true;

                        _client.Move(toMove, toDir, ma);
                    }

                    foreach (string f in files.Keys)
                    {
                        if (File.Exists(f))
                        {
                            if ((int)(File.GetAttributes(f) & FileAttributes.ReadOnly) != 0)
                            {
                                setReadOnly = new List<string>();
                                setReadOnly.Add(f);
                            }

                            MaybeRevertReplaced(f);
                        }
                    }
                }

                if (setReadOnly != null)
                    foreach (string f in setReadOnly)
                        File.SetAttributes(f, File.GetAttributes(f) | FileAttributes.ReadOnly);
            }

            return true;
        }

        public bool SafeWcCopyFixup(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");
            else if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            if (!File.Exists(fromPath))
                throw new InvalidOperationException();

            bool ok;
            bool setReadOnly = false;
            using (MarkIgnoreFile(toPath))
            {
                using (MoveAway(toPath, true))
                {
                    string toDir = SvnTools.GetNormalizedDirectoryName(toPath);

                    EnsureAdded(toDir);

                    SvnCopyArgs ca = new SvnCopyArgs();
                    ca.AlwaysCopyAsChild = false;
                    ca.CreateParents = false; // We just did that ourselves. Use Svn for this?
                    ca.ThrowOnError = false;

                    ok = _client.Copy(fromPath, toPath, ca);

                    if (ok && File.Exists(toPath))
                    {
                        setReadOnly = (int)(File.GetAttributes(toPath) & FileAttributes.ReadOnly) != 0;
                    }

                    MaybeRevertReplaced(toPath);
                }

                if (setReadOnly)
                    File.SetAttributes(toPath, File.GetAttributes(toPath) | FileAttributes.ReadOnly);
            }

            return ok;
        }

        private void EnsureAdded(string toDir)
        {
            if (!SvnTools.IsManagedPath(toDir))
            {
                SvnAddArgs aa = new SvnAddArgs();
                aa.Depth = SvnDepth.Empty;
                aa.AddParents = true;
                aa.Force = true;
                aa.ThrowOnError = false;

                if (!_client.Add(toDir, aa))
                    throw new InvalidOperationException();
            }
        }

        internal bool SafeWcMoveFixup(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");
            else if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            bool setReadOnly = false;
            bool ok = true;

            using (HandsOff(fromPath))
            using (HandsOff(toPath))
            using (MarkIgnoreFile(fromPath))
            using (MarkIgnoreFile(toPath))
            {
                using (TempFile(fromPath, toPath))
                using (MoveAway(toPath, true))
                {
                    SvnMoveArgs ma = new SvnMoveArgs();
                    ma.AlwaysMoveAsChild = false;
                    ma.CreateParents = true; 
                    ma.Force = true;
                    ma.ThrowOnError = false;

                    ok = _client.Move(fromPath, toPath, ma);

                    if (!ok && ma.LastException.SvnErrorCode == SvnErrorCode.SVN_ERR_WC_PATH_NOT_FOUND)
                    {
                        // The directory exists, but is no working copy
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.Depth = SvnDepth.Empty;
                        aa.AddParents = true;
                        aa.Force = true;
                        aa.ThrowOnError = false;

                        _client.Add(SvnTools.GetNormalizedDirectoryName(toPath), aa);

                        // And retry
                        ok = _client.Move(fromPath, toPath, ma);
                    }

                    if (ok)
                    {
                        setReadOnly = (File.GetAttributes(toPath) & FileAttributes.ReadOnly) != (FileAttributes)0;
                    }

                    MaybeRevertReplaced(toPath);
                }

                if (setReadOnly)
                    File.SetAttributes(toPath, File.GetAttributes(toPath) | FileAttributes.ReadOnly);
            }

            return ok;
        }

        internal void SafeWcDirectoryCopyFixUp(string oldDir, string newDir, bool safeRename)
        {
            if (string.IsNullOrEmpty(oldDir))
                throw new ArgumentNullException("oldDir");
            else if (string.IsNullOrEmpty(newDir))
                throw new ArgumentNullException("newDir");

            using (HandsOffRecursive(newDir))
            using (MarkIgnoreRecursive(newDir))
            {
                RetriedRename(newDir, oldDir);

                if (safeRename)
                    RecursiveCopyWc(oldDir, newDir);
                else
                    RecursiveCopyNotVersioned(oldDir, newDir, false);
            }
        }

        private IDisposable MarkIgnoreRecursive(string newDir)
        {
            IAnkhOpenDocumentTracker dt = GetService<IAnkhOpenDocumentTracker>();
            IVsFileChangeEx change = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

            if (dt == null || change == null)
                return null;

            ICollection<string> files = dt.GetDocumentsBelow(newDir);

            if (files == null || files.Count == 0)
                return null;

            foreach (string file in files)
            {
                Marshal.ThrowExceptionForHR(change.IgnoreFile(0, file, 1));
            }

            return new DelegateRunner(
                delegate()
                {
                    foreach (string file in files)
                    {
                        change.SyncFile(file);
                        change.IgnoreFile(0, file, 0);
                    }
                });
        }

        private IDisposable HandsOffRecursive(string newDir)
        {
            IAnkhOpenDocumentTracker dt = GetService<IAnkhOpenDocumentTracker>();
            IVsTrackProjectDocuments3 tracker = GetService<IVsTrackProjectDocuments3>(typeof(SVsTrackProjectDocuments));

            if (dt == null || tracker == null)
                return null;

            ICollection<string> files = dt.GetDocumentsBelow(newDir);

            if (files == null || files.Count == 0)
                return null;

            string[] fileArray = new List<string>(files).ToArray();

            Marshal.ThrowExceptionForHR(tracker.HandsOffFiles(
                (uint)__HANDSOFFMODE.HANDSOFFMODE_DeleteAccess,
                fileArray.Length, fileArray));

            return new DelegateRunner(
                delegate()
                {
                    tracker.HandsOnFiles(fileArray.Length, fileArray);
                });
        }

        /// <summary>
        /// Creates an unversioned copy of <paramref name="from"/> in <paramref name="to"/>. (Recursive copy skipping administrative directories)
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        private void RecursiveCopyNotVersioned(string from, string to, bool force)
        {
            DirectoryInfo fromDir = new DirectoryInfo(from);
            DirectoryInfo toDir = new DirectoryInfo(to);

            if (!fromDir.Exists)
                return;

            if (!toDir.Exists)
                toDir.Create();

            foreach (FileInfo file in fromDir.GetFiles())
            {
                string toFile = Path.Combine(to, file.Name);
                if (force)
                {
                    // toFile might be read only
                    FileInfo toInfo = new FileInfo(toFile);

                    if (toInfo.Exists && (toInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        toInfo.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }

                File.Copy(file.FullName, toFile, force);
            }

            foreach (DirectoryInfo dir in fromDir.GetDirectories())
            {
                if (!string.Equals(dir.Name, SvnClient.AdministrativeDirectoryName, StringComparison.OrdinalIgnoreCase))
                    RecursiveCopyNotVersioned(dir.FullName, Path.Combine(to, dir.Name), force);
            }
        }

        private void RecursiveCopyWc(string from, string to)
        {
            // First, copy the way subversion likes it
            SvnCopyArgs ca = new SvnCopyArgs();
            ca.AlwaysCopyAsChild = false;
            ca.CreateParents = false;
            ca.ThrowOnError = false;
            _client.Copy(from, to, ca);

            // Now copy everything unversioned from our local backup back
            // into the new workingcopy, to be 100% sure VS finds what it expects

            RecursiveCopyNotVersioned(from, to, true);
        }

        public bool SafeDelete(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnDeleteArgs da = new SvnDeleteArgs();
            da.Force = true;
            da.KeepLocal = false;
            da.ThrowOnError = false;
            da.KeepLocal = !SvnItem.PathExists(path); // This will stop the error if the file was already deleted

            return _client.Delete(path, da);
        }

        public bool IsUnversioned(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            SvnWorkingCopyEntryEventArgs status = SafeGetEntry(name);

            if (status == null)
                return true;

            switch (status.Schedule)
            {
                case SvnSchedule.Delete:
                    return true; // The item was already deleted
                default:
                    return false;
            }
        }

        sealed class DelegateRunner : IDisposable
        {
            AnkhAction _runner;
            public DelegateRunner(AnkhAction runner)
            {
                if (runner == null)
                    throw new ArgumentNullException("runner");
                _runner = runner;
            }

            public void Dispose()
            {
                AnkhAction runner = _runner;
                _runner = null;
                runner();
            }
        }

        public IDisposable HandsOff(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            IVsTrackProjectDocuments3 tracker = GetService<IVsTrackProjectDocuments3>(typeof(SVsTrackProjectDocuments));

            if (tracker != null)
            {
                string[] paths = new string[] { path, null };

                int hr = tracker.HandsOffFiles(
                    (uint)__HANDSOFFMODE.HANDSOFFMODE_DeleteAccess,
                    1,
                    paths);


                Marshal.ThrowExceptionForHR(hr);

                return new DelegateRunner(
                    delegate()
                    {
                        tracker.HandsOnFiles(1, paths);
                    });
            }
            else
                return null;
        }

        public IDisposable MarkIgnoreFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            IVsFileChangeEx change = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

            if (change != null)
            {
                Marshal.ThrowExceptionForHR(change.IgnoreFile(0, path, 1));

                return new DelegateRunner(
                    delegate()
                    {
                        change.SyncFile(path);
                        change.IgnoreFile(0, path, 0);
                    });
            }
            else
                return null;
        }

        public IDisposable MarkIgnoreFiles(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            List<IDisposable> disps = new List<IDisposable>();

            foreach (string path in paths)
            {
                IDisposable d = MarkIgnoreFile(path);

                if (d != null)
                    disps.Add(d);
            }

            if (disps.Count > 0)
                return new DelegateRunner(
                    delegate
                    {
                        foreach (IDisposable d in disps)
                        {
                            d.Dispose();
                        }
                    });
            else
                return null;
        }

        public IDisposable TempRevertForCopy(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            List<string> deletePaths = new List<string>();
            SvnRevertArgs ra = new SvnRevertArgs();
            ra.ThrowOnError = false;

            foreach (string p in paths)
            {
                if (_client.Revert(p, ra))
                    deletePaths.Add(p);
            }

            if (deletePaths.Count > 0)
            {
                return new DelegateRunner(
                    delegate
                    {
                        SvnDeleteArgs da = new SvnDeleteArgs();
                        da.ThrowOnError = false;
                        foreach (string p in deletePaths)
                        {
                            SvnItem item = StatusCache[p];

                            item.MarkDirty();

                            if (item.Exists && !item.IsDeleteScheduled)
                                _client.Delete(p, da);
                        }
                    });
            }
            else
                return null;
        }

        public IDisposable MoveAway(string path, bool touch)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);
            bool isFile = true;

            if (!File.Exists(path))
            {
                if (Directory.Exists(path))
                    isFile = false;
                else
                    throw new InvalidOperationException();
            }

            FileAttributes attrs = FileAttributes.Normal;
            if (isFile)
            {
                attrs = File.GetAttributes(path);
                File.SetAttributes(path, FileAttributes.Normal);
            }

            string tmp;
            int n = 0;
            do
            {
                tmp = path + string.Format(".AnkhSVN.{0}.tmp", n++);
            }
            while (File.Exists(tmp) || Directory.Exists(tmp));

            RetriedRename(path, tmp);

            if (isFile)
                File.SetAttributes(tmp, FileAttributes.ReadOnly);

            return new DelegateRunner(
                delegate()
                {
                    if (SvnItem.PathExists(path))
                    {
                        SvnItem.DeleteNode(path);
                    }

                    if (isFile)
                        File.SetAttributes(tmp, FileAttributes.Normal);

                    RetriedRename(tmp, path);

                    if (isFile)
                    {
                        if (touch)
                            File.SetLastWriteTime(path, DateTime.Now);

                        File.SetAttributes(path, attrs);
                    }
                });
        }

        public IDisposable MoveAwayFiles(IEnumerable<string> paths, bool touch)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            List<IDisposable> disps = new List<IDisposable>();

            foreach (string path in paths)
            {
                IDisposable d = MoveAway(path, touch);

                if (d != null)
                    disps.Add(d);
            }

            if (disps.Count > 0)
                return new DelegateRunner(
                    delegate
                    {
                        foreach (IDisposable d in disps)
                        {
                            d.Dispose();
                        }
                    });
            else
                return null;
        }

        /// <summary>
        /// Performs a few attempts on renaming a directory and only fails if all fail
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tmp"></param>
        internal static void RetriedRename(string path, string tmp)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            else if (string.IsNullOrEmpty(tmp))
                throw new ArgumentNullException("tmp");

            const int retryCount = 5;
            for (int i = 0; i < retryCount; i++)
            {
                // Don't throw an exception on the common case the file is locked
                // The project just renamed the file so a virusscanner or directory scanner (Tortoise, VS itself)
                // Will now look at the file
                if (!NativeMethods.MoveFile(path, tmp))
                {
                    if (i == retryCount - 1)
                    {
                        // Throw an exception after 4 attempts

                        if (Directory.Exists(path))
                            Directory.Move(path, tmp);
                        else
                            File.Move(path, tmp);

                    }
                    else
                        System.Threading.Thread.Sleep(50 * (i + 1));
                }
                else
                    break;
            }
        }

        public IDisposable TempFile(string path, string contentFrom)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            else if (string.IsNullOrEmpty(contentFrom))
                throw new ArgumentNullException("contentFrom");

            IDisposable moveAway = null;

            if (SvnItem.PathExists(path))
                moveAway = MoveAway(path, false);
            else if (!SvnItem.PathExists(contentFrom))
                throw new InvalidOperationException("Source does not exist");

            string dir = SvnTools.GetNormalizedDirectoryName(path);
            int nDirsCreated = 0;

            if (!SvnItem.PathExists(dir))
            {
                nDirsCreated = 1;

                string pd = SvnTools.GetNormalizedDirectoryName(dir);
                while (pd != Path.GetPathRoot(pd)
                       && !SvnItem.PathExists(pd))
                {
                    nDirsCreated++;
                    pd = SvnTools.GetNormalizedDirectoryName(pd);
                }

                Directory.CreateDirectory(dir);
            }

            File.Copy(contentFrom, path);

            return new DelegateRunner(
                delegate()
                {
                    if (SvnItem.PathExists(path))
                    {
                        try
                        {
                            File.SetAttributes(path, FileAttributes.Normal);
                        }
                        catch { }
                        SvnItem.DeleteNode(path);
                    }

                    string dd = SvnTools.GetNormalizedDirectoryName(path);
                    while (nDirsCreated-- > 0)
                    {
                        if (SvnItem.PathExists(dd))
                            if (!SvnItem.DeleteDirectory(dd))
                                break;

                        dd = SvnTools.GetNormalizedDirectoryName(dd);
                    }

                    if (moveAway != null)
                        moveAway.Dispose();
                });
        }

        /// <summary>
        /// Removes all administrative directories from the specified path recursively
        /// </summary>
        /// <param name="directory"></param>
        public void UnversionRecursive(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("directory");

            DirectoryInfo dir = new DirectoryInfo(directory);

            if (!dir.Exists)
                return;

            foreach (DirectoryInfo subDir in dir.GetDirectories(SvnClient.AdministrativeDirectoryName, SearchOption.AllDirectories))
            {
                SvnItem.DeleteDirectory(subDir.FullName, true);
            }
        }

        internal string MakeBackup(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            if (SvnItem.PathExists(fullPath))
            {
                string tmp;
                int n = 0;

                do
                {
                    tmp = string.Format("{0}.tmp{1}", fullPath, n++);
                }
                while (SvnItem.PathExists(tmp));

                DirectoryInfo source = new DirectoryInfo(fullPath);
                DirectoryInfo dest = Directory.CreateDirectory(tmp);

                RecursiveCopy(source, dest);

                return tmp;
            }
            else
                return null;
        }

        private void RecursiveCopy(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (FileInfo sourceFile in source.GetFiles())
            {
                FileInfo destFile = sourceFile.CopyTo(Path.Combine(destination.FullName, sourceFile.Name));
                destFile.Attributes = sourceFile.Attributes;
            }

            foreach (DirectoryInfo subDirSource in source.GetDirectories())
            {
                DirectoryInfo subDirDestination = destination.CreateSubdirectory(subDirSource.Name);
                subDirDestination.Attributes = subDirSource.Attributes;
                RecursiveCopy(subDirSource, subDirDestination);
            }
        }

        static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool MoveFile(string src, string dst);
        }
    }
}
