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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// Container of a coupling between a single project(1) and a file(ReferenceCount)
    /// </summary>
    [DebuggerDisplay("{Filename}, Project={Project}")]
    sealed class SccProjectFileReference
    {
        readonly IAnkhServiceProvider _context;
        readonly SccProjectFile _file;
        readonly SccProjectData _project;
        int _refCount;
        uint _id;
        bool _isProjectFile;
        IList<string> _subFiles;

        internal SccProjectFileReference _nextReference; // Linked list managed by SccProjectFile

        public SccProjectFileReference(IAnkhServiceProvider context, SccProjectData project, SccProjectFile file)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (project == null)
                throw new ArgumentNullException("project");

            _context = context;
            _project = project;
            _file = file;
            _refCount = 1; // One should only create a reference if one needs one
            file.AddReference(this);
        }

        public SccProjectFile ProjectFile
        {
            get { return _file; }
        }

        public SccProjectFileReference NextReference
        {
            get { return _nextReference; }
        }

        public bool IsProjectFile
        {
            get { return _isProjectFile; }
            internal set { _isProjectFile = value; }
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename
        {
            get { return ProjectFile.FullPath; }
        }

        public SccProjectData Project
        {
            get { return _project; }
        }

        public int ReferenceCount
        {
            get { return _refCount; }
        }

        public void AddReference()
        {
            if (_refCount <= 0)
                throw new InvalidOperationException();

            _refCount++;
        }

        public void ReleaseReference()
        {
            _refCount--;

            if (_refCount <= 0)
            {
                _file.RemoveReference(this);
                _project.InvokeRemoveReference(this);
            }
        }

        public void ClearReferences()
        {
            _refCount = 0;
            _project.InvokeRemoveReference(this);
        }

        /// <summary>
        /// Gets the item id of the file within the project or <see cref="VSConstants.VSITEMID_NIL"/> if no id is assigned
        /// </summary>
        internal uint ProjectItemId
        {
            get
            {
                if (_id != 0)
                    return _id;

                uint id;
                if (_project.TryGetProjectFileId(Filename, out id))
                    return _id = id;
                else
                    return _id = VSConstants.VSITEMID_NIL;
            }
        }

        internal bool TryGetIcon(out ProjectIconReference icon)
        {
            uint id = ProjectItemId;
            icon = null;

            if (id == 0 || id == VSConstants.VSITEMID_NIL)
                return false;

            try
            {

                IntPtr imageList = _project.ProjectImageList;
                object value;

                if (imageList != IntPtr.Zero)
                {
                    if (ErrorHandler.Succeeded(
                        Project.ProjectHierarchy.GetProperty(id, (int)__VSHPROPID.VSHPROPID_IconIndex, out value)))
                    {
                        icon = new ProjectIconReference(imageList, (int)value);
                        return true;
                    }
                }

                // Only do this if we know there is no imagelist behind the icons
                // (This will create a cached icon handle if called on a managed project, which we only need once)
                if (ErrorHandler.Succeeded(
                    Project.ProjectHierarchy.GetProperty(id, (int)__VSHPROPID.VSHPROPID_IconHandle, out value)))
                {
                    icon = new ProjectIconReference((IntPtr)(int)value); // Marshalled by VS as 32 bit integer
                    return true;
                }
            }
            catch
            { /* Eat all project exceptions */ }

            return false;
        }

        internal IList<string> GetSubFiles()
        {
            if (_subFiles != null)
                return _subFiles;

            ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

            List<string> files = new List<string>(walker.GetSccFiles(Project.ProjectHierarchy, ProjectItemId, ProjectWalkDepth.SpecialFiles, null));
            files.Remove(Filename);

            _subFiles = files.AsReadOnly();

            return _subFiles;
        }

        public bool OpenAsDocument()
        {
            Guid editorType = Guid.Empty;

            IntPtr DOCDATAEXISTING_UNKNOWN = (IntPtr)(int)-1;
            IVsWindowFrame frame;
            return ErrorHandler.Succeeded(
                Project.VsProject.OpenItem(ProjectItemId, ref editorType, DOCDATAEXISTING_UNKNOWN, out frame));
        }

        internal void ClearIdCache()
        {
            _id = 0;
            _subFiles = null;
        }

        internal void SetId(uint id)
        {
            _id = id;
        }
    }
}
