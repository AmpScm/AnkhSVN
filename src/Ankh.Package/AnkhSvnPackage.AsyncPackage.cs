using System;
using Microsoft.VisualStudio.Shell;
using System.Threading;

namespace Ankh.VSPackage
{
    // Either 'AnkhSvnPackage.LegacyPackage.cs' or 'AnkhSvnPackage.AsyncPackage.cs' is compiled,
    // make sure these files stay in sync

    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(AnkhId.SccProviderId, PackageAutoLoadFlags.BackgroundLoad)] // Load on 'Scc active' for Subversion
    public partial class AnkhSvnPackage : AsyncPackage
    {

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

        protected override object GetService(Type serviceType)
        {
            if (_staticServices.TryGetValue(serviceType, out var v))
            {
                return v;
            }

            try
            {
                return base.GetService(serviceType);
            }
            catch (InvalidOperationException)
            {
                if (serviceType == typeof(IAnkhServiceProvider)
                    || serviceType == typeof(IAnkhQueryService))
                {
                    return this;
                }

                throw;
            }
        }
    }
}
