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
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Package;

using Ankh.VS.LanguageServices.Core;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.VS.LanguageServices.UnifiedDiff
{
    [Guid(AnkhId.UnifiedDiffLanguageServiceId), ComVisible(true), CLSCompliant(false)]
    [GlobalService(typeof(UnifiedDiffLanguage), PublicService = true)]
    public class UnifiedDiffLanguage : AnkhLanguage
    {
        public const string ServiceName = AnkhId.UnifiedDiffServiceName;

        public UnifiedDiffLanguage(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public override string Name
        {
            get { return ServiceName; }
        }

        protected override AnkhColorizer CreateColorizer(IVsTextLines lines)
        {
            return new UnifiedDiffColorizer(this, lines);
        }

        public override AnkhLanguageDropDownBar CreateDropDownBar(AnkhCodeWindowManager manager)
        {
            return new UnifiedDiffDropDownBar(this, manager);
        }

    }
}
