// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;

using Ankh.Scc;
using Ankh.UI;

namespace Ankh.VS.SolutionExplorer
{
    sealed class SolutionTreeViewManager : AnkhService
    {
        bool _useAnkhIcons;
        IntPtr _originalImageList;

        Win32TreeView _treeViewControl;

        public SolutionTreeViewManager(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public Win32TreeView TreeView
        {
            get { return _treeViewControl; }
        }

        public void Ensure(SolutionExplorerWindow solutionExplorerWindow)
        {
            if (solutionExplorerWindow == null)
                throw new ArgumentNullException("solutionExplorerWindow");

            EnsureControl(solutionExplorerWindow);

            if (_useAnkhIcons)
                SetIcons();
        }

        private void EnsureControl(SolutionExplorerWindow solutionExplorerWindow)
        {
            if (_treeViewControl != null && _treeViewControl.IsValid)
                return;

            EnvDTE.Window window;
            try
            {
                window = VsShellUtilities.GetWindowObject(solutionExplorerWindow.SolutionExplorerFrame);
            }
            catch (COMException)
            {
                // for VS2010 - WPF Shell compatibility (we cannot find the solutionexplorer frame there)
                return;
            }

            string expCaption = window.Caption;

            IntPtr handle = IntPtr.Zero;

            if (window.HWnd != 0)
            {
                // We've got the parent
                handle = (IntPtr)window.HWnd;
            }
            else
            {
                EnvDTE.Window hostWindow = window.LinkedWindowFrame;

                if (hostWindow != null)
                    handle = SearchForSolutionExplorer((IntPtr)hostWindow.HWnd, expCaption);

                if (handle == IntPtr.Zero)
                {
                    hostWindow = window.DTE.MainWindow;

                    if (hostWindow != null)
                        handle = SearchForSolutionExplorer((IntPtr)hostWindow.HWnd, expCaption);
                }

                if (handle == IntPtr.Zero)
                    handle = SearchFloatingPalettes(expCaption);
            }

            if (handle == IntPtr.Zero)
                return; // Not found :(

            IntPtr uiHierarchy = NativeMethods.FindWindowEx(handle, IntPtr.Zero,
                UIHIERARCHY, null);
            IntPtr treeHwnd = NativeMethods.FindWindowEx(uiHierarchy, IntPtr.Zero, TREEVIEW,
                null);

            if (treeHwnd == IntPtr.Zero)
                return;

            this._treeViewControl = new Win32TreeView(treeHwnd);
        }


        /// <summary>
        /// Searches floating palettes for the solution explorer window.
        /// </summary>
        /// <param name="slnExplorerCaption"></param>
        /// <returns></returns>
        private IntPtr SearchFloatingPalettes(string slnExplorerCaption)
        {
            IntPtr floatingPalette = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, VBFLOATINGPALETTE, null);
            while (floatingPalette != IntPtr.Zero)
            {
                IntPtr slnExplorer = this.SearchForSolutionExplorer(floatingPalette, slnExplorerCaption);
                if (slnExplorer != IntPtr.Zero)
                {
                    return slnExplorer;
                }
                floatingPalette = NativeMethods.FindWindowEx(IntPtr.Zero, floatingPalette, VBFLOATINGPALETTE, null);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Searches recursively for the solution explorer window.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        private IntPtr SearchForSolutionExplorer(IntPtr parent, string caption)
        {
            // is it directly under the parent?
            IntPtr solutionExplorer = NativeMethods.FindWindowEx(parent, IntPtr.Zero, GENERICPANE, caption);
            if (solutionExplorer != IntPtr.Zero)
                return solutionExplorer;

            IntPtr win = NativeMethods.FindWindowEx(parent, IntPtr.Zero, null, null);
            while (win != IntPtr.Zero)
            {
                solutionExplorer = SearchForSolutionExplorer(win, caption);
                if (solutionExplorer != IntPtr.Zero)
                {
                    return solutionExplorer;
                }
                win = NativeMethods.FindWindowEx(parent, win, null, null);
            }

            return IntPtr.Zero;
        }
        IStatusImageMapper _statusImageMapper;
        IStatusImageMapper StatusImageMapper
        {
            get { return _statusImageMapper ?? (_statusImageMapper = GetService<IStatusImageMapper>()); }
        }

        void SetIcons()
        {
            if (TreeView == null)
                return; // Nothing to do

            // store the original image list (check that we're not storing our own statusImageList
            if (StatusImageMapper.StatusImageList.Handle != TreeView.StatusImageList)
                _originalImageList = TreeView.StatusImageList;

            // and assign the status image list to the tree
            TreeView.StatusImageList = StatusImageMapper.StatusImageList.Handle;
            TreeView.SuppressStatusImageChange = true;
        }

        void RestoreIcons()
        {
            if (TreeView == null)
                return; // Nothing to do

            // if someone wants VSS images now, let them.
            TreeView.SuppressStatusImageChange = false;

            if (_originalImageList != IntPtr.Zero)
            {
                TreeView.StatusImageList = _originalImageList;
                _originalImageList = IntPtr.Zero;
            }
        }

        public void SetAnkhIcons(bool enabled)
        {
            if (enabled == _useAnkhIcons)
            {
                if (enabled)
                    SetIcons();

                return;
            }

            _useAnkhIcons = enabled;
            if (enabled)
                SetIcons();
            else
                RestoreIcons();
        }

        const string GENERICPANE = "GenericPane";
        const string VSAUTOHIDE = "VsAutoHide";
        const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        const string TREEVIEW = "SysTreeView32";
        const string VBFLOATINGPALETTE = "VBFloatingPalette";

        static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr afterChild, string className,
                string windowName);
        }
    }
}
