using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;

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
    [ToolBar("Reply")]
    [MenuItem("MainMenu.Mail.Reply to current")]
    internal class ReplyCommand : CommandBase
    {
        public ReplyCommand( MainFormUCP ucp )
            : base( ucp )
        {
            this.ucp.SelectedReportChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get { return this.ucp.SelectedMailItem != null; }
        }

        public override void Execute()
        {
            this.ucp.InitiateReplyForSelectedReport();
        }
    }

    [KeyBinding(Keys.M)]
    [ToolBar("Mark as read")]
    [MenuItem("MainMenu.Mail.Mark as read")]
    internal class MarkAsReadCommand : CommandBase
    {
        public MarkAsReadCommand( MainFormUCP ucp ) : base(ucp)
        {
            this.ucp.SelectedMailItemChanged += delegate { this.RaiseEnabledChanged(); };
        }

        public override bool Enabled
        {
            get
            {
                return this.ucp.SelectedMailItem != null && !this.ucp.SelectedMailItem.Read;
            }
        }

        public override void Execute()
        {
            this.ucp.MarkSelectedMailItemAsRead();
        }
    }

    [KeyBinding( Keys.S )]
    [ToolBar( "Send" )]
    [MenuItem( "MainMenu.Mail.Send current reply" )]
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
            this.ucp.SendReplyForSelectedItem();
            this.ucp.MarkSelectedMailItemAsRead();
            this.ucp.NextReport();
        }
    }

    [KeyBinding(Keys.N)]
    [ToolBar( "Next" )]
    [MenuItem("MainMenu.Navigate.Next")]
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

    [ToolBar( "Edit signature" )]
    internal class EditSignatureCommand : CommandBase
    {
        public EditSignatureCommand( MainFormUCP ucp )
            : base( ucp )
        {

        }

        public override void Execute()
        {
            using ( SignatureDialog dialog = new SignatureDialog() )
            {
                dialog.Signature = ucp.Signature;

                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    ucp.Signature = dialog.Signature;
                }
            }
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }
    }

    [KeyBinding( Keys.P )]
    [ToolBar( "Previous" )]
    [MenuItem( "MainMenu.Navigate.Previous" )]
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

    [ToolBar("Edit Templates")]
    [MenuItem( "MainMenu.Templates.Edit" )]
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
            using(TemplateEditor editor = new TemplateEditor(ucp.Callback, ucp.ServiceProvider))
            {
                editor.ShowDialog();
                ucp.ResetTemplates();
            }
        }
    }

    [ToolBar( "Show templates" )]
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

    [MenuItem("MainMenu.Import.Import from Outlook")]
    internal class ImportFromOutlookCommand : CommandBase
    {
        public ImportFromOutlookCommand(MainFormUCP ucp) : base(ucp)
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
            IStorage storage = this.ucp.ServiceProvider.GetService<IStorage>();
            ImportDialogUCP ucp = new ImportDialogUCP( this.ucp.Callback, storage , this.ucp.Invoker);
            using ( ImportDialog dialog = new ImportDialog( ucp ) )
            {
                dialog.ShowDialog();
            }
        }
    }

    [MenuItem("MainMenu.View.Toggle flat or threaded")]
    internal class ToggleFlatOrThreadedCommand : CommandBase
    {
        public ToggleFlatOrThreadedCommand( MainFormUCP ucp ) : base(ucp)
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
            this.ucp.ToggleFlatOrThreaded();
        }
    }
}
