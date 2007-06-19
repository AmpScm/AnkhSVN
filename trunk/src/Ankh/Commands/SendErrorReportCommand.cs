using System;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to send the AnkhSVN team comments and suggestions.
    /// </summary>
    [VSNetCommand("SendErrorReport",
         Text = "Send Feedback", 
         Tooltip = "Send the AnkhSVN team comments and suggestions.",
         Bitmap = ResourceBitmaps.SendSuggest),
         VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class SendErrorReportCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.ErrorHandler.SendReport();
        }

        #endregion
    }
}
