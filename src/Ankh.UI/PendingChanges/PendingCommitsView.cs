using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio;

namespace Ankh.UI.PendingChanges
{
    class PendingCommitsView : ListViewWithSelection<PendingCommitItem>
    {
        internal override ListViewHierarchy CreateHierarchy()
        {
            return new PendingCommitsHierarchy(this);
        }

        protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
        {
            return base.IsInputKey(keyData);
            //return true;
        }

        protected override bool IsInputChar(char charCode)
        {
            return base.IsInputChar(charCode);
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
