using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.UI
{
    [CLSCompliant(false)]
    public interface IAnkhHasVsTextView
    {
        IVsTextView TextView { get; }
    }
}
