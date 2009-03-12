using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using SharpSvn;

namespace Ankh.Scc
{
    sealed class SccFilesystemNode
    {
        readonly NativeMethods.WIN32_FIND_DATA _findData;
        SccFilesystemNode(NativeMethods.WIN32_FIND_DATA findData)
        {
            if (findData == null)
                throw new ArgumentNullException("findData");

            _findData = findData;
        }

        /// <summary>
        /// Gets the directory nodes below the specified path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static IEnumerable<SccFilesystemNode> GetDirectoryNodes(string path)
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
        public static IEnumerable<SccFilesystemNode> GetDirectoryNodes(string path, out bool canRead)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            string fullPath = path;

            if (!fullPath.EndsWith("\\"))
                fullPath += "\\*";
            else
                fullPath += "*";

            if (fullPath.Length > 240)
            {
                if (fullPath.StartsWith("\\\\"))
                    fullPath = "\\\\?\\UNC" + fullPath.Substring(1);
                else
                    fullPath = "\\\\?\\" + fullPath;
            }

            NativeMethods.WIN32_FIND_DATA firstResult;
            SafeFindHandle sh = NativeMethods.FindFirstFileW(fullPath, out firstResult);

            if (sh.IsInvalid)
            {
                canRead = false;
                return new SccFilesystemNode[0];
            }
            else
            {
                canRead = true;
                return DoGetDirectoryNodes(firstResult, sh);
            }
        }

        static IEnumerable<SccFilesystemNode> DoGetDirectoryNodes(NativeMethods.WIN32_FIND_DATA result, SafeFindHandle findHandle)
        {
            using (findHandle)
            {
                if (!IsDotPath(result))
                    yield return new SccFilesystemNode(result);

                while (NativeMethods.FindNextFileW(findHandle, out result))
                {
                    if (!IsDotPath(result))
                        yield return new SccFilesystemNode(result);
                }
            }
        }

        private static bool IsDotPath(NativeMethods.WIN32_FIND_DATA result)
        {
            string p = result.cFileName;

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
            public class WIN32_FIND_DATA
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
