// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using Ankh.Selection;
using Ankh.VS;
using System.IO;
using Microsoft.VisualStudio.Shell;
using SharpSvn;

namespace Ankh.Scc.SccUI
{
    sealed class ChangeSourceControlRow : DataGridViewRow
    {
        readonly IAnkhServiceProvider _context;
        readonly SccProject _project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSourceControlRow"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="project">The project.</param>
        public ChangeSourceControlRow(IAnkhServiceProvider context, SccProject project)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (project == null)
                throw new ArgumentNullException("project");

            _context = context;
            _project = project;
        }

        static Stack<ChangeSourceControlRow> _cloning;
        /// <summary>
        /// [Implementation detail of Clone()]
        /// </summary>
        public ChangeSourceControlRow()
        {
            ChangeSourceControlRow from = _cloning.Pop();
            _context = from._context;
            _project = from._project;
        }

        /// <summary>
        /// Creates an exact copy of this row.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the cloned <see cref="T:System.Windows.Forms.DataGridViewRow"/>.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override object Clone()
        {
            if (_cloning == null)
                _cloning = new Stack<ChangeSourceControlRow>();

            _cloning.Push(this);

            return base.Clone();
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>The project.</value>
        public SccProject Project
        {
            get { return _project; }
        }

        IAnkhSolutionSettings _solutionSettings;
        IAnkhSolutionSettings SolutionSettings
        {
            get { return _solutionSettings ?? (_solutionSettings = _context.GetService<IAnkhSolutionSettings>()); }
        }

        IProjectFileMapper _projectMap;
        IProjectFileMapper ProjectMap
        {
            get { return _projectMap ?? (_projectMap = _context.GetService<IProjectFileMapper>()); }
        }

        IAnkhSccService _scc;
        IAnkhSccService Scc
        {
            get { return _scc ?? (_scc = _context.GetService<IAnkhSccService>()); }
        }

        ISvnStatusCache _cache;
        ISvnStatusCache Cache
        {
            get { return _cache ?? (_cache = _context.GetService<ISvnStatusCache>()); }
        }

        string SafeToString(object value)
        {
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            ISccProjectInfo projectInfo;
            if (_project.IsSolution)
            {
                SvnItem rootItem = SolutionSettings.ProjectRootSvnItem;

                SetValues(
                    Scc.IsSolutionManaged,
                    "Solution: " + Path.GetFileNameWithoutExtension(SolutionSettings.SolutionFilename),
                    SafeRepositoryRoot(rootItem),
                    SafeRepositoryPath(rootItem),
                    GetStatus(rootItem, null, SolutionSettings.SolutionFilename),
                    (rootItem != null)
                        ? EmptyToDot(SvnItem.MakeRelative(rootItem.FullPath, SvnTools.GetNormalizedDirectoryName(SolutionSettings.SolutionFilename)))
                        : "",
                    (rootItem != null)
                        ? rootItem.FullPath
                        : ""
                    );
            }
            else if (null != (projectInfo = ProjectMap.GetProjectInfo(_project)) && null != (projectInfo.ProjectDirectory))
            {
                SvnItem dirItem = Cache[projectInfo.SccBaseDirectory];

                SetValues(
                    Scc.IsProjectManaged(_project),
                    projectInfo.UniqueProjectName,
                    SafeRepositoryRoot(dirItem),
                    SafeRepositoryPath(dirItem),
                    GetStatus(dirItem, projectInfo, projectInfo.ProjectFile),
                    EmptyToDot(SvnItem.MakeRelative(projectInfo.SccBaseDirectory, projectInfo.ProjectDirectory)),
                    projectInfo.SccBaseDirectory
                    );
            }
            else
            {
                // Should have been filtered before; probably a buggy project that changed while the dialog was open
                SetValues(
                    false,
                    "-?-",
                    "-?-",
                    "-?-",
                    "-?-",
                    "-?-",
                    "-?-"
                    );
            }
        }

        private string GetStatus(SvnItem dirItem, ISccProjectInfo projectInfo, string file)
        {
            if (dirItem == null || !dirItem.Exists || !dirItem.IsVersioned)
                return "<not found>";

            if (projectInfo == null)
            {
                if (Scc.IsSolutionManaged)
                    return "Connected"; // Solution itself + Connected
                else
                    return "Not Connected";
            }

            if (dirItem.IsBelowPath(SolutionSettings.ProjectRootSvnItem)
                    && dirItem.WorkingCopy == SolutionSettings.ProjectRootSvnItem.WorkingCopy)
            {
                // In master working copy
                if (Scc.IsSolutionManaged && Scc.IsProjectManaged(_project))
                    return "Connected";
                else
                    return "Valid"; // In master working copy
            }
            else if (Scc.IsSolutionManaged && Scc.IsProjectManaged(_project))
                return "Connected"; // Required information in solution
            else
                return "Detached"; // Separate working copy
        }

        string SafeRepositoryPath(SvnItem item)
        {
            if (item == null || item.Uri == null)
                return "";

            SvnWorkingCopy wc = item.WorkingCopy;
            if (wc != null)
            {
                Uri root = wc.RepositoryRoot;

                if (root != null)
                {
                    Uri relative = root.MakeRelativeUri(item.Uri);

                    if (!relative.IsAbsoluteUri)
                    {
                        string v = SvnTools.UriPartToPath(relative.ToString()).Replace(Path.DirectorySeparatorChar, '/');

                        if (!string.IsNullOrEmpty(v) && !v.StartsWith("/") && !v.StartsWith("../") && v != ".")
                            return "^/" + v;
                    }
                }
            }

            return item.Uri.ToString();
        }

        string SafeRepositoryRoot(SvnItem item)
        {
            if (item == null || item.WorkingCopy == null)
                return "";

            Uri root = item.WorkingCopy.RepositoryRoot;

            if (root != null)
                return root.ToString();

            return "";
        }

        private string EmptyToDot(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ".";
            else
                return value;
        }
    }
}
