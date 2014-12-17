using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Configuration;
using Ankh.Scc.ProjectMap;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    [GuidAttribute(AnkhId.SccServiceId), ComVisible(true), CLSCompliant(false)]
    public interface ITheAnkhSvnSccProvider : IVsSccProvider
    {
    }

    [GuidAttribute(AnkhId.GitSccServiceId), ComVisible(true), CLSCompliant(false)]
    public interface ITheAnkhGitSccProvider : IVsSccProvider
    {
    }

    // From Microsoft.VisualStudio.Shell.Interop.11.0
    [CLSCompliant(false)]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("224209ED-E56C-4C8D-A7FF-31CF4686798D")]
    public interface ICOMVsSccManager3 : IVsSccManager2
    {
        [PreserveSig]
        new int RegisterSccProject([In, MarshalAs(UnmanagedType.Interface)] IVsSccProject2 pscp2Project, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccProjectName, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccAuxPath, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccLocalPath, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszProvider);
        [PreserveSig]
        new int UnregisterSccProject([In, MarshalAs(UnmanagedType.Interface)] IVsSccProject2 pscp2Project);
        [PreserveSig]
        new int GetSccGlyph([In] int cFiles, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)] string[] rgpszFullPaths, [Out, ComAliasName("VsShell.VsStateIcon"), MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] VsStateIcon[] rgsiGlyphs, [Out, ComAliasName("OLE.DWORD"), MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] uint[] rgdwSccStatus);
        [PreserveSig]
        new int GetSccGlyphFromStatus([In, ComAliasName("OLE.DWORD")] uint dwSccStatus, [Out, ComAliasName("VsShell.VsStateIcon"), MarshalAs(UnmanagedType.LPArray)] VsStateIcon[] psiGlyph);
        [PreserveSig]
        new int IsInstalled([ComAliasName("OLE.BOOL")] out int pbInstalled);
        [PreserveSig]
        new int BrowseForProject([MarshalAs(UnmanagedType.BStr)] out string pbstrDirectory, [ComAliasName("OLE.BOOL")] out int pfOK);
        [PreserveSig]
        new int CancelAfterBrowseForProject();
        bool IsBSLSupported();
    }

    [CLSCompliant(false)]
    public abstract partial class SccProvider : AnkhService, IVsSccProvider, IVsSccManager2, ICOMVsSccManager3, IVsSccManagerTooltip
    {
        bool _active;
        readonly SccProjectMap _projectMap;

        protected SccProvider(IAnkhServiceProvider context, SccProjectMap projectMap)
            : base(context)
        {
            if (projectMap == null)
                throw new ArgumentNullException("projectMap");

            _projectMap = projectMap;
        }

        public SccProjectMap ProjectMap
        {
            [DebuggerStepThrough]
            get { return _projectMap; }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    DisposeGlyphList();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        int IVsSccProvider.AnyItemsUnderSourceControl(out int pfResult)
        {
            if (AnyItemsUnderSourceControl())
                pfResult = 1;
            else
                pfResult = 0;

            return VSErr.S_OK;
        }

        public abstract bool AnyItemsUnderSourceControl();

        protected virtual void SetActive(bool active)
        {
            _active = active;

            if (!active)
            {
                // If VS asked us for custom glyphs, we can release the handle now
                DisposeGlyphList();

                // Remove all glyphs currently set
                foreach (SccProjectData pd in new List<SccProjectData>(ProjectMap.AllSccProjects))
                {
                    pd.NotifyGlyphsChanged();
                    pd.Dispose();
                }

                ClearSolutionGlyph();

                ProjectMap.Clear();
            }
        }

        int IVsSccProvider.SetActive()
        {
            try
            {
                SetActive(true);
            }
            catch(Exception e)
            {
                return VSErr.GetHRForException(e);
            }

            return VSErr.S_OK;
        }

        int IVsSccProvider.SetInactive()
        {
            try
            {
                SetActive(false);
            }
            catch (Exception e)
            {
                return VSErr.GetHRForException(e);
            }

            return VSErr.S_OK;
        }

        protected abstract Guid ProviderGuid {get;}

        public void RegisterAsPrimarySccProvider()
        {
            IVsRegisterScciProvider rscp = GetService<IVsRegisterScciProvider>();
            if (rscp == null)
                return;

            VSErr.ThrowOnFailure(rscp.RegisterSourceControlProvider(ProviderGuid));
        }

        string _tempPath;
        public string TempPathWithSeparator
        {
            get
            {
                if (_tempPath == null)
                {
                    string p = System.IO.Path.GetTempPath();

                    if (p.Length > 0 && p[p.Length - 1] != Path.DirectorySeparatorChar)
                        p += Path.DirectorySeparatorChar;

                    _tempPath = p;
                }
                return _tempPath;
            }
        }


        public bool IsSafeSccPath(string file)
        {
            if (string.IsNullOrEmpty(file))
                return false;
            else if (!SvnItem.IsValidPath(file))
                return false;
            else if (file.StartsWith(TempPathWithSeparator, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public virtual void OnSolutionOpened(bool onLoad)
        {
            ClearSolutionInfo();
        }

        public bool IsActive
        {
            get { return _active; }
        }

        public virtual void VerifySolutionNaming() { }

        public virtual void OnStartedSolutionClose() { }

        public virtual void OnSolutionClosed()
        {
            ClearSolutionInfo();
        }

        public bool TrackProjectChanges(IVsSccProject2 project)
        {
            bool trackCopies;

            return TrackProjectChanges(project, out trackCopies);
        }

        public virtual bool TrackProjectChanges(IVsSccProject2 project, out bool trackCopies)
        {
            trackCopies = false;
            return false;
        }

        public virtual void OnProjectLoaded(IVsSccProject2 project)
        {
            SccProjectData data;

            ProjectMap.EnsureSccProject(project, out data);
        }

        public virtual void OnProjectOpened(IVsSccProject2 project, bool added)
        {

        }

        public virtual void OnProjectFileAdded(IVsSccProject2 project)
        {

        }

        public virtual void OnProjectClosed(IVsSccProject2 project, bool removed)
        {

        }

        public virtual void OnProjectDirectoryAdded(IVsSccProject2 project, string directory, string origin)
        {

        }

        public virtual void OnProjectBeforeUnload(IVsSccProject2 project, IVsHierarchy stub)
        {

        }

        public virtual void OnProjectRenamed(IVsSccProject2 project)
        {

        }

        public virtual void OnProjectFileAdded(IVsSccProject2 project, string filename, string fileOrigin, VSADDFILEFLAGS flags)
        {

        }

        public virtual void OnProjectFileRemoved(IVsSccProject2 project, string filename, VSREMOVEFILEFLAGS flags)
        {

        }

        public virtual void OnProjectDirectoryRemoved(IVsSccProject2 project, string filename, VSREMOVEDIRECTORYFLAGS flags)
        {

        }

        public virtual void OnProjectRenamedFile(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEFILEFLAGS flags)
        {

        }

        public virtual void OnProjectDirectoryRenamed(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEDIRECTORYFLAGS flags)
        {

        }

        public virtual void OnSolutionRenamedFile(string oldName, string newName)
        {
            ClearSolutionInfo();
        }

        public virtual void AddDelayedDelete(string path)
        {

        }

        public virtual void Translate_SolutionRenamed(string oldRawName, string newRawName)
        {

        }

        public virtual void OnSolutionRefreshCommand(EventArgs e)
        {
            
        }

        public abstract IEnumerable<string> GetAllDocumentFiles(string documentName);

        /// <summary>
        /// Obsolete: returns E_NOTIMPL.
        /// </summary>
        /// <param name="pbstrDirectory">The PBSTR directory.</param>
        /// <param name="pfOK">The pf OK.</param>
        /// <returns></returns>
        int IVsSccManager2.BrowseForProject(out string pbstrDirectory, out int pfOK)
        {
            pbstrDirectory = null;
            pfOK = 0;

            return VSErr.E_NOTIMPL;
        }

        /// <summary>
        /// Obsolete: returns E_NOTIMPL.
        /// </summary>
        /// <returns></returns>
        int IVsSccManager2.CancelAfterBrowseForProject()
        {
            return VSErr.E_NOTIMPL;
        }

        int ICOMVsSccManager3.BrowseForProject(out string pbstrDirectory, out int pfOK)
        {
            pbstrDirectory = null;
            pfOK = 0;

            return VSErr.E_NOTIMPL;
        }

        /// <summary>
        /// Obsolete: returns E_NOTIMPL.
        /// </summary>
        /// <returns></returns>
        int ICOMVsSccManager3.CancelAfterBrowseForProject()
        {
            return VSErr.E_NOTIMPL;
        }

        protected virtual bool IsInstalled
        {
            get { return true; }
        }
        int IVsSccManager2.IsInstalled(out int pbInstalled)
        {
            pbInstalled = IsInstalled ? 1 : 0;
            return VSErr.S_OK;
        }

        int ICOMVsSccManager3.IsInstalled(out int pbInstalled)
        {
            pbInstalled = IsInstalled ? 1 : 0;
            return VSErr.S_OK;
        }

        public virtual int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider)
        {
            return VSErr.S_OK;
        }

        public virtual int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            return VSErr.S_OK;
        }

        public bool IsBSLSupported()
        {
            return true;
        }

        int IVsSccManagerTooltip.GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode, out string pbstrTooltipText)
        {
            if (phierHierarchy == null)
            {
                pbstrTooltipText = null;
                return VSErr.E_INVALIDARG;
            }

            try
            {
                pbstrTooltipText = GetGlyphTipText(phierHierarchy, itemidNode);

                return VSErr.S_OK;
            }
            catch (Exception e)
            {
                pbstrTooltipText = null;
                return VSErr.GetHRForException(e);
            }
        }

        protected virtual string GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode)
        {
            return null;
        }
    }
}
