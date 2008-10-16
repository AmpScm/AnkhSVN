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
