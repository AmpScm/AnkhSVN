using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Shapes;
using System.Windows;
using System.Collections.Generic;

namespace Ankh.VS.WpfServices.UI
{
    /// <summary>Base class for WPF dialogs in Visual Studio 10 and later. The class provides consistent styling and caption buttons with other dialogs in VisualStudio.</summary>
    public abstract class VSWpfDialogBase : Window
    {
        public static readonly DependencyProperty HasMaximizeButtonProperty;
        public static readonly DependencyProperty HasMinimizeButtonProperty;
        public static readonly DependencyProperty HasDialogFrameProperty;
        public static readonly DependencyProperty HasHelpButtonProperty;
        private HwndSource hwndSource;
        public bool HasMaximizeButton
        {
            get
            {
                return (bool)base.GetValue(VSWpfDialogBase.HasMaximizeButtonProperty);
            }
            set
            {
                base.SetValue(VSWpfDialogBase.HasMaximizeButtonProperty, value);
            }
        }
        public bool HasMinimizeButton
        {
            get
            {
                return (bool)base.GetValue(VSWpfDialogBase.HasMinimizeButtonProperty);
            }
            set
            {
                base.SetValue(VSWpfDialogBase.HasMinimizeButtonProperty, value);
            }
        }
        public bool HasDialogFrame
        {
            get
            {
                return (bool)base.GetValue(VSWpfDialogBase.HasDialogFrameProperty);
            }
            set
            {
                base.SetValue(VSWpfDialogBase.HasDialogFrameProperty, value);
            }
        }
        public bool HasHelpButton
        {
            get
            {
                return (bool)base.GetValue(VSWpfDialogBase.HasHelpButtonProperty);
            }
            set
            {
                base.SetValue(VSWpfDialogBase.HasHelpButtonProperty, value);
            }
        }
        static VSWpfDialogBase()
        {
            VSWpfDialogBase.HasMaximizeButtonProperty = DependencyProperty.Register("HasMaximizeButton", typeof(bool), typeof(VSWpfDialog), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(VSWpfDialogBase.OnWindowStyleChanged)));
            VSWpfDialogBase.HasMinimizeButtonProperty = DependencyProperty.Register("HasMinimizeButton", typeof(bool), typeof(VSWpfDialog), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(VSWpfDialogBase.OnWindowStyleChanged)));
            VSWpfDialogBase.HasDialogFrameProperty = DependencyProperty.Register("HasDialogFrame", typeof(bool), typeof(VSWpfDialog), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(VSWpfDialogBase.OnWindowStyleChanged)));
            VSWpfDialogBase.HasHelpButtonProperty = DependencyProperty.Register("HasHelpButton", typeof(bool), typeof(VSWpfDialog), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(VSWpfDialogBase.OnWindowStyleChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(VSWpfDialogBase), new FrameworkPropertyMetadata(typeof(VSWpfDialogBase)));
        }
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (this.hwndSource != null)
            {
                this.hwndSource.Dispose();
                this.hwndSource = null;
            }
            base.OnClosed(e);
        }
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            this.hwndSource.AddHook(new HwndSourceHook(this.WndProcHook));
            this.UpdateWindowStyle(this.hwndSource.Handle);
            base.OnSourceInitialized(e);
        }
        private static void OnWindowStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VSWpfDialog dialogWindow = (VSWpfDialog)obj;
            HwndSource hwndSource = dialogWindow.hwndSource;
            if (hwndSource != null)
            {
                dialogWindow.UpdateWindowStyle(hwndSource.Handle);
            }
        }
        private void UpdateWindowStyle(IntPtr hwnd)
        {
            int num = NativeMethods.GetWindowLong(hwnd, -16);
            if (this.HasMaximizeButton)
            {
                num |= 65536;
            }
            else
            {
                num &= -65537;
            }
            if (this.HasMinimizeButton)
            {
                num |= 131072;
            }
            else
            {
                num &= -131073;
            }
            NativeMethods.SetWindowLong(hwnd, -16, num);
            num = NativeMethods.GetWindowLong(hwnd, -20);
            if (this.HasDialogFrame)
            {
                num |= 1;
            }
            else
            {
                num &= -2;
            }
            if (this.HasHelpButton)
            {
                num |= 1024;
            }
            else
            {
                num &= -1025;
            }
            NativeMethods.SetWindowLong(hwnd, -20, num);
            NativeMethods.SendMessage(hwnd, 128, new IntPtr(1), IntPtr.Zero);
            NativeMethods.SendMessage(hwnd, 128, new IntPtr(0), IntPtr.Zero);
        }
        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 274 && wParam.ToInt32() == 61824)
            {
                this.InvokeDialogHelp();
                handled = true;
            }
            if (msg == 256 && wParam.ToInt32() == 112)
            {
                this.InvokeDialogHelp();
                handled = true;
            }
            if ((msg == 26 && wParam.ToInt32() == 67) || msg == 21)
            {
                this.OnDialogThemeChanged();
                handled = true;
            }
            return IntPtr.Zero;
        }
        protected virtual void OnDialogThemeChanged()
        {
        }
        protected abstract void InvokeDialogHelp();


        internal static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
            public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

            public static int GetWindowLong(IntPtr hWnd, int nIndex)
            {
                return NativeMethods.GetWindowLong32(hWnd, nIndex);
            }

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int SetWindowLong(IntPtr hWnd, short nIndex, int value);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hwnd, out NativeMethods.RECT lpRect);

            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
                public System.Windows.Point Position
                {
                    get
                    {
                        return new System.Windows.Point((double)this.Left, (double)this.Top);
                    }
                }
                public System.Windows.Size Size
                {
                    get
                    {
                        return new System.Windows.Size((double)this.Width, (double)this.Height);
                    }
                }
                public int Height
                {
                    get
                    {
                        return this.Bottom - this.Top;
                    }
                    set
                    {
                        this.Bottom = this.Top + value;
                    }
                }
                public int Width
                {
                    get
                    {
                        return this.Right - this.Left;
                    }
                    set
                    {
                        this.Right = this.Left + value;
                    }
                }
                public RECT(int left, int top, int right, int bottom)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Right = right;
                    this.Bottom = bottom;
                }
                public RECT(System.Drawing.Rectangle r)
                {
                    this.Left = r.Left;
                    this.Top = r.Top;
                    this.Right = r.Right;
                    this.Bottom = r.Bottom;
                }
                public RECT(Rect rect)
                {
                    this.Left = (int)rect.Left;
                    this.Top = (int)rect.Top;
                    this.Right = (int)rect.Right;
                    this.Bottom = (int)rect.Bottom;
                }
                public void Offset(int dx, int dy)
                {
                    this.Left += dx;
                    this.Right += dx;
                    this.Top += dy;
                    this.Bottom += dy;
                }
            }

            internal static void FindMaximumSingleMonitorRectangle(NativeMethods.RECT windowRect, out NativeMethods.RECT screenSubRect, out NativeMethods.RECT monitorRect)
            {
                List<NativeMethods.RECT> rects = new List<NativeMethods.RECT>();
                NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeMethods.RECT rect, IntPtr lpData)
                {
                    NativeMethods.MONITORINFO mONITORINFO = default(NativeMethods.MONITORINFO);
                    mONITORINFO.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.MONITORINFO));
                    NativeMethods.GetMonitorInfo(hMonitor, ref mONITORINFO);
                    rects.Add(mONITORINFO.rcWork);
                    return true;
                }, IntPtr.Zero);
                long num = 0L;
                screenSubRect = new NativeMethods.RECT
                {
                    Left = 0,
                    Right = 0,
                    Top = 0,
                    Bottom = 0
                };
                monitorRect = new NativeMethods.RECT
                {
                    Left = 0,
                    Right = 0,
                    Top = 0,
                    Bottom = 0
                };
                foreach (NativeMethods.RECT current in rects)
                {
                    NativeMethods.RECT rECT = current;
                    NativeMethods.RECT rECT2;
                    NativeMethods.IntersectRect(out rECT2, ref rECT, ref windowRect);
                    long num2 = (long)(rECT2.Width * rECT2.Height);
                    if (num2 > num)
                    {
                        screenSubRect = rECT2;
                        monitorRect = current;
                        num = num2;
                    }
                }
            }

            [return: MarshalAs(UnmanagedType.Bool)]
            internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeMethods.RECT lprcMonitor, IntPtr dwData);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, NativeMethods.EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IntersectRect(out NativeMethods.RECT lprcDst, [In] ref NativeMethods.RECT lprcSrc1, [In] ref NativeMethods.RECT lprcSrc2);

            public struct MONITORINFO
            {
                public uint cbSize;
                public NativeMethods.RECT rcMonitor;
                public NativeMethods.RECT rcWork;
                public uint dwFlags;
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.MONITORINFO monitorInfo);

        }
    }
}
