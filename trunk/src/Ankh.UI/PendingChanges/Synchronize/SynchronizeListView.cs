using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.PendingChanges.Synchronize
{    
    class SynchronizeListView : ListViewWithSelection<SynchronizeListItem>
    {
        IAnkhServiceProvider _context;

        public SynchronizeListView()
        {
            Initialize();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; SelectionPublishServiceProvider = value; }
        }

        protected override void OnResolveItem(ListViewWithSelection<SynchronizeListItem>.ResolveItemEventArgs e)
        {
            e.Item = ((SynchronizeItem)e.SelectionItem).ListItem;
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<SynchronizeListItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new SynchronizeItem(Context, e.Item);
        }

        void Initialize()
        {
            SmartColumn path = new SmartColumn(this, PCStrings.PathColumn, 288);
            SmartColumn project = new SmartColumn(this, PCStrings.ProjectColumn, 76);
            SmartColumn localChange = new SmartColumn(this, PCStrings.LocalChangeColumn, 102);
            SmartColumn remoteChange = new SmartColumn(this, PCStrings.RemoteChangeColumn, 102);
            SmartColumn fullPath = new SmartColumn(this, PCStrings.FullPathColumn, 327);

            SmartColumn changeList = new SmartColumn(this, PCStrings.ChangeListColumn, 76);
            SmartColumn folder = new SmartColumn(this, PCStrings.FolderColumn, 196);
            SmartColumn locked = new SmartColumn(this, PCStrings.LockedColumn, 38);
            SmartColumn modified = new SmartColumn(this, PCStrings.ModifiedColumn, 76);
            SmartColumn name = new SmartColumn(this, PCStrings.NameColumn, 76);
            SmartColumn type = new SmartColumn(this, PCStrings.TypeColumn, 76);
            SmartColumn workingCopy = new SmartColumn(this, PCStrings.WorkingCopyColumn, 76);

            Columns.AddRange(new ColumnHeader[]
            {
                path,
                project,
                localChange,
                remoteChange,
                fullPath
            });

            localChange.Groupable = true;
            remoteChange.Groupable = true;

            changeList.Groupable = true;
            folder.Groupable = true;
            locked.Groupable = true;
            project.Groupable = true;
            type.Groupable = true;
            workingCopy.Groupable = true;

            path.Hideable = false;

            modified.Sorter = new SortWrapper(
                delegate(SynchronizeListItem x, SynchronizeListItem y)
                {
                    return x.SvnItem.Modified.CompareTo(y.SvnItem.Modified);
                });
            
            AllColumns.Add(changeList);
            AllColumns.Add(folder);
            AllColumns.Add(fullPath);
            AllColumns.Add(localChange);            
            AllColumns.Add(locked);
            AllColumns.Add(modified);
            AllColumns.Add(name);
            AllColumns.Add(path);
            AllColumns.Add(project);
            AllColumns.Add(remoteChange);
            AllColumns.Add(type);
            AllColumns.Add(workingCopy);

            SortColumns.Add(path);
            GroupColumns.Add(changeList);

            FinalSortColumn = path;
        }

        protected override string GetCanonicalName(SynchronizeListItem item)
        {
            return item.SvnItem.FullPath;
        }
    }
}
