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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.ComponentModel.Design;
using Ankh.UI;

namespace Ankh.VSPackage
{
    [ProvideEditorFactoryAttribute(typeof(AnkhDynamicEditorFactory), 303)]
    partial class AnkhSvnPackage
    {
        void RegisterEditors()
        {
            AnkhDynamicEditorFactory def = new AnkhDynamicEditorFactory(this);

            RegisterEditorFactory(def);
            _runtime.GetService<IServiceContainer>().AddService(typeof(IAnkhDynamicEditorFactory), def);
        }
    }
}
