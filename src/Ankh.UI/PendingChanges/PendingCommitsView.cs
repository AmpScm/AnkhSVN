using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio;
using System.ComponentModel;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;
using System.ComponentModel.Design;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges
{
    public interface IPendingChangeSource
    {
        bool HasPendingChanges { get; }
        IEnumerable<PendingChange> PendingChanges { get; }
    }

    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>, IPendingChangeSource
    {
        public PendingCommitsView()
        {
            StrictCheckboxesClick = true;
            FullRowSelect = true;
            HideSelection = false;
            AllowColumnReorder = true;
            CheckBoxes = true;
            Sorting = SortOrder.Ascending;
            Initialize();
        }

        public PendingCommitsView(IContainer container)
            : this()
        {
            container.Add(this);
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

            FinalSortColumn = path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            HookCommands();
        }

        bool _hooked;
        public void HookCommands()
        {
            if (_hooked)
                return;

			if (Context != null)
			{
                _hooked = true;
				VSCommandHandler.Install(Context, this,
					new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.SelectAll),
					OnSelectAll);
			}
        }

		void OnSelectAll(object sender, CommandEventArgs e)
		{
            SelectAllItems();
		}

        bool IPendingChangeSource.HasPendingChanges
        {
            get { return CheckedIndices.Count > 0; }
        }

        IEnumerable<PendingChange> IPendingChangeSource.PendingChanges
        {
            get
            {
                List<ListViewItem> list = new List<ListViewItem>();
                foreach (PendingCommitItem pi in CheckedItems)
                {
                    list.Add(pi);
                }

                IComparer<ListViewItem> sorter = ListViewItemSorter as IComparer<ListViewItem>;

                if (sorter != null)
                    list.Sort(sorter);

                foreach(PendingCommitItem pi in list)
                    yield return pi.PendingChange;
            }
        }

        IAnkhServiceProvider _context;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set 
			{ 
				_context = value;
			}
        }

        PendingCommitsSelectionMap _map;
        internal override SelectionItemMap SelectionMap
        {
            get { return _map ?? (_map = new PendingCommitsSelectionMap(this)); }
        }
   
        sealed class PendingCommitsSelectionMap : SelectionItemMap
        {
            public PendingCommitsSelectionMap(PendingCommitsView view)
                : base(CreateData(view))
            {

            }            
        }

        protected override string GetCanonicalName(PendingCommitItem item)
        {
            return item.FullPath;
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<PendingCommitItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = e.Item.PendingChange;
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Select();

            Point p = e.Location;
            bool showSort = false;
            if (p != new Point(-1, -1))
            {
                // Mouse context menu
                Point clP = PointToClient(e.Location);
                ListViewHitTestInfo hti = HitTest(clP);

                showSort = (hti.Item == null || hti.Location == ListViewHitTestLocations.None || hti.Location == ListViewHitTestLocations.AboveClientArea);
                if (!showSort && hti.Item != null)
                {
                    Rectangle r = hti.Item.GetBounds(ItemBoundsPortion.Entire);

                    if (!r.Contains(clP))
                        showSort = true;
                }
            }
            else
            {
                ListViewItem fi = FocusedItem;

                if (fi != null)
                    p = PointToScreen(fi.Position);
            }

            IAnkhCommandService mcs = Context.GetService<IAnkhCommandService>();
            if (mcs != null)
            {
                if (showSort)
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingCommitsSortContextMenu, p);
                else
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingChangesContextMenu, p);
            }
        }
    }
}
