using System;
using System.Runtime.InteropServices;
using System.ComponentModel;


using SharpSvn;


namespace Ankh.Extenders
{

    [ComVisible(true)]
    [InterfaceType( ComInterfaceType.InterfaceIsDual )]
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
        public ResourceExtender( )
        {
            // empty
        }

        public override bool Equals(object o)
        {
            if (o == (object)this)
                return true;

            ResourceExtender other = o as ResourceExtender;
            if (other == null)
                return false;

            return other.status.Uri == this.status.Uri;
        }

        public override int GetHashCode()
        {
            return this.status.Uri.GetHashCode();
        }


        [Category( "Subversion" ),
         Description("URL" )]
        public string Url
        {
            get{ return this.status.Uri.ToString(); }
        }

        [Category( "Subversion" ),
         Description( "Repository UUID" )]
        public string RepositoryUuid
        {
            get  {  return this.status.WorkingCopyInfo.RepositoryId.ToString(); }
        }

        [Category( "Subversion" ),
         Description( "Last committed author" )]
        public string LastCommittedAuthor
        {
            get{ return this.status.WorkingCopyInfo.LastChangeAuthor; }
        }

        [Category("Subversion"),
         Description( "Revision" )]                
        public long Revision
        {
            get{ return this.status.WorkingCopyInfo.Revision; }
        }

        [Category("Subversion"),
         Description( "Last committed date" )]
        public DateTime LastCommittedDate
        {
            get{ return this.status.WorkingCopyInfo.LastChangeTime.ToLocalTime(); }
        }

        [Category("Subversion"),
         Description( "Last committed revision" )]
        public long LastCommittedRevision
        {
            get{ return this.status.WorkingCopyInfo.LastChangeRevision; }
        }

        [Category("Subversion"),
         Description( "Text status" )]
        public string TextStatus
        {
            get{ return this.status.LocalContentStatus.ToString(); }
        }

        [Category("Subversion"),
         Description( "Property status" )]
        public string PropertyStatus
        {
            get{ return this.status.LocalPropertyStatus.ToString(); }
        }

        [Category("Subversion"),
        Description( "Locked" )]
        public bool Locked
        {
            get{ return this.status.LocalLocked; }
        }

        public void SetStatus(AnkhStatus status)
        {
            if (status == null)
                throw new ArgumentNullException("status");
            this.status = status;
        }

		private AnkhStatus status;
    }
}
