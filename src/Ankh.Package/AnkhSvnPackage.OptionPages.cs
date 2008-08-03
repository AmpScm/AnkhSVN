using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VSPackage.Attributes;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage
{
    [ProvideOptionPage(typeof(AnkhSourceControlSettingsPage), "Source Control", "Subversion", 106, 107, false)]
    [ProvideToolsOptionsPageVisibility("Source Control", "Subversion", AnkhId.SccProviderId)]
    partial class AnkhSvnPackage
    {
    }
}
