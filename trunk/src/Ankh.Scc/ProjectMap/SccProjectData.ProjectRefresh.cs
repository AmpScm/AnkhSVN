using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc.ProjectMap
{
    partial class SccProjectData
    {
        internal bool RequiresForcedRefresh()
        {
            return IsWebSite;
        }

        sealed class RefreshState
        {
            readonly IFileStatusCache _cache;
            readonly IVsHierarchy _hier;
            readonly IVsProject2 _project;
            readonly ISccProjectWalker _walker;
            readonly string _projectDir;
            readonly SvnItem _projectDirItem;
            readonly Dictionary<string, uint> _map = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            bool _added;

            public RefreshState(IAnkhServiceProvider context, IVsHierarchy hier, IVsProject project, string projectDir)
            {
                _hier = hier;
                _cache = context.GetService<IFileStatusCache>();
                _walker = context.GetService<ISccProjectWalker>();
                _project = project as IVsProject2;
                _projectDir = projectDir;
                if(projectDir != null)
                    _projectDirItem = Cache[projectDir];
            }

            public IFileStatusCache Cache
            {
                [DebuggerStepThrough]
                get { return _cache; }
            }

            public IVsProject2 VsProject
            {
                [DebuggerStepThrough]
                get { return _project; }
            }

            public string ProjectDirectory
            {
                [DebuggerStepThrough]
                get { return _projectDir; }
            }

            public bool AddItem(SvnItem item)
            {
                uint parentId;
                return AddItem(item, out parentId);
            }

            private bool AddItem(SvnItem item, out uint parentId)
            {
                string itemDir = item.Directory;

                if(string.Equals(itemDir, ProjectDirectory, StringComparison.OrdinalIgnoreCase))
                    parentId = VSConstants.VSITEMID_ROOT;
                else
                {
                    parentId = GetId(itemDir, VSConstants.VSITEMID_ROOT);

                    if(parentId == VSConstants.VSITEMID_NIL)
                    {
                        if(!AddItem(item.Parent, out parentId))
                            return false;

                        parentId = GetId(itemDir, parentId);

                        if (parentId == VSConstants.VSITEMID_NIL)
                            return false;
                    }
                }
                VSADDRESULT[] result = new VSADDRESULT[1];

                return ErrorHandler.Succeeded(VsProject.AddItem(parentId, VSADDITEMOPERATION.VSADDITEMOP_OPENFILE, item.FullPath, 
                    1, new string[] { item.FullPath }, IntPtr.Zero, result)) && result[0] == VSADDRESULT.ADDRESULT_Success;
            }

            private uint GetId(string itemPath, uint searchBelow)
            {
                uint id;
                if(_map.TryGetValue(itemPath, out id))
                    return id;

                foreach(string s in _walker.GetSccFiles(_hier, searchBelow, ProjectWalkDepth.AllDescendants, _map))
                {
                    if(string.Equals(itemPath, s, StringComparison.OrdinalIgnoreCase))
                    {
                        if(_map.TryGetValue(itemPath, out id))
                            return id;
                    }
                }

                if(_map.TryGetValue(itemPath, out id))
                    return id;
                else
                    return VSConstants.VSITEMID_NIL;
            }

            public void RemoveItem(SvnItem item, uint id)
            {
                int found;
                VsProject.RemoveItem(0, id, out found);
                _map.Clear(); // Flush the cache to make sure ids stay valid
            }
        }

        public void PerformRefresh(IEnumerable<SvnClientAction> sccRefreshItems)
        {
            Debug.Assert(RequiresForcedRefresh(), "Refreshing a project that manages itself");

            RefreshState state = new RefreshState(_context, ProjectHierarchy, VsProject, ProjectDirectory);

            if (state.VsProject == null || state.ProjectDirectory == null)
                return; // Can't fix it

            VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];

            foreach (SvnClientAction action in sccRefreshItems)
            {
                if (!action.AddOrRemove)
                    continue; // Not for me

                SvnItem item = state.Cache[action.FullPath];
                if (!item.IsBelowPath(ProjectDirectory))
                    return;

                int found;
                uint id;

                // Check the real project here instead of our cache to keep the update initiative
                // at the project. Checking our cache might be unsafe, as we get file add and remove 
                // events from the project while we are updating
                if (!ErrorHandler.Succeeded(
                    VsProject.IsDocumentInProject(item.FullPath, out found, prio, out id)))
                    continue;

                bool bFound = (found == 0);
                if (bFound == item.Exists)
                    continue; //

                if (!bFound)
                    state.AddItem(item);
                else if (!item.Exists && found == 1)
                    state.RemoveItem(item, id);
            }
        }
    }
}
