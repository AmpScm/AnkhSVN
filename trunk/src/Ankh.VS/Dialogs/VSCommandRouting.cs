﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Forms;
using Ankh.UI;
using IMessageFilter = System.Windows.Forms.IMessageFilter;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Drawing;
using System.Runtime.InteropServices;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VS.Dialogs
{
    class VSCommandRouting : AnkhService, IMessageFilter, IDisposable, IVsToolWindowToolbar, IOleCommandTarget
    {
        readonly VSContainerForm _form;
        readonly IAnkhVSContainerForm _vsForm;
        readonly static Dictionary<VSContainerForm, VSCommandRouting> _map = new Dictionary<VSContainerForm, VSCommandRouting>();
        VSFormContainerPane _pane;
        IVsToolWindowToolbarHost _tbHost;
        IVsFilterKeys2 _fKeys;
        IVsRegisterPriorityCommandTarget _rPct;
        uint _csCookie;
        Panel _panel;
        List<IOleCommandTarget> _ctList;
        List<IVsWindowPane> _paneList;

        bool _installed;
        public VSCommandRouting(IAnkhServiceProvider context, VSContainerForm form)
            : base(context)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            _form = form;
            _vsForm = form;

            Application.AddMessageFilter(this);
            _installed = true;
            _map.Add(form, this);

            _rPct = GetService<IVsRegisterPriorityCommandTarget>(typeof(SVsRegisterPriorityCommandTarget));

            if(_rPct != null)
            {
                Marshal.ThrowExceptionForHR(_rPct.RegisterPriorityCommandTarget(0, this, out _csCookie));
            }
        }

        public static VSCommandRouting FromForm(VSContainerForm form)
        {
            VSCommandRouting vr;

            if (_map.TryGetValue(form, out vr))
                return vr;

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_csCookie != 0 && _rPct != null)
            {
                _rPct.UnregisterPriorityCommandTarget(_csCookie);
                _csCookie = 0;
            }

            if (_pane != null)
            {
                _pane.Dispose(); // Unhook
                _pane = null;
            }
            _map.Remove(_form);
            if (_installed)
                Application.RemoveMessageFilter(this);
        }

        #endregion

        #region IMessageFilter Members

        readonly MSG[] tmpMsg = new MSG[1];
        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.
        /// </returns>
        public bool PreFilterMessage(ref Message m)
        {
            int hr;
            IVsToolWindowToolbarHost host = _tbHost;
            if (host != null)
            {
                int lResult;
                hr = host.ProcessMouseActivationModal(m.HWnd, (uint)m.Msg, (uint)m.WParam, (int)m.LParam, out lResult);
                // Check for errors.
                if (ErrorHandler.Succeeded(hr))
                {
                    // ProcessMouseActivationModal returns S_FALSE to stop the message processing, but this
                    // function have to return true in this case.
                    if (hr == VSConstants.S_FALSE)
                    {
                        m.Result = (IntPtr)lResult;
                        return true;
                    }
                }
            }

            MSG[] messages = new MSG[1];
                messages[0].hwnd = m.HWnd;
                messages[0].lParam = m.LParam;
                messages[0].wParam = m.WParam;
                messages[0].message = (uint)m.Msg;

            VSContainerMode mode = _vsForm.ContainerMode;
            if (_fKeys != null && 0 != (mode & (VSContainerMode.TranslateKeys | VSContainerMode.UseTextEditorScope)))
            {
                Guid cmdGuid;
                uint cmdCode;
                int cmdTranslated;
                int keyComboStarts;

                uint dwFlags = (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_AllowModalState;

                if ((mode & VSContainerMode.UseTextEditorScope) != 0)
                    dwFlags |= (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseTextEditorKBScope;

                hr = _fKeys.TranslateAcceleratorEx(messages,
                    dwFlags,
                    0,
                    null,
                    out cmdGuid,
                    out cmdCode,
                    out cmdTranslated,
                    out keyComboStarts);

                if (hr == VSConstants.S_OK)
                {
                    if (cmdTranslated != 0)
                        return true;
                }

            }

            if (_paneList != null)
            {
                foreach (IVsWindowPane pane in _paneList)
                {
                    if (pane.TranslateAccelerator(messages) == 0)
                        return true;
                }
            }

            return false;
        }

        #endregion

        bool _loadRegistered;
        internal void OnHandleCreated()
        {
            if (_pane == null)
            {
                if (_panel == null)
                {
                    _panel = new Panel();
                    _panel.Location = new Point(0, 0);
                    _panel.Size = _form.ClientRectangle.Size;
                    _form.Controls.Add(_panel);
                }

                _pane = new VSFormContainerPane(_form.Context, this, _panel);

                IVsWindowPane p = _pane;

                IntPtr hwnd;
                Rectangle r = new Rectangle(_form.Location, _form.Size);
                _form.Location = new Point(0, 0);

                if (!ErrorHandler.Succeeded(p.CreatePaneWindow(_form.Handle, 0, 0, r.Width, r.Height, out hwnd)))
                {
                    _pane.Dispose();
                    _pane = null;
                    return;
                }
                _form.Size = r.Size;
                _panel.Size = _form.ClientSize;

                IButtonControl cancelButton = _form.CancelButton;
                IButtonControl acceptButton = _form.AcceptButton;

                for (int i = 0; i < _form.Controls.Count; i++)
                {
                    Control cc = _form.Controls[i];

                    if (cc != _panel)
                    {
                        _panel.Controls.Add(cc);
                        i--;
                        if (cc == cancelButton)
                            _form.CancelButton = cancelButton;

                        if (cc == acceptButton)
                            _form.AcceptButton = acceptButton;

                    }
                }
                _form.SizeChanged += new EventHandler(VSForm_SizeChanged);
            }
            if (!_loadRegistered)
            {
                _loadRegistered = true;
                _form.Load += new EventHandler(OnLoad);
            }
        }

        void OnLoad(object sender, EventArgs e)
        {
            if (_fKeys == null)
                _fKeys = GetService<IVsFilterKeys2>(typeof(SVsFilterKeys));

            if (_form.ToolBar != 0)
            {
                System.ComponentModel.Design.CommandID tbId = new System.ComponentModel.Design.CommandID(Ankh.Ids.AnkhId.CommandSetGuid, (int)_form.ToolBar);

                if (_tbHost == null)
                {
                    IVsUIShell uiShell = GetService<IVsUIShell>(typeof(SVsUIShell));

                    Marshal.ThrowExceptionForHR(uiShell.SetupToolbar(_form.Handle, (IVsToolWindowToolbar)this, out _tbHost));
                }                

                Guid toolbarCommandSet = tbId.Guid;
                Marshal.ThrowExceptionForHR(
                    _tbHost.AddToolbar(VSTWT_LOCATION.VSTWT_TOP, ref toolbarCommandSet, (uint)tbId.ID));
                Marshal.ThrowExceptionForHR(_tbHost.Show(0));
                Marshal.ThrowExceptionForHR(_tbHost.ForceUpdateUI());
            }
        }

        void VSForm_SizeChanged(object sender, EventArgs e)
        {
            Size formSz = _form.ClientRectangle.Size;

            Size sz = new Size(
                formSz.Width - reserved.left - reserved.right,
                formSz.Height - reserved.top - reserved.bottom);

            _panel.Size = sz;

            if (_tbHost != null)
                Marshal.ThrowExceptionForHR(_tbHost.BorderChanged());
            else
            {
                _panel.Location = new Point(0, 0);
                _panel.Size = _form.ClientSize;
            }
        }

        #region IVsToolWindowToolbar Members

        RECT reserved; // The number of pixels reserved on all sides

        /// <summary>
        /// Gets the border.
        /// </summary>
        /// <param name="prc">The PRC.</param>
        /// <returns></returns>
        public int GetBorder(RECT[] prc)
        {
            if (_panel == null)
                return VSConstants.E_NOTIMPL;

            Size sz = _form.ClientRectangle.Size;
            prc[0].left = 0;
            prc[0].top = 0;
            prc[0].right = sz.Width;
            prc[0].bottom = sz.Height;
            return VSConstants.S_OK;
        }

        bool _initialSet;
        public int SetBorderSpace(RECT[] pbw)
        {
            if ((reserved.left == pbw[0].left) &&
                (reserved.top == pbw[0].top) &&
                (reserved.right == pbw[0].right) &&
                (reserved.bottom == pbw[0].bottom))
            {
                return VSConstants.S_OK;
            }

            reserved = pbw[0];

            Size sz = _panel.Size;

            if (!_initialSet)
            {
                _initialSet = true;
                _form.ClientSize = new Size(
                    sz.Width + reserved.left + reserved.right,
                    sz.Height + reserved.top + reserved.bottom);
            }

            _panel.Location = new Point(reserved.left, reserved.top);

            VSForm_SizeChanged(null, EventArgs.Empty);

            return VSConstants.S_OK;
        }

        #endregion

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            if (_ctList != null)
            {
                foreach (IOleCommandTarget ct in _ctList)
                {
                    hr = ct.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                    if (((hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED) && (hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)))
                        break;
                }
            }
            
            return hr;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            int hr = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            if (_ctList != null)
            {
                foreach (IOleCommandTarget ct in _ctList)
                {
                    hr = ct.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                    if (((hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED) && (hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)))
                        break;
                }
            }
            
            return hr;
        }

        #endregion

        internal void AddCommandTarget(IOleCommandTarget commandTarget)
        {
            if (_ctList == null)
                _ctList = new List<IOleCommandTarget>();

            _ctList.Add(commandTarget);
        }

        internal void AddWindowPane(IVsWindowPane pane)
        {
            if (_paneList == null)
                _paneList = new List<IVsWindowPane>();

            _paneList.Add(pane);
        }
    }
}
