// $Id$

#include "AuthenticationBatonTest.h"
#include "AuthenticationBaton.h"
#include "SvnClientException.h"

void NSvn::Core::Tests::MCpp::AuthenticationBatonTest::TestSingleProvider()
{
    Pool pool;

    AuthenticationBaton* baton = new AuthenticationBaton();
    baton->Add( AuthenticationProvider::GetSimplePromptProvider(
        new SimplePromptDelegate( this, &AuthenticationBatonTest::SimplePrompt ), 1 ) );

    svn_auth_cred_simple_t* creds;
    svn_auth_iterstate_t* iterstate;
    apr_hash_t* params = apr_hash_make( pool );

    HandleError( svn_auth_first_credentials( ((void**)&creds), &iterstate, SVN_AUTH_CRED_SIMPLE, 
        "Realm", baton->GetAuthBaton(), pool ) );

    Assertion::AssertEquals( "Username not correct", S"Arild", Utf8ToString( creds->username , pool ) );
    Assertion::AssertEquals( "Password not correct", S"Fines", Utf8ToString( creds->password , pool ) );

}

void NSvn::Core::Tests::MCpp::AuthenticationBatonTest::TestParams()
{
    AuthenticationBaton* baton = new AuthenticationBaton();

    baton->SetParameter( "Foo", "Bar" );
    
    Assertion::AssertEquals( "Param not same", S"Bar", baton->GetParameter( "Foo" ) );
}

void NSvn::Core::Tests::MCpp::AuthenticationBatonTest::TestDefaultUsernameAndPassword()
{

    AuthenticationBaton* baton = new AuthenticationBaton();

    baton->Add( AuthenticationProvider::GetSimplePromptProvider(
        new SimplePromptDelegate( this, &AuthenticationBatonTest::SimplePrompt ), 1 ) );

    
    baton->SetParameter( AuthenticationBaton::ParamDefaultUsername, S"Humpty" );
    baton->SetParameter( AuthenticationBaton::ParamDefaultPassword, S"Dumpty" );

    Pool pool;

    svn_auth_cred_simple_t* creds;
    svn_auth_iterstate_t* iterstate;
    apr_hash_t* params = apr_hash_make( pool );

    HandleError( svn_auth_first_credentials( ((void**)&creds), &iterstate, SVN_AUTH_CRED_SIMPLE, 
        "Realm", baton->GetAuthBaton(), pool ) );

    Assertion::AssertEquals( "Username not correct", S"Humpty", Utf8ToString( creds->username, pool ) );
    Assertion::AssertEquals( "Password not correct", S"Dumpty", Utf8ToString( creds->password, pool ) );

}

NSvn::Core::SimpleCredential* NSvn::Core::Tests::MCpp::AuthenticationBatonTest::SimplePrompt( 
    String* realm, String* username, bool maySave )
{
    Assertion::AssertEquals( "Realm string is wrong", S"Realm", realm );
    Assertion::AssertEquals( "Username is wrong", Environment::UserName, username );

    return new SimpleCredential( S"Arild", S"Fines", false );
}
