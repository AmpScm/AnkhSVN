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

using Ankh.UI;
using Microsoft.VisualStudio;


namespace Ankh.VS.Dialogs
{
    [ComVisible(true), CLSCompliant(false)]
    public sealed class VSDocumentInstance : AnkhService/*, IOleCommandTarget*/, IVsPersistDocData, IPersistFileFormat, IVsPersistDocData2, IVsPersistDocData3
    {
        public VSDocumentInstance(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IVsPersistDocData Members

        public int Close()
        {
            return 0;
        }

        public int GetGuidEditorType(out Guid pClassID)
        {
            throw new NotImplementedException();
        }

        public int IsDocDataDirty(out int pfDirty)
        {
            //throw new NotImplementedException();
            pfDirty = 0;
            return 0;
        }

        public int IsDocDataReloadable(out int pfReloadable)
        {
            throw new NotImplementedException();
        }

        public int LoadDocData(string pszMkDocument)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public int ReloadDocData(uint grfFlags)
        {
            return 0;
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return 0;
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            throw new NotImplementedException();
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            return 0;
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

    [ComVisible(true), CLSCompliant(false)]
    public sealed class VSDocumentFormPane : WindowPane, IOleCommandTarget
    {
        readonly List<IOleCommandTarget> _targets = new List<IOleCommandTarget>();
        readonly VSEditorControl _form;
        readonly VSDocumentInstance _instance;
        readonly IAnkhServiceProvider _context;
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
                target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                if (hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED && hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)
                    return hr;
            }

            IOleCommandTarget t = BaseTarget;

            if (t != null)
                hr = t.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            return hr;
        }

        #endregion
    }
}
