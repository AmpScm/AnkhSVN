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
            if(_hierarchyEventsCookie == 0 && hook)
            {
                if(ErrorHandler.Succeeded(ProjectHierarchy.AdviseHierarchyEvents(this, out cookie)))
                {
                    _hierarchyEventsCookie = cookie;
                }
            }
            else if(!hook)
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

        public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            SetDirty();
            return VSConstants.S_OK;
        }

        public int OnItemDeleted(uint itemid)
        {
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
