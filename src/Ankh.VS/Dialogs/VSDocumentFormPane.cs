using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Dialogs
{
    [ComVisible(true), CLSCompliant(false)]
    public sealed class VSDocumentFormPane : WindowPane, IOleCommandTarget, IVsPersistDocData,
                                IPersistFileFormat
    {
        readonly VSDocumentForm _form;
        public VSDocumentFormPane(IAnkhServiceProvider context, VSDocumentForm form)
            : base(context)
        {
            if (form == null)
                throw new ArgumentNullException("form");

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

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            //throw new NotImplementedException();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        #endregion

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
            throw new NotImplementedException();
        }

        public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            throw new NotImplementedException();
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            throw new NotImplementedException();
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            throw new NotImplementedException();
        }

        public int Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            throw new NotImplementedException();
        }

        public int SaveCompleted(string pszFilename)
        {
            throw new NotImplementedException();
        }

        #endregion




    }
}
