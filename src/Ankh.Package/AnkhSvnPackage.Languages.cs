using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.UI.PendingChanges;

namespace Ankh.VSPackage
{
    [ProvideLanguageService(typeof(LogMessageLanguageService), LogMessageLanguageService.ServiceName, 301,
        AutoOutlining = false,
        CodeSense = false,
        DefaultToInsertSpaces = true,
        EnableAdvancedMembersOption=false,
        EnableAsyncCompletion=false,
        EnableCommenting=true,
        EnableLineNumbers=false,
        MatchBraces=true,
        MatchBracesAtCaret=true,
        MaxErrorMessages=10,
        QuickInfo=false,
        RequestStockColors=false,
        ShowCompletion=false,
        ShowHotURLs=true,
        ShowMatchingBrace=true,
        ShowSmartIndent=true,
        SingleCodeWindowOnly=true)]
    [ProvideService(typeof(LogMessageLanguageService))]
    partial class AnkhSvnPackage
    {        
    }
}
