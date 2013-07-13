// $Id$
//
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
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace Ankh.VSPackage.Attributes
{
    public enum SccProviderCommand
    {
        Open,
        Share
    }
    /// <summary>
    /// This attribute registers the package as source control provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideSourceControlCommandAttribute : RegistrationAttribute
    {
        readonly string _providerId;
        readonly SccProviderCommand _command;
        readonly Guid _commandGroup;
        readonly int _commandId;

        /// <summary>
        /// </summary>
        public ProvideSourceControlCommandAttribute(string providerId, SccProviderCommand command, Guid commandGroup, int commandId)
        {
            _providerId = providerId;
            _command = command;
            _commandGroup = commandGroup;
            _commandId = commandId;
        }

        public ProvideSourceControlCommandAttribute(string providerId, SccProviderCommand command, object commandValue)
            : this(providerId, command, commandValue.GetType().GUID, GetEnumValue(commandValue))
        {
        }

        private static int GetEnumValue(object commandValue)
        {
            if (commandValue == null)
                return 0;

            Type enumType = commandValue.GetType();
            Type undertype = Enum.GetUnderlyingType(enumType);

            commandValue  = Convert.ChangeType(commandValue, undertype);

            return (int)commandValue;
        }

        /// <summary>
        /// Get the unique guid identifying the provider
        /// </summary>
        public Guid RegGuid
        {
            get { return new Guid(_providerId); }
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        public Guid UINamePkg
        {
            get { return AnkhId.PackageGuid; }
        }

        /// <summary>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     It also contains other information such as the type being registered and path information.
        /// </summary>
        public override void Register(RegistrationContext context)
        {
            // Declare the source control provider, its name, the provider's service 
            // and aditionally the packages implementing this provider
            using (Key sccProviders = context.CreateKey("SourceControlProviders"))
            {
                using (Key sccProviderKey = sccProviders.CreateSubkey(RegGuid.ToString("B")))
                {
                    using (Key sccProviderNameKey = sccProviderKey.CreateSubkey("Commands"))
                    {
                        sccProviderNameKey.SetValue(
                            string.Format("{0}Command", _command),
                            string.Format("{0:B}|{1}", _commandGroup, _commandId));

                        sccProviderNameKey.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Unregister the source control provider
        /// </summary>
        /// <param name="context"></param>
        public override void Unregister(RegistrationContext context)
        {
        }
    }
}
