﻿// Copyright 2008-2009 The AnkhSVN Project
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

using Ankh.Scc;
using Ankh.VS;
using Ankh.UI;
using Ankh.VSPackage.Attributes;
using Ankh.GitScc;
using Ankh.WpfUI;
using System.Collections.Generic;

namespace Ankh.VSPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading =true)]
    [Description(AnkhId.PackageDescription)]

    [Guid(AnkhId.PackageId)]
    [ProvideAutoLoad(AnkhId.SccProviderId, PackageAutoLoadFlags.BackgroundLoad)] // Load on 'Scc active' for Subversion
    [ProvideAutoLoad(AnkhId.GitSccProviderId, PackageAutoLoadFlags.BackgroundLoad)] // Load on 'Scc active' for Git

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("1000.ctmenu", 1)] // The numbers must match the number in the .csproj file for the ctc task

    [ProvideKeyBindingTable(AnkhId.LogViewContext, 501)]
    [ProvideKeyBindingTable(AnkhId.DiffMergeViewContext, 502)]
    //[ProvideKeyBindingTable(AnkhId.PendingChangeViewContext, 503)] // Won't work at this time
    [ProvideKeyBindingTable(AnkhId.SccExplorerViewContext, 504)]

    [ProvideAnkhExtensionRedirect()]

    [CLSCompliant(false)]
    [ProvideOutputWindow(AnkhId.AnkhOutputPaneId, "#111", InitiallyInvisible = false, Name = AnkhId.PlkProduct, ClearWithSolution = false)]
    sealed partial class AnkhSvnPackage : AsyncPackage, IAnkhPackage, IAnkhQueryService, IAnkhStaticServiceRegistry
    {
        private AnkhRuntime _runtime;
        readonly Dictionary<Type, object> _staticServices = new Dictionary<Type, object>();

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        public AnkhSvnPackage()
        {
            // This function is executed async, so don't initialize here.
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected async override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (InCommandLineMode)
                return; // Do nothing; speed up devenv /setup by not loading all our modules!

            InitializeRuntime(); // Moved to function of their own to speed up devenv /setup
            RegisterAsOleComponent();
        }

        void InitializeRuntime()
        {
            _runtime = new AnkhRuntime(this);
            _runtime.PreLoad();

            IServiceContainer container = GetService<IServiceContainer>();
            container.AddService(typeof(IAnkhPackage), this, true);
            container.AddService(typeof(IAnkhQueryService), this, true);

            _runtime.AddModule(new AnkhModule(_runtime));
            _runtime.AddModule(new AnkhSccModule(_runtime));
            _runtime.AddModule(new AnkhGitSccModule(_runtime));
            _runtime.AddModule(new AnkhVSModule(_runtime));
            _runtime.AddModule(new AnkhUIModule(_runtime));
            _runtime.AddModule(new AnkhWpfUIModule(_runtime));

            RegisterEditors();

            NotifyLoaded(false);

            _runtime.Start();

            NotifyLoaded(true);
        }

        private void NotifyLoaded(bool started)
        {
            // We set the user context AnkhLoadCompleted active when we are loaded
            // This event can be used to trigger loading other packages that depend on AnkhSVN
            //
            // When the use:
            // [ProvideAutoLoad(AnkhId.AnkhLoadCompleted)]
            // On their package, they load automatically when we are completely loaded
            //

            IVsMonitorSelection ms = GetService<IVsMonitorSelection>();
            if (ms != null)
            {
                Guid gAnkhLoaded = new Guid(started ? AnkhId.AnkhRuntimeStarted : AnkhId.AnkhServicesAvailable);

                uint cky;
                if (VSErr.Succeeded(ms.GetCmdUIContextCookie(ref gAnkhLoaded, out cky)))
                {
                    ms.SetCmdUIContext(cky, 1);
                }
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public AnkhContext Context
        {
            get { return _runtime.Context; }
        }

        bool? _inCommandLineMode;
        /// <summary>
        /// Get a boolean indicating whether we are running in commandline mode
        /// </summary>
        public bool InCommandLineMode
        {
            get
            {
                if (!_inCommandLineMode.HasValue)
                {
                    IVsShell shell = (IVsShell)GetService(typeof(SVsShell));

                    if (shell == null)
                        _inCommandLineMode = false; // Probably running in a testcase; the shell loads us!
                    else
                    {
                        object value;
                        if (VSErr.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_IsInCommandLineMode, out value)))
                        {
                            _inCommandLineMode = Convert.ToBoolean(value);
                        }
                    }
                }

                return _inCommandLineMode.Value;
            }
        }

        #region IAnkhQueryService Members

        static Guid IID_IUnknown = VSConstants.IID_IUnknown;

        public T QueryService<T>(Guid serviceGuid) where T : class
        {
            IOleServiceProvider sp = GetService<IOleServiceProvider>();
            IntPtr handle;

            if (sp == null)
                return null;

            if (!VSErr.Succeeded(sp.QueryService(ref serviceGuid, ref IID_IUnknown, out handle))
                || handle == IntPtr.Zero)
                return null;

            try
            {
                object ob = Marshal.GetObjectForIUnknown(handle);

                return ob as T;
            }
            finally
            {
                Marshal.Release(handle);
            }
        }

        protected override object GetService(Type serviceType)
        {
            try
            {
                return base.GetService(serviceType);
            }
            catch(InvalidOperationException)
            {
                if (serviceType == typeof(IAnkhServiceProvider)
                    || serviceType == typeof(IAnkhQueryService))
                {
                    return this;
                }
                else if (_staticServices.TryGetValue(serviceType, out var v))
                {
                    return v;
                }

                throw;
            }
        }

        void IAnkhStaticServiceRegistry.AddStaticService(Type type, object instance, bool promote)
        {
            ((IServiceContainer)this).AddService(type, instance, promote);
            _staticServices[type] = instance;
        }

        #endregion
    }
}
