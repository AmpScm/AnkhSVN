using System;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace Ankh.UI.PendingChanges
{
    // VS2005 SDK only addition to the LogMessageLanguage class
    // (The VS2008 SDK version compiles LogMessageLanguageService.VS2008.cs instead)
    public partial class LogMessageLanguageService
    {
        // Intensionally left blank
    }

	partial class LogMessageViewFilter
	{
		public override void ShowContextMenu(int menuId, Guid groupGuid, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target)
		{
            PrepareLogMessageContextMenu(ref menuId, ref groupGuid, ref target);

			base.ShowContextMenu(menuId, groupGuid, target);
		}
	}
}
