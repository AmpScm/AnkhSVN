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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.Selection
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>All members are based on IEnumerable to be able to delayload all cases required by commands. They return lists when completed
    /// but might return just an IEnumerable creating a list while not complete (e.g. we might traverse all files for a complete result,
    /// while we need only 1 un-added one)</para>
    /// 
    /// <para>A simple test implementation could just return arrays</para>
    /// </remarks>
    public interface ISelectionContext
    {
        /// <summary>
        /// Gets a list of the currently selected files
        /// </summary>
        /// <param name="recursive"><c>true</c> to select all descendants of selected nodes too</param>
        /// <returns></returns>
        IEnumerable<string> GetSelectedFiles(bool recursive);
        /// <summary>
        /// Gets a list of the currently selected <see cref="SvnItem"/> instances, mapped via their path. See <see cref="GetSelectedFiles(Boolean)"/>
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        IEnumerable<SvnItem> GetSelectedSvnItems(bool recursive);

        /// <summary>
        /// Gets the projects owning selected files
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnProject> GetOwnerProjects();

        /// <summary>
        /// Gets a list of currently selected projects
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnProject> GetSelectedProjects(bool recursive);


        /// <summary>
        /// Gets the item selection if it is available in the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetSelection<T>()
            where T : class;

        /// <summary>
        /// Gets a boolean indicating whether the solution node is currently selected
        /// </summary>
        bool IsSolutionSelected
        {
            get;
        }

        /// <summary>
        /// Gets a boolean indicating whether only a single node is selected in a tree
        /// </summary>
        bool IsSingleNodeSelection
        {
            get;
        }

        /// <summary>
        /// Gets the current solution filename (full path)
        /// </summary>
        /// <value>The solution filename.</value>
        string SolutionFilename { get; }

        /// <summary>
        /// Gets the name of the active document file (full path)
        /// </summary>
        /// <value>The name of the active document file.</value>
        string ActiveDocumentFilename { get; }

        /// <summary>
        /// Gets the active document SvnItem.
        /// </summary>
        /// <value>The active document item.</value>
        SvnItem ActiveDocumentItem { get; }

        /// <summary>
        /// Gets the .Net control of the <see cref="ISelectionContextEx.ActiveFrame"/>
        /// </summary>
        Control ActiveFrameControl { get; }

        /// <summary>
        /// Gets the .Net control of the <see cref="ISelectionContextEx.ActiveDocumentFrame"/>
        /// </summary>
        Control ActiveDocumentFrameControl { get; }

        /// <summary>
        /// Gets the active dialog.
        /// </summary>
        /// <value>The active dialog.</value>
        Control ActiveDialog { get; }

        /// <summary>
        /// Gets the active dialog or frame control.
        /// </summary>
        /// <value>The active dialog or frame control.</value>
        Control ActiveDialogOrFrameControl { get; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        Hashtable Cache { get; }

        /// <summary>
        /// Gets the active control of the specified type
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <returns></returns>
        TControl GetActiveControl<TControl>()
            where TControl : class;
    }

    /// <summary>
    /// Raw selection access; implemented by the <see cref="ISelectionContext"/> service
    /// </summary>
    [CLSCompliant(false)]
    public interface ISelectionContextEx : ISelectionContext
    {
        /// <summary>
        /// Gets the currently active frame (Document or Toolwindow)
        /// </summary>
        IVsWindowFrame ActiveFrame { get; }

        /// <summary>
        /// Gets the frame of the currently active document
        /// </summary>
        IVsWindowFrame ActiveDocumentFrame { get; }

        /// <summary>
        /// Gets the user context.
        /// </summary>
        /// <value>The user context.</value>
        IVsUserContext UserContext { get; }

        /// <summary>
        /// Pushes the popup context.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        IDisposable PushPopupContext(Control control);

        /// <summary>
        /// Gets a selection tracker that is valid for the current popup context
        /// </summary>
        /// <returns></returns>
        IVsTrackSelectionEx GetModalTracker(Control control);

        IVsTextView ActiveFrameTextView { get; }

        /// <summary>
        /// Maybes the install delay handler.
        /// </summary>
        void MaybeInstallDelayHandler();
    }
}
