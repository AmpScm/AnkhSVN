using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio;
using System.ComponentModel;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Windows.Forms;

namespace Ankh.UI.PendingChanges
{
    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>
    {
        public PendingCommitsView()
        {
            StrictCheckboxesClick = true;
        }

        public void Initialize()
        {
            SmartColumn path = new SmartColumn(this, PCStrings.PathColumn, 288);
            SmartColumn project = new SmartColumn(this, PCStrings.ProjectColumn, 76);
            SmartColumn change = new SmartColumn(this, PCStrings.ChangeColumn, 76);
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
                change,
                fullPath
            });

            
            change.Groupable = true;
            changeList.Groupable = true;
            folder.Groupable = true;
            locked.Groupable = true;
            project.Groupable = true;
            type.Groupable = true;
            workingCopy.Groupable = true;

            path.Hideable = false;
            
            AllColumns.Add(change);
            AllColumns.Add(changeList);
            AllColumns.Add(folder);
            AllColumns.Add(fullPath);
            AllColumns.Add(locked);
            AllColumns.Add(modified);
            AllColumns.Add(name);
            AllColumns.Add(path);
            AllColumns.Add(project);
            AllColumns.Add(type);
            AllColumns.Add(workingCopy);

            SortColumns.Add(path);
            GroupColumns.Add(changeList);
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public PendingCommitsView(IContainer container)
        {
            container.Add(this);
            StrictCheckboxesClick = true;
        }

        PendingCommitsSelectionMap _map;
        internal override SelectionItemMap SelectionMap
        {
            get { return _map ?? (_map = new PendingCommitsSelectionMap(this)); }
        }
   
        sealed class PendingCommitsSelectionMap : SelectionItemMap, IAnkhGetMkDocument
        {
            public PendingCommitsSelectionMap(PendingCommitsView view)
                : base(CreateData(view))
            {

            }

            #region IAnkhGetMkDocument Members

            public int GetMkDocument(uint itemid, out string pbstrMkDocument)
            {
                PendingCommitItem pci = (PendingCommitItem)GetItem(itemid);

                if (pci != null)
                {
                    pbstrMkDocument = pci.FullPath;
                    return VSConstants.S_OK;
                }
                else
                    pbstrMkDocument = null;

                return VSConstants.E_FAIL;
            }

            #endregion
        }

        protected override string GetCanonicalName(PendingCommitItem item)
        {
            return item.FullPath;
        }
    }
}
