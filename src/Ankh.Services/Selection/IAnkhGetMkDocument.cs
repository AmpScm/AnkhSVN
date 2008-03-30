using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.Selection
{
    [CLSCompliant(false)]
    [Guid("E6DFB41E-D156-44F6-B38C-CB1BA2E62ACE")]
    public interface IAnkhGetMkDocument
    {
        [return: MarshalAs(UnmanagedType.Error)]
        int GetMkDocument(uint itemid, out string pbstrMkDocument);
    }
}
