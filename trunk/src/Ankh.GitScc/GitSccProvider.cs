using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Commands;
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

            GetService<AnkhServiceEvents>().RuntimeStarted
                += delegate
                {
                    IAnkhCommandStates states;

                    states = GetService<IAnkhCommandStates>();

                    if (states == null || !states.GitSccProviderActive)
                        return;

                    // Ok, Visual Studio decided to activate the user context with our GUID
                    // This tells us VS wants us to be the active SCC
                    //
                    // This is not documented directly. But it is documented that we should
                    // enable our commands on that context

                    // Set us active; this makes VS initialize the provider
                    RegisterAsPrimarySccProvider();
                };
        
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
            pbstrActionName = Resources.AddToGitCommandName;
            return VSErr.S_OK;
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

        protected override Guid ProviderGuid
        {
            get { return AnkhId.GitSccProviderGuid; }
        }

        public void LoadingManagedSolution(bool asPrimarySccProvider)
        {
            //throw new NotImplementedException();
        }

        public void SetProjectManaged(Selection.SvnProject project, bool managed)
        {
            //throw new NotImplementedException();
        }

        public bool IsProjectManaged(Selection.SvnProject project)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool IsSolutionManaged
        {
            get { return false; }
        }

        public AnkhGlyph GetPathGlyph(string path)
        {
            return AnkhGlyph.Added;
        }

        public void SerializeSccTranslateData(System.IO.Stream store, bool writeData)
        {
            //throw new NotImplementedException();
        }

        public void SerializeSccExcludeData(System.IO.Stream store, bool writeData)
        {
            //throw new NotImplementedException();
        }

        public bool IgnoreEnumerationSideEffects(IVsSccProject2 sccProject)
        {
            return false;
            //throw new NotImplementedException();
        }

        public void EnsureCheckOutReference(Selection.SvnProject project)
        {
            //throw new NotImplementedException();
        }

        public void EnsureLoaded()
        {
            //throw new NotImplementedException();
        }

        public bool HasProjectProperties(IVsHierarchy pHierarchy)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void StoreProjectProperties(IVsHierarchy pHierarchy, IPropertyMap map)
        {
            //throw new NotImplementedException();
        }

        public void ReadProjectProperties(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, IPropertyMap map)
        {
            //throw new NotImplementedException();
        }

        public void ProjectLoadFailed(string pszProjectMk)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
