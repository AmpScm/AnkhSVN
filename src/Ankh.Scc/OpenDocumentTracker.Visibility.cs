using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
    partial class OpenDocumentTracker
    {
        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            ProjectMap.SccDocumentData dd;

            if (TryGetDocument(docCookie, out dd))
            {
                dd.CheckDirty();
            }
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }
    }
}
