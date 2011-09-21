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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;
using System.Runtime.InteropServices;


namespace Ankh.VS.Selection
{
    static class SelectionUtils
    {
        /// <summary>
        /// Gets the files from an OLE String buffer and clears the buffer
        /// </summary>
        /// <param name="pathStr">The path STR.</param>
        /// <returns></returns>
        //[CLSCompliant(false)]
        internal static string[] GetFileNamesFromOleBuffer(CALPOLESTR[] pathStr, bool free)
        {
            int nEls = (int)pathStr[0].cElems;
            string[] files = new string[nEls];

            for (int i = 0; i < nEls; i++)
            {
                IntPtr pathIntPtr = Marshal.ReadIntPtr(pathStr[0].pElems, i * IntPtr.Size);
                files[i] = Marshal.PtrToStringUni(pathIntPtr);

                if (free)
                    Marshal.FreeCoTaskMem(pathIntPtr);
            }
            if (free && pathStr[0].pElems != IntPtr.Zero)
                Marshal.FreeCoTaskMem(pathStr[0].pElems);

            return files;
        }

        //[CLSCompliant(false)]
        internal static int[] GetFlagsFromOleBuffer(CADWORD[] dwords, bool free)
        {
            if (dwords == null)
                throw new ArgumentNullException("dwords");

            int n = (int)dwords[0].cElems;
            int[] items = (n > 0) ? new int[n] : null;

            bool foundFlag = false;

            for (int i = 0; i < n; i++)
            {
                int v = items[i] = Marshal.ReadInt32(dwords[0].pElems, i * sizeof(int));

                if (v != 0)
                    foundFlag = true;
            }

            if (free && dwords[0].pElems != IntPtr.Zero)
                Marshal.FreeCoTaskMem(dwords[0].pElems);

            return foundFlag ? items : null;
        }

        //[CLSCompliant(false)]
        public static bool GetSccFiles(IVsHierarchy hierarchy, IVsSccProject2 sccProject, uint id, out string[] files, out int[] flags, bool includeNoScc, IDictionary<string, uint> map)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            int hr;
            bool ok = false;

            files = null;
            flags = null;

            try
            {

                if (sccProject != null)
                {
                    CALPOLESTR[] str = new CALPOLESTR[1];
                    CADWORD[] dw = new CADWORD[1];

                    if (ErrorHandler.Succeeded(hr = sccProject.GetSccFiles(id, str, dw)))
                    {
                        files = GetFileNamesFromOleBuffer(str, true);
                        flags = GetFlagsFromOleBuffer(dw, true);

                        if (!includeNoScc || files.Length > 0)
                            return ok = true; // We have a result
                        else
                            ok = true; // Try the GetMkDocument route to find an alternative
                    }
                    else if (hr != VSConstants.E_NOTIMPL)
                        return false; // 
                }

                // If sccProject2.GetSccFiles() returns E_NOTIMPL we must try GetMkDocument
                // We also try this if the item does not implement IVsSccProject2

                IVsProject project = hierarchy as IVsProject;
                if (project != null)
                {
                    string mkDocument;

                    if (ErrorHandler.Succeeded(project.GetMkDocument(id, out mkDocument)))
                    {
                        if (!IsValidPath(mkDocument))
                            files = new string[0];
                        else
                            files = new string[] { mkDocument };

                        return true;
                    }

                    return ok; // No need to check our interface for projects
                }

                if (hierarchy is IVsSolution)
                {
                    return ok; // Will fail in GetCanonicalName in VS2008 SP1 Beta 1
                }

                string name;
                try
                {
                    if (ErrorHandler.Succeeded(hierarchy.GetCanonicalName(id, out name)))
                    {
                        if (IsValidPath(name, true))
                        {
                            files = new string[] { name };
                            return true;
                        }
                    }
                }
                catch { } // Ok, this seems to error in some managed tree implementations like TFS :(

                return ok;
            }
            finally
            {
                if (ok && map != null && files != null)
                {
                    foreach (string file in files)
                        map[file] = id;
                }
            }
        }

