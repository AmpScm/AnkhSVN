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

namespace Ankh.VSPackage.Attributes
{
    /// <summary>
    /// This attribute registers the package as a solution property parser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideSolutionPropertiesAttribute : RegistrationAttribute
    {
        private string _propName;

        public ProvideSolutionPropertiesAttribute(string propName)
        {
            _propName = propName;
        }

        public override void Register(RegistrationContext context)
        {
            context.Log.WriteLine(string.Format(CultureInfo.InvariantCulture, "ProvideSolutionProps: ({0} = {1})", context.ComponentType.GUID.ToString("B"), PropName));

            Key childKey = null;

            try
            {
                childKey = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SolutionPersistence", PropName));

                childKey.SetValue(string.Empty, context.ComponentType.GUID.ToString("B").ToUpperInvariant());
            }
            finally
            {
                if (childKey != null) childKey.Close();
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SolutionPersistence", PropName));
        }

        public string PropName { get { return _propName; } }
    }
}
