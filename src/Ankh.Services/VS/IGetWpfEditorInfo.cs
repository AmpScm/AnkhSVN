using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.VS
{
    [CLSCompliant(false)]
    public interface IGetWpfEditorInfo
    {
        IWpfEditorInfo GetWpfInfo(IVsTextView textView);
    }

    public interface IWpfEditorInfo
    {
        Point GetTopLeft();

        int GetLineHeight();
    }
}
