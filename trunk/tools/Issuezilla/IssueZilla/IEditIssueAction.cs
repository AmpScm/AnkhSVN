using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;
using IssueZilla.EditIssueUserControls;

namespace IssueZilla
{
    public interface IEditIssueAction
    {
        void SetIssue( issue issue );
        UserControl EditingControl { get; }
        void Commit();
    }

    public abstract class EditIssueActionBase : IEditIssueAction
    {
        public EditIssueActionBase(issue issue)
        {
            this.issue = issue;
        }

        public void SetIssue( issue issue )
        {
            this.issue = issue;
        }

        public UserControl EditingControl
        {
            get
            {
                if ( this.userControl == null )
                {
                    this.userControl = this.CreateUserControl();
                };
                return this.userControl;
            }
        }

        public abstract void Commit();

        protected virtual UserControl CreateUserControl()
        {
            return null;
        }


        protected issue issue;
        private UserControl userControl;
    }




    public class NoChangeEditIssueAction : EditIssueActionBase
    {
        public NoChangeEditIssueAction( issue issue )
            : base( issue )
        {

        }
        public override void Commit()
        {
            this.issue.knob = "none";
        }

        public override string ToString()
        {
            return "Leave";
        }
    }

    public class AcceptIssueEditAction : EditIssueActionBase
    {
        public AcceptIssueEditAction( issue issue )
            : base( issue )
        {

        }
        public override void Commit()
        {
            this.issue.knob = "accept";
        }

        public override string ToString()
        {
            return "Accept issue";
        }
    }

    public class ResolveIssueEditAction : EditIssueActionBase
    {
        public ResolveIssueEditAction( issue issue, IMetadataSource source )
            : base( issue )
        {
            this.userControl = new ResolveIssueUserControl( source );
        }
        public override void Commit()
        {
            this.issue.knob = "accept";
        }

        protected override UserControl CreateUserControl()
        {
            return this.userControl;
        }

        public override string ToString()
        {
            return "Resolve issue";
        }

        private ResolveIssueUserControl userControl;
    }
}
