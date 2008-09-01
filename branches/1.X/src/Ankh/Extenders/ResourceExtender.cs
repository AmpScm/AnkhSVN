using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

using NSvn.Core;


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

        int Revision
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

        int LastCommittedRevision
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

        public override bool Equals( object o )
        {
            if ( o == (object)this )
                return true;

            ResourceExtender other = o as ResourceExtender;
            if ( other == null )
                return false;

            return other.status.Entry.Url == this.status.Entry.Url;
        }

        public override int GetHashCode()
        {
            return this.status.Entry.Url.GetHashCode();
        }


        [Category( "Subversion" ),
         Description("URL" )]
        public string Url
        {
            get{ return this.status.Entry.Url; }
        }

        [Category( "Subversion" ),
         Description( "Repository UUID" )]
        public string RepositoryUuid
        {
            get  {  return this.status.Entry.Uuid; }
        }

        [Category( "Subversion" ),
         Description( "Last committed author" )]
        public string LastCommittedAuthor
        {
            get{ return this.status.Entry.CommitAuthor; }
        }

        [Category("Subversion"),
         Description( "Revision" )]                
        public int Revision
        {
            get{ return this.status.Entry.Revision; }
        }

        [Category("Subversion"),
         Description( "Last committed date" )]
        public DateTime LastCommittedDate
        {
            get{ return this.status.Entry.CommitDate; }
        }

        [Category("Subversion"),
         Description( "Last committed revision" )]
        public int LastCommittedRevision
        {
            get{ return this.status.Entry.CommitRevision; }
        }

        [Category("Subversion"),
         Description( "Text status" )]
        public string TextStatus
        {
            get{ return this.status.TextStatus.ToString(); }
        }

        [Category("Subversion"),
         Description( "Property status" )]
        public string PropertyStatus
        {
            get{ return this.status.PropertyStatus.ToString(); }
        }

        [Category("Subversion"),
        Description( "Locked" )]
        public bool Locked
        {
            get{ return this.status.Entry != null && this.status.Entry.LockToken != null; }
        }

        public void SetStatus( Status status )
        {
            this.status = status;                            
        }

        private Status status;
    }
}
