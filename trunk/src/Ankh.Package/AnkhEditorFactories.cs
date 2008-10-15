﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell;
using Ankh.Ids;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Ankh.UI.Blame;
using Ankh.VS.Dialogs;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.VSPackage
{
    [ComVisible(true)]
    class AnkhMiniDoc: IOleCommandTarget
    {

        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            throw new NotImplementedException();
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    abstract class AnkhEditorFactory : AnkhService, IVsEditorFactory, IVsEditorFactory2
    {
        IOleServiceProvider _site;

        protected AnkhEditorFactory(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IVsEditorFactory Members

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocData = IntPtr.Zero;
            ppunkDocView = IntPtr.Zero;
            pbstrEditorCaption = "QWQ";
            pguidCmdUI = new Guid("{00000000-0000-0000-e4e7-120000008400}");
            pgrfCDW = 0;
            // Validate inputs
            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }

            TstForm tst = new TstForm();
            VSDocumentFormPane pane = new VSDocumentFormPane(Context, tst);
            ppunkDocView = Marshal.GetIUnknownForObject(pane);
            ppunkDocData = Marshal.GetIUnknownForObject(pane);
            return VSConstants.S_OK;
        }

        public virtual int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            if (rguidLogicalView == VSConstants.LOGVIEWID_Primary)
            {
                pbstrPhysicalView = null;
                return VSConstants.S_OK;
            }



            return VSConstants.E_NOTIMPL;
        }

        public int SetSite(IOleServiceProvider psp)
        {
            _site = psp;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsEditorFactory2 Members

        public int RetargetCodeOrDesignerToOpen(string pszMkDocumentSource, ref Guid rguidLogicalViewSource, IVsHierarchy pvHier, uint itemidSource, out uint pitemidTarget, out uint pgrfEditorFlags, out Guid pguidEditorTypeTarget, out string pbstrPhysicalViewTarget, out Guid pguidLogicalViewTarget)
        {
            pitemidTarget = VSConstants.VSITEMID_NIL;
            pgrfEditorFlags = 0;
            pguidEditorTypeTarget = Guid.Empty;
            pbstrPhysicalViewTarget = null;
            pguidLogicalViewTarget = Guid.Empty;
            return VSConstants.S_OK; // Don't retarget
        }

        #endregion
    }

    [Guid(AnkhId.DiffEditorId), ComVisible(true)]
    sealed class AnkhDiffEditorFactory : AnkhEditorFactory
    {
        public AnkhDiffEditorFactory(AnkhSvnPackage package)
            : base(package)
        {

        }

        public override int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            if (ErrorHandler.Succeeded(base.MapLogicalView(ref rguidLogicalView, out pbstrPhysicalView)))
                return VSConstants.S_OK;

            if (rguidLogicalView == new Guid(AnkhId.DiffEditorViewId))
            {
                pbstrPhysicalView = null;
                return VSConstants.S_OK;
            }

            return VSConstants.E_NOTIMPL;
        }
    }

    [Guid(AnkhId.AnnotateEditorId), ComVisible(true)]
    sealed class AnkhAnnotateEditorFactory : AnkhEditorFactory
    {
        public AnkhAnnotateEditorFactory(AnkhSvnPackage package)
            : base(package)
        {
        }

        public override int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            if (ErrorHandler.Succeeded(base.MapLogicalView(ref rguidLogicalView, out pbstrPhysicalView)))
                return VSConstants.S_OK;

            if (rguidLogicalView == new Guid(AnkhId.AnnotateEditorViewId))
            {
                pbstrPhysicalView = null;
                return VSConstants.S_OK;
            }

            return VSConstants.E_NOTIMPL;
        }
    }
}