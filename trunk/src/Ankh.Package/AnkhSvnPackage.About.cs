using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Reflection;
using SharpSvn;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage
{
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(true, null, null, null)]
    public partial class AnkhSvnPackage : IVsInstalledProduct
    {
        #region IVsInstalledProduct Members

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0; // Not used by VS2005+
            return VSConstants.E_NOTIMPL;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 400;
            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName)
        {
            if (InCommandLineMode)
            {
                // We are running in /setup. The text is cached for the about box
                pbstrName = Resources.AboutTitleNameShort;
            }
            else
            {
                // We are running with full UI. Probably used for the about box
                pbstrName = Resources.AboutTitleName;
            }
            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = string.Format(Resources.AboutDetails,
                new AssemblyName(typeof(AnkhSvnPackage).Assembly.FullName).Version,
                SvnClient.Version,
                SvnClient.SharpSvnVersion);

            return VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = new AssemblyName(typeof(AnkhSvnPackage).Assembly.FullName).Version.ToString();

            return VSConstants.S_OK;
        }

        #endregion
    }
}
