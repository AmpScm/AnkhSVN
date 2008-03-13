using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
	partial class AnkhSccProvider
	{
        /// <summary>
        /// Called when a file is added to a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileAdded(IVsSccProject2 project, string filename, VSADDFILEFLAGS flags)
        {
        }

        /// <summary>
        /// Called when a file is removed from a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileRemoved(IVsSccProject2 project, string filename, VSREMOVEFILEFLAGS flags)
        {
        }

        /// <summary>
        /// Called when a directory is added to a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryAdded(IVsSccProject2 project, string directoryname, VSADDDIRECTORYFLAGS flags)
        {
        }

        /// <summary>
        /// Called when a directory is removed from a project
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryRemoved(IVsSccProject2 sccProject, string directoryname, VSREMOVEDIRECTORYFLAGS flags)
        {
        }

        /// <summary>
        /// Called just before a file in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="vSQUERYRENAMEFILEFLAGS">The v SQUERYRENAMEFILEFLAGS.</param>
        /// <param name="ok">if set to <c>true</c> [ok].</param>
        internal void OnBeforeProjectRenameFile(IVsSccProject2 sccProject, string oldName, string newName, VSQUERYRENAMEFILEFLAGS vSQUERYRENAMEFILEFLAGS, out bool ok)
        {
            ok = true;
        }

        /// <summary>
        /// Called when a file in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectRenameFile(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEFILEFLAGS flags)
        {
        }

        /// <summary>
        /// Called just before a directory in a project is renamed
        /// </summary>
        /// <param name="sccProject"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="vSQUERYRENAMEDIRECTORYFLAGS"></param>
        /// <param name="ok"></param>
        internal void OnBeforeProjectDirectoryRename(IVsSccProject2 sccProject, string oldName, string newName, VSQUERYRENAMEDIRECTORYFLAGS vSQUERYRENAMEDIRECTORYFLAGS, out bool ok)
        {
            ok = true;
        }

        /// <summary>
        /// Called when a directory in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="vSRENAMEDIRECTORYFLAGS">The v SRENAMEDIRECTORYFLAGS.</param>
        internal void OnProjectDirectoryRename(IVsSccProject2 sccProject, string oldName, string newName, VSRENAMEDIRECTORYFLAGS vSRENAMEDIRECTORYFLAGS)
        {
        }
	}
}
