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
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Ankh.Selection;
using SharpSvn;

namespace Ankh.VS.Selection
{
	partial class SelectionContext
	{
        IVsWindowFrame _activeFrame;
        object _activeFrameObject;
        Control _activeFrameControl;

        IVsWindowFrame _activeDocumentFrame;
        object _activeDocumentFrameObject;
        Control _activeDocumentControl;

        IVsUserContext _userContext;

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            switch ((VSConstants.VSSELELEMID)elementid)
            {
                case VSConstants.VSSELELEMID.SEID_WindowFrame:
                    _activeFrameObject = _activeFrameControl = null;
                    _activeFrame = varValueNew as IVsWindowFrame;
                    break;
                case VSConstants.VSSELELEMID.SEID_DocumentFrame:
                    _activeDocumentFrameObject = _activeDocumentControl = null;
                    _activeDocumentFrame = varValueNew as IVsWindowFrame;
                    break;
                case VSConstants.VSSELELEMID.SEID_UserContext:
                    _userContext = varValueNew as IVsUserContext;
                    break;
#if NEVER
                case VSConstants.VSSELELEMID.SEID_PropertyBrowserSID:
                    IVsPropertyBrowser pb = varValueNew as IVsPropertyBrowser;
                    break;
                case VSConstants.VSSELELEMID.SEID_ResultList:
                    IOleCommandTarget ct = varValueNew as IOleCommandTarget;
                    break;
                case VSConstants.VSSELELEMID.SEID_StartupProject:
                    IVsProject2 pr = varValueNew as IVsProject2;
                    break;
                case VSConstants.VSSELELEMID.SEID_UndoManager:
                    IOleUndoManager ud = varValueNew as IOleUndoManager;
                    break;                
#endif
            }
            // Some property changed
            return VSConstants.S_OK;
        }

        #region ISelectionContextEx Members

        public IVsWindowFrame ActiveFrame
        {
            get { return _activeFrame; }
        }

        public IVsWindowFrame ActiveDocumentFrame
        {
            get { return _activeDocumentFrame; }
        }

        public IVsUserContext UserContext
        {
            get { return _userContext; }
        }

        #endregion

        #region ISelectionContext Members

        object ActiveFrameObject
        {
            get
            {
                if (_activeFrameObject == null && ActiveFrame != null)
                {
                    object v;
                    if (ErrorHandler.Succeeded(ActiveFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out v)))
                    {
                        _activeFrameObject = v;
                    }
                }

