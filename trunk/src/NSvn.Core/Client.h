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
            static void Checkout(String* url, String* path, Revision* revision, bool recurse, 
                ClientContext* context);

	        static void Update(String* path, Revision* revision, bool recurse, ClientContext* context);

	        /*static void Switch(String* path, String* url, Revision* revision, bool recurse, 
                ClientContext* context);*/

	        static void Add(String* path, bool recursive, ClientContext* context);
            
            ///<summary>Make a dir</summary>
            ///<param name="path">Path to the directory</param>
            ///<return>A commit
	        static CommitInfo* MakeDir(String* path, ClientContext* context);
/*
	        static CommitInfo* Delete(String* path, AdminAccessBaton* admAccessBaton, bool force, 
                ClientContext* context);

	        static CommitInfo* Import(String* path, String* url, String* newEntry, bool nonRecursive, 
                ClientContext* context);
*/
	        static CommitInfo* Commit(String __gc* targets[], bool nonRecursive, ClientContext* context);

            //TO-DO:  Core::Revision to be implemented 
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

	        */static void Cleanup(String* dir);

	        static void Revert(String* path, bool recursive, ClientContext* context);
	        static void Resolve(String* path, bool recursive, ClientContext* context);
            /*


	        static CommitInfo* Copy(String* srcPath, Revision* srcRevision, String* dst, 
                ClientContext* context);

	        static CommitInfo* Move(String* srcPath, Revision* srcRevision, String* dstPath, 
                bool force, ClientContext* context);
*/
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
