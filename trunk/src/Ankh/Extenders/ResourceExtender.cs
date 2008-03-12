using System;
using System.Runtime.InteropServices;
using System.ComponentModel;


using SharpSvn;


namespace Ankh.Extenders
{

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IResourceExtender
    {
        string Url
        {
            get;
        }

        string RepositoryUuid
        {
            get;
        }

        DateTime LastCommittedDate
        {
            get;
        }

        long Revision
        {
            get;
        }

        string TextStatus
        {
            get;
        }

        string PropertyStatus
        {
            get;
        }



        string LastCommittedAuthor
        {
            get;
        }

        long LastCommittedRevision
        {
            get;
        }

        bool Locked
        {
            get;
        }

    }
    /// <summary>
    /// Summary description for TestExtender.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ResourceExtender : IResourceExtender
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public ResourceExtender(SvnItem item, IAnkhServiceProvider context)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (context == null)
                throw new ArgumentNullException("context");

            _item = item;
            _context = context;
        }

        [Category("Subversion"),
         Description("URL")]
        public string Url
        {
            get { return _item.Status.Uri.ToString(); }
        }

        [Category("Subversion"),
         Description("Repository UUID")]
        public string RepositoryUuid
        {
            get { return _item.Status.WorkingCopyInfo.RepositoryId.ToString(); }
        }

        [Category("Subversion"),
         Description("Last committed author")]
        public string LastCommittedAuthor
        {
            get { return _item.Status.WorkingCopyInfo.LastChangeAuthor; }
        }

        [Category("Subversion"),
         Description("Revision")]
        public long Revision
        {
            get { return _item.Status.WorkingCopyInfo.Revision; }
        }

        [Category("Subversion"),
         Description("Last committed date")]
        public DateTime LastCommittedDate
        {
            get { return _item.Status.WorkingCopyInfo.LastChangeTime.ToLocalTime(); }
        }

        [Category("Subversion"),
         Description("Last committed revision")]
        public long LastCommittedRevision
        {
            get { return _item.Status.WorkingCopyInfo.LastChangeRevision; }
        }

        [Category("Subversion"),
         Description("Text status")]
        public string TextStatus
        {
            get { return _item.Status.LocalContentStatus.ToString(); }
        }

        [Category("Subversion"),
         Description("Property status")]
        public string PropertyStatus
        {
            get { return _item.Status.LocalPropertyStatus.ToString(); }
        }

        [Category("Subversion"),
        Description("Locked")]
        public bool Locked
        {
            get { return _item.Status.LocalLocked; }
        }
    }
}