                return _activeFrameObject;
            }
        }

        public Control ActiveFrameControl
        {
            get { return _activeFrameControl ?? (_activeFrameControl = FindControl(ActiveFrameObject)); }
        }

        private Control FindControl(object frameObject)
        {
            if (frameObject == null)
                return null;

            Microsoft.VisualStudio.Shell.WindowPane twp = frameObject as Microsoft.VisualStudio.Shell.WindowPane;

            if (twp != null)
            {
                return twp.Window as Control;
            }

            return null;
        }

        object ActiveDocumentFrameObject
        {
            get 
            {
                if (_activeDocumentFrameObject == null && ActiveDocumentFrame != null)
                {
                    object v;
                    if (ErrorHandler.Succeeded(ActiveDocumentFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out v)))
                    {
                        _activeFrameObject = v;
                    }
                }

                return _activeDocumentFrameObject;
            }
        }

        public Control ActiveDocumentFrameControl
        {
            get { return _activeDocumentControl ?? (_activeDocumentControl = FindControl(ActiveDocumentFrameObject)); }
        }

        public string ActiveDocumentFilename
        {
            get 
            {
                if (_activeDocumentFrame != null)
                {
                    object value;
                    string path;
                    if (ErrorHandler.Succeeded(_activeDocumentFrame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out value))
                        && null != (path = value as string))
                    {
                        if (SvnItem.IsValidPath(path))
                            return SvnTools.GetNormalizedFullPath(path);                        
                    }
                }
                return null;
            }
        }

        public SvnItem ActiveDocumentItem 
        {
            get
            {
                string p = ActiveDocumentFilename;

                if (p != null)
                    return _cache[p];
                else
                    return null;
            }
        }

        bool _shouldRefresh;
        IVsHierarchy _hierCached;
        uint _idCached;
        IVsMultiItemSelect _misCached;
        ISelectionContainer _contCached;

        private void RefreshContext()
        {
            if(_shouldRefresh)
            {
                _shouldRefresh = false;
            
                // TODO: Perhaps get the global context again instead of returning cached values
                OnSelectionChanged(_currentHierarchy, _currentItem, _currentSelection, _currentContainer,
                    _hierCached, _idCached, _misCached, _contCached);

                _hierCached = null;
                _misCached = null;
                _contCached = null;
            }
        }

        #endregion

        readonly Stack<Control> _popups = new Stack<Control>();
        Control _topPopup;

        public class PopupDisposer : IDisposable
        {
            readonly SelectionContext _context;
            readonly Control _control;

            public PopupDisposer(SelectionContext context, Control control)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (control == null)
                    throw new ArgumentNullException("contrl");

                _context = context;
                _control = control;
            }

            public void Dispose()
            {
                if (_context._topPopup == _control)
                {
                    Stack<Control> _stack = _context._popups;
                    _stack.Pop();

                    if (_stack.Count > 0)
                        _context._topPopup = _context._popups.Peek();
                    else
                        _context._topPopup = null;

                    if(_context._topPopup == null)
                        _context.RefreshContext();
                }
            }
        }

        public IDisposable PushPopupContext(System.Windows.Forms.Control control)
        {
            if(control == null)
                throw new ArgumentNullException("control");

            if (!_shouldRefresh)
            {
                _shouldRefresh = true;
                _hierCached = _currentHierarchy;
                _idCached = _currentItem;
                _misCached = _currentSelection;
                _contCached = _currentContainer;
            }

            _popups.Push(control);
            _topPopup = control;

            return new PopupDisposer(this, control);
        }

        public System.Windows.Forms.Control ActiveDialog
        {
            get { return _topPopup; }
        }

        /// <summary>
        /// Gets the active dialog or frame control.
        /// </summary>
        /// <value>The active dialog or frame control.</value>
        public System.Windows.Forms.Control ActiveDialogOrFrameControl
        {
            get { return ActiveDialog ?? ActiveFrameControl; }
        }

        public TControl GetActiveControl<TControl>()
            where TControl : class
        {
            Control ctrl = ActiveDialogOrFrameControl;

            ContainerControl cc;

            while (null != (cc = ctrl as ContainerControl))
            {
                ctrl = cc.ActiveControl;

                if (ctrl == null || ctrl == cc)
                {
                    ctrl = cc;
                    break;
                }
            }

            if (ctrl == null)
                return null;

            while (ctrl != null)
            {
                TControl tc = ctrl as TControl;

                if (tc != null)
                    return tc;

                ctrl = ctrl.Parent;
            }

            return null;
        }

        IVsTrackSelectionEx ISelectionContextEx.GetModalTracker(Control control)
        {
            if (_topPopup != null && (control == null || _topPopup.Contains(control)))
                return new ModalSelection(this, _topPopup);
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        sealed class ModalSelection : IVsTrackSelectionEx
        {
            SelectionContext _ctx;
            Control _top;
            public ModalSelection(SelectionContext ctx, Control top)
            {
                if (ctx == null)
                    throw new ArgumentNullException("ctx");

                _ctx = ctx;
                _top = top;
            }

            #region IVsTrackSelectionEx Members

            public int GetCurrentSelection(out IntPtr ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS, out IntPtr ppSC)
            {
                ppHier = IntPtr.Zero; // Not used by our code
                pitemid = VSConstants.VSITEMID_NIL;
                ppMIS = null;
                ppSC = IntPtr.Zero;
                return VSConstants.S_OK;
            }

            public int IsMyHierarchyCurrent(out int pfCurrent)
            {
                pfCurrent = 0; // Not used by our code
                return VSConstants.S_OK;
            }

            public int OnElementValueChange(uint elementid, int fDontPropagate, object varValue)
            {
                // Not used by our code
                return VSConstants.S_OK;
            }

            public int OnSelectChange(ISelectionContainer pSC)
            {
                if (_ctx._topPopup == _top)
                {
                    _ctx.OnSelectionChanged(_ctx._currentHierarchy, _ctx._currentItem, _ctx._currentSelection, _ctx._currentContainer,
                        _ctx._currentHierarchy, _ctx._currentItem, _ctx._currentSelection, pSC);
                }

                return VSConstants.S_OK;                
            }

            public int OnSelectChangeEx(IntPtr pHier, uint itemid, IVsMultiItemSelect pMIS, IntPtr pSC)
            {
                if (_ctx._topPopup == _top)
                {
                    IVsHierarchy hier = (pHier != IntPtr.Zero) ? Marshal.GetObjectForIUnknown(pHier) as IVsHierarchy : null;
                    ISelectionContainer cont = (pSC != IntPtr.Zero) ? Marshal.GetObjectForIUnknown(pSC) as ISelectionContainer : null;
                    if (_ctx._topPopup == _top)
                    {
                        _ctx.OnSelectionChanged(_ctx._currentHierarchy, _ctx._currentItem, _ctx._currentSelection, _ctx._currentContainer,
                            hier, itemid, pMIS, cont);
                    }
                }

                if (pHier != IntPtr.Zero)
                    Marshal.Release(pHier);

                if (pSC != IntPtr.Zero)
                    Marshal.Release(pSC);

                return VSConstants.S_OK;
            }

            #endregion
        }

	}
}
