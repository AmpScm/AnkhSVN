using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.WpfServices.UI
{
    public class VSWpfDialog : VSWpfDialogBase
    {
        private string helpTopic;
        public VSWpfDialog()
        {
        }
        public VSWpfDialog(string helpTopic)
        {
            if (helpTopic == null)
            {
                throw new ArgumentNullException("helpTopic");
            }
            this.helpTopic = helpTopic;
            base.HasHelpButton = true;
        }

        public bool? ShowModal(IAnkhServiceProvider context)
        {
            int num = WindowHelper.ShowModal(context, this, IntPtr.Zero);
            if (num == 0)
            {
                return null;
            }
            return new bool?(num == 1);
        }

        static class WindowHelper
        {
            // Microsoft.Internal.VisualStudio.PlatformUI.WindowHelper
            public static int ShowModal(IAnkhServiceProvider context, Window window, IntPtr parent)
            {
                if (window == null)
                    throw new ArgumentNullException("window");

                IVsUIShell vsUIShell = context.GetService<IVsUIShell>(typeof(SVsUIShell));
                if (vsUIShell == null)
                {
                    throw new COMException("Can't get UI shell", -2147467259);
                }

                int hr = vsUIShell.GetDialogOwnerHwnd(out parent);
                if (hr != 0)
                {
                    throw new COMException("Can't fetch dialog owner", hr);
                }

                hr = vsUIShell.EnableModeless(0);
                if (hr != 0)
                {
                    throw new COMException("Can't enter modal mode", hr);
                }

                int result;
                try
                {
                    WindowInteropHelper helper = new WindowInteropHelper(window);
                    helper.Owner = parent;
                    if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
                    {
                        window.SourceInitialized += delegate(object param0, EventArgs param1)
                        {
                            NativeMethods.RECT parentRect = default(NativeMethods.RECT);
                            if (NativeMethods.GetWindowRect(parent, out parentRect))
                            {
                                HwndSource hwndSource = HwndSource.FromHwnd(helper.Handle);
                                if (hwndSource != null)
                                {
                                    Point point = hwndSource.CompositionTarget.TransformToDevice.Transform(new Point(window.ActualWidth, window.ActualHeight));
                                    NativeMethods.RECT rECT = WindowHelper.CenterRectOnSingleMonitor(parentRect, (int)point.X, (int)point.Y);
                                    Point point2 = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point((double)rECT.Left, (double)rECT.Top));
                                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                                    window.Left = point2.X;
                                    window.Top = point2.Y;
                                }
                            }
                        };
                    }
                    bool? flag = window.ShowDialog();
                    result = (flag.HasValue ? (flag.Value ? 1 : 2) : 0);
                }
                finally
                {
                    vsUIShell.EnableModeless(1);
                }
                return result;
            }

            private static NativeMethods.RECT CenterRectOnSingleMonitor(NativeMethods.RECT parentRect, int childWidth, int childHeight)
            {
                NativeMethods.RECT parentRect2;
                NativeMethods.RECT monitorClippingRect;
                NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out parentRect2, out monitorClippingRect);
                return WindowHelper.CenterInRect(parentRect2, childWidth, childHeight, monitorClippingRect);
            }

            private static NativeMethods.RECT CenterInRect(NativeMethods.RECT parentRect, int childWidth, int childHeight, NativeMethods.RECT monitorClippingRect)
            {
                NativeMethods.RECT result = default(NativeMethods.RECT);
                result.Left = parentRect.Left + (parentRect.Width - childWidth) / 2;
                result.Top = parentRect.Top + (parentRect.Height - childHeight) / 2;
                result.Width = childWidth;
                result.Height = childHeight;
                result.Left = Math.Min(result.Right, monitorClippingRect.Right) - result.Width;
                result.Top = Math.Min(result.Bottom, monitorClippingRect.Bottom) - result.Height;
                result.Left = Math.Max(result.Left, monitorClippingRect.Left);
                result.Top = Math.Max(result.Top, monitorClippingRect.Top);
                return result;
            }

        }

        protected override void InvokeDialogHelp()
        {
            throw new NotImplementedException();
        }
    }
}
