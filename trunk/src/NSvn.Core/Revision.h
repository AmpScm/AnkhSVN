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
            //TODO: doc comments
            static Revision* const Unspecified = new Revision( svn_opt_revision_unspecified );
            static Revision* const Committed = new Revision( svn_opt_revision_committed );
            static Revision* const Previous = new Revision( svn_opt_revision_previous );
            static Revision* const Base = new Revision( svn_opt_revision_base );
            static Revision* const Working = new Revision( svn_opt_revision_working );
            static Revision* const Head = new Revision( svn_opt_revision_head );

            /// <summary>Creates a revision from a revision number</summary>
            static Revision* FromNumber( int revision )
            {
                Revision* rev = new Revision( svn_opt_revision_number );
                rev->revision = revision;
                return rev;
            }

            /// <summary>Creates a revision from a date</summary>
            static Revision* FromDate( DateTime date )
            {
                Revision* rev = new Revision( svn_opt_revision_date);
                rev->date = date;
                return rev;
            }

            
            // convert to an svn_opt_revision_t*
            // allocate in pool
            svn_opt_revision_t* ToSvnOptRevision( const Pool& pool )
            {
                svn_opt_revision_t* rev = static_cast<svn_opt_revision_t*>(
                    pool.PCalloc( sizeof(*rev) ) );
                rev->kind = this->kind;

                //what kind?                
                switch( this->kind )
                {
                case svn_opt_revision_date:
                    {
                        DateTime epoch( 1970, 1, 1 );
                        TimeSpan t = this->date - epoch;
                        //apr_time_t is the number of microseconds since the epoch
                        rev->value.date = static_cast<Int64>(t.TotalMilliseconds * 1000);
                        break;
                    }
                case svn_opt_revision_number:
                    rev->value.number = this->revision;
                    break;
                default:
                    break;
                }

                return rev;
            }
            
        private:
            Revision( svn_opt_revision_kind kind ) : kind( kind )
            {;}
            DateTime date;       
            int revision;
            svn_opt_revision_kind kind;

        };
    }
}

