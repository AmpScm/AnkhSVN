using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Ankh.UI
{
    public sealed class PendingChange 
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;

        public PendingChange(IAnkhServiceProvider context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
        }

        [Browsable(false)]
        public SvnItem Item
        {
            get { return _item; }
        }

        public string FullPath
        {
            get { return _item.FullPath; }
        }
    }
}
