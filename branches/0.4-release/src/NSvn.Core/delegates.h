// $Id$
#pragma once
#using <mscorlib.dll>

#include "credentials.h"


namespace NSvn
{
    namespace Core
    {
        public __gc class Notification;
        public __gc class CommitItem;
        public __gc class LogMessage;
        public __gc class Status;
        public __gc class SslServerCertificateInfo;
        public __gc class SslServerTrustCredential;
        public __gc class SslClientCertificateCredential;
        public __value enum SslFailures;
        

        public __delegate void NotifyCallback( Notification* notification );
        public __delegate System::String* LogMessageCallback( CommitItem* items[] );
        public __delegate void LogMessageReceiver( LogMessage* logMessage );
        public __delegate void PromptCallback();

        public __delegate SimpleCredential* SimplePromptDelegate( 
            System::String* realm, String* username, bool maySave );

        public __delegate SslServerTrustCredential* SslServerTrustPromptDelegate(
            System::String* realm, SslFailures failures, SslServerCertificateInfo* info, 
            bool maySave );

        public __delegate SslClientCertificateCredential* SslClientCertPromptDelegate( 
            String* realm, bool maySave );

        public __delegate SslClientCertificatePasswordCredential* 
            SslClientCertPasswordPromptDelegate( String* realm, bool maySave );

        public __delegate void StatusCallback( System::String* path, Status* status );

        public __delegate CancelOperation CancelCallback();
    }
}
