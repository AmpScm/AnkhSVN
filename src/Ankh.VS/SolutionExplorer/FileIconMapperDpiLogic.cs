// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ankh.VS.SolutionExplorer
{
    internal static class FileIconMapperDpiLogic
    {
        internal const int DefaultDpi = 96;

        internal static int GetPixelSize(int logicalSize, int dpi)
        {
            if (logicalSize <= 0)
                throw new ArgumentOutOfRangeException("logicalSize");

            if (dpi <= 0)
                dpi = DefaultDpi;

            return Math.Max(
                1,
                (int)Math.Round(
                    logicalSize * dpi / (double)DefaultDpi,
                    MidpointRounding.AwayFromZero));
        }

        internal static int GetCurrentDpi()
        {
            try
            {
                IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
                if (hwnd != IntPtr.Zero)
                {
                    uint dpi = NativeMethods.GetDpiForWindow(hwnd);
                    if (dpi > 0)
                        return unchecked((int)dpi);
                }
            }
            catch (EntryPointNotFoundException)
            {
            }
            catch (DllNotFoundException)
            {
            }

            try
            {
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    int dpi = (int)Math.Round(graphics.DpiX);
                    return dpi > 0 ? dpi : DefaultDpi;
                }
            }
            catch
            {
                return DefaultDpi;
            }
        }

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern uint GetDpiForWindow(IntPtr hwnd);
        }
    }
}
