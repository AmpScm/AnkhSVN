// VsPkg.cs : Implementation of AnkhSvn
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
using AnkhSvn.Ids;
using Ankh.UI.Services;
using Ankh.VSPackage.Scc;

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
	[ProvideLoadKey("Standard", "2.0", "AnkhSvn", "AnkhSvn", 1)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource(1000, 1)]
	[Guid(AnkhId.PackageId)]
	[CLSCompliant(false)]
	[ProvideSourceControlProvider("AnkhSVN Source Control Provider", "#100")]
	[ProvideService(typeof(AnkhSccProvider), ServiceName = "AnkhSVN - Subversion Source Control Provider Service")]
    [ProvideAutoLoad("F1536EF8-92EC-443C-9ED7-FDADF150DA82")] // = VSConstants.UICONTEXT_SolutionExists.ToString()
	public sealed partial class AnkhSvnPackage : Package, IAnkhPackage
	{
		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public AnkhSvnPackage()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
		}



		/////////////////////////////////////////////////////////////////////////////
		// Overriden Package Implementation
		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			IServiceContainer container = (IServiceContainer)GetService(typeof(IServiceContainer));
			container.AddService(typeof(IAnkhPackage), this, true);

			Debug.Assert(container != null, "Service container available");

            AnkhSccProvider service = new AnkhSccProvider(this.AnkhContext);
            container.AddService(typeof(AnkhSccProvider), service, true);
			container.AddService(typeof(IAnkhSccService), service, true);

			IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)GetService(typeof(IVsRegisterScciProvider));
            if (rscp != null)
            {
                rscp.RegisterSourceControlProvider(AnkhId.SccProviderGuid);
            }


			// container.AddService(.., ..., true)
			// container.AddService(.., new ServiceCreatorCallback(..), true) // Delayed creation of the service
		}
		#endregion
	}
}
