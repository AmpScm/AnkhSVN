using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using AnkhSvn.Ids;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh
{
    public interface IAnkhPackage : System.IServiceProvider
    {
        void ShowToolWindow(AnkhToolWindow window);
        void ShowToolWindow(AnkhToolWindow window, int id, bool create);
    }

    public interface IAnkhToolWindowSite : ISite
    {
        IAnkhPackage Package { get; }
        IVsWindowFrame Frame { get; }
        IVsWindowPane Pane { get; }
    }
}
