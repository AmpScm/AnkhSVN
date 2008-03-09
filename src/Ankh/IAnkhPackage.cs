using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using AnkhSvn.Ids;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI.Services;

namespace Ankh
{
    [CLSCompliant(false)]
    public interface IAnkhToolWindowSite : IAnkhUISite
    {
        IVsWindowFrame Frame { get; }
        IVsWindowPane Pane { get; }
    }
}
