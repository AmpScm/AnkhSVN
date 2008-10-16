using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
	partial class AnkhLanguageService
	{
        public override string GetFormatFilterList()
        {
            return "";
        }
	}

    partial class AnkhViewFilter
    {
        public override void ShowContextMenu(int menuId, Guid groupGuid, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target, int x, int y)
        {
            PrepareContextMenu(ref menuId, ref groupGuid, ref target);

            base.ShowContextMenu(menuId, groupGuid, target, x, y);
        }
    }
}
