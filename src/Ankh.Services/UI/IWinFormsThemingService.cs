using System;
using System.Windows.Forms;

namespace Ankh.UI
{
    public interface IWinFormsThemingService
    {
        void ThemeRecursive(Control control);

        // 
        void VSThemeWindow(Control control);

        bool TryGetIcon(string path, out IntPtr hIcon);
        bool GetCurrentTheme(out Guid themeGuid);
    }
}
