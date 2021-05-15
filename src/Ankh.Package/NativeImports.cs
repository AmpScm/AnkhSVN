using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.VSPackage
{
    //
    // Copied from
    // https://support.microsoft.com/en-us/help/4490421/webbrowser-or-wpf-control-content-may-not-display-correctly-in-office
    //
    internal class NativeImports
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

        [DllImport("user32.dll")]
        internal static extern DPI_HOSTING_BEHAVIOR SetThreadDpiHostingBehavior(DPI_HOSTING_BEHAVIOR value);

        [DllImport("user32.dll")]
        internal static extern DPI_HOSTING_BEHAVIOR GetThreadDpiHostingBehavior();

        [DllImport("user32.dll")]
        internal static extern DPI_HOSTING_BEHAVIOR GetWindowDpiHostingBehavior(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //[DllImport("user32.dll", SetLastError=true)]
        //static extern IntPtr GetWindowDpiAwarenessContext(IntPtr hWnd);

        //[DllImport("user32.dll", SetLastError=true)]
        //static extern IntPtr GetThreadDpiAwarenessContext();

        //[DllImport("user32.dll", SetLastError=true)]
        //static extern int GetAwarenessFromDpiAwarenessContext(InPtr DPI_AWARENESS_CONTEXT);


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern DPI_AWARENESS_CONTEXT SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT value);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT value);



    }
}
