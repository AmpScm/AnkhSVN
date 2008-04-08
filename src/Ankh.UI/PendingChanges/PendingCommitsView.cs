using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio;
using System.ComponentModel;

namespace Ankh.UI.PendingChanges
{
    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>
    {
        public PendingCommitsView()
        {
            StrictCheckboxesClick = true;
        }


        public PendingCommitsView(IContainer container)
        {
            container.Add(this);
            StrictCheckboxesClick = true;
        }

        internal override ListViewHierarchy CreateHierarchy()
        {
            return new PendingCommitsHierarchy(this);
        }

        sealed class PendingCommitsHierarchy : ListViewHierarchy, IAnkhGetMkDocument
        {
            public PendingCommitsHierarchy(PendingCommitsView view)
                : base(view)
            {

            }

            #region IAnkhGetMkDocument Members

            public int GetMkDocument(uint itemid, out string pbstrMkDocument)
            {
                PendingCommitItem pci = GetItem(itemid);

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
    }
}
