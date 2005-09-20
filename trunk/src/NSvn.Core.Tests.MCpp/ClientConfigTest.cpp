// $Id$

#include <apr_hash.h>
#include <svn_config.h>
#include "ClientConfigTest.h"
#include "../NSvn.Core/ClientConfig.h"
#include "../NSvn.Core/utils.h"

struct svn_config_t
{};

void NSvn::Core::Tests::MCpp::ClientConfigTest::TestBasic()
{
    ClientConfig* config = new ClientConfig();
    
    config->Set( S"FooSection", S"BarOption", S"Baz" );

    Assertion::AssertEquals( "Not the same", S"Baz", config->Get( S"FooSection", S"BarOption", "" ) );
}


void NSvn::Core::Tests::MCpp::ClientConfigTest::TestGetHash()
{
    ClientConfig* config = new ClientConfig();

    config->Set( S"FooSection", S"BarOption", S"Baz" );

    apr_hash_t* hash = config->GetHash();

    svn_config_t* cfg = static_cast<svn_config_t*>( 
                      apr_hash_get( hash, SVN_CONFIG_CATEGORY_CONFIG,
                      APR_HASH_KEY_STRING ) ); 

    const char* val;
    svn_config_get( cfg, &val, "FooSection", "BarOption", "" );

    Pool pool;

    Assertion::AssertEquals( S"Not the same", S"Baz", Utf8ToString(val, pool) );
}



