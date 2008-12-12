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
using Ankh.UI.PendingChanges;
using Ankh.Ids;
using Ankh.VSPackage.Attributes;
using Ankh.VS.LanguageServices;

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
    [ProvideLanguageSettings(typeof(LogMessageLanguageService), LogMessageLanguageService.ServiceName, LogMessageLanguageService.ServiceName, 301, 301)]
    [ProvideService(typeof(LogMessageLanguageService), ServiceName = AnkhId.LogMessageServiceName)]
    [ProvideLanguageService(typeof(UnifiedDiffLanguageService), UnifiedDiffLanguageService.ServiceName, 304)]
    [ProvideLanguageSettings(typeof(UnifiedDiffLanguageService), UnifiedDiffLanguageService.ServiceName, UnifiedDiffLanguageService.ServiceName, 304, 304)]
    [ProvideLanguageExtension(typeof(UnifiedDiffLanguageService), ".patch")]
    [ProvideService(typeof(UnifiedDiffLanguageService), ServiceName=AnkhId.UnifiedDiffServiceName)]
    partial class AnkhSvnPackage
    {
    }
}
