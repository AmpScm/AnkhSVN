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

        class EmptySelectionContext : ISelectionContext
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

            public IEnumerable<string> GetOwnerProjects()
            {
                yield break;
            }

            public IEnumerable<string> GetOwnerProjects(bool recursive)
            {
                yield break;
            }

            public string SolutionFilename
            {
                get { return null; }
            }
        }
    }
}