        /// <summary>
        /// Determines whether the path is not null and a valid path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the path contains a valid path; otherwise (including null and empty) <c>false</c>.
        /// </returns>
        static bool IsValidPath(string path)
        {
            if(string.IsNullOrEmpty(path))
                return false;

            return SvnItem.IsValidPath(path);
        }

        /// <summary>
        /// Determines whether the path is not null and a valid path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the path contains a valid path; otherwise (including null and empty) <c>false</c>.
        /// </returns>
        static bool IsValidPath(string path, bool extraChecks)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return SvnItem.IsValidPath(path, extraChecks);
        }

        internal static bool GetSccFiles(SelectionItem item, out string[] files, bool includeSpecial, bool includeNoScc, IDictionary<string, uint> map)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (GetSccFiles(item.Hierarchy, item.SccProject, item.Id, out files, includeSpecial, includeNoScc, map))
            {
                // The managed package SDK for VS 2005 and VS2008 returns
                // new string[] { GetMkDocument() }; on the generic HierarchyNode
                // without checking if GetMkDocument returns a valid path

                // So: Unless the project itself overrides this method it might contain garbage
                // At least the BlackBerry add-in 1.0.1 has this issue, so we have to check

                int nEmpty = 0;

                foreach (string s in files)
                {
                    if (!IsValidPath(s))
                    {
                        nEmpty++;
                    }
                }

                if (nEmpty == 0)
                    return true; // No need to copy

                string[] fls = new string[files.Length - nEmpty];

                if (fls.Length > 0)
                {
                    int n = 0;
                    for (int i = 0; i < files.Length; i++)
                    {
                        string s = files[i];
                        if (!IsValidPath(s))
                            continue;

                        fls[n++] = s;
                    }
                }

                files = fls;
                return true;
            }

            return false;
        }

        //[CLSCompliant(false)]
        static bool GetSccFiles(IVsHierarchy hierarchy, IVsSccProject2 sccProject, uint id, out string[] files, bool includeSpecial, bool includeNoScc, IDictionary<string, uint> map)
        {
            int[] flags;
            files = null;

            if (!GetSccFiles(hierarchy, sccProject, id, out files, out flags, includeNoScc, map))
                return false;
            else if (flags == null || sccProject == null || !includeSpecial)
                return true;

            int n = Math.Min(files.Length, flags.Length);

            List<string> allFiles = new List<string>(files);
            for (int i = 0; i < n; i++)
            {
                if (0 != (flags[i] & (int)tagVsSccFilesFlags.SFF_HasSpecialFiles))
                {
                    CALPOLESTR[] str = new CALPOLESTR[1];
                    CADWORD[] dw = new CADWORD[1];

                    if (ErrorHandler.Succeeded(sccProject.GetSccSpecialFiles(id, allFiles[i], str, dw)))
                    {
                        files = GetFileNamesFromOleBuffer(str, true);
                        GetFlagsFromOleBuffer(dw, true); // Free the flags (No need to parse at this time)

                        if (files != null && files.Length > 0)
                            allFiles.AddRange(files);
                    }
                }
            }

            files = allFiles.ToArray();
            return true;
        }

        /// <summary>
        /// Returns the filename of the solution
        /// </summary>
        public static string GetSolutionFileName(IServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            IVsSolution sol = (IVsSolution)context.GetService(typeof(SVsSolution));
            string solutionDirectory, solutionFile, solutionUserOptions;
            if (ErrorHandler.Succeeded(sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions))
                && IsValidPath(solutionFile))
            {
                return solutionFile;
            }
            else
            {
                return null;
            }
        }

        //[CLSCompliant(false)]
        public static IVsSccProject2 GetSolutionAsSccProject(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return new SolutionSccHelper(context);
        }

        public static bool IsSolutionSccProject(IVsSccProject2 project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            return project is SolutionSccHelper;
        }

        class SolutionSccHelper : IVsSccProject2
        {
            readonly IAnkhServiceProvider _context;
            IVsSolution _solution;
            IVsHierarchy _solAsHierarchy;

            public SolutionSccHelper(IAnkhServiceProvider context)
            {
                _context = context;
            }

            protected IVsSolution Solution
            {
                get
                {
                    if (_solution != null)
                        return _solution;

                    return _solution = (IVsSolution)_context.GetService(typeof(SVsSolution));
                }
            }

            protected IVsHierarchy SolutionAsHierarchy
            {
                get
                {
                    if (_solAsHierarchy == null)
                        _solAsHierarchy = Solution as IVsHierarchy;

                    return _solAsHierarchy;
                }
            }


            #region IVsSccProject2 Members

            public int GetSccFiles(uint itemid, Microsoft.VisualStudio.OLE.Interop.CALPOLESTR[] pCaStringsOut, Microsoft.VisualStudio.OLE.Interop.CADWORD[] pCaFlagsOut)
            {
                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    string solutionFilename = SelectionUtils.GetSolutionFileName(_context);

                    if (!string.IsNullOrEmpty(solutionFilename))
                        pCaStringsOut[0] = CreateCALPOLESTR(new string[] { solutionFilename });
                    else
                        pCaStringsOut[0] = new CALPOLESTR();

                    pCaFlagsOut[0].cElems = 0;
                    pCaFlagsOut[0].pElems = IntPtr.Zero;

                    return VSConstants.S_OK;
                }

                return VSConstants.E_NOTIMPL;
            }

            public int GetSccSpecialFiles(uint itemid, string pszSccFile, Microsoft.VisualStudio.OLE.Interop.CALPOLESTR[] pCaStringsOut, Microsoft.VisualStudio.OLE.Interop.CADWORD[] pCaFlagsOut)
            {
                pCaStringsOut[0].cElems = 0;
                pCaStringsOut[0].pElems = IntPtr.Zero;
                return VSConstants.E_NOTIMPL;
            }

            public int SccGlyphChanged(int cAffectedNodes, uint[] rgitemidAffectedNodes, VsStateIcon[] rgsiNewGlyphs, uint[] rgdwNewSccStatus)
            {
                IVsSccManager2 sccService = _context.GetService<IVsSccManager2>(typeof(SVsSccManager));

                string[] rgpszFullPaths = new string[1];
                rgpszFullPaths[0] = GetSolutionFileName(_context);
                VsStateIcon[] rgsiGlyphs = new VsStateIcon[1];
                uint[] rgdwSccStatus = new uint[1];
                sccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                // Set the solution's glyph directly in the hierarchy
                IVsHierarchy solHier = (IVsHierarchy)_context.GetService(typeof(SVsSolution));
                return solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, (int)rgsiGlyphs[0]);
            }

            public int SetSccLocation(string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszSccProvider)
            {
                return VSConstants.E_NOTIMPL;
            }

            #endregion
        }

        internal static CALPOLESTR CreateCALPOLESTR(IList<string> strings)
        {
            CALPOLESTR calpolStr = new CALPOLESTR();

            if (strings != null)
            {
                // Demand unmanaged permissions in order to access unmanaged memory.
                calpolStr.cElems = (uint)strings.Count;

                int size = Marshal.SizeOf(typeof(IntPtr));

                calpolStr.pElems = Marshal.AllocCoTaskMem(strings.Count * size);

                IntPtr ptr = calpolStr.pElems;

                foreach (string aString in strings)
                {
                    IntPtr tempPtr = Marshal.StringToCoTaskMemUni(aString);
                    Marshal.WriteIntPtr(ptr, tempPtr);
                    ptr = new IntPtr(ptr.ToInt64() + size);
                }
            }

            return calpolStr;
        }
    }
}
