// $Id$
#using <mscorlib.dll>
#using <NSvn.Common.dll>
#using <System.dll>
#include <apr_tables.h>

#include "Revision.h"
#include "CommitInfo.h"
#include "ClientContext.h"
#include "DirectoryEntry.h"
#include "Status.h"
#include "StatusDictionary.h"



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
            ///<param name="url">Path to the files/directory in the repository to be checked out.</param>
            ///<param name="path">Path to the destination.</param>
            ///<param name="revision">A revision, specified in Core::Revision.<see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
            static void Checkout(String* url, String* path, Revision* revision, bool recurse, 
                ClientContext* context);

            ///<summary>Update working tree path to revision.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
	        static void Update(String* path, Revision* revision, bool recurse, ClientContext* context);

            ///<summary>Switch working tree path to url at revision, authenticating with the 
            ///         authentication baton </summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="url">Path to the files/directory in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
	        static void Switch(String* path, String* url, Revision* revision, bool recurse, 
                ClientContext* context);

            ///<summary>Add a file/directory, not already under revision control to a working copy.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
             static void Add(String* path, bool recursive, ClientContext* context);
            
            ///<summary>Create a directory, either in a repository or a working copy.</summary>
            ///<param name="path">Path to the directory.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various data. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
	        static CommitInfo* MakeDir(String* path, ClientContext* context);
        
            ///<summary>Delete a file/directory, either in a repository or a working copy.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="admAccessBaton">Either a baton that holds a write 
			///								lock for the parent of path in 
			///								working copy, or NULL.</param>
            ///<param name="force">If force is set all the files and all 
			///						unversioned items in a directory in a 
			///						working copy  will be removed.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various data. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
			//TODO: Implement the variable admAccessBaton   
             static CommitInfo* Delete(String* path, bool force, ClientContext* context);

            ///<summary>Import file or directory path into repository directory url at head, 
            ///         authenticating with the authentication baton</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="url">Path to the files/directory in the repository.</param>
            ///<param name="newEntry">New entry (directory) created in the repository  
			///	 					identified by url, may be null.</param>
            ///<param name="nonRecursive">Indicate that subdirectories of directory targets 
            ///                           should be ignored.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various data. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns> 
	        static CommitInfo* Import(String* path, String* url, String* newEntry, bool nonRecursive, 
                ClientContext* context);

            ///<summary>Commit file/directory into repository, authenticating with the 
            ///         authentication baton.</summary>
            ///<param name="targets">Array of paths to commit.</param>
            ///<param name="nonRecursive">Indicate that subdirectories of directory targets 
            ///                           should be ignored.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
             static CommitInfo* Commit(String __gc* targets[], bool nonRecursive, ClientContext* context);

            ///<summary>Obtain the statuses of all the items in a working copy path.</summary>
            ///<param name="youngest">A revision number</param>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="descend">If descend is true, recurse fully, else do only 
            ///                      immediate children. (-n flag: nonrecursive)</param>
            ///<param name="getAll">If getAll is set, then all entries are retrieved; otherwise 
            ///                     only "interesting" entries will be fetched. (-v flag: verbose)</param>
            ///<param name="upDate">If upDate is set, then the repository will be contacted, so that 
            ///                     the structures in statushash are augmented with information 
            ///                     about out-of-dateness, and *youngest is set to the youngest 
            ///                     repository revision. (-u flag: show update) </param>
            ///<param name="noIgnore"></param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns></returns>
            //TODO:  StringDictionary to be reconsidered
	        static StatusDictionary* Status(
                [System::Runtime::InteropServices::Out]System::Int32* youngest, 
                String* path, bool descend, bool getAll, bool upDate,  
                bool noIgnore, ClientContext* context);
 /*          
            ///<summary>Obtain log information from the repository.</summary>
            ///<param name="targets">Targets contains all the working copy paths for 
            ///                      desired log messages.</param>
            ///<param name="start">Revision to start the log.</param> 
            ///<param name="end">Revision to end the log.</param> 
            ///<param name="discoverChangePath">If discoverChangePath is set, 
            ///             then the `changedPaths' argument to receiver will be 
            ///             passed on each invocation.</param> 
            ///<param name="strictNodeHistory"></param> 
            ///<<param name="receiver">Receiver of the log.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things.<see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>   
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
	        static void Log(String* targets[], Revision* start, Revision* end, bool discoverChangePath, 
                bool strictNodeHistory, LogMessageReceiver* receiver, ClientContext* context);
 */           
            ///<summary>Produce diff output which describes the delta between path1/revision1 
            ///         and path2/revision2.</summary>
            ///<param name="path1">Path to first file/directory in working copy or repository.</param> 
            ///<param name="revision1">First revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param> 
            ///<param name="path2">Path to second file/directory in working copy or repository.</param> 
            ///<param name="revision2">Second revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param> 
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param> 
            ///<param name="noDiffDeleted">If noDiffDeleted is true, then no diff output will 
            ///                             be generated on deleted files</param> 
            ///<param name="outfile">File that contains output of the diff.</param> 
            ///<param name="errFile">File that contains errors of the diff.</param> 
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>   
            ///<returns></returns>
	        /*static void Diff(String* diffOptions[], String* path1, Revision* revision1, 
                String* path2, Revision* revision2, bool recurse, bool noDiffDeleted, 
                Stream* outfile, Stream* errFile, ClientContext* context);*/


            ///<summary>Apply file differences into a working copy. Merge changes 
            ///         from url1/revision1 to url2/revision2 into a working-copy. 
            ///</summary>
            ///<param name="url1">Path to first file/directory in repository.</param> 
            ///<param name="revision1">First revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param> 
            ///<param name="url2">Path to second file/directory in repository.</param> 
            ///<param name="revision2">Second revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param> 
            ///<param name="targetWCPath">Working copy paths for desired merge.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                      all of its contents will be included in the merge.</param> 
            ///<param name="force">If force is set locally modified or 
            ///                    unversioned items will be deleted if essential.</param>
            ///<param name="dryRun">If dryRun is true  the merge is carried out, and full 
            ///                     notfication feedback is provided, but the working 
            ///                     copy is not modified.</param> 
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        static void Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                String* targetWcPath, bool recurse, bool force, bool dryRun, ClientContext* context);

            ///<summary>Cleanup a working copy directory, finishing any incomplete operations, 
            ///         removing lockfiles, etc.</summary>
            ///<param name="dir">Path to the directory.</param>
            static void Cleanup(String* dir);

            ///<summary>Restore the pristine version of a working copy path.</summary>
            ///<param name="path">Path to the file/directory</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for revert as well.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        static void Revert(String* path, bool recursive, ClientContext* context);

            ///<summary>Resolve conflict. Remove the 'conflicted' state on a working copy path.</summary>
            ///<param name="path">Path to the file(/directory)</param>
            ///<param name="recursive">If recursive is set, recurse below path, looking for 
            ///                         conflicts to resolve. (To be implemented in the future.)</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        static void Resolve(String* path, bool recursive, ClientContext* context);
        

            ///<summary>Copy a file/directory.</summary>
            ///<param name="srcPath">Path to the file/directory to be copied.</param>
            ///<param name="srcRevision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="dstPath">Path to the destination.</param>
            ///<param name="optionalAdmAccess></param>  //New 6.3.2003
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns> 
            //TODO: Implement the variable optionalAdmAccess
	        static CommitInfo* Copy(String* srcPath, Revision* srcRevision, String* dstPath,
                ClientContext* context); 


            ///<summary>Move a file/directory.</summary>
            ///<param name="srcPath">Path to the file/directory to be moved.</param>
            ///<param name="srcRevision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="dstPath">Path to the destination.</param>
            ///<param name="force">If force is set locally modified and/or unversioned items will 
            ///                     be removed.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
	        static CommitInfo* Move(String* srcPath, Revision* srcRevision, String* dstPath, 
                bool force, ClientContext* context);

            ///<summary>Set a property to a file/directory</summary>
            ///<param name="propName">Name of property</param>
            ///<param name="propval">Value of property</param>
            ///<param name="target">Target of property. Which file/directory to set the property.</param>
            ///<param name="recurse">If recurse is true, then propname will be set recursively 
            ///                      on target and all children.</param>  
	        static void PropSet(Property* property, String* target, bool recurse);
