#include "AuthenticationBaton.h"
#include "AdminAccessBaton.h"
#include "Revision.h"
#include "CommitInfo.h"
#include "PropListItem.h"
#include "delegates.h"

#using <mscorlib.dll>
#using <System.dll>

using namespace System;
using namespace System::IO;
using namespace System::Collections::Specialized;

namespace NSvn
{
    namespace Core
    {
        public __gc class Client
        {
        public:
            
            static void Checkout( NotifyCallback* callback, AuthenticationBaton* authBaton, String* url, String* path, 
                Revision* revision, bool recurse );

            static void Update( AuthenticationBaton* authBaton, String* path, Revision* revision, bool recurse,
                NotifyCallback* callback );

            static void Switch( AuthenticationBaton* authBaton, String* path, String* url, Revision* revision,
                bool recurse, NotifyCallback* notifyCallback );

            static void Add( String* path, bool recursive, NotifyCallback* notifyCallback );

            static CommitInfo* MakeDir( String* path, AuthenticationBaton* authBaton, 
                LogMessageCallback* logMessageCallback, NotifyCallback* notifyCallback );

            static CommitInfo* Delete( String* path, AdminAccessBaton* adminAccessBaton, bool force,
                AuthenticationBaton* authBaton, LogMessageCallback* logMessageCallback, 
                NotifyCallback* callback );

            static CommitInfo* Import( NotifyCallback* notifyCallback, AuthenticationBaton* authBaton, 
                String* path, String* url, String* newEntry, LogMessageCallback* logMessageCallback,
                bool nonRecursive );

            static CommitInfo* Commit( NotifyCallback* notifyCallback, AuthenticationBaton* authBaton,
                String* targets[], LogMessageCallback* logMessageCallback, bool nonRecursive );

            static StringDictionary* Status( Revision* youngest, AuthenticationBaton* authBaton, 
                bool descend, bool getAll, bool update, bool noIgnore, NotifyCallback* notifyCallback );

            static void Log(AuthenticationBaton* authBaton, String* targets[], Revision* start, 
                Revision* end, bool discoverChangePath, bool strictNodeHistory, 
                LogMessageReceiver* receiver);

            static void Diff( String* diffOptions[], AuthenticationBaton* authBaton, String* path1, 
                Revision* revision1, String* path2, Revision* revision2, bool recurse, 
                bool noDiffDeleted, Stream* outfile, Stream* errFile);

            static void Merge( NotifyCallback* notifyCallback, AuthenticationBaton* authBaton, String* url1, 
                Revision* revision1, String* url2, Revision* revision2, String* targetWcPath, 
                bool recurse, bool force, bool dryRun);

            static void Cleanup( String* dir);

            static void Revert( String* path, bool recursive, NotifyCallback* notifyCallback );

            static void Resolve( String* path, NotifyCallback* notifyCallback, bool recursive );

            static CommitInfo* Copy(String* srcPath, Revision* srcRevision, String* dst, 
                AdminAccessBaton* admAccess, AuthenticationBaton* authBatton, 
                LogMessageCallback* logMessageCallback, NotifyCallback* notifyCallback);

            static CommitInfo* Move( String* srcPath, Revision* srcRevision, String* dstPath, 
                bool force, AuthenticationBaton* authBaton, LogMessageCallback* logMessageCallback, 
                NotifyCallback* notifyCallback);

            static void PropSet(String* propName, Byte propval[], String* target, bool recurse);

            static void RevPropSet(String* propName, Byte propval[], String* url, Revision* revision, 
                AuthenticationBaton* authBaton, RevisionNumber* setRev);

            static StringDictionary* PropGet( String* propName, String* target, Revision* revision, 
                AuthenticationBaton* authBaton, bool recurse);

            static Byte RevPropGet( String* propName, String* url, Revision* revision, 
                AuthenticationBaton* authBaton, RevisionNumber* setRev) [];

            static PropListItem* PropList( String* target, Revision* revision, AuthenticationBaton* authBaton, 
                bool recurse) [];

            static StringDictionary* RevPropList( String* url, Revision* revision, AuthenticationBaton* authBaton, 
                RevisionNumber* setRev);

            static void Export( String* url, String* to, Revision* revision, 
                AuthenticationBaton* authBaton, NotifyCallback* notifyCallback);

            static String* Ls( String* url, Revision* revision, AuthenticationBaton* authBaton, bool recurse) [];

            static void Cat( Stream* out, String* url, Revision* revision, AuthenticationBaton* authBaton);

        private:
            // ctor made private to avoid instantiation of this class
            Client();
        };
    }
}