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

        public static IEnumerable EnumerateProjectItems( ProjectItems items )
        {
            return new Enumerable( new ProjectItemEnumerator( items ) );
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

        private class ProjectItemEnumerator : IEnumerator
        {
            public ProjectItemEnumerator( ProjectItems items )
            {
                this.items = items;
                if ( items != null )
                {
                    this.targetCount = items.Count;
                }
                else
                {
                    this.targetCount = Int32.MinValue;
                }
                this.currentCount = 0; // the first index is 1, so we start at 0
            }

            #region IEnumerator Members
            public object Current
            {
                get { return this.items.Item( this.currentCount ); }
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
            private ProjectItems items;
        }
    }
}
