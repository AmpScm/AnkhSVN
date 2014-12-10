// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using Ankh.VSPackage.Attributes;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage
{
    [ProvideOptionPage(typeof(UserToolsSettingsPage), AnkhSvnPackage.SccOptionGroup, "Subversion User Tools", 106, 108, false)]
    [ProvideToolsOptionsPageVisibility(AnkhSvnPackage.SccOptionGroup, "Subversion User Tools", AnkhId.SccProviderId)]
    [ProvideToolsOptionsPageVisibility(AnkhSvnPackage.SccOptionGroup, "Subversion User Tools", AnkhId.GitSccProviderId)]
    [ProvideOptionPage(typeof(SvnSettingsPage), AnkhSvnPackage.SccOptionGroup, "Subversion", 106, 107, false)]
    [ProvideToolsOptionsPageVisibility(AnkhSvnPackage.SccOptionGroup, "Subversion", AnkhId.SccProviderId)]
    [ProvideOptionPage(typeof(GitSettingsPage), AnkhSvnPackage.SccOptionGroup, "AnkhGIT", 106, 109, false)]
    [ProvideToolsOptionsPageVisibility(AnkhSvnPackage.SccOptionGroup, "AnkhGIT", AnkhId.GitSccProviderId)]
    partial class AnkhSvnPackage
    {
        internal const string SccOptionGroup = "Source Control";
    }
}
