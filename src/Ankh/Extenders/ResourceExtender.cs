using System;
using System.Runtime.InteropServices;
using System.ComponentModel;


using SharpSvn;


namespace Ankh.Extenders
{
    /// <summary>
    /// Extends <see cref="SvnItem"/> in the property grid
    /// </summary>
    [ComVisible(true)]
    public class SvnItemExtender
    {
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
        }

        [Category("Subversion"), Description("URL")]
        public Uri Url
        {
            get { return _item.Status.Uri; }
        }

        [Category("Subversion"), Description("Repository UUID")]
        public string RepositoryUuid
        {
            get { return _item.Status.RepositoryId; }
        }

        [Category("Subversion"), Description("Last committed author")]
        public string LastCommittedAuthor
        {
            get { return _item.Status.LastChangeAuthor; }
        }

        [Category("Subversion"), Description("Revision")]
        public long Revision
        {
            get { return _item.Status.Revision; }
        }

        [Category("Subversion"), Description("Last committed date")]
        public DateTime LastCommittedDate
        {
            get { return _item.Status.LastChangeTime.ToLocalTime(); }
        }

        [Category("Subversion"), Description("Last committed revision")]
        public long LastCommittedRevision
        {
            get { return _item.Status.LastChangeRevision; }
        }

        [Category("Subversion"), Description("Text status")]
        public string TextStatus
        {
            get { return _item.Status.LocalContentStatus.ToString(); }
        }

        [Category("Subversion"), Description("Property status")]
        public string PropertyStatus
        {
            get { return _item.Status.LocalPropertyStatus.ToString(); }
        }

        [Category("Subversion"), Description("Locked")]
        public bool Locked
        {
            get { return _item.Status.IsLockedLocal; }
        }
    }
}
