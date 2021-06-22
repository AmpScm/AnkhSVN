using System;
using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace Ankh.VSPackage
{
    // Either 'AnkhSvnPackage.LegacyPackage.cs' or 'AnkhSvnPackage.AsyncPackage.cs' is compiled,
    // make sure these files stay in sync

    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    [ProvideAutoLoad(AnkhId.SccProviderId)] // Load on 'Scc active' for Subversion
    public partial class AnkhSvnPackage : Package
    {
        protected override void Initialize()
        {
            if (InCommandLineMode)
                return; // Do nothing; speed up devenv /setup by not loading all our modules!

            InitializeRuntime(); // Moved to function of their own to speed up devenv /setup
            RegisterAsOleComponent();
        }
    }
}
