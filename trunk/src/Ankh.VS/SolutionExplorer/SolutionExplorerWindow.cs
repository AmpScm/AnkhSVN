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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Ankh.Commands;
using Ankh.Scc;

namespace Ankh.VS.SolutionExplorer
{
    interface IAnkhSolutionExplorerWindow { }
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IAnkhSolutionExplorerWindow), MaxVersion=VSInstance.VS2010)]
    sealed class SolutionExplorerWindow : AnkhService, IVsWindowFrameNotify, IVsWindowFrameNotify2, IAnkhSolutionExplorerWindow
    {
        readonly SolutionTreeViewManager _manager;
        
        IVsWindowFrame _solutionExplorer;
        IVsWindowFrame2 _solutionExplorer2;
        IVsUIHierarchyWindow _tree;
        
        uint _cookie;
        bool _hookImageList;

        public SolutionExplorerWindow(IAnkhServiceProvider context)
            : base(context)
        {
            if (!VSVersion.VS2012OrLater)
            {
                _hookImageList = true;
                _manager = new SolutionTreeViewManager(Context);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (_hookImageList)
            {
                AnkhServiceEvents ev = GetService<AnkhServiceEvents>();

                ev.SccProviderActivated += OnSccProviderActivated;
                ev.SccProviderDeactivated += OnSccProviderDeactivated;
                ev.GitSccProviderActivated += OnSccProviderActivated;
                ev.GitSccProviderDeactivated += OnSccProviderDeactivated;
                ev.SolutionClosed += OnSolutionClosed;
                ev.SolutionOpened += OnSolutionOpened;
            }
        }

        private void OnSccProviderActivated(object sender, EventArgs e)
        {
            // _manager is available or we wouldn't have hooked
            _manager.SetAnkhIcons(true);

            GetService<IAnkhCommandService>().PostIdleAction(MaybeEnsure);
        }

        private void OnSccProviderDeactivated(object sender, EventArgs e)
        {
            EnableAnkhIcons(false);
        }

        private void OnSolutionClosed(object sender, EventArgs e)
        {
            EnableAnkhIcons(false);
        }

        private void OnSolutionOpened(object sender, EventArgs e)
        {
            if (GetService<IAnkhSccService>().IsActive)
                EnableAnkhIcons(true);
        }

        void MaybeEnsure()
        {
            if (_hookImageList && SolutionExplorerFrame.IsVisible() == VSErr.S_OK)
                _manager.Ensure(this);
        }

        void OnUIShellActivate(object sender, EventArgs e)
        {
            IAnkhCommandStates states = Context.GetService<IAnkhCommandStates>();

            if (states != null && states.SccProviderActive)
                MaybeEnsure();
        }

        public void Dispose()
        {
            if (_hookImageList && _solutionExplorer2 != null)
            {
                try
                {
                    _solutionExplorer2.Unadvise(_cookie);
                }
                catch { /* Probably unclean shutdown */ }
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

                            if (_hookImageList && solutionExplorer2 != null)
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

        int IVsWindowFrameNotify.OnDockableChange(int fDockable)
        {
            return VSErr.S_OK;
        }

        int IVsWindowFrameNotify.OnMove()
        {
            return VSErr.S_OK;
        }

        int IVsWindowFrameNotify.OnShow(int fShow)
        {
            _manager.Ensure(this);
            return VSErr.S_OK;
        }

        int IVsWindowFrameNotify.OnSize()
        {
            _manager.Ensure(this);
            return VSErr.S_OK;
        }

        int IVsWindowFrameNotify2.OnClose(ref uint pgrfSaveOptions)
        {
            return VSErr.S_OK;
        }

        #region IAnkhSolutionExplorerToolWindow Members
        public void EnableAnkhIcons(bool enabled)
        {
            if (_hookImageList)
            {
                _manager.Ensure(this);
                _manager.SetAnkhIcons(enabled);
            }
        }

        #endregion
    }
}
