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
using Ankh.Selection;
using Rhino.Mocks;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class SelectionContextMock
    {
        public static ISelectionContext EmptyContext(MockRepository mocks)
        {
            return new EmptySelectionContext();
        }

        class EmptySelectionContext : ISelectionContext, ISelectionContextEx
        {
            public IEnumerable<string> GetSelectedFiles()
            {
                yield break;
            }

            public IEnumerable<string> GetSelectedFiles(bool recursive)
            {
                yield break;
            }

            public IEnumerable<Ankh.SvnItem> GetSelectedSvnItems()
            {
                yield break;
            }

            public IEnumerable<Ankh.SvnItem> GetSelectedSvnItems(bool recursive)
            {
                yield break;
            }

            public IEnumerable<SvnProject> GetOwnerProjects()
            {
                yield break;
            }

            public IEnumerable<SvnProject> GetOwnerProjects(bool recursive)
            {
                yield break;
            }

            public string SolutionFilename
            {
                get { return null; }
            }

            public IEnumerable<SvnProject> GetSelectedProjects(bool recursive)
            {
                yield break;
            }

            public bool IsSolutionSelected
            {
                get { return false; }
            }

            public IEnumerable<T> GetSelection<T>() where T : class
            {
                yield break;
            }

            #region ISelectionContext Members


            public bool IsSingleNodeSelection
            {
                get { return false; }
            }

            #endregion

            #region ISelectionContext Members


            public System.Windows.Forms.Control ActiveFrameControl
            {
                get { throw new NotImplementedException(); }
            }

            public System.Windows.Forms.Control ActiveDocumentFrameControl
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region ISelectionContextEx Members

            public Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame ActiveFrame
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame ActiveDocumentFrame
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.VisualStudio.Shell.Interop.IVsUserContext UserContext
            {
                get { throw new NotImplementedException(); }
            }

            public IDisposable PushPopupContext(System.Windows.Forms.Control control)
            {
                return null;
            }

            #endregion

            #region ISelectionContext Members


            public System.Windows.Forms.Control ActiveDialogOrFrameControl
            {
                get { return ActiveDialog ?? ActiveFrameControl; }
            }

            public System.Windows.Forms.Control ActiveDialog
            {
                get { return null; }
            }

            public Microsoft.VisualStudio.Shell.Interop.IVsTrackSelectionEx GetModalTracker(System.Windows.Forms.Control control)
            {
                return null;
            }

            public TControl GetActiveControl<TControl>() where TControl : class
            {
                return null;
            }

            public string ActiveDocumentFilename
            {
                get { return null; }
            }

            public Ankh.SvnItem ActiveDocumentItem
            {
                get { return null; }
            }

            public object ActiveDocumentInstance
            {
                get { return null; }
            }

            #endregion

            #region ISelectionContext Members


            public System.Collections.Hashtable Cache
            {
                get { return new System.Collections.Hashtable(); }
            }

            #endregion
        }
    }
}
