using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using NSvn;


namespace Ankh.Extenders
{

    [InterfaceType( ComInterfaceType.InterfaceIsDual )]
    public interface ITestExtender
    {
        string Url
        {
            get;
        }

        string RepositoryUuid
        {
            get;
        }

        string Author
        {
            get;
        }

    }
    /// <summary>
    /// Summary description for TestExtender.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public class TestExtender : ITestExtender
    {
        [Category( "Subversion" )]
        public string Url
        {
            get{ return this.Resource.Status.Entry.Url; }
        }

        [Category( "Subversion" )]
        public string RepositoryUuid
        {
            get  {  return this.Resource.Status.Entry.Uuid; }
        }

        [Category( "Subversion" )]
        public string Author
        {
            get{ return this.Resource.Status.Entry.CommitAuthor; }
        }

        internal WorkingCopyResource Resource
        {
            get{ return this.resource; }
            set{ this.resource = value; }
        }

        private WorkingCopyResource resource;
    }
}
