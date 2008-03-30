using System;

namespace Ankh.UI.PendingChanges
{
    // VS2008 SDK only addition to the LogMessageLanguage class
    // (The VS2005 SDK version compiles LogMessageLanguageService.VS2005.cs instead)
    public partial class LogMessageLanguageService
    {
        public override string GetFormatFilterList()
        {
            return "";
        }
    }
}
