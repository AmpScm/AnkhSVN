using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;

using Ankh.VSPackage.Attributes;
using Ankh.Scc;
using Ankh.Scc.ProjectMap;
using Ankh.GitScc;

namespace Ankh.VSPackage
{
#if DEBUG
    [ProvideSourceControlProvider(AnkhId.GitSccProviderId, AnkhId.GitSccProviderTitle, "#101", typeof(ITheAnkhGitSccProvider))]
#endif
    [ProvideService(typeof(ITheAnkhGitSccProvider), ServiceName = AnkhId.GitSccName)]
    //[ProvideSourceControlCommand(AnkhId.GitSccProviderId, SccProviderCommand.Open, AnkhCommand.FileFileOpenFromSubversion)]
    //[ProvideSourceControlCommand(AnkhId.GitSccProviderId, SccProviderCommand.Share, AnkhCommand.FileSccAddSolutionToSubversion)]
    partial class AnkhSvnPackage
    {
    }
}
