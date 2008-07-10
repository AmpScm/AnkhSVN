﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;
using System.Runtime.InteropServices;


namespace Ankh.Selection
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
        public static bool GetSccFiles(IVsHierarchy hierarchy, IVsSccProject2 sccProject, uint id, out string[] files, out int[] flags, bool includeNoScc)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            int hr;
            bool ok = false;

            files = null;
            flags = null;

            if (sccProject != null)
            {
                CALPOLESTR[] str = new CALPOLESTR[1];
                CADWORD[] dw = new CADWORD[1];

                if (ErrorHandler.Succeeded(hr = sccProject.GetSccFiles(id, str, dw)))
                {
                    files = GetFileNamesFromOleBuffer(str, true);
                    flags = GetFlagsFromOleBuffer(dw, true);

                    if (!includeNoScc || files.Length > 0)
                        return true; // We have a result
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
                    if (string.IsNullOrEmpty(mkDocument) || !IsFilePath(mkDocument))
                        files = new string[0];
                    else
                        files = new string[] { mkDocument };

                    return true;
                }
            }

            IAnkhGetMkDocument getDoc = hierarchy as IAnkhGetMkDocument;
            if (getDoc != null)
            {
                string mkDocument;

                if (ErrorHandler.Succeeded(getDoc.GetMkDocument(id, out mkDocument)))
                {
                    if (string.IsNullOrEmpty(mkDocument) || !IsFilePath(mkDocument))
                        files = new string[0];
                    else
                        files = new string[] { mkDocument };

                    return true;
                }
            }

            return ok;
        }

        private static bool IsFilePath(string path)
        {
            return SvnItem.IsValidPath(path);
        }

        internal static bool GetSccFiles(SelectionContext.SelectionItem item, out string[] files, bool includeSpecial, bool includeNoScc)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return GetSccFiles(item.Hierarchy, item.SccProject, item.Id, out files, includeSpecial, includeNoScc);
        }

        //[CLSCompliant(false)]
        internal static bool GetSccFiles(IVsHierarchy hierarchy, IVsSccProject2 sccProject, uint id, out string[] files, bool includeSpecial, bool includeNoScc)
        {
            int[] flags;
            files = null;

            if (!GetSccFiles(hierarchy, sccProject, id, out files, out flags, includeNoScc))
                return false;
            else if (flags == null || sccProject == null)
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
                        GC.KeepAlive(GetFlagsFromOleBuffer(dw, true)); // Don't parse the flags as none are defined on special files

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
            if (ErrorHandler.Succeeded(sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions)))
            {
                return solutionFile;
            }
            else
            {
                return null;
            }
        }

        //[CLSCompliant(false)]
        public static IVsSccProject2 GetSolutionAsSccProject(IServiceProvider context)
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
            readonly IServiceProvider _context;
            IVsSolution _solution;
            IVsHierarchy _solAsHierarchy;

            public SolutionSccHelper(IServiceProvider context)
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
                        pCaStringsOut[0] = CreateCALPOLESTR(new string[] { SelectionUtils.GetSolutionFileName(_context) });
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
                IVsSccManager2 sccService = (IVsSccManager2)_context.GetService(typeof(SVsSccManager));

                string[] rgpszFullPaths = new string[1];
                rgpszFullPaths[0] = GetSolutionFileName(_context);
                VsStateIcon[] rgsiGlyphs = new VsStateIcon[1];
                uint[] rgdwSccStatus = new uint[1];
                sccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus);

                // Set the solution's glyph directly in the hierarchy
                IVsHierarchy solHier = (IVsHierarchy)_context.GetService(typeof(SVsSolution));
                return solHier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, rgsiGlyphs[0]);
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
