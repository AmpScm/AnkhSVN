#include "StdAfx.h"
#include "aprfileadapter.h"

using namespace System;
using namespace System::Threading;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

// necessary to force the compiler to emit metadata for the type
struct apr_file_t
{};

apr_file_t* NSvn::Core::AprFileAdapter::Start( Pool& pool )
{
    apr_file_t* infile;
    apr_file_t* outfile;
    apr_status_t err = apr_file_pipe_create( &outfile, &infile, pool );
    if ( err != APR_SUCCESS )
        throw new ApplicationException( S"Could not create pipe" );   

    this->outfile = outfile;

    this->thread = new Thread( new ThreadStart(this, &AprFileAdapter::Read) );
    this->thread->Start();

    return infile;
}
#include "StringHelper.h"
void NSvn::Core::AprFileAdapter::Read()
{
    char unmanagedBuffer[BUFSIZE];
    Byte managedBuffer[] = new Byte[ BUFSIZE ];

    apr_size_t noBytes = BUFSIZE;
    apr_status_t status;
    while( (status = apr_file_read( this->outfile, reinterpret_cast<void*>(unmanagedBuffer), 
        &noBytes )) != APR_EOF )
    {
        if( status != APR_SUCCESS )
        {
            String* str = StringHelper(apr_strerror(status, unmanagedBuffer, BUFSIZE) );
            this->errorMessage = str;
            throw new IOException( str );
        }
        Marshal::Copy( unmanagedBuffer, managedBuffer, 0, noBytes );
        this->stream->Write( managedBuffer, 0, noBytes );

        noBytes = BUFSIZE;
    }
}

void NSvn::Core::AprFileAdapter::WaitForExit()
{
    this->thread->Join();
    if ( this->errorMessage != 0 )
    {
        String* msg = this->errorMessage;
        this->errorMessage = 0;
        throw new ApplicationException( msg );
    }
}


