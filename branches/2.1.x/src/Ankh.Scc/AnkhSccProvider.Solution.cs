// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Commands;

namespace Ankh.Scc
{
    partial class AnkhSccProvider : IVsSccControlNewSolution, IVsSccOpenFromSourceControl
	{
        /// <summary>
        /// Adds the current solution to source control.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int AddNewSolutionToSourceControl()
        {
            CommandService.PostExecCommand(AnkhCommand.FileSccAddSolutionToSubversion);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Retrieves the text to be displayed with the "Add to Source Control" check box in the New Projects dialog box.
        /// </summary>
        /// <param name="pbstrActionName">[out] Returns the text to be used for the "Add to Source Control" check box.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int GetDisplayStringForAction(out string pbstrActionName)
        {
            pbstrActionName = Resources.AddToSubversionCommandName;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Adds the specified item or items to the specified project directly from source control.
        /// </summary>
        /// <param name="pProject">[in] <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsProject"></see> interface for the project to add the items to.T:Microsoft.VisualStudio.Shell.Interop.IVsProject is an old interface that is not used directly. Instead, query the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsProject3"></see> interface for the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsProject"></see> interface and pass that in.</param>
        /// <param name="itemidLoc">[in] A value indicating where in the project hierarchy to add the items. This is a unique identifier for a project or folder item or one of the following values: <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_NIL"></see>, <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT"></see>, or <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_SELECTION"></see>.</param>
        /// <param name="cFilesToAdd">[in] Number of files specified in rgpszFilesToAdd array.</param>
        /// <param name="rgpszFilesToAdd">[in] Array of files names to add to the project from source control.</param>
        /// <param name="hwndDlgOwner">[in] Handle to the parent of the dialog box that will be used.</param>
        /// <param name="grfEditorFlags">[in] A combination of flags from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSSPECIFICEDITORFLAGS"></see> enumeration.</param>
        /// <param name="rguidEditorType">[in] GUID that specifies the type of editor being used.</param>
        /// <param name="pszPhysicalView">[in] Name of the physical view being used.</param>
        /// <param name="rguidLogicalView">[in] GUID that identifies the logical view.</param>
        /// <param name="pResult">[out] Returns a <see cref="T:Microsoft.VisualStudio.Shell.Interop.VSADDRESULT"></see> code indicating the overall status of the add process.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int AddItemFromSourceControl(IVsProject pProject, uint itemidLoc, uint cFilesToAdd, string[] rgpszFilesToAdd, IntPtr hwndDlgOwner, uint grfEditorFlags, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, VSADDRESULT[] pResult)
        {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Adds the specified project to the current solution directly from source control.
        /// </summary>
        /// <param name="pszProjectStoreUrl">[in] The URL of the project in the source control to be added to the current solution (for example, msss://server/…/MyProject.proj).</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int AddProjectFromSourceControl(string pszProjectStoreUrl)
        {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Retrieves the source control Namespace Extension (NSE) information for use in Open dialog boxes..
        /// </summary>
        /// <param name="vsofsdDlg">[in] A value from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSOPENFROMSCCDLG"></see> enumeration specifying the type of dialog box that will be opened.</param>
        /// <param name="pbstrNamespaceGUID">[out] Returns the GUID (in string form) of the NSE allowing the dialog boxes to explore the NSE space.</param>
        /// <param name="pbstrTrayDisplayName">[out] Returns the display name of the NSE (which can appear in the Places section of the Window border of the dialog boxes).</param>
        /// <param name="pbstrProtocolPrefix">[out] Returns the NSE protocol prefix (for example, "msss://").</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.E_NOTIMPL"></see> or an error code.
        /// </returns>
        public int GetNamespaceExtensionInformation(int vsofsdDlg, out string pbstrNamespaceGUID, out string pbstrTrayDisplayName, out string pbstrProtocolPrefix)
        {
            pbstrNamespaceGUID = Guid.Empty.ToString();
            pbstrTrayDisplayName = "Subversion";
            pbstrProtocolPrefix = "svn://";
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Opens the specified solution directly from source control, creating a local copy as necessary.
        /// </summary>
        /// <param name="pszSolutionStoreUrl">[in] The URL of the solution in source control to be opened (for example, msss://server/.../MySolution.sln).</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OpenSolutionFromSourceControl(string pszSolutionStoreUrl)
        {
            return VSConstants.E_NOTIMPL;
        }
    }
}
