using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellPackage = Microsoft.VisualStudio.Shell.Package;

using Ankh.UI;
using Microsoft.VisualStudio;
using System.Diagnostics;


namespace Ankh.VS.Dialogs
{
    [ComVisible(true), CLSCompliant(false)]
    public sealed class VSDocumentInstance : AnkhService/*, IOleCommandTarget*/, IVsPersistDocData, IPersistFileFormat, IVsPersistDocData2, IVsPersistDocData3
    {
        readonly Guid _factoryId;
        public VSDocumentInstance(IAnkhServiceProvider context, Guid factoryId)
            : base(context)
        {
            _factoryId = factoryId;
        }

        #region IVsPersistDocData Members

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int GetGuidEditorType(out Guid pClassID)
        {
            pClassID = _factoryId;
            return VSConstants.S_OK;
        }

        public int IsDocDataDirty(out int pfDirty)
        {
            //throw new NotImplementedException();
            pfDirty = 0;
            return VSConstants.S_OK;
        }

        public int IsDocDataReloadable(out int pfReloadable)
        {
            pfReloadable = 0;
            return VSConstants.S_OK;
        }

        public int LoadDocData(string pszMkDocument)
        {
            //throw new NotImplementedException();
            return VSConstants.S_OK;
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            //throw new NotImplementedException();
            return VSConstants.S_OK;
        }

        public int ReloadDocData(uint grfFlags)
        {
            return VSConstants.S_OK;
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            throw new NotImplementedException();
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IPersistFileFormat Members

        public int GetClassID(out Guid pClassID)
        {
            throw new NotImplementedException();
        }

        public int GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            throw new NotImplementedException();
        }

        public int GetFormatList(out string ppszFormatList)
        {
            throw new NotImplementedException();
        }

        public int InitNew(uint nFormatIndex)
        {
            throw new NotImplementedException();
        }

        public int IsDirty(out int pfIsDirty)
        {
            pfIsDirty = 0;
            return 0;
        }

        public int Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            return 0;
        }

        public int Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            return 0;
        }

        public int SaveCompleted(string pszFilename)
        {
            return 0;
        }

        #endregion 
    
        #region IVsPersistDocData2 Members


        public int IsDocDataReadOnly(out int pfReadOnly)
        {
            pfReadOnly = 0;
            return 0;
        }

        public int SetDocDataDirty(int fDirty)
        {
            return 0;
        }

        public int SetDocDataReadOnly(int fReadOnly)
        {
            return 0;
        }

        #endregion

        #region IVsPersistDocData3 Members

        public int HandsOffDocDataStorage()
        {
            return 0;
        }

        public int HandsOnDocDataStorage()
        {
            return 0;
        }

        #endregion
    }

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
            set {}
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
                return GetService<IAnkhPackage>(typeof(IAnkhPackage)).AmbientProperties;
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


    [ComVisible(true), CLSCompliant(false)]
    public sealed class VSDocumentFormPane : WindowPane, IOleCommandTarget
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
            else if(instance == null)
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
    }
}
