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
        readonly NativeMethods.WIN32_FIND_DATA _findData;
        string _basePath;
        string _fullPath;
        SccFileSystemNode(string basePath, NativeMethods.WIN32_FIND_DATA findData)
        {
            _basePath = basePath;
            _findData = findData;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public FileAttributes Attributes
        {
            get { return (FileAttributes)_findData.dwFileAttributes; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _findData.cFileName; }
        }

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
                if (fullPath.StartsWith("\\\\"))
                    fullPath = "\\\\?\\UNC" + fullPath.Substring(1);
                else
                    fullPath = "\\\\?\\" + fullPath;
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
                return DoGetDirectoryNodes(new SccFileSystemNode(path, data), sh);
            }
        }

        static IEnumerable<SccFileSystemNode> DoGetDirectoryNodes(SccFileSystemNode result, SafeFindHandle findHandle)
        {
            string basePath = result._basePath;
            using (findHandle)
            {
                if (!IsDotPath(result))
                    yield return result;

                NativeMethods.WIN32_FIND_DATA data;
                while (NativeMethods.FindNextFileW(findHandle, out data))
                {
                    result = new SccFileSystemNode(basePath, data);
                    if (!IsDotPath(result))
                        yield return result;
                }
            }
        }

        private static bool IsDotPath(SccFileSystemNode result)
        {
            string p = result.Name;

            if (p.Length == 1 && p[0] == '.')
                return true;
            else if (p.Length == 2 && p == "..")
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
