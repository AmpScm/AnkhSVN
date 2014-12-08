using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.GitScc
{
    [GuidAttribute(AnkhId.GitSccServiceId), ComVisible(true), CLSCompliant(false)]
    public interface ITheAnkhGitSccProvider : IVsSccProvider
    {
    }

    [GlobalService(typeof(GitSccProvider))]
    [GlobalService(typeof(IAnkhGitSccService))]
    [GlobalService(typeof(ITheAnkhGitSccProvider), true)]
    partial class GitSccProvider : SccProviderBase, ITheAnkhGitSccProvider, IVsSccProvider, IVsSccControlNewSolution, IAnkhGitSccService
    {
        bool _active;
        public GitSccProvider(IAnkhServiceProvider context)
            : base(context)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override bool AnyItemsUnderSourceControl()
        {
            return false;
        }

        public override int SetActive()
        {
            _active = true;
            return VSErr.S_OK;
        }

        public override int SetInactive()
        {
            _active = false;
            return VSErr.S_OK;
        }

        #region STUB

        public int AddNewSolutionToSourceControl()
        {
            throw new NotImplementedException();
        }

        public int GetDisplayStringForAction(out string pbstrActionName)
        {
            throw new NotImplementedException();
        }

        public bool IsActive
        {
            get { return _active; }
        }

        public bool IsSolutionDirty
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void LoadingManagedSolution(bool asPrimarySccProvider)
        {
            throw new NotImplementedException();
        }

        public void SetProjectManaged(Selection.SvnProject project, bool managed)
        {
            throw new NotImplementedException();
        }

        public bool IsProjectManaged(Selection.SvnProject project)
        {
            throw new NotImplementedException();
        }

        public bool IsSolutionManaged
        {
            get { throw new NotImplementedException(); }
        }

        public void RegisterAsPrimarySccProvider()
        {
            throw new NotImplementedException();
        }

        public AnkhGlyph GetPathGlyph(string path)
        {
            throw new NotImplementedException();
        }

        public void SerializeSccTranslateData(System.IO.Stream store, bool writeData)
        {
            throw new NotImplementedException();
        }

        public void SerializeSccExcludeData(System.IO.Stream store, bool writeData)
        {
            throw new NotImplementedException();
        }

        public bool IgnoreEnumerationSideEffects(IVsSccProject2 sccProject)
        {
            throw new NotImplementedException();
        }

        public void EnsureCheckOutReference(Selection.SvnProject project)
        {
            throw new NotImplementedException();
        }

        public void EnsureLoaded()
        {
            throw new NotImplementedException();
        }

        public bool HasProjectProperties(IVsHierarchy pHierarchy)
        {
            throw new NotImplementedException();
        }

        public void StoreProjectProperties(IVsHierarchy pHierarchy, IPropertyMap map)
        {
            throw new NotImplementedException();
        }

        public void ReadProjectProperties(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, IPropertyMap map)
        {
            throw new NotImplementedException();
        }

        public void ProjectLoadFailed(string pszProjectMk)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
