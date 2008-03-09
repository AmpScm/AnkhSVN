using System;
using System.IO;

namespace Utils
{
    /// <summary>
    /// Contains utility functions for path manipulations.
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// Normalizes the path using the current directory as base directory
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            return NormalizePath(path, Environment.CurrentDirectory);
        }

        /// <summary>
        /// Normalizes the path to standard form
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="basepath">The basepath.</param>
        /// <returns></returns>
        public static string NormalizePath(string path, string basepath)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            else if (string.IsNullOrEmpty(basepath))
                throw new ArgumentNullException("basepath");

            string normPath = path.Replace('/', '\\');

            if (!IsPathAbsolute(normPath))
            {
                normPath = Path.GetFullPath(Path.Combine(basepath, path));
            }

            return normPath;
        }


        /// <summary>
        /// Determines whether path is in the tree under directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool IsSubPathOf(string path, string directory)
        {
            if (!IsPathAbsolute(path))
                path = Path.GetFullPath(path);

            if (!IsPathAbsolute(directory))
                directory = Path.GetFullPath(directory);

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directory += Path.DirectorySeparatorChar;
            }

            path = path.ToUpperInvariant();
            directory = directory.ToUpperInvariant();

            return path.StartsWith(directory, StringComparison.OrdinalIgnoreCase);
        }

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

        /// <summary>
        /// Retrieves the parent directory of a path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The parent directory.</returns>
        public static string GetParent(string path)
        {
            path = PathUtils.StripTrailingSlash(path);
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Get the name of the item(file or folder).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetName(string path)
        {
            path = PathUtils.StripTrailingSlash(path);
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Strips the trailing slash off a path, if present.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StripTrailingSlash(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            char c = path[path.Length-1];
            if (c == '/' || c == '\\')
                path = path.Substring(0, path.Length - 1);

            return path;
        }


        public static bool AreEqual(string path1, string path2)
        {
            return string.Equals(NormalizePath(path1), NormalizePath(path2), StringComparison.OrdinalIgnoreCase);
        }
    }
}
