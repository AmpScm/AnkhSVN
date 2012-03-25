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
using SvnEntry = SharpSvn.SvnWorkingCopyEntryEventArgs;

namespace Ankh.Scc
{
    /// <summary>
    /// Container of Svn/SharpSvn helper tools which should be refactored to a better location
    /// in a future version, but which functionality is required to get file tracking working
    /// </summary>
    sealed class SvnSccContext : AnkhService
    {
        SvnClient _svnClient;
        SvnWorkingCopyClient _wcClient;
        readonly IFileStatusCache _statusCache;
        bool _disposed;

        public SvnSccContext(IAnkhServiceProvider context)
            : base(context)
        {
            _statusCache = GetService<IFileStatusCache>();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_svnClient != null)
                    ((IDisposable)_svnClient).Dispose();

                if (_wcClient != null)
                    ((IDisposable)_wcClient).Dispose();
            }
            base.Dispose(disposing);
        }

        public SvnClient Client
        {
            get { return _svnClient ?? (_svnClient = GetService<ISvnClientPool>().GetNoUIClient()); }
        }

        public SvnWorkingCopyClient WcClient
        {
            get { return _wcClient ?? (_wcClient = GetService<ISvnClientPool>().GetWcClient()); }
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
        public SvnEntry SafeGetEntry(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            // We only have to look in the parent.
            // If the path is the working copy root, the name doesn't matter!

            string dir = SvnTools.GetNormalizedDirectoryName(path);

            SvnWorkingCopyEntryEventArgs entry = null;
            SvnWorkingCopyEntriesArgs wa = new SvnWorkingCopyEntriesArgs();
            wa.ThrowOnError = false;
            wa.ThrowOnCancel = false;
            WcClient.ListEntries(dir, wa,
                delegate(object sender, SvnEntry e)
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

        public void MetaMove(string oldName, string newName, bool tryRevert)
        {
            SvnEntry toBefore = null;

            if (tryRevert)
                toBefore = SafeGetEntry(newName);

            SvnWorkingCopyMoveArgs ma = new SvnWorkingCopyMoveArgs();
            ma.ThrowOnError = false;
            ma.MetaDataOnly = true;

            if(WcClient.Move(oldName, newName, ma) && tryRevert)
                MaybeRevert(newName, toBefore);
        }

        public void MetaCopy(string from, string newName, bool tryRevert)
        {
            SvnEntry toBefore = null;

            if (tryRevert)
                toBefore = SafeGetEntry(newName);

            SvnWorkingCopyCopyArgs ca = new SvnWorkingCopyCopyArgs();
            ca.ThrowOnError = false;
            ca.MetaDataOnly = true;

            if (WcClient.Copy(from, newName, ca) && tryRevert)
                MaybeRevert(newName, toBefore);
        }

        public bool MetaDelete(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnDeleteArgs da = new SvnDeleteArgs();
            da.ThrowOnError = false;
            da.Force = true;
            da.KeepLocal = true;

            return Client.Delete(path, da);
        }

        private void MaybeRevert(string newName, SvnEntry toBefore)
        {
            if (toBefore == null)
                return;

            SvnInfoArgs ia = new SvnInfoArgs();
            SvnInfoEventArgs info = null;

            ia.ThrowOnError = false;
            ia.Depth = SvnDepth.Empty;
            ia.Info += delegate(object sender, SvnInfoEventArgs e) { e.Detach(); info = e; };

            if (!Client.Info(newName, ia, null) || info == null)
                return;

            // Use SvnEntry to peek below the current delete
            if (toBefore.RepositoryId != info.RepositoryId
                || toBefore.Uri != info.CopyFromUri
                || toBefore.Revision != info.CopyFromRevision)
            {
                return;
            }

            using (MarkIgnoreRecursive(newName))
            using (MoveAway(newName))
            {
                SvnRevertArgs ra = new SvnRevertArgs();
                ra.Depth = SvnDepth.Empty;
                ra.ThrowOnError = false;

                // Do a quick check if we can safely revert with depth infinity
                using (new SharpSvn.Implementation.SvnFsOperationRetryOverride(0))
                {
                    SvnStatusArgs sa = new SvnStatusArgs();
                    sa.ThrowOnError = false;
                    bool modifications = false;
                    if (Client.Status(newName, sa,
                                  delegate(object sender, SvnStatusEventArgs e)
                                  {
                                      if (e.LocalPropertyStatus != SvnStatus.Normal
                                          && e.LocalPropertyStatus != SvnStatus.None)
                                      {
                                          e.Cancel = modifications = true;
                                      }
                                      else if (e.FullPath != newName)
                                          switch (e.LocalNodeStatus)
                                          {
                                              case SvnStatus.None:
                                              case SvnStatus.Modified: // Text only change is ok
                                              case SvnStatus.Ignored:
                                              case SvnStatus.External:
                                                  break;
                                              default:
                                                  e.Cancel = modifications = true;
                                                  break;
                                          }
                                  })
                        && !modifications)
                    {
                        ra.Depth = SvnDepth.Infinity;
                    }
                }
                
                Client.Revert(newName, ra);
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

        public IDisposable MoveAway(string path)
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

                    RetriedRename(tmp, path);

                    if (isFile)
                    {
                        File.SetAttributes(path, attrs);
                    }
                });
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

        internal void AddParents(string newParent)
        {
            SvnAddArgs aa = new SvnAddArgs();
            aa.AddParents = true;
            aa.ThrowOnError = false;
            aa.Depth = SvnDepth.Empty;

            Client.Add(SvnTools.GetNormalizedDirectoryName(newParent), aa);
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
