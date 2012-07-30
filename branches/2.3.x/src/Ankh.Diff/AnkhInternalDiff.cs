using System;
using System.Collections.Generic;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.DiffWindow;
using Ankh.Selection;
using Ankh.VS;

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

        private bool RunInternalDiff(AnkhDiffArgs args)
        {
            DiffEditorControl diffEditor = new DiffEditorControl();

            IAnkhEditorResolver er = GetService<IAnkhEditorResolver>();

            diffEditor.CreateDiffEditor(this, args);

            return true;
        }
    }
}
