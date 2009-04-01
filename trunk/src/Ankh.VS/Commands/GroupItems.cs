using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS.Selection;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.IO;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectItemGroup, AlwaysAvailable = true)]
    [Command(AnkhCommand.ProjectItemUngroup, AlwaysAvailable = true)]
    class GroupItems : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
            /*
            switch (e.Command)
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
            */
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
                {
                    SortedList<string,string> names = new SortedList<string,string>(StringComparer.OrdinalIgnoreCase);
                    foreach (SvnItem i in selection.GetSelectedSvnItems(false))
                    {
                        if (!i.Exists || !i.InSolution)
                        {
                            e.Enabled = false;
                        }

                        string name = i.NameWithoutExtension;

                        if (names.ContainsKey(name))
                        {
                            e.Enabled = false;
                            break;
                        }
                        names.Add(name, i.FullPath);
                    }

                    string first = null;
                    string firstName = null;
                    foreach (string i in names.Keys)
                    {
                        if (first == null)
                        {
                            first = i + ".";
                            firstName = i;
                        }
                        else
                            if (!i.StartsWith(i, StringComparison.OrdinalIgnoreCase))
                            {
                                e.Enabled = false;
                            }
                    }

                    e.Selection.Cache[AnkhCommand.ProjectItemGroup] = names[firstName];
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

        private void OnGroupItems(CommandEventArgs e)
        {
            // OnUpdateGroup checked whether the selection is ok, we just
            // have to move the nodes below the parent node now

            SelectionContext selection = e.Selection as SelectionContext;
            string parentItem = e.Selection.Cache[AnkhCommand.ProjectItemGroup] as string;

            if (parentItem == null || selection == null)
                return;

            IVsProject2 p2 = null;
            uint newParent = VSConstants.VSITEMID_NIL; ;
            List<KeyValuePair<string,SelectionItem>> children = new List<KeyValuePair<string,SelectionItem>>();
            foreach (SelectionItem item in selection.GetSelectedItems(false))
            {
                if(p2 == null)
                    p2 = (IVsProject2)item.Hierarchy;

                string doc;
                if (!ErrorHandler.Succeeded(p2.GetMkDocument(item.Id, out doc)))
                    return;

                doc = SvnTools.GetTruePath(doc);

                if(doc == null)
                    return;

                if (string.Equals(parentItem, doc, StringComparison.OrdinalIgnoreCase))
                    newParent = item.Id;
                else
                    children.Add(new KeyValuePair<string,SelectionItem>(doc,item));
            }

            IAnkhOpenDocumentTracker odt = e.GetService<IAnkhOpenDocumentTracker>();

            VSDOCUMENTPRIORITY[] prio = new VSDOCUMENTPRIORITY[1];
            VSADDRESULT[] addResult = new VSADDRESULT[1];
            using(e.GetService<IAnkhSccService>().DisableSvnUpdates())
                foreach (KeyValuePair<string, SelectionItem> v in children)
                {
                    int result;
                    object vv;

                    odt.SaveDocument(v.Key);

                    string tmp = v.Key + ".!tmp~";
                    File.Move(v.Key, tmp);
                    try
                    {
                        if (!ErrorHandler.Succeeded(((IVsHierarchy)p2).GetProperty(v.Value.Id, (int)__VSHPROPID.VSHPROPID_Parent, out vv)) ||
                            !ErrorHandler.Succeeded(p2.RemoveItem(0, v.Value.Id, out result)))
                            continue;
                    }
                    finally
                    {
                        File.Move(tmp, v.Key);
                    }

                    uint parentId = SelectionContext.GetItemIdFromObject(vv);

                    if (parentId == VSConstants.VSITEMID_NIL)
                        return;

                    if (!ErrorHandler.Succeeded(p2.AddItem(newParent, VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE, v.Key, 1, new string[] { v.Key }, IntPtr.Zero, addResult))
                        || addResult[0] != VSADDRESULT.ADDRESULT_Success)
                    {
                        // Insert below old parent
                        p2.AddItem(parentId, VSADDITEMOPERATION.VSADDITEMOP_OPENFILE, v.Key, 1, new string[] { v.Key }, IntPtr.Zero, addResult);
                    }
                }
        }

        private void OnUngroupItems(CommandEventArgs e)
        {
            
        }

        
    }
}
