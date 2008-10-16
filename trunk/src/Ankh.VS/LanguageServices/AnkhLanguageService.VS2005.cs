using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    // VS2005 SDK only addition to the LogMessageLanguage class
    // (The VS2008 SDK version compiles LogMessageLanguageService.VS2008.cs instead)
    partial class AnkhLanguageService
    {
        // Intentionally left blank
    }

    partial class AnkhViewFilter
    {
        public override void ShowContextMenu(int menuId, Guid groupGuid, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target)
        {
            PrepareContextMenu(ref menuId, ref groupGuid, ref target);

            base.ShowContextMenu(menuId, groupGuid, target);
        }
    }
}
