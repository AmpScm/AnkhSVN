using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.UI
{
    /// <summary>
    /// Public api of the ankh package as used by other components
    /// </summary>
    public interface IAnkhPackage : IAnkhServiceProvider, System.ComponentModel.Design.IServiceContainer
    {
        void ShowToolWindow(AnkhToolWindow window);
        void ShowToolWindow(AnkhToolWindow window, int id, bool create);
    }
}
