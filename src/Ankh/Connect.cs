// $Id$
namespace Ankh
{
    using System;
    using Microsoft.Office.Core;
    using Microsoft.Win32;
    using Extensibility;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    
    using System.IO;

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
            //            File.Delete( "N:\\ankhlog.txt" );
            //            Debug.Listeners.Add( new TextWriterTraceListener( "N:\\ankhlog.txt" ) );
            //            Debug.AutoFlush = true;
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
            // we don't want to load on command line builds.
            if ( Regex.IsMatch( Environment.CommandLine, "/build" ) )
                return;
#if LOGTOFILE
            string logfile = Path.Combine( Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData ), 
                Path.Combine("AnkhSVN", "ankhsvn.log" ) );
            Debug.Listeners.Add( new TextWriterTraceListener( logfile ) );
            Debug.AutoFlush = true;
#endif
            try
            {
                
                this.context = new AnkhContext( (_DTE)application, (AddIn)addInInst );
                

                Extenders.ExtenderProvider.Register( this.context );

#if ALWAYSREGISTER
                bool register = true;
#else
                bool register = connectMode == ext_ConnectMode.ext_cm_UISetup;
#endif
                // get rid of the old ones
                if( register )
                    Ankh.CommandMap.DeleteCommands( this.context );

                // register the new ones
                this.commands= 
                    Ankh.CommandMap.LoadCommands( this.context, register );  

                this.CreateAboutBoxText( ((_DTE)application).RegistryRoot );
               
                // is there already a solution open? 
                // can happen if we are loaded after startup
                if ( this.context.DTE.Solution.IsOpen )
                    this.context.SolutionOpened();
  
                
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
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
            try
            {
                if( this.context != null )
                {
                    Extenders.ExtenderProvider.Unregister( this.context.DTE );				
                    this.context.Shutdown();
                }
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
            }
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
            this.context.DTE.ExecuteCommand( "Tools.Alias", "svn Ankh.RunSvn" );
            this.context.DTE.ExecuteCommand( "Tools.Alias", "cd Ankh.RunSvn cd" );
            this.context.DTE.ExecuteCommand( "Tools.Alias", "pwd Ankh.RunSvn pwd" );
            this.context.DTE.ExecuteCommand( "Tools.Alias", "dir Ankh.RunSvn dir" );
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
            Utils.DebugTimer t = Utils.DebugTimer.Start();
            try
            {
                if( this.commands != null && 
                    neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
                {
                    // this the same command as the last QueryStatus call?
                    if ( commandName == this.lastQueriedCommand && 
                        (DateTime.Now-this.lastQuery).Milliseconds < CACHESTATUS_INTERVAL )
                    {
                        // don't bother looking it up - just use the same
                        // as last time
                        status = this.cachedStatus;                        
                    }
                    else
                    {
                        Ankh.ICommand cmd;
                        if ( (cmd = (ICommand)this.commands[commandName]) != null )
                            status = cmd.QueryStatus( this.context );
                    }
                    // store these
                    this.lastQuery = DateTime.Now;
                    this.lastQueriedCommand = commandName;
                    this.cachedStatus = status;
                }
            }
//            catch( StatusException )
//            {
//                // couldn't get status for an item on disk - maybe its been renamed etc from
//                // outside VS
//                this.context.SolutionExplorer.RefreshSelection();
//                status = vsCommandStatus.vsCommandStatusSupported;
//            }
            catch( Exception ex )
            {   
                Error.Handle( ex );
            }
            t.End( "Query status for " + commandName + ": " + status, "Ankh" );
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
                if( (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault) ||
                    ((executeOption & vsCommandExecOption.vsCommandExecOptionShowHelp) != 0) )
                {
                    string args = (string)varIn;

                    // does the user need help?
                    if ((executeOption & vsCommandExecOption.vsCommandExecOptionShowHelp) != 0)
                        args = "/?";

                    ICommand cmd;
                    if ( (cmd = (ICommand)this.commands[commandName]) != null )
                    { 
                        cmd.Execute( this.context, args );
                        handled = true;
                    }
                }
            }
//            catch( StatusException )
//            {
//                // couldn't get status for an item on disk - maybe its been renamed etc from
//                // outside VS
//                this.context.SolutionExplorer.RefreshSelection();
//            }
            catch( Exception ex )
            {   
                Error.Handle( ex );
            }
        }

        /// <summary>
        /// Create the text that goes in the VS.NET about box, with the 
        /// version numbers of Ankh and the linked libraries.
        /// </summary>
        private void CreateAboutBoxText( string registryRoot )
        {
            string text = "";			

            // get the assembly version
            string ankhVersion = 
                typeof(NSvn.Core.Client).Assembly.GetName().Version.ToString();
			
            text += String.Format( "AnkhSVN {0}{1}", 
                ankhVersion, Environment.NewLine );

            // get the library versions
            object[] attributes = typeof(NSvn.Core.Client).Assembly.GetCustomAttributes(
                typeof(NSvn.Common.LibraryAttribute), true );
            foreach( NSvn.Common.LibraryAttribute version in attributes )
                text += version.ToString() + Environment.NewLine;

            // set the registry value
            RegistryKey key = Registry.CurrentUser.CreateSubKey( registryRoot + @"\AddIns\Ankh" );
            key.SetValue( "AboutBoxDetails", text );
        }
        
        private AnkhContext context;
        Ankh.CommandMap commands;


        private EnvDTE.vsCommandStatus cachedStatus;
        private string lastQueriedCommand = "";
        private DateTime lastQuery = DateTime.Now;

        private const int CACHESTATUS_INTERVAL = 800;
		
    }
}


