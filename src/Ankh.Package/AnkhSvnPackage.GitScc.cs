using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;

using Ankh.VSPackage.Attributes;
using Ankh.Scc;
using Ankh.Scc.ProjectMap;
using Ankh.GitScc;
using ProvideSourceControlProviderAttribute = Ankh.VSPackage.Attributes.ProvideSourceControlProviderAttribute;

namespace Ankh.VSPackage
{
    [ProvideSourceControlProvider(AnkhId.GitSccProviderId, AnkhId.GitSccProviderTitle, "#101", typeof(ITheAnkhGitSccProvider))]
    [ProvideService(typeof(ITheAnkhGitSccProvider), ServiceName = AnkhId.GitSccName)]
    //[ProvideSourceControlCommand(AnkhId.GitSccProviderId, SccProviderCommand.Open, AnkhCommand.FileFileOpenFromSubversion)]
    //[ProvideSourceControlCommand(AnkhId.GitSccProviderId, SccProviderCommand.Share, AnkhCommand.FileSccAddSolutionToSubversion)]
    partial class AnkhSvnPackage
    {
    }
}
