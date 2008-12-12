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
        const string CATID_CscFileBrowse = "8d58e6af-ed4e-48b0-8c7b-c74ef0735451";
        const string CATID_CscFolderBrowse = "914fe278-054a-45db-bf9e-5f22484cc84c";
        const string CATID_CscProjectBrowse = "4ef9f003-de95-4d60-96b0-212979f2a857";
        const string CATID_VbFileBrowse = "ea5bd05d-3c72-40a5-95a0-28a2773311ca";
        const string CATID_VbFolderBrowse = "932dc619-2eaa-4192-b7e6-3d15ad31df49";
        const string CATID_VbProjectBrowse = "e0fdc879-c32a-4751-a3d3-0b3824bd575f";
        const string CATID_VjFileBrowse = "e6fdf869-f3d1-11d4-8576-0002a516ece8";
        const string CATID_VjFolderBrowse = "e6fdf86a-f3d1-11d4-8576-0002a516ece8";
        const string CATID_VjProjectBrowse = "e6fdf86c-f3d1-11d4-8576-0002a516ece8";
        const string CATID_SolutionBrowse = "a2392464-7c22-11d3-bdca-00c04f688e50";
        const string CATID_CcFileBrowse = "ee8299c9-19b6-4f20-abea-e1fd9a33b683";
        const string CATID_CcProjectBrowse = "ee8299cb-19b6-4f20-abea-e1fd9a33b683";
        const string CATID_GenericProject = "610d4611-d0d5-11d2-8599-006097c68e81";
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

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            ISelectionContext selection = GetService<ISelectionContext>();

            if (selection != null)
            {
                bool first = true;
                foreach (SvnItem item in selection.GetSelectedSvnItems(false))
                {
                    if (!item.IsVersioned && !item.IsVersionable)
                        return false;
                    else if (first)
                        first = false;
                    else
                        return true;
                }

                if (!first)
                    return true;
            }

            return false;
        }

        [CLSCompliant(false)]
        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, EnvDTE.IExtenderSite ExtenderSite, int Cookie)
        {
            ISelectionContext selection = GetService<ISelectionContext>();

            if (selection != null)
            {
                SvnItem selected = null;

                foreach (SvnItem item in selection.GetSelectedSvnItems(false))
                {
                    if (!item.IsVersioned && !item.IsVersionable)
                        return false;

                    selected = item;
                    break;
                }

                if (selected == null)
                    return null;

                return new SvnItemExtender(selected, Context);
            }

            return null;
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
