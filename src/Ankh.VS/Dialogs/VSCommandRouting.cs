using System;
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

namespace Ankh.VS.Dialogs
{
    class VSCommandRouting : AnkhService, IMessageFilter, IDisposable, IVsToolWindowToolbar
    {
        readonly VSContainerForm _form;
        readonly IAnkhVSContainerForm _vsForm;
        readonly static Dictionary<VSContainerForm, VSCommandRouting> _map = new Dictionary<VSContainerForm, VSCommandRouting>();
        VSFormContainerPane _pane;
        IVsToolWindowToolbarHost _tbHost;
        IVsFilterKeys2 _filterKeys;
        Panel _panel;
        //IntPtr _paneHwnd;

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

        readonly MSG[] tmpMsg = new MSG[0];
        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.
        /// </returns>
        public bool PreFilterMessage(ref Message m)
        {
            IVsToolWindowToolbarHost host = _tbHost;
            if (host != null)
            {
                int lResult;
                int hr = host.ProcessMouseActivationModal(m.HWnd, (uint)m.Msg, (uint)m.WParam, (int)m.LParam, out lResult);
                // Check for errors.
                if (ErrorHandler.Failed(hr))
                    return false;

                // ProcessMouseActivationModal returns S_FALSE to stop the message processing, but this
                // function have to return true in this case.
                return (hr == VSConstants.S_FALSE);
            }

            if(_filterKeys == null)
                _filterKeys = (IVsFilterKeys2)GetService(typeof(SVsFilterKeys));

            if (_filterKeys != null)
            {
                MSG[] messages = tmpMsg;
                messages[0].hwnd = m.HWnd;
                messages[0].lParam = m.LParam;
                messages[0].wParam = m.WParam;
                messages[0].message = (uint)m.Msg;

                Guid cmdGuid;
                uint cmdCode;
                int cmdTranslated;
                int keyComboStarts;

                int hr = _filterKeys.TranslateAcceleratorEx(messages,
                    (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseTextEditorKBScope //Translates keys using TextEditor key bindings. Equivalent to passing CMDUIGUID_TextEditor, CMDSETID_StandardCommandSet97, and guidKeyDupe for scopes and the VSTAEXF_IgnoreActiveKBScopes flag. 
                    | (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_AllowModalState,  //By default this function cannot be called when the shell is in a modal state, since command routing is inherently dangerous. However if you must access this in a modal state, specify this flag, but keep in mind that many commands will cause unpredictable behavior if fired. 
                    0,
                    null,
                    out cmdGuid,
                    out cmdCode,
                    out cmdTranslated,
                    out keyComboStarts);

                if (hr != VSConstants.S_OK)
                {
                    return false;
                }

                return cmdTranslated != 0;
            }

            return false;
        }

        #endregion

        internal void OnHandleCreated()
        {
            if (_pane == null)
            {
                _pane = new VSFormContainerPane(_form.Context, this, _form);

                IVsWindowPane p = _pane;

                IntPtr hwnd;
                Rectangle r = new Rectangle(_form.Location, _form.Size);
                _form.Location = new Point(0, 0);
                if (!ErrorHandler.Succeeded(p.CreatePaneWindow(IntPtr.Zero, r.X, r.Y, r.Width, r.Height, out hwnd)))
                {
                    _pane.Dispose();
                    _pane = null;
                    return;
                }
                System.ComponentModel.Design.CommandID tbId = new System.ComponentModel.Design.CommandID(AnkhSvn.Ids.AnkhId.CommandSetGuid, (int)AnkhSvn.Ids.AnkhToolBar.PendingChanges);


                if (tbId != null)
                {
                    if (_panel == null)
                    {
                        _panel = new Panel();
                        _panel.Size = _form.ClientRectangle.Size;
                        _form.Controls.Add(_panel);

                        foreach (Control c in _form.Controls)
                        {
                            if (c != _panel)
                                _panel.Controls.Add(c);
                        }
                        _form.SizeChanged += new EventHandler(VSForm_SizeChanged);
                    }

                    if (_tbHost == null)
                    {
                        IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

                        Marshal.ThrowExceptionForHR(uiShell.SetupToolbar(_form.Handle, (IVsToolWindowToolbar)this, out _tbHost));
                    }

                    Guid toolbarCommandSet = tbId.Guid;
                    Marshal.ThrowExceptionForHR(
                        _tbHost.AddToolbar(VSTWT_LOCATION.VSTWT_TOP, ref toolbarCommandSet, (uint)tbId.ID));
                    Marshal.ThrowExceptionForHR(_tbHost.Show(0));
                    Marshal.ThrowExceptionForHR(_tbHost.ForceUpdateUI());
                }
            }
        }

        void VSForm_SizeChanged(object sender, EventArgs e)
        {
            Size formSz = _form.ClientRectangle.Size;

            Size sz = new Size(
                formSz.Width - reserved.left - reserved.right,
                formSz.Height - reserved.top - reserved.bottom);

            _panel.Size = sz;

            Marshal.ThrowExceptionForHR(_tbHost.BorderChanged());
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
                _form.Size = new Size(
                    sz.Width + reserved.left + reserved.right,
                    sz.Height + reserved.top + reserved.bottom);
            }

            _panel.Location = new Point(reserved.left, reserved.top);

            VSForm_SizeChanged(null, EventArgs.Empty);

            return VSConstants.S_OK;
        }

        #endregion
    }
}
