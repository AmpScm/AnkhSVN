using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.UI.Services
{
    /// <summary>
    /// Site as set on package hosted components
    /// </summary>
    [CLSCompliant(false)]
    public interface IAnkhUISite : ISite, IAnkhServiceProvider
    {
        IAnkhPackage Package { get; }
        string Title { get; set; }
        string OriginalTitle { get; }
        IOleCommandTarget CommandTarget { get; set; }


        bool ShowContextMenu(AnkhCommandMenu menu, int x, int y);
        bool ShowContextMenu(CommandID menu, int x, int y);
    }
}
