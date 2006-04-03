using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor.ReplyTemplatesDataSetTableAdapters;

namespace ErrorReportExtractor
{
    class TemplateManager : ErrorReportExtractor.ITemplateManager
    {
        public TemplateManager( IProgressCallback callback )
        {
            this.callback = callback;
        }

        public IEnumerable<IReplyTemplate> GetTemplates()
        {
            ReplyTemplatesTableAdapter adapter = new ReplyTemplatesTableAdapter();
            foreach ( ReplyTemplatesDataSet.ReplyTemplatesRow row in adapter.GetData() )
            {
                ReplyTemplate template = new ReplyTemplate( row.ID, row.TemplateText );
                template.State = TemplateState.Unmodified;
                yield return template;
            }
        }

        public void UpdateTemplates( IEnumerable<IReplyTemplate> templates )
        {
            ReplyTemplatesDataSet.ReplyTemplatesDataTable table = new ReplyTemplatesDataSet.ReplyTemplatesDataTable();
            ReplyTemplatesDataSet.ReplyTemplatesRow row;
            table.BeginLoadData();
            foreach ( IReplyTemplate template in templates )
            {
                row = table.NewReplyTemplatesRow();
                row.ID = template.ID;
                row.TemplateText = template.TemplateText;
                table.AddReplyTemplatesRow( row );
                row.AcceptChanges();

                if ( template.State == TemplateState.Deleted )
                {
                    row.Delete();
                }
                else if ( template.State == TemplateState.New )
                {
                    row.SetAdded();
                }
                else if ( template.State == TemplateState.Modified )
                {
                    row.SetModified();
                }
            }

            ReplyTemplatesTableAdapter adapter = new ReplyTemplatesTableAdapter();
            adapter.Update( table );
        }

        public IReplyTemplate NewTemplate()
        {
            ReplyTemplate template = new ReplyTemplate( 0, "Fill in template text here" );
            template.State = TemplateState.New;
            return template;
        }

        private IProgressCallback callback;
    }
}
