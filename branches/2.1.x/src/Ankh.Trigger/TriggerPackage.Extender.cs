// $Id: TriggerPackage.cs 7840 2010-02-26 13:22:09Z rhuijben $
//
// Copyright 2010 The AnkhSVN Project
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
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace Ankh.Trigger
{
    [ProvideExtender(AnkhCatId.CscFileBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.CscFolderBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.CscProjectBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VbFileBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VbFolderBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VbProjectBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VjFileBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VjFolderBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.VjProjectBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.SolutionBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.CppFileBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.CppProjectBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.GenericProject, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.ExtProjectBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideExtender(AnkhCatId.ExtFileBrowse, AnkhId.TriggerExtenderGuid, AnkhId.TriggerExtenderName)]
    [ProvideObject(typeof(TriggerExtenderHandler))]
    partial class TriggerPackage
    {
        readonly Dictionary<IDisposable, IDisposable> _disposers = new Dictionary<IDisposable, IDisposable>();

        internal void RemoveDispose(IDisposable value)
        {
            _disposers.Remove(value);
        }

        internal void AddDispose(IDisposable value)
        {
            _disposers[value] = value;
        }

        void DisposeComHandles()
        {
            List<IDisposable> d = new List<IDisposable>(_disposers.Values);
            _disposers.Clear();

            foreach (IDisposable dd in d)
            {
                dd.Dispose();
            }
        }
    }

    // This class is created via its guid
    [Guid(AnkhId.TriggerExtenderGuid), ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(EnvDTE.IExtenderProviderUnk))]
    sealed class TriggerExtenderHandler : EnvDTE.IExtenderProviderUnk, EnvDTE.IExtenderProvider
    {
        public TriggerExtenderHandler()
        {
        }
        #region IExtenderProviderUnk Members

        TriggerPackage _tp;

        TriggerPackage TriggerPackage
        {
            get { return _tp ?? (_tp = (TriggerPackage)Package.GetGlobalService(typeof(TriggerPackage))); }
        }

        AnkhCatId.IAnkhInternalExtenderProvider _extender;

        AnkhCatId.IAnkhInternalExtenderProvider Extender
        {
            get
            {
                if (_extender != null)
                    return _extender;

                TriggerPackage tp = TriggerPackage;

                if (tp == null || !tp.SccProviderLoaded)
                    return null;

                _extender = tp.GetService<AnkhCatId.IAnkhInternalExtenderProvider>();

                return _extender;
            }
        }

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            AnkhCatId.IAnkhInternalExtenderProvider extender = Extender;

            if (extender != null)
                return extender.CanExtend(ExtendeeObject, ExtenderCATID);

            return false;
        }

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, EnvDTE.IExtenderSite ExtenderSite, int Cookie)
        {
            AnkhCatId.IAnkhInternalExtenderProvider extender = Extender;

            IDisposable disposer = new ExtenderDisposer(TriggerPackage, ExtenderSite, Cookie);

            if (extender != null)
                return extender.GetExtender(ExtendeeObject, ExtenderCATID, new ExtenderDisposer(TriggerPackage, ExtenderSite, Cookie));

            return null;
        }

        sealed class ExtenderDisposer : IDisposable
        {
            TriggerPackage _package;
            IExtenderSite _site;
            int? _cookie;

            public ExtenderDisposer(TriggerPackage package, IExtenderSite site, int cookie)
            {
                _package = package;
                _site = site;
                _cookie = cookie;
                package.AddDispose(this);
            }

            private void Dispose(bool dispose)
            {
                if (_cookie.HasValue && !_package.Zombied && _site != null)
                    try
                    {
                        _package.RemoveDispose(this);
                        DoDispose();
                    }
                    catch (Exception e)
                    {
                        Debug.Write("Failure disposing extender: " + e.Message);
                    }
                    finally
                    {
                        _cookie = null;
                    }
                _site = null;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            [DebuggerNonUserCode]
            private void DoDispose()
            {
                _site.NotifyDelete(_cookie.Value);
            }
        }

        #endregion
    }
}
