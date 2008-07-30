using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio;
using System.ComponentModel;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

namespace Ankh.UI.PendingChanges
{
    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>
    {
        public PendingCommitsView()
        {
            StrictCheckboxesClick = true;
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public PendingCommitsView(IContainer container)
        {
            container.Add(this);
            StrictCheckboxesClick = true;
        }

        PendingCommitsSelectionMap _map;
        internal override SelectionItemMap SelectionMap
        {
            get { return _map ?? (_map = new PendingCommitsSelectionMap(this)); }
        }
   
        sealed class PendingCommitsSelectionMap : SelectionItemMap, IAnkhGetMkDocument
        {
            public PendingCommitsSelectionMap(PendingCommitsView view)
                : base(CreateData(view))
            {

            }

            #region IAnkhGetMkDocument Members

            public int GetMkDocument(uint itemid, out string pbstrMkDocument)
            {
                PendingCommitItem pci = (PendingCommitItem)GetItem(itemid);

                if (pci != null)
                {
                    pbstrMkDocument = pci.FullPath;
                    return VSConstants.S_OK;
                }
                else
                    pbstrMkDocument = null;

                return VSConstants.E_FAIL;
            }

            #endregion
        }

        protected override string GetCanonicalName(PendingCommitItem item)
        {
            return item.FullPath;
        }
    }
}
