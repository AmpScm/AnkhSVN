#include "stdafx.h"
#include "Revision.h"


/// <summary>Creates a revision from a revision number</summary>
NSvn::Core::Revision* NSvn::Core::Revision::FromNumber( int revision )
{
    Revision* rev = new Revision( svn_opt_revision_number );
    rev->revision = revision;
    return rev;
}

/// <summary>Creates a revision from a date</summary>
NSvn::Core::Revision* NSvn::Core::Revision::FromDate( DateTime date )
{
    Revision* rev = new Revision( svn_opt_revision_date);
    rev->date = date;
    return rev;
}


// convert to an svn_opt_revision_t*
// allocate in pool
svn_opt_revision_t* NSvn::Core::Revision::ToSvnOptRevision( const Pool& pool )
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

String* NSvn::Core::Revision::ToString()
{
    switch( this->kind )
    {
    case svn_opt_revision_unspecified:
        return S"Unspecified";
        break;
    case svn_opt_revision_committed:
        return S"Committed";
        break;
    case svn_opt_revision_previous:
        return S"Previous";
        break;
    case svn_opt_revision_base:
        return S"Base";
        break;
    case svn_opt_revision_working:
        return S"Working";
        break;
    case svn_opt_revision_head:
        return S"Head";
        break;
    case svn_opt_revision_date:
        return this->date.ToString();
        break;
    case svn_opt_revision_number:
        return __box( this->revision )->ToString();
        break;
    default:
        return S"Invalid revision";
    }
}

NSvn::Core::Revision* NSvn::Core::Revision::Parse( String* s )
{
    String* lc = s->ToLower();

    if ( lc->Equals( "head" ) )
        return NSvn::Core::Revision::Head;
    else if ( lc->Equals( "base" ) )
        return NSvn::Core::Revision::Base;
    else if ( lc->Equals( "committed" ) )
        return NSvn::Core::Revision::Committed;
    else if ( lc->Equals( "working" ) )
        return NSvn::Core::Revision::Working;
    else if ( lc->Equals( "unspecified" ) )
        return NSvn::Core::Revision::Unspecified;
    else if ( lc->Equals( "previous" ) )
        return NSvn::Core::Revision::Previous;

    // not one of the special ones

    // perhaps it's a number?
    double rev;
    if ( Double::TryParse( s, System::Globalization::NumberStyles::Integer,
        System::Globalization::CultureInfo::InvariantCulture, &rev ) )
        return NSvn::Core::Revision::FromNumber(static_cast<int>(rev));
    else
    { 
        // last chance - is it a datetime?
        try
        {
            DateTime t = DateTime::Parse( s );
            return NSvn::Core::Revision::FromDate( t );
        }
        catch( FormatException* ex )
        {
            throw new FormatException( "Cannot parse string to a valid revision object",
                ex );
        }
    }

}