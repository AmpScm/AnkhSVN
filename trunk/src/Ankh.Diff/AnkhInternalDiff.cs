using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.DiffWindow;
using Ankh.Selection;

namespace Ankh.Diff
{
    [GlobalService(typeof(IAnkhInternalDiff), AllowPreRegistered=true)]
    [GlobalService(typeof(AnkhInternalDiff))]
    sealed class AnkhInternalDiff : AnkhService, IAnkhInternalDiff
    {
        public AnkhInternalDiff(IAnkhServiceProvider context)
            : base(context)
        {

        }

        public bool RunDiff(AnkhDiffArgs args)
        {
            return RunInternalDiff(args);
        }

        public bool HasDiff
        {
            get { return true; }
        }

        public bool RunMerge(AnkhMergeArgs args)
        {
            throw new NotImplementedException();
        }

        public bool HasMerge
        {
            get { return false; }
        }

        readonly List<int> _freeDiffs = new List<int>();
        int _nNext;
        
        private bool RunInternalDiff(AnkhDiffArgs args)
        {
            IAnkhPackage pkg = GetService<IAnkhPackage>();

            int nWnd;

            if (_freeDiffs.Count > 0)
            {
                nWnd = _freeDiffs[0];
                _freeDiffs.RemoveAt(0);
            }
            else
                nWnd = _nNext++;

            pkg.ShowToolWindow(AnkhToolWindow.Diff, nWnd, true);

            DiffToolWindowControl twc = GetService<ISelectionContext>().ActiveFrameControl as DiffToolWindowControl;

            if (twc != null)
                twc.Reset(nWnd, args);

            return false;
        }

        internal void ReleaseDiff(int frame)
        {
            if(!_freeDiffs.Contains(frame))
                _freeDiffs.Add(frame);
        }
    }
}
