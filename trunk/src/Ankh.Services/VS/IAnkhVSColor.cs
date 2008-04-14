using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS
{
    [CLSCompliant(false)]
    public interface IAnkhVSColor
    {
        bool TryGetColor(__VSSYSCOLOREX vsColor, out Color color);
    }
}
