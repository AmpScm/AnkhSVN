using System;
using System.IO;
using System.Diagnostics;
using SharpSvn;

namespace Utils
{
    /// <summary>
    /// Contains utility functions for path manipulations.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// Determines whether the specified path is absolute
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is path absolute] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPathAbsolute(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!Path.IsPathRooted(path))
                return false;
            else if ((path[0] == '\\' || path[0] == '/') && path.Length > 2 && !(path[0] == '\\' || path[0] == '/'))
                return false;
            else if (path.Contains("\\."))
                return false;

            return true;
        }


        /// <summary>
        /// Recursively deletes a directory.
        /// </summary>
        /// <param name="path"></param>
        public static void RecursiveDelete(string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                RecursiveDelete(dir);
            }

            foreach (string file in Directory.GetFiles(path))
                File.SetAttributes(file, FileAttributes.Normal);

            File.SetAttributes(path, FileAttributes.Normal);
            Directory.Delete(path, true);
        }

        public static bool AreEqual(string path1, string path2)
        {
            return string.Equals(
                SvnTools.GetNormalizedFullPath(path1), 
                SvnTools.GetNormalizedFullPath(path2), StringComparison.OrdinalIgnoreCase);
        }
    }
}
