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

    partial class LogMessageEditor
    {
        // The VS2008 SDK seems to handle this by itself; while the VS2005 compatibility does not

        IVsFilterKeys2 _filterKeys2;
        public override bool PreProcessMessage(ref Message msg)
        {
            if(_filterKeys2 == null)
                _filterKeys2 = _context.GetService<IVsFilterKeys2>(typeof(SVsFilterKeys));

            if (_filterKeys2 != null)
            {

                //IVsFilterKeys2 performs advanced keyboard message translation

                MSG[] messages = new MSG[1];
                messages[0].hwnd = msg.HWnd;
                messages[0].lParam = msg.LParam;
                messages[0].wParam = msg.WParam;
                messages[0].message = (uint)msg.Msg;

                Guid cmdGuid;
                uint cmdCode;
                int cmdTranslated;
                int keyComboStarts;

                int hr = _filterKeys2.TranslateAcceleratorEx(messages,
                    (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseTextEditorKBScope, //Translates keys using TextEditor key bindings. Equivalent to passing CMDUIGUID_TextEditor, CMDSETID_StandardCommandSet97, and guidKeyDupe for scopes and the VSTAEXF_IgnoreActiveKBScopes flag. 
                    0,
                    null,
                    out cmdGuid,
                    out cmdCode,
                    out cmdTranslated,
                    out keyComboStarts);

                if (hr == VSConstants.S_OK)
                {
                    return true;
                }
            }

            return base.PreProcessMessage(ref msg);
        }
    }
}
