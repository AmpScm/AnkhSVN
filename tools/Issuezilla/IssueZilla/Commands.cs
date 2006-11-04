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

    public class StartupCommand : CommandBase
    {
        public StartupCommand( MainFormUCP ucp ) : base(ucp)
        {

        }

        public override void Execute()
        {
            WebIssueSource source = new WebIssueSource( @"http://ankhsvn.tigris.org/issues/xml.cgi" );
            //FileIssueSource source = new FileIssueSource( @"D:\tmp\issues.xml" );
            this.ucp.LoadIssues( source );
        }
    }
}
