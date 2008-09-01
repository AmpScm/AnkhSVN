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
    Assert::AreEqual( 1000, len, "Not enough bytes written" );

    Byte memStreamBuf[] = memStream->ToArray();
    Assert::AreEqual( 1000, memStreamBuf->Length, "Returned buffer wrong size" );
    for( int i = 0; i < 1000; i++ )
        Assert::AreEqual( buf[i], memStreamBuf[i], "Byte not equal: " + i );

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

    Assert::AreEqual( 1000, len, "Returned buffer wrong size" );

    Byte memStreamBuf[] = memStream->ToArray();
    for( int i = 0; i < 1000; i++ )
        Assert::AreEqual( buf[i], memStreamBuf[i], "Byte not equal" );


}

void NSvn::Core::Tests::MCpp::StreamTest::TestReadFromNullStream()
{
    Pool pool;
    svn_stream_t* stream = CreateSvnStream( Stream::Null, pool );
    char buf[1000];
    apr_size_t len = 1000;
    HandleError( svn_stream_read( stream, buf, &len ) );

    Assert::AreEqual( 0, len, "Should not be able to read from a null stream" );
}

void NSvn::Core::Tests::MCpp::StreamTest::TestWriteNullBuffer()
{
    Pool pool;
    svn_stream_t* stream = CreateSvnStream( Stream::Null, pool );

    char* buf = 0;
    apr_size_t len = 1;
    HandleError( svn_stream_write( stream, buf, &len ) );

    // won't get here if the test fails.

}
