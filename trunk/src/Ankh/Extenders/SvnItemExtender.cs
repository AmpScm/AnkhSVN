using System;
using System.Runtime.InteropServices;
using System.ComponentModel;


using SharpSvn;
using Ankh.Selection;


namespace Ankh.Extenders
{
    /// <summary>
    /// Extends <see cref="SvnItem"/> in the property grid
    /// </summary>
    [ComVisible(true)]
    public class SvnItemExtender
    {
        readonly ISelectionContext _selContext;
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public SvnItemExtender(SvnItem item, IAnkhServiceProvider context)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (context == null)
                throw new ArgumentNullException("context");

            _item = item;
            _context = context;
            _selContext = context.GetService<ISelectionContext>();
        }

        [Browsable(false)]
        internal SvnItem SvnItem
        {
            get 
            {
                foreach (SvnItem item in _selContext.GetSelectedSvnItems(false))
                {
                    return item;
                }

                return _item; 
            }
        }

        [Category("Subversion"), Description("Url"), DisplayName("Url")]
        public Uri Url
        {
            get { return SvnItem.Status.Uri; }
        }

        [Category("Subversion"), DisplayName("Change List"), Description("Change List")]
        public string ChangeList
        {
            get { return SvnItem.Status.ChangeList; }
        }

        [Category("Subversion"), Description("Last committed author"), DisplayName("Last Author")]
        public string LastCommittedAuthor
        {
            get { return SvnItem.Status.LastChangeAuthor; }
        }

        [Category("Subversion"), Description("Revision")]
        public long Revision
        {
            get { return SvnItem.Status.Revision; }
        }

        [Category("Subversion"), Description("Last committed date")]
        public DateTime LastCommittedDate
        {
            get { return SvnItem.Status.LastChangeTime.ToLocalTime(); }
        }

        [Category("Subversion"), Description("Last committed revision")]
        public long LastCommittedRevision
        {
            get { return SvnItem.Status.LastChangeRevision; }
        }

        [Category("Subversion"), Description("Text Status"), DisplayName("Text Status")]
        public string TextStatus
        {
            get { return SvnItem.Status.LocalContentStatus.ToString(); }
        }

        [Category("Subversion"), Description("Property status"), DisplayName("Property Status")]
        public string PropertyStatus
        {
            get { return SvnItem.Status.LocalPropertyStatus.ToString(); }
        }

        [Category("Subversion"), Description("Locked")]
        public bool Locked
        {
            get { return SvnItem.Status.IsLockedLocal; }
        }
    }
}
