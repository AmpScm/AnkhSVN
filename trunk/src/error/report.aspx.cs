using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Mail;
using System.Configuration;

namespace error
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class report : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.TextBox errorMessageBox;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Button sendButton;
        protected System.Web.UI.WebControls.TextBox emailBox;
        protected System.Web.UI.WebControls.Label Label4;
        protected System.Web.UI.WebControls.TextBox commentsBox;
        protected System.Web.UI.WebControls.Label Label1;
    
		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !Page.IsPostBack )
                this.FillInForm();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
            this.commentsBox.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
		#endregion

        private void FillInForm()
        {
            if ( this.Request["message"] != null )
                this.errorMessageBox.Text = this.Request["message"];
        }

        private void sendButton_Click(object sender, System.EventArgs e)
        {
            MailMessage msg = new MailMessage();
            msg.To = ConfigurationSettings.AppSettings["recipient"];
            if ( this.emailBox.Text != "" )
                msg.From = this.emailBox.Text;
            else
                msg.From = "anonymous@nowhere.com";
            msg.Subject = "Exception";
            msg.Body = this.errorMessageBox.Text + 
                Environment.NewLine + Environment.NewLine + 
                this.commentsBox.Text;

            SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["smtpserver"];
            SmtpMail.Send( msg );
            Response.Redirect( "thanks.html" );
        }

        private void TextBox1_TextChanged(object sender, System.EventArgs e)
        {
        
        }
	}
}
