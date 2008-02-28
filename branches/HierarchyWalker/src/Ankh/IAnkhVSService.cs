using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh
{
  
    [GuidAttribute(Guids.guidAnkhVSPackageString)]
    public interface IAnkhVSService
    {
        void SetContext(IContext context);

        VSITEMSELECTION[] GetSelection();
    }
}
