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
using Ankh.UI.PendingChanges;
using Ankh.VSPackage.Attributes;
using Ankh.VS.LanguageServices;
using Ankh.Selection;

namespace Ankh.VSPackage
{
    [ProvideLanguageService(typeof(LogMessageLanguageService), LogMessageLanguageService.ServiceName, 301,
        AutoOutlining = false,
        CodeSense = false,
        DefaultToInsertSpaces = true,
        EnableAdvancedMembersOption = false,
        EnableAsyncCompletion = false,
        EnableCommenting = true,
        EnableLineNumbers = false,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        MaxErrorMessages = 10,
        QuickInfo = false,
        RequestStockColors = true,
        ShowCompletion = false,
        ShowHotURLs = true,
        ShowMatchingBrace = true,
        SingleCodeWindowOnly = true)]
    [ProvideLanguageSettings(typeof(LogMessageLanguageService), LogMessageLanguageService.ServiceName, LogMessageLanguageService.ServiceName, LogMessageLanguageService.ServiceName, 305)]
    [ProvideService(typeof(LogMessageLanguageService), ServiceName = AnkhId.LogMessageServiceName)]
    [ProvideLanguageService(typeof(UnifiedDiffLanguageService), UnifiedDiffLanguageService.ServiceName, 304,
        CodeSense = false,
        ShowDropDownOptions = true,
        RequestStockColors = true)]
    [ProvideLanguageSettings(typeof(UnifiedDiffLanguageService), UnifiedDiffLanguageService.ServiceName, UnifiedDiffLanguageService.ServiceName, UnifiedDiffLanguageService.ServiceName, 306)]
    [ProvideLanguageExtension(typeof(UnifiedDiffLanguageService), ".patch")]
    [ProvideLanguageExtension(typeof(UnifiedDiffLanguageService), ".diff")]
    [ProvideService(typeof(UnifiedDiffLanguageService), ServiceName = AnkhId.UnifiedDiffServiceName)]
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
                    IObjectWithAutomation automationParent = GetService<IObjectWithAutomation>(ps.Type);

                    if (automationParent != null)
                        obj = automationParent.AutomationObject;

                    if (obj != null)
                        return obj;
                }
            }

            return null;
        }
    }
}
