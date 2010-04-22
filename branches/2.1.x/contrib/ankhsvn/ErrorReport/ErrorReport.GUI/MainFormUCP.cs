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
using ErrorReport.GUI.Properties;

namespace ErrorReport.GUI
{
    class MainFormUCP : INotifyPropertyChanged
    {
        public event EventHandler SelectedReportChanged;
        public event EventHandler SelectedMailItemChanged;
        public event EventHandler IsReplyingChanged;
        public event EventHandler ReplyTextChanged;
        public event EventHandler TemplatesWanted;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ReportsLoaded;
        public event EventHandler ReformatSelection;
        public event EventHandler InsertionPointChanged;

        public enum Mode
        {
            Threaded, 
            Flat
        }

        

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


        public IEnumerable<IMailItem> MailItems
        {
            get{ return this.mailItems; }
        }

        public IErrorReport SelectedReport
        {
            get { return selectedReport; }
            set 
            { 
                selectedReport = value;
                if ( this.SelectedReportChanged != null )
                {
                    this.SelectedReportChanged( this, EventArgs.Empty );
                }
                
                this.SelectedMailItem = value;
            }
        }

        public IMailItem SelectedMailItem
        {
            get { return selectedMailItem; }
            set
            {
                selectedMailItem = value;
                if ( SelectedMailItemChanged != null )
                {
                    SelectedMailItemChanged( this, EventArgs.Empty );
                }
            }
        }

        public int TotalCount
        {
            get
            {
                if ( this.mailItems != null )
                {
                    return this.mailItems.Count;
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
                if ( this.mailItems != null )
                {

                    return ListUtils.Count<IMailItem>( this.mailItems, delegate( IMailItem report )
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
                if ( this.SelectedMailItem != null )
                {
                    return this.mailItems.IndexOf( this.SelectedMailItem );
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

        public int InsertionPoint
        {
            get { return this.insertionPoint; }
            set
            {
                this.insertionPoint = value;
                if ( this.InsertionPointChanged != null )
                {
                    this.InsertionPointChanged( this, EventArgs.Empty );
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

        public string Signature
        {
            get
            {
                return Settings.Default.Signature ?? String.Empty;
            }

            set
            {
                Settings.Default.Signature = value;
                Settings.Default.Save();
            }
        }

        public bool CanGoNext
        {
            get 
            {
                int index = this.SelectedIndex;
                return this.mailItems != null && index < this.mailItems.Count - 1; 
            }
        }

        public bool CanGoPrevious
        {
            get
            {
                int index = this.SelectedIndex;
                return this.mailItems != null && index > 0;
            }
        }

        public ISynchronizeInvoke Invoker
        {
            get { return invoker; }
        }
	

        public void InitiateReplyForSelectedReport()
        {
            Debug.Assert( this.SelectedMailItem != null );

            this.ReplyText = Quotify( this.SelectedMailItem.Body ) + Environment.NewLine + Environment.NewLine;


            int insertionPoint = this.replyText.Length - this.ReplyText.Split( new string[] { Environment.NewLine }, 
                StringSplitOptions.None ).Length;
            this.ReplyText += "--" + Environment.NewLine + this.Signature;
            OnReplyTextChanged();

            this.InsertionPoint = insertionPoint;
            
            this.IsReplying = true;
        }

       

        public void SendReplyForSelectedItem()
        {
            Debug.Assert(this.SelectedMailItem != null);
            this.mailer.SendReply( this.SelectedMailItem, this.ReplyText );
            this.storage.AnswerReport( this.SelectedMailItem, this.ReplyText );

            if ( this.ReformatSelection != null )
            {
                this.ReformatSelection( this, EventArgs.Empty );
            }


            this.IsReplying = false;
            this.ReplyText = String.Empty;
            OnReplyTextChanged();
            this.OnNotifyPropertyChanged( "UnansweredCount" );
        }

        public void GetReplies( IErrorReport report )
        {
            this.storage.GetReplies( report );
        }

        public void NextReport()
        {
            if ( this.CanGoNext )
            {
                this.SelectedMailItem = this.mailItems[ this.SelectedIndex + 1 ];
            }
        }

        public void PreviousReport()
        {
            if ( this.CanGoPrevious )
            {
                this.SelectedMailItem = this.mailItems[ this.SelectedIndex - 1 ];
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

            if ( this.currentMode == Mode.Threaded )
            {
                worker.Work += delegate
                {
                    this.mailItems = ListUtils.ConvertTo<IErrorReport, List<IMailItem>, IMailItem>(
                           this.storage.GetAllReports() );
                };
            }
            else
            {
                worker.Work += delegate
                {
                    this.mailItems = ListUtils.ConvertTo<IMailItem, List<IMailItem>>(
                        this.storage.GetAllItems() );
                };
            }

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

        public void MarkSelectedMailItemAsRead()
        {
            if ( this.SelectedMailItem != null )
            {
                this.SelectedMailItem.Read = true;
                this.storage.UpdateMailItem( this.SelectedMailItem );
                if ( this.ReformatSelection != null )
                {
                    this.ReformatSelection( this, EventArgs.Empty );
                }
            }
        }

        public void ToggleFlatOrThreaded()
        {
            this.currentMode = this.currentMode == Mode.Threaded ? Mode.Flat : Mode.Threaded;
            this.LoadReports();
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

        private int insertionPoint;
        private IMailItem selectedMailItem;
        private IEnumerable<IReplyTemplate> templates;
        private IErrorReport selectedReport;
        private bool isReplying = false;
        private string replyText;
        private Mode currentMode = Mode.Flat;
        private IServiceProvider provider;
        private IList<IMailItem> mailItems;
        private IStorage storage;
        private IMailer mailer;
        private const int MailWidth = 78;



    }
}
