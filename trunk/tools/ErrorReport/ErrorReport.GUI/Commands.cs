using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ErrorReport.GUI
{
    public interface ICommand
    {
        event EventHandler EnabledChanged;
        bool Enabled {get;}
        void Execute();
    }

    internal abstract class CommandBase : ICommand
    {

        public event EventHandler  EnabledChanged;

        public CommandBase(MainFormUCP ucp)
        {
            this.ucp = ucp;
        }

        public virtual bool  Enabled
        {
            get { return enabled; }
        }

        protected void RaiseEnabledChanged()
        {
            if ( this.EnabledChanged != null)
            {
                this.EnabledChanged(this, EventArgs.Empty);
            }
        }

        public abstract void Execute();

        protected bool enabled = false;
        protected MainFormUCP ucp;


    }

    [KeyBinding(Keys.R)]
    [Command("Reply")]
    internal class ReplyCommand : CommandBase
    {
        public ReplyCommand( MainFormUCP ucp )
            : base( ucp )
        {
            this.ucp.SelectedReportChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get { return this.ucp.SelectedReport != null; }
        }

        public override void Execute()
        {
            this.ucp.InitiateReplyForSelectedReport();
        }
    }

    [KeyBinding( Keys.S )]
    [Command( "Send" )]
    internal class SendCommand : CommandBase
    {
        public SendCommand( MainFormUCP ucp )
            : base( ucp )
        {
            this.ucp.IsReplyingChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get { return this.ucp.IsReplying; }
        }

        public override void Execute()
        {
            this.ucp.SendReplyForSelectedReport();
            this.ucp.NextReport();
        }
    }

    [KeyBinding(Keys.N)]
    [Command( "Next" )]
    internal class NextCommand : CommandBase
    {
        public NextCommand(MainFormUCP ucp) : base(ucp)
        {
            this.ucp.SelectedReportChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get
            {
                return this.ucp.CanGoNext;
            }
        }
        public override void Execute()
        {
            this.ucp.NextReport();
        }
    }

    [KeyBinding( Keys.P )]
    [Command( "Previous" )]
    internal class PreviousCommand : CommandBase
    {
        public PreviousCommand( MainFormUCP ucp )
            : base( ucp )
        {
            this.ucp.SelectedReportChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get
            {
                return this.ucp.CanGoPrevious;
            }
        }
        public override void Execute()
        {
            this.ucp.PreviousReport();
        }
    }

    [Command("Edit Templates")]
    internal class EditTemplatesCommand : CommandBase
    {
        public EditTemplatesCommand(MainFormUCP ucp) : base(ucp)
        {

        }

        public override bool Enabled
        {
            get { return true; }
        }

        public override void Execute()
        {
            using(TemplateEditor editor = new TemplateEditor(ucp.Callback, ucp.Factory))
            {
                editor.ShowDialog();
                ucp.ResetTemplates();
            }
        }
    }

    [Command( "Show templates" )]
    [KeyBinding(Keys.T)]
    internal class ShowTemplatesCommand : CommandBase
    {
        public ShowTemplatesCommand(MainFormUCP ucp) : base(ucp)
        {

        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void Execute()
        {
            this.ucp.ShowTemplates();
        }
    }
}
