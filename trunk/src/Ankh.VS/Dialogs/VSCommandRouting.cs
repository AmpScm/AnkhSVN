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
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VS.Dialogs
{
    class VSCommandRouting : AnkhService, IMessageFilter, IDisposable, IVsToolWindowToolbar
    {
        readonly VSContainerForm _form;
        readonly IAnkhVSContainerForm _vsForm;
        readonly static Dictionary<VSContainerForm, VSCommandRouting> _map = new Dictionary<VSContainerForm, VSCommandRouting>();
        VSFormContainerPane _pane;
        IVsToolWindowToolbarHost _tbHost;
        Panel _panel;

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
            IVsToolWindowToolbarHost host = _tbHost;
            if (host != null)
            {
                int lResult;
                int hr = host.ProcessMouseActivationModal(m.HWnd, (uint)m.Msg, (uint)m.WParam, (int)m.LParam, out lResult);
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
                //                _form.EnabledChanged += new EventHandler(Form_EnabledChanged);
                //                _form.VisibleChanged += new EventHandler(Form_EnabledChanged);

                IVsWindowPane p = _pane;

                IntPtr hwnd;
                Rectangle r = new Rectangle(_form.Location, _form.Size);
                _form.Location = new Point(0, 0);

                if (!ErrorHandler.Succeeded(p.CreatePaneWindow(_form.Handle, r.X, r.Y, r.Width, r.Height, out hwnd)))
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
            if (_form.ToolBar != 0)
            {
                System.ComponentModel.Design.CommandID tbId = new System.ComponentModel.Design.CommandID(Ankh.Ids.AnkhId.CommandSetGuid, (int)_form.ToolBar);

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
    }
}
