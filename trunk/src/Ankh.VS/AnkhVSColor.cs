using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Drawing;

namespace Ankh.VS
{
    [GlobalService(typeof(IAnkhVSColor))]
    class AnkhVSColor : AnkhService, IAnkhVSColor
    {
        public AnkhVSColor(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell2 _uiShell;
        IVsUIShell2 UIShell
        {
            get { return _uiShell ?? (_uiShell = GetService<IVsUIShell2>(typeof(SVsUIShell))); }
        }

        public bool TryGetColor(__VSSYSCOLOREX vsColor, out Color color)
        {
            uint rgb;
            if (ErrorHandler.Succeeded(UIShell.GetVSSysColorEx((int)vsColor, out rgb)))
            {
                color = ColorTranslator.FromWin32(unchecked((int)rgb));
                return true;
            }
            color = Color.Empty;
            return false;
        }
    }
}
