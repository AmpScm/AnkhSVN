// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
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
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Ankh.VS.SolutionExplorer
{
    /// <summary>
    /// Wraps a HWND to a Win32 treeview
    /// </summary>
    class Win32TreeView : NativeWindow
    {
        readonly IntPtr _hwnd;
        IntPtr _statusImageList;

        public Win32TreeView(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentNullException("hwnd");

            _hwnd = hwnd;
        }

        public bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Whether status image list messages should be suppressed.
        /// </summary>
        public bool SuppressStatusImageChange
        {
            get
            {
                return this.Handle != IntPtr.Zero;
            }
            set
            {
                if ((this.Handle != IntPtr.Zero) == value)
                    return;

                if (value)
                    this.AssignHandle(_hwnd);
                else
                    this.ReleaseHandle();
            }
        }

        /// <summary>
        /// Sets the imagelist to be used for the status images.
        /// </summary>
        public IntPtr StatusImageList
        {
            get
            {
                _statusImageList = (IntPtr)NativeMethods.SendMessage(_hwnd, NativeMethods.TVM_GETIMAGELIST,
                    NativeMethods.TVSIL_STATE, IntPtr.Zero);
                return _statusImageList;
            }
            set
            {
                _statusImageList = value;
                NativeMethods.SendMessage(_hwnd, NativeMethods.TVM_SETIMAGELIST, NativeMethods.TVSIL_STATE,
                    value);
            }
        }     

        /// <summary>
        /// Use this to suppress the TVM_SETIMAGELIST message if it is for the
        /// status image list.
        /// </summary>
        /// <param name="m"></param>
        [DebuggerNonUserCode]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)NativeMethods.TVM_SETIMAGELIST && m.WParam == NativeMethods.TVSIL_STATE)
            {
                return;
            }

            // When you see a crash here; it probably is not in AnkhSVN itself
            // but at somebody else handling events raised from the solution explorer
            base.WndProc(ref m);
        }

        static class NativeMethods
        {
            /// <summary>
            /// Sets the normal or state image list for a tree-view control 
            /// and redraws the control using the new images. 
            /// </summary>
            /// <param name="wParam">Type of image list to set. 
            /// This parameter can be one of the following values: TVSIL_NORMAL or TVSIL_STATE</param>; 
            /// <param name="lParam">Handle to the image list. If himl is NULL, the message 
            /// removes the specified image list from the tree-view control.</param>         /// 
            /// <return>Returns the handle to the previous image list, if any, or NULL otherwise.</return>
            public const uint TVM_SETIMAGELIST = 0x1109;

            /// <summary>
            /// Retrieves the handle to the normal 
            /// or state image list associated with a tree-view control. 
            /// </summary>
            public const uint TVM_GETIMAGELIST = 4360;

            /// <summary>
            /// Sets an imagelist for a listview.
            /// </summary>
            public const uint LVM_SETIMAGELIST = 0x1003;

            public static readonly IntPtr TVSIL_STATE = (IntPtr)0x2;
            public static readonly IntPtr LVSIL_NORMAL = (IntPtr)0x0;

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}
