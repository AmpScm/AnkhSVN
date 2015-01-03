using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace Ankh.Scc.Engine
{
    partial class SccItem
    {
        /// <summary>
        /// Long path safe version of File.Exists(path) || Directory.Exists(path)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PathExists(string path)
        {
            return NativeMethods.GetFileAttributes(path) != NativeMethods.INVALID_FILE_ATTRIBUTES;
        }

        /// <summary>
        /// Long path safe version of File.Delete(path)
        /// </summary>
        /// <param name="path"></param>
        public static bool DeleteFile(string path)
        {
            return NativeMethods.DeleteFile(path);
        }

        /// <summary>
        /// Long path safe version of Directory.Delete(path)
        /// </summary>
        /// <param name="fullPath"></param>
        public static bool DeleteDirectory(string fullPath)
        {
            return NativeMethods.RemoveDirectory(fullPath);
        }

        public static bool DeleteDirectory(string fullPath, bool recursive)
        {
            if (recursive)
            {
                bool allOk = true;
                foreach (SccFileSystemNode n in SccFileSystemNode.GetDirectoryNodes(fullPath))
                {
                    if (n.IsDirectory)
                    {
                        if (!DeleteDirectory(n.FullPath, true))
                            allOk = false;
                    }
                    else if (!DeleteFile(n.FullPath))
                        allOk = false;
                }
                if (!allOk)
                    return false;
            }

            return DeleteDirectory(fullPath);
        }

        public static bool DeleteNode(string fullPath)
        {
            uint type = NativeMethods.GetFileAttributes(fullPath);

            if (type == NativeMethods.INVALID_FILE_ATTRIBUTES)
                return true;
            else if ((type & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
                return DeleteDirectory(fullPath, true);
            else
                return DeleteFile(fullPath);
        }

        internal static string MakeLongPath(string fullPath)
        {
            // Windows doesn't normalize long paths via this trick, so we should
            fullPath = SvnTools.GetNormalizedFullPath(fullPath);

            // Documented method of allowing paths over 160 characters (APR+SharpSvn use this too!)
            if (!fullPath.StartsWith("\\\\"))
                return "\\\\?\\" + fullPath;
            else
                return "\\\\?\\UNC" + fullPath.Substring(1);
        }

        internal static class NativeMethods
        {
            /// <summary>
            /// Gets the fileattributes of the specified file without going through the .Net normalization rules
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static uint GetFileAttributes(string filename)
            {
                // This method assumes filename is an absolute and/or rooted path
                if (string.IsNullOrEmpty(filename))
                    throw new ArgumentNullException("filename");

                if (filename.Length < 240)
                    return GetFileAttributesW(filename);
                else
                    return GetFileAttributesW(MakeLongPath(filename)); // Documented method of allowing paths over 160 characters (APR+SharpSvn use this too!)
            }

            public static bool DeleteFile(string filename)
            {
                if (string.IsNullOrEmpty(filename))
                    throw new ArgumentNullException("filename");

                if (filename.Length >= 240)
                    filename = MakeLongPath(filename);

                SetFileAttributesW(filename, FILE_ATTRIBUTE_NORMAL);
                return DeleteFileW(filename);
            }

            public static bool RemoveDirectory(string pathname)
            {
                // This method assumes filename is an absolute and/or rooted path
                if (string.IsNullOrEmpty(pathname))
                    throw new ArgumentNullException("pathname");

                if (pathname.Length >= 240)
                    pathname = MakeLongPath(pathname);

                return RemoveDirectoryW(MakeLongPath(pathname));
            }

            [DllImport("kernel32.dll", ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            extern static bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)]string lpFilename);

            [DllImport("kernel32.dll", ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            extern static bool RemoveDirectoryW([MarshalAs(UnmanagedType.LPWStr)]string lpFilename);

            [DllImport("kernel32.dll", ExactSpelling = true)]
            extern static uint GetFileAttributesW([MarshalAs(UnmanagedType.LPWStr)]string filename);

            [DllImport("kernel32.dll", ExactSpelling = true)]
            extern static bool SetFileAttributesW([MarshalAs(UnmanagedType.LPWStr)]string filename, [MarshalAs(UnmanagedType.U4)] uint fileAttributes);

            public const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFF;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
            public const uint FILE_ATTRIBUTE_READONLY = 0x1;
            const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }
    }

    partial class SccItem<T>
    {

    }
}
