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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ankh.UI;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using Microsoft.VisualStudio.TextManager.Interop;


namespace Ankh.VS.Dialogs
{
    sealed class VSDocumentHost : ISite, IAnkhEditorPane, IOleCommandTarget, IAnkhServiceProvider
    {
        readonly VSDocumentFormPane _pane;

        public VSDocumentHost(VSDocumentFormPane pane)
        {
            _pane = pane;
        }
        #region ISite Members

        public IComponent Component
        {
            get { return _pane.Window as IComponent; }
        }

        Container _container;
        public IContainer Container
        {
            get { return _container ?? (_container = new Container()); }
        }

        public bool DesignMode
        {
            get { return false; }
        }

        public string Name
        {
            get { return ToString(); }
            set { }
        }

        #endregion

        #region IServiceProvider Members

        IAnkhPackage _package;
        public IAnkhPackage Package
        {
            get
            {
                if (_package != null)
                    return _package;

                if (_pane != null && _pane.Package != null)
                    _package = (IAnkhPackage)_pane.Package;

                return _package;
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(AmbientProperties))
            {
                return GetService<IAnkhPackage>().AmbientProperties;
            }

            System.IServiceProvider paneSp = _pane;

            object ob = paneSp.GetService(serviceType);

            if (ob != null)
                return ob;
            else if (Package != null)
                return Package.GetService(serviceType);
            else
                return null;
        }

        #region IAnkhServiceProvider Members

        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return (T)GetService(typeof(T));
        }

        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return (T)GetService(serviceType);
        }

        #endregion

        #endregion

        #region IAnkhEditorPane Members

        public void AddCommandTarget(IOleCommandTarget target)
        {
            _pane.AddCommandTarget(target);
        }

        public void SetFindTarget(object findTarget)
        {
            _pane.SetFindTarget(findTarget);
        }

        #endregion

        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return _pane.Exec(ref pguidCmdGroup, nCmdexecopt, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return _pane.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        #endregion
    }


    [ComVisible(true)]
    sealed class VSDocumentFormPane : WindowPane, IOleCommandTarget
    {
        readonly List<IOleCommandTarget> _targets = new List<IOleCommandTarget>();
        readonly VSEditorControl _form;
        readonly VSDocumentInstance _instance;
        readonly IAnkhServiceProvider _context;
        readonly VSDocumentHost _host;

        public VSDocumentFormPane(IAnkhServiceProvider context, VSDocumentInstance instance, VSEditorControl form)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (instance == null)
                throw new ArgumentNullException("instance");
            else if (form == null)
                throw new ArgumentNullException("form");

            _context = context;
            _instance = instance;
            _form = form;
            _host = new VSDocumentHost(this);
        }

        public IAnkhEditorPane Host
        {
            get { return _host; }
        }

        public IAnkhPackage Package
        {
            get { return _context.GetService<IAnkhPackage>(); }
        }

        bool _created;
        public override IWin32Window Window
        {
            get
            {
                if (!_created)
                {
                    _created = true;
                    if (!_form.IsHandleCreated)
                    {
                        _form.Visible = true; // If .Visible = false no window is created!
                        _form.CreateControl();
                        _form.Visible = false; // And hide the window now or we hijack the focus. See issue #507
                    }
                }
                return _form;
            }
        }

        protected override void OnCreate()
        {
            //_host.Load();
            _form.Site = _host;
            _form.Context = _host;
            base.OnCreate();
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IOleCommandTarget))
                return _host;
            else
            {
                object o = base.GetService(serviceType);

                return o;
            }
        }


        internal void Show()
        {
            base.Initialize();
            //throw new NotImplementedException();
        }

        IOleCommandTarget _baseTarget;

        IOleCommandTarget BaseTarget
        {
            get { return _baseTarget ?? (_baseTarget = _context.GetService<IOleCommandTarget>(typeof(Ankh.UI.IAnkhPackage))); }
        }


        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            foreach (IOleCommandTarget target in _targets)
            {
                hr = target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                if (hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED && hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)
                    return hr;
            }

            IOleCommandTarget t = BaseTarget;

            if (t != null)
                hr = t.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            return hr;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            int hr = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            foreach (IOleCommandTarget target in _targets)
            {
                hr = target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                if (hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED && hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)
                    return hr;
            }

            IOleCommandTarget t = BaseTarget;

            if (t != null)
                hr = t.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            return hr;
        }

        #endregion

        public void AddCommandTarget(IOleCommandTarget target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (!_targets.Contains(target))
                _targets.Add(target);
        }

        public void SetFindTarget(object findTarget)
        {
            IVsFindTarget ft = (findTarget as IVsFindTarget);
            if (null == ft)
                throw new ArgumentNullException("findTarget");

            _instance.SetFindTarget(ft);
        }
    }
}
