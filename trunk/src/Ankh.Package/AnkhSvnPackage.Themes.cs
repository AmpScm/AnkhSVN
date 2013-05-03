using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VSPackage.Attributes;
using Ankh.Scc.ProjectMap;

namespace Ankh.VSPackage
{
    [ProvideThemeInformation("{de3dbbcd-f642-433c-8353-8f1df4370aba}", true)] // Light
    [ProvideThemeInformation("{1ded0138-47ce-435e-84ef-9ec1f439b749}", false)] // Dark
    [ProvideThemeInformation("{a4d6a176-b948-4b29-8c66-53c97a1ed7d0}", true)] // Blue
    partial class AnkhSvnPackage
    {
    }
}
