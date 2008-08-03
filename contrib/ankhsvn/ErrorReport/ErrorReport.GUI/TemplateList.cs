using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;

namespace ErrorReport.GUI
{
    public partial class TemplateList : Form
    {
        public TemplateList()
        {
            InitializeComponent();

            this.Deactivate += delegate { this.Hide(); };
            this.listBox.SelectedIndexChanged += delegate { this.timer.Start(); };
            this.timer.Tick += delegate
            {
                IReplyTemplate template = this.replyTemplateBindingSource.Current as IReplyTemplate;
                if ( template != null )
                {
                    this.toolTip.Show( template.TemplateText, this.listBox ); 
                }
                this.timer.Stop();
            };
        }

        public IReplyTemplate SelectedTemplate
        {
            get { return this.selectedTemplate; }
        }

        public void SetTemplates( IEnumerable<IReplyTemplate> templates )
        {
            //this.replyTemplateBindingSource.DataSource = null;
            this.replyTemplateBindingSource.DataSource = templates;
            //this.replyTemplateBindingSource.ResetBindings( false );
        }

        private void TemplateList_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter )
            {
                this.selectedTemplate = this.replyTemplateBindingSource.Current as IReplyTemplate;
                this.Hide();
            }
            else if ( e.KeyCode == Keys.Escape)
            {
                this.selectedTemplate = null;
                this.Hide();
            }
        }

        private IReplyTemplate selectedTemplate;
    }
}