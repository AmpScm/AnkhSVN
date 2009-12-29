using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;

namespace Ankh.VS.LanguageServices.Core
{
    public class AnkhLanguageDropDownBar : AnkhService, IVsDropdownBarClient
    {
        AnkhCodeWindowManager _manager;
        uint _codeWindowCookie;

        public AnkhLanguageDropDownBar(AnkhLanguage language, AnkhCodeWindowManager manager)
            : base(language)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
        }

        protected internal virtual void Initialize(AnkhCodeWindowManager codeWindowManager)
        {

            
            //throw new NotImplementedException();
        }
        internal void Close()
        {
            _manager = null;
        }

        #region IVsDropdownBarClient Members

        [CLSCompliant(false)]
        public int GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList)
        {
            pcEntries = 0;
            puEntryType = 0;
            phImageList = IntPtr.Zero;
            return VSConstants.E_NOTIMPL;
        }

        public int GetComboTipText(int iCombo, out string pbstrText)
        {
            pbstrText = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetEntryAttributes(int iCombo, int iIndex, out uint pAttr)
        {
            pAttr = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetEntryImage(int iCombo, int iIndex, out int piImageIndex)
        {
            piImageIndex = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetEntryText(int iCombo, int iIndex, out string ppszText)
        {
            ppszText = null;
            return VSConstants.E_NOTIMPL;
        }

        public int OnComboGetFocus(int iCombo)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnItemChosen(int iCombo, int iIndex)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnItemSelected(int iCombo, int iIndex)
        {
            return VSConstants.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        public int SetDropdownBar(IVsDropdownBar pDropdownBar)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion


        internal void OnNewView(IVsTextView view)
        {
            //throw new NotImplementedException();
        }

        internal void OnCloseView(IVsTextView view)
        {
            //throw new NotImplementedException();
        }
    }
}
