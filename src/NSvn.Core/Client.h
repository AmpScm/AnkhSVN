// $Id$
#using <mscorlib.dll>
#using <NSvn.Common.dll>
#using <System.dll>

#include "Revision.h"
#include "CommitInfo.h"
#include "PropListItem.h"
#include "ClientContext.h"



using namespace System;
using namespace System::IO;
using namespace NSvn::Common;
using namespace System::Collections::Specialized;

namespace NSvn
{
    namespace Core
    {
        public __gc class Client
        {
        public:   

            ///<summary>Checkout a working copy.</summary>
            ///<param name="url">Path to the files/directory in the repository to be checked out.</url>
            ///<param name="path">Path to the destination.</param>
            ///<param name="revision">A revision, specified in Core::Revision.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown by NSvn.Core.</exception>
            static void Checkout(String* url, String* path, Revision* revision, bool recurse, 
                ClientContext* context);

            ///<summary>Update working tree path to revision.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="revision">A revision, specified in Core::Revision</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown by NSvn.Core.</exception>
	        static void Update(String* path, Revision* revision, bool recurse, ClientContext* context);

            ///<summary>Switch working tree path to url at revision, authenticating with the 
            ///         authentication baton </summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="url">Path to the files/directory in the repository.</url>
            ///<param name="revision">A revision, specified in Core::Revision.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown by NSvn.Core.</exception>
	        /*static void Switch(String* path, String* url, Revision* revision, bool recurse, 
                ClientContext* context);*/

            ///<summary>Add a file/directory, not already under revision control to a working copy.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
             static void Add(String* path, bool recursive, ClientContext* context);
            
            ///<summary>Create a directory, either in a repository or a working copy.</summary>
            ///<param name="path">Path to the directory.</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various data.</param> 
            ///<returns>Commit info object containing information about revision, date and author</returns>
	        static CommitInfo* MakeDir(String* path, ClientContext* context);
/*          
             static CommitInfo* Delete(String* path, AdminAccessBaton* admAccessBaton, bool force, 
                ClientContext* context);

	        static CommitInfo* Import(String* path, String* url, String* newEntry, bool nonRecursive, 
                ClientContext* context);
*/
            ///<summary>Commit file/directory into repository, authenticating with the 
            ///         authentication baton.</summary>
            ///<param name="targets">Array of paths to commit.</param>
            ///<param name="nonRecursive">Indicate that subdirectories of directory targets 
            ///                           should be ignored.</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
            ///<returns>Commit info object containing information about revision, date and author.</returns>
             static CommitInfo* Commit(String __gc* targets[], bool nonRecursive, ClientContext* context);

            ///<summary>Obtain the statuses of all the items in a working copy path.</summary>
            ///<param name="youngest">A revision number</param>
            ///<param name="path">Path to the directory.</param>
            ///<param name="descend">If descend is true, recurse fully, else do only 
            ///                      immediate children. (-n flag: nonrecursive)</param>
            ///<param name="getAll">If getAll is set, then all entries are retrieved; otherwise 
            ///                     only "interesting" entries will be fetched. (-v flag: verbose)</param>
            ///<param name="upDate">If upDate is set, then the repository will be contacted, so that 
            ///                     the structures in statushash are augmented with information 
            ///                     about out-of-dateness, and *youngest is set to the youngest 
            ///                     repository revision. (-u flag: show update) </param>
            ///<param name="noIgnore">?</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
            ///<returns></returns>
            //TODO:  StringDictionary to be reconsidered
	        static StringDictionary* Status(long* youngest, String* path, bool descend, 
                bool getAll, bool upDate,  bool noIgnore, ClientContext* context);
 /*           
	        static void Log(String* targets[], Revision* start, Revision* end, bool discoverChangePath, 
                bool strictNodeHistory, LogMessageReceiver* receiver, ClientContext* context);

	        static void Diff(String* diffOptions[], String* path1, Revision* revision1, 
                String* path2, Revision* revision2, bool recurse, bool noDiffDeleted, 
                Stream* outfile, Stream* errFile, ClientContext* context);

	        static void Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                String* targetWcPath, bool recurse, bool force, bool dryRun, ClientContext* context);

            ///<summary>Cleanup a working copy directory, finishing any incomplete operations, 
            ///         removing lockfiles, etc.</summary>
            ///<param name="dir">Path to the directory.</url>
              */static void Cleanup(String* dir);

            ///<summary>Restore the pristine version of a working copy path.</summary>
            ///<param name="path">Path to the file/directory</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for revert as well.</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
	        static void Revert(String* path, bool recursive, ClientContext* context);

            ///<summary>Resolve conflict. Remove the 'conflicted' state on a working copy path.</summary>
            ///<param name="path">Path to the file(/directory)</param>
            ///<param name="recursive">If recursive is set, recurse below path, looking for 
            ///                         conflicts to resolve. (To be implemented in the future.)</param>
            ///<param name="context">A client context class, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.</param> 
	        static void Resolve(String* path, bool recursive, ClientContext* context);
            /*

	        static CommitInfo* Copy(String* srcPath, Revision* srcRevision, String* dst, 
                ClientContext* context);

	        static CommitInfo* Move(String* srcPath, Revision* srcRevision, String* dstPath, 
                bool force, ClientContext* context);
*/
            ///<summary>Set a property to a file/directory</summary>
            ///<param name="propName">Name of property</param>
            ///<param name="propval">Value of property</param>
            ///<param name="target">Target of property. Which file/directory to set the property.</param>
            ///<param name="recurse">If recurse is true, then propname will be set recursively 
            ///                      on target and all children.</param>  
	        static void PropSet(String* propName, Byte propval[], String* target, bool recurse);
/*
	        static void RevPropSet(String* propName, Byte propval[], String* url, Revision* revision, 
                RevisionNumber* setRev, ClientContext* context);

	        static StringDictionary* PropGet(String* propName, String* target, Revision* revision, 
                bool recurse, ClientContext* context);

	        static Byte RevPropGet(String* propName, String* url, Revision* revision, 
                RevisionNumber* setRev, ClientContext* context) [];

	        static PropListItem* PropList(String* target, Revision* revision, 
                AuthenticationBaton* authBaton, bool recurse, ClientContext* context)[];

	        static StringDictionary* RevPropList(String* url, Revision* revision, RevisionNumber* setRev, 
                ClientContext* context);

	        static void Export(String* url, String* to, Revision* revision, ClientContext* context);

	        static String* Ls(String* url, Revision* revision, bool recurse, ClientContext* context)[];

	        static void Cat(Stream* out, String* url, Revision* revision, ClientContext* context);*/
        private:
            // ctor made private to avoid instantiation of this class
            Client(){;}
            static void ByteArrayToSvnString( svn_string_t* string, Byte array[], 
                const Pool& pool );
            static String* CanonicalizePath( String* path );

            static apr_array_header_t* StringArrayToAprArray( String* strings[], Pool& pool );
        };
    }
}
