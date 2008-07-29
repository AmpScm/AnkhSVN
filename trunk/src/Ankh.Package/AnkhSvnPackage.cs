﻿// VsPkg.cs : Implementation of AnkhSvn
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
using Ankh.Ids;
using Ankh.UI.Services;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI;
using Ankh.VSPackage.Attributes;

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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]

    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", AnkhId.PlkVersion, AnkhId.PlkProduct, AnkhId.PlkCompany, 1)]
    [Guid(AnkhId.PackageId)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)] // The number must match the number in the .csproj file for the ctc task

    [ProvideKeyBindingTable(AnkhId.LogViewContext, 501)]
    [ProvideKeyBindingTable(AnkhId.DiffMergeViewContext, 502)]
    //[ProvideKeyBindingTable(AnkhId.PendingChangeViewContext, 503)] // Won't work at this time
    [ProvideKeyBindingTable(AnkhId.SccExplorerViewContext, 504)]

    [CLSCompliant(false)]
    [ProvideSourceControlProvider("AnkhSVN - Subversion Support for Visual Studio", "#100")]
    [ProvideService(typeof(ITheAnkhSvnSccProvider), ServiceName="AnkhSVN SubversionScc")]    
    sealed partial class AnkhSvnPackage : Package, IAnkhPackage
    {
        AnkhRuntime _runtime;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public AnkhSvnPackage()
        {
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation        

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();                        

            if (InCommandLineMode)
            {
                Trace.WriteLine("Ankh.Package: Skipping package initialization. (VS Running in commandline mode)");
                return; // Do nothing; speed up devenv /setup by not loading all our modules!
            }

            Trace.WriteLine("AnkhSVN: Loading package");

            InitializeRuntime(); // Moved to function of their own to speed up devenv /setup
        }

        void InitializeRuntime()
        {
            IServiceContainer container = GetService<IServiceContainer>();
            container.AddService(typeof(IAnkhPackage), this);

            _runtime = new AnkhRuntime(this); // 
            _runtime.AddModule(new AnkhModule(_runtime));
            _runtime.AddModule(new AnkhSccModule(_runtime));
            _runtime.AddModule(new AnkhVSModule(_runtime));
            _runtime.AddModule(new AnkhUIModule(_runtime));

            _runtime.Start();

            NotifyLoaded();
        }

        private void NotifyLoaded()
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
                Guid gAnkhLoaded = new Guid(AnkhId.AnkhLoadCompleted);

                uint cky;
                if(ErrorHandler.Succeeded(ms.GetCmdUIContextCookie(ref gAnkhLoaded, out cky)))
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
