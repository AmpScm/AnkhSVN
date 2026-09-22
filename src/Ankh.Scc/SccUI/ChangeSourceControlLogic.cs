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
using System.Collections.Generic;
using System.IO;

namespace Ankh.Scc.SccUI
{
    internal sealed class ChangeSourceControlProjectBinding
    {
        public ChangeSourceControlProjectBinding(
            bool isUsable,
            string projectBase,
            string projectDirectory,
            string projectLocation,
            Uri projectUri)
        {
            IsUsable = isUsable;
            ProjectBase = projectBase;
            ProjectDirectory = projectDirectory;
            ProjectLocation = projectLocation;
            ProjectUri = projectUri;
        }

        public bool IsUsable { get; private set; }
        public string ProjectBase { get; private set; }
        public string ProjectDirectory { get; private set; }
        public string ProjectLocation { get; private set; }
        public Uri ProjectUri { get; private set; }
    }

    internal sealed class ChangeSourceControlSelectionState
    {
        public ChangeSourceControlSelectionState(
            string projectBase,
            string relativePath,
            string projectLocation,
            Uri projectUri)
        {
            ProjectBase = projectBase;
            RelativePath = relativePath;
            ProjectLocation = projectLocation;
            ProjectUri = projectUri;
        }

        public string ProjectBase { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectLocation { get; private set; }
        public Uri ProjectUri { get; private set; }

        public string ProjectBaseText
        {
            get { return ProjectBase ?? ""; }
        }

        public string RelativePathText
        {
            get { return string.IsNullOrEmpty(RelativePath) ? "." : RelativePath; }
        }

        public string ProjectLocationText
        {
            get { return ProjectLocation ?? ""; }
        }

        public string ProjectUrlText
        {
            get { return ProjectUri != null ? ProjectUri.ToString() : ""; }
        }
    }

    internal static class ChangeSourceControlLogic
    {
        public static ChangeSourceControlSelectionState BuildSelection(
            IEnumerable<ChangeSourceControlProjectBinding> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            string projectBase = null;
            string relativePath = null;
            string projectLocation = null;
            bool first = true;
            Uri projectUri = null;

            foreach (ChangeSourceControlProjectBinding project in projects)
            {
                if (project == null)
                    throw new ArgumentException(
                        "Project bindings cannot contain null entries.",
                        "projects");

                if (!project.IsUsable)
                {
                    projectBase = null;
                    break;
                }

                string relPath = NormalizeRelativePath(
                    project.ProjectDirectory,
                    project.ProjectBase);

                KeepOneIgnoreCase(
                    ref projectBase,
                    project.ProjectBase,
                    first);
                KeepOneIgnoreCase(
                    ref relativePath,
                    relPath,
                    first);
                KeepOneIgnoreCase(
                    ref projectLocation,
                    project.ProjectLocation,
                    first);
                KeepOne(
                    ref projectUri,
                    project.ProjectUri,
                    first);

                first = false;
            }

            if (projectBase == null)
            {
                relativePath = null;
                projectLocation = null;
            }

            return new ChangeSourceControlSelectionState(
                projectBase,
                relativePath,
                projectLocation,
                projectUri);
        }

        public static string NormalizeRelativePath(
            string projectDirectory,
            string projectBase)
        {
            string relativePath = projectDirectory;

            if (relativePath.StartsWith(projectBase))
            {
                int projectLength = projectDirectory.Length;
                int baseLength = projectBase.Length;

                if (projectLength == baseLength)
                    return ".";

                if (baseLength < projectLength
                    && projectDirectory[baseLength] == Path.DirectorySeparatorChar)
                {
                    return projectDirectory.Substring(baseLength + 1);
                }
            }

            return relativePath;
        }

        static void KeepOneIgnoreCase(
            ref string result,
            string newValue,
            bool first)
        {
            if (first)
                result = newValue;
            else if (result == null
                || string.Equals(
                    result,
                    newValue,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            else
                result = null;
        }

        static void KeepOne<T>(
            ref T result,
            T value,
            bool first)
            where T : class
        {
            if (first)
                result = value;
            else if (result == null || result.Equals(value))
                return;
            else
                result = null;
        }
    }
}
