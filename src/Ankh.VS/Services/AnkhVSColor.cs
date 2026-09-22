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
using System.Drawing;
using System.Runtime.InteropServices;
using Ankh.Services;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Services
{
    [GlobalService(typeof(IAnkhVSColor))]
    sealed class AnkhVSColor : AnkhService, IAnkhVSColor
    {
        public AnkhVSColor(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell2 _uiShell;
        IVsUIShell2 UIShell
        {
            get { return _uiShell ?? (_uiShell = GetService<IVsUIShell2>(typeof(SVsUIShell))); }
        }

        public bool TryGetColor(__VSSYSCOLOREX vsColor, out Color color)
        {
            color = Color.Empty;

            IVsUIShell2 uiShell = UIShell;
            if (uiShell == null)
                return false;

            try
            {
                uint rgb;
                if (VSErr.Succeeded(uiShell.GetVSSysColorEx((int)vsColor, out rgb)))
                {
                    color = ColorTranslator.FromWin32(unchecked((int)rgb));
                    return true;
                }
            }
            catch (COMException)
            {
            }

            return false;
        }
    }
}
