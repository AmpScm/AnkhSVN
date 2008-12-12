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
                if (ErrorHandler.Succeeded(ProjectHierarchy.AdviseHierarchyEvents(this, out cookie)))
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
            return VSConstants.S_OK;
        }

        public int OnInvalidateItems(uint itemidParent)
        {
            // Should be set the project dirty.. 
            // But is called in some cases when it really shouldn't
            return VSConstants.S_OK;
        }

        private void SetDirty()
        {
            if (!_loaded)
                return; // The project is just loading.. don't set it dirty
            else if (string.IsNullOrEmpty(ProjectFile))
                return; // No file to set dirty

            IAnkhOpenDocumentTracker dt = _context.GetService<IAnkhOpenDocumentTracker>();

            dt.SetDirty(ProjectFile, true);
        }

        void SetPreCreatedItem(uint itemid)
        {
            if (!_loaded)
                return;

            ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

            if (walker == null)
                return;

            if (itemid != VSConstants.VSITEMID_NIL)
                walker.SetPrecreatedFilterItem(ProjectHierarchy, itemid);
            else
                walker.SetPrecreatedFilterItem(null, VSConstants.VSITEMID_NIL);

        }

        public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            string r;

            object var;
            if (ErrorHandler.Succeeded(ProjectHierarchy.GetProperty(itemidAdded, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out var))
                && (bool)var)
            {
                return VSConstants.S_OK; // Extra item for show all files
            }

            if (_loaded)
            {
                if (ErrorHandler.Succeeded(VsProject.GetMkDocument(itemidAdded, out r))
                    && !string.IsNullOrEmpty(r) && SvnItem.IsValidPath(r))
                {
                    if (!System.IO.File.Exists(r) && !System.IO.Directory.Exists(r))
                    {
                        SetPreCreatedItem(itemidAdded);
                    }
                    else
                    {
                        SetPreCreatedItem(VSConstants.VSITEMID_NIL);

                        SetDirty();
                    }
                }
            }

            return VSConstants.S_OK;
        }

        public int OnItemDeleted(uint itemid)
        {
            SetPreCreatedItem(VSConstants.VSITEMID_NIL);

            object var;
            if (ErrorHandler.Succeeded(ProjectHierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out var))
                && (bool)var)
            {
                return VSConstants.S_OK; // Extra item for show all files
            }

            SetDirty();

            return VSConstants.S_OK;
        }

        public int OnItemsAppended(uint itemidParent)
        {
            SetDirty();
            return VSConstants.S_OK;
        }

        public int OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            return VSConstants.S_OK;
        }
    }
}
