using System;
using System.Collections.Generic;
using System.Text;
using Fines.IssueZillaLib;

namespace IssueZilla
{
    public abstract class CommandBase
    {
        protected readonly MainFormUCP ucp;

        public CommandBase( MainFormUCP ucp )
        {
            this.ucp = ucp;
        }

        public abstract void Execute();
    }

    public class SaveCommand : CommandBase
    {
        public SaveCommand( MainFormUCP ucp ) : base(ucp)
        {
        }

        public override void Execute()
        {
            FileIssueSource source = new FileIssueSource( @"D:\tmp\issues.xml" );
            this.ucp.StoreIssues( source );
        }
    }

    public class NewIssueCommand : CommandBase
    {
        public NewIssueCommand( MainFormUCP ucp )
            : base( ucp )
        {

        }

        public override void Execute()
        {
            using ( NewIssueForm form = new NewIssueForm() )
            {
                NewIssueFormUCP ucp = new NewIssueFormUCP( form, this.ucp );
                if ( form.ShowDialog( this.ucp.WindowHandle ) == System.Windows.Forms.DialogResult.OK )
                {
                    this.ucp.AddNewIssue( ucp.Issue );
                    this.ucp.SelectedIssue = ucp.Issue;
                }
            }
        }
    }

    public class StartupCommand : CommandBase
    {
        public StartupCommand( MainFormUCP ucp ) : base(ucp)
        {

        }

        public override void Execute()
        {
            WebIssueSource source = new WebIssueSource( @"http://ankhsvn.open.collab.net/", 
                new CredentialService(this.ucp.WindowHandle, this.ucp.Invoker));
            //FileIssueSource source = new FileIssueSource( @"D:\tmp\issues.xml" );
            this.ucp.LoadIssues( source );

        }
    }

    public class RowClickCommand : CommandBase
    {
        public RowClickCommand( MainFormUCP ucp )
            : base( ucp )
        {

        }

        public override void Execute()
        {
            using ( EditIssueForm form = new EditIssueForm() )
            {
                EditIssueFormUCP ucp = new EditIssueFormUCP( this.ucp.SelectedIssue, form, this.ucp );
                form.ShowDialog( this.ucp.WindowHandle );

            }
        }
    }
}
