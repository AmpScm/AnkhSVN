using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

using NSvn.Core;


namespace Ankh.Extenders
{

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

    }
    /// <summary>
    /// Summary description for TestExtender.
    /// </summary>
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

            return other.Status.Entry.Url == this.Status.Entry.Url;
        }

        public override int GetHashCode()
        {
            return this.Status.Entry.Url.GetHashCode();
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

        public Status Status
        {
            get{ return this.status; }
            set
            {
                this.status = value;                
            }
        }

        private Status status;
    }
}
