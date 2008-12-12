// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Ankh.VS;

namespace Ankh.VS.SolutionExplorer
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IAnkhSolutionExplorerWindow))]
    sealed class SolutionExplorerWindow : AnkhService, IVsWindowFrameNotify, IVsWindowFrameNotify2, IDisposable, IAnkhSolutionExplorerWindow
    {
        readonly SolutionTreeViewManager _manager;
        
        IVsWindowFrame _solutionExplorer;
        IVsWindowFrame2 _solutionExplorer2;
        IVsUIHierarchyWindow _tree;
        
        uint _cookie;

        public SolutionExplorerWindow(IAnkhServiceProvider context)
            : base(context)
        {
            _manager = new SolutionTreeViewManager(Context);
        }

        internal void Initialize()
        {
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
                    IVsUIShell shell = GetService<IVsUIShell>(typeof(SVsUIShell));

                    Debug.Assert(shell != null); // Must be true

                    if (shell != null)
                    {
                        IVsWindowFrame solutionExplorer;
                        Guid solutionExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);

                        Marshal.ThrowExceptionForHR(shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorerGuid, out solutionExplorer));

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
    

        public IVsUIHierarchyWindow HierarchyWindow
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
