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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using Ankh.Commands;
using Ankh;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Ankh.VSPackage
{
    // The command routing at package level
    public partial class AnkhSvnPackage : IOleCommandTarget
    {
        /// <summary>
        /// Queries the status of a command handled by this package
        /// </summary>
        /// <param name="pguidCmdGroup">The guid of the Command.</param>
        /// <param name="cCmds">The number of commands.</param>
        /// <param name="prgCmds">The Commands to update.</param>
        /// <param name="pCmdText">Command text update pointer.</param>
        /// <returns></returns>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if ((prgCmds == null))
                return VSConstants.E_INVALIDARG;

            Debug.Assert(cCmds == 1, "Multiple commands"); // Should never happen in VS

            if (Zombied || pguidCmdGroup != AnkhId.CommandSetGuid)
            {
                // Filter out commands that are not defined by this package
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            }

            int hr = CommandMapper.QueryStatus(Context, cCmds, prgCmds, pCmdText);


            return hr;
        }


        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (Zombied || pguidCmdGroup != AnkhId.CommandSetGuid)
            {
                return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
            }

            switch ((OLECMDEXECOPT)nCmdexecopt)
            {
                case OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT:
                case OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER:
                case OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER:
                    break;
                case OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
                default:
                    // VS Doesn't use OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP                    
                    return VSConstants.E_NOTIMPL;
                case (OLECMDEXECOPT)0x00010000 | OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP:
                    // Retrieve parameter information of command for immediate window
                    // See http://blogs.msdn.com/dr._ex/archive/2005/03/16/396877.aspx for more info

                    if (pvaOut == IntPtr.Zero)
                        return VSConstants.E_POINTER;

                    string definition;
                    if (pvaOut != IntPtr.Zero && CommandMapper.TryGetParameterList((AnkhCommand)nCmdID, out definition))
                    {
                        Marshal.GetNativeVariantForObject(definition, pvaOut);

                        return VSConstants.S_OK;
                    }

                    return VSConstants.E_NOTIMPL;
            }

            object argIn = null;

            if (pvaIn != IntPtr.Zero)
                argIn = Marshal.GetObjectForNativeVariant(pvaIn);

            CommandEventArgs args = new CommandEventArgs(
                (AnkhCommand)nCmdID,
                Context,
                argIn,
                (OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
                (OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);

            if (!CommandMapper.Execute(args.Command, args))
                return (int)OLEConstants.OLECMDERR_E_DISABLED;

            if (pvaOut != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(args.Result, pvaOut);
            }

            return VSConstants.S_OK;
        }

        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return (T)GetService(typeof(T));
        }

        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return (T)GetService(serviceType);
        }

        Ankh.Commands.CommandMapper _mapper;

        public Ankh.Commands.CommandMapper CommandMapper
        {
            get { return _mapper ?? (_mapper = _runtime.CommandMapper); }
        }
    }
}
