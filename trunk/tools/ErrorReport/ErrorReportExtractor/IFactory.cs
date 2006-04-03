using System;
namespace ErrorReportExtractor
{
    public interface IFactory
    {
        IReportContainer GetReportContainer( IProgressCallback cb );
        IStorage GetStorage( IProgressCallback cb );
        IMailer GetMailer( IProgressCallback cb );
        ITemplateManager GetTemplateManager( IProgressCallback cb );
    }
}
