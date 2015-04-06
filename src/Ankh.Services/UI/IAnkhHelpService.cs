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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Ankh.Configuration;
using Ankh.VS;

namespace Ankh.UI
{
    public interface IAnkhControlWithHelp
    {
        string DialogHelpTypeName { get; }

        Control Control { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IAnkhHelpService
    {
        /// <summary>
        /// Shows generic help for the specified form
        /// </summary>
        /// <param name="form">The form.</param>
        void RunHelp(VSDialogForm form);

        void RunHelp(IAnkhControlWithHelp control);
    }

}
