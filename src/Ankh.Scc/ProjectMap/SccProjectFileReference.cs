using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

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

        public bool TryGetId(out uint id)
        {
            IVsProject project = _project.Project as IVsProject;

            if (project == null)
            {
                id = 0;
                return false;
            }

            int found;
            VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
            if (ErrorHandler.Succeeded(project.IsDocumentInProject(Filename, out found, prio, out id)) && id != VSConstants.VSITEMID_NIL)
                return true;

            id = 0;
            return false;
        }

        internal bool TryGetIcon(out ProjectIconReference icon)
        {
            uint id;
            icon = null;

            if (!TryGetId(out id))
                return false;

            IVsHierarchy hier = _project.Project as IVsHierarchy;

            if (hier == null)
                return false;

            object value;
            if (ErrorHandler.Succeeded(hier.GetProperty(id, (int)__VSHPROPID.VSHPROPID_IconHandle, out value)))
            {
                icon = new ProjectIconReference((IntPtr)(int)value);
                return true;
            }

            IntPtr list;

            if(!ErrorHandler.Succeeded(hier.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconImgList, out value)))
                return false;

            list = (IntPtr)(int)value;


            if (!ErrorHandler.Succeeded(hier.GetProperty(id, (int)__VSHPROPID.VSHPROPID_IconIndex, out value)))
                return false;

            icon = new ProjectIconReference(list, (int)value);

            return true;
        }
    }
}
