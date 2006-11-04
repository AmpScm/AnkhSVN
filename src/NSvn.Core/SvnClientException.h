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

            SvnClientException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) 
            {
                errorCode = info->GetInt32("_errorCode");
                svnError = info->GetString("_svnError");
                line = info->GetInt32("_errorCode");
                file = info->GetString("_file");
            }

            void GetObjectData(System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context )
            {
                __super::GetObjectData(info,context);

                info->AddValue("_errorCode",get_ErrorCode());
                info->AddValue("_svnError",get_SvnError());
                info->AddValue("_line",get_Line());
                info->AddValue("_file",get_File());
            }

            /// <summary>The SVN error code associated with this exception</summary>
            __property int get_ErrorCode()
            { return this->errorCode; }

            /// <summary>The error message returned by the SVN libraries</summary>
            __property System::String* get_SvnError()
            { return this->svnError; }

            /// <summary>The line number reported by Subversion.</summary>
            __property int get_Line()
            { return this->line; }

            /// <summary>The filename reported by Subversion.</summary>
            __property System::String* get_File()
            { return this->file; }

        protected:

        private:
            static SvnClientException* CreateExceptionsRecursively( svn_error_t* err );
            int errorCode;
            System::String* svnError;
            int line;
            System::String* file;

        };

    public __gc class AuthorizationFailedException : public SvnClientException
    {
    public:
        AuthorizationFailedException( System::Exception* innerException ) :
          SvnClientException( "Authorization failed", innerException )
          {;}
          AuthorizationFailedException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}

    };

    public __gc class RepositoryHookFailedException : public SvnClientException
    {
    public:
        RepositoryHookFailedException(System::String *message, System::Exception *innerException):
          SvnClientException( message, innerException)
          {;}
    protected:
        RepositoryHookFailedException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
             SvnClientException( info, context )
             {;}

    };

    public __gc class WorkingCopyLockedException : public SvnClientException
    {
    public:
        WorkingCopyLockedException( System::Exception* innerException ) :
          SvnClientException( "Working copy locked", innerException )
          {;}
        WorkingCopyLockedException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}
    };

    public __gc class NotVersionControlledException : public SvnClientException
    {
    public:
        NotVersionControlledException( System::Exception* innerException ) :
          SvnClientException( "Path is not version controlled", innerException )
          {;}
        NotVersionControlledException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}
    };

     public __gc class ResourceOutOfDateException : public SvnClientException
    {
    public:
        ResourceOutOfDateException( System::Exception* innerException ) :
          SvnClientException( "The resource is out of date relative to the repository. Run update.", innerException )
          {;}
        ResourceOutOfDateException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}
    };

    public __gc class IllegalTargetException : public SvnClientException 
    {
    public:
        IllegalTargetException( System::Exception* innerException ) :
            SvnClientException( "The item is not a valid target for this operation", innerException )
            {;}
        IllegalTargetException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}
    };

    public __gc class OperationCancelledException : public SvnClientException 
    {
    public:
        OperationCancelledException( System::Exception* innerException ) :
            SvnClientException( "The operation was cancelled.", innerException )
            {;}
        OperationCancelledException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {;}
    };

    public __gc class InvalidUrlException : public SvnClientException 
    {
    public:
        InvalidUrlException( System::String* message, System::String* url ) : SvnClientException( message ), url(url)
            {;}
        InvalidUrlException( System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context ) :
            SvnClientException( info, context )
            {
                this->url = info->GetString( "_url" );
            }

            void GetObjectData(System::Runtime::Serialization::SerializationInfo* info, System::Runtime::Serialization::StreamingContext context )
            {
                __super::GetObjectData(info,context);
                info->AddValue("_url", this->url);
            }


        __property System::String* get_Url()
        { return this->url; }

    private:
        System::String* url;
    };

#define HandleError( func ) { svn_error_t* err__ = func; \
        if ( (err__) != 0 ) throw NSvn::Core::SvnClientException::FromSvnError( (err__) ); }
    }
}
