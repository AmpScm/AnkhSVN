using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.GitScc
{
    [GuidAttribute(AnkhId.SccServiceId), ComVisible(true), CLSCompliant(false)]
    public interface ITheAnkhGitSccProvider : IVsSccProvider
    {
    }

    [GlobalService(typeof(GitSccProvider))]
    [GlobalService(typeof(IAnkhGitSccService))]
    [GlobalService(typeof(ITheAnkhGitSccProvider), true)]
    partial class GitSccProvider : AnkhService, ITheAnkhGitSccProvider, IVsSccProvider, IVsSccControlNewSolution, IAnkhGitSccService, IVsSccEnlistmentPathTranslation
    {
        public GitSccProvider(IAnkhServiceProvider context)
            : base(context)
        {

        }
        #region STUB
        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            throw new NotImplementedException();
        }

        public int SetActive()
        {
            throw new NotImplementedException();
        }

        public int SetInactive()
        {
            throw new NotImplementedException();
        }

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
            get { throw new NotImplementedException(); }
        }

        public bool IsSolutionDirty
        {
            get
            {
                throw new NotImplementedException();
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

        public int TranslateEnlistmentPathToProjectPath(string lpszEnlistmentPath, out string pbstrProjectPath)
        {
            throw new NotImplementedException();
        }

        public int TranslateProjectPathToEnlistmentPath(string lpszProjectPath, out string pbstrEnlistmentPath, out string pbstrEnlistmentPathUNC)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
