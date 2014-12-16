using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Configuration;
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

    [CLSCompliant(false)]
    public abstract class SccProvider : AnkhService, IVsSccProvider
    {
        bool _active;

        protected SccProvider(IAnkhServiceProvider context)
            : base(context)
        {

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

        public virtual void OnSolutionOpened(bool onLoad) { }

        public bool IsActive
        {
            get { return _active; }
        }

        public virtual void VerifySolutionNaming() { }

        public virtual void OnStartedSolutionClose() { }

        public virtual void OnSolutionClosed() { }

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

        public abstract Selection.SccProject CreateProject(ProjectMap.SccProjectData sccProjectData);

        public abstract ProjectMap.SccProjectFile GetFile(string path);

        public abstract int GetSccGlyph(int p, string[] namesArray, VsStateIcon[] newGlyphs, uint[] sccState);

        public abstract IEnumerable<string> GetAllDocumentFiles(string documentName);
    }
}
