// $Id$
#include "StdAfx.h"
#include "MiscTests.h"
#include "Pool.h"
#include "ParameterDictionary.h"
#include "Revision.h"
#include <svn_auth.h>
#include <svn_utf.h>
#include "SvnClientException.h"


void NSvn::Core::Tests::MCpp::MiscTests::TestRevisionFromNumber()
{
    Pool pool;
    Revision* rev = Revision::FromNumber( 42 );

    svn_opt_revision_t* svnRev = rev->ToSvnOptRevision( pool );

    Assert::AreEqual( 42, svnRev->value.number, "Wrong revision number" );
    Assert::AreEqual( svn_opt_revision_number, svnRev->kind, "Wrong revision kind" );
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


    Assert::AreEqual( svn_opt_revision_date, svnRev->kind, "Wrong revision kind" );
    Assert::AreEqual( nowString, Utf8ToString( dateString, pool ), "Wrong date" );

}

void NSvn::Core::Tests::MCpp::MiscTests::TestParameterDictionary()
{
    Pool pool;
    apr_hash_t* hash = apr_hash_make( pool );
    ParameterDictionary* dict1 = new ParameterDictionary( hash, pool );
    dict1->Item[S"Hello"] = S"World";

    Assert::AreEqual( S"World", dict1->Item[S"Hello"],
        "Item not working" );
    Assert::IsTrue( dict1->Contains( S"Hello" ), "Contains not working" );

    ParameterDictionary* dict2 = new ParameterDictionary( hash, pool );
    Assert::AreEqual( S"World", dict2->Item[S"Hello"],
        "Item not working in new dict" );

    dict2->Remove( S"Hello" );

    ParameterDictionary* dict3 = new ParameterDictionary( hash, pool );
    Assert::IsTrue( !dict3->Contains( S"Hello" ), "Remove not working" );
}

void NSvn::Core::Tests::MCpp::MiscTests::TestStringToUtf8()
{
    String* s = S"Æ e i a æ å, sjø";
    Pool pool;
    const char* utf8 = StringToUtf8( s, pool );
    const char* ansi;
    HandleError( svn_utf_cstring_from_utf8( &ansi, utf8, pool ) );
    Assert::IsTrue( strcmp( "Æ e i a æ å, sjø", ansi ) == 0 );
}

void NSvn::Core::Tests::MCpp::MiscTests::TestUtf8ToString()
{
    const char* ansi = "Æ e i a æ å, sjø";
    const char* utf8;
    Pool pool;
    HandleError( svn_utf_cstring_to_utf8( &utf8, ansi, pool ) );
    String* s = Utf8ToString( utf8, pool );
    Assert::AreEqual( S"Æ e i a æ å, sjø", s );
}

void NSvn::Core::Tests::MCpp::MiscTests::TestUrlEscaping()
{
    String* s = S"http://server.com/file with spaces.txt";

    Pool pool;
    const char* escaped = CanonicalizePath(s, pool);
    Assert::IsTrue(strcmp("http://server.com/file%20with%20spaces.txt", escaped) == 0);
}

void NSvn::Core::Tests::MCpp::MiscTests::TestUrlEscapingThrowsForBackpathPresent()
{
    String* s = S"http://server.com/file/../blah.txt";

    Pool pool;
    try
    {
        CanonicalizePath(s, pool);
        Assert::Fail("Should have thrown an exception");
    }
    catch( InvalidUrlException* )
    {
        // ignore
    }
}


