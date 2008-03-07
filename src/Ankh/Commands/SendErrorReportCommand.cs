using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to send the AnkhSVN team comments and suggestions.
    /// </summary>
    [VSNetCommand(AnkhCommand.SendFeedback,
		"SendErrorReport",
         Text = "Send Feedback...", 
         Tooltip = "Send the AnkhSVN team comments and suggestions.",
         Bitmap = ResourceBitmaps.SendSuggest),
         VSNetControl( "Tools.AnkhSVN", Position = 7 )]
    public class SendErrorReportCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            context.ErrorHandler.SendReport();
        }
    }
}
