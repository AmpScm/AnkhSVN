// $Id$
#pragma once
#include "stdafx.h"
#include <svn_client.h>

namespace NSvn
{
    namespace Core
    {

        /// <summary>Base class for all exceptions thrown by NSvn.Core</summary>
    public __gc class SvnClientException :
        public NSvn::Common::SvnException
        {
        public:
            /// <summary>Create an exception from an svn_error_t*</summary>
            static SvnClientException* FromSvnError( svn_error_t* error );

            SvnClientException()
            {;}
            SvnClientException( System::String* message ) : NSvn::Common::SvnException( message )
            {;}
            SvnClientException( System::String* message, System::Exception* innerException ) : 
            NSvn::Common::SvnException( message, innerException )
            {;}

            /// <summary>The SVN error code associated with this exception</summary>
            __property int get_ErrorCode()
            { return this->errorCode; }


        protected:

        private:
            static SvnClientException* CreateExceptionsRecursively( svn_error_t* err );
            int errorCode;

        };

    public __gc class AuthorizationFailedException : public SvnClientException
    {
    public:
        AuthorizationFailedException( System::Exception* innerException ) :
          SvnClientException( "Authorization failed", innerException )
          {;}
    };



    public __gc class WorkingCopyLockedException : public SvnClientException
    {
    public:
        WorkingCopyLockedException( System::Exception* innerException ) :
          SvnClientException( "Working copy locked", innerException )
          {;}
    };

    public __gc class NotVersionControlledException : public SvnClientException
    {
    public:
        NotVersionControlledException( System::Exception* innerException ) :
          SvnClientException( "Path is not version controlled", innerException )
          {;}
    };

     public __gc class ResourceOutOfDateException : public SvnClientException
    {
    public:
        ResourceOutOfDateException( System::Exception* innerException ) :
          SvnClientException( "The resource is out of date relative to the repository. Run update.", innerException )
          {;}
    };

    public __gc class IllegalTargetException : public SvnClientException 
    {
    public:
        IllegalTargetException( System::Exception* innerException ) :
            SvnClientException( "The item is not a valid target for this operation", innerException )
            {;}
    };

    inline void HandleError( svn_error_t* err )
    {
        if ( err != 0 )
            throw NSvn::Core::SvnClientException::FromSvnError( err );
    }
    }
}
