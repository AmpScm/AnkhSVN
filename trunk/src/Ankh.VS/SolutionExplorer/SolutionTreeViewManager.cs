﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Utils.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Ankh.Scc;

namespace Ankh.VS.SolutionExplorer
{
    class SolutionTreeViewManager
    {
        readonly IAnkhServiceProvider _environment;

        bool _useAnkhIcons;
        IntPtr _originalImageList;

        Win32TreeView _treeViewControl;

        public SolutionTreeViewManager(IAnkhServiceProvider environment)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");

            _environment = environment;            
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

            EnvDTE.Window window = Microsoft.VisualStudio.Shell.VsShellUtilities.GetWindowObject(solutionExplorerWindow.SolutionExplorerFrame);

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

                if (handle == null)
                    handle = SearchFloatingPalettes(expCaption);
            }

            if (handle == IntPtr.Zero)
                return; // Not found :(

            IntPtr uiHierarchy = Win32.FindWindowEx(handle, IntPtr.Zero,
                UIHIERARCHY, null);
            IntPtr treeHwnd = Win32.FindWindowEx(uiHierarchy, IntPtr.Zero, TREEVIEW,
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
            IntPtr floatingPalette = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, VBFLOATINGPALETTE, null);
            while (floatingPalette != IntPtr.Zero)
            {
                IntPtr slnExplorer = this.SearchForSolutionExplorer(floatingPalette, slnExplorerCaption);
                if (slnExplorer != IntPtr.Zero)
                {
                    return slnExplorer;
                }
                floatingPalette = Win32.FindWindowEx(IntPtr.Zero, floatingPalette, VBFLOATINGPALETTE, null);
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
            IntPtr solutionExplorer = Win32.FindWindowEx(parent, IntPtr.Zero, GENERICPANE, caption);
            if (solutionExplorer != IntPtr.Zero)
                return solutionExplorer;

            IntPtr win = Win32.FindWindowEx(parent, IntPtr.Zero, null, null);
            while (win != IntPtr.Zero)
            {
                solutionExplorer = SearchForSolutionExplorer(win, caption);
                if (solutionExplorer != IntPtr.Zero)
                {
                    return solutionExplorer;
                }
                win = Win32.FindWindowEx(parent, win, null, null);
            }

            return IntPtr.Zero;
        }
        IStatusImageMapper _statusImageMapper;
        IStatusImageMapper StatusImageMapper
        {
            get { return _statusImageMapper ?? (_statusImageMapper = _environment.GetService<IStatusImageMapper>()); }
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
    }
}
