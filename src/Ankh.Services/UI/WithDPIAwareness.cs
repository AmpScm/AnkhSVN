using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.UI
{
    public enum AnkhDpiAwareness
    {
        Unaware = -1,
        SystemAware = -2,
        PerMonitorAware = -3,
        PerMonitorAwareV2 = -4,
        UnawareGdiScaled = -5
    }

    public static class WithDPIAwareness
    {

        //
        // Copied from
        // https://support.microsoft.com/en-us/help/4490421/webbrowser-or-wpf-control-content-may-not-display-correctly-in-office
        //
        class NativeImports
        {
            internal enum DPI_HOSTING_BEHAVIOR
            {
                DPI_HOSTING_BEHAVIOR_INVALID = -1,
                DPI_HOSTING_BEHAVIOR_DEFAULT = 0,
                DPI_HOSTING_BEHAVIOR_MIXED = 1
            };

            internal enum DPI_AWARENESS_CONTEXT
            {
                Unaware = -1,
                SystemAware = -2,
                PerMonitorAware = -3,
                PerMonitorAwareV2 = -4,
                UnawareGdiScaled = -5
            }
           
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern AnkhDpiAwareness SetThreadDpiAwarenessContext(AnkhDpiAwareness value);
        }

        public static void Run(AnkhDpiAwareness aw, Action action)
        {
            AnkhDpiAwareness last = AnkhDpiAwareness.Unaware;
            bool updatedSetting;
            try 
            {
                last = NativeImports.SetThreadDpiAwarenessContext(aw);
                updatedSetting = (last != aw);
            }
            catch(EntryPointNotFoundException)
            {
                updatedSetting = false;
            }

            action();

            if (updatedSetting)
                NativeImports.SetThreadDpiAwarenessContext(last);
        }
    }
}
