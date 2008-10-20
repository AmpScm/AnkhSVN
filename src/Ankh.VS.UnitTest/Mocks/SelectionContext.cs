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

            public Microsoft.VisualStudio.Shell.Interop.IVsTrackSelectionEx GetModalTracker()
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

            public object ActiveDocumentInstance
            {
                get { return null; }
            }

            #endregion
        }
    }
}
