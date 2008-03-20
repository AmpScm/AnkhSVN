﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// Container of a coupling between a single project(1) and a file(ReferenceCount)
    /// </summary>
    class SccProjectFileReference
    {
        readonly IAnkhServiceProvider _context;
        readonly SccProjectFile _file;
        readonly SccProjectData _project;
        int _refCount;
        uint[] _ids;

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

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename
        {
            get { return ProjectFile.Filename; }
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

        internal void OnFileOpen(uint itemId)
        {
            if (_ids == null)
                _ids = new uint[] { itemId };
            else
            {
                // More than one item should never happen; but accept anyway
                uint[] ids = new uint[_ids.Length + 1];

                ids[1] = itemId;
                _ids.CopyTo(ids, 1);
                _ids = ids;
            }
        }

        internal void OnFileClose(uint itemId)
        {
            if (_ids == null)
                return;
            else if (_ids.Length == 1 && _ids[0] == itemId)
                _ids = null; // 99% case
            else
            {
                int n = Array.IndexOf(_ids, itemId);

                if (n >= 0)
                {
                    uint[] ids = new uint[_ids.Length - 1];
                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (i < n)
                            ids[i] = _ids[i];
                        else
                            ids[i] = _ids[i + 1];
                    }
                    _ids = ids;
                }
            }
        }
    }
}
