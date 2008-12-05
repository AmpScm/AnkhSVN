using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Windows.Forms;
using System.Drawing;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListView : ListViewWithSelection<RepositoryListItem>
    {
        IAnkhServiceProvider _context;

        public RepositoryListView()
        {
            InitializeColumns();
        }

        private void InitializeColumns()
        {
            SmartColumn file = new SmartColumn(this, RepositoryStrings.FileColumn, 100);
            SmartColumn extension = new SmartColumn(this, RepositoryStrings.ExtensionColumn, 70);
            SmartColumn revision = new SmartColumn(this, RepositoryStrings.RevisionColumn, 60);
            SmartColumn author = new SmartColumn(this, RepositoryStrings.AuthorColumn, 60);
            SmartColumn size = new SmartColumn(this, RepositoryStrings.SizeColumn, 60);
            SmartColumn date = new SmartColumn(this, RepositoryStrings.DateColumn, 100);
            SmartColumn lockOwner = new SmartColumn(this, RepositoryStrings.LockOwnerColumn, 100);

            file.Sorter = new SortWrapper(
                delegate(RepositoryListItem x, RepositoryListItem y)
                {
                    if (x.IsFolder ^ y.IsFolder)
                    {
                        return x.IsFolder ? -1 : 1;
                    }

                    return StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text);
                });
            size.Sorter = new SortWrapper(
                delegate(RepositoryListItem x, RepositoryListItem y)
                {
                    if (x.IsFolder ^ y.IsFolder)
                    {
                        return x.IsFolder ? -1 : 1;
                    }

                    long lx = x.Info.Entry.FileSize;
                    long ly = y.Info.Entry.FileSize;

                    if (lx < ly)
                        return -1;
                    else if (lx > ly)
                        return 1;
                    else
                        return 0;
                });
            date.Sorter = new SortWrapper(
                delegate(RepositoryListItem x, RepositoryListItem y)
                {
                    return x.Info.Entry.Time.CompareTo(y.Info.Entry.Time);
                });

            AllColumns.Add(file);
            AllColumns.Add(extension);
            AllColumns.Add(revision);
            AllColumns.Add(author);
            AllColumns.Add(size);
            AllColumns.Add(date);
            AllColumns.Add(lockOwner);

            Columns.AddRange(new ColumnHeader[]
            {
                file,
                extension,
                revision,
                author,
                size,
                date,
                lockOwner
            });

            SortColumns.Add(file);
            FinalSortColumn = file;
            UpdateSortGlyphs();

            FinalSortColumn = file;
            AllowColumnReorder = true;
        }

        protected override string GetCanonicalName(RepositoryListItem item)
        {
            Uri uri = item.Info.EntryUri;

            if (uri != null)
                return uri.AbsoluteUri;
            else
                return null;
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                if (value != null)
                    OnContextChanged(EventArgs.Empty);
            }
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Point screen = (e.Location != new Point(-1, -1)) ? e.Location : PointToScreen(new Point(0, 0));

            IAnkhCommandService sc = Context.GetService<IAnkhCommandService>();

            Point p = PointToClient(e.Location);

            AnkhCommandMenu menu;
            if (p.Y < HeaderHeight)
            {
                Select(); // Must be the active control for the menu to work
                menu = AnkhCommandMenu.ListViewHeader;
            }
            else
                menu = AnkhCommandMenu.RepositoryExplorerContextMenu;

            sc.ShowContextMenu(menu, screen);
        }

        private void OnContextChanged(EventArgs eventArgs)
        {
            IFileIconMapper fim = Context.GetService<IFileIconMapper>();

            if (fim != null)
                SmallImageList = fim.ImageList;
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((RepositoryExplorerItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        protected override void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new RepositoryExplorerItem(Context, e.Item.Origin, e.Item);
            base.OnRetrieveSelection(e);
        }
    }
}
