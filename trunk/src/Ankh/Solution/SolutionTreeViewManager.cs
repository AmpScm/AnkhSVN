using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.Solution
{
    class SolutionTreeViewManager
    {
        Win32TreeView _treeViewControl;

        public void Ensure(SolutionExplorerWindow solutionExplorerWindow)
        {
            if (solutionExplorerWindow == null)
                throw new ArgumentNullException("solutionExplorerWindow");

            EnsureControl(solutionExplorerWindow);
        }

        private void EnsureControl(SolutionExplorerWindow solutionExplorerWindow)
        {
            if(_treeViewControl != null && _treeViewControl.Handle != IntPtr.Zero)
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

                if(hostWindow != null)
                    handle = SearchForSolutionExplorer((IntPtr)hostWindow.HWnd, expCaption);

                if(handle == null)
                {
                    hostWindow = window.DTE.MainWindow;

                    if(hostWindow != null)
                        handle = SearchForSolutionExplorer((IntPtr)hostWindow.HWnd, expCaption);
                }

                if(handle == null)
                    handle = SearchFloatingPalettes(expCaption);
            }

            if(handle == IntPtr.Zero)
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

        private const string VSNETWINDOW = "wndclass_desked_gsk";
        private const string GENERICPANE = "GenericPane";
        private const string VSAUTOHIDE = "VsAutoHide";
        private const string UIHIERARCHY = "VsUIHierarchyBaseWin";
        private const string TREEVIEW = "SysTreeView32";
        private const string VBFLOATINGPALETTE = "VBFloatingPalette";

        internal void SetAnkhIcons(bool enabled)
        {
            //throw new NotImplementedException();
        }
    }
}
