using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh
{
  
    [GuidAttribute(Guids.guidAnkhVSPackageString)]
    public interface IAnkhVSService
    {
        void SetContext(IContext context);
    }
}
