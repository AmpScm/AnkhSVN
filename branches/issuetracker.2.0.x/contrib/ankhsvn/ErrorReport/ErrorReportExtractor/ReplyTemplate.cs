using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public enum TemplateState
    {
        New,
        Modified,
        Deleted,
        Unmodified,
        None
    }

    class ReplyTemplate : ErrorReportExtractor.IReplyTemplate
    {
        public ReplyTemplate(int id, string templateText)
        {
            this.templateText = templateText;
            this.id = id;
        }

        public TemplateState State
        {
            get { return state; }
            set { state = value; }
        }
	
        public int ID
        {
            get { return id; }
        }
	
        public string TemplateText
        {
            get { return templateText; }
            set 
            { 
                templateText = value;
                state = state == TemplateState.Unmodified ? TemplateState.Modified : state;
            }
        }

        public void Delete()
        {
            if ( state == TemplateState.New )
            {
                this.state = TemplateState.None;
            }
            else
            {
                this.state = TemplateState.Deleted;
            }
        }

        private string templateText;
        private int id;
        private TemplateState state;
	
    }
}
