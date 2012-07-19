using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Ankh.UI
{
    public interface ISupportsVSTheming
    {
        void OnThemeChange(IAnkhServiceProvider sender, CancelEventArgs e);
    }
}
