// $Id$
#include "StdAfx.h"
#include "MiscTests.h"
#include "Pool.h"
#include "StringHelper.h"
#include "ParameterDictionary.h"
#include "Revision.h"
#include <svn_auth.h>


void NSvn::Core::Tests::MCpp::MiscTests::TestRevisionFromNumber()
{
    Pool pool;
    Revision* rev = Revision::FromNumber( 42 );

    svn_opt_revision_t* svnRev = rev->ToSvnOptRevision( pool );

    Assertion::AssertEquals( "Wrong revision number", 42, svnRev->value.number );
    Assertion::AssertEquals( "Wrong revision kind", svn_opt_revision_number, svnRev->kind );
}

void NSvn::Core::Tests::MCpp::MiscTests::TestRevisionFromDate()
{
    Pool pool;
    DateTime dt = DateTime::Now;

    Revision* rev = Revision::FromDate( dt );

    svn_opt_revision_t* svnRev = rev->ToSvnOptRevision( pool );

    // Create a date string, so we can compare
    char dateString[50];
    apr_time_exp_t expTime;
    apr_time_exp_gmt( &expTime, svnRev->value.date );
    apr_size_t length;
    apr_strftime( dateString, &length, 49, "%H:%M:%S %d.%m.%Y", &expTime );

    String* nowString = dt.ToString( "HH':'mm':'ss dd'.'MM'.'yyyy" );


    Assertion::AssertEquals( "Wrong revision kind", svn_opt_revision_date, svnRev->kind );
    Assertion::AssertEquals( "Wrong date", nowString, StringHelper( dateString ) );

}

void NSvn::Core::Tests::MCpp::MiscTests::TestParameterDictionary()
{
    Pool pool;
    apr_hash_t* hash = apr_hash_make( pool );
    ParameterDictionary* dict1 = new ParameterDictionary( hash, pool );
    dict1->Item[S"Hello"] = S"World";

    Assertion::AssertEquals( "Item not working", S"World", 
        dict1->Item[S"Hello"] );
    Assertion::Assert( "Contains not working", dict1->Contains( S"Hello" ) );

    ParameterDictionary* dict2 = new ParameterDictionary( hash, pool );
    Assertion::AssertEquals( "Item not working in new dict", S"World", 
        dict2->Item[S"Hello"] );

    dict2->Remove( S"Hello" );

    ParameterDictionary* dict3 = new ParameterDictionary( hash, pool );
    Assertion::Assert( "Remove not working", !dict3->Contains( S"Hello" ) );
}