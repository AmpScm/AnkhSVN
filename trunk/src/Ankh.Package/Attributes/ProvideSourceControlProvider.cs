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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Globalization;
using Ankh.Ids;

namespace Ankh.VSPackage.Attributes
{
    /// <summary>
    /// This attribute registers the package as source control provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideSourceControlProviderAttribute : RegistrationAttribute
    {
        private string _regName = null;
        private string _uiName = null;

        /// <summary>
        /// </summary>
        public ProvideSourceControlProviderAttribute(string regName, string uiName)
        {
            _regName = regName;
            _uiName = uiName;
        }

        /// <summary>
        /// Get the friendly name of the provider (written in registry)
        /// </summary>
        public string RegName
        {
            get { return _regName; }
        }

        /// <summary>
        /// Get the unique guid identifying the provider
        /// </summary>
        public Guid RegGuid
        {
            get { return AnkhId.SccProviderGuid; }
        }

        /// <summary>
        /// Get the UI name of the provider (string resource ID)
        /// </summary>
        public string UIName
        {
            get { return _uiName; }
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        public Guid UINamePkg
        {
            get { return AnkhId.PackageGuid; }
        }

        /// <summary>
        /// Get the guid of the provider's service
        /// </summary>
        public Guid SccProviderService
        {
            get { return typeof(Ankh.Scc.ITheAnkhSvnSccProvider).GUID; }
        }

        /// <summary>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     It also contains other information such as the type being registered and path information.
        /// </summary>
        public override void Register(RegistrationContext context)
        {
            // Write to the context's log what we are about to do
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture,
                "Ankh Source Control Provider:\t\t{0}\n", RegName));

            // Declare the source control provider, its name, the provider's service 
            // and aditionally the packages implementing this provider
            using (Key sccProviders = context.CreateKey("SourceControlProviders"))
            {
                // BH: Set ourselves as current default SCC Provider
                sccProviders.SetValue("", RegGuid.ToString("B"));

                using (Key sccProviderKey = sccProviders.CreateSubkey(RegGuid.ToString("B").ToUpperInvariant()))
                {
                    sccProviderKey.SetValue("", RegName);
                    sccProviderKey.SetValue("Service", SccProviderService.ToString("B").ToUpperInvariant());

                    using (Key sccProviderNameKey = sccProviderKey.CreateSubkey("Name"))
                    {
                        sccProviderNameKey.SetValue("", UIName);
                        sccProviderNameKey.SetValue("Package", UINamePkg.ToString("B").ToUpperInvariant());

                        sccProviderNameKey.Close();
                    }

                    // Additionally, you can create a "Packages" subkey where you can enumerate the dll
                    // that are used by the source control provider, something like "Package1"="BasicSccProvider.dll"
                    // but this is not a requirement.
                    sccProviderKey.Close();
                }

                sccProviders.Close();
            }
        }

        /// <summary>
        /// Unregister the source control provider
        /// </summary>
        /// <param name="context"></param>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey("SourceControlProviders\\" + AnkhId.SccProviderGuid.ToString("B"));
        }
    }
}
