// $Id$
#using <mscorlib.dll>
#include "svnenums.h"
#include "Pool.h"
#include "utils.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class Revision
        {
        public:
			/// <summary> No revision information given.</summary>    
            static Revision* const Unspecified = new Revision( svn_opt_revision_unspecified );
            /// <summary>Revision of item's last commit in working copy.</summary>        
            static Revision* const Committed = new Revision( svn_opt_revision_committed );
            /// <summary>Revision before item's last commit in working copy. 
			///						  (current committed revision in wc) - 1</summary>      
            static Revision* const Previous = new Revision( svn_opt_revision_previous );
            /// <summary>Base revision of item's working copy.</summary>        
            static Revision* const Base = new Revision( svn_opt_revision_base );
            /// <summary>Current committed revision, plus local changes in working copy.</summary>           
            static Revision* const Working = new Revision( svn_opt_revision_working );
            /// <summary>Latest revision in repository.</summary> 
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
                        rev->value.date = DateTimeToAprTime( this->date );
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

