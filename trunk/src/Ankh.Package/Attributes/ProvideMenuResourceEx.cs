// $Id: ProvideLanguageSettings.cs 5699 2008-12-12 13:03:53Z rhuijben $
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

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal sealed class ProvideMenuResourceExAttribute : RegistrationAttribute
    {

        private string _resourceID;
        private int _version;

        /// <include file='doc\ProvideMenuResourceAttribute.uex' path='docs/doc[@for="ProvideMenuResourceAttribute.ProvideMenuResourceAttribute"]' />
        /// <devdoc>
        ///     Creates a new ProvideMenuResourceAttribute.
        /// </devdoc>
        public ProvideMenuResourceExAttribute(short resourceID, int version)
        {
            _resourceID = resourceID.ToString();
            _version = version;
        }

        /// <include file='doc\ProvideMenuResourceAttribute.uex' path='docs/doc[@for="ProvideMenuResourceAttribute.ProvideMenuResourceAttribute"]' />
        /// <devdoc>
        ///     Creates a new ProvideMenuResourceAttribute.
        /// </devdoc>
        public ProvideMenuResourceExAttribute(string resourceID, int version)
        {
            if (string.IsNullOrEmpty(resourceID))
            {
                throw new ArgumentNullException("resourceID");
            }
            _resourceID = resourceID;
            _version = version;
        }

        /// <include file='doc\ProvideMenuResourceAttribute.uex' path='docs/doc[@for="ProvideMenuResourceAttribute.ResourceID"]' />
        /// <devdoc>
        ///     Returns the native resource ID for the menu resource.
        /// </devdoc>
        public string ResourceID
        {
            get
            {
                return _resourceID;
            }
        }

        /// <include file='doc\ProvideMenuResourceAttribute.uex' path='docs/doc[@for="ProvideMenuResourceAttribute.Version"]' />
        /// <devdoc>
        ///     Returns the version of this menu resource.
        /// </devdoc>
        public int Version
        {
            get
            {
                return _version;
            }
        }

        /// <include file='doc\ProvideMenuResourceAttribute.uex' path='docs/doc[@for="Register"]' />
        /// <devdoc>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     it also contains such as the type being registered, and path information.
        ///
        ///     This method is called both for registration and unregistration.  The difference is
        ///     that unregistering just uses a hive that reverses the changes applied to it.
        /// </devdoc>
        public override void Register(RegistrationContext context)
        {
            //context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyMenuResource, ResourceID, Version));

            using (Key childKey = context.CreateKey("Menus"))
            {
                childKey.SetValue(context.ComponentType.GUID.ToString("B"), string.Format(CultureInfo.InvariantCulture, ", {0}, {1}", ResourceID, Version));
            }
        }

        /// <summary>
        ///     Called to unregister this attribute with the given context.
        /// </summary>
        /// <param name="context">
        ///     Contains the location where the registration inforomation should be placed.
        ///     It also contains other informations as the type being registered and path information.
        /// </param>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveValue("Menus", context.ComponentType.GUID.ToString("B"));
        }
    }
}

