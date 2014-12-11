// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

namespace Ankh.Scc.ProjectMap
{
    partial class SccProjectData : IVsHierarchyEvents
    {
        uint _hierarchyEventsCookie;
        internal void Hook(bool hook)
        {
            uint cookie;
            if (_hierarchyEventsCookie == 0 && hook)
            {
                if (VSErr.Succeeded(ProjectHierarchy.AdviseHierarchyEvents(this, out cookie)))
                {
                    _hierarchyEventsCookie = cookie;
                }
            }
            else if (!hook && (_hierarchyEventsCookie != 0))
            {
                cookie = _hierarchyEventsCookie;
                _hierarchyEventsCookie = 0;

                ProjectHierarchy.UnadviseHierarchyEvents(cookie);
            }
        }

        public int OnInvalidateIcon(IntPtr hicon)
        {
            return VSErr.S_OK;
        }

        [CLSCompliant(false)]
        public int OnInvalidateItems(uint itemidParent)
        {
            // Should be set the project dirty.. 
            // But is called in some cases when it really shouldn't
            return VSErr.S_OK;
        }

        private void SetDirty()
        {
            if (!_loaded)
                return; // The project is just loading.. don't set it dirty
            else if (string.IsNullOrEmpty(ProjectFile))
                return; // No file to set dirty

            IAnkhOpenDocumentTracker dt = _context.GetService<IAnkhOpenDocumentTracker>();

            dt.CheckDirty(ProjectFile);
        }

        void SetPreCreatedItem(uint itemid)
        {
            if (!_loaded)
                return;

            ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

            if (walker == null)
                return;

            if (itemid != VSItemId.Nil)
                walker.SetPrecreatedFilterItem(ProjectHierarchy, itemid);
            else
                walker.SetPrecreatedFilterItem(null, VSItemId.Nil);

        }

        [CLSCompliant(false)]
        public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            string r;

            object var;
            if (VSErr.Succeeded(ProjectHierarchy.GetProperty(itemidAdded, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out var))
                && (bool)var)
            {
                return VSErr.S_OK; // Extra item for show all files
            }

            if (_loaded)
            {
                if (VSErr.Succeeded(VsProject.GetMkDocument(itemidAdded, out r))
                    && SvnItem.IsValidPath(r))
                {
                    // Check out VSHPROPID_IsNewUnsavedItem
                    if (!SvnItem.PathExists(r))
                    {
                        SetPreCreatedItem(itemidAdded);
                    }
                    else
                    {
                        SetPreCreatedItem(VSItemId.Nil);

                        SetDirty();
                    }
                }
            }

            return VSErr.S_OK;
        }

        [CLSCompliant(false)]
        public int OnItemDeleted(uint itemid)
        {
            SetPreCreatedItem(VSItemId.Nil);

            object var;
            if (VSErr.Succeeded(ProjectHierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out var))
                && (bool)var)
            {
                return VSErr.S_OK; // Extra item for show all files
            }

            SetDirty();

            return VSErr.S_OK;
        }

        [CLSCompliant(false)]
        public int OnItemsAppended(uint itemidParent)
        {
            SetDirty();
            return VSErr.S_OK;
        }

        [CLSCompliant(false)]
        public int OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            if (itemid == VSItemId.Root)
            {
                _uniqueName = null;
                _projectDirectory = null;
                _projectName = null;
                _projectFile = null;
                _projectLocation = null;
                _sccBaseDirectory = null;
                _checkedProjectFile = false;
            }
            return VSErr.S_OK;
        }
    }
}
