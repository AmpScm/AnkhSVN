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
    class SvnSccContext : AnkhService
    {
        readonly SvnClient _client;
        readonly IFileStatusCache _statusCache;

        public SvnSccContext(IAnkhServiceProvider context)
            : base(context)
        {
            _client = context.GetService<ISvnClientPool>().GetNoUIClient();
            _statusCache = GetService<IFileStatusCache>();
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }

        /// <summary>
        /// Gets the SVN-Status of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>Returns the status of a file; eats all Svn exceptions</remarks>
        public SvnStatusEventArgs SafeGetStatus(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (File.Exists(path))
                path = SvnTools.GetTruePath(path); // Resolve casing issues

            SvnStatusArgs a = new SvnStatusArgs();
            a.ThrowOnError = false;
            a.RetrieveAllEntries = true;
            a.Depth = SvnDepth.Empty;
            a.RetrieveIgnoredEntries = true;

            SvnStatusEventArgs status = null;

            if (_client.Status(path, a,
                delegate(object sender, SvnStatusEventArgs e)
                {
                    e.Detach();
                    status = e;
                }))
            {
                return status;
            }

            return null;
        }

        /// <summary>
        /// Gets the SVN-Info of a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>Returns the info of a file; eats all Svn exceptions</remarks>
        public SvnInfoEventArgs SafeGetInfo(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (File.Exists(path))
                path = SvnTools.GetTruePath(path); // Resolve casing issues

            SvnInfoArgs a = new SvnInfoArgs();
            a.ThrowOnError = false;
            a.Depth = SvnDepth.Empty;

            SvnInfoEventArgs info = null;

            if (_client.Info(path, a,
                delegate(object sender, SvnInfoEventArgs e)
                {
                    e.Detach();
                    info = e;
                }))
            {
                return info;
            }

            return null;
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

            path = Path.GetFullPath(path);

            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            if (Directory.Exists(path))
                path = SvnTools.GetTruePath(path); // Resolve casing issues

            string root = Path.GetPathRoot(path);
            repositoryId = Guid.Empty;

            Guid repId = Guid.Empty;

            // Svn does not allow a repository in a path root 
            // (See: canonicalization rules)
            while (!string.IsNullOrEmpty(path))
            {
                if (SvnTools.IsManagedPath(path))
                {
                    SvnInfoArgs a = new SvnInfoArgs();
                    a.ThrowOnError = false;
                    a.Depth = SvnDepth.Empty;

                    if (_client.Info(new SvnPathTarget(path),
                        delegate(object sender, SvnInfoEventArgs e)
                        {
                            repId = e.RepositoryId;
                        }))
                    {
                        if (repId != Guid.Empty) // Directory was just added; Guid not updated
                        {
                            repositoryId = repId;
                            return true;
                        }
                    }
                    else
                        return false;
                }

                path = Path.GetDirectoryName(path);
            }

            return false;
        }

        /// <summary>
        /// Reverts the specified path if the path specifies a file replaced with itself
        /// </summary>
        /// <param name="path"></param>
        void MaybeRevertReplaced(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item = GetService<IFileStatusCache>()[path];
            item.MarkDirty();

            if (!item.IsFile || item.Status.LocalContentStatus != SvnStatus.Replaced)
                return;

            SvnInfoEventArgs info = null;
            SvnInfoArgs ia = new SvnInfoArgs();
            ia.ThrowOnError = false;
            ia.Depth = SvnDepth.Empty;

            if (!_client.Info(new SvnPathTarget(path), ia,
                delegate(object sender, SvnInfoEventArgs e)
                {
                    e.Detach();
                    info = e;
                }))
            {
                return;
            }

            if(info == null)
                return;

            if((info.CopyFromUri != null) && (info.Uri != info.CopyFromUri))
                return;
            else if(info.CopyFromRev >= 0 && info.CopyFromRev != info.Revision)
                return;
            
            // Ok, the file was copied back to its original location!

            SvnRevertArgs ra = new SvnRevertArgs();
            ra.Depth = SvnDepth.Empty;
            ra.ThrowOnError = false;

            _client.Revert(path, ra);
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
                    string toDir = Path.GetDirectoryName(toPath);

                    if (!SvnTools.IsManagedPath(toDir))
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.Depth = SvnDepth.Empty;
                        aa.AddParents = true;
                        aa.Force = true;
                        aa.ThrowOnError = false;

                        if (!_client.Add(toDir, aa))
                            return false;
                    }

                    Debug.Assert(SvnTools.IsManagedPath(toDir));

                    SvnCopyArgs ca = new SvnCopyArgs();
                    ca.AlwaysCopyAsChild = false;
                    ca.MakeParents = false; // We just did that ourselves. Use Svn for this?
                    ca.ThrowOnError = false;

                    ok = _client.Copy(new SvnPathTarget(fromPath), toPath, ca);


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
                    string toDir = Path.GetDirectoryName(toPath);

                    if (!SvnTools.IsManagedPath(toDir))
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.Depth = SvnDepth.Empty;
                        aa.AddParents = true;
                        aa.Force = true;
                        aa.ThrowOnError = false;

                        if (!_client.Add(toDir, aa))
                            return false;
                    }

                    Debug.Assert(SvnTools.IsManagedPath(toDir));

                    SvnMoveArgs ma = new SvnMoveArgs();
                    ma.AlwaysMoveAsChild = false;
                    ma.MakeParents = false; // We just did that ourselves. Use Svn for this?
                    ma.Force = true;
                    ma.ThrowOnError = false;

                    ok = _client.Move(fromPath, toPath, ma);

                    if (ok)
                    {
                        setReadOnly = (File.GetAttributes(toPath) & FileAttributes.ReadOnly) != (FileAttributes)0;
                    }
                }

                MaybeRevertReplaced(toPath);

                if (setReadOnly)
                    File.SetAttributes(toPath, File.GetAttributes(toPath) | FileAttributes.ReadOnly);
            }

            return ok;
        }

        public bool SafeDelete(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnDeleteArgs da = new SvnDeleteArgs();
            da.Force = true;
            da.KeepLocal = false;
            da.ThrowOnError = false;

            return _client.Delete(path, da);
        }

        /// <summary>
        /// Gets a boolean indicating whether to calculate the specified status as unversioned
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool IsUnversioned(SvnStatusEventArgs status)
        {
            if (status == null)
                return true;

            switch (status.LocalContentStatus)
            {
                case SvnStatus.Ignored:
                case SvnStatus.None:
                case SvnStatus.Zero:
                case SvnStatus.NotVersioned:
                    return true;

                case SvnStatus.Deleted:
                    // What to do with this?
                    // * If there is a file it is unmanaged; but probably was once managed?
                default:
                    return false;
            }
        }

        sealed class DelegateRunner : IDisposable
        {
            public delegate void Runner();
            Runner _runner;
            public DelegateRunner(Runner runner)
            {
                if (runner == null)
                    throw new ArgumentNullException("runner");
                _runner = runner;
            }

            public void Dispose()
            {
                Runner runner = _runner;
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

                int hr =tracker.HandsOffFiles(
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

            IVsFileChangeEx change = GetService <IVsFileChangeEx>(typeof(SVsFileChangeEx));

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

        public IDisposable MoveAway(string path, bool touch)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = Path.GetFullPath(path);
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
  
            if(isFile)
                File.SetAttributes(tmp, FileAttributes.ReadOnly);

            return new DelegateRunner(
                delegate()
                {
                    if (isFile && File.Exists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        File.Delete(path);
                    }
                    else if (!isFile && Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }

                    if(isFile)
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

        private static void RetriedRename(string path, string tmp)
        {
            const int retryCount = 4;
            for (int i = 0; i < retryCount; i++)
            {
                // Don't throw an exception on the common case the file is locked
                // The project just renamed the file so a virusscanner or directory scanner (Tortoise)
                // Will now look at the file
                if (!NativeMethods.MoveFile(path, tmp))
                {
                    if (i == retryCount - 1)
                    {
                        File.Move(path, tmp); // Throw an exception after 4 attempts
                    }
                    else
                        System.Threading.Thread.Sleep(20 * (i + 1));
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
                throw new ArgumentNullException("path");

            if (File.Exists(path))
                throw new InvalidOperationException("Destination exists");
            else if (!File.Exists(contentFrom))
                throw new InvalidOperationException("Source does not exist");

            File.Copy(contentFrom, path);

            return new DelegateRunner(
                delegate()
                {
                    if (File.Exists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        File.Delete(path);
                    }
                });
        }

        /// <summary>
        /// Check if adding the path might succeed
        /// </summary>
        /// <param name="path"></param>
        /// <returns><c>false</c> when adding the file will fail, <c>true</c> if it could succeed</returns>
        public bool CouldAdd(string path, SvnNodeKind nodeKind)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            
            SvnItem item = _statusCache[path];            
            string file = Path.GetFileName(path);

            if (item.IsVersioned && item.Name == file)
                return true;

            string parentDir = Path.GetDirectoryName(path);
            SvnStatusArgs sa = new SvnStatusArgs();
            sa.ThrowOnError = false;
            sa.RetrieveAllEntries = true;
            
            sa.Depth = nodeKind == SvnNodeKind.File ? SvnDepth.Files : SvnDepth.Children;
            bool ok = true;

            _client.Status(parentDir, sa,
                delegate(object sender, SvnStatusEventArgs e)
                {
                    if (string.Equals(Path.GetDirectoryName(e.FullPath), parentDir, StringComparison.OrdinalIgnoreCase))
                    {
                        string fn = Path.GetFileName(e.FullPath);
                        if (string.Equals(fn, file, StringComparison.OrdinalIgnoreCase))
                        {
                            if (fn != file)
                                ok = false; // Casing issue
                        }
                    }
                });

            return ok;
        }

        static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool MoveFile(string src, string dst);
 


        }
    }
}
