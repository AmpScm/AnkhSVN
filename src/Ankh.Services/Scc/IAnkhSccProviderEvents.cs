using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    [CLSCompliant(false)]
    public interface IAnkhSccProviderEvents
    {
        void AddDelayedDelete(string file);

        bool TrackProjectChanges(IVsSccProject2 sccProject, out bool trackCopies);

        bool TrackProjectChanges(IVsSccProject2 sccProject);

        void VerifySolutionNaming();

        void OnSolutionRefreshCommand(EventArgs e);


        void OnSolutionOpened(bool onLoad);

        void OnSolutionClosed();

        void OnStartedSolutionClose();

        

        void OnProjectLoaded(IVsSccProject2 project);

        void Translate_SolutionRenamed(string p1, string p2);

        void OnSolutionRenamedFile(string oldName, string newName);

        void OnProjectClosed(IVsSccProject2 project, bool p);

        void OnProjectBeforeUnload(IVsSccProject2 project, IVsHierarchy pStubHierarchy);

        
        void OnProjectOpened(IVsSccProject2 project, bool p);
        void OnProjectRenamed(IVsSccProject2 project);


        void OnProjectDirectoryAdded(IVsSccProject2 sccProject, string dir, string origin);
        void OnProjectDirectoryRenamed(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEDIRECTORYFLAGS vSRENAMEDIRECTORYFLAGS);
        void OnProjectDirectoryRemoved(IVsSccProject2 sccProject, string dir, VSREMOVEDIRECTORYFLAGS vSREMOVEDIRECTORYFLAGS);

        void OnProjectFileAdded(IVsSccProject2 sccProject, string newName);
        void OnProjectFileRenamed(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEFILEFLAGS vSRENAMEFILEFLAGS);
        void OnProjectFileRemoved(IVsSccProject2 sccProject, string oldName);

    }
}
