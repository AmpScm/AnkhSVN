using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Commands;
using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.GitScc
{
    [GlobalService(typeof(GitSccProvider))]
    [GlobalService(typeof(IAnkhGitSccService))]
    [GlobalService(typeof(ITheAnkhGitSccProvider), true)]
    partial class GitSccProvider : SccProvider, ITheAnkhGitSccProvider, IVsSccProvider, IVsSccControlNewSolution, IAnkhGitSccService
    {
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

        protected override void SetActive(bool active)
        {
            base.SetActive(active);

            if (active)
            {
                // Send activate before scheduling glyphs to make sure the project data is
                // loaded
                GetService<IAnkhServiceEvents>().OnGitSccProviderActivated(EventArgs.Empty);
            }
            else
            {
                GetService<IAnkhServiceEvents>().OnGitSccProviderDeactivated(EventArgs.Empty);
            }
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

        public void SetProjectManaged(Selection.SccProject project, bool managed)
        {
            //throw new NotImplementedException();
        }

        public bool IsProjectManaged(Selection.SccProject project)
        {
            //throw new NotImplementedException();
            return false;
        }

        public bool IsSolutionManaged
        {
            get { return false; }
        }

        public override AnkhGlyph GetPathGlyph(string path)
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

        public void EnsureCheckOutReference(Selection.SccProject project)
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

        public override Selection.SccProject CreateProject(Scc.ProjectMap.SccProjectData sccProjectData)
        {
            throw new NotImplementedException();
        }

        public override Scc.ProjectMap.SccProjectFile GetFile(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetAllDocumentFiles(string documentName)
        {
            yield return documentName;
        }
    }
}
