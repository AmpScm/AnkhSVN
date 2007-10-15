// VsPkg.cs : Implementation of Ankh_VSPackage
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

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
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    // 
    [ProvideAutoLoad(GuidList.guidAnkh_VSPackagePkgString)]
    [ProvideLoadKey("Standard", "1.0", "AnkhSVN VSPackage", "Ankh", 1)]
    [Guid(GuidList.guidAnkh_VSPackagePkgString)]
    [ProvideSourceControlProvider( "Ankh Source Control Provider", "#100" )]
    [ProvideService( typeof( SccProviderService ), ServiceName = "Ankh Source Control Provider Service" )]
    [ProvideService(typeof(AnkhVSService))]
    public sealed class Ankh_VSPackage : Package, IVsSccGlyphs
    {

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public Ankh_VSPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        //public IContext Context
        //{
        //    get { return this.context; }
        //    set { this.context = context; }
        //}



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            SccProviderService service = new SccProviderService();
            ( (IServiceContainer)this ).AddService( typeof( SccProviderService ), service, true );

            AnkhVSService ankhService = new AnkhVSService(this, service);
            ((IServiceContainer)this).AddService(typeof(AnkhVSService), ankhService, true);

            IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)GetService( typeof( IVsRegisterScciProvider ) );
            rscp.RegisterSourceControlProvider( GuidList.guidAnkhSccProviderService );

        }

        public int GetCustomGlyphList( uint BaseIndex, out uint pdwImageListHandle )
        {
            pdwImageListHandle = (uint)StatusImages.StatusImageList.Handle.ToInt32();

            return VSConstants.S_OK;
        }
    }
}
