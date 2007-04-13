using System;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets the user send a generic error report.
    /// </summary>
    [VSNetCommand("SendErrorReport", Text = "Send suggestion/error re&port", 
         Tooltip = "Show the repository explorer window",
         Bitmap = ResourceBitmaps.SendSuggest),
    VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class SendErrorReportCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // always enabled.
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.ErrorHandler.SendReport();
        }


    }
}
