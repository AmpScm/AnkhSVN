using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    public interface IHostWindowService
    {
        System.Windows.Forms.IWin32Window HostWindow { get; }
    }
}
