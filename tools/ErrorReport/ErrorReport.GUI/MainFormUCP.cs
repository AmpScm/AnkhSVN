using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using System.Diagnostics;
using Fines.Utils.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;

using IServiceProvider = ErrorReportExtractor.IServiceProvider;
using System.Windows.Forms;
using System.Threading;

namespace ErrorReport.GUI
{
    class MainFormUCP : INotifyPropertyChanged
    {
        public event EventHandler SelectedReportChanged;
        public event EventHandler IsReplyingChanged;
        public event EventHandler ReplyTextChanged;
        public event EventHandler TemplatesWanted;
        public event EventHandler SelectedReportModified;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ReportsLoaded;


        public MainFormUCP( IServiceProvider provider, IProgressCallback cb, ISynchronizeInvoke invoker )
        {
            this.callback = cb;
            this.storage = provider.GetService<IStorage>();
            this.mailer = provider.GetService<IMailer>();
            this.provider = provider;
            this.invoker = invoker;
        }

        public IProgressCallback Callback
        {
            get { return callback; }
        }

        public IServiceProvider ServiceProvider
        {
            get { return this.provider; }
        }


        public IEnumerable<IErrorReport> Reports
        {
            get{ return this.errorReports; }
        }

        public IErrorReport SelectedReport
        {
            get { return selectedReport; }
            set 
            { 
                selectedReport = value;
                if ( SelectedReportChanged != null )
                {
                    SelectedReportChanged( this, EventArgs.Empty );
                }
            }
        }

        public int TotalCount
        {
            get
            {
                if ( this.errorReports != null )
                {
                    return this.errorReports.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int UnansweredCount
        {
            get
            {
                if ( this.errorReports != null )
                {

                    return ListUtils.Count<IErrorReport>( this.errorReports, delegate( IErrorReport report )
                    {
                        return !report.RepliedTo;
                    } );
                }
                else
                {
                    return 0;
                }
            }
        }
        

        public IEnumerable<IReplyTemplate> Templates
        {
            get
            {
                if ( this.templates == null )
                {
                    this.templates = this.provider.GetService<ITemplateManager>().GetTemplates();
                }
                return this.templates;
            }
        }

        public int SelectedIndex
        {
            get
            {
                if ( this.SelectedReport != null )
                {
                    return this.errorReports.IndexOf( this.SelectedReport );
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool IsReplying
        {
            get { return isReplying; }

            private set
            {
                isReplying = value;
                if ( this.IsReplyingChanged != null )
                {
                    this.IsReplyingChanged( this, EventArgs.Empty );
                }
            }
        }

        public string ReplyText
        {
            get { return replyText; }
            set 
            { 
                replyText = value;
               
            }
        }

        public bool CanGoNext
        {
            get 
            {
                int index = this.SelectedIndex;
                return this.errorReports != null && index < this.errorReports.Count - 1; 
            }
        }

        public bool CanGoPrevious
        {
            get
            {
                int index = this.SelectedIndex;
                return this.errorReports != null && index > 0;
            }
        }

        public ISynchronizeInvoke Invoker
        {
            get { return invoker; }
        }
	

        public void InitiateReplyForSelectedReport()
        {
            Debug.Assert( this.SelectedReport != null );

            this.ReplyText = Quotify( this.SelectedReport.Body ) + Environment.NewLine + Environment.NewLine;
            OnReplyTextChanged();
            this.IsReplying = true;
        }

       

        public void SendReplyForSelectedReport()
        {
            Debug.Assert(this.SelectedReport != null);
            this.mailer.SendReply( this.SelectedReport, this.ReplyText );
            this.storage.AnswerReport( this.SelectedReport, this.ReplyText );

            if ( this.SelectedReportModified != null )
            {
                this.SelectedReportModified( this, EventArgs.Empty );
            }

            this.IsReplying = false;
            this.ReplyText = String.Empty;
            OnReplyTextChanged();
            this.OnNotifyPropertyChanged( "UnansweredCount" );
        }

        public void NextReport()
        {
            if ( this.CanGoNext )
            {
                this.SelectedReport = this.errorReports[ this.SelectedIndex + 1 ];
            }
        }

        public void PreviousReport()
        {
            if ( this.CanGoPrevious )
            {
                this.SelectedReport = this.errorReports[ this.SelectedIndex - 1 ];
            }
        }

        public void ShowTemplates()
        {
            if ( TemplatesWanted != null )
            {
                this.TemplatesWanted( this, EventArgs.Empty );
            }
        }

        public void ResetTemplates()
        {
            this.templates = this.provider.GetService<ITemplateManager>().GetTemplates();
        }

        public void LoadReports()
        {
            ThreadWorker worker = new ThreadWorker( this.invoker );

            worker.Work += delegate 
            { 
                this.errorReports = ListUtils.ConvertTo<IErrorReport, List<IErrorReport>>(
                    this.storage.GetAllReports() ); 
            };

            worker.WorkFinished += delegate
            {
                if ( this.ReportsLoaded != null )
                {
                    this.ReportsLoaded( this, EventArgs.Empty );
                }

                this.OnNotifyPropertyChanged( "TotalCount" );
                this.OnNotifyPropertyChanged( "UnansweredCount" );
            };

            worker.Exception += delegate( object sender, ThreadExceptionEventArgs a ) { this.callback.Exception( a.Exception ); };

            worker.Start();
        }

        protected void OnNotifyPropertyChanged( string property )
        {
            if ( this.PropertyChanged != null )
            {
                this.PropertyChanged( this, new PropertyChangedEventArgs( property ) );
            }
        }

        private void OnReplyTextChanged()
        {
            if ( this.ReplyTextChanged != null )
            {
                this.ReplyTextChanged( this, EventArgs.Empty );
            }
        }

        private string Quotify( string body )
        {
            IEnumerable<string> words = this.BreakIntoWords( body );
            IEnumerable<string> lines = this.BreakIntoLines( words);
            return ListUtils.Reduce( lines, delegate( string s1, string s2 )
                {
                    return s1 + Environment.NewLine + s2;
                } ) ;
        }


        private IEnumerable<string> BreakIntoLines( IEnumerable<string> words )
        {
            StringBuilder currentLine = new StringBuilder();
            currentLine.Append( "> " );

            foreach ( string word in words )
            {
                if ( word == Environment.NewLine )
                {
                    yield return currentLine.ToString();
                    currentLine.Length = 0;
                    currentLine.Append( "> " );
                }
                else
                {
                    if ( currentLine.Length + word.Length > MailWidth )
                    {
                        yield return currentLine.ToString();
                        currentLine.Length = 0;
                        currentLine.Append( "> " );
                    }
                    currentLine.Append( word + " " );
                }
            }

            if ( currentLine.Length > "> ".Length )
            {
                yield return currentLine.ToString();
            }
        }

        private IEnumerable<string> BreakIntoWords( string body )
        {
            string[] lines = body.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
            foreach ( string line in lines )
            {
                string[] words = line.Split( ' ' );
                foreach ( string word in words )
                {
                    yield return word;
                }
                yield return Environment.NewLine;
            }
        }

        private IProgressCallback callback;
        private ISynchronizeInvoke invoker;

       
        private IEnumerable<IReplyTemplate> templates;
        private IErrorReport selectedReport;
        private bool isReplying = false;
        private string replyText;
        private IServiceProvider provider;
        private IList<IErrorReport> errorReports;
        private IStorage storage;
        private IMailer mailer;
        private const int MailWidth = 78;
    }
}
