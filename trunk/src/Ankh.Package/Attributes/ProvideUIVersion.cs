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
using MsVsShell = Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage.Attributes
{
	internal sealed class ProvideUIVersionAttribute : MsVsShell.RegistrationAttribute
    {
        public ProvideUIVersionAttribute()
        {
        }

        internal const string RemapName = "AnkhSVN-UI-Version";
        string GetPath(RegistrationAttribute.RegistrationContext context)
        {
            return "Packages\\" + context.ComponentType.GUID.ToString("B").ToUpperInvariant();
        }

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            // Create the visibility key.
            using (Key childKey = context.CreateKey(GetPath(context)))
            {
                // Set the value for the command UI guid.
                childKey.SetValue(RemapName, "[ProductVersion]");
            }
        }

        public override void Unregister(Microsoft.VisualStudio.Shell.RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveValue(GetPath(context), RemapName);
        }
    }
}
