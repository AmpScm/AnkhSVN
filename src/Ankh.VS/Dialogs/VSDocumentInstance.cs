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
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellPackage = Microsoft.VisualStudio.Shell.Package;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;


namespace Ankh.VS.Dialogs
{
    [ComVisible(true)]
    sealed class VSDocumentInstance : AnkhService/*, IOleCommandTarget*/, IVsPersistDocData, IPersistFileFormat, IVsPersistDocData2, IVsPersistDocData3, IVsFindTarget, IVsFindTarget2
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
            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;
            return VSConstants.S_OK;
        }

        public int SetUntitledDocPath(string pszDocDataPath)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IPersistFileFormat Members

        public int GetClassID(out Guid pClassID)
        {
            pClassID = Guid.Empty;
            return VSConstants.E_UNEXPECTED;
        }

        public int GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            ppszFilename = "";
            pnFormatIndex = 0;
            return VSConstants.E_UNEXPECTED;
        }

        public int GetFormatList(out string ppszFormatList)
        {
            ppszFormatList = "";
            return VSConstants.E_UNEXPECTED;
        }

        public int InitNew(uint nFormatIndex)
        {
            return VSConstants.S_OK;
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

        #region IVsFindTarget Members

        int IVsFindTarget.Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult)
        {
            if (_findTarget != null)
                return _findTarget.Find(pszSearch, grfOptions, fResetStartPoint, pHelper, out pResult);

            pResult = 0;
            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.GetCapabilities(bool[] pfImage, uint[] pgrfOptions)
        {
            if (_findTarget != null)
                return _findTarget.GetCapabilities(pfImage, pgrfOptions);

            pfImage[0] = false;
            pgrfOptions[0] = 0;
            return 0;
        }

        int IVsFindTarget.GetCurrentSpan(TextSpan[] pts)
        {
            if (_findTarget != null)
                return _findTarget.GetCurrentSpan(pts);

            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.GetFindState(out object ppunk)
        {
            if (_findTarget != null)
                return _findTarget.GetFindState(out ppunk);

            ppunk = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.GetMatchRect(RECT[] prc)
        {
            if (_findTarget != null)
                return _findTarget.GetMatchRect(prc);

            return 0;
        }

        int IVsFindTarget.GetProperty(uint propid, out object pvar)
        {
            if (_findTarget != null)
                return _findTarget.GetProperty(propid, out pvar);

            pvar = null;
            return VSConstants.E_FAIL; // All properties are documented to be optional!
        }

        int IVsFindTarget.GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage)
        {
            if (_findTarget != null)
                return _findTarget.GetSearchImage(grfOptions, ppSpans, out ppTextImage);

            ppTextImage = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.MarkSpan(TextSpan[] pts)
        {
            if (_findTarget != null)
                return _findTarget.MarkSpan(pts);

            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.NavigateTo(TextSpan[] pts)
        {
            if (_findTarget != null)
                return _findTarget.NavigateTo(pts);

            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.NotifyFindTarget(uint notification)
        {
            if (_findTarget != null)
                return _findTarget.NotifyFindTarget(notification);

            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced)
        {
            if (_findTarget != null)
                return _findTarget.Replace(pszSearch, pszReplace, grfOptions, fResetStartPoint, pHelper, out pfReplaced);

            pfReplaced = 0;
            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget.SetFindState(object pUnk)
        {
            if (_findTarget != null)
                return _findTarget.SetFindState(pUnk);

            return VSConstants.E_NOTIMPL;
        }

        int IVsFindTarget2.NavigateTo2(IVsTextSpanSet pSpans, TextSelMode iSelMode)
        {
            if (_findTarget2 != null)
                return _findTarget2.NavigateTo2(pSpans, iSelMode);

            return VSConstants.E_NOTIMPL;
        }


        #endregion

        IVsFindTarget _findTarget;
        IVsFindTarget2 _findTarget2;
        internal void SetFindTarget(IVsFindTarget ft)
        {
            _findTarget = ft;
            _findTarget2 = ft as IVsFindTarget2;
        }
    }
}
