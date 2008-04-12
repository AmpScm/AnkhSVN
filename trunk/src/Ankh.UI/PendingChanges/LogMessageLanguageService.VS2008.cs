using System;
using AnkhSvn.Ids;

namespace Ankh.UI.PendingChanges
{
    // VS2008 SDK only addition to the LogMessageLanguage class
    // (The VS2005 SDK version compiles LogMessageLanguageService.VS2005.cs instead)
    public partial class LogMessageLanguageService
    {
        public override string GetFormatFilterList()
        {
            return "";
        }
    }

	partial class LogMessageViewFilter
	{
		public override void ShowContextMenu(int menuId, Guid groupGuid, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target, int x, int y)
		{
            PrepareLogMessageContextMenu(ref menuId, ref groupGuid, ref target);
				
            base.ShowContextMenu(menuId, groupGuid, target, x, y);
		}
	}
}
