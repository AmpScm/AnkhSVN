// Copyright 2008-2009 The AnkhSVN Project
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
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Reflection;
using SharpSvn;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using Ankh.Configuration;
using Ankh.UI;

namespace Ankh.VSPackage
{
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(true, null, null, null)]
    [Obsolete]
    public partial class AnkhSvnPackage : IVsInstalledProduct
    {
        Version _packageVersion;

        /// <summary>
        /// Gets the package version. The assembly version of Ankh.Package.dll
        /// </summary>
        /// <value>The package version.</value>
        public Version PackageVersion
        {
            get { return _packageVersion ?? (_packageVersion = typeof(AnkhSvnPackage).Assembly.GetName().Version); }
        }

        #region IVsInstalledProduct Members

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0; // Not used by VS2005+
            return VSErr.E_NOTIMPL;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 400;
            return VSErr.S_OK;
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
            return VSErr.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            StringBuilder sb = new StringBuilder();

            string svnVersion = SvnClient.VersionString;
            if (svnVersion.EndsWith("-SharpSvn"))
                svnVersion = svnVersion.Substring(0, svnVersion.Length - 9);

            sb.AppendFormat(Resources.AboutDetails,
                PackageVersion,
                svnVersion,
                SvnClient.SharpSvnVersion);

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat(Resources.AboutLinkedTo, "SharpSvn");
            foreach (SharpSvn.Implementation.SvnLibrary lib in SvnClient.SvnLibraries)
            {
                if (!lib.Optional)
                {
                    sb.AppendFormat("{0} {1}", lib.Name, lib.VersionString);
                    sb.Append(", ");
                }
            }

            sb.Length -= 2;
            sb.AppendLine();

            bool has = false;
            foreach (SharpSvn.Implementation.SvnLibrary lib in SvnClient.SvnLibraries)
            {
                if (lib.Optional)
                {
                    if (!has)
                    {
                        has = true;
                        sb.AppendFormat(Resources.AboutOptionallyLinkedTo, "SharpSvn");
                    }

                    sb.AppendFormat("{0} {1}", lib.Name, lib.VersionString);
                    sb.Append(", ");
                }
            }

            sb.Length -= 2;

            pbstrProductDetails = sb.ToString();

            return VSErr.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = PackageVersion.ToString();

            return VSErr.S_OK;
        }

        #endregion
    }
}
