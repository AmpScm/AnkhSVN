// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using Ankh.Ids;
using Ankh.Selection;
using Ankh.Commands;
using Ankh.Scc;


namespace Ankh.VS.Extenders
{
    /// <summary>
    /// This is the class factory for extender objects
    /// </summary>
    [GlobalService(typeof(AnkhExtenderProvider), true)]
    public sealed class AnkhExtenderProvider : AnkhService, EnvDTE.IExtenderProvider, IDisposable
    {
        public const string ServiceName = AnkhId.ExtenderProviderName;
        #region CATIDs
        const string CATID_CscFileBrowse = "{8D58E6AF-ED4E-48B0-8C7B-C74EF0735451}";
        const string CATID_CscFolderBrowse = "{914FE278-054A-45DB-BF9E-5F22484CC84C}";
        const string CATID_CscProjectBrowse = "{4EF9F003-DE95-4D60-96B0-212979F2A857}";
        const string CATID_VbFileBrowse = "{EA5BD05D-3C72-40A5-95A0-28A2773311CA}";
        const string CATID_VbFolderBrowse = "{932DC619-2EAA-4192-B7E6-3D15AD31DF49}";
        const string CATID_VbProjectBrowse = "{E0FDC879-C32A-4751-A3D3-0B3824BD575F}";
        const string CATID_VjFileBrowse = "{E6FDF869-F3D1-11D4-8576-0002A516ECE8}";
        const string CATID_VjFolderBrowse = "{E6FDF86A-F3D1-11D4-8576-0002A516ECE8}";
        const string CATID_VjProjectBrowse = "{E6FDF86C-F3D1-11D4-8576-0002A516ECE8}";
        const string CATID_SolutionBrowse = "{A2392464-7C22-11D3-BDCA-00C04F688E50}";
        const string CATID_CcFileBrowse = "{EE8299C9-19B6-4F20-ABEA-E1FD9A33B683}";
        const string CATID_CcProjectBrowse = "{EE8299CB-19B6-4F20-ABEA-E1FD9A33B683}";
        const string CATID_GenericProject = "{610D4611-D0D5-11D2-8599-006097C68E81}";
        #endregion

        int[] _cookies;

        public AnkhExtenderProvider(IAnkhServiceProvider context)
            : base(context)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // This will call Initialize() as postback command
            IAnkhCommandService cs = GetService<IAnkhCommandService>();

            if (cs != null)
            {
                cs.PostExecCommand(AnkhCommand.ActivateVsExtender); // Delay this until after loading the package
            }
        }

        internal void Initialize()
        {
            EnvDTE._DTE dte = GetService<EnvDTE._DTE>();
            EnvDTE.ObjectExtenders extenders;
            if (dte != null && ((extenders = dte.ObjectExtenders) != null))
            {
                _cookies = new int[CATIDS.Length];
                int n = 0;
                foreach (string catid in CATIDS)
                {
                    string cid = new Guid(catid).ToString("B");

                    _cookies[n++] = extenders.RegisterExtenderProvider(cid, ServiceName, this, ServiceName);
                }
            }
        }

        public void Dispose()
        {
            if (_cookies == null)
                return;
            EnvDTE._DTE dte = GetService<EnvDTE._DTE>();
            EnvDTE.ObjectExtenders extenders;
            if (dte != null && ((extenders = dte.ObjectExtenders) != null))
                foreach (int cookie in _cookies)
                {
                    extenders.UnregisterExtenderProvider(cookie);
                }

            _cookies = null;
        }

        IFileStatusCache _cache;
        IFileStatusCache FileStatusCache
        {
            get { return _cache ?? (_cache = GetService<IFileStatusCache>()); }
        }

        internal SvnItem FindItem(object extendeeObject, string catId)
        {
            if (extendeeObject == null)
                return null;
            try
            {
                string path = null;
                Type type = extendeeObject.GetType();
                switch (catId)
                {
                    case CATID_CscFileBrowse:
                    case CATID_CscFolderBrowse:
                    case CATID_CscProjectBrowse:
                    case CATID_VbFileBrowse:
                    case CATID_VbFolderBrowse:
                    case CATID_VbProjectBrowse:
                    case CATID_CcFileBrowse:
                        path = type.InvokeMember("FullPath", BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, extendeeObject, null) as string;
                        break;
                    case CATID_CcProjectBrowse:
                        path = type.InvokeMember("ProjectFile", BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, extendeeObject, null) as string;
                        break;
                    case CATID_SolutionBrowse:
                        path = GetService<IAnkhSolutionSettings>().SolutionFilename;
                        break;
                    default:
                        // Currently untested project types
                        path = type.InvokeMember("FullPath", BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, extendeeObject, null) as string;
                        break;
                }

                if (!string.IsNullOrEmpty(path) && SvnItem.IsValidPath(path))
                {
                    SvnItem i = FileStatusCache[path];
                    if (!i.IsVersioned && !i.IsVersionable)
                        return null;
                    else
                        return i;
                }
            }
            catch
            {
            }
            return null;
        }

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            SvnItem i = FindItem(ExtendeeObject, ExtenderCATID);
            return i != null;
        }

        [CLSCompliant(false)]
        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, EnvDTE.IExtenderSite ExtenderSite, int Cookie)
        {
            return new SvnItemExtender(ExtendeeObject, this, ExtenderSite, Cookie, ExtenderCATID);
        }

        private readonly static string[] CATIDS = new string[]{
        CATID_CscFileBrowse,
        CATID_CscFolderBrowse,
        CATID_CscProjectBrowse,
        CATID_VbFileBrowse,
        CATID_VbFolderBrowse,
        CATID_VbProjectBrowse,
        CATID_VjFileBrowse,
        CATID_VjFolderBrowse,
        CATID_VjProjectBrowse,
        CATID_SolutionBrowse,
        CATID_CcFileBrowse,
        CATID_CcProjectBrowse,
        CATID_GenericProject
        };
    }
}
