// $Id$
#include "stdafx.h"
#include "stream.h"
#include <svn_io.h>
#include "SvnClientException.h"
using namespace System::IO;
using namespace System;
#include "streamtest.h"

void NSvn::Core::Tests::MCpp::StreamTest::TestWrite()
{
    unsigned char* buf = new unsigned char[ 1000 ];
    Random* rnd = new Random();
    for( int i = 0; i < 1000; i++ )
        buf[ i ] = rnd->Next();

    MemoryStream* memStream = new MemoryStream();

    Pool pool;
    svn_stream_t* svnStream = CreateSvnStream( memStream, pool );

    apr_size_t len = 1000;
    HandleError( svn_stream_write( svnStream, reinterpret_cast<char*>(buf), &len ) );
    Assertion::AssertEquals( "Not enough bytes written", 1000, len );

    Byte memStreamBuf[] = memStream->ToArray();
    Assertion::AssertEquals( "Returned buffer wrong size", 1000, memStreamBuf->Length );
    for( int i = 0; i < 1000; i++ )
        Assertion::AssertEquals( "Byte not equal: " + i, buf[i], memStreamBuf[i] );

    delete [] buf;
}

void NSvn::Core::Tests::MCpp::StreamTest::TestRead()
{
    Random* rnd = new Random();
    MemoryStream* memStream = new MemoryStream();
    for( int i = 0; i < 1000; i++ )
        memStream->WriteByte( static_cast<unsigned char>(rnd->Next()) );

    memStream->Seek( 0, SeekOrigin::Begin );


    Pool pool;
    svn_stream_t* svnStream = CreateSvnStream( memStream, pool );

    apr_size_t len = 1000;
    unsigned char buf[ 1000 ];
    HandleError( svn_stream_read( svnStream, reinterpret_cast<char*>(buf), &len ) );

    Assertion::AssertEquals( "Returned buffer wrong size", 1000, len );

    Byte memStreamBuf[] = memStream->ToArray();
    for( int i = 0; i < 1000; i++ )
        Assertion::AssertEquals( "Byte not equal", buf[i], memStreamBuf[i] );


}

void NSvn::Core::Tests::MCpp::StreamTest::TestReadFromNullStream()
{
    Pool pool;
    svn_stream_t* stream = CreateSvnStream( Stream::Null, pool );
    char buf[1000];
    apr_size_t len = 1000;
    HandleError( svn_stream_read( stream, buf, &len ) );

    Assertion::AssertEquals( "Should not be able to read from a null stream", 0, len );
}
