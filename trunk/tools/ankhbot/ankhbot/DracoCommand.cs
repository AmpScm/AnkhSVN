using System;
using System.Collections.Generic;
using System.Text;


namespace AnkhBot
{
	[Command("draco")]
	class DracoCommand : CommandBase
	{			

		[SubCommand( "getbuilds" )]
		private void GetBuilds( CommandArgs args )
		{
			string[] builds = args.Bot.ServiceProvider.GetService<DracoService>().GetBuilds();
			string msg = "Available builds: " + string.Join( ", ", builds );

			args.SendMessage( msg );
		}

		[SubCommand( "build" )]
		private void Build( CommandArgs args )
		{
			if (args.Args.Length > 1)
			{
				string module = args.Args[1];
				args.Bot.ServiceProvider.GetService<DracoService>().Build( module );
				args.SendMessage( "Starting build of " + module );
			}
		}

		[SubCommand( "stop" )]
		private void Stop( CommandArgs args )
		{
			if (args.Args.Length > 1)
			{
				string module = args.Args[1];
				args.Bot.ServiceProvider.GetService<DracoService>().StopBuild( module );
				args.SendMessage( "Attempting to stop build of " + module );
			}
		}

		[SubCommand( "status" )]
		private void BuildStatus( CommandArgs args )
		{
			if (args.Args.Length > 1)
			{
				string module = args.Args[1];
				string status = 
					args.Bot.ServiceProvider.GetService<DracoService>().GetBuildStatus( module );
				args.SendMessage( string.Format( "Status of {0}: {1}", module, status ) );
			}
		}

		[SubCommand( "serverstatus" )]
		private void ServerStatus( CommandArgs args )
		{			
			string status =
				args.Bot.ServiceProvider.GetService<DracoService>().ServerStatus();
			args.SendMessage( "Server status: " + status );
		}

	}
}
