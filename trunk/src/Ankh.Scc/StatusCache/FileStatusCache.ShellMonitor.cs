// $Id$
//
// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Ankh.Scc.StatusCache
{
    partial class FileStatusCache
    {
        NotifyWindow _notifyWindow;
        uint _notifyCookie;
        static readonly bool _preVista = (Environment.OSVersion.Version < new Version(6, 0));

        void InitializeShellMonitor()
        {
            NotifyWindow nw = new NotifyWindow(this);
            uint cookie = 0;
            try
            {
                NativeMethods.SHChangeNotifyEntry what;
                what.pIdl = IntPtr.Zero;
                what.Recursively = true;

                // We are only interested in Shell events. We let VS handle the real filesystem changes

                // TortoiseSVN has to notify the shell for all his updates, and we just listen to what TortoiseSVN sends us :)

                cookie = NativeMethods.SHChangeNotifyRegister(nw.Handle, 0x0002 | 0x8000, NativeMethods.SHCNE.SHCNE_ALLEVENTS, 0xDEDE, 1, ref what);
            }
            catch(Exception e)
            {
                GC.KeepAlive(e);
                if (nw != null)
                {
                    nw.Dispose();
                    nw = null;
                }
            }
            finally
            {
                _notifyWindow = nw;
                _notifyCookie = cookie;
            }
        }

        void ReleaseShellMonitor(bool disposing)
        {
            if (_notifyCookie != 0)
            {
                NativeMethods.SHChangeNotifyUnregister(_notifyCookie);
                _notifyCookie = 0;
            }
            if (disposing && _notifyWindow != null)
            {
                _notifyWindow.Dispose();
            }
        }

        private void OnShellChange(int lEvent, int process, string path1, string path2)
        {
            lock (_lock)
            {
                SvnItem item;

                if (!string.IsNullOrEmpty(path1) && _map.TryGetValue(path1, out item))
                    item.MarkDirty();

                if (!string.IsNullOrEmpty(path2) && _map.TryGetValue(path2, out item))
                    item.MarkDirty();
            }
        }

        static bool SHGetPathFromIDList(IntPtr pidl, StringBuilder sb)
        {
            if (_preVista)
                return NativeMethods.SHGetPathFromIDList(pidl, sb);
            else
                return NativeMethods.SHGetPathFromIDListEx(pidl, sb, sb.Length - 1, NativeMethods.GPFIDL_FLAGS.GPFIDL_DEFAULT);
        }

        void OnShellChange(int lEvent, IntPtr pidl1, IntPtr pidl2, int process)
        {
            if (pidl1 == IntPtr.Zero && pidl2 == IntPtr.Zero)
                return;

            int maxLen = (_preVista ? 260 : 512) + 10;
            StringBuilder pathBuilder = new StringBuilder(maxLen);
            string path1, path2;
            bool haveOne = false;
            
            pathBuilder.Append('\0', pathBuilder.Capacity - 5);
            if(pidl1 != IntPtr.Zero && NativeMethods.SHGetPathFromIDList(pidl1, pathBuilder) && pathBuilder.Length > 0)
            {
                path1 = pathBuilder.ToString();
                haveOne = true;
            }
            else
                path1 = null;

            pathBuilder.Length = 0;
            pathBuilder.Append('\0', pathBuilder.Capacity - 5);

            if (pidl2 != IntPtr.Zero && NativeMethods.SHGetPathFromIDList(pidl2, pathBuilder) && pathBuilder.Length > 0)
            {
                path2 = pathBuilder.ToString();
                haveOne = true;
            }
            else
                path2 = null;

            if (haveOne)
                OnShellChange(lEvent, process, path1, path2);
        }

        static class NativeMethods
        {
            // Most of these imports are publicly available since Windows XP SP2, but were already available since NT 4 via a numeric import

            [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#2", CharSet = CharSet.Auto)]
            public static extern UInt32 SHChangeNotifyRegister(IntPtr hWnd, int fSources, SHCNE fEvents, uint wMsg, int cEntries, ref SHChangeNotifyEntry pFsne);

            [DllImport("shell32.dll", EntryPoint = "#4", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean SHChangeNotifyUnregister(ulong hNotify);

            [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#644", CharSet = CharSet.Auto)]
            public static extern IntPtr SHChangeNotification_Lock(IntPtr hChange, IntPtr dwProcId, out IntPtr pidlList, ref int plEvent);

            [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#645", CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SHChangeNotification_Unlock(IntPtr hLock);

            [DllImport("shell32.dll", SetLastError = false, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

            // SHGetPathFromIDListEx is available on Vista+
            [DllImport("shell32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SHGetPathFromIDListEx(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath, int cchPath, GPFIDL_FLAGS uOpts);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHChangeNotifyEntry
            {
                public IntPtr pIdl;
                [MarshalAs(UnmanagedType.Bool)]
                public Boolean Recursively;
            }

            [Flags]
            public enum SHCNE
            {
                SHCNE_RENAMEITEM = 0x00000001,
                SHCNE_CREATE = 0x00000002,
                SHCNE_DELETE = 0x00000004,
                SHCNE_MKDIR = 0x00000008,
                SHCNE_RMDIR = 0x00000010,
                SHCNE_MEDIAINSERTED = 0x00000020,
                SHCNE_MEDIAREMOVED = 0x00000040,
                SHCNE_DRIVEREMOVED = 0x00000080,
                SHCNE_DRIVEADD = 0x00000100,
                SHCNE_NETSHARE = 0x00000200,
                SHCNE_NETUNSHARE = 0x00000400,
                SHCNE_ATTRIBUTES = 0x00000800,
                SHCNE_UPDATEDIR = 0x00001000,
                SHCNE_UPDATEITEM = 0x00002000,
                SHCNE_SERVERDISCONNECT = 0x00004000,
                SHCNE_UPDATEIMAGE = 0x00008000,
                SHCNE_DRIVEADDGUI = 0x00010000,
                SHCNE_RENAMEFOLDER = 0x00020000,
                SHCNE_FREESPACE = 0x00040000,
                SHCNE_EXTENDED_EVENT = 0x04000000,
                SHCNE_ASSOCCHANGED = 0x08000000,
                SHCNE_DISKEVENTS = 0x0002381F,
                SHCNE_GLOBALEVENTS = 0x0C0581E0,
                SHCNE_ALLEVENTS = 0x7FFFFFFF,
                SHCNE_INTERRUPT = unchecked((int)0x80000000)
            }
            public enum GPFIDL_FLAGS
            {
                GPFIDL_DEFAULT = 0x0000,
                GPFIDL_ALTNAME = 0x0001,
                GPFIDL_UNCPRINTER = 0x0002
            }
        }

        sealed class NotifyWindow : NativeWindow, IDisposable
        {
            readonly FileStatusCache _cache;
            public NotifyWindow(FileStatusCache cache)
            {
                _cache = cache;
                CreateParams cp = new CreateParams();
                CreateHandle(cp);
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (this.Handle != IntPtr.Zero)
                    DestroyHandle();
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == 0xDEDE)
                {
                    IntPtr pidls;
                    int lEvent = 0;
                    IntPtr handle = NativeMethods.SHChangeNotification_Lock(m.WParam, m.LParam, out pidls, ref lEvent);
                    if(handle != IntPtr.Zero)
                        try
                        {
                            _cache.OnShellChange(lEvent, Marshal.ReadIntPtr(pidls,0), Marshal.ReadIntPtr(pidls, IntPtr.Size), (int)m.LParam);
                        }
                        catch
                        {}
                        finally
                        {
                            NativeMethods.SHChangeNotification_Unlock(handle);
                        }
                }
            }

            #endregion
        }
    }
}
