using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using NSvn;
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
        internal ResourceExtender( )
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


        [Category( "Subversion" )]
        public string Url
        {
            get{ return this.status.Entry.Url; }
        }

        [Category( "Subversion" )]
        public string RepositoryUuid
        {
            get  {  return this.status.Entry.Uuid; }
        }

        [Category( "Subversion" )]
        public string LastCommittedAuthor
        {
            get{ return this.status.Entry.CommitAuthor; }
        }

        [Category("Subversion")]
        public int Revision
        {
            get{ return this.status.Entry.Revision; }
        }

        [Category("Subversion")]
        public DateTime LastCommittedDate
        {
            get{ return this.status.Entry.CommitDate; }
        }

        [Category("Subversion")]
        public int LastCommittedRevision
        {
            get{ return this.status.Entry.CommitRevision; }
        }

        [Category("Subversion")]
        public string TextStatus
        {
            get{ return this.status.TextStatus.ToString(); }
        }

        [Category("Subversion")]
        public string PropertyStatus
        {
            get{ return this.status.PropertyStatus.ToString(); }
        }

        internal Status Status
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
