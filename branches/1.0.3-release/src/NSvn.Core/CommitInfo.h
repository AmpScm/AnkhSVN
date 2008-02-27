// $Id$
#using <mscorlib.dll>
#include <svn_client.h>

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class CommitInfo
        {
        private public:
            CommitInfo( svn_client_commit_info_t* info, apr_pool_t* pool ) 
            {
                this->author = Utf8ToString( info->author, pool );
                try
                {
                    if ( info->date )
                    {
                        this->date = DateTime::ParseExact( Utf8ToString(info->date, pool),  
                            "yyyy-MM-dd\\THH:mm:ss.ffffff\\Z", 
                            System::Globalization::CultureInfo::InvariantCulture );
                    }
                    else
                        this->date = DateTime::MinValue;
                }
                catch( System::FormatException* )
                {
                    this->date = DateTime::MinValue;
                }
                this->revision = info->revision;
            }

        public:
            ///<summary>Server side date of the commit</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property DateTime get_Date()
            { return this->date; }

            ///<summary>Author of the commit</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property String* get_Author()
            { return this->author; }

            ///<summary>The just committed revision</summary>
            [System::Diagnostics::DebuggerStepThrough]
            __property int get_Revision()
            { return revision; }

            [System::Diagnostics::DebuggerStepThrough]
            __property static CommitInfo* get_Invalid()
            { return CommitInfo::invalid; }

        private:
            CommitInfo() : date( 0 ), author( "" ), revision( 0 )
            {}

            static CommitInfo* invalid = new CommitInfo();

            DateTime date;
            String* author;
            int revision;

        };
    }
}
