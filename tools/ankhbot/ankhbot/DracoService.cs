using System;
using System.Collections.Generic;
using System.Text;
using Draco.Core.Remote;
using System.Runtime.Remoting;

namespace AnkhBot
{
	class DracoService : RemotelyDelegatableObject, IService
	{
		public void Initialize( AnkhBot bot )
		{
			this.bot = bot;

			RemotingConfiguration.Configure( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile );

			this.monitor = new RemoteEventMonitor();
			BuildStatusChangedEventHandler changed = delegate( BuildEventArgs args )
			{
				if (args.Status != Draco.Core.Remote.BuildStatus.Queued)
				{
					this.bot.Broadcast( String.Format( "New status for {0}: {1}.",
						args.BuildName, args.Status ) );
				}
			};

			this.monitor.BuildStarted += delegate( string moduleName )
			{
				this.bot.Broadcast( "Starting build of " + moduleName );
				this.monitor.BuildStatusChanged += changed;
			};

			this.monitor.BuildCompleted += delegate( BuildEventArgs args )
			{
				this.bot.Broadcast( string.Format( "Build of {0} completed with status {1}",
					args.BuildName, args.Status ) );
				this.monitor.BuildStatusChanged -= changed;
			};

			this.monitor.ServerStatusChanged += delegate( ServerStatusChangedEventArgs args )
			{
				this.bot.Broadcast( String.Format( "Server status: {0}", args.Status ) );
			};

			this.draco = (IDracoRemote)
				Activator.GetObject( typeof( IDracoRemote ), "tcp://10.0.0.3:8086/Draco" );
			this.monitor.Remote = this.draco;
						
			//this.draco.BuildStarted += new BuildStartedEventHandler( this.BuildStartedCallback );
			//this.draco.BuildCompleted += new BuildCompletedEventHandler( this.BuildCompletedCallback );
			//this.draco.BuildStatusChanged += new BuildStatusChangedEventHandler( this.BuildStatusChangedCallback );;
			//this.draco.ServerStatusChanged += new ServerStatusChangedEventHandler(this.ServerStatusChangedCallback);


			//this.draco.BuildCompleted += delegate( BuildEventArgs e )
			//{
			//    this.bot.Broadcast( e.BuildName + " completed." );
			//};

			//this.draco.BuildStarted += delegate( string moduleName )
			//{
			//    this.bot.Broadcast( "Building " + moduleName + "." );
			//};

			//this.draco.BuildStatusChanged += delegate( BuildEventArgs args )
			//{
			//    this.bot.Broadcast( String.Format( "New status for {0}: {1}.",
			//        args.BuildName, args.Status ) );
			//};

			//this.draco.DracoStarted += delegate
			//{
			//    this.bot.Broadcast( "Draco started." );
			//};

			//this.draco.DracoStopped += delegate
			//{
			//    this.bot.Broadcast( "Draco stopped." );
			//};
		}

		//protected override void  InternalBuildStartedCallback(string moduleName)
		//{
		//    this.bot.Broadcast( "Build of " + moduleName + " started." );
		//}

		//protected override void InternalBuildCompletedCallback( BuildEventArgs args )
		//{
		//    this.bot.Broadcast( string.Format( "Built {0}: {1}", args.BuildName, args.Status ) );
		//}

		//protected override void InternalBuildStatusChangedCallback( BuildEventArgs args )
		//{
		//    this.bot.Broadcast( string.Format( "Status changed for {0}: {1}", 
		//        args.BuildName, args.Status ) );
		//}

		//protected override void  InternalServerStatusChangedCallback(ServerStatusChangedEventArgs args)
		//{
		//    this.bot.Broadcast( string.Format( "Server status changed: {0}",
		//        args.Status ) );
		//}


		public void Build( string module )
		{
			this.draco.StartBuild( module, true );
		}

		public void StopBuild( string module )
		{
			this.draco.StopBuild( module );
		}

		public string BuildStatus( string module )
		{
			return this.draco.GetBuildStatus( module ).ToString();
		}

		public string ServerStatus()
		{
			return this.draco.GetServerStatus().ToString();
		}
	

		public string[] GetBuilds()
		{
			return this.draco.GetAvailableBuilds();
		}

		/// <summary>
		/// The object that receives the events
		/// </summary>
		private class RemoteEventMonitor : RemotelyDelegatableObject
		{
			protected override void InternalBuildStatusChangedCallback( BuildEventArgs e )
			{
				// Rebroadcast events
				BuildStatusChanged( e );
			}

			protected override void InternalServerStatusChangedCallback( ServerStatusChangedEventArgs e )
			{
				// Rebroadcast events
				ServerStatusChanged( e );
			}

			protected override void InternalBuildStartedCallback( string moduleName )
			{
				BuildStarted( moduleName );
			}

			protected override void InternalBuildCompletedCallback( BuildEventArgs args )
			{
				BuildCompleted( args );
			}

			public IDracoRemote Remote
			{
				get { return _remote; }

				set
				{
					// Are we already null?
					if (value == null && _remote == null)
						return;

					// Try to unregister the previous remote's event (just in case)
					try
					{
						if (_remote != null)
						{
							_remote.BuildStatusChanged -= new BuildStatusChangedEventHandler( BuildStatusChangedCallback );
							_remote.ServerStatusChanged -= new ServerStatusChangedEventHandler( ServerStatusChangedCallback );
						}
					}
					catch (Exception)
					{
						// Ignore errors here
					}

					_remote = value;

					try
					{
						_remote.BuildStatusChanged += new BuildStatusChangedEventHandler( BuildStatusChangedCallback );
						_remote.ServerStatusChanged += new ServerStatusChangedEventHandler( ServerStatusChangedCallback );
					}
					catch (Exception)
					{
						_remote = null;
						throw;
					}
				}
			}

			public BuildStatusChangedEventHandler BuildStatusChanged;
			public ServerStatusChangedEventHandler ServerStatusChanged;
			public BuildStartedEventHandler BuildStarted;
			public BuildCompletedEventHandler BuildCompleted;
			private IDracoRemote _remote;
		}

		private AnkhBot bot;
		private IDracoRemote draco;
		private RemoteEventMonitor monitor;


	}
}
