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
using Fines.Utils.Debugging;

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

            DebugUtils.DebugEvents( this.replyTextBox );

            this.ucp.TemplatesWanted += delegate { this.ShowTemplates(); };
            this.templateList = new TemplateList();
            this.templateList.Deactivate += delegate
            {
                if ( this.templateList.SelectedTemplate != null )
                {
                    this.replyTextBox.SelectedText = this.templateList.SelectedTemplate.TemplateText;
                    this.replyTextBox.ScrollToCaret();
                }
            };

            this.ucp.ReformatSelection += delegate
            {                
                if (this.reportsListView.SelectedItems.Count > 0)
	            {
                    ListViewItem item = this.reportsListView.SelectedItems[0];
		            IMailItem mailItem = item.Tag as IMailItem;
                    this.FormatListItem( item, mailItem );
	            }
            };

            this.ucp.InsertionPointChanged += delegate
            {
                this.replyTextBox.SelectionStart = this.ucp.InsertionPoint;
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

            this.ucp.SelectedMailItemChanged += delegate
            {
                if ( !selecting && this.ucp.SelectedIndex >= 0)
                {
                    selecting = true;
                    this.reportsListView.SelectedIndices.Clear();
                    this.reportsListView.SelectedIndices.Add( this.ucp.SelectedIndex );
                    this.reportsListView.EnsureVisible( this.ucp.SelectedIndex );
                    selecting = false;
                }
                
            };

            this.ucp.SelectedMailItemChanged += delegate
            {
                if ( this.ucp.SelectedMailItem != null )
                {
                    this.messageDetailRichTextBox.Text = this.ucp.SelectedMailItem.Body;
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
                        this.ucp.SelectedMailItem = this.reportsListView.SelectedItems[ 0 ].Tag as IMailItem;
                    }
                    selecting = false;
                }
            };

            this.reportsListView.BeforeExpand += delegate( object sender, TreeListItemEventArgs args )
            {
                if ( args.Item.Children.Count > 0 && args.Item.Children[0].Tag == Dummy )
                {
                    this.LoadChildren( args.Item );
                }
            };
        }

        private void LoadChildren( TreeListItem treeListItem )
        {
            IErrorReport report = treeListItem.Tag as IErrorReport;
            if ( report != null )
            {
                treeListItem.Children.RemoveAt( 0 );

                this.ucp.GetReplies( report );
                this.reportsListView.BeginUpdate();
                AddRepliesToTreeList( report.Replies, treeListItem );
                this.reportsListView.EndUpdate();
            }
        }

        private void AddRepliesToTreeList( IEnumerable<IMailItem> replies, TreeListItem treeListItem )
        {
            foreach ( IMailItem reply in replies )
            {
                TreeListItem child = new TreeListItem( new string[]
                    {
                        reply.ReceivedTime.ToString(),
                        String.Format("{0} <{1}>", reply.SenderName, reply.SenderEmail),
                    } );
                child.Tag = reply;
                this.FormatListItem( child, reply );
                treeListItem.Children.Add( child );

                AddRepliesToTreeList( reply.Replies, child );
            }
        }

        private void DataBind()
        {
            this.ucp.IsReplyingChanged += delegate
            {
                this.splitContainerBottom.Panel2Collapsed = !this.ucp.IsReplying;
                if ( ucp.IsReplying )
                {
                    this.replyTextBox.SelectionStart = this.ucp.InsertionPoint;
                    this.replyTextBox.Focus();
                }
            };
            this.splitContainerBottom.Panel2Collapsed = !this.ucp.IsReplying;

            this.replyTextBox.DataBindings.Add( "Text", this.ucp, "ReplyText", false, DataSourceUpdateMode.OnPropertyChanged );

            Binding totalBinding = this.totalLabel.DataBindings.Add( "Text", this.ucp, "TotalCount" );
            totalBinding.Format += delegate( object sender, ConvertEventArgs args )
            {
                args.Value = String.Format( "Total number of reports: {0}", args.Value );
            };

            Binding unansweredBinding = this.unansweredLabel.DataBindings.Add( "Text", this.ucp, "UnansweredCount", true);
            unansweredBinding.Format +=  delegate( object sender, ConvertEventArgs args )
            {
                args.Value = String.Format( "Unanswered reports: {0}", args.Value );
            };
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
            this.ucp.ReportsLoaded += delegate { LoadErrorReportsIntoListView(); };
            this.progressCallback.Info( "Loading reports from storage." );
            this.ucp.LoadReports();
        }

        private void LoadErrorReportsIntoListView()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.progressCallback.Info( "Loading reports into list view." );
            this.reportsListView.Items.Clear();
            this.reportsListView.BeginUpdate();
            foreach ( IMailItem mailItem in ucp.MailItems )
            {
                TreeListItem item;
                
                if ( mailItem is IErrorReport )
                {
                    IErrorReport report = mailItem as IErrorReport;
                    item = new TreeListItem( new string[] { 
                        report.ReceivedTime.ToString(), 
                        String.Format("{0} <{1}>", report.SenderName, report.SenderEmail), 
                        report.ExceptionType,
                        String.Format("{0}.{1}.{2}.{3}", report.MajorVersion, report.MinorVersion, report.PatchVersion, report.Revision),
                        report.DteVersion != null ? report.DteVersion : string.Empty
                    } );

                    if ( report.HasReplies )
                    {
                        TreeListItem child = new TreeListItem( "Dummy" );
                        child.Tag = Dummy;
                        item.Children.Add( child );
                    }
                }
                else
                {
                    item = new TreeListItem( new string[]
                    {
                        mailItem.ReceivedTime.ToString(),
                        String.Format("{0} <{1}>", mailItem.SenderName, mailItem.SenderEmail),
                    } );
                }

                item.Tag = mailItem;
                this.FormatListItem( item, mailItem );
                
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

        private void FormatListItem( ListViewItem item, IMailItem report )
        {
            //if ( report.ReceivedTime.Date == ( new DateTime( 2006, 4, 25 ).Date ) )
            //{
            //    Debugger.Break();
            //}

            item.Font = report.Read ? Settings.Default.BaseFont : UnreadFont;
            item.ForeColor = report.RepliedTo ? this.reportsListView.ForeColor : Color.Red;
        }

        private static readonly Font UnreadFont = new Font( Settings.Default.BaseFont, FontStyle.Bold );
        private MainFormUCP ucp;
        private TemplateList templateList;
        private IDictionary<Keys, ICommand> keyCommandMap = new Dictionary<Keys, ICommand>();
        private MenuManager menuManager = new MenuManager();
        private static readonly object Dummy = new object();
    }
}