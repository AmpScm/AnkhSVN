using Ankh.Configuration;
using Ankh.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;

namespace Ankh.VS.LanguageServices.Core
{
    /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory"]/*' />
    /// <summary>
    /// You must inherit from this class and simply add a [ComVisible] and 
    /// [GuidAttribute] and then specify the EditorFactoryGuid, EditorFactoryGuid 
    /// and EditorName variables in your Registration class.
    /// This base class provides a default editor factory implementation
    /// that hosts the Visual Studio Core editor.  
    /// </summary>
    [CLSCompliant(false), ComVisible(true), Guid(AnkhId.EditorFactoryId)]
    public class AnkhEditorFactory : IVsEditorFactory
    {
        Microsoft.VisualStudio.Shell.Package package;
        IServiceProvider site;

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GUID_VsBufferDetectLangSID;"]/*' />
        public static readonly Guid GuidVSBufferDetectLangSid = new Guid(0x17F375AC, 0xC814, 0x11D1, 0x88, 0xAD, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2);

        __PROMPTONLOADFLAGS promptFlags = __PROMPTONLOADFLAGS.codepageNoPrompt;

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorInfo"]/*' />
        public class EditorInfo
        {
            private string name;
            private Guid guid;
            private int priority;
            private EditorInfo next;

            /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorInfo.Name"]/*' />
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorInfo.Guid"]/*' />
            public Guid Guid
            {
                get { return guid; }
                set { guid = value; }
            }

            /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorInfo.Priority"]/*' />
            public int Priority
            {
                get { return priority; }
                set { priority = value; }
            }

            /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorInfo.Next"]/*' />
            public EditorInfo Next
            {
                get { return next; }
                set { next = value; }
            }
        }

        // all registered editors.
        static Hashtable editors;  // extension -> EditorInfo

        // Registered exenstions for this editor under HKLM\Software\Microsoft\Visual Studio\9.0\Editors\\{" + this.GetType().GUID.ToString() + "}\\Extensions
        StringDictionary editorExtensions; // string -> string

