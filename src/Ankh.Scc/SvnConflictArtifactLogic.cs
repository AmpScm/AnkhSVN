// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;

namespace Ankh.Scc
{
    /// <summary>
    /// Identifies temporary files created by Subversion for text conflicts.
    /// Filename shape alone is not sufficient: callers must also verify the
    /// candidate against Subversion's conflict metadata before suppressing it.
    /// </summary>
    internal static class SvnConflictArtifactLogic
    {
        public static bool TryGetConflictedPath(
            string artifactPath,
            out string conflictedPath)
        {
            conflictedPath = null;

            if (string.IsNullOrEmpty(artifactPath))
                return false;

            string fileName = Path.GetFileName(artifactPath);
            if (string.IsNullOrEmpty(fileName))
                return false;

            int suffixIndex = -1;

            if (fileName.EndsWith(".mine", StringComparison.OrdinalIgnoreCase))
            {
                suffixIndex = fileName.Length - ".mine".Length;
            }
            else
            {
                int revisionIndex =
                    fileName.LastIndexOf(".r", StringComparison.OrdinalIgnoreCase);

                if (revisionIndex > 0
                    && revisionIndex + 2 < fileName.Length
                    && IsDecimalRevision(
                        fileName,
                        revisionIndex + 2))
                {
                    suffixIndex = revisionIndex;
                }
            }

            if (suffixIndex <= 0)
                return false;

            string conflictedName = fileName.Substring(0, suffixIndex);
            string directory = Path.GetDirectoryName(artifactPath);

            conflictedPath = string.IsNullOrEmpty(directory)
                ? conflictedName
                : Path.Combine(directory, conflictedName);

            return true;
        }

        public static bool MatchesConflictMetadata(
            string artifactPath,
            string conflictedPath,
            string conflictOld,
            string conflictNew,
            string conflictWork)
        {
            if (string.IsNullOrEmpty(artifactPath)
                || string.IsNullOrEmpty(conflictedPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(conflictedPath);

            return MatchesConflictFile(
                    artifactPath,
                    directory,
                    conflictOld)
                || MatchesConflictFile(
                    artifactPath,
                    directory,
                    conflictNew)
                || MatchesConflictFile(
                    artifactPath,
                    directory,
                    conflictWork);
        }

        static bool IsDecimalRevision(string value, int startIndex)
        {
            if (startIndex >= value.Length)
                return false;

            for (int i = startIndex; i < value.Length; i++)
            {
                if (value[i] < '0' || value[i] > '9')
                    return false;
            }

            return true;
        }

        static bool MatchesConflictFile(
            string artifactPath,
            string conflictedDirectory,
            string conflictFile)
        {
            if (string.IsNullOrEmpty(conflictFile))
                return false;

            string conflictPath = Path.IsPathRooted(conflictFile)
                ? conflictFile
                : Path.Combine(conflictedDirectory ?? string.Empty, conflictFile);

            return string.Equals(
                NormalizePath(artifactPath),
                NormalizePath(conflictPath),
                StringComparison.OrdinalIgnoreCase);
        }

        static string NormalizePath(string path)
        {
            try
            {
                return Path.GetFullPath(path)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch (Exception)
            {
                return path;
            }
        }
    }
}
