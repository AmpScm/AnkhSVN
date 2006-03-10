// $Id$
#include "StdAfx.h"
#include "aprfileadaptertest.h"
#include "AprFileAdapter.h"
#include "utils.h"

#using <mscorlib.dll>
using namespace System::IO;
using namespace System::Text;
using namespace System;


void NSvn::Core::Tests::MCpp::AprFileAdapterTest::TestBasic()
{
    Pool pool;
    MemoryStream* memStream = new MemoryStream();
    AprFileAdapter* adapter = new AprFileAdapter( memStream, pool );
    apr_file_t* aprFile = adapter->Start();

    String* to = S"Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est "
        S"Ipso factum dei ipsum est Ipso factum dei ipsum est Ipso factum dei ipsum est ";

    apr_file_puts( StringToUtf8(to, pool), aprFile );
    apr_file_close( aprFile );

    adapter->WaitForExit();

    String* from = Encoding::Default->GetString( memStream->ToArray() );

    Assert::AreEqual( to, from, "Written string is not equal" );


}
