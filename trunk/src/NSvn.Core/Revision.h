// $Id$
#using <mscorlib.dll>
#include "svnenums.h"
#include "Pool.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class Revision
        {
        public:
            /// <summary>Creates a revision from a revision number</summary>
            static Revision* FromNumber( int revision )
            {
                Revision* rev = new Revision();
                rev->kind = svn_opt_revision_number;
                rev->revision = revision;
                return rev;
            }

            /// <summary>Creates a revision from a date</summary>
            static Revision* FromDate( DateTime date )
            {
                Revision* rev = new Revision();
                rev->kind = svn_opt_revision_date;
                rev->date = date;
                return rev;
            }

            /// <summary>Creates a special revision</summary>
            static Revision* Special( RevisionKind kind )
            {
                Revision* rev = new Revision();
                rev->kind = static_cast<svn_opt_revision_kind>(kind);
                return rev;
            }

            svn_opt_revision_t* ToSvnOptRevision( const Pool& pool )
            {
                svn_opt_revision_t* rev = static_cast<svn_opt_revision_t*>(
                    pool.PCalloc( sizeof(*rev) ) );
                rev->kind = static_cast<svn_opt_revision_kind>(this->kind);

                //what kind?
                
                switch( kind )
                {
                case RevisionKind::Date:
                    {
                        DateTime epoch( 1970, 1, 1 );
                        TimeSpan t = this->date - epoch;
                        //apr_time_t is the number of microseconds since the epoch
                        rev->value.date = static_cast<Int64>(t.TotalMilliseconds * 1000);
                        break;
                    }
                case RevisionKind::Number:
                    rev->value.number = this->revision;
                    break;
                default:
                    break;
                }

                return rev;
            }
            
        private:
            Revision()
            {;}
            DateTime date;       
            int revision;
            svn_opt_revision_kind kind;

        };

       

        
    }
}

