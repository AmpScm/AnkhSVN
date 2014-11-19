using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;

using Ankh.VSPackage.Attributes;
using Ankh.Scc;
using Ankh.Scc.ProjectMap;

namespace Ankh.VSPackage
{
#if NO
    [ProvideService(typeof(ITheAnkhSvnSccProvider), ServiceName = AnkhId.SubversionSccName)]
    [ProvideSourceControlProvider(AnkhId.SccProviderId, AnkhId.SccProviderTitle, "#100", typeof(ITheAnkhSvnSccProvider))]
    [ProvideSourceControlCommand(AnkhId.SccProviderId, SccProviderCommand.Open, AnkhCommand.FileFileOpenFromSubversion)]
    [ProvideSourceControlCommand(AnkhId.SccProviderId, SccProviderCommand.Share, AnkhCommand.FileSccAddSolutionToSubversion)]
#endif
    partial class AnkhSvnPackage
    {
    }
}
