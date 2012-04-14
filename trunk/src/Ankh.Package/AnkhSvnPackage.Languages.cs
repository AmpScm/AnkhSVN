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
using Microsoft.VisualStudio.Shell;

using Ankh.VS.LanguageServices.Core;
using Ankh.VS.LanguageServices.LogMessages;
using Ankh.VS.LanguageServices.UnifiedDiff;
using Ankh.VSPackage.Attributes;

namespace Ankh.VSPackage
{
    [ProvideLanguageService(typeof(LogMessageLanguage), LogMessageLanguage.ServiceName, 301,
        DefaultToInsertSpaces = true,
        EnableLineNumbers = true,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        MaxErrorMessages = 10,
        RequestStockColors = true,
        ShowHotURLs = true,
        ShowMatchingBrace = true,
        SingleCodeWindowOnly = true)]
    [ProvideLanguageSettings(typeof(LogMessageLanguage), LogMessageLanguage.ServiceName, LogMessageLanguage.RegistryName, LogMessageLanguage.ServiceName, 305)]
    [ProvideAutomationObject(LogMessageLanguage.RegistryName, Description = LogMessageLanguage.ServiceName)]
    [ProvideService(typeof(LogMessageLanguage), ServiceName = AnkhId.LogMessageServiceName)]
    [ProvideLanguageService(typeof(UnifiedDiffLanguage), UnifiedDiffLanguage.ServiceName, 304,
        CodeSense = false,
        ShowDropDownOptions = true,
        RequestStockColors = true)]
    [ProvideLanguageSettings(typeof(UnifiedDiffLanguage), UnifiedDiffLanguage.ServiceName, UnifiedDiffLanguage.RegistryName, UnifiedDiffLanguage.ServiceName, 306)]
    [ProvideAutomationObject(UnifiedDiffLanguage.RegistryName, Description = UnifiedDiffLanguage.ServiceName)]
    [ProvideLanguageExtension(typeof(UnifiedDiffLanguage), ".patch")]
    [ProvideLanguageExtension(typeof(UnifiedDiffLanguage), ".diff")]
    [ProvideService(typeof(UnifiedDiffLanguage), ServiceName = AnkhId.UnifiedDiffServiceName)]
    partial class AnkhSvnPackage
    {
        protected override object GetAutomationObject(string name)
        {
            object obj = base.GetAutomationObject(name);
            if (obj != null || name == null)
                return obj;

            // Look for setting objects that must be accessible by their automation name for setting persistence.

            System.ComponentModel.AttributeCollection attributes = System.ComponentModel.TypeDescriptor.GetAttributes(this);
            foreach (Attribute attr in attributes)
            {
                ProvideLanguageSettingsAttribute ps = attr as ProvideLanguageSettingsAttribute;

                if (ps != null && name == ps.Name)
                {
                    AnkhLanguage language = GetService<AnkhLanguage>(ps.Type);

                    if (language != null)
                        obj = language.LanguagePreferences;

                    if (obj != null)
                        return obj;
                }
            }

            return null;
        }
    }
}
