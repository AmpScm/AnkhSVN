using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS.Selection;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;
using Ankh.Scc;

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
            }
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
                else 
                    foreach (SvnItem i in selection.GetSelectedSvnItems(false))
                    {
                        if (!i.Exists || !i.InSolution)
                        {
                            e.Enabled = false;
                        }
                    }
            }
        }

        private void OnUpdateUngroup(CommandUpdateEventArgs e)
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
            uint firstId = VSConstants.VSITEMID_NIL;
            bool enable = false;
            bool foundParent = false;

            try
            {
                foreach (SelectionItem item in selection.GetSelectedItems(false))
                {
                    if (project == null)
                    {
                        project = item.SccProject;

                        if (project == null)
                            return;

                        hier = item.Hierarchy;
                        if (hier == null)
                            return;
                    }
                    else if (item.SccProject != project || hier == null)
                        return;

                    object value;

                    if (!ErrorHandler.Succeeded(hier.GetProperty(item.Id, (int)__VSHPROPID.VSHPROPID_Parent, out value)))
                        return;
                    
                    uint pId = SelectionContext.GetItemIdFromObject(value);

                    if(pId == VSConstants.VSITEMID_NIL)
                        return;

                    if (firstId == VSConstants.VSITEMID_NIL && parentId == VSConstants.VSITEMID_NIL)
                    {
                        firstId = item.Id;
                        parentId = pId;
                    }
                    else
                    {
                        if (parentId == pId)
                            continue; // Same parent, continue
                        else if (foundParent)
                            return; // Not the same parent -> Break

                        if (item.Id == parentId)
                        {
                            // Current node is the parent, continue
                            foundParent = true;
                        }
                        else if (firstId == pId)
                        {
                            // First node was the parent
                            parentId = pId;
                            foundParent = true;
                        }
                        else
                            return; // No parent child relation
                    }
                }

                if (!foundParent)
                    return; // You must select the parent to enable this command

                string[] files;
                if (!SelectionUtils.GetSccFiles(new SelectionItem(hier, parentId), out files, false, false, null)
                    || files == null || files.Length == 0)
                    return; // Parent is not a file

                SvnItem parent = e.GetService<IFileStatusCache>()[files[0]];

                if(parent.IsFile)
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
            switch (e.Command)
            {
                case AnkhCommand.ProjectItemGroup:
                    OnGroupItems(e);
                    break;
                case AnkhCommand.ProjectItemUngroup:
                    OnUngroupItems(e);
                    break;
            }
        }

        private void OnUngroupItems(CommandEventArgs e)
        {
            
        }

        private void OnGroupItems(CommandEventArgs e)
        {
            
        }
    }
}
