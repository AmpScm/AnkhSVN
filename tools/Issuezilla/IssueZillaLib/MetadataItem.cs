using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public class MetadataItem<TKey, TDisplay>
    {
        public MetadataItem( TKey key, TDisplay displayText)
        {
            this.key = key;
            this.displayText = displayText;
        }

        public TKey Key
        {
            get { return this.key; }
        }

        public TDisplay DisplayText 
        {
            get { return this.displayText; }
        }

        private TKey key;
        private TDisplay displayText;
    }
}
