// $Id$
#include "StdAfx.h"
#include "aprfileadaptertest.h"
#include "StringHelper.h"
#include "AprFileAdapter.cpp"

#using <mscorlib.dll>
using namespace System::IO;
using namespace System::Text;


void NSvn::Core::Tests::MCpp::AprFileAdapterTest::TestBasic()
{
    Pool pool;
    MemoryStream* memStream = new MemoryStream();
    AprFileAdapter* adapter = new AprFileAdapter( memStream );
    apr_file_t* aprFile = adapter->Start( pool );

    String* to = S"Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est ";

    apr_file_puts( StringHelper(to), aprFile );
    apr_file_close( aprFile );

    adapter->WaitForExit();

    String* from = Encoding::Default->GetString( memStream->ToArray() );

    Assertion::AssertEquals( "Written string is not equal", to, from );


}
