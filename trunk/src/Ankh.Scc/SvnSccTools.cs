﻿using System;
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
    class SvnSccContext : IDisposable
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnClient _client;
        readonly IFileStatusCache _statusCache;

        public SvnSccContext(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _client = context.GetService<ISvnClientPool>().GetClient();
            _statusCache = context.GetService<IFileStatusCache>();
        }

        public void Dispose()
        {
            _client.Dispose();
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
            while (path.Length > root.Length)
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

        public IDisposable MarkIgnoreFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            IVsFileChangeEx change = (IVsFileChangeEx)_context.GetService(typeof(SVsFileChangeEx));

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

            if (!File.Exists(path))
                throw new InvalidOperationException();

            FileAttributes attrs = File.GetAttributes(path);
            File.SetAttributes(path, FileAttributes.Normal);
            string tmp = path + ".AnkhSVN.tmp";

            File.Move(path, tmp);
            File.SetAttributes(tmp, FileAttributes.ReadOnly);

            return new DelegateRunner(
                delegate()
                {
                    if (File.Exists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        File.Delete(path);
                    }
                    File.SetAttributes(tmp, FileAttributes.Normal);

                    File.Move(tmp, path);

                    if (touch)
                        File.SetLastWriteTime(path, DateTime.Now);

                    File.SetAttributes(path, attrs);
                });
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
    }
}
