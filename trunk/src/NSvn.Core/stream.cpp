// $Id$
#include "stdafx.h"

#include "stream.h"
#include "ManagedPointer.h"


using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace NSvn::Core;
using namespace System;

namespace
{
    svn_error_t* svn_read_fn(void* baton, char* buffer, apr_size_t* len) 
    {
        try
        {
            Stream* stream = *(static_cast<ManagedPointer<Stream*>* >(baton));
            Byte managedBuffer __gc[] = new Byte[ *len ];
            int bytesRead;
            int totalBytesRead = 0;
            int remainingBytes = *len;
            while( (bytesRead = stream->Read( managedBuffer, totalBytesRead, remainingBytes )) > 0 )
            {
                totalBytesRead += bytesRead;
                remainingBytes -= bytesRead;
            }

            *len = totalBytesRead;
            
            Marshal::Copy( managedBuffer, 0,  buffer, *len );

            return SVN_NO_ERROR;
        }
        catch( IOException* )
        {
            return svn_error_create( SVN_ERR_STREAM_UNEXPECTED_EOF, 0, "IOException thrown" );
        }
    }

    svn_error_t* svn_write_fn(void* baton, const char* buffer, apr_size_t* len)
    {
        try
        {
            Stream* stream = *(static_cast<ManagedPointer<Stream*>* >(baton));
            Byte managedBuffer __gc[] = new Byte[ *len ];
            Marshal::Copy( const_cast<char*>(buffer), managedBuffer, 0, *len );

            stream->Write( managedBuffer, 0, *len );

            return SVN_NO_ERROR;
        }
        catch( IOException* )
        {
            return svn_error_create( SVN_ERR_STREAM_UNEXPECTED_EOF, 0, "IOException thrown" );
        }
    }

    svn_error_t* svn_close_fn(void* baton)
    {
        Stream* stream = *(static_cast<ManagedPointer<Stream*>* >(baton));
        stream->Close();

        return SVN_NO_ERROR;
    }

}

// necessary in order to get the MCpp compiler to generate metadata for the type
struct svn_stream_t
{};

svn_stream_t* NSvn::Core::CreateSvnStream( Stream* stream, Pool& pool )
{
    void* baton = pool.AllocateObject( ManagedPointer<Stream*>( stream ) );

    svn_stream_t* svnStream = svn_stream_create( baton, pool );
    svn_stream_set_read( svnStream, svn_read_fn );
    svn_stream_set_write( svnStream, svn_write_fn );
    svn_stream_set_close( svnStream, svn_close_fn );

    return svnStream;

}

