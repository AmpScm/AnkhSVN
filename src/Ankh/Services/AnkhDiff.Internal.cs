using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.DiffWindow;
using Ankh.Selection;

namespace Ankh.Services
{
    partial class AnkhDiff
    {
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

            pkg.ShowToolWindow(Ankh.Ids.AnkhToolWindow.Diff, nWnd, true);

            DiffToolWindowControl twc = GetService<ISelectionContext>().ActiveFrameControl as DiffToolWindowControl;

            if (twc != null)
                twc.Reset(nWnd, args);

            return false;
        }

        void IAnkhDiffHandler.ReleaseDiff(int frame)
        {
            if(!_freeDiffs.Contains(frame))
                _freeDiffs.Add(frame);
        }
    }
}
