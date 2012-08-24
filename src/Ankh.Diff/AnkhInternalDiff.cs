using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Scc.UI;
using Ankh.UI.DiffWindow;
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

            if (diffEditor.WindowFrame != null && !args.ShowDiffAsDocument && VSVersion.VS2010OrLater)
                diffEditor.WindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, (int)VSFRAMEMODE.VSFM_Float);

            return true;
        }
    }
}
