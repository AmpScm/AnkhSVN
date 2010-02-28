// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using Ankh.Selection;
using Ankh.Commands;
using Ankh.Scc;


namespace Ankh.VS.Extenders
{
    /// <summary>
    /// This is the class factory for extender objects
    /// </summary>
    [GlobalService(typeof(AnkhExtenderProvider))]
    [GlobalService(typeof(Ankh.AnkhCatId.IAnkhInternalExtenderProvider), true)]
    [ComVisible(true)] // Required for promotion
    public sealed class AnkhExtenderProvider : AnkhService, AnkhCatId.IAnkhInternalExtenderProvider
    {
        public AnkhExtenderProvider(IAnkhServiceProvider context)
            : base(context)
        {
        }

        bool _solutionOpen;
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            AnkhServiceEvents events = GetService<AnkhServiceEvents>();
            events.SccProviderActivated += new EventHandler(OnSccProviderActivated);
            events.SccProviderDeactivated += new EventHandler(OnSccProviderDeactivated);
            events.SolutionOpened += new EventHandler(OnSolutionOpened);
            events.SolutionClosed += new EventHandler(OnSolutionClosed);
            events.Disposed += new EventHandler(OnRuntimeDisposed);
        }

        void OnRuntimeDisposed(object sender, EventArgs e)
        {
            _enabled = false;
        }

        void OnSolutionClosed(object sender, EventArgs e)
        {
            _solutionOpen = false;
        }

        void OnSolutionOpened(object sender, EventArgs e)
        {
            _solutionOpen = true;
        }

        bool _enabled;
        void OnSccProviderDeactivated(object sender, EventArgs e)
        {
            _enabled = false;
        }

        void OnSccProviderActivated(object sender, EventArgs e)
        {
            _enabled = true;
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
                    case AnkhCatId.CscFileBrowse:
                    case AnkhCatId.CscFolderBrowse:
                    case AnkhCatId.CscProjectBrowse:
                    case AnkhCatId.VbFileBrowse:
                    case AnkhCatId.VbFolderBrowse:
                    case AnkhCatId.VbProjectBrowse:
                    case AnkhCatId.CppFileBrowse:
                        path = type.InvokeMember("FullPath", BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, extendeeObject, null) as string;
                        break;
                    case AnkhCatId.CppProjectBrowse:
                        path = type.InvokeMember("ProjectFile", BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, extendeeObject, null) as string;
                        break;
                    case AnkhCatId.SolutionBrowse:
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

        #region IAnkhInternalExtenderProvider Members

        public bool CanExtend(object extendeeObject, string catId)
        {
            if (!_enabled || !_solutionOpen)
                return false;

            SvnItem i = FindItem(extendeeObject, catId);
            return i != null;
        }

        public object GetExtender(object extendeeObject, string catId, IDisposable disposer)
        {
            switch (catId)
            {
                case AnkhCatId.SolutionBrowse:
                    return new SvnSolutionExtender(extendeeObject, this, disposer, catId);
                case AnkhCatId.CscProjectBrowse:
                case AnkhCatId.VbProjectBrowse:
                case AnkhCatId.VjProjectBrowse:
                case AnkhCatId.CppProjectBrowse:
                case AnkhCatId.GenericProject:
                case AnkhCatId.ExtProjectBrowse:
                    return new SvnProjectExtender(extendeeObject, this, disposer, catId);
                default:
                    return new SvnItemExtender(extendeeObject, this, disposer, catId);
            }
        }

        #endregion
    }
}
