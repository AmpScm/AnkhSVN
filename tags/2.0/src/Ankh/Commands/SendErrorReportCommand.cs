using System;
using Ankh.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to send the AnkhSVN team comments and suggestions.
    /// </summary>
    [Command(AnkhCommand.SendFeedback)]
    public class SendErrorReportCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhErrorHandler handler = e.Context.GetService<IAnkhErrorHandler>();

            if (handler != null)
            {
                handler.SendReport();
            }
        }
    }
}
