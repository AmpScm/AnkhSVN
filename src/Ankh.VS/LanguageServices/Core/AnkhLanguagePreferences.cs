using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;

namespace Ankh.VS.LanguageServices.Core
{
    [ComVisible(true), Guid(AnkhId.LanguagePreferencesId)]
    public class AnkhLanguagePreferences : AnkhService, IVsTextManagerEvents2
    {
        Guid langSvc;
        LANGPREFERENCES2 prefs;
        uint? _connection;
        bool enableCodeSense;
        bool enableMatchBraces;
        bool enableQuickInfo;
        bool enableShowMatchingBrace;
        bool enableMatchBracesAtCaret;
        bool enableFormatSelection;
        bool enableCommenting;
        int maxErrorMessages;
        int codeSenseDelay;
        bool enableAsyncCompletion;
        bool autoOutlining;
        int maxRegionTime;
        _HighlightMatchingBraceFlags braceFlags;
        string name;

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.LanguagePreferences"]/*' />
        /// <summary>
        /// Gets the language preferences.
        /// </summary>
        public AnkhLanguagePreferences(IAnkhServiceProvider site, Guid langSvc, string name)
            : base(site)
        {
            this.langSvc = langSvc;
            this.name = name;
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.LanguageName;"]/*' />
        protected string LanguageName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        // Our base language service perferences (from Babel originally)
        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableCodeSense;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableCodeSense
        {
            get { return this.enableCodeSense; }
            set { this.enableCodeSense = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableMatchBraces;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableMatchBraces
        {
            get
            {
                return this.enableMatchBraces;
            }
            set { this.enableMatchBraces = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableQuickInfo;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableQuickInfo
        {
            get { return this.enableQuickInfo; }
            set { this.enableQuickInfo = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableShowMatchingBrace;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableShowMatchingBrace
        {
            get { return this.enableShowMatchingBrace; }
            set { this.enableShowMatchingBrace = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableMatchBracesAtCaret;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableMatchBracesAtCaret
        {
            get { return this.enableMatchBracesAtCaret; }
            set { this.enableMatchBracesAtCaret = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.MaxErrorMessages;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxErrorMessages
        {
            get { return this.maxErrorMessages; }
            set { this.maxErrorMessages = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.CodeSenseDelay;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CodeSenseDelay
        {
            get { return this.codeSenseDelay; }
            set { this.codeSenseDelay = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableAsyncCompletion;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableAsyncCompletion
        {
            get { return this.enableAsyncCompletion; }
            set { this.enableAsyncCompletion = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableFormatSelection;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableFormatSelection
        {
            get { return this.enableFormatSelection; }
            set { this.enableFormatSelection = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableCommenting;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableCommenting
        {
            get { return this.enableCommenting; }
            set { this.enableCommenting = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.MaxRegionTime;"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxRegionTime
        {
            get { return this.maxRegionTime; }
            set { this.maxRegionTime = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.HighlightMatchingBraceFlags;"]/*' />
        [CLSCompliant(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public _HighlightMatchingBraceFlags HighlightMatchingBraceFlags
        {
            get { return this.braceFlags; }
            set { this.braceFlags = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.Init"]/*' />
        public virtual void Init()
        {
            ILocalRegistry3 localRegistry = GetService<ILocalRegistry3>(typeof(SLocalRegistry));
            string root = null;
            if (localRegistry != null)
            {
                ErrorHandler.ThrowOnFailure(localRegistry.GetLocalRegistryRoot(out root));
            }
            if (root != null)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(root, false))
                {
                    if (key != null)
                    {
                        InitMachinePreferences(key, name);
                    }
                }
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(root, false))
                {
                    if (key != null)
                    {
                        InitUserPreferences(key, name);
                    }
                }
            }
            Connect();
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.InitUserPreferences"]/*' />
        public virtual void InitUserPreferences(RegistryKey key, string name)
        {
            this.GetLanguagePreferences();
        }
        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.GetIntegerValue"]/*' />
        public int GetIntegerValue(RegistryKey key, string name, int def)
        {
            object o = key.GetValue(name);
            if (o is int) return (int)o;
            int result;
            if (o is string && int.TryParse((string)o, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                return result;
            return def;
        }
        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.GetBooleanValue"]/*' />
        public bool GetBooleanValue(RegistryKey key, string name, bool def)
        {
            object o = key.GetValue(name);
            if (o is int) return ((int)o != 0);
            bool result;
            if (o is string && bool.TryParse((string)o, out result))
                return result;
            return def;
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.InitMachinePreferences"]/*' />
        public virtual void InitMachinePreferences(RegistryKey key, string name)
        {
            using (RegistryKey keyLanguage = key.OpenSubKey("languages\\language services\\" + name, false))
            {
                if (keyLanguage != null)
                {
                    this.EnableCodeSense = GetBooleanValue(keyLanguage, "CodeSense", true);
                    this.EnableMatchBraces = GetBooleanValue(keyLanguage, "MatchBraces", true);
                    this.EnableQuickInfo = GetBooleanValue(keyLanguage, "QuickInfo", true);
                    this.EnableShowMatchingBrace = GetBooleanValue(keyLanguage, "ShowMatchingBrace", true);
                    this.EnableMatchBracesAtCaret = GetBooleanValue(keyLanguage, "MatchBracesAtCaret", true);
                    this.MaxErrorMessages = GetIntegerValue(keyLanguage, "MaxErrorMessages", 10);
                    this.CodeSenseDelay = GetIntegerValue(keyLanguage, "CodeSenseDelay", 1000);
                    this.EnableAsyncCompletion = GetBooleanValue(keyLanguage, "EnableAsyncCompletion", true);
                    this.EnableFormatSelection = GetBooleanValue(keyLanguage, "EnableFormatSelection", false);
                    this.EnableCommenting = GetBooleanValue(keyLanguage, "EnableCommenting", true);
                    this.AutoOutlining = GetBooleanValue(keyLanguage, "AutoOutlining", true);
                    this.MaxRegionTime = GetIntegerValue(keyLanguage, "MaxRegionTime", 2000); // 2 seconds
                    this.braceFlags = (_HighlightMatchingBraceFlags)GetIntegerValue(keyLanguage, "HighlightMatchingBraceFlags", (int)_HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                Disconnect();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // General tab
        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.AutoListMembers"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoListMembers
        {
            get { return prefs.fAutoListMembers != 0; }
            set { prefs.fAutoListMembers = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.HideAdvancedMembers"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HideAdvancedMembers
        {
            get { return prefs.fHideAdvancedAutoListMembers != 0; }
            set { prefs.fHideAdvancedAutoListMembers = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.ParameterInformation"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ParameterInformation
        {
            get { return prefs.fAutoListParams != 0; }
            set { prefs.fAutoListParams = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.VirtualSpace"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VirtualSpace
        {
            get { return prefs.fVirtualSpace != 0; }
            set { prefs.fVirtualSpace = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.WordWrap"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WordWrap
        {
            get { return prefs.fWordWrap != 0; }
            set { prefs.fWordWrap = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.WordWrapGlyphs"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WordWrapGlyphs
        {
            get { return (int)prefs.fWordWrapGlyphs != 0; }
            set { prefs.fWordWrapGlyphs = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.CutCopyBlankLines"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CutCopyBlankLines
        {
            get { return (int)prefs.fCutCopyBlanks != 0; }
            set { prefs.fCutCopyBlanks = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.LineNumbers"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LineNumbers
        {
            get { return prefs.fLineNumbers != 0; }
            set { prefs.fLineNumbers = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableLeftClickForURLs"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableLeftClickForURLs
        {
            get { return prefs.fHotURLs != 0; }
            set { prefs.fHotURLs = (uint)(value ? 1 : 0); }
        }

        // Tabs tab
        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.Indenting"]/*' />
        [CLSCompliant(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public vsIndentStyle IndentStyle
        {
            get { return prefs.IndentStyle; }
            set { prefs.IndentStyle = value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.TabSize"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabSize
        {
            get { return (int)prefs.uTabSize; }
            set { prefs.uTabSize = (uint)value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.IndentSize"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndentSize
        {
            get { return (int)prefs.uIndentSize; }
            set { prefs.uIndentSize = (uint)value; }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.InsertSpaces"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertTabs
        {
            get { return prefs.fInsertTabs != 0; }
            set { prefs.fInsertTabs = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.ShowNavigationBar"]/*' />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowNavigationBar
        {
            get { return (int)prefs.fDropdownBar != 0; }
            set { prefs.fDropdownBar = (uint)(value ? 1 : 0); }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.EnableAutoOutlining"]/*' />
        public bool AutoOutlining
        {
            get { return this.autoOutlining; }
            set { this.autoOutlining = value; }
        }

        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.GetLanguagePrefs"]/*' />
        public virtual void GetLanguagePreferences()
        {
            IVsTextManager textMgr = GetService<IVsTextManager>(typeof(SVsTextManager));
            if (textMgr != null)
            {
                this.prefs.guidLang = langSvc;
                IVsTextManager2 textMgr2 = textMgr as IVsTextManager2;
                if (textMgr != null)
                {
                    LANGPREFERENCES2[] langPrefs2 = new LANGPREFERENCES2[1];
                    langPrefs2[0] = this.prefs;
                    if (ErrorHandler.Succeeded(textMgr2.GetUserPreferences2(null, null, langPrefs2, null)))
                    {
                        this.prefs = langPrefs2[0];
                    }
                    else
                    {
                        Debug.Assert(false, "textMgr2.GetUserPreferences2");
                    }
                }
            }
        }

        /// <include file='doc\PropertySheet.uex' path='docs/doc[@for="LanguagePreferences.Apply"]/*' />
        public virtual void Apply()
        {
            IVsTextManager2 textMgr2 = GetService<IVsTextManager2>(typeof(SVsTextManager));
            if (textMgr2 != null)
            {
                this.prefs.guidLang = langSvc;
                LANGPREFERENCES2[] langPrefs2 = new LANGPREFERENCES2[1];
                langPrefs2[0] = this.prefs;
                if (!ErrorHandler.Succeeded(textMgr2.SetUserPreferences2(null, null, langPrefs2, null)))
                {
                    Debug.Assert(false, "textMgr2.SetUserPreferences2");
                }
            }
        }

        private void Connect()
        {
            IVsTextManager2 textMgr = GetService<IVsTextManager2>(typeof(SVsTextManager));
            uint cookie;
            if (textMgr != null
                && TryHookConnectionPoint<IVsTextManagerEvents2>(textMgr, this, out cookie))
            {
                _connection = cookie;
            }
        }

        private void Disconnect()
        {
            if (_connection.HasValue)
            {
                IVsTextManager2 textMgr = GetService<IVsTextManager2>(typeof(SVsTextManager));

                if (textMgr != null)
                    ReleaseHook<IVsTextManagerEvents2>(textMgr, _connection.Value);
                _connection = null;
            }
        }

        #region IVsTextManagerEvents2 Members

        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnRegisterMarkerType"]/*' />
        public virtual int OnRegisterMarkerType(int iMarkerType)
        {
            return VSConstants.S_OK;
        }
        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnRegisterView"]/*' />
        [CLSCompliant(false)]
        public virtual int OnRegisterView(IVsTextView view)
        {
            return VSConstants.S_OK;
        }
        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnUnregisterView"]/*' />
        [CLSCompliant(false)]
        public virtual int OnUnregisterView(IVsTextView view)
        {
            return VSConstants.S_OK;
        }
        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnReplaceAllInFilesBegin"]/*' />
        public virtual int OnReplaceAllInFilesBegin()
        {
            return VSConstants.S_OK;
        }
        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnReplaceAllInFilesEnd"]/*' />
        public virtual int OnReplaceAllInFilesEnd()
        {
            return VSConstants.S_OK;
        }

        /// <include file='doc\Preferences.uex' path='docs/doc[@for="LanguagePreferences.OnUserPreferencesChanged2"]/*' />
        [CLSCompliant(false)]
        public virtual int OnUserPreferencesChanged2(VIEWPREFERENCES2[] viewPrefs, FRAMEPREFERENCES2[] framePrefs, LANGPREFERENCES2[] langPrefs, FONTCOLORPREFERENCES2[] fontColorPrefs)
        {
            if (langPrefs != null && langPrefs.Length > 0 && langPrefs[0].guidLang == this.langSvc)
            {
                this.prefs = langPrefs[0];
            }
            return VSConstants.S_OK;
        }

        #endregion
    }
}
