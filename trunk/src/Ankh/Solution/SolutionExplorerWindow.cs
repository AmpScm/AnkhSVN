using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;

namespace Ankh.Solution
{
    interface IAnkhSolutionExplorerWindow
    {
        void Show();

        void EnableAnkhIcons(bool enabled);
    }

    /// <summary>
    /// 
    /// </summary>
    class SolutionExplorerWindow : IVsWindowFrameNotify, IVsWindowFrameNotify2, IDisposable, IAnkhSolutionExplorerWindow
    {
        ISynchronizeInvoke _synchronizer;
        IServiceProvider _environment;
        IVsWindowFrame _solutionExplorer;
        IVsWindowFrame2 _solutionExplorer2;
        IVsUIHierarchyWindow _tree;
        SolutionTreeViewManager _manager;
        uint _cookie;

        public SolutionExplorerWindow(IServiceProvider environment, ISynchronizeInvoke synchronizer)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");

            _environment = environment;
            _synchronizer = synchronizer;
            _manager = new SolutionTreeViewManager();

            if (SolutionExplorerFrame.IsVisible() == VSConstants.S_OK)
                _manager.Ensure(this);
        }

        public void Dispose()
        {
            if (_solutionExplorer2 != null)
            {
                _solutionExplorer2.Unadvise(_cookie);
                _solutionExplorer2 = null;
            }

            _environment = null;
            _solutionExplorer = null;
            _solutionExplorer2 = null;
            _tree = null;
        }

        public IVsWindowFrame SolutionExplorerFrame
        {
            get
            {
                if (_solutionExplorer == null)
                {
                    IVsUIShell shell = (IVsUIShell)_environment.GetService(typeof(SVsUIShell));

                    Debug.Assert(shell != null); // Must be true

                    if (shell != null)
                    {
                        IVsWindowFrame solutionExplorer;
                        Guid solutionExplorerGuid = new Guid("3ae79031-e1bc-11d0-8f78-00a0c9110057"); // GUID_SolutionExplorer from vsshell.h

                        Marshal.ThrowExceptionForHR(shell.FindToolWindow(0, ref solutionExplorerGuid, out solutionExplorer));

                        if (solutionExplorer != null)
                        {
                            _solutionExplorer = solutionExplorer;
                            IVsWindowFrame2 solutionExplorer2 = solutionExplorer as IVsWindowFrame2;

                            if (solutionExplorer2 != null)
                            {
                                uint cookie;
                                Marshal.ThrowExceptionForHR(solutionExplorer2.Advise(this, out cookie));
                                _cookie = cookie;
                                _solutionExplorer2 = solutionExplorer2;
                            }
                        }
                    }
                }
                return _solutionExplorer;
            }
        }
    

        public IVsUIHierarchyWindow TreeWindow
        {
            get
            {
                if (_tree == null)
                {
                    IVsWindowFrame frame = SolutionExplorerFrame;

                    if (frame != null)
                    {
                        object pvar = null;
                        Marshal.ThrowExceptionForHR(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out pvar));

                        _tree = pvar as IVsUIHierarchyWindow;
                    }
                }
                return _tree;
            }
        }        

        public int OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        public int OnMove()
        {
            return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            _manager.Ensure(this);
            return VSConstants.S_OK;
        }

        public int OnSize()
        {
            _manager.Ensure(this);
            return VSConstants.S_OK;
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
            return VSConstants.S_OK;
        }

        public void Show()
        {
            if (SolutionExplorerFrame != null)
                Marshal.ThrowExceptionForHR(SolutionExplorerFrame.Show());
        }

        #region IAnkhSolutionExplorerToolWindow Members


        public void EnableAnkhIcons(bool enabled)
        {
            _manager.Ensure(this);

            _manager.SetAnkhIcons(enabled);
        }

        #endregion
    }
}
