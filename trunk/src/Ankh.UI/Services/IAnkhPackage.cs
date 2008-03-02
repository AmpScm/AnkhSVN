﻿using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.UI.Services
{
    /// <summary>
    /// Public api of the ankh package as used by other components
    /// </summary>
    public interface IAnkhPackage : System.IServiceProvider
    {
        void ShowToolWindow(AnkhToolWindow window);
        void ShowToolWindow(AnkhToolWindow window, int id, bool create);
    }
}
