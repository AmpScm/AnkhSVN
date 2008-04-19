using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using System.IO;
using Ankh.Ids;
using System.Collections;
using System.Diagnostics;
using Ankh.Commands;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage
    {
        bool _sortDescending;
        bool _groupDescending;
        readonly List<PcComparer> _sorters = new List<PcComparer>();
        internal void SetSort(AnkhCommand command)
        {
            PcComparer comparer = null;
            switch (command)
            {
                case AnkhCommand.PcSortAscending:
                    _sortDescending = false;
                    break;
                case AnkhCommand.PcSortDescending:
                    _sortDescending = true;
                    break;
                case AnkhCommand.PcSortPath:
                    comparer = new PcComparer(PcComparer.CompareByPath, command);
                    break;
                case AnkhCommand.PcSortProject:
                    comparer = new PcComparer(PcComparer.CompareByProject, command);
                    break;
                case AnkhCommand.PcSortChange:
                    comparer = new PcComparer(PcComparer.CompareByChange, command);
                    break;
                case AnkhCommand.PcSortChangeList:
                    comparer = new PcComparer(PcComparer.CompareByChangeList, command);
                    break;
                case AnkhCommand.PcSortFullPath:
                    comparer = new PcComparer(PcComparer.CompareByFullPath, command);
                    break;
                case AnkhCommand.PcSortLocked:
                    comparer = new PcComparer(PcComparer.CompareByLocked, command);
                    break;
                case AnkhCommand.PcSortModified:
                    comparer = new PcComparer(PcComparer.CompareByModified, command);
                    break;
                case AnkhCommand.PcSortName:
                    comparer = new PcComparer(PcComparer.CompareByName, command);
                    break;
                case AnkhCommand.PcSortRepository:
                    comparer = new PcComparer(PcComparer.CompareByRepository, command);
                    break;
                case AnkhCommand.PcSortType:
                    comparer = new PcComparer(PcComparer.CompareByType, command);
                    break;
            }


            if (comparer != null && ((ModifierKeys & System.Windows.Forms.Keys.Control) == 0))
                _sorters.Clear();

            if (comparer != null)
            {
                _sorters.Add(comparer);
            }

            pendingCommits.ListViewItemSorter = new PcSorter(this);
            pendingCommits.Sort();
        }

        internal void UpdateSort(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.PcSortAscending:
                case AnkhCommand.PcSortDescending:
                    e.Latched = (e.Command == AnkhCommand.PcSortDescending) == _sortDescending;
                    return;
            }

            foreach (PcComparer ps in _sorters)
            {
                if (ps.Order == e.Command)
                {
                    e.Latched = true;
                    break;
                }
            }
        }

        sealed class PcSorter : IComparer
        {
            readonly PendingCommitsPage _owner;

            public PcSorter(PendingCommitsPage owner)
            {
                if(owner == null)
                    throw new ArgumentNullException("owner");
                _owner = owner;
            }

            public int Compare(object x, object y)
            {
                PendingCommitItem p1 = (PendingCommitItem)x;
                PendingCommitItem p2 = (PendingCommitItem)y;
                return _owner.Sort(p1, p2);
            }
        }

        internal int Sort(PendingCommitItem p1, PendingCommitItem p2)
        {
            PendingChange pc1 = p1.PendingChange;
            PendingChange pc2 = p2.PendingChange;
            int n = 0;
            foreach (IComparer<PendingChange> pcc in _sorters)
            {
                n = pcc.Compare(pc1, pc2);

                if (n != 0)
                    break;
            }

            if (_sortDescending)
                return -n;
            else
                return n;
        }

        sealed class PcComparer : IComparer<PendingChange>, IComparer
        {
            public delegate int CompareHandler(PendingChange one, PendingChange two);
            readonly CompareHandler _handler;
            readonly AnkhCommand _order;

            public PcComparer(CompareHandler handler, AnkhCommand order)
            {
                if (handler == null)
                    throw new ArgumentNullException("handler");

                _handler = handler;
                _order = order;
            }

            public PcComparer(CompareHandler handler, bool negate)
            {
                if (handler == null)
                    throw new ArgumentNullException("handler");

                _handler = handler;
            }

            public AnkhCommand Order
            {
                get { return _order; }
            }

            [DebuggerStepThrough]
            public int Compare(PendingChange x, PendingChange y)
            {
                return _handler(x, y);
            }

            [DebuggerStepThrough]
            int IComparer.Compare(object x, object y)
            {
                return Compare((PendingChange)x, (PendingChange)y);
            }


            public static int CompareByPath(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.RelativePath, pc2.RelativePath);
            }

            public static int CompareByProject(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.Project, pc2.Project);
            }

            public static int CompareByChange(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.ChangeText, pc2.ChangeText);
            }

            public static int CompareByChangeList(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.ChangeList, pc2.ChangeList);
            }

            public static int CompareByFullPath(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.FullPath, pc2.FullPath);
            }

            public static int CompareByLocked(PendingChange pc1, PendingChange pc2)
            {
                // TODO: Fix
                return 0; // StringComparer.OrdinalIgnoreCase.Compare(pc1.FullPath, pc2.FullPath);
            }

            public static int CompareByModified(PendingChange pc1, PendingChange pc2)
            {
                // TODO: Fix
                return 0; // StringComparer.OrdinalIgnoreCase.Compare(pc1.ChangeList, pc2.ChangeList);
            }

            public static int CompareByName(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(pc1.Name, pc2.Name);
            }

            public static int CompareByRepository(PendingChange pc1, PendingChange pc2)
            {
                return 0;
            }

            public static int CompareByType(PendingChange pc1, PendingChange pc2)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(Path.GetExtension(pc1.Name), Path.GetExtension(pc2.Name));
            }
        }        
    }
}
