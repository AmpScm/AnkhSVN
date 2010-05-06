// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

namespace Ankh.VS.Extenders
{
    [ComVisible(true)] // This class must be public or the extender won't accept it.
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public sealed class SvnProjectExtender : SvnItemExtender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvnProjectExtender"/> class.
        /// </summary>
        /// <param name="extendeeObject">The extendee object.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="disposer">The disposer.</param>
        /// <param name="catId">The cat id.</param>
        internal SvnProjectExtender(object extendeeObject, AnkhExtenderProvider provider, IDisposable disposer, string catId)
            : base(extendeeObject, provider, disposer, catId)
        {
        }
    }
}
