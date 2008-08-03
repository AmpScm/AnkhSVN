using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;
using ErrorReport.GUI.Properties;

using IServiceProvider = ErrorReportExtractor.IServiceProvider;

namespace ErrorReport.GUI
{
    public partial class TemplateEditor : Form
    {
        public TemplateEditor(IProgressCallback callback, IServiceProvider factory)
        {
            InitializeComponent();

            this.ucp = new TemplateEditorUCP( callback, factory );

            SetUpEvents();
            CreateFonts();
        }

        private void CreateFonts()
        {
            this.baseFont = Settings.Default.BaseFont;
            this.addedFont = new Font( this.baseFont, FontStyle.Italic);
            this.deletedFont = new Font( this.baseFont, FontStyle.Strikeout );
            this.modifiedFont = new Font( this.baseFont, FontStyle.Bold );

        }

        private void SetUpEvents()
        {
            this.Load += new EventHandler( TemplateEditor_Load );

            this.ucp.TemplatesChanged += new EventHandler( ucp_TemplatesChanged );

            this.timer.Tick += delegate
            {
                if ( this.templatesListView.SelectedItems.Count > 0 )
                {
                    ListViewItem item = this.templatesListView.SelectedItems[ 0 ];
                    IReplyTemplate template = item.Tag as IReplyTemplate;
                    if ( template != null )
                    {
                        this.SetListItem( item, template );
                    }
                }
            };

            

            this.HandleSelections();
            this.timer.Start();
        }

        private void HandleSelections()
        {
            bool selecting = false;

            this.ucp.SelectedTemplateChanged += delegate
            {
                int index = this.ucp.SelectedTemplateIndex;
                if ( !selecting && index >= 0 )
                {
                    this.templatesListView.SelectedIndices.Clear();
                    this.templatesListView.SelectedIndices.Add( index );
                    this.templatesListView.EnsureVisible( index );
                }
                this.replyTemplateBindingSource.DataSource = this.ucp.SelectedTemplate;
            };

            this.templatesListView.SelectedIndexChanged += delegate
            {
                selecting = true;
                if ( this.templatesListView.SelectedItems.Count > 0 )
                {
                    this.ucp.SelectedTemplate = this.templatesListView.SelectedItems[ 0 ].Tag as IReplyTemplate;
                }
                selecting = false;
            };

        }

        void ucp_TemplatesChanged( object sender, EventArgs e )
        {
            this.templatesListView.Items.Clear();
            foreach ( IReplyTemplate template in this.ucp.Templates )
            {
                ListViewItem item = new ListViewItem( );
                this.SetListItem( item, template );
                item.Tag = template;

                this.templatesListView.Items.Add( item );
            }
        }

        private void SetListItem( ListViewItem item, IReplyTemplate template )
        {
            string text = template.TemplateText.Length > 60 ? template.TemplateText.Substring( 0, 60 ) :
                    template.TemplateText;

            text += "...";
            item.Text = text;

            switch ( template.State )
            {
                case TemplateState.Deleted:
                case TemplateState.None:
                    item.Font = this.deletedFont;
                    break;
                case TemplateState.New:
                    item.Font = this.addedFont;
                    break;
                case TemplateState.Unmodified:
                    item.Font = this.baseFont;
                    break;
                case TemplateState.Modified:
                    item.Font = this.modifiedFont;
                    break;
            }
        }

        void TemplateEditor_Load( object sender, EventArgs e )
        {
            this.ucp.LoadTemplates();            
        }

        private void newButton_Click( object sender, EventArgs e )
        {
            this.ucp.NewTemplate();
        }

        private void okButton_Click( object sender, EventArgs e )
        {
            this.ucp.SaveTemplates();
        }

        private void deleteButton_Click( object sender, EventArgs e )
        {
            this.ucp.DeleteTemplate();
        }

        private void richTextBox_TextChanged( object sender, EventArgs e )
        {
            
        }


        private Font baseFont;
        private Font addedFont;
        private Font modifiedFont;
        private Font deletedFont;

        private TemplateEditorUCP ucp;

       

        
    }
}