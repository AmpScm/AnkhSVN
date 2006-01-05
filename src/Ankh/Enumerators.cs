using System;
using System.Text;
using System.Collections;
using EnvDTE;

namespace Ankh
{
    public class Enumerators
    {
        public static IEnumerable EnumerateProjects( _DTE dte )
        {
            return new Enumerable( new ProjectEnumerator(dte) );
        }

        private class Enumerable : IEnumerable
        {
            public Enumerable( IEnumerator enumerator )
            {
                this.enumerator = enumerator;
            }
            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            {
                return this.enumerator;
            }

            #endregion

            private IEnumerator enumerator;
        }

        private class ProjectEnumerator : IEnumerator
        {
            public ProjectEnumerator( _DTE dte )
            {
                this.dte = dte;
                this.targetCount = dte.Solution.Projects.Count;
                this.currentCount = 0; // the first index is 1, so we start at 0
            }

            #region IEnumerator Members
            public object Current
            {
                get { return this.dte.Solution.Projects.Item( this.currentCount ); }
            }

            public bool MoveNext()
            {
                this.currentCount++;
                return this.currentCount <= this.targetCount;
            }

            public void Reset()
            {
                this.currentCount = 0;
            }

            #endregion

            private int targetCount;
            private int currentCount;
            private _DTE dte;
        }
    }
}
