using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using Fines.Utils.Collections;
using IServiceProvider = ErrorReportExtractor.IServiceProvider;

namespace ErrorReport.GUI
{
    class TemplateEditorUCP
    {
        public event EventHandler SelectedTemplateChanged;
        public event EventHandler TemplatesChanged;

        public TemplateEditorUCP(IProgressCallback callback, IServiceProvider provider)
        {
            this.callback = callback;
            this.templateManager = provider.GetService<ITemplateManager>();
        }

        public void LoadTemplates()
        {
            this.templates =  ListUtils.ConvertTo<IReplyTemplate, List<IReplyTemplate>>(
                        this.templateManager.GetTemplates() );
            this.OnTemplatesChanged();
        }

        public IEnumerable<IReplyTemplate> Templates
        {
            get
            {
                if ( this.templates == null)
                {
                    this.LoadTemplates();
                }
                return this.templates;
            }
        }

        public IReplyTemplate SelectedTemplate
        {
            get{ return this.selectedTemplate; }
            set
            {
                this.selectedTemplate = value;
                if ( this.SelectedTemplateChanged != null)
                {
                    this.SelectedTemplateChanged(this, EventArgs.Empty);
                }
            }
        }

        public int SelectedTemplateIndex
        {
            get
            {
                if ( this.SelectedTemplate != null )
                {
                    return this.templates.IndexOf( this.SelectedTemplate );
                }
                else
                {
                    return -1;
                }
            }
        }

        public void SaveTemplates()
        {
            this.templateManager.UpdateTemplates(this.templates);
        }

        public void NewTemplate()
        {
            IReplyTemplate template = this.templateManager.NewTemplate();
            this.templates.Add( template );
            this.OnTemplatesChanged();
            this.SelectedTemplate = template;
        }

        public void DeleteTemplate()
        {
            if ( this.SelectedTemplate != null )
            {
                this.SelectedTemplate.Delete();
                this.OnTemplatesChanged();
            }
        }

        private void OnTemplatesChanged()
        {
            if ( this.TemplatesChanged != null)
            {
                this.TemplatesChanged(this, EventArgs.Empty);
            }
        }

        private IReplyTemplate selectedTemplate;
        private IList<IReplyTemplate> templates;
        private IProgressCallback callback;
        private ITemplateManager templateManager;
    }
}
