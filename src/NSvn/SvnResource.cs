// $Id$
using System;
using NSvn.Common;
using NSvn.Core;
using System.Collections;
using System.IO;

namespace NSvn
{
    /// <summary>
    /// Base class for all entity classes
    /// </summary>
    public class SvnResource
    {       
        protected SvnResource()
        {
        }

        /// <summary>
        /// This is essentially a null object, representing a resource
        /// that cannot be versioned.
        /// </summary>
        public static readonly ILocalResource Unversionable = 
            new UnversionableResource();

        /// <summary>
        /// Create a ILocalResource from a local path.
        /// </summary>
        /// <param name="path">The path to the file/directory.</param>
        /// <returns>An ILocalResource object.</returns>
        public static ILocalResource FromLocalPath( string path )
        {
            if ( !Utils.IsWorkingCopyPath( path ) )
                return Unversionable;

            Status status  = Client.SingleStatus( path );
            System.Diagnostics.Debug.Assert( status != null, 
                "Couldn't get status for " + path );

            if ( status.TextStatus != StatusKind.Unversioned )
                return WorkingCopyResource.FromPath( path, status );
            else
                return UnversionedResource.FromPath( path );
        }



        /// <summary>
        /// The context object used in version control operations.
        /// </summary>
        public NSvnContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if ( this.context == null )
                    this.context = new NSvnContext();
                return this.context;
            }

            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.context = value; 
            }
        }

               

        /// <summary>
        /// The ClientContext to be used in version control operations.
        /// </summary>
        protected ClientContext ClientContext
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            { 
                return this.Context.ClientContext;
            }
        }      
  
        
        #region class UnversionableResource
        private class UnversionableResource : ILocalResource
        {

            #region Implementation of ILocalResource
            public void Accept(NSvn.ILocalResourceVisitor visitor)
            {
                // nothing        
            }

            public string Path
            {
                [System.Diagnostics.DebuggerStepThrough]
                get
                {
                    return "";
                }
            }

            public bool IsDirectory
            {
                [System.Diagnostics.DebuggerStepThrough]
                get
                {
                    return false;
                }
            }

            public bool IsVersioned
            {
                [System.Diagnostics.DebuggerStepThrough]
                get
                {
                    return false;
                }
            }

            public NSvn.Core.Status Status
            {
                [System.Diagnostics.DebuggerStepThrough]
                get
                {
                    return Status.None;
                }
            }

            public NSvn.NSvnContext Context
            {
                [System.Diagnostics.DebuggerStepThrough]
                get
                {
                    return null;
                }
                [System.Diagnostics.DebuggerStepThrough]
                set
                {
                }
            }
        #endregion



        }
        #endregion
        
        private NSvnContext context;
        private const string WCAREA=".svn";
    }
}
