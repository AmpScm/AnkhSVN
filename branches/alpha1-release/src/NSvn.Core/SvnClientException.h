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


        protected:
            
        private:
            static SvnClientException* CreateExceptionsRecursively( svn_error_t* err );

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

        inline void HandleError( svn_error_t* err )
        {
            if ( err != 0 )
                throw NSvn::Core::SvnClientException::FromSvnError( err );
        }
    }
}
