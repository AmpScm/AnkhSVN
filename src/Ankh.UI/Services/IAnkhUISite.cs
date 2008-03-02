using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using AnkhSvn.Ids;

namespace Ankh.UI.Services
{
    /// <summary>
    /// Site as set on package hosted components
    /// </summary>
    public interface IAnkhUISite : ISite
    {
        IAnkhPackage Package { get; }

        void ShowContextMenu(AnkhCommandMenu menu, Point position);
        void ShowContextMenu(CommandID menu, Point position);
    }
}
