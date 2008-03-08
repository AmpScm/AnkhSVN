using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;
using System.Runtime.InteropServices;


namespace Ankh.Selection
{
    public static class SelectionUtils
    {
        /// <summary>
        /// Gets the files from an OLE String buffer and clears the buffer
        /// </summary>
        /// <param name="pathStr">The path STR.</param>
        /// <returns></returns>
        public static string[] GetFileNamesFromOleBuffer(CALPOLESTR[] pathStr, bool free)
        {
            int nEls = (int)pathStr[0].cElems;
            string[] files = new string[nEls];

            for (int i = 0; i < nEls; i++)
            {
                IntPtr pathIntPtr = Marshal.ReadIntPtr(pathStr[0].pElems, i);
                files[i] = Marshal.PtrToStringUni(pathIntPtr);

                if(free)
                Marshal.FreeCoTaskMem(pathIntPtr);
            }
            if (free && pathStr[0].pElems != IntPtr.Zero)
                Marshal.FreeCoTaskMem(pathStr[0].pElems);

            return files;
        }

        public static int[] GetFlagsFromOleBuffer(CADWORD[] dwords, bool free)
        {
            if(dwords == null)
                throw new ArgumentNullException("dwords");

            int[] items = new int[dwords[0].cElems];

            for(int i = 0; i < items.Length; i++)
            {
                items[i] = Marshal.ReadInt32(dwords[0].pElems, i);
            }

            if(free && dwords[0].pElems != IntPtr.Zero)
                Marshal.FreeCoTaskMem(dwords[0].pElems);
            
            return items;
        }

        public static int GetSccFiles(IVsHierarchy hierarchy, uint id, out string[] files, out int[] flags)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            IVsSccProject2 p2 = hierarchy as IVsSccProject2;
            int hr;

            if (p2 != null)
            {
                CALPOLESTR[] str = new CALPOLESTR[1];
                CADWORD[] dw = new CADWORD[1];
                hr = p2.GetSccFiles(id, str, dw);

                if (hr == VSConstants.S_OK)
                {
                    files = GetFileNamesFromOleBuffer(str, true);
                    flags = GetFlagsFromOleBuffer(dw, true);
                    return VSConstants.S_OK;
                }
            }

            IVsProject3 p3 = hierarchy as IVsProject3;

            if(p3 != null)
            {
                string mkdocument;
                if (p3.GetMkDocument(id, out mkdocument) == VSConstants.S_OK && mkdocument != null)
                {
                    files = new string[] { mkdocument };
                    flags = null;
                    return VSConstants.S_OK;
                }
            }

            files = null;
            flags = null;

            return VSConstants.E_FAIL;
        }

        public static int GetSccFiles(IVsHierarchy hierarchy, uint id, out string[] files, bool includeSpecial)
        {
            string[] fls;
            int[] flags;
            files = null;

            int hr = GetSccFiles(hierarchy, id, out fls, out flags);

            if (hr != VSConstants.S_OK || !includeSpecial || flags == null || flags.Length == 0)
            {
                if (hr == VSConstants.S_OK)
                    files = fls;

                return hr;
            }

            bool foundOne = false;
            foreach(uint u in flags)
            {
                if(u != 0)
                {
                    foundOne = true;
                    break;
                }
            }

            if(!foundOne)
            {
                files = fls;
                return VSConstants.S_OK;
            }

            IVsSccProject2 p2 = (IVsSccProject2)hierarchy;

            List<string> fileList = new List<string>(fls);
            List<int> flagList = new List<int>(flags);

            if(flagList.Count > fileList.Count)
                flagList.RemoveRange(fileList.Count, flagList.Count - fileList.Count);
            else
                while(flagList.Count < fileList.Count)
                    flagList.Add(0);

            for (int i = 0; i < fileList.Count; i++)
            {
                if (flagList[i] != 0)
                {
                    CALPOLESTR[] str = new CALPOLESTR[1];
                    CADWORD[] dw = new CADWORD[1];
                    hr = p2.GetSccSpecialFiles(id, fileList[i], str, dw);

                    if (hr != VSConstants.S_OK)
                        return hr;

                    fls = GetFileNamesFromOleBuffer(str, true);
                    flags = GetFlagsFromOleBuffer(dw, true);

                    foreach (string s in fls)
                        fileList.Add(s);

                    if (flags != null)
                        foreach (int n in flags)
                            flagList.Add(n);

                    if (flagList.Count > fileList.Count)
                        flagList.RemoveRange(fileList.Count, flagList.Count - fileList.Count);
                    else
                        while (flagList.Count < fileList.Count)
                            flagList.Add(0);
                }
            }

            files = fileList.ToArray();

            return VSConstants.S_OK;
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
            if (sol.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions) == VSConstants.S_OK)
            {
                return solutionFile;
            }
            else
            {
                return null;
            }
        }

        class SolutionSccHelper : IVsSccProject2
        {
            readonly IServiceProvider _context;

            public SolutionSccHelper(IServiceProvider context)
            {
                _context = context;
            }
            #region IVsSccProject2 Members

            public int GetSccFiles(uint itemid, Microsoft.VisualStudio.OLE.Interop.CALPOLESTR[] pCaStringsOut, Microsoft.VisualStudio.OLE.Interop.CADWORD[] pCaFlagsOut)
            {
                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    pCaStringsOut[0] = CreateCALPOLESTR(new string[] { SelectionUtils.GetSolutionFileName(_context) });
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
