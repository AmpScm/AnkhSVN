// $Id$
#using <mscorlib.dll>
#include "StringHelper.h"
#include <svn_client.h>

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class CommitInfo
        {
        public:
            CommitInfo( svn_client_commit_info_t* info ) 
            {
                this->author = StringHelper( info->author );
                this->date = DateTime::ParseExact( StringHelper(info->date),  
                    "yyyy-MM-dd\\THH:mm:ss.ffffff\\Z", 
                    System::Globalization::CultureInfo::CurrentCulture );
                this->revision = info->revision;
            }

            ///<summary>Server side date of the commit</summary>
            __property DateTime get_Date()
            { return this->date; }

            ///<summary>Author of the commit</summary>
            __property String* get_Author()
            { return this->author; }

            ///<summary>The just committed revision</summary>
            __property int get_Revision()
            { return revision; }

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
