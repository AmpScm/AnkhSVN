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

// VsPkg.cs : Implementation of Ankh_Trigger
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

using Ankh.Ids;
using Ankh.Trigger.Hooks;

namespace Ankh.Trigger
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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    //
    [ProvideAutoLoad("adfc4e64-0397-11d1-9f4e-00a0c911004f")]   // Load on 'no solution'
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]   // Load on 'solution'
    [ProvideAutoLoad(AnkhId.SccProviderId)] // Load on 'Scc active'
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", AnkhId.TriggerPlkVersion, AnkhId.TriggerPlkProduct, AnkhId.TriggerPlkCompany, 1)]
    [Guid(AnkhId.TriggerPackageId)]
    sealed class TriggerPackage : Package, IVsShellPropertyEvents, IOleCommandTarget
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TriggerPackage()
        {
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return GetService<T>(typeof(T));
        }

        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return GetService(typeof(T)) as T;
        }


        uint _shellCookie;
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (InCommandLineMode)
            {
                Trace.WriteLine("Ankh.Trigger: Skipping package initialization. (VS Running in commandline mode)");
                return;
            }

            IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

            if (shell != null)
            {
                shell.AdviseShellPropertyChanges(this, out _shellCookie);

                EnsureHooks(); // Will unregister the advise if no longer needed
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                ReleaseShellHook();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        #region IVsShellPropertyEvents Members

        bool _loaded;
        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                bool value = (bool)var;

                if (value == false)
                {
                    Load();
                }
            }
            return VSConstants.S_OK;
        }

        void Load()
        {
            if (_loaded)
                return;
            _loaded = true;

            EnsureHooks();

            EnsureMigration();

            ReleaseShellHook();

            _filter.Load();
        }

        const string MigrateId = "MigrateId";
        private void EnsureMigration()
        {
            using (RegistryKey rkRoot = this.UserRegistryRoot)
            using (RegistryKey ankhMigration = rkRoot.CreateSubKey("AnkhSVN-Trigger"))
            {
                int migrateFrom = 0;
                object value = ankhMigration.GetValue(MigrateId, migrateFrom);

                if (value is int)
                    migrateFrom = (int)value;
                else
                    ankhMigration.DeleteValue(MigrateId, false);

                if (migrateFrom < 0)
                    migrateFrom = 0;

                if (migrateFrom >= AnkhId.MigrateVersion)
                    return; // Nothing to do

                try
                {
                    OleMenuCommandService mcs = new OleMenuCommandService(this);
                    if (mcs.GlobalInvoke(new CommandID(new Guid(AnkhId.CommandSet), unchecked((int)AnkhCommand.MigrateSettings)), migrateFrom))
                    {
                        ankhMigration.SetValue(MigrateId, AnkhId.MigrateVersion);
                    }
                }
                catch
                { /* NOP: Don't fail here... ever! */}
            }
        }

        private void ReleaseShellHook()
        {
            if (_shellCookie != 0)
            {
                IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

                if (shell != null)
                {
                    shell.UnadviseShellPropertyChanges(_shellCookie);
                    _shellCookie = 0;
                }
            }
        }

        #endregion

        SelectionFilter _filter;

        private void EnsureHooks()
        {
            if (_filter != null)
                return;

            IVsMonitorSelection monitorSelection = GetService<IVsMonitorSelection>();

            if (monitorSelection != null)
            {
                _filter = new SelectionFilter(this, monitorSelection);
            }

        }


        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        #endregion

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
                        if (ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_IsInCommandLineMode, out value)))
                        {
                            _inCommandLineMode = Convert.ToBoolean(value);
                        }
                    }
                }

                return _inCommandLineMode.Value;
            }
        }
    }
}
