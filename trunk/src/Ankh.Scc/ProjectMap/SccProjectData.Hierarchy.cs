﻿using System;
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
                        SetPreCreatedItem(VSConstants.VSITEMID_NIL);
                }
            }

            return VSConstants.S_OK;
        }

        public int OnItemDeleted(uint itemid)
        {
            SetPreCreatedItem(VSConstants.VSITEMID_NIL);

            return VSConstants.S_OK;
        }

        public int OnItemsAppended(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        public int OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            return VSConstants.S_OK;
        }
    }
}
