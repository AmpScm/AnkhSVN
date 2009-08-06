using System;
namespace ErrorReportExtractor
{
    public interface IReplyTemplate
    {
        int ID { get; }
        TemplateState State { get; }
        string TemplateText { get; set; }

        void Delete();
    }
}
