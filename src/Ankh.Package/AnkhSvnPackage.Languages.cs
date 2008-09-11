using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.UI.PendingChanges;
using Ankh.Ids;
using Ankh.VSPackage.Attributes;

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
    partial class AnkhSvnPackage
    {
    }
}