/*
            ///<summary>Set a property to a revision in the repository.</summary>
            ///<param name="propName">Name of property.</param>
            ///<param name="propval">Value of property.</param>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">A revision number.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
              static void RevPropSet(String* propName, Byte propval[], String* url, Revision* revision, 
                RevisionNumber* setRev, ClientContext* context);
            */
            ///<summary>Get properties from an entry in a working copy or repository.</summary>
            ///<param name="propName">Name of property.</param>
            ///<param name="target">Target of property. Which file/directory to get the property.</param>         
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is true, then propname will be received recursively 
            ///                      on target and all children.</param>  	
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        static PropertyDictionary* PropGet(String* propName, String* target, Revision* revision, 
                bool recurse, ClientContext* context);

            /*
   
            ///<summary>Get a revision property from a repository.</summary>
            ///<param name="propName">Name of property.</param>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">A revision number.</param>        
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>  
            ///<returns></returns>
	        static Byte RevPropGet(String* propName, String* url, Revision* revision, 
                RevisionNumber* setRev, ClientContext* context) [];
*/
            ///<summary>List the properties on an entry in a working copy or repository.</summary>          
            ///<param name="target">An url or working copy path.</param>         
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is false property will contain only a single 
            ///                        element.</param>  
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>  
            ///<returns></returns>
	        static PropListItem* PropList(String* target, Revision* revision, bool recurse, ClientContext* context)[];

            ///<summary>List the revision properties on an entry in a repository.</summary>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">Set to the actual revision number affected upon return.</param>        
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>  
            ///<returns></returns>
            static PropertyDictionary* RevPropList(String* url, Revision* revision, 
                [System::Runtime::InteropServices::Out]System::Int32* setRev, 
                ClientContext* context);

            ///<summary>Export the contents of either a subversion repository or a subversion. </summary>
            ///         working copy into a directory with no svn administrative directories (.svn).</summary> 
            ///<param name="from">Path to the files/directory to be exported.</param>
            ///<param name="to">Path to the directory where you wish to create 
            ///                 the exported tree.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param>       
	        static void Export(String* from, String* to, Revision* revision, ClientContext* context);

            ///<summary>List the contents of an url or path.</summary>
            ///<param name="path">Path to the files/directory to be listed.</param>  
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is true, then propname will be received recursively 
            ///                      on target and all children.</param>  	
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        ///<returns>String table of the paths to be listed.</returns>
	        static DirectoryEntry* List(String* path, Revision* revision, bool recurse, 
                ClientContext* context) [];

            ///<summary>List the contents of a file.</summary>
            ///<param name="out"></param>  
            ///<param name="path">Path to the file to be edited.</param>  
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="context">A client context object, which holds client specific 
            ///                      callbacks, batons, serves as a cache for configuration options, 
            ///                      and other various things. <see cref="NSvn.Core.ClientContext"/> 
            ///                      for more information.</param> 
	        static void Cat(Stream* out, String*path, Revision* revision, ClientContext* context);
        private:
            // ctor made private to avoid instantiation of this class
            Client(){;}
           
            static NSvn::Common::PropertyDictionary* ConvertToPropertyDictionary( 
                apr_hash_t* propertyHash, String* propertyName, Pool& pool );

            static NSvn::Common::PropListItem* ConvertPropListArray( 
                apr_array_header_t* propListItems, Pool& pool ) [];
            
        };
    }
}
