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
        /// Create a ILocalResource from a local path.
        /// </summary>
        /// <param name="path">The path to the file/directory.</param>
        /// <returns>An ILocalResource object.</returns>
        public static ILocalResource FromLocalPath( string path )
        {
            if ( !IsVersioned( path ) )
                return null;

            int youngest;
            StatusDictionary dict = Client.Status( out youngest, path, 
                false, true, false, false, new ClientContext() );
            Status status = dict.GetFirst();
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
            get
            {
                if ( this.context == null )
                    this.context = new NSvnContext();
                return this.context;
            }

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
            get
            { 
               return this.Context.ClientContext;
            }
        }        

        /// <summary>
        /// Checks whether a given path is versioned.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsVersioned( string path )
        {
            string baseDir = File.Exists( path ) ? Path.GetDirectoryName( path ) : 
                path;
            
            return Directory.Exists( Path.Combine( baseDir, WCAREA ) );
        }

        
        
            

        private NSvnContext context;
        private const string WCAREA=".svn";
	}
}
