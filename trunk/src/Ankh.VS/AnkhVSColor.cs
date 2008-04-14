using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Drawing;

namespace Ankh.VS
{
    class AnkhVSColor : IAnkhVSColor
    {
        readonly IAnkhServiceProvider _context;
        
        public AnkhVSColor(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        IVsUIShell2 _shell;
        IVsUIShell2 UIShell
        {
            get
            {
                if (_shell == null)
                    _shell = _context.GetService<IVsUIShell2>(typeof(SVsUIShell));
                return _shell;
            }
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
