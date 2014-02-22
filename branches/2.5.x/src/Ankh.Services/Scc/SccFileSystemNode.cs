// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using SharpSvn;
using System.IO;

namespace Ankh.Scc
{
    /// <summary>
    /// Long path capable <see cref="FileSystemInfo"/> variant
    /// </summary>
    public sealed class SccFileSystemNode
    {
        readonly string _basePath;
        readonly string _fileName;
        readonly FileAttributes _attributes;

        SccFileSystemNode(string basePath, string fileName, FileAttributes attributes)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException("basePath");
            else if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            _basePath = basePath;
            _fileName = fileName;
            _attributes = attributes;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public FileAttributes Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _fileName; }
        }

        string _fullPath;
        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _fullPath ?? (_fullPath = _basePath + Name); }
        }

        /// <summary>
        /// Gets a value indicating whether this node represents a directory.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this node is a directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory
        {
            get { return (Attributes & FileAttributes.Directory) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this node represents a file.
        /// </summary>
        /// <value><c>true</c> if this node is a file; otherwise, <c>false</c>.</value>
        public bool IsFile
        {
            get { return (Attributes & FileAttributes.Directory) == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this node is hidden or system.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this node is hidden or system; otherwise, <c>false</c>.
        /// </value>
        public bool IsHiddenOrSystem
        {
            get { return (Attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0; }
        }

        /// <summary>
        /// Gets the directory nodes below the specified path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IEnumerable<SccFileSystemNode> GetDirectoryNodes(string path)
        {
            bool canRead;
            return GetDirectoryNodes(path, out canRead);
        }

        /// <summary>
        /// Gets the directory nodes below the specified path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="canRead">if set to <c>true</c> [can read].</param>
        /// <returns></returns>
        public static IEnumerable<SccFileSystemNode> GetDirectoryNodes(string path, out bool canRead)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            string fullPath = path;

            if (!path.EndsWith("\\"))
                path += "\\";
            
            fullPath = path + "*";

            if (fullPath.Length > 240)
            {
                fullPath = SvnItem.MakeLongPath(fullPath);
            }

            NativeMethods.WIN32_FIND_DATA data;
            SafeFindHandle sh = NativeMethods.FindFirstFileW(fullPath, out data);

            if (sh.IsInvalid)
            {
                canRead = false;
                return new SccFileSystemNode[0];
            }
            else
            {
                canRead = true;
                return DoGetDirectoryNodes(new SccFileSystemNode(path, data.cFileName, (FileAttributes)data.dwFileAttributes), sh);
            }
        }

        static IEnumerable<SccFileSystemNode> DoGetDirectoryNodes(SccFileSystemNode result, SafeFindHandle findHandle)
        {
            string basePath = result._basePath;
            using (findHandle)
            {
                if (!IsDotPath(result.Name))
                    yield return result;

                NativeMethods.WIN32_FIND_DATA data;
                while (NativeMethods.FindNextFileW(findHandle, out data))
                {
                    if (IsDotPath(data.cFileName))
                        continue;

                    yield return new SccFileSystemNode(basePath, data.cFileName, (FileAttributes)data.dwFileAttributes);
                }
            }
        }

        private static bool IsDotPath(string name)
        {
            if (name.Length == 1 && name[0] == '.')
                return true;
            else if (name.Length == 2 && name == "..")
                return true;

            return false;
        }

        internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            // Methods
            internal SafeFindHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return NativeMethods.FindClose(base.handle);
            }
        }



        static class NativeMethods
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public extern static bool FindClose(IntPtr handle);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            public static extern SafeFindHandle FindFirstFileW(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FindNextFileW(SafeFindHandle hFindFile, out WIN32_FIND_DATA lpFindFileData);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct WIN32_FIND_DATA
            {
                public uint dwFileAttributes;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                public uint dwReserved0;
                public uint dwReserved1;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;
            }
        }
    }
}
