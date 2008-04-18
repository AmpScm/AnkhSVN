using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ankh.Scc
{
    public class SvnItemsEventArgs : EventArgs
    {
        readonly ReadOnlyCollection<SvnItem> _changedItems;

        public SvnItemsEventArgs(IList<SvnItem> changedItems)
        {
            if(changedItems == null)
                throw new ArgumentNullException("changedItems");

            _changedItems = new ReadOnlyCollection<SvnItem>(changedItems);
        }

        public ReadOnlyCollection<SvnItem> ChangedItems
        {
            get { return _changedItems; }
        }
    }

    public interface ISvnItemChange
    {
        /// <summary>
        /// Occurs when the state of one or more <see cref="SvnItem"/> instances changes
        /// </summary>
        event EventHandler<SvnItemsEventArgs> SvnItemsChanged;

        /// <summary>
        /// Raises the <see cref="E:SvnItemsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.SvnItemsEventArgs"/> instance containing the event data.</param>
        void OnSvnItemsChanged(SvnItemsEventArgs e);
    }    
}
