using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.UI
{
    /// <summary>
    /// Public api of the ankh package as used by other components
    /// </summary>
    public interface IAnkhPackage : IAnkhServiceProvider, System.ComponentModel.Design.IServiceContainer
    {
        /// <summary>
        /// Gets the UI version. Retrieved from the registry after being installed by our MSI
        /// </summary>
        /// <value>The UI version.</value>
        Version UIVersion { get; }

        /// <summary>
        /// Gets the package version. The assembly version of Ankh.Package.dll
        /// </summary>
        /// <value>The package version.</value>
        Version PackageVersion { get; }

        void ShowToolWindow(AnkhToolWindow window);
        void ShowToolWindow(AnkhToolWindow window, int id, bool create);
    }
}
