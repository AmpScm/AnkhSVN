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
using Microsoft.VisualStudio.TextManager.Interop;
using Ankh.UI;

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
        string _activeDocumentFileName;
        IVsTextView _activeFrameTextView;
        bool _determinedActiveFrameTextView;
        IVsTextView _activeDocumentFrameTextView;
        bool _determinedActiveDocumentFrameTextView;

        static Guid IID_IVsClassView = typeof(IVsClassView).GUID;

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            switch ((VSConstants.VSSELELEMID)elementid)
            {
                case VSConstants.VSSELELEMID.SEID_WindowFrame:
                    _activeFrameObject = _activeFrameControl = null;
                    _activeFrameTextView = null;
                    _determinedActiveFrameTextView = false;
                    _activeFrame = varValueNew as IVsWindowFrame;
                    break;
                case VSConstants.VSSELELEMID.SEID_DocumentFrame:
                    _activeDocumentFrameObject = _activeDocumentControl = null;
                    _activeDocumentFrameTextView = null;
                    _determinedActiveDocumentFrameTextView = false;
                    _activeDocumentFrame = varValueNew as IVsWindowFrame;
                    break;
#if NEVER
                case VSConstants.VSSELELEMID.SEID_UserContext:
                    IVsUserContext userContext = varValueNew as IVsUserContext;
                    break;
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

            _activeDocumentFileName = null; // Flush on every document state change
            ClearCache();

            return VSErr.S_OK;
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

        #endregion

        #region ISelectionContext Members

        object ActiveFrameObject
        {
            get
            {
                if (_activeFrameObject == null && ActiveFrame != null)
                {
                    object v;
                    if (VSErr.Succeeded(ActiveFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out v)))
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
                    if (VSErr.Succeeded(ActiveDocumentFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out v)))
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
                // ### The cached active document filename might be invalid if the document is saved to another name!
                if (_activeDocumentFileName != null)
                    return string.IsNullOrEmpty(_activeDocumentFileName) ? null : _activeDocumentFileName;

                _activeDocumentFileName = "";
                if (_activeDocumentFrame != null)
                {
                    object value;
                    string path;
                    if (VSErr.Succeeded(_activeDocumentFrame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out value))
                        && null != (path = value as string))
                    {
                        if (SvnItem.IsValidPath(path))
                            return _activeDocumentFileName = SvnTools.GetNormalizedFullPath(path);
                    }
                }
                return null;
            }
        }

        public SvnItem ActiveDocumentSvnItem
        {
            get
            {
                string p = ActiveDocumentFilename;

                if (p != null)
                    return SvnCache[p];
                else
                    return null;
            }
        }
        public IVsTextView ActiveFrameTextView
        {
            get
            {
                if (!_determinedActiveFrameTextView)
                {
                    _determinedActiveFrameTextView = true;
                    _activeFrameTextView = null;
                    IVsWindowFrame frame = ActiveFrame;

                    if (frame != null)
                    {
                        _activeFrameTextView = GetTextView(frame);

                        if (_activeFrameTextView == null)
                        {
                            IAnkhHasVsTextView hasView = ActiveFrameControl as IAnkhHasVsTextView;

                            if (hasView != null)
                                _activeFrameTextView = hasView.TextView;
                        }
                    }
                }

                return _activeFrameTextView;
            }
        }

        public IVsTextView ActiveDocumentFrameTextView
        {
            get
            {
                if (!_determinedActiveDocumentFrameTextView)
                {
                    _determinedActiveDocumentFrameTextView = true;
                    _activeDocumentFrameTextView = null;
                    IVsWindowFrame frame = ActiveDocumentFrame;

                    if (frame != null)
                    {
                        _activeDocumentFrameTextView = GetTextView(frame);

                        if (_activeDocumentFrameTextView == null)
                        {
                            IAnkhHasVsTextView hasView = ActiveDocumentFrameControl as IAnkhHasVsTextView;

                            if (hasView != null)
                                _activeFrameTextView = hasView.TextView;
                        }
                    }
                }

                return _activeDocumentFrameTextView;
            }
        }

        bool _shouldRefresh;
        IVsHierarchy _hierCached;
        uint _idCached;
        IVsMultiItemSelect _misCached;
        ISelectionContainer _contCached;

        private void RefreshContext()
        {
            if (_shouldRefresh)
            {
                _shouldRefresh = false;

                // TODO: Perhaps get the global context again instead of returning cached values
                OnSelectionChanged(current.hierarchy, current.id, current.selection, _currentContainer,
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

                    if (_context._topPopup == null)
                        _context.RefreshContext();
                }
            }
        }

        public IDisposable PushPopupContext(System.Windows.Forms.Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (!_shouldRefresh)
            {
                _shouldRefresh = true;
                _hierCached = current.hierarchy;
                _idCached = current.id;
                _misCached = current.selection;
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

            IContainerControl cc;

            while (null != (cc = ctrl as IContainerControl))
            {
                ctrl = cc.ActiveControl;

                if (ctrl == null || ctrl == cc)
                {
                    ctrl = cc as Control;
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
                ppHier = IntPtr.Zero;
                pitemid = VSItemId.Nil;
                ppMIS = null;
                ppSC = IntPtr.Zero;

                if (_ctx._topPopup == _top)
                {
                    if (_ctx.current.hierarchy != null)
                        ppHier = Marshal.GetIUnknownForObject(_ctx.current.hierarchy);
                    pitemid = _ctx.current.id;
                    ppMIS = _ctx.current.selection;
                    if (_ctx._currentContainer != null)
                        ppSC = Marshal.GetIUnknownForObject(_ctx._currentContainer);
                }
                return VSErr.S_OK;
            }

            public int IsMyHierarchyCurrent(out int pfCurrent)
            {
                pfCurrent = 0; // Not used by our code
                return VSErr.S_OK;
            }

            public int OnElementValueChange(uint elementid, int fDontPropagate, object varValue)
            {
                // Not used by our code
                return VSErr.S_OK;
            }

            public int OnSelectChange(ISelectionContainer pSC)
            {
                if (_ctx._topPopup == _top)
                {
                    _ctx.OnSelectionChanged(_ctx.current.hierarchy, _ctx.current.id, _ctx.current.selection, _ctx._currentContainer,
                        _ctx.current.hierarchy, _ctx.current.id, _ctx.current.selection, pSC);
                }

                return VSErr.S_OK;
            }

            public int OnSelectChangeEx(IntPtr pHier, uint itemid, IVsMultiItemSelect pMIS, IntPtr pSC)
            {
                if (_ctx._topPopup == _top)
                {
                    IVsHierarchy hier = (pHier != IntPtr.Zero) ? Marshal.GetObjectForIUnknown(pHier) as IVsHierarchy : null;
                    ISelectionContainer cont = (pSC != IntPtr.Zero) ? Marshal.GetObjectForIUnknown(pSC) as ISelectionContainer : null;
                    if (_ctx._topPopup == _top)
                    {
                        _ctx.OnSelectionChanged(_ctx.current.hierarchy, _ctx.current.id, _ctx.current.selection, _ctx._currentContainer,
                            hier, itemid, pMIS, cont);
                    }
                }

                if (pHier != IntPtr.Zero)
                    Marshal.Release(pHier);

                if (pSC != IntPtr.Zero)
                    Marshal.Release(pSC);

                return VSErr.S_OK;
            }

            #endregion
        }

    }
}
