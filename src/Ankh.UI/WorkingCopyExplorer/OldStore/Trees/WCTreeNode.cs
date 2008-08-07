using System;
using System.Text;
using System.Collections;

using SharpSvn;

namespace Ankh
{
    public abstract class WCTreeNode
    {
        public WCTreeNode(WCTreeNode parent)
        {
            this.parent = parent;
            this.children = new ArrayList();
        }  

        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public IList Children
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.children; }
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public WCTreeNode Parent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.parent; }
        }

        /// <summary>
        /// Derived classes implement this method to append their resources
        /// to the list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void GetResources(IList list, bool getChildItems,
            Predicate<SvnItem> filter);

        public void Refresh()
        {
            this.Refresh(true);
        }

        public abstract void Refresh(bool rescan);

        protected void GetChildResources(System.Collections.IList list, bool getChildItems,
            Predicate<SvnItem> filter)
        {
            if (getChildItems)
            {
                foreach (WCTreeNode node in this.Children)
                    node.GetResources(list, getChildItems, filter);
            }
        }

        protected void FilterResources(IList inList, IList outList, Predicate<SvnItem> filter)
        {
            foreach (SvnItem item in inList)
            {
                if (filter == null || filter(item))
                {
                    outList.Add(item);
                }
            }
        }

        private WCTreeNode parent;
        private IList children;
    }
}
