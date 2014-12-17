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

    public abstract partial class SccProvider : AnkhService, IVsSccProvider, IVsSccManager2, ICOMVsSccManager3, IVsSccManagerTooltip, IAnkhSccProviderEvents, IVsSccControlNewSolution
    {
        bool _active;
        readonly SccProjectMap _projectMap;

        [CLSCompliant(false)]
        protected SccProvider(IAnkhServiceProvider context, SccProjectMap projectMap)
            : base(context)
        {
            if (projectMap == null)
                throw new ArgumentNullException("projectMap");

            _projectMap = projectMap;
        }

        [CLSCompliant(false)]
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
            get { return _tempPath ?? (_tempPath = ProjectMap.TempPathWithSeparator); }
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

        protected virtual void OnSolutionOpened(bool onLoad)
        {
            ClearSolutionInfo();
        }

        public bool IsActive
        {
            get { return _active; }
        }

        protected virtual void OnStartedSolutionClose() { }

        protected virtual void OnSolutionClosed()
        {
            ClearSolutionInfo();
        }

        void IAnkhSccProviderEvents.OnProjectLoaded(IVsSccProject2 project)
        {
            SccProjectData data;

            ProjectMap.EnsureSccProject(project, out data);

            OnProjectLoaded(data);
        }

        protected virtual void OnProjectLoaded(SccProjectData project)
        {

        }


        protected virtual void OnProjectOpened(SccProjectData project, bool added)
        {

        }

        protected virtual void OnProjectFileAdded(SccProjectData project)
        {

        }

        void IAnkhSccProviderEvents.OnProjectClosed(IVsSccProject2 project, bool removed)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(project, out data);
            OnProjectClosed(data, removed);
        }

        protected virtual void OnProjectClosed(SccProjectData data, bool removed)
        {
            data.OnClose();
            ProjectMap.RemoveProject(data.SccProject);
        }

        void IAnkhSccProviderEvents.OnProjectDirectoryAdded(IVsSccProject2 project, string directory, string origin)
        {
            SccProjectData data;
            if (!ProjectMap.TryGetSccProject(project, out data))
                return; // Not managed by us

            // Add a directory like a folder but with an ending '\'
            string dir = directory.TrimEnd('\\') + '\\';
            data.AddPath(dir);

            OnProjectDirectoryAdded(data, directory, origin);
        }

        protected virtual void OnProjectDirectoryAdded(SccProjectData data, string directory, string origin)
        {
            
        }

        void IAnkhSccProviderEvents.OnProjectBeforeUnload(IVsSccProject2 project, IVsHierarchy stub)
        {
            SccProjectData data;
            if (ProjectMap.TryGetSccProject(project, out data))
            {
                data.Unloading = true;

                OnProjectBeforeUnload(data);
            }
        }

        protected virtual void OnProjectBeforeUnload(SccProjectData data)
        {
            
        }

        void IAnkhSccProviderEvents.OnProjectRenamed(IVsSccProject2 project)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(project, out data);
            OnProjectRenamed(data);
        }

        protected virtual void OnProjectRenamed(SccProjectData data)
        {
            data.Reload(); // Reload project name, etc.
        }

        void IAnkhSccProviderEvents.OnProjectFileAdded(IVsSccProject2 project, string filename)
        {
            SccProjectData data;

            ProjectMap.EnsureSccProject(project, out data);
            OnProjectFileAdded(data, filename);
        }

        protected virtual void OnProjectFileAdded(SccProjectData data, string filename)
        {
            data.AddPath(filename);
        }

        void IAnkhSccProviderEvents.OnProjectFileRemoved(IVsSccProject2 project, string oldName)
        {
            SccProjectData data;

            ProjectMap.EnsureSccProject(project, out data);
            OnProjectFileRemoved(data, oldName);
        }

        protected virtual void OnProjectFileRemoved(SccProjectData data, string oldName)
        {
            data.RemovePath(oldName);
        }

        protected virtual void OnProjectDirectoryRemoved(SccProjectData data, string directoryname)
        {
            // a directory can be added like a folder but with an ending '\'
            string dir = directoryname.TrimEnd('\\') + '\\';
            data.RemovePath(dir);
        }

        protected virtual void OnProjectFileRenamed(SccProjectData project, string oldName, string newName)
        {
            project.CheckProjectRename(oldName, newName); // Just to be sure (should be handled by other event)

            project.RemovePath(oldName);
            project.AddPath(newName);
        }

        protected virtual void OnProjectDirectoryRenamed(SccProjectData project, string oldName, string newName)
        {

        }

        protected virtual void OnSolutionRenamedFile(string oldName, string newName)
        {
            ClearSolutionInfo();
        }

        protected virtual void AddDelayedDelete(string path)
        {

        }

        void IAnkhSccProviderEvents.AddDelayedDelete(string path)
        {
            AddDelayedDelete(path);
        }

        protected virtual void Translate_SolutionRenamed(string oldRawName, string newRawName)
        {

        }

        protected virtual void OnSolutionRefreshCommand(EventArgs e)
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

        [CLSCompliant(false)] // Implements 2 interfaces
        public int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider)
        {
            try
            {
                SccProjectData data;
                ProjectMap.EnsureSccProject(pscp2Project, out data);

                OnRegisterSccProject(data, pszProvider);
                return VSErr.S_OK;
            }
            catch(Exception e)
            {
                return VSErr.GetHRForException(e);
            }
        }

        protected virtual void OnRegisterSccProject(SccProjectData data, string pszProvider)
        {
            data.IsRegistered = true;
        }

        [CLSCompliant(false)] // Implements 2 interfaces
        public int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            try
            {
                SccProjectData data;
                ProjectMap.EnsureSccProject(pscp2Project, out data);

                OnUnregisterSccProject(data);
                return VSErr.S_OK;
            }
            catch (Exception e)
            {
                return VSErr.GetHRForException(e);
            }
        }

        protected virtual void OnUnregisterSccProject(SccProjectData data)
        {
            data.IsRegistered = false;
            data.IsManaged = false;
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

        [CLSCompliant(false)]
        protected virtual string GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode)
        {
            return null;
        }

        bool IAnkhSccProviderEvents.TrackProjectChanges(IVsSccProject2 sccProject, out bool trackCopies)
        {
            SccProjectData data;
            if (ProjectMap.TryGetSccProject(sccProject, out data))
            {
                return TrackProjectChanges(data, out trackCopies);
            }
            else
            {
                trackCopies = false;
                return false;
            }
        }

        protected virtual bool TrackProjectChanges(SccProjectData data, out bool trackCopies)
        {
            trackCopies = true;
            return data.TrackProjectChanges();
        }

        bool IAnkhSccProviderEvents.TrackProjectChanges(IVsSccProject2 sccProject)
        {
            bool trackCopies;

            SccProjectData data;
            if (ProjectMap.TryGetSccProject(sccProject, out data))
            {
                return TrackProjectChanges(data, out trackCopies);
            }
            else
            {
                trackCopies = false;
                return false;
            }
        }

        void IAnkhSccProviderEvents.OnSolutionRefreshCommand(EventArgs e)
        {
            OnSolutionRefreshCommand(e);
        }

        void IAnkhSccProviderEvents.OnSolutionOpened(bool onLoad)
        {
            OnSolutionOpened(onLoad);
        }

        void IAnkhSccProviderEvents.OnSolutionClosed()
        {
            OnSolutionClosed();
        }

        void IAnkhSccProviderEvents.OnStartedSolutionClose()
        {
            OnStartedSolutionClose();
        }

        void IAnkhSccProviderEvents.Translate_SolutionRenamed(string p1, string p2)
        {
            Translate_SolutionRenamed(p1, p2);
        }

        void IAnkhSccProviderEvents.OnSolutionRenamedFile(string oldName, string newName)
        {
            OnSolutionRenamedFile(oldName, newName);
        }

        void IAnkhSccProviderEvents.OnProjectOpened(IVsSccProject2 sccProject, bool added)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(sccProject, out data);
            OnProjectOpened(data, added);
        }

        void IAnkhSccProviderEvents.OnProjectDirectoryRenamed(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEDIRECTORYFLAGS vSRENAMEDIRECTORYFLAGS)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(sccProject, out data);
            OnProjectDirectoryRenamed(data, oldName, newName);
        }

        void IAnkhSccProviderEvents.OnProjectDirectoryRemoved(IVsSccProject2 sccProject, string dir, VSREMOVEDIRECTORYFLAGS vSREMOVEDIRECTORYFLAGS)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(sccProject, out data);
            OnProjectDirectoryRemoved(data, dir);
        }

        void IAnkhSccProviderEvents.OnProjectFileRenamed(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEFILEFLAGS vSRENAMEFILEFLAGS)
        {
            SccProjectData data;
            ProjectMap.EnsureSccProject(sccProject, out data);
            OnProjectFileRenamed(data, oldName, newName);
        }

        int IVsSccControlNewSolution.AddNewSolutionToSourceControl()
        {
            try
            {
                OnAddNewSolutionToSourceControl(EventArgs.Empty);
                return VSErr.S_OK;
            }
            catch(Exception e)
            {
                return VSErr.GetHRForException(e);
            }
        }

        protected virtual void OnAddNewSolutionToSourceControl(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        int IVsSccControlNewSolution.GetDisplayStringForAction(out string pbstrActionName)
        {
            try
            {
                pbstrActionName = GetDisplayStringForAddNewSolutionToSourceControl();
                return VSErr.S_OK;
            }
            catch (Exception e)
            {
                pbstrActionName = null;
                return VSErr.GetHRForException(e);
            }
        }

        protected virtual string GetDisplayStringForAddNewSolutionToSourceControl()
        {
            throw new NotImplementedException();
        }
    }
}
