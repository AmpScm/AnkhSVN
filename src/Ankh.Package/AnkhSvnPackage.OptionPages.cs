// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
