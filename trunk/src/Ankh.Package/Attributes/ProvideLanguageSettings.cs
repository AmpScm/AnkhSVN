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
using Ankh.Ids;

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class ProvideLanguageSettingsAttribute : RegistrationAttribute
    {
        readonly string _key;
        readonly string _name;
        readonly int _n, _desc;
        public ProvideLanguageSettingsAttribute(Type type, string key, string name, int nameId, int descriptionId)
        {
            _key = key;
            _name = name;
            _n = nameId;
            _desc = descriptionId;
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        public Guid UINamePkg
        {
            get { return AnkhId.PackageGuid; }
        }

        string GetKey(string key)
        {
            return "AutomationProperties\\TextEditor\\" + key;
        }

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            using(Key key = context.CreateKey(GetKey(_key)))
            {
                key.SetValue("", string.Format("#{0}", _n));
                key.SetValue("Description", string.Format("#{0}", _desc));
                key.SetValue("Name", _name);
                key.SetValue("Package", UINamePkg.ToString("B").ToUpperInvariant());
                key.SetValue("ProfileSave", 1);
                key.SetValue("ResourcePackage", UINamePkg.ToString("B").ToUpperInvariant());
            }            
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(GetKey(_key));
        }
    }
}