        // all known extensions under HKLM\Software\Microsoft\Visual Studio\9.0\Language Services\Extensions
        static StringDictionary languageExtensions; // string -> language service guid.

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorFactory"]/*' />
        public AnkhEditorFactory(Microsoft.VisualStudio.Shell.Package package)
        {
            this.package = package;
            this.site = package;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.EditorFactory"]/*' />
        public AnkhEditorFactory() { }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.Finalize"]/*' />
        ~AnkhEditorFactory()
        {
            site = null;
#if LANGTRACE
            Trace.WriteLine("~EditorFactory");
#endif
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetSite"]/*' />
        protected IServiceProvider GetSite()
        {
            return this.site;
        }
        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetPackage"]/*' />
        protected Microsoft.VisualStudio.Shell.Package GetPackage()
        {
            return this.package;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.IsRegisteredExtension"]/*' />
        /// <summary>Returns true if the given extension is one of our registered extensions</summary>
        public virtual bool IsRegisteredExtension(string extension)
        {
            return GetEditorExtensions().ContainsKey(extension);
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetExtensions"]/*' />
        /// <summary>Return list of file extensions registered for this editor factory under 
        /// HKLM\Software\Microsoft\Visual Studio\9.0\Editors\\{" + this.GetType().GUID.ToString() + "}\\Extensions
        /// </summary>
        public virtual string[] GetExtensions()
        {
            ArrayList list = new ArrayList(this.GetEditorExtensions().Keys);
            return (string[])list.ToArray(typeof(string));
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetLanguageService"]/*' />
        /// <summary>Returns the guid of the language service registered for this file extension
        /// HKLM\Software\Microsoft\Visual Studio\9.0\Language Services\Extensions</summary>
        public virtual string GetLanguageService(string fileExtension)
        {
            return GetLanguageExtensions(this.site)[fileExtension] as string;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.WithEncoding"]/*' />
        public virtual __PROMPTONLOADFLAGS CodePagePrompt
        {
            get { return this.promptFlags; }
            set { this.promptFlags = value; }
        }

        bool CheckAllFileTypes()
        {
            return GetEditorExtensions().ContainsKey("*");
        }

        // Return your language service Guid here, this is used to set the language service on the 
        // IVsTextLines buffer returned from CreateEditorInstance.
        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetLanguageServiceGuid"]/*' />
        public virtual Guid GetLanguageServiceGuid()
        {
            return Guid.Empty;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetRegisteredEditor"]/*' />
        /// <summary>Returns the guid of the highest priority editor registered for this extension.
        /// This will also pick up user defined file extension to editor associations</summary>
        public virtual Guid GetRegisteredEditor(string extension)
        {
            EditorInfo ei = GetRegisteredEditorInfo(extension);
            if (ei != null)
            {
                return ei.Guid;
            }
            return Guid.Empty;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetRegisteredEditor"]/*' />
        /// <summary>Returns the guid of the highest priority editor registered for this extension.
        /// This will also pick up user defined file extension to editor associations.
        /// You can then access all registered editors via the .Next property.</summary>
        public virtual EditorInfo GetRegisteredEditorInfo(string extension)
        {
            string key = extension.ToUpperInvariant();
            return (EditorInfo)GetEditors(this.site)[key];
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.GetUserDefinedEditor"]/*' />
        /// <summary>Returns the guid of the editor that the user has defined for this file extension or
        /// Guid.Empty if none is found</summary>
        public Guid GetUserDefinedEditor(string extension)
        {
            StringDictionary map = this.GetFileExtensionMappings();
            object v = map[extension];
            if (v != null)
            {
                return GetGuid(v.ToString());
            }
            return Guid.Empty;
        }

        /// <summary>Returns true if the file extension is one that you registered for this editor
        /// factory, or your have registered the "*" extension and (this file type matches your 
        /// GetLanguageServiceGuid() or there is no other language service registered for this file extension).</summary>
        bool IsFileExtensionWeShouldEditAnyway(string ext)
        {

            if (!this.CheckAllFileTypes())
                return false;// you did not register "*".

            Guid langSid = GetLanguageServiceGuid();

            string guid = this.GetLanguageService(ext);
            if (!string.IsNullOrEmpty(guid))
            {
                // There is a language service associated with this file extension, see if it is ours or not.
                if (GetGuid(guid) != langSid)
                {
                    // Then it is not our extension.
                    return false;
                }
            }
            return true;
        }

        static Guid GetGuid(string value)
        {
            // we want to ignore badly registered guids.
            try
            {
                Guid guid = new Guid(value);
                return guid;
            }
            catch (FormatException)
            {
                // ignore badly registered file extensions
                return Guid.Empty;
            }
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.IsOurFileFormat"]/*' />
        // This method is called when the user has not explicitly requested your editor
        // and if the file extension doesn't match an explicit extension you registered,
        // and if you have registered the extension "*" to determine if this file is 
        // really something you want to take over or not.  If you return false then VS
        // will find the next best editor in the list.
        public virtual bool IsOurFileFormat(string moniker)
        {
            return true;
        }

        #region IVsEditorFactory

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.CreateEditorInstance"]/*' />
        /// <summary>
        /// This method checks to see if the specified file is one that your editor supports
        /// and if so, creates the core text editor and associated your language service 
        /// with it.  To figure out if the file is one that your editor supports it performs
        /// the following check:
        /// <list>
        /// <item>
        /// Call IsRegisteredExtension to see if the file extension is explicitly 
        /// registered to your editor.      
        /// </item>
        /// <item>
        /// Call GetUserDefinedEditor to see if the user has explicitly mapped the
        /// extension to your editor.
        /// </item>
        /// <item>
        /// If your editor registered the "*" extension, then it also calls 
        /// IsFileExtensionWeShouldEditAnyway and IsOurFileFormat to let you sniff
        /// the file and see if you think it contains stuff that your editor recognizes
        /// </item>
        /// </list>
        /// If all this is true then it goes ahead with the next step which is to
        /// get an IVsTextLines buffer and set it up as follows:
        /// <list>
        /// <item>
        /// If existingDocData is non-null then it checks to see if it can get an
        /// IVsTextLines buffer from this docData, and if not, returns VS_E_INCOMPATIBLEDOCDATA.
        /// Otherwise it creates a new VsTextBufferClass.
        /// </item>
        /// Calls IVsUserData.SetData on the IVsTextLines buffer with any code page prompt
        /// flags you have provided via the CodePagePrompt property.
        /// </list>
        /// <list>
        /// Calls SetLanguageServiceID to pass in your language service Guid and 
        /// sets the GuidVSBufferDetectLangSid IVsUserData to false to stop the core
        /// text editor from looking up a different language service.
        /// </list>
        /// Lastly it calls CreateEditorView to create the docView.
        /// </summary>
        public virtual int CreateEditorInstance(uint createDocFlags, string moniker, string physicalView, IVsHierarchy pHier, uint itemid, IntPtr existingDocData, out IntPtr docView, out IntPtr docData, out string editorCaption, out Guid cmdUI, out int cancelled)
        {
            docView = IntPtr.Zero;
            docData = IntPtr.Zero;
            editorCaption = null;
            cmdUI = Guid.Empty;
            cancelled = 0;
            int hr = VSErr.S_OK;

            if (AnkhEditorFactoryLogic.ShouldRejectEncodingPrompt(
                    this.promptFlags == __PROMPTONLOADFLAGS.codepagePrompt,
                    existingDocData != IntPtr.Zero))
            {
                hr = (int)VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                goto cleanup;
            }

            bool takeover = false;

            if (!string.IsNullOrEmpty(moniker))
            {
                string extension = Path.GetExtension(moniker);
                bool openSpecific =
                    (createDocFlags & (uint)__VSCREATEEDITORFLAGS2.CEF_OPENSPECIFIC) != 0;
                bool isOurs = IsRegisteredExtension(extension);
                bool isUserDefined =
                    GetUserDefinedEditor(extension) == this.GetType().GUID;

                bool canEditAnyway = true;
                if (!isOurs && !isUserDefined)
                    canEditAnyway = IsFileExtensionWeShouldEditAnyway(extension);

                EditorExtensionDecision extensionDecision =
                    AnkhEditorFactoryLogic.EvaluateExtension(
                        true,
                        openSpecific,
                        isOurs,
                        isUserDefined,
                        canEditAnyway,
                        CheckAllFileTypes());

                if (!extensionDecision.IsSupported)
                    return VSConstants.VS_E_UNSUPPORTEDFORMAT;

                takeover = extensionDecision.Takeover;

                if (extensionDecision.RequiresFormatCheck
                    && !IsOurFileFormat(moniker))
                {
                    return VSConstants.VS_E_UNSUPPORTEDFORMAT;
                }
            }

            IVsTextLines buffer;
            if (!TryGetTextBuffer(moniker, existingDocData, out buffer))
            {
                hr = VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                goto cleanup;
            }

            ConfigureEncodingPrompt(buffer);

            if (!TryConfigureLanguageService(buffer, ref takeover))
            {
                hr = VSConstants.VS_E_INCOMPATIBLEDOCDATA;
                goto cleanup;
            }

            if (takeover)
                DisableAutomaticLanguageDetection(buffer);

            docData = GetDocumentData(buffer, existingDocData);
            docView = CreateEditorView(
                moniker,
                buffer,
                physicalView,
                out editorCaption,
                out cmdUI);

            if (docView == IntPtr.Zero)
                hr = VSConstants.VS_E_UNSUPPORTEDFORMAT;

        cleanup:
            CleanupDocumentData(docView, existingDocData, ref docData);
            return hr;
        }

        private bool TryGetTextBuffer(
            string moniker,
            IntPtr existingDocData,
            out IVsTextLines buffer)
        {
            if (existingDocData != IntPtr.Zero)
            {
                object dataObject = Marshal.GetObjectForIUnknown(existingDocData);
                buffer = dataObject as IVsTextLines;

                if (buffer == null && dataObject is IVsTextBufferProvider provider)
                    Marshal.ThrowExceptionForHR(provider.GetTextBuffer(out buffer));

                return buffer != null;
            }

            Type textLinesType = typeof(IVsTextLines);
            Guid riid = textLinesType.GUID;
            Guid clsid = typeof(VsTextBufferClass).GUID;
            buffer = (IVsTextLines)package.CreateInstance(
                ref clsid,
                ref riid,
                textLinesType);

            if (!string.IsNullOrEmpty(moniker) && buffer is IVsUserData userData)
            {
                Guid bufferMoniker = typeof(IVsUserData).GUID;
                Marshal.ThrowExceptionForHR(
                    userData.SetData(ref bufferMoniker, moniker));
            }

            if (buffer is IObjectWithSite objectWithSite)
            {
                objectWithSite.SetSite(
                    this.site.GetService(typeof(IOleServiceProvider)));
            }

            return true;
        }

        private void ConfigureEncodingPrompt(IVsTextLines buffer)
        {
            if (this.promptFlags != __PROMPTONLOADFLAGS.codepagePrompt)
                return;

            IVsUserData userData = buffer as IVsUserData;
            if (userData == null)
                return;

            Guid encodingPrompt = new Guid(
                0x99ec03f0,
                0xc843,
                0x4c09,
                0xbe,
                0x74,
                0xcd,
                0xca,
                0x51,
                0x58,
                0xd3,
                0x6c);

            Marshal.ThrowExceptionForHR(
                userData.SetData(
                    ref encodingPrompt,
                    (uint)this.CodePagePrompt));
        }

        private bool TryConfigureLanguageService(
            IVsTextLines buffer,
            ref bool takeover)
        {
            Guid languageService = GetLanguageServiceGuid();
            if (languageService == Guid.Empty)
                return true;

            Guid defaultLanguageService =
                new Guid("{8239bec4-ee87-11d0-8c98-00c04fc2ab22}");

            Guid currentLanguageService;
            Marshal.ThrowExceptionForHR(
                buffer.GetLanguageServiceID(out currentLanguageService));

            EditorLanguageDecision decision =
                AnkhEditorFactoryLogic.EvaluateLanguageService(
                    languageService,
                    currentLanguageService,
                    defaultLanguageService);

            if (decision.Action == EditorLanguageAction.Incompatible)
                return false;

            if (decision.Action == EditorLanguageAction.SetRequested)
            {
                Marshal.ThrowExceptionForHR(
                    buffer.SetLanguageServiceID(ref languageService));
            }

            takeover = decision.Takeover;
            return true;
        }

        private void DisableAutomaticLanguageDetection(IVsTextLines buffer)
        {
            IVsUserData userData = (IVsUserData)buffer;
            Guid bufferDetectLanguage = GuidVSBufferDetectLangSid;
            Marshal.ThrowExceptionForHR(
                userData.SetData(ref bufferDetectLanguage, false));
        }

        private static IntPtr GetDocumentData(
            IVsTextLines buffer,
            IntPtr existingDocData)
        {
            if (existingDocData != IntPtr.Zero)
            {
                Marshal.AddRef(existingDocData);
                return existingDocData;
            }

            return Marshal.GetIUnknownForObject(buffer);
        }

        private static void CleanupDocumentData(
            IntPtr docView,
            IntPtr existingDocData,
            ref IntPtr docData)
        {
            if (docView != IntPtr.Zero)
                return;

            if (existingDocData == docData || docData == IntPtr.Zero)
                return;

            Marshal.Release(docData);
            docData = IntPtr.Zero;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.CreateEditorView"]/*' />
        /// <summary>Return docView IUnknown COM object.</summary>
        public virtual IntPtr CreateEditorView(string moniker, IVsTextLines buffer, string physicalView, out string editorCaption, out Guid cmdUI)
        {
            Type tcw = typeof(IVsCodeWindow);
            Guid riid = tcw.GUID;
            // Once this is done the project's assembly reference to "$(EnvRefPath)\Microsoft.VisualStudio.Editor.dll" may be removed
            Guid clsid = typeof(VsCodeWindowClass).GUID;
            IVsCodeWindow window = (IVsCodeWindow)package.CreateInstance(ref clsid, ref riid, tcw);

            Marshal.ThrowExceptionForHR(window.SetBuffer(buffer));
            Marshal.ThrowExceptionForHR(window.SetBaseEditorCaption(null));
            Marshal.ThrowExceptionForHR(window.GetEditorCaption(READONLYSTATUS.ROSTATUS_Unknown, out editorCaption));

            Guid CMDUIGUID_TextEditor = new Guid(0x8B382828, 0x6202, 0x11d1, 0x88, 0x70, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2);
            cmdUI = CMDUIGUID_TextEditor;
            IntPtr docView = Marshal.GetIUnknownForObject(window);

            return docView;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.MapLogicalView"]/*' />
        /// <devdoc>The default implementation supports LOGVIEWID_Code, LOGVIEWID_TextView,
        /// LOGVIEWID_Debugging, and LOGVIEWID_Primary returning null for
        /// the physicalView string.</devdoc>
        public virtual int MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            physicalView = null;
            // The default suppo
            if (logicalView == VSConstants.LOGVIEWID_Code ||
                logicalView == VSConstants.LOGVIEWID_TextView ||
                logicalView == VSConstants.LOGVIEWID_Debugging ||
                logicalView == VSConstants.LOGVIEWID_Primary)
            {
                physicalView = null;
                return VSErr.S_OK;
            }
            return VSErr.E_NOTIMPL;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.SetSite"]/*' />
        public virtual int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            this.site = new Microsoft.VisualStudio.Shell.ServiceProvider(psp);
            return VSErr.S_OK;
        }

        StringDictionary GetEditorExtensions()
        {
            if (this.editorExtensions != null)
                return this.editorExtensions;

            Guid ourGuid = this.GetType().GUID;
            StringDictionary map = new StringDictionary();
            Hashtable editors = GetEditors(this.site);
            foreach (string ext in editors.Keys)
            {
                EditorInfo ei = (EditorInfo)editors[ext];
                while (ei != null)
                {
                    if (ei.Guid == ourGuid)
                    {
                        map[ext] = ext;
                        break;
                    }
                    ei = ei.Next;
                }
            }
            return this.editorExtensions = map;
        }

        static StringDictionary GetLanguageExtensions(IServiceProvider site)
        {
            if (AnkhEditorFactory.languageExtensions != null)
                return AnkhEditorFactory.languageExtensions;

            StringDictionary extensions = new StringDictionary();

            if (site.GetService(typeof(IAnkhConfigurationService)) is IAnkhConfigurationService configService)
                using (RegistryKey key = configService.OpenVSInstanceKey("Languages\\File Extensions"))
                {
                    if (key != null)
                    {
                        foreach (string ext in key.GetSubKeyNames())
                        {
                            using (RegistryKey extkey = key.OpenSubKey(ext, false))
                            {
                                if (extkey != null)
                                {
                                    string fe = ext;
                                    string guid = extkey.GetValue(null) as string; // get default value
                                    if (!extensions.ContainsKey(fe))
                                    {
                                        extensions.Add(fe, guid);
                                    }
                                }
                            }
                        }
                    }
                }

            return AnkhEditorFactory.languageExtensions = extensions;
        }

        static Hashtable GetEditors(IServiceProvider site)
        {
            if (AnkhEditorFactory.editors != null)
                return AnkhEditorFactory.editors;

            Hashtable editors = new Hashtable();

            if (!(site.GetService(typeof(IAnkhConfigurationService)) is IAnkhConfigurationService configService))
                return editors;

            using (RegistryKey editorsKey = configService.OpenVSInstanceKey("Editors"))
            {
                if (editorsKey != null)
                    foreach (string editorGuid in editorsKey.GetSubKeyNames())
                    {
                        Guid guid = GetGuid(editorGuid);
                        using (RegistryKey editorKey = editorsKey.OpenSubKey(editorGuid, false))
                        {
                            object value = editorKey.GetValue(null);
                            string name = (value != null) ? value.ToString() : editorGuid.ToString();
                            RegistryKey extensions = editorKey.OpenSubKey("Extensions", false);
                            if (extensions != null)
                            {
                                foreach (string s in extensions.GetValueNames())
                                {
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        EditorInfo ei = new EditorInfo
                                        {
                                            Name = name,
                                            Guid = guid
                                        };
                                        object obj = extensions.GetValue(s);
                                        if (obj is int priority)
                                        {
                                            ei.Priority = priority;
                                        }
                                        string ext = (s == "*") ? s : "." + s;
                                        AddEditorInfo(editors, ext, ei);
                                    }
                                }
                            }
                        }
                    }
            }
            return AnkhEditorFactory.editors = editors;
        }

        static void AddEditorInfo(Hashtable editors, string ext, EditorInfo ei)
        {
            ext = ext.ToUpperInvariant();
            EditorInfo other = (EditorInfo)editors[ext];
            if (other != null)
            {
                EditorInfo previous = null;
                while (other != null && other.Priority > ei.Priority)
                {
                    previous = other;
                    other = other.Next;
                }
                if (previous == null)
                {
                    editors[ext] = ei;
                    ei.Next = other;
                }
                else
                {
                    ei.Next = previous.Next;
                    previous.Next = ei;
                }
            }
            else
            {
                editors[ext] = ei;
            }
        }

        StringDictionary GetFileExtensionMappings()
        {
            StringDictionary map = new StringDictionary();

            if (!(site.GetService(typeof(IAnkhConfigurationService)) is IAnkhConfigurationService configService))
                return map;

            using (RegistryKey key = configService.OpenVSInstanceKey("Default Editors"))
            {
                if (key != null)
                {
                    foreach (string name in key.GetSubKeyNames())
                    {
                        using (RegistryKey extkey = key.OpenSubKey(name, false))
                        {
                            if (extkey != null)
                            {
                                object obj = extkey.GetValue("Custom");
                                if (obj is string)
                                {
                                    string ext = "." + name;
                                    map[ext] = obj.ToString(); // extension -> editor.
                                }
                            }
                        }
                    }
                }
            }
            return map;
        }

        /// <include file='doc\EditorFactory.uex' path='docs/doc[@for="EditorFactory.Close"]/*' />
        public virtual int Close()
        {
            this.site = null;
            this.package = null;
            return VSErr.S_OK;
        }
        #endregion
    }
}
