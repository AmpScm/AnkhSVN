#include "StdAfx.h"
#include "MiscTests.h"
#include "SimpleCredential.h"
#include "UsernameCredential.h"
#include "Pool.h"
#include <svn_auth.h>

void NSvn::Core::Tests::MCpp::MiscTests::TestSimpleCredential()
{
    Pool pool;

    SimpleCredential* scred = new SimpleCredential( S"Moo", S"Foo" );
    svn_auth_cred_simple_t* cred = static_cast<svn_auth_cred_simple_t*>(
        scred->GetCredential( static_cast<apr_pool_t*>(pool) ).ToPointer() );
    Assertion::AssertEquals( StringHelper(cred->username), S"Moo" );
    Assertion::AssertEquals( StringHelper(cred->password), S"Foo" );
}

void NSvn::Core::Tests::MCpp::MiscTests::TestUsernameCredential()
{
    Pool pool;

    UsernameCredential* ucred = new UsernameCredential( "Foo" );
    svn_auth_cred_username_t* cred = static_cast<svn_auth_cred_username_t*>(
        ucred->GetCredential( static_cast<apr_pool_t*>(pool) ).ToPointer() );

    Assertion::AssertEquals( StringHelper( cred->username ), S"Foo" );
}