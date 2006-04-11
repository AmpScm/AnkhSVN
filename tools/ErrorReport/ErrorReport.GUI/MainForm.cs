using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;
using Fines.Utils.Collections;
using ErrorReport.GUI.Properties;
using Ankh.Tools;
using System.Diagnostics;

using IServiceProvider = ErrorReportExtractor.IServiceProvider;

namespace ErrorReport.GUI
{
    public partial class MainForm : Form
    {
        public MainForm(IServiceProvider provider)
        {
            InitializeComponent();

            this.MainMenuStrip = this.menuManager.MainMenu;
            this.Controls.Add( this.MainMenuStrip );

            provider.SetProgressCallback( this.progressCallback );

            this.ucp = new MainFormUCP( provider, this.progressCallback, this );

            this.Load += new EventHandler( MainForm_Load );
            HandleListViewSelections();

            this.SetupCommands();
            this.DataBind();

            this.ucp.TemplatesWanted += delegate { this.ShowTemplates(); };
            this.templateList = new TemplateList();
            this.templateList.Deactivate += delegate
            {
                if ( this.templateList.SelectedTemplate != null )
                {
                    this.replyTextBox.AppendText( this.templateList.SelectedTemplate.TemplateText );
                }
            };

            this.ucp.SelectedReportModified += delegate
            {
                if ( this.reportsListView.SelectedItems.Count > 0 )
                {
                    ListViewItem item = this.reportsListView.SelectedItems[ 0 ];
                    this.FormatListItem( item, item.Tag as IErrorReport );
                }
            };
        }

        private void ShowTemplates()
        {
            this.templateList.SetTemplates( this.ucp.Templates );
            
            this.templateList.Location = this.replyTextBox.PointToScreen(
                this.replyTextBox.GetPositionFromCharIndex( 
                    this.replyTextBox.SelectionStart ) );
            this.templateList.Show();
        }

        private void HandleListViewSelections()
        {
            bool selecting = false;

            this.ucp.SelectedReportChanged += delegate
            {
                if ( !selecting && this.ucp.SelectedIndex >= 0)
                {
                    selecting = true;
                    this.reportsListView.SelectedIndices.Clear();
                    this.reportsListView.SelectedIndices.Add( this.ucp.SelectedIndex );
                    this.reportsListView.EnsureVisible( this.ucp.SelectedIndex );
                    selecting = false;
                }
                if ( this.ucp.SelectedReport != null )
                {
                    this.messageDetailRichTextBox.Text = this.ucp.SelectedReport.Body;
                }
            };

            this.reportsListView.SelectedIndexChanged += delegate
            {
                if ( !selecting )
                {
                    selecting = true;
                    if ( this.reportsListView.SelectedItems.Count > 0 )
                    {
                        this.ucp.SelectedReport = this.reportsListView.SelectedItems[ 0 ].Tag as IErrorReport;
                    }
                    selecting = false;
                }
            };
        }

        private void DataBind()
        {
            this.ucp.IsReplyingChanged += delegate
            {
                this.splitContainerBottom.Panel2Collapsed = !this.ucp.IsReplying;
                if ( ucp.IsReplying )
                {
                    this.replyTextBox.SelectionStart = this.replyTextBox.Text.Length;
                    this.replyTextBox.Focus();
                }
            };
            this.splitContainerBottom.Panel2Collapsed = !this.ucp.IsReplying;

            this.replyTextBox.DataBindings.Add( "Text", this.ucp, "ReplyText", false, DataSourceUpdateMode.OnPropertyChanged );
        }

        private void SetupCommands()
        {
            foreach ( ICommand cmd in GetICommands( this.GetType().Assembly ) )
            {
                this.SetupCommand( cmd );
            }


        }
        
        private void SetupCommand( ICommand command )
        {
            object[] attrs = command.GetType().GetCustomAttributes( typeof( ToolBarAttribute ), false );
            if ( attrs.Length != 0 )
            {
                ToolBarAttribute commandAttribute = attrs[ 0 ] as ToolBarAttribute;
                ToolStripButton toolStripButton = new ToolStripButton( commandAttribute.Text );

                toolStripButton.Click += delegate { command.Execute(); };
                command.EnabledChanged += delegate { toolStripButton.Enabled = command.Enabled; };
                toolStripButton.Enabled = command.Enabled;
                this.toolStrip.Items.Add( toolStripButton );
            }

            attrs = command.GetType().GetCustomAttributes( typeof( MenuItemAttribute ), false );
            if ( attrs.Length != 0 )
            {
                MenuItemAttribute menuAttribute = attrs[ 0 ] as MenuItemAttribute;
                menuManager.AddCommand( command, menuAttribute.Path );
            }

            attrs = command.GetType().GetCustomAttributes( typeof(KeyBindingAttribute), false );
            if ( attrs.Length > 0 )
            {
                foreach ( KeyBindingAttribute attr in attrs )
                {
                    this.keyCommandMap[ attr.Keys ] = command;
                }
            }
        }

        private IEnumerable<ICommand> GetICommands( System.Reflection.Assembly assembly )
        {
            Type[] commandTypes = assembly.ManifestModule.FindTypes( delegate( Type type, object o )
            {
                return !type.IsAbstract && typeof( ICommand ).IsAssignableFrom( type );
            }, null );

            return ListUtils.Map<Type, ICommand>( commandTypes, delegate( Type type )
            {
                return (ICommand)Activator.CreateInstance( type, this.ucp );
            } );

        }
        void MainForm_Load( object sender, EventArgs e )
        {
            LoadErrorReportsIntoListView();
        }

        private void LoadErrorReportsIntoListView()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            this.reportsListView.BeginUpdate();
            foreach ( IErrorReport report in ucp.GetUnansweredReports() )
            {
                TreeListItem item = new TreeListItem( new string[] { 
                    report.ReceivedTime.ToString(), 
                    String.Format("{0} <{1}>", report.SenderName, report.SenderEmail), 
                    report.ExceptionType,
                    String.Format("{0}.{1}.{2}.{3}", report.MajorVersion, report.MinorVersion, report.PatchVersion, report.Revision),
                    report.DteVersion != null ? report.DteVersion : string.Empty
                } );
                item.Tag = report;
                this.FormatListItem( item, report );
                this.reportsListView.Items.Add( item );
            }
            this.reportsListView.EndUpdate();
            stopWatch.Stop();
            Debug.WriteLine( stopWatch.Elapsed.ToString() );
        }

        private void MainForm_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.Control )
            {
                ICommand cmd;
                if ( this.keyCommandMap.TryGetValue( e.KeyCode, out cmd ) && cmd.Enabled )
                {
                    cmd.Execute();
                    e.Handled = true;
                }
            }
        }

        private void FormatListItem( ListViewItem item, IErrorReport report )
        {
            item.Font = report.RepliedTo ? Settings.Default.BaseFont : UnreadFont;
        }

        private static readonly Font UnreadFont = new Font( Settings.Default.BaseFont, FontStyle.Bold );
        private MainFormUCP ucp;
        private TemplateList templateList;
        private IDictionary<Keys, ICommand> keyCommandMap = new Dictionary<Keys, ICommand>();
        private MenuManager menuManager = new MenuManager();
    }
}