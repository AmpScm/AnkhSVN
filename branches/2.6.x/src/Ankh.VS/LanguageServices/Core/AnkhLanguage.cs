﻿using System;
using System.Collections.Generic;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using Ankh.Selection;


namespace Ankh.VS.LanguageServices.Core
{
    public abstract class AnkhLanguage : AnkhService, IVsLanguageInfo
    {
        protected AnkhLanguage(IAnkhServiceProvider context)
            : base(context)
        {
        }

        AnkhCommandMenu _defaultContextMenu;
        public AnkhCommandMenu DefaultContextMenu
        {
            get { return _defaultContextMenu; }
            set { _defaultContextMenu = value; }
        }

        #region IVsLanguageInfo Members

        int IVsLanguageInfo.GetCodeWindowManager(IVsCodeWindow codeWindow, out IVsCodeWindowManager codeWindowManager)
        {
            codeWindowManager = CreateCodeWindowManager(codeWindow);

            if (codeWindowManager != null)
                return VSErr.S_OK;
            else
                return VSErr.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        protected virtual AnkhCodeWindowManager CreateCodeWindowManager(IVsCodeWindow codeWindow)
        {
            return new AnkhCodeWindowManager(this, codeWindow);
        }

        int IVsLanguageInfo.GetColorizer(IVsTextLines lines, out IVsColorizer colorizer)
        {
            colorizer = CreateColorizer(lines);

            if (colorizer != null)
                return VSErr.S_OK;
            else
                return VSErr.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        protected virtual AnkhColorizer CreateColorizer(IVsTextLines lines)
        {
            return new AnkhColorizer(this, lines);
        }

        int IVsLanguageInfo.GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = FileExtensions ?? "";
            return VSErr.S_OK;
        }

        protected virtual string FileExtensions
        {
            get { return ""; }
        }

        public int GetLanguageName(out string name)
        {
            name = Name;
            return VSErr.S_OK;
        }

        public abstract string Name
        {
            get;
        }

        protected virtual AnkhLanguagePreferences CreatePreferences()
        {
            AnkhLanguagePreferences preferences;

            preferences = new AnkhLanguagePreferences(this, GetType().GUID, Name);
            preferences.Init();

            return preferences;
        }

        AnkhLanguagePreferences _preferences;
        public AnkhLanguagePreferences LanguagePreferences
        {
            get { return _preferences ?? (_preferences = CreatePreferences()); }
        }

        #endregion

        #region IObjectWithAutomation Members

        public object AutomationObject
        {
            get { return LanguagePreferences; }
        }

        #endregion

        [CLSCompliant(false)]
        protected internal virtual void OnNewView(AnkhCodeWindowManager codeWindowManager, IVsTextView view)
        {
            AnkhViewFilter filter = CreateFilter(codeWindowManager, view);

            if (filter != null)
            {
                IOleCommandTarget chained;
                view.AddCommandFilter(filter, out chained);

                filter.AddChained(chained);
            }
        }

        internal void OnCloseView(AnkhCodeWindowManager ankhCodeWindowManager, IVsTextView view)
        {

        }

        [CLSCompliant(false)]
        protected virtual AnkhViewFilter CreateFilter(AnkhCodeWindowManager manager, IVsTextView view)
        {
            return new AnkhViewFilter(manager, view);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool NeedsPerLineState
        {
            get { return false; }
        }

        public virtual AnkhLanguageDropDownBar CreateDropDownBar(AnkhCodeWindowManager manager)
        {
            return null;
        }
    }
}
