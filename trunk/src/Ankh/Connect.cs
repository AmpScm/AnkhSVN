// $Id$
namespace Ankh
{
	using System;
	using Microsoft.Office.Core;
	using Extensibility;
	using System.Runtime.InteropServices;
	using EnvDTE;
    using System.Diagnostics;

    using Ankh.Commands;

	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the MyAddin21Setup project 
	// by right clicking the project in the Solution Explorer, then choosing install.
	#endregion
	
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("DA979B20-78DF-45BC-A7D7-F5EF9DC77D30"), ProgId("Ankh")]
	public class Connect : Object, Extensibility.IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
            try
            {
                this.context = new AnkhContext( (_DTE)application, (AddIn)addInInst );

                // get rid of the old ones
                Ankh.CommandMap.DeleteCommands( this.context );

                // register the new ones
                this.commands= 
                    Ankh.CommandMap.RegisterCommands( this.context );    
            }
            catch( Exception ex )
            {
                HandleError( ex );
                throw;
            }
		}

		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
            Ankh.CommandMap.DeleteCommands( this.context );
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
		}
		
		/// <summary>
		///      Implements the QueryStatus method of the IDTCommandTarget interface.
		///      This is called when the command's availability is updated
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to determine state for.
		/// </param>
		/// <param term='neededText'>
		///		Text that is needed for the command.
		/// </param>
		/// <param term='status'>
		///		The state of the command in the user interface.
		/// </param>
		/// <param term='commandText'>
		///		Text requested by the neededText parameter.
		/// </param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText)
		{
            this.timer.Start();
            try
            {
                

                if( this.commands != null && 
                    neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
                {
                    Ankh.ICommand cmd;
                    if ( (cmd = (ICommand)this.commands[commandName]) != null )
                        status = cmd.QueryStatus( this.context );

                    //    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    //				if(commandName == "Ankh.Connect.Ankh")
                    //				{
                    //					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
                    //				}
                }
            }
            catch( Exception ex )
            {   
                HandleError( ex );
                throw;
            }
            this.timer.End();

            Trace.WriteLine( commandName + ": " + this.timer.Interval, "Ankh" );
		}

		/// <summary>
		///      Implements the Exec method of the IDTCommandTarget interface.
		///      This is called when the command is invoked.
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to execute.
		/// </param>
		/// <param term='executeOption'>
		///		Describes how the command should be run.
		/// </param>
		/// <param term='varIn'>
		///		Parameters passed from the caller to the command handler.
		/// </param>
		/// <param term='varOut'>
		///		Parameters passed from the command handler to the caller.
		/// </param>
		/// <param term='handled'>
		///		Informs the caller if the command was handled or not.
		/// </param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
            try
            {
                if(executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    ICommand cmd;
                    if ( (cmd = (ICommand)this.commands[commandName]) != null )
                    { 
                        cmd.Execute( this.context );
                        handled = true;
                    }
                }
            }
            catch( Exception ex )
            {   
                HandleError( ex );
                throw;
            }
		}

        public static void HandleError( Exception ex )
        {
#if REPORTERROR
            Utils.ErrorMessage.QuerySendByWeb( "http://arild.no-ip.com/error/report.aspx", ex );
#else
            System.Windows.Forms.MessageBox.Show( ex.ToString() );
#endif
            //
        }        
        

        private string GenerateNestedExceptionMessage( Exception ex )
        {
            string msg = ex.Message;
            Exception innerEx = ex.InnerException;
            while ( innerEx != null )
            {
                msg += innerEx.Message;
                innerEx = innerEx.InnerException;
            }
            return msg;
        }
        private Utils.Timer timer = new Utils.Timer();
        private AnkhContext context;
        Ankh.CommandMap commands;
		
	}
}


