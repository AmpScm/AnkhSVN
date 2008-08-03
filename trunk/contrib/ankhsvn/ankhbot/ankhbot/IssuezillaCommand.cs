using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    /// <summary>
    /// A command to interrogate an IZ database.
    /// </summary>
    [Command("issue")]
    public class IssuezillaCommand : CommandBase
    {
        [SubCommand("timeout")]
        private void Timeout( CommandArgs args )
        {
            IssuezillaService service = args.Bot.ServiceProvider.GetService<IssuezillaService>();
            int timeout = Int32.Parse( args.Args[1] );
            service.SetTimeout( timeout );

            args.SendMessage( "Ack" );
        }

        protected override void NoSubCommand( CommandArgs args )
        {
            IssuezillaService service = args.Bot.ServiceProvider.GetService<IssuezillaService>();

            try
            {
                int issueNumber = Int32.Parse( args.Args[0] );
                Issue issue = service.GetIssue( issueNumber );
                string url = service.GetUrl( issueNumber );

                string msg = String.Format( "Issue #{0}: {1}\nStatus: {2}\nResolution: {3}\n({4})",
                    issue.Id, issue.ShortDescription, issue.Status, issue.Resolution,
                    url );

                args.SendMessage( msg );
            }
            catch ( FormatException )
            {
                args.SendMessage( "Issue ID must be integer" );
            }
            catch ( IssuezillaException ex )
            {
                args.SendMessage( ex.Message );
            }
        }

    }
}
