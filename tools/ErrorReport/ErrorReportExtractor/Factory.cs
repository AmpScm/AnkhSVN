using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ErrorReportExtractor
{
    public class Factory : ErrorReportExtractor.IFactory
    {
        public IReportContainer GetReportContainer(IProgressCallback cb)
        {
            return new MailContainer( ConfigurationManager.AppSettings[ "FolderPath" ], cb );
        }

        public IStorage GetStorage( IProgressCallback cb )
        {
            return new Storage( cb );
        }

        public IMailer GetMailer( IProgressCallback cb )
        {
            return new Mailer( cb );
        }

        public ITemplateManager GetTemplateManager( IProgressCallback cb )
        {
            return new TemplateManager( cb );
        }
    }
}
