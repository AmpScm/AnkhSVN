// $Id$
#using <mscorlib.dll>
#using <NSvn.Common.dll>
#using <System.dll>
#include <apr_tables.h>

#include "stdafx.h"
#include "delegates.h"



using namespace System;
using namespace System::IO;
using namespace NSvn::Common;
using namespace System::Collections::Specialized;

namespace NSvn
{
    namespace Core
    {
        public __gc class ClientContext;
        public __gc class LogMessageEventArgs;
        public __gc class NotificationEventArgs;
        public __gc class CancelEventArgs;
        public __gc class Revision;
        public __gc class Status;
        public __gc class DirectoryEntry;
        public __gc class StatusDictionary;
        public __gc class CommitInfo;
        public __gc class AuthenticationProvider;
        public __gc class AuthenticationBaton;

        public __gc class Client
        {
        public:
            ///<summary>This event is fired to alert about various actions performed 
            /// on paths.</summary>
            __event NotificationDelegate* Notification;

            ///<summary>This event is fired during long running operations to give
            /// the user a chance to cancel the operation.</summary>
            __event CancelDelegate* Cancel;

            ///<summary>This event is fired whenever the current operation requires a
            ///log message,</summary>
            __event LogMessageDelegate* LogMessage;

            ///<summary>Constructor.</summary>
            Client();
            

            ///<summary>Constructor.</summary>
            ///<param name="url">The Subversion configuration directory to use.</param>
            Client( String* configDir );


            [System::Diagnostics::DebuggerStepThrough]
             __property AuthenticationBaton* get_AuthBaton();

             /// <summary>The name of the Subversion administrative
             /// directory.</summary>
             [System::Diagnostics::DebuggerStepThrough]
             __property static String* get_AdminDirectoryName();
#if defined(ALT_ADMIN_DIR)    
             [System::Diagnostics::DebuggerStepThrough]
             __property static void set_AdminDirectoryName( System::String* name );
#endif
            ///<summary>Checkout a working copy.</summary>
            ///<param name="url">Path to the files/directory in the repository to be checked out.</param>
            ///<param name="path">Path to the destination.</param>
            ///<param name="revision">A revision, specified in Core::Revision.<see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            /// <returns>The revision affected</returns>
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
            int Checkout(String* url, String* path, Revision* revision, bool recurse);

            ///<summary>Update working tree path to revision.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            /// <returns>The revision affected</returns>
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
            int Update(String* path, Revision* revision, bool recurse );

            ///<summary>Switch working tree path to url at revision, authenticating with the 
            ///         authentication baton </summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="url">Path to the files/directory in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                      for more information.</param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>            
            /// <returns>The revision affected</returns>
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
            int Switch(String* path, String* url, Revision* revision, bool recurse);

           

            ///<summary>Add a file/directory, not already under revision control to a working copy.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for addition as well.</param>
            void Add(String* path, bool recursive);

            ///<summary>Create a directory, either in a repository or a working copy.</summary>
            ///<param name="path">Path to the directory.</param>
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
            CommitInfo* MakeDir(String* paths[]);

            ///<summary>Delete a file/directory, either in a repository or a working copy.</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="admAccessBaton">Either a baton that holds a write 
            ///								lock for the parent of path in 
            ///								working copy, or NULL.</param>
            ///<param name="force">If force is set all the files and all 
            ///						unversioned items in a directory in a 
            ///						working copy  will be removed.</param>
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
            //TODO: Implement the variable admAccessBaton   
            CommitInfo* Delete(String* paths[], bool force);

            ///<summary>Import file or directory path into repository directory url at head, 
            ///         authenticating with the authentication baton</summary>
            ///<param name="path">Path to the file/directory.</param>
            ///<param name="url">Path to the files/directory in the repository.</param>
            ///<param name="nonRecursive">Indicate that subdirectories of directory targets 
            ///                           should be ignored.</param>
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns> 
            CommitInfo* Import(String* path, String* url, bool nonRecursive);

            ///<summary>Commit file/directory into repository, authenticating with the 
            ///         authentication baton.</summary>
            ///<param name="targets">Array of paths to commit.</param>
            ///<param name="nonRecursive">Indicate that subdirectories of directory targets 
            ///                           should be ignored.</param>
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
            CommitInfo* Commit(String __gc* targets[], bool nonRecursive);

            /// TODO: doc comments
            Status* SingleStatus( String* path );

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
            ///<returns>StatusDictionary object containing status information. 
            ///        <see cref="NSvn.Core.StatusDictionary"></returns>
            void Status(
                [System::Runtime::InteropServices::Out]System::Int32* youngest, 
                String* path, Revision* revision, StatusCallback* statusCallback, bool descend, bool getAll,
                bool update,  bool noIgnore );



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
            ///<exception cref="NSvn.Core.SvnClientException">Exceptions thrown if an error occurs.</exception>
            void Log(String* targets[], Revision* start, Revision* end, bool discoverChangePath, 
                bool strictNodeHistory, LogMessageReceiver* receiver );

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
            ///<param name="ignoreAncestry">Use ignore_ancestry to control whether or not items 
            /// being diffed will be checked for relatedness first. Unrelated items are 
            /// typically transmitted to the editor as a deletion of one thing and the 
            /// addition of another, but if this flag is TRUE, unrelated items will be 
            /// diffed as if they were related. </param>


            ///<param name="noDiffDeleted">If noDiffDeleted is true, then no diff output will 
            ///                             be generated on deleted files</param> 
            ///<param name="outfile">File that contains output of the diff.</param> 
            ///<param name="errFile">File that contains errors of the diff.</param>  
            void Diff(String* diffOptions[], String* path1, Revision* revision1, 
                String* path2, Revision* revision2, bool recurse, bool ignoreAncestry, 
                bool noDiffDeleted, 
                Stream* outfile, Stream* errFile);

            String* GetPristinePath(String* path);

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
            ///<param name="ignoreAncestry">Use ignore_ancestry to control whether or not items 
            /// being diffed will be checked for relatedness first. Unrelated items are 
            /// typically transmitted to the editor as a deletion of one thing and the 
            /// addition of another, but if this flag is TRUE, unrelated items will be 
            /// diffed as if they were related. </param>
            ///<param name="recurse">If recursive is set, assuming path is a directory 
            ///                      all of its contents will be included in the merge.</param> 
            ///<param name="force">If force is set locally modified or 
            ///                    unversioned items will be deleted if essential.</param>
            ///<param name="dryRun">If dryRun is true  the merge is carried out, and full 
            ///                     notfication feedback is provided, but the working 
            ///                     copy is not modified.</param> 
            void Merge(String* url1, Revision* revision1, String* url2, Revision* revision2, 
                String* targetWcPath, bool recurse, bool ignoreAncestry, bool force, bool dryRun);

            ///<summary>Cleanup a working copy directory, finishing any incomplete operations, 
            ///         removing lockfiles, etc.</summary>
            ///<param name="dir">Path to the directory.</param>
            void Cleanup( String* dir );

            ///<summary>Restore the pristine version of a working copy path.</summary>
            ///<param name="path">Paths to the files/directories</param>
            ///<param name="recursive">If recursive is set, assuming path is a directory 
            ///                        all of its contents will be scheduled for revert as well.</param>
            void Revert(String* paths[], bool recursive);

            ///<summary>Resolve conflict. Remove the 'conflicted' state on a working copy path.</summary>
            ///<param name="path">Path to the file(/directory)</param>
            ///<param name="recursive">If recursive is set, recurse below path, looking for 
            ///                         conflicts to resolve. (To be implemented in the future.)</param>
            void Resolved(String* path, bool recursive);       

            ///<summary>Copy a file/directory.</summary>
            ///<param name="srcPath">Path to the file/directory to be copied.</param>
            ///<param name="srcRevision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="dstPath">Path to the destination.</param>
            ///<param name="optionalAdmAccess></param>  //New 6.3.2003
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns> 
            //TODO: Implement the variable optionalAdmAccess
            CommitInfo* Copy(String* srcPath, Revision* srcRevision, String* dstPath); 

            ///<summary>Move a file/directory.</summary>
            ///<param name="srcPath">Path to the file/directory to be moved.</param>
            ///<param name="srcRevision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="dstPath">Path to the destination.</param>
            ///<param name="force">If force is set locally modified and/or unversioned items will 
            ///                     be removed.</param>
            ///<returns>Commit info object containing information about revision, date and author. 
            ///         <see cref="NSvn.Core.CommitInfo"/> for more information.</returns>
            CommitInfo* Move(String* srcPath, Revision* srcRevision, String* dstPath, 
                bool force);

            ///<summary>Set a property to a file/directory</summary>
            ///<param name="property">Object that contain a value and a name.
            ///         <see cref="NSvn.Common.Property"/></param>
            ///<param name="target">Target of property. Which file/directory to set the property.</param>
            ///<param name="recurse">If recurse is true, then propname will be set recursively 
            ///                      on target and all children.</param>  
            void PropSet(Property* property, String* target, bool recurse);

            /// <summary>Modify a working copy directory dir, changing any 
            /// repository URLs that begin with from to begin with to instead, 
            /// recursing into subdirectories if recurse is true.</summary>
            /// <param name="dir">Working copy directory.</param>
            /// <param name="from">Original URL.</param>
            /// <param name="to">New URL.</param>
            /// <param name="recurse">Whether to recurse into subdirectories.</a>
            void Relocate(String* dir, String* from, String* to, bool recurse);

            ///<summary>Set a property to a revision in the repository.</summary>
            ///<param name="property">Object that contain a value and a name.
            ///         <see cref="NSvn.Common.Property"/></param>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">A revision number.</param>
            void RevPropSet(Property* property, String* url, Revision* revision, 
                [System::Runtime::InteropServices::Out]System::Int32* setRev, bool force);

            ///<summary>Get properties from an entry in a working copy or repository.</summary>
            ///<param name="propName">Name of property.</param>
            ///<param name="target">Target of property. Which file/directory to get the property.</param>         
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is true, then propname will be received recursively 
            ///                      on target and all children.</param>
            ///<returns>PropertyDictionary object that contain a list of names and values of properties.
            ///         <see cref="NSvn.Common.PropertyDictionary"/></returns>
            PropertyDictionary* PropGet(String* propName, String* target, Revision* revision, 
                bool recurse);



            ///<summary>Get a revision property from a repository.</summary>
            ///<param name="propName">Name of property.</param>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">A revision number.</param>     
            ///<returns>Property object that contain a value and a name.
            ///         <see cref="NSvn.Common.Property"/></returns>
            Property* RevPropGet(String* propName, String* url, 
                Revision* revision, [System::Runtime::InteropServices::Out]System::Int32* setRev);

            ///<summary>List the properties on an entry in a working copy or repository.</summary>          
            ///<param name="target">An url or working copy path.</param>         
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is false property will contain only a single 
            ///                        element.</param>   
            ///<returns>PropListItem object that contain a list of names and values of properties.
            ///         <see cref="NSvn.Common.PropListItem"/> </returns>
            PropListItem* PropList(String* target, Revision* revision, bool recurse)[];

            ///<summary>List the revision properties on an entry in a repository.</summary>
            ///<param name="url">Path to the "revision" in the repository.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="setRev">Set to the actual revision number affected upon return.</param>  
            ///<returns>PropertyDictionary object that contain a list of names and values of properties.
            ///         <see cref="NSvn.Common.PropertyDictionary"/></returns>
            PropertyDictionary* RevPropList(String* url, Revision* revision, 
                [System::Runtime::InteropServices::Out]System::Int32* setRev);

            ///<summary>Export the contents of either a subversion repository or a subversion. </summary>
            ///         working copy into a directory with no svn administrative directories (.svn).</summary> 
            ///<param name="from">Path to the files/directory to be exported.</param>
            ///<param name="to">Path to the directory where you wish to create 
            ///                 the exported tree.</param>
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="force">Whether to force the export</param>   
            /// <returns>The revision affected</returns>
            int Export(String* from, String* to, Revision* revision, bool force);

            ///<summary>List the contents of an url or path.</summary>
            ///<param name="path">Path to the files/directory to be listed.</param>  
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            ///<param name="recurse">If recurse is true, then propname will be received recursively 
            ///                      on target and all children.</param> 
            ///<returns>String table of the paths to be listed.</returns>
            DirectoryEntry* List(String* path, Revision* revision, bool recurse) [];

            ///<summary>List the contents of a file.</summary>
            ///<param name="out"></param>  
            ///<param name="path">Path to the file to be edited.</param>  
            ///<param name="revision">A revision, specified in Core::Revision. <see cref="NSvn.Core.Revision"/> 
            ///                         for more information.</param>
            void Cat(Stream* out, String*path, Revision* revision);


            /// <summary>Retrieves an URL from a working copy path</summary>
            /// <param name="path">The working copy path.</param>
            String* UrlFromPath( String* path );

            /// <summary>Retrieves the UUID for a specified repository.</summary>
            /// <param name="url">The URL to the repository.</param>
            String* UuidFromUrl( String* url);

            /// <summary>Check whether a path has svn:mimetype set to a binary type.</summary>
            bool HasBinaryProp( String* path );

            /// <summary>Whether an item is ignored.</summary>
            bool IsIgnored( String* path );

        protected public:
            /// <summary>Invokes the Notification event.</summary>
            virtual void OnNotification( NotificationEventArgs* args );

            /// <summary>Invokes the LogMessage event.</summary>
            virtual void OnLogMessage( LogMessageEventArgs* args );

            /// <summary>Invokes the Cancel event.</summary>
            virtual void OnCancel( CancelEventArgs* args );
            

        private:
            NSvn::Common::PropertyDictionary* ConvertToPropertyDictionary( 
                apr_hash_t* propertyHash, String* propertyName, Pool& pool );

            NSvn::Common::PropListItem* ConvertPropListArray( 
                apr_array_header_t* propListItems, Pool& pool ) [];

            ClientContext* context;
        };
    }
}
