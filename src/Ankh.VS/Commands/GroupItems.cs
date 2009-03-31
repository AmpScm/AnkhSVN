using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS.Selection;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectItemGroup, AlwaysAvailable = true)]
    [Command(AnkhCommand.ProjectItemUngroup, AlwaysAvailable = true)]
    class GroupItems : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
            /*switch (e.Command)
            {
                case AnkhCommand.ProjectItemGroup:
                    OnUpdateGroup(e);
                    break;
                case AnkhCommand.ProjectItemUngroup:
                    OnUpdateUngroup(e);
                    break;
                default:
                    e.Enabled = false;
                    break;
            }*/
        }

        private void OnUpdateGroup(CommandUpdateEventArgs e)
        {
            SelectionContext selection = e.Selection as SelectionContext;

            if (selection == null || selection.IsSingleNodeSelection)
            {
                e.Enabled = false;
                return;
            }

            IVsSccProject2 project = null;
            IVsHierarchy hier = null;
            uint parentId = VSConstants.VSITEMID_NIL;
            bool enable = false;

            try
            {
                foreach (SelectionItem item in selection.GetSelectedItems(false))
                {
                    if (project == null)
                    {
                        project = item.SccProject;

                        if (project == null)
                            return;

                        hier = (IVsHierarchy)project;
                        if (hier == null)
                            return;
                    }
                    else if (item.SccProject != project || hier == null)
                        return;

                    object value;

                    if (!ErrorHandler.Succeeded(hier.GetProperty(item.Id, (int)__VSHPROPID.VSHPROPID_Parent, out value)))
                        return;

                    if (parentId == VSConstants.VSITEMID_NIL)
                    {
                        parentId = SelectionContext.GetItemIdFromObject(value);

                        if (parentId == VSConstants.VSITEMID_NIL)
                            return;
                    }
                    else if (parentId != SelectionContext.GetItemIdFromObject(value))
                        return;
                }

                enable = true; // All items are from the same project and have the same parent.. Ok!
            }
            catch
            { /* Just disable on buggy projects */ }
            finally
            {
                if (!enable)
                    e.Enabled = false;
            }
        }

        private void OnUpdateUngroup(CommandUpdateEventArgs e)
        {
            SelectionContext selection = e.Selection as SelectionContext;

            if (selection == null)
            {
                e.Enabled = false;
                return;
            }

            IVsSccProject2 project = null;
            IVsHierarchy hier = null;
            uint parentId = VSConstants.VSITEMID_NIL;
            bool enable = false;

            try
            {
                foreach (SelectionItem item in selection.GetSelectedItems(false))
                {
                    if (project == null)
                    {
                        project = item.SccProject;

                        if (project == null)
                            return;

                        hier = (IVsHierarchy)project;
                        if (hier == null)
                            return;
                    }
                    else if (item.SccProject != project || hier == null)
                        return;

                    object value;

                    if (!ErrorHandler.Succeeded(hier.GetProperty(item.Id, (int)__VSHPROPID.VSHPROPID_Parent, out value)))
                        return;
                    if (!ErrorHandler.Succeeded(hier.GetProperty(item.Id, (int)__VSHPROPID.VSHPROPID_Parent, out value)))
                        return;

                    if (parentId == VSConstants.VSITEMID_NIL)
                    {
                        parentId = SelectionContext.GetItemIdFromObject(value);

                        if (parentId == VSConstants.VSITEMID_NIL)
                            return;
                    }
                    else if (parentId != SelectionContext.GetItemIdFromObject(value))
                        return;
                }

                // All nodes have the same parent. Check if their filenames match the parent
                if (hier == null || parentId == VSConstants.VSITEMID_NIL || parentId == VSConstants.VSITEMID_ROOT)
                    return;

                IVsProject2 project2 = project as IVsProject2;
                if (project2 == null)
                    return;

                string parentName;
                if (!ErrorHandler.Succeeded(project2.GetMkDocument(parentId, out parentName)) || !SvnItem.IsValidPath(parentName))
                    return;

                parentName = Path.Combine(SharpSvn.SvnTools.GetNormalizedDirectoryName(parentName), Path.GetFileNameWithoutExtension(parentName) + ".");

                foreach (string file in selection.GetSelectedFiles(false))
                {
                    if (!file.StartsWith(parentName, StringComparison.OrdinalIgnoreCase) && 0 > file.IndexOf(Path.DirectorySeparatorChar, parentName.Length))
                        return; // File doesn't match parent name
                }

                enable = true; 
            }
            catch
            { /* Just disable on buggy projects */ }
            finally
            {
                if (!enable)
                    e.Enabled = false;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            return;
        }

    }
}
