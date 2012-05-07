using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace Ankh.UI
{
    public interface ISupportsVSTheming
    {
        bool UseVSTheming { get; }
        void OnThemeChange(IUIService ui);
    }
}
