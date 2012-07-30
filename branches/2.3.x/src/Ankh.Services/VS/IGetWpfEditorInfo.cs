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
        WpfEditorInfo GetWpfInfo(IVsTextView textView);
    }

    public abstract class WpfEditorInfo
    {
        public abstract Point GetTopLeft();

        public abstract int GetLineHeight();
    }
}
