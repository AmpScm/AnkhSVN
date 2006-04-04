using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using System.Diagnostics;
using Fines.Utils.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ErrorReport.GUI
{
    class MainFormUCP
    {
        public event EventHandler SelectedReportChanged;
        public event EventHandler IsReplyingChanged;
        public event EventHandler ReplyTextChanged;
        public event EventHandler TemplatesWanted;
        public event EventHandler SelectedReportModified;

        public MainFormUCP( IFactory factory, IProgressCallback cb, ISynchronizeInvoke invoker )
        {
            this.callback = cb;
            this.storage = factory.GetStorage(cb);
            this.mailer = factory.GetMailer( cb );
            this.factory = factory;
            this.invoker = invoker;
        }

        public IProgressCallback Callback
        {
            get { return callback; }
        }

        public IFactory Factory
        {
            get { return this.factory; }
        }


        public IEnumerable<IErrorReport> GetUnansweredReports()
        {
            this.errorReports = ListUtils.ConvertTo<IErrorReport, List<IErrorReport>>(
                this.storage.GetAllReports() );

            return this.errorReports;
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

        public IEnumerable<IReplyTemplate> Templates
        {
            get
            {
                if ( this.templates == null )
                {
                    this.templates = this.factory.GetTemplateManager( this.callback ).GetTemplates();
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
            this.templates = this.factory.GetTemplateManager( this.callback ).GetTemplates();
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
        private IFactory factory;
        private IList<IErrorReport> errorReports;
        private IStorage storage;
        private IMailer mailer;
        private const int MailWidth = 78;



        
    }
}
