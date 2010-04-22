using System;
namespace ErrorReportExtractor
{
    public interface ITemplateManager : IService
    {
        System.Collections.Generic.IEnumerable<IReplyTemplate> GetTemplates();
        void UpdateTemplates( System.Collections.Generic.IEnumerable<IReplyTemplate> templates );

        IReplyTemplate NewTemplate();
    }
}
